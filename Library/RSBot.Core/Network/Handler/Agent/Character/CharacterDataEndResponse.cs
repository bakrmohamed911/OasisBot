using System;
using System.IO;
using RSBot.Core.Components;
using RSBot.Core.Event;
using RSBot.Core.Extensions;
using RSBot.Core.Objects;
using RSBot.Core.Objects.Quests;

namespace RSBot.Core.Network.Handler.Agent.Character;

internal class CharacterDataEndResponse : IPacketHandler
{
    /// <summary>
    ///     Gets or sets the opcode.
    /// </summary>
    /// <value>
    ///     The opcode.
    /// </value>
    public ushort Opcode => 0x34A6;

    /// <summary>
    ///     Gets or sets the destination.
    /// </summary>
    /// <value>
    ///     The destination.
    /// </value>
    public PacketDestination Destination => PacketDestination.Client;

    /// <summary>
    ///     Handles the packet.
    /// </summary>
    /// <param name="packet">The packet.</param>
    public void Invoke(Packet packet)
    {
        SpawnManager.Clear();

        packet = Game.ChunkedPacket;
        packet.Lock();

        Log.Debug($"[CharData] Packet total length: {packet.Length}");

        if (Game.ClientType >= GameClientType.Thailand)
        {
            var ts = packet.ReadUInt(); // serverTimestamp
            Log.Debug($"[CharData] pos={packet.Length - packet.Remaining} serverTimestamp={ts}");
        }

        var modelId = packet.ReadUInt();
        Log.Debug($"[CharData] pos={packet.Length - packet.Remaining} modelId={modelId}");

        var character = new Player(modelId);
        character.Scale = packet.ReadByte();
        character.Level = packet.ReadByte();
        character.MaxLevel = packet.ReadByte();
        character.Experience = packet.ReadLong();
        character.SkillExperience = packet.ReadUInt();
        character.Gold = packet.ReadULong();
        character.SkillPoints = packet.ReadUInt();
        character.StatPoints = packet.ReadUShort();
        character.BerzerkPoints = packet.ReadByte();
        character.ExperienceChunk = packet.ReadUInt();
        character.Health = packet.ReadInt();
        character.Mana = packet.ReadInt();
        character.AutoInverstExperience = (AutoInverstType)packet.ReadByte();
        Log.Debug($"[CharData] pos={packet.Length - packet.Remaining} after basic stats: lvl={character.Level} hp={character.Health} mp={character.Mana} gold={character.Gold}");

        if (Game.ClientType == GameClientType.Chinese_Old)
            character.DailyPK = (byte)packet.ReadUShort();
        else
            character.DailyPK = packet.ReadByte();

        character.TotalPK = packet.ReadUShort();
        character.PKPenaltyPoint = packet.ReadUInt();

        if (Game.ClientType >= GameClientType.Thailand)
            character.BerzerkLevel = packet.ReadByte();

        if (Game.ClientType > GameClientType.Thailand)
            /*character.PvpFlag = (PvpFlag)*/packet.ReadByte();

        Log.Debug($"[CharData] pos={packet.Length - packet.Remaining} after pk/berzerk");

        if (Game.ClientType >= GameClientType.Chinese || Game.ClientType == GameClientType.Vietnam274)
        {
            if (Game.ClientType != GameClientType.Chinese)
                packet.ReadByte();

            packet.ReadUInt(); //You can use VIP service until this time
            packet.ReadByte();

            if (
                Game.ClientType == GameClientType.Turkey
                || Game.ClientType == GameClientType.VTC_Game
                || Game.ClientType == GameClientType.RuSro
                || Game.ClientType == GameClientType.Taiwan
            )
                packet.ReadUInt();

            if (Game.ClientType == GameClientType.Rigid)
                packet.ReadBytes(12);

            if (Game.ClientType == GameClientType.VTC_Game)
                packet.ReadByte(); // ??

            if (Game.ClientType == GameClientType.Taiwan)
                packet.ReadBytes(5);

            Log.Debug($"[CharData] pos={packet.Length - packet.Remaining} before serverCap, remaining={packet.Remaining}");

            var serverCap = packet.ReadByte();
            Log.Notify($"The game server cap is {serverCap}!");

            // vSRO 274 does not include this trailing unknown ushort either (same family
            // of gaps as the JOB2 and collection-book exclusions below) - a hex dump of a
            // failing CharacterDataEndResponse packet showed the very next byte, read here
            // as this ushort's low byte, is actually Inventory's Capacity: with this read
            // in place it decoded as 0 (InventoryItemCollection.Deserialize returns
            // immediately on Capacity<=0, explaining the suspiciously 1-byte-only
            // "before/after Inventory" gap), while the byte 2 positions earlier - where
            // Capacity lands once this read is skipped - decodes to a plausible 47.
            if (
                Game.ClientType != GameClientType.Korean
                && Game.ClientType != GameClientType.Chinese
                && Game.ClientType != GameClientType.Japanese
                && Game.ClientType != GameClientType.Vietnam274
            )
                packet.ReadUShort();

            Log.Debug($"[CharData] pos={packet.Length - packet.Remaining} after client-specific block, remaining={packet.Remaining}");
        }

        // Wraps everything from here through the final reads: a Vietnam274 misalignment
        // anywhere in this chain (Inventory/Avatars/Skills/QuestLog/collection-book/
        // ParseBionicDetails/Name/JobInfo/transport-CTF fields) can leave a later section
        // parsing without throwing right away (e.g. Skills' 0x01-terminated loops just exit
        // immediately on a wrong byte instead of failing) - the drifted cursor only actually
        // runs off the end of the packet somewhere further along. The hex dump is what's
        // needed to pin down which section is actually off, the same way it did for the
        // Vietnam274 State.Deserialize bug (see State.cs) and the Inventory Capacity bug
        // (see the Vietnam274 exclusion added above, near serverCap).
        try
        {
            Log.Debug($"[CharData] pos={packet.Length - packet.Remaining} before Inventory, remaining={packet.Remaining}");

            // On this Vietnam274 server, Inventory.Deserialize's own item-position assumption
            // (items start right after Capacity+Count) doesn't hold - Capacity/Count themselves
            // read fine, but the bytes immediately following aren't the real item list (they're
            // some other, still-unidentified structure). The item wire format itself is
            // confirmed correct, though (verified byte-for-byte: OptLevel/Attributes/Durability/
            // MagicOptionsAmount/bindings all decode to sane values, e.g. Durability=73, for a
            // named item the player confirmed owning) - only its *offset* is unknown, the same
            // shape of problem the mastery-list scan above already solved. Recover the same way:
            // scan forward for a position where the real, unmodified InventoryItem.FromPacket
            // successfully parses two items in a row, then read from there instead of guessing.
            if (Game.ClientType == GameClientType.Vietnam274)
            {
                // The single wire Capacity byte (confirmed correctly read: e.g. 48) undercounts
                // this server's real storage - its inventory UI has 3 pages of 4x8 (32) slots,
                // i.e. 96 total, and real items (confirmed by name) were found sitting in slots
                // up to at least 49, past that byte's value. Rather than guess why the wire byte
                // reads low, just use the UI's own stated ceiling so real items past slot 48
                // stop getting rejected by the slot-range check in RecoverItems.
                const int effectiveCapacity = 96;
                character.Inventory = new CharacterInventory((byte)effectiveCapacity);
                RecoverItems(packet, character.Inventory, effectiveCapacity, "Inventory");
            }
            else
            {
                character.Inventory = new CharacterInventory(packet);
            }

            Log.Debug(
                $"[CharData] pos={packet.Length - packet.Remaining} after Inventory (Capacity={character.Inventory.Capacity}, Count={character.Inventory.Count}), remaining={packet.Remaining}"
            );

            if (Game.ClientType == GameClientType.Vietnam274)
            {
                // Same root cause and same fix as Inventory above - Avatars (costume/fashion
                // slots) reads its own Capacity/Count fine but then drifts into the same
                // unidentified structure instead of the real item list. Some "Seal"-type items
                // the player confirmed owning turned out to live here rather than in the normal
                // backpack. Avatar counts are normally small (a handful of slots); 50 is a
                // generous ceiling with margin, not a measured value.
                const int avatarEffectiveCapacity = 50;
                character.Avatars = new InventoryItemCollection((byte)avatarEffectiveCapacity);
                RecoverItems(packet, character.Avatars, avatarEffectiveCapacity, "Avatars");
            }
            else if (Game.ClientType >= GameClientType.Thailand)
            {
                character.Avatars = new InventoryItemCollection(packet);
            }
            else
            {
                character.Avatars = new InventoryItemCollection(5);
            }

            Log.Debug(
                $"[CharData] pos={packet.Length - packet.Remaining} after Avatars (Capacity={character.Avatars.Capacity}, Count={character.Avatars.Count}), remaining={packet.Remaining}"
            );

            // JOB2
            // vSRO 274 does not include the JOB2 section either (same family of gaps as the
            // red-arrow-effect flag and collection-book section below).
            if (Game.ClientType > GameClientType.Vietnam && Game.ClientType != GameClientType.Vietnam274)
            {
                character.Job2SpecialtyBag = new InventoryItemCollection(packet);

                character.Job2 = new InventoryItemCollection(packet);
            }
            Log.Debug($"[CharData] pos={packet.Length - packet.Remaining} after JOB2, remaining={packet.Remaining}");

            // vSRO274 (at least on this server) has a per-item option-block layout that
            // InventoryItem.FromPacket doesn't understand: Capacity/Count read fine (verified
            // against a raw hex capture - e.g. Capacity=48, Count=3), but RentInfo/ItemId then
            // drift into garbage for each item, so the cursor is NOT reliably positioned here.
            // Rather than also reverse-engineer that item format, recover alignment by content:
            // the Masteries+Skills list itself already uses the exact same wire format every
            // other client type uses (confirmed byte-for-byte against a real character's
            // Sword/Cold/Lightning/Fire mastery levels captured from another tool's UI) - only
            // its *offset* is unknown here. Scan forward for its self-describing signature (a
            // run of 6-byte [0x01][id-lo][id-hi][0x00][0x00][level] records whose id resolves
            // in the real SkillMasteryData table) and seek there before calling the existing,
            // unmodified Skills.FromPacket.
            if (Game.ClientType == GameClientType.Vietnam274)
            {
                var searchStart = packet.Length - packet.Remaining;
                var masteryListOffset = FindMasteryListOffset(packet.GetBytes(), searchStart);
                if (masteryListOffset > 0)
                    packet.SeekRead(masteryListOffset - 1, SeekOrigin.Begin); // -1: Skills.FromPacket reads one "unknown" byte before its first flag check
                else
                    Log.Debug("[CharData] Vietnam274 mastery-list signature not found - Skills will likely come back empty.");
            }

            character.Skills = Skills.FromPacket(packet);
            Log.Debug(
                $"[CharData] pos={packet.Length - packet.Remaining} after Skills (Masteries={character.Skills.Masteries.Count}, KnownSkills={character.Skills.KnownSkills.Count}), remaining={packet.Remaining}"
            );

            character.QuestLog = QuestLog.FromPacket(packet);
            Log.Debug(
                $"[CharData] pos={packet.Length - packet.Remaining} after QuestLog (Active={character.QuestLog.ActiveQuests.Count}, Completed={character.QuestLog.CompletedQuests.Length}), remaining={packet.Remaining}"
            );

            packet.ReadByte(); // Unknown

            // vSRO 274 does not include the collection-book section here.
            if (Game.ClientType > GameClientType.Thailand && Game.ClientType != GameClientType.Vietnam274)
            {
                var collectionBookStartedThemeCount = packet.ReadUInt();
                for (var i = 0; i < collectionBookStartedThemeCount; i++)
                {
                    packet.ReadUInt(); //index
                    packet.ReadUInt(); //Starttime
                    packet.ReadUInt(); //pages
                }
            }
            Log.Debug($"[CharData] pos={packet.Length - packet.Remaining} after collection book, remaining={packet.Remaining}");

            // Now that Skills/QuestLog land at the right offset (see the mastery-list scan
            // above), this position is confirmed correct too - decoded against known-good
            // markers in a captured packet (UniqueId, then a Position/Movement, then a
            // WalkSpeed/RunSpeed/BerzerkSpeed=19.2/60/100 triplet with sane LifeState/
            // Motion/BodyState, ending exactly at the real Name field). The one gap: an
            // extra 4-byte field sits before UniqueId here that ParseBionicDetails doesn't
            // read - reading UniqueId straight off the wire without it produces 0, which is
            // exactly the corrupted-UniqueId symptom behind the "HP/MP/EXP live updates"
            // bug (EntityUpdateStatusResponse's "uniqueId == Game.Player.UniqueId" check
            // never matches real update packets when Game.Player.UniqueId is 0). This 4-byte
            // field isn't part of the shared ParseBionicDetails/Movement/State helpers used
            // by entity-spawn parsing too (already confirmed working there without it) - so
            // it's consumed here, specific to this CHAR_DATA call site, rather than inside
            // ParseBionicDetails itself.
            if (Game.ClientType == GameClientType.Vietnam274)
                packet.ReadUInt(); // unknown - precedes UniqueId only in CHAR_DATA's own bionic block

            character.ParseBionicDetails(packet);
            Log.Debug($"[CharData] pos={packet.Length - packet.Remaining} after bionic, remaining={packet.Remaining}");

            character.Name = packet.ReadString();
            Log.Debug($"[CharData] pos={packet.Length - packet.Remaining} after Name='{character.Name}', remaining={packet.Remaining}");

            character.JobInformation = JobInfo.FromPacket(packet);
            Log.Debug($"[CharData] pos={packet.Length - packet.Remaining} after JobInfo, remaining={packet.Remaining}");

            character.State.PvpState = (PvpState)packet.ReadByte();
            character.OnTransport = packet.ReadBool(); //On transport?
            character.InCombat = packet.ReadBool();
            Log.Debug($"[CharData] pos={packet.Length - packet.Remaining} after PvpState/OnTransport/InCombat, remaining={packet.Remaining}");

            // Kept consistent with the Vietnam274 carve-in above (line ~81): if that client
            // shares the Chinese+ VIP/serverCap block, it should share these Chinese+ reads
            // too, otherwise every field from here on is misaligned for Vietnam274.
            if (Game.ClientType >= GameClientType.Chinese || Game.ClientType == GameClientType.Vietnam274)
                packet.ReadByte();

            if (character.OnTransport)
                character.TransportUniqueId = packet.ReadUInt();

            if (Game.ClientType >= GameClientType.Chinese || Game.ClientType == GameClientType.Vietnam274)
                packet.ReadUInt(); //unkUint2 i think it is using for balloon event or buff for events

            if (Game.ClientType > GameClientType.Vietnam)
                packet.ReadByte();

            packet.ReadByte(); //PVP dress for the CTF event //0 = Red Side, 1 = Blue Side, 0xFF = None
            Log.Debug($"[CharData] pos={packet.Length - packet.Remaining} after transport/CTF, remaining={packet.Remaining}");

            if (
                Game.ClientType > GameClientType.Chinese
                && Game.ClientType != GameClientType.Global
                && Game.ClientType != GameClientType.Rigid
                && Game.ClientType != GameClientType.RuSro
                && Game.ClientType != GameClientType.Korean
                && Game.ClientType != GameClientType.VTC_Game
                && Game.ClientType != GameClientType.Japanese
            )
            {
                packet.ReadByte(); // 0xFF
                packet.ReadUShort(); // 0xFF
                packet.ReadUShort(); // 0xFF
            }

            //GuideFlag
            if (Game.ClientType >= GameClientType.Thailand)
                packet.ReadULong();
            else
                packet.ReadUInt();

            if (
                Game.ClientType == GameClientType.Chinese_Old
                || Game.ClientType == GameClientType.Chinese
                || Game.ClientType == GameClientType.Global
                || Game.ClientType == GameClientType.RuSro
                || Game.ClientType == GameClientType.Korean
                || Game.ClientType == GameClientType.VTC_Game
                || Game.ClientType == GameClientType.Japanese
            )
                packet.ReadByte();

            if (Game.ClientType == GameClientType.Chinese)
                packet.ReadByte();

            character.JID = packet.ReadUInt();
            character.IsGameMaster = packet.ReadBool();
            Log.Debug($"[CharData] pos={packet.Length - packet.Remaining} FINAL remaining={packet.Remaining}, name={character.Name}");

            // A misalignment doesn't always throw - if the drifted cursor happens to land
            // on byte(s) that still form a "valid" read (e.g. a string length prefix that
            // decodes to 0, or a field within the buffer that just holds a small number),
            // this reaches here having silently parsed garbage instead of throwing: an
            // empty Name and/or a large chunk of the packet left completely unconsumed are
            // the tell. Dump in that case too, the same way the throwing case already does.
            if (string.IsNullOrEmpty(character.Name) || packet.Remaining > 8)
            {
                var suspiciousDumpPath = Path.Combine(Kernel.BasePath, "User", "Logs", "CharDataSuspiciousDump.txt");
                File.WriteAllText(suspiciousDumpPath, packet.GetBytes().HexDump(0, packet.Length));
                Log.Debug(
                    $"[CharData] Finished without throwing but looks wrong (name='{character.Name}', remaining={packet.Remaining}) - raw packet bytes written to {suspiciousDumpPath}"
                );
            }
        }
        catch (EndOfStreamException ex)
        {
            Log.Error($"[CharData] CRASH parsing bionic/name/job/etc: remaining={packet.Remaining}, exception={ex.Message}");
            Log.Debug($"[CharData] stack: {ex.StackTrace}");

            // Log.Debug's underlying TextBoxBaseExtensions.Write truncates any single line
            // past 4000 chars (a defensive cap against a native RichEdit crash - see its own
            // comment), which silently cut off this packet's hex dump well before its actual
            // end when it was logged that way. Writing straight to its own file has no such
            // cap and doesn't touch the log RichTextBox at all, so it's still safe to do
            // unconditionally here (this only runs on the rare parse-failure path, not
            // per-entity/per-tick).
            var dumpPath = Path.Combine(Kernel.BasePath, "User", "Logs", "CharDataCrashDump.txt");
            File.WriteAllText(dumpPath, packet.GetBytes().HexDump(0, packet.Length));
            Log.Debug($"[CharData] Raw packet bytes written to {dumpPath}");

            throw;
        }

        // Load Notification sound settings
        character.NotificationSounds.LoadPlayerSettings();

        //Set instance..
        Game.Player = character;
        Game.ChunkedPacket = null;

        EventManager.FireEvent("OnLoadCharacter");

        ClientManager.SetTitle($"{character.Name} - OasisBot");

        if (!Game.Clientless)
            return;

        PacketManager.SendPacket(new Packet(0x3012), PacketDestination.Server);
        Game.Ready = true;
    }

