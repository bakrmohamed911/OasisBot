using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RSBot.Core.Objects;
using RSBot.Core.Objects.Spawn;

namespace RSBot.Core.Components;

public class PickupManager
{
    /// <summary>
    ///     Gets or sets a value indicating whether this <see cref="PickupManager" /> is running.
    /// </summary>
    /// <value>
    ///     <c>true</c> if running; otherwise, <c>false</c>.
    /// </value>
    public static bool RunningPlayerPickup { get; private set; }

    /// <summary>
    ///     Gets or sets a value indicating whether this <see cref="PickupManager" /> is running for AbilityPet.
    /// </summary>
    /// <value>
    ///     <c>true</c> if running; otherwise, <c>false</c>.
    /// </value>
    public static bool RunningAbilityPetPickup { get; private set; }

    /// <summary>
    ///     Gets or sets the pickup items.
    /// </summary>
    /// <value>
    ///     The pickup items.
    /// </value>
    public static List<(string CodeName, bool PickOnlyChar)> PickupFilter { get; } = new();

    /// <summary>
    ///     Gets or sets a value indicating whether [pickup gold].
    /// </summary>
    /// <value>
    ///     <c>true</c> if [pickup gold]; otherwise, <c>false</c>.
    /// </value>
    public static bool PickupGold => PlayerConfig.Get("RSBot.Items.Pickup.Gold", true);

    /// <summary>
    ///     Gets or sets a value indicating whether [pickup rare items].
    /// </summary>
    /// <value>
    ///     <c>true</c> if [pickup rare items]; otherwise, <c>false</c>.
    /// </value>
    public static bool PickupRareItems => PlayerConfig.Get("RSBot.Items.Pickup.Rare", true);

    /// <summary>
    ///     Gets or sets a value indicating whether [pickup rare items].
    /// </summary>
    /// <value>
    ///     <c>true</c> if [pickup rare items]; otherwise, <c>false</c>.
    /// </value>
    public static bool PickupBlueItems => PlayerConfig.Get("RSBot.Items.Pickup.Blue", true);

    /// <summary>
    ///     Gets or sets a value indicating whether [pickup quest items].
    /// </summary>
    /// <value>
    ///     <c>true</c> if [pickup quest items]; otherwise, <c>false</c>.
    /// </value>
    public static bool PickupQuestItems => PlayerConfig.Get("RSBot.Items.Pickup.Quest", true);

    /// <summary>
    ///     Gets or sets a value indicating whether [pickup clean equips].
    /// </summary>
    /// <value>
    ///     <c>true</c> if [pickup clean equips]; otherwise, <c>false</c>.
    /// </value>
    public static bool PickupAnyEquips => PlayerConfig.Get("RSBot.Items.Pickup.AnyEquips", true);

    /// <summary>
    ///     Gets or sets a value indicating whether [pickup everything].
    /// </summary>
    /// <value>
    ///     <c>true</c> if [pickup everything]; otherwise, <c>false</c>.
    /// </value>
    public static bool PickupEverything => PlayerConfig.Get("RSBot.Items.Pickup.Everything", true);

    /// <summary>
    ///     Gets or sets a value indicating whether [use ability pet].
    /// </summary>
    /// <value>
    ///     <c>true</c> if [use ability pet]; otherwise, <c>false</c>.
    /// </value>
    public static bool UseAbilityPet => PlayerConfig.Get("RSBot.Items.Pickup.EnableAbilityPet", true);

    /// <summary>
    ///     Gets or sets a value indicating whether [just pick my items].
    /// </summary>
    /// <value>
    ///     <c>true</c> if [use ability pet]; otherwise, <c>false</c>.
    /// </value>
    public static bool JustPickMyItems => PlayerConfig.Get("RSBot.Items.Pickup.JustPickMyItems", false);

    /// <summary>
    ///     Cheap existence check for whether any item near <paramref name="centerPosition" />
    ///     currently passes <see cref="Condition" /> - i.e. whether <see cref="RunPlayer" />
    ///     would actually have something to do, without doing any of its walking/pickup work.
    ///     Used to hold off engaging a new target while there's still loot from the last kill
    ///     to grab: during a long/chained fight (a tanky champion in particular - see the call
    ///     site) <see cref="Game.Player" />'s InAction flag can stay continuously true with no
    ///     clean gap for <see cref="RunPlayer" /> to ever get a turn before the next fight is
    ///     already underway, so the previous kill's drop - often the more valuable one, for a
    ///     champion - gets left behind and risks despawning while the bot moves on.
    /// </summary>
    /// <param name="playerPosition">The player position.</param>
    /// <param name="centerPosition">The center position.</param>
    /// <param name="radius">The radius.</param>
    public static bool HasPendingLoot(Position playerPosition, Position centerPosition, int radius = 50)
    {
        var flag = UseAbilityPet && Game.Player.HasActiveAbilityPet;
        return SpawnManager.TryGetEntities<SpawnedItem>(i => Condition(i, centerPosition, radius, flag, flag), out _);
    }

