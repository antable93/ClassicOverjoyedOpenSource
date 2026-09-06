using ExtendedMessageBoxLibrary;
using GregsStack.InputSimulatorStandard.Native;
using Microsoft.Maui.ApplicationModel;
using SharedVars;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Wisej.Web;


namespace OverjoyedReleaseLocal
{
    public partial class Active : Page
    {
        private const string ArrowsMoreTapMode = "More...";
        private const string ArrowsLStickMode = "LStick";
        private const string ArrowsRStickMode = "RStick";
        private const string ArrowsDPadMode = "DPad";
        private const string ArrowsTapX = "X (Tap)";
        private const string ArrowsTapY = "Y (Tap)";
        private const string ArrowsTapL = "L (Tap)";
        private const string ArrowsTapR = "R (Tap)";
        private const string ArrowsTapL3 = "L3 (Tap)";
        private const string ArrowsTapR3 = "R3 (Tap)";
        private const string ArrowsTapLSB = "LSB (Tap)";
        private const string ArrowsTapRSB = "RSB (Tap)";
        private const string ArrowsTapCapture = "Capture (Tap)";
        private const string ArrowsTapPsTriangle = "ꕔ (Tap)";
        private const string ArrowsTapPsSquare = "⬜ (Tap)";

        private bool suppressArrowsGamepadSelectionChanged;
        private string activeArrowsDirectionalMode = ArrowsLStickMode;

        private void InitializeArrowsGamepadOptions()
        {
            suppressArrowsGamepadSelectionChanged = true;
            cmbArrowsGamepad.Items.Clear();

            cmbArrowsGamepad.Items.Add(ArrowsMoreTapMode);

            if (activeArrowsDirectionalMode != ArrowsLStickMode)
            {
                cmbArrowsGamepad.Items.Add(ArrowsLStickMode);
            }

            if (activeArrowsDirectionalMode != ArrowsRStickMode)
            {
                cmbArrowsGamepad.Items.Add(ArrowsRStickMode);
            }

            if (activeArrowsDirectionalMode != ArrowsDPadMode)
            {
                cmbArrowsGamepad.Items.Add(ArrowsDPadMode);
            }

            if (switchMode)
            {
                cmbArrowsGamepad.Items.Add(ArrowsTapX);
                cmbArrowsGamepad.Items.Add(ArrowsTapY);
                cmbArrowsGamepad.Items.Add(ArrowsTapL);
                cmbArrowsGamepad.Items.Add(ArrowsTapR);
                cmbArrowsGamepad.Items.Add(ArrowsTapL3);
                cmbArrowsGamepad.Items.Add(ArrowsTapR3);
                cmbArrowsGamepad.Items.Add(ArrowsTapCapture);
            }
            else if (xboxMode)
            {
                if (psLabels)
                {
                    cmbArrowsGamepad.Items.Add(ArrowsTapPsTriangle);
                    cmbArrowsGamepad.Items.Add(ArrowsTapPsSquare);
                    cmbArrowsGamepad.Items.Add(ArrowsTapL3);
                    cmbArrowsGamepad.Items.Add(ArrowsTapR3);
                }
                else
                {
                    cmbArrowsGamepad.Items.Add(ArrowsTapX);
                    cmbArrowsGamepad.Items.Add(ArrowsTapY);
                    cmbArrowsGamepad.Items.Add(ArrowsTapLSB);
                    cmbArrowsGamepad.Items.Add(ArrowsTapRSB);
                }
            }

            cmbArrowsGamepad.SelectedItem = ArrowsMoreTapMode;
            suppressArrowsGamepadSelectionChanged = false;
            ApplyArrowsGamepadButtonLabels();
        }

        private void SetArrowsGamepadMode(string mode)
        {
            if (IsArrowsDirectionalMode(mode))
            {
                activeArrowsDirectionalMode = mode;
                InitializeArrowsGamepadOptions();
                return;
            }

            suppressArrowsGamepadSelectionChanged = true;
            cmbArrowsGamepad.SelectedItem = ArrowsMoreTapMode;
            suppressArrowsGamepadSelectionChanged = false;
            ApplyArrowsGamepadButtonLabels();
        }

        private bool IsArrowsDirectionalMode(string mode)
        {
            return mode == ArrowsDPadMode || mode == ArrowsLStickMode || mode == ArrowsRStickMode;
        }

