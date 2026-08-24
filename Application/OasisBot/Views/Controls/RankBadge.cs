using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using RSBot.Core.Objects;

namespace RSBot.Views.Controls;

/// <summary>
///     Small flat vector badge standing in for a monster rank's "logo": a distinct shape + color
///     per <see cref="MonsterRarity" />, drawn directly in code (no icon files needed). Swap the
///     shape/color table in <see cref="GetBadgeStyle" /> for real artwork later if desired.
/// </summary>
public class RankBadge : Control
{
    private MonsterRarity? _rarity;

    public RankBadge()
    {
        SetStyle(
            ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor,
            true
        );
        BackColor = Color.Transparent;
        Size = new Size(18, 18);
    }

    /// <summary>
    ///     The rank to draw, or null to draw nothing (no entity selected / not a monster).
    /// </summary>
    public MonsterRarity? Rarity
    {
        get => _rarity;
        set
        {
            if (_rarity == value)
                return;

            _rarity = value;
            Invalidate();
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        if (_rarity == null)
            return;

        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        var (shape, color) = GetBadgeStyle(_rarity.Value);
        var rect = new RectangleF(1, 1, Width - 2, Height - 2);

        using var brush = new SolidBrush(color);
        using var pen = new Pen(Color.FromArgb(160, Color.Black), 1f);

        switch (shape)
        {
            case BadgeShape.Circle:
                g.FillEllipse(brush, rect);
                g.DrawEllipse(pen, rect);
                break;
            default:
                var points = shape switch
                {
                    BadgeShape.Pentagon => PolygonPoints(rect, 5),
                    BadgeShape.Hexagon => PolygonPoints(rect, 6),
                    BadgeShape.Diamond => PolygonPoints(rect, 4),
                    BadgeShape.Star => StarPoints(rect),
                    _ => PolygonPoints(rect, 4),
                };
                g.FillPolygon(brush, points);
                g.DrawPolygon(pen, points);
                break;
        }
    }

    private enum BadgeShape
    {
        Circle,
        Pentagon,
        Hexagon,
        Diamond,
        Star,
    }

    /// <summary>
    ///     Maps a rank to a (shape, color) pair. Party variants share their base rank's look -
    ///     the entity panel already shows "(Party)" via the tooltip/name text elsewhere.
    /// </summary>
    private static (BadgeShape Shape, Color Color) GetBadgeStyle(MonsterRarity rarity)
    {
        return rarity switch
        {
            MonsterRarity.General or MonsterRarity.GeneralParty => (BadgeShape.Circle, Color.Silver),
            MonsterRarity.Champion or MonsterRarity.ChampionParty => (BadgeShape.Pentagon, Color.FromArgb(205, 127, 50)),
            MonsterRarity.Giant or MonsterRarity.GiantParty => (BadgeShape.Hexagon, Color.FromArgb(150, 111, 51)),
            MonsterRarity.Titan or MonsterRarity.TitanParty => (BadgeShape.Hexagon, Color.FromArgb(139, 0, 0)),
            MonsterRarity.Elite or MonsterRarity.EliteParty => (BadgeShape.Diamond, Color.MediumPurple),
            MonsterRarity.EliteStrong => (BadgeShape.Diamond, Color.FromArgb(102, 0, 153)),
            MonsterRarity.Unique
                or MonsterRarity.UniqueParty
                or MonsterRarity.Unique2
                or MonsterRarity.Unique2Party => (BadgeShape.Star, Color.Gold),
            MonsterRarity.Event => (BadgeShape.Star, Color.Cyan),
            _ => (BadgeShape.Circle, Color.LightGray),
        };
    }

    private static PointF[] PolygonPoints(RectangleF r, int sides)
    {
        var points = new PointF[sides];
        var cx = r.Left + r.Width / 2;
        var cy = r.Top + r.Height / 2;
        var rx = r.Width / 2;
        var ry = r.Height / 2;

        for (var i = 0; i < sides; i++)
        {
            var angle = 2 * Math.PI / sides * i - Math.PI / 2;
            points[i] = new PointF(cx + rx * (float)Math.Cos(angle), cy + ry * (float)Math.Sin(angle));
        }

        return points;
    }

    private static PointF[] StarPoints(RectangleF r)
    {
        var points = new PointF[10];
        var cx = r.Left + r.Width / 2;
        var cy = r.Top + r.Height / 2;
        var outerR = r.Width / 2;
        var innerR = outerR * 0.45f;

        for (var i = 0; i < 10; i++)
        {
            var angle = Math.PI / 5 * i - Math.PI / 2;
            var radius = i % 2 == 0 ? outerR : innerR;
            points[i] = new PointF(cx + radius * (float)Math.Cos(angle), cy + radius * (float)Math.Sin(angle));
        }

        return points;
    }
}