    /// <summary>
    ///     Runs the specified center position.
    /// </summary>
    /// <param name="playerPosition">The player position.</param>
    /// <param name="centerPosition">The center position.</param>
    /// <param name="radius">The radius.</param>
    public static void RunPlayer(Position playerPosition, Position centerPosition, int radius = 50)
    {
        if (RunningPlayerPickup)
            return;

        RunningPlayerPickup = true;
        try
        {
            var flag = UseAbilityPet && Game.Player.HasActiveAbilityPet;
            if (
                !SpawnManager.TryGetEntities<SpawnedItem>(
                    i => Condition(i, centerPosition, radius, flag, flag),
                    out var entities
                )
            )
            {
                RunningPlayerPickup = false;
                return;
            }

            foreach (
                var item in entities.OrderBy(item =>
                    item.Movement.Source.DistanceTo(playerPosition) /*.Take(5)*/
                )
            )
            {
                if (!RunningPlayerPickup)
                    return;

                // Used to busy-wait here until InAction cleared. Botbase.Tick() runs every
                // 100ms but skips attack/movement/everything else entirely while
                // RunningPlayerPickup is true, so this wasn't a harmless pause - during
                // continuous combat, InAction can flip back to true asynchronously (server
                // packets, not this tick's own Attack.Invoke()) before the sleep loop ever
                // exits, stalling the *whole bot* for seconds per item, not just pickup. Bail
                // instead: the very next tick's LootBundle.Invoke() already re-checks
                // Game.Player.InAction cheaply (no blocking) before calling RunPlayer again, so
                // deferring the rest of this pass to then costs at most ~100ms, not seconds.
                if (Game.Player.InAction)
                    break;

                if (item.Record.IsSpecialtyGoodBox && Game.Player.Job2SpecialtyBag.Full)
                    continue;

                // Pickup() just sends a "pick this up" request - the server enforces its own
                // range check and silently ignores it if the player isn't close enough, with
                // no retry here. Without walking over first, loot sitting anywhere beyond
                // pickup range (e.g. dropped while chasing a different mob mid-fight, or from
                // an earlier kill by the time the loot bundle gets a turn) never actually gets
                // picked up despite passing every filter - indistinguishable from "the bot is
                // just being lazy about it" to anyone watching.
                MoveToItem(item);
                item.Pickup();
            }
        }
        catch (Exception e)
        {
            Log.Fatal(e);
        }
        finally
        {
            RunningPlayerPickup = false;
        }
    }

    /// <summary>
    ///     Walks the player to within pickup range of an item, if not already close enough.
    ///     <see cref="SpawnedItem.Pickup" /> only sends the pickup request - the server enforces
    ///     its own range check and just ignores it if the player is too far, with no feedback
    ///     or retry - so without this, loot outside pickup range (e.g. dropped while chasing a
    ///     different mob, or sitting from an earlier kill) silently never gets picked up despite
    ///     passing every filter.
    /// </summary>
    /// <param name="item">The item to move toward.</param>
    private static void MoveToItem(SpawnedItem item)
    {
        const int pickupRange = 5;
        const int moveTimeoutMs = 5000;

        if (Game.Player.Position.DistanceTo(item.Movement.Source) <= pickupRange)
            return;

        if (!Game.Player.MoveTo(item.Movement.Source))
            return;

        var deadline = Kernel.TickCount + moveTimeoutMs;
        while (RunningPlayerPickup && Kernel.TickCount < deadline)
        {
            if (Game.Player.Position.DistanceTo(item.Movement.Source) <= pickupRange)
                return;

            Thread.Sleep(100);
        }
    }

    public static async void RunAbilityPet(Position centerPosition, int radius = 50)
    {
        if (RunningAbilityPetPickup)
            return;

        RunningAbilityPetPickup = true;

        try
        {
            if (
                !SpawnManager.TryGetEntities<SpawnedItem>(
                    i => Condition(i, centerPosition, radius, true),
                    out var entities
                )
            )
            {
                RunningAbilityPetPickup = false;
                return;
            }

            foreach (
                var item in entities.OrderBy(item => item.Movement.Source.DistanceTo(Game.Player.AbilityPet.Position))
            )
            {
                if (!RunningAbilityPetPickup)
                    return;

                if (item.Record.IsSpecialtyGoodBox && Game.Player.Job2SpecialtyBag.Full)
                    continue;

                await Game.Player.AbilityPet.PickupAsync(item.UniqueId);
                await Task.Yield();
            }
        }
        catch (Exception e)
        {
            Log.Fatal(e);
        }
        finally
        {
            RunningAbilityPetPickup = false;
        }
    }