    /// <summary>
    ///     Finds the absolute offset of the first Mastery record's flag byte in the raw packet,
    ///     by scanning for a run of well-formed, self-consistent records rather than trusting
    ///     the reader's current (possibly drifted) position. See the call site's comment for
    ///     why this is needed on Vietnam274.
    /// </summary>
    /// <param name="data">The full raw packet bytes.</param>
    /// <param name="searchStart">Where to start scanning (never before the already-consumed header).</param>
    /// <returns>The absolute offset of the first record's flag byte, or -1 if no confident match was found.</returns>
    private static int FindMasteryListOffset(byte[] data, int searchStart)
    {
        const int recordSize = 6;
        const int requiredConsecutiveRecords = 3;

        for (var i = searchStart; i + recordSize * requiredConsecutiveRecords <= data.Length; i++)
        {
            var allValid = true;

            for (var r = 0; r < requiredConsecutiveRecords; r++)
            {
                var pos = i + r * recordSize;

                if (data[pos] != 0x01 || data[pos + 3] != 0x00 || data[pos + 4] != 0x00)
                {
                    allValid = false;
                    break;
                }

                var id = (uint)(data[pos + 1] + data[pos + 2] * 256);
                if (Game.ReferenceManager.GetRefSkillMastery(id) == null)
                {
                    allValid = false;
                    break;
                }
            }

            if (allValid)
                return i;
        }

        return -1;
    }

