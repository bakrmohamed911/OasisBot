using System;
using System.Windows.Forms;
using SDUI.Controls;

namespace RSBot.Training.Views.Dialogs;

/// <summary>
///     Prompts for the Name + Level metadata used to catalog a training place (a recorded or
///     imported patrol-route script), so it can later be found again by the search box in the
///     "Select training place" section.
/// </summary>
public partial class SaveTrainingPlaceDialog : UIWindowBase
{
    public SaveTrainingPlaceDialog(string defaultName = "", int defaultLevel = 1, bool nameEditable = true)
    {
        InitializeComponent();

        PlaceName.Text = defaultName;
        // Disabled for a just-recorded script - its file (and thus this name) was already
        // chosen once in the recorder's own Save dialog; letting it be edited here again would
        // just desync the catalog Name from the actual file name without renaming the file.
        PlaceName.Enabled = nameEditable;
        Level.Value = Math.Clamp(defaultLevel, (int)Level.Minimum, (int)Level.Maximum);
    }

    private void buttonAccept_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(PlaceName.Text))
            DialogResult = DialogResult.Retry;
    }

    private void SaveTrainingPlaceDialog_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (DialogResult == DialogResult.Retry)
            e.Cancel = true;
    }
}
