namespace OverjoyedReleaseLocal
{
    partial class Active
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Wisej Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            Wisej.Resources.ComponentResourceManager resources = new Wisej.Resources.ComponentResourceManager(typeof(Active));
            timer = new Wisej.Web.Timer(components);
            pnlDraw = new Wisej.Web.Panel();
            lblLegend = new Wisej.Web.Label();
            hoverButtons = new Wisej.Web.Label();
            lblClose = new Wisej.Web.Label();
            clickButtons = new Wisej.Web.Label();
            quadrantsSVG = new Wisej.Web.HtmlPanel();
            chkSafeZone = new Wisej.Web.CheckBox();
            pnlBaseOverlay = new Wisej.Web.HtmlPanel();
            chkCombineRS = new Wisej.Web.CheckBox();
            chkCombineLT = new Wisej.Web.CheckBox();
            chkCombineRT = new Wisej.Web.CheckBox();
            chkCombineLS = new Wisej.Web.CheckBox();
            lblUseOverjoyed = new Wisej.Web.Label();
            chkSteam = new Wisej.Web.CheckBox();
            chkCombineControllers = new Wisej.Web.CheckBox();
            chkAltRTC = new Wisej.Web.CheckBox();
            chkVoice = new Wisej.Web.CheckBox();
            chkDisableClicks = new Wisej.Web.CheckBox();
            chkStreamer = new Wisej.Web.CheckBox();
            chkActiveHover = new Wisej.Web.CheckBox();
            hideBG = new Wisej.Web.CheckBox();
            chkFPS = new Wisej.Web.CheckBox();
            advancedPanel = new Wisej.Web.Panel();
            chkMarioKart = new Wisej.Web.CheckBox();
            chkMouseLock = new Wisej.Web.CheckBox();
            chkRemote = new Wisej.Web.CheckBox();
            lblCombine = new Wisej.Web.Label();
            pnlExtraButtons = new Wisej.Web.Panel();
            cmbArrowsKeyboard = new Wisej.Web.ComboBox();
            cmbArrowsGamepad = new Wisej.Web.ComboBox();
            btnZL = new Wisej.Web.Button();
            btnZR = new Wisej.Web.Button();
            showMouseMoveLabels = new Wisej.Web.CheckBox();
            showLeftClickLabels = new Wisej.Web.CheckBox();
            showRightClickLabels = new Wisej.Web.CheckBox();
            lblQuickControls = new Wisej.Web.Label();
            btnStartScreen = new Wisej.Web.Button();
            btnBack = new Wisej.Web.Button();
            btnAccept = new Wisej.Web.Button();
            btnLeft = new Wisej.Web.Button();
            btnRight = new Wisej.Web.Button();
            btnDown = new Wisej.Web.Button();
            btnUp = new Wisej.Web.Button();
            btnSettings = new Wisej.Web.Button();
            btnSwitchWakeup = new Wisej.Web.Button();
            btnSwitchStart = new Wisej.Web.Button();
            btnSwitchHome = new Wisej.Web.Button();
            btnSwitchSelect = new Wisej.Web.Button();
            btnConfigSizeDown = new Wisej.Web.Button();
            btnConfigSizeUp = new Wisej.Web.Button();
            combineDiagram = new Wisej.Web.HtmlPanel();
            closeDiagram = new Wisej.Web.Button();
            chkKeepMouseInside = new Wisej.Web.CheckBox();
            pnlPrompts = new Wisej.Web.Panel();
            stylesActive = new Wisej.Web.StyleSheet(components);
            pnlEverything = new Wisej.Web.Panel();
            pnlDraw.SuspendLayout();
            pnlBaseOverlay.SuspendLayout();
            pnlExtraButtons.SuspendLayout();
            pnlEverything.SuspendLayout();
            SuspendLayout();
            // 
            // timer
            // 
            timer.Enabled = true;
            // 
            // pnlDraw
            // 
            pnlDraw.BackColor = System.Drawing.Color.Transparent;
            pnlDraw.Controls.Add(lblLegend);
            pnlDraw.Controls.Add(hoverButtons);
            pnlDraw.Controls.Add(lblClose);
            pnlDraw.Controls.Add(clickButtons);
            pnlDraw.Cursor = null;
            pnlDraw.Dock = Wisej.Web.DockStyle.Fill;
            pnlDraw.Location = new System.Drawing.Point(0, 0);
            pnlDraw.Name = "pnlDraw";
            pnlDraw.ScrollBars = Wisej.Web.ScrollBars.None;
            pnlDraw.Size = new System.Drawing.Size(1515, 562);
            pnlDraw.TabIndex = 0;
            // 
            // lblLegend
            // 
            lblLegend.AutoEllipsis = true;
            lblLegend.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            lblLegend.Dock = Wisej.Web.DockStyle.Top;
            lblLegend.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lblLegend.ForeColor = System.Drawing.Color.Black;
            lblLegend.Location = new System.Drawing.Point(0, 0);
            lblLegend.Margin = new Wisej.Web.Padding(0);
            lblLegend.Name = "lblLegend";
            lblLegend.Size = new System.Drawing.Size(1515, 32);
            lblLegend.TabIndex = 5;
            lblLegend.Text = "Legend: MM -None | LC - None | RC - None";
            lblLegend.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblLegend.Visible = false;
            // 
            // hoverButtons
            // 
            hoverButtons.BackColor = System.Drawing.Color.FromArgb(0, 0, 0);
            hoverButtons.CssStyle = "border-radius:10px;";
            hoverButtons.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            hoverButtons.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            hoverButtons.Location = new System.Drawing.Point(12, 47);
            hoverButtons.Name = "hoverButtons";
            hoverButtons.Padding = new Wisej.Web.Padding(5);
            hoverButtons.Size = new System.Drawing.Size(560, 32);
            hoverButtons.TabIndex = 1;
            hoverButtons.Text = "Mouse Move: No buttons are being pressed.";
            hoverButtons.Visible = false;
            // 
            // lblClose
            // 
            lblClose.AutoSize = true;
            lblClose.BackColor = System.Drawing.Color.DarkRed;
            lblClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lblClose.ForeColor = System.Drawing.Color.White;
            lblClose.Location = new System.Drawing.Point(453, 94);
            lblClose.Margin = new Wisej.Web.Padding(0);
            lblClose.Name = "lblClose";
            lblClose.Padding = new Wisej.Web.Padding(5);
            lblClose.Size = new System.Drawing.Size(63, 32);
            lblClose.TabIndex = 2;
            lblClose.Text = "Close";
            lblClose.Visible = false;
            // 
            // clickButtons
            // 
            clickButtons.BackColor = System.Drawing.Color.FromArgb(0, 0, 0);
            clickButtons.CssStyle = "border-radius:10px;";
            clickButtons.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            clickButtons.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            clickButtons.Location = new System.Drawing.Point(12, 85);
            clickButtons.MaximumSize = new System.Drawing.Size(560, 32);
            clickButtons.MinimumSize = new System.Drawing.Size(560, 32);
            clickButtons.Name = "clickButtons";
            clickButtons.Padding = new Wisej.Web.Padding(5);
            clickButtons.Size = new System.Drawing.Size(560, 32);
            clickButtons.TabIndex = 4;
            clickButtons.Text = "Click: No buttons are being pressed.";
            clickButtons.Visible = false;
            // 
            // quadrantsSVG
            // 
            quadrantsSVG.Dock = Wisej.Web.DockStyle.Fill;
            quadrantsSVG.Focusable = false;
            quadrantsSVG.Html = resources.GetString("quadrantsSVG.Html");
            quadrantsSVG.Location = new System.Drawing.Point(0, 0);
            quadrantsSVG.Name = "quadrantsSVG";
            quadrantsSVG.ScrollBars = Wisej.Web.ScrollBars.None;
            quadrantsSVG.Size = new System.Drawing.Size(1515, 562);
            quadrantsSVG.TabIndex = 5;
            quadrantsSVG.TabStop = false;
            quadrantsSVG.Visible = false;
            // 
            // pnlBaseOverlay
            // 
            pnlBaseOverlay.Controls.Add(chkCombineRS);
            pnlBaseOverlay.Controls.Add(chkCombineLT);
            pnlBaseOverlay.Controls.Add(chkCombineRT);
            pnlBaseOverlay.Controls.Add(chkCombineLS);
            pnlBaseOverlay.Controls.Add(lblUseOverjoyed);
            pnlBaseOverlay.Controls.Add(chkSteam);
            pnlBaseOverlay.Controls.Add(chkCombineControllers);
            pnlBaseOverlay.Controls.Add(chkAltRTC);
            pnlBaseOverlay.Controls.Add(chkVoice);
            pnlBaseOverlay.Controls.Add(chkDisableClicks);
            pnlBaseOverlay.Controls.Add(chkStreamer);
            pnlBaseOverlay.Controls.Add(chkActiveHover);
            pnlBaseOverlay.Controls.Add(hideBG);
            pnlBaseOverlay.Controls.Add(chkFPS);
            pnlBaseOverlay.Controls.Add(advancedPanel);
            pnlBaseOverlay.Controls.Add(chkMarioKart);
            pnlBaseOverlay.Controls.Add(chkMouseLock);
            pnlBaseOverlay.Controls.Add(chkRemote);
            pnlBaseOverlay.Controls.Add(lblCombine);
            pnlBaseOverlay.Controls.Add(pnlExtraButtons);
            pnlBaseOverlay.Controls.Add(combineDiagram);
            pnlBaseOverlay.Controls.Add(closeDiagram);
            pnlBaseOverlay.Controls.Add(btnConfigSizeDown);
            pnlBaseOverlay.Controls.Add(btnConfigSizeUp);
            pnlBaseOverlay.Cursor = Wisej.Web.Cursors.Default;
            pnlBaseOverlay.Dock = Wisej.Web.DockStyle.Fill;
            pnlBaseOverlay.Enabled = false;
            pnlBaseOverlay.Focusable = false;
            pnlBaseOverlay.Html = resources.GetString("pnlBaseOverlay.Html");
            pnlBaseOverlay.Location = new System.Drawing.Point(0, 0);
            pnlBaseOverlay.Name = "pnlBaseOverlay";
            pnlBaseOverlay.ScrollBars = Wisej.Web.ScrollBars.None;
            pnlBaseOverlay.Size = new System.Drawing.Size(1515, 562);
            pnlBaseOverlay.TabIndex = 13;
            pnlBaseOverlay.TabStop = false;
            pnlBaseOverlay.DoubleClick += pnlBaseOverlay_DoubleClick;
            pnlBaseOverlay.MouseDown += pnlBaseOverlay_MouseDown;
            pnlBaseOverlay.MouseLeave += pnlBaseOverlay_MouseLeave;
            pnlBaseOverlay.MouseUp += pnlBaseOverlay_MouseUp;
            pnlBaseOverlay.Appear += pnlBaseOverlay_Appear;
            // 
            // chkCombineRS
            // 
            chkCombineRS.AutoSize = false;
            chkCombineRS.BackColor = System.Drawing.Color.FromArgb(0, 0, 0);
            chkCombineRS.CheckState = Wisej.Web.CheckState.Unchecked;
            chkCombineRS.CssStyle = "border-radius:5px;";
            chkCombineRS.Cursor = Wisej.Web.Cursors.Hand;
            chkCombineRS.Focusable = false;
            chkCombineRS.Font = new System.Drawing.Font("default", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkCombineRS.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            chkCombineRS.Location = new System.Drawing.Point(201, 44);
            chkCombineRS.Name = "chkCombineRS";
            chkCombineRS.Size = new System.Drawing.Size(65, 23);
            chkCombineRS.TabIndex = 41;
            chkCombineRS.Text = "RStick";
            chkCombineRS.Visible = false;
            // 
            // chkCombineLT
            // 
            chkCombineLT.AutoSize = false;
            chkCombineLT.BackColor = System.Drawing.Color.FromArgb(0, 0, 0);
            chkCombineLT.CheckState = Wisej.Web.CheckState.Unchecked;
            chkCombineLT.CssStyle = "border-radius:5px;";
            chkCombineLT.Cursor = Wisej.Web.Cursors.Hand;
            chkCombineLT.Focusable = false;
            chkCombineLT.Font = new System.Drawing.Font("default", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkCombineLT.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            chkCombineLT.Location = new System.Drawing.Point(270, 44);
            chkCombineLT.Name = "chkCombineLT";
            chkCombineLT.Size = new System.Drawing.Size(75, 23);
            chkCombineLT.TabIndex = 40;
            chkCombineLT.Text = "LTrigger";
            chkCombineLT.Visible = false;
            // 
            // chkCombineRT
            // 
            chkCombineRT.AutoSize = false;
            chkCombineRT.BackColor = System.Drawing.Color.FromArgb(0, 0, 0);
            chkCombineRT.CheckState = Wisej.Web.CheckState.Unchecked;
            chkCombineRT.CssStyle = "border-radius:5px;";
            chkCombineRT.Cursor = Wisej.Web.Cursors.Hand;
            chkCombineRT.Focusable = false;
            chkCombineRT.Font = new System.Drawing.Font("default", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkCombineRT.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            chkCombineRT.Location = new System.Drawing.Point(349, 44);
            chkCombineRT.Name = "chkCombineRT";
            chkCombineRT.Size = new System.Drawing.Size(80, 23);
            chkCombineRT.TabIndex = 39;
            chkCombineRT.Text = "RTrigger";
            chkCombineRT.Visible = false;
            // 
            // chkCombineLS
            // 
            chkCombineLS.AutoSize = false;
            chkCombineLS.BackColor = System.Drawing.Color.FromArgb(0, 0, 0);
            chkCombineLS.CheckState = Wisej.Web.CheckState.Unchecked;
            chkCombineLS.CssStyle = "border-radius:5px;";
            chkCombineLS.Cursor = Wisej.Web.Cursors.Hand;
            chkCombineLS.Focusable = false;
            chkCombineLS.Font = new System.Drawing.Font("default", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkCombineLS.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            chkCombineLS.Location = new System.Drawing.Point(132, 44);
            chkCombineLS.Name = "chkCombineLS";
            chkCombineLS.Size = new System.Drawing.Size(65, 23);
            chkCombineLS.TabIndex = 37;
            chkCombineLS.Text = "LStick";
            chkCombineLS.Visible = false;
            // 
            // lblUseOverjoyed
            // 
            lblUseOverjoyed.BackColor = System.Drawing.Color.FromArgb(0, 0, 0);
            lblUseOverjoyed.CssStyle = "border-radius:5px;";
            lblUseOverjoyed.Font = new System.Drawing.Font("default", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblUseOverjoyed.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            lblUseOverjoyed.Location = new System.Drawing.Point(9, 46);
            lblUseOverjoyed.Name = "lblUseOverjoyed";
            lblUseOverjoyed.Size = new System.Drawing.Size(120, 18);
            lblUseOverjoyed.TabIndex = 38;
            lblUseOverjoyed.Text = "Use Overjoyed For:";
            lblUseOverjoyed.Visible = false;
            // 
            // chkSteam
            // 
            chkSteam.AutoSize = false;
            chkSteam.BackColor = System.Drawing.Color.FromArgb(0, 0, 0);
            chkSteam.CssStyle = "border-radius:5px;";
            chkSteam.Cursor = Wisej.Web.Cursors.Hand;
            chkSteam.Focusable = false;
            chkSteam.Font = new System.Drawing.Font("default", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkSteam.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            chkSteam.Location = new System.Drawing.Point(434, 44);
            chkSteam.Name = "chkSteam";
            chkSteam.Size = new System.Drawing.Size(154, 23);
            chkSteam.TabIndex = 33;
            chkSteam.Text = "Watch Steam Tutorial";
            chkSteam.Visible = false;
            chkSteam.CheckedChanged += chkSteam_CheckedChanged;
            // 
            // chkCombineControllers
            // 
            chkCombineControllers.Focusable = false;
            chkCombineControllers.Font = new System.Drawing.Font("default", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkCombineControllers.ForeColor = System.Drawing.Color.White;
            chkCombineControllers.Location = new System.Drawing.Point(53, 232);
            chkCombineControllers.Name = "chkCombineControllers";
            chkCombineControllers.Size = new System.Drawing.Size(30, 22);
            chkCombineControllers.TabIndex = 16;
            chkCombineControllers.Visible = false;
            chkCombineControllers.CheckedChanged += chkCombineControllers_CheckedChanged;
            // 
            // chkAltRTC
            // 
            chkAltRTC.Font = new System.Drawing.Font("default", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkAltRTC.ForeColor = System.Drawing.Color.White;
            chkAltRTC.Location = new System.Drawing.Point(53, 232);
            chkAltRTC.Name = "chkAltRTC";
            chkAltRTC.Size = new System.Drawing.Size(30, 22);
            chkAltRTC.TabIndex = 15;
            chkAltRTC.Visible = false;
            // 
            // chkVoice
            // 
            chkVoice.Focusable = false;
            chkVoice.Font = new System.Drawing.Font("default", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkVoice.ForeColor = System.Drawing.Color.White;
            chkVoice.Location = new System.Drawing.Point(99, 232);
            chkVoice.Name = "chkVoice";
            chkVoice.Size = new System.Drawing.Size(30, 22);
            chkVoice.TabIndex = 18;
            chkVoice.Visible = false;
            chkVoice.CheckedChanged += chkVoice_CheckedChanged;
            // 
            // chkDisableClicks
            // 
            chkDisableClicks.Focusable = false;
            chkDisableClicks.Font = new System.Drawing.Font("default", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkDisableClicks.ForeColor = System.Drawing.Color.White;
            chkDisableClicks.Location = new System.Drawing.Point(53, 232);
            chkDisableClicks.Name = "chkDisableClicks";
            chkDisableClicks.Size = new System.Drawing.Size(30, 22);
            chkDisableClicks.TabIndex = 14;
            chkDisableClicks.Visible = false;
            chkDisableClicks.CheckedChanged += chkDisableClicks_CheckedChanged;
            // 
            // chkStreamer
            // 
            chkStreamer.CheckedForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            chkStreamer.CssStyle = "border-radius:5px;";
            chkStreamer.Cursor = Wisej.Web.Cursors.Hand;
            chkStreamer.Focusable = false;
            chkStreamer.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkStreamer.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            chkStreamer.Location = new System.Drawing.Point(31, 201);
            chkStreamer.Name = "chkStreamer";
            chkStreamer.Size = new System.Drawing.Size(121, 23);
            chkStreamer.TabIndex = 19;
            chkStreamer.Text = "Admin Mode";
            chkStreamer.Checked = false;
            chkStreamer.CheckedChanged += chkStreamer_CheckedChanged;
            // 
            // chkActiveHover
            // 
            chkActiveHover.Location = new System.Drawing.Point(53, 232);
            chkActiveHover.Name = "chkActiveHover";
            chkActiveHover.Size = new System.Drawing.Size(30, 23);
            chkActiveHover.TabIndex = 5;
            chkActiveHover.Visible = false;
            // 
            // hideBG
            // 
            hideBG.CssStyle = "border-radius:5px;";
            hideBG.Cursor = Wisej.Web.Cursors.Hand;
            hideBG.Focusable = false;
            hideBG.Font = new System.Drawing.Font("default", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            hideBG.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            hideBG.Location = new System.Drawing.Point(177, 185);
            hideBG.Name = "hideBG";
            hideBG.Size = new System.Drawing.Size(127, 22);
            hideBG.TabIndex = 32;
            hideBG.Text = "Hide Background";
            hideBG.CheckedChanged += hideBG_CheckedChanged;
            // 
            // chkFPS
            // 
            chkFPS.Focusable = false;
            chkFPS.Font = new System.Drawing.Font("default", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkFPS.ForeColor = System.Drawing.Color.White;
            chkFPS.Location = new System.Drawing.Point(53, 232);
            chkFPS.Name = "chkFPS";
            chkFPS.Size = new System.Drawing.Size(30, 22);
            chkFPS.TabIndex = 12;
            chkFPS.Visible = false;
            chkFPS.CheckedChanged += chkFPS_CheckedChanged;
            // 
            // advancedPanel
            // 
            advancedPanel.Cursor = null;
            advancedPanel.Location = new System.Drawing.Point(261, 232);
            advancedPanel.Name = "advancedPanel";
            advancedPanel.Size = new System.Drawing.Size(200, 100);
            advancedPanel.TabIndex = 43;
            // 
            // chkMarioKart
            // 
            chkMarioKart.Focusable = false;
            chkMarioKart.Font = new System.Drawing.Font("default", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkMarioKart.ForeColor = System.Drawing.Color.White;
            chkMarioKart.Location = new System.Drawing.Point(53, 232);
            chkMarioKart.Name = "chkMarioKart";
            chkMarioKart.Size = new System.Drawing.Size(30, 22);
            chkMarioKart.TabIndex = 5;
            chkMarioKart.Visible = false;
            // 
            // chkSafeZone
            // 
            chkSafeZone.Focusable = false;
            chkSafeZone.Font = new System.Drawing.Font("default", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkSafeZone.ForeColor = System.Drawing.Color.White;
            chkSafeZone.Location = new System.Drawing.Point(53, 232);
            chkSafeZone.Name = "chkSafeZone";
            chkSafeZone.Size = new System.Drawing.Size(30, 22);
            chkSafeZone.TabIndex = 5;
            chkSafeZone.Visible = false;
            chkSafeZone.CheckedChanged += chkSafeZone_CheckedChanged;
            // 
            // chkMouseLock
            // 
            chkMouseLock.AutoSize = false;
            chkMouseLock.CheckState = Wisej.Web.CheckState.Checked;
            chkMouseLock.CssStyle = "border-radius:5px;";
            chkMouseLock.Cursor = Wisej.Web.Cursors.Hand;
            chkMouseLock.Focusable = false;
            chkMouseLock.Font = new System.Drawing.Font("default", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkMouseLock.ForeColor = System.Drawing.Color.White;
            chkMouseLock.Location = new System.Drawing.Point(443, 487);
            chkMouseLock.MaximumSize = new System.Drawing.Size(225, 23);
            chkMouseLock.MinimumSize = new System.Drawing.Size(225, 23);
            chkMouseLock.Name = "chkMouseLock";
            chkMouseLock.Size = new System.Drawing.Size(225, 23);
            chkMouseLock.TabIndex = 42;
            chkMouseLock.Text = "Prevent Game From Locking Mouse";
            chkMouseLock.Visible = false;
            chkMouseLock.CheckedChanged += chkMouseLock_CheckedChanged;
            // 
            // chkRemote
            // 
            chkRemote.AutoSize = false;
            chkRemote.BackColor = System.Drawing.Color.Black;
            chkRemote.CssStyle = "border-radius:5px;";
            chkRemote.Cursor = Wisej.Web.Cursors.Hand;
            chkRemote.Focusable = false;
            chkRemote.Font = new System.Drawing.Font("default", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkRemote.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            chkRemote.Location = new System.Drawing.Point(9, 557);
            chkRemote.MaximumSize = new System.Drawing.Size(138, 23);
            chkRemote.MinimumSize = new System.Drawing.Size(138, 23);
            chkRemote.Name = "chkRemote";
            chkRemote.Size = new System.Drawing.Size(138, 23);
            chkRemote.TabIndex = 36;
            chkRemote.Text = "Remote Overjoyed";
            chkRemote.Visible = false;
            chkRemote.CheckedChanged += chkRemote_CheckedChanged;
            // 
            // lblCombine
            // 
            lblCombine.AutoEllipsis = true;
            lblCombine.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            lblCombine.Dock = Wisej.Web.DockStyle.Bottom;
            lblCombine.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lblCombine.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            lblCombine.Location = new System.Drawing.Point(0, 530);
            lblCombine.Margin = new Wisej.Web.Padding(0);
            lblCombine.Name = "lblCombine";
            lblCombine.Size = new System.Drawing.Size(1515, 32);
            lblCombine.TabIndex = 5;
            lblCombine.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblCombine.Visible = false;
            lblCombine.MouseClick += lblCombine_MouseClick;
            lblCombine.MouseDoubleClick += lblCombine_MouseDoubleClick;
            lblCombine.MouseHover += lblCombine_MouseHover;
            lblCombine.MouseLeave += lblCombine_MouseLeave;
            // 
            // pnlExtraButtons
            // 
            pnlExtraButtons.BackColor = System.Drawing.Color.Transparent;
            pnlExtraButtons.Controls.Add(cmbArrowsKeyboard);
            pnlExtraButtons.Controls.Add(cmbArrowsGamepad);
            pnlExtraButtons.Controls.Add(btnZL);
            pnlExtraButtons.Controls.Add(btnZR);
            pnlExtraButtons.Controls.Add(showMouseMoveLabels);
            pnlExtraButtons.Controls.Add(showLeftClickLabels);
            pnlExtraButtons.Controls.Add(showRightClickLabels);
            pnlExtraButtons.Controls.Add(lblQuickControls);
            pnlExtraButtons.Controls.Add(btnStartScreen);
            pnlExtraButtons.Controls.Add(btnBack);
            pnlExtraButtons.Controls.Add(btnAccept);
            pnlExtraButtons.Controls.Add(btnLeft);
            pnlExtraButtons.Controls.Add(btnRight);
            pnlExtraButtons.Controls.Add(btnDown);
            pnlExtraButtons.Controls.Add(btnUp);
            pnlExtraButtons.Controls.Add(btnSettings);
            pnlExtraButtons.Controls.Add(btnSwitchWakeup);
            pnlExtraButtons.Controls.Add(btnSwitchStart);
            pnlExtraButtons.Controls.Add(btnSwitchHome);
            pnlExtraButtons.Controls.Add(btnSwitchSelect);
            pnlExtraButtons.CssStyle = "border-radius:5px;";
            pnlExtraButtons.Cursor = null;
            pnlExtraButtons.Enabled = false;
            pnlExtraButtons.Location = new System.Drawing.Point(9, 35);
            pnlExtraButtons.Name = "pnlExtraButtons";
            pnlExtraButtons.Size = new System.Drawing.Size(568, 126);
            pnlExtraButtons.TabIndex = 4;
            // 
            // cmbArrowsKeyboard
            // 
            cmbArrowsKeyboard.AutoSize = false;
            cmbArrowsKeyboard.BackColor = System.Drawing.Color.LightCyan;
            cmbArrowsKeyboard.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            cmbArrowsKeyboard.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            cmbArrowsKeyboard.Items.AddRange(new object[] { "Arrows", "WASD" });
            cmbArrowsKeyboard.Location = new System.Drawing.Point(22, 40);
            cmbArrowsKeyboard.MaximumSize = new System.Drawing.Size(96, 20);
            cmbArrowsKeyboard.MinimumSize = new System.Drawing.Size(96, 20);
            cmbArrowsKeyboard.Name = "cmbArrowsKeyboard";
            cmbArrowsKeyboard.Size = new System.Drawing.Size(96, 20);
            cmbArrowsKeyboard.TabIndex = 45;
            cmbArrowsKeyboard.Visible = false;
            cmbArrowsKeyboard.SelectedIndexChanged += cmbArrowsKeyboard_SelectedIndexChanged;
            // 
            // cmbArrowsGamepad
            // 
            cmbArrowsGamepad.AutoSize = false;
            cmbArrowsGamepad.BackColor = System.Drawing.Color.LightCyan;
            cmbArrowsGamepad.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            cmbArrowsGamepad.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            cmbArrowsGamepad.Items.AddRange(new object[] { "More...", "LStick", "RStick", "DPad" });
            cmbArrowsGamepad.Location = new System.Drawing.Point(22, 40);
            cmbArrowsGamepad.MaximumSize = new System.Drawing.Size(96, 20);
            cmbArrowsGamepad.MinimumSize = new System.Drawing.Size(96, 20);
            cmbArrowsGamepad.Name = "cmbArrowsGamepad";
            cmbArrowsGamepad.Size = new System.Drawing.Size(96, 20);
            cmbArrowsGamepad.TabIndex = 44;
            cmbArrowsGamepad.Visible = false;
            cmbArrowsGamepad.SelectedIndexChanged += cmbArrowsGamepad_SelectedIndexChanged;
            // 
            // btnZL
            // 
            btnZL.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            btnZL.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            btnZL.Cursor = Wisej.Web.Cursors.Hand;
            btnZL.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnZL.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            btnZL.Location = new System.Drawing.Point(128, 40);
            btnZL.Margin = new Wisej.Web.Padding(0);
            btnZL.MaximumSize = new System.Drawing.Size(24, 22);
            btnZL.MinimumSize = new System.Drawing.Size(24, 22);
            btnZL.Name = "btnZL";
            btnZL.Size = new System.Drawing.Size(24, 22);
            btnZL.TabIndex = 41;
            btnZL.TabStop = false;
            btnZL.Text = "ZL";
            btnZL.Visible = false;
            btnZL.Click += btnZL_Click;
            btnZL.MouseEnter += btnSettings_MouseEnter;
            btnZL.MouseLeave += btnSettings_MouseLeave;
            // 
            // btnZR
            // 
            btnZR.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            btnZR.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            btnZR.Cursor = Wisej.Web.Cursors.Hand;
            btnZR.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnZR.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            btnZR.Location = new System.Drawing.Point(226, 41);
            btnZR.Margin = new Wisej.Web.Padding(0);
            btnZR.MaximumSize = new System.Drawing.Size(24, 22);
            btnZR.MinimumSize = new System.Drawing.Size(24, 22);
            btnZR.Name = "btnZR";
            btnZR.Size = new System.Drawing.Size(24, 22);
            btnZR.TabIndex = 40;
            btnZR.TabStop = false;
            btnZR.Text = "ZR";
            btnZR.Visible = false;
            btnZR.Click += btnZR_Click;
            btnZR.MouseEnter += btnSettings_MouseEnter;
            btnZR.MouseLeave += btnSettings_MouseLeave;
            // 
            // showMouseMoveLabels
            // 
            showMouseMoveLabels.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            showMouseMoveLabels.Location = new System.Drawing.Point(0, 0);
            showMouseMoveLabels.Name = "showMouseMoveLabels";
            showMouseMoveLabels.Size = new System.Drawing.Size(183, 23);
            showMouseMoveLabels.TabIndex = 0;
            showMouseMoveLabels.Text = "Show Mouse Move Labels";
            showMouseMoveLabels.CheckedChanged += showMouseMoveLabels_CheckedChanged;
            // 
            // showLeftClickLabels
            // 
            showLeftClickLabels.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            showLeftClickLabels.Location = new System.Drawing.Point(0, 0);
            showLeftClickLabels.Name = "showLeftClickLabels";
            showLeftClickLabels.Size = new System.Drawing.Size(160, 23);
            showLeftClickLabels.TabIndex = 1;
            showLeftClickLabels.Text = "Show Left Click Labels";
            showLeftClickLabels.CheckedChanged += showLeftClickLabels_CheckedChanged;
            // 
            // showRightClickLabels
            // 
            showRightClickLabels.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            showRightClickLabels.Location = new System.Drawing.Point(0, 0);
            showRightClickLabels.Name = "showRightClickLabels";
            showRightClickLabels.Size = new System.Drawing.Size(169, 23);
            showRightClickLabels.TabIndex = 2;
            showRightClickLabels.Text = "Show Right Click Labels";
            showRightClickLabels.CheckedChanged += showRightClickLabels_CheckedChanged;
            // 
            // lblQuickControls
            // 
            lblQuickControls.BackColor = System.Drawing.Color.Black;
            lblQuickControls.CssStyle = "border-radius:5px;";
            lblQuickControls.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblQuickControls.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            lblQuickControls.Location = new System.Drawing.Point(22, 9);
            lblQuickControls.Name = "lblQuickControls";
            lblQuickControls.Size = new System.Drawing.Size(339, 22);
            lblQuickControls.TabIndex = 39;
            lblQuickControls.Text = "Quick Controls (for navigating game menus)";
            // 
            // btnStartScreen
            // 
            btnStartScreen.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            btnStartScreen.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            btnStartScreen.Cursor = Wisej.Web.Cursors.Hand;
            btnStartScreen.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnStartScreen.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            btnStartScreen.Location = new System.Drawing.Point(22, 95);
            btnStartScreen.MaximumSize = new System.Drawing.Size(96, 20);
            btnStartScreen.MinimumSize = new System.Drawing.Size(96, 20);
            btnStartScreen.Name = "btnStartScreen";
            btnStartScreen.Size = new System.Drawing.Size(96, 20);
            btnStartScreen.TabIndex = 11;
            btnStartScreen.TabStop = false;
            btnStartScreen.Text = "L + R";
            btnStartScreen.Click += btnStartScreen_Click;
            btnStartScreen.MouseEnter += btnSettings_MouseEnter;
            btnStartScreen.MouseLeave += btnSettings_MouseLeave;
            // 
            // btnBack
            // 
            btnBack.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            btnBack.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            btnBack.Cursor = Wisej.Web.Cursors.Hand;
            btnBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnBack.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            btnBack.Location = new System.Drawing.Point(192, 68);
            btnBack.MaximumSize = new System.Drawing.Size(28, 22);
            btnBack.MinimumSize = new System.Drawing.Size(28, 22);
            btnBack.Name = "btnBack";
            btnBack.Size = new System.Drawing.Size(28, 22);
            btnBack.TabIndex = 10;
            btnBack.TabStop = false;
            btnBack.Text = "B";
            btnBack.Click += btnBack_Click;
            btnBack.MouseEnter += btnSettings_MouseEnter;
            btnBack.MouseLeave += btnSettings_MouseLeave;
            // 
            // btnAccept
            // 
            btnAccept.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            btnAccept.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            btnAccept.Cursor = Wisej.Web.Cursors.Hand;
            btnAccept.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnAccept.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            btnAccept.Location = new System.Drawing.Point(159, 68);
            btnAccept.MaximumSize = new System.Drawing.Size(28, 22);
            btnAccept.MinimumSize = new System.Drawing.Size(28, 22);
            btnAccept.Name = "btnAccept";
            btnAccept.Size = new System.Drawing.Size(28, 22);
            btnAccept.TabIndex = 9;
            btnAccept.TabStop = false;
            btnAccept.Text = "A";
            btnAccept.Click += btnAccept_Click;
            btnAccept.MouseEnter += btnSettings_MouseEnter;
            btnAccept.MouseLeave += btnSettings_MouseLeave;
            // 
            // btnLeft
            // 
            btnLeft.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            btnLeft.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            btnLeft.Cursor = Wisej.Web.Cursors.Hand;
            btnLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnLeft.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            btnLeft.Location = new System.Drawing.Point(128, 68);
            btnLeft.MaximumSize = new System.Drawing.Size(24, 48);
            btnLeft.MinimumSize = new System.Drawing.Size(24, 48);
            btnLeft.Name = "btnLeft";
            btnLeft.Size = new System.Drawing.Size(24, 48);
            btnLeft.TabIndex = 8;
            btnLeft.TabStop = false;
            btnLeft.Text = "🠄";
            btnLeft.Click += btnLeft_Click;
            btnLeft.MouseEnter += btnSettings_MouseEnter;
            btnLeft.MouseLeave += btnSettings_MouseLeave;
            // 
            // btnRight
            // 
            btnRight.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            btnRight.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            btnRight.Cursor = Wisej.Web.Cursors.Hand;
            btnRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnRight.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            btnRight.Location = new System.Drawing.Point(226, 68);
            btnRight.MaximumSize = new System.Drawing.Size(24, 48);
            btnRight.MinimumSize = new System.Drawing.Size(24, 48);
            btnRight.Name = "btnRight";
            btnRight.Size = new System.Drawing.Size(24, 48);
            btnRight.TabIndex = 7;
            btnRight.TabStop = false;
            btnRight.Text = "🠆";
            btnRight.Click += btnRight_Click;
            btnRight.MouseEnter += btnSettings_MouseEnter;
            btnRight.MouseLeave += btnSettings_MouseLeave;
            // 
            // btnDown
            // 
            btnDown.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            btnDown.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            btnDown.Cursor = Wisej.Web.Cursors.Hand;
            btnDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnDown.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            btnDown.Location = new System.Drawing.Point(159, 93);
            btnDown.MaximumSize = new System.Drawing.Size(60, 24);
            btnDown.MinimumSize = new System.Drawing.Size(60, 24);
            btnDown.Name = "btnDown";
            btnDown.Size = new System.Drawing.Size(60, 24);
            btnDown.TabIndex = 6;
            btnDown.TabStop = false;
            btnDown.Text = "🠇";
            btnDown.Click += btnDown_Click;
            btnDown.MouseEnter += btnSettings_MouseEnter;
            btnDown.MouseLeave += btnSettings_MouseLeave;
            // 
            // btnUp
            // 
            btnUp.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            btnUp.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            btnUp.Cursor = Wisej.Web.Cursors.Hand;
            btnUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnUp.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            btnUp.Location = new System.Drawing.Point(159, 41);
            btnUp.MaximumSize = new System.Drawing.Size(60, 24);
            btnUp.MinimumSize = new System.Drawing.Size(60, 24);
            btnUp.Name = "btnUp";
            btnUp.Size = new System.Drawing.Size(60, 24);
            btnUp.TabIndex = 5;
            btnUp.TabStop = false;
            btnUp.Text = "🠅";
            btnUp.Click += btnUp_Click;
            btnUp.MouseEnter += btnSettings_MouseEnter;
            btnUp.MouseLeave += btnSettings_MouseLeave;
            // 
            // btnSettings
            // 
            btnSettings.AllowHtml = true;
            btnSettings.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            btnSettings.BackColor = System.Drawing.Color.FromArgb(0, 167, 209);
            btnSettings.Cursor = Wisej.Web.Cursors.Hand;
            btnSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnSettings.ForeColor = System.Drawing.Color.FromName("@activeCaptionText");
            btnSettings.Location = new System.Drawing.Point(464, 41);
            btnSettings.MaximumSize = new System.Drawing.Size(96, 76);
            btnSettings.MinimumSize = new System.Drawing.Size(96, 76);
            btnSettings.Name = "btnSettings";
            btnSettings.Size = new System.Drawing.Size(96, 76);
            btnSettings.TabIndex = 4;
            btnSettings.TabStop = false;
            btnSettings.Click += btnSettings_Click;
            btnSettings.MouseEnter += btnSettings_MouseEnter;
            btnSettings.MouseLeave += btnSettings_MouseLeave;
            // 
            // btnSwitchWakeup
            // 
            btnSwitchWakeup.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            btnSwitchWakeup.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            btnSwitchWakeup.Cursor = Wisej.Web.Cursors.Hand;
            btnSwitchWakeup.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnSwitchWakeup.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            btnSwitchWakeup.Location = new System.Drawing.Point(22, 68);
            btnSwitchWakeup.MaximumSize = new System.Drawing.Size(96, 20);
            btnSwitchWakeup.MinimumSize = new System.Drawing.Size(96, 20);
            btnSwitchWakeup.Name = "btnSwitchWakeup";
            btnSwitchWakeup.Size = new System.Drawing.Size(96, 20);
            btnSwitchWakeup.TabIndex = 0;
            btnSwitchWakeup.TabStop = false;
            btnSwitchWakeup.Text = "Turn On Switch";
            btnSwitchWakeup.Click += btnSwitchWakeup_Click;
            btnSwitchWakeup.MouseEnter += btnSettings_MouseEnter;
            btnSwitchWakeup.MouseLeave += btnSettings_MouseLeave;
            // 
            // btnSwitchStart
            // 
            btnSwitchStart.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            btnSwitchStart.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            btnSwitchStart.Cursor = Wisej.Web.Cursors.Hand;
            btnSwitchStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnSwitchStart.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            btnSwitchStart.Location = new System.Drawing.Point(260, 41);
            btnSwitchStart.MaximumSize = new System.Drawing.Size(96, 36);
            btnSwitchStart.MinimumSize = new System.Drawing.Size(96, 36);
            btnSwitchStart.Name = "btnSwitchStart";
            btnSwitchStart.Size = new System.Drawing.Size(96, 36);
            btnSwitchStart.TabIndex = 3;
            btnSwitchStart.TabStop = false;
            btnSwitchStart.Text = "Start Button";
            btnSwitchStart.Click += btnSwitchStart_Click;
            btnSwitchStart.MouseEnter += btnSettings_MouseEnter;
            btnSwitchStart.MouseLeave += btnSettings_MouseLeave;
            // 
            // btnSwitchHome
            // 
            btnSwitchHome.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            btnSwitchHome.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            btnSwitchHome.Cursor = Wisej.Web.Cursors.Hand;
            btnSwitchHome.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnSwitchHome.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            btnSwitchHome.Location = new System.Drawing.Point(362, 41);
            btnSwitchHome.MaximumSize = new System.Drawing.Size(96, 76);
            btnSwitchHome.MinimumSize = new System.Drawing.Size(96, 76);
            btnSwitchHome.Name = "btnSwitchHome";
            btnSwitchHome.Size = new System.Drawing.Size(96, 76);
            btnSwitchHome.TabIndex = 1;
            btnSwitchHome.TabStop = false;
            btnSwitchHome.Text = "Home Button";
            btnSwitchHome.Click += btnSwitchHome_Click;
            btnSwitchHome.MouseEnter += btnSettings_MouseEnter;
            btnSwitchHome.MouseLeave += btnSettings_MouseLeave;
            btnSwitchHome.MouseClick += btnSwitchHome_MouseClick;
            // 
            // btnSwitchSelect
            // 
            btnSwitchSelect.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            btnSwitchSelect.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            btnSwitchSelect.Cursor = Wisej.Web.Cursors.Hand;
            btnSwitchSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnSwitchSelect.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            btnSwitchSelect.Location = new System.Drawing.Point(260, 81);
            btnSwitchSelect.MaximumSize = new System.Drawing.Size(96, 36);
            btnSwitchSelect.MinimumSize = new System.Drawing.Size(96, 36);
            btnSwitchSelect.Name = "btnSwitchSelect";
            btnSwitchSelect.Size = new System.Drawing.Size(96, 36);
            btnSwitchSelect.TabIndex = 2;
            btnSwitchSelect.TabStop = false;
            btnSwitchSelect.Text = "Select Button";
            btnSwitchSelect.Click += btnSwitchSelect_Click;
            btnSwitchSelect.MouseEnter += btnSettings_MouseEnter;
            btnSwitchSelect.MouseLeave += btnSettings_MouseLeave;
            // 
            // combineDiagram
            // 
            combineDiagram.Dock = Wisej.Web.DockStyle.Fill;
            combineDiagram.Focusable = false;
            combineDiagram.Html = resources.GetString("combineDiagram.Html");
            combineDiagram.Location = new System.Drawing.Point(0, 0);
            combineDiagram.Name = "combineDiagram";
            combineDiagram.ScrollBars = Wisej.Web.ScrollBars.None;
            combineDiagram.Size = new System.Drawing.Size(1515, 562);
            combineDiagram.TabIndex = 34;
            combineDiagram.TabStop = false;
            combineDiagram.Visible = false;
            // 
            // closeDiagram
            // 
            closeDiagram.CssStyle = "border-radius:10px;";
            closeDiagram.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            closeDiagram.Location = new System.Drawing.Point(225, 519);
            closeDiagram.Name = "closeDiagram";
            closeDiagram.Size = new System.Drawing.Size(150, 37);
            closeDiagram.TabIndex = 35;
            closeDiagram.Text = "Close Diagram";
            closeDiagram.Visible = false;
            closeDiagram.Click += closeDiagram_Click;
            // 
            // btnConfigSizeDown
            // 
            btnConfigSizeDown.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Right;
            btnConfigSizeDown.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            btnConfigSizeDown.Cursor = Wisej.Web.Cursors.Hand;
            btnConfigSizeDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnConfigSizeDown.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            btnConfigSizeDown.Location = new System.Drawing.Point(1454, 526);
            btnConfigSizeDown.MaximumSize = new System.Drawing.Size(28, 28);
            btnConfigSizeDown.MinimumSize = new System.Drawing.Size(28, 28);
            btnConfigSizeDown.Name = "btnConfigSizeDown";
            btnConfigSizeDown.Size = new System.Drawing.Size(28, 28);
            btnConfigSizeDown.TabIndex = 44;
            btnConfigSizeDown.TabStop = false;
            btnConfigSizeDown.Text = "-";
            btnConfigSizeDown.ToolTipText = "Shrink Active window by 10%";
            btnConfigSizeDown.Click += btnConfigSizeDown_Click;
            btnConfigSizeDown.MouseEnter += btnSettings_MouseEnter;
            btnConfigSizeDown.MouseLeave += btnSettings_MouseLeave;
            // 
            // btnConfigSizeUp
            // 
            btnConfigSizeUp.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Right;
            btnConfigSizeUp.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            btnConfigSizeUp.Cursor = Wisej.Web.Cursors.Hand;
            btnConfigSizeUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnConfigSizeUp.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            btnConfigSizeUp.Location = new System.Drawing.Point(1484, 526);
            btnConfigSizeUp.MaximumSize = new System.Drawing.Size(28, 28);
            btnConfigSizeUp.MinimumSize = new System.Drawing.Size(28, 28);
            btnConfigSizeUp.Name = "btnConfigSizeUp";
            btnConfigSizeUp.Size = new System.Drawing.Size(28, 28);
            btnConfigSizeUp.TabIndex = 45;
            btnConfigSizeUp.TabStop = false;
            btnConfigSizeUp.Text = "+";
            btnConfigSizeUp.ToolTipText = "Grow Active window by 10%";
            btnConfigSizeUp.Click += btnConfigSizeUp_Click;
            btnConfigSizeUp.MouseEnter += btnSettings_MouseEnter;
            btnConfigSizeUp.MouseLeave += btnSettings_MouseLeave;
            // 
            // chkKeepMouseInside
            // 
            chkKeepMouseInside.AutoSize = false;
            chkKeepMouseInside.CheckState = Wisej.Web.CheckState.Checked;
            chkKeepMouseInside.CssStyle = "border-radius:5px;";
            chkKeepMouseInside.Cursor = Wisej.Web.Cursors.Hand;
            chkKeepMouseInside.Focusable = false;
            chkKeepMouseInside.Font = new System.Drawing.Font("default", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkKeepMouseInside.ForeColor = System.Drawing.Color.White;
            chkKeepMouseInside.Location = new System.Drawing.Point(443, 487);
            chkKeepMouseInside.MaximumSize = new System.Drawing.Size(200, 23);
            chkKeepMouseInside.MinimumSize = new System.Drawing.Size(200, 23);
            chkKeepMouseInside.Name = "chkKeepMouseInside";
            chkKeepMouseInside.Size = new System.Drawing.Size(200, 23);
            chkKeepMouseInside.TabIndex = 42;
            chkKeepMouseInside.Text = "Keep Mouse Inside Overjoyed";
            chkKeepMouseInside.Visible = false;
            // 
            // pnlPrompts
            // 
            pnlPrompts.BackColor = System.Drawing.Color.Transparent;
            pnlPrompts.Cursor = null;
            pnlPrompts.Location = new System.Drawing.Point(0, 0);
            pnlPrompts.Name = "pnlPrompts";
            pnlPrompts.ScrollBars = Wisej.Web.ScrollBars.None;
            pnlPrompts.Size = new System.Drawing.Size(600, 600);
            pnlPrompts.TabIndex = 14;
            // 
            // stylesActive
            // 
            stylesActive.Styles = resources.GetString("stylesActive.Styles");
            // 
            // pnlEverything
            // 
            pnlEverything.BackColor = System.Drawing.Color.FromArgb(0, 0, 0);
            pnlEverything.Controls.Add(pnlBaseOverlay);
            pnlEverything.Controls.Add(quadrantsSVG);
            pnlEverything.Controls.Add(pnlDraw);
            pnlEverything.Controls.Add(pnlPrompts);
            pnlEverything.Cursor = null;
            pnlEverything.Dock = Wisej.Web.DockStyle.Fill;
            pnlEverything.Location = new System.Drawing.Point(0, 0);
            pnlEverything.Name = "pnlEverything";
            pnlEverything.ScrollBars = Wisej.Web.ScrollBars.None;
            pnlEverything.Size = new System.Drawing.Size(1515, 562);
            pnlEverything.TabIndex = 15;
            pnlEverything.Appear += pnlEverything_Appear;
            // 
            // Active
            // 
            AutoScaleMode = Wisej.Web.AutoScaleMode.None;
            BackColor = System.Drawing.Color.FromArgb(0, 0, 0);
            Controls.Add(pnlEverything);
            KeyPreview = true;
            Name = "Active";
            ScrollBars = Wisej.Web.ScrollBars.None;
            Size = new System.Drawing.Size(1515, 562);
            Load += Form1_Load;
            pnlDraw.ResumeLayout(false);
            pnlDraw.PerformLayout();
            pnlBaseOverlay.ResumeLayout(false);
            pnlBaseOverlay.PerformLayout();
            pnlExtraButtons.ResumeLayout(false);
            pnlExtraButtons.PerformLayout();
            pnlEverything.ResumeLayout(false);
            ResumeLayout(false);

        }



        #endregion

        private Wisej.Web.Timer timer;
        private Wisej.Web.Panel pnlDraw;
        private Wisej.Web.Label hoverButtons;
        private Wisej.Web.Label clickButtons;
        private Wisej.Web.HtmlPanel pnlBaseOverlay;
        private Wisej.Web.Label lblClose;
        private Wisej.Web.Panel pnlPrompts;
        private Wisej.Web.Button btnSwitchWakeup;
        private Wisej.Web.Button btnSwitchStart;
        private Wisej.Web.Button btnSwitchSelect;
        private Wisej.Web.Button btnSwitchHome;
        private Wisej.Web.Panel pnlExtraButtons;
        private Wisej.Web.Button btnSettings;
        private Wisej.Web.Button btnRight;
        private Wisej.Web.Button btnDown;
        private Wisej.Web.Button btnUp;
        private Wisej.Web.Button btnLeft;
        private Wisej.Web.Button btnAccept;
        private Wisej.Web.Button btnBack;
        private Wisej.Web.Button btnStartScreen;
        private Wisej.Web.CheckBox chkMarioKart;
        private Wisej.Web.CheckBox chkFPS;
        private Wisej.Web.Label lblLegend;
        private Wisej.Web.StyleSheet stylesActive;
        private Wisej.Web.CheckBox chkActiveHover;
        private Wisej.Web.CheckBox chkDisableClicks;
        private Wisej.Web.CheckBox chkAltRTC;
        private Wisej.Web.CheckBox chkCombineControllers;
        private Wisej.Web.CheckBox chkVoice;
        private Wisej.Web.CheckBox chkStreamer;
        private Wisej.Web.HtmlPanel quadrantsSVG;
        private Wisej.Web.CheckBox chkSafeZone;
        private Wisej.Web.Label lblCombine;
        private Wisej.Web.CheckBox hideBG;
        private Wisej.Web.CheckBox chkSteam;
        private Wisej.Web.HtmlPanel combineDiagram;
        private Wisej.Web.Button closeDiagram;
        private Wisej.Web.CheckBox chkRemote;
        private Wisej.Web.CheckBox chkCombineLS;
        private Wisej.Web.Label lblUseOverjoyed;
        private Wisej.Web.CheckBox chkCombineLT;
        private Wisej.Web.CheckBox chkCombineRT;
        private Wisej.Web.CheckBox chkCombineRS;
        private Wisej.Web.CheckBox chkMouseLock;
        private Wisej.Web.Panel advancedPanel;
        private Wisej.Web.Panel pnlEverything;
        private Wisej.Web.Label lblQuickControls;
        private Wisej.Web.CheckBox showRightClickLabels;
        private Wisej.Web.CheckBox showLeftClickLabels;
        private Wisej.Web.CheckBox showMouseMoveLabels;
        private Wisej.Web.CheckBox chkKeepMouseInside;
        private Wisej.Web.Button btnZR;
        private Wisej.Web.Button btnZL;
        private Wisej.Web.ComboBox cmbArrowsGamepad;
        private Wisej.Web.ComboBox cmbArrowsKeyboard;
        private Wisej.Web.Button btnConfigSizeDown;
        private Wisej.Web.Button btnConfigSizeUp;
    }
}