    /// <summary>
    ///     Shared recovery logic for both Inventory and Avatars on Vietnam274: reads the wire
    ///     Capacity/Count header (kept only for logging - not trusted as a position anchor),
    ///     scans forward for the real item list's start via <see cref="FindInventoryItemsStart" />,
    ///     then extracts items into <paramref name="collection" /> the same way the real
    ///     extraction loop always has, stopping on the first null/EOF/out-of-range/non-ascending
    ///     item.
    /// </summary>
    private static void RecoverItems(Packet packet, InventoryItemCollection collection, int effectiveCapacity, string label)
    {
        var wireCapacity = packet.ReadByte();
        packet.ReadByte(); // Count - not trusted as a position anchor, only the item scan below is

        var searchStart = (int)(packet.Length - packet.Remaining);
        var itemsStart = FindInventoryItemsStart(packet, packet.GetBytes(), searchStart, effectiveCapacity);

        if (itemsStart < 0)
        {
            // FindInventoryItemsStart's scan (via CountValidItemChain) seeks the packet to
            // every candidate offset it tries, including the last one it gave up on - so on
            // a "nothing found" result the cursor is left wherever that last failed attempt
            // happened to land, not restored to where it was before the scan. Leaving it
            // there silently corrupts every read after this point (JOB2/Skills/QuestLog/...),
            // which is what let a failed Avatars recovery take down the entire character load
            // (Skills.FromPacket running off the end of the buffer) even though Avatars itself
            // failing is harmless on its own. Always put the cursor back to right after
            // Capacity/Count before giving up, so a failed scan only costs this collection.
            packet.SeekRead(searchStart, SeekOrigin.Begin);
            Log.Debug($"[CharData] Vietnam274 {label} item-list signature not found - {label} will likely come back empty.");
            return;
        }

        packet.SeekRead(itemsStart, SeekOrigin.Begin);
        var lastSlot = -1;

        for (var i = 0; i < effectiveCapacity; i++)
        {
            var beforeItem = (int)(packet.Length - packet.Remaining);

            InventoryItem item;
            try
            {
                item = InventoryItem.FromPacket(packet);
            }
            catch (Exception e)
            {
                // Not just EndOfStreamException: this is walking real (if misaligned) bytes
                // through every branch of InventoryItem.FromPacket - including rarely-exercised
                // ones, like the pet branch's item.Cos.Id write, where item.Cos is never
                // actually initialized by FromPacket and a coincidental "pet" item id there
                // throws NullReferenceException, not EndOfStreamException. Since this is
                // reached via content-based recovery in the first place (there's no reliable
                // position to trust here), any exception - not just the one we happened to
                // predict - means "this position doesn't work", not "abort the whole
                // CharacterData parse and leave Game.Player unset" (which is what letting an
                // uncaught exception escape this loop actually did).
                Log.Debug($"[CharData] {label} scan: FromPacket threw {e.GetType().Name} at attempt {i}, stopping this candidate.");
                break;
            }

            if (item == null || item.Slot >= effectiveCapacity || item.Slot <= lastSlot)
            {
                Log.Debug(
                    $"[CharData] {label} scan stopped at attempt {i}: "
                        + (item == null ? "item was null" : $"slot={item.Slot} (lastSlot={lastSlot}, wireCapacity={wireCapacity})")
                );
                packet.SeekRead(beforeItem, SeekOrigin.Begin);
                break;
            }

            Log.Debug($"[CharData] {label} item: slot={item.Slot} amount={item.Amount} name={item.Record?.GetRealName()}");

            lastSlot = item.Slot;
            collection.Add(item);
        }
    }