        private void ApplyArrowsGamepadButtonLabels()
        {
            string mode = activeArrowsDirectionalMode;
            if (mode == ArrowsDPadMode)
            {
                btnUp.Text = "D🠅";
                btnDown.Text = "D🠇";
                btnLeft.Text = "D\n🠄";
                btnRight.Text = "D\n🠆";
                return;
            }

            if (mode == ArrowsLStickMode)
            {
                btnUp.Text = "🠅";
                btnDown.Text = "🠇";
                btnLeft.Text = "🠄";
                btnRight.Text = "🠆";
                return;
            }

            if (mode == ArrowsRStickMode)
            {
                btnUp.Text = "R🠅";
                btnDown.Text = "R🠇";
                btnLeft.Text = "R\n🠄";
                btnRight.Text = "R\n🠆";
                return;
            }

            btnUp.Text = "🠅";
            btnDown.Text = "🠇";
            btnLeft.Text = "🠄";
            btnRight.Text = "🠆";
        }

        private void ApplyArrowsKeyboardButtonLabels()
        {
            if (cmbArrowsKeyboard.Text == "WASD")
            {
                btnUp.Text = "W";
                btnDown.Text = "S";
                btnLeft.Text = "A";
                btnRight.Text = "D";
                return;
            }

            btnUp.Text = "🠅";
            btnDown.Text = "🠇";
            btnLeft.Text = "🠄";
            btnRight.Text = "🠆";
        }

        private void cmbArrowsKeyboard_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!keyboardMode)
            {
                return;
            }

