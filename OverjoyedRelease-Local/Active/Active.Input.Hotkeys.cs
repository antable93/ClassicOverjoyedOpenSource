using ExtendedMessageBoxLibrary;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Wisej.Web;

namespace OverjoyedReleaseLocal
{
    public partial class Active : Page
    {
        private DateTime lastHotkeyDownTime = DateTime.MinValue;

        public static bool hotkeyDown = false;
        public static bool hotkeyUp = false;

        private bool combineControllers = false;
        private bool disableClicks = false;


        private void lblCombine_MouseLeave(object sender, EventArgs e)
        {
            if (isActive)
            {
                lblCombine.Text = "";
            }
        }

        private void lblCombine_MouseClick(object sender, MouseEventArgs e)
        {
            if (lblCombine.Text == "Gamepad disconnected. Click here to retry detection.")
            {
                Task.Run(() => StartGamepadDetection());
                SetLblActiveInactiveUi("Gamepad detection started. Press A button on your controller.");

            }
        }


        private void HandleHotkeyPayload(string payload)
        {
            if (payload.Contains("keydown-"))
            {
                HandleHotkeyDown(payload.Replace("keydown-", ""));
            }

            if (payload.Contains("keyup-"))
            {
                HandleHotkeyUp(payload.Replace("keyup-", ""));
            }
        }

        private bool ShouldHandleQuadrantHotkeys()
        {
            return chkCombineControllers.Checked || chkDisableClicks.Checked || chkVoice.Checked;
        }

        private void HandleHotkeyDown(string hotkey)
        {
            bool treatAsRegularDisableClick = chkDisableClicks.Checked && (hotkey == "0" || hotkey == "1");

            if (!treatAsRegularDisableClick)
            {
                if ((DateTime.Now - lastHotkeyDownTime).TotalMilliseconds < 200)
                {
                    return;
                }

                lastHotkeyDownTime = DateTime.Now;
            }

            if (chkDisableClicks.Checked)
            {
                Debug.WriteLine($"Hotkey down received: {hotkey}");
                if (hotkey == "0" && IsAltLeftClickEnabled())
                {
                    HandleDisableClicksHotkeyDown0();
                    Eval(@"const event = new MouseEvent('pointerdown', {
                        clientX: " + mousePosX + @",
                        clientY: " + mousePosY + @",
                        button: 0,
                        buttons: 1
                    });
                    document.querySelector('div[name=""pnlBaseOverlay""]').dispatchEvent(event);
                    ");
                }
                else if (hotkey == "1" && IsAltRightClickEnabled())
                {
                    HandleDisableClicksHotkeyDown1();
                    Eval(@"const event = new MouseEvent('pointerdown', {
                        clientX: " + mousePosX + @",
                        clientY: " + mousePosY + @",
                        button: 2,
                        buttons: 2
                    });
                    document.querySelector('div[name=""pnlBaseOverlay""]').dispatchEvent(event);
                    ");
                }
            }

            if (ShouldHandleQuadrantHotkeys())
            {
                if (!chkDisableClicks.Checked)
                {
                    if (hotkey == "0")
                    {
                        c1 = 0; c2 = 0; c3 = 0;
                        if (buttonConfig[c1][2] == "false")
                        {
                            HotkeyClickLoop(c1, c2, c3, true);
                        }
                    }
                    else if (hotkey == "1")
                    {
                        c1 = 1; c2 = 1; c3 = 1;
                        if (diagUpRightHover)
                        {
                            c1 = 1; c2 = 0; c3 = 2;
                        }
                        if (buttonConfig[c1][2] == "false")
                        {
                            HotkeyClickLoop(c1, c2, c3, true);
                        }
                    }
                }
                if (hotkey == "2")
                {
                    c1 = 2; c2 = 2; c3 = 2;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, true);
                    }
                }
                else if (hotkey == "3")
                {
                    c1 = 3; c2 = 3; c3 = 3;
                    if (diagDownRightHover)
                    {
                        c1 = 3; c2 = 2; c3 = 4;
                    }
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, true);
                    }
                }
                else if (hotkey == "4")
                {
                    c1 = 4; c2 = 4; c3 = 4;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, true);
                    }
                }
                else if (hotkey == "5")
                {
                    c1 = 5; c2 = 5; c3 = 5;
                    if (diagDownLeftHover)
                    {
                        c1 = 5; c2 = 4; c3 = 6;
                    }
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, true);
                    }
                }
                else if (hotkey == "6")
                {
                    c1 = 6; c2 = 6; c3 = 6;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, true);
                    }
                }
                else if (hotkey == "7")
                {
                    c1 = 7; c2 = 7; c3 = 7;
                    if (diagUpLeftHover)
                    {
                        c1 = 7; c2 = 6; c3 = 0;
                    }
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, true);
                    }
                }
                else if (hotkey == "8")
                {
                    c1 = 8; c2 = 8; c3 = 8;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, true);
                    }
                }
                else if (hotkey == "9")
                {
                    c1 = 9; c2 = 9; c3 = 9;
                    if (diagUpRightLC)
                    {
                        c1 = 9; c2 = 8; c3 = 10;
                    }
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, true);
                    }
                }
                else if (hotkey == "10")
                {
                    c1 = 10; c2 = 10; c3 = 10;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, true);
                    }
                }
                else if (hotkey == "11")
                {
                    c1 = 11; c2 = 11; c3 = 11;
                    if (diagDownRightLC)
                    {
                        c1 = 11; c2 = 10; c3 = 12;
                    }
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, true);
                    }
                }
                else if (hotkey == "12")
                {
                    c1 = 12; c2 = 12; c3 = 12;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, true);
                    }
                }
                else if (hotkey == "13")
                {
                    c1 = 13; c2 = 13; c3 = 13;
                    if (diagDownLeftLC)
                    {
                        c1 = 13; c2 = 12; c3 = 14;
                    }
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, true);
                    }
                }
                else if (hotkey == "14")
                {
                    c1 = 14; c2 = 14; c3 = 14;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, true);
                    }
                }
                else if (hotkey == "15")
                {
                    c1 = 15; c2 = 15; c3 = 15;
                    if (diagUpLeftLC)
                    {
                        c1 = 15; c2 = 14; c3 = 8;
                    }
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, true);
                    }
                }
                else if (hotkey == "16")
                {
                    c1 = 16; c2 = 16; c3 = 16;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, true);
                    }
                }
                else if (hotkey == "17")
                {
                    c1 = 17; c2 = 17; c3 = 17;
                    if (diagUpRightRC)
                    {
                        c1 = 17; c2 = 16; c3 = 18;
                    }
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, true);
                    }
                }
                else if (hotkey == "18")
                {
                    c1 = 18; c2 = 18; c3 = 18;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, true);
                    }
                }
                else if (hotkey == "19")
                {
                    c1 = 19; c2 = 19; c3 = 19;
                    if (diagDownRightRC)
                    {
                        c1 = 19; c2 = 18; c3 = 20;
                    }
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, true);
                    }
                }
                else if (hotkey == "20")
                {
                    c1 = 20; c2 = 20; c3 = 20;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, true);
                    }
                }
                else if (hotkey == "21")
                {
                    c1 = 21; c2 = 21; c3 = 21;
                    if (diagDownLeftRC)
                    {
                        c1 = 21; c2 = 20; c3 = 22;
                    }
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, true);
                    }
                }
                else if (hotkey == "22")
                {
                    c1 = 22; c2 = 22; c3 = 22;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, true);
                    }
                }
                else if (hotkey == "23")
                {
                    c1 = 23; c2 = 23; c3 = 23;
                    if (diagUpLeftRC)
                    {
                        c1 = 23; c2 = 22; c3 = 16;
                    }
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, true);
                    }
                }
                else if (hotkey == "24")
                {
                    c1 = 24; c2 = 24; c3 = 24;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, true);
                    }
                }
                else if (hotkey == "27")
                {
                    c1 = 27; c2 = 27; c3 = 27;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, true);
                    }
                }
                else if (hotkey == "30")
                {
                    c1 = 30; c2 = 30; c3 = 30;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, true);
                    }
                }
                else if (hotkey == "25")
                {
                    c1 = 25; c2 = 25; c3 = 25;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, true);
                    }
                }
                else if (hotkey == "28")
                {
                    c1 = 28; c2 = 28; c3 = 28;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, true);
                    }
                }
                else if (hotkey == "31")
                {
                    c1 = 31; c2 = 31; c3 = 31;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, true);
                    }
                }
                else if (hotkey == "26")
                {
                    c1 = 26; c2 = 26; c3 = 26;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, true);
                    }
                }
                else if (hotkey == "29")
                {
                    c1 = 29; c2 = 29; c3 = 29;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, true);
                    }
                }
                else if (hotkey == "32")
                {
                    c1 = 32; c2 = 32; c3 = 32;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, true);
                    }
                }
            }
        }

        private void HandleHotkeyUp(string hotkey)
        {
            if (chkDisableClicks.Checked)
            {
                if (hotkey == "0" && IsAltLeftClickEnabled())
                {
                    HandleDisableClicksHotkeyUp0();
                    Eval(@"const event = new MouseEvent('pointerup', {
                        clientX: " + mousePosX + @",
                        clientY: " + mousePosY + @",
                        button: 0,
                        buttons: 0
                    });
                    document.querySelector('div[name=""pnlBaseOverlay""]').dispatchEvent(event);
                    ");
                }
                else if (hotkey == "1" && IsAltRightClickEnabled())
                {
                    HandleDisableClicksHotkeyUp1();
                    Eval(@"const event = new MouseEvent('pointerup', {
                        clientX: " + mousePosX + @",
                        clientY: " + mousePosY + @",
                        button: 2,
                        buttons: 0
                    });
                    document.querySelector('div[name=""pnlBaseOverlay""]').dispatchEvent(event);
                    ");
                }
            }

            if (ShouldHandleQuadrantHotkeys())
            {
                if (!chkDisableClicks.Checked)
                {
                    if (hotkey == "0")
                    {
                        c1 = 0; c2 = 0; c3 = 0;
                        if (buttonConfig[c1][2] == "false")
                        {
                            HotkeyClickLoop(c1, c2, c3, false);
                        }
                    }
                    else if (hotkey == "1")
                    {
                        c1 = 1; c2 = 1; c3 = 1;
                        if (diagUpRightHover)
                        {
                            c1 = 1; c2 = 0; c3 = 2;
                        }
                        if (buttonConfig[c1][2] == "false")
                        {
                            HotkeyClickLoop(c1, c2, c3, false);
                        }
                    }
                }

                if (hotkey == "2")
                {
                    c1 = 2; c2 = 2; c3 = 2;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, false);
                    }
                }
                else if (hotkey == "3")
                {
                    c1 = 3; c2 = 3; c3 = 3;
                    if (diagDownRightHover)
                    {
                        c1 = 3; c2 = 2; c3 = 4;
                    }
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, false);
                    }
                }
                else if (hotkey == "4")
                {
                    c1 = 4; c2 = 4; c3 = 4;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, false);
                    }
                }
                else if (hotkey == "5")
                {
                    c1 = 5; c2 = 5; c3 = 5;
                    if (diagDownLeftHover)
                    {
                        c1 = 5; c2 = 4; c3 = 6;
                    }
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, false);
                    }
                }
                else if (hotkey == "6")
                {
                    c1 = 6; c2 = 6; c3 = 6;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, false);
                    }
                }
                else if (hotkey == "7")
                {
                    c1 = 7; c2 = 7; c3 = 7;
                    if (diagUpLeftHover)
                    {
                        c1 = 7; c2 = 6; c3 = 0;
                    }
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, false);
                    }
                }
                else if (hotkey == "8")
                {
                    c1 = 8; c2 = 8; c3 = 8;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, false);
                    }
                }
                else if (hotkey == "9")
                {
                    c1 = 9; c2 = 9; c3 = 9;
                    if (diagUpRightLC)
                    {
                        c1 = 9; c2 = 8; c3 = 10;
                    }
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, false);
                    }
                }
                else if (hotkey == "10")
                {
                    c1 = 10; c2 = 10; c3 = 10;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, false);
                    }
                }
                else if (hotkey == "11")
                {
                    c1 = 11; c2 = 11; c3 = 11;
                    if (diagDownRightLC)
                    {
                        c1 = 11; c2 = 10; c3 = 12;
                    }
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, false);
                    }
                }
                else if (hotkey == "12")
                {
                    c1 = 12; c2 = 12; c3 = 12;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, false);
                    }
                }
                else if (hotkey == "13")
                {
                    c1 = 13; c2 = 13; c3 = 13;
                    if (diagDownLeftLC)
                    {
                        c1 = 13; c2 = 12; c3 = 14;
                    }
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, false);
                    }
                }
                else if (hotkey == "14")
                {
                    c1 = 14; c2 = 14; c3 = 14;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, false);
                    }
                }
                else if (hotkey == "15")
                {
                    c1 = 15; c2 = 15; c3 = 15;
                    if (diagUpLeftLC)
                    {
                        c1 = 15; c2 = 14; c3 = 8;
                    }
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, false);
                    }
                }
                else if (hotkey == "16")
                {
                    c1 = 16; c2 = 16; c3 = 16;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, false);
                    }
                }
                else if (hotkey == "17")
                {
                    c1 = 17; c2 = 17; c3 = 17;
                    if (diagUpRightRC)
                    {
                        c1 = 17; c2 = 16; c3 = 18;
                    }
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, false);
                    }
                }
                else if (hotkey == "18")
                {
                    c1 = 18; c2 = 18; c3 = 18;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, false);
                    }
                }
                else if (hotkey == "19")
                {
                    c1 = 19; c2 = 19; c3 = 19;
                    if (diagDownRightRC)
                    {
                        c1 = 19; c2 = 18; c3 = 20;
                    }
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, false);
                    }
                }
                else if (hotkey == "20")
                {
                    c1 = 20; c2 = 20; c3 = 20;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, false);
                    }
                }
                else if (hotkey == "21")
                {
                    c1 = 21; c2 = 21; c3 = 21;
                    if (diagDownLeftRC)
                    {
                        c1 = 21; c2 = 20; c3 = 22;
                    }
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, false);
                    }
                }
                else if (hotkey == "22")
                {
                    c1 = 22; c2 = 22; c3 = 22;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, false);
                    }
                }
                else if (hotkey == "23")
                {
                    c1 = 23; c2 = 23; c3 = 23;
                    if (diagUpLeftRC)
                    {
                        c1 = 23; c2 = 22; c3 = 16;
                    }
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, false);
                    }
                }
                else if (hotkey == "24")
                {
                    c1 = 24; c2 = 24; c3 = 24;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, false);
                    }
                }
                else if (hotkey == "27")
                {
                    c1 = 27; c2 = 27; c3 = 27;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, false);
                    }
                }
                else if (hotkey == "30")
                {
                    c1 = 30; c2 = 30; c3 = 30;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, false);
                    }
                }
                else if (hotkey == "25")
                {
                    c1 = 25; c2 = 25; c3 = 25;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, false);
                    }
                }
                else if (hotkey == "28")
                {
                    c1 = 28; c2 = 28; c3 = 28;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, false);
                    }
                }
                else if (hotkey == "31")
                {
                    c1 = 31; c2 = 31; c3 = 31;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, false);
                    }
                }
                else if (hotkey == "26")
                {
                    c1 = 26; c2 = 26; c3 = 26;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, false);
                    }
                }
                else if (hotkey == "29")
                {
                    c1 = 29; c2 = 29; c3 = 29;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, false);
                    }
                }
                else if (hotkey == "32")
                {
                    c1 = 32; c2 = 32; c3 = 32;
                    if (buttonConfig[c1][2] == "false")
                    {
                        HotkeyClickLoop(c1, c2, c3, false);
                    }
                }
            }
        }

        private void UpdateHotkeyPointerEndPosition()
        {
            if (positionCheck)
            {
                xEnd = (int)remoteX;
                yEnd = (int)remoteY;
            }
            else if (chkDisableClicks.Checked && IsMoveMouseUsingLeftClicksEnabled())
            {
                xEnd = mousePosX;
                yEnd = mousePosY;
            }
            else
            {
                xEnd = (int)(Cursor.Position.X / zoomFactor);
                yEnd = (int)(Cursor.Position.Y / zoomFactor);
            }
        }

        private void HandleDisableClicksHotkeyDown0()
        {
            HandleDisableClicksHotkeyMouseButton("left", true);
        }

        private void HandleDisableClicksHotkeyDown1()
        {
            HandleDisableClicksHotkeyMouseButton("right", true);
        }

        private void HandleDisableClicksHotkeyUp0()
        {
            HandleDisableClicksHotkeyMouseButton("left", false);
        }

        private void HandleDisableClicksHotkeyUp1()
        {
            HandleDisableClicksHotkeyMouseButton("right", false);
        }

        private void HandleDisableClicksHotkeyMouseButton(string mouseButton, bool isDown)
        {
            if (!positionCheck && IsLblActiveInactiveState())
            {
                if (isDown)
                {
                    if (!IsMoveMouseUsingLeftClicksEnabled())
                    {
                        SetPointerGlow(mouseButton, true);
                    }
                    StartLblActiveHoldAnimation();
                }
                else
                {
                    mouseUp(mouseButton);
                    CancelLblActiveHoldAnimation();
                }

                return;
            }

            UpdateHotkeyPointerEndPosition();
            bool isLeft = mouseButton == "left";
            bool shouldTrackDeactivateHold =
                !positionCheck
                && isActive
                && IsMoveMouseUsingLeftClicksEnabled()
                && IsPointInConfiguredDeactivateDeadZone(mousePosX, mousePosY);

            if (isDown)
            {
                if (isLeft)
                {
                    syntheticLeftMouseButtonHeld = true;
                    lastLeftMouseDownAt = DateTime.UtcNow;
                }
                else
                {
                    syntheticRightMouseButtonHeld = true;
                    lastRightMouseDownAt = DateTime.UtcNow;
                }

                SetPointerGlow(mouseButton, true);

                if (shouldTrackDeactivateHold)
                {
                    StartLblActiveHoldAnimation();
                    StartAltMouseDeactivateHold();
                }

                if (TryHandleRemoteMouseButtonAction(mouseButton, true))
                {
                    return;
                }

                mouseDown(mouseButton);

                return;
            }

            if (isLeft)
            {
                if (!syntheticLeftMouseButtonHeld)
                {
                    return;
                }

                syntheticLeftMouseButtonHeld = false;
            }
            else
            {
                if (!syntheticRightMouseButtonHeld)
                {
                    return;
                }

                syntheticRightMouseButtonHeld = false;
            }

            SetPointerGlow(mouseButton, false);

            if (!positionCheck && IsMoveMouseUsingLeftClicksEnabled())
            {
                CancelAltMouseDeactivateHold();
            }

            if (TryHandleRemoteMouseButtonAction(mouseButton, false))
            {
                return;
            }

            mouseUp(mouseButton);
        }

        private void chkCombineControllers_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCombineControllers.Checked)
            {
                chkCombineControllers.Checked = true;

                resetTransparency(false, true);

                ExtendedDialogResult result = null;

                if (xboxMode)
                {
                    result = ExtendedMessageBox.Show("<p style='margin:0;padding:0;line-height:1.6;'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 3; -webkit-box-orient: vertical; overflow: hidden; font-size:" + truncateSize + "pt; ' title='Would you like to press certain gamepad buttons on your non-Overjoyed controller and combine them with your Overjoyed setup to form one Xbox gamepad?'>Would you like to press certain gamepad buttons on your non-Overjoyed controller and combine them with your Overjoyed setup to form one Xbox gamepad?</span></p>", "Combine Into Xbox Gamepad?", ExtendedMessageBoxLibrary.MessageBoxButtons.YesNo, lines: 3, zoomFactor: zoomFactor);
                }
                                else if (switchMode)
                {
                    result = ExtendedMessageBox.Show("<p style='margin:0;padding:0;line-height:1.6;'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 3; -webkit-box-orient: vertical; overflow: hidden; font-size:" + truncateSize + "pt; ' title='Would you like to press certain gamepad buttons on your non-Overjoyed controller and combine them with your Overjoyed setup to form one Nintendo Switch gamepad?'>Would you like to press certain gamepad buttons on your non-Overjoyed controller and combine them with your Overjoyed setup to form one Nintendo Switch gamepad?</span></p>", "Combine Into Nintendo Switch Gamepad?", ExtendedMessageBoxLibrary.MessageBoxButtons.YesNo, lines: 3, zoomFactor: zoomFactor);
                }
                resetTransparency(true, true);

                if (result != null && result.Result == DialogResult.Yes)
                {
                    lblQuickControls.Visible = false;

                    lblUseOverjoyed.Visible = true;
                    chkCombineLS.Visible = true;
                    chkCombineRS.Visible = true;
                    chkCombineLT.Visible = true;
                    chkCombineRT.Visible = true;

                    if (xboxMode)
                    {
                    

                        chkSteam.Text = "Watch Steam Tutorial";
                        chkSteam.Visible = true;
                        resetTransparency(false, true);

                        ExtendedDialogResult tutorialResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0;line-height:1.6;'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; font-size:" + truncateSize + "pt; ' title='Would you like to watch a tutorial on how to set up Steam to see your combined Overjoyed gamepad as 1 controller?'>Would you like to watch a tutorial on how to set up Steam to see your combined Overjoyed gamepad as 1 controller?</span></p>", "Watch Steam Tutorial?", ExtendedMessageBoxLibrary.MessageBoxButtons.YesNo, lines: 2, zoomFactor: zoomFactor);
                        resetTransparency(true, true);

                        if (tutorialResult.Result == DialogResult.Yes)
                        {
                            ProcessStartInfo psi = new ProcessStartInfo
                            {
                                FileName = "https://www.youtube.com/watch?v=bAVdCvSKFRs",
                                UseShellExecute = true
                            };
                            Process.Start(psi);
                        }

                        Task.Run(() => StartGamepadDetection());
                    }
                    else if (switchMode)
                    {
                        Task.Run(() => StartGamepadDetection());
                    }
                }
                else if (result == null || result.Result == DialogResult.No)
                {
                    resetTransparency(false, true);
                    ExtendedDialogResult keyboardResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0;line-height:1.6;'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; font-size:" + truncateSize + "pt; ' title='Would you like to have your other system press certain keyboard keys to trigger quadrant actions in Overjoyed?'>Would you like to have your other system press certain keyboard keys to trigger quadrant actions in Overjoyed?</span></p>", "Trigger Actions with Keyboard Keys", ExtendedMessageBoxLibrary.MessageBoxButtons.YesNo, lines: 2, zoomFactor: zoomFactor);
                    resetTransparency(true, true);

                    if (keyboardResult.Result == DialogResult.Yes)
                    {
                        keyboardCombine = true;
                        chkSteam.Text = "View Key Bindings";
                        chkSteam.Visible = true;
                        resetTransparency(false, true);

                        ExtendedDialogResult keysResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0;line-height:1.6;'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 1; -webkit-box-orient: vertical; overflow: hidden; font-size:" + truncateSize + "pt; ' title='Would you like to view a diagram of key bindings?'>Would you like to view a diagram of key bindings?</span></p>", "View Key Binding Diagram?", ExtendedMessageBoxLibrary.MessageBoxButtons.YesNo, lines: 1, zoomFactor: zoomFactor);
                        resetTransparency(true, true);

                        if (keysResult.Result == DialogResult.Yes)
                        {
                            ShowCombineDiagramOverlay();
                            closeDiagram.Visible = true;
                            closeDiagram.BringToFront();

                            pnlExtraButtons.Visible = false;
                            SetLblActiveVisibleUi(false);
                            hideBG.Visible = false;
                            chkSteam.Visible = false;
                            Eval(@"setTimeout(() => {
        document.querySelectorAll('svg text[data-id]').forEach(svgText => {
            const id = svgText.getAttribute('data-id');
            const htmlText = document.querySelector('#translations [data-id=""'+id+'""]');
            if(htmlText) {
                svgText.childNodes.forEach(node => {
                    if(node.nodeType === Node.TEXT_NODE) node.remove();
                });
                const firstTspan = svgText.querySelector('tspan');
                if(firstTspan) {
                    svgText.insertBefore(document.createTextNode(htmlText.textContent), firstTspan);
                } else {
                    svgText.textContent = htmlText.textContent;
                }

                const parentGroup = svgText.parentNode;
                if(parentGroup && parentGroup.querySelector('rect')) {
                    const bbox = svgText.getBBox();
                    const paddingX = 10;
                    const paddingY = 6;
                    const rect = parentGroup.querySelector('rect');
                    rect.setAttribute('width', bbox.width + paddingX*2);
                    rect.setAttribute('height', bbox.height + paddingY*2);
                    rect.setAttribute('x', bbox.x - paddingX);
                    rect.setAttribute('y', bbox.y - paddingY);
                }
            }
        });
    }, 1000);
");
                        }
                        else
                        {
                            chkSteam.BringToFront();
                        }
                    }
                    else if (keyboardResult.Result == DialogResult.No)
                    {
                        keyboardCombine = false;
                        chkCombineControllers.Checked = false;
                        return;
                    }
                }
            }
            else
            {
                chkCombineControllers.Checked = false; lblQuickControls.Visible = true;

                chkSteam.Visible = false;
                lblUseOverjoyed.Visible = false;
                chkCombineLS.Visible = false;
                chkCombineRS.Visible = false;
                chkCombineLT.Visible = false;
                chkCombineRT.Visible = false;

                bool hadGamepadDetection = scanning || scanned;

                if (!keyboardCombine)
                {
                    scanning = false;
                    lblCombine.Visible = false;

                    if (hadGamepadDetection)
                    {
                        resetTransparency(false, true);

                        ExtendedMessageBox.Show("<p style='margin:0;padding:0;line-height:1.6;'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 1; -webkit-box-orient: vertical; overflow: hidden; font-size:" + truncateSize + "pt; ' title='Gamepad Detection Canceled'>Gamepad Detection Canceled</span></p>", "Detection", zoomFactor: zoomFactor);
                        resetTransparency(true, true);
                    }
                }
                keyboardCombine = false;
            }

            try
            {
                using (FileStream fs = new FileStream(Path.Combine(configPath, "AdvancedModes.txt"), FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.WriteLine(chkFPS.Checked);
                    if (keyboardCombine)
                    {
                        writer.WriteLine("keys");
                    }
                    else
                    {
                        writer.WriteLine(chkCombineControllers.Checked);
                    }
                    writer.WriteLine(chkMouseLock.Checked);
                    writer.WriteLine(chkDisableClicks.Checked);
                    writer.WriteLine(chkAltRTC.Checked);
                    writer.WriteLine(chkMarioKart.Checked);
                    writer.WriteLine(chkVoice.Checked);
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
        }

        private void chkDisableClicks_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDisableClicks.Checked)
            {
                keyboardCombine = true;
            }
            else
            {
                keyboardCombine = false;
            }

            UpdateAlternativeMouseControlsOptionsVisibility();
            drawPrompts();
            RefreshOverlayPointerState();

            if (chkDisableClicks.Checked)
            {

                resetTransparency(false, true);
                ExtendedMessageBox.Show(
                    $"<p style='margin:0;padding:0;line-height:1.6;'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 6; -webkit-box-orient: vertical; overflow: hidden; font-size:{truncateSize}pt; ' title='Alternative Mouse Controls will disable left and right mouse clicks from triggering Left or Right Click Actions at the location of your cursor. You must configure your setup to use the Minus (-) key and Plus (+) key as replacements for left and right clicks. There are more advanced settings for mouse controls below the top menu bar'>Alternative Mouse Controls will <b>disable</b> left and right mouse clicks from triggering Left or Right Click Actions at the location of your cursor. You must configure your setup to use the <b>Minus (-)</b> key and <b>Plus (+)</b> key as replacements for left and right clicks. There are more advanced settings for mouse controls below the top menu bar.</span></p>",
                    "Alternative Mouse Controls Overview",
                    ExtendedMessageBoxLibrary.MessageBoxButtons.OK,
                    lines: 6,
                    zoomFactor: zoomFactor);
                resetTransparency(true, true);
            }
        }
    }
}