using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using RSBot.Core;
using RSBot.Core.Client.ReferenceObjects;
using RSBot.Core.Event;
using RSBot.Core.Extensions;
using RSBot.Core.Objects.Spawn;
using SDUI.Controls;
using Label = SDUI.Controls.Label;

namespace RSBot.Views.Controls;

/// <summary>
///     Sidebar panel that shows live session statistics (EXP/SP rates, kills per monster category).
/// </summary>
public class StatisticsPanel : DoubleBufferedControl
{
    private const int RingSize = 60;
    private const int RowHeight = 17;
    private const int ValueColumnWidth = 140;

    private static readonly string[] KillCategories =
    {
        "General",
        "Champion",
        "Giant",
        "Titan",
        "Elite",
        "Elite Strong",
        "Unique",
        "Other",
    };

    private readonly Timer _timer = new() { Interval = 1000 };

    private readonly long[] _expSamples = new long[RingSize];
    private readonly long[] _spSamples = new long[RingSize];
    private readonly int[] _killSamples = new int[RingSize];

    private readonly Dictionary<string, int> _rarityCounts = new();
    private readonly Dictionary<string, Label> _rarityValueLabels = new();

    private int _expSampleIndex = -1;
    private long _lastExpValue;
    private bool _expBaselineReady;

    private int _spSampleIndex = -1;
    private uint _lastSpValue;
    private bool _spBaselineReady;

    private int _killSampleIndex = -1;
    private int _lastKillCount;
    private int _killCount;
    private int _totalKills;

    private long _totalExp;
    private long _totalSp;
    private int _contentHeight;

    private Label lblPlayerName;
    private Label lblLevelRace;
    private readonly Dictionary<string, Label> _valueLabels = new();

    public StatisticsPanel()
    {
        CheckForIllegalCrossThreadCalls = false;

        InitializeComponent();
        SubscribeEvents();

        _timer.Tick += Timer_Tick;
        _timer.Start();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _timer.Tick -= Timer_Tick;
            _timer.Stop();
            _timer.Dispose();
        }

