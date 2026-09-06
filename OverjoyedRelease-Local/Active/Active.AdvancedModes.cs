using ExtendedMessageBoxLibrary;
using GregsStack.InputSimulatorStandard.Native;
using Microsoft.Maui.ApplicationModel;
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

        float aimDotX = 300f;
        float aimDotY = 350f;

        float joyCenterX = 300f;
        float joyCenterY = 350f;

        DateTime lastUpdate = DateTime.Now;

        void UpdateRightStickDot(float mouseX, float mouseY)
        {
            const float radius = 60f;

            const float centerX = 300f;
            const float centerY = 350f;

            // joystick offset
            float dx = mouseX - joyCenterX;
            float dy = mouseY - joyCenterY;

            float dist = MathF.Sqrt(dx * dx + dy * dy);

            float stickX = 0f;
            float stickY = 0f;

            if (dist > 0)
            {
                stickX = dx / radius;
                stickY = dy / radius;

                stickX = Math.Clamp(stickX, -1f, 1f);
                stickY = Math.Clamp(stickY, -1f, 1f);
            }

            // floating joystick behavior
            if (dist > radius)
            {
                float overflow = dist - radius;

                float nx = dx / dist;
                float ny = dy / dist;

                joyCenterX += nx * overflow;
                joyCenterY += ny * overflow;
            }

            // speed when stick fully held
            const float speedX = 120f;
            const float speedY = 120f;

            // time limits
            const float maxTimeX = 999.0f;
            const float maxTimeY = 999.0f;

            float minX = centerX - speedX * maxTimeX;
            float maxX = centerX + speedX * maxTimeX;

            float minY = centerY - speedY * maxTimeY;
            float maxY = centerY + speedY * maxTimeY;

            var now = DateTime.Now;
            float dt = (float)(now - lastUpdate).TotalSeconds;
            lastUpdate = now;

            // move aim dot
            aimDotX += stickX * speedX * dt;
            aimDotY += stickY * speedY * dt;

            aimDotX = Math.Clamp(aimDotX, minX, maxX);
            aimDotY = Math.Clamp(aimDotY, minY, maxY);

            // stop stick at edges
            if ((aimDotX <= minX && stickX < 0) || (aimDotX >= maxX && stickX > 0))
                stickX = 0;

            if ((aimDotY <= minY && stickY < 0) || (aimDotY >= maxY && stickY > 0))
                stickY = 0;

            // convert -1..1 ? 0..255 for Switch
            int rxVal = (int)(127 + stickX * 127);
            int ryVal = (int)(127 + stickY * 127);

            rxVal = Math.Clamp(rxVal, 0, 255);
            ryVal = Math.Clamp(ryVal, 0, 255);

            // output depending on mode
            if (xboxMode)
            {
                // Xbox right stick uses -1..1 floats
                var pad = SimGamePad.Instance;
                pad.MoveSticks(2, stickX, invert: false);
                pad.MoveSticks(3, stickY, invert: true);
            }
            else if (switchMode)
            {
                // Switch microHID uses 0..255
                switchGamepad.MoveRightStick(rxVal, ryVal);
            }

            // joystick knob position
            float knobX = joyCenterX + stickX * radius;
            float knobY = joyCenterY + stickY * radius;

        }

        private void chkFPS_CheckedChanged(object sender, EventArgs e)
        {
            if (chkFPS.Checked)
            {
                showLeftClickLabels.Checked = true;
                //pnlPrompts.Visible = true;
                //showLabelsHover = false;
                //showLabelsLC = true;
                //showLabelsRC = false;
                //drawPrompts();

            }
            else
            {
                fpsDeadZoneLookModeLatched = false;
            }

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
        }


        private void chkVoice_CheckedChanged(object sender, EventArgs e)
        {
            if (chkVoice.Checked)
            {

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

                        writer.WriteLine("keys");

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

                resetTransparency(false, true);
                ExtendedDialogResult result = ExtendedMessageBox.Show("<p style='margin:0;padding:0;line-height:1.6;'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 4; -webkit-box-orient: vertical; overflow: hidden; font-size:" + truncateSize + "pt; ' title='Voice Control mode allows you to trigger Left Click actions with just your voice! Have you already installed the Cephable software and imported the profile that allows Overjoyed to connect to it?'>Voice Control mode allows you to trigger Left Click actions with just your voice! Have you already installed the Cephable software and imported the profile that allows Overjoyed to connect to it?</span></p>", "Install Cephable for Voice Control", ExtendedMessageBoxLibrary.MessageBoxButtons.YesNo, lines: 4, zoomFactor: zoomFactor);
                resetTransparency(true, true);

                if (result.Result == DialogResult.No)
                {
                    resetTransparency(false, true);
                   ExtendedMessageBox.Show("<p style='margin:0;padding:0;line-height:1.6;'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; font-size:" + truncateSize + "pt; ' title='After you click OK, install Cephable on the Microsoft Store website we redirect you to.'>After you click OK, install Cephable on the Microsoft Store website we redirect you to.</span></p>", "Install Cephable", ExtendedMessageBoxLibrary.MessageBoxButtons.OK, lines: 2, zoomFactor: zoomFactor);
                    resetTransparency(true, true);

                    System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "https://apps.microsoft.com/detail/9nk48q9p0vds",
                        UseShellExecute = true
                    };
                    System.Diagnostics.Process.Start(psi);

                    resetTransparency(false, true);
                    ExtendedMessageBox.Show("<p style='margin:0;padding:0;line-height:1.6;'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 3; -webkit-box-orient: vertical; overflow: hidden; font-size:" + truncateSize + "pt; ' title='After installing Cephable and setting up your account, click OK on this message box to access the Cephable profile for Overjoyed in your browser.'>After installing Cephable and setting up your account, click OK on this message box to access the Cephable profile for Overjoyed in your browser.</span></p>", "Access Cephable Profile", ExtendedMessageBoxLibrary.MessageBoxButtons.OK, lines: 3, zoomFactor: zoomFactor);
                    resetTransparency(true, true); 
                    
                    System.Diagnostics.ProcessStartInfo psi2 = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "https://share.cephable.com/profileshare/copy/ZGY0ZmUyZDgtZjVjNS00YTQxLTgxYjctODdiODAzNDJjYjJlLTk1NTEwMjlhLTNjZjItNDA4MS1iOTg3LTM0NGFhNDBjM2ZjZA",
                        UseShellExecute = true
                    };
                    System.Diagnostics.Process.Start(psi2);

                    resetTransparency(false, true);
                    ExtendedMessageBox.Show("<p style='margin:0;padding:0;line-height:1.6;'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 4; -webkit-box-orient: vertical; overflow: hidden; font-size:" + truncateSize + "pt; ' title='On the website we redirected you to, press the Preview Control Profile in App button (if a popup appears in your browser, approve it). In Cephable, click Customize App Controls (upper right) then Save And Finish (bottom).'>On the website we redirected you to, press the Preview Control Profile in App button (if a popup appears in your browser, approve it). In Cephable, click Customize App Controls (upper right) then Save And Finish (bottom).</span></p>", "Import Overjoyed Profile", ExtendedMessageBoxLibrary.MessageBoxButtons.OK, lines: 4, zoomFactor: zoomFactor);
                    resetTransparency(true, true);

                   


                }

                resetTransparency(false, true);
                ExtendedMessageBox.Show("<p style='margin:0;padding:0;line-height:1.6;'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 5; -webkit-box-orient: vertical; overflow: hidden; font-size:" + truncateSize + "pt; ' title='Open Cephable, choose Voice Control for Overjoyed in the Current Controls dropdown, and turn your microphone on in the sidebar. Say UP to trigger North quadrant, DOWN  for South, LEFT for West, RIGHT for East, ONE for Northwest, TWO for Northeast, THREE for Southwest, FOUR for Southeast.'>Open Cephable, choose 'Voice Control for Overjoyed' in the Current Controls dropdown, and turn your microphone on in the sidebar. Say UP to trigger North quadrant, DOWN  for South, LEFT for West, RIGHT for East, ONE for Northwest, TWO for Northeast, THREE for Southwest, FOUR for Southeast.</span></p>", "Import Overjoyed Profile", ExtendedMessageBoxLibrary.MessageBoxButtons.OK, lines: 5, zoomFactor: zoomFactor);
                resetTransparency(true, true);
            }
            else
            {

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

                        writer.WriteLine(chkCombineControllers.Checked);

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
        }




    }
}