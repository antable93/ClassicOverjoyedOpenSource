using GregsStack.InputSimulatorStandard;
using GregsStack.InputSimulatorStandard.Native;
using ExtendedMessageBoxLibrary;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using SharedVars;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Wisej.Web;
using static OverjoyedReleaseLocal.DictionaryEnums;


namespace OverjoyedReleaseLocal
{
    public partial class Active : Page
    {
      
        private void pnlBaseOverlay_MouseDown(object sender, MouseEventArgs e)
        {
            if (positionCheck)
            {
                return;
            }

            if (IsHoldFriendlyMouseButton(e.Button) && IsLblActiveInactiveState())
            {
                if (!IsMoveMouseUsingLeftClicksEnabled())
                {
                    SetPointerGlow(e.Button == MouseButtons.Right ? "right" : "left", true);
                }
                StartLblActiveHoldAnimation();
                return;
            }

            if (IsHoldFriendlyMouseButton(e.Button)
                && isActive
                && chkDisableClicks.Checked
                && IsMoveMouseUsingLeftClicksEnabled())
            {
                StartLblActiveHoldAnimation();
                StartAltMouseDeactivateHold();
            }

            if (chkDisableClicks.Checked)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (IsMoveMouseUsingLeftClicksEnabled())
                    {
                        mousePosX = (int)(Cursor.Position.X / zoomFactor);
                        mousePosY = (int)(Cursor.Position.Y / zoomFactor);
                        xEnd = mousePosX;
                        yEnd = mousePosY;
                        SetOverlayPointerPosition(mousePosX, mousePosY);
                        QueueMouseMoveActions();
                    }

                    if (IsAltLeftClickEnabled() && !IsMoveMouseUsingLeftClicksEnabled())
                    {
                        localLeftPointerDown = true;
                        mouseDown("left");
                    }
                }
                else if (e.Button == MouseButtons.Right && IsAltRightClickEnabled() && !IsMoveMouseUsingLeftClicksEnabled())
                {
                    localRightPointerDown = true;
                    mouseDown("right");
                }

                return;
            }

            


            if (e.Button == MouseButtons.Left) //Btn A Down
            {
                localLeftPointerDown = true;
                mouseDown("left");

            }