    /// <summary>
    ///     Finds the absolute offset of the first real inventory item's slot byte, by trying
    ///     every position where a cheap pre-check (<see cref="LooksLikeItemStart" />) suggests a
    ///     plausible start, walking each one's *full* chain via the existing, unmodified
    ///     <see cref="InventoryItem.FromPacket" /> (confirmed - via a brute-force scan cross-
    ///     referencing every 4-byte window against the real item reference table, then hand-
    ///     decoding the hits - to be the exact same wire format every other client type uses;
    ///     only its position in this packet is unknown), and keeping whichever candidate yields
    ///     the longest run of valid, ascending-slot items. Picking the first candidate that
    ///     merely got two items in a row missed several equipped pieces (weapon/armor, which
    ///     carry more magic options/sockets than the plainer accessories that happened to
    ///     satisfy a 2-item check first, further into the packet) - the real start is the one
    ///     that stays aligned for the *whole* chain, not just its first couple of records.
    /// </summary>
    private static int FindInventoryItemsStart(Packet packet, byte[] data, int searchStart, int effectiveCapacity)
    {
        // Bounded rather than data.Length: the real item list is a few hundred bytes at most,
        // and scanning deep into the Masteries/Skills region beyond it would both waste time and
        // needlessly spam "no item found" for every coincidental match there (that region is
        // dense with small integers that happen to double as valid item ids - see the mastery-
        // list scan's own comment above).
        var searchEnd = Math.Min(searchStart + 2000, data.Length - 9);

        var bestStart = -1;
        var bestLength = 0;

        // This probes potentially hundreds of candidate offsets, and InventoryItem.FromPacket
        // calls Log.Notify on every failed item-id lookup - which, at this volume, was enough
        // to exhaust the process's Windows USER handle quota and take down logging entirely
        // ("Error creating window handle"). None of these candidate attempts are real user-
        // facing events; only the final extraction pass (using whichever offset wins here)
        // should actually reach the log.
        Log.Suppressed = true;
        try
        {
            for (var i = searchStart; i <= searchEnd; i++)
            {
                if (!LooksLikeItemStart(data, i))
                    continue;

                var length = CountValidItemChain(packet, i, effectiveCapacity);
                if (length > bestLength)
                {
                    bestLength = length;
                    bestStart = i;
                }
            }
        }
        finally
        {
            Log.Suppressed = false;
        }

        return bestLength >= 2 ? bestStart : -1;
    }

