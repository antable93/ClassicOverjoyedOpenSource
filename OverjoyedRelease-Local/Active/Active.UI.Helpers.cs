using GregsStack.InputSimulatorStandard.Native;
using Microsoft.Maui.ApplicationModel;
using SharedVars;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Threading.Tasks;
using Wisej.Core;
using Wisej.Web;

namespace OverjoyedReleaseLocal
{
    public partial class Active : Page
    {
        private bool IsControllerDetectionGateActive()
        {
            return chkCombineControllers.Checked && !keyboardCombine && !scanned;
        }

        private void ApplyActivationAvailabilityGate()
        {
            bool gateActive = IsControllerDetectionGateActive();

            // Keep the container enabled so Settings remains clickable while other controls are gated.
            pnlExtraButtons.Enabled = true;

            foreach (Control control in pnlExtraButtons.Controls)
            {
                if (!ReferenceEquals(control, btnSettings))
                {
                    control.Enabled = !gateActive;
                }
            }

            btnSettings.Enabled = true;
        }

        private async void activeFalse()
        {
            SetMenuBarVisibleForActiveState(false);
            SetResizeButtonsVisibleForActiveState(false);
            await MinimizeMessenger.SendMessageAsync("active-false|" + playerNum);

        }

        private void DeactivateFromExitHold(bool releaseFocusMouse = false)
        {
            Reset();

            if (chkCombineControllers.Checked)
            {
                SetLblActiveVisibleUi(true);
            }

            ApplyActivationAvailabilityGate();
            pnlExtraButtons.Visible = true;
            if (chkCombineControllers.Checked && !keyboardCombine)
            {
                lblUseOverjoyed.Visible = true;
                chkCombineLS.Visible = true;
                chkCombineRS.Visible = true;
                chkCombineLT.Visible = true;
                chkCombineRT.Visible = true;
            }

            if(chkCombineControllers.Checked)
            {
               chkSteam.Visible = true;
            }

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
            btnStartScreen.Visible = false;
            btnStartScreen.Visible = false;

            if (xboxMode)
            {
                btnSwitchHome.Text = "Xbox Button";
                if (psLabels)
                {
                    btnSwitchHome.Text = "PS Button";
                }

                btnSwitchSelect.Text = "Select Button";
                btnSwitchStart.Text = "Start Button";

                btnAccept.Text = "A";
                btnBack.Text = "B";
                if (psLabels)
                {
                    btnAccept.Text = "X";
                    btnBack.Text = "O";
                }

                btnStartScreen.Visible = true;
                btnStartScreen.Visible = true;
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
                if (psLabels)
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
                btnStartScreen.Visible = true;
                btnStartScreen.Visible = true;
                btnSwitchWakeup.Visible = true;
                btnSwitchWakeup.Visible = true;

                cmbArrowsKeyboard.Visible = false;
                cmbArrowsKeyboard.SelectedIndex = 0;
                cmbArrowsGamepad.Visible = true;
                InitializeArrowsGamepadOptions();
                btnZL.Visible = true;
                btnZR.Visible = true;
            }

            isActive = false;

            hoverButtons.Visible = false;
            clickButtons.Visible = false;
            SetLblActiveInactiveUi("Overjoyed is inactive. Hold Click Here to activate!");
            activeFalse();
            lblLegend.BackColor = System.Drawing.Color.White;
            lblLegend.ForeColor = System.Drawing.Color.Black;

            if (releaseFocusMouse && chkKeepMouseInside.Checked)
            {
                UnclipCursor();
            }
        }

        [WebMethod]
        public void ExitHoldDeactivate()
        {
            if (!isActive || positionCheck || chkRemote.Checked)
            {
                return;
            }

            DeactivateFromExitHold();
        }

        [WebMethod]
        public void TriggerEscapeHatchDeactivate()
        {
            if (!isActive || positionCheck || chkRemote.Checked)
            {
                return;
            }

            DeactivateFromExitHold(true);
        }

        [WebMethod]
        public void TriggerEscapeHatchDoubleClickDeactivate()
        {
            if (!isActive || positionCheck || chkRemote.Checked)
            {
                return;
            }

            suppressBaseOverlayDoubleClickUntil = DateTime.UtcNow.AddMilliseconds(DoubleClickThreshold + 100);
            DeactivateFromExitHold(true);
        }

        [WebMethod]
        public void CompleteLblActiveHoldActivate()
        {
            if (positionCheck || chkRemote.Checked || !IsLblActiveInactiveState())
            {
                return;
            }

            ActivateFromDeadZone();
        }

        [WebMethod]
        public void ToggleFpsDeadZoneLookModeFromPrompt()
        {
            if (!isActive || !chkFPS.Checked || !showLabelsLC)
            {
                return;
            }

            suppressBaseOverlayDoubleClickUntil = DateTime.UtcNow.AddMilliseconds(DoubleClickThreshold + 100);
            fpsDeadZoneLookModeLatched = !fpsDeadZoneLookModeLatched;
            fpsDeadZoneCenterEntryCount = fpsDeadZoneLookModeLatched ? 1 : 0;
            drawPrompts();
        }

        [WebMethod]
        public void TestFunction(string payload)
        {
            string safePayload = payload ?? string.Empty;
            Debug.WriteLine("[WebfuseBridge] TestFunction invoked. payload=" + safePayload);
            if(UsesNintendoStreamerPlayerRouting()){
                if (payload.Contains(GetEffectiveNintendoStreamerPlayer() + "|"))
                {
                    checkPosition(payload);
                }
            }
            else {
            if (payload.Contains(playerNum + "|"))
            {
                   checkPosition(payload);

            }}
        }