        base.Dispose(disposing);
    }

    private void SubscribeEvents()
    {
        EventManager.SubscribeEvent("OnLoadCharacter", ResetSession);
        EventManager.SubscribeEvent("OnKillEnemy", new Action<SpawnedBionic>(OnKillEnemy));
    }

    private void InitializeComponent()
    {
        SuspendLayout();

        AutoScroll = true;
        BackColor = Color.Transparent;
        Name = "StatisticsPanel";
        Padding = new Padding(12, 6, 12, 8);
        Size = new Size(312, 100);

        var y = Padding.Top;

        lblPlayerName = CreateCaptionLabel("Player");
        lblPlayerName.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        FitLabelToText(lblPlayerName);
        lblPlayerName.Location = new Point(Padding.Left, y);
        y += 21;

        lblLevelRace = CreateCaptionLabel("-");
        lblLevelRace.Location = new Point(Padding.Left, y);
        y += RowHeight + 2;

        y = AddSection(y, "EXP STATISTICS");
        y = AddRow(y, "Exp / sec", "exp_sec");
        y = AddRow(y, "Exp / min", "exp_min");
        y = AddRow(y, "Exp / hour", "exp_hour");
        y = AddRow(y, "Total this session", "exp_total");

        y = AddSection(y, "SP STATISTICS");
        y = AddRow(y, "SP / sec", "sp_sec");
        y = AddRow(y, "SP / min", "sp_min");
        y = AddRow(y, "SP / hour", "sp_hour");
        y = AddRow(y, "Total this session", "sp_total");

        y = AddSection(y, "KILLS");
        y = AddRow(y, "Kills / sec", "kills_sec");
        y = AddRow(y, "Kills / min", "kills_min");
        y = AddRow(y, "Kills / hour", "kills_hour");
        y = AddRow(y, "Total this session", "kills_total");

        foreach (var category in KillCategories)
        {
            var valueLabel = new Label();
            y = AddRow(y, $"      {category}", valueLabel);
            valueLabel.Visible = false;
            _rarityValueLabels[category] = valueLabel;
        }

        _contentHeight = y + Padding.Bottom;
        Height = _contentHeight;

        ResumeLayout(false);
    }

    private Label CreateCaptionLabel(string text)
    {
        var label = new Label
        {
            ApplyGradient = false,
            // AutoSize fights back against any manual Width tweak (snaps back to its own
            // GetPreferredSize()), and that preferred size itself renders a character or two
            // narrower than what DrawString actually needs for some strings. Measuring and
            // sizing by hand avoids both problems.
            AutoSize = false,
            BackColor = Color.Transparent,
            ForeColor = Color.FromArgb(0, 0, 0),
            Gradient = new[] { Color.Gray, Color.Black },
            GradientAnimation = false,
        };

        Controls.Add(label);
        label.Text = text;
        FitLabelToText(label);

        return label;
    }

    /// <summary>
    ///     Resizes a non-AutoSize label to fit its current Text/Font. Call again after changing
    ///     either on a label returned by <see cref="CreateCaptionLabel" />.
    /// </summary>
    /// <remarks>
    ///     Measures with <see cref="Graphics.MeasureString(string, Font)" /> rather than
    ///     <see cref="TextRenderer.MeasureText(string, Font)" /> because the label paints with
    ///     GDI+'s <c>Graphics.DrawString</c> (see <see cref="SDUI.Controls.Label.OnPaint" />) —
    ///     GDI's text metrics run narrower/shorter than GDI+'s for the same string and font,
    ///     which clipped a trailing character (or glyph ascenders) when the two were mixed.
    /// </remarks>
    private static void FitLabelToText(Label label)
    {
        using var graphics = label.CreateGraphics();
        var measured = graphics.MeasureString(label.Text, label.Font);
        label.Size = new Size((int)Math.Ceiling(measured.Width) + 6, (int)Math.Ceiling(measured.Height) + 2);
    }

    private int AddSection(int y, string title)
    {
        var label = CreateCaptionLabel(title);
        label.Font = new Font("Segoe UI Semibold", 8.25F, FontStyle.Bold);
        FitLabelToText(label);
        label.ForeColor = Color.FromArgb(130, 130, 130);
        label.Location = new Point(Padding.Left, y);

        var separator = new Separator
        {
            Location = new Point(Padding.Left, y + 15),
            Size = new Size(ClientSize.Width - Padding.Horizontal, 6),
            Text = $"{title}Separator",
        };

        Controls.Add(separator);

        return y + 26;
    }

    private int AddRow(int y, string caption, string name)
    {
        var valueLabel = new Label();
        var rowBottom = AddRow(y, caption, valueLabel);
        _valueLabels[name] = valueLabel;

        return rowBottom;
    }

    private int AddRow(int y, string caption, Label valueLabel)
    {
        var captionLabel = CreateCaptionLabel(caption);
        captionLabel.Location = new Point(Padding.Left, y);

        valueLabel.ApplyGradient = false;
        valueLabel.AutoSize = false;
        valueLabel.BackColor = Color.Transparent;
        valueLabel.ForeColor = Color.FromArgb(0, 0, 0);
        valueLabel.Gradient = new[] { Color.Gray, Color.Black };
        valueLabel.GradientAnimation = false;
        valueLabel.Name = $"value_{caption.Trim()}";
        valueLabel.Size = new Size(ValueColumnWidth, 15);
        valueLabel.Tag = "value";
        valueLabel.Text = "0";
        valueLabel.TextAlign = ContentAlignment.MiddleRight;
        valueLabel.Top = y;
        Controls.Add(valueLabel);

        PositionValueLabel(valueLabel);

        return y + RowHeight;
    }

    private void PositionValueLabel(Label valueLabel)
    {
        valueLabel.Location = new Point(
            ClientSize.Width - Padding.Right - ValueColumnWidth,
            valueLabel.Top
        );
    }

    protected override void OnResize(EventArgs eventargs)
    {
        base.OnResize(eventargs);

        foreach (Control control in Controls)
        {
            if (control.Tag is "value")
                PositionValueLabel((Label)control);
            else if (control is Separator)
                control.Width = ClientSize.Width - Padding.Horizontal;
        }
    }

    #region Session tracking

    private void ResetSession()
    {
        Array.Clear(_expSamples, 0, _expSamples.Length);
        Array.Clear(_spSamples, 0, _spSamples.Length);
        Array.Clear(_killSamples, 0, _killSamples.Length);

        _expSampleIndex = _spSampleIndex = _killSampleIndex = -1;
        _totalExp = _totalSp = 0;
        _totalKills = _killCount = _lastKillCount = 0;
        _rarityCounts.Clear();

        // Every category row's caption is laid out up front in InitializeComponent(), reserving
        // its RowHeight regardless of whether that category has been seen yet — only the value
        // label's visibility toggles, so Height itself never needs adjusting here.
        foreach (var label in _rarityValueLabels.Values)
        {
            label.Text = "0";
            label.Visible = false;
        }

        // Don't baseline _lastExpValue/_lastSpValue here: "OnLoadCharacter" can fire before
        // Game.Player.Experience/SkillPoints are actually populated from the server, so reading
        // them right now risks baselining against 0 (or a stale value) and crediting the
        // player's entire lifetime total as "gained this session" on the very next tick.
        // SampleExperience()/SampleSkillPoints() capture the real baseline safely instead, the
        // first time they run after this reset.
        _expBaselineReady = false;
        _spBaselineReady = false;

        UpdatePlayerInfo();
        UpdateValues();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        UpdatePlayerInfo();

        if (!Game.Ready || Game.Player == null)
            return;

        SampleExperience();
        SampleSkillPoints();
        SampleKills();

        UpdateValues();
    }

    private void SampleExperience()
    {
        var currentValue = Game.Player.Experience;

        // First sample after a reset: just capture the real baseline, no delta yet — the value
        // read right after "OnLoadCharacter" fires isn't trustworthy (see ResetSession).
        if (!_expBaselineReady)
        {
            _lastExpValue = currentValue;
            _expBaselineReady = true;
            PushSample(_expSamples, ref _expSampleIndex, 0);
            return;
        }

        var delta = currentValue - _lastExpValue;

        if (delta < 0)
        {
            // Experience resets at each level up: credit whatever was missing on the previous level.
            try
            {
                var previousLevelCap = Game.ReferenceManager.GetRefLevel(Math.Max((byte)1, (byte)(Game.Player.Level - 1))).Exp_C;
                delta = Math.Max(0, previousLevelCap - _lastExpValue);
            }
            catch
            {
                delta = 0;
            }
        }

        _lastExpValue = currentValue;
        _totalExp += delta;
        PushSample(_expSamples, ref _expSampleIndex, delta);
    }

    private void SampleSkillPoints()
    {
        var currentValue = Game.Player.SkillPoints;

        if (!_spBaselineReady)
        {
            _lastSpValue = currentValue;
            _spBaselineReady = true;
            PushSample(_spSamples, ref _spSampleIndex, 0);
            return;
        }

        var delta = Math.Max(0, currentValue - _lastSpValue);

        _lastSpValue = currentValue;
        _totalSp += delta;
        PushSample(_spSamples, ref _spSampleIndex, delta);
    }

    private void SampleKills()
    {
        var delta = _killCount - _lastKillCount;

        _lastKillCount = _killCount;
        PushSample(_killSamples, ref _killSampleIndex, delta);
    }

    private static void PushSample(long[] ring, ref int index, long value)
    {
        if (++index >= ring.Length)
            index = 0;

        ring[index] = value;
    }

    private static void PushSample(int[] ring, ref int index, int value)
    {
        if (++index >= ring.Length)
            index = 0;

        ring[index] = value;
    }

    private static long Sum(long[] ring)
    {
        long sum = 0;
        foreach (var value in ring)
            sum += value;

        return sum;
    }

    private static long Sum(int[] ring)
    {
        long sum = 0;
        foreach (var value in ring)
            sum += value;

        return sum;
    }

    private static long SumLast(long[] ring, int currentIndex, int count)
    {
        long sum = 0;
        var length = ring.Length;

        for (var i = 0; i < count; i++)
            sum += ring[((currentIndex - i) % length + length) % length];

        return sum;
    }

    private static long SumLast(int[] ring, int currentIndex, int count)
    {
        long sum = 0;
        var length = ring.Length;

        for (var i = 0; i < count; i++)
            sum += ring[((currentIndex - i) % length + length) % length];

        return sum;
    }

    #endregion

    #region Kill tracking

    private void OnKillEnemy(SpawnedBionic entity)
    {
        if (entity is not SpawnedMonster monster)
            return;

        _killCount++;
        _totalKills++;

        var category = GetKillCategory(monster.Rarity.GetName());
        _rarityCounts.TryGetValue(category, out var count);
        _rarityCounts[category] = ++count;

        if (!_rarityValueLabels.TryGetValue(category, out var valueLabel))
            return;

        valueLabel.Text = count.ToString();
        valueLabel.Visible = true;
    }

    private static string GetKillCategory(string rarityName)
    {
        switch (rarityName)
        {
            case "General":
            case "General (Party)":
                return "General";

            case "Champion":
            case "Champion (Party)":
                return "Champion";

            case "Giant":
            case "Giant (Party)":
                return "Giant";

            case "Titan":
            case "Titan (Party)":
                return "Titan";

            case "Elite":
            case "Elite (Party)":
                return "Elite";

            case "Elite (Strong)":
                return "Elite Strong";

            case "Unique":
            case "Unique (Party)":
                return "Unique";

            default:
                return "Other";
        }
    }

    #endregion

    #region UI updates

    private void UpdatePlayerInfo()
    {
        if (!Game.Ready || Game.Player == null)
            return;

        lblPlayerName.Text = Game.Player.Name;
        FitLabelToText(lblPlayerName);

        lblLevelRace.Text = $"Level {Game.Player.Level}  |  {GetRaceName(Game.Player.Race)}";
        FitLabelToText(lblLevelRace);
    }

    private static string GetRaceName(ObjectCountry race)
    {
        switch (race)
        {
            case ObjectCountry.Chinese:
                return "Chinese";
            case ObjectCountry.Europe:
                return "European";
            default:
                return race.ToString();
        }
    }

    private void UpdateValues()
    {
        // Rates are based on a trailing 60 second window; the per second figure is smoothed over the last 5 seconds.
        var expPerMinute = Sum(_expSamples);
        var spPerMinute = Sum(_spSamples);
        var killsPerMinute = Sum(_killSamples);

        var expPerSecond = SumLast(_expSamples, _expSampleIndex, 5) / 5.0;
        var spPerSecond = SumLast(_spSamples, _spSampleIndex, 5) / 5.0;
        var killsPerSecond = SumLast(_killSamples, _killSampleIndex, 5) / 5.0;

        _valueLabels["exp_sec"].Text = expPerSecond.ToString("N1");
        _valueLabels["exp_min"].Text = expPerMinute.ToString("N0");
        _valueLabels["exp_hour"].Text = (expPerMinute * 60).ToString("N0");
        _valueLabels["exp_total"].Text = _totalExp.ToString("N0");

        _valueLabels["sp_sec"].Text = spPerSecond.ToString("N1");
        _valueLabels["sp_min"].Text = spPerMinute.ToString("N0");
        _valueLabels["sp_hour"].Text = (spPerMinute * 60).ToString("N0");
        _valueLabels["sp_total"].Text = _totalSp.ToString("N0");

        _valueLabels["kills_sec"].Text = killsPerSecond.ToString("N2");
        _valueLabels["kills_min"].Text = killsPerMinute.ToString("N1");
        _valueLabels["kills_hour"].Text = (killsPerMinute * 60).ToString("N1");
        _valueLabels["kills_total"].Text = _totalKills.ToString("N0");
    }

    #endregion
}