    /// <summary>
    ///     Walks a full chain of items starting at <paramref name="start" />, stopping on the
    ///     same conditions the real extraction loop uses (null/EOF/non-ascending/out-of-range
    ///     slot), and returns how many valid items were found - used to compare candidate start
    ///     positions by how far their alignment actually holds up, not just their first item.
    /// </summary>
    private static int CountValidItemChain(Packet packet, int start, int effectiveCapacity)
    {
        packet.SeekRead(start, SeekOrigin.Begin);

        var lastSlot = -1;
        var count = 0;

        for (var i = 0; i < effectiveCapacity; i++)
        {
            InventoryItem item;
            try
            {
                item = InventoryItem.FromPacket(packet);
            }
            catch (Exception)
            {
                // Broad on purpose - see the matching catch in RecoverItems for why (this is
                // the exploratory half of the same speculative-parsing scan, so it needs the
                // same tolerance for hitting FromPacket's rarely-exercised, not-fully-defensive
                // branches on essentially-random candidate bytes).
                break;
            }

            if (item == null || item.Slot >= effectiveCapacity || item.Slot <= lastSlot)
                break;

            lastSlot = item.Slot;
            count++;
        }

        return count;
    }

    /// <summary>
    ///     Cheap, non-parsing pre-check for a plausible item start at <paramref name="offset" />:
    ///     a slot byte followed by a 4-byte RentInfo.Type of 0 (the common "not rented" case -
    ///     rented items are rare enough that this heuristic skipping them is an acceptable
    ///     trade-off for not having to replicate RentInfo's own branching here) and then a
    ///     4-byte little-endian value that resolves to a real item.
    /// </summary>
    private static bool LooksLikeItemStart(byte[] data, int offset)
    {
        var rentType = (uint)(data[offset + 1] | (data[offset + 2] << 8) | (data[offset + 3] << 16) | (data[offset + 4] << 24));
        if (rentType != 0)
            return false;

        var itemId = (uint)(data[offset + 5] | (data[offset + 6] << 8) | (data[offset + 7] << 16) | (data[offset + 8] << 24));
        return Game.ReferenceManager.GetRefItem(itemId) != null;
    }
}