        private async void activeTrue()
        {
            SetMenuBarVisibleForActiveState(true);
            SetResizeButtonsVisibleForActiveState(true);
            await MinimizeMessenger.SendMessageAsync("active-true|" + playerNum);
        }

        private void SetResizeButtonsVisibleForActiveState(bool isOverjoyedActive)
        {
            bool showResizeButtons = !isOverjoyedActive;
            btnConfigSizeDown.Visible = showResizeButtons;
            btnConfigSizeUp.Visible = showResizeButtons;
        }

        private static string SerializeForJs(string value)
        {
            return JsonSerializer.Serialize(value ?? string.Empty);
        }

        private bool TryResolveMouseButtonPromptAction(string mouseButton, int endX, int endY, out int resolvedC1, out int resolvedC2, out int resolvedC3)
        {
            resolvedC1 = -1;
            resolvedC2 = -1;
            resolvedC3 = -1;

            float magnitudeX = Math.Abs(xStart - endX);
            float magnitudeY = Math.Abs(yStart - endY);
            double distance = Math.Sqrt(Math.Pow(magnitudeX, 2) + Math.Pow(magnitudeY, 2));

            if (distance > deadZone)
            {
                float angle = (float)Math.Atan2((endY - yStart), (endX - xStart));
                angle = (float)(180 / Math.PI) * angle;
                angle = SnapQuadrantBoundaryAngleToCardinal(angle);

                if (mouseButton == "left")
                {
                    if (angle >= -112.5 && angle < -67.5)
                    {
                        resolvedC1 = 8;
                        resolvedC2 = chkMarioKart.Checked ? 10 : 8;
                        resolvedC3 = 8;
                    }
                    else if (angle >= -67.5 && angle < -22.5)
                    {
                        resolvedC1 = 9;
                        if (diagUpRightLC)
                        {
                            resolvedC2 = 8;
                            resolvedC3 = 10;
                        }
                        else
                        {
                            resolvedC2 = 9;
                            resolvedC3 = 9;
                        }
                    }
                    else if (angle >= -22.5 && angle < 22.5)
                    {
                        resolvedC1 = 10;
                        resolvedC2 = 10;
                        resolvedC3 = 10;
                    }
                    else if (angle >= 22.5 && angle < 67.5)
                    {
                        resolvedC1 = 11;
                        if (diagDownRightLC)
                        {
                            resolvedC2 = 10;
                            resolvedC3 = 12;
                        }
                        else
                        {
                            resolvedC2 = 11;
                            resolvedC3 = 11;
                        }
                    }
                    else if (angle >= 67.5 && angle < 112.5)
                    {
                        resolvedC1 = 12;
                        resolvedC2 = 12;
                        resolvedC3 = 12;
                    }
                    else if (angle >= 112.5 && angle < 157.5)
                    {
                        resolvedC1 = 13;
                        if (diagDownLeftLC)
                        {
                            resolvedC2 = 12;
                            resolvedC3 = 14;
                        }
                        else
                        {
                            resolvedC2 = 13;
                            resolvedC3 = 13;
                        }
                    }
                    else if (angle >= 157.5 || angle < -157.5)
                    {
                        resolvedC1 = 14;
                        resolvedC2 = 14;
                        resolvedC3 = 14;
                    }
                    else if (angle >= -157.5 && angle < -112.5)
                    {
                        resolvedC1 = 15;
                        if (diagUpLeftLC)
                        {
                            resolvedC2 = 14;
                            resolvedC3 = 8;
                        }
                        else
                        {
                            resolvedC2 = 15;
                            resolvedC3 = 15;
                        }
                    }
                }
                else if (mouseButton == "right")
                {
                    if (angle >= -112.5 && angle < -67.5)
                    {
                        resolvedC1 = 16;
                        resolvedC2 = 16;
                        resolvedC3 = 16;
                    }
                    else if (angle >= -67.5 && angle < -22.5)
                    {
                        resolvedC1 = 17;
                        if (diagUpRightRC)
                        {
                            resolvedC2 = 16;
                            resolvedC3 = 18;
                        }
                        else
                        {
                            resolvedC2 = 17;
                            resolvedC3 = 17;
                        }
                    }
                    else if (angle >= -22.5 && angle < 22.5)
                    {
                        resolvedC1 = 18;
                        resolvedC2 = 18;
                        resolvedC3 = 18;
                    }
                    else if (angle >= 22.5 && angle < 67.5)
                    {
                        resolvedC1 = 19;
                        if (diagDownRightRC)
                        {
                            resolvedC2 = 18;
                            resolvedC3 = 20;
                        }
                        else
                        {
                            resolvedC2 = 19;
                            resolvedC3 = 19;
                        }
                    }
                    else if (angle >= 67.5 && angle < 112.5)
                    {
                        resolvedC1 = 20;
                        resolvedC2 = 20;
                        resolvedC3 = 20;
                    }
                    else if (angle >= 112.5 && angle < 157.5)
                    {
                        resolvedC1 = 21;
                        if (diagDownLeftRC)
                        {
                            resolvedC2 = 20;
                            resolvedC3 = 22;
                        }
                        else
                        {
                            resolvedC2 = 21;
                            resolvedC3 = 21;
                        }
                    }
                    else if (angle >= 157.5 || angle < -157.5)
                    {
                        resolvedC1 = 22;
                        resolvedC2 = 22;
                        resolvedC3 = 22;
                    }
                    else if (angle >= -157.5 && angle < -112.5)
                    {
                        resolvedC1 = 23;
                        if (diagUpLeftRC)
                        {
                            resolvedC2 = 22;
                            resolvedC3 = 16;
                        }
                        else
                        {
                            resolvedC2 = 23;
                            resolvedC3 = 23;
                        }
                    }
                }
            }
            else if (mouseButton == "left")
            {
                if (distance > deadZone / 2 && (yStart - endY) > 0)
                {
                    resolvedC1 = 25;
                    resolvedC2 = 25;
                    resolvedC3 = 25;
                }
                else if (distance <= deadZone / 2)
                {
                    resolvedC1 = 28;
                    resolvedC2 = 28;
                    resolvedC3 = 28;
                }
                else if ((yStart - endY) < 0 && distance > deadZone / 2)
                {
                    resolvedC1 = 31;
                    resolvedC2 = 31;
                    resolvedC3 = 31;
                }
            }
            else if (mouseButton == "right")
            {
                if (distance > deadZone / 2 && (yStart - endY) > 0)
                {
                    resolvedC1 = 26;
                    resolvedC2 = 26;
                    resolvedC3 = 26;
                }
                else if (distance <= deadZone / 2)
                {
                    resolvedC1 = 29;
                    resolvedC2 = 29;
                    resolvedC3 = 29;
                }
                else if ((yStart - endY) < 0 && distance > deadZone / 2)
                {
                    resolvedC1 = 32;
                    resolvedC2 = 32;
                    resolvedC3 = 32;
                }
            }

            return resolvedC1 >= 0;
        }

        private void InitialSize()
        {
            string initialSizePath = Path.Combine(configPath, "InitialSize.txt");

            var content = System.IO.File.ReadAllText(Path.Combine(configPath, "player.txt"));

            if (content?.IndexOf("player1", StringComparison.OrdinalIgnoreCase) >= 0 && content?.IndexOf("player2", StringComparison.OrdinalIgnoreCase) >= 0 && content?.IndexOf("player3", StringComparison.OrdinalIgnoreCase) < 0 && content?.IndexOf("player4", StringComparison.OrdinalIgnoreCase) < 0)
            {
                initialSizePath = Path.Combine(configPath, "InitialSize2.txt");
            }

            else if (content?.IndexOf("player1", StringComparison.OrdinalIgnoreCase) >= 0 && content?.IndexOf("player2", StringComparison.OrdinalIgnoreCase) >= 0 && content?.IndexOf("player3", StringComparison.OrdinalIgnoreCase) >= 0 && content?.IndexOf("player4", StringComparison.OrdinalIgnoreCase) < 0)
            {
                initialSizePath = Path.Combine(configPath, "InitialSize3.txt");
            }
            else if (content?.IndexOf("player1", StringComparison.OrdinalIgnoreCase) >= 0 && content?.IndexOf("player2", StringComparison.OrdinalIgnoreCase) >= 0 && content?.IndexOf("player3", StringComparison.OrdinalIgnoreCase) >= 0 && content?.IndexOf("player4", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                initialSizePath = Path.Combine(configPath, "InitialSize4.txt");
            }

            try
            {
                // Open the file with FileShare.Read to allow simultaneous reading
                using (FileStream fs = new FileStream(initialSizePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader reader = new StreamReader(fs))
                {

                    browserWidth = double.Parse(reader.ReadLine() ?? "616") - 16;
                }
            }
            catch (FormatException ex)
            {
                // Handle cases where parsing fails (e.g., invalid format)
                Debug.WriteLine($"Error parsing the file: {ex.Message}");
            }
            double browserHeight = this.Height;

            //browserWidth = this.Width;

            float scale = (float)(browserWidth / formWidth);

            //MessageBox.Show(browserWidth.ToString());

            //MessageBox.Show(initWidth.ToString());
            //scale = 1.6f;

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
            int currentFont = 12;

            if (Screen.Bounds.Height >= 1100 && Screen.Bounds.Width > 1920)
            {
                currentFont = 16;
            }

            Eval(@"function updateTruncateCSS(scale = 1) {
  const styleId = 'dynamic-truncate-style';
  const oldStyle = document.getElementById(styleId);

  // Default font size (if none previously defined)
  let currentFontSize = " + currentFont + @";

  // If a previous style block exists, extract its font size
  if (oldStyle) {
    
    oldStyle.remove();
  }

  // Compute the new size
  const newFontSize = currentFontSize * scale;

  // Create new style block
  const style = document.createElement('style');
  style.id = styleId;
  style.textContent = `
    .truncate {
      
      font-size: ${newFontSize}pt!important;
    }
  `;

  // Append new style to head
  document.head.appendChild(style);
}updateTruncateCSS(" + scale + ")");


            // Calculate the zoom factor based on the difference between form height and browser height.
            zoomFactor = 1.0 - (double)((formWidth - browserWidth) / formWidth);
        }

        private void SetLblActiveInactiveUi(string text)
        {
            int topPx = (int)(331 * zoomFactor);
            Eval(@"(() => {
                const p = window.overjoyedParts;
                if (p && typeof p.setLblActiveInactive === 'function') {
                    p.setLblActiveInactive(" + SerializeForJs(text) + @", " + topPx + @");
                }
            })();");
        }

        private void SetLblActiveActiveUi(string text)
        {
            if (chkCombineControllers.Checked && scanned)
            {
                Eval(@"(() => {
                    const p = window.overjoyedParts;
                    if (!p) {
                        return;
                    }

                    if (typeof p.setLblActiveText === 'function') {
                        p.setLblActiveText(" + SerializeForJs(text) + @");
                    }

                    if (typeof p.applyLblActiveState === 'function') {
                        p.applyLblActiveState('active');
                    }

                    if (typeof p.setLblActiveVisible === 'function') {
                        p.setLblActiveVisible(false);
                    }
                })();");
                return;
            }

            Eval(@"(() => {
                const p = window.overjoyedParts;
                if (p && typeof p.setLblActiveActive === 'function') {
                    p.setLblActiveActive(" + SerializeForJs(text) + @");
                }
            })();");
        }

        private void SetLblActiveTextUi(string text)
        {
            Eval(@"(() => {
                const p = window.overjoyedParts;
                if (p && typeof p.setLblActiveText === 'function') {
                    p.setLblActiveText(" + SerializeForJs(text) + @");
                }
            })();");
        }

        private void SetLblActiveTopUi(int topPx)
        {
        }

        private void SetLblActiveVisibleUi(bool visible)
        {
            Eval(@"(() => {
                const p = window.overjoyedParts;
                if (p && typeof p.setLblActiveVisible === 'function') {
                    p.setLblActiveVisible(" + visible.ToString().ToLower() + @");
                }
            })();");
        }

        private string GetActiveLblActiveMessage()
        {
            if (ActivateUpper)
            {
                return "Overjoyed is active. Hold click in Upper Deadzone to deactivate!";
            }

            if (ActivateMiddle)
            {
                return "Overjoyed is active. Hold click in Middle Deadzone to deactivate!";
            }

            return "Overjoyed is active. Hold click in Lower Deadzone to deactivate!";
        }

        private bool IsLblActiveInactiveState()
        {
            return !isActive;
        }

        private bool IsHoldFriendlyMouseButton(MouseButtons button)
        {
            return button == MouseButtons.Left || button == MouseButtons.Right;
        }

        private void EnsureLblActiveHoldAnimationCss()
        {
            // Static CSS for the activation overlay hold-fill lives in Active.resx.
        }

        private void CancelLblActiveHoldAnimation()
        {
            Eval(@"(() => {
                const p = window.overjoyedParts;
                if (p && typeof p.cancelLblActiveHoldAnimation === 'function') {
                    p.cancelLblActiveHoldAnimation();
                }
            })();");
        }

        private void StartLblActiveHoldAnimation()
        {
            Eval(@"(() => {
                const p = window.overjoyedParts;
                if (p && typeof p.startLblActiveHoldAnimation === 'function') {
                    p.startLblActiveHoldAnimation();
                }
            })();");
        }

        private void ActivateFromDeadZone(bool captureFocusMouse = true)
        {
            if (IsControllerDetectionGateActive())
            {
                SetLblActiveInactiveUi("Gamepad detection started. Press A button on your controller.");
                return;
            }

            CancelLblActiveHoldAnimation();
            Reset();

            if (scanned || chkCombineControllers.Checked || lblCombine.Visible)
            {
                SetLblActiveVisibleUi(false);
            }

            pnlExtraButtons.Enabled = false;
            pnlExtraButtons.Visible = false;
            chkSteam.Visible = false;
            lblUseOverjoyed.Visible = false;
            chkCombineLS.Visible = false;
            chkCombineRS.Visible = false;
            chkCombineLT.Visible = false;
            chkCombineRT.Visible = false;

            isActive = true;

            hoverButtons.Visible = true;
            clickButtons.Visible = true;

            if (captureFocusMouse && chkKeepMouseInside.Checked)
            {
                ClipCursor();
            }

            lblLegend.BackColor = System.Drawing.Color.FromArgb(0, 167, 209);
            lblLegend.ForeColor = System.Drawing.Color.White;

            SetLblActiveActiveUi(GetActiveLblActiveMessage());

            activeTrue();
        }

        private void pnlBaseOverlay_MouseLeave(object sender, EventArgs e)
        {
            if (IsLblActiveInactiveState())
            {
                CancelLblActiveHoldAnimation();
            }

            if (!positionCheck)
            {
                // Get the current cursor position in screen coordinates.
                Point cursorPos = Wisej.Web.Cursor.Position;

                // Convert to panel's client coordinates.
                Point clientPos = pnlBaseOverlay.PointToClient(cursorPos);

                // If the client rectangle still contains the point, do nothing.
                if (pnlBaseOverlay.ClientRectangle.Contains(clientPos))
                {
                    return;
                }


                if (isActive)
                {
                    DeactivateFromExitHold(true);
                }
            }

        }

        private void pnlBaseOverlay_DoubleClick(object sender, EventArgs e)
        {
            if (chkRemote.Checked || DateTime.UtcNow <= suppressBaseOverlayDoubleClickUntil)
            {
                return;
            }

            pnlDoubleClick();


        }

        private void pnlDoubleClick()
        {
            if (!chkDisableClicks.Checked)
            {
                if (positionCheck)
                {

                    xEnd = (int)(remoteX);
                    yEnd = (int)(remoteY);
                }
                else
                {
                    xEnd = (int)(Cursor.Position.X / zoomFactor);
                    yEnd = (int)(Cursor.Position.Y / zoomFactor);
                }

                float _magnitudeX = Math.Abs(xStart - xEnd);
                float _magnitudeY = Math.Abs(yStart - yEnd);
                double distance = Math.Sqrt(Math.Pow(_magnitudeX, 2) + Math.Pow(_magnitudeY, 2));

                if (isActive && chkFPS.Checked && showLabelsLC && distance <= deadZone / 2)
                {
                    fpsDeadZoneLookModeLatched = !fpsDeadZoneLookModeLatched;
                    drawPrompts();
                    return;
                }

                //if ((distance <= deadZone) && isActive)
                //{
                //    if ((distance > deadZone / 2 && ((yStart - yEnd) > 0)))
                //    {

                //        c1 = 25; c2 = 25; c3 = 25;

                //        if (ActivateUpper && !positionCheck)
                //        {
                //            DeactivateFromExitHold(true);
                //        }




                //    }
                //    else if ((distance <= deadZone / 2))
                //    {
                //        //if (chkFPS.Checked && lblPrompts[1].ForeColor == Color.Black)
                //        //{

                //        //    lblPrompts[1].Font = new System.Drawing.Font(lblPrompts[1].Font.Name, lblPrompts[1].Font.Size, FontStyle.Bold);
                //        //    lblPrompts[1].ForeColor = Color.FromArgb(0, 167, 209);
                //        //}
                //        //else if (chkFPS.Checked && lblPrompts[1].ForeColor == Color.FromArgb(0, 167, 209))
                //        //{
                //        //    lblPrompts[1].ForeColor = Color.Black;
                //        //    lblPrompts[1].Font = new System.Drawing.Font(lblPrompts[1].Font.Name, lblPrompts[1].Font.Size, FontStyle.Regular);
                //        //}
                //        c1 = 28; c2 = 28; c3 = 28;



                //        if (ActivateMiddle && !positionCheck)
                //        {
                //            DeactivateFromExitHold(true);
                //        }




                //    }
                //    else if (((yStart - yEnd) < 0) && (distance > deadZone / 2))
                //    {

                //        c1 = 31; c2 = 31; c3 = 31;

                //        if (ActivateLower && !positionCheck)
                //        {
                //            DeactivateFromExitHold(true);
                //        }




                //    }



                //}

                if (!isActive)
                {
                    if (positionCheck || chkRemote.Checked)
                    {
                        return;
                    }

                    ActivateFromDeadZone();


                }
            }

        }

        private async void resizer()
        {
            await ActiveMessenger.SendMessageAsyncActive("resize|" + playerNum);

        }

        private void chkSafeZone_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSafeZone.Checked)
            {
                safeZone = true;
                ShowSafeZoneOverlay();
            }
            else
            {
                safeZone = false;
                Eval("safeZone = document.getElementById('safeZone'); if(safeZone) {safeZone.style.visibility = 'hidden';}");
            }
        }


        private void resetCursor()
        {
            if (!switchMode)
            {
                cursorOn();
            }
            else if (switchMode)
            {
                cursorOff();
            }
        }



        public async void ClipCursor()
        {
            await MinimizeMessenger.SendMessageAsync("clipcursor|" + playerNum);

        }


        public async void UnclipCursor()
        {
            await MinimizeMessenger.SendMessageAsync("unclipcursor|" + playerNum);


        }

        private async void cursorOn()
        {
            await MinimizeMessenger.SendMessageAsync("resetcursor-on|" + playerNum);
        }

        private async void cursorOff()
        {
            await MinimizeMessenger.SendMessageAsync("resetcursor-off|" + playerNum);
        }

        private void SetPointerGlow(string mouseButton, bool enabled, bool delayShortRelease = false)
        {
            string glowClass = mouseButton == "right" ? "glow-right" : "glow-left";
            string tokenKey = mouseButton == "right" ? "right" : "left";

            if (enabled)
            {
                if (mouseButton == "right")
                {
                    lastRightMouseDownAt = DateTime.UtcNow;
                }
                else
                {
                    lastLeftMouseDownAt = DateTime.UtcNow;
                }

                Eval(@"const cursor = document.getElementById('cursorSquare');
const socket = document.getElementById('socketSquare');
window.overjoyedPointerGlowTokens = window.overjoyedPointerGlowTokens || Object.create(null);
window.overjoyedPointerGlowTokens['" + tokenKey + @"'] = (window.overjoyedPointerGlowTokens['" + tokenKey + @"'] || 0) + 1;

if (cursor) cursor.classList.add('" + glowClass + @"');
if (socket) socket.classList.add('" + glowClass + @"');
");
                return;
            }

            DateTime pressedAt = mouseButton == "right" ? lastRightMouseDownAt : lastLeftMouseDownAt;
            int remainingMs = delayShortRelease
                    ? MinimumPointerGlowMs
                    : Math.Max(0, MinimumPointerGlowMs - (int)(DateTime.UtcNow - pressedAt).TotalMilliseconds);

            if (remainingMs > 0)
            {
                Eval(@"(() => {
    window.overjoyedPointerGlowTokens = window.overjoyedPointerGlowTokens || Object.create(null);
    const token = window.overjoyedPointerGlowTokens['" + tokenKey + @"'] || 0;

    setTimeout(() => {
        if ((window.overjoyedPointerGlowTokens['" + tokenKey + @"'] || 0) !== token) {
            return;
        }

  const cursor = document.getElementById('cursorSquare');
  const socket = document.getElementById('socketSquare');

  if (cursor) cursor.classList.remove('" + glowClass + @"');
  if (socket) socket.classList.remove('" + glowClass + @"');
    }, " + remainingMs + @");
})();
");
                return;
            }

            Eval(@"const cursor = document.getElementById('cursorSquare');
const socket = document.getElementById('socketSquare');

if (cursor) cursor.classList.remove('" + glowClass + @"');
if (socket) socket.classList.remove('" + glowClass + @"');
");
        }

        private void hideBG_CheckedChanged(object sender, EventArgs e)
        {
            //            if (hideBG.Checked)
            //            {
            //                               resetTransparency(false, true);
            //            ExtendedMessageBox.Show("<p style='margin:0;padding:0;line-height:1.6;'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 15; -webkit-box-orient: vertical; overflow: hidden; font-size:"+truncateSize+"pt; ' title=''>After you press OK a popup will appear. Please choose your game window, then click Share.", zoomFactor: zoomFactor);
            //            resetTransparency(true, true);

            //                Eval(@"// Check if the screenShare div and the container already exist
            //let screenShareDiv = document.querySelector('.screenshare');
            //let screenContainer = document.querySelector('#container');

            //if (!screenShareDiv) {
            //    // Create the main container div
            //    screenShareDiv = document.createElement('div');
            //    screenShareDiv.classList.add('screenshare');
            //}

            //if (!screenContainer) {
            //    // Create the screen container div
            //    screenContainer = document.createElement('div');
            //    screenContainer.classList.add('screen-container');
            //    screenContainer.id = 'container';

            //    // Append the screen container to the screen share div
            //    screenShareDiv.appendChild(screenContainer);

            //    // Append the whole structure to the body (or any other container)
            //    document.body.appendChild(screenShareDiv);
            //}

            //const container = document.querySelector('.screen-container');
            //if (container) {

            //container.style.width = (screen.width * window.devicePixelRatio) + 'px';
            //container.style.height = (screen.height * window.devicePixelRatio) + 'px';
            //}

            //// Get the parent element
            //const parent = document.documentElement;
            //if (parent) {

            //// Get the zoom value of the parent
            //const zoom = window.getComputedStyle(parent).zoom || 1;}

            //// Calculate the inverse of the zoom value
            //const inverseZoom = 1 / parseFloat(zoom);

            //// Get the child element
            //const child = document.querySelector('.screenshare');
            //if(child) {
            //// Apply the inverse zoom to the child element
            //child.style.zoom = inverseZoom / window.devicePixelRatio;
            //}
            //const displayMediaOptions = {
            //  video: {
            //    cursor: 'never',
            //    frameRate: { ideal: 60, max: 60 }
            //  },
            //  monitorTypeSurfaces: 'exclude'
            //};

            //// Request screen share with cursor constraints
            //navigator.mediaDevices.getDisplayMedia(displayMediaOptions)
            //    .then(function (stream) {
            //        window.screenShareStream = stream; // Store the stream globally for later stopping

            //        var video = document.createElement('video');
            //        video.srcObject = stream;
            //        video.autoplay = true;
            //        video.classList.add('screen');
            //        document.getElementById('container').appendChild(video);

            //        let lastX = window.screenX;
            //        let lastY = window.screenY;

            //        function updateSize() {
            //            var settings = stream.getVideoTracks()[0].getSettings();
            //            video.style.width = settings.width + 'px';
            //            video.style.height = settings.height + 'px';

            //            const dpr = window.devicePixelRatio || 1;
            //            const adjustment = 4 - Math.round((dpr - 1) / 0.25) * 12;
            //            const baseLeft = ((screen.width - 600) / 2) * dpr;
            //            const baseTop = (screen.height - 648) / 2 + 32 * dpr + adjustment;

            //            // Adjust based on window movement
            //            const deltaX = baseLeft - lastX;
            //            const deltaY = baseTop - lastY;

            //            video.style.left = `-${baseLeft}px`;
            //            video.style.top = `-${(baseTop * (dpr === 1 ? 1 : dpr))}px`;

            //            screenShareDiv.style.left = `${deltaX}px`;
            //            screenShareDiv.style.top = `${deltaY}px`;
            //        }

            //        // Run when metadata is loaded
            //        video.onloadedmetadata = updateSize;

            //        // Check for resolution changes
            //        setInterval(updateSize, 500);

            //        // Detect window movement and adjust position
            //        setInterval(() => {
            //            if (window.screenX !== lastX || window.screenY !== lastY) {
            //                updateSize(); // Update the video position
            //                lastX = window.screenX;
            //                lastY = window.screenY;
            //            }
            //        }, 10);
            //    })
            //    .catch(function (error) {
            //        console.error('Screen share failed:', error);
            //    });



            //");

            //            }
            //            else { Eval(@"if (window.screenShareStream) {
            //    window.screenShareStream.getTracks().forEach(track => track.stop());
            //    console.log('Screen sharing stopped.');
            //    window.screenShareStream = null;
            //} else {
            //    console.log('No active screen sharing session.');
            //}

            //"); }

        }

        private void lblCombine_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (isActive && !positionCheck)
            {
                lblCombine.Text = "";
                DeactivateFromExitHold(true);
            }
        }

        private void lblCombine_MouseHover(object sender, EventArgs e)
        {
            if (isActive)
            {
                lblCombine.Text = "Double Click to Use Emergency Escape Hatch";
            }
        }

        private void chkMouseLock_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMouseLock.Checked)
            {
                cursorOn();
            }
            else
            {
                cursorOff();
            }
        }

        private void closeDiagram_Click(object sender, EventArgs e)
        {
            HideCombineDiagramOverlay();

            closeDiagram.Visible = false;
            closeDiagram.SendToBack();
            hideBG.Visible = false;
            chkSteam.Visible = true;
            chkSteam.Checked = false;
            pnlExtraButtons.Visible = true;
            pnlBaseOverlay.Visible = true;
            pnlBaseOverlay.BringToFront();
            SetLblActiveVisibleUi(true);
            chkSteam.BringToFront();

        }

        private void ShowCombineDiagramOverlay()
        {
            combineDiagram.Visible = true;
            combineDiagram.BringToFront();
            SetElementVisibility("#outerSVG", "hidden");
            SetElementVisibility("div[name=\"combineDiagram\"] div[name=\"html\"]", "visible");
        }

        private void HideCombineDiagramOverlay()
        {
            SetElementVisibility("div[name=\"combineDiagram\"] div[name=\"html\"]", "hidden");
            SetElementVisibility("#outerSVG", "visible");
            combineDiagram.Visible = false;
            combineDiagram.SendToBack();
        }

        private void EnsureWebfusePanelCreated()
        {
            Eval(@"(() => {
    let panel = document.getElementById('webfusePanel');
    if (!panel) {
        panel = document.createElement('iframe');
        panel.id = 'webfusePanel';
        panel.style.width = '100%';
        panel.style.height = '100%';
        panel.style.background = 'beige';
        panel.style.overflow = 'hidden';
        panel.style.display = 'none';
        panel.style.position = 'fixed';
        panel.style.top = '0';
        panel.style.left = '0';
        panel.style.zIndex = '-1';
        panel.style.pointerEvents = 'none';

        document.body.appendChild(panel);
    }

    let sendBackButton = document.getElementById('webfusePanelSendBackButton');
    if (!sendBackButton) {
        sendBackButton = document.createElement('button');
        sendBackButton.id = 'webfusePanelSendBackButton';
        sendBackButton.type = 'button';
        sendBackButton.textContent = 'Send Back';
        sendBackButton.style.position = 'absolute';
        sendBackButton.style.top = '8px';
        sendBackButton.style.right = '8px';
        sendBackButton.style.zIndex = '10000';
        sendBackButton.style.padding = '6px 10px';
        sendBackButton.style.border = '1px solid #666';
        sendBackButton.style.borderRadius = '6px';
        sendBackButton.style.background = '#fff';
        sendBackButton.style.cursor = 'pointer';

        sendBackButton.addEventListener('click', () => {
            const currentPanel = document.getElementById('webfusePanel');
            if (currentPanel) {
                currentPanel.style.zIndex = '-1';
                currentPanel.style.pointerEvents = 'none';
            }
            sendBackButton.style.display = 'none';
        });
        sendBackButton.style.display = 'none';
        document.body.appendChild(sendBackButton);
    }
})();");
        }

        private void ShowWebfusePanel()
        {
            EnsureWebfusePanelCreated();
            EnsureWebfuseBridgeInjected();
            Eval(@"(() => {
    const panel = document.getElementById('webfusePanel');
    const sendBackButton = document.getElementById('webfusePanelSendBackButton');
    if (panel) {
        panel.style.display = 'block';
        panel.style.zIndex = '9999';
        panel.style.pointerEvents = 'auto';
    }
    if (sendBackButton) {
        sendBackButton.style.display = 'block';
    }
})();");
            Eval(@"(function (w, e, b, f, u, s) {
    w[f] = w[f] || {
        initSpace: function () {
            return new Promise(resolve => {
                w[f].q = arguments;
                w[f].resolve = resolve;
            });
        },
    };
    u = e.createElement(b);
    s = e.getElementsByTagName(b)[0];
    u.async = 1;
    u.src = 'https://webfuse.com/surfly.js';
    s.parentNode.insertBefore(u, s);
})(window, document, 'script', 'webfuse');

webfuse.initSpace('wk_azRmWG7bu1oRVQue30PUgMzWu_OIzXld', '3052', { 
    session_autorestore_enabled: false
})
 .then(async space => {
    const session = space.session();

        session.on('session_started', function(session) {
                // Open a tab when the session starts
                session.openTab('https://overjoyed.vercel.app/OBS.html');
                // Activate the new tab
                session.getTabs().then(function(tabs) {
                    if (tabs.length > 0) {
                        session.activateTab(tabs[tabs.length - 1].id);
                    }
                });
        });
         session.on('message', function(session,event){
                    console.log(event.data);
            });

      const isAliveHost = (candidate) => {
                if (!candidate) {
                    return false;
                }

                if (typeof candidate.isDisposed === 'function') {
                    try {
                        if (candidate.isDisposed()) {
                            return false;
                        }
                    } catch (_) {
                        // Ignore lifecycle probing errors and try invocation path.
                    }
                }

                return typeof candidate.TestFunction === 'function';
            };

      const invokeTestFunction = (payload) => {
                if (!window.App) {
                    return false;
                }

                if (isAliveHost(window.App.Active)) {
                    window.App.Active.TestFunction(payload);
                    return true;
                }

                const appKeys = Object.keys(window.App);
                for (let i = 0; i < appKeys.length; i++) {
                    const candidate = window.App[appKeys[i]];
                    if (isAliveHost(candidate)) {
                        candidate.TestFunction(payload);
                        return true;
                    }
                }

                return false;
            };

      session.on('chat_message', (session, event) => {
                const message = event.message;

                if (message.includes('|')) {
                    const invoked = invokeTestFunction(message);
                    if (!invoked) {
                        console.warn('Could not find TestFunction on App host for message:', message);
                    }
                }
                else {
                    console.log('Received message:', event.message);
                }
            });


    // Start the Session
    await session.start('#webfusePanel');
})
  .catch(function (error) {
    console.error('Failed:', error);
  });");
        }

        private void HideWebfusePanel()
        {
            Eval(@"(() => {
    const panel = document.getElementById('webfusePanel');
    const sendBackButton = document.getElementById('webfusePanelSendBackButton');
    if (panel) {
        panel.style.display = 'none';
        panel.style.zIndex = '-1';
        panel.style.pointerEvents = 'none';
    }
    if (sendBackButton) {
        sendBackButton.style.display = 'none';
    }
})();");
        }

        private void DeleteWebfusePanel()
        {
            Eval(@"(() => {
    const panel = document.getElementById('webfusePanel');
    const sendBackButton = document.getElementById('webfusePanelSendBackButton');

    if (sendBackButton && sendBackButton.parentNode) {
        sendBackButton.parentNode.removeChild(sendBackButton);
    }

    if (panel && panel.parentNode) {
        panel.parentNode.removeChild(panel);
    }
})();");
        }

        private void EnsureWebfuseBridgeInjected()
        {
            if (webfuseBridgeInjected)
            {
                return;
            }

            webfuseBridgeInjected = true;
        }

        private void SetElementVisibility(string selector, string visibility)
        {
            if (string.IsNullOrWhiteSpace(selector) || string.IsNullOrWhiteSpace(visibility))
                return;

            string safeSelector = JsonSerializer.Serialize(selector);
            string safeVisibility = JsonSerializer.Serialize(visibility);
            Eval($@"(() => {{
    const element = document.querySelector({safeSelector});
    if (element) {{
        element.style.visibility = {safeVisibility};
    }}
}})();");
        }

        private void ScaleFonts(Control control, float scale)
        {
            if (control?.Font != null)
            {
                control.Font = new Font(control.Font.FontFamily, control.Font.Size * scale, control.Font.Style, control.Font.Unit);
            }

            foreach (Control child in control.Controls)
            {
                ScaleFonts(child, scale);
            }
        }

        private void SetClickButtonsText(ClickThreadMode mode, int c1, int c2, int c3, bool ongoing)
        {
            if (c2 == c3)
            {
                clickButtons.Text = ongoing
                    ? "Click: " + GetClickThreadButtonLabel(mode, c2) + " is being pressed."
                    : "Click: " + GetClickThreadButtonLabel(mode, c2) + " was pressed.";
                return;
            }

            clickButtons.Text = ongoing
                ? "Click: " + GetClickThreadButtonLabel(mode, c2) + " and " + GetClickThreadButtonLabel(mode, c3) + " are being pressed."
                : "Click: " + GetClickThreadButtonLabel(mode, c2) + " and " + GetClickThreadButtonLabel(mode, c3) + " were pressed.";
        }

        private string GetClickThreadButtonLabel(ClickThreadMode mode, int index)
        {
            return mode switch
            {
                ClickThreadMode.Keyboard => FormatInputLabel(keyCodes[index].ToString()),
                ClickThreadMode.Xbox => FormatInputLabel(xbuttons[index].ToString()),
                _ => FormatInputLabel(switchbuttons[index].ToString()),
            };
        }

        private void SetClickButtonsIdleText()
        {
            clickButtons.Text = "Click: Nothing is being pressed.";
        }

        private void SetClickThreadText(ClickThreadMode mode, int b1, int b2)
        {
            if (b1 == b2)
            {
                clickButtons.Text = "Click: " + GetClickThreadButtonLabel(mode, b2) + " was pressed.";
            }
            else
            {
                clickButtons.Text = "Click: " + GetClickThreadButtonLabel(mode, b1) + " and " + GetClickThreadButtonLabel(mode, b2) + " were pressed.";
            }
        }

        private void SetHoverButtonsText(ClickThreadMode mode, int b0, int b1, int b2, bool ongoing)
        {
            string firstLabel = GetClickThreadButtonLabel(mode, b1);
            string secondLabel = GetClickThreadButtonLabel(mode, b2);

            if (b1 == b2)
            {
                hoverButtons.Text = ongoing
                    ? "Move: " + firstLabel + " is being pressed."
                    : "Move: " + firstLabel + " was pressed.";
                return;
            }

            hoverButtons.Text = ongoing
                ? "Move: " + firstLabel + " and " + secondLabel + " are being pressed."
                : "Move: " + firstLabel + " and " + secondLabel + " were pressed.";
        }

        private Task SetHoverButtonsIdleTextAsync()
        {
            return MainThread.InvokeOnMainThreadAsync(() => { hoverButtons.Text = "Move: Nothing is being pressed."; });
        }

        private Task SetMouseMoveTextAsync(ClickThreadMode mode, int b1, int b2, bool ongoing)
        {
            return MainThread.InvokeOnMainThreadAsync(() =>
            {
                SetHoverButtonsText(mode, b1, b1, b2, ongoing);
            });
        }

        private Task SetSingleMouseMoveTextAsync(ClickThreadMode mode, int keyboardIndex, int displayIndex, bool ongoing)
        {
            int labelIndex = mode == ClickThreadMode.Keyboard ? keyboardIndex : displayIndex;
            return SetMouseMoveTextAsync(mode, labelIndex, labelIndex, ongoing);
        }

        private Task SetMouseMoveLegendAsync(string moveLegend, string leftLegend, string rightLegend)
        {
            return MainThread.InvokeOnMainThreadAsync(new Action(() =>
            {
                lblLegend.Text = "LEGEND: MM - " + FormatLegendLabel(moveLegend)
                    + " | LC - " + FormatLegendLabel(leftLegend)
                    + " | RC - " + FormatLegendLabel(rightLegend, expandSymbols: false);
            }));
        }

        private void NormalizeClickThreadTargets(ClickThreadMode mode, int originalB0, ref int b0, ref int b1, ref int b2)
        {
            // Preserve the existing targets by default. Some call paths mutate these before invocation,
            // so a no-op normalization keeps behavior stable for the extracted click-thread helpers.
        }

        private void RestoreNormalizedClickThreadTarget(ClickThreadMode mode)
        {
            // No-op companion to NormalizeClickThreadTargets.
        }

        private static string FormatInputLabel(string value)
        {
            return value
                .Replace("VK_", "")
                .Replace("OEM_1", "Semicolon")
                .Replace("OEM_2", "Slash")
                .Replace("OEM_3", "Backtick")
                .Replace("OEM_4", "LBracket")
                .Replace("OEM_5", "Backslash")
                .Replace("OEM_6", "RBracket")
                .Replace("OEM_7", "Quote")
                .Replace("LWIN", "Win")
                .Replace("OEM_COMMA", "Comma")
                .Replace("OEM_PERIOD", "Period")
                .Replace("OEM_PLUS", "Equal")
                .Replace("OEM_MINUS", "Minus")
                .Replace("CAPITAL", "CapsLk")
                .Replace("ESCAPE", "Esc")
                .Replace("PRIOR", "PageUp");
        }

        private static string FormatLegendLabel(string value, bool expandSymbols = true)
        {
            var formatted = value.Replace("VK_", "");
            if (!expandSymbols)
                return formatted;

            return FormatInputLabel(formatted);
        }
    }
}