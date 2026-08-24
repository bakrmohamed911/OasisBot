using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using RSBot.Core;
using RSBot.Core.Event;
using RSBot.Core.Objects;
using RSBot.Training;
using RSBot.Training.Components;
using RSBot.Training.Views.Dialogs;
using SDUI.Controls;
using CheckBox = SDUI.Controls.CheckBox;

namespace RSBot.Training.Views;

[ToolboxItem(false)]
public partial class Main : DoubleBufferedControl
{
    private const int ScriptRecorderOwnerId = 2000;

    /// <summary>
    ///     Radius shown/applied for the Area section when a training place (patrol route) is
    ///     selected - a patrol route has no inherent radius of its own, so this is just a
    ///     sensible display default rather than a value read from the route itself.
    /// </summary>
    private const int DefaultTrainingPlaceRadius = 50;

    /// <summary>
    ///     The fixed set of rows shown in the "Mob preferences" checklist: a display label plus
    ///     the <see cref="MonsterRarity" /> flag(s) it represents. "Unique" combines Unique +
    ///     Unique2 into one row/flag, matching the original ListView's item tagging. Kept under
    ///     the "Avoidance" name internally since it backs the same "RSBot.Avoidance.*"
    ///     PlayerConfig keys AvoidanceBundle reads.
    /// </summary>
    private static readonly (string Label, MonsterRarity Rarity)[] AvoidanceRarities =
    {
        ("General", MonsterRarity.General),
        ("Champion", MonsterRarity.Champion),
        ("Giant", MonsterRarity.Giant),
        ("General (party)", MonsterRarity.GeneralParty),
        ("Champion (party)", MonsterRarity.ChampionParty),
        ("Giant (party)", MonsterRarity.GiantParty),
        ("Unique", MonsterRarity.Unique | MonsterRarity.Unique2),
        ("Strong", MonsterRarity.EliteStrong),
        ("Elite", MonsterRarity.Elite),
        ("Event", MonsterRarity.Event),
    };

    #region Fields

    private bool _settingsLoaded;

    /// <summary>
    ///     One entry per row built by <see cref="BuildAvoidanceChecklist" />, so
    ///     <see cref="SaveAvoidance" />/<see cref="LoadAvoidance" /> can read/write all three
    ///     checkboxes per monster rarity without walking the panel's Controls collection.
    /// </summary>
    private readonly List<(MonsterRarity Rarity, CheckBox Avoid, CheckBox Prefer, CheckBox Berserk)> _avoidanceRows =
        new();

    #endregion Fields

    /// <summary>
    ///     Initializes a new instance of the <see cref="Main" /> class.
    /// </summary>
    public Main()
    {
        InitializeComponent();
        txtSearchPlace.Enter += txtSearchPlace_Enter;
        SubscribeEvents();
    }

    /// <summary>
    ///     Subscribes the events.
    /// </summary>
    private void SubscribeEvents()
    {
        EventManager.SubscribeEvent("OnLoadCharacter", OnLoadCharacter);
        EventManager.SubscribeEvent("OnSetTrainingArea", OnSetTrainingArea);
        EventManager.SubscribeEvent("OnSaveScript", new Action<int, string>(OnSaveScript));
    }

    /// <summary>
    ///     Loads the settings.
    /// </summary>
    private void LoadSettings()
    {
        const string key = "RSBot.Training.";

        foreach (var checkbox in groupBoxAdvanced.Controls.OfType<CheckBox>())
            checkbox.Checked = PlayerConfig.Get(key + checkbox.Name, checkbox.Checked);

        foreach (var checkbox in groupBoxBerserk.Controls.OfType<CheckBox>())
            checkbox.Checked = PlayerConfig.Get(key + checkbox.Name, checkbox.Checked);

        foreach (var num in groupBoxBerserk.Controls.OfType<NumUpDown>())
            num.Value = PlayerConfig.Get(key + num.Name, num.Value);

        foreach (var checkbox in groupBoxTrainingPlace.Controls.OfType<CheckBox>())
            checkbox.Checked = PlayerConfig.Get(key + checkbox.Name, checkbox.Checked);

        radioCenter.Checked = PlayerConfig.Get(key + radioCenter.Name, false);
        radioWalkAround.Checked = PlayerConfig.Get(key + radioWalkAround.Name, true);

        LoadAvoidance();
        RefreshTrainingPlaceList();
    }

    /// <summary>
    ///     Saves the settings.
    /// </summary>
    private void ApplySettings()
    {
        const string key = "RSBot.Training.";

        foreach (var checkbox in groupBoxAdvanced.Controls.OfType<CheckBox>())
            PlayerConfig.Set(key + checkbox.Name, checkbox.Checked);

        foreach (var checkbox in groupBoxBerserk.Controls.OfType<CheckBox>())
            PlayerConfig.Set(key + checkbox.Name, checkbox.Checked);

        foreach (var num in groupBoxBerserk.Controls.OfType<NumUpDown>())
            PlayerConfig.Set(key + num.Name, num.Value);

        foreach (var checkbox in groupBoxTrainingPlace.Controls.OfType<CheckBox>())
            PlayerConfig.Set(key + checkbox.Name, checkbox.Checked);

        PlayerConfig.Set(key + radioCenter.Name, radioCenter.Checked);
        PlayerConfig.Set(key + radioWalkAround.Name, radioWalkAround.Checked);
    }

    /// <summary>
    ///     Handles the CheckedChanged event of the settings control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private void settings_CheckedChanged(object sender, EventArgs e)
    {
        if (_settingsLoaded)
            ApplySettings();
    }

    /// <summary>
    ///     Handles the ValueChanged event of the numSettings control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private void numSettings_ValueChanged(object sender, EventArgs e)
    {
        if (_settingsLoaded)
            ApplySettings();
    }

    /// <summary>
    ///     Called when the script recorder saves a script - the recorder's own Save dialog
    ///     already wrote the file and chose its name, so this only asks for the missing Level
    ///     (see <see cref="PromptLevelAndCatalogScript" />) rather than prompting to save it a
    ///     second time.
    /// </summary>
    private void OnSaveScript(int ownerId, string path)
    {
        if (IsDisposed || Disposing)
            return;

        if (ownerId != ScriptRecorderOwnerId)
            return;

        PromptLevelAndCatalogScript(path);
    }

    /// <summary>
    ///     Called when the training area has been set to a certain location. Will append a new script command "area" and
    ///     update the UI.
    /// </summary>
    private void OnSetTrainingArea()
    {
        if (IsDisposed || Disposing)
            return;

        var area = Kernel.Bot.Botbase.Area;

        txtXCoord.Text = area.Position.X.ToString("0.0");
        txtYCoord.Text = area.Position.Y.ToString("0.0");
        txtRadius.Text = area.Radius.ToString();
        txtRegion.Text = area.Position.Region.Id.ToString();

        EventManager.FireEvent("AppendScriptCommand", area.GetScriptLine());
    }

    #region Mob preferences checklist

    /// <summary>
    ///     Builds the "Mob preferences" checklist: one row per <see cref="AvoidanceRarities" />
    ///     entry, each with 3 mutually-exclusive checkboxes (Avoid/Prefer/Berserk) - replaces the
    ///     old ListView + right-click "assign to group" context menu with direct checkboxes.
    /// </summary>
    private void BuildAvoidanceChecklist()
    {
        const int checkboxSize = 22;
        const int nameX = 4;
        const int nameToChecklistGap = 10;
        const int colGap = 6;
        const int col1W = 44, col2W = 44, col3W = 56; // "Berserk" needs more room than the other two.

        avoidanceListPanel.SuspendLayout();
        avoidanceListPanel.Controls.Clear();
        _avoidanceRows.Clear();

        // Build the name labels first (AutoSize=true, explicit Font so ambient-font timing
        // doesn't matter) and let their real measured widths decide where the checkbox columns
        // start, rather than a hand-picked fixed width that clipped longer entries - e.g.
        // "Champion (party)" clipped down to "Champion" read as a duplicate of the plain
        // "Champion" row above it.
        var rows = new List<(SDUI.Controls.Label NameLabel, MonsterRarity Rarity)>();
        var maxNameWidth = 0;
        var y = 26;
        foreach (var (text, rarity) in AvoidanceRarities)
        {
            var nameLabel = new SDUI.Controls.Label
            {
                ApplyGradient = false,
                AutoSize = true,
                BackColor = System.Drawing.Color.Transparent,
                Font = Font,
                ForeColor = System.Drawing.Color.FromArgb(0, 0, 0),
                Text = text,
            };
            nameLabel.Location = new System.Drawing.Point(nameX, y + 2);

            avoidanceListPanel.Controls.Add(nameLabel);
            rows.Add((nameLabel, rarity));

            // nameLabel.Width right after construction is unreliable for this - an AutoSize
            // label only resolves its true size once it's actually parented/laid out, reporting
            // the plain WinForms-default 100 until then. That inflated "100" silently flowed
            // into col1X/col2X/col3X below (and, worse, into CreateColumnHeader's centering,
            // which read the same kind of not-yet-resolved Width) - measuring the text directly
            // sidesteps the timing issue entirely.
            maxNameWidth = Math.Max(maxNameWidth, TextRenderer.MeasureText(text, Font).Width);

            y += 26;
        }

        var col1X = nameX + maxNameWidth + nameToChecklistGap;
        var col2X = col1X + col1W + colGap;
        var col3X = col2X + col2W + colGap;
        // A separate, smaller Font (7.5F) for these clipped its last character even with
        // AutoSize - reusing the same ambient Font the (correctly-sized) name labels use avoids
        // whatever made the smaller size's AutoSize computation come up just short.
        var headerFont = Font;

        avoidanceListPanel.Controls.Add(CreateColumnHeader("Avoid", headerFont, col1X + col1W / 2));
        avoidanceListPanel.Controls.Add(CreateColumnHeader("Prefer", headerFont, col2X + col2W / 2));
        avoidanceListPanel.Controls.Add(CreateColumnHeader("Berserk", headerFont, col3X + col3W / 2));

        // Read each row's checkbox Y straight off its already-placed name label rather than
        // tracking a second running offset in parallel - the two used different starting values
        // (26 vs 22) and drifted apart, leaving every row's checkboxes a few pixels above its
        // name label instead of level with it.
        foreach (var (nameLabel, rarity) in rows)
        {
            var rowY = nameLabel.Top - (checkboxSize - nameLabel.Height) / 2;
            var avoidCheck = CreateRowCheckbox(col1X + (col1W - checkboxSize) / 2, rowY, checkboxSize, rarity);
            var preferCheck = CreateRowCheckbox(col2X + (col2W - checkboxSize) / 2, rowY, checkboxSize, rarity);
            var berserkCheck = CreateRowCheckbox(col3X + (col3W - checkboxSize) / 2, rowY, checkboxSize, rarity);

            void OnRowChanged(CheckBox changed)
            {
                if (changed.Checked)
                {
                    if (avoidCheck != changed)
                        avoidCheck.Checked = false;
                    if (preferCheck != changed)
                        preferCheck.Checked = false;
                    if (berserkCheck != changed)
                        berserkCheck.Checked = false;
                }

                if (_settingsLoaded)
                    SaveAvoidance();
            }

            avoidCheck.CheckedChanged += (s, e) => OnRowChanged(avoidCheck);
            preferCheck.CheckedChanged += (s, e) => OnRowChanged(preferCheck);
            berserkCheck.CheckedChanged += (s, e) => OnRowChanged(berserkCheck);

            avoidanceListPanel.Controls.Add(avoidCheck);
            avoidanceListPanel.Controls.Add(preferCheck);
            avoidanceListPanel.Controls.Add(berserkCheck);

            _avoidanceRows.Add((rarity, avoidCheck, preferCheck, berserkCheck));
        }

        avoidanceListPanel.ResumeLayout(true);
    }

    /// <summary>
    ///     Builds a column header centered on <paramref name="centerX" />, sized by AutoSize (an
    ///     explicit Font, so ambient-font-resolution timing doesn't matter) rather than a guessed
    ///     pixel width - "Berserk" clipped to "Ber"/"Bers" in a manually-sized, fixed-width label.
    /// </summary>
    private static SDUI.Controls.Label CreateColumnHeader(string text, System.Drawing.Font font, int centerX)
    {
        var label = new SDUI.Controls.Label
        {
            ApplyGradient = false,
            AutoSize = true,
            BackColor = System.Drawing.Color.Transparent,
            Font = font,
            ForeColor = System.Drawing.Color.FromArgb(130, 130, 130),
            Text = text,
        };

        // label.Width is unreliable here - AutoSize only resolves the label's true size once
        // it's actually parented/laid out, so immediately after construction it still reports
        // the plain WinForms-default 100 regardless of Font/Text. Centering off that stale 100
        // instead of the true ~38-45px width left every header several px left of its column's
        // real center once the label's later auto-resize (which preserves Location) kicked in.
        var measuredWidth = TextRenderer.MeasureText(text, font).Width;
        label.Location = new System.Drawing.Point(centerX - measuredWidth / 2, 2);

        return label;
    }

    private static CheckBox CreateRowCheckbox(int x, int y, int size, MonsterRarity rarity)
    {
        return new CheckBox
        {
            AutoSize = false,
            BackColor = System.Drawing.Color.Transparent,
            Depth = 0,
            Ripple = true,
            Location = new System.Drawing.Point(x, y),
            Size = new System.Drawing.Size(size, size),
            Tag = rarity,
            Text = string.Empty,
        };
    }

    /// <summary>
    ///     Saves the avoidance.
    /// </summary>
    private void SaveAvoidance()
    {
        var avoid = new List<MonsterRarity>();
        var prefer = new List<MonsterRarity>();
        var berserk = new List<MonsterRarity>();

        foreach (var row in _avoidanceRows)
            if (row.Avoid.Checked)
                avoid.Add(row.Rarity);
            else if (row.Prefer.Checked)
                prefer.Add(row.Rarity);
            else if (row.Berserk.Checked)
                berserk.Add(row.Rarity);

        PlayerConfig.SetArray("RSBot.Avoidance.Avoid", avoid);
        PlayerConfig.SetArray("RSBot.Avoidance.Prefer", prefer);
        PlayerConfig.SetArray("RSBot.Avoidance.Berserk", berserk);
    }

    /// <summary>
    ///     Loads the avoidance.
    /// </summary>
    private void LoadAvoidance()
    {
        var avoid = PlayerConfig.GetEnums<MonsterRarity>("RSBot.Avoidance.Avoid");
        var prefer = PlayerConfig.GetEnums<MonsterRarity>("RSBot.Avoidance.Prefer");
        var berserk = PlayerConfig.GetEnums<MonsterRarity>("RSBot.Avoidance.Berserk");

        foreach (var row in _avoidanceRows)
        {
            row.Avoid.Checked = avoid.Any(v => (row.Rarity & v) == v);
            row.Prefer.Checked = !row.Avoid.Checked && prefer.Any(v => (row.Rarity & v) == v);
            row.Berserk.Checked = !row.Avoid.Checked && !row.Prefer.Checked && berserk.Any(v => (row.Rarity & v) == v);
        }
    }

    #endregion Mob preferences checklist

    #region Training place selection

    /// <summary>
    ///     A borderless popup window that never takes keyboard focus/activation (WS_EX_NOACTIVATE)
    ///     - used for <see cref="_placeResultsList" /> so showing/updating it while the user types
    ///     in <see cref="txtSearchPlace" /> never steals focus away from the search box, the same
    ///     way a native combo box's own suggestion dropdown behaves. A ToolStripDropDown was tried
    ///     first, but showing it (and re-showing it on every keystroke as matches change) handed
    ///     focus to its hosted ListBox, which then swallowed every keystroke after the first.
    /// </summary>
    private sealed class NonActivatingPopup : Form
    {
        public NonActivatingPopup()
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
        }

        protected override bool ShowWithoutActivation => true;

        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_NOACTIVATE = 0x08000000;
                var cp = base.CreateParams;
                cp.ExStyle |= WS_EX_NOACTIVATE;
                return cp;
            }
        }
    }

    /// <summary>
    ///     Lazily-created popup listing catalog matches for whatever's typed in
    ///     <see cref="txtSearchPlace" /> - a real dropdown that only appears while there's
    ///     something to search/show, replacing the old always-visible (and, when empty, blank
    ///     black-looking) result list.
    /// </summary>
    private NonActivatingPopup _placeDropDown;

    private ListBox _placeResultsList;

    private void EnsurePlaceDropDown()
    {
        if (_placeDropDown != null)
            return;

        _placeResultsList = new ListBox
        {
            BorderStyle = BorderStyle.FixedSingle,
            IntegralHeight = false,
            ItemHeight = 20,
            Font = txtSearchPlace.Font,
            Dock = DockStyle.Fill,
        };
        _placeResultsList.Click += (s, e) => CommitPlaceDropDownSelection();
        _placeResultsList.KeyDown += (s, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                CommitPlaceDropDownSelection();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                _placeDropDown.Hide();
            }
        };

        _placeDropDown = new NonActivatingPopup();
        _placeDropDown.Controls.Add(_placeResultsList);
    }

    private void CommitPlaceDropDownSelection()
    {
        if (_placeResultsList?.SelectedItem is TrainingPlaceEntry entry)
            ActivateTrainingPlace(entry);

        _placeDropDown?.Hide();
    }

    /// <summary>
    ///     Re-filters the catalog by whatever's currently typed in <see cref="txtSearchPlace" />
    ///     (name substring or exact level match) and shows the matches as a dropdown beneath it -
    ///     closed entirely when there's nothing to show or the search box isn't focused.
    /// </summary>
    private void RefreshTrainingPlaceList()
    {
        var matches = TrainingPlaceCatalog
            .Search(txtSearchPlace.Text)
            .OrderBy(en => en.Level)
            .ThenBy(en => en.Name)
            .ToList();

        EnsurePlaceDropDown();

        _placeResultsList.BeginUpdate();
        _placeResultsList.Items.Clear();
        foreach (var entry in matches)
            _placeResultsList.Items.Add(entry);
        _placeResultsList.EndUpdate();

        // txtSearchPlace is a composite control (SDUI.Controls.TextBox) that forwards real
        // keyboard focus to a private inner native TextBox, so its own Focused is always false
        // while the user is actually typing - ContainsFocus checks the whole subtree instead.
        if (matches.Count == 0 || !txtSearchPlace.ContainsFocus)
        {
            _placeDropDown.Hide();
            return;
        }

        var visibleRows = Math.Min(matches.Count, 6);
        var height = visibleRows * _placeResultsList.ItemHeight + 4;
        var screenLocation = txtSearchPlace.PointToScreen(new System.Drawing.Point(0, txtSearchPlace.Height));
        _placeDropDown.Bounds = new System.Drawing.Rectangle(
            screenLocation,
            new System.Drawing.Size(txtSearchPlace.Width, height)
        );

        if (!_placeDropDown.Visible)
            _placeDropDown.Show(this);
    }

    /// <summary>
    ///     Re-selects (and re-activates) whichever training place was active last session, if it's
    ///     still in the catalog.
    /// </summary>
    private void SelectSavedTrainingPlace()
    {
        var savedName = PlayerConfig.Get("RSBot.Training.SelectedPlaceScript", string.Empty);
        if (string.IsNullOrEmpty(savedName))
            return;

        var entry = TrainingPlaceCatalog.Entries.FirstOrDefault(en => en.Name == savedName);
        if (entry != null)
            ActivateTrainingPlace(entry);
    }

    private void txtSearchPlace_TextChanged(object sender, EventArgs e)
    {
        RefreshTrainingPlaceList();
    }

    private void txtSearchPlace_Enter(object sender, EventArgs e)
    {
        RefreshTrainingPlaceList();
    }

    /// <summary>
    ///     Loads and activates a catalogued training place, reflecting it in the search box and
    ///     the Area section.
    /// </summary>
    private void ActivateTrainingPlace(TrainingPlaceEntry entry)
    {
        if (!TrainingPlaceManager.Load(entry.FilePath, entry.Name))
        {
            MessageBox.Show(
                $"Could not load the walk script for \"{entry.Name}\" - the file may have been moved or deleted.",
                "Training place",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
            return;
        }

        PlayerConfig.Set("RSBot.Training.SelectedPlaceScript", entry.Name);
        SetSearchPlaceTextSilently(entry.ToString());

        if (entry.Area != null)
        {
            // Use the area captured from this script's own "area ..." line (see
            // PromptLevelAndCatalogScript/PromptAndCatalogScript) - set exactly the way that
            // line itself would apply it (see TrainingAreaScriptCommand), rather than guessing
            // one from the route's first waypoint.
            var area = entry.Area;
            PlayerConfig.Set("RSBot.Area.Region", area.Region);
            PlayerConfig.Set("RSBot.Area.X", area.XOffset);
            PlayerConfig.Set("RSBot.Area.Y", area.YOffset);
            PlayerConfig.Set("RSBot.Area.Z", area.ZOffset);
            PlayerConfig.Set("RSBot.Area.Radius", area.Radius);
            EventManager.FireEvent("OnSetTrainingArea");
        }
        else
        {
            // No captured area (imported/converted script, or an older catalog entry saved
            // before this was tracked) - reflect the route's own starting point instead, rather
            // than leaving the Area section showing whatever was there before. MovementBundle's
            // own per-tick sync will keep moving it to follow the player once the patrol is
            // actually running. A patrol route has no real "radius" of its own (it follows a
            // fixed waypoint loop, not a circular wander-area), so default it to a sensible
            // value rather than leaving whatever unrelated radius happened to be configured
            // before.
            PlayerConfig.Set("RSBot.Area.Radius", DefaultTrainingPlaceRadius);

            var start = TrainingPlaceManager.CurrentWaypoint;
            TrainingManager.ApplyTrainingArea(start.X, start.Y, start.Region);
        }
    }

    /// <summary>
    ///     Sets <see cref="txtSearchPlace" />'s text without re-triggering the search filter -
    ///     used when reflecting a selection rather than the user actively searching.
    /// </summary>
    private void SetSearchPlaceTextSilently(string text)
    {
        txtSearchPlace.TextChanged -= txtSearchPlace_TextChanged;
        txtSearchPlace.Text = text;
        txtSearchPlace.TextChanged += txtSearchPlace_TextChanged;
    }

    private void btnClearPlace_Click(object sender, EventArgs e)
    {
        TrainingPlaceManager.Deactivate();
        PlayerConfig.Set("RSBot.Training.SelectedPlaceScript", string.Empty);
        SetSearchPlaceTextSilently(string.Empty);
        _placeDropDown?.Hide();
    }

    #endregion Training place selection

    #region Create a walk script

    private void btnRecord_Click(object sender, EventArgs e)
    {
        EventManager.FireEvent("OnShowScriptRecorder", ScriptRecorderOwnerId, true);
    }

    private void btnImportScript_Click(object sender, EventArgs e)
    {
        var diag = new OpenFileDialog
        {
            Filter = @"RSBot Bot script or legacy walk script (*.rbs;*.txt)|*.rbs;*.txt",
            Title = @"Import a training place walk script",
        };

        if (diag.ShowDialog() != DialogResult.OK)
            return;

        var importPath = ConvertIfLegacyScript(diag.FileName);
        if (importPath != null)
            PromptAndCatalogScript(importPath);
    }

    /// <summary>
    ///     Detects and converts a legacy training-place script (go(x,y)/go,x,y/delay(N)/wait/
    ///     teleport/fly/buff/skill/kill/scriptversion= syntax) into OasisBot's native "move"/
    ///     "wait" script syntax via <see cref="LegacyScriptConverter" />, so importing doesn't
    ///     require the file to already be in that format. Returns the path to hand to
    ///     <see cref="PromptAndCatalogScript" /> - the original path unchanged if it's already
    ///     native, a new converted file's path if it needed converting, or null if the user
    ///     should not proceed (unsupported format, or read failure).
    /// </summary>
    private string ConvertIfLegacyScript(string filePath)
    {
        string[] lines;
        try
        {
            lines = File.ReadAllLines(filePath);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Could not read \"{Path.GetFileName(filePath)}\": {ex.Message}",
                "Import training place script",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
            return null;
        }

        if (LegacyScriptConverter.IsUnsupportedHexFormat(lines))
        {
            MessageBox.Show(
                $"\"{Path.GetFileName(filePath)}\" uses an old packed-format move(...) that can't be "
                    + "automatically converted - there's no known way to decode it safely. "
                    + "Please provide a plain-text walk script instead.",
                "Import training place script",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
            return null;
        }

        if (!LegacyScriptConverter.NeedsConversion(lines))
            return filePath;

        var converted = LegacyScriptConverter.Convert(lines, Path.GetFileName(filePath), out var unrecognizedCount);
        var destPath = GetUniqueConvertedScriptPath(filePath);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(destPath));
            File.WriteAllLines(destPath, converted);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Could not save the converted script: {ex.Message}",
                "Import training place script",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
            return null;
        }

        if (unrecognizedCount > 0)
            MessageBox.Show(
                $"\"{Path.GetFileName(filePath)}\" was converted to OasisBot's walk script format, but "
                    + $"{unrecognizedCount} line(s) (teleport/fly/buff/skill/kill/scriptversion= commands, or "
                    + "lines that weren't recognized at all) could not be translated automatically and were "
                    + "commented out instead - the route will still walk correctly, but you may want to "
                    + "review those lines manually.",
                "Import training place script",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

        return destPath;
    }

    /// <summary>
    ///     Picks a non-colliding destination for a converted script under the same
    ///     Data/Scripts/TrainingPlaces folder recorded/catalogued scripts live in, named after
    ///     the original file (with a numeric suffix if that name's already taken).
    /// </summary>
    private static string GetUniqueConvertedScriptPath(string sourceFilePath)
    {
        var destDir = Path.Combine(Kernel.BasePath, "Data", "Scripts", "TrainingPlaces");
        var baseName = Path.GetFileNameWithoutExtension(sourceFilePath);

        var destPath = Path.Combine(destDir, baseName + ".rbs");
        var suffix = 1;
        while (File.Exists(destPath))
            destPath = Path.Combine(destDir, $"{baseName} ({suffix++}).rbs");

        return destPath;
    }

    /// <summary>
    ///     Save flow for an imported script (from <see cref="btnImportScript_Click" />): prompts
    ///     for Name/Level - offering to rename makes sense here, since the file's own name may be
    ///     a cryptic legacy one, or one just handed back from <see cref="ConvertIfLegacyScript" />
    ///     - then adds the entry to the training-place catalog and selects it in the search
    ///     results.
    /// </summary>
    private void PromptAndCatalogScript(string filePath)
    {
        var defaultName = Path.GetFileNameWithoutExtension(filePath);

        using var dialog = new SaveTrainingPlaceDialog(defaultName);
        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;

        var entry = new TrainingPlaceEntry
        {
            Name = dialog.PlaceName.Text.Trim(),
            Level = (int)dialog.Level.Value,
            FilePath = filePath,
            Area = TryReadCapturedArea(filePath),
        };

        TrainingPlaceCatalog.Add(entry);
        ActivateTrainingPlace(entry);
    }

    /// <summary>
    ///     Save flow for a freshly-recorded script (from <see cref="OnSaveScript" />): the
    ///     recorder's own Save dialog already wrote the file under a name the user chose, so
    ///     re-prompting for Name here would just be a second, redundant save step (and risk the
    ///     catalog Name drifting from the actual file name) - only Level is missing, so that's
    ///     all this asks for, reusing the recorder's file and name as-is.
    /// </summary>
    private void PromptLevelAndCatalogScript(string filePath)
    {
        var name = Path.GetFileNameWithoutExtension(filePath);

        using var dialog = new SaveTrainingPlaceDialog(name, nameEditable: false);
        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;

        var entry = new TrainingPlaceEntry
        {
            Name = name,
            Level = (int)dialog.Level.Value,
            FilePath = filePath,
            Area = TryReadCapturedArea(filePath),
        };

        TrainingPlaceCatalog.Add(entry);

        // Deliberately not calling ActivateTrainingPlace here, unlike the import flow - this
        // runs synchronously inside the ScriptRecorder's own Save handler (see OnSaveScript,
        // fired before the recorder closes), and activating changes the Area, which fires
        // "OnSetTrainingArea" -> "AppendScriptCommand" - the still-open recorder would catch
        // that and silently append a bogus "area ..." line (reflecting whatever the area was
        // *before* this recording, not anything just recorded) to its script buffer. The user
        // can select the new entry from the search dropdown themselves once they're done.
        SetSearchPlaceTextSilently(entry.ToString());
    }

    /// <summary>
    ///     Reads <paramref name="filePath" /> and looks for a captured "area ..." line (see
    ///     <see cref="TrainingPlaceCatalog.TryParseAreaLine" />) - null (not an error) if the
    ///     file has none, or can't be read at all.
    /// </summary>
    private static TrainingPlaceArea TryReadCapturedArea(string filePath)
    {
        try
        {
            return TrainingPlaceCatalog.TryParseAreaLine(File.ReadAllLines(filePath));
        }
        catch
        {
            return null;
        }
    }

    #endregion Create a walk script

    /// <summary>
    ///     Handles the Click event of the btnGetCurrent control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private void btnGetCurrent_Click(object sender, EventArgs e)
    {
        var pos = Game.Player.Position;

        PlayerConfig.Set("RSBot.Area.Region", pos.Region);
        PlayerConfig.Set("RSBot.Area.X", pos.XOffset);
        PlayerConfig.Set("RSBot.Area.Y", pos.YOffset);
        PlayerConfig.Set("RSBot.Area.Z", pos.ZOffset);

        EventManager.FireEvent("OnSetTrainingArea");
    }

    /// <summary>
    ///     Handles the TextChanged event of the txtXCoord control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private void txtXCoord_TextChanged(object sender, EventArgs e)
    {
        if (!float.TryParse(txtXCoord.Text, out var result))
            return;
    }

    /// <summary>
    ///     Handles the TextChanged event of the txtYCoord control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private void txtYCoord_TextChanged(object sender, EventArgs e)
    {
        if (!float.TryParse(txtYCoord.Text, out var result))
            return;
    }

    /// <summary>
    ///     Handles the TextChanged event of the txtRadius control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private void txtRadius_TextChanged(object sender, EventArgs e)
    {
        if (!int.TryParse(txtRadius.Text, out var result))
            return;

        PlayerConfig.Set("RSBot.Area.Radius", result);
    }

    /// <summary>
    ///     Handles the TextChanged event of the txtRegion control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private void txtRegion_TextChanged(object sender, EventArgs e)
    {
        if (!ushort.TryParse(txtRegion.Text, out ushort result))
            return;
    }

    /// <summary>
    ///     Handles the Click event of the btnApplyArea control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private void btnApplyArea_Click(object sender, EventArgs e)
    {
        if (
            !float.TryParse(txtYCoord.Text, out var y)
            || !float.TryParse(txtXCoord.Text, out var x)
            || !ushort.TryParse(txtRegion.Text, out ushort region)
        )
        {
            Log.Warn("[Training area] Check that the X, Y, and Region coordinates have been entered correctly.");
            return;
        }

        TrainingManager.ApplyTrainingArea(x, y, region);
    }

    /// <summary>
    /// </summary>
    private void OnLoadCharacter()
    {
        if (IsDisposed || Disposing)
            return;

        var area = Kernel.Bot.Botbase.Area;
        //Training Area
        txtXCoord.Text = area.Position.X.ToString("0.0");
        txtYCoord.Text = area.Position.Y.ToString("0.0");
        txtRadius.Text = area.Radius.ToString();
        txtRegion.Text = area.Position.Region.Id.ToString();

        RefreshTrainingPlaceList();
        SelectSavedTrainingPlace();
    }

    private void buttonSelectTrainingArea_Click(object sender, EventArgs e)
    {
        var trainingArea = new TrainingAreasDialog();
        if (trainingArea.ShowDialog(this) == DialogResult.OK)
            EventManager.FireEvent("OnSetTrainingArea");
    }

    private void Main_Load(object sender, EventArgs e)
    {
        // Built here rather than the constructor: Main's AutoScaleMode.Dpi rescales every
        // control present at scale time (which runs between construction and Load), and that
        // included these dynamically-added rows/headers when they were built in the
        // constructor - throwing off pixel sizes computed from a since-superseded Graphics
        // context/font, most visibly clipping "Berserk" to "Bers"/"Ber". Building after Load
        // means nothing scales out from under them afterward.
        BuildAvoidanceChecklist();

        _settingsLoaded = false;
        LoadSettings();
        _settingsLoaded = true;
    }
}