            else if (e.Button == MouseButtons.Right) //Btn B Down
            {


                localRightPointerDown = true;

                mouseDown("right");

            }


        }

        private void StartAltMouseDeactivateHold()
        {
            CancelAltMouseDeactivateHold();

            altMouseDeactivateHoldCts = new CancellationTokenSource();
            var cts = altMouseDeactivateHoldCts;

            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(AltMouseDeactivateHoldMs, cts.Token);
                }
                catch (TaskCanceledException)
                {
                    return;
                }

                this.Invoke(new Action(() =>
                {
                    if (cts.IsCancellationRequested
                        || !isActive
                        || positionCheck
                        || chkRemote.Checked
                        || !chkDisableClicks.Checked
                        || !IsMoveMouseUsingLeftClicksEnabled()
                        || !IsPointInConfiguredDeactivateDeadZone(mousePosX, mousePosY))
                    {
                        return;
                    }

                    CancelLblActiveHoldAnimation();
                    SetPointerGlow("left", false);
                    SetPointerGlow("right", false);
                    localLeftPointerDown = false;
                    localRightPointerDown = false;
                    DeactivateFromExitHold(true);
                }));
            });
        }

        private void CancelAltMouseDeactivateHold()
        {
            if (altMouseDeactivateHoldCts != null)
            {
                altMouseDeactivateHoldCts.Cancel();
                altMouseDeactivateHoldCts.Dispose();
                altMouseDeactivateHoldCts = null;
            }

            CancelLblActiveHoldAnimation();
        }

        private bool IsPointInConfiguredDeactivateDeadZone(int endX, int endY)
        {
            float magnitudeX = Math.Abs(xStart - endX);
            float magnitudeY = Math.Abs(yStart - endY);
            double distance = Math.Sqrt(Math.Pow(magnitudeX, 2) + Math.Pow(magnitudeY, 2));

            if (distance > deadZone)
            {
                return false;
            }

            if (distance <= deadZone / 2)
            {
                return ActivateMiddle;
            }

            if ((yStart - endY) > 0)
            {
                return ActivateUpper;
            }

            return ActivateLower;
        }

        private void mouseDown(string mouseButton)
        {              
            if (mouseButton == "left")
            {
                if (positionCheck)
                {
                    //trigger left pointerdown Event in JS with remoteX/Y
                    Eval(@"const event = new MouseEvent('pointerdown', {
                        clientX: " + remoteX + @",
                        clientY: " + remoteY + @",
                        button: 0,
                        buttons: 1
                    });
                    document.querySelector('div[name=""pnlBaseOverlay""]').dispatchEvent(event);
                    ");
                }

                lastLeftMouseDownAt = DateTime.Now;

                SetPointerGlow("left", true);

                if (positionCheck)
                {
                    xEnd = (int)(remoteX);
                    yEnd = (int)(remoteY);
                }
                else if (chkDisableClicks.Checked)
                {
                    xEnd = mousePosX;
                    yEnd = mousePosY;
                }
                else
                {
                    xEnd = (int)(Cursor.Position.X / zoomFactor);
                    yEnd = (int)(Cursor.Position.Y / zoomFactor);
                }

                if (positionCheck && TryHandleRemoteMouseButtonAction(mouseButton, true))
                {
                    return;
                }

                float _magnitudeX = Math.Abs(xStart - xEnd);
                float _magnitudeY = Math.Abs(yStart - yEnd);
                double distance = Math.Sqrt(Math.Pow(_magnitudeX, 2) + Math.Pow(_magnitudeY, 2));

                if ((distance > deadZone) && isActive)

                {
                    if (!positionCheck)
                    {
                        if ((DateTime.Now - lastLclickTime).TotalMilliseconds < 200)
                        {
                            return;
                        }

                        lastLclickTime = DateTime.Now;
                    }

                    float _angle = (float)Math.Atan2((yEnd - yStart), (xEnd - xStart));

                    _angle = (float)(180 / Math.PI) * _angle;
                    _angle = SnapQuadrantBoundaryAngleToCardinal(_angle);

                    if (_angle >= -112.5 && _angle < -67.5)
                    {
                        c1 = 8; c2 = 8; c3 = 8;
                        if (chkMarioKart.Checked)
                        {
                            c1 = 8; c2 = 10; c3 = 8;
                        }

                        if (buttonConfig[c1][2] == "false")
                        {
                            ClickLoop(c1, c2, c3, false);
                        }


                    }
                    else if (_angle >= -67.5 && _angle < -22.5)
                    {

                        if (buttonConfig[c1][2] == "false" && diagUpRightLC)
                        {
                            c1 = 9; c2 = 8; c3 = 10;

                            ClickLoop(c1, c2, c3, false);

                        }
                        else if (buttonConfig[c1][2] == "false" && !diagUpRightLC)
                        {
                            c1 = 9; c2 = 9; c3 = 9;

                            ClickLoop(c1, c2, c3, false);
                        }

                    }
                    else if (_angle >= -22.5 && _angle < 22.5)
                    {
                        c1 = 10; c2 = 10; c3 = 10;
                        if (buttonConfig[c1][2] == "false")
                        {
                            ClickLoop(c1, c2, c3, false);
                        }
                    }
                    else if (_angle >= 22.5 && _angle < 67.5)
                    {
                        if (buttonConfig[c1][2] == "false" && diagDownRightLC)
                        {
                            c1 = 11; c2 = 10; c3 = 12;

                            ClickLoop(c1, c2, c3, false);
                        }
                        else if (buttonConfig[c1][2] == "false" && !diagDownRightLC)
                        {
                            c1 = 11; c2 = 11; c3 = 11;

                            ClickLoop(c1, c2, c3, false);
                        }
                    }

                    else if (_angle >= 67.5 && _angle < 112.5)
                    {
                        c1 = 12; c2 = 12; c3 = 12;

                        if (buttonConfig[c1][2] == "false")
                        {
                            ClickLoop(c1, c2, c3, false);
                        }

                    }
                    else if (_angle >= 112.5 && _angle < 157.5)
                    {
                        if (buttonConfig[c1][2] == "false" && diagDownLeftLC)
                        {
                            c1 = 13; c2 = 12; c3 = 14;

                            ClickLoop(c1, c2, c3, false);
                        }
                        else if (buttonConfig[c1][2] == "false" && !diagDownLeftLC)
                        {
                            c1 = 13; c2 = 13; c3 = 13;
                            ClickLoop(c1, c2, c3, false);
                        }
                    }
                    else if (_angle >= 157.5 || _angle < -157.5) //FLAGGED MIGHT BE WRONG
                    {
                        c1 = 14; c2 = 14; c3 = 14;
                        if (buttonConfig[c1][2] == "false")
                        {
                            ClickLoop(c1, c2, c3, false);
                        }

                    }
                    else if (_angle >= -157.5 && _angle < -112.5)
                    {
                        if (buttonConfig[c1][2] == "false" && diagUpLeftLC)
                        {
                            c1 = 15; c2 = 14; c3 = 8;

                            ClickLoop(c1, c2, c3, false);
                        }
                        else if (buttonConfig[c1][2] == "false" && !diagUpLeftLC)
                        {
                            c1 = 15; c2 = 15; c3 = 15;
                            ClickLoop(c1, c2, c3, false);
                        }
                    }
                }

                else if ((distance <= deadZone) && isActive)
                {

                    if ((distance > deadZone / 2 && ((yStart - yEnd) > 0)))
                    {
                        //if (isLeftMouseButtonDown == false && chkFPS.Checked)
                        //{

                        //    isLeftMouseButtonDown = true;
                        //    lblPrompts[0].Font = new System.Drawing.Font("Arial", 12, FontStyle.Bold);
                        //    lblPrompts[0].ForeColor = Color.FromArgb(0, 167, 209);
                        //}
                        //else if (isLeftMouseButtonDown == true && chkFPS.Checked)
                        //{
                        //    isLeftMouseButtonDown = false;
                        //    lblPrompts[0].ForeColor = Color.Black;
                        //    lblPrompts[0].Font = new System.Drawing.Font("Arial", 12, FontStyle.Regular);
                        //}

                        c1 = 25; c2 = 25; c3 = 25;
                        if (buttonConfig[c1][2] == "false")
                        {
                            ClickLoop(c1, c2, c3, false);
                        }

                    }
                    else if ((distance <= deadZone / 2))
                    {
                        //isLeftMouseButtonDown = false;
                        //lblPrompts[0].ForeColor = Color.Black; lblPrompts[0].Font = new System.Drawing.Font("Arial", 12, FontStyle.Regular);

                        c1 = 28; c2 = 28; c3 = 28;
                        if (buttonConfig[c1][2] == "false")
                        {
                            ClickLoop(c1, c2, c3, false);
                        }

                    }
                    else if (((yStart - yEnd) < 0) && (distance > deadZone / 2))
                    {
                        //isLeftMouseButtonDown = false; lblPrompts[0].ForeColor = Color.Black; lblPrompts[0].Font = new System.Drawing.Font("Arial", 12, FontStyle.Regular);

                        c1 = 31; c2 = 31; c3 = 31;
                        if (buttonConfig[c1][2] == "false")
                        {
                            ClickLoop(c1, c2, c3, false);
                        }


                    }



                }

            }
            else if (mouseButton == "right")
            {
                //trigger right pointerdown Event in JS with remoteX/Y
                if (positionCheck)
                {
                    Eval(@"const event = new MouseEvent('pointerdown', {
                        clientX: " + remoteX + @",
                        clientY: " + remoteY + @",
                        button: 2,
                        buttons: 2
                    });
                    document.querySelector('div[name=""pnlBaseOverlay""]').dispatchEvent(event);
                    ");
                }

                DateTime currentRightMouseDownAt = DateTime.Now;
                rightLookModeLatched = (currentRightMouseDownAt - lastRightMouseDownAt).TotalMilliseconds <= DoubleClickThreshold;
                lastRightMouseDownAt = currentRightMouseDownAt;

                SetPointerGlow("right", true);
                if (positionCheck)
                {
                    xEnd = (int)(remoteX);
                    yEnd = (int)(remoteY);
                }
                else if (chkDisableClicks.Checked)
                {
                    xEnd = mousePosX;
                    yEnd = mousePosY;
                }
                else
                {

                    xEnd = (int)(Cursor.Position.X / zoomFactor);
                    yEnd = (int)(Cursor.Position.Y / zoomFactor);
                }

                if (positionCheck && TryHandleRemoteMouseButtonAction(mouseButton, true))
                {
                    return;
                }

                float _magnitudeX = Math.Abs(xStart - xEnd);
                float _magnitudeY = Math.Abs(yStart - yEnd);
                double distance = Math.Sqrt(Math.Pow(_magnitudeX, 2) + Math.Pow(_magnitudeY, 2));

        //        if ((DateTime.Now - _lastClickTime).TotalMilliseconds <= DoubleClickThreshold &&
        //_lastButtonClicked == e.Button)
        //        {


        //            pnlDoubleClick();


        //            // Reset the last click time to prevent additional double-click detections
        //            _lastClickTime = DateTime.MinValue;
        //        }
        //        else
        //        {
        //            // Store the time and button for the first click
        //            _lastClickTime = DateTime.Now;
        //            _lastButtonClicked = e.Button;
        //        }

                if ((distance > deadZone) && isActive)

                {
                    if (!positionCheck)
                    {
                        if ((DateTime.Now - lastRclickTime).TotalMilliseconds < 200)
                        {
                            return;
                        }

                        lastRclickTime = DateTime.Now;
                    }

                    float _angle = (float)Math.Atan2((yEnd - yStart), (xEnd - xStart));

                    _angle = (float)(180 / Math.PI) * _angle;
                    _angle = SnapQuadrantBoundaryAngleToCardinal(_angle);

                    if (_angle >= -112.5 && _angle < -67.5)
                    {
                        c1 = 16; c2 = 16; c3 = 16;
                        if (buttonConfig[c1][2] == "false")
                        {
                            ClickLoop(c1, c2, c3, false);
                        }

                    }
                    else if (_angle >= -67.5 && _angle < -22.5)
                    {

                        if (buttonConfig[c1][2] == "false" && diagUpRightRC)
                        {
                            c1 = 17; c2 = 16; c3 = 18;
                            ClickLoop(c1, c2, c3, false);
                        }
                        else if (buttonConfig[c1][2] == "false" && !diagUpRightRC)
                        {
                            c1 = 17; c2 = 17; c3 = 17;
                            ClickLoop(c1, c2, c3, false);
                        }

                    }
                    else if (_angle >= -22.5 && _angle < 22.5)
                    {
                        c1 = 18; c2 = 18; c3 = 18;
                        if (buttonConfig[c1][2] == "false")
                        {
                            ClickLoop(c1, c2, c3, false);
                        }
                    }
                    else if (_angle >= 22.5 && _angle < 67.5)
                    {

                        if (buttonConfig[c1][2] == "false" && diagDownRightRC)
                        {
                            c1 = 19; c2 = 18; c3 = 20;
                            ClickLoop(c1, c2, c3, false);
                        }
                        else if (buttonConfig[c1][2] == "false" && !diagDownRightRC)
                        {
                            c1 = 19; c2 = 19; c3 = 19;
                            ClickLoop(c1, c2, c3, false);
                        }
                    }

                    else if (_angle >= 67.5 && _angle < 112.5)
                    {
                        c1 = 20; c2 = 20; c3 = 20;
                        if (buttonConfig[c1][2] == "false")
                        {
                            ClickLoop(c1, c2, c3, false);
                        }

                    }
                    else if (_angle >= 112.5 && _angle < 157.5)
                    {

                        if (buttonConfig[c1][2] == "false" && diagDownLeftRC)
                        {
                            c1 = 21; c2 = 20; c3 = 22;
                            ClickLoop(c1, c2, c3, false);
                        }
                        else if (buttonConfig[c1][2] == "false" && !diagDownLeftRC)
                        {
                            c1 = 21; c2 = 21; c3 = 21;
                            ClickLoop(c1, c2, c3, false);
                        }
                    }
                    else if (_angle >= 157.5 || _angle < -157.5) //FLAGGED MIGHT BE WRONG
                    {
                        c1 = 22; c2 = 22; c3 = 22;
                        if (buttonConfig[c1][2] == "false")
                        {
                            ClickLoop(c1, c2, c3, false);
                        }

                    }
                    else if (_angle >= -157.5 && _angle < -112.5)
                    {

                        if (buttonConfig[c1][2] == "false" && diagUpLeftRC)
                        {
                            c1 = 23; c2 = 22; c3 = 16;
                            ClickLoop(c1, c2, c3, false);
                        }
                        else if (buttonConfig[c1][2] == "false" && !diagUpLeftRC)
                        {
                            c1 = 23; c2 = 23; c3 = 23;
                            ClickLoop(c1, c2, c3, false);
                        }
                    }
                }
                else if ((distance <= deadZone) && isActive)
                {

                    if ((distance > deadZone / 2 && ((yStart - yEnd) > 0)))
                    {

                        c1 = 26; c2 = 26; c3 = 26;
                        if (buttonConfig[c1][2] == "false")
                        {
                            ClickLoop(c1, c2, c3, false);
                        }

                    }
                    else if ((distance <= deadZone / 2))
                    {

                        c1 = 29; c2 = 29; c3 = 29;
                        if (buttonConfig[c1][2] == "false")
                        {
                            ClickLoop(c1, c2, c3, false);
                        }

                    }
                    else if (((yStart - yEnd) < 0) && (distance > deadZone / 2))
                    {
                        c1 = 32; c2 = 32; c3 = 32;
                        if (buttonConfig[c1][2] == "false")
                        {
                            ClickLoop(c1, c2, c3, false);
                        }


                    }




                }


            }
        }

        public Active()
        {
            InitializeComponent();
            _currentInstance = this;
            InitializeNintendoStreamerPlayerDropdown();
            InitializeAlternativeMouseControlsOptions();
        }

        private bool IsMoveMouseUsingLeftClicksEnabled()
        {
            return chkMoveMouseUsingLeftClicks?.Checked == true;
        }

        private bool IsAltLeftClickEnabled()
        {
            return chkAltLeftClick == null || chkAltLeftClick.Checked;
        }

        private bool IsAltRightClickEnabled()
        {
            return chkAltRightClick == null || chkAltRightClick.Checked;
        }

        private static int GetAssignedPlayerNum()
        {
            return PlayerAssignment.GetAssignedPlayerOrDefault();
        }

        public int GetEffectiveNintendoStreamerPlayer()
        {
            if (cmbNintendoStreamerPlayer != null &&
                cmbNintendoStreamerPlayer.Visible &&
                int.TryParse(cmbNintendoStreamerPlayer.Text, out int selectedPlayer) &&
                selectedPlayer >= 1 && selectedPlayer <= 4)
            {
                return selectedPlayer;
            }

            return playerNum;
        }

        public bool UsesNintendoStreamerPlayerRouting()
        {
            return switchMode && cmbNintendoStreamerPlayer != null && cmbNintendoStreamerPlayer.Visible;
        }

        public string GetEffectiveNintendoStreamerPlayerStatusText(bool isActiveStatus)
        {
            return "Overjoyed Remote Player " + GetEffectiveNintendoStreamerPlayer() + (isActiveStatus ? " is active." : " is inactive.");
        }

        private void InitializeNintendoStreamerPlayerDropdown()
        {
            cmbNintendoStreamerPlayer = new ComboBox
            {
                AutoSize = false,
                BackColor = Color.LightCyan,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Regular, GraphicsUnit.Point),
                Location = new Point(373, 10),
                MaximumSize = new Size(72, 20),
                MinimumSize = new Size(72, 20),
                Name = "cmbNintendoStreamerPlayer",
                Size = new Size(72, 20),
                TabIndex = 46,
                Visible = false
            };

            cmbNintendoStreamerPlayer.Items.AddRange(new object[] { "1", "2", "3", "4" });
            cmbNintendoStreamerPlayer.SelectedIndexChanged += cmbNintendoStreamerPlayer_SelectedIndexChanged;

            btnNintendoStreamerX = new Button
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                AutoSize = false,
                BackColor = Color.FromArgb(255, 255, 255),
                Cursor = Cursors.Hand,
                Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold, GraphicsUnit.Point),
                ForeColor = Color.FromArgb(0, 0, 0),
                Location = new Point(449, 10),
                MaximumSize = new Size(24, 20),
                MinimumSize = new Size(24, 20),
                Name = "btnNintendoStreamerX",
                Size = new Size(24, 20),
                TabIndex = 47,
                TabStop = false,
                Text = "X",
                Visible = false
            };
            btnNintendoStreamerX.Click += btnNintendoStreamerX_Click;
            btnNintendoStreamerX.MouseEnter += btnSettings_MouseEnter;
            btnNintendoStreamerX.MouseLeave += btnSettings_MouseLeave;

            btnNintendoStreamerY = new Button
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                AutoSize = false,
                BackColor = Color.FromArgb(255, 255, 255),
                Cursor = Cursors.Hand,
                Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold, GraphicsUnit.Point),
                ForeColor = Color.FromArgb(0, 0, 0),
                Location = new Point(477, 10),
                MaximumSize = new Size(24, 20),
                MinimumSize = new Size(24, 20),
                Name = "btnNintendoStreamerY",
                Size = new Size(24, 20),
                TabIndex = 48,
                TabStop = false,
                Text = "Y",
                Visible = false
            };
            btnNintendoStreamerY.Click += btnNintendoStreamerY_Click;
            btnNintendoStreamerY.MouseEnter += btnSettings_MouseEnter;
            btnNintendoStreamerY.MouseLeave += btnSettings_MouseLeave;

            pnlExtraButtons.Controls.Add(cmbNintendoStreamerPlayer);
            pnlExtraButtons.Controls.Add(btnNintendoStreamerX);
            pnlExtraButtons.Controls.Add(btnNintendoStreamerY);
            cmbNintendoStreamerPlayer.BringToFront();
            btnNintendoStreamerX.BringToFront();
            btnNintendoStreamerY.BringToFront();
        }

        private void InitializeAlternativeMouseControlsOptions()
        {
            chkMoveMouseUsingLeftClicks = new CheckBox
            {
                AutoSize = false,
                BackColor = Color.FromArgb(0, 0, 0),
                CssStyle = "border-radius:5px;",
                Cursor = Cursors.Hand,
                Focusable = false,
                Font = new Font("default", 9.5F, FontStyle.Regular, GraphicsUnit.Point),
                ForeColor = Color.FromArgb(255, 255, 255),
                Location = new Point(9, 10),
                Name = "chkMoveMouseUsingLeftClicks",
                Size = new Size(210, 23),
                TabIndex = 49,
                Text = "Move Mouse Using Left Clicks",
                Visible = false
            };
            chkMoveMouseUsingLeftClicks.CheckedChanged += chkMoveMouseUsingLeftClicks_CheckedChanged;

            chkAltLeftClick = new CheckBox
            {
                AutoSize = false,
                BackColor = Color.FromArgb(0, 0, 0),
                CheckState = CheckState.Checked,
                CssStyle = "border-radius:5px;",
                Cursor = Cursors.Hand,
                Focusable = false,
                Font = new Font("default", 9.5F, FontStyle.Regular, GraphicsUnit.Point),
                ForeColor = Color.FromArgb(255, 255, 255),
                Location = new Point(223, 10),
                Name = "chkAltLeftClick",
                Size = new Size(100, 23),
                TabIndex = 50,
                Text = "Alt Left Click",
                Visible = false
            };
            chkAltLeftClick.CheckedChanged += chkAltLeftClick_CheckedChanged;

            chkAltRightClick = new CheckBox
            {
                AutoSize = false,
                BackColor = Color.FromArgb(0, 0, 0),
                CheckState = CheckState.Checked,
                CssStyle = "border-radius:5px;",
                Cursor = Cursors.Hand,
                Focusable = false,
                Font = new Font("default", 9.5F, FontStyle.Regular, GraphicsUnit.Point),
                ForeColor = Color.FromArgb(255, 255, 255),
                Location = new Point(327, 10),
                Name = "chkAltRightClick",
                Size = new Size(110, 23),
                TabIndex = 51,
                Text = "Alt Right Click",
                Visible = false
            };
            chkAltRightClick.CheckedChanged += chkAltRightClick_CheckedChanged;

            pnlExtraButtons.Controls.Add(chkMoveMouseUsingLeftClicks);
            pnlExtraButtons.Controls.Add(chkAltLeftClick);
            pnlExtraButtons.Controls.Add(chkAltRightClick);

            chkMoveMouseUsingLeftClicks.BringToFront();
            chkAltLeftClick.BringToFront();
            chkAltRightClick.BringToFront();

            ApplyAlternativeMouseControlsOptionRules();
        }

        private void UpdateAlternativeMouseControlsOptionsVisibility()
        {
            bool visible = chkDisableClicks.Checked;

            if (chkMoveMouseUsingLeftClicks != null)
            {
                chkMoveMouseUsingLeftClicks.Visible = visible;
            }

            if (chkAltLeftClick != null)
            {
                chkAltLeftClick.Visible = visible;
            }

            if (chkAltRightClick != null)
            {
                chkAltRightClick.Visible = visible;
            }

            if (visible)
            {
                lblQuickControls.Visible = false;
            }

            ApplyAlternativeMouseControlsOptionRules();
        }

        private void chkMoveMouseUsingLeftClicks_CheckedChanged(object sender, EventArgs e)
        {
            ApplyAlternativeMouseControlsOptionRules();
            drawPrompts();
            RefreshOverlayPointerState();
            QueueMouseMoveActions();
        }

        private void chkAltLeftClick_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAltLeftClick != null && chkAltLeftClick.Checked)
            {
                ShowAlternativeMouseHotkeyDialog("Left", "- (Minus)");
            }
        }

        private void chkAltRightClick_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAltRightClick != null && chkAltRightClick.Checked)
            {
                ShowAlternativeMouseHotkeyDialog("Right", "+ (Plus)");
            }
        }

        private void ShowAlternativeMouseHotkeyDialog(string clickName, string keyName)
        {
            resetTransparency(false, true);
            ExtendedMessageBox.Show(
                "<p style='margin:0;padding:0;line-height:1.6;'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 1; -webkit-box-orient: vertical; overflow: hidden; font-size:" + truncateSize + "pt; ' title='With this enabled, use the " + keyName + " key for " + clickName + " Click actions.'>With this enabled, use the <b>" + keyName + "</b> key for <b>" + clickName + " Click</b> actions.</span></p>",
                "Alternative Mouse Controls Hotkey",
                ExtendedMessageBoxLibrary.MessageBoxButtons.OK,
                lines: 1,
                zoomFactor: zoomFactor);
            resetTransparency(true, true);
        }

        private void ApplyAlternativeMouseControlsOptionRules()
        {
            if (chkMoveMouseUsingLeftClicks == null || chkAltLeftClick == null)
            {
                return;
            }

            if (chkMoveMouseUsingLeftClicks.Checked)
            {
                lblQuickControls.Visible = false;
                chkAltLeftClick.Checked = true;
                chkAltLeftClick.Enabled = false;
            }
            else
            {
                chkAltLeftClick.Enabled = true;
            }
        }

        private void UpdateNintendoStreamerPlayerDropdown()
        {
            if (cmbNintendoStreamerPlayer == null)
            {
                return;
            }

            bool showDropdown = switchMode && IsNintendoStreamerDropdownEnabled();
            cmbNintendoStreamerPlayer.Visible = showDropdown;
            UpdateNintendoStreamerQuickButtons(showDropdown);

            if (!showDropdown)
            {
                return;
            }

            int selectedPlayer = ReadNintendoStreamerPlayerSelection();

            suppressNintendoStreamerPlayerChanged = true;
            try
            {
                cmbNintendoStreamerPlayer.Text = selectedPlayer.ToString();
            }
            finally
            {
                suppressNintendoStreamerPlayerChanged = false;
            }

            RefreshObsNintendoRoutingIfNeeded();
        }

        private void UpdateNintendoStreamerQuickButtons(bool visible)
        {
            if (btnNintendoStreamerX != null)
            {
                btnNintendoStreamerX.Visible = visible;
            }

            if (btnNintendoStreamerY != null)
            {
                btnNintendoStreamerY.Visible = visible;
            }
        }

        private void RefreshObsNintendoRoutingIfNeeded()
        {
            int currentRoutePlayer = UsesNintendoStreamerPlayerRouting()
                ? GetEffectiveNintendoStreamerPlayer()
                : 0;

            if (currentRoutePlayer == lastObsNintendoRoutePlayer)
            {
                return;
            }

            lastObsNintendoRoutePlayer = currentRoutePlayer;

            if (!OBSWebSocket.IsRunning)
            {
                return;
            }

            OBSWebSocket.Stop();
            OBSWebSocket.Initialize(this);
            _ = OBSWebSocket.StartAsync();
        }

        private bool IsNintendoStreamerDropdownEnabled()
        {
            try
            {
                var streamerModePath = Path.Combine(configPath, "StreamerMode.txt");

                if (!File.Exists(streamerModePath))
                {
                    return false;
                }

                using var fs = new FileStream(streamerModePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = new StreamReader(fs);
                return !string.IsNullOrWhiteSpace(reader.ReadLine());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading StreamerMode.txt for Nintendo dropdown: {ex.Message}");
                return false;
            }
        }

        private int ReadNintendoStreamerPlayerSelection()
        {
            try
            {
                string filePath = GetNintendoStreamerPlayerSelectionPath();

                if (!File.Exists(filePath))
                {
                    return playerNum;
                }

                using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = new StreamReader(fs);
                string? rawValue = reader.ReadLine();

                if (int.TryParse(rawValue, out int selectedPlayer) && selectedPlayer >= 1 && selectedPlayer <= 4)
                {
                    return selectedPlayer;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading Nintendo streamer selection: {ex.Message}");
            }

            return playerNum;
        }

        private string GetNintendoStreamerPlayerSelectionPath()
        {
            return Path.Combine(configPath, $"nintendo{playerNum}.txt");
        }

        private void PersistNintendoStreamerPlayerSelection()
        {
            if (cmbNintendoStreamerPlayer == null || !int.TryParse(cmbNintendoStreamerPlayer.Text, out int selectedPlayer))
            {
                return;
            }

            string valueToPersist = selectedPlayer == playerNum ? string.Empty : selectedPlayer.ToString();

            try
            {
                using var fs = new FileStream(GetNintendoStreamerPlayerSelectionPath(), FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                using var writer = new StreamWriter(fs);
                writer.WriteLine(valueToPersist);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error writing Nintendo streamer selection: {ex.Message}");
            }
        }

        public void ClearNintendoStreamerPlayerSelection()
        {
            try
            {
                using var fs = new FileStream(GetNintendoStreamerPlayerSelectionPath(), FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                using var writer = new StreamWriter(fs);
                writer.WriteLine(string.Empty);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error clearing Nintendo streamer selection: {ex.Message}");
            }
        }

        private void cmbNintendoStreamerPlayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (suppressNintendoStreamerPlayerChanged)
            {
                return;
            }

            PersistNintendoStreamerPlayerSelection();

            if (chkRemote.Checked)
            {
                if (isActive)
                {
                    SetLblActiveActiveUi(GetEffectiveNintendoStreamerPlayerStatusText(true));
                }
                else
                {
                    SetLblActiveInactiveUi(GetEffectiveNintendoStreamerPlayerStatusText(false));
                }
            }

            RefreshObsNintendoRoutingIfNeeded();
        }

        private void TimerTick(object sender, EventArgs e)
        {

            if (toggleRemote) { toggleRemote = false; Debug.WriteLine("Toggle Remote Triggered");

                string toggleContent = System.IO.File.ReadAllText(Path.Combine(configPath, "ToggleRemote.txt"));

                if (!toggleContent.StartsWith(playerNum.ToString())&& chkRemote.Checked)
                {
                    btnSwitchHome.PerformClick();
                } 
            }

            if (combineControllers && scanning)
            {
                SetLblActiveInactiveUi("Gamepad detection started. Press A button on your controller.");
            }

            tailscale = chkAltRTC.Checked;
      

            if (!positionCheck)
            {
                QueueMouseMoveActions();
            }





        }

 
        private void HandleMessage(string message)
        {
            // Key payloads can be tagged as "payload|playerNum"; normalize before hotkey checks.
            string payload = message;
            int? targetPlayer = null;

            int sep = message.LastIndexOf('|');
            if (sep > 0 && sep < message.Length - 1)
            {
                payload = message.Substring(0, sep);
                if (int.TryParse(message.Substring(sep + 1), out var pn) && pn >= 1 && pn <= 4)
                    targetPlayer = pn;
            }

            if (targetPlayer.HasValue && targetPlayer.Value != playerNum)
                return;

            Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(() =>
            {
                if (payload.StartsWith("key"))
                {
                    if (payload != previousMessage)
                    {


                        previousMessage = payload;

                        Task.Run(() =>
                        {


                            if (positionCheck)
                            {

                                mousePosX = (int)(remoteX);
                                mousePosY = (int)(remoteY);

                                xEnd = (int)(remoteX);
                                yEnd = (int)(remoteY);
                            }

                            else
                            {
                                if (!chkDisableClicks.Checked || !IsMoveMouseUsingLeftClicksEnabled())
                                {
                                    mousePosX = (int)(Cursor.Position.X / zoomFactor);
                                    mousePosY = (int)(Cursor.Position.Y / zoomFactor);

                                    xEnd = (int)(Cursor.Position.X / zoomFactor);
                                    yEnd = (int)(Cursor.Position.Y / zoomFactor);
                                }
                                else
                                {
                                    xEnd = mousePosX;
                                    yEnd = mousePosY;
                                }
                            }

                            float _magnitudeX = Math.Abs(xStart - xEnd);
                            float _magnitudeY = Math.Abs(yStart - yEnd);
                            double distance = Math.Sqrt(Math.Pow(_magnitudeX, 2) + Math.Pow(_magnitudeY, 2));

                            float _angle = (float)Math.Atan2((yEnd - yStart), (xEnd - xStart));

                            _angle = (float)(180 / Math.PI) * _angle;

                            HandleHotkeyPayload(payload);

                        });


                    }
                }
                else if (payload == "resize")
                {
                    browserWidth = this.Width;
                    double browserHeight = this.Height;

                    float scale = (float)(browserWidth / oldWindowWidth);
                    Debug.WriteLine(browserWidth.ToString());
                    Debug.WriteLine(oldWindowWidth.ToString());

                    chkSteam.Scale(scale);
                    chkCombineLS.Scale(scale);
                    chkCombineRS.Scale(scale);
                    chkCombineLT.Scale(scale);
                    chkCombineRT.Scale(scale);
                    lblUseOverjoyed.Scale(scale);
                    lblCombine.Scale(scale);
                    closeDiagram.Scale(scale);
                    menuBar.Scale(scale);
                    this.Controls["viewPanel"].Scale(scale);
                    pnlPrompts.Scale(scale);
                    pnlDraw.Scale(scale);
                    combineDiagram.Scale(scale);
                    quadrantsSVG.Scale(scale);
                    pnlExtraButtons.Scale(scale);
                    advancedPanel.Scale(scale);

                    ScaleFonts(closeDiagram, scale);
                    ScaleFonts(chkSteam, scale);
                    ScaleFonts(chkCombineLS, scale);
                    ScaleFonts(chkCombineRS, scale);
                    ScaleFonts(chkCombineLT, scale);
                    ScaleFonts(chkCombineRT, scale);
                    ScaleFonts(lblUseOverjoyed, scale);
                    ScaleFonts(lblCombine, scale);
                    ScaleFonts(pnlPrompts, scale);
                    ScaleFonts(combineDiagram, scale);
                    ScaleFonts(pnlDraw, scale);
                    ScaleFonts(quadrantsSVG, scale);
                    ScaleFonts(pnlExtraButtons, scale);
                    ScaleFonts(advancedPanel, scale);
                    ScaleFonts(menuBar, scale);
                    ScaleFonts(this.Controls["viewPanel"], scale);
                    oldWindowWidth = browserWidth;

                    // Calculate zoomFactor...
                    zoomFactor = 1.0 - (double)((formWidth - browserWidth) / formWidth);

                    btnSettings.Text = "<p style='line-height:1.2;padding:0;margin:0;'><span style='-webkit-line-clamp:3;font-size:" + (truncateSize * zoomFactor).ToString(System.Globalization.CultureInfo.InvariantCulture) + "pt;' class='truncate' title='Change Overjoyed Settings'>Change Overjoyed Settings</span></p>";

                    double currentFont = 12 * zoomFactor;
                    if (Screen.Bounds.Height >= 1100 && Screen.Bounds.Width > 1920)
                    {
                        currentFont = 16 * zoomFactor;
                    }

                    Debug.WriteLine(currentFont.ToString());

                    Eval(@"function updateTruncateCSS(scale = 1) {
          const styleId = 'dynamic-truncate-style';
          const oldStyle = document.getElementById(styleId);

          let currentFontSize = 12;
          if (oldStyle) {
            const match = oldStyle.textContent.match(/font-size:\s*([\d.]+)pt/);
            if (match) currentFontSize = parseFloat(match[1]);
            oldStyle.remove();
          }

          const newFontSize = " + currentFont.ToString(System.Globalization.CultureInfo.InvariantCulture) + @";

          const style = document.createElement('style');
          style.id = styleId;
          style.textContent = `
            .truncate {
              font-size: ${newFontSize}pt!important;
            }
          `;
          document.head.appendChild(style);
        }updateTruncateCSS(1)");
                }
                else if (payload == "isLocked")
                {
                    chkStreamer.Enabled = false;
                    pnlEverything.ScrollBars = ScrollBars.Vertical;
                    pnlEverything.AutoScroll = true;
                    pnlEverything.AutoScrollMinSize = new Size(0, pnlEverything.Height + 1);
                    //force a scrollbar
                }
                else if (payload == "xbox-off")
                {
                    pnlBaseOverlay.Enabled = false;
                    pnlExtraButtons.Enabled = false;
                }
                else if (payload == "xbox-on")
                {
                    pnlBaseOverlay.Enabled = true;
                    ApplyActivationAvailabilityGate();
                    resetTransparency(true);
                }
                else if (payload == "ds4-off")
                {
                    pnlBaseOverlay.Enabled = false;
                    pnlExtraButtons.Enabled = false;
                }
                else if (payload == "ds4-on")
                {
                    pnlBaseOverlay.Enabled = true;
                    ApplyActivationAvailabilityGate();
                    resetTransparency(true);
                }
            });
        }

        private void AssignKeyCodes()
        {
            // Initialize variables
            int counter = 0;
            previousMode = xboxMode ? "Xbox" : switchMode ? "Switch" : keyboardMode ? "Keyboard" : "";

            keyCodes.Clear();

            try
            {

                // Read the parameters file with shared read access
                using (FileStream fs = new FileStream(parametersFilePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader reader = new StreamReader(fs))
                {
                    var parameterLines = reader.ReadToEnd().Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                    buttonMap = int.Parse(parameterLines.ElementAtOrDefault(0) ?? "0");

                    string line = parameterLines.ElementAtOrDefault(1) ?? "";
                    string[] fillConfigs = line.Split(',');

                    string configName = buttonMap == 0 ? "Music.txt" : fillConfigs[buttonMap - 1] + ".txt";
                    string configFilePath = Path.Combine(profilesPath, configName);


                    // Read the configuration file with shared read access
                    using (FileStream configFs = new FileStream(configFilePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
                    using (StreamReader configReader = new StreamReader(configFs))
                    {
                        while ((line = configReader.ReadLine()) != null)
                        {
                            VirtualKeyCode code;

                            if (counter <= 32)
                            {
                                kbButtons[counter] = new List<string>(line.Split(','));

                                line = line.ToLower();
                                buttonConfig[counter] = new List<string>(line.Split(','));
                                if (buttonConfig[counter][0] == "quote")
                                {
                                    code = VirtualKeyCode.OEM_7;
                                }
                                else if (buttonConfig[counter][0] == "semicolon")
                                {
                                    code = VirtualKeyCode.OEM_1;
                                }
                                else if (buttonConfig[counter][0] == "slash")
                                {
                                    code = VirtualKeyCode.OEM_2;
                                }
                                else if (buttonConfig[counter][0] == "backtick")
                                {
                                    code = VirtualKeyCode.OEM_3;
                                }
                                else if (buttonConfig[counter][0] == "lbracket")
                                {
                                    code = VirtualKeyCode.OEM_4;
                                }
                                else if (buttonConfig[counter][0] == "backslash")
                                {
                                    code = VirtualKeyCode.OEM_5;
                                }
                                else if (buttonConfig[counter][0] == "rbracket")
                                {
                                    code = VirtualKeyCode.OEM_6;
                                }
                                else if (buttonConfig[counter][0] == "win")
                                {
                                    code = VirtualKeyCode.LWIN;
                                }
                                else if (buttonConfig[counter][0] == "comma")
                                {
                                    code = VirtualKeyCode.OEM_COMMA;
                                }
                                else if (buttonConfig[counter][0] == "period")
                                {
                                    code = VirtualKeyCode.OEM_PERIOD;
                                }
                                else if (buttonConfig[counter][0] == "equal")
                                {
                                    code = VirtualKeyCode.OEM_PLUS;
                                }
                                else if (buttonConfig[counter][0] == "minus")
                                {
                                    code = VirtualKeyCode.OEM_MINUS;
                                }
                                else if (buttonConfig[counter][0] == "capslk")
                                {
                                    code = VirtualKeyCode.CAPITAL;
                                }
                                else if (buttonConfig[counter][0] == "esc")
                                {
                                    code = VirtualKeyCode.ESCAPE;
                                }
                                else if (buttonConfig[counter][0] == "pageup")
                                {
                                    code = VirtualKeyCode.PRIOR;
                                }
                                else
                                {
                                    strToKey.TryGetValue(buttonConfig[counter][0], out code);
                                }

                                keyCodes.Add(code);
                                xbuttons[counter] = kbButtons[counter][3];
                                switchbuttons[counter] = kbButtons[counter][6];

                                if (counter < 8)
                                {
                                    if (kbButtons[counter][1] != "True" && kbButtons[counter][2] == "False")
                                    {

                                        releaseKBHover.Add(keyCodes[counter]);
                                        if (kbButtons[counter][3] != "None")
                                        {
                                            releaseXboxHover.Add(kbButtons[counter][3]);
                                        }
                                        if (kbButtons[counter][6] != "None")
                                        {
                                            releaseSwitchHover.Add(kbButtons[counter][6]);
                                        }
                                    }
                                }
                                if (counter < 24 && counter > 7)
                                {
                                    if (kbButtons[counter][1] != "True" && kbButtons[counter][2] == "False")
                                    {
                                        releaseKBClick.Add(keyCodes[counter]);
                                        if (kbButtons[counter][3] != "None")
                                        {
                                            releaseXboxClick.Add(kbButtons[counter][3]);
                                        }
                                        if (kbButtons[counter][6] != "None")
                                        {
                                            releaseSwitchClick.Add(kbButtons[counter][6]);
                                        }
                                    }
                                }
                                if (counter == 24)
                                {
                                    if (kbButtons[counter][1] != "True" && kbButtons[24][2] == "False")
                                    {
                                        releaseKBHover.Add(keyCodes[24]);
                                        if (kbButtons[counter][3] != "None")
                                        {
                                            releaseXboxHover.Add(kbButtons[24][3]);
                                        }
                                        if (kbButtons[counter][6] != "None")
                                        {
                                            releaseSwitchHover.Add(kbButtons[counter][6]);
                                        }
                                    }
                                }
                                if (counter == 27)
                                {
                                    if (kbButtons[counter][1] != "True" && kbButtons[27][2] == "False")
                                    {
                                        releaseKBHover.Add(keyCodes[27]);
                                        if (kbButtons[counter][3] != "None")
                                        {
                                            releaseXboxHover.Add(kbButtons[27][3]);
                                        }
                                        if (kbButtons[counter][6] != "None")
                                        {
                                            releaseSwitchHover.Add(kbButtons[counter][6]);
                                        }
                                    }
                                }
                                if (counter == 30)
                                {
                                    if (kbButtons[counter][1] != "True" && kbButtons[30][2] == "False")
                                    {
                                        releaseKBHover.Add(keyCodes[30]);
                                        if (kbButtons[counter][3] != "None")
                                        {
                                            releaseXboxHover.Add(kbButtons[30][3]);
                                        }
                                        if (kbButtons[counter][6] != "None")
                                        {
                                            releaseSwitchHover.Add(kbButtons[counter][6]);
                                        }
                                    }
                                }
                                if (counter == 25)
                                {
                                    if (kbButtons[counter][1] != "True" && kbButtons[counter][2] == "False")
                                    {
                                        releaseKBClick.Add(keyCodes[counter]);
                                        if (kbButtons[counter][3] != "None")
                                        {
                                            releaseXboxClick.Add(kbButtons[counter][3]);
                                        }
                                        if (kbButtons[counter][6] != "None")
                                        {
                                            releaseSwitchClick.Add(kbButtons[counter][6]);
                                        }
                                    }
                                }
                                if (counter == 28)
                                {
                                    if (kbButtons[counter][1] != "True" && kbButtons[counter][2] == "False")
                                    {
                                        releaseKBClick.Add(keyCodes[counter]);
                                        if (kbButtons[counter][3] != "None")
                                        {
                                            releaseXboxClick.Add(kbButtons[counter][3]);
                                        }
                                        if (kbButtons[counter][6] != "None")
                                        {
                                            releaseSwitchClick.Add(kbButtons[counter][6]);
                                        }
                                    }
                                }
                                if (counter == 31)
                                {
                                    if (kbButtons[counter][1] != "True" && kbButtons[counter][2] == "False")
                                    {
                                        releaseKBClick.Add(keyCodes[counter]);
                                        if (kbButtons[counter][3] != "None")
                                        {
                                            releaseXboxClick.Add(kbButtons[counter][3]);
                                        }
                                        if (kbButtons[counter][6] != "None")
                                        {
                                            releaseSwitchClick.Add(kbButtons[counter][6]);
                                        }
                                    }
                                }
                                if (counter == 26)
                                {
                                    if (kbButtons[counter][1] != "True" && kbButtons[counter][2] == "False")
                                    {
                                        releaseKBClick.Add(keyCodes[counter]);
                                        if (kbButtons[counter][3] != "None")
                                        {
                                            releaseXboxClick.Add(kbButtons[counter][3]);
                                        }
                                        if (kbButtons[counter][6] != "None")
                                        {
                                            releaseSwitchClick.Add(kbButtons[counter][6]);
                                        }
                                    }
                                }
                                if (counter == 29)
                                {
                                    if (kbButtons[counter][1] != "True" && kbButtons[counter][2] == "False")
                                    {
                                        releaseKBClick.Add(keyCodes[counter]);
                                        if (kbButtons[counter][3] != "None")
                                        {
                                            releaseXboxClick.Add(kbButtons[counter][3]);
                                        }
                                        if (kbButtons[counter][6] != "None")
                                        {
                                            releaseSwitchClick.Add(kbButtons[counter][6]);
                                        }
                                    }
                                }
                                if (counter == 32)
                                {
                                    if (kbButtons[counter][1] != "True" && kbButtons[counter][2] == "False")
                                    {
                                        releaseKBClick.Add(keyCodes[counter]);
                                        if (kbButtons[counter][3] != "None")
                                        {
                                            releaseXboxClick.Add(kbButtons[counter][3]);
                                        }
                                        if (kbButtons[counter][6] != "None")
                                        {
                                            releaseSwitchClick.Add(kbButtons[counter][6]);
                                        }
                                    }
                                }
                                
                            }
                            else if (counter >= 33 && counter <= 57)
                            {
                                bool parsedValue;
                                switch (counter)
                                {
                                    case 33:
                                        if (bool.TryParse(line, out parsedValue)) ActivateUpper = parsedValue;
                                        break;
                                    case 34:
                                        if (bool.TryParse(line, out parsedValue)) ActivateMiddle = parsedValue;
                                        break;
                                    case 35:
                                        if (bool.TryParse(line, out parsedValue)) ActivateLower = parsedValue;
                                        break;
                                    case 36:
                                        if (bool.TryParse(line, out parsedValue)) showLabelsHover = parsedValue;
                                        break;
                                    case 37:
                                        if (bool.TryParse(line, out parsedValue)) showLabelsLC = parsedValue;
                                        break;
                                    case 38:
                                        if (bool.TryParse(line, out parsedValue)) showLabelsRC = parsedValue;
                                        break;
                                    case 39:
                                        musicMode = false;
                                        break;
                                    case 40:

                                        switch (line)
                                        {

                                            case "Xbox":
                                                xboxMode = true;
                                                switchMode = false;
                                                keyboardMode = false;
                                                newMode = "Xbox";
                                                break;
                                            case "Switch":
                                                switchMode = true;
                                                xboxMode = false;
                                                keyboardMode = false;
                                                newMode = "Switch";
                                                break;
                                            case "Keyboard":
                                                switchMode = false;
                                                xboxMode = false;
                                                keyboardMode = true;
                                                newMode = "Keyboard";
                                                break;
                                        }

                                        break;
                                    case 41:
                                        break;
                                    case 43:
                                        if (bool.TryParse(line, out parsedValue)) diagUpRightHover = parsedValue;
                                        break;
                                    case 44:
                                        if (bool.TryParse(line, out parsedValue)) diagUpRightLC = parsedValue;
                                        break;
                                    case 45:
                                        if (bool.TryParse(line, out parsedValue)) diagUpRightRC = parsedValue;
                                        break;
                                    case 46:
                                        if (bool.TryParse(line, out parsedValue)) diagDownRightHover = parsedValue;
                                        break;
                                    case 47:
                                        if (bool.TryParse(line, out parsedValue)) diagDownRightLC = parsedValue;
                                        break;
                                    case 48:
                                        if (bool.TryParse(line, out parsedValue)) diagDownRightRC = parsedValue;
                                        break;
                                    case 49:
                                        if (bool.TryParse(line, out parsedValue)) diagUpLeftHover = parsedValue;
                                        break;
                                    case 50:
                                        if (bool.TryParse(line, out parsedValue)) diagUpLeftLC = parsedValue;
                                        break;
                                    case 51:
                                        if (bool.TryParse(line, out parsedValue)) diagUpLeftRC = parsedValue;
                                        break;
                                    case 52:
                                        if (bool.TryParse(line, out parsedValue)) diagDownLeftHover = parsedValue;
                                        break;
                                    case 53:
                                        if (bool.TryParse(line, out parsedValue)) diagDownLeftLC = parsedValue;
                                        break;
                                    case 54:
                                        if (bool.TryParse(line, out parsedValue)) diagDownLeftRC = parsedValue;
                                        break;
                                    case 55:
                                        if (bool.TryParse(line, out parsedValue)) clickFocus = parsedValue;
                                        break;
                                    case 57:
                                        if (line.Contains("1"))
                                        {
                                            FPS = true;

                                        }


                                        if (line.Contains("2"))
                                        {
                                            combineControllers = true;
                                        }


                                        if (line.Contains("3"))
                                        {
                                            disableClicks = true;
                                        }


                                        if (line.Contains("4"))
                                        {
                                            AltRTC = true;
                                        }


                                        if (line.Contains("5"))
                                        {

                                            MarioKart = true;
                                        }

                                        if (line.Contains("6"))
                                        {
                                            Voice = true;
                                        }

                                        if (line.Contains("7"))
                                        {

                                            FocusMouse = true;
                                        }

                                        if (line.Contains("8")) { MouseLock = true; }

                                        if (line.Contains("9"))
                                        {
                                            if (xboxMode)
                                            {

                                                SimGamePad.resetFlag();

                                                btnStartScreen.Enabled = false;
                                                btnSwitchWakeup.Enabled = true;
                                                psLabels = true;

                                                SimGamePad.Instance.Initialize(true, "ds4");

                                                SimGamePad.Instance.PlugIn();
                                            }
                                        }
                                        else
                                        {
                                            if (xboxMode)
                                            {

                                                SimGamePad.resetFlag();

                                                btnSwitchWakeup.Enabled = false;
                                                btnStartScreen.Enabled = true;
                                                psLabels = false;

                                                SimGamePad.Instance.Initialize(true, "xbox");
                                                Debug.WriteLine("Initialized Xbox Mode");
                                                SimGamePad.Instance.PlugIn();
                                            }
                                        }
                                        if (line.Contains("A"))
                                        {
                                            SafeZone = true;
                                        }
                                        break;
                                }
                            }

                            counter++;
                        }
                    }

                    // FPS Turn Right
                    kbButtons[33] = new List<string> { "Right", "False", "False", "RS Right", "False", "True", "RS Right" };
                    xbuttons[33] = kbButtons[33][3];
                    switchbuttons[33] = kbButtons[33][6];
                    buttonConfig[33] = new List<string> { "right", "false", "false", "rs right", "false", "true", "rs right" };

                    // FPS Turn Left
                    kbButtons[34] = new List<string> { "Left", "False", "False", "RS Left", "False", "True", "RS Left" };
                    xbuttons[34] = kbButtons[34][3];
                    switchbuttons[34] = kbButtons[34][6];
                    buttonConfig[34] = new List<string> { "left", "false", "false", "rs left", "false", "true", "rs left" };

                    // FPS Look Up digital
                    kbButtons[35] = new List<string> { "Up", "False", "False", "RS Up", "False", "False", "RS Up" };
                    xbuttons[35] = kbButtons[35][3];
                    switchbuttons[35] = kbButtons[35][6];
                    buttonConfig[35] = new List<string> { "up", "false", "false", "rs up", "false", "false", "rs up" };

                    // FPS Look Down digital
                    kbButtons[36] = new List<string> { "Down", "False", "False", "RS Down", "False", "False", "RS Down" };
                    xbuttons[36] = kbButtons[36][3];
                    switchbuttons[36] = kbButtons[36][6];
                    buttonConfig[36] = new List<string> { "down", "false", "false", "rs down", "false", "false", "rs down" };

                    // FPS Look Up analog
                    kbButtons[37] = new List<string> { "Up", "False", "False", "RS Up", "False", "True", "RS Up" };
                    xbuttons[37] = kbButtons[37][3];
                    switchbuttons[37] = kbButtons[37][6];
                    buttonConfig[37] = new List<string> { "up", "false", "false", "rs up", "false", "true", "rs up" };

                    // FPS Look Down analog
                    kbButtons[38] = new List<string> { "Down", "False", "False", "RS Down", "False", "True", "RS Down" };
                    xbuttons[38] = kbButtons[38][3];
                    switchbuttons[38] = kbButtons[38][6];
                    buttonConfig[38] = new List<string> { "down", "false", "false", "rs down", "false", "true", "rs down" };
                }

            }
            catch (IOException ex)
            {
                Console.WriteLine($"File I/O Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            if (ActivateUpper) { ActivateMiddle = false; ActivateLower = false; }
            else if (ActivateMiddle) { ActivateUpper = false; ActivateLower = false; }
            else if (ActivateLower) { ActivateUpper = false; ActivateMiddle = false; }
            else { ActivateUpper = true; ActivateMiddle = false; ActivateLower = false; }

            Debug.WriteLine(ActivateUpper.ToString() + ActivateMiddle.ToString() + ActivateLower.ToString());



            if (combineControllers && (scanning || scanned))
            {
                SetLblActiveInactiveUi("Gamepad detection started. Press A button on your controller.");
            }
            else
            {
                SetLblActiveInactiveUi("Overjoyed is inactive. Hold Click Here to activate!");
            }



            //if (previousMode == "Xbox" && modeCounter > 0 && previousMode != newMode)
            //{
            //    SimGamePad.Instance.Unplug();
            //    SimGamePad.Instance.ShutDown();
            //}

            if (keyboardMode)
            {
                scanning = false;
                scanned = false;
                lblCombine.Visible = false;
            }

            //modeCounter++;

            ApplyActivationAvailabilityGate();
            pnlExtraButtons.Visible = true;
            //if (chkCombineControllers.Checked)
            //{

            //    lblUseOverjoyed.Visible = true;
            //    chkCombineLS.Visible = true;
            //    chkCombineRS.Visible = true;
            //    chkCombineLT.Visible = true;
            //    chkCombineRT.Visible = true;
            //    chkCombineLS.Checked = true;
            //    chkCombineRS.Checked = true;
            //}
            //    chkCombineLT.Checked = true;
            //    chkCombineRT.Checked = true;
            btnSwitchWakeup.Visible = false;
            btnSwitchWakeup.Visible = false;
            btnSwitchHome.Visible = true;
            btnSwitchHome.Visible = true;
            btnSwitchSelect.Visible = true;
            btnSwitchSelect.Visible = true;
            btnSwitchStart.Visible = true;
            btnSwitchStart.Visible = true;
            btnAccept.Visible = true;
            btnAccept.Visible = true;
            btnBack.Visible = true;
            btnBack.Visible = true;
            btnRight.Visible = true;
            btnRight.Visible = true;
            btnLeft.Visible = true;
            btnLeft.Visible = true;
            btnUp.Visible = true;
            btnUp.Visible = true;
            btnDown.Visible = true;
            btnDown.Visible = true;
            btnStartScreen.Visible = false; btnStartScreen.Visible = false;

            if (xboxMode)
            {
                btnSwitchHome.Text = "Xbox Button";
                btnSwitchSelect.Text = "Select Button";
                btnSwitchStart.Text = "Start Button";
                btnAccept.Text = "A";
                btnBack.Text = "B";
                btnStartScreen.Visible = true; btnStartScreen.Visible = true;
                btnStartScreen.Text = "PS Controls";
                btnSwitchWakeup.Visible = true;
                btnSwitchWakeup.Visible = true;
                btnSwitchWakeup.Text = "Xbox Controls";





            }
            else if (keyboardMode)
            {
                btnSwitchHome.Text = "Alt + Tab";
                btnSwitchSelect.Text = "Space Key";
                btnSwitchStart.Text = "Windows Key";
                btnAccept.Text = "✓";
                btnBack.Text = "X";


                btnSwitchWakeup.Visible = false;
                btnSwitchWakeup.Visible = false;
                btnStartScreen.Visible = false;
                btnStartScreen.Visible = false;


            }

            else if (switchMode)
            {
                btnSwitchHome.Text = "Home Button";
                btnSwitchSelect.Text = "Select Button";
                btnSwitchStart.Text = "Start Button";
                btnAccept.Text = "A";
                btnBack.Text = "B";
                btnStartScreen.Text = "L + R";
                btnSwitchWakeup.Text = "Turn On Switch";
                btnStartScreen.Visible = true; btnStartScreen.Visible = true;
                btnSwitchWakeup.Visible = true;
                btnSwitchWakeup.Visible = true;



            }

            Debug.WriteLine(string.Join(",", toggleXbox.ToArray()));

            drawPrompts();

            if (switchMode)
            {
                if (chkMarioKart.Parent != null)
                    chkMarioKart.Parent.Controls.Remove(chkMarioKart);
                chkMarioKart.Visible = true;
                chkMarioKart.Dock = DockStyle.Top;
                chkMarioKart.Text = "Mario Kart Mode"; // Set the text for the checkbox
                advancedPanel.Controls.Remove(chkMarioKart);
                advancedPanel.Controls.Add(chkMarioKart);

            }
            else
            {
                advancedPanel.Controls.Remove(chkMarioKart);
                chkMarioKart.Checked = false;
                chkMarioKart.Visible = false;
            }

            if (!showLabelsHover && !showLabelsLC && !showLabelsRC) { pnlPrompts.Visible = false; }


        }

        private void mouseUp(string mouseButton)
        {
            SetPointerGlow(mouseButton, false);

            if (mouseButton == "right")
            {
                rightLookModeLatched = false;
            }

            if (positionCheck)
            {
                xEnd = (int)remoteX;
                yEnd = (int)remoteY;
            }
            else
            {
                xEnd = (int)(Cursor.Position.X / zoomFactor);
                yEnd = (int)(Cursor.Position.Y / zoomFactor);
            }

            if (positionCheck && TryHandleRemoteMouseButtonAction(mouseButton, false))
            {
                return;
            }

            float _magnitudeX = Math.Abs(xStart - xEnd);
            float _magnitudeY = Math.Abs(yStart - yEnd);
            double distance = Math.Sqrt(Math.Pow(_magnitudeX, 2) + Math.Pow(_magnitudeY, 2));

            if ((distance > deadZone) && isActive)
            {
                float _angle = (float)Math.Atan2((yEnd - yStart), (xEnd - xStart));

                _angle = (float)(180 / Math.PI) * _angle;
                _angle = SnapQuadrantBoundaryAngleToCardinal(_angle);

                if (mouseButton == "left")
                {
                    Eval(@"const event = new MouseEvent('pointerup', {
                        clientX: " + xEnd + @",
                        clientY: " + yEnd + @",
                        button: 0,
                        buttons: 0
                    });
                    document.querySelector('div[name=""pnlBaseOverlay""]').dispatchEvent(event);
                    ");

                    if (_angle >= -112.5 && _angle < -67.5)
                    {
                        c1 = 8;
                    }
                    else if (_angle >= -67.5 && _angle < -22.5)
                    {
                        c1 = 9;
                    }
                    else if (_angle >= -22.5 && _angle < 22.5)
                    {
                        c1 = 10;
                    }
                    else if (_angle >= 22.5 && _angle < 67.5)
                    {
                        c1 = 11;
                    }
                    else if (_angle >= 67.5 && _angle < 112.5)
                    {
                        c1 = 12;
                    }
                    else if (_angle >= 112.5 && _angle < 157.5)
                    {
                        c1 = 13;
                    }
                    else if (_angle >= 157.5 || _angle < -157.5)
                    {
                        c1 = 14;
                    }
                    else if (_angle >= -157.5 && _angle < -112.5)
                    {
                        c1 = 15;
                    }
                }
                else
                {
                    Eval(@"const event = new MouseEvent('pointerup', {
                        clientX: " + xEnd + @",
                        clientY: " + yEnd + @",
                        button: 2,
                        buttons: 0
                    });
                    document.querySelector('div[name=""pnlBaseOverlay""]').dispatchEvent(event);
                    ");

                    if (_angle >= -112.5 && _angle < -67.5)
                    {
                        c1 = 16;
                    }
                    else if (_angle >= -67.5 && _angle < -22.5)
                    {
                        c1 = 17;
                    }
                    else if (_angle >= -22.5 && _angle < 22.5)
                    {
                        c1 = 18;
                    }
                    else if (_angle >= 22.5 && _angle < 67.5)
                    {
                        c1 = 19;
                    }
                    else if (_angle >= 67.5 && _angle < 112.5)
                    {
                        c1 = 20;
                    }
                    else if (_angle >= 112.5 && _angle < 157.5)
                    {
                        c1 = 21;
                    }
                    else if (_angle >= 157.5 || _angle < -157.5)
                    {
                        c1 = 22;
                    }
                    else if (_angle >= -157.5 && _angle < -112.5)
                    {
                        c1 = 23;
                    }
                }

                if (buttonConfig[c1][4] == "true")
                {
                    ReturnToCenter();
                }
            }
        }

        private void pnlBaseOverlay_MouseUp(object sender, MouseEventArgs e)
        {
            if (positionCheck)
            {
                return;
            }  

            if (IsHoldFriendlyMouseButton(e.Button) && IsLblActiveInactiveState())
            {
                mouseUp(e.Button == MouseButtons.Right ? "right" : "left");
                CancelLblActiveHoldAnimation();
                return;
            }

            if (IsHoldFriendlyMouseButton(e.Button)
                && isActive
                && chkDisableClicks.Checked
                && IsMoveMouseUsingLeftClicksEnabled())
            {
                CancelAltMouseDeactivateHold();
            }
            
             if (chkDisableClicks.Checked)
                {
                    if (e.Button == MouseButtons.Left && IsAltLeftClickEnabled() && !IsMoveMouseUsingLeftClicksEnabled())
                    {
                        localLeftPointerDown = false;
                        mouseUp("left");
                    }
                    else if (e.Button == MouseButtons.Right && IsAltRightClickEnabled() && !IsMoveMouseUsingLeftClicksEnabled())
                    {
                        localRightPointerDown = false;
                        mouseUp("right");
                    }

                    return;
                }

             



                if (isActive)
                {

                    if (e.Button == MouseButtons.Left) //Btn A Up 
                    {
                        localLeftPointerDown = false;
                        mouseUp("left");
                    }
                    else if (e.Button == MouseButtons.Right) //Btn B Up
                    {
                        localRightPointerDown = false;
                        mouseUp("right");
                    }
                }
            
        }

        private void CheckHash()
        { 
            timer.Tick += new EventHandler(TimerTick);
            //timer.Tick += new EventHandler(TimerTick2);

            pnlPrompts.BringToFront();
            pnlBaseOverlay.BringToFront();

            appFolderPath = Path.GetDirectoryName(AppContext.BaseDirectory);
            timer.Interval = 16; //Counts by ms
            timer.Enabled = true; //Ensure that the timer is enabled#
            timer.Start(); //Start the timer
            AssignKeyCodes();
            UpdateNintendoStreamerPlayerDropdown();
            UpdateAlternativeMouseControlsOptionsVisibility();
           
            resetCursor();


            string alphaFilePath = Path.Combine(configPath, "Transparency.txt");

            string alphaValue = System.IO.File.ReadLines(alphaFilePath).ElementAtOrDefault(0);

            try
            {
                // Open the file for writing with shared access
                using (FileStream fs = new FileStream(alphaFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                using (StreamWriter file2 = new StreamWriter(fs))
                {
                    file2.WriteLine(alphaValue);                  // Write the alpha value
                    file2.WriteLine(switchMode); // Write the Nintendo Switch checkbox state
                    file2.WriteLine("true");                // Write "true"
                }

                Console.WriteLine("File written successfully with simultaneous read/write access.");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"An error occurred while writing to the file: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access denied: {ex.Message}");
            }


            for (int i = 0; i < activateToggle.Count; i++) { activateToggle[i] = true; }


        }

        private async void Form1_Shown()
        {
            await MinimizeMessenger.SendMessageAsync("active|" + playerNum);

            if (Screen.Bounds.Height >= 1100 && Screen.Bounds.Width > 1920)
            {
                await MinimizeMessenger.SendMessageAsync("max|" + playerNum);
            }

      

            //await Task.Delay(1000);
            //await MainThread.Invoke
        }

        private static void ToggleWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            toggleRemote = true;
            Debug.WriteLine("ToggleRemote.txt changed, setting toggleRemote to true.");
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            
            try
            {
                var streamerModePath = Path.Combine(configPath, "StreamerMode.txt");
                string? firstLine = null;

                if (System.IO.File.Exists(streamerModePath))
                {
                    using var fs = new FileStream(streamerModePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using var reader = new StreamReader(fs);
                    firstLine = reader.ReadLine();
                }

                if (!string.IsNullOrWhiteSpace(firstLine))
                {
                    specialMode = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading StreamerMode.txt: {ex.Message}");
            }

            if (specialMode)
            {
                toggleXboxFPS.Add("RS Left");
                toggleXboxFPS.Add("RS Right");
                toggleXboxFPS.Add("RS Up");
                toggleXboxFPS.Add("RS Down");
            }

            try
            {
                var content = System.IO.File.ReadAllText(Path.Combine(configPath, "RemoteActive.txt"));
                string? firstLine = content?.Split(new[] { Environment.NewLine }, StringSplitOptions.None).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(firstLine))
                {
                    remoteActive = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading RemoteActive.txt: {ex.Message}");
            }

            _toggleWatcher = new FileSystemWatcher
            {
                Path = configPath,              // directory to watch
                Filter = "ToggleRemote.txt",    // file to watch
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
            };

            _toggleWatcher.Changed += ToggleWatcher_Changed;
            _toggleWatcher.EnableRaisingEvents = true;
            _toggleWatcher.Changed += ToggleWatcher_Changed;
            _toggleWatcher.EnableRaisingEvents = true;

            playerNum = GetAssignedPlayerNum();

            UpdateNintendoStreamerPlayerDropdown();


           

            //MouseMuxClient.Initialize(this);
            //_ = MouseMuxClient.StartAsync(); // ?? fire-and-forget

            playerNum = GetAssignedPlayerNum();

            BasicRemoteClient.AttachForm(this);

            if (Screen.Bounds.Height >= 1100 && Screen.Bounds.Width > 1920)
            {
                truncateSize = 16;
            }
            btnSettings.Text = "<p style='line-height:1.2;padding:0;margin:0;'><span style='-webkit-line-clamp:3;font-size:"+(truncateSize*zoomFactor).ToString(System.Globalization.CultureInfo.InvariantCulture)+"pt;' class='truncate' title='Change Overjoyed Settings'>Change Overjoyed Settings</span></p>";
            Form1_Shown();
//            else
//            {
//                quadrantsSVG.Eval(@"
//    (function () {

//        if (!window.realeyeLoaded) {

//            var script = document.createElement('script');
//            script.src = 'http://localhost:" + realEyePort + @"/configs/realeye-sdk-mar27c.min.js';

//            script.onload = function () {
//                window.realeyeLoaded = true;
//                console.log('RealEye SDK loaded');
//            };

//            script.onerror = function (e) {
//                console.error('Failed to load SDK', e);
//            };

//            document.head.appendChild(script);
//        }

//    })();
//");
//            }




            CheckHash();



            CreateMenuBar();

            if (showLabelsHover && !chkFPS.Checked) { showMouseMoveLabels.Checked = true; }
            else if (showLabelsLC || chkFPS.Checked) { showLeftClickLabels.Checked = true; }
            else if (showLabelsRC && !chkFPS.Checked) { showRightClickLabels.Checked = true; }

            StartListening();


            pnlExtraButtons.Enabled = true;
            pnlExtraButtons.Visible = true;
         

            btnSwitchWakeup.Visible = false;
            btnSwitchWakeup.Visible = false;
            btnSwitchHome.Visible = true;
            btnSwitchHome.Visible = true;
            btnSwitchSelect.Visible = true;
            btnSwitchSelect.Visible = true;
            btnSwitchStart.Visible = true;
            btnSwitchStart.Visible = true;
            btnAccept.Visible = true;
            btnAccept.Visible = true;
            btnBack.Visible = true;
            btnBack.Visible = true;
            btnRight.Visible = true;
            btnRight.Visible = true;
            btnLeft.Visible = true;
            btnLeft.Visible = true;
            btnUp.Visible = true;
            btnUp.Visible = true;
            btnDown.Visible = true;
            btnDown.Visible = true;
            btnStartScreen.Visible = false; btnStartScreen.Visible = false;
      

            if (xboxMode)
            {
                btnSwitchHome.Text = "Xbox Button";
                if(psLabels)
                {
                    btnSwitchHome.Text = "PS Button";
                }
                btnSwitchSelect.Text = "Select Button";
                btnSwitchStart.Text = "Start Button";

                btnAccept.Text = "A";
                btnBack.Text = "B";
                if(psLabels)
                {
                    btnAccept.Text = "X";
                    btnBack.Text = "O";
                }
                btnStartScreen.Visible = true; btnStartScreen.Visible = true;
                btnStartScreen.Text = "PS Controls";
                btnSwitchWakeup.Visible = true;
                btnSwitchWakeup.Visible = true;
                btnSwitchWakeup.Text = "Xbox Controls";
                cmbArrowsKeyboard.Visible = false;
                cmbArrowsKeyboard.SelectedIndex = 0;
                cmbArrowsGamepad.Visible = true;
                InitializeArrowsGamepadOptions();
                btnZL.Visible = true;
                btnZR.Visible = true;
                btnZL.Text = "LB";
                btnZR.Text = "RB";
                if(psLabels)
                {
                    btnZL.Text = "L1";
                    btnZR.Text = "R1";
                }



            }
            else if (keyboardMode)
            {
                btnSwitchHome.Text = "Alt + Tab";
                btnSwitchSelect.Text = "Space Key";
                btnSwitchStart.Text = "Windows Key";
                btnAccept.Text = "✓";
                btnBack.Text = "X";


                btnSwitchWakeup.Visible = false;
                btnSwitchWakeup.Visible = false;
                btnStartScreen.Visible = false;
                btnStartScreen.Visible = false;

                cmbArrowsKeyboard.Visible = true;
                cmbArrowsKeyboard.SelectedIndex = 0;
                cmbArrowsGamepad.Visible = false;
                InitializeArrowsGamepadOptions();
                btnZL.Visible = false;
                btnZR.Visible = false;
            }

            else if (switchMode)
            {
                btnSwitchHome.Text = "Home Button";
                btnSwitchSelect.Text = "Select Button";
                btnSwitchStart.Text = "Start Button";
                btnAccept.Text = "A";
                btnBack.Text = "B";
                btnStartScreen.Text = "L + R";
                btnSwitchWakeup.Text = "Turn On Switch";
                btnStartScreen.Visible = true; btnStartScreen.Visible = true;
                btnSwitchWakeup.Visible = true;
                btnSwitchWakeup.Visible = true;

                cmbArrowsKeyboard.Visible = false;
                cmbArrowsKeyboard.SelectedIndex = 0;
                cmbArrowsGamepad.Visible = true;
                InitializeArrowsGamepadOptions();
                btnZL.Visible = true;
                btnZR.Visible = true;

            }


            //            Eval(@"let lastX = window.screenX;
            //let lastY = window.screenY;
            //let isMoving = false;
            //let stopTimeout;
            //let mouseOutside = false;

            //// Set the movement threshold (e.g., 100px)
            //const movementThreshold = 100;

            //function checkWindowMove() {
            //    // Check if the mouse is outside the window
            //    if (mouseOutside) {
            //        // Calculate the distance moved from last known position
            //        const deltaX = Math.abs(window.screenX - lastX);
            //        const deltaY = Math.abs(window.screenY - lastY);

            //        // Only trigger if the movement exceeds the threshold
            //        if (deltaX >= movementThreshold || deltaY >= movementThreshold) {
            //            if (!isMoving) {
            //                isMoving = true;
            //                console.log('?? Window drag started (mouse outside website)!');
            //                document.getElementById('container').style.display = 'none'; // Hide element when dragging starts
            //            }

            //            lastX = window.screenX;
            //            lastY = window.screenY;

            //            // Reset stop detection
            //            clearTimeout(stopTimeout);
            //            stopTimeout = setTimeout(() => {
            //                isMoving = false;
            //                console.log('?? Window drag stopped (mouse outside website)!');
            //            }, 200); // Stops after 200ms of no movement
            //        }
            //    }
            //}

            //setInterval(checkWindowMove, 100); // Check every 100ms

            //// Event listener for when the mouse leaves the website
            //document.addEventListener('mouseout', function (event) {
            //    if (event.clientY < 0) { // When mouse leaves the top of the window
            //        mouseOutside = true;
            //        console.log('?? Mouse left the website, start tracking window move.');
            //    }
            //});

            //// Event listener for when the mouse returns to the website
            //document.addEventListener('mouseover', function () {
            //    mouseOutside = false;
            //    console.log('?? Mouse back on the website, stop tracking window move.');
            //                document.getElementById('container').style.display = 'block'; // Show element when dragging stops

            //});



            //");

            InitialSize();


            Eval(@"function applyMarquee(el) {
  if ((el.dataset.marqueeApplied === 'true' && el.querySelector('.marquee-outer')) || el.querySelector('.truncate')){ return;}
  let text = el.textContent.trim();
if (!text || text.length === 1) return;

el.style.whiteSpace = 'nowrap'; // already in CSS

  let cs = getComputedStyle(el);
  let wasCentered = cs.textAlign === 'center';

  el.textContent = '';
  let outer = document.createElement('span');
  outer.className = 'marquee-outer';
  outer.style.display = 'block';
  outer.style.width = '100%';
if (wasCentered) {
        outer.style.textAlign = 'center';
      }
else {
        outer.style.textAlign = 'left';
      }
  let inner = document.createElement('span');
  inner.className = 'marquee-inner';

  let s1 = document.createElement('span');
  s1.textContent = text;
  let s2 = document.createElement('span');
  s2.textContent = text;

  inner.appendChild(s1);
  inner.appendChild(s2);
  outer.appendChild(inner);
  el.appendChild(outer);

  s1.style.display = 'inline-flex'; // already in CSS
if (wasCentered) {
    let space = el.clientWidth - s1.offsetWidth;
    s1.style.paddingLeft = (space/2) + 'px';
}

  

  el.dataset.marqueeApplied = 'true';
}

setTimeout(() => {
  console.log('After 2 seconds');
document.querySelectorAll('[name^=""lbl""]').forEach(el => {
    applyMarquee(el);
});

document.querySelectorAll('[name^=""label""]').forEach(el => {
    applyMarquee(el);
});

document.querySelectorAll('[name^=""TitleLabel""]').forEach(el => {
    applyMarquee(el);
});

document.querySelectorAll('[class*=""qx-combobox-textfield-inner""]').forEach(el => {
    applyMarquee(el);
});

document.querySelectorAll('[class*=""qx-textlabel-borderNone""]').forEach(el => {
    applyMarquee(el);
});
}, 2000); // 2000 milliseconds = 2 seconds
");
            Eval(@"
               function isShowingOverflowEllipsis(el) {
  return el.scrollWidth > el.clientWidth || el.scrollHeight > el.clientHeight;
}

function setupDynamicEllipsisTooltip(selector) {
  document.querySelectorAll(selector).forEach(el => {
  // Apply min-height based on line-height and minLines
    const cs = getComputedStyle(el);
    const minLines = parseInt(cs.getPropertyValue('-webkit-line-clamp')) || 1;
    let lineHeight = 1.2;
    const fontSize = parseFloat(cs.fontSize);

    const minHeightPx = lineHeight * fontSize * minLines;
    el.parentElement.style.minHeight = minHeightPx + 'px';

    const updateTooltip = () => {
      if (isShowingOverflowEllipsis(el)) {
        el.title = el.textContent.trim();
      } else {
        el.removeAttribute('title');
      }
    };

    // Initial check
    updateTooltip();

    // Watch for text or style changes
    const observer = new MutationObserver(updateTooltip);
    observer.observe(el, {
      childList: true,      // watch for added/removed child nodes
      characterData: true,  // watch for text content changes
      subtree: true         // watch inside child elements too
    });

    // Optionally handle resizes (affects overflow)
    window.addEventListener('resize', updateTooltip);
  });
}

setTimeout(() => {
  console.log('After 2 seconds');
  setupDynamicEllipsisTooltip('.truncate');
}, 2000); // 2000 milliseconds = 2 seconds
                ");

            if (switchMode)
            {
                switchGamepad.Form1_Load();
            }
        }


        private bool CheckKey(GregsStack.InputSimulatorStandard.Native.VirtualKeyCode key)
        {
            return inputSimulator.InputDeviceState.IsHardwareKeyDown(key);
        }

        private async void ReturnToCenter()
        {
            if (chkAltRTC.Checked)
            {
                await MinimizeMessenger.SendMessageAsync("rtc-window|" + playerNum);
            }
            else
            {
                await MinimizeMessenger.SendMessageAsync("rtc-cursor|" + playerNum);
            }

            rtcDraw = true;
            //pnlBaseOverlay.Invalidate();
            //pnlDraw.Invalidate();
        }

        private void pnlBaseOverlay_Appear(object sender, EventArgs e)
        {
      
            if (MarioKart) { chkMarioKart.Checked = true; }
            if (FPS) { chkFPS.Checked = true; }
            if (disableClicks) { chkDisableClicks.Checked = true; }
            if (AltRTC) { chkAltRTC.Checked = true; }
            if (Voice) { chkVoice.Checked = true; }
            if (FocusMouse) { chkKeepMouseInside.Checked = false; }
            if (MouseLock) { chkMouseLock.Checked = false; }
            if (combineControllers) { chkCombineControllers.Checked = true; }
            if (SafeZone) { chkSafeZone.Checked = true; }
            Debug.WriteLine("QuadrantsSVG_Appear triggered. Current settings - MarioKart: " + MarioKart + ", FPS: " + FPS + ", disableClicks: " + disableClicks + ", AltRTC: " + AltRTC + ", Voice: " + Voice + ", FocusMouse: " + FocusMouse + ", MouseLock: " + MouseLock + ", combineControllers: " + combineControllers + ", SafeZone: " + SafeZone);

            // get text from configpath/port.txt
            int realEyePort = 0;
            try
            {
                // Same logic as in Startup.CreateConfigApp
                string appDataPath = FileSystem.Current.AppDataDirectory;
                string configPath = Path.Combine(appDataPath, "configs");
                string portFilePath = Path.Combine(configPath, "port.txt");

                if (System.IO.File.Exists(portFilePath))
                {
                    var text = System.IO.File.ReadAllText(portFilePath).Trim();
                    if (!int.TryParse(text, out realEyePort))
                    {
                        Debug.WriteLine($"Invalid port in port.txt: '{text}'");
                        realEyePort = 0;
                    }
                }
                else
                {
                    Debug.WriteLine("port.txt not found in configs.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to read port.txt: {ex.Message}");
            }

            var promptStateJson = JsonSerializer.Serialize(new
            {
                showLabelsHover,
                showLabelsLC,
                showLabelsRC,
                keyboardMode,
                xboxMode,
                switchMode,
                psLabels,
                chkFPS = chkFPS.Checked,
                isRemoteCheckboxChecked = positionCheck,
                activeHoverQuadrant = currentQuadrant2,
                positionCheck,
                remoteX,
                remoteY,
                buttonConfig,
                diagUpRightHover,
                diagUpLeftHover,
                diagDownRightHover,
                diagDownLeftHover,
                diagUpRightLC,
                diagUpLeftLC,
                diagDownRightLC,
                diagDownLeftLC,
                diagUpRightRC,
                diagUpLeftRC,
                diagDownRightRC,
                diagDownLeftRC
            });

            InitializePromptOverlay(realEyePort, promptStateJson);

      
            if (remoteActive)
            {
                chkRemote.Checked = true;
                drawPrompts();
                RefreshOverlayPointerState();
            }



            OBSWebSocket.Initialize(this);
            UpdateNintendoStreamerPlayerDropdown();
            _ = OBSWebSocket.StartAsync(); // ?? fire-and-forget

        }

        private void pnlEverything_Appear(object sender, EventArgs e)
        {
            pnlBaseOverlay.Enabled = true;
            pnlExtraButtons.Enabled = true;

            resizer();
        }
      
    }

}