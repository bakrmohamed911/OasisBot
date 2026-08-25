using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using RSBot.Alchemy.Bot;
using RSBot.Alchemy.Bundle.Attribute;
using RSBot.Alchemy.Helper;
using RSBot.Core;
using RSBot.Core.Event;
using RSBot.Core.Objects;
using SDUI.Controls;

namespace RSBot.Alchemy.Views.Settings;

[ToolboxItem(false)]
public partial class AttributesSettingsView : DoubleBufferedControl
{
    private List<AttributeInfoPanel> _attributePanels;

    public AttributesSettingsView()
    {
        InitializeComponent();

        CheckForIllegalCrossThreadCalls = false;

        EventManager.SubscribeEvent("OnEnterGame", SubscribeMainFormEvents);
    }

    internal AttributeBundleConfig BundleConfig
    {
        get
        {
            var result = new AttributeBundleConfig { Item = SelectedItem };

            var attributes = new List<AttributeBundleConfig.AttributeBundleConfigItem>(10);

            foreach (var attributePanel in _attributePanels)
            {
                if (!attributePanel.Checked)
                    continue;

                if (attributePanel.Stones != null && attributePanel.Stones.Any())
                    attributes.Add(
                        new AttributeBundleConfig.AttributeBundleConfigItem
                        {
                            Group = attributePanel.AttributeGroup,
                            MaxValue = attributePanel.MaxValue,
                            Stone = attributePanel.Stones.First(),
                        }
                    );
            }

            result.Attributes = attributes;

            return result;
        }
    }

    private InventoryItem? SelectedItem { get; set; }

    /// <summary>
    ///     Subscribes to the ItemChanged event
    /// </summary>
    private void SubscribeMainFormEvents()
    {
        if (Globals.View != null)
        {
            Globals.View.ItemChanged += View_ItemChanged;
            Globals.View.EngineChanged += View_EngineChanged;
        }
    }

    public void PopulateView()
    {
        try
        {
            _attributePanels = new List<AttributeInfoPanel>(6);
            var selectedItem = Globals.View.SelectedItem;

            if (selectedItem == null)
                return;

            var availableItemAttributes = ItemAttributesInfo.GetAvailableAttributeGroupsForItem(selectedItem.Record);

            if (availableItemAttributes == null)
            {
                Log.Error(
                    $"[Alchemy] Could not identify the selected item's attribute information. [ItemId = {selectedItem.ItemId}]"
                );

                return;
            }

            if (Globals.Botbase.AttributeBundleConfig == null)
                Globals.Botbase.AttributeBundleConfig = BundleConfig;

            foreach (var attributeGroup in availableItemAttributes)
            {
                var matchingStones = AlchemyItemHelper.GetAttributeStones(selectedItem, attributeGroup);

                var config = Globals.Botbase.AttributeBundleConfig.Attributes.FirstOrDefault(x =>
                    x.Group == attributeGroup
                );

                var panel = new AttributeInfoPanel(
                    attributeGroup,
                    matchingStones,
                    selectedItem,
                    config == null ? 0 : config.MaxValue
                )
                {
                    Dock = DockStyle.Top,
                };
                _attributePanels.Add(panel);

                if (config == null || !matchingStones.Any())
                {
                    panel.OnChange += PanelOnChange;

                    continue;
                }

                panel.Checked = true;
                panel.OnChange += PanelOnChange;
            }

            BeginInvoke(() =>
            {
                Hide();

                // Controls.Clear() only detaches the outgoing AttributeInfoPanel controls from
                // this container - it does NOT destroy their native window handles (nor their
                // children's, including each panel's own ToolTip component). This runs on every
                // single automated alchemy action (wired to Globals.View.ItemChanged, fired
                // unconditionally from OnAlchemy regardless of which engine is active), so over
                // a long unattended session that leaked several HWNDs per action - enough to
                // eventually exhaust the process's Windows USER-object quota and start throwing
                // "Error creating window handle" everywhere, including in unrelated UI (Log/Chat).
                // Snapshot to an array first - Dispose() removes the control from Controls as a
                // side effect, which would otherwise invalidate a foreach over Controls directly.
                foreach (Control control in Controls.Cast<Control>().ToArray())
                    control.Dispose();

                Controls.Clear();
                Controls.AddRange(_attributePanels.ToArray());
                Show();
            });

            Globals.Botbase.AttributeBundleConfig = BundleConfig;
        }
        catch (Exception ex)
        {
            Log.Debug($"[Alchemy] Error refreshing attributes view: {ex.Message}");
        }
    }

    private void PanelOnChange(bool @checked, int maxValue)
    {
        Globals.Botbase.AttributeBundleConfig = BundleConfig;
    }

    private void View_EngineChanged(InventoryItem item, AlchemyEngine alchemyEngine)
    {
        PopulateView();
    }

    /// <summary>
    ///     Will be triggered when the selected item changed
    /// </summary>
    /// <param name="item"></param>
    private void View_ItemChanged(InventoryItem item)
    {
        SelectedItem = item;
        PopulateView();
    }
}
