using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using RSBot.Core;
using RSBot.Core.Client;
using RSBot.Core.Components;
using RSBot.Core.Event;
using RSBot.Core.Objects.Skill;
using RSBot.General.Components;
using RSBot.General.Models;
using RSBot.Protection.Components.Player;
using RSBot.Skills;
using SDUI.Controls;

namespace RSBot.General.Views;

[ToolboxItem(false)]
internal partial class Main : DoubleBufferedControl
{
    private bool _clientVisible;
    private static int _reloginSeq;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Main" /> class.
    /// </summary>
    public Main()
    {
        //CheckForIllegalCrossThreadCalls = false;

        InitializeComponent();
        SubscribeEvents();
    }

    /// <summary>
    ///     Subscribes the events.
    /// </summary>
    private void SubscribeEvents()
    {
        EventManager.SubscribeEvent("OnLoadVersionInfo", new Action<VersionInfo>(OnLoadVersionInfo));
        EventManager.SubscribeEvent("OnClientConnected", OnClientConnected);
        EventManager.SubscribeEvent("OnStartClient", OnStartClient);
        EventManager.SubscribeEvent("OnCharacterListReceived", OnCharacterListReceived);
        EventManager.SubscribeEvent("OnInitialized", OnInitialized);
        EventManager.SubscribeEvent("OnAutoLoginAborted", OnAutoLoginAborted);
        EventManager.SubscribeEvent("OnSwitchToClientless", OnSwitchToClientless);
        EventManager.SubscribeEvent("OnAutoReloginStarted", OnAutoReloginStarted);
        EventManager.SubscribeEvent("OnClientDisconnected", OnClientDisconnected);
        EventManager.SubscribeEvent("OnAutoReloginOngoing", OnAutoReloginOngoing);
        EventManager.SubscribeEvent("OnEnterGame", OnEnterGame);
        EventManager.SubscribeEvent("OnExitClient", OnExitClient);
        EventManager.SubscribeEvent("OnClientProcessStarted", OnClientProcessStarted);
        EventManager.SubscribeEvent("OnClientlessProcessStarted", OnClientlessProcessStarted);

        EventManager.SubscribeEvent("OnLoadCharacter", OnLoadCharacterRefreshSkillSections);
        EventManager.SubscribeEvent("OnSkillLearned", new Action<SkillInfo>(OnSkillLearned));
        EventManager.SubscribeEvent("OnSkillUpgraded", new Action<SkillInfo, SkillInfo>(OnSkillChanged));
        EventManager.SubscribeEvent("OnWithdrawSkill", new Action<SkillInfo, SkillInfo>(OnSkillChanged));
        EventManager.SubscribeEvent("OnLearnSkillMastery", new Action<MasteryInfo>(OnLearnSkillMastery));

        EventManager.SubscribeEvent("OnIncreaseStrength", OnIncreaseStat);
        EventManager.SubscribeEvent("OnIncreaseIntelligence", OnIncreaseStat);

        EventManager.SubscribeEvent("OnTogglePendingWindowRequested", OnTogglePendingWindowRequested);
    }

    /// <summary>
    ///     Handles a request (fired from the Extras tab's "Toggle Pending Window" button) to
    ///     show/hide the pending window that is owned & managed by the automated-login flow here.
    /// </summary>
    private void OnTogglePendingWindowRequested()
    {
        if (this.InvokeRequired)
        {
            this.Invoke(new Action(OnTogglePendingWindowRequested));
            return;
        }
        if (!AutoLogin.Pending)
            return;

        if (View.PendingWindow?.Visible == false)
            View.PendingWindow.ShowAtTop(View.Instance);
        else
            View.PendingWindow.Hide();
    }

    /// <summary>
    ///     Refreshes the resurrection skill & mastery combo boxes moved in from the Skills tab.
    /// </summary>
    private void RefreshSkillSections()
    {
        if (Game.Player == null)
            return;

        LoadResurrectionSkills();
        LoadMasteries();
    }

    private void OnLoadCharacterRefreshSkillSections()
    {
        if (this.InvokeRequired)
        {
            this.Invoke(new Action(OnLoadCharacterRefreshSkillSections));
            return;
        }
        RefreshSkillSections();
    }

    private void OnSkillLearned(SkillInfo learnedSkill)
    {
        if (this.InvokeRequired)
        {
            this.Invoke(new Action<SkillInfo>(OnSkillLearned), learnedSkill);
            return;
        }
        RefreshSkillSections();
    }

    private void OnSkillChanged(SkillInfo oldSkill, SkillInfo newSkill)
    {
        if (this.InvokeRequired)
        {
            this.Invoke(new Action<SkillInfo, SkillInfo>(OnSkillChanged), oldSkill, newSkill);
            return;
        }
        RefreshSkillSections();
    }

    private void OnLearnSkillMastery(MasteryInfo info)
    {
        if (this.InvokeRequired)
        {
            this.Invoke(new Action<MasteryInfo>(OnLearnSkillMastery), info);
            return;
        }
        RefreshSkillSections();
    }

    /// <summary>
    ///     Called when main window loaded.
    /// </summary>
    private void OnInitialized()
    {
        if (this.InvokeRequired)
        {
            this.Invoke(new Action(OnInitialized));
            return;
        }
        comboBoxClientType.Items.AddRange(Enum.GetNames(typeof(GameClientType)));
        comboCharacter.SelectedIndex = 0;

        Accounts.Load();
        LoadAccounts();

        //Load and display config

        txtSilkroadPath.Text = Path.Combine(
            GlobalConfig.Get<string>("RSBot.SilkroadDirectory"),
            GlobalConfig.Get<string>("RSBot.SilkroadExecutable")
        );
        checkEnableStaticCaptcha.Checked = GlobalConfig.Get<bool>("RSBot.General.EnableStaticCaptcha");
        checkEnableAutoLogin.Checked = GlobalConfig.Get<bool>("RSBot.General.EnableAutomatedLogin");
        checkStartBot.Checked = GlobalConfig.Get<bool>("RSBot.General.StartBot");
        checkUseReturnScroll.Checked = GlobalConfig.Get<bool>("RSBot.General.UseReturnScroll");
        txtStaticCaptcha.Text = GlobalConfig.Get<string>("RSBot.General.StaticCaptcha");
        checkEnableLoginDelay.Checked = GlobalConfig.Get<bool>("RSBot.General.EnableLoginDelay");
        numLoginDelay.Value = GlobalConfig.Get("RSBot.General.LoginDelay", 3);
        checkWaitAfterDC.Checked = GlobalConfig.Get<bool>("RSBot.General.EnableWaitAfterDC");
        numWaitAfterDC.Value = GlobalConfig.Get("RSBot.General.WaitAfterDC", 3);
        checkHideClient.Checked = GlobalConfig.Get<bool>("RSBot.General.HideOnStartClient");
        checkCharAutoSelect.Checked = GlobalConfig.Get<bool>("RSBot.General.CharacterAutoSelect");
        radioAutoSelectFirst.Checked = GlobalConfig.Get<bool>("RSBot.General.CharacterAutoSelectFirst", true);
        radioAutoSelectHigher.Checked = GlobalConfig.Get<bool>("RSBot.General.CharacterAutoSelectHigher");

        checkAcceptResurrection.Checked = PlayerConfig.Get("RSBot.Skills.checkAcceptResurrection", true);
        checkResurrectParty.Checked = PlayerConfig.Get<bool>("RSBot.Skills.checkResurrectParty");
        numResDelay.Value = PlayerConfig.Get("RSBot.Skills.numResDelay", 120);
        numResRadius.Value = PlayerConfig.Get("RSBot.Skills.numResRadius", 100);

        checkLearnMastery.Checked = PlayerConfig.Get<bool>("RSBot.Skills.checkLearnMastery");
        checkLearnMasteryBotStopped.Checked = PlayerConfig.Get<bool>("RSBot.Skills.checkLearnMasteryBotStopped");
        numMasteryGap.Value = PlayerConfig.Get("RSBot.Skills.numMasteryGap", 0);

        checkIncInt.Checked = PlayerConfig.Get<bool>("RSBot.Protection.checkIncInt");
        checkIncStr.Checked = PlayerConfig.Get<bool>("RSBot.Protection.checkIncStr");
        checkIncBotStopped.Checked = PlayerConfig.Get("RSBot.Protection.checkIncBotStopped", true);
        numIncInt.Value = PlayerConfig.Get("RSBot.Protection.numIncInt", 0);
        numIncStr.Value = PlayerConfig.Get("RSBot.Protection.numIncStr", 0);
        numIncStr.Maximum = 3 - numIncInt.Value;
        numIncInt.Maximum = 3 - numIncStr.Value;

        if (GlobalConfig.Get<bool>("RSBot.General.CharacterAutoSelect"))
        {
            radioAutoSelectFirst.Enabled = true;
            radioAutoSelectHigher.Enabled = true;
        }

        comboBoxClientType.SelectedIndex = (int)Game.ClientType;

        if (!File.Exists(GlobalConfig.Get<string>("RSBot.SilkroadDirectory") + "\\media.pk2"))
            txtSilkroadPath.BackColor = Color.Red;

        if (!string.IsNullOrEmpty(Kernel.LaunchMode))
        {
            Task.Run(async () =>
            {
                await Task.Delay(5000);
                if (Kernel.LaunchMode == "client")
                {
                    BeginInvoke(
                        new Action(() =>
                        {
                            btnStartClient_Click(btnStartClient, EventArgs.Empty);
                        })
                    );
                }
                else if (Kernel.LaunchMode == "clientless")
                {
                    BeginInvoke(
                        new Action(() =>
                        {
                            btnStartClientless_Click(btnStartClientless, EventArgs.Empty);
                        })
                    );
                }
            });
        }
    }

    /// <summary>
    ///     Called when account character list updated
    /// </summary>
    private void OnCharacterListReceived()
    {
        LoadAccounts();
    }

    /// <summary>
    ///     Loads the accounts.
    /// </summary>
    private void LoadAccounts()
    {
        comboAccounts.Items.Clear();
        comboAccounts.Items.Add(LanguageManager.GetLang("NotSelected"));

        var autoLoginUserName = GlobalConfig.Get<string>("RSBot.General.AutoLoginAccountUsername");
        foreach (var account in Accounts.SavedAccounts)
        {
            var index = comboAccounts.Items.Add(account);
            if (account.Username == autoLoginUserName)
                comboAccounts.SelectedIndex = index;
        }

        if (comboAccounts.SelectedIndex == -1)
            comboAccounts.SelectedIndex = 0;
    }

    /// <summary>
    ///     Fill the combobox on the form
    /// </summary>
    private void LoadCharacters()
    {
        comboCharacter.Items.Clear();
        comboCharacter.Items.Add(LanguageManager.GetLang("NotSelected"));

        var selectedAccount = comboAccounts.SelectedItem as Account;
        if (selectedAccount?.Characters == null)
        {
            comboCharacter.SelectedIndex = 0;
            return;
        }

        foreach (var character in selectedAccount.Characters.Where(n => n != null))
        {
            var index = comboCharacter.Items.Add(character);
            if (character == selectedAccount.SelectedCharacter)
                comboCharacter.SelectedIndex = index;
        }

        if (comboCharacter.SelectedIndex == -1 || string.IsNullOrWhiteSpace(selectedAccount.SelectedCharacter))
            comboCharacter.SelectedIndex = 0;
    }

    /// <summary>
    ///     Called when [start client].
    /// </summary>
    private void OnStartClient()
    {
        if (this.InvokeRequired)
        {
            this.Invoke(new Action(OnStartClient));
            return;
        }
        btnStartClient.Enabled = false;
        btnStartClientless.Enabled = false;
        _clientVisible = true;
        btnClientHideShow.Enabled = true;

        if (GlobalConfig.Get<bool>("RSBot.General.HideOnStartClient"))
            ClientManager.SetVisible(false);
    }

    /// <summary>
    ///     Called when [exit client].
    /// </summary>
    private void OnExitClient()
    {
        if (this.InvokeRequired)
        {
            this.Invoke(new Action(OnExitClient));
            return;
        }
        btnStartClient.Text = LanguageManager.GetLang("Start") + " Client";

        if (GeneralManager.IsClientless)
            return;

        btnStartClient.Enabled = true;
        btnStartClientless.Enabled = true;
        btnClientHideShow.Enabled = false;
    }

    /// <summary>
    ///     The on load version information.
    /// </summary>
    /// <param name="info">The information.</param>
    private void OnLoadVersionInfo(VersionInfo info)
    {
        lblVersion.Text = "v" + ((1000f + info.Version) / 1000f).ToString("0.000", CultureInfo.InvariantCulture);
    }

    /// <summary>
    ///     Called when [client connected].
    /// </summary>
    private void OnClientConnected()
    {
        if (this.InvokeRequired)
        {
            this.Invoke(new Action(OnClientConnected));
            return;
        }
        btnStartClientless.Enabled = false;
    }

    private void OnSwitchToClientless()
    {
        if (this.InvokeRequired)
        {
            this.Invoke(new Action(OnSwitchToClientless));
            return;
        }
        btnStartClientless.Text = LanguageManager.GetLang("Disconnect");
        btnGoClientless.Enabled = false;
        btnStartClient.Enabled = true;
        btnStartClientless.Enabled = true;
        btnClientHideShow.Enabled = false;
    }

    #region LogicEvents
    private void OnAutoLoginAborted()
    {
        if (this.InvokeRequired)
        {
            this.Invoke(new Action(OnAutoLoginAborted));
            return;
        }
        View.PendingWindow?.Hide();
        View.PendingWindow?.StopClientlessQueueTask();
    }

    private void OnAutoReloginStarted()
    {
        if (this.InvokeRequired)
        {
            this.Invoke(new Action(OnAutoReloginStarted));
            return;
        }
        btnStartClient.Enabled = false;
        btnStartClientless.Enabled = false;
    }

    private void OnClientDisconnected()
    {
        if (this.InvokeRequired)
        {
            this.Invoke(new Action(OnClientDisconnected));
            return;
        }

        btnGoClientless.Enabled = false;
        btnStartClient.Enabled = true;
        btnStartClientless.Enabled = true;

        btnStartClient.Text = LanguageManager.GetLang("Start") + " Client";
        btnStartClientless.Text = LanguageManager.GetLang("Start") + " Clientless";
    }

    private void OnAutoReloginOngoing()
    {
        if (this.InvokeRequired)
        {
            this.Invoke(new Action(OnAutoReloginOngoing));
            return;
        }

        btnStartClient.Enabled = true;
        btnStartClientless.Enabled = true;
        btnStartClientless.Text = LanguageManager.GetLang("Start") + " Clientless";
    }

    private void OnClientProcessStarted()
    {
        if (this.InvokeRequired)
        {
            this.Invoke(new Action(OnClientProcessStarted));
            return;
        }

        btnStartClient.Enabled = false;
    }

    private void OnClientlessProcessStarted()
    {
        if (this.InvokeRequired)
        {
            this.Invoke(new Action(OnClientlessProcessStarted));
            return;
        }

        btnStartClientless.Text = LanguageManager.GetLang("Disconnect");
    }

    private void OnEnterGame()
    {
        if (this.InvokeRequired)
        {
            this.Invoke(new Action(OnEnterGame));
            return;
        }
        if (!Game.Clientless)
        {
            btnClientHideShow.Enabled = true;
            btnClientHideShow.Text = LanguageManager.GetLang("Hide") + " Client";
            btnStartClient.Enabled = true;
            btnStartClient.Text = LanguageManager.GetLang("Kill") + " Client";
            btnGoClientless.Enabled = true;
        }
    }
    #endregion

    /// <summary>
    ///     Handles the Click event of the btnBrowseSilkroadPath control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private void btnBrowseSilkroadPath_Click(object sender, EventArgs e)
    {
        using (var dialog = new OpenFileDialog())
        {
            var title = LanguageManager.GetLang("BrowseSilkroadPathDialogTitle");

            var msgBoxTitle = LanguageManager.GetLang("BrowseSilkroadPathMsgBoxTitle");
            var msgBoxContent = LanguageManager.GetLang("BrowseSilkroadPathMsgBoxContent");

            dialog.Title = title;
            dialog.Filter = "App (*.exe)|*.exe";
            dialog.FileName = "sro_client.exe";

            var result = dialog.ShowDialog();
            if (result != DialogResult.OK)
                return;

            GeneralManager.ChangeSilkroadPath(dialog.FileName);

            txtSilkroadPath.Text = dialog.FileName;

            result = MessageBox.Show(msgBoxContent, msgBoxTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            GlobalConfig.Save();

            if (result == DialogResult.Yes)
            {
                Application.Restart();
            }
        }
    }

    /// <summary>
    ///     Handles the CheckedChanged event of the checkStartBot control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private void checkAutoStartBot_CheckedChanged(object sender, EventArgs e)
    {
        GlobalConfig.Set("RSBot.General.StartBot", checkStartBot.Checked);
    }

    /// <summary>
    ///     Handles the CheckedChanged event of the checkUseReturnScroll control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private void checkUseReturnScroll_CheckedChanged(object sender, EventArgs e)
    {
        GlobalConfig.Set("RSBot.General.UseReturnScroll", checkUseReturnScroll.Checked);
    }

    /// <summary>
    ///     Handles the Click event of the btnAutoLoginSettings control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private void btnAutoLoginSettings_Click(object sender, EventArgs e)
    {
        if (View.AccountsWindow.ShowDialog() == DialogResult.OK)
            LoadAccounts();
    }

    /// <summary>
    ///     Handles the CheckedChanged event of the checkEnableAutoLogin control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private void checkEnableAutoLogin_CheckedChanged(object sender, EventArgs e)
    {
        GlobalConfig.Set("RSBot.General.EnableAutomatedLogin", checkEnableAutoLogin.Checked.ToString());
    }

    /// <summary>
    ///     Handles the SelectedIndexChanged event of the comboAccounts control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private void comboAccounts_SelectedIndexChanged(object sender, EventArgs e)
    {
        var selectedAccount = comboAccounts.SelectedIndex == 0 ? string.Empty : comboAccounts.SelectedItem.ToString();

        GlobalConfig.Set("RSBot.General.AutoLoginAccountUsername", selectedAccount);

        LoadCharacters();
    }

    /// <summary>
    ///     Handles the TextChanged event of the txtStaticCaptcha control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private void txtStaticCaptcha_TextChanged(object sender, EventArgs e)
    {
        GlobalConfig.Set("RSBot.General.StaticCaptcha", txtStaticCaptcha.Text);
    }

    /// <summary>
    ///     Handles the CheckedChanged event of the checkEnableStaticCaptcha control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private void checkEnableStaticCaptcha_CheckedChanged(object sender, EventArgs e)
    {
        GlobalConfig.Set("RSBot.General.EnableStaticCaptcha", checkEnableStaticCaptcha.Checked.ToString());
    }

    /// <summary>
    ///     Handles the SelectedIndexChanged event of the comboCharacter control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private void comboCharacter_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (comboAccounts.SelectedIndex == 0)
            return;

        checkCharAutoSelect.Enabled = comboCharacter.SelectedIndex == 0;

        var selectedAccount = comboAccounts.SelectedItem as Account;
        if (selectedAccount == null)
            return;

        selectedAccount.SelectedCharacter =
            comboCharacter.SelectedIndex == 0 ? string.Empty : comboCharacter.SelectedItem.ToString();

        Accounts.Save();
    }

    /// <summary>
    ///     Handles the Click event of the btnGoClientless control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private void btnGoClientless_Click(object sender, EventArgs e)
    {
        if (Game.Clientless)
            return;
        var msgBoxTitle = LanguageManager.GetLang("GoClientlessMsgBoxTitle");
        var msgBoxContent = LanguageManager.GetLang("GoClientlessMsgBoxContent");

        if (
            MessageBox.Show(msgBoxContent, msgBoxTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
            != DialogResult.Yes
        )
            return;
        GeneralManager.GoClientless();
    }

    /// <summary>
    ///     Handles the Click event of the btnStartClientless control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private async void btnStartClientless_Click(object sender, EventArgs e)
    {
        await Task.Run(async () =>
        {
            if (!Game.Clientless)
            {
                if (!checkEnableAutoLogin.Checked || comboAccounts.SelectedIndex <= 0)
                {
                    var msgBoxTitle = LanguageManager.GetLang("StartClientlessMsgBoxTitle");
                    var msgBoxContent = LanguageManager.GetLang("StartClientlessMsgBoxContent");

                    MessageBox.Show(msgBoxContent, msgBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                btnStartClient.Enabled = false;
                btnClientHideShow.Enabled = false;

                await GeneralManager.StartClientlessAsync();
            }
            else
            {
                var msgBoxTitle = LanguageManager.GetLang("MsgBoxDisconnectDialogTitle");
                var msgBoxContent = LanguageManager.GetLang("MsgBoxDisconnectDialogContent");

                var result = MessageBox.Show(
                    msgBoxContent,
                    msgBoxTitle,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );
                if (result == DialogResult.No)
                    return;

                await GeneralManager.DisconnectAsync();
            }
        });
    }

    /// <summary>
    ///     Handles the Click event of the btnStartClient control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private async void btnStartClient_Click(object sender, EventArgs e)
    {
        if (!GeneralManager.IsClientless && GeneralManager.IsConnected)
        {
            var extraStr = LanguageManager.GetLang("KillClientWarnMsgBoxSplit1");
            if (!GlobalConfig.Get<bool>("RSBot.General.StayConnected"))
                extraStr = LanguageManager.GetLang("KillClientWarnMsgBoxSplit2");

            var title = LanguageManager.GetLang("Warning");
            var content = LanguageManager.GetLang("KillClientWarnMsgBoxContent", extraStr);

            if (MessageBox.Show(content, title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                GeneralManager.KillClient();
            return;
        }
        if (GeneralManager.IsConnected)
            return;
        await GeneralPlugin.Instance.Manager.StartClientAsync();
    }

    private void btnClientHideShow_Click(object sender, EventArgs e)
    {
        if (!ClientManager.IsRunning)
            return;

        if (!_clientVisible)
        {
            _clientVisible = true;
            ClientManager.SetVisible(true);
            btnClientHideShow.Text = LanguageManager.GetLang("Hide") + " Client";
        }
        else
        {
            _clientVisible = false;
            ClientManager.SetVisible(false);
            btnClientHideShow.Text = LanguageManager.GetLang("Show") + " Client";
        }
    }

    /// <summary>
    ///     Handles the SelectedIndexChanged event of the comboBoxClientType control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private void comboBoxClientType_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Created from Activator.CreateInstance easy fix ^^
        if (comboBoxClientType.Parent.Parent == null)
            return;

        if (Game.Player != null)
        {
            MessageBox.Show(LanguageManager.GetLang("MsgBoxClientTypeWarn"));
            return;
        }

        var clientType = (GameClientType)comboBoxClientType.SelectedIndex;

        GlobalConfig.Set("RSBot.Game.ClientType", clientType);
        Game.ClientType = clientType;
        GlobalConfig.Save();

        if (clientType.ToString().StartsWith("Vietnam"))
            captchaPanel.Visible = true;
        else
            captchaPanel.Visible = false;
    }

    /// <summary>
    ///     Handles the CheckedChanged event of the checkEnableLoginDelay control.
    /// </summary>
    /// <param name="sender">
    ///     The source of the event.
    /// </param>
    /// <param name="e">
    ///     The <see cref="EventArgs" /> instance containing the event data.
    /// </param>
    private void checkEnableLoginDelay_CheckedChanged(object sender, EventArgs e)
    {
        GlobalConfig.Set("RSBot.General.EnableLoginDelay", checkEnableLoginDelay.Checked);
    }

    /// <summary>
    ///     Handles the ValueChanged event of the numLoginDelay control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private void numLoginDelay_ValueChanged(object sender, EventArgs e)
    {
        GlobalConfig.Set("RSBot.General.LoginDelay", numLoginDelay.Value);
    }

    /// <summary>
    ///     Handles the CheckedChanged event of the checkWaitAfterDC control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private void checkWaitAfterDC_CheckedChanged(object sender, EventArgs e)
    {
        GlobalConfig.Set("RSBot.General.EnableWaitAfterDC", checkWaitAfterDC.Checked);
    }

    /// <summary>
    ///     Handles the ValueChanged event of the numWaitAfterDC control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private void numWaitAfterDC_ValueChanged(object sender, EventArgs e)
    {
        GlobalConfig.Set("RSBot.General.WaitAfterDC", numWaitAfterDC.Value);
    }

    /// <summary>
    ///     Handles the CheckedChanged event of the checkHideClient control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void checkHideClient_CheckedChanged(object sender, EventArgs e)
    {
        GlobalConfig.Set("RSBot.General.HideOnStartClient", checkHideClient.Checked);
    }

    /// <summary>
    ///     Handles the CheckedChanged event of the checkCharAutoSelect control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void checkCharAutoSelect_CheckedChanged(object sender, EventArgs e)
    {
        if (!GlobalConfig.Get<bool>("RSBot.General.CharacterAutoSelect"))
        {
            radioAutoSelectFirst.Enabled = true;
            radioAutoSelectHigher.Enabled = true;
        }
        else
        {
            radioAutoSelectFirst.Enabled = false;
            radioAutoSelectHigher.Enabled = false;
        }

        GlobalConfig.Set("RSBot.General.CharacterAutoSelect", checkCharAutoSelect.Checked);
    }

    /// <summary>
    ///     Handles the CheckedChanged event of the radioAutoSelectFirst control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void radioAutoSelectFirst_CheckedChanged(object sender, EventArgs e)
    {
        GlobalConfig.Set("RSBot.General.CharacterAutoSelectFirst", radioAutoSelectFirst.Checked);
    }

    /// <summary>
    ///     Handles the CheckedChanged event of the radioAutoSelectHigher control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void radioAutoSelectHigher_CheckedChanged(object sender, EventArgs e)
    {
        GlobalConfig.Set("RSBot.General.CharacterAutoSelectHigher", radioAutoSelectHigher.Checked);
    }

    /// <summary>
    ///     Handles the SelectedIndexChanged event of the comboResurrectionSkill control.
    /// </summary>
    private void comboResurrectionSkill_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (comboResurrectionSkill.SelectedIndex < 0)
            return;

        SkillInfo skill = null;
        if (comboResurrectionSkill.SelectedIndex > 0)
            skill = comboResurrectionSkill.SelectedItem as SkillInfo;

        SkillsManager.SetResurrectionSkill(skill);
    }

    /// <summary>
    ///     Handles the CheckedChanged event of the checkAcceptResurrection control.
    /// </summary>
    private void checkAcceptResurrection_CheckedChanged(object sender, EventArgs e)
    {
        PlayerConfig.Set("RSBot.Skills.checkAcceptResurrection", checkAcceptResurrection.Checked);
    }

    /// <summary>
    ///     Handles the CheckedChanged event of the checkResurrectParty control.
    /// </summary>
    private void checkResurrectParty_CheckedChanged(object sender, EventArgs e)
    {
        PlayerConfig.Set("RSBot.Skills.checkResurrectParty", checkResurrectParty.Checked);
    }

    /// <summary>
    ///     Handles the ValueChanged event of the numResDelay control.
    /// </summary>
    private void numResDelay_ValueChanged(object sender, EventArgs e)
    {
        PlayerConfig.Set("RSBot.Skills.numResDelay", numResDelay.Value);
    }

    /// <summary>
    ///     Handles the ValueChanged event of the numResRadius control.
    /// </summary>
    private void numResRadius_ValueChanged(object sender, EventArgs e)
    {
        PlayerConfig.Set("RSBot.Skills.numResRadius", numResRadius.Value);
    }

    /// <summary>
    ///     Handles the CheckedChanged event of the checkLearnMastery control.
    /// </summary>
    private void checkLearnMastery_CheckedChanged(object sender, EventArgs e)
    {
        PlayerConfig.Set("RSBot.Skills.checkLearnMastery", checkLearnMastery.Checked);
    }

    /// <summary>
    ///     Handles the CheckedChanged event of the checkLearnMasteryBotStopped control.
    /// </summary>
    private void checkLearnMasteryBotStopped_CheckedChanged(object sender, EventArgs e)
    {
        PlayerConfig.Set("RSBot.Skills.checkLearnMasteryBotStopped", checkLearnMasteryBotStopped.Checked);
    }

    /// <summary>
    ///     Handles the ValueChanged event of the numMasteryGap control.
    /// </summary>
    private void numMasteryGap_ValueChanged(object sender, EventArgs e)
    {
        PlayerConfig.Set("RSBot.Skills.numMasteryGap", numMasteryGap.Value);
    }

    /// <summary>
    ///     Loads the resurrection skills into the combo box.
    /// </summary>
    private void LoadResurrectionSkills()
    {
        comboResurrectionSkill.Items.Clear();
        comboResurrectionSkill.Items.Add("None");

        foreach (
            var skill in Game.Player.Skills.KnownSkills.Where(s =>
                s.Record != null
                && ((s.Record.TargetEtc_SelectDeadBody && !s.Record.TargetGroup_Enemy_M) || s.Record.GroupID == 659)
            )
        )
        {
            if (skill.IsLowLevel())
                continue;

            var index = comboResurrectionSkill.Items.Add(skill);
            var resurrectionSkillId = PlayerConfig.Get<int>("RSBot.Skills.ResurrectionSkill");
            if (skill.Id == resurrectionSkillId)
                comboResurrectionSkill.SelectedIndex = index;
        }

        if (comboResurrectionSkill.SelectedIndex <= 0)
            comboResurrectionSkill.SelectedIndex = 0;
    }

    /// <summary>
    ///     Tag payload attached to each mastery entry in <see cref="_masteryMenu" />.
    /// </summary>
    private sealed class MasteryMenuTag
    {
        public string NameCode;
        public string DisplayName;
    }

    private SDUI.Controls.ContextMenuStrip _masteryMenu;

    /// <summary>
    ///     Builds the mastery multi-select dropdown menu, listing only masteries the character has
    ///     actually started learning (Level &gt; 0) so it never shows a wall of "lv. 0" entries, and
    ///     never reserves empty screen space while no character is loaded.
    /// </summary>
    private void LoadMasteries()
    {
        var selectedMasteries = SkillsManager.GetMasteriesToLearn();

        _masteryMenu?.Dispose();
        _masteryMenu = new SDUI.Controls.ContextMenuStrip();
        _masteryMenu.Closing += (s, e) =>
        {
            // Keep the dropdown open after each click so multiple masteries can be toggled in one go.
            if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
                e.Cancel = true;
        };

        var learnableMasteries = Game.Player.Skills.Masteries.Where(m => m.Level > 0).ToList();

        foreach (var mastery in learnableMasteries)
        {
            var item = new ToolStripMenuItem($"{mastery.Record.Name} (lv. {mastery.Level})")
            {
                CheckOnClick = true,
                Checked = selectedMasteries.Contains(mastery.Record.NameCode),
                Tag = new MasteryMenuTag { NameCode = mastery.Record.NameCode, DisplayName = mastery.Record.Name },
            };
            item.Click += MasteryMenuItem_Click;
            _masteryMenu.Items.Add(item);
        }

        if (_masteryMenu.Items.Count == 0)
            _masteryMenu.Items.Add(new ToolStripMenuItem("No masteries learned yet") { Enabled = false });

        UpdateMasterySelectButtonText();
    }

    /// <summary>
    ///     Refreshes <see cref="btnMasterySelect" />'s text to summarize the currently checked masteries.
    /// </summary>
    private void UpdateMasterySelectButtonText()
    {
        var checkedNames = _masteryMenu.Items
            .OfType<ToolStripMenuItem>()
            .Where(i => i.Checked && i.Tag is MasteryMenuTag)
            .Select(i => ((MasteryMenuTag)i.Tag).DisplayName)
            .ToList();

        btnMasterySelect.Text = checkedNames.Count == 0
            ? "Select masteries...  ▾"
            : string.Join(", ", checkedNames) + "  ▾";
    }

    /// <summary>
    ///     Handles the Click event of the btnMasterySelect control, opening the multi-select dropdown.
    /// </summary>
    private void btnMasterySelect_Click(object sender, EventArgs e)
    {
        if (_masteryMenu == null || _masteryMenu.Items.Count == 0)
            return;

        _masteryMenu.Show(btnMasterySelect, new Point(0, btnMasterySelect.Height));
    }

    /// <summary>
    ///     Handles the Click event of a per-mastery entry inside <see cref="_masteryMenu" />.
    /// </summary>
    private void MasteryMenuItem_Click(object sender, EventArgs e)
    {
        if (sender is not ToolStripMenuItem item || item.Tag is not MasteryMenuTag tag)
            return;

        if (item.Checked)
            SkillsManager.AddMasteryToLearn(tag.NameCode);
        else
            SkillsManager.RemoveMasteryToLearn(tag.NameCode);

        UpdateMasterySelectButtonText();
    }

    /// <summary>
    ///     Handles the CheckedChanged event of the stat point settings controls.
    /// </summary>
    private void settings_CheckedChanged(object sender, EventArgs e)
    {
        PlayerConfig.Set("RSBot.Protection.checkIncInt", checkIncInt.Checked);
        PlayerConfig.Set("RSBot.Protection.checkIncStr", checkIncStr.Checked);
        PlayerConfig.Set("RSBot.Protection.checkIncBotStopped", checkIncBotStopped.Checked);
    }

    /// <summary>
    ///     Re-calculates the max points of the Str numeric.
    /// </summary>
    private void numIncInt_ValueChanged(object sender, EventArgs e)
    {
        numIncStr.Maximum = 3 - numIncInt.Value;
        PlayerConfig.Set("RSBot.Protection.numIncInt", numIncInt.Value);
    }

    /// <summary>
    ///     Re-calculates the max points of the Int numeric.
    /// </summary>
    private void numIncStr_ValueChanged(object sender, EventArgs e)
    {
        numIncInt.Maximum = 3 - numIncStr.Value;
        PlayerConfig.Set("RSBot.Protection.numIncStr", numIncStr.Value);
    }

    private void OnIncreaseStat()
    {
        if (this.InvokeRequired)
        {
            this.Invoke(new Action(OnIncreaseStat));
            return;
        }
        if (Game.Player.StatPoints < numIncInt.Value + numIncStr.Value)
        {
            buttonRun.Text = "Run";
            _statIncreaseRunning = false;
        }
    }

    private void buttonRun_Click(object sender, EventArgs e)
    {
        if (_statIncreaseRunning)
        {
            buttonRun.Text = "Run";
            _statIncreaseRunning = false;
            StatPointsHandler.CancellationRequested = true;
            return;
        }

        StatPointsHandler.CancellationRequested = false;
        var stepSize = numIncInt.Value + numIncStr.Value;

        if (stepSize == 0)
            return;
        if (Game.Player.StatPoints < stepSize)
            return;

        var availableSteps = Math.Floor(Game.Player.StatPoints / stepSize);

        if (Game.Player.StatPoints == stepSize)
            availableSteps = 1;

        if (availableSteps == 0)
            return;

        Task.Run(() => StatPointsHandler.IncreaseStatPoints((int)availableSteps));

        _statIncreaseRunning = true;
        buttonRun.Text = "Cancel";
    }

    #region Fields (moved-in sections)

    private bool _statIncreaseRunning;

    #endregion Fields (moved-in sections)
}
