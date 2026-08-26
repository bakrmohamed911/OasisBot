using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using RSBot.Core.Event;
using RSBot.Core.Objects;
using SDUI.Controls;

namespace RSBot.Chat.Views;

[ToolboxItem(false)]
public partial class Main : DoubleBufferedControl
{
    public Main()
    {
        InitializeComponent();
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        EventManager.SubscribeEvent("OnMessageReceived", AppendMessage);
    }

    /// <summary>
    ///     Sends the chat message.
    /// </summary>
    /// <param name="sender">The sender.</param>
    private void SendChatMessage(Control sender)
    {
        if (!Enum.TryParse<ChatType>(sender.Tag.ToString(), out var chatType))
            return;

        ChatManager.Send(chatType, sender.Text, txtRecievePrivate.Text);
    }

    /// <summary>
    ///     Appends the message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="sender">The sender.</param>
    /// <param name="type">The type.</param>
    public void AppendMessage(string message, string sender, ChatType type)
    {
        if (this.InvokeRequired)
        {
            // BeginInvoke, not blocking Invoke: this is dispatched off a fire-and-forget
            // Task.Run by EventManager.FireEvent for every chat packet, so nothing waits on
            // synchronous completion - blocking here just ties up a fresh ThreadPool worker
            // per message until the UI thread gets to it. A burst of chat activity (party/
            // guild/world spam) could otherwise pile up one permanently-blocked thread per
            // message with no bound - the same failure mode confirmed live (via a frozen-
            // process dump) in RSBot.Skills' buff handlers, fixed the same way there.
            if (IsHandleCreated)
                this.BeginInvoke(new Action<string, string, ChatType>(AppendMessage), message, sender, type);
            return;
        }

        message = $"({sender}): {message}";

        switch (type)
        {
            case ChatType.Academy:
                txtAcademy.Write(message);
                break;

            case ChatType.All:
                txtAll.Write(message);
                break;

            case ChatType.AllGM:
                txtAll.Write(message);
                break;

            case ChatType.Global:
                txtGlobal.Write(message);
                break;

            case ChatType.Guild:
                txtGuild.Write(message);
                break;

            case ChatType.Notice:
                txtGlobal.Write(message);
                break;

            case ChatType.Npc:
                txtAll.Write(message);
                break;

            case ChatType.Party:
                txtParty.Write(message);
                break;

            case ChatType.Private:
                txtPrivate.Write(message);
                break;

            case ChatType.Union:
                txtUnion.Write(message);
                break;

            case ChatType.Stall:
                txtStall.Write(message);
                break;
        }
    }

    /// <summary>
    ///     Messages the preview key down.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="PreviewKeyDownEventArgs" /> instance containing the event data.</param>
    private void MessagePreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
    {
        if (e.KeyCode != Keys.Enter)
            return;
        SendChatMessage((Control)sender);
        ((Control)sender).ResetText();
    }
}
