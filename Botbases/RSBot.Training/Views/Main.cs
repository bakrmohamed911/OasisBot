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
    ///     The fixed set of rows shown in the Avoidance checklist: a display label plus the
    ///     <see cref="MonsterRarity" /> flag(s) it represents. "Unique" combines Unique + Unique2
    ///     into one row/flag, matching the original ListView's item tagging.
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
        InitializeCustomComponents();
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
    ///     Called when the script recorder saves a script - prompts for the Name/Level metadata
    ///     and adds it to the training-place catalog (see <see cref="PromptAndCatalogScript" />).
    /// </summary>
    private void OnSaveScript(int ownerId, string path)
    {
        if (IsDisposed || Disposing)
            return;

        if (ownerId != ScriptRecorderOwnerId)
            return;

        PromptAndCatalogScript(path);
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

    #region Avoidance checklist

    /// <summary>
    ///     Builds the Avoidance checklist: one row per <see cref="AvoidanceRarities" /> entry,
    ///     each with 3 mutually-exclusive checkboxes (Avoid/Prefer/Berserk) - replaces the old
    ///     ListView + right-click "assign to group" context menu with direct checkboxes.
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

            y += 26;
        }

        var col1X = nameX + rows.Max(r => r.NameLabel.Width) + nameToChecklistGap;
        var col2X = col1X + col1W + colGap;
        var col3X = col2X + col2W + colGap;
        // A separate, smaller Font (7.5F) for these clipped its last character even with
        // AutoSize - reusing the same ambient Font the (correctly-sized) name labels use avoids
        // whatever made the smaller size's AutoSize computation come up just short.
        var headerFont = Font;

        avoidanceListPanel.Controls.Add(CreateColumnHeader("Avoid", headerFont, col1X + col1W / 2));
        avoidanceListPanel.Controls.Add(CreateColumnHeader("Prefer", headerFont, col2X + col2W / 2));
        avoidanceListPanel.Controls.Add(CreateColumnHeader("Berserk", headerFont, col3X + col3W / 2));

        y = 22;
        foreach (var (_, rarity) in rows)
        {
            var avoidCheck = CreateRowCheckbox(col1X + (col1W - checkboxSize) / 2, y, checkboxSize, rarity);
            var preferCheck = CreateRowCheckbox(col2X + (col2W - checkboxSize) / 2, y, checkboxSize, rarity);
            var berserkCheck = CreateRowCheckbox(col3X + (col3W - checkboxSize) / 2, y, checkboxSize, rarity);

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

            y += 26;
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
        label.Location = new System.Drawing.Point(centerX - label.Width / 2, 2);

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

    #endregion Avoidance checklist

    #region Training place selection

    /// <summary>
    ///     Re-populates <see cref="lstTrainingPlaces" /> from the catalog, filtered by whatever's
    ///     currently typed in <see cref="txtSearchPlace" /> (name substring or exact level match).
    /// </summary>
    private void RefreshTrainingPlaceList()
    {
        var results = TrainingPlaceCatalog.Search(txtSearchPlace.Text).OrderBy(en => en.Level).ThenBy(en => en.Name);

        lstTrainingPlaces.BeginUpdate();
        lstTrainingPlaces.Items.Clear();
        foreach (var entry in results)
            lstTrainingPlaces.Items.Add(entry);
        lstTrainingPlaces.EndUpdate();
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

        for (var i = 0; i < lstTrainingPlaces.Items.Count; i++)
            if (lstTrainingPlaces.Items[i] is TrainingPlaceEntry entry && entry.Name == savedName)
            {
                lstTrainingPlaces.SelectedIndex = i;
                return;
            }
    }

    private void txtSearchPlace_TextChanged(object sender, EventArgs e)
    {
        RefreshTrainingPlaceList();
    }

    private void lstTrainingPlaces_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lstTrainingPlaces.SelectedItem is not TrainingPlaceEntry entry)
            return;

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

        // Reflect the route's own starting point in the Area section right away, rather than
        // leaving it showing whatever was there before - MovementBundle's own per-tick sync
        // will keep moving it to follow the player once the patrol is actually running. A patrol
        // route has no real "radius" of its own (it follows a fixed waypoint loop, not a
        // circular wander-area), so default it to a sensible value rather than leaving whatever
        // unrelated radius happened to be configured before.
        PlayerConfig.Set("RSBot.Area.Radius", DefaultTrainingPlaceRadius);

        var start = TrainingPlaceManager.CurrentWaypoint;
        TrainingManager.ApplyTrainingArea(start.X, start.Y, start.Region);
    }

    private void btnClearPlace_Click(object sender, EventArgs e)
    {
        TrainingPlaceManager.Deactivate();
        PlayerConfig.Set("RSBot.Training.SelectedPlaceScript", string.Empty);
        lstTrainingPlaces.ClearSelected();
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
            Filter = @"RSBot Bot script (*.rbs)|*.rbs",
            Title = @"Import a training place walk script",
        };

        if (diag.ShowDialog() != DialogResult.OK)
            return;

        PromptAndCatalogScript(diag.FileName);
    }

    /// <summary>
    ///     Shared save flow for both a freshly-recorded script (from <see cref="OnSaveScript" />)
    ///     and an imported one (from <see cref="btnImportScript_Click" />): prompts for Name/Level,
    ///     then adds the entry to the training-place catalog and selects it in the search results.
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
        };

        TrainingPlaceCatalog.Add(entry);

        // Clear any active filter so the newly-added entry is guaranteed to show up, then select
        // (and thereby activate) it immediately.
        txtSearchPlace.Text = string.Empty;
        RefreshTrainingPlaceList();

        var index = lstTrainingPlaces.Items.IndexOf(entry);
        if (index >= 0)
            lstTrainingPlaces.SelectedIndex = index;
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

    private void linkAttackWeakerMobsHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        MessageBox.Show(
            "If the player is under attack by a monster that is set to be avoided the bot will counter attack weaker mobs that are currently attacking the player first before targeting the avoided monster again. The bot will only kill weaker monsters that are attacking the player and won't start to pull new mobs to the battle.",
            "Attack weaker mobs first",
            MessageBoxButtons.OK,
            MessageBoxIcon.Question
        );
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

    /// <summary>
    ///    Initializes custom components that are not handled by the designer.
    /// </summary>
    private void InitializeCustomComponents()
    {
        btnUpdateNavLink.Click += async (s, e) =>
        {
            btnUpdateNavLink.Enabled = false;
            btnUpdateNavLink.Text = "Updating...";

            await Bot.NavigationManager.FetchRemoteLinkageData();

            btnUpdateNavLink.Text = "Update NavLink";
            btnUpdateNavLink.Enabled = true;
        };
    }
}