    private static bool Condition(
        SpawnedItem e,
        Position centerPosition,
        int radius,
        bool applyPickOnlyChar = false,
        bool pickOnlyChar = false
    )
    {
        var playerJid = Game.Player.JID;

        // An item behind an obstacle can never actually be reached - Pickup() itself already
        // refuses to send the request for exactly this reason. Without checking it here too,
        // this Condition (shared by RunPlayer's real pickup pass and HasPendingLoot's cheap
        // existence check) treats a permanently-unreachable item as "pending" forever: Loot
        // wastes a walk/pickup attempt on it every tick with no backoff, and - since
        // Botbase.Tick() now holds Target/Attack back for as long as HasPendingLoot says
        // there's something to grab (see its own comment for why) - a single stuck item left
        // over from an earlier session (e.g. still sitting in the world across a bot restart)
        // can permanently deadlock combat: the character just stands there taking hits with no
        // target ever selected, because the bot believes it still has looting left to do.
        if (e.IsBehindObstacle)
            return false;

        if (JustPickMyItems && e.OwnerJID != playerJid)
            return false;

        // Check if the item is within the training area + tolerance, OR close enough to
        // the player to grab regardless of the area shape. The area-center check alone
        // rejects loot dropped near the *edge* of the training circle (e.g. a kill made
        // while chasing/engaging a mob right at the area boundary) even though the player
        // is standing right next to the drop - that loot then never satisfies this check
        // and never gets picked up at all, not just "eventually". The player-distance
        // check is the actual intent ("can I reach this"); the center check stays too so
        // items far from both the player and the area (e.g. dropped by someone else on
        // the far side of the circle) still get filtered out.
        const int tolerance = 15;
        const int playerPickupRadius = 30;
        var withinArea = e.Movement.Source.DistanceTo(centerPosition) <= radius + tolerance;
        var withinPlayerReach = e.Movement.Source.DistanceTo(Game.Player.Position) <= playerPickupRadius;
        if (!withinArea && !withinPlayerReach)
            return false;

        if (applyPickOnlyChar && e.IsBehindObstacle)
            return false;

        bool isItemAutoShareParty = Game.Party.IsInParty && Game.Party.Settings.GetPartyType() is 2 or 3 or 6 or 7;

        if (isItemAutoShareParty && PickupGold && e.Record.IsGold)
        {
            if (!(applyPickOnlyChar && pickOnlyChar))
                return true;
        }

        if (e.HasOwner && e.OwnerJID != playerJid)
        {
            if (!isItemAutoShareParty)
                return false;

            if (e.Record.IsQuest && Game.Party.Members.Any(m => m.MemberId == e.OwnerJID))
                return false;
        }

        if (PickupGold && e.Record.IsGold && !(applyPickOnlyChar && pickOnlyChar))
            return true;

        if (
            (PickupRareItems && (byte)e.Rarity >= 2)
            || (PickupBlueItems && (byte)e.Rarity >= 1)
            || (PickupAnyEquips && e.Record.IsEquip)
            || (PickupQuestItems && e.Record.IsQuest)
            || PickupEverything
        )
            return true;

        return applyPickOnlyChar
            ? PickupFilter.Any(p => p.CodeName == e.Record.CodeName && p.PickOnlyChar == pickOnlyChar)
            : PickupFilter.Any(p => p.CodeName == e.Record.CodeName);
    }

    public static void AddFilter(string codeName, bool pickOnlyChar = false)
    {
        PickupFilter.RemoveAll(p => p.CodeName == codeName);
        PickupFilter.Add((codeName, pickOnlyChar));

        SaveFilter();
    }

    public static void RemoveFilter(string codeName)
    {
        PickupFilter.RemoveAll(p => p.CodeName == codeName);
        SaveFilter();
    }

    public static void LoadFilter()
    {
        // Same "OnEnterGame" fires-once-per-login (not once-per-process) situation as
        // ShoppingManager.LoadFilters() - clear first or a relogin mid-session duplicates
        // every entry already in this static list.
        PickupFilter.Clear();

        var config = PlayerConfig.GetArray<string>("RSBot.Shopping.Pickup");

        foreach (var item in config)
        {
            var split = item.Split('|');
            if (split.Length < 2)
                continue;

            PickupFilter.Add((split[0], Convert.ToBoolean(split[1])));
        }
    }

    public static void SaveFilter()
    {
        var array = PickupFilter.Select(p => $"{p.CodeName}|{p.PickOnlyChar}").ToArray();
        if (array.Length == 0)
            return;

        PlayerConfig.SetArray("RSBot.Shopping.Pickup", array);
    }

    /// <summary>
    ///     Stops this instance.
    /// </summary>
    public static void Stop()
    {
        RunningPlayerPickup = false;
        RunningAbilityPetPickup = false;
    }
}