            ApplyArrowsKeyboardButtonLabels();
        }

        private bool TryTriggerArrowsTapAction(string mode)
        {
            if (mode == ArrowsTapX)
            {
                if (switchMode)
                {
                    Task.Run(() => SwitchXThread());
                }
                else if (xboxMode)
                {
                    SimGamePad.Instance.Use(14, toggleXbox, 1, 100);
                }

                return true;
            }

            if (mode == ArrowsTapY)
            {
                if (switchMode)
                {
                    Task.Run(() => SwitchYThread());
                }
                else if (xboxMode)
                {
                    SimGamePad.Instance.Use(15, toggleXbox, 1, 100);
                }

                return true;
            }

            if (mode == ArrowsTapPsTriangle)
            {
                if (xboxMode)
                {
                    SimGamePad.Instance.Use(15, toggleXbox, 1, 100);
                }

                return true;
            }

            if (mode == ArrowsTapPsSquare)
            {
                if (xboxMode)
                {
                    SimGamePad.Instance.Use(14, toggleXbox, 1, 100);
                }

                return true;
            }

            if (mode == ArrowsTapL)
            {
                if (switchMode)
                {
                    Task.Run(() => SwitchLThread());
                }

                return true;
            }

            if (mode == ArrowsTapR)
            {
                if (switchMode)
                {
                    Task.Run(() => SwitchRThread());
                }

                return true;
            }

            if (mode == ArrowsTapL3)
            {
                if (switchMode)
                {
                    Task.Run(() => SwitchL3Thread());
                }
                else if (xboxMode)
                {
                    SimGamePad.Instance.Use(6, toggleXbox, 1, 100);
                }

                return true;
            }

            if (mode == ArrowsTapLSB)
            {
                if (xboxMode)
                {
                    SimGamePad.Instance.Use(6, toggleXbox, 1, 100);
                }

                return true;
            }

            if (mode == ArrowsTapR3)
            {
                if (switchMode)
                {
                    Task.Run(() => SwitchR3Thread());
                }
                else if (xboxMode)
                {
                    SimGamePad.Instance.Use(7, toggleXbox, 1, 100);
                }

                return true;
            }

            if (mode == ArrowsTapRSB)
            {
                if (xboxMode)
                {
                    SimGamePad.Instance.Use(7, toggleXbox, 1, 100);
                }

                return true;
            }

            if (mode == ArrowsTapCapture)
            {
                if (switchMode)
                {
                    Task.Run(() => SwitchCaptureThread());
                }

                return true;
            }

            return false;
        }

        private void btnSettings_MouseEnter(object sender, EventArgs e)
        {
            chkActiveHover.Checked = true;

            var ctrl = sender as Control;

            ctrl.BackColor = Color.FromArgb(255, 0, 0, 0);
            ctrl.ForeColor = Color.FromArgb(255, 255, 255, 255);




        }


        async Task SwitchWakeupThread()
        {

            switchGamepad.Button_Wakeup_Down();
            await Task.Delay(100);
            switchGamepad.Button_Wakeup_Up();

            btnSwitchWakeup.Enabled = false;
            btnSwitchWakeup.Enabled = false;
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            DeleteWebfusePanel();

            if (xboxMode)
            {

                SimGamePad.Instance.Unplug();
                SimGamePad.Instance.ShutDown();
            }
            OBSWebSocket.DetachForm(this);
            BasicRemoteClient.DetachForm(this);

            positionCheck = false;
            remotePositionCheck = false;

            StopListening();

            if (!keyboardCombine)
            {
                scanning = false;
                lblCombine.Visible = false;

            }
            keyboardCombine = false;


            try
            {
                // Open the file with shared read/write access
                using (FileStream fs = new FileStream(Path.Combine(configPath, "AdvancedModes.txt"), FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    // Click Here to Turn On / Off Advanced Modes
                    // (Click to Turn On) Twin Stick Mode
                    // (Click to Turn On) Combine Inputs from Other Systems
                    // (Click to Turn On) Prevent Games from Locking Mouse
                    // (Click to Turn On) Alternative Mouse Controls
                    // (Click to Turn On) Alternative Return To Center
                    // (Click to Turn On) Mario Kart Mode

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
            timer.Stop();
            timer.Tick -= new EventHandler(TimerTick);

            Application.MainPage = new Config();



        }

        private async void btnConfigSizeDown_Click(object sender, EventArgs e)
        {
            await MinimizeMessenger.SendMessageAsync("active-size-down|" + playerNum);
        }

        private async void btnConfigSizeUp_Click(object sender, EventArgs e)
        {
            await MinimizeMessenger.SendMessageAsync("active-size-up|" + playerNum);
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            string arrowsMode = activeArrowsDirectionalMode;

            if (xboxMode)
            {
                if (arrowsMode == ArrowsDPadMode)
                {
                    SimGamePad.Instance.Use(3, toggleXbox, 1, 100);
                }
                else if (arrowsMode == ArrowsRStickMode)
                {
                    SimGamePad.Instance.Use(23, toggleXbox, 1, 100);
                }
                else if (arrowsMode == ArrowsLStickMode)
                {
                    SimGamePad.Instance.Use(19, toggleXbox, 1, 100);
                }
            }

            if (switchMode)
            {
                if (arrowsMode == ArrowsDPadMode)
                {
                    // Start the method in a separate thread
                    Task.Run(() => SwitchDpadRightThread());
                }
                else if (arrowsMode == ArrowsRStickMode)
                {
                    // Start the method in a separate thread
                    Task.Run(() => SwitchRStickRightThread());
                }
                else if (arrowsMode == ArrowsLStickMode)
                {
                    // Start the method in a separate thread
                    Task.Run(() => SwitchRightThread());
                }

            }

            if (keyboardMode)
            {
                if (cmbArrowsKeyboard.Text == "WASD")
                {
                    inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VK_D);
                }
                else
                {
                    inputSimulator.Keyboard.KeyPress(VirtualKeyCode.RIGHT);
                }
            }


        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            string arrowsMode = activeArrowsDirectionalMode;
            if (xboxMode)
            {
                if (arrowsMode == ArrowsDPadMode)
                {
                    SimGamePad.Instance.Use(1, toggleXbox, 1, 100);
                    Debug.WriteLine("Using DPad for Down");
                }
                else if (arrowsMode == ArrowsRStickMode)
                {
                    SimGamePad.Instance.Use(25, toggleXbox, 1, 100);
                    Debug.WriteLine("Using RStick for Down");
                }
                else if (arrowsMode == ArrowsLStickMode)
                {
                    SimGamePad.Instance.Use(21, toggleXbox, 1, 100);
                    Debug.WriteLine("Using Face Buttons for Down");
                }
            }

            if (switchMode)
            {
                if (arrowsMode == ArrowsDPadMode)
                {
                    // Start the method in a separate thread
                    Task.Run(() => SwitchDpadDownThread());
                }
                else if (arrowsMode == ArrowsRStickMode)
                {
                    // Start the method in a separate thread
                    Task.Run(() => SwitchRStickDownThread());
                }
                else if (arrowsMode == ArrowsLStickMode)
                {
                    // Start the method in a separate thread
                    Task.Run(() => SwitchDownThread());
                }

            }

            if (keyboardMode)
            {
                if (cmbArrowsKeyboard.Text == "WASD")
                {
                    inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VK_S);
                }
                else
                {
                    inputSimulator.Keyboard.KeyPress(VirtualKeyCode.DOWN);
                }
            }

        }

        private void cmbArrowsGamepad_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (suppressArrowsGamepadSelectionChanged)
            {
                return;
            }

            string selectedMode = cmbArrowsGamepad.Text;

            if (IsArrowsDirectionalMode(selectedMode))
            {
                SetArrowsGamepadMode(selectedMode);
            }

            if (TryTriggerArrowsTapAction(selectedMode))
            {
                SetArrowsGamepadMode(ArrowsMoreTapMode);
                return;
            }

            ApplyArrowsGamepadButtonLabels();

            var streamerModePath = Path.Combine(configPath, "StreamerMode.txt");
            string? firstLine = null;

            if (System.IO.File.Exists(streamerModePath))
            {
                using var fs = new FileStream(streamerModePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = new StreamReader(fs);
                firstLine = reader.ReadLine();
            }

            if ((firstLine?.IndexOf("true", StringComparison.OrdinalIgnoreCase) ?? -1) >= 0 && !keyboardMode)
            {
            if (activeArrowsDirectionalMode == ArrowsDPadMode)
                {
                    btnSwitchHome.Text = "Webfuse OFF";
                }
                if (activeArrowsDirectionalMode != ArrowsDPadMode)
                {
                    if(xboxMode)
                    {
                        btnSwitchHome.Text = "Xbox Button";
                    }
                    else if (switchMode)
                    {
                        btnSwitchHome.Text = "Switch Home";
                    }
                }

            }

            
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            string arrowsMode = activeArrowsDirectionalMode;

            if (xboxMode)
            {
                if (arrowsMode == ArrowsDPadMode)
                {
                    SimGamePad.Instance.Use(0, toggleXbox, 1, 100);
                }
                else if (arrowsMode == ArrowsRStickMode)
                {
                    SimGamePad.Instance.Use(24, toggleXbox, 1, 100);
                }
                else if (arrowsMode == ArrowsLStickMode)
                {
                    SimGamePad.Instance.Use(20, toggleXbox, 1, 100);
                }
            }

            if (switchMode)
            {
                if (arrowsMode == ArrowsDPadMode)
                {
                    // Start the method in a separate thread
                    Task.Run(() => SwitchDpadUpThread());
                }
                else if (arrowsMode == ArrowsRStickMode)
                {
                    // Start the method in a separate thread
                    Task.Run(() => SwitchRStickUpThread());
                }
                else if (arrowsMode == ArrowsLStickMode)
                {
                    // Start the method in a separate thread
                    Task.Run(() => SwitchUpThread());
                }

            }

            if (keyboardMode)
            {
                if (cmbArrowsKeyboard.Text == "WASD")
                {
                    inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VK_W);
                }

                else
                {
                    inputSimulator.Keyboard.KeyPress(VirtualKeyCode.UP);
                }
            }

        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            string arrowsMode = activeArrowsDirectionalMode;


            if (xboxMode)
            {
                if (arrowsMode == ArrowsDPadMode)
                {
                    SimGamePad.Instance.Use(2, toggleXbox, 1, 100);
                }
                else if (arrowsMode == ArrowsRStickMode)
                {
                    SimGamePad.Instance.Use(22, toggleXbox, 1, 100);
                }
                else if (arrowsMode == ArrowsLStickMode)
                {
                    SimGamePad.Instance.Use(18, toggleXbox, 1, 100);
                }
            }

            if (switchMode)
            {
                if (arrowsMode == ArrowsDPadMode)
                {
                    // Start the method in a separate thread
                    Task.Run(() => SwitchDpadLeftThread());
                }
                else if (arrowsMode == ArrowsRStickMode)
                {
                    // Start the method in a separate thread
                    Task.Run(() => SwitchRStickLeftThread());
                }
                else if (arrowsMode == ArrowsLStickMode)
                {
                    // Start the method in a separate thread
                    Task.Run(() => SwitchLeftThread());
                }

            }

            if (keyboardMode)
            {
                if (cmbArrowsKeyboard.Text == "WASD")
                {
                    inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VK_A);
                }
                else
                {
                    inputSimulator.Keyboard.KeyPress(VirtualKeyCode.LEFT);
                }
            }

        }

        private void btnZR_Click(object sender, EventArgs e)
        {
            if (xboxMode)
            {

                SimGamePad.Instance.Use(9, toggleXbox, 1, 100);
            }

            if (switchMode)
            {
                // Start the method in a separate thread
                Task.Run(() => SwitchZRThread());

            }
        }

        private void btnZL_Click(object sender, EventArgs e)
        {
            if (xboxMode)
            {
                SimGamePad.Instance.Use(8, toggleXbox, 1, 100);
            }
            if (switchMode)
            {
                // Start the method in a separate thread
                Task.Run(() => SwitchZLThread());

            }
        }

        async Task SwitchAcceptThread()
        {
            
            switchGamepad.ButtonA_Down();
            await Task.Delay(100);
            switchGamepad.ButtonA_Up();

        }


        async Task SwitchBackThread()
        {

            switchGamepad.ButtonB_Down();
            await Task.Delay(100);
            switchGamepad.ButtonB_Up();

        }


        async Task SwitchStartScreenThread()
        {

            switchGamepad.ButtonL_Down();
            switchGamepad.ButtonR_Down();
            await Task.Delay(100);
            switchGamepad.ButtonL_Up();
            switchGamepad.ButtonR_Up();
        }


        async Task SwitchSelectThread()
        {

            switchGamepad.ButtonMinus_Down();
            await Task.Delay(100);
            switchGamepad.ButtonMinus_Up();

        }


        async Task SwitchHomeThread()
        {

            switchGamepad.ButtonHome_Down();
            await Task.Delay(100);
            switchGamepad.ButtonHome_Up();

        }

        async Task SwitchStartThread()
        {

            switchGamepad.ButtonPlus_Down();
            await Task.Delay(100);
            switchGamepad.ButtonPlus_Up();

        }

        private void btnSettings_MouseLeave(object sender, EventArgs e)
        {
            chkActiveHover.Checked = false;
            var ctrl = sender as Control;

            if (ctrl.Name != "btnSettings")
            {
                ctrl.ForeColor = Color.FromArgb(255, 0, 0, 0);
                ctrl.BackColor = Color.FromArgb(255, 255, 255, 255);

            }
            else
            {

                ctrl.BackColor = Color.FromArgb(255, 0, 167, 209);
            }


        }

        async Task SwitchDpadRightThread()
        {
            switchGamepad.ButtonDpadRight_Down();
            await Task.Delay(100);
            switchGamepad.ButtonDpadRight_Up();
        }

        async Task SwitchRightThread()
        {
            switchGamepad.leftStick_MoveRight(100);
            await Task.Delay(100);
            switchGamepad.leftStick_Stop();
        }

        async Task SwitchDpadDownThread()
        {
            switchGamepad.ButtonDpadDown_Down();
            await Task.Delay(100);
            switchGamepad.ButtonDpadDown_Up();
        }

        async Task SwitchDpadUpThread()
        {
            switchGamepad.ButtonDpadUp_Down();
            await Task.Delay(100);
            switchGamepad.ButtonDpadUp_Up();
        }

        async Task SwitchDpadLeftThread()
        {
            switchGamepad.ButtonDpadLeft_Down();
            await Task.Delay(100);
            switchGamepad.ButtonDpadLeft_Up();
        }

        async Task SwitchLeftThread()
        {
            switchGamepad.leftStick_MoveLeft(100);
            await Task.Delay(100);
            switchGamepad.leftStick_Stop();
        }

        async Task SwitchZLThread()
        {
            switchGamepad.ButtonZR_Down();
            await Task.Delay(100);
            switchGamepad.ButtonZR_Up();
        }

        async Task SwitchLThread()
        {
            switchGamepad.ButtonL_Down();
            await Task.Delay(100);
            switchGamepad.ButtonL_Up();
        }

        async Task SwitchRThread()
        {
            switchGamepad.ButtonR_Down();
            await Task.Delay(100);
            switchGamepad.ButtonR_Up();
        }

        async Task SwitchZRThread()
        {
            switchGamepad.ButtonZR_Down();
            await Task.Delay(100);
            switchGamepad.ButtonZR_Up();
        }

        async Task SwitchUpThread()
        {
            switchGamepad.leftStick_MoveUp(100);
            await Task.Delay(100);
            switchGamepad.leftStick_Stop();
        }

        async Task SwitchDownThread()
        {

            switchGamepad.leftStick_MoveDown(100);
            await Task.Delay(100);
            switchGamepad.leftStick_Stop();

        }

        async Task SwitchRStickLeftThread()
        {
            switchGamepad.rightStick_MoveLeft(100);
            await Task.Delay(100);
            switchGamepad.rightStick_Stop();
        }

        async Task SwitchRStickRightThread()
        {
            switchGamepad.rightStick_MoveRight(100);
            await Task.Delay(100);
            switchGamepad.rightStick_Stop();
        }

        async Task SwitchRStickUpThread()
        {

            switchGamepad.rightStick_MoveUp(100);
            await Task.Delay(100);
            switchGamepad.rightStick_Stop();

        }

        async Task SwitchRStickDownThread()
        {
            switchGamepad.rightStick_MoveDown(100);
            await Task.Delay(100);
            switchGamepad.rightStick_Stop();

        }

        async Task SwitchL3Thread()
        {
            switchGamepad.ButtonL3_Down();
            await Task.Delay(100);
            switchGamepad.ButtonL3_Up();
        }

        async Task SwitchR3Thread()
        {
            switchGamepad.ButtonR3_Down();
            await Task.Delay(100);
            switchGamepad.ButtonR3_Up();
        }

        async Task SwitchCaptureThread()
        {
            switchGamepad.ButtonCapture_Down();
            await Task.Delay(100);
            switchGamepad.ButtonCapture_Up();
        }


        private void btnSwitchWakeup_Click(object sender, EventArgs e)
        {
            if (xboxMode)
            {
                SimGamePad.Instance.Unplug();
                SimGamePad.Instance.ShutDown();
                btnSwitchWakeup.Enabled = false;
                btnStartScreen.Enabled = true;

                SimGamePad.Instance.Initialize(true, "xbox");

                SimGamePad.Instance.PlugIn();
                psLabels = false; drawPrompts(); btnAccept.Text = "A"; btnBack.Text = "B"; btnSwitchHome.Text = "Xbox Button"; btnZL.Text = "LB"; btnZR.Text = "RB";
                InitializeArrowsGamepadOptions();
            }
            if (switchMode)
            {
                resetTransparency(false, true);
                ExtendedDialogResult result = ExtendedMessageBox.Show("<p style='margin:0;padding:0;line-height:1.6;'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; font-size:" + truncateSize + "pt; ' title='Are you using a Switch 2? It does not support the Wake function for 3rd party accessories yet.'>Are you using a Switch 2? It does not support the Wake function for 3rd party accessories yet.</span></p>", "Switch 1 or 2?", ExtendedMessageBoxLibrary.MessageBoxButtons.YesNo, MessageBoxIcon.None, lines: 2, zoomFactor: zoomFactor);

                resetTransparency(true, true);
                if (result.Result == DialogResult.No)
                {
                    Task.Run(() => SwitchWakeupThread());
                }
                else
                {
                    btnSwitchWakeup.Enabled = false;
                }
            }
        }

        private void btnStartScreen_Click(object sender, EventArgs e)
        {
            if (xboxMode)
            {
                SimGamePad.Instance.Unplug();
                SimGamePad.Instance.ShutDown();
                btnStartScreen.Enabled = false;
                btnSwitchWakeup.Enabled = true;

                SimGamePad.Instance.Initialize(true, "ds4");

                SimGamePad.Instance.PlugIn();
                psLabels = true; drawPrompts(); btnAccept.Text = "X"; btnBack.Text = "O"; btnSwitchHome.Text = "PS Button"; btnZL.Text = "L1"; btnZR.Text = "R1";
                InitializeArrowsGamepadOptions();
            }
            if (switchMode)
            {
                Task.Run(() => SwitchStartScreenThread());
            }
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            if (switchMode)
            {
             
                    Task.Run(() => SwitchAcceptThread());
                
            }
            else if (xboxMode)
            {
                if (cmbArrowsGamepad.Text == "XY")
                {
                    SimGamePad.Instance.Use(14, toggleXbox, 1, 100);
                }
                else
                {
                    SimGamePad.Instance.Use(12, toggleXbox, 1, 100);
                }
            }
            else if (keyboardMode)
            {
                inputSimulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (switchMode)
            {
               
                    Task.Run(() => SwitchBackThread());
                
            }
            else if (xboxMode)
            {
                if (cmbArrowsGamepad.Text == "XY")
                {
                    SimGamePad.Instance.Use(15, toggleXbox, 1, 100);
                }
                else
                {
                    SimGamePad.Instance.Use(13, toggleXbox, 1, 100);
                }
            }
            else if (keyboardMode)
            {
                inputSimulator.Keyboard.KeyPress(VirtualKeyCode.ESCAPE);
            }
        }

        private void btnNintendoStreamerX_Click(object sender, EventArgs e)
        {
            if (!switchMode || !UsesNintendoStreamerPlayerRouting())
            {
                return;
            }

            Task.Run(() => SwitchXThread());
        }

        private void btnNintendoStreamerY_Click(object sender, EventArgs e)
        {
            if (!switchMode || !UsesNintendoStreamerPlayerRouting())
            {
                return;
            }

            Task.Run(() => SwitchYThread());
        }

        async Task SwitchXThread()
        {
            switchGamepad.ButtonX_Down();
            await Task.Delay(100);
            switchGamepad.ButtonX_Up();
        }

        async Task SwitchYThread()
        {
            switchGamepad.ButtonY_Down();
            await Task.Delay(100);
            switchGamepad.ButtonY_Up();
        }

        private void btnSwitchStart_Click(object sender, EventArgs e)
        {


            if (switchMode)
            {
                Task.Run(() => SwitchStartThread());
            }
            else if (xboxMode)
            {
                SimGamePad.Instance.Use(4, toggleXbox, 1, 100);
            }
            else if (keyboardMode)
            {
                inputSimulator.Keyboard.KeyPress(VirtualKeyCode.LWIN);
            }
        }

        private void btnSwitchSelect_Click(object sender, EventArgs e)
        {
            if (switchMode)
            {
                Task.Run(() => SwitchSelectThread());
            }
            else if (xboxMode)
            {
                SimGamePad.Instance.Use(5, toggleXbox, 1, 100);

            }
            else if (keyboardMode)
            {
                inputSimulator.Keyboard.KeyPress(VirtualKeyCode.SPACE);
            }
        }

        private (string? streamerMode, string? playerList) ReadSwitchHomeModeFiles()
        {
            var streamerModePath = Path.Combine(configPath, "StreamerMode.txt");
            string? streamerMode = null;

            if (System.IO.File.Exists(streamerModePath))
            {
                using var fs = new FileStream(streamerModePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = new StreamReader(fs);
                streamerMode = reader.ReadLine();
            }

            var playerListPath = Path.Combine(configPath, "player.txt");
            string? playerList = null;

            if (System.IO.File.Exists(playerListPath))
            {
                using var ps = new FileStream(playerListPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var readerps = new StreamReader(ps);
                playerList = readerps.ReadLine();
            }

            return (streamerMode, playerList);
        }

        private void btnSwitchHome_Click(object sender, EventArgs e)
        {
            var (firstLine, playerList) = ReadSwitchHomeModeFiles();
            bool playerTwoPresent = playerList?.Contains("player2") == true;
            bool isRemoteChecked = chkRemote?.Checked == true;
            bool arrowsUseDpad = string.Equals(cmbArrowsGamepad?.Text, "DPad", StringComparison.Ordinal);



            bool streamerModeContainsTrue = (firstLine?.IndexOf("true", StringComparison.OrdinalIgnoreCase) ?? -1) >= 0;

            if ((!isRemoteChecked && !streamerModeContainsTrue) || (!isRemoteChecked && streamerModeContainsTrue && arrowsUseDpad && playerNum == 1) || (!isRemoteChecked && streamerModeContainsTrue && playerNum == 1 && !playerTwoPresent))
            {

                
                    if (switchMode)
                {
                    
                        Task.Run(() => SwitchHomeThread());
                    
                }
                else if (xboxMode)
                {
                    
                        SimGamePad.Instance.Use(10, toggleXbox, 1, 100);
                    
                }
                else if (keyboardMode)
                {
                    inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.MENU, VirtualKeyCode.TAB);
                }

            }
            else
            {
                Reset();

                if (remotePositionCheck)
                {
                    remotePositionCheck = false;
                    btnSwitchHome.Text = "Webfuse OFF";
                    HideWebfusePanel();


                    if (isActive)
                    {

                        isActive = false;

                        SetLblActiveInactiveUi(GetEffectiveNintendoStreamerPlayerStatusText(false));
                        activeFalse();
                        lblLegend.BackColor = System.Drawing.Color.White; lblLegend.ForeColor = System.Drawing.Color.Black;
                    }
                }
                else
                {
                    remotePositionCheck = true;
                    btnSwitchHome.Text = "Webfuse ON";
                    HideWebfusePanel();

                    if (!isActive)
                    {

                        isActive = true;

                        SetLblActiveActiveUi(GetEffectiveNintendoStreamerPlayerStatusText(true));


                        activeTrue();
                    }
                }
            }

        }

        private void btnSwitchHome_MouseClick(object sender, MouseEventArgs e)
        {
            //            quadrantsSVG.Eval(@"
            //        // Check if running from file:// protocol
            //        if (window.location.protocol === 'file:') {
            //            document.getElementById('warningBanner').classList.add('show');
            //        }

            //alert('hi');

            //        let sdk = null;
            //        let gazePreviewEnabled = false;

            //        // UI Elements
            //        const requestCameraBtn = document.getElementById('requestCameraBtn');
            //        const initializeBtn = document.getElementById('initializeBtn');
            //        const calibrateBtn = document.getElementById('calibrateBtn');
            //        const pauseBtn = document.getElementById('pauseBtn');
            //        const resumeBtn = document.getElementById('resumeBtn');
            //        const togglePreviewBtn = document.getElementById('togglePreviewBtn');

            //        const versionValue = document.getElementById('versionValue');
            //        const cameraStatus = document.getElementById('cameraStatus');
            //        const sdkStatus = document.getElementById('sdkStatus');
            //        const calibrationStatus = document.getElementById('calibrationStatus');
            //        const trackingStatus = document.getElementById('trackingStatus');
            //        const gazeValue = document.getElementById('gazeValue');

            //        // Request Camera
            //        requestCameraBtn.onclick = async () => {
            //            try {
            //                requestCameraBtn.disabled = true;
            //                cameraStatus.textContent = 'Requesting...';

            //                sdk = new RealEyeSDK();
            //                versionValue.textContent = sdk.version;

            //navigator.mediaDevices.enumerateDevices()
            //  .then(devices => {
            //      const cameras = devices.filter(d => d.kind === 'videoinput');
            //      console.log('Available cameras:', cameras);

            //      // Choose the second camera (for example)
            //      if (cameras.length > 1) {
            //          window.defaultCameraId = cameras[1].deviceId;
            //      } else if (cameras.length > 0) {
            //          window.defaultCameraId = cameras[0].deviceId;
            //      }
            //  });

            //await sdk.requestCamera({ deviceId: '6be111fc15e6a201c302d606eedf77b892dd464e1da370d906f245ecde78386b'  });

            //                cameraStatus.textContent = 'Ready';
            //                initializeBtn.disabled = false;

            //            } catch (error) {
            //                console.error('Camera request failed:', error);
            //                cameraStatus.textContent = 'Failed: ' + error.message;
            //                requestCameraBtn.disabled = false;
            //            }
            //        };

            //        // Initialize SDK
            //        initializeBtn.onclick = async () => {
            //            try {
            //                initializeBtn.disabled = true;
            //                sdkStatus.textContent = 'Loading models...';

            //                await sdk.initialize();

            //                sdkStatus.textContent = 'Ready';
            //                calibrateBtn.disabled = false;

            //            } catch (error) {
            //                console.error('Initialization failed:', error);
            //                sdkStatus.textContent = 'Failed: ' + error.message;
            //                initializeBtn.disabled = false;
            //            }
            //        };

            //        // Start Calibration
            //        calibrateBtn.onclick = async () => {
            //            try {
            //                calibrateBtn.disabled = true;
            //                calibrationStatus.textContent = 'Calibrating...';

            //                await sdk.startCalibration({
            //                    onProgress: (current, total) => {
            //                        calibrationStatus.textContent = `Calibrating (${current}/${total})...`;
            //                    },
            //                    onComplete: () => {
            //                        calibrationStatus.textContent = 'Calibrated ?';
            //                        trackingStatus.textContent = 'Active';
            //                        pauseBtn.disabled = false;
            //                        resumeBtn.disabled = true;
            //                        togglePreviewBtn.disabled = false;

            //                        // Auto-enable gaze preview after calibration
            //                        gazePreviewEnabled = true;
            //                        sdk.toggleGazePreview(true);
            //                        togglePreviewBtn.textContent = 'Hide Gaze Preview';
            //                    }
            //                });

            //                // Subscribe to gaze updates
            //                sdk.onGazeUpdate((x, y) => {
            //                    // Update status
            //                    gazeValue.textContent = `x: ${x.toFixed(3)}, y: ${y.toFixed(3)}`;
            //                });

            //            } catch (error) {
            //                console.error('Calibration failed:', error);
            //                calibrationStatus.textContent = 'Failed: ' + error.message;
            //                calibrateBtn.disabled = false;
            //            }
            //        };

            //        // Pause
            //        pauseBtn.onclick = () => {
            //            sdk.pause();
            //            trackingStatus.textContent = 'Paused';
            //            pauseBtn.disabled = true;
            //            resumeBtn.disabled = false;
            //        };

            //        // Resume
            //        resumeBtn.onclick = () => {
            //            sdk.resume();
            //            trackingStatus.textContent = 'Active';
            //            resumeBtn.disabled = true;
            //            pauseBtn.disabled = false;
            //        };

            //        // Toggle Gaze Preview
            //        togglePreviewBtn.onclick = () => {
            //            gazePreviewEnabled = !gazePreviewEnabled;
            //            sdk.toggleGazePreview(gazePreviewEnabled);
            //            togglePreviewBtn.textContent = gazePreviewEnabled 
            //                ? 'Hide Gaze Preview' 
            //                : 'Show Gaze Preview';
            //        };

            //        // Cleanup on page unload
            //        window.addEventListener('beforeunload', () => {
            //            if (sdk) {
            //                sdk.destroy();
            //            }
            //        });");

            var (firstLine, playerList) = ReadSwitchHomeModeFiles();
            bool playerTwoPresent = playerList?.Contains("player2") == true;
            bool isRemoteChecked = chkRemote?.Checked == true;

            if ((string.IsNullOrWhiteSpace(firstLine) == false && isRemoteChecked) || (playerNum == 1 && playerTwoPresent))
            {


                if (remotePositionCheck)
                {


                    try
                    {
                        using (FileStream fs = new FileStream(Path.Combine(configPath, "ToggleRemote.txt"), FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                        using (StreamWriter writer = new StreamWriter(fs))
                        {
                            writer.WriteLine(playerNum + "-false");
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
                else
                {


                    try
                    {
                        using (FileStream fs = new FileStream(Path.Combine(configPath, "ToggleRemote.txt"), FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                        using (StreamWriter writer = new StreamWriter(fs))
                        {
                            writer.WriteLine(playerNum + "-true");
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
            }
        }

    }
}