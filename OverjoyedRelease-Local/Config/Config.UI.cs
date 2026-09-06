using ExtendedMessageBoxLibrary;
using GregsStack.InputSimulatorStandard.Native;
using Microsoft.Maui.ApplicationModel;
using SharedVars;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Wisej.Web;


namespace OverjoyedReleaseLocal
{
    public partial class Config : Wisej.Web.Page
    {


        private void DisableAllExceptOne(Wisej.Web.Panel panel, Wisej.Web.Panel exceptionPanel, Control exceptionControl)
        {
            foreach (Wisej.Web.Panel ctrl in panel.Controls)
            {
                ctrl.Enabled = false;
            }
            exceptionPanel.Enabled = true;
            if (originalEnabledStates.Count == 0)
            {
                foreach (Wisej.Web.Control ctrl in exceptionPanel.Controls)
                {
                    originalEnabledStates[ctrl] = ctrl.Enabled;
                }
            }

            foreach (Wisej.Web.Control ctrl in exceptionPanel.Controls)
            {
                if (ctrl != exceptionControl)
                {
                    ctrl.Enabled = false;
                }
            }
        }

        private void EnableAllControls(Wisej.Web.Panel panel, Wisej.Web.Panel exceptionControl)
        {
            foreach (Wisej.Web.Panel ctrl in panel.Controls)
            {
                ctrl.Enabled = true;
            }

            foreach (Wisej.Web.Control ctrl in exceptionControl.Controls)
            {
                if (originalEnabledStates.TryGetValue(ctrl, out bool wasEnabled))
                {
                    ctrl.Enabled = wasEnabled;
                }
            }
            originalEnabledStates.Clear();
        }


        public void SetSvgBackgroundButton(Button btn, string svgFilePath, string svgSize)
        {
            //Read the content of the SVG file
            string svgContent = System.IO.File.ReadAllText(svgFilePath);

            if (svgSize == "contain")
            {

                //Set the background image using CSS
                string css = $@"
        background-image: url('data:image/svg+xml;utf8,{Uri.EscapeDataString(svgContent)}');
        background-repeat: no-repeat;
        background-size:60%;
        background-position: center;
    ";

                //Apply the CSS to the HtmlPanel
                btn.CssStyle = css;
            }
            else if (svgSize == "cover")
            {

                //Set the background image using CSS
                string css = $@"
        background-image: url('data:image/svg+xml;utf8,{Uri.EscapeDataString(svgContent)}');
        background-repeat: no-repeat;
        background-size: cover;
    ";

                //Apply the CSS to the HtmlPanel
                btn.CssStyle = css;
            }
        }

        public void SetSvgBackground(Panel panel, string svgFilePath, string svgSize)
        {
            //Read the content of the SVG file
            string svgContent = System.IO.File.ReadAllText(svgFilePath);

            if (svgSize == "contain")
            {

                //Set the background image using CSS
                string css = $@"
        background-image: url('data:image/svg+xml;utf8,{Uri.EscapeDataString(svgContent)}');
        background-repeat: no-repeat;
        background-size:63%;
        background-position: center;
    ";

                //Apply the CSS to the HtmlPanel
                panel.CssStyle = css;
            }
            else if (svgSize == "cover")
            {

                //Set the background image using CSS
                string css = $@"
        background-image: url('data:image/svg+xml;utf8,{Uri.EscapeDataString(svgContent)}');
        background-repeat: no-repeat;
        background-size: cover;
    ";

                //Apply the CSS to the HtmlPanel
                panel.CssStyle = css;
            }
            else if (svgSize == "special")
            {

                //Set the background image using CSS
                string css = $@"
        background-image: url('data:image/svg+xml;utf8,{Uri.EscapeDataString(svgContent)}');
        background-repeat: no-repeat;
        background-size: cover;
        background-position-y: -20px;
        background-position-x: center;
outline:3px solid #000000;
    ";

                //Apply the CSS to the HtmlPanel
                panel.CssStyle = css;
            }
            else if (svgSize == "quadrants")
            {

                //Set the background image using CSS
                string css = $@"
        background-image: url('data:image/svg+xml;utf8,{Uri.EscapeDataString(svgContent)}');
        background-repeat: no-repeat;
        background-size: cover;
background-position-y:2px;
        background-position-x: center;
outline:4px solid rgb(0, 167, 209);border-radius:0px;
    ";

                //Apply the CSS to the HtmlPanel
                panel.CssStyle = css;
            }
        }

        private void ScaleFonts(Control parent, float scale)
        {
            // Scale the font size for the parent control
            parent.Font = new System.Drawing.Font(
                parent.Font.Name,
                parent.Font.Size * scale,
                parent.Font.Style
            );

            foreach (Control ctrl in parent.Controls)
            {
                // Scale the font size
                ctrl.Font = new System.Drawing.Font(
                    ctrl.Font.Name,
                    ctrl.Font.Size * scale,
                    ctrl.Font.Style
                );

                // Recursively scale fonts in child controls
                if (ctrl.HasChildren)
                    ScaleFonts(ctrl, scale);
            }
        }



        private void chkLabelsHover_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLabelsHover.Checked) { chkLabelsLC.Checked = false; chkLabelsRC.Checked = false; }
        }

        private void chkLabelsLC_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLabelsLC.Checked) { chkLabelsHover.Checked = false; chkLabelsRC.Checked = false; }
        }

        private void chkLabelsRC_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLabelsRC.Checked) { chkLabelsLC.Checked = false; chkLabelsHover.Checked = false; }
        }

        private void disUpHover_CheckedChanged(object sender, EventArgs e)
        {
            if (disUpHover.Checked == true) { togUpHover.Enabled = false; togUpHover.Checked = false; varUpHover.Checked = false; varUpHover.Enabled = false; }
            else { togUpHover.Enabled = true; varUpHover.Enabled = true; }
        }

        private void disUpRightHover_CheckedChanged(object sender, EventArgs e)
        {
            if (disUpRightHover.Checked == true) { togUpRightHover.Enabled = false; togUpRightHover.Checked = false; varUpRightHover.Checked = false; varUpRightHover.Enabled = false; }
            else { togUpRightHover.Enabled = true; varUpRightHover.Enabled = true; }
        }

        private void disRightHover_CheckedChanged(object sender, EventArgs e)
        {
            if (disRightHover.Checked == true) { togRightHover.Enabled = false; togRightHover.Checked = false; varRightHover.Checked = false; varRightHover.Enabled = false; }
            else { togRightHover.Enabled = true; varRightHover.Enabled = true; }
        }

        private void disDownRightHover_CheckedChanged(object sender, EventArgs e)
        {
            if (disDownRightHover.Checked == true) { togDownRightHover.Enabled = false; togDownRightHover.Checked = false; varDownRightHover.Checked = false; varDownRightHover.Enabled = false; }
            else { togDownRightHover.Enabled = true; varDownRightHover.Enabled = true; }
        }

        private void disDownHover_CheckedChanged(object sender, EventArgs e)
        {
            if (disDownHover.Checked == true) { togDownHover.Enabled = false; togDownHover.Checked = false; varDownHover.Enabled = false; varDownHover.Checked = false; }
            else { togDownHover.Enabled = true; varDownHover.Enabled = true; }
        }

        private void disDownLeftHover_CheckedChanged(object sender, EventArgs e)
        {
            if (disDownLeftHover.Checked == true) { togDownLeftHover.Enabled = false; togDownLeftHover.Checked = false; varDownLeftHover.Enabled = false; varDownLeftHover.Checked = false; }
            else { togDownLeftHover.Enabled = true; varDownLeftHover.Enabled = true; }
        }

        private void disLeftHover_CheckedChanged(object sender, EventArgs e)
        {
            if (disLeftHover.Checked == true) { togLeftHover.Enabled = false; togLeftHover.Checked = false; varLeftHover.Enabled = false; varLeftHover.Checked = false; }
            else { togLeftHover.Enabled = true; varLeftHover.Enabled = true; }
        }

        private void disUpLeftHover_CheckedChanged(object sender, EventArgs e)
        {
            if (disUpLeftHover.Checked == true) { togUpLeftHover.Enabled = false; togUpLeftHover.Checked = false; varUpLeftHover.Enabled = false; varUpLeftHover.Checked = false; }
            else { togUpLeftHover.Enabled = true; varUpLeftHover.Enabled = true; }
        }

        private void disDZUpperHover_CheckedChanged(object sender, EventArgs e)
        {
            if (disDZUpperHover.Checked == true) { togDZUpperHover.Enabled = false; togDZUpperHover.Checked = false; btnDZUpperHover.Enabled = false; cmbDZUpperHover.Enabled = false; switchDZUpperHover.Enabled = false; }
            else { togDZUpperHover.Enabled = true; btnDZUpperHover.Enabled = true; cmbDZUpperHover.Enabled = true; switchDZUpperHover.Enabled = true; }
        }

        //left click

        private void disUpLC_CheckedChanged(object sender, EventArgs e)
        {
            if (disUpLC.Checked == true) { togUpLC.Enabled = false; togUpLC.Checked = false; rtcUpLC.Enabled = false; rtcUpLC.Checked = false; }
            else { togUpLC.Enabled = true; rtcUpLC.Enabled = true; }
        }

        private void disUpRightLC_CheckedChanged(object sender, EventArgs e)
        {
            if (disUpRightLC.Checked == true) { togUpRightLC.Enabled = false; togUpRightLC.Checked = false; rtcUpRightLC.Enabled = false; rtcUpRightLC.Checked = false; }
            else { togUpRightLC.Enabled = true; rtcUpRightLC.Enabled = true; }
        }

        private void disRightLC_CheckedChanged(object sender, EventArgs e)
        {
            if (disRightLC.Checked == true) { togRightLC.Enabled = false; togRightLC.Checked = false; rtcRightLC.Enabled = false; rtcRightLC.Checked = false; }
            else { togRightLC.Enabled = true; rtcRightLC.Enabled = true; }
        }

        private void disDownRightLC_CheckedChanged(object sender, EventArgs e)
        {
            if (disDownRightLC.Checked == true) { togDownRightLC.Enabled = false; togDownRightLC.Checked = false; rtcDownRightLC.Enabled = false; rtcDownRightLC.Checked = false; }
            else { togDownRightLC.Enabled = true; rtcDownRightLC.Enabled = true; }
        }

        private void disDownLC_CheckedChanged(object sender, EventArgs e)
        {
            if (disDownLC.Checked == true) { togDownLC.Enabled = false; togDownLC.Checked = false; rtcDownLC.Enabled = false; rtcDownLC.Checked = false; }
            else { togDownLC.Enabled = true; rtcDownLC.Enabled = true; }
        }

        private void disDownLeftLC_CheckedChanged(object sender, EventArgs e)
        {
            if (disDownLeftLC.Checked == true) { togDownLeftLC.Enabled = false; togDownLeftLC.Checked = false; rtcDownLeftLC.Enabled = false; rtcDownLeftLC.Checked = false; }
            else { togDownLeftLC.Enabled = true; rtcDownLeftLC.Enabled = true; }
        }

        private void disLeftLC_CheckedChanged(object sender, EventArgs e)
        {
            if (disLeftLC.Checked == true) { togLeftLC.Enabled = false; togLeftLC.Checked = false; rtcLeftLC.Enabled = false; rtcLeftLC.Checked = false; }
            else { togLeftLC.Enabled = true; rtcLeftLC.Enabled = true; }
        }

        private void disUpLeftLC_CheckedChanged(object sender, EventArgs e)
        {
            if (disUpLeftLC.Checked == true) { togUpLeftLC.Enabled = false; togUpLeftLC.Checked = false; rtcUpLeftLC.Enabled = false; rtcUpLeftLC.Checked = false; }
            else { togUpLeftLC.Enabled = true; rtcUpLeftLC.Enabled = true; }
        }

        private void disDZUpperLC_CheckedChanged(object sender, EventArgs e)
        {
            if (disDZUpperLC.Checked == true) { togDZUpperLC.Enabled = false; togDZUpperLC.Checked = false; }
            else { togDZUpperLC.Enabled = true; }
        }

        //right click

        private void disUpRC_CheckedChanged(object sender, EventArgs e)
        {
            if (disUpRC.Checked == true) { togUpRC.Enabled = false; togUpRC.Checked = false; rtcUpRC.Enabled = false; rtcUpRC.Checked = false; }
            else { togUpRC.Enabled = true; rtcUpRC.Enabled = true; }
        }

        private void disUpRightRC_CheckedChanged(object sender, EventArgs e)
        {
            if (disUpRightRC.Checked == true) { togUpRightRC.Enabled = false; togUpRightRC.Checked = false; rtcUpRightRC.Enabled = false; rtcUpRightRC.Checked = false; }
            else { togUpRightRC.Enabled = true; rtcUpRightRC.Enabled = true; }
        }

        private void disRightRC_CheckedChanged(object sender, EventArgs e)
        {
            if (disRightRC.Checked == true) { togRightRC.Enabled = false; togRightRC.Checked = false; rtcRightRC.Enabled = false; rtcRightRC.Checked = false; }
            else { togRightRC.Enabled = true; rtcRightRC.Enabled = true; }
        }

        private void disDownRightRC_CheckedChanged(object sender, EventArgs e)
        {
            if (disDownRightRC.Checked == true) { togDownRightRC.Enabled = false; togDownRightRC.Checked = false; rtcDownRightRC.Enabled = false; rtcDownRightRC.Checked = false; }
            else { togDownRightRC.Enabled = true; rtcDownRightRC.Enabled = true; }
        }

        private void disDownRC_CheckedChanged(object sender, EventArgs e)
        {
            if (disDownRC.Checked == true) { togDownRC.Enabled = false; togDownRC.Checked = false; rtcDownRC.Enabled = false; rtcDownRC.Checked = false; }
            else { togDownRC.Enabled = true; rtcDownRC.Enabled = true; }
        }

        private void disDownLeftRC_CheckedChanged(object sender, EventArgs e)
        {
            if (disDownLeftRC.Checked == true) { togDownLeftRC.Enabled = false; togDownLeftRC.Checked = false; rtcDownLeftRC.Enabled = false; rtcDownLeftRC.Checked = false; }
            else { togDownLeftRC.Enabled = true; rtcDownLeftRC.Enabled = true; }
        }

        private void disLeftRC_CheckedChanged(object sender, EventArgs e)
        {
            if (disLeftRC.Checked == true) { togLeftRC.Enabled = false; togLeftRC.Checked = false; rtcLeftRC.Enabled = false; rtcLeftRC.Checked = false; }
            else { togLeftRC.Enabled = true; rtcLeftRC.Enabled = true; }
        }

        private void disUpLeftRC_CheckedChanged(object sender, EventArgs e)
        {
            if (disUpLeftRC.Checked == true) { togUpLeftRC.Enabled = false; togUpLeftRC.Checked = false; rtcUpLeftRC.Enabled = false; rtcUpLeftRC.Checked = false; }
            else { togUpLeftRC.Enabled = true; rtcUpLeftRC.Enabled = true; }
        }

        private void disDZUpperRC_CheckedChanged(object sender, EventArgs e)
        {
            if (disDZUpperRC.Checked == true) { togDZUpperRC.Enabled = false; togDZUpperRC.Checked = false; }
            else { togDZUpperRC.Enabled = true; }
        }


        private void btnSubmit_Click(object sender, EventArgs e)
        {



       
            try
            {

                // Open the file with shared read/write access
                using (FileStream fs = new FileStream(parametersFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                using (StreamWriter file2 = new StreamWriter(fs))

                {
                    file2.WriteLine(cmbProfile.SelectedIndex);
                    string configs = "";
                    for (int i = 1; i <= cmbProfile.Items.Count - 1; i++)
                    {

                        configs += cmbProfile.Items[i].ToString() + ",";


                    }
                    file2.WriteLine(configs);
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

            try
            {
                SaveCurrentProfileButtonConfig();


            }
            catch (IOException ex)
            {
                Console.WriteLine($"File I/O Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }




            //string exeFilePath = Path.Combine(appFolderPath, "OverjoyedReleaseClient.exe");

            string lineMicro = File.ReadLines(tourFilePath).ElementAtOrDefault(1);


            if (nintendoSwitch.Checked == true && lineMicro == "microYes")
            {
                MicroSetup();
                MinimizeMessenger.firmwareToggle();
            }

            string firmwareDialogPath = Path.Combine(configPath, "FirmwareDialog.txt");

            using var fd = new FileStream(firmwareDialogPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = new StreamReader(fd);
            string content = reader.ReadToEnd();

            if (!(nintendoSwitch.Checked == true && lineMicro == "microYes"))
            {
                if (nintendoSwitch.Checked == true && MinimizeMessenger.showFirmware && string.IsNullOrEmpty(content))
                {
                    MinimizeMessenger.firmwareToggle();
                    var result = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Have you purchased an 8BitDo Micro adapter and updated the firmware to support Overjoyed?'>Have you purchased an 8BitDo Micro adapter and updated the firmware to support Overjoyed?</span></p>", "Firmware Check", ExtendedMessageBoxLibrary.MessageBoxButtons.YesNo);

                    if (result.Result == DialogResult.Yes)
                    {
                        var result2 = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 1; -webkit-box-orient: vertical; overflow: hidden; ' title='Are you migrating from Switch 1 to Switch 2?'>Are you migrating from Switch 1 to Switch 2?</span></p>", "Migration Check", ExtendedMessageBoxLibrary.MessageBoxButtons.YesNo);
                        if (result2.Result == DialogResult.Yes)
                        {
                            var result3 = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Did you update your adapter firmware? Switch 2 requires updating to the version I added to Overjoyed in August 2025.'>Did you update your adapter firmware? Switch 2 requires updating to the version I added to Overjoyed in August 2025.</span></p>", "Firmware Check", ExtendedMessageBoxLibrary.MessageBoxButtons.YesNo);

                            if (result3.Result == DialogResult.Yes)
                            {
                                Application.MainPage = new Active();
                                btnSubmit.Text = "(6)\u00A0Apply\u00A0New\u00A0Settings";
                            }
                            else
                            {
                                MicroSetup();
                            }
                        }
                        else
                        {
                            //make content of FirmwareDialog.txt in configPath "true"

                            File.WriteAllText(firmwareDialogPath, "true");
                            Application.MainPage = new Active();
                            btnSubmit.Text = "(6)\u00A0Apply\u00A0New\u00A0Settings";
                        }
                    }
                    else
                    {
                        MicroSetup();
                    }


                }
                else
                {
                    Application.MainPage = new Active();

                    btnSubmit.Text = "(6)\u00A0Apply\u00A0New\u00A0Settings";
                }

            }



        }

        private void cmbUpHover_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isApplyingDisableButtonState)
            {
                return;
            }

            HandleNoneSelectionAsDisableButton(cmbUpHover, "disableUpHover");

            UpdateAllHoverVariableOptions();

        }

        private void cmbUpRightHover_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isApplyingDisableButtonState)
            {
                return;
            }

            HandleNoneSelectionAsDisableButton(cmbUpRightHover, "disableUpRightHover");

            UpdateAllHoverVariableOptions();

        }

        private void cmbRightHover_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isApplyingDisableButtonState)
            {
                return;
            }

            HandleNoneSelectionAsDisableButton(cmbRightHover, "disableRightHover");

            UpdateAllHoverVariableOptions();

        }

        private void cmbDownRightHover_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isApplyingDisableButtonState)
            {
                return;
            }

            HandleNoneSelectionAsDisableButton(cmbDownRightHover, "disableDownRightHover");

            UpdateAllHoverVariableOptions();


        }

        private void cmbDownHover_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isApplyingDisableButtonState)
            {
                return;
            }

            HandleNoneSelectionAsDisableButton(cmbDownHover, "disableDownHover");

            UpdateAllHoverVariableOptions();

        }

        private void cmbDownLeftHover_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isApplyingDisableButtonState)
            {
                return;
            }

            HandleNoneSelectionAsDisableButton(cmbDownLeftHover, "disableDownLeftHover");

            UpdateAllHoverVariableOptions();

        }

        private void cmbLeftHover_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isApplyingDisableButtonState)
            {
                return;
            }

            HandleNoneSelectionAsDisableButton(cmbLeftHover, "disableLeftHover");

            UpdateAllHoverVariableOptions();

        }

        private void cmbUpLeftHover_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isApplyingDisableButtonState)
            {
                return;
            }

            HandleNoneSelectionAsDisableButton(cmbUpLeftHover, "disableUpLeftHover");

            UpdateAllHoverVariableOptions();
        }



        private void chkXbox_CheckedChanged(object sender, EventArgs e)
        {
            if (isLoadingProfile)
            {
                return;
            }

            pnlDeadzone.SuspendLayout();
            pnlQuadrants.SuspendLayout();
            btnHover.PerformClick();

            optUpRight.SelectedIndex = 0;
            optRight.SelectedIndex = 0; optDownRight.SelectedIndex = 0; optDown.SelectedIndex = 0;
            optDownLeft.SelectedIndex = 0; optLeft.SelectedIndex = 0; optUpLeft.SelectedIndex = 0; optUp.SelectedIndex = 0;
            optDZUpper.SelectedIndex = 0; optDZMiddle.SelectedIndex = 0; optDZLower.SelectedIndex = 0;



            if (chkXbox.Checked && !nintendoSwitch.Checked)
            {
                UpdateAllHoverVariableOptions();
            }

            if (chkXbox.Checked)
            {
                nintendoSwitch.Checked = false; chkKeyboard.Checked = false; chkKeyboard.Enabled = true;

                btnUpLclick.Visible = false; btnUpRclick.Visible = false; btnUpHover.Visible = false;
                btnUpRightLclick.Visible = false; btnUpRightRclick.Visible = false; btnUpRightHover.Visible = false;
                btnRightLclick.Visible = false; btnRightRclick.Visible = false; btnRightHover.Visible = false;
                btnDownRightLclick.Visible = false; btnDownRightRclick.Visible = false; btnDownRightHover.Visible = false;
                btnDownLclick.Visible = false; btnDownRclick.Visible = false; btnDownHover.Visible = false;
                btnDownLeftLclick.Visible = false; btnDownLeftRclick.Visible = false; btnDownLeftHover.Visible = false;
                btnLeftLclick.Visible = false; btnLeftRclick.Visible = false; btnLeftHover.Visible = false;
                btnUpLeftLclick.Visible = false; btnUpLeftRclick.Visible = false; btnUpLeftHover.Visible = false;
                btnDZUpperLC.Visible = false; btnDZUpperRC.Visible = false; btnDZUpperHover.Visible = false;
                btnDZMiddleLC.Visible = false; btnDZMiddleRC.Visible = false; btnDZMiddleHover.Visible = false;
                btnDZLowerLC.Visible = false; btnDZLowerRC.Visible = false; btnDZLowerHover.Visible = false;

                cmbUpLclick.Visible = false; cmbUpRclick.Visible = false; cmbUpHover.Visible = true;
                cmbUpRightLclick.Visible = false; cmbUpRightRclick.Visible = false; cmbUpRightHover.Visible = true;
                cmbRightLclick.Visible = false; cmbRightRclick.Visible = false; cmbRightHover.Visible = true;
                cmbDownRightLclick.Visible = false; cmbDownRightRclick.Visible = false; cmbDownRightHover.Visible = true;
                cmbDownLclick.Visible = false; cmbDownRclick.Visible = false; cmbDownHover.Visible = true;
                cmbDownLeftLclick.Visible = false; cmbDownLeftRclick.Visible = false; cmbDownLeftHover.Visible = true;
                cmbLeftLclick.Visible = false; cmbLeftRclick.Visible = false; cmbLeftHover.Visible = true;
                cmbUpLeftLclick.Visible = false; cmbUpLeftRclick.Visible = false; cmbUpLeftHover.Visible = true;
                cmbDZUpperLC.Visible = false; cmbDZUpperRC.Visible = false; cmbDZUpperHover.Visible = true;
                cmbDZMiddleLC.Visible = false; cmbDZMiddleRC.Visible = false; cmbDZMiddleHover.Visible = true;
                cmbDZLowerLC.Visible = false; cmbDZLowerRC.Visible = false; cmbDZLowerHover.Visible = true;

            }
            else if (!chkXbox.Checked)
            {
                if (!isLoadingProfile && !nintendoSwitch.Checked)
                {
                    chkKeyboard.Checked = true;
                }
                cmbUpLclick.Visible = false; cmbUpRclick.Visible = false; cmbUpHover.Visible = false;
                cmbUpRightLclick.Visible = false; cmbUpRightRclick.Visible = false; cmbUpRightHover.Visible = false;
                cmbRightLclick.Visible = false; cmbRightRclick.Visible = false; cmbRightHover.Visible = false;
                cmbDownRightLclick.Visible = false; cmbDownRightRclick.Visible = false; cmbDownRightHover.Visible = false;
                cmbDownLclick.Visible = false; cmbDownRclick.Visible = false; cmbDownHover.Visible = false;
                cmbDownLeftLclick.Visible = false; cmbDownLeftRclick.Visible = false; cmbDownLeftHover.Visible = false;
                cmbLeftLclick.Visible = false; cmbLeftRclick.Visible = false; cmbLeftHover.Visible = false;
                cmbUpLeftLclick.Visible = false; cmbUpLeftRclick.Visible = false; cmbUpLeftHover.Visible = false;
                cmbDZUpperHover.Visible = false; cmbDZUpperLC.Visible = false; cmbDZUpperRC.Visible = false;
                cmbDZMiddleHover.Visible = false; cmbDZMiddleLC.Visible = false; cmbDZMiddleRC.Visible = false;
                cmbDZLowerHover.Visible = false; cmbDZLowerLC.Visible = false; cmbDZLowerRC.Visible = false;

                btnUpLclick.Visible = false; btnUpRclick.Visible = false; btnUpHover.Visible = true;
                btnUpRightLclick.Visible = false; btnUpRightRclick.Visible = false; btnUpRightHover.Visible = true;
                btnRightLclick.Visible = false; btnRightRclick.Visible = false; btnRightHover.Visible = true;
                btnDownRightLclick.Visible = false; btnDownRightRclick.Visible = false; btnDownRightHover.Visible = true;
                btnDownLclick.Visible = false; btnDownRclick.Visible = false; btnDownHover.Visible = true;
                btnDownLeftLclick.Visible = false; btnDownLeftRclick.Visible = false; btnDownLeftHover.Visible = true;
                btnLeftLclick.Visible = false; btnLeftRclick.Visible = false; btnLeftHover.Visible = true;
                btnUpLeftLclick.Visible = false; btnUpLeftRclick.Visible = false; btnUpLeftHover.Visible = true;
                btnDZUpperLC.Visible = false; btnDZUpperRC.Visible = false; btnDZUpperHover.Visible = true;
                btnDZMiddleLC.Visible = false; btnDZMiddleRC.Visible = false; btnDZMiddleHover.Visible = true;
                btnDZLowerLC.Visible = false; btnDZLowerRC.Visible = false; btnDZLowerHover.Visible = true;

                //varUpHover.Visible = false; varUpRightHover.Visible = false; varRightHover.Visible = false; varDownRightHover.Visible = false;
                //varDownHover.Visible = false; varDownLeftHover.Visible = false; varLeftHover.Visible = false; varUpLeftHover.Visible = false;

            }

            if (chkXbox.Checked && !nintendoSwitch.Checked)
            {

                optUp.Items.Remove(optDisable); optUpRight.Items.Remove(optDisable); optRight.Items.Remove(optDisable); optDownRight.Items.Remove(optDisable);
                optDown.Items.Remove(optDisable); optDownLeft.Items.Remove(optDisable); optLeft.Items.Remove(optDisable); optUpLeft.Items.Remove(optDisable);
                optDZUpper.Items.Remove(optDisable); optDZMiddle.Items.Remove(optDisable); optDZLower.Items.Remove(optDisable);
            }
            pnlDeadzone.ResumeLayout();
            pnlQuadrants.ResumeLayout();

            if (chkXbox.Checked == true && !chkKeyboard.Checked && !nintendoSwitch.Checked)
            {

                cmbAdvanced.Items.Remove("(Click to Turn Off) Keep Mouse Inside Overjoyed");
                cmbAdvanced.Items.Remove("(Click to Turn On) Keep Mouse Inside Overjoyed");
                if (advKeepMouse)
                {
                    cmbAdvanced.Items.Add("(Click to Turn On) Keep Mouse Inside Overjoyed");
                }

                else
                { cmbAdvanced.Items.Add("(Click to Turn Off) Keep Mouse Inside Overjoyed"); }

                cmbAdvanced.Items.Remove("(Click to Turn On) Press PlayStation Buttons");
                cmbAdvanced.Items.Remove("(Click to Turn Off) Press PlayStation Buttons");
                if (advPSbuttons)
                {
                    cmbAdvanced.Items.Add("(Click to Turn Off) Press PlayStation Buttons");
                }
                else
                { cmbAdvanced.Items.Add("(Click to Turn On) Press PlayStation Buttons"); }

                cmbAdvanced.Items.Remove("(Click to Turn Off) Prevent Game From Locking Mouse");
                cmbAdvanced.Items.Remove("(Click to Turn On) Prevent Game From Locking Mouse");
                if (advMouseLock)
                {
                    cmbAdvanced.Items.Add("(Click to Turn On) Prevent Game From Locking Mouse");
                }
                else
                { cmbAdvanced.Items.Add("(Click to Turn Off) Prevent Game From Locking Mouse"); }

                cmbAdvanced.Items.Remove("(Click to Turn On) Mario Kart Mode");
                cmbAdvanced.Items.Remove("(Click to Turn Off) Mario Kart Mode");
            }
            Eval(@"function applyMarquee(el) {  if ((el.dataset.marqueeApplied === 'true' && el.querySelector('.marquee-outer')) || el.querySelector('.truncate')){ return;}

  let text = el.textContent.trim();
  if (!text) return;

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
}, 2000); // 2000 milliseconds = 2 seconds
");
        }

        private void disDZMiddleRC_CheckedChanged(object sender, EventArgs e)
        {
            if (disDZMiddleRC.Checked == true) { togDZMiddleRC.Enabled = false; togDZMiddleRC.Checked = false; btnDZMiddleRC.Enabled = false; cmbDZMiddleRC.Enabled = false; }
            else { togDZMiddleRC.Enabled = true; btnDZMiddleRC.Enabled = true; cmbDZMiddleRC.Enabled = true; }

        }

        private void disDZLowerRC_CheckedChanged(object sender, EventArgs e)
        {
            if (disDZLowerRC.Checked == true) { togDZLowerRC.Enabled = false; togDZLowerRC.Checked = false; btnDZLowerRC.Enabled = false; cmbDZLowerRC.Enabled = false; }
            else { togDZLowerRC.Enabled = true; btnDZLowerRC.Enabled = true; cmbDZLowerRC.Enabled = true; }
        }

        private void disDZLowerLC_CheckedChanged(object sender, EventArgs e)
        {
            if (disDZLowerLC.Checked == true) { togDZLowerLC.Enabled = false; togDZLowerLC.Checked = false; btnDZLowerLC.Enabled = false; cmbDZLowerLC.Enabled = false; }
            else { togDZLowerLC.Enabled = true; btnDZLowerLC.Enabled = true; cmbDZLowerLC.Enabled = true; }
        }

        private void disDZLowerHover_CheckedChanged(object sender, EventArgs e)
        {
            if (disDZLowerHover.Checked == true) { togDZLowerHover.Enabled = false; togDZLowerHover.Checked = false; btnDZLowerHover.Enabled = false; cmbDZLowerHover.Enabled = false; switchDZLowerHover.Enabled = false; }
            else { togDZLowerHover.Enabled = true; btnDZLowerHover.Enabled = true; cmbDZLowerHover.Enabled = true; switchDZLowerHover.Enabled = true; }
        }

        private void disDZMiddleLC_CheckedChanged(object sender, EventArgs e)
        {
            if (disDZMiddleLC.Checked == true) { togDZMiddleLC.Enabled = false; togDZMiddleLC.Checked = false; btnDZMiddleLC.Enabled = false; cmbDZMiddleLC.Enabled = false; }
            else { togDZMiddleLC.Enabled = true; btnDZMiddleLC.Enabled = true; cmbDZMiddleLC.Enabled = true; }
        }

        private void disDZMiddleHover_CheckedChanged(object sender, EventArgs e)
        {
            if (disDZMiddleHover.Checked == true) { togDZMiddleHover.Enabled = false; togDZMiddleHover.Checked = false; btnDZMiddleHover.Enabled = false; cmbDZMiddleHover.Enabled = false; }
            else { togDZMiddleHover.Enabled = true; btnDZMiddleHover.Enabled = true; cmbDZMiddleHover.Enabled = true; }
        }

        private void btnDZMiddleHover_Click(object sender, EventArgs e)
        {

            btnDZMiddleHover.Text = "Press Key";
            ToggleForm(false);

        }

        private void btnDZMiddleLC_Click(object sender, EventArgs e)
        {

            btnDZMiddleLC.Text = "Press Key";
            ToggleForm(false);

        }

        private void btnDZLowerRC_Click(object sender, EventArgs e)
        {

            btnDZLowerRC.Text = "Press Key";
            ToggleForm(false);

        }

        private void btnDZLowerHover_Click(object sender, EventArgs e)
        {

            btnDZLowerHover.Text = "Press Key";
            ToggleForm(false);

        }

        private void btnDZLowerLC_Click(object sender, EventArgs e)
        {

            btnDZLowerLC.Text = "Press Key";
            ToggleForm(false);

        }

        private void btnDZMiddleRC_Click(object sender, EventArgs e)
        {

            btnDZMiddleRC.Text = "Press Key";
            ToggleForm(false);

        }

        void ToggleForm(bool status)
        {
            //btnFeedback.Enabled = status;
            //btnSubmit.Enabled = status;
            //cmbProfile.Enabled = status;
            //btnNew.Enabled = status;
            //btnAdd.Enabled = status;
            //btnDefault.Enabled = status;
        }



        private void btnHover_Click(object sender, EventArgs e)
        {
            pnlQuadrants.SuspendLayout();
            pnlDeadzone.SuspendLayout();

            currentTab = "Hover";
            HideAllSwitchAssignmentControls();
            if (diagUpRightHover.Checked)
            {
                mergeUpRight.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(315) translate(10,5)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#00a7d1' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
            }
            else
            {
                mergeUpRight.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(315) translate(10,5)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#636372' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
            }

            if (diagDownRightHover.Checked)
            {
                mergeDownRight.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(45) translate(10,10)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#00a7d1' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
            }
            else
            {
                mergeDownRight.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(45) translate(10,10)'><path transform='rotate(0) scale(2.9) translate(0,-25)' fill='#636372' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
            }

            if (diagUpLeftHover.Checked)
            {
                mergeUpLeft.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(225) translate(11,11)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#00a7d1' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
            }
            else
            {
                mergeUpLeft.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(225) translate(11,11)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#636372' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
            }


            if (diagDownLeftHover.Checked)
            {
                mergeDownLeft.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(135) translate(11,11)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#00a7d1' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
            }
            else
            {
                mergeDownLeft.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(135) translate(11,11)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#636372' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
            }


            optUpRight.SelectedIndex = 0;
            optRight.SelectedIndex = 0; optDownRight.SelectedIndex = 0; optDown.SelectedIndex = 0;
            optDownLeft.SelectedIndex = 0; optLeft.SelectedIndex = 0; optUpLeft.SelectedIndex = 0; optUp.SelectedIndex = 0;
            optDZUpper.SelectedIndex = 0; optDZMiddle.SelectedIndex = 0; optDZLower.SelectedIndex = 0;

            diagDownLeftHover.Visible = true; diagDownRightHover.Visible = true; diagUpLeftHover.Visible = true; diagUpRightHover.Visible = true;
            diagDownLeftLC.Visible = false; diagDownRightLC.Visible = false; diagUpLeftLC.Visible = false; diagUpRightLC.Visible = false;
            diagDownLeftRC.Visible = false; diagDownRightRC.Visible = false; diagUpLeftRC.Visible = false; diagUpRightRC.Visible = false;

            optUpRight.Items.Remove(optHold); optRight.Items.Remove(optHold); optDownRight.Items.Remove(optHold); optDown.Items.Remove(optHold);
            optDownLeft.Items.Remove(optHold); optLeft.Items.Remove(optHold); optUpLeft.Items.Remove(optHold); optUp.Items.Remove(optHold);
            optDZUpper.Items.Remove(optHold); optDZMiddle.Items.Remove(optHold); optDZLower.Items.Remove(optHold);

            optUpRight.Items.Remove(optRTC); optRight.Items.Remove(optRTC); optDownRight.Items.Remove(optRTC); optDown.Items.Remove(optRTC);
            optDownLeft.Items.Remove(optRTC); optLeft.Items.Remove(optRTC); optUpLeft.Items.Remove(optRTC); optUp.Items.Remove(optRTC);

            optDZUpper.Items.Remove(optActivate); optDZMiddle.Items.Remove(optActivate); optDZLower.Items.Remove(optActivate);



            optUpRight.Items.Remove(optOnce); optRight.Items.Remove(optOnce); optDownRight.Items.Remove(optOnce); optDown.Items.Remove(optOnce);
            optDownLeft.Items.Remove(optOnce); optLeft.Items.Remove(optOnce); optUpLeft.Items.Remove(optOnce); optUp.Items.Remove(optOnce);

            optDZUpper.Items.Remove(optOnce); optDZMiddle.Items.Remove(optOnce); optDZLower.Items.Remove(optOnce);


            optUpRight.Items.Add(optOnce); optRight.Items.Add(optOnce); optDownRight.Items.Add(optOnce); optDown.Items.Add(optOnce);
            optDownLeft.Items.Add(optOnce); optLeft.Items.Add(optOnce); optUpLeft.Items.Add(optOnce); optUp.Items.Add(optOnce);

            optDZUpper.Items.Add(optOnce); optDZMiddle.Items.Add(optOnce); optDZLower.Items.Add(optOnce);


            optUpRight.Items.Remove(optVariable); optRight.Items.Remove(optVariable); optDownRight.Items.Remove(optVariable); optDown.Items.Remove(optVariable);
            optDownLeft.Items.Remove(optVariable); optLeft.Items.Remove(optVariable); optUpLeft.Items.Remove(optVariable); optUp.Items.Remove(optVariable);

            disableUpHover.Visible = true; disableUpRightHover.Visible = true; disableRightHover.Visible = true; disableDownRightHover.Visible = true;
            disableDownHover.Visible = true; disableDownLeftHover.Visible = true; disableLeftHover.Visible = true; disableUpLeftHover.Visible = true;

            disableUpLC.Visible = false; disableUpRightLC.Visible = false; disableRightLC.Visible = false; disableDownRightLC.Visible = false;
            disableDownLC.Visible = false; disableDownLeftLC.Visible = false; disableLeftLC.Visible = false; disableUpLeftLC.Visible = false;

            disableUpRC.Visible = false; disableUpRightRC.Visible = false; disableRightRC.Visible = false; disableDownRightRC.Visible = false;
            disableDownRC.Visible = false; disableDownLeftRC.Visible = false; disableLeftRC.Visible = false; disableUpLeftRC.Visible = false;

            disableDZUpperHover.Visible = true; disableDZUpperLC.Visible = false; disableDZUpperRC.Visible = false;

            disableDZMiddleHover.Visible = true; disableDZMiddleLC.Visible = false; disableDZMiddleRC.Visible = false;

            disableDZLowerHover.Visible = true; disableDZLowerLC.Visible = false; disableDZLowerRC.Visible = false;

            if (chkXbox.Checked && !nintendoSwitch.Checked)
            {
                UpdateAllHoverVariableOptions();

            }
            else if (nintendoSwitch.Checked && !chkXbox.Checked)
            {
                UpdateAllHoverVariableOptions();

            }

            btnHover.CssStyle = "border:4px solid rgb(0, 167, 209);border-radius:5px;";
            lblHover.CssStyle = "border-bottom:4px solid rgb(240,240,240);font-weight:bold;";
            lblHover.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);

            btnLeftClick.CssStyle = "border:4px solid black;border-radius:5px;";
            lblLeftClick.CssStyle = "border-bottom:4px solid rgb(0, 167, 209);font-weight:normal;";
            lblLeftClick.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);

            btnRightClick.CssStyle = "border:4px solid black;border-radius:5px;";
            lblRightClick.CssStyle = "border-bottom:4px solid rgb(0, 167, 209);font-weight:normal;";
            lblRightClick.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);

            if (!chkXbox.Checked && !nintendoSwitch.Checked)
            {
                cmbUpLclick.Visible = false; cmbUpRclick.Visible = false; cmbUpHover.Visible = false;
                cmbUpRightLclick.Visible = false; cmbUpRightRclick.Visible = false; cmbUpRightHover.Visible = false;
                cmbRightLclick.Visible = false; cmbRightRclick.Visible = false; cmbRightHover.Visible = false;
                cmbDownRightLclick.Visible = false; cmbDownRightRclick.Visible = false; cmbDownRightHover.Visible = false;
                cmbDownLclick.Visible = false; cmbDownRclick.Visible = false; cmbDownHover.Visible = false;
                cmbDownLeftLclick.Visible = false; cmbDownLeftRclick.Visible = false; cmbDownLeftHover.Visible = false;
                cmbLeftLclick.Visible = false; cmbLeftRclick.Visible = false; cmbLeftHover.Visible = false;
                cmbUpLeftLclick.Visible = false; cmbUpLeftRclick.Visible = false; cmbUpLeftHover.Visible = false;
                cmbDZUpperLC.Visible = false; cmbDZUpperRC.Visible = false; cmbDZUpperHover.Visible = false;
                cmbDZMiddleLC.Visible = false; cmbDZMiddleRC.Visible = false; cmbDZMiddleHover.Visible = false;
                cmbDZLowerLC.Visible = false; cmbDZLowerRC.Visible = false; cmbDZLowerHover.Visible = false;

                btnUpLclick.Visible = false; btnUpRclick.Visible = false; btnUpHover.Visible = true;
                btnUpRightLclick.Visible = false; btnUpRightRclick.Visible = false; btnUpRightHover.Visible = true;
                btnRightLclick.Visible = false; btnRightRclick.Visible = false; btnRightHover.Visible = true;
                btnDownRightLclick.Visible = false; btnDownRightRclick.Visible = false; btnDownRightHover.Visible = true;
                btnDownLclick.Visible = false; btnDownRclick.Visible = false; btnDownHover.Visible = true;
                btnDownLeftLclick.Visible = false; btnDownLeftRclick.Visible = false; btnDownLeftHover.Visible = true;
                btnLeftLclick.Visible = false; btnLeftRclick.Visible = false; btnLeftHover.Visible = true;
                btnUpLeftLclick.Visible = false; btnUpLeftRclick.Visible = false; btnUpLeftHover.Visible = true;
                btnDZUpperLC.Visible = false; btnDZUpperRC.Visible = false; btnDZUpperHover.Visible = true;
                btnDZMiddleLC.Visible = false; btnDZMiddleRC.Visible = false; btnDZMiddleHover.Visible = true;
                btnDZLowerLC.Visible = false; btnDZLowerRC.Visible = false; btnDZLowerHover.Visible = true;

            }
            else if (chkXbox.Checked)
            {
                btnUpLclick.Visible = false; btnUpRclick.Visible = false; btnUpHover.Visible = false;
                btnUpRightLclick.Visible = false; btnUpRightRclick.Visible = false; btnUpRightHover.Visible = false;
                btnRightLclick.Visible = false; btnRightRclick.Visible = false; btnRightHover.Visible = false;
                btnDownRightLclick.Visible = false; btnDownRightRclick.Visible = false; btnDownRightHover.Visible = false;
                btnDownLclick.Visible = false; btnDownRclick.Visible = false; btnDownHover.Visible = false;
                btnDownLeftLclick.Visible = false; btnDownLeftRclick.Visible = false; btnDownLeftHover.Visible = false;
                btnLeftLclick.Visible = false; btnLeftRclick.Visible = false; btnLeftHover.Visible = false;
                btnUpLeftLclick.Visible = false; btnUpLeftRclick.Visible = false; btnUpLeftHover.Visible = false;
                btnDZUpperLC.Visible = false; btnDZUpperRC.Visible = false; btnDZUpperHover.Visible = false;
                btnDZMiddleLC.Visible = false; btnDZMiddleRC.Visible = false; btnDZMiddleHover.Visible = false;
                btnDZLowerLC.Visible = false; btnDZLowerRC.Visible = false; btnDZLowerHover.Visible = false;

                cmbUpLclick.Visible = false; cmbUpRclick.Visible = false; cmbUpHover.Visible = true;
                cmbUpRightLclick.Visible = false; cmbUpRightRclick.Visible = false; cmbUpRightHover.Visible = true;
                cmbRightLclick.Visible = false; cmbRightRclick.Visible = false; cmbRightHover.Visible = true;
                cmbDownRightLclick.Visible = false; cmbDownRightRclick.Visible = false; cmbDownRightHover.Visible = true;
                cmbDownLclick.Visible = false; cmbDownRclick.Visible = false; cmbDownHover.Visible = true;
                cmbDownLeftLclick.Visible = false; cmbDownLeftRclick.Visible = false; cmbDownLeftHover.Visible = true;
                cmbLeftLclick.Visible = false; cmbLeftRclick.Visible = false; cmbLeftHover.Visible = true;
                cmbUpLeftLclick.Visible = false; cmbUpLeftRclick.Visible = false; cmbUpLeftHover.Visible = true;
                cmbDZUpperLC.Visible = false; cmbDZUpperRC.Visible = false; cmbDZUpperHover.Visible = true;
                cmbDZMiddleLC.Visible = false; cmbDZMiddleRC.Visible = false; cmbDZMiddleHover.Visible = true;
                cmbDZLowerLC.Visible = false; cmbDZLowerRC.Visible = false; cmbDZLowerHover.Visible = true;


            }

            else if (nintendoSwitch.Checked)
            {

                switchUpLC.Visible = false; switchUpRC.Visible = false; switchUpHover.Visible = true;
                switchUpRightLC.Visible = false; switchUpRightRC.Visible = false; switchUpRightHover.Visible = true;
                switchRightLC.Visible = false; switchRightRC.Visible = false; switchRightHover.Visible = true;
                switchDownRightLC.Visible = false; switchDownRightRC.Visible = false; switchDownRightHover.Visible = true;
                switchDownLC.Visible = false; switchDownRC.Visible = false; switchDownHover.Visible = true;
                switchDownLeftLC.Visible = false; switchDownLeftRC.Visible = false; switchDownLeftHover.Visible = true;
                switchLeftLC.Visible = false; switchLeftRC.Visible = false; switchLeftHover.Visible = true;
                switchUpLeftLC.Visible = false; switchUpLeftRC.Visible = false; switchUpLeftHover.Visible = true;
                switchDZUpperLC.Visible = false; switchDZUpperRC.Visible = false; switchDZUpperHover.Visible = true;
                switchDZMiddleLC.Visible = false; switchDZMiddleRC.Visible = false; switchDZMiddleHover.Visible = true;
                switchDZLowerLC.Visible = false; switchDZLowerRC.Visible = false; switchDZLowerHover.Visible = true;

            }

            //chkLabelsHover.Visible = true; chkLabelsLC.Visible = false; chkLabelsRC.Visible = false;


            optUpRight.Items.Remove("Press Up-Right Quadrants");
            //optUpRight.Items.Add("Press Up-Right Quadrants");
            optDownRight.Items.Remove("Press Down-Right Quadrants");
            //optDownRight.Items.Add("Press Down-Right Quadrants");
            optUpLeft.Items.Remove("Press Up-Left Quadrants");
            //optUpLeft.Items.Add("Press Up-Left Quadrants");
            optDownLeft.Items.Remove("Press Down-Left Quadrants");
            //optDownLeft.Items.Add("Press Down-Left Quadrants");
            optUpRight.Items.Remove("Press Up & Right Quadrants");
            optDownRight.Items.Remove("Press Down & Right Quadrants");
            optUpLeft.Items.Remove("Press Up & Left Quadrants");
            optDownLeft.Items.Remove("Press Down & Left Quadrants");


            if (diagUpRightHover.Checked)
            {

                diagUpRightHover.ToolTipText = "Click to Uncombine";
                mergeUpRight.ToolTipText = "Click to Uncombine";
            }
            else
            {
                diagUpRightHover.ToolTipText = "Click to Combine";
                mergeUpRight.ToolTipText = "Click to Combine";
            }

            if (diagUpLeftHover.Checked)
            {
                diagUpLeftHover.ToolTipText = "Click to Uncombine";
                mergeUpLeft.ToolTipText = "Click to Uncombine";
            }
            else
            {
                diagUpLeftHover.ToolTipText = "Click to Combine";
                mergeUpLeft.ToolTipText = "Click to Combine";
            }
            if (diagDownRightHover.Checked)
            {
                diagDownRightHover.ToolTipText = "Click to Uncombine";
                mergeDownRight.ToolTipText = "Click to Uncombine";
            }
            else
            {
                diagDownRightHover.ToolTipText = "Click to Combine";
                mergeDownRight.ToolTipText = "Click to Combine";
            }
            if (diagDownLeftHover.Checked)
            {
                diagDownLeftHover.ToolTipText = "Click to Uncombine";
                mergeDownLeft.ToolTipText = "Click to Uncombine";
            }
            else
            {
                diagDownLeftHover.ToolTipText = "Click to Combine";
                mergeDownLeft.ToolTipText = "Click to Combine";
            }

            actUp.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togUpHover.Checked) ? "T" : "") + ((currentTab == "Hover" && varUpHover.Checked && !chkKeyboard.Checked) ? "V" : "") + ((currentTab == "Hover" && onceUp.Checked) ? "O" : "") + ((currentTab == "LC" && togUpLC.Checked) ? "T" : "") + ((currentTab == "LC" && rtcUpLC.Checked) ? "R" : "") + ((currentTab == "RC" && togUpRC.Checked) ? "T" : "") + ((currentTab == "RC" && rtcUpRC.Checked) ? "R" : "") + ")").Replace(" (Current - )", "");
            actUpRight.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togUpRightHover.Checked) ? "T" : "") + ((currentTab == "Hover" && varUpRightHover.Checked && !chkKeyboard.Checked) ? "V" : "") + ((currentTab == "Hover" && onceUpRight.Checked) ? "O" : "") + ((currentTab == "Hover" && diagUpRightHover.Checked) ? "C" : "") + ((currentTab == "LC" && togUpRightLC.Checked) ? "T" : "") + ((currentTab == "LC" && rtcUpRightLC.Checked) ? "R" : "") + ((currentTab == "LC" && diagUpRightLC.Checked) ? "C" : "") + ((currentTab == "RC" && togUpRightRC.Checked) ? "T" : "") + ((currentTab == "RC" && rtcUpRightRC.Checked) ? "R" : "") + ((currentTab == "RC" && diagUpRightRC.Checked) ? "C" : "") + ")").Replace(" (Current - )", "");
            actRight.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togRightHover.Checked) ? "T" : "") + ((currentTab == "Hover" && varRightHover.Checked && !chkKeyboard.Checked) ? "V" : "") + ((currentTab == "Hover" && onceRight.Checked) ? "O" : "") + ((currentTab == "LC" && togRightLC.Checked) ? "T" : "") + ((currentTab == "LC" && rtcRightLC.Checked) ? "R" : "") + ((currentTab == "RC" && togRightRC.Checked) ? "T" : "") + ((currentTab == "RC" && rtcRightRC.Checked) ? "R" : "") + ")").Replace(" (Current - )", "");
            actDownRight.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togDownRightHover.Checked) ? "T" : "") + ((currentTab == "Hover" && varDownRightHover.Checked && !chkKeyboard.Checked) ? "V" : "") + ((currentTab == "Hover" && onceDownRight.Checked) ? "O" : "") + ((currentTab == "Hover" && diagDownRightHover.Checked) ? "C" : "") + ((currentTab == "LC" && togDownRightLC.Checked) ? "T" : "") + ((currentTab == "LC" && rtcDownRightLC.Checked) ? "R" : "") + ((currentTab == "LC" && diagDownRightLC.Checked) ? "C" : "") + ((currentTab == "RC" && togDownRightRC.Checked) ? "T" : "") + ((currentTab == "RC" && rtcDownRightRC.Checked) ? "R" : "") + ((currentTab == "RC" && diagDownRightRC.Checked) ? "C" : "") + ")").Replace(" (Current - )", "");
            actDown.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togDownHover.Checked) ? "T" : "") + ((currentTab == "Hover" && varDownHover.Checked && !chkKeyboard.Checked) ? "V" : "") + ((currentTab == "Hover" && onceDown.Checked) ? "O" : "") + ((currentTab == "LC" && togDownLC.Checked) ? "T" : "") + ((currentTab == "LC" && rtcDownLC.Checked) ? "R" : "") + ((currentTab == "RC" && togDownRC.Checked) ? "T" : "") + ((currentTab == "RC" && rtcDownRC.Checked) ? "R" : "") + ")").Replace(" (Current - )", "");
            actDownLeft.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togDownLeftHover.Checked) ? "T" : "") + ((currentTab == "Hover" && varDownLeftHover.Checked && !chkKeyboard.Checked) ? "V" : "") + ((currentTab == "Hover" && onceDownLeft.Checked) ? "O" : "") + ((currentTab == "Hover" && diagDownLeftHover.Checked) ? "C" : "") + ((currentTab == "LC" && togDownLeftLC.Checked) ? "T" : "") + ((currentTab == "LC" && rtcDownLeftLC.Checked) ? "R" : "") + ((currentTab == "LC" && diagDownLeftLC.Checked) ? "C" : "") + ((currentTab == "RC" && togDownLeftRC.Checked) ? "T" : "") + ((currentTab == "RC" && rtcDownLeftRC.Checked) ? "R" : "") + ((currentTab == "RC" && diagDownLeftRC.Checked) ? "C" : "") + ")").Replace(" (Current - )", "");
            actLeft.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togLeftHover.Checked) ? "T" : "") + ((currentTab == "Hover" && varLeftHover.Checked && !chkKeyboard.Checked) ? "V" : "") + ((currentTab == "Hover" && onceLeft.Checked) ? "O" : "") + ((currentTab == "LC" && togLeftLC.Checked) ? "T" : "") + ((currentTab == "LC" && rtcLeftLC.Checked) ? "R" : "") + ((currentTab == "RC" && togLeftRC.Checked) ? "T" : "") + ((currentTab == "RC" && rtcLeftRC.Checked) ? "R" : "") + ")").Replace(" (Current - )", "");
            actUpLeft.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togUpLeftHover.Checked) ? "T" : "") + ((currentTab == "Hover" && varUpLeftHover.Checked && !chkKeyboard.Checked) ? "V" : "") + ((currentTab == "Hover" && onceUpLeft.Checked) ? "O" : "") + ((currentTab == "Hover" && diagUpLeftHover.Checked) ? "C" : "") + ((currentTab == "LC" && togUpLeftLC.Checked) ? "T" : "") + ((currentTab == "LC" && rtcUpLeftLC.Checked) ? "R" : "") + ((currentTab == "LC" && diagUpLeftLC.Checked) ? "C" : "") + ((currentTab == "RC" && togUpLeftRC.Checked) ? "T" : "") + ((currentTab == "RC" && rtcUpLeftRC.Checked) ? "R" : "") + ((currentTab == "RC" && diagUpLeftRC.Checked) ? "C" : "") + ")").Replace(" (Current - )", "");
            actDZUpper.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togDZUpperHover.Checked) ? "T" : "") + ((currentTab == "Hover" && onceDZUpper.Checked) ? "O" : "") + ((currentTab == "LC" && togDZUpperLC.Checked) ? "T" : "") + ((currentTab == "RC" && togDZUpperRC.Checked) ? "T" : "") + ")").Replace(" (Current - )", "");
            actDZMiddle.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togDZMiddleHover.Checked) ? "T" : "") + ((currentTab == "Hover" && onceDZMiddle.Checked) ? "O" : "") + ((currentTab == "LC" && togDZMiddleLC.Checked) ? "T" : "") + ((currentTab == "RC" && togDZMiddleRC.Checked) ? "T" : "") + ")").Replace(" (Current - )", "");
            actDZLower.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togDZLowerHover.Checked) ? "T" : "") + ((currentTab == "Hover" && onceDZLower.Checked) ? "O" : "") + ((currentTab == "LC" && togDZLowerLC.Checked) ? "T" : "") + ((currentTab == "RC" && togDZLowerRC.Checked) ? "T" : "") + ")").Replace(" (Current - )", "");



            pnlDeadzone.ResumeLayout();
            pnlQuadrants.ResumeLayout();
            Eval(@"function applyMarquee(el) {  if ((el.dataset.marqueeApplied === 'true' && el.querySelector('.marquee-outer')) || el.querySelector('.truncate')){ return;}

  let text = el.textContent.trim();
  if (!text) return;

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
}, 2000); // 2000 milliseconds = 2 seconds
");
        }



        private void btnLeftClick_Click(object sender, EventArgs e)
        {
            pnlQuadrants.SuspendLayout();
            pnlDeadzone.SuspendLayout();

            currentTab = "LC";
            HideAllSwitchAssignmentControls();

            if (diagUpRightLC.Checked)
            {
                mergeUpRight.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(315) translate(10,5)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#00a7d1' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
            }
            else
            {
                mergeUpRight.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(315) translate(10,5)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#636372' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
            }

            if (diagDownRightLC.Checked)
            {
                mergeDownRight.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(45) translate(10,10)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#00a7d1' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
            }
            else
            {
                mergeDownRight.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(45) translate(10,10)'><path transform='rotate(0) scale(2.9) translate(0,-25)' fill='#636372' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
            }

            if (diagUpLeftLC.Checked)
            {
                mergeUpLeft.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(225) translate(11,11)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#00a7d1' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
            }
            else
            {
                mergeUpLeft.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(225) translate(11,11)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#636372' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
            }


            if (diagDownLeftLC.Checked)
            {
                mergeDownLeft.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(135) translate(11,11)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#00a7d1' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
            }
            else
            {
                mergeDownLeft.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(135) translate(11,11)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#636372' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
            }


            optUpRight.SelectedIndex = 0;
            optRight.SelectedIndex = 0; optDownRight.SelectedIndex = 0; optDown.SelectedIndex = 0;
            optDownLeft.SelectedIndex = 0; optLeft.SelectedIndex = 0; optUpLeft.SelectedIndex = 0; optUp.SelectedIndex = 0;
            optDZUpper.SelectedIndex = 0; optDZMiddle.SelectedIndex = 0; optDZLower.SelectedIndex = 0;


            diagDownLeftHover.Visible = false; diagDownRightHover.Visible = false; diagUpLeftHover.Visible = false; diagUpRightHover.Visible = false;
            diagDownLeftLC.Visible = true; diagDownRightLC.Visible = true; diagUpLeftLC.Visible = true; diagUpRightLC.Visible = true;
            diagDownLeftRC.Visible = false; diagDownRightRC.Visible = false; diagUpLeftRC.Visible = false; diagUpRightRC.Visible = false;

            optUpRight.Items.Remove(optHold); optRight.Items.Remove(optHold); optDownRight.Items.Remove(optHold); optDown.Items.Remove(optHold);
            optDownLeft.Items.Remove(optHold); optLeft.Items.Remove(optHold); optUpLeft.Items.Remove(optHold); optUp.Items.Remove(optHold);
            optDZUpper.Items.Remove(optHold); optDZMiddle.Items.Remove(optHold); optDZLower.Items.Remove(optHold);

            optUpRight.Items.Remove(optOnce); optRight.Items.Remove(optOnce); optDownRight.Items.Remove(optOnce); optDown.Items.Remove(optOnce);
            optDownLeft.Items.Remove(optOnce); optLeft.Items.Remove(optOnce); optUpLeft.Items.Remove(optOnce); optUp.Items.Remove(optOnce);

            optDZUpper.Items.Remove(optOnce); optDZMiddle.Items.Remove(optOnce); optDZLower.Items.Remove(optOnce);

            optUpRight.Items.Remove(optRTC); optRight.Items.Remove(optRTC); optDownRight.Items.Remove(optRTC); optDown.Items.Remove(optRTC);
            optDownLeft.Items.Remove(optRTC); optLeft.Items.Remove(optRTC); optUpLeft.Items.Remove(optRTC); optUp.Items.Remove(optRTC);

            optDZUpper.Items.Remove(optActivate); optDZMiddle.Items.Remove(optActivate); optDZLower.Items.Remove(optActivate);

            disableUpHover.Visible = false; disableUpRightHover.Visible = false; disableRightHover.Visible = false; disableDownRightHover.Visible = false;
            disableDownHover.Visible = false; disableDownLeftHover.Visible = false; disableLeftHover.Visible = false; disableUpLeftHover.Visible = false;

            disableUpLC.Visible = true; disableUpRightLC.Visible = true; disableRightLC.Visible = true; disableDownRightLC.Visible = true;
            disableDownLC.Visible = true; disableDownLeftLC.Visible = true; disableLeftLC.Visible = true; disableUpLeftLC.Visible = true;

            disableUpRC.Visible = false; disableUpRightRC.Visible = false; disableRightRC.Visible = false; disableDownRightRC.Visible = false;
            disableDownRC.Visible = false; disableDownLeftRC.Visible = false; disableLeftRC.Visible = false; disableUpLeftRC.Visible = false;

            disableDZUpperHover.Visible = false; disableDZUpperLC.Visible = true; disableDZUpperRC.Visible = false;

            disableDZMiddleHover.Visible = false; disableDZMiddleLC.Visible = true; disableDZMiddleRC.Visible = false;

            disableDZLowerHover.Visible = false; disableDZLowerLC.Visible = true; disableDZLowerRC.Visible = false;

            optUpRight.Items.Add(optRTC); optRight.Items.Add(optRTC); optDownRight.Items.Add(optRTC); optDown.Items.Add(optRTC);
            optDownLeft.Items.Add(optRTC); optLeft.Items.Add(optRTC); optUpLeft.Items.Add(optRTC); optUp.Items.Add(optRTC);

            optUpRight.Items.Remove(optVariable); optRight.Items.Remove(optVariable); optDownRight.Items.Remove(optVariable); optDown.Items.Remove(optVariable);
            optDownLeft.Items.Remove(optVariable); optLeft.Items.Remove(optVariable); optUpLeft.Items.Remove(optVariable); optUp.Items.Remove(optVariable);

            btnLeftClick.CssStyle = "border:4px solid rgb(0, 167, 209);border-radius:5px;";
            lblLeftClick.CssStyle = "border-bottom:4px solid rgb(240,240,240);font-weight:bold;";
            lblLeftClick.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);

            btnHover.CssStyle = "border:4px solid black;border-radius:5px;";
            lblHover.CssStyle = "border-bottom:4px solid rgb(0, 167, 209);font-weight:normal;";
            lblHover.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);

            btnRightClick.CssStyle = "border:4px solid black;border-radius:5px;";
            lblRightClick.CssStyle = "border-bottom:4px solid rgb(0, 167, 209);font-weight:normal;";
            lblRightClick.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);

            if (!chkXbox.Checked && !nintendoSwitch.Checked)
            {
                cmbUpLclick.Visible = false; cmbUpRclick.Visible = false; cmbUpHover.Visible = false;
                cmbUpRightLclick.Visible = false; cmbUpRightRclick.Visible = false; cmbUpRightHover.Visible = false;
                cmbRightLclick.Visible = false; cmbRightRclick.Visible = false; cmbRightHover.Visible = false;
                cmbDownRightLclick.Visible = false; cmbDownRightRclick.Visible = false; cmbDownRightHover.Visible = false;
                cmbDownLclick.Visible = false; cmbDownRclick.Visible = false; cmbDownHover.Visible = false;
                cmbDownLeftLclick.Visible = false; cmbDownLeftRclick.Visible = false; cmbDownLeftHover.Visible = false;
                cmbLeftLclick.Visible = false; cmbLeftRclick.Visible = false; cmbLeftHover.Visible = false;
                cmbUpLeftLclick.Visible = false; cmbUpLeftRclick.Visible = false; cmbUpLeftHover.Visible = false;
                cmbDZUpperLC.Visible = false; cmbDZUpperRC.Visible = false; cmbDZUpperHover.Visible = false;
                cmbDZMiddleLC.Visible = false; cmbDZMiddleRC.Visible = false; cmbDZMiddleHover.Visible = false;
                cmbDZLowerLC.Visible = false; cmbDZLowerRC.Visible = false; cmbDZLowerHover.Visible = false;

                btnUpLclick.Visible = true; btnUpRclick.Visible = false; btnUpHover.Visible = false;
                btnUpRightLclick.Visible = true; btnUpRightRclick.Visible = false; btnUpRightHover.Visible = false;
                btnRightLclick.Visible = true; btnRightRclick.Visible = false; btnRightHover.Visible = false;
                btnDownRightLclick.Visible = true; btnDownRightRclick.Visible = false; btnDownRightHover.Visible = false;
                btnDownLclick.Visible = true; btnDownRclick.Visible = false; btnDownHover.Visible = false;
                btnDownLeftLclick.Visible = true; btnDownLeftRclick.Visible = false; btnDownLeftHover.Visible = false;
                btnLeftLclick.Visible = true; btnLeftRclick.Visible = false; btnLeftHover.Visible = false;
                btnUpLeftLclick.Visible = true; btnUpLeftRclick.Visible = false; btnUpLeftHover.Visible = false;
                btnDZUpperLC.Visible = true; btnDZUpperRC.Visible = false; btnDZUpperHover.Visible = false;
                btnDZMiddleLC.Visible = true; btnDZMiddleRC.Visible = false; btnDZMiddleHover.Visible = false;
                btnDZLowerLC.Visible = true; btnDZLowerRC.Visible = false; btnDZLowerHover.Visible = false;

            }
            else if (chkXbox.Checked)
            {
                btnUpLclick.Visible = false; btnUpRclick.Visible = false; btnUpHover.Visible = false;
                btnUpRightLclick.Visible = false; btnUpRightRclick.Visible = false; btnUpRightHover.Visible = false;
                btnRightLclick.Visible = false; btnRightRclick.Visible = false; btnRightHover.Visible = false;
                btnDownRightLclick.Visible = false; btnDownRightRclick.Visible = false; btnDownRightHover.Visible = false;
                btnDownLclick.Visible = false; btnDownRclick.Visible = false; btnDownHover.Visible = false;
                btnDownLeftLclick.Visible = false; btnDownLeftRclick.Visible = false; btnDownLeftHover.Visible = false;
                btnLeftLclick.Visible = false; btnLeftRclick.Visible = false; btnLeftHover.Visible = false;
                btnUpLeftLclick.Visible = false; btnUpLeftRclick.Visible = false; btnUpLeftHover.Visible = false;
                btnDZUpperLC.Visible = false; btnDZUpperRC.Visible = false; btnDZUpperHover.Visible = false;
                btnDZMiddleLC.Visible = false; btnDZMiddleRC.Visible = false; btnDZMiddleHover.Visible = false;
                btnDZLowerLC.Visible = false; btnDZLowerRC.Visible = false; btnDZLowerHover.Visible = false;

                cmbUpLclick.Visible = true; cmbUpRclick.Visible = false; cmbUpHover.Visible = false;
                cmbUpRightLclick.Visible = true; cmbUpRightRclick.Visible = false; cmbUpRightHover.Visible = false;
                cmbRightLclick.Visible = true; cmbRightRclick.Visible = false; cmbRightHover.Visible = false;
                cmbDownRightLclick.Visible = true; cmbDownRightRclick.Visible = false; cmbDownRightHover.Visible = false;
                cmbDownLclick.Visible = true; cmbDownRclick.Visible = false; cmbDownHover.Visible = false;
                cmbDownLeftLclick.Visible = true; cmbDownLeftRclick.Visible = false; cmbDownLeftHover.Visible = false;
                cmbLeftLclick.Visible = true; cmbLeftRclick.Visible = false; cmbLeftHover.Visible = false;
                cmbUpLeftLclick.Visible = true; cmbUpLeftRclick.Visible = false; cmbUpLeftHover.Visible = false;
                cmbDZUpperLC.Visible = true; cmbDZUpperRC.Visible = false; cmbDZUpperHover.Visible = false;
                cmbDZMiddleLC.Visible = true; cmbDZMiddleRC.Visible = false; cmbDZMiddleHover.Visible = false;
                cmbDZLowerLC.Visible = true; cmbDZLowerRC.Visible = false; cmbDZLowerHover.Visible = false;
            }

            else if (nintendoSwitch.Checked)
            {

                switchUpLC.Visible = true; switchUpRC.Visible = false; switchUpHover.Visible = false;
                switchUpRightLC.Visible = true; switchUpRightRC.Visible = false; switchUpRightHover.Visible = false;
                switchRightLC.Visible = true; switchRightRC.Visible = false; switchRightHover.Visible = false;
                switchDownRightLC.Visible = true; switchDownRightRC.Visible = false; switchDownRightHover.Visible = false;
                switchDownLC.Visible = true; switchDownRC.Visible = false; switchDownHover.Visible = false;
                switchDownLeftLC.Visible = true; switchDownLeftRC.Visible = false; switchDownLeftHover.Visible = false;
                switchLeftLC.Visible = true; switchLeftRC.Visible = false; switchLeftHover.Visible = false;
                switchUpLeftLC.Visible = true; switchUpLeftRC.Visible = false; switchUpLeftHover.Visible = false;
                switchDZUpperLC.Visible = true; switchDZUpperRC.Visible = false; switchDZUpperHover.Visible = false;
                switchDZMiddleLC.Visible = true; switchDZMiddleRC.Visible = false; switchDZMiddleHover.Visible = false;
                switchDZLowerLC.Visible = true; switchDZLowerRC.Visible = false; switchDZLowerHover.Visible = false;

            }

            //chkLabelsHover.Visible = false; chkLabelsLC.Visible = true; chkLabelsRC.Visible = false;





            optUpRight.Items.Remove("Press Up-Right Quadrants");
            //optUpRight.Items.Add("Press Up-Right Quadrants");
            optDownRight.Items.Remove("Press Down-Right Quadrants");
            optUpLeft.Items.Remove("Press Up-Left Quadrants");
            //optUpLeft.Items.Add("Press Up-Left Quadrants");
            optDownLeft.Items.Remove("Press Down-Left Quadrants");
            //optDownLeft.Items.Add("Press Down-Left Quadrants");
            optUpRight.Items.Remove("Press Up & Right Quadrants");
            optDownRight.Items.Remove("Press Down & Right Quadrants");
            optUpLeft.Items.Remove("Press Up & Left Quadrants");
            optDownLeft.Items.Remove("Press Down & Left Quadrants");


            if (diagUpRightLC.Checked)
            {
                diagUpRightLC.ToolTipText = "Click to Uncombine";
                mergeUpRight.ToolTipText = "Click to Uncombine";
            }
            else
            {
                diagUpRightLC.ToolTipText = "Click to Combine";
                mergeUpRight.ToolTipText = "Click to Combine";
            }
            if (diagUpLeftLC.Checked)
            {
                diagUpLeftLC.ToolTipText = "Click to Uncombine";
                mergeUpLeft.ToolTipText = "Click to Uncombine";
            }
            else
            {
                diagUpLeftLC.ToolTipText = "Click to Combine";
                mergeUpLeft.ToolTipText = "Click to Combine";
            }
            if (diagDownRightLC.Checked)
            {
                diagDownRightLC.ToolTipText = "Click to Uncombine";
                mergeDownRight.ToolTipText = "Click to Uncombine";
            }
            else
            {
                diagDownRightLC.ToolTipText = "Click to Combine";
                mergeDownRight.ToolTipText = "Click to Combine";
            }
            if (diagDownLeftLC.Checked)
            {
                diagDownLeftLC.ToolTipText = "Click to Uncombine";
                mergeDownLeft.ToolTipText = "Click to Uncombine";
            }
            else
            {
                diagDownLeftLC.ToolTipText = "Click to Combine";
                mergeDownLeft.ToolTipText = "Click to Combine";
            }

            actUp.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togUpHover.Checked) ? "T" : "") + ((currentTab == "Hover" && varUpHover.Checked && !chkKeyboard.Checked) ? "V" : "") + ((currentTab == "Hover" && onceUp.Checked) ? "O" : "") + ((currentTab == "LC" && togUpLC.Checked) ? "T" : "") + ((currentTab == "LC" && rtcUpLC.Checked) ? "R" : "") + ((currentTab == "RC" && togUpRC.Checked) ? "T" : "") + ((currentTab == "RC" && rtcUpRC.Checked) ? "R" : "") + ")").Replace(" (Current - )", "");
            actUpRight.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togUpRightHover.Checked) ? "T" : "") + ((currentTab == "Hover" && varUpRightHover.Checked && !chkKeyboard.Checked) ? "V" : "") + ((currentTab == "Hover" && onceUpRight.Checked) ? "O" : "") + ((currentTab == "Hover" && diagUpRightHover.Checked) ? "C" : "") + ((currentTab == "LC" && togUpRightLC.Checked) ? "T" : "") + ((currentTab == "LC" && rtcUpRightLC.Checked) ? "R" : "") + ((currentTab == "LC" && diagUpRightLC.Checked) ? "C" : "") + ((currentTab == "RC" && togUpRightRC.Checked) ? "T" : "") + ((currentTab == "RC" && rtcUpRightRC.Checked) ? "R" : "") + ((currentTab == "RC" && diagUpRightRC.Checked) ? "C" : "") + ")").Replace(" (Current - )", "");
            actRight.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togRightHover.Checked) ? "T" : "") + ((currentTab == "Hover" && varRightHover.Checked && !chkKeyboard.Checked) ? "V" : "") + ((currentTab == "Hover" && onceRight.Checked) ? "O" : "") + ((currentTab == "LC" && togRightLC.Checked) ? "T" : "") + ((currentTab == "LC" && rtcRightLC.Checked) ? "R" : "") + ((currentTab == "RC" && togRightRC.Checked) ? "T" : "") + ((currentTab == "RC" && rtcRightRC.Checked) ? "R" : "") + ")").Replace(" (Current - )", "");
            actDownRight.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togDownRightHover.Checked) ? "T" : "") + ((currentTab == "Hover" && varDownRightHover.Checked && !chkKeyboard.Checked) ? "V" : "") + ((currentTab == "Hover" && onceDownRight.Checked) ? "O" : "") + ((currentTab == "Hover" && diagDownRightHover.Checked) ? "C" : "") + ((currentTab == "LC" && togDownRightLC.Checked) ? "T" : "") + ((currentTab == "LC" && rtcDownRightLC.Checked) ? "R" : "") + ((currentTab == "LC" && diagDownRightLC.Checked) ? "C" : "") + ((currentTab == "RC" && togDownRightRC.Checked) ? "T" : "") + ((currentTab == "RC" && rtcDownRightRC.Checked) ? "R" : "") + ((currentTab == "RC" && diagDownRightRC.Checked) ? "C" : "") + ")").Replace(" (Current - )", "");
            actDown.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togDownHover.Checked) ? "T" : "") + ((currentTab == "Hover" && varDownHover.Checked && !chkKeyboard.Checked) ? "V" : "") + ((currentTab == "Hover" && onceDown.Checked) ? "O" : "") + ((currentTab == "LC" && togDownLC.Checked) ? "T" : "") + ((currentTab == "LC" && rtcDownLC.Checked) ? "R" : "") + ((currentTab == "RC" && togDownRC.Checked) ? "T" : "") + ((currentTab == "RC" && rtcDownRC.Checked) ? "R" : "") + ")").Replace(" (Current - )", "");
            actDownLeft.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togDownLeftHover.Checked) ? "T" : "") + ((currentTab == "Hover" && varDownLeftHover.Checked && !chkKeyboard.Checked) ? "V" : "") + ((currentTab == "Hover" && onceDownLeft.Checked) ? "O" : "") + ((currentTab == "Hover" && diagDownLeftHover.Checked) ? "C" : "") + ((currentTab == "LC" && togDownLeftLC.Checked) ? "T" : "") + ((currentTab == "LC" && rtcDownLeftLC.Checked) ? "R" : "") + ((currentTab == "LC" && diagDownLeftLC.Checked) ? "C" : "") + ((currentTab == "RC" && togDownLeftRC.Checked) ? "T" : "") + ((currentTab == "RC" && rtcDownLeftRC.Checked) ? "R" : "") + ((currentTab == "RC" && diagDownLeftRC.Checked) ? "C" : "") + ")").Replace(" (Current - )", "");
            actLeft.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togLeftHover.Checked) ? "T" : "") + ((currentTab == "Hover" && varLeftHover.Checked && !chkKeyboard.Checked) ? "V" : "") + ((currentTab == "Hover" && onceLeft.Checked) ? "O" : "") + ((currentTab == "LC" && togLeftLC.Checked) ? "T" : "") + ((currentTab == "LC" && rtcLeftLC.Checked) ? "R" : "") + ((currentTab == "RC" && togLeftRC.Checked) ? "T" : "") + ((currentTab == "RC" && rtcLeftRC.Checked) ? "R" : "") + ")").Replace(" (Current - )", "");
            actUpLeft.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togUpLeftHover.Checked) ? "T" : "") + ((currentTab == "Hover" && varUpLeftHover.Checked && !chkKeyboard.Checked) ? "V" : "") + ((currentTab == "Hover" && onceUpLeft.Checked) ? "O" : "") + ((currentTab == "Hover" && diagUpLeftHover.Checked) ? "C" : "") + ((currentTab == "LC" && togUpLeftLC.Checked) ? "T" : "") + ((currentTab == "LC" && rtcUpLeftLC.Checked) ? "R" : "") + ((currentTab == "LC" && diagUpLeftLC.Checked) ? "C" : "") + ((currentTab == "RC" && togUpLeftRC.Checked) ? "T" : "") + ((currentTab == "RC" && rtcUpLeftRC.Checked) ? "R" : "") + ((currentTab == "RC" && diagUpLeftRC.Checked) ? "C" : "") + ")").Replace(" (Current - )", "");
            actDZUpper.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togDZUpperHover.Checked) ? "T" : "") + ((currentTab == "Hover" && onceDZUpper.Checked) ? "O" : "") + ((currentTab == "LC" && togDZUpperLC.Checked) ? "T" : "") + ((currentTab == "RC" && togDZUpperRC.Checked) ? "T" : "") + ")").Replace(" (Current - )", "");
            actDZMiddle.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togDZMiddleHover.Checked) ? "T" : "") + ((currentTab == "Hover" && onceDZMiddle.Checked) ? "O" : "") + ((currentTab == "LC" && togDZMiddleLC.Checked) ? "T" : "") + ((currentTab == "RC" && togDZMiddleRC.Checked) ? "T" : "") + ")").Replace(" (Current - )", "");
            actDZLower.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togDZLowerHover.Checked) ? "T" : "") + ((currentTab == "Hover" && onceDZLower.Checked) ? "O" : "") + ((currentTab == "LC" && togDZLowerLC.Checked) ? "T" : "") + ((currentTab == "RC" && togDZLowerRC.Checked) ? "T" : "") + ")").Replace(" (Current - )", "");



            pnlDeadzone.ResumeLayout();
            pnlQuadrants.ResumeLayout();
            Eval(@"function applyMarquee(el) {  if ((el.dataset.marqueeApplied === 'true' && el.querySelector('.marquee-outer')) || el.querySelector('.truncate')){ return;}

  let text = el.textContent.trim();
  if (!text) return;

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
}, 2000); // 2000 milliseconds = 2 seconds
");
        }

        private void btnRightClick_Click(object sender, EventArgs e)
        {
            pnlQuadrants.SuspendLayout();
            pnlDeadzone.SuspendLayout();

            currentTab = "RC";
            HideAllSwitchAssignmentControls();

            if (diagUpRightRC.Checked)
            {
                mergeUpRight.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(315) translate(10,5)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#00a7d1' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
            }
            else
            {
                mergeUpRight.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(315) translate(10,5)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#636372' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
            }

            if (diagDownRightRC.Checked)
            {
                mergeDownRight.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(45) translate(10,10)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#00a7d1' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
            }
            else
            {
                mergeDownRight.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(45) translate(10,10)'><path transform='rotate(0) scale(2.9) translate(0,-25)' fill='#636372' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
            }

            if (diagUpLeftRC.Checked)
            {
                mergeUpLeft.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(225) translate(11,11)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#00a7d1' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
            }
            else
            {
                mergeUpLeft.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(225) translate(11,11)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#636372' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
            }


            if (diagDownLeftRC.Checked)
            {
                mergeDownLeft.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(135) translate(11,11)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#00a7d1' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
            }
            else
            {
                mergeDownLeft.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(135) translate(11,11)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#636372' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
            }

            /* optUpRight.SelectedIndex = 0;*/
            optRight.SelectedIndex = 0; optDownRight.SelectedIndex = 0; optDown.SelectedIndex = 0;
            optDownLeft.SelectedIndex = 0; optLeft.SelectedIndex = 0; optUpLeft.SelectedIndex = 0; optUp.SelectedIndex = 0;
            optDZUpper.SelectedIndex = 0; optDZMiddle.SelectedIndex = 0; optDZLower.SelectedIndex = 0;

            diagDownLeftHover.Visible = false; diagDownRightHover.Visible = false; diagUpLeftHover.Visible = false; diagUpRightHover.Visible = false;
            diagDownLeftLC.Visible = false; diagDownRightLC.Visible = false; diagUpLeftLC.Visible = false; diagUpRightLC.Visible = false;
            diagDownLeftRC.Visible = true; diagDownRightRC.Visible = true; diagUpLeftRC.Visible = true; diagUpRightRC.Visible = true;

            optUpRight.Items.Remove(optHold); optRight.Items.Remove(optHold); optDownRight.Items.Remove(optHold); optDown.Items.Remove(optHold);
            optDownLeft.Items.Remove(optHold); optLeft.Items.Remove(optHold); optUpLeft.Items.Remove(optHold); optUp.Items.Remove(optHold);
            optDZUpper.Items.Remove(optHold); optDZMiddle.Items.Remove(optHold); optDZLower.Items.Remove(optHold);


            optUpRight.Items.Remove(optOnce); optRight.Items.Remove(optOnce); optDownRight.Items.Remove(optOnce); optDown.Items.Remove(optOnce);
            optDownLeft.Items.Remove(optOnce); optLeft.Items.Remove(optOnce); optUpLeft.Items.Remove(optOnce); optUp.Items.Remove(optOnce);

            optDZUpper.Items.Remove(optOnce); optDZMiddle.Items.Remove(optOnce); optDZLower.Items.Remove(optOnce);

            optUpRight.Items.Remove(optRTC); optRight.Items.Remove(optRTC); optDownRight.Items.Remove(optRTC); optDown.Items.Remove(optRTC);
            optDownLeft.Items.Remove(optRTC); optLeft.Items.Remove(optRTC); optUpLeft.Items.Remove(optRTC); optUp.Items.Remove(optRTC);

            optDZUpper.Items.Remove(optActivate); optDZMiddle.Items.Remove(optActivate); optDZLower.Items.Remove(optActivate);

            disableUpHover.Visible = false; disableUpRightHover.Visible = false; disableRightHover.Visible = false; disableDownRightHover.Visible = false;
            disableDownHover.Visible = false; disableDownLeftHover.Visible = false; disableLeftHover.Visible = false; disableUpLeftHover.Visible = false;

            disableUpLC.Visible = false; disableUpRightLC.Visible = false; disableRightLC.Visible = false; disableDownRightLC.Visible = false;
            disableDownLC.Visible = false; disableDownLeftLC.Visible = false; disableLeftLC.Visible = false; disableUpLeftLC.Visible = false;

            disableUpRC.Visible = true; disableUpRightRC.Visible = true; disableRightRC.Visible = true; disableDownRightRC.Visible = true;
            disableDownRC.Visible = true; disableDownLeftRC.Visible = true; disableLeftRC.Visible = true; disableUpLeftRC.Visible = true;

            disableDZUpperHover.Visible = false; disableDZUpperLC.Visible = false; disableDZUpperRC.Visible = true;

            disableDZMiddleHover.Visible = false; disableDZMiddleLC.Visible = false; disableDZMiddleRC.Visible = true;

            disableDZLowerHover.Visible = false; disableDZLowerLC.Visible = false; disableDZLowerRC.Visible = true;

            //if (nintendoSwitch.Checked)
            //{

            //    optUpRight.Items.Add(optHold); optRight.Items.Add(optHold); optDownRight.Items.Add(optHold); optDown.Items.Add(optHold);
            //    optDownLeft.Items.Add(optHold); optLeft.Items.Add(optHold); optUpLeft.Items.Add(optHold); optUp.Items.Add(optHold);
            //    optDZUpper.Items.Add(optHold); optDZMiddle.Items.Add(optHold); optDZLower.Items.Add(optHold);

            //}

            optUpRight.Items.Add(optRTC); optRight.Items.Add(optRTC); optDownRight.Items.Add(optRTC); optDown.Items.Add(optRTC);
            optDownLeft.Items.Add(optRTC); optLeft.Items.Add(optRTC); optUpLeft.Items.Add(optRTC); optUp.Items.Add(optRTC);

            optUpRight.Items.Remove(optVariable); optRight.Items.Remove(optVariable); optDownRight.Items.Remove(optVariable); optDown.Items.Remove(optVariable);
            optDownLeft.Items.Remove(optVariable); optLeft.Items.Remove(optVariable); optUpLeft.Items.Remove(optVariable); optUp.Items.Remove(optVariable);

            btnRightClick.CssStyle = "border:4px solid rgb(0, 167, 209);border-radius:5px;";
            lblRightClick.CssStyle = "border-bottom:4px solid rgb(240,240,240);font-weight:bold;";
            lblRightClick.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);

            btnLeftClick.CssStyle = "border:4px solid black;border-radius:5px;";
            lblLeftClick.CssStyle = "border-bottom:4px solid rgb(0, 167, 209);font-weight:normal;";
            lblLeftClick.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);

            btnHover.CssStyle = "border:4px solid black;border-radius:5px;";
            lblHover.CssStyle = "border-bottom:4px solid rgb(0, 167, 209);font-weight:normal;";
            lblHover.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);

            if (!chkXbox.Checked && !nintendoSwitch.Checked)
            {
                cmbUpLclick.Visible = false; cmbUpRclick.Visible = false; cmbUpHover.Visible = false;
                cmbUpRightLclick.Visible = false; cmbUpRightRclick.Visible = false; cmbUpRightHover.Visible = false;
                cmbRightLclick.Visible = false; cmbRightRclick.Visible = false; cmbRightHover.Visible = false;
                cmbDownRightLclick.Visible = false; cmbDownRightRclick.Visible = false; cmbDownRightHover.Visible = false;
                cmbDownLclick.Visible = false; cmbDownRclick.Visible = false; cmbDownHover.Visible = false;
                cmbDownLeftLclick.Visible = false; cmbDownLeftRclick.Visible = false; cmbDownLeftHover.Visible = false;
                cmbLeftLclick.Visible = false; cmbLeftRclick.Visible = false; cmbLeftHover.Visible = false;
                cmbUpLeftLclick.Visible = false; cmbUpLeftRclick.Visible = false; cmbUpLeftHover.Visible = false;
                cmbDZUpperLC.Visible = false; cmbDZUpperRC.Visible = false; cmbDZUpperHover.Visible = false;
                cmbDZMiddleLC.Visible = false; cmbDZMiddleRC.Visible = false; cmbDZMiddleHover.Visible = false;
                cmbDZLowerLC.Visible = false; cmbDZLowerRC.Visible = false; cmbDZLowerHover.Visible = false;

                btnUpLclick.Visible = false; btnUpRclick.Visible = true; btnUpHover.Visible = false;
                btnUpRightLclick.Visible = false; btnUpRightRclick.Visible = true; btnUpRightHover.Visible = false;
                btnRightLclick.Visible = false; btnRightRclick.Visible = true; btnRightHover.Visible = false;
                btnDownRightLclick.Visible = false; btnDownRightRclick.Visible = true; btnDownRightHover.Visible = false;
                btnDownLclick.Visible = false; btnDownRclick.Visible = true; btnDownHover.Visible = false;
                btnDownLeftLclick.Visible = false; btnDownLeftRclick.Visible = true; btnDownLeftHover.Visible = false;
                btnLeftLclick.Visible = false; btnLeftRclick.Visible = true; btnLeftHover.Visible = false;
                btnUpLeftLclick.Visible = false; btnUpLeftRclick.Visible = true; btnUpLeftHover.Visible = false;
                btnDZUpperLC.Visible = false; btnDZUpperRC.Visible = true; btnDZUpperHover.Visible = false;
                btnDZMiddleLC.Visible = false; btnDZMiddleRC.Visible = true; btnDZMiddleHover.Visible = false;
                btnDZLowerLC.Visible = false; btnDZLowerRC.Visible = true; btnDZLowerHover.Visible = false;

            }
            else if (chkXbox.Checked)
            {
                btnUpLclick.Visible = false; btnUpRclick.Visible = false; btnUpHover.Visible = false;
                btnUpRightLclick.Visible = false; btnUpRightRclick.Visible = false; btnUpRightHover.Visible = false;
                btnRightLclick.Visible = false; btnRightRclick.Visible = false; btnRightHover.Visible = false;
                btnDownRightLclick.Visible = false; btnDownRightRclick.Visible = false; btnDownRightHover.Visible = false;
                btnDownLclick.Visible = false; btnDownRclick.Visible = false; btnDownHover.Visible = false;
                btnDownLeftLclick.Visible = false; btnDownLeftRclick.Visible = false; btnDownLeftHover.Visible = false;
                btnLeftLclick.Visible = false; btnLeftRclick.Visible = false; btnLeftHover.Visible = false;
                btnUpLeftLclick.Visible = false; btnUpLeftRclick.Visible = false; btnUpLeftHover.Visible = false;
                btnDZUpperLC.Visible = false; btnDZUpperRC.Visible = false; btnDZUpperHover.Visible = false;
                btnDZMiddleLC.Visible = false; btnDZMiddleRC.Visible = false; btnDZMiddleHover.Visible = false;
                btnDZLowerLC.Visible = false; btnDZLowerRC.Visible = false; btnDZLowerHover.Visible = false;

                cmbUpLclick.Visible = false; cmbUpRclick.Visible = true; cmbUpHover.Visible = false;
                cmbUpRightLclick.Visible = false; cmbUpRightRclick.Visible = true; cmbUpRightHover.Visible = false;
                cmbRightLclick.Visible = false; cmbRightRclick.Visible = true; cmbRightHover.Visible = false;
                cmbDownRightLclick.Visible = false; cmbDownRightRclick.Visible = true; cmbDownRightHover.Visible = false;
                cmbDownLclick.Visible = false; cmbDownRclick.Visible = true; cmbDownHover.Visible = false;
                cmbDownLeftLclick.Visible = false; cmbDownLeftRclick.Visible = true; cmbDownLeftHover.Visible = false;
                cmbLeftLclick.Visible = false; cmbLeftRclick.Visible = true; cmbLeftHover.Visible = false;
                cmbUpLeftLclick.Visible = false; cmbUpLeftRclick.Visible = true; cmbUpLeftHover.Visible = false;
                cmbDZUpperLC.Visible = false; cmbDZUpperRC.Visible = true; cmbDZUpperHover.Visible = false;
                cmbDZMiddleLC.Visible = false; cmbDZMiddleRC.Visible = true; cmbDZMiddleHover.Visible = false;
                cmbDZLowerLC.Visible = false; cmbDZLowerRC.Visible = true; cmbDZLowerHover.Visible = false;

            }

            else if (nintendoSwitch.Checked)
            {

                switchUpRC.Visible = true; switchUpLC.Visible = false; switchUpHover.Visible = false;
                switchUpRightRC.Visible = true; switchUpRightLC.Visible = false; switchUpRightHover.Visible = false;
                switchRightRC.Visible = true; switchRightLC.Visible = false; switchRightHover.Visible = false;
                switchDownRightRC.Visible = true; switchDownRightLC.Visible = false; switchDownRightHover.Visible = false;
                switchDownRC.Visible = true; switchDownLC.Visible = false; switchDownHover.Visible = false;
                switchDownLeftRC.Visible = true; switchDownLeftLC.Visible = false; switchDownLeftHover.Visible = false;
                switchLeftRC.Visible = true; switchLeftLC.Visible = false; switchLeftHover.Visible = false;
                switchUpLeftRC.Visible = true; switchUpLeftLC.Visible = false; switchUpLeftHover.Visible = false;
                switchDZUpperRC.Visible = true; switchDZUpperLC.Visible = false; switchDZUpperHover.Visible = false;
                switchDZMiddleRC.Visible = true; switchDZMiddleLC.Visible = false; switchDZMiddleHover.Visible = false;
                switchDZLowerRC.Visible = true; switchDZLowerLC.Visible = false; switchDZLowerHover.Visible = false;

            }

            //chkLabelsHover.Visible = false; chkLabelsLC.Visible = false; chkLabelsRC.Visible = true;

            optUpRight.Items.Remove("Press Up-Right Quadrants");
            //optUpRight.Items.Add("Press Up-Right Quadrants");
            optDownRight.Items.Remove("Press Down-Right Quadrants");
            optUpLeft.Items.Remove("Press Up-Left Quadrants");
            //optUpLeft.Items.Add("Press Up-Left Quadrants");
            optDownLeft.Items.Remove("Press Down-Left Quadrants");
            //optDownLeft.Items.Add("Press Down-Left Quadrants");

            optUpRight.Items.Remove("Press Up & Right Quadrants");
            optDownRight.Items.Remove("Press Down & Right Quadrants");
            optUpLeft.Items.Remove("Press Up & Left Quadrants");
            optDownLeft.Items.Remove("Press Down & Left Quadrants");

            if (diagUpRightRC.Checked)
            {
                diagUpRightRC.ToolTipText = "Click to Uncombine";
                mergeUpRight.ToolTipText = "Click to Uncombine";
            }
            else
            {
                diagUpRightRC.ToolTipText = "Click to Combine";
                mergeUpRight.ToolTipText = "Click to Combine";
            }
            if (diagUpLeftRC.Checked)
            {
                diagUpLeftRC.ToolTipText = "Click to Uncombine";
                mergeUpLeft.ToolTipText = "Click to Uncombine";
            }
            else
            {
                diagUpLeftRC.ToolTipText = "Click to Combine";
                mergeUpLeft.ToolTipText = "Click to Combine";
            }
            if (diagDownRightRC.Checked)
            {
                diagDownRightRC.ToolTipText = "Click to Uncombine";
                mergeDownRight.ToolTipText = "Click to Uncombine";
            }
            else
            {
                diagDownRightRC.ToolTipText = "Click to Combine";
                mergeDownRight.ToolTipText = "Click to Combine";
            }
            if (diagDownLeftRC.Checked)
            {
                diagDownLeftRC.ToolTipText = "Click to Uncombine";
                mergeDownLeft.ToolTipText = "Click to Uncombine";
            }
            else
            {
                diagDownLeftRC.ToolTipText = "Click to Combine";
                mergeDownLeft.ToolTipText = "Click to Combine";
            }

            actUp.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togUpHover.Checked) ? "T" : "") + ((currentTab == "Hover" && varUpHover.Checked && !chkKeyboard.Checked) ? "V" : "") + ((currentTab == "Hover" && onceUp.Checked) ? "O" : "") + ((currentTab == "LC" && togUpLC.Checked) ? "T" : "") + ((currentTab == "LC" && rtcUpLC.Checked) ? "R" : "") + ((currentTab == "RC" && togUpRC.Checked) ? "T" : "") + ((currentTab == "RC" && rtcUpRC.Checked) ? "R" : "") + ")").Replace(" (Current - )", "");
            actUpRight.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togUpRightHover.Checked) ? "T" : "") + ((currentTab == "Hover" && varUpRightHover.Checked && !chkKeyboard.Checked) ? "V" : "") + ((currentTab == "Hover" && onceUpRight.Checked) ? "O" : "") + ((currentTab == "Hover" && diagUpRightHover.Checked) ? "C" : "") + ((currentTab == "LC" && togUpRightLC.Checked) ? "T" : "") + ((currentTab == "LC" && rtcUpRightLC.Checked) ? "R" : "") + ((currentTab == "LC" && diagUpRightLC.Checked) ? "C" : "") + ((currentTab == "RC" && togUpRightRC.Checked) ? "T" : "") + ((currentTab == "RC" && rtcUpRightRC.Checked) ? "R" : "") + ((currentTab == "RC" && diagUpRightRC.Checked) ? "C" : "") + ")").Replace(" (Current - )", "");
            actRight.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togRightHover.Checked) ? "T" : "") + ((currentTab == "Hover" && varRightHover.Checked && !chkKeyboard.Checked) ? "V" : "") + ((currentTab == "Hover" && onceRight.Checked) ? "O" : "") + ((currentTab == "LC" && togRightLC.Checked) ? "T" : "") + ((currentTab == "LC" && rtcRightLC.Checked) ? "R" : "") + ((currentTab == "RC" && togRightRC.Checked) ? "T" : "") + ((currentTab == "RC" && rtcRightRC.Checked) ? "R" : "") + ")").Replace(" (Current - )", "");
            actDownRight.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togDownRightHover.Checked) ? "T" : "") + ((currentTab == "Hover" && varDownRightHover.Checked && !chkKeyboard.Checked) ? "V" : "") + ((currentTab == "Hover" && onceDownRight.Checked) ? "O" : "") + ((currentTab == "Hover" && diagDownRightHover.Checked) ? "C" : "") + ((currentTab == "LC" && togDownRightLC.Checked) ? "T" : "") + ((currentTab == "LC" && rtcDownRightLC.Checked) ? "R" : "") + ((currentTab == "LC" && diagDownRightLC.Checked) ? "C" : "") + ((currentTab == "RC" && togDownRightRC.Checked) ? "T" : "") + ((currentTab == "RC" && rtcDownRightRC.Checked) ? "R" : "") + ((currentTab == "RC" && diagDownRightRC.Checked) ? "C" : "") + ")").Replace(" (Current - )", "");
            actDown.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togDownHover.Checked) ? "T" : "") + ((currentTab == "Hover" && varDownHover.Checked && !chkKeyboard.Checked) ? "V" : "") + ((currentTab == "Hover" && onceDown.Checked) ? "O" : "") + ((currentTab == "LC" && togDownLC.Checked) ? "T" : "") + ((currentTab == "LC" && rtcDownLC.Checked) ? "R" : "") + ((currentTab == "RC" && togDownRC.Checked) ? "T" : "") + ((currentTab == "RC" && rtcDownRC.Checked) ? "R" : "") + ")").Replace(" (Current - )", "");
            actDownLeft.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togDownLeftHover.Checked) ? "T" : "") + ((currentTab == "Hover" && varDownLeftHover.Checked && !chkKeyboard.Checked) ? "V" : "") + ((currentTab == "Hover" && onceDownLeft.Checked) ? "O" : "") + ((currentTab == "Hover" && diagDownLeftHover.Checked) ? "C" : "") + ((currentTab == "LC" && togDownLeftLC.Checked) ? "T" : "") + ((currentTab == "LC" && rtcDownLeftLC.Checked) ? "R" : "") + ((currentTab == "LC" && diagDownLeftLC.Checked) ? "C" : "") + ((currentTab == "RC" && togDownLeftRC.Checked) ? "T" : "") + ((currentTab == "RC" && rtcDownLeftRC.Checked) ? "R" : "") + ((currentTab == "RC" && diagDownLeftRC.Checked) ? "C" : "") + ")").Replace(" (Current - )", "");
            actLeft.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togLeftHover.Checked) ? "T" : "") + ((currentTab == "Hover" && varLeftHover.Checked && !chkKeyboard.Checked) ? "V" : "") + ((currentTab == "Hover" && onceLeft.Checked) ? "O" : "") + ((currentTab == "LC" && togLeftLC.Checked) ? "T" : "") + ((currentTab == "LC" && rtcLeftLC.Checked) ? "R" : "") + ((currentTab == "RC" && togLeftRC.Checked) ? "T" : "") + ((currentTab == "RC" && rtcLeftRC.Checked) ? "R" : "") + ")").Replace(" (Current - )", "");
            actUpLeft.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togUpLeftHover.Checked) ? "T" : "") + ((currentTab == "Hover" && varUpLeftHover.Checked && !chkKeyboard.Checked) ? "V" : "") + ((currentTab == "Hover" && onceUpLeft.Checked) ? "O" : "") + ((currentTab == "Hover" && diagUpLeftHover.Checked) ? "C" : "") + ((currentTab == "LC" && togUpLeftLC.Checked) ? "T" : "") + ((currentTab == "LC" && rtcUpLeftLC.Checked) ? "R" : "") + ((currentTab == "LC" && diagUpLeftLC.Checked) ? "C" : "") + ((currentTab == "RC" && togUpLeftRC.Checked) ? "T" : "") + ((currentTab == "RC" && rtcUpLeftRC.Checked) ? "R" : "") + ((currentTab == "RC" && diagUpLeftRC.Checked) ? "C" : "") + ")").Replace(" (Current - )", "");
            actDZUpper.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togDZUpperHover.Checked) ? "T" : "") + ((currentTab == "Hover" && onceDZUpper.Checked) ? "O" : "") + ((currentTab == "LC" && togDZUpperLC.Checked) ? "T" : "") + ((currentTab == "RC" && togDZUpperRC.Checked) ? "T" : "") + ")").Replace(" (Current - )", "");
            actDZMiddle.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togDZMiddleHover.Checked) ? "T" : "") + ((currentTab == "Hover" && onceDZMiddle.Checked) ? "O" : "") + ((currentTab == "LC" && togDZMiddleLC.Checked) ? "T" : "") + ((currentTab == "RC" && togDZMiddleRC.Checked) ? "T" : "") + ")").Replace(" (Current - )", "");
            actDZLower.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togDZLowerHover.Checked) ? "T" : "") + ((currentTab == "Hover" && onceDZLower.Checked) ? "O" : "") + ((currentTab == "LC" && togDZLowerLC.Checked) ? "T" : "") + ((currentTab == "RC" && togDZLowerRC.Checked) ? "T" : "") + ")").Replace(" (Current - )", "");



            pnlDeadzone.ResumeLayout();
            pnlQuadrants.ResumeLayout();
            Eval(@"function applyMarquee(el) {  if ((el.dataset.marqueeApplied === 'true' && el.querySelector('.marquee-outer')) || el.querySelector('.truncate')){ return;}

  let text = el.textContent.trim();
  if (!text) return;

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
}, 2000); // 2000 milliseconds = 2 seconds
");

        }


        private void togUpRightLC_CheckedChanged(object sender, EventArgs e)
        {
            if (togUpRightLC.Checked) { holdUpRightLC.Checked = false; holdUpRightLC.Enabled = false; }
            else { holdUpRightLC.Enabled = true; }
        }

        private void togUpRightRC_CheckedChanged(object sender, EventArgs e)
        {
            if (togUpRightRC.Checked) { holdUpRightRC.Checked = false; holdUpRightRC.Enabled = false; }
            else { holdUpRightRC.Enabled = true; holdUpRightRC.Checked = true; }
        }

        private void togRightLC_CheckedChanged(object sender, EventArgs e)
        {
            if (togRightLC.Checked) { holdRightLC.Checked = false; holdRightLC.Enabled = false; }
            else { holdRightLC.Enabled = true; holdRightLC.Checked = true; }
        }

        private void togRightRC_CheckedChanged(object sender, EventArgs e)
        {
            if (togRightRC.Checked) { holdRightRC.Checked = false; holdRightRC.Enabled = false; }
            else
            {
                holdRightRC.Enabled = true;

                holdRightRC.Checked = true;
            }
        }

        private void togDownRightLC_CheckedChanged(object sender, EventArgs e)
        {
            if (togDownRightLC.Checked) { holdDownRightLC.Checked = false; holdDownRightLC.Enabled = false; }
            else
            {
                holdDownRightLC.Enabled = true;
                holdDownRightLC.Checked = true;
            }
        }

        private void togDownRightRC_CheckedChanged(object sender, EventArgs e)
        {
            if (togDownRightRC.Checked) { holdDownRightRC.Checked = false; holdDownRightRC.Enabled = false; }
            else
            {
                holdDownRightRC.Enabled = true;
                holdDownRightRC.Checked = true;
            }
        }

        private void togDownLC_CheckedChanged(object sender, EventArgs e)
        {
            if (togDownLC.Checked) { holdDownLC.Checked = false; holdDownLC.Enabled = false; }
            else
            {
                holdDownLC.Enabled = true;
                holdDownLC.Checked = true;
            }
        }

        private void togDownRC_CheckedChanged(object sender, EventArgs e)
        {
            if (togDownRC.Checked) { holdDownRC.Checked = false; holdDownRC.Enabled = false; }
            else
            {
                holdDownRC.Enabled = true;
                holdDownRC.Checked = true;
            }
        }

        private void togDownLeftLC_CheckedChanged(object sender, EventArgs e)
        {
            if (togDownLeftLC.Checked) { holdDownLeftLC.Checked = false; holdDownLeftLC.Enabled = false; }
            else
            {
                holdDownLeftLC.Enabled = true;
                holdDownLeftLC.Checked = true;
            }
        }

        private void togDownLeftRC_CheckedChanged(object sender, EventArgs e)
        {
            if (togDownLeftRC.Checked) { holdDownLeftRC.Checked = false; holdDownLeftRC.Enabled = false; }
            else
            {
                holdDownLeftRC.Enabled = true;
                holdDownLeftRC.Checked = true;
            }
        }

        private void togLeftLC_CheckedChanged(object sender, EventArgs e)
        {
            if (togLeftLC.Checked) { holdLeftLC.Checked = false; holdLeftLC.Enabled = false; }
            else
            {
                holdLeftLC.Enabled = true;
                holdLeftLC.Checked = true;
            }
        }

        private void togLeftRC_CheckedChanged(object sender, EventArgs e)
        {
            if (togLeftRC.Checked) { holdLeftRC.Checked = false; holdLeftRC.Enabled = false; }
            else
            {
                holdLeftRC.Enabled = true;
                holdLeftRC.Checked = true;
            }
        }

        private void togUpLeftLC_CheckedChanged(object sender, EventArgs e)
        {
            if (togUpLeftLC.Checked) { holdUpLeftLC.Checked = false; holdUpLeftLC.Enabled = false; }
            else
            {
                holdUpLeftLC.Enabled = true;
                holdUpLeftLC.Checked = true;
            }
        }

        private void togUpLeftRC_CheckedChanged(object sender, EventArgs e)
        {
            if (togUpLeftRC.Checked) { holdUpLeftRC.Checked = false; holdUpLeftRC.Enabled = false; }
            else
            {
                holdUpLeftRC.Enabled = true;
                holdUpLeftRC.Checked = true;
            }
        }

        private void togUpLC_CheckedChanged(object sender, EventArgs e)
        {
            if (togUpLC.Checked) { holdUpLC.Checked = false; holdUpLC.Enabled = false; }
            else
            {
                holdUpLC.Enabled = true;
                holdUpLC.Checked = true;
            }
        }

        private void togUpRC_CheckedChanged(object sender, EventArgs e)
        {
            if (togUpRC.Checked) { holdUpRC.Checked = false; holdUpRC.Enabled = false; }
            else
            {
                holdUpRC.Enabled = true;
                holdUpRC.Checked = true;
            }
        }

        private void togDZUpperLC_CheckedChanged(object sender, EventArgs e)
        {
            if (togDZUpperLC.Checked) { holdDZUpperLC.Checked = false; holdDZUpperLC.Enabled = false; }
            else
            {
                holdDZUpperLC.Enabled = true;
                holdDZUpperLC.Checked = true;
            }
        }

        private void togDZUpperRC_CheckedChanged(object sender, EventArgs e)
        {
            if (togDZUpperRC.Checked) { holdDZUpperRC.Checked = false; holdDZUpperRC.Enabled = false; }
            else
            {
                holdDZUpperRC.Enabled = true;
                holdDZUpperRC.Checked = true;
            }
        }

        private void togDZMiddleLC_CheckedChanged(object sender, EventArgs e)
        {
            if (togDZMiddleLC.Checked) { holdDZMiddleLC.Checked = false; holdDZMiddleLC.Enabled = false; }
            else
            {
                holdDZMiddleLC.Enabled = true;
                holdDZMiddleLC.Checked = true;
            }
        }

        private void togDZMiddleRC_CheckedChanged(object sender, EventArgs e)
        {
            if (togDZMiddleRC.Checked) { holdDZMiddleRC.Checked = false; holdDZMiddleRC.Enabled = false; }
            else
            {
                holdDZMiddleRC.Enabled = true;
                holdDZMiddleRC.Checked = true;
            }
        }

        private void togDZLowerLC_CheckedChanged(object sender, EventArgs e)
        {
            if (togDZLowerLC.Checked) { holdDZLowerLC.Checked = false; holdDZLowerLC.Enabled = false; }
            else
            {
                holdDZLowerLC.Enabled = true;
                holdDZLowerLC.Checked = true;
            }
        }

        private void togDZLowerRC_CheckedChanged(object sender, EventArgs e)
        {
            if (togDZLowerRC.Checked) { holdDZLowerRC.Checked = false; holdDZLowerRC.Enabled = false; }
            else
            {
                holdDZLowerRC.Enabled = true;
                holdDZLowerRC.Checked = true;
            }
        }


        private void nintendoSwitch_CheckedChanged(object sender, EventArgs e)
        {
            if (isLoadingProfile)
            {
                return;
            }

            if (nintendoSwitch.Checked == true)
            {

                cmbAdvanced.Items.Remove("(Click to Turn On) Press PlayStation Buttons");
                cmbAdvanced.Items.Remove("(Click to Turn Off) Press PlayStation Buttons");

                cmbAdvanced.Items.Remove("(Click to Turn On) Keep Mouse Inside Overjoyed");
                cmbAdvanced.Items.Remove("(Click to Turn Off) Keep Mouse Inside Overjoyed");

                if (advKeepMouse == true)
                {
                    cmbAdvanced.Items.Add("(Click to Turn On) Keep Mouse Inside Overjoyed");
                }
                else
                {
                    cmbAdvanced.Items.Add("(Click to Turn Off) Keep Mouse Inside Overjoyed");
                }

                cmbAdvanced.Items.Remove("(Click to Turn On) Mario Kart Mode");
                cmbAdvanced.Items.Remove("(Click to Turn Off) Mario Kart Mode");

                if (advMarioKart)
                {
                    cmbAdvanced.Items.Add("(Click to Turn Off) Mario Kart Mode");
                }
                else
                {
                    cmbAdvanced.Items.Add("(Click to Turn On) Mario Kart Mode");
                }

                cmbAdvanced.Items.Remove("(Click to Turn Off) Prevent Game From Locking Mouse");
                cmbAdvanced.Items.Remove("(Click to Turn On) Prevent Game From Locking Mouse");
            }
            pnlDeadzone.SuspendLayout();
            pnlQuadrants.SuspendLayout();
            btnHover.PerformClick();


            optUpRight.SelectedIndex = 0;
            optRight.SelectedIndex = 0; optDownRight.SelectedIndex = 0; optDown.SelectedIndex = 0;
            optDownLeft.SelectedIndex = 0; optLeft.SelectedIndex = 0; optUpLeft.SelectedIndex = 0; optUp.SelectedIndex = 0;
            optDZUpper.SelectedIndex = 0; optDZMiddle.SelectedIndex = 0; optDZLower.SelectedIndex = 0;



            string lineMicro = File.ReadLines(tourFilePath).ElementAtOrDefault(1);

            if (nintendoSwitch.Checked == true && lineMicro == "microYes")
            {
                MicroSetup();
            }


            if (!chkXbox.Checked && nintendoSwitch.Checked)
            {


                if (nintendoSwitch.Checked == true && nonVariableSwitch.Contains(switchUpRightHover.Text) == false)
                {
                    if (optUpRight.Items.Contains(optVariable) == false) { optUpRight.Items.Add(optVariable); }
                }
                else
                {
                    optUpRight.Items.Remove(optVariable);
                }

                if (nintendoSwitch.Checked == true && nonVariableSwitch.Contains(switchRightHover.Text) == false)
                {
                    if (optRight.Items.Contains(optVariable) == false) { optRight.Items.Add(optVariable); }
                }
                else
                {
                    optRight.Items.Remove(optVariable);
                }

                if (nintendoSwitch.Checked == true && nonVariableSwitch.Contains(switchDownRightHover.Text) == false)
                {
                    if (optDownRight.Items.Contains(optVariable) == false) { optDownRight.Items.Add(optVariable); }
                }
                else
                {
                    optDownRight.Items.Remove(optVariable);
                }

                if (nintendoSwitch.Checked == true && nonVariableSwitch.Contains(switchDownHover.Text) == false)
                {
                    if (optDown.Items.Contains(optVariable) == false) { optDown.Items.Add(optVariable); }
                }
                else
                {
                    optDown.Items.Remove(optVariable);
                }

                if (nintendoSwitch.Checked == true && nonVariableSwitch.Contains(switchDownLeftHover.Text) == false)
                {
                    if (optDownLeft.Items.Contains(optVariable) == false) { optDownLeft.Items.Add(optVariable); }
                }
                else
                {
                    optDownLeft.Items.Remove(optVariable);
                }

                if (nintendoSwitch.Checked == true && nonVariableSwitch.Contains(switchLeftHover.Text) == false)
                {
                    if (optLeft.Items.Contains(optVariable) == false) { optLeft.Items.Add(optVariable); }
                }
                else
                {
                    optLeft.Items.Remove(optVariable);
                }

                if (nintendoSwitch.Checked == true && nonVariableSwitch.Contains(switchUpLeftHover.Text) == false)
                {
                    if (optUpLeft.Items.Contains(optVariable) == false) { optUpLeft.Items.Add(optVariable); }
                }
                else
                {
                    optUpLeft.Items.Remove(optVariable);
                }

                if (nintendoSwitch.Checked == true && nonVariableSwitch.Contains(switchUpHover.Text) == false)
                {
                    if (optUp.Items.Contains(optVariable) == false) { optUp.Items.Add(optVariable); }
                }
                else
                {
                    optUp.Items.Remove(optVariable);
                }
            }

            if (nintendoSwitch.Checked)
            {
                chkXbox.Checked = false; chkKeyboard.Checked = false; chkKeyboard.Enabled = true;
                optUp.Items.Remove(optDisable); optUpRight.Items.Remove(optDisable); optRight.Items.Remove(optDisable); optDownRight.Items.Remove(optDisable);
                optDown.Items.Remove(optDisable); optDownLeft.Items.Remove(optDisable); optLeft.Items.Remove(optDisable); optUpLeft.Items.Remove(optDisable);
                optDZUpper.Items.Remove(optDisable); optDZMiddle.Items.Remove(optDisable); optDZLower.Items.Remove(optDisable);

                chkStreamer.Checked = false;
                chkStreamer.Visible = false;

                btnUpLclick.Visible = false; btnUpRclick.Visible = false; btnUpHover.Visible = false;
                btnUpRightLclick.Visible = false; btnUpRightRclick.Visible = false; btnUpRightHover.Visible = false;
                btnRightLclick.Visible = false; btnRightRclick.Visible = false; btnRightHover.Visible = false;
                btnDownRightLclick.Visible = false; btnDownRightRclick.Visible = false; btnDownRightHover.Visible = false;
                btnDownLclick.Visible = false; btnDownRclick.Visible = false; btnDownHover.Visible = false;
                btnDownLeftLclick.Visible = false; btnDownLeftRclick.Visible = false; btnDownLeftHover.Visible = false;
                btnLeftLclick.Visible = false; btnLeftRclick.Visible = false; btnLeftHover.Visible = false;
                btnUpLeftLclick.Visible = false; btnUpLeftRclick.Visible = false; btnUpLeftHover.Visible = false;
                btnDZUpperLC.Visible = false; btnDZUpperRC.Visible = false; btnDZUpperHover.Visible = false;
                btnDZMiddleLC.Visible = false; btnDZMiddleRC.Visible = false; btnDZMiddleHover.Visible = false;
                btnDZLowerLC.Visible = false; btnDZLowerRC.Visible = false; btnDZLowerHover.Visible = false;

                switchUpLC.Visible = false; switchUpRC.Visible = false; switchUpHover.Visible = true;
                switchUpRightLC.Visible = false; switchUpRightRC.Visible = false; switchUpRightHover.Visible = true;
                switchRightLC.Visible = false; switchRightRC.Visible = false; switchRightHover.Visible = true;
                switchDownRightLC.Visible = false; switchDownRightRC.Visible = false; switchDownRightHover.Visible = true;
                switchDownLC.Visible = false; switchDownRC.Visible = false; switchDownHover.Visible = true;
                switchDownLeftLC.Visible = false; switchDownLeftRC.Visible = false; switchDownLeftHover.Visible = true;
                switchLeftLC.Visible = false; switchLeftRC.Visible = false; switchLeftHover.Visible = true;
                switchUpLeftLC.Visible = false; switchUpLeftRC.Visible = false; switchUpLeftHover.Visible = true;
                switchDZUpperLC.Visible = false; switchDZUpperRC.Visible = false; switchDZUpperHover.Visible = true;
                switchDZMiddleLC.Visible = false; switchDZMiddleRC.Visible = false; switchDZMiddleHover.Visible = true;
                switchDZLowerLC.Visible = false; switchDZLowerRC.Visible = false; switchDZLowerHover.Visible = true;

            }
            else if (!nintendoSwitch.Checked)
            {
                chkStreamer.Visible = false;

                if (!isLoadingProfile && !chkXbox.Checked)
                {
                    chkKeyboard.Checked = true;
                }


                btnUpLclick.Visible = false; btnUpRclick.Visible = false; btnUpHover.Visible = true;
                btnUpRightLclick.Visible = false; btnUpRightRclick.Visible = false; btnUpRightHover.Visible = true;
                btnRightLclick.Visible = false; btnRightRclick.Visible = false; btnRightHover.Visible = true;
                btnDownRightLclick.Visible = false; btnDownRightRclick.Visible = false; btnDownRightHover.Visible = true;
                btnDownLclick.Visible = false; btnDownRclick.Visible = false; btnDownHover.Visible = true;
                btnDownLeftLclick.Visible = false; btnDownLeftRclick.Visible = false; btnDownLeftHover.Visible = true;
                btnLeftLclick.Visible = false; btnLeftRclick.Visible = false; btnLeftHover.Visible = true;
                btnUpLeftLclick.Visible = false; btnUpLeftRclick.Visible = false; btnUpLeftHover.Visible = true;
                btnDZUpperLC.Visible = false; btnDZUpperRC.Visible = false; btnDZUpperHover.Visible = true;
                btnDZMiddleLC.Visible = false; btnDZMiddleRC.Visible = false; btnDZMiddleHover.Visible = true;
                btnDZLowerLC.Visible = false; btnDZLowerRC.Visible = false; btnDZLowerHover.Visible = true;


                switchUpLC.Visible = false; switchUpRC.Visible = false; switchUpHover.Visible = false;
                switchUpRightLC.Visible = false; switchUpRightRC.Visible = false; switchUpRightHover.Visible = false;
                switchRightLC.Visible = false; switchRightRC.Visible = false; switchRightHover.Visible = false;
                switchDownRightLC.Visible = false; switchDownRightRC.Visible = false; switchDownRightHover.Visible = false;
                switchDownLC.Visible = false; switchDownRC.Visible = false; switchDownHover.Visible = false;
                switchDownLeftLC.Visible = false; switchDownLeftRC.Visible = false; switchDownLeftHover.Visible = false;
                switchLeftLC.Visible = false; switchLeftRC.Visible = false; switchLeftHover.Visible = false;
                switchUpLeftLC.Visible = false; switchUpLeftRC.Visible = false; switchUpLeftHover.Visible = false;
                switchDZUpperLC.Visible = false; switchDZUpperRC.Visible = false; switchDZUpperHover.Visible = false;
                switchDZMiddleLC.Visible = false; switchDZMiddleRC.Visible = false; switchDZMiddleHover.Visible = false;
                switchDZLowerLC.Visible = false; switchDZLowerRC.Visible = false; switchDZLowerHover.Visible = false;

            }


            pnlDeadzone.ResumeLayout();
            pnlQuadrants.ResumeLayout();
            Eval(@"function applyMarquee(el) {  if ((el.dataset.marqueeApplied === 'true' && el.querySelector('.marquee-outer')) || el.querySelector('.truncate')){ return;}

  let text = el.textContent.trim();
  if (!text) return;

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
}, 2000); // 2000 milliseconds = 2 seconds
");
        }

        private void switchDownHover_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isApplyingDisableButtonState)
            {
                return;
            }

            pnlDeadzone.SuspendLayout(); pnlQuadrants.ResumeLayout(); string selected = switchDownHover.Text;
            optDown.SelectedIndex = 0;
            selected = selected.Substring(0, 3); selected = selected.Replace("Cho", "Non").Replace("Up-", "Non").Replace("Dow", "Non");

            HandleNoneSelectionAsDisableButton(switchDownHover, "disableDownHover");

            UpdateAllHoverVariableOptions();

            pnlDeadzone.ResumeLayout();
            pnlQuadrants.ResumeLayout();

        }

        private void switchDownLeftHover_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isApplyingDisableButtonState)
            {
                return;
            }

            pnlDeadzone.SuspendLayout(); pnlQuadrants.ResumeLayout(); string selected = switchDownLeftHover.Text;
            optDownLeft.SelectedIndex = 0;
            selected = selected.Substring(0, 3); selected = selected.Replace("Cho", "Non").Replace("Up-", "Non").Replace("Dow", "Non");

            HandleNoneSelectionAsDisableButton(switchDownLeftHover, "disableDownLeftHover");

            UpdateAllHoverVariableOptions();

            pnlDeadzone.ResumeLayout();
            pnlQuadrants.ResumeLayout();
        }

        private void switchDownRightHover_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isApplyingDisableButtonState)
            {
                return;
            }

            pnlDeadzone.SuspendLayout(); pnlQuadrants.ResumeLayout(); string selected = switchDownRightHover.Text;
            optDownRight.SelectedIndex = 0;
            selected = selected.Substring(0, 3); selected = selected.Replace("Cho", "Non").Replace("Up-", "Non").Replace("Dow", "Non");

            HandleNoneSelectionAsDisableButton(switchDownRightHover, "disableDownRightHover");

            UpdateAllHoverVariableOptions();

            pnlDeadzone.ResumeLayout();
            pnlQuadrants.ResumeLayout();
        }

        private void switchDZLowerHover_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isApplyingDisableButtonState)
            {
                return;
            }

            pnlDeadzone.SuspendLayout(); pnlQuadrants.ResumeLayout(); string selected = switchDZLowerHover.Text;
            optDZLower.SelectedIndex = 0;
            selected = selected.Substring(0, 3); selected = selected.Replace("Cho", "Non").Replace("Up-", "Non").Replace("Dow", "Non");

            HandleNoneSelectionAsDisableButton(switchDZLowerHover, "disableDZLowerHover");



            if (nintendoSwitch.Checked && nonVariableSwitch.Contains(switchDZLowerHover.Text) == false)
            {
                if (optDZLower.Items.Contains(optVariable) == false) { optDZLower.Items.Add(optVariable); }
            }
            else
            {
                optDZLower.Items.Remove(optVariable);
            }

            pnlDeadzone.ResumeLayout();
            pnlQuadrants.ResumeLayout();
        }

        private void switchDZMiddleHover_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isApplyingDisableButtonState)
            {
                return;
            }

            pnlDeadzone.SuspendLayout(); pnlQuadrants.ResumeLayout(); string selected = switchDZMiddleHover.Text;
            optDZMiddle.SelectedIndex = 0;
            selected = selected.Substring(0, 3); selected = selected.Replace("Cho", "Non").Replace("Up-", "Non").Replace("Dow", "Non");

            HandleNoneSelectionAsDisableButton(switchDZMiddleHover, "disableDZMiddleHover");




            if (nintendoSwitch.Checked && nonVariableSwitch.Contains(switchDZMiddleHover.Text) == false)
            {
                if (optDZMiddle.Items.Contains(optVariable) == false) { optDZMiddle.Items.Add(optVariable); }
            }
            else
            {
                optDZMiddle.Items.Remove(optVariable);
            }

            pnlDeadzone.ResumeLayout();
            pnlQuadrants.ResumeLayout();
        }

        private void switchDZUpperHover_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isApplyingDisableButtonState)
            {
                return;
            }

            pnlDeadzone.SuspendLayout(); pnlQuadrants.ResumeLayout(); string selected = switchDZUpperHover.Text;
            optDZUpper.SelectedIndex = 0;
            selected = selected.Substring(0, 3); selected = selected.Replace("Cho", "Non").Replace("Up-", "Non").Replace("Dow", "Non");

            HandleNoneSelectionAsDisableButton(switchDZUpperHover, "disableDZUpperHover");


            if (nintendoSwitch.Checked && nonVariableSwitch.Contains(switchDZUpperHover.Text) == false)
            {
                if (optDZUpper.Items.Contains(optVariable) == false) { optDZUpper.Items.Add(optVariable); }
            }
            else
            {
                optDZUpper.Items.Remove(optVariable);
            }

            pnlDeadzone.ResumeLayout();
            pnlQuadrants.ResumeLayout();
        }

        private void switchLeftHover_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isApplyingDisableButtonState)
            {
                return;
            }

            pnlDeadzone.SuspendLayout(); pnlQuadrants.ResumeLayout(); string selected = switchLeftHover.Text;
            optLeft.SelectedIndex = 0;
            selected = selected.Substring(0, 3); selected = selected.Replace("Cho", "Non").Replace("Up-", "Non").Replace("Dow", "Non");

            HandleNoneSelectionAsDisableButton(switchLeftHover, "disableLeftHover");

            UpdateAllHoverVariableOptions();

            pnlDeadzone.ResumeLayout();
            pnlQuadrants.ResumeLayout();
        }

        private void switchRightHover_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isApplyingDisableButtonState)
            {
                return;
            }

            pnlDeadzone.SuspendLayout(); pnlQuadrants.ResumeLayout(); string selected = switchRightHover.Text;
            optRight.SelectedIndex = 0;
            selected = selected.Substring(0, 3); selected = selected.Replace("Cho", "Non").Replace("Up-", "Non").Replace("Dow", "Non");

            HandleNoneSelectionAsDisableButton(switchRightHover, "disableRightHover");

            UpdateAllHoverVariableOptions();

            pnlDeadzone.ResumeLayout();
            pnlQuadrants.ResumeLayout();
        }

        private void switchUpHover_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isApplyingDisableButtonState)
            {
                return;
            }

            pnlDeadzone.SuspendLayout(); pnlQuadrants.ResumeLayout(); string selected = switchUpHover.Text;
            optUp.SelectedIndex = 0;
            selected = selected.Substring(0, 3); selected = selected.Replace("Cho", "Non").Replace("Up-", "Non").Replace("Dow", "Non");

            HandleNoneSelectionAsDisableButton(switchUpHover, "disableUpHover");

            UpdateAllHoverVariableOptions();


            pnlDeadzone.ResumeLayout();
            pnlQuadrants.ResumeLayout();
        }

        private void switchUpLeftHover_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isApplyingDisableButtonState)
            {
                return;
            }

            pnlDeadzone.SuspendLayout(); pnlQuadrants.ResumeLayout(); string selected = switchUpLeftHover.Text;
            optUpLeft.SelectedIndex = 0;
            selected = selected.Substring(0, 3); selected = selected.Replace("Cho", "Non").Replace("Up-", "Non").Replace("Dow", "Non");

            HandleNoneSelectionAsDisableButton(switchUpLeftHover, "disableUpLeftHover");

            UpdateAllHoverVariableOptions();

            pnlDeadzone.ResumeLayout();
            pnlQuadrants.ResumeLayout();
        }

        private void switchUpRightHover_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isApplyingDisableButtonState)
            {
                return;
            }

            pnlDeadzone.SuspendLayout(); pnlQuadrants.ResumeLayout(); string selected = switchUpRightHover.Text;
            optUpRight.SelectedIndex = 0;
            selected = selected.Substring(0, 3); selected = selected.Replace("Cho", "Non").Replace("Up-", "Non").Replace("Dow", "Non");

            HandleNoneSelectionAsDisableButton(switchUpRightHover, "disableUpRightHover");

            UpdateAllHoverVariableOptions();

            pnlDeadzone.ResumeLayout();
            pnlQuadrants.ResumeLayout();
        }

        private void disableButton_Click(object sender, EventArgs e)
        {
            var ctrl = sender as Control;
            ApplyDisableButtonState(ctrl, ctrl.CssClass == "disableButtonOn");
        }

        private void ApplyDisableButtonState(Control ctrl, bool isDisabled)
        {
            string controlNameXbox, controlNameSwitch, controlNameKeyboard, controlNameDisable;

            Control targetControlXbox, targetControlSwitch, targetControlKeyboard, targetControlDisable;

            ComboBox cmbXbox, cmbSwitch;

            Button btnKeyboard;

            CheckBox chkDisable;

            if (ctrl == null)
            {
                return;
            }

            if (isApplyingDisableButtonState)
            {
                return;
            }

            isApplyingDisableButtonState = true;

            try
            {

                if (ctrl.Name.Contains("DZ") == false)
                {
                    controlNameXbox = ctrl.Name.Replace("LC", "Lclick").Replace("disable", "cmb").Replace("RC", "Rclick");
                }
                else
                {
                    controlNameXbox = ctrl.Name.Replace("disable", "cmb");
                }

                targetControlXbox = Controls.Find(controlNameXbox, true)[0];
                cmbXbox = targetControlXbox as ComboBox;

                controlNameSwitch = ctrl.Name.Replace("disable", "switch");
                targetControlSwitch = Controls.Find(controlNameSwitch, true)[0];
                cmbSwitch = targetControlSwitch as ComboBox;

                if (ctrl.Name.Contains("DZ") == false)
                {
                    controlNameKeyboard = ctrl.Name.Replace("LC", "Lclick").Replace("disable", "btn").Replace("RC", "Rclick");
                }
                else
                {
                    controlNameKeyboard = ctrl.Name.Replace("disable", "btn");
                }

                targetControlKeyboard = Controls.Find(controlNameKeyboard, true)[0];
                btnKeyboard = targetControlKeyboard as Button;

                controlNameDisable = ctrl.Name.Replace("able", "");
                targetControlDisable = Controls.Find(controlNameDisable, true)[0];
                chkDisable = targetControlDisable as CheckBox;

                if (isDisabled)
                {
                    UncheckDiagonalCombineForDisableControl(ctrl);
                }

                bool isCombined = IsCombinedAssignmentControl(ctrl);
                bool keepControllerCombosEnabled = (chkXbox.Checked || nintendoSwitch.Checked);

                if (isDisabled)
                {
                    bool wasBlueOnEntry = string.Equals(ctrl.CssClass, "disableButtonOn", StringComparison.Ordinal);

                    ctrl.RemoveCssClass("disableButtonOn");
                    ctrl.AddCssClass("disableButtonOff");
                    ctrl.ToolTipText = "Click to Enable";

                    if (!isCombined && wasBlueOnEntry)
                    {
                        cmbXbox.Items[0] = "None";
                        cmbSwitch.Items[0] = "None";
                        btnKeyboard.Text = "None";
                        cmbXbox.SelectedIndex = 0;
                        cmbXbox.Enabled = keepControllerCombosEnabled;
                        ApplyAssignmentComboBoxVisualState(cmbXbox, keepControllerCombosEnabled);

                        cmbSwitch.SelectedIndex = 0;
                        cmbSwitch.Enabled = keepControllerCombosEnabled;
                        ApplyAssignmentComboBoxVisualState(cmbSwitch, keepControllerCombosEnabled);
                    }

                    chkDisable.Checked = true;
                    btnKeyboard.Enabled = false;
                }
                else
                {
                    ctrl.RemoveCssClass("disableButtonOff");
                    ctrl.AddCssClass("disableButtonOn");
                    ctrl.ToolTipText = "Click to Disable";

                    if (!isCombined)
                    {
                        cmbXbox.Items[0] = "Click to Set";
                        cmbSwitch.Items[0] = "Click to Set";
                        btnKeyboard.Text = "Backtick";
                        cmbXbox.Enabled = true;
                        ApplyAssignmentComboBoxVisualState(cmbXbox, true);
                        // Preserve a user-chosen nonzero assignment when enabling from black/off state.
                        if (cmbXbox.SelectedIndex <= 0)
                        {
                            cmbXbox.SelectedIndex = 0;
                        }
                        btnKeyboard.Enabled = true;
                        cmbSwitch.Enabled = true;
                        ApplyAssignmentComboBoxVisualState(cmbSwitch, true);
                    }

                    chkDisable.Checked = false;
                }
            }
            finally
            {
                isApplyingDisableButtonState = false;
            }
        }

        private void NormalizeProfileUiBeforeLoad()
        {
            string[] disableButtonNames =
            {
                "disableUpHover", "disableUpRightHover", "disableRightHover", "disableDownRightHover", "disableDownHover", "disableDownLeftHover", "disableLeftHover", "disableUpLeftHover",
                "disableUpLC", "disableUpRightLC", "disableRightLC", "disableDownRightLC", "disableDownLC", "disableDownLeftLC", "disableLeftLC", "disableUpLeftLC",
                "disableUpRC", "disableUpRightRC", "disableRightRC", "disableDownRightRC", "disableDownRC", "disableDownLeftRC", "disableLeftRC", "disableUpLeftRC",
                "disableDZUpperHover", "disableDZUpperLC", "disableDZUpperRC", "disableDZMiddleHover", "disableDZMiddleLC", "disableDZMiddleRC", "disableDZLowerHover", "disableDZLowerLC", "disableDZLowerRC"
            };

            foreach (string disableButtonName in disableButtonNames)
            {
                Control[] disableControls = Controls.Find(disableButtonName, true);
                if (disableControls.Length == 0)
                {
                    continue;
                }

                Button disableButton = disableControls[0] as Button;
                if (disableButton == null)
                {
                    continue;
                }

                string disableCheckboxName = disableButtonName.Replace("able", "");
                Control[] disableCheckboxControls = Controls.Find(disableCheckboxName, true);
                if (disableCheckboxControls.Length > 0 && disableCheckboxControls[0] is CheckBox disableCheckbox)
                {
                    disableCheckbox.Checked = false;
                }

                EnsureDisableButtonClickedState(disableButton, true);
            }
        }


        private void ApplyLoadedDiagonalMergeState(HtmlPanel mergePanel, bool isCombined, string diagonalLabel)
        {
            mergePanel.Html = GetDiagonalMergeHtml(diagonalLabel, isCombined);
            mergePanel.ToolTipText = isCombined ? "Click to Uncombine" : "Click to Combine";
        }

        private string GetDiagonalMergeHtml(string diagonalLabel, bool isCombined)
        {
            string fillColor = isCombined ? "#00a7d1" : "#636372";

            return diagonalLabel switch
            {
                "Up-Right" => $"<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(315) translate(10,5)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='{fillColor}' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>",
                "Down-Right" => $"<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(45) translate(10,10)'><path transform='rotate(0) scale(2.9) translate(0,-25)' fill='{fillColor}' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>",
                "Down-Left" => $"<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(135) translate(11,11)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='{fillColor}' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>",
                "Up-Left" => $"<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(225) translate(11,11)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='{fillColor}' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>",
                _ => string.Empty
            };
        }

        private bool HasAssignedActionValue(string value)
        {
            return !string.IsNullOrEmpty(value)
                && value != "None"
                && value != "Click to Set"
                && value != "Backtick"
                && value != "Press Key";
        }

        private int GetDisableButtonConfigIndex(string disableButtonName)
        {
            string[] disableButtonNames =
            {
                "disableUpHover", "disableUpRightHover", "disableRightHover", "disableDownRightHover", "disableDownHover", "disableDownLeftHover", "disableLeftHover", "disableUpLeftHover",
                "disableUpLC", "disableUpRightLC", "disableRightLC", "disableDownRightLC", "disableDownLC", "disableDownLeftLC", "disableLeftLC", "disableUpLeftLC",
                "disableUpRC", "disableUpRightRC", "disableRightRC", "disableDownRightRC", "disableDownRC", "disableDownLeftRC", "disableLeftRC", "disableUpLeftRC",
                "disableDZUpperHover", "disableDZUpperLC", "disableDZUpperRC", "disableDZMiddleHover", "disableDZMiddleLC", "disableDZMiddleRC", "disableDZLowerHover", "disableDZLowerLC", "disableDZLowerRC"
            };

            return Array.IndexOf(disableButtonNames, disableButtonName);
        }

        private bool GetDisableCheckboxState(Control ctrl)
        {
            if (ctrl == null)
            {
                return false;
            }

            string controlNameDisable = ctrl.Name.Replace("able", "");
            Control[] disableControls = Controls.Find(controlNameDisable, true);
            return disableControls.Length > 0 && disableControls[0] is CheckBox disableCheckbox && disableCheckbox.Checked;
        }

        private void EnsureDisableButtonClickedState(Control ctrl, bool shouldBeDisabled)
        {
            if (ctrl == null)
            {
                return;
            }

            bool visuallyDisabled = string.Equals(ctrl.CssClass, "disableButtonOff", StringComparison.Ordinal);
            if (GetDisableCheckboxState(ctrl) == shouldBeDisabled && visuallyDisabled == shouldBeDisabled)
            {
                return;
            }

            ApplyDisableButtonState(ctrl, shouldBeDisabled);
        }

        private bool DisableButtonHasActiveAssignment(Control ctrl)
        {
            if (ctrl == null)
            {
                return false;
            }

            string controlNameXbox = ctrl.Name.Contains("DZ")
                ? ctrl.Name.Replace("disable", "cmb")
                : ctrl.Name.Replace("LC", "Lclick").Replace("disable", "cmb").Replace("RC", "Rclick");
            string controlNameSwitch = ctrl.Name.Replace("disable", "switch");
            string controlNameKeyboard = ctrl.Name.Contains("DZ")
                ? ctrl.Name.Replace("disable", "btn")
                : ctrl.Name.Replace("LC", "Lclick").Replace("disable", "btn").Replace("RC", "Rclick");

            string xboxValue = string.Empty;
            string switchValue = string.Empty;
            string keyboardValue = string.Empty;

            Control[] xboxControls = Controls.Find(controlNameXbox, true);
            if (xboxControls.Length > 0 && xboxControls[0] is ComboBox cmbXbox)
            {
                xboxValue = GetCurrentComboBoxActionValue(cmbXbox);
            }

            Control[] switchControls = Controls.Find(controlNameSwitch, true);
            if (switchControls.Length > 0 && switchControls[0] is ComboBox cmbSwitch)
            {
                switchValue = GetCurrentComboBoxActionValue(cmbSwitch);
            }

            Control[] keyboardControls = Controls.Find(controlNameKeyboard, true);
            if (keyboardControls.Length > 0 && keyboardControls[0] is Button btnKeyboard)
            {
                keyboardValue = btnKeyboard.Text;
            }

            if (chkXbox.Checked)
            {
                return HasAssignedActionValue(xboxValue);
            }

            if (nintendoSwitch.Checked)
            {
                return HasAssignedActionValue(switchValue);
            }

            int configIndex = GetDisableButtonConfigIndex(ctrl.Name);
            if (configIndex >= 0 && buttonConfig != null && configIndex < buttonConfig.Length && buttonConfig[configIndex] != null)
            {
                if (chkXbox.Checked)
                {
                    return buttonConfig[configIndex].Length > 3 && HasAssignedActionValue(buttonConfig[configIndex][3]);
                }

                if (nintendoSwitch.Checked)
                {
                    return buttonConfig[configIndex].Length > 6 && HasAssignedActionValue(buttonConfig[configIndex][6]);
                }

                return buttonConfig[configIndex].Length > 0 && HasAssignedActionValue(buttonConfig[configIndex][0]);
            }

            return HasAssignedActionValue(keyboardValue);
        }

        private static string GetCurrentComboBoxActionValue(ComboBox comboBox)
        {
            if (comboBox == null)
            {
                return string.Empty;
            }

            string action = comboBox.Text;
            if (string.IsNullOrWhiteSpace(action) && comboBox.SelectedItem != null)
            {
                action = comboBox.SelectedItem.ToString();
            }

            return action ?? string.Empty;
        }

        private bool IsCombinedAssignmentControl(Control ctrl)
        {
            if (ctrl == null)
            {
                return false;
            }

            return (ctrl.Name == "disableUpRightHover" && diagUpRightHover.Checked)
                || (ctrl.Name == "disableDownRightHover" && diagDownRightHover.Checked)
                || (ctrl.Name == "disableDownLeftHover" && diagDownLeftHover.Checked)
                || (ctrl.Name == "disableUpLeftHover" && diagUpLeftHover.Checked)
                || (ctrl.Name == "disableUpRightLC" && diagUpRightLC.Checked)
                || (ctrl.Name == "disableDownRightLC" && diagDownRightLC.Checked)
                || (ctrl.Name == "disableDownLeftLC" && diagDownLeftLC.Checked)
                || (ctrl.Name == "disableUpLeftLC" && diagUpLeftLC.Checked)
                || (ctrl.Name == "disableUpRightRC" && diagUpRightRC.Checked)
                || (ctrl.Name == "disableDownRightRC" && diagDownRightRC.Checked)
                || (ctrl.Name == "disableDownLeftRC" && diagDownLeftRC.Checked)
                || (ctrl.Name == "disableUpLeftRC" && diagUpLeftRC.Checked);
        }

        private readonly ConcurrentDictionary<string, int> assignmentComboPreviousIndices = new();

        private void HandleNoneSelectionAsDisableButton(ComboBox assignmentCombo, string disableButtonName)
        {
            if (assignmentCombo == null)
            {
                return;
            }

            int currentIndex = assignmentCombo.SelectedIndex;
            bool hasPreviousIndex = assignmentComboPreviousIndices.TryGetValue(assignmentCombo.Name, out int previousIndex);
            assignmentComboPreviousIndices[assignmentCombo.Name] = currentIndex;

            if (isLoadingProfile || isApplyingDisableButtonState)
            {
                return;
            }

            if (!chkXbox.Checked && !nintendoSwitch.Checked)
            {
                return;
            }

            if (!hasPreviousIndex || previousIndex == currentIndex)
            {
                return;
            }

            bool previousIsZeroState = previousIndex <= 0;
            bool currentIsZeroState = currentIndex <= 0;
            if (previousIsZeroState == currentIsZeroState)
            {
                return;
            }

            Control[] disableControls = Controls.Find(disableButtonName, true);
            if (disableControls.Length == 0)
            {
                return;
            }

            Control disableButton = disableControls[0];
            bool shouldBeDisabled = currentIsZeroState;
            EnsureDisableButtonClickedState(disableButton, shouldBeDisabled);
        }

        private void RegisterNonHoverDisableSelectionHandlers()
        {
            var mappings = new (string assignmentControlName, string disableButtonName)[]
            {
                ("cmbUpHover", "disableUpHover"), ("cmbUpRightHover", "disableUpRightHover"), ("cmbRightHover", "disableRightHover"), ("cmbDownRightHover", "disableDownRightHover"),
                ("cmbDownHover", "disableDownHover"), ("cmbDownLeftHover", "disableDownLeftHover"), ("cmbLeftHover", "disableLeftHover"), ("cmbUpLeftHover", "disableUpLeftHover"),
                ("cmbDZUpperHover", "disableDZUpperHover"), ("cmbDZMiddleHover", "disableDZMiddleHover"), ("cmbDZLowerHover", "disableDZLowerHover"),

                ("cmbUpLclick", "disableUpLC"), ("cmbUpRightLclick", "disableUpRightLC"), ("cmbRightLclick", "disableRightLC"), ("cmbDownRightLclick", "disableDownRightLC"),
                ("cmbDownLclick", "disableDownLC"), ("cmbDownLeftLclick", "disableDownLeftLC"), ("cmbLeftLclick", "disableLeftLC"), ("cmbUpLeftLclick", "disableUpLeftLC"),
                ("cmbUpRclick", "disableUpRC"), ("cmbUpRightRclick", "disableUpRightRC"), ("cmbRightRclick", "disableRightRC"), ("cmbDownRightRclick", "disableDownRightRC"),
                ("cmbDownRclick", "disableDownRC"), ("cmbDownLeftRclick", "disableDownLeftRC"), ("cmbLeftRclick", "disableLeftRC"), ("cmbUpLeftRclick", "disableUpLeftRC"),
                ("cmbDZUpperLC", "disableDZUpperLC"), ("cmbDZMiddleLC", "disableDZMiddleLC"), ("cmbDZLowerLC", "disableDZLowerLC"),
                ("cmbDZUpperRC", "disableDZUpperRC"), ("cmbDZMiddleRC", "disableDZMiddleRC"), ("cmbDZLowerRC", "disableDZLowerRC"),

                ("switchUpHover", "disableUpHover"), ("switchUpRightHover", "disableUpRightHover"), ("switchRightHover", "disableRightHover"), ("switchDownRightHover", "disableDownRightHover"),
                ("switchDownHover", "disableDownHover"), ("switchDownLeftHover", "disableDownLeftHover"), ("switchLeftHover", "disableLeftHover"), ("switchUpLeftHover", "disableUpLeftHover"),
                ("switchDZUpperHover", "disableDZUpperHover"), ("switchDZMiddleHover", "disableDZMiddleHover"), ("switchDZLowerHover", "disableDZLowerHover"),

                ("switchUpLC", "disableUpLC"), ("switchUpRightLC", "disableUpRightLC"), ("switchRightLC", "disableRightLC"), ("switchDownRightLC", "disableDownRightLC"),
                ("switchDownLC", "disableDownLC"), ("switchDownLeftLC", "disableDownLeftLC"), ("switchLeftLC", "disableLeftLC"), ("switchUpLeftLC", "disableUpLeftLC"),
                ("switchUpRC", "disableUpRC"), ("switchUpRightRC", "disableUpRightRC"), ("switchRightRC", "disableRightRC"), ("switchDownRightRC", "disableDownRightRC"),
                ("switchDownRC", "disableDownRC"), ("switchDownLeftRC", "disableDownLeftRC"), ("switchLeftRC", "disableLeftRC"), ("switchUpLeftRC", "disableUpLeftRC"),
                ("switchDZUpperLC", "disableDZUpperLC"), ("switchDZMiddleLC", "disableDZMiddleLC"), ("switchDZLowerLC", "disableDZLowerLC"),
                ("switchDZUpperRC", "disableDZUpperRC"), ("switchDZMiddleRC", "disableDZMiddleRC"), ("switchDZLowerRC", "disableDZLowerRC")
            };

            foreach (var mapping in mappings)
            {
                Control[] assignmentControls = Controls.Find(mapping.assignmentControlName, true);
                if (assignmentControls.Length == 0 || assignmentControls[0] is not ComboBox assignmentCombo)
                {
                    continue;
                }

                assignmentCombo.SelectedIndexChanged -= NonHoverDisableSelectionHandler;
                assignmentCombo.SelectedIndexChanged += NonHoverDisableSelectionHandler;
                assignmentCombo.DropDown -= AssignmentCombo_DropDown;
                assignmentCombo.DropDown += AssignmentCombo_DropDown;
            }
        }

        private void AssignmentCombo_DropDown(object sender, EventArgs e)
        {
            if (sender is not ComboBox assignmentCombo)
            {
                return;
            }

            if (assignmentCombo.Items.Count > 0
                && string.Equals(assignmentCombo.Items[0]?.ToString(), "Click to Set", StringComparison.OrdinalIgnoreCase))
            {
                assignmentCombo.Items[0] = "None";
            }
        }

        private void InitializeAssignmentComboPreviousIndices()
        {
            assignmentComboPreviousIndices.Clear();
            SeedAssignmentComboPreviousIndices(this);
        }

        private void SeedAssignmentComboPreviousIndices(Control parent)
        {
            if (parent == null)
            {
                return;
            }

            foreach (Control child in parent.Controls)
            {
                if (child is ComboBox assignmentCombo)
                {
                    string disableButtonName = assignmentCombo.Name
                        .Replace("cmb", "disable")
                        .Replace("switch", "disable")
                        .Replace("Lclick", "LC")
                        .Replace("Rclick", "RC");

                    if (!string.Equals(disableButtonName, assignmentCombo.Name, StringComparison.Ordinal)
                        && Controls.Find(disableButtonName, true).Length > 0)
                    {
                        assignmentComboPreviousIndices[assignmentCombo.Name] = assignmentCombo.SelectedIndex;
                    }
                }

                if (child.HasChildren)
                {
                    SeedAssignmentComboPreviousIndices(child);
                }
            }
        }

        private void NonHoverDisableSelectionHandler(object sender, EventArgs e)
        {
            if (sender is not ComboBox assignmentCombo)
            {
                return;
            }

            string disableButtonName = assignmentCombo.Name
                .Replace("cmb", "disable")
                .Replace("switch", "disable")
                .Replace("Lclick", "LC")
                .Replace("Rclick", "RC");

            HandleNoneSelectionAsDisableButton(assignmentCombo, disableButtonName);
        }

        private void UncheckDiagonalCombineForDisableControl(Control ctrl)
        {
            if (ctrl == null)
            {
                return;
            }

            string suffix;
            if (ctrl.Name.EndsWith("Hover", StringComparison.Ordinal))
            {
                suffix = "Hover";
            }
            else if (ctrl.Name.EndsWith("LC", StringComparison.Ordinal))
            {
                suffix = "LC";
            }
            else if (ctrl.Name.EndsWith("RC", StringComparison.Ordinal))
            {
                suffix = "RC";
            }
            else
            {
                return;
            }

            string direction;
            if (ctrl.Name.Contains("UpRight", StringComparison.Ordinal))
            {
                direction = "UpRight";
            }
            else if (ctrl.Name.Contains("DownRight", StringComparison.Ordinal))
            {
                direction = "DownRight";
            }
            else if (ctrl.Name.Contains("DownLeft", StringComparison.Ordinal))
            {
                direction = "DownLeft";
            }
            else if (ctrl.Name.Contains("UpLeft", StringComparison.Ordinal))
            {
                direction = "UpLeft";
            }
            else
            {
                return;
            }

            string combineName = "diag" + direction + suffix;
            Control[] combineControls = Controls.Find(combineName, true);
            if (combineControls.Length > 0 && combineControls[0] is CheckBox combineCheckBox && combineCheckBox.Checked)
            {
                combineCheckBox.Checked = false;
            }
        }

        private void RestoreDisableButtonEnabledState(Control ctrl)
        {
            if (ctrl == null)
            {
                return;
            }

            bool hasActiveAssignment = DisableButtonHasActiveAssignment(ctrl);

            string controlNameXbox = ctrl.Name.Contains("DZ")
                ? ctrl.Name.Replace("disable", "cmb")
                : ctrl.Name.Replace("LC", "Lclick").Replace("disable", "cmb").Replace("RC", "Rclick");
            string controlNameSwitch = ctrl.Name.Replace("disable", "switch");
            string controlNameKeyboard = ctrl.Name.Contains("DZ")
                ? ctrl.Name.Replace("disable", "btn")
                : ctrl.Name.Replace("LC", "Lclick").Replace("disable", "btn").Replace("RC", "Rclick");
            string controlNameDisable = ctrl.Name.Replace("able", "");

            Control[] disableControls = Controls.Find(controlNameDisable, true);
            if (disableControls.Length > 0 && disableControls[0] is CheckBox disableCheckbox)
            {
                disableCheckbox.Checked = false;
            }

            ctrl.RemoveCssClass("disableButtonOff");
            ctrl.RemoveCssClass("disableButtonOn");
            ctrl.AddCssClass("disableButtonOn");
            ctrl.ToolTipText = "Click to Disable";

            bool isCombined = IsCombinedAssignmentControl(ctrl);
            bool shouldEnableAssignmentControls = !isCombined && hasActiveAssignment;

            Control[] xboxControls = Controls.Find(controlNameXbox, true);
            if (xboxControls.Length > 0 && xboxControls[0] is ComboBox cmbXbox)
            {
                cmbXbox.Enabled = shouldEnableAssignmentControls;
                ApplyAssignmentComboBoxVisualState(cmbXbox, shouldEnableAssignmentControls);
            }

            Control[] switchControls = Controls.Find(controlNameSwitch, true);
            if (switchControls.Length > 0 && switchControls[0] is ComboBox cmbSwitch)
            {
                cmbSwitch.Enabled = shouldEnableAssignmentControls;
                ApplyAssignmentComboBoxVisualState(cmbSwitch, shouldEnableAssignmentControls);
            }

            Control[] keyboardControls = Controls.Find(controlNameKeyboard, true);
            if (keyboardControls.Length > 0 && keyboardControls[0] is Button btnKeyboard)
            {
                btnKeyboard.Enabled = shouldEnableAssignmentControls;
            }
        }

        private void RefreshDisableButtonsByClick()
        {
            if (isRefreshingDisableButtons)
            {
                return;
            }

            isRefreshingDisableButtons = true;
            try
            {
                string[] disableButtonNames =
                {
                "disableUpHover", "disableUpRightHover", "disableRightHover", "disableDownRightHover", "disableDownHover", "disableDownLeftHover", "disableLeftHover", "disableUpLeftHover",
                "disableUpLC", "disableUpRightLC", "disableRightLC", "disableDownRightLC", "disableDownLC", "disableDownLeftLC", "disableLeftLC", "disableUpLeftLC",
                "disableUpRC", "disableUpRightRC", "disableRightRC", "disableDownRightRC", "disableDownRC", "disableDownLeftRC", "disableLeftRC", "disableUpLeftRC",
                "disableDZUpperHover", "disableDZUpperLC", "disableDZUpperRC", "disableDZMiddleHover", "disableDZMiddleLC", "disableDZMiddleRC", "disableDZLowerHover", "disableDZLowerLC", "disableDZLowerRC"
            };

                foreach (string disableButtonName in disableButtonNames)
                {
                    Control[] disableControls = Controls.Find(disableButtonName, true);
                    if (disableControls.Length == 0)
                    {
                        continue;
                    }

                    Control disableControl = disableControls[0];
                    bool isCombined = IsCombinedAssignmentControl(disableControl);
                    bool isDisabled = GetDisableCheckboxState(disableControl);
                    bool shouldBeDisabled = isDisabled || (!isCombined && !DisableButtonHasActiveAssignment(disableControl));

                    if (isCombined && !isDisabled)
                    {
                        RestoreDisableButtonEnabledState(disableControl);
                    }
                    else if (shouldBeDisabled)
                    {
                        EnsureDisableButtonClickedState(disableControl, true);
                    }
                    else
                    {
                        RestoreDisableButtonEnabledState(disableControl);
                    }

                    EnforceAssignmentControlEnabledState(disableControl);
                }
            }
            finally
            {
                isRefreshingDisableButtons = false;
            }
        }

        private void EnforceAssignmentControlEnabledState(Control ctrl)
        {
            if (ctrl == null)
            {
                return;
            }

            bool hasActiveAssignment = DisableButtonHasActiveAssignment(ctrl);
            bool isCombined = IsCombinedAssignmentControl(ctrl);
            bool isDisabled = GetDisableCheckboxState(ctrl);
            bool shouldEnableAssignmentControls = hasActiveAssignment && !isCombined && !isDisabled;
            bool keepControllerCombosEnabled = isDisabled && (chkXbox.Checked || nintendoSwitch.Checked);

            if (isCombined && !isDisabled)
            {
                ctrl.RemoveCssClass("disableButtonOff");
                ctrl.RemoveCssClass("disableButtonOn");
                ctrl.AddCssClass("disableButtonOn");
                ctrl.ToolTipText = "Click to Disable";
            }

            string controlNameXbox = ctrl.Name.Contains("DZ")
                ? ctrl.Name.Replace("disable", "cmb")
                : ctrl.Name.Replace("LC", "Lclick").Replace("disable", "cmb").Replace("RC", "Rclick");
            string controlNameSwitch = ctrl.Name.Replace("disable", "switch");
            string controlNameKeyboard = ctrl.Name.Contains("DZ")
                ? ctrl.Name.Replace("disable", "btn")
                : ctrl.Name.Replace("LC", "Lclick").Replace("disable", "btn").Replace("RC", "Rclick");

            Control[] xboxControls = Controls.Find(controlNameXbox, true);
            if (xboxControls.Length > 0 && xboxControls[0] is ComboBox cmbXbox)
            {
                bool shouldEnableXboxControl = shouldEnableAssignmentControls || keepControllerCombosEnabled;
                cmbXbox.Enabled = shouldEnableXboxControl;
                ApplyAssignmentComboBoxVisualState(cmbXbox, shouldEnableXboxControl);
            }

            Control[] switchControls = Controls.Find(controlNameSwitch, true);
            if (switchControls.Length > 0 && switchControls[0] is ComboBox cmbSwitch)
            {
                bool shouldEnableSwitchControl = shouldEnableAssignmentControls || keepControllerCombosEnabled;
                cmbSwitch.Enabled = shouldEnableSwitchControl;
                ApplyAssignmentComboBoxVisualState(cmbSwitch, shouldEnableSwitchControl);
            }

            Control[] keyboardControls = Controls.Find(controlNameKeyboard, true);
            if (keyboardControls.Length > 0 && keyboardControls[0] is Button btnKeyboard)
            {
                btnKeyboard.Enabled = shouldEnableAssignmentControls;
            }
        }

        private static void ApplyAssignmentComboBoxVisualState(ComboBox comboBox, bool isEnabled)
        {
            if (comboBox == null)
            {
                return;
            }

            comboBox.CssStyle = string.Empty;
        }

        private void HideAllSwitchAssignmentControls()
        {
            switchUpHover.Visible = false;
            switchUpRightHover.Visible = false;
            switchRightHover.Visible = false;
            switchDownRightHover.Visible = false;
            switchDownHover.Visible = false;
            switchDownLeftHover.Visible = false;
            switchLeftHover.Visible = false;
            switchUpLeftHover.Visible = false;

            switchUpLC.Visible = false;
            switchUpRightLC.Visible = false;
            switchRightLC.Visible = false;
            switchDownRightLC.Visible = false;
            switchDownLC.Visible = false;
            switchDownLeftLC.Visible = false;
            switchLeftLC.Visible = false;
            switchUpLeftLC.Visible = false;

            switchUpRC.Visible = false;
            switchUpRightRC.Visible = false;
            switchRightRC.Visible = false;
            switchDownRightRC.Visible = false;
            switchDownRC.Visible = false;
            switchDownLeftRC.Visible = false;
            switchLeftRC.Visible = false;
            switchUpLeftRC.Visible = false;

            switchDZUpperHover.Visible = false;
            switchDZMiddleHover.Visible = false;
            switchDZLowerHover.Visible = false;
            switchDZUpperLC.Visible = false;
            switchDZMiddleLC.Visible = false;
            switchDZLowerLC.Visible = false;
            switchDZUpperRC.Visible = false;
            switchDZMiddleRC.Visible = false;
            switchDZLowerRC.Visible = false;
        }



        private void Logo_Appear(object sender, EventArgs e)
        {
            Eval("window.innerHeight", new Action<object>(OnBrowserHeightReceived));
        }

        private void OnBrowserHeightReceived(object result)
        {
            if (result is int height)
            {
                browserHeight = height;

                zoomFactor = (browserHeight / 878);

            }
        }


        private void diagUpRightHover_CheckedChanged(object sender, EventArgs e)
        {
            if (isLoadingProfile)
            {
                return;
            }

            if (diagUpRightHover.Checked)
            {
                mergeUpRight.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(315) translate(10,5)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#00a7d1' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
                btnUpRightHover.Text = "Up-Right";
                cmbUpRightHover.Items[0] = "Up-Right"; switchUpRightHover.Items[0] = "Up-Right";
                cmbUpRightHover.SelectedIndex = 0; switchUpRightHover.SelectedIndex = 0;
                cmbUpRightHover.Enabled = false; switchUpRightHover.Enabled = false; btnUpRightHover.Enabled = false;
                diagUpRightHover.ToolTipText = "Click to Uncombine";
                mergeUpRight.ToolTipText = "Click to Uncombine";
            }
            else
            {
                mergeUpRight.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(315) translate(10,5)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#636372' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
                btnUpRightHover.Text = "Backtick";
                cmbUpRightHover.Items[0] = "Click to Set"; switchUpRightHover.Items[0] = "Click to Set";
                cmbUpRightHover.Enabled = true; switchUpRightHover.Enabled = true; btnUpRightHover.Enabled = true;
                diagUpRightHover.ToolTipText = "Click to Combine";
                mergeUpRight.ToolTipText = "Click to Combine";
            }
        }

        private void diagDownRightHover_CheckedChanged(object sender, EventArgs e)
        {
            if (isLoadingProfile)
            {
                return;
            }

            if (diagDownRightHover.Checked)
            {
                mergeDownRight.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(45) translate(10,10)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#00a7d1' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
                btnDownRightHover.Text = "Down-Right";
                cmbDownRightHover.Items[0] = "Down-Right"; switchDownRightHover.Items[0] = "Down-Right";
                cmbDownRightHover.SelectedIndex = 0; switchDownRightHover.SelectedIndex = 0;
                cmbDownRightHover.Enabled = false; switchDownRightHover.Enabled = false; btnDownRightHover.Enabled = false;
                diagDownRightHover.ToolTipText = "Click to Uncombine";
                mergeDownRight.ToolTipText = "Click to Uncombine";
            }
            else
            {
                mergeDownRight.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(45) translate(10,10)'><path transform='rotate(0) scale(2.9) translate(0,-25)' fill='#636372' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
                btnDownRightHover.Text = "Backtick";
                cmbDownRightHover.Items[0] = "Click to Set"; switchDownRightHover.Items[0] = "Click to Set";
                cmbDownRightHover.SelectedIndex = 0; switchDownRightHover.SelectedIndex = 0;
                cmbDownRightHover.Enabled = true; switchDownRightHover.Enabled = true; btnDownRightHover.Enabled = true;
                diagDownRightHover.ToolTipText = "Click to Combine";
                mergeDownRight.ToolTipText = "Click to Combine";
            }
        }

        private void diagDownLeftHover_CheckedChanged(object sender, EventArgs e)
        {
            if (isLoadingProfile)
            {
                return;
            }

            if (diagDownLeftHover.Checked)
            {
                mergeDownLeft.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(135) translate(11,11)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#00a7d1' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
                btnDownLeftHover.Text = "Down-Left";
                cmbDownLeftHover.Items[0] = "Down-Left"; switchDownLeftHover.Items[0] = "Down-Left";
                cmbDownLeftHover.SelectedIndex = 0; switchDownLeftHover.SelectedIndex = 0;
                cmbDownLeftHover.Enabled = false; switchDownLeftHover.Enabled = false; btnDownLeftHover.Enabled = false;
                diagDownLeftHover.ToolTipText = "Click to Uncombine";
                mergeDownLeft.ToolTipText = "Click to Uncombine";
            }
            else
            {
                mergeDownLeft.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(135) translate(11,11)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#636372' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
                btnDownLeftHover.Text = "Backtick";
                cmbDownLeftHover.Items[0] = "Click to Set"; switchDownLeftHover.Items[0] = "Click to Set";
                cmbDownLeftHover.SelectedIndex = 0; switchDownLeftHover.SelectedIndex = 0;
                cmbDownLeftHover.Enabled = true; switchDownLeftHover.Enabled = true; btnDownLeftHover.Enabled = true;
                diagDownLeftHover.ToolTipText = "Click to Combine";
                mergeDownLeft.ToolTipText = "Click to Combine";
            }
        }

        private void diagUpLeftHover_CheckedChanged(object sender, EventArgs e)
        {
            if (isLoadingProfile)
            {
                return;
            }

            if (diagUpLeftHover.Checked)
            {
                mergeUpLeft.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(225) translate(11,11)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#00a7d1' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
                btnUpLeftHover.Text = "Up-Left";
                cmbUpLeftHover.Items[0] = "Up-Left"; switchUpLeftHover.Items[0] = "Up-Left";
                cmbUpLeftHover.SelectedIndex = 0; switchUpLeftHover.SelectedIndex = 0;
                cmbUpLeftHover.Enabled = false; switchUpLeftHover.Enabled = false; btnUpLeftHover.Enabled = false;
                diagUpLeftHover.ToolTipText = "Click to Uncombine";
                mergeUpLeft.ToolTipText = "Click to Uncombine";
            }
            else
            {
                mergeUpLeft.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(225) translate(11,11)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#636372' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
                btnUpLeftHover.Text = "Backtick";
                cmbUpLeftHover.Items[0] = "Click to Set"; switchUpLeftHover.Items[0] = "Click to Set";
                cmbUpLeftHover.SelectedIndex = 0; switchUpLeftHover.SelectedIndex = 0;
                cmbUpLeftHover.Enabled = true; switchUpLeftHover.Enabled = true; btnUpLeftHover.Enabled = true;
                diagUpLeftHover.ToolTipText = "Click to Combine";
                mergeUpLeft.ToolTipText = "Click to Combine";
            }
        }

        private void diagUpRightLC_CheckedChanged(object sender, EventArgs e)
        {
            if (isLoadingProfile)
            {
                return;
            }

            if (diagUpRightLC.Checked)
            {
                mergeUpRight.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(315) translate(10,5)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#00a7d1' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
                btnUpRightLclick.Text = "Up-Right";
                cmbUpRightLclick.Items[0] = "Up-Right"; switchUpRightLC.Items[0] = "Up-Right";
                cmbUpRightLclick.SelectedIndex = 0; switchUpRightLC.SelectedIndex = 0;
                cmbUpRightLclick.Enabled = false; switchUpRightLC.Enabled = false; btnUpRightLclick.Enabled = false;
                diagUpRightLC.ToolTipText = "Click to Uncombine";
                mergeUpRight.ToolTipText = "Click to Uncombine";
            }
            else
            {
                mergeUpRight.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(315) translate(10,5)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#636372' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
                btnUpRightLclick.Text = "Backtick";
                cmbUpRightLclick.Items[0] = "Click to Set"; switchUpRightLC.Items[0] = "Click to Set";
                cmbUpRightLclick.SelectedIndex = 0; switchUpRightLC.SelectedIndex = 0;
                cmbUpRightLclick.Enabled = true; switchUpRightLC.Enabled = true; btnUpRightLclick.Enabled = true;
                diagUpRightLC.ToolTipText = "Click to Combine";
                mergeUpRight.ToolTipText = "Click to Combine";

            }
        }

        private void diagDownRightLC_CheckedChanged(object sender, EventArgs e)
        {
            if (isLoadingProfile)
            {
                return;
            }

            if (diagDownRightLC.Checked)
            {
                mergeDownRight.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(45) translate(10,10)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#00a7d1' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
                btnDownRightLclick.Text = "Down-Right";
                cmbDownRightLclick.Items[0] = "Down-Right"; switchDownRightLC.Items[0] = "Down-Right";
                cmbDownRightLclick.SelectedIndex = 0; switchDownRightLC.SelectedIndex = 0;
                cmbDownRightLclick.Enabled = false; switchDownRightLC.Enabled = false; btnDownRightLclick.Enabled = false;
                diagDownRightLC.ToolTipText = "Click to Uncombine";
                mergeDownRight.ToolTipText = "Click to Uncombine";
            }
            else
            {
                mergeDownRight.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(45) translate(10,10)'><path transform='rotate(0) scale(2.9) translate(0,-25)' fill='#636372' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
                btnDownRightLclick.Text = "Backtick";
                cmbDownRightLclick.Items[0] = "Click to Set"; switchDownRightLC.Items[0] = "Click to Set";
                cmbDownRightLclick.SelectedIndex = 0; switchDownRightLC.SelectedIndex = 0;
                cmbDownRightLclick.Enabled = true; switchDownRightLC.Enabled = true; btnDownRightLclick.Enabled = true;
                diagDownRightLC.ToolTipText = "Click to Combine";
                mergeDownRight.ToolTipText = "Click to Combine";
            }
        }

        private void diagDownLeftLC_CheckedChanged(object sender, EventArgs e)
        {
            if (isLoadingProfile)
            {
                return;
            }

            if (diagDownLeftLC.Checked)
            {
                mergeDownLeft.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(135) translate(11,11)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#00a7d1' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
                btnDownLeftLclick.Text = "Down-Left";
                cmbDownLeftLclick.Items[0] = "Down-Left"; switchDownLeftLC.Items[0] = "Down-Left";
                cmbDownLeftLclick.SelectedIndex = 0; switchDownLeftLC.SelectedIndex = 0;
                cmbDownLeftLclick.Enabled = false; switchDownLeftLC.Enabled = false; btnDownLeftLclick.Enabled = false;
                diagDownLeftLC.ToolTipText = "Click to Uncombine";
                mergeDownLeft.ToolTipText = "Click to Uncombine";
            }
            else
            {
                mergeDownLeft.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(135) translate(11,11)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#636372' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
                btnDownLeftLclick.Text = "Backtick";
                cmbDownLeftLclick.Items[0] = "Click to Set"; switchDownLeftLC.Items[0] = "Click to Set";
                cmbDownLeftLclick.SelectedIndex = 0; switchDownLeftLC.SelectedIndex = 0;
                cmbDownLeftLclick.Enabled = true; switchDownLeftLC.Enabled = true; btnDownLeftLclick.Enabled = true;
                diagDownLeftLC.ToolTipText = "Click to Combine";
                mergeDownLeft.ToolTipText = "Click to Combine";
            }
        }

        private void diagUpLeftLC_CheckedChanged(object sender, EventArgs e)
        {
            if (isLoadingProfile)
            {
                return;
            }

            if (diagUpLeftLC.Checked)
            {
                mergeUpLeft.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(225) translate(11,11)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#00a7d1' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
                btnUpLeftLclick.Text = "Up-Left";
                cmbUpLeftLclick.Items[0] = "Up-Left"; switchUpLeftLC.Items[0] = "Up-Left";
                cmbUpLeftLclick.SelectedIndex = 0; switchUpLeftLC.SelectedIndex = 0;
                cmbUpLeftLclick.Enabled = false; switchUpLeftLC.Enabled = false; btnUpLeftLclick.Enabled = false;
                diagUpLeftLC.ToolTipText = "Click to Uncombine";
                mergeUpLeft.ToolTipText = "Click to Uncombine";
            }
            else
            {
                mergeUpLeft.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(225) translate(11,11)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#636372' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
                btnUpLeftLclick.Text = "Backtick";
                cmbUpLeftLclick.Items[0] = "Click to Set"; switchUpLeftLC.Items[0] = "Click to Set";
                cmbUpLeftLclick.SelectedIndex = 0; switchUpLeftLC.SelectedIndex = 0;
                cmbUpLeftLclick.Enabled = true; switchUpLeftLC.Enabled = true; btnUpLeftLclick.Enabled = true;
                diagUpLeftLC.ToolTipText = "Click to Combine";
                mergeUpLeft.ToolTipText = "Click to Combine";
            }
        }

        private void diagUpRightRC_CheckedChanged(object sender, EventArgs e)
        {
            if (isLoadingProfile)
            {
                return;
            }

            if (diagUpRightRC.Checked)
            {
                mergeUpRight.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(315) translate(10,5)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#00a7d1' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";

                btnUpRightRclick.Text = "Up-Right";
                cmbUpRightRclick.Items[0] = "Up-Right"; switchUpRightRC.Items[0] = "Up-Right";
                cmbUpRightRclick.SelectedIndex = 0; switchUpRightRC.SelectedIndex = 0;
                cmbUpRightRclick.Enabled = false; switchUpRightRC.Enabled = false; btnUpRightRclick.Enabled = false;
                diagUpRightRC.ToolTipText = "Click to Uncombine";
                mergeUpRight.ToolTipText = "Click to Uncombine";
            }
            else
            {
                mergeUpRight.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(315) translate(10,5)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#636372' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
                btnUpRightRclick.Text = "Backtick";
                cmbUpRightRclick.Items[0] = "Click to Set"; switchUpRightRC.Items[0] = "Click to Set";
                cmbUpRightRclick.SelectedIndex = 0; switchUpRightRC.SelectedIndex = 0;
                cmbUpRightRclick.Enabled = true; switchUpRightRC.Enabled = true; btnUpRightRclick.Enabled = true;
                diagUpRightRC.ToolTipText = "Click to Combine";
                mergeUpRight.ToolTipText = "Click to Combine";
            }
        }

        private void diagDownRightRC_CheckedChanged(object sender, EventArgs e)
        {
            if (isLoadingProfile)
            {
                return;
            }

            if (diagDownRightRC.Checked)
            {
                mergeDownRight.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(45) translate(10,10)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#00a7d1' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
                btnDownRightRclick.Text = "Down-Right";
                cmbDownRightRclick.Items[0] = "Down-Right"; switchDownRightRC.Items[0] = "Down-Right";
                cmbDownRightRclick.SelectedIndex = 0; switchDownRightRC.SelectedIndex = 0;
                cmbDownRightRclick.Enabled = false; switchDownRightRC.Enabled = false; btnDownRightRclick.Enabled = false;
                diagDownRightRC.ToolTipText = "Click to Uncombine";
                mergeDownRight.ToolTipText = "Click to Uncombine";
            }
            else
            {
                mergeDownRight.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(45) translate(10,10)'><path transform='rotate(0) scale(2.9) translate(0,-25)' fill='#636372' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
                btnDownRightRclick.Text = "Backtick";
                cmbDownRightRclick.Items[0] = "Click to Set"; switchDownRightRC.Items[0] = "Click to Set";
                cmbDownRightRclick.SelectedIndex = 0; switchDownRightRC.SelectedIndex = 0;
                cmbDownRightRclick.Enabled = true; switchDownRightRC.Enabled = true; btnDownRightRclick.Enabled = true;
                diagDownRightRC.ToolTipText = "Click to Combine";
                mergeDownRight.ToolTipText = "Click to Combine";
            }
        }

        private void diagDownLeftRC_CheckedChanged(object sender, EventArgs e)
        {
            if (isLoadingProfile)
            {
                return;
            }

            if (diagDownLeftRC.Checked)
            {
                mergeDownLeft.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(135) translate(11,11)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#00a7d1' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
                btnDownLeftRclick.Text = "Down-Left";
                cmbDownLeftRclick.Items[0] = "Down-Left"; switchDownLeftRC.Items[0] = "Down-Left";
                cmbDownLeftRclick.SelectedIndex = 0; switchDownLeftRC.SelectedIndex = 0;
                cmbDownLeftRclick.Enabled = false; switchDownLeftRC.Enabled = false; btnDownLeftRclick.Enabled = false;
                diagDownLeftRC.ToolTipText = "Click to Uncombine";
                mergeDownLeft.ToolTipText = "Click to Uncombine";
            }
            else
            {
                mergeDownLeft.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(135) translate(11,11)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#636372' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
                btnDownLeftRclick.Text = "Backtick";
                cmbDownLeftRclick.Items[0] = "Click to Set"; switchDownLeftRC.Items[0] = "Click to Set";
                cmbDownLeftRclick.SelectedIndex = 0; switchDownLeftRC.SelectedIndex = 0;
                cmbDownLeftRclick.Enabled = true; switchDownLeftRC.Enabled = true; btnDownLeftRclick.Enabled = true;
                diagDownLeftRC.ToolTipText = "Click to Combine";
            }
        }

        private void diagUpLeftRC_CheckedChanged(object sender, EventArgs e)
        {
            if (isLoadingProfile)
            {
                return;
            }

            if (diagUpLeftRC.Checked)
            {
                mergeUpLeft.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(225) translate(11,11)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#00a7d1' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
                btnUpLeftRclick.Text = "Up-Left";
                cmbUpLeftRclick.Items[0] = "Up-Left"; switchUpLeftRC.Items[0] = "Up-Left";
                cmbUpLeftRclick.SelectedIndex = 0; switchUpLeftRC.SelectedIndex = 0;
                cmbUpLeftRclick.Enabled = false; switchUpLeftRC.Enabled = false; btnUpLeftRclick.Enabled = false;
                diagUpLeftRC.ToolTipText = "Click to Uncombine";
                mergeUpLeft.ToolTipText = "Click to Uncombine";

            }
            else
            {
                mergeUpLeft.Html = "<svg width='60' height='60' viewBox='0 0 800 800' transform='rotate(225) translate(11,11)'><path transform = 'rotate(0) scale(2.9) translate(0,-25)' fill='#636372' d='M10.5,49.9c0.4,17.7,0.8,22.5,2.5,28.9c2.5,9.5,7.4,20,12.9,27.7c4.5,6.2,14.8,16.4,19.6,19.2c1.6,1,3,2,3,2.3c0,0.3-2.2,1.9-4.8,3.7c-6.8,4.6-17.3,15.9-22,23.9c-8.7,14.7-11.5,27.9-11.5,55.2v16.5h11h11v-18.9c0-15.8,0.3-20.2,1.9-26.4c4.1-16.1,14.9-29.5,29.6-36.8c13.7-6.7,14-6.8,80.7-6.8h59.2l-14.3,14.3L175,167l2.8,3.1c1.5,1.7,5,5.4,7.6,8.2l4.8,5l27.9-27.9c15.4-15.4,28-28.2,28-28.4c0-0.3-12.4-13.1-27.5-28.5L191,70.3l-7.2,7.4c-3.9,4.1-7.3,7.9-7.4,8.4c-0.2,0.6,6.3,7.6,14.3,15.7l14.7,14.7h-60.6c-67.9,0-68.4-0.1-81.6-6.6C48.8,102.9,38.3,89.5,34.1,73c-1.6-6.1-1.9-10.6-1.9-25.9V28.7H21.1H10L10.5,49.9z'/></svg>";
                btnUpLeftRclick.Text = "Backtick";
                cmbUpLeftRclick.Items[0] = "Click to Set"; switchUpLeftRC.Items[0] = "Click to Set";
                cmbUpLeftRclick.SelectedIndex = 0; switchUpLeftRC.SelectedIndex = 0;
                cmbUpLeftRclick.Enabled = true; switchUpLeftRC.Enabled = true; btnUpLeftRclick.Enabled = true;
                diagUpLeftRC.ToolTipText = "Click to Combine";
                mergeUpLeft.ToolTipText = "Click to Combine";
            }
        }

        private void lblActiveLower_Click(object sender, EventArgs e)
        {

            lblActiveUpper.CssStyle = "font-weight:normal!important";
            lblActiveMiddle.CssStyle = "font-weight:normal!important;";
            lblActiveLower.CssStyle = "font-weight:bold!important";

            holdDZLowerLC.Checked = false; holdDZLowerRC.Checked = false;


        }

        private void lblActiveMiddle_Click(object sender, EventArgs e)
        {

            lblActiveUpper.CssStyle = "font-weight:normal!important";
            lblActiveMiddle.CssStyle = "font-weight:bold!important;";
            lblActiveLower.CssStyle = "font-weight:normal!important";

            holdDZMiddleLC.Checked = false; holdDZMiddleRC.Checked = false;
        }

        private void lblActiveUpper_Click(object sender, EventArgs e)
        {

            lblActiveUpper.CssStyle = "font-weight:bold!important";
            lblActiveMiddle.CssStyle = "font-weight:normal!important;";
            lblActiveLower.CssStyle = "font-weight:normal!important";

            holdDZUpperLC.Checked = false; holdDZUpperRC.Checked = false; 
        }

        private void lblHover_Click(object sender, EventArgs e)
        {
            btnHover.PerformClick();
        }

        private void lblLeftClick_Click(object sender, EventArgs e)
        {

            btnLeftClick.PerformClick();
        }

        private void lblRightClick_Click(object sender, EventArgs e)
        {

            btnRightClick.PerformClick();
        }

        private void mergeUpRight_Click(object sender, EventArgs e)
        {
            if (lblHover.CssStyle.Contains("bold"))
            {
                if (diagUpRightHover.Checked)
                {
                    diagUpRightHover.Checked = false;
                }
                else
                {
                    diagUpRightHover.Checked = true;
                }
            }

            else if (lblLeftClick.CssStyle.Contains("bold"))
            {



                if (diagUpRightLC.Checked)
                {
                    diagUpRightLC.Checked = false;
                }
                else
                {
                    diagUpRightLC.Checked = true;
                }
            }
            else if (lblRightClick.CssStyle.Contains("bold"))
            {
                actionType = " you Right Click in";


                if (diagUpRightRC.Checked)
                {
                    diagUpRightRC.Checked = false;
                }
                else
                {
                    diagUpRightRC.Checked = true;
                }
            }
        }

        private void NormalizeHoverVariableCheckBoxes()
        {
            CheckBox[] variableBoxes =
            {
                varUpHover,
                varUpRightHover,
                varRightHover,
                varDownRightHover,
                varDownHover,
                varDownLeftHover,
                varLeftHover,
                varUpLeftHover
            };

            ComboBox[] xboxCombos =
            {
                cmbUpHover,
                cmbUpRightHover,
                cmbRightHover,
                cmbDownRightHover,
                cmbDownHover,
                cmbDownLeftHover,
                cmbLeftHover,
                cmbUpLeftHover
            };

            ComboBox[] switchCombos =
            {
                switchUpHover,
                switchUpRightHover,
                switchRightHover,
                switchDownRightHover,
                switchDownHover,
                switchDownLeftHover,
                switchLeftHover,
                switchUpLeftHover
            };

            if (chkKeyboard != null && chkKeyboard.Checked)
            {
                foreach (CheckBox variableBox in variableBoxes)
                {
                    if (variableBox != null)
                    {
                        variableBox.Checked = false;
                    }
                }

                return;
            }

            if (chkXbox != null && chkXbox.Checked)
            {
                for (int i = 0; i < variableBoxes.Length; i++)
                {
                    if (variableBoxes[i] != null && !IsHoverVariableEligible(i))
                    {

                        variableBoxes[i].Checked = false;
                    }

                }

                return;
            }

            if (nintendoSwitch != null && nintendoSwitch.Checked)
            {
                for (int i = 0; i < variableBoxes.Length; i++)
                {
                    if (variableBoxes[i] != null && !IsHoverVariableEligible(i))
                    {
                        variableBoxes[i].Checked = false;
                    }
                }
            }
        }

        private bool IsHoverVariableEligible(int index)
        {
            if (chkXbox != null && chkXbox.Checked)
            {
                return IsHoverVariableEligible(index, GetXboxHoverCombo, IsXboxVariableEligibleAction);
            }

            if (nintendoSwitch != null && nintendoSwitch.Checked)
            {
                return IsHoverVariableEligible(index, GetSwitchHoverCombo, IsSwitchVariableEligibleAction);
            }

            return false;
        }

        private bool IsHoverVariableEligible(int index, Func<int, ComboBox> getCombo, Func<string, bool> isActionEligible)
        {
            if (IsHoverVariableEligibleAction(getCombo(index), isActionEligible))
            {
                return true;
            }

            if (!IsHoverDiagonalCombined(index) || !TryGetHoverAdjacentIndices(index, out int firstAdjacent, out int secondAdjacent))
            {
                return false;
            }

            return IsHoverVariableEligibleAction(getCombo(firstAdjacent), isActionEligible)
                || IsHoverVariableEligibleAction(getCombo(secondAdjacent), isActionEligible);
        }

        private static bool IsHoverVariableEligibleAction(ComboBox comboBox, Func<string, bool> isActionEligible)
        {
            return comboBox != null && isActionEligible(GetSelectedActionText(comboBox));
        }

        private bool IsHoverDiagonalCombined(int index)
        {
            switch (index)
            {
                case 1:
                    return diagUpRightHover != null && diagUpRightHover.Checked;
                case 3:
                    return diagDownRightHover != null && diagDownRightHover.Checked;
                case 5:
                    return diagDownLeftHover != null && diagDownLeftHover.Checked;
                case 7:
                    return diagUpLeftHover != null && diagUpLeftHover.Checked;
                default:
                    return false;
            }
        }

        private static bool TryGetHoverAdjacentIndices(int index, out int firstAdjacent, out int secondAdjacent)
        {
            switch (index)
            {
                case 1:
                    firstAdjacent = 0;
                    secondAdjacent = 2;
                    return true;
                case 3:
                    firstAdjacent = 2;
                    secondAdjacent = 4;
                    return true;
                case 5:
                    firstAdjacent = 4;
                    secondAdjacent = 6;
                    return true;
                case 7:
                    firstAdjacent = 6;
                    secondAdjacent = 0;
                    return true;
                default:
                    firstAdjacent = -1;
                    secondAdjacent = -1;
                    return false;
            }
        }

        private ComboBox GetXboxHoverCombo(int index)
        {
            switch (index)
            {
                case 0: return cmbUpHover;
                case 1: return cmbUpRightHover;
                case 2: return cmbRightHover;
                case 3: return cmbDownRightHover;
                case 4: return cmbDownHover;
                case 5: return cmbDownLeftHover;
                case 6: return cmbLeftHover;
                case 7: return cmbUpLeftHover;
                default: return null;
            }
        }

        private ComboBox GetSwitchHoverCombo(int index)
        {
            switch (index)
            {
                case 0: return switchUpHover;
                case 1: return switchUpRightHover;
                case 2: return switchRightHover;
                case 3: return switchDownRightHover;
                case 4: return switchDownHover;
                case 5: return switchDownLeftHover;
                case 6: return switchLeftHover;
                case 7: return switchUpLeftHover;
                default: return null;
            }
        }

        private ComboBox GetHoverOptionCombo(int index)
        {
            switch (index)
            {
                case 0: return optUp;
                case 1: return optUpRight;
                case 2: return optRight;
                case 3: return optDownRight;
                case 4: return optDown;
                case 5: return optDownLeft;
                case 6: return optLeft;
                case 7: return optUpLeft;
                default: return null;
            }
        }

        private bool IsSwitchVariableEligibleAction(string action)
        {
            return !string.IsNullOrEmpty(action) && !nonVariableSwitch.Contains(action);
        }

        private void UpdateHoverVariableOption(int index)
        {
            ComboBox optionCombo = GetHoverOptionCombo(index);
            if (optionCombo == null)
            {
                return;
            }

            if (IsHoverVariableEligible(index))
            {
                if (!optionCombo.Items.Contains(optVariable))
                {
                    optionCombo.Items.Add(optVariable);
                }
            }
            else
            {
                optionCombo.Items.Remove(optVariable);
            }
        }

        private void UpdateAllHoverVariableOptions()
        {
            if (isLoadingProfile || isApplyingDisableButtonState)
            {
                return;
            }

            for (int i = 0; i < 8; i++)
            {
                UpdateHoverVariableOption(i);
            }

            RefreshDisableButtonsByClick();
        }

        private static string GetSelectedActionText(ComboBox comboBox)
        {
            string action = comboBox?.SelectedItem?.ToString() ?? "None";
            return string.IsNullOrEmpty(action) || action.Equals("Click to Set", StringComparison.OrdinalIgnoreCase)
                ? "None"
                : action;
        }

        private static bool HasDiagonalAction(ComboBox xboxComboBox, ComboBox switchComboBox, string diagonalAction)
        {
            return string.Equals(GetSelectedActionText(xboxComboBox), diagonalAction, StringComparison.OrdinalIgnoreCase)
                || string.Equals(GetSelectedActionText(switchComboBox), diagonalAction, StringComparison.OrdinalIgnoreCase);
        }

        private void ApplyLegacyCombineDefaults()
        {
            diagUpRightHover.Checked = HasDiagonalAction(cmbUpRightHover, switchUpRightHover, "Up-Right");
            diagUpRightLC.Checked = HasDiagonalAction(cmbUpRightLclick, switchUpRightLC, "Up-Right");
            diagUpRightRC.Checked = HasDiagonalAction(cmbUpRightRclick, switchUpRightRC, "Up-Right");

            diagUpLeftHover.Checked = HasDiagonalAction(cmbUpLeftHover, switchUpLeftHover, "Up-Left");
            diagUpLeftLC.Checked = HasDiagonalAction(cmbUpLeftLclick, switchUpLeftLC, "Up-Left");
            diagUpLeftRC.Checked = HasDiagonalAction(cmbUpLeftRclick, switchUpLeftRC, "Up-Left");
        }

        private static bool HasLoadedDiagonalAction(string[] configRow, string diagonalAction)
        {
            return configRow != null
                && configRow.Length > 0
                && string.Equals(configRow[0], diagonalAction, StringComparison.OrdinalIgnoreCase);
        }

        private void SyncDiagonalCombineStatesFromLoadedActions()
        {
            diagUpRightHover.Checked = HasLoadedDiagonalAction(buttonConfig[1], "Up-Right");
            diagUpRightLC.Checked = HasLoadedDiagonalAction(buttonConfig[9], "Up-Right");
            diagUpRightRC.Checked = HasLoadedDiagonalAction(buttonConfig[17], "Up-Right");

            diagDownRightHover.Checked = HasLoadedDiagonalAction(buttonConfig[3], "Down-Right");
            diagDownRightLC.Checked = HasLoadedDiagonalAction(buttonConfig[11], "Down-Right");
            diagDownRightRC.Checked = HasLoadedDiagonalAction(buttonConfig[19], "Down-Right");

            diagUpLeftHover.Checked = HasLoadedDiagonalAction(buttonConfig[7], "Up-Left");
            diagUpLeftLC.Checked = HasLoadedDiagonalAction(buttonConfig[15], "Up-Left");
            diagUpLeftRC.Checked = HasLoadedDiagonalAction(buttonConfig[23], "Up-Left");

            diagDownLeftHover.Checked = HasLoadedDiagonalAction(buttonConfig[5], "Down-Left");
            diagDownLeftLC.Checked = HasLoadedDiagonalAction(buttonConfig[13], "Down-Left");
            diagDownLeftRC.Checked = HasLoadedDiagonalAction(buttonConfig[21], "Down-Left");
        }

        private static bool IsStickAction(string action)
        {
            return !string.IsNullOrEmpty(action) &&
                (action.StartsWith("LS ", StringComparison.OrdinalIgnoreCase) ||
                 action.StartsWith("RS ", StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsXboxVariableEligibleAction(string action)
        {
            return IsStickAction(action) ||
                string.Equals(action, "LT or L2", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(action, "RT or R2", StringComparison.OrdinalIgnoreCase);
        }

        private void mergeUpLeft_Click(object sender, EventArgs e)
        {

            if (lblHover.CssStyle.Contains("bold"))
            {
                if (diagUpLeftHover.Checked)
                {
                    diagUpLeftHover.Checked = false;
                }
                else
                {
                    diagUpLeftHover.Checked = true;
                }
            }
            else if (lblLeftClick.CssStyle.Contains("bold"))
            {
                if (diagUpLeftLC.Checked)
                {
                    diagUpLeftLC.Checked = false;
                }
                else
                {
                    diagUpLeftLC.Checked = true;
                }
            }
            else if (lblRightClick.CssStyle.Contains("bold"))
            {
                actionType = " you Right Click in";
                if (diagUpLeftRC.Checked)
                {
                    diagUpLeftRC.Checked = false;
                }
                else
                {
                    diagUpLeftRC.Checked = true;
                }

            }
        }

        private void mergeDownLeft_Click(object sender, EventArgs e)
        {

            if (lblHover.CssStyle.Contains("bold"))
            {
                if (diagDownLeftHover.Checked)
                {
                    diagDownLeftHover.Checked = false;
                }
                else
                {
                    diagDownLeftHover.Checked = true;
                }
            }
            else if (lblLeftClick.CssStyle.Contains("bold"))
            {
                if (diagDownLeftLC.Checked)
                {
                    diagDownLeftLC.Checked = false;
                }
                else
                {
                    diagDownLeftLC.Checked = true;
                }
            }
            else if (lblRightClick.CssStyle.Contains("bold"))
            {
                actionType = " you Right Click in";
                if (diagDownLeftRC.Checked)
                {
                    diagDownLeftRC.Checked = false;
                }
                else
                {
                    diagDownLeftRC.Checked = true;
                }

            }
        }

        private void mergeDownRight_Click(object sender, EventArgs e)
        {

            if (lblHover.CssStyle.Contains("bold"))
            {
                if (diagDownRightHover.Checked)
                {
                    diagDownRightHover.Checked = false;
                }
                else
                {
                    diagDownRightHover.Checked = true;
                }
            }
            else if (lblLeftClick.CssStyle.Contains("bold"))
            {
                if (diagDownRightLC.Checked)
                {
                    diagDownRightLC.Checked = false;
                }
                else
                {
                    diagDownRightLC.Checked = true;
                }
            }
            else if (lblRightClick.CssStyle.Contains("bold"))
            {
                actionType = " you Right Click in";
                if (diagDownRightRC.Checked)
                {
                    diagDownRightRC.Checked = false;
                }
                else
                {
                    diagDownRightRC.Checked = true;
                }

            }
        }

        private void checkBox1_Click(object sender, EventArgs e)
        {

            nintendoSwitch.Checked = false;
            chkXbox.Checked = false;
            Eval(@"function applyMarquee(el) {  if ((el.dataset.marqueeApplied === 'true' && el.querySelector('.marquee-outer')) || el.querySelector('.truncate')){ return;}

  let text = el.textContent.trim();
  if (!text) return;

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
}, 2000); // 2000 milliseconds = 2 seconds
");
        }


        private void actUp_Click(object sender, EventArgs e)
        {
            string actionType = "";

            if (lblHover.CssStyle.Contains("bold"))
            {
                actionType = " you Move Your Mouse in";

                if (optUp.Items.Contains(optToggle))
                {

                    if (togUpHover.Checked)
                    {


                        //simplify
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant holds a button down until{actionType}to it again.'>This quadrant holds a button down until" + actionType + "to it again.", "MM▲ - Turn Off 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);




                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togUpHover.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpHover.Checked = false;
                            togUpRightHover.Checked = false;
                            togRightHover.Checked = false;
                            togDownRightHover.Checked = false;
                            togDownHover.Checked = false;
                            togDownLeftHover.Checked = false;
                            togLeftHover.Checked = false;
                            togUpLeftHover.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to hold a button down until{actionType}to it again?'>Would you like this quadrant to hold a button down until" + actionType + "to it again?", "MM▲ - Turn On 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togUpHover.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpHover.Checked = true;
                            togUpRightHover.Checked = true;
                            togRightHover.Checked = true;
                            togDownRightHover.Checked = true;
                            togDownHover.Checked = true;
                            togDownLeftHover.Checked = true;
                            togLeftHover.Checked = true;
                            togUpLeftHover.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (optUp.Items.Contains(optOnce))
                {

                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: darkblue !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid darkblue !important; border-bottom: 1px solid darkblue !important; border-left: 1px solid darkblue !important; }'; document.head.appendChild(style);}");

                    if (onceUp.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant presses a button once (instead of holding) when{actionType}to it.'>This quadrant presses a button once (instead of holding) when" + actionType + "to it.", "MM▲ - Turn Off 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            onceUp.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {


                            onceUp.Checked = false;
                            onceRight.Checked = false;
                            onceDownRight.Checked = false;
                            onceDown.Checked = false;
                            onceDownLeft.Checked = false;
                            onceLeft.Checked = false;
                            onceUpLeft.Checked = false;
                            onceUpRight.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to press a button once (instead of holding it) when{actionType}to it?'>Would you like this quadrant to press a button once (instead of holding it) when" + actionType + "to it?", "MM▲ - Turn On 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            onceUp.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            onceUp.Checked = true;
                            onceUpRight.Checked = true;
                            onceRight.Checked = true;
                            onceDownRight.Checked = true;
                            onceDown.Checked = true;
                            onceDownLeft.Checked = true;
                            onceLeft.Checked = true;
                            onceUpLeft.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (IsHoverVariableEligible(0))
                {

                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: black !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid black !important; border-bottom: 1px solid black !important; border-left: 1px solid black !important; }'; document.head.appendChild(style);}");

                    if (varUpHover.Checked)
                    {
                        ExtendedDialogResult LobjResult;

                        if (chkXbox.Checked)
                        {

                            LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant pushes the analog stick further in set direction (or the trigger further down) as you Move Your Mouse away from the center.'>This quadrant pushes the analog stick further in set direction (or the trigger further down) as you Move Your Mouse away from the center.</span></p>", "MM▲ - Turn Off 'Variable Sticks & Triggers' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        }
                        else
                        {
                            LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant pushes the analog stick further in set direction as you Move Your Mouse away from the center.'>This quadrant pushes the analog stick further in set direction as you Move Your Mouse away from the center.</span></p>", "MM▲ - Turn Off 'Variable Sticks' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        }

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            varUpHover.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {


                            varUpHover.Checked = false;
                            varUpRightHover.Checked = false;
                            varRightHover.Checked = false;
                            varDownRightHover.Checked = false;
                            varDownHover.Checked = false;
                            varDownLeftHover.Checked = false;
                            varLeftHover.Checked = false;
                            varUpLeftHover.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult;

                        if (chkXbox.Checked)
                        {

                            LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to push the analog stick further in set direction (or the trigger further down) as you Move Your Mouse away from the center?'>Would you like to push the analog stick further in set direction (or the trigger further down) as you Move Your Mouse away from the center?</span></p>", "MM▲ - Turn On 'Variable Sticks & Triggers' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);

                        }
                        else
                        {
                            LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to push the analog stick further in set direction as you Move Your Mouse away from the center?'>Would you like to push the analog stick further in set direction as you Move Your Mouse away from the center?</span></p>", "MM▲ - Turn On 'Variable Sticks' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);

                        }
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            varUpHover.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            varUpHover.Checked = true;
                            varUpRightHover.Checked = true;
                            varRightHover.Checked = true;
                            varDownRightHover.Checked = true;
                            varDownHover.Checked = true;
                            varDownLeftHover.Checked = true;
                            varLeftHover.Checked = true;
                            varUpLeftHover.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }


            }

            else if (lblLeftClick.CssStyle.Contains("bold"))
            {
                actionType = " you Left Click in";

                if (optUp.Items.Contains(optToggle))
                {

                    if (togUpLC.Checked)
                    {


                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant holds a button down until{actionType} it again.'>This quadrant holds a button down until" + actionType + " it again.", "LC▲ - Turn Off 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togUpLC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpLC.Checked = false;
                            togUpRightLC.Checked = false;
                            togRightLC.Checked = false;
                            togDownRightLC.Checked = false;
                            togDownLC.Checked = false;
                            togDownLeftLC.Checked = false;
                            togLeftLC.Checked = false;
                            togUpLeftLC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to hold a button down until{actionType} it again?'>Would you like this quadrant to hold a button down until" + actionType + " it again?", "LC▲ - Turn On 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togUpLC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpLC.Checked = true;
                            togUpRightLC.Checked = true;
                            togRightLC.Checked = true;
                            togDownRightLC.Checked = true;
                            togDownLC.Checked = true;
                            togDownLeftLC.Checked = true;
                            togLeftLC.Checked = true;
                            togUpLeftLC.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (optUp.Items.Contains(optRTC))
                {
                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: darkblue !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid darkblue !important; border-bottom: 1px solid darkblue !important; border-left: 1px solid darkblue !important; }'; document.head.appendChild(style);}");


                    if (rtcUpLC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant returns your mouse to the center when you release your Left Click in it.'>This quadrant returns your mouse to the center when you release your Left Click in it.</span></p>", "LC▲ - Turn Off 'Return To Center On Click' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            rtcUpLC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {


                            rtcUpLC.Checked = false;
                            rtcUpLeftLC.Checked = false;
                            rtcUpRightLC.Checked = false;
                            rtcRightLC.Checked = false;
                            rtcDownRightLC.Checked = false;
                            rtcDownLC.Checked = false;
                            rtcDownLeftLC.Checked = false;
                            rtcLeftLC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to return your mouse to the center when you release your Left Click in it?'>Would you like this quadrant to return your mouse to the center when you release your Left Click in it?</span></p>", "LC▲ - Turn On 'Return To Center On Click' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            rtcUpLC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            rtcUpLC.Checked = true;
                            rtcUpLeftLC.Checked = true;
                            rtcUpRightLC.Checked = true;
                            rtcRightLC.Checked = true;
                            rtcDownRightLC.Checked = true;
                            rtcDownLC.Checked = true;
                            rtcDownLeftLC.Checked = true;
                            rtcLeftLC.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }
                Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: black !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid black !important; border-bottom: 1px solid black !important; border-left: 1px solid black !important; }'; document.head.appendChild(style);}");

                if (onceUpLC.Checked)
                {
                    ExtendedDialogResult LobjResult;

                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant fully presses a button once (instead of pressing as long as you hold click) when{actionType} it.'>This quadrant fully presses a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "LC▲ - Turn Off 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);


                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceUpLC.Checked = false;

                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {


                        onceUpLC.Checked = false;
                        onceRightLC.Checked = false;
                        onceDownRightLC.Checked = false;
                        onceDownLC.Checked = false;
                        onceDownLeftLC.Checked = false;
                        onceLeftLC.Checked = false;
                        onceUpLeftLC.Checked = false;
                        onceUpRightLC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {

                    }
                    else
                    {
                        return;


                    }

                }
                else
                {
                    ExtendedDialogResult LobjResult;

                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to fully push a button once (instead of pressing as long as you hold click) when{actionType} it.'>Would you like to fully push a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "LC▲ - Turn On 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);

                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceUpLC.Checked = true;

                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceUpLC.Checked = true;
                        onceUpRightLC.Checked = true;
                        onceRightLC.Checked = true;
                        onceDownRightLC.Checked = true;
                        onceDownLC.Checked = true;
                        onceDownLeftLC.Checked = true;
                        onceLeftLC.Checked = true;
                        onceUpLeftLC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {

                    }
                    else
                    {
                        return;


                    }



                }
            }

            else if (lblRightClick.CssStyle.Contains("bold"))
            {
                actionType = " you Right Click in";

                if (optUp.Items.Contains(optToggle))
                {

                    if (togUpRC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant holds a button down until{actionType} it again.'>This quadrant holds a button down until" + actionType + " it again.", "RC▲ - Turn Off 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togUpRC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpRC.Checked = false;
                            togUpRightRC.Checked = false;
                            togRightRC.Checked = false;
                            togDownRightRC.Checked = false;
                            togDownRC.Checked = false;
                            togDownLeftRC.Checked = false;
                            togLeftRC.Checked = false;
                            togUpLeftRC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to hold a button down until{actionType} it again?'>Would you like this quadrant to hold a button down until" + actionType + " it again?", "RC▲ - Turn On 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togUpRC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpRC.Checked = true;
                            togUpRightRC.Checked = true;
                            togRightRC.Checked = true;
                            togDownRightRC.Checked = true;
                            togDownRC.Checked = true;
                            togDownLeftRC.Checked = true;
                            togLeftRC.Checked = true;
                            togUpLeftRC.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (optUp.Items.Contains(optRTC))
                {
                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: darkblue !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid darkblue !important; border-bottom: 1px solid darkblue !important; border-left: 1px solid darkblue !important; }'; document.head.appendChild(style);}");



                    if (rtcUpRC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant returns your mouse to the center when you release your Right Click in it.'>This quadrant returns your mouse to the center when you release your Right Click in it.</span></p>", "RC▲ - Turn Off 'Return To Center On Click' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            rtcUpRC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {


                            rtcUpRC.Checked = false;
                            rtcUpLeftRC.Checked = false;
                            rtcUpRightRC.Checked = false;
                            rtcRightRC.Checked = false;
                            rtcDownRightRC.Checked = false;
                            rtcDownRC.Checked = false;
                            rtcDownLeftRC.Checked = false;
                            rtcLeftRC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to return your mouse to the center when you release your Right Click in it?'>Would you like this quadrant to return your mouse to the center when you release your Right Click in it?</span></p>", "RC▲ - Turn On 'Return To Center On Click' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            rtcUpRC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            rtcUpRC.Checked = true;
                            rtcUpLeftRC.Checked = true;
                            rtcUpRightRC.Checked = true;
                            rtcRightRC.Checked = true;
                            rtcDownRightRC.Checked = true;
                            rtcDownRC.Checked = true;
                            rtcDownLeftRC.Checked = true;
                            rtcLeftRC.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }


                Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: black !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid black !important; border-bottom: 1px solid black !important; border-left: 1px solid black !important; }'; document.head.appendChild(style);}");

                if (onceUpRC.Checked)
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant fully presses a button once (instead of pressing as long as you hold click) when{actionType} it.'>This quadrant fully presses a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "RC▲ - Turn Off 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceUpRC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceUpRC.Checked = false;
                        onceRightRC.Checked = false;
                        onceDownRightRC.Checked = false;
                        onceDownRC.Checked = false;
                        onceDownLeftRC.Checked = false;
                        onceLeftRC.Checked = false;
                        onceUpLeftRC.Checked = false;
                        onceUpRightRC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to fully push a button once (instead of pressing as long as you hold click) when{actionType} it.'>Would you like to fully push a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "RC▲ - Turn On 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceUpRC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceUpRC.Checked = true;
                        onceUpRightRC.Checked = true;
                        onceRightRC.Checked = true;
                        onceDownRightRC.Checked = true;
                        onceDownRC.Checked = true;
                        onceDownLeftRC.Checked = true;
                        onceLeftRC.Checked = true;
                        onceUpLeftRC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
            }
            Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); }");


        }

        private void actUpRight_Click(object sender, EventArgs e)
        {
            if (lblHover.CssStyle.Contains("bold"))
            {
                actionType = " you Move Your Mouse in";

                if (optUpRight.Items.Contains(optToggle))
                {
                    if (togUpRightHover.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden;' title='This quadrant holds a button down until{actionType}to it again.'>This quadrant holds a button down until" + actionType + "to it again.", "MM◥ - Turn Off 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togUpRightHover.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpHover.Checked = false;
                            togUpRightHover.Checked = false;
                            togRightHover.Checked = false;
                            togDownRightHover.Checked = false;
                            togDownHover.Checked = false;
                            togDownLeftHover.Checked = false;
                            togLeftHover.Checked = false;
                            togUpLeftHover.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to hold a button down until{actionType}to it again?'>Would you like this quadrant to hold a button down until" + actionType + "to it again?", "MM◥ - Turn On 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togUpRightHover.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpHover.Checked = true;
                            togUpRightHover.Checked = true;
                            togRightHover.Checked = true;
                            togDownRightHover.Checked = true;
                            togDownHover.Checked = true;
                            togDownLeftHover.Checked = true;
                            togLeftHover.Checked = true;
                            togUpLeftHover.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (optUpRight.Items.Contains(optOnce))
                {
                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: darkblue !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid darkblue !important; border-bottom: 1px solid darkblue !important; border-left: 1px solid darkblue !important; }'; document.head.appendChild(style);}");

                    if (onceUpRight.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant presses a button once (instead of holding) when{actionType}to it.'>This quadrant presses a button once (instead of holding) when" + actionType + "to it.", "MM◥ - Turn Off 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            onceUpRight.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {


                            onceUp.Checked = false;
                            onceRight.Checked = false;
                            onceDownRight.Checked = false;
                            onceDown.Checked = false;
                            onceDownLeft.Checked = false;
                            onceLeft.Checked = false;
                            onceUpLeft.Checked = false;
                            onceUpRight.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to press a button once (instead of holding it) when{actionType}to it?'>Would you like this quadrant to press a button once (instead of holding it) when" + actionType + "to it?", "MM◥ - Turn On 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            onceUpRight.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            onceUp.Checked = true;
                            onceUpRight.Checked = true;
                            onceRight.Checked = true;
                            onceDownRight.Checked = true;
                            onceDown.Checked = true;
                            onceDownLeft.Checked = true;
                            onceLeft.Checked = true;
                            onceUpLeft.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (IsHoverVariableEligible(1))
                {
                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: black !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid black !important; border-bottom: 1px solid black !important; border-left: 1px solid black !important; }'; document.head.appendChild(style);}");
                    if (varUpRightHover.Checked)
                    {
                        ExtendedDialogResult LobjResult;

                        if (chkXbox.Checked)
                        {

                            LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant pushes the analog stick further in set direction (or the trigger further down) as you Move Your Mouse away from the center.'>This quadrant pushes the analog stick further in set direction (or the trigger further down) as you Move Your Mouse away from the center.</span></p>", "MM◥ - Turn Off 'Variable Sticks & Triggers' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        }
                        else
                        {
                            LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant pushes the analog stick further in set direction as you Move Your Mouse away from the center.'>This quadrant pushes the analog stick further in set direction as you Move Your Mouse away from the center.</span></p>", "MM◥ - Turn Off 'Variable Sticks & Triggers' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        }

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            varUpRightHover.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {


                            varUpHover.Checked = false;
                            varUpRightHover.Checked = false;
                            varRightHover.Checked = false;
                            varDownRightHover.Checked = false;
                            varDownHover.Checked = false;
                            varDownLeftHover.Checked = false;
                            varLeftHover.Checked = false;
                            varUpLeftHover.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult;

                        if (chkXbox.Checked)
                        {

                            LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to push the analog stick further in set direction (or the trigger further down) as you Move Your Mouse away from the center?'>Would you like to push the analog stick further in set direction (or the trigger further down) as you Move Your Mouse away from the center?</span></p>", "MM◥ - Turn On 'Variable Sticks & Triggers' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);

                        }
                        else
                        {
                            LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to push the analog stick further in set direction as you Move Your Mouse away from the center?'>Would you like to push the analog stick further in set direction as you Move Your Mouse away from the center?</span></p>", "MM◥ - Turn On 'Variable Sticks' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);

                        }
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            varUpRightHover.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            varUpHover.Checked = true;
                            varUpRightHover.Checked = true;
                            varRightHover.Checked = true;
                            varDownRightHover.Checked = true;
                            varDownHover.Checked = true;
                            varDownLeftHover.Checked = true;
                            varLeftHover.Checked = true;
                            varUpLeftHover.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

            }

            else if (lblLeftClick.CssStyle.Contains("bold"))
            {
                actionType = " you Left Click in";

                if (optUpRight.Items.Contains(optToggle))
                {
                    if (togUpRightLC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant holds a button down until{actionType} it again.'>This quadrant holds a button down until" + actionType + " it again.", "LC◥ - Turn Off 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togUpRightLC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpLC.Checked = false;
                            togUpRightLC.Checked = false;
                            togRightLC.Checked = false;
                            togDownRightLC.Checked = false;
                            togDownLC.Checked = false;
                            togDownLeftLC.Checked = false;
                            togLeftLC.Checked = false;
                            togUpLeftLC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to hold a button down until{actionType} it again?'>Would you like this quadrant to hold a button down until" + actionType + " it again?", "LC◥ - Turn On 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togUpRightLC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpLC.Checked = true;
                            togUpRightLC.Checked = true;
                            togRightLC.Checked = true;
                            togDownRightLC.Checked = true;
                            togDownLC.Checked = true;
                            togDownLeftLC.Checked = true;
                            togLeftLC.Checked = true;
                            togUpLeftLC.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (optUpRight.Items.Contains(optRTC))
                {
                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: darkblue !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid darkblue !important; border-bottom: 1px solid darkblue !important; border-left: 1px solid darkblue !important; }'; document.head.appendChild(style);}");
                    if (rtcUpRightLC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant returns your mouse to the center when you release your Left Click in it.'>This quadrant returns your mouse to the center when you release your Left Click in it.</span></p>", "LC◥ - Turn Off 'Return To Center On Click' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            rtcUpRightLC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {


                            rtcUpLC.Checked = false;
                            rtcUpLeftLC.Checked = false;
                            rtcUpRightLC.Checked = false;
                            rtcRightLC.Checked = false;
                            rtcDownRightLC.Checked = false;
                            rtcDownLC.Checked = false;
                            rtcDownLeftLC.Checked = false;
                            rtcLeftLC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to return your mouse to the center when you release your Left Click in it?'>Would you like this quadrant to return your mouse to the center when you release your Left Click in it?</span></p>", "LC◥ - Turn On 'Return To Center On Click' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            rtcUpRightLC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            rtcUpLC.Checked = true;
                            rtcUpLeftLC.Checked = true;
                            rtcUpRightLC.Checked = true;
                            rtcRightLC.Checked = true;
                            rtcDownRightLC.Checked = true;
                            rtcDownLC.Checked = true;
                            rtcDownLeftLC.Checked = true;
                            rtcLeftLC.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: black !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid black !important; border-bottom: 1px solid black !important; border-left: 1px solid black !important; }'; document.head.appendChild(style);}");

                if (onceUpRightLC.Checked)
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant fully presses a button once (instead of pressing as long as you hold click) when{actionType} it.'>This quadrant fully presses a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "LC◥ - Turn Off 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceUpRightLC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceUpLC.Checked = false;
                        onceRightLC.Checked = false;
                        onceDownRightLC.Checked = false;
                        onceDownLC.Checked = false;
                        onceDownLeftLC.Checked = false;
                        onceLeftLC.Checked = false;
                        onceUpLeftLC.Checked = false;
                        onceUpRightLC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to fully push a button once (instead of pressing as long as you hold click) when{actionType} it.'>Would you like to fully push a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "LC◥ - Turn On 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceUpRightLC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceUpLC.Checked = true;
                        onceUpRightLC.Checked = true;
                        onceRightLC.Checked = true;
                        onceDownRightLC.Checked = true;
                        onceDownLC.Checked = true;
                        onceDownLeftLC.Checked = true;
                        onceLeftLC.Checked = true;
                        onceUpLeftLC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
            }

            else if (lblRightClick.CssStyle.Contains("bold"))
            {
                actionType = " you Right Click in";

                if (optUpRight.Items.Contains(optToggle))
                {
                    if (togUpRightRC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant holds a button down until{actionType} it again.'>This quadrant holds a button down until" + actionType + " it again.", "RC◥ - Turn Off 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togUpRightRC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpRC.Checked = false;
                            togUpRightRC.Checked = false;
                            togRightRC.Checked = false;
                            togDownRightRC.Checked = false;
                            togDownRC.Checked = false;
                            togDownLeftRC.Checked = false;
                            togLeftRC.Checked = false;
                            togUpLeftRC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to hold a button down until{actionType} it again?'>Would you like this quadrant to hold a button down until" + actionType + " it again?", "RC◥ - Turn On 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togUpRightRC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpRC.Checked = true;
                            togUpRightRC.Checked = true;
                            togRightRC.Checked = true;
                            togDownRightRC.Checked = true;
                            togDownRC.Checked = true;
                            togDownLeftRC.Checked = true;
                            togLeftRC.Checked = true;
                            togUpLeftRC.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (optUpRight.Items.Contains(optRTC))
                {
                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: darkblue !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid darkblue !important; border-bottom: 1px solid darkblue !important; border-left: 1px solid darkblue !important; }'; document.head.appendChild(style);}");
                    if (rtcUpRightRC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant returns your mouse to the center when you release your Right Click in it.'>This quadrant returns your mouse to the center when you release your Right Click in it.</span></p>", "RC◥ - Turn Off 'Return To Center On Click' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            rtcUpRightRC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {


                            rtcUpRC.Checked = false;
                            rtcUpLeftRC.Checked = false;
                            rtcUpRightRC.Checked = false;
                            rtcRightRC.Checked = false;
                            rtcDownRightRC.Checked = false;
                            rtcDownRC.Checked = false;
                            rtcDownLeftRC.Checked = false;
                            rtcLeftRC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to return your mouse to the center when you release your Right Click in it?'>Would you like this quadrant to return your mouse to the center when you release your Right Click in it?</span></p>", "RC◥ - Turn On 'Return To Center On Click' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            rtcUpRightRC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            rtcUpRC.Checked = true;
                            rtcUpLeftRC.Checked = true;
                            rtcUpRightRC.Checked = true;
                            rtcRightRC.Checked = true;
                            rtcDownRightRC.Checked = true;
                            rtcDownRC.Checked = true;
                            rtcDownLeftRC.Checked = true;
                            rtcLeftRC.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: black !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid black !important; border-bottom: 1px solid black !important; border-left: 1px solid black !important; }'; document.head.appendChild(style);}");

                if (onceUpRightRC.Checked)
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant fully presses a button once (instead of pressing as long as you hold click) when{actionType} it.'>This quadrant fully presses a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "RC◥ - Turn Off 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceUpRightRC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceUpRC.Checked = false;
                        onceRightRC.Checked = false;
                        onceDownRightRC.Checked = false;
                        onceDownRC.Checked = false;
                        onceDownLeftRC.Checked = false;
                        onceLeftRC.Checked = false;
                        onceUpLeftRC.Checked = false;
                        onceUpRightRC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to fully push a button once (instead of pressing as long as you hold click) when{actionType} it.'>Would you like to fully push a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "RC◥ - Turn On 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceUpRightRC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceUpRC.Checked = true;
                        onceUpRightRC.Checked = true;
                        onceRightRC.Checked = true;
                        onceDownRightRC.Checked = true;
                        onceDownRC.Checked = true;
                        onceDownLeftRC.Checked = true;
                        onceLeftRC.Checked = true;
                        onceUpLeftRC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
            }

            Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); }");

        }

        private void actRight_Click(object sender, EventArgs e)
        {
            if (lblHover.CssStyle.Contains("bold"))
            {
                actionType = " you Move Your Mouse in";

                if (optRight.Items.Contains(optToggle))
                {
                    if (togRightHover.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant holds a button down until{actionType}to it again.'>This quadrant holds a button down until" + actionType + "to it again.", "MM▶ - Turn Off 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togRightHover.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpHover.Checked = false;
                            togUpRightHover.Checked = false;
                            togRightHover.Checked = false;
                            togDownRightHover.Checked = false;
                            togDownHover.Checked = false;
                            togDownLeftHover.Checked = false;
                            togLeftHover.Checked = false;
                            togUpLeftHover.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to hold a button down until you Move Your Mouse back into it?'>Would you like this quadrant to hold a button down until you Move Your Mouse back into it?</span></p>", "MM▶ - Turn On 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togRightHover.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpHover.Checked = true;
                            togUpRightHover.Checked = true;
                            togRightHover.Checked = true;
                            togDownRightHover.Checked = true;
                            togDownHover.Checked = true;
                            togDownLeftHover.Checked = true;
                            togLeftHover.Checked = true;
                            togUpLeftHover.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (optRight.Items.Contains(optOnce))
                {
                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: darkblue !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid darkblue !important; border-bottom: 1px solid darkblue !important; border-left: 1px solid darkblue !important; }'; document.head.appendChild(style);}");
                    if (onceRight.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant presses a button once (instead of holding) when{actionType}to it.'>This quadrant presses a button once (instead of holding) when" + actionType + "to it.", "MM▶ - Turn Off 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            onceRight.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {


                            onceUp.Checked = false;
                            onceRight.Checked = false;
                            onceDownRight.Checked = false;
                            onceDown.Checked = false;
                            onceDownLeft.Checked = false;
                            onceLeft.Checked = false;
                            onceUpLeft.Checked = false;
                            onceUpRight.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to press a button once (instead of holding it) when{actionType}to it?'>Would you like this quadrant to press a button once (instead of holding it) when" + actionType + "to it?", "MM▶ - Turn On 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            onceRight.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            onceUp.Checked = true;
                            onceUpRight.Checked = true;
                            onceRight.Checked = true;
                            onceDownRight.Checked = true;
                            onceDown.Checked = true;
                            onceDownLeft.Checked = true;
                            onceLeft.Checked = true;
                            onceUpLeft.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (IsHoverVariableEligible(2))
                {
                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: black !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid black !important; border-bottom: 1px solid black !important; border-left: 1px solid black !important; }'; document.head.appendChild(style);}");
                    if (varRightHover.Checked)
                    {
                        ExtendedDialogResult LobjResult;

                        if (chkXbox.Checked)
                        {

                            LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant pushes the analog stick further in set direction (or the trigger further down) as you Move Your Mouse away from the center.'>This quadrant pushes the analog stick further in set direction (or the trigger further down) as you Move Your Mouse away from the center.</span></p>", "MM▶ - Turn Off 'Variable Sticks & Triggers' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        }
                        else
                        {
                            LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant pushes the analog stick further in set direction as you Move Your Mouse away from the center.'>This quadrant pushes the analog stick further in set direction as you Move Your Mouse away from the center.</span></p>", "MM▶ - Turn Off 'Variable Sticks' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        }

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            varRightHover.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {


                            varUpHover.Checked = false;
                            varUpRightHover.Checked = false;
                            varRightHover.Checked = false;
                            varDownRightHover.Checked = false;
                            varDownHover.Checked = false;
                            varDownLeftHover.Checked = false;
                            varLeftHover.Checked = false;
                            varUpLeftHover.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult;

                        if (chkXbox.Checked)
                        {

                            LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to push the analog stick further in set direction (or the trigger further down) as you Move Your Mouse away from the center?'>Would you like to push the analog stick further in set direction (or the trigger further down) as you Move Your Mouse away from the center?</span></p>", "MM▶ - Turn On 'Variable Sticks & Triggers' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);

                        }
                        else
                        {
                            LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to push the analog stick further in set direction as you Move Your Mouse away from the center?'>Would you like to push the analog stick further in set direction as you Move Your Mouse away from the center?</span></p>", "MM▶ - Turn On 'Variable Sticks' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);

                        }
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            varRightHover.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            varUpHover.Checked = true;
                            varUpRightHover.Checked = true;
                            varRightHover.Checked = true;
                            varDownRightHover.Checked = true;
                            varDownHover.Checked = true;
                            varDownLeftHover.Checked = true;
                            varLeftHover.Checked = true;
                            varUpLeftHover.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

            }

            else if (lblLeftClick.CssStyle.Contains("bold"))
            {
                actionType = " you Left Click in";

                if (optRight.Items.Contains(optToggle))
                {
                    if (togRightLC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant holds a button down until{actionType} it again.'>This quadrant holds a button down until" + actionType + " it again.", "LC▶ - Turn Off 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togRightLC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpLC.Checked = false;
                            togUpRightLC.Checked = false;
                            togRightLC.Checked = false;
                            togDownRightLC.Checked = false;
                            togDownLC.Checked = false;
                            togDownLeftLC.Checked = false;
                            togLeftLC.Checked = false;
                            togUpLeftLC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to hold a button down until{actionType} it again?'>Would you like this quadrant to hold a button down until" + actionType + " it again?", "LC▶ - Turn On 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togRightLC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpLC.Checked = true;
                            togUpRightLC.Checked = true;
                            togRightLC.Checked = true;
                            togDownRightLC.Checked = true;
                            togDownLC.Checked = true;
                            togDownLeftLC.Checked = true;
                            togLeftLC.Checked = true;
                            togUpLeftLC.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (optRight.Items.Contains(optRTC))
                {
                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: darkblue !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid darkblue !important; border-bottom: 1px solid darkblue !important; border-left: 1px solid darkblue !important; }'; document.head.appendChild(style);}");
                    if (rtcRightLC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant returns your mouse to the center when you release your Left Click in it.'>This quadrant returns your mouse to the center when you release your Left Click in it.</span></p>", "LC▶ - Turn Off 'Return To Center On Click' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            rtcRightLC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {


                            rtcUpLC.Checked = false;
                            rtcUpLeftLC.Checked = false;
                            rtcUpRightLC.Checked = false;
                            rtcRightLC.Checked = false;
                            rtcDownRightLC.Checked = false;
                            rtcDownLC.Checked = false;
                            rtcDownLeftLC.Checked = false;
                            rtcLeftLC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to return your mouse to the center when you release your Left Click in it?'>Would you like this quadrant to return your mouse to the center when you release your Left Click in it?</span></p>", "LC▶ - Turn On 'Return To Center On Click' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            rtcRightLC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            rtcUpLC.Checked = true;
                            rtcUpLeftLC.Checked = true;
                            rtcUpRightLC.Checked = true;
                            rtcRightLC.Checked = true;
                            rtcDownRightLC.Checked = true;
                            rtcDownLC.Checked = true;
                            rtcDownLeftLC.Checked = true;
                            rtcLeftLC.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: black !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid black !important; border-bottom: 1px solid black !important; border-left: 1px solid black !important; }'; document.head.appendChild(style);}");

                if (onceRightLC.Checked)
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant fully presses a button once (instead of pressing as long as you hold click) when{actionType} it.'>This quadrant fully presses a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "LC▶ - Turn Off 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceRightLC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceUpLC.Checked = false;
                        onceRightLC.Checked = false;
                        onceDownRightLC.Checked = false;
                        onceDownLC.Checked = false;
                        onceDownLeftLC.Checked = false;
                        onceLeftLC.Checked = false;
                        onceUpLeftLC.Checked = false;
                        onceUpRightLC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to fully push a button once (instead of pressing as long as you hold click) when{actionType} it.'>Would you like to fully push a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "LC▶ - Turn On 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceRightLC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceUpLC.Checked = true;
                        onceUpRightLC.Checked = true;
                        onceRightLC.Checked = true;
                        onceDownRightLC.Checked = true;
                        onceDownLC.Checked = true;
                        onceDownLeftLC.Checked = true;
                        onceLeftLC.Checked = true;
                        onceUpLeftLC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
            }

            else if (lblRightClick.CssStyle.Contains("bold"))
            {
                actionType = " you Right Click in";

                if (optRight.Items.Contains(optToggle))
                {
                    if (togRightRC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant holds a button down until{actionType} it again.'>This quadrant holds a button down until" + actionType + " it again.", "RC▶ - Turn Off 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togRightRC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpRC.Checked = false;
                            togUpRightRC.Checked = false;
                            togRightRC.Checked = false;
                            togDownRightRC.Checked = false;
                            togDownRC.Checked = false;
                            togDownLeftRC.Checked = false;
                            togLeftRC.Checked = false;
                            togUpLeftRC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to hold a button down until{actionType} it again?'>Would you like this quadrant to hold a button down until" + actionType + " it again?", "RC▶ - Turn On 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togRightRC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpRC.Checked = true;
                            togUpRightRC.Checked = true;
                            togRightRC.Checked = true;
                            togDownRightRC.Checked = true;
                            togDownRC.Checked = true;
                            togDownLeftRC.Checked = true;
                            togLeftRC.Checked = true;
                            togUpLeftRC.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (optRight.Items.Contains(optRTC))
                {
                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: darkblue !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid darkblue !important; border-bottom: 1px solid darkblue !important; border-left: 1px solid darkblue !important; }'; document.head.appendChild(style);}");
                    if (rtcRightRC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to return your mouse to the center when you release your Rght Click in it?'>Would you like this quadrant to return your mouse to the center when you release your Rght Click in it?</span></p>", "RC▶ - Turn Off 'Return To Center On Click' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            rtcRightRC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {


                            rtcUpRC.Checked = false;
                            rtcUpLeftRC.Checked = false;
                            rtcUpRightRC.Checked = false;
                            rtcRightRC.Checked = false;
                            rtcDownRightRC.Checked = false;
                            rtcDownRC.Checked = false;
                            rtcDownLeftRC.Checked = false;
                            rtcLeftRC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to return your mouse to the center when you release your Right Click in it?'>Would you like this quadrant to return your mouse to the center when you release your Right Click in it?</span></p>", "RC▶ - Turn On 'Return To Center On Click' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            rtcRightRC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            rtcUpRC.Checked = true;
                            rtcUpLeftRC.Checked = true;
                            rtcUpRightRC.Checked = true;
                            rtcRightRC.Checked = true;
                            rtcDownRightRC.Checked = true;
                            rtcDownRC.Checked = true;
                            rtcDownLeftRC.Checked = true;
                            rtcLeftRC.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: black !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid black !important; border-bottom: 1px solid black !important; border-left: 1px solid black !important; }'; document.head.appendChild(style);}");

                if (onceRightRC.Checked)
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant fully presses a button once (instead of pressing as long as you hold click) when{actionType} it.'>This quadrant fully presses a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "RC▶ - Turn Off 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceRightRC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceUpRC.Checked = false;
                        onceRightRC.Checked = false;
                        onceDownRightRC.Checked = false;
                        onceDownRC.Checked = false;
                        onceDownLeftRC.Checked = false;
                        onceLeftRC.Checked = false;
                        onceUpLeftRC.Checked = false;
                        onceUpRightRC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to fully push a button once (instead of pressing as long as you hold click) when{actionType} it.'>Would you like to fully push a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "RC▶ - Turn On 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceRightRC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceUpRC.Checked = true;
                        onceUpRightRC.Checked = true;
                        onceRightRC.Checked = true;
                        onceDownRightRC.Checked = true;
                        onceDownRC.Checked = true;
                        onceDownLeftRC.Checked = true;
                        onceLeftRC.Checked = true;
                        onceUpLeftRC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
            }

            Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); }");

        }



        private void actDownRight_Click(object sender, EventArgs e)
        {
            if (lblHover.CssStyle.Contains("bold"))
            {
                actionType = " you Move Your Mouse in";

                if (optDownRight.Items.Contains(optToggle))
                {
                    if (togDownRightHover.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant holds a button down until{actionType}to it again.'>This quadrant holds a button down until" + actionType + "to it again.", "MM◢ - Turn Off 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDownRightHover.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpHover.Checked = false;
                            togUpRightHover.Checked = false;
                            togRightHover.Checked = false;
                            togDownRightHover.Checked = false;
                            togDownHover.Checked = false;
                            togDownLeftHover.Checked = false;
                            togLeftHover.Checked = false;
                            togUpLeftHover.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to hold a button down until{actionType}to it again?'>Would you like this quadrant to hold a button down until" + actionType + "to it again?", "MM◢ - Turn On 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDownRightHover.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpHover.Checked = true;
                            togUpRightHover.Checked = true;
                            togRightHover.Checked = true;
                            togDownRightHover.Checked = true;
                            togDownHover.Checked = true;
                            togDownLeftHover.Checked = true;
                            togLeftHover.Checked = true;
                            togUpLeftHover.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (optDownRight.Items.Contains(optOnce))
                {
                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: darkblue !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid darkblue !important; border-bottom: 1px solid darkblue !important; border-left: 1px solid darkblue !important; }'; document.head.appendChild(style);}");
                    if (onceDownRight.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant presses a button once (instead of holding) when{actionType}to it.'>This quadrant presses a button once (instead of holding) when" + actionType + "to it.", "MM◢ - Turn Off 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            onceDownRight.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {


                            onceUp.Checked = false;
                            onceRight.Checked = false;
                            onceDownRight.Checked = false;
                            onceDown.Checked = false;
                            onceDownLeft.Checked = false;
                            onceLeft.Checked = false;
                            onceUpLeft.Checked = false;
                            onceUpRight.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to press a button once (instead of holding it) when{actionType}to it?'>Would you like this quadrant to press a button once (instead of holding it) when" + actionType + "to it?", "MM◢ - Turn On 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            onceDownRight.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            onceUp.Checked = true;
                            onceUpRight.Checked = true;
                            onceRight.Checked = true;
                            onceDownRight.Checked = true;
                            onceDown.Checked = true;
                            onceDownLeft.Checked = true;
                            onceLeft.Checked = true;
                            onceUpLeft.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (IsHoverVariableEligible(3))
                {
                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: black !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid black !important; border-bottom: 1px solid black !important; border-left: 1px solid black !important; }'; document.head.appendChild(style);}");
                    if (varDownRightHover.Checked)
                    {
                        ExtendedDialogResult LobjResult;

                        if (chkXbox.Checked)
                        {

                            LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant pushes the analog stick further in set direction (or the trigger further down) as you Move Your Mouse away from the center.'>This quadrant pushes the analog stick further in set direction (or the trigger further down) as you Move Your Mouse away from the center.</span></p>", "MM◢ - Turn Off 'Variable Sticks & Triggers' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        }
                        else
                        {
                            LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant pushes the analog stick further in set direction as you Move Your Mouse away from the center.'>This quadrant pushes the analog stick further in set direction as you Move Your Mouse away from the center.</span></p>", "MM◢ - Turn Off 'Variable Sticks' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        }

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            varDownRightHover.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {


                            varUpHover.Checked = false;
                            varUpRightHover.Checked = false;
                            varRightHover.Checked = false;
                            varDownRightHover.Checked = false;
                            varDownHover.Checked = false;
                            varDownLeftHover.Checked = false;
                            varLeftHover.Checked = false;
                            varUpLeftHover.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult;

                        if (chkXbox.Checked)
                        {

                            LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to push the analog stick further in set direction (or the trigger further down) as you Move Your Mouse away from the center?'>Would you like to push the analog stick further in set direction (or the trigger further down) as you Move Your Mouse away from the center?</span></p>", "MM◢ - Turn On 'Variable Sticks & Triggers' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);

                        }
                        else
                        {
                            LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to push the analog stick further in set direction as you Move Your Mouse away from the center?'>Would you like to push the analog stick further in set direction as you Move Your Mouse away from the center?</span></p>", "MM◢ - Turn On 'Variable Sticks' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);

                        }
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            varDownRightHover.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            varUpHover.Checked = true;
                            varUpRightHover.Checked = true;
                            varRightHover.Checked = true;
                            varDownRightHover.Checked = true;
                            varDownHover.Checked = true;
                            varDownLeftHover.Checked = true;
                            varLeftHover.Checked = true;
                            varUpLeftHover.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

            }

            else if (lblLeftClick.CssStyle.Contains("bold"))
            {
                actionType = " you Left Click in";

                if (optDownRight.Items.Contains(optToggle))
                {
                    if (togDownRightLC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant holds a button down until{actionType} it again.'>This quadrant holds a button down until" + actionType + " it again.", "LC◢ - Turn Off 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDownRightLC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpLC.Checked = false;
                            togUpRightLC.Checked = false;
                            togRightLC.Checked = false;
                            togDownRightLC.Checked = false;
                            togDownLC.Checked = false;
                            togDownLeftLC.Checked = false;
                            togLeftLC.Checked = false;
                            togUpLeftLC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to hold a button down until{actionType} it again?'>Would you like this quadrant to hold a button down until" + actionType + " it again?", "LC◢ - Turn On 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDownRightLC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpLC.Checked = true;
                            togUpRightLC.Checked = true;
                            togRightLC.Checked = true;
                            togDownRightLC.Checked = true;
                            togDownLC.Checked = true;
                            togDownLeftLC.Checked = true;
                            togLeftLC.Checked = true;
                            togUpLeftLC.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (optDownRight.Items.Contains(optRTC))
                {
                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: darkblue !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid darkblue !important; border-bottom: 1px solid darkblue !important; border-left: 1px solid darkblue !important; }'; document.head.appendChild(style);}");
                    if (rtcDownRightLC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant returns your mouse to the center when you release your Left Click in it.'>This quadrant returns your mouse to the center when you release your Left Click in it.</span></p>", "LC◢ - Turn Off 'Return To Center On Click' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            rtcDownRightLC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {


                            rtcUpLC.Checked = false;
                            rtcUpLeftLC.Checked = false;
                            rtcUpRightLC.Checked = false;
                            rtcRightLC.Checked = false;
                            rtcDownRightLC.Checked = false;
                            rtcDownLC.Checked = false;
                            rtcDownLeftLC.Checked = false;
                            rtcLeftLC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to return your mouse to the center when you release your Left Click in it?'>Would you like this quadrant to return your mouse to the center when you release your Left Click in it?</span></p>", "LC◢ - Turn On 'Return To Center On Click' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            rtcDownRightLC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            rtcUpLC.Checked = true;
                            rtcUpLeftLC.Checked = true;
                            rtcUpRightLC.Checked = true;
                            rtcRightLC.Checked = true;
                            rtcDownRightLC.Checked = true;
                            rtcDownLC.Checked = true;
                            rtcDownLeftLC.Checked = true;
                            rtcLeftLC.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: black !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid black !important; border-bottom: 1px solid black !important; border-left: 1px solid black !important; }'; document.head.appendChild(style);}");

                if (onceDownRightLC.Checked)
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant fully presses a button once (instead of pressing as long as you hold click) when{actionType} it.'>This quadrant fully presses a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "LC◢ - Turn Off 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceDownRightLC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceUpLC.Checked = false;
                        onceRightLC.Checked = false;
                        onceDownRightLC.Checked = false;
                        onceDownLC.Checked = false;
                        onceDownLeftLC.Checked = false;
                        onceLeftLC.Checked = false;
                        onceUpLeftLC.Checked = false;
                        onceUpRightLC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to fully push a button once (instead of pressing as long as you hold click) when{actionType} it.'>Would you like to fully push a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "LC◢ - Turn On 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceDownRightLC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceUpLC.Checked = true;
                        onceUpRightLC.Checked = true;
                        onceRightLC.Checked = true;
                        onceDownRightLC.Checked = true;
                        onceDownLC.Checked = true;
                        onceDownLeftLC.Checked = true;
                        onceLeftLC.Checked = true;
                        onceUpLeftLC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }

            }

            else if (lblRightClick.CssStyle.Contains("bold"))
            {
                actionType = " you Right Click in";

                if (optDownRight.Items.Contains(optToggle))
                {
                    if (togDownRightRC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant holds a button down until{actionType} it again.'>This quadrant holds a button down until" + actionType + " it again.", "RC◢ - Turn Off 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDownRightRC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpRC.Checked = false;
                            togUpRightRC.Checked = false;
                            togRightRC.Checked = false;
                            togDownRightRC.Checked = false;
                            togDownRC.Checked = false;
                            togDownLeftRC.Checked = false;
                            togLeftRC.Checked = false;
                            togUpLeftRC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to hold a button down until{actionType} it again?'>Would you like this quadrant to hold a button down until" + actionType + " it again?", "RC◢ - Turn On 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDownRightRC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpRC.Checked = true;
                            togUpRightRC.Checked = true;
                            togRightRC.Checked = true;
                            togDownRightRC.Checked = true;
                            togDownRC.Checked = true;
                            togDownLeftRC.Checked = true;
                            togLeftRC.Checked = true;
                            togUpLeftRC.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (optDownRight.Items.Contains(optRTC))
                {
                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: darkblue !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid darkblue !important; border-bottom: 1px solid darkblue !important; border-left: 1px solid darkblue !important; }'; document.head.appendChild(style);}");
                    if (rtcDownRightRC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant returns your mouse to the center when you release your Right Click in it.'>This quadrant returns your mouse to the center when you release your Right Click in it.</span></p>", "RC◢ - Turn Off 'Return To Center On Click' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            rtcDownRightRC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {


                            rtcUpRC.Checked = false;
                            rtcUpLeftRC.Checked = false;
                            rtcUpRightRC.Checked = false;
                            rtcRightRC.Checked = false;
                            rtcDownRightRC.Checked = false;
                            rtcDownRC.Checked = false;
                            rtcDownLeftRC.Checked = false;
                            rtcLeftRC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to return your mouse to the center when you release your Right Click in it?'>Would you like this quadrant to return your mouse to the center when you release your Right Click in it?</span></p>", "RC◢ - Turn On 'Return To Center On Click' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            rtcDownRightRC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            rtcUpRC.Checked = true;
                            rtcUpLeftRC.Checked = true;
                            rtcUpRightRC.Checked = true;
                            rtcRightRC.Checked = true;
                            rtcDownRightRC.Checked = true;
                            rtcDownRC.Checked = true;
                            rtcDownLeftRC.Checked = true;
                            rtcLeftRC.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: black !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid black !important; border-bottom: 1px solid black !important; border-left: 1px solid black !important; }'; document.head.appendChild(style);}");

                if (onceDownRightRC.Checked)
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant fully presses a button once (instead of pressing as long as you hold click) when{actionType} it.'>This quadrant fully presses a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "RC◢ - Turn Off 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceDownRightRC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceUpRC.Checked = false;
                        onceRightRC.Checked = false;
                        onceDownRightRC.Checked = false;
                        onceDownRC.Checked = false;
                        onceDownLeftRC.Checked = false;
                        onceLeftRC.Checked = false;
                        onceUpLeftRC.Checked = false;
                        onceUpRightRC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to fully push a button once (instead of pressing as long as you hold click) when{actionType} it.'>Would you like to fully push a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "RC◢ - Turn On 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceDownRightRC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceUpRC.Checked = true;
                        onceUpRightRC.Checked = true;
                        onceRightRC.Checked = true;
                        onceDownRightRC.Checked = true;
                        onceDownRC.Checked = true;
                        onceDownLeftRC.Checked = true;
                        onceLeftRC.Checked = true;
                        onceUpLeftRC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }

            }

            Eval(@"var styleElement = 
                document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); }
            ");

        }

        private void actDown_Click(object sender, EventArgs e)
        {
            if (lblHover.CssStyle.Contains("bold"))
            {
                actionType = " you Move Your Mouse in";

                if (optDown.Items.Contains(optToggle))
                {
                    if (togDownHover.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant holds a button down until{actionType}to it again.'>This quadrant holds a button down until" + actionType + "to it again.", "MM▼ - Turn Off 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDownHover.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpHover.Checked = false;
                            togUpRightHover.Checked = false;
                            togRightHover.Checked = false;
                            togDownRightHover.Checked = false;
                            togDownHover.Checked = false;
                            togDownLeftHover.Checked = false;
                            togLeftHover.Checked = false;
                            togUpLeftHover.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to hold a button down until{actionType}to it again?'>Would you like this quadrant to hold a button down until" + actionType + "to it again?", "MM▼ - Turn On 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDownHover.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpHover.Checked = true;
                            togUpRightHover.Checked = true;
                            togRightHover.Checked = true;
                            togDownRightHover.Checked = true;
                            togDownHover.Checked = true;
                            togDownLeftHover.Checked = true;
                            togLeftHover.Checked = true;
                            togUpLeftHover.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (optDown.Items.Contains(optOnce))
                {
                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: darkblue !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid darkblue !important; border-bottom: 1px solid darkblue !important; border-left: 1px solid darkblue !important; }'; document.head.appendChild(style);}");
                    if (onceDown.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant presses a button once (instead of holding) when{actionType}to it.'>This quadrant presses a button once (instead of holding) when" + actionType + "to it.", "MM▼ - Turn Off 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            onceDown.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {


                            onceUp.Checked = false;
                            onceRight.Checked = false;
                            onceDownRight.Checked = false;
                            onceDown.Checked = false;
                            onceDownLeft.Checked = false;
                            onceLeft.Checked = false;
                            onceUpLeft.Checked = false;
                            onceUpRight.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to press a button once (instead of holding it) when{actionType}to it?'>Would you like this quadrant to press a button once (instead of holding it) when" + actionType + "to it?", "MM▼ - Turn On 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            onceDown.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            onceUp.Checked = true;
                            onceUpRight.Checked = true;
                            onceRight.Checked = true;
                            onceDownRight.Checked = true;
                            onceDown.Checked = true;
                            onceDownLeft.Checked = true;
                            onceLeft.Checked = true;
                            onceUpLeft.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (IsHoverVariableEligible(4))
                {
                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: black !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid black !important; border-bottom: 1px solid black !important; border-left: 1px solid black !important; }'; document.head.appendChild(style);}");
                    if (varDownHover.Checked)
                    {
                        ExtendedDialogResult LobjResult;

                        if (chkXbox.Checked)
                        {

                            LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant pushes the analog stick further in set direction (or the trigger further down) as you Move Your Mouse away from the center.'>This quadrant pushes the analog stick further in set direction (or the trigger further down) as you Move Your Mouse away from the center.</span></p>", "MM▼ - Turn Off 'Variable Sticks & Triggers' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        }
                        else
                        {
                            LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant pushes the analog stick further in set direction as you Move Your Mouse away from the center.'>This quadrant pushes the analog stick further in set direction as you Move Your Mouse away from the center.</span></p>", "MM▼ - Turn Off 'Variable Sticks' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        }

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            varDownHover.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {


                            varUpHover.Checked = false;
                            varUpRightHover.Checked = false;
                            varRightHover.Checked = false;
                            varDownRightHover.Checked = false;
                            varDownHover.Checked = false;
                            varDownLeftHover.Checked = false;
                            varLeftHover.Checked = false;
                            varUpLeftHover.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult;

                        if (chkXbox.Checked)
                        {

                            LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to push the analog stick further in set direction (or the trigger further down) as you Move Your Mouse away from the center?'>Would you like to push the analog stick further in set direction (or the trigger further down) as you Move Your Mouse away from the center?</span></p>", "MM▼ - Turn On 'Variable Sticks & Triggers' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);

                        }
                        else
                        {
                            LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to push the analog stick further in set direction as you Move Your Mouse away from the center?'>Would you like to push the analog stick further in set direction as you Move Your Mouse away from the center?</span></p>", "MM▼ - Turn On 'Variable Sticks' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);

                        }
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            varDownHover.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            varUpHover.Checked = true;
                            varUpRightHover.Checked = true;
                            varRightHover.Checked = true;
                            varDownRightHover.Checked = true;
                            varDownHover.Checked = true;
                            varDownLeftHover.Checked = true;
                            varLeftHover.Checked = true;
                            varUpLeftHover.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

            }

            else if (lblLeftClick.CssStyle.Contains("bold"))
            {
                actionType = " you Left Click in";

                if (optDown.Items.Contains(optToggle))
                {
                    if (togDownLC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant holds a button down until{actionType} it again.'>This quadrant holds a button down until" + actionType + " it again.", "LC▼ - Turn Off 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDownLC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpLC.Checked = false;
                            togUpRightLC.Checked = false;
                            togRightLC.Checked = false;
                            togDownRightLC.Checked = false;
                            togDownLC.Checked = false;
                            togDownLeftLC.Checked = false;
                            togLeftLC.Checked = false;
                            togUpLeftLC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to hold a button down until{actionType} it again?'>Would you like this quadrant to hold a button down until" + actionType + " it again?", "LC▼ - Turn On 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDownLC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpLC.Checked = true;
                            togUpRightLC.Checked = true;
                            togRightLC.Checked = true;
                            togDownRightLC.Checked = true;
                            togDownLC.Checked = true;
                            togDownLeftLC.Checked = true;
                            togLeftLC.Checked = true;
                            togUpLeftLC.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (optDown.Items.Contains(optRTC))
                {
                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: darkblue !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid darkblue !important; border-bottom: 1px solid darkblue !important; border-left: 1px solid darkblue !important; }'; document.head.appendChild(style);}");
                    if (rtcDownLC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant returns your mouse to the center when you release your Left Click in it.'>This quadrant returns your mouse to the center when you release your Left Click in it.</span></p>", "LC▼ - Turn Off 'Return To Center On Click' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            rtcDownLC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {


                            rtcUpLC.Checked = false;
                            rtcUpLeftLC.Checked = false;
                            rtcUpRightLC.Checked = false;
                            rtcRightLC.Checked = false;
                            rtcDownRightLC.Checked = false;
                            rtcDownLC.Checked = false;
                            rtcDownLeftLC.Checked = false;
                            rtcLeftLC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to return your mouse to the center when you release your Left Click in it?'>Would you like this quadrant to return your mouse to the center when you release your Left Click in it?</span></p>", "LC▼ - Turn On 'Return To Center On Click' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            rtcDownLC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            rtcUpLC.Checked = true;
                            rtcUpLeftLC.Checked = true;
                            rtcUpRightLC.Checked = true;
                            rtcRightLC.Checked = true;
                            rtcDownRightLC.Checked = true;
                            rtcDownLC.Checked = true;
                            rtcDownLeftLC.Checked = true;
                            rtcLeftLC.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: black !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid black !important; border-bottom: 1px solid black !important; border-left: 1px solid black !important; }'; document.head.appendChild(style);}");

                if (onceDownLC.Checked)
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant fully presses a button once (instead of pressing as long as you hold click) when{actionType} it.'>This quadrant fully presses a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "LC▼ - Turn Off 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceDownLC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceUpLC.Checked = false;
                        onceRightLC.Checked = false;
                        onceDownRightLC.Checked = false;
                        onceDownLC.Checked = false;
                        onceDownLeftLC.Checked = false;
                        onceLeftLC.Checked = false;
                        onceUpLeftLC.Checked = false;
                        onceUpRightLC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to fully push a button once (instead of pressing as long as you hold click) when{actionType} it.'>Would you like to fully push a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "LC▼ - Turn On 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceDownLC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceUpLC.Checked = true;
                        onceUpRightLC.Checked = true;
                        onceRightLC.Checked = true;
                        onceDownRightLC.Checked = true;
                        onceDownLC.Checked = true;
                        onceDownLeftLC.Checked = true;
                        onceLeftLC.Checked = true;
                        onceUpLeftLC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
            }

            else if (lblRightClick.CssStyle.Contains("bold"))
            {
                actionType = " you Right Click in";

                if (optDown.Items.Contains(optToggle))
                {
                    if (togDownRC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant holds a button down until{actionType} it again.'>This quadrant holds a button down until" + actionType + " it again.", "RC▼ - Turn Off 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDownRC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpRC.Checked = false;
                            togUpRightRC.Checked = false;
                            togRightRC.Checked = false;
                            togDownRightRC.Checked = false;
                            togDownRC.Checked = false;
                            togDownLeftRC.Checked = false;
                            togLeftRC.Checked = false;
                            togUpLeftRC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to hold a button down until{actionType} it again?'>Would you like this quadrant to hold a button down until" + actionType + " it again?", "RC▼ - Turn On 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDownRC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpRC.Checked = true;
                            togUpRightRC.Checked = true;
                            togRightRC.Checked = true;
                            togDownRightRC.Checked = true;
                            togDownRC.Checked = true;
                            togDownLeftRC.Checked = true;
                            togLeftRC.Checked = true;
                            togUpLeftRC.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (optDown.Items.Contains(optRTC))
                {
                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: darkblue !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid darkblue !important; border-bottom: 1px solid darkblue !important; border-left: 1px solid darkblue !important; }'; document.head.appendChild(style);}");
                    if (rtcDownRC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant returns your mouse to the center when you release your Right Click in it.'>This quadrant returns your mouse to the center when you release your Right Click in it.</span></p>", "RC▼ - Turn Off 'Return To Center On Click' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            rtcDownRC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {


                            rtcUpRC.Checked = false;
                            rtcUpLeftRC.Checked = false;
                            rtcUpRightRC.Checked = false;
                            rtcRightRC.Checked = false;
                            rtcDownRightRC.Checked = false;
                            rtcDownRC.Checked = false;
                            rtcDownLeftRC.Checked = false;
                            rtcLeftRC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to return your mouse to the center when you release your Right Click in it?'>Would you like this quadrant to return your mouse to the center when you release your Right Click in it?</span></p>", "RC▼ - Turn On 'Return To Center On Click' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            rtcDownRC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            rtcUpRC.Checked = true;
                            rtcUpLeftRC.Checked = true;
                            rtcUpRightRC.Checked = true;
                            rtcRightRC.Checked = true;
                            rtcDownRightRC.Checked = true;
                            rtcDownRC.Checked = true;
                            rtcDownLeftRC.Checked = true;
                            rtcLeftRC.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: black !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid black !important; border-bottom: 1px solid black !important; border-left: 1px solid black !important; }'; document.head.appendChild(style);}");

                if (onceDownRC.Checked)
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant fully presses a button once (instead of pressing as long as you hold click) when{actionType} it.'>This quadrant fully presses a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "RC▼ - Turn Off 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceDownRC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceUpRC.Checked = false;
                        onceRightRC.Checked = false;
                        onceDownRightRC.Checked = false;
                        onceDownRC.Checked = false;
                        onceDownLeftRC.Checked = false;
                        onceLeftRC.Checked = false;
                        onceUpLeftRC.Checked = false;
                        onceUpRightRC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to fully push a button once (instead of pressing as long as you hold click) when{actionType} it.'>Would you like to fully push a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "RC▼ - Turn On 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceDownRC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceUpRC.Checked = true;
                        onceUpRightRC.Checked = true;
                        onceRightRC.Checked = true;
                        onceDownRightRC.Checked = true;
                        onceDownRC.Checked = true;
                        onceDownLeftRC.Checked = true;
                        onceLeftRC.Checked = true;
                        onceUpLeftRC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
            }

            Eval(@"var styleElement = 
                document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); }");


        }

        private void actUpLeft_Click(object sender, EventArgs e)
        {
            if (lblHover.CssStyle.Contains("bold"))
            {
                actionType = " you Move Your Mouse in";

                if (optUpLeft.Items.Contains(optToggle))
                {
                    if (togUpLeftHover.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant holds a button down until{actionType}to it again.'>This quadrant holds a button down until" + actionType + "to it again.", "MM◤ - Turn Off 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togUpLeftHover.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpHover.Checked = false;
                            togUpRightHover.Checked = false;
                            togRightHover.Checked = false;
                            togDownRightHover.Checked = false;
                            togDownHover.Checked = false;
                            togDownLeftHover.Checked = false;
                            togLeftHover.Checked = false;
                            togUpLeftHover.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to hold a button down until{actionType}to it again?'>Would you like this quadrant to hold a button down until" + actionType + "to it again?", "MM◤ - Turn On 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togUpLeftHover.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpHover.Checked = true;
                            togUpRightHover.Checked = true;
                            togRightHover.Checked = true;
                            togDownRightHover.Checked = true;
                            togDownHover.Checked = true;
                            togDownLeftHover.Checked = true;
                            togLeftHover.Checked = true;
                            togUpLeftHover.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (optUpLeft.Items.Contains(optOnce))
                {
                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: darkblue !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid darkblue !important; border-bottom: 1px solid darkblue !important; border-left: 1px solid darkblue !important; }'; document.head.appendChild(style);}");
                    if (onceUpLeft.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant presses a button once (instead of holding) when{actionType}to it.'>This quadrant presses a button once (instead of holding) when" + actionType + "to it.", "MM◤ - Turn Off 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            onceUpLeft.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {


                            onceUp.Checked = false;
                            onceRight.Checked = false;
                            onceDownRight.Checked = false;
                            onceDown.Checked = false;
                            onceDownLeft.Checked = false;
                            onceLeft.Checked = false;
                            onceUpLeft.Checked = false;
                            onceUpRight.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to press a button once (instead of holding it) when{actionType}to it?'>Would you like this quadrant to press a button once (instead of holding it) when" + actionType + "to it?", "MM◤ - Turn On 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            onceUpLeft.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            onceUp.Checked = true;
                            onceUpRight.Checked = true;
                            onceRight.Checked = true;
                            onceDownRight.Checked = true;
                            onceDown.Checked = true;
                            onceDownLeft.Checked = true;
                            onceLeft.Checked = true;
                            onceUpLeft.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (IsHoverVariableEligible(7))
                {
                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: black !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid black !important; border-bottom: 1px solid black !important; border-left: 1px solid black !important; }'; document.head.appendChild(style);}");
                    if (varUpLeftHover.Checked)
                    {
                        ExtendedDialogResult LobjResult;

                        if (chkXbox.Checked)
                        {

                            LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant pushes the analog stick further in set direction (or the trigger further down) as you Move Your Mouse away from the center.'>This quadrant pushes the analog stick further in set direction (or the trigger further down) as you Move Your Mouse away from the center.</span></p>", "MM◤ - Turn Off 'Variable Sticks & Triggers' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        }
                        else
                        {
                            LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant pushes the analog stick further in set direction as you Move Your Mouse away from the center.'>This quadrant pushes the analog stick further in set direction as you Move Your Mouse away from the center.</span></p>", "MM◤ - Turn Off 'Variable Sticks' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        }

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            varUpLeftHover.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {


                            varUpHover.Checked = false;
                            varUpRightHover.Checked = false;
                            varRightHover.Checked = false;
                            varDownRightHover.Checked = false;
                            varDownHover.Checked = false;
                            varDownLeftHover.Checked = false;
                            varLeftHover.Checked = false;
                            varUpLeftHover.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult;

                        if (chkXbox.Checked)
                        {

                            LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to push the analog stick further in set direction (or the trigger further down) as you Move Your Mouse away from the center?'>Would you like to push the analog stick further in set direction (or the trigger further down) as you Move Your Mouse away from the center?</span></p>", "MM◤ - Turn On 'Variable Sticks & Triggers' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);

                        }
                        else
                        {
                            LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to push the analog stick further in set direction as you Move Your Mouse away from the center?'>Would you like to push the analog stick further in set direction as you Move Your Mouse away from the center?</span></p>", "MM◤ - Turn On 'Variable Sticks' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);

                        }
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            varUpLeftHover.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            varUpHover.Checked = true;
                            varUpRightHover.Checked = true;
                            varRightHover.Checked = true;
                            varDownRightHover.Checked = true;
                            varDownHover.Checked = true;
                            varDownLeftHover.Checked = true;
                            varLeftHover.Checked = true;
                            varUpLeftHover.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

            }

            else if (lblLeftClick.CssStyle.Contains("bold"))
            {
                actionType = " you Left Click in";

                if (optUpLeft.Items.Contains(optToggle))
                {
                    if (togUpLeftLC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant holds a button down until{actionType} it again.'>This quadrant holds a button down until" + actionType + " it again.", "LC◤ - Turn Off 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togUpLeftLC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpLC.Checked = false;
                            togUpRightLC.Checked = false;
                            togRightLC.Checked = false;
                            togDownRightLC.Checked = false;
                            togDownLC.Checked = false;
                            togDownLeftLC.Checked = false;
                            togLeftLC.Checked = false;
                            togUpLeftLC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to hold a button down until{actionType} it again?'>Would you like this quadrant to hold a button down until" + actionType + " it again?", "LC◤ - Turn On 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togUpLeftLC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpLC.Checked = true;
                            togUpRightLC.Checked = true;
                            togRightLC.Checked = true;
                            togDownRightLC.Checked = true;
                            togDownLC.Checked = true;
                            togDownLeftLC.Checked = true;
                            togLeftLC.Checked = true;
                            togUpLeftLC.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (optUpLeft.Items.Contains(optRTC))
                {
                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: darkblue !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid darkblue !important; border-bottom: 1px solid darkblue !important; border-left: 1px solid darkblue !important; }'; document.head.appendChild(style);}");
                    if (rtcUpLeftLC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant returns your mouse to the center when you release your Left Click in it.'>This quadrant returns your mouse to the center when you release your Left Click in it.</span></p>", "LC◤ - Turn Off 'Return To Center On Click' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            rtcUpLeftLC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {


                            rtcUpLC.Checked = false;
                            rtcUpLeftLC.Checked = false;
                            rtcUpRightLC.Checked = false;
                            rtcRightLC.Checked = false;
                            rtcDownRightLC.Checked = false;
                            rtcDownLC.Checked = false;
                            rtcDownLeftLC.Checked = false;
                            rtcLeftLC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to return your mouse to the center when you release your Left Click in it?'>Would you like this quadrant to return your mouse to the center when you release your Left Click in it?</span></p>", "LC◤ - Turn On 'Return To Center On Click' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            rtcUpLeftLC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            rtcUpLC.Checked = true;
                            rtcUpLeftLC.Checked = true;
                            rtcUpRightLC.Checked = true;
                            rtcRightLC.Checked = true;
                            rtcDownRightLC.Checked = true;
                            rtcDownLC.Checked = true;
                            rtcDownLeftLC.Checked = true;
                            rtcLeftLC.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: black !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid black !important; border-bottom: 1px solid black !important; border-left: 1px solid black !important; }'; document.head.appendChild(style);}");

                if (onceUpLeftLC.Checked)
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant fully presses a button once (instead of pressing as long as you hold click) when{actionType} it.'>This quadrant fully presses a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "LC◤ - Turn Off 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceUpLeftLC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceUpLC.Checked = false;
                        onceRightLC.Checked = false;
                        onceDownRightLC.Checked = false;
                        onceDownLC.Checked = false;
                        onceDownLeftLC.Checked = false;
                        onceLeftLC.Checked = false;
                        onceUpLeftLC.Checked = false;
                        onceUpRightLC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to fully push a button once (instead of pressing as long as you hold click) when{actionType} it.'>Would you like to fully push a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "LC◤ - Turn On 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceUpLeftLC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceUpLC.Checked = true;
                        onceUpRightLC.Checked = true;
                        onceRightLC.Checked = true;
                        onceDownRightLC.Checked = true;
                        onceDownLC.Checked = true;
                        onceDownLeftLC.Checked = true;
                        onceLeftLC.Checked = true;
                        onceUpLeftLC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
            }

            else if (lblRightClick.CssStyle.Contains("bold"))
            {
                actionType = " you Right Click in";

                if (optUpLeft.Items.Contains(optToggle))
                {
                    if (togUpLeftRC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant holds a button down until{actionType} it again.'>This quadrant holds a button down until" + actionType + " it again.", "RC◤ - Turn Off 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togUpLeftRC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpRC.Checked = false;
                            togUpRightRC.Checked = false;
                            togRightRC.Checked = false;
                            togDownRightRC.Checked = false;
                            togDownRC.Checked = false;
                            togDownLeftRC.Checked = false;
                            togLeftRC.Checked = false;
                            togUpLeftRC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to hold a button down until{actionType} it again?'>Would you like this quadrant to hold a button down until" + actionType + " it again?", "RC◤ - Turn On 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togUpLeftRC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpRC.Checked = true;
                            togUpRightRC.Checked = true;
                            togRightRC.Checked = true;
                            togDownRightRC.Checked = true;
                            togDownRC.Checked = true;
                            togDownLeftRC.Checked = true;
                            togLeftRC.Checked = true;
                            togUpLeftRC.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (optUpLeft.Items.Contains(optRTC))
                {
                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: darkblue !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid darkblue !important; border-bottom: 1px solid darkblue !important; border-left: 1px solid darkblue !important; }'; document.head.appendChild(style);}");
                    if (rtcUpLeftRC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant returns your mouse to the center when you release your Right Click in it.'>This quadrant returns your mouse to the center when you release your Right Click in it.</span></p>", "RC◤ - Turn Off 'Return To Center On Click' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            rtcUpLeftRC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {


                            rtcUpRC.Checked = false;
                            rtcUpLeftRC.Checked = false;
                            rtcUpRightRC.Checked = false;
                            rtcRightRC.Checked = false;
                            rtcDownRightRC.Checked = false;
                            rtcDownRC.Checked = false;
                            rtcDownLeftRC.Checked = false;
                            rtcLeftRC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to return your mouse to the center when you release your Right Click in it?'>Would you like this quadrant to return your mouse to the center when you release your Right Click in it?</span></p>", "RC◤ - Turn On 'Return To Center On Click' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            rtcUpLeftRC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            rtcUpRC.Checked = true;
                            rtcUpLeftRC.Checked = true;
                            rtcUpRightRC.Checked = true;
                            rtcRightRC.Checked = true;
                            rtcDownRightRC.Checked = true;
                            rtcDownRC.Checked = true;
                            rtcDownLeftRC.Checked = true;
                            rtcLeftRC.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: black !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid black !important; border-bottom: 1px solid black !important; border-left: 1px solid black !important; }'; document.head.appendChild(style);}");

                if (onceUpLeftRC.Checked)
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant fully presses a button once (instead of pressing as long as you hold click) when{actionType} it.'>This quadrant fully presses a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "RC◤ - Turn Off 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceUpLeftRC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceUpRC.Checked = false;
                        onceRightRC.Checked = false;
                        onceDownRightRC.Checked = false;
                        onceDownRC.Checked = false;
                        onceDownLeftRC.Checked = false;
                        onceLeftRC.Checked = false;
                        onceUpLeftRC.Checked = false;
                        onceUpRightRC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to fully push a button once (instead of pressing as long as you hold click) when{actionType} it.'>Would you like to fully push a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "RC◤ - Turn On 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceUpLeftRC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceUpRC.Checked = true;
                        onceUpRightRC.Checked = true;
                        onceRightRC.Checked = true;
                        onceDownRightRC.Checked = true;
                        onceDownRC.Checked = true;
                        onceDownLeftRC.Checked = true;
                        onceLeftRC.Checked = true;
                        onceUpLeftRC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
            }

            Eval(@"var styleElement = 
                document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); }");

        }

        private void actDownLeft_Click(object sender, EventArgs e)
        {
            if (lblHover.CssStyle.Contains("bold"))
            {
                actionType = " you Move Your Mouse in";

                if (optDownLeft.Items.Contains(optToggle))
                {
                    if (togDownLeftHover.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant holds a button down until{actionType}to it again.'>This quadrant holds a button down until" + actionType + "to it again.", "MM◣ - Turn Off 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDownLeftHover.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpHover.Checked = false;
                            togUpRightHover.Checked = false;
                            togRightHover.Checked = false;
                            togDownRightHover.Checked = false;
                            togDownHover.Checked = false;
                            togDownLeftHover.Checked = false;
                            togLeftHover.Checked = false;
                            togUpLeftHover.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to hold a button down until{actionType}to it again?'>Would you like this quadrant to hold a button down until" + actionType + "to it again?", "MM◣ - Turn On 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDownLeftHover.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpHover.Checked = true;
                            togUpRightHover.Checked = true;
                            togRightHover.Checked = true;
                            togDownRightHover.Checked = true;
                            togDownHover.Checked = true;
                            togDownLeftHover.Checked = true;
                            togLeftHover.Checked = true;
                            togUpLeftHover.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (optDownLeft.Items.Contains(optOnce))
                {
                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: darkblue !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid darkblue !important; border-bottom: 1px solid darkblue !important; border-left: 1px solid darkblue !important; }'; document.head.appendChild(style);}");
                    if (onceDownLeft.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant presses a button once (instead of holding) when{actionType}to it.'>This quadrant presses a button once (instead of holding) when" + actionType + "to it.", "MM◣ - Turn Off 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            onceDownLeft.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {


                            onceUp.Checked = false;
                            onceRight.Checked = false;
                            onceDownRight.Checked = false;
                            onceDown.Checked = false;
                            onceDownLeft.Checked = false;
                            onceLeft.Checked = false;
                            onceUpLeft.Checked = false;
                            onceUpRight.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to press a button once (instead of holding it) when{actionType}to it?'>Would you like this quadrant to press a button once (instead of holding it) when" + actionType + "to it?", "MM◣ - Turn On 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            onceDownLeft.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            onceUp.Checked = true;
                            onceUpRight.Checked = true;
                            onceRight.Checked = true;
                            onceDownRight.Checked = true;
                            onceDown.Checked = true;
                            onceDownLeft.Checked = true;
                            onceLeft.Checked = true;
                            onceUpLeft.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (IsHoverVariableEligible(5))
                {
                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: black !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid black !important; border-bottom: 1px solid black !important; border-left: 1px solid black !important; }'; document.head.appendChild(style);}");
                    if (varDownLeftHover.Checked)
                    {
                        ExtendedDialogResult LobjResult;

                        if (chkXbox.Checked)
                        {

                            LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant pushes the analog stick further in set direction (or the trigger further down) as you Move Your Mouse away from the center.'>This quadrant pushes the analog stick further in set direction (or the trigger further down) as you Move Your Mouse away from the center.</span></p>", "MM◣ - Turn Off 'Variable Sticks & Triggers' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        }
                        else
                        {
                            LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant pushes the analog stick further in set direction as you Move Your Mouse away from the center.'>This quadrant pushes the analog stick further in set direction as you Move Your Mouse away from the center.</span></p>", "MM◣ - Turn Off 'Variable Sticks' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        }

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            varDownLeftHover.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {


                            varUpHover.Checked = false;
                            varUpRightHover.Checked = false;
                            varRightHover.Checked = false;
                            varDownRightHover.Checked = false;
                            varDownHover.Checked = false;
                            varDownLeftHover.Checked = false;
                            varLeftHover.Checked = false;
                            varUpLeftHover.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult;

                        if (chkXbox.Checked)
                        {

                            LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to push the analog stick further in set direction (or the trigger further down) as you Move Your Mouse away from the center?'>Would you like to push the analog stick further in set direction (or the trigger further down) as you Move Your Mouse away from the center?</span></p>", "MM◣ - Turn On 'Variable Sticks & Triggers' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);

                        }
                        else
                        {
                            LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to push the analog stick further in set direction as you Move Your Mouse away from the center?'>Would you like to push the analog stick further in set direction as you Move Your Mouse away from the center?</span></p>", "MM◣ - Turn On 'Variable Sticks' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);

                        }
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            varDownLeftHover.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            varUpHover.Checked = true;
                            varUpRightHover.Checked = true;
                            varRightHover.Checked = true;
                            varDownRightHover.Checked = true;
                            varDownHover.Checked = true;
                            varDownLeftHover.Checked = true;
                            varLeftHover.Checked = true;
                            varUpLeftHover.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

            }

            else if (lblLeftClick.CssStyle.Contains("bold"))
            {
                actionType = " you Left Click in";

                if (optDownLeft.Items.Contains(optToggle))
                {
                    if (togDownLeftLC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant holds a button down until{actionType} it again.'>This quadrant holds a button down until" + actionType + " it again.", "LC◣ - Turn Off 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDownLeftLC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpLC.Checked = false;
                            togUpRightLC.Checked = false;
                            togRightLC.Checked = false;
                            togDownRightLC.Checked = false;
                            togDownLC.Checked = false;
                            togDownLeftLC.Checked = false;
                            togLeftLC.Checked = false;
                            togUpLeftLC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to hold a button down until{actionType} it again?'>Would you like this quadrant to hold a button down until" + actionType + " it again?", "LC◣ - Turn On 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDownLeftLC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpLC.Checked = true;
                            togUpRightLC.Checked = true;
                            togRightLC.Checked = true;
                            togDownRightLC.Checked = true;
                            togDownLC.Checked = true;
                            togDownLeftLC.Checked = true;
                            togLeftLC.Checked = true;
                            togUpLeftLC.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (optDownLeft.Items.Contains(optRTC))
                {
                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: darkblue !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid darkblue !important; border-bottom: 1px solid darkblue !important; border-left: 1px solid darkblue !important; }'; document.head.appendChild(style);}");
                    if (rtcDownLeftLC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant returns your mouse to the center when you release your Left Click in it.'>This quadrant returns your mouse to the center when you release your Left Click in it.</span></p>", "LC◣ - Turn Off 'Return To Center On Click' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            rtcDownLeftLC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {


                            rtcUpLC.Checked = false;
                            rtcUpLeftLC.Checked = false;
                            rtcUpRightLC.Checked = false;
                            rtcRightLC.Checked = false;
                            rtcDownRightLC.Checked = false;
                            rtcDownLC.Checked = false;
                            rtcDownLeftLC.Checked = false;
                            rtcLeftLC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to return your mouse to the center when you release your Left Click in it?'>Would you like this quadrant to return your mouse to the center when you release your Left Click in it?</span></p>", "LC◣ - Turn On 'Return To Center On Click' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            rtcDownLeftLC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            rtcUpLC.Checked = true;
                            rtcUpLeftLC.Checked = true;
                            rtcUpRightLC.Checked = true;
                            rtcRightLC.Checked = true;
                            rtcDownRightLC.Checked = true;
                            rtcDownLC.Checked = true;
                            rtcDownLeftLC.Checked = true;
                            rtcLeftLC.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: black !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid black !important; border-bottom: 1px solid black !important; border-left: 1px solid black !important; }'; document.head.appendChild(style); }");

                if (onceDownLeftLC.Checked)
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant fully presses a button once (instead of pressing as long as you hold click) when{actionType} it.'>This quadrant fully presses a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "LC◣ - Turn Off 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceDownLeftLC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceUpLC.Checked = false;
                        onceRightLC.Checked = false;
                        onceDownRightLC.Checked = false;
                        onceDownLC.Checked = false;
                        onceDownLeftLC.Checked = false;
                        onceLeftLC.Checked = false;
                        onceUpLeftLC.Checked = false;
                        onceUpRightLC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to fully push a button once (instead of pressing as long as you hold click) when{actionType} it.'>Would you like to fully push a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "LC◣ - Turn On 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceDownLeftLC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceUpLC.Checked = true;
                        onceUpRightLC.Checked = true;
                        onceRightLC.Checked = true;
                        onceDownRightLC.Checked = true;
                        onceDownLC.Checked = true;
                        onceDownLeftLC.Checked = true;
                        onceLeftLC.Checked = true;
                        onceUpLeftLC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
            }

            else if (lblRightClick.CssStyle.Contains("bold"))
            {
                actionType = " you Right Click in";

                if (optDownLeft.Items.Contains(optToggle))
                {
                    if (togDownLeftRC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant holds a button down until{actionType} it again.'>This quadrant holds a button down until" + actionType + " it again.", "RC◣ - Turn Off 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDownLeftRC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpRC.Checked = false;
                            togUpRightRC.Checked = false;
                            togRightRC.Checked = false;
                            togDownRightRC.Checked = false;
                            togDownRC.Checked = false;
                            togDownLeftRC.Checked = false;
                            togLeftRC.Checked = false;
                            togUpLeftRC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to hold a button down until{actionType} it again?'>Would you like this quadrant to hold a button down until" + actionType + " it again?", "RC◣ - Turn On 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDownLeftRC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpRC.Checked = true;
                            togUpRightRC.Checked = true;
                            togRightRC.Checked = true;
                            togDownRightRC.Checked = true;
                            togDownRC.Checked = true;
                            togDownLeftRC.Checked = true;
                            togLeftRC.Checked = true;
                            togUpLeftRC.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (optDownLeft.Items.Contains(optRTC))
                {
                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: darkblue !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid darkblue !important; border-bottom: 1px solid darkblue !important; border-left: 1px solid darkblue !important; }'; document.head.appendChild(style);}");
                    if (rtcDownLeftRC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant returns your mouse to the center when you release your Right Click in it.'>This quadrant returns your mouse to the center when you release your Right Click in it.</span></p>", "RC◣ - Turn Off 'Return To Center On Click' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            rtcDownLeftRC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {


                            rtcUpRC.Checked = false;
                            rtcUpLeftRC.Checked = false;
                            rtcUpRightRC.Checked = false;
                            rtcRightRC.Checked = false;
                            rtcDownRightRC.Checked = false;
                            rtcDownRC.Checked = false;
                            rtcDownLeftRC.Checked = false;
                            rtcLeftRC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to return your mouse to the center when you release your Right Click in it?'>Would you like this quadrant to return your mouse to the center when you release your Right Click in it?</span></p>", "RC◣ - Turn On 'Return To Center On Click' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            rtcDownLeftRC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            rtcUpRC.Checked = true;
                            rtcUpLeftRC.Checked = true;
                            rtcUpRightRC.Checked = true;
                            rtcRightRC.Checked = true;
                            rtcDownRightRC.Checked = true;
                            rtcDownRC.Checked = true;
                            rtcDownLeftRC.Checked = true;
                            rtcLeftRC.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: black !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid black !important; border-bottom: 1px solid black !important; border-left: 1px solid black !important; }'; document.head.appendChild(style); }");

                if (onceDownLeftRC.Checked)
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant fully presses a button once (instead of pressing as long as you hold click) when{actionType} it.'>This quadrant fully presses a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "RC◣ - Turn Off 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceDownLeftRC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceUpRC.Checked = false;
                        onceRightRC.Checked = false;
                        onceDownRightRC.Checked = false;
                        onceDownRC.Checked = false;
                        onceDownLeftRC.Checked = false;
                        onceLeftRC.Checked = false;
                        onceUpLeftRC.Checked = false;
                        onceUpRightRC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to fully push a button once (instead of pressing as long as you hold click) when{actionType} it.'>Would you like to fully push a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "RC◣ - Turn On 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceDownLeftRC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceUpRC.Checked = true;
                        onceUpRightRC.Checked = true;
                        onceRightRC.Checked = true;
                        onceDownRightRC.Checked = true;
                        onceDownRC.Checked = true;
                        onceDownLeftRC.Checked = true;
                        onceLeftRC.Checked = true;
                        onceUpLeftRC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
            }

            Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); }");
        }

        private void actLeft_Click(object sender, EventArgs e)
        {
            if (lblHover.CssStyle.Contains("bold"))
            {
                actionType = " you Move Your Mouse in";

                if (optLeft.Items.Contains(optToggle))
                {
                    if (togLeftHover.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant holds a button down until{actionType}to it again.'>This quadrant holds a button down until" + actionType + "to it again.", "MM◀ - Turn Off 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togLeftHover.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpHover.Checked = false;
                            togUpRightHover.Checked = false;
                            togRightHover.Checked = false;
                            togDownRightHover.Checked = false;
                            togDownHover.Checked = false;
                            togDownLeftHover.Checked = false;
                            togLeftHover.Checked = false;
                            togUpLeftHover.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to hold a button down until{actionType}to it again?'>Would you like this quadrant to hold a button down until" + actionType + "to it again?", "MM◀ - Turn On 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togLeftHover.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpHover.Checked = true;
                            togUpRightHover.Checked = true;
                            togRightHover.Checked = true;
                            togDownRightHover.Checked = true;
                            togDownHover.Checked = true;
                            togDownLeftHover.Checked = true;
                            togLeftHover.Checked = true;
                            togUpLeftHover.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (optLeft.Items.Contains(optOnce))
                {
                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: darkblue !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid darkblue !important; border-bottom: 1px solid darkblue !important; border-left: 1px solid darkblue !important; }'; document.head.appendChild(style);}");
                    if (onceLeft.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant presses a button once (instead of holding) when{actionType}to it.'>This quadrant presses a button once (instead of holding) when" + actionType + "to it.", "MM◀ - Turn Off 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            onceLeft.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {


                            onceUp.Checked = false;
                            onceRight.Checked = false;
                            onceDownRight.Checked = false;
                            onceDown.Checked = false;
                            onceDownLeft.Checked = false;
                            onceLeft.Checked = false;
                            onceUpLeft.Checked = false;
                            onceUpRight.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to press a button once (instead of holding it) when{actionType}to it?'>Would you like this quadrant to press a button once (instead of holding it) when" + actionType + "to it?", "MM◀ - Turn On 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            onceLeft.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            onceUp.Checked = true;
                            onceUpRight.Checked = true;
                            onceRight.Checked = true;
                            onceDownRight.Checked = true;
                            onceDown.Checked = true;
                            onceDownLeft.Checked = true;
                            onceLeft.Checked = true;
                            onceUpLeft.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (IsHoverVariableEligible(6))
                {
                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: black !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid black !important; border-bottom: 1px solid black !important; border-left: 1px solid black !important; }'; document.head.appendChild(style);}");
                    if (varLeftHover.Checked)
                    {
                        ExtendedDialogResult LobjResult;

                        if (chkXbox.Checked)
                        {

                            LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant pushes the analog stick further in set direction (or the trigger further down) as you Move Your Mouse away from the center.'>This quadrant pushes the analog stick further in set direction (or the trigger further down) as you Move Your Mouse away from the center.</span></p>", "MM◀ - Turn Off 'Variable Sticks & Triggers' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        }
                        else
                        {
                            LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant pushes the analog stick further in set direction as you Move Your Mouse away from the center.'>This quadrant pushes the analog stick further in set direction as you Move Your Mouse away from the center.</span></p>", "MM◀ - Turn Off 'Variable Sticks' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        }

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            varLeftHover.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {


                            varUpHover.Checked = false;
                            varUpRightHover.Checked = false;
                            varRightHover.Checked = false;
                            varDownRightHover.Checked = false;
                            varDownHover.Checked = false;
                            varDownLeftHover.Checked = false;
                            varLeftHover.Checked = false;
                            varUpLeftHover.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult;

                        if (chkXbox.Checked)
                        {

                            LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to push the analog stick further in set direction (or the trigger further down) as you Move Your Mouse away from the center?'>Would you like to push the analog stick further in set direction (or the trigger further down) as you Move Your Mouse away from the center?</span></p>", "MM◀ - Turn On 'Variable Sticks & Triggers' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);

                        }
                        else
                        {
                            LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to push the analog stick further in set direction as you Move Your Mouse away from the center?'>Would you like to push the analog stick further in set direction as you Move Your Mouse away from the center?</span></p>", "MM◀ - Turn On 'Variable Sticks' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);

                        }
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            varLeftHover.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            varUpHover.Checked = true;
                            varUpRightHover.Checked = true;
                            varRightHover.Checked = true;
                            varDownRightHover.Checked = true;
                            varDownHover.Checked = true;
                            varDownLeftHover.Checked = true;
                            varLeftHover.Checked = true;
                            varUpLeftHover.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

            }

            else if (lblLeftClick.CssStyle.Contains("bold"))
            {
                actionType = " you Left Click in";

                if (optLeft.Items.Contains(optToggle))
                {
                    if (togLeftLC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant holds a button down until{actionType} it again.'>This quadrant holds a button down until" + actionType + " it again.", "LC◀ - Turn Off 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togLeftLC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpLC.Checked = false;
                            togUpRightLC.Checked = false;
                            togRightLC.Checked = false;
                            togDownRightLC.Checked = false;
                            togDownLC.Checked = false;
                            togDownLeftLC.Checked = false;
                            togLeftLC.Checked = false;
                            togUpLeftLC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to hold a button down until{actionType} it again?'>Would you like this quadrant to hold a button down until" + actionType + " it again?", "LC◀ - Turn On 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togLeftLC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpLC.Checked = true;
                            togUpRightLC.Checked = true;
                            togRightLC.Checked = true;
                            togDownRightLC.Checked = true;
                            togDownLC.Checked = true;
                            togDownLeftLC.Checked = true;
                            togLeftLC.Checked = true;
                            togUpLeftLC.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (optLeft.Items.Contains(optRTC))
                {
                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: darkblue !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid darkblue !important; border-bottom: 1px solid darkblue !important; border-left: 1px solid darkblue !important; }'; document.head.appendChild(style);}");
                    if (rtcLeftLC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant returns your mouse to the center when you release your Left Click in it.'>This quadrant returns your mouse to the center when you release your Left Click in it.</span></p>", "LC◀ - Turn Off 'Return To Center On Click' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            rtcLeftLC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {


                            rtcUpLC.Checked = false;
                            rtcUpLeftLC.Checked = false;
                            rtcUpRightLC.Checked = false;
                            rtcRightLC.Checked = false;
                            rtcDownRightLC.Checked = false;
                            rtcDownLC.Checked = false;
                            rtcDownLeftLC.Checked = false;
                            rtcLeftLC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to return your mouse to the center when you release your Left Click in it?'>Would you like this quadrant to return your mouse to the center when you release your Left Click in it?</span></p>", "LC◀ - Turn On 'Return To Center On Click' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            rtcLeftLC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            rtcUpLC.Checked = true;
                            rtcUpLeftLC.Checked = true;
                            rtcUpRightLC.Checked = true;
                            rtcRightLC.Checked = true;
                            rtcDownRightLC.Checked = true;
                            rtcDownLC.Checked = true;
                            rtcDownLeftLC.Checked = true;
                            rtcLeftLC.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: black !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid black !important; border-bottom: 1px solid black !important; border-left: 1px solid black !important; }'; document.head.appendChild(style); }");

                if (onceLeftLC.Checked)
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant fully presses a button once (instead of pressing as long as you hold click) when{actionType} it.'>This quadrant fully presses a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "LC◀ - Turn Off 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceLeftLC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceUpLC.Checked = false;
                        onceRightLC.Checked = false;
                        onceDownRightLC.Checked = false;
                        onceDownLC.Checked = false;
                        onceDownLeftLC.Checked = false;
                        onceLeftLC.Checked = false;
                        onceUpLeftLC.Checked = false;
                        onceUpRightLC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to fully push a button once (instead of pressing as long as you hold click) when{actionType} it.'>Would you like to fully push a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "LC◀ - Turn On 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceLeftLC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceUpLC.Checked = true;
                        onceUpRightLC.Checked = true;
                        onceRightLC.Checked = true;
                        onceDownRightLC.Checked = true;
                        onceDownLC.Checked = true;
                        onceDownLeftLC.Checked = true;
                        onceLeftLC.Checked = true;
                        onceUpLeftLC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
            }

            else if (lblRightClick.CssStyle.Contains("bold"))
            {
                actionType = " you Right Click in";

                if (optLeft.Items.Contains(optToggle))
                {
                    if (togLeftRC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant holds a button down until{actionType} it again.'>This quadrant holds a button down until" + actionType + " it again.", "RC◀ - Turn Off 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togLeftRC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpRC.Checked = false;
                            togUpRightRC.Checked = false;
                            togRightRC.Checked = false;
                            togDownRightRC.Checked = false;
                            togDownRC.Checked = false;
                            togDownLeftRC.Checked = false;
                            togLeftRC.Checked = false;
                            togUpLeftRC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to hold a button down until{actionType} it again?'>Would you like this quadrant to hold a button down until" + actionType + " it again?", "RC◀ - Turn On 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togLeftRC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togUpRC.Checked = true;
                            togUpRightRC.Checked = true;
                            togRightRC.Checked = true;
                            togDownRightRC.Checked = true;
                            togDownRC.Checked = true;
                            togDownLeftRC.Checked = true;
                            togLeftRC.Checked = true;
                            togUpLeftRC.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (optLeft.Items.Contains(optRTC))
                {
                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: darkblue !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid darkblue !important; border-bottom: 1px solid darkblue !important; border-left: 1px solid darkblue !important; }'; document.head.appendChild(style);}");
                    if (rtcLeftRC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant returns your mouse to the center when you release your Right Click in it.'>This quadrant returns your mouse to the center when you release your Right Click in it.</span></p>", "RC◀ - Turn Off 'Return To Center On Click' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            rtcLeftRC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {


                            rtcUpRC.Checked = false;
                            rtcUpLeftRC.Checked = false;
                            rtcUpRightRC.Checked = false;
                            rtcRightRC.Checked = false;
                            rtcDownRightRC.Checked = false;
                            rtcDownRC.Checked = false;
                            rtcDownLeftRC.Checked = false;
                            rtcLeftRC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to return your mouse to the center when you release your Right Click in it?'>Would you like this quadrant to return your mouse to the center when you release your Right Click in it?</span></p>", "RC◀ - Turn On 'Return To Center On Click' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            rtcLeftRC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            rtcUpRC.Checked = true;
                            rtcUpLeftRC.Checked = true;
                            rtcUpRightRC.Checked = true;
                            rtcRightRC.Checked = true;
                            rtcDownRightRC.Checked = true;
                            rtcDownRC.Checked = true;
                            rtcDownLeftRC.Checked = true;
                            rtcLeftRC.Checked = true;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: black !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid black !important; border-bottom: 1px solid black !important; border-left: 1px solid black !important; }'; document.head.appendChild(style); }");

                if (onceLeftRC.Checked)
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant fully presses a button once (instead of pressing as long as you hold click) when{actionType} it.'>This quadrant fully presses a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "RC◀ - Turn Off 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, true);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceLeftRC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceUpRC.Checked = false;
                        onceRightRC.Checked = false;
                        onceDownRightRC.Checked = false;
                        onceDownRC.Checked = false;
                        onceDownLeftRC.Checked = false;
                        onceLeftRC.Checked = false;
                        onceUpLeftRC.Checked = false;
                        onceUpRightRC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to fully push a button once (instead of pressing as long as you hold click) when{actionType} it.'>Would you like to fully push a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "RC◀ - Turn On 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, false, false);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceLeftRC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceUpRC.Checked = true;
                        onceUpRightRC.Checked = true;
                        onceRightRC.Checked = true;
                        onceDownRightRC.Checked = true;
                        onceDownRC.Checked = true;
                        onceDownLeftRC.Checked = true;
                        onceLeftRC.Checked = true;
                        onceUpLeftRC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }

            }

            Eval(@"var 
styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); }");

        }

        private void actDZUpper_Click(object sender, EventArgs e)
        {
            if (lblHover.CssStyle.Contains("bold"))
            {
                actionType = " you Move Your Mouse in";

                if (optDZUpper.Items.Contains(optToggle))
                {
                    if (togDZUpperHover.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant holds a button down until{actionType}to it again.'>This quadrant holds a button down until" + actionType + "to it again.", "MM◓ - Turn Off 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, true, lines:2);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDZUpperHover.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togDZLowerHover.Checked = false;
                            togDZUpperHover.Checked = false;
                            togDZMiddleHover.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to hold a button down until{actionType}to it again?'>Would you like this quadrant to hold a button down until" + actionType + "to it again?", "MM◓ - Turn On 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDZUpperHover.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togDZLowerHover.Checked = true;
                            togDZUpperHover.Checked = true;
                            togDZMiddleHover.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (optDZUpper.Items.Contains(optOnce))
                {
                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: darkblue !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid darkblue !important; border-bottom: 1px solid darkblue !important; border-left: 1px solid darkblue !important; }'; document.head.appendChild(style);}");
                    if (onceDZUpper.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant presses a button once (instead of holding) when{actionType}to it.'>This quadrant presses a button once (instead of holding) when" + actionType + "to it.", "MM◓ - Turn Off 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, true, lines:2);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            onceDZUpper.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {

                            onceDZUpper.Checked = false;
                            onceDZLower.Checked = false;
                            onceDZMiddle.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        //continue here
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to press a button once (instead of holding it) when{actionType}to it?'>Would you like this quadrant to press a button once (instead of holding it) when" + actionType + "to it?", "MM◓ - Turn On 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            onceDZUpper.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            onceDZUpper.Checked = true;
                            onceDZMiddle.Checked = true;
                            onceDZLower.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

            }

            else if (lblLeftClick.CssStyle.Contains("bold"))
            {
                actionType = " you Left Click in";

                if (optDZUpper.Items.Contains(optToggle))
                {
                    if (togDZUpperLC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant holds a button down until{actionType} it again.'>This quadrant holds a button down until" + actionType + " it again.", "LC◓ - Turn Off 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, true, lines:2);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDZUpperLC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togDZUpperLC.Checked = false;
                            togDZMiddleLC.Checked = false;
                            togDZLowerLC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to hold a button down until{actionType} it again?'>Would you like this quadrant to hold a button down until" + actionType + " it again?", "LC◓ - Turn On 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDZUpperLC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togDZUpperLC.Checked = true;
                            togDZMiddleLC.Checked = true;
                            togDZLowerLC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }


                Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: black !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid black !important; border-bottom: 1px solid black !important; border-left: 1px solid black !important; }'; document.head.appendChild(style); }");

                if (onceDZUpperLC.Checked)
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant fully presses a button once (instead of pressing as long as you hold click) when{actionType} it.'>This quadrant fully presses a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "LC◓ - Turn Off 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, true, lines:2);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceDZUpperLC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceDZMiddleLC.Checked = false;
                        onceDZLowerLC.Checked = false;
                        onceDZUpperLC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to fully push a button once (instead of pressing as long as you hold click) when{actionType} it.'>Would you like to fully push a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "LC◓ - Turn On 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, false);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceDZUpperLC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceDZMiddleLC.Checked = true;
                        onceDZLowerLC.Checked = true;
                        onceDZUpperLC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }

            }

            else if (lblRightClick.CssStyle.Contains("bold"))
            {
                actionType = " you Right Click in";

                if (optDZUpper.Items.Contains(optToggle))
                {
                    if (togDZUpperRC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant holds a button down until{actionType} it again.'>This quadrant holds a button down until" + actionType + " it again.", "RC◓ - Turn Off 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, true, lines:2);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDZUpperRC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togDZUpperRC.Checked = false;
                            togDZMiddleRC.Checked = false;
                            togDZLowerRC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to hold a button down until{actionType} it again?'>Would you like this quadrant to hold a button down until" + actionType + " it again?", "RC◓ - Turn On 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDZUpperRC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togDZUpperRC.Checked = true;
                            togDZMiddleRC.Checked = true;
                            togDZLowerRC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: black !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid black !important; border-bottom: 1px solid black !important; border-left: 1px solid black !important; }'; document.head.appendChild(style); }");

                if (onceDZUpperRC.Checked)
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant fully presses a button once (instead of pressing as long as you hold click) when{actionType} it.'>This quadrant fully presses a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "RC◓ - Turn Off 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, true, lines:2);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceDZUpperRC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceDZMiddleRC.Checked = false;
                        onceDZLowerRC.Checked = false;
                        onceDZUpperRC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to fully push a button once (instead of pressing as long as you hold click) when{actionType} it.'>Would you like to fully push a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "RC◓ - Turn On 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, false);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceDZUpperRC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceDZMiddleRC.Checked = true;
                        onceDZLowerRC.Checked = true;
                        onceDZUpperRC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
            }

            Eval(@"var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); }");
        }

        private void actDZMiddle_Click(object sender, EventArgs e)
        {
            if (lblHover.CssStyle.Contains("bold"))
            {
                actionType = " you Move Your Mouse in";

                if (optDZMiddle.Items.Contains(optToggle))
                {
                    if (togDZMiddleHover.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant holds a button down until{actionType}to it again.'>This quadrant holds a button down until" + actionType + "to it again.", "MM◌ - Turn Off 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, true, lines:2);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDZMiddleHover.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togDZLowerHover.Checked = false;
                            togDZUpperHover.Checked = false;
                            togDZMiddleHover.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to hold a button down until{actionType}to it again?'>Would you like this quadrant to hold a button down until" + actionType + "to it again?", "MM◌ - Turn On 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDZMiddleHover.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togDZLowerHover.Checked = true;
                            togDZUpperHover.Checked = true;
                            togDZMiddleHover.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (optDZMiddle.Items.Contains(optOnce))
                {
                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: darkblue !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid darkblue !important; border-bottom: 1px solid darkblue !important; border-left: 1px solid darkblue !important; }'; document.head.appendChild(style);}");
                    if (onceDZMiddle.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant presses a button once (instead of holding) when{actionType}to it.'>This quadrant presses a button once (instead of holding) when" + actionType + "to it.", "MM◌ - Turn Off 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, true, lines:2);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            onceDZMiddle.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {

                            onceDZUpper.Checked = false;
                            onceDZLower.Checked = false;
                            onceDZMiddle.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        //continue here
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to press a button once (instead of holding it) when{actionType}to it?'>Would you like this quadrant to press a button once (instead of holding it) when" + actionType + "to it?", "MM◌ - Turn On 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            onceDZMiddle.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            onceDZUpper.Checked = true;
                            onceDZMiddle.Checked = true;
                            onceDZLower.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

            }

            else if (lblLeftClick.CssStyle.Contains("bold"))
            {
                actionType = " you Left Click in";

                if (optDZMiddle.Items.Contains(optToggle))
                {
                    if (togDZMiddleLC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant holds a button down until{actionType} it again.'>This quadrant holds a button down until" + actionType + " it again.", "LC◌ - Turn Off 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, true, lines:2);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDZMiddleLC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togDZUpperLC.Checked = false;
                            togDZMiddleLC.Checked = false;
                            togDZLowerLC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to hold a button down until{actionType} it again?'>Would you like this quadrant to hold a button down until" + actionType + " it again?", "LC◌ - Turn On 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDZMiddleLC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togDZUpperLC.Checked = true;
                            togDZMiddleLC.Checked = true;
                            togDZLowerLC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }


                Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: black !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid black !important; border-bottom: 1px solid black !important; border-left: 1px solid black !important; }'; document.head.appendChild(style); }");

                if (onceDZMiddleLC.Checked)
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant fully presses a button once (instead of pressing as long as you hold click) when{actionType} it.'>This quadrant fully presses a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "LC◌ - Turn Off 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, true, lines:2);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceDZMiddleLC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceDZLowerLC.Checked = false;
                        onceDZUpperLC.Checked = false;
                        onceDZMiddleLC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to fully push a button once (instead of pressing as long as you hold click) when{actionType} it.'>Would you like to fully push a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "LC◌ - Turn On 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, false);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceDZMiddleLC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceDZLowerLC.Checked = true;
                        onceDZUpperLC.Checked = true;
                        onceDZMiddleLC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }

            }

            else if (lblRightClick.CssStyle.Contains("bold"))
            {
                actionType = " you Right Click in";

                if (optDZMiddle.Items.Contains(optToggle))
                {
                    if (togDZMiddleRC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant holds a button down until{actionType} it again.'>This quadrant holds a button down until" + actionType + " it again.", "RC◌ - Turn Off 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, true, lines:2);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDZMiddleRC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togDZUpperRC.Checked = false;
                            togDZMiddleRC.Checked = false;
                            togDZLowerRC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to hold a button down until{actionType} it again?'>Would you like this quadrant to hold a button down until" + actionType + " it again?", "RC◌ - Turn On 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDZMiddleRC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togDZUpperRC.Checked = true;
                            togDZMiddleRC.Checked = true;
                            togDZLowerRC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: black !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid black !important; border-bottom: 1px solid black !important; border-left: 1px solid black !important; }'; document.head.appendChild(style); }");

                if (onceDZMiddleRC.Checked)
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant fully presses a button once (instead of pressing as long as you hold click) when{actionType} it.'>This quadrant fully presses a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "RC◌ - Turn Off 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, true, lines:2);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceDZMiddleRC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceDZLowerRC.Checked = false;
                        onceDZUpperRC.Checked = false;
                        onceDZMiddleRC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to fully push a button once (instead of pressing as long as you hold click) when{actionType} it.'>Would you like to fully push a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "RC◌ - Turn On 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, false);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceDZMiddleRC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceDZLowerRC.Checked = true;
                        onceDZUpperRC.Checked = true;
                        onceDZMiddleRC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
            }

            Eval(@"var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); }");


        }

        private void actDZLower_Click(object sender, EventArgs e)
        {
            if (lblHover.CssStyle.Contains("bold"))
            {
                actionType = " you Move Your Mouse in";

                if (optDZLower.Items.Contains(optToggle))
                {
                    if (togDZLowerHover.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant holds a button down until{actionType}to it again.'>This quadrant holds a button down until" + actionType + "to it again.", "MM◒ - Turn Off 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, true, lines:2);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDZLowerHover.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togDZLowerHover.Checked = false;
                            togDZUpperHover.Checked = false;
                            togDZMiddleHover.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to hold a button down until{actionType}to it again?'>Would you like this quadrant to hold a button down until" + actionType + "to it again?", "MM◒ - Turn On 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDZLowerHover.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togDZLowerHover.Checked = true;
                            togDZUpperHover.Checked = true;
                            togDZMiddleHover.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

                if (optDZLower.Items.Contains(optOnce))
                {
                    Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: darkblue !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid darkblue !important; border-bottom: 1px solid darkblue !important; border-left: 1px solid darkblue !important; }'; document.head.appendChild(style);}");
                    if (onceDZLower.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant presses a button once (instead of holding) when{actionType}to it.'>This quadrant presses a button once (instead of holding) when" + actionType + "to it.", "MM◒ - Turn Off 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, true, lines:2);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            onceDZLower.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {

                            onceDZUpper.Checked = false;
                            onceDZLower.Checked = false;
                            onceDZMiddle.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        //continue here
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to press a button once (instead of holding it) when{actionType}to it?'>Would you like this quadrant to press a button once (instead of holding it) when" + actionType + "to it?", "MM◒ - Turn On 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            onceDZLower.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            onceDZUpper.Checked = true;
                            onceDZMiddle.Checked = true;
                            onceDZLower.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }

            }

            else if (lblLeftClick.CssStyle.Contains("bold"))
            {
                actionType = " you Left Click in";

                if (optDZLower.Items.Contains(optToggle))
                {
                    if (togDZLowerLC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant holds a button down until{actionType} it again.'>This quadrant holds a button down until" + actionType + " it again.", "LC◒ - Turn Off 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, true, lines:2);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDZLowerLC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togDZUpperLC.Checked = false;
                            togDZMiddleLC.Checked = false;
                            togDZLowerLC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to hold a button down until{actionType} it again?'>Would you like this quadrant to hold a button down until" + actionType + " it again?", "LC◒ - Turn On 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDZLowerLC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togDZUpperLC.Checked = true;
                            togDZMiddleLC.Checked = true;
                            togDZLowerLC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }


                Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: black !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid black !important; border-bottom: 1px solid black !important; border-left: 1px solid black !important; }'; document.head.appendChild(style); }");

                if (onceDZLowerLC.Checked)
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant fully presses a button once (instead of pressing as long as you hold click) when{actionType} it.'>This quadrant fully presses a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "LC◒ - Turn Off 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, true, lines:2);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceDZLowerLC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceDZLowerLC.Checked = false;
                        onceDZUpperLC.Checked = false;
                        onceDZMiddleLC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to fully push a button once (instead of pressing as long as you hold click) when{actionType} it.'>Would you like to fully push a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "LC◒ - Turn On 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, false);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceDZLowerLC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceDZLowerLC.Checked = true;
                        onceDZUpperLC.Checked = true;
                        onceDZMiddleLC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }

            }

            else if (lblRightClick.CssStyle.Contains("bold"))
            {
                actionType = " you Right Click in";

                if (optDZLower.Items.Contains(optToggle))
                {
                    if (togDZLowerRC.Checked)
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant holds a button down until{actionType} it again.'>This quadrant holds a button down until" + actionType + " it again.", "RC◒ - Turn Off 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, true, lines:2);

                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDZLowerRC.Checked = false;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togDZUpperRC.Checked = false;
                            togDZMiddleRC.Checked = false;
                            togDZLowerRC.Checked = false;
                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }

                    }
                    else
                    {
                        ExtendedDialogResult LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like this quadrant to hold a button down until{actionType} it again?'>Would you like this quadrant to hold a button down until" + actionType + " it again?", "RC◒ - Turn On 'Toggle' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, false);
                        if (LobjResult.Result == DialogResult.Abort)
                        {
                            togDZLowerRC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Retry)
                        {
                            togDZUpperRC.Checked = true;
                            togDZMiddleRC.Checked = true;
                            togDZLowerRC.Checked = true;

                        }
                        else if (LobjResult.Result == DialogResult.Ignore)
                        {

                        }
                        else
                        {
                            return;


                        }



                    }
                }


                Eval("var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); } if (!document.getElementById('customStyleBlock')) { var style = document.createElement('style'); style.id = 'customStyleBlock'; style.innerHTML = '.qx-window-captionbar-modal-active { background-color: black !important;} .qx-window-borderFixed-modal-active {border-right: 1px solid black !important; border-bottom: 1px solid black !important; border-left: 1px solid black !important; }'; document.head.appendChild(style); }");

                if (onceDZLowerRC.Checked)
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='This quadrant fully presses a button once (instead of pressing as long as you hold click) when{actionType} it.'>This quadrant fully presses a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "RC◒ - Turn Off 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, true, lines:2);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceDZLowerRC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceDZLowerRC.Checked = false;
                        onceDZUpperRC.Checked = false;
                        onceDZMiddleRC.Checked = false;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    ExtendedDialogResult LobjResult;
                    LobjResult = ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Would you like to fully push a button once (instead of pressing as long as you hold click) when{actionType} it.'>Would you like to fully push a button once (instead of pressing as long as you hold click) when" + actionType + " it.", "RC◒ - Turn On 'Press Button Once' Option?", ExtendedMessageBoxLibrary.MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, "", null, null, 0, true, false);
                    if (LobjResult.Result == DialogResult.Abort)
                    {
                        onceDZLowerRC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Retry)
                    {
                        onceDZLowerRC.Checked = true;
                        onceDZUpperRC.Checked = true;
                        onceDZMiddleRC.Checked = true;
                    }
                    else if (LobjResult.Result == DialogResult.Ignore)
                    {
                    }
                    else
                    {
                        return;
                    }
                }
            }

            Eval(@"var styleElement = document.getElementById('customStyleBlock'); if (styleElement) { styleElement.remove(); }");


        }

        private void cmbAdvanced_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbAdvanced.SelectedIndex = -1;
            cmbAdvanced.Text = "Lock Advanced Option";

        }

        private void cmbAdvanced_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //if (cmbAdvanced.SelectedIndex < 0)
            //{
            //    return;
            //}

            if (cmbAdvanced.Items[cmbAdvanced.SelectedIndex].ToString().Contains("Twin Stick Mode"))
            {
                int index = cmbAdvanced.SelectedIndex;
                if (cmbAdvanced.Items[index].ToString().Contains("Click to Turn On"))
                {
                    cmbAdvanced.Items.Insert(index, "(Click to Turn Off) Twin Stick Mode"); // Insert the new item
                    cmbAdvanced.Items.RemoveAt(index + 1); // Remove the old item
                }
                else
                {
                    cmbAdvanced.Items.Insert(index, "(Click to Turn On) Twin Stick Mode"); // Insert the new item

                    cmbAdvanced.Items.RemoveAt(index + 1); // Remove the old item
                }

            }
            else if (cmbAdvanced.Items[cmbAdvanced.SelectedIndex].ToString().Contains("Combine Inputs from Other Systems"))
            {
                int index = cmbAdvanced.SelectedIndex;

                if (cmbAdvanced.Items[index].ToString().Contains("Click to Turn On"))
                {
                    cmbAdvanced.Items.Insert(index, "(Click to Turn Off) Combine Inputs from Other Systems"); // Insert the new item

                    cmbAdvanced.Items.RemoveAt(index + 1); // Remove the old item
                }
                else
                {
                    cmbAdvanced.Items.Insert(index, "(Click to Turn On) Combine Inputs from Other Systems"); // Insert the new item

                    cmbAdvanced.Items.RemoveAt(index + 1); // Remove the old item
                }

            }

            else if (cmbAdvanced.Items[cmbAdvanced.SelectedIndex].ToString().Contains("Alternative Mouse Controls"))
            {
                int index = cmbAdvanced.SelectedIndex;
                if (cmbAdvanced.Items[index].ToString().Contains("Click to Turn On"))
                {
                    cmbAdvanced.Items.Insert(index, "(Click to Turn Off) Alternative Mouse Controls"); // Insert the new item
                    cmbAdvanced.Items.RemoveAt(index + 1); // Remove the old item
                }
                else
                {
                    cmbAdvanced.Items.Insert(index, "(Click to Turn On) Alternative Mouse Controls"); // Insert the new item
                    cmbAdvanced.Items.RemoveAt(index + 1); // Remove the old item
                }
            }

            else if (cmbAdvanced.Items[cmbAdvanced.SelectedIndex].ToString().Contains("Use Click Safe Zone"))
            {
                int index = cmbAdvanced.SelectedIndex;

                if (cmbAdvanced.Items[index].ToString().Contains("Click to Turn On"))
                {
                    cmbAdvanced.Items.Insert(index, "(Click to Turn Off) Use Click Safe Zone"); // Insert the new item

                    cmbAdvanced.Items.RemoveAt(index + 1); // Remove the old item
                }
                else
                {
                    cmbAdvanced.Items.Insert(index, "(Click to Turn On) Use Click Safe Zone"); // Insert the new item

                    cmbAdvanced.Items.RemoveAt(index + 1); // Remove the old item
                }

            }
            else if (cmbAdvanced.Items[cmbAdvanced.SelectedIndex].ToString().Contains("Alternative Return To Center"))
            {
                int index = cmbAdvanced.SelectedIndex;

                if (cmbAdvanced.Items[index].ToString().Contains("Click to Turn On"))
                {
                    cmbAdvanced.Items.Insert(index, "(Click to Turn Off) Alternative Return To Center"); // Insert the new item

                    cmbAdvanced.Items.RemoveAt(index + 1); // Remove the old item
                }
                else
                {
                    cmbAdvanced.Items.Insert(index, "(Click to Turn On) Alternative Return To Center"); // Insert the new item

                    cmbAdvanced.Items.RemoveAt(index + 1); // Remove the old item
                }

            }
            else if (cmbAdvanced.Items[cmbAdvanced.SelectedIndex].ToString().Contains("Mario Kart Mode"))
            {
                int index = cmbAdvanced.SelectedIndex;

                if (cmbAdvanced.Items[index].ToString().Contains("Click to Turn On"))
                {
                    cmbAdvanced.Items.Insert(index, "(Click to Turn Off) Mario Kart Mode"); // Insert the new item

                    cmbAdvanced.Items.RemoveAt(index + 1); // Remove the old item
                }
                else
                {
                    cmbAdvanced.Items.Insert(index, "(Click to Turn On) Mario Kart Mode"); // Insert the new item

                    cmbAdvanced.Items.RemoveAt(index + 1); // Remove the old item
                }
            }
            else if (cmbAdvanced.Items[cmbAdvanced.SelectedIndex].ToString().Contains("Voice Control Mode"))
            {
                int index = cmbAdvanced.SelectedIndex;
                if (cmbAdvanced.Items[index].ToString().Contains("Click to Turn On"))
                {
                    cmbAdvanced.Items.Insert(index, "(Click to Turn Off) Voice Control Mode"); // Insert the new item
                    cmbAdvanced.Items.RemoveAt(index + 1); // Remove the old item
                }
                else
                {
                    cmbAdvanced.Items.Insert(index, "(Click to Turn On) Voice Control Mode"); // Insert the new item
                    cmbAdvanced.Items.RemoveAt(index + 1); // Remove the old item
                }
            }
            else if (cmbAdvanced.Items[cmbAdvanced.SelectedIndex].ToString().Contains("Keep Mouse Inside Overjoyed"))
            {
                int index = cmbAdvanced.SelectedIndex;
                if (cmbAdvanced.Items[index].ToString().Contains("Click to Turn On"))
                {
                    cmbAdvanced.Items.Insert(index, "(Click to Turn Off) Keep Mouse Inside Overjoyed"); // Insert the new item
                    cmbAdvanced.Items.RemoveAt(index + 1); // Remove the old item
                }
                else
                {
                    cmbAdvanced.Items.Insert(index, "(Click to Turn On) Keep Mouse Inside Overjoyed"); // Insert the new item
                    cmbAdvanced.Items.RemoveAt(index + 1); // Remove the old item
                }
            }
            else if (cmbAdvanced.Items[cmbAdvanced.SelectedIndex].ToString().Contains("Prevent Game From Locking Mouse"))
            {

                int index = cmbAdvanced.SelectedIndex;
                if (cmbAdvanced.Items[index].ToString().Contains("Click to Turn On"))
                {
                    cmbAdvanced.Items.Insert(index, "(Click to Turn Off) Prevent Game From Locking Mouse"); // Insert the new item
                    cmbAdvanced.Items.RemoveAt(index + 1); // Remove the old item
                }
                else
                {
                    cmbAdvanced.Items.Insert(index, "(Click to Turn On) Prevent Game From Locking Mouse"); // Insert the new item
                    cmbAdvanced.Items.RemoveAt(index + 1); // Remove the old item
                }
            }
            else if (cmbAdvanced.Items[cmbAdvanced.SelectedIndex].ToString().Contains("Press PlayStation Buttons"))
            {
                int index = cmbAdvanced.SelectedIndex;
                if (cmbAdvanced.Items[index].ToString().Contains("Click to Turn On"))
                {
                    cmbAdvanced.Items.Insert(index, "(Click to Turn Off) Press PlayStation Buttons"); // Insert the new item
                    cmbAdvanced.Items.RemoveAt(index + 1); // Remove the old item
                }
                else
                {
                    cmbAdvanced.Items.Insert(index, "(Click to Turn On) Press PlayStation Buttons"); // Insert the new item
                    cmbAdvanced.Items.RemoveAt(index + 1); // Remove the old item
                }
            }
            else if (cmbAdvanced.Items[cmbAdvanced.SelectedIndex].ToString().Contains("Show Mouse Move Labels"))
            {
                int index = cmbAdvanced.SelectedIndex;
                if (cmbAdvanced.Items[index].ToString().Contains("Click to Turn On"))
                {
                    cmbAdvanced.Items.Insert(index, "(Click to Turn Off) Show Mouse Move Labels"); // Insert the new item
                    cmbAdvanced.Items.RemoveAt(index + 1); // Remove the old item
                    chkLabelsHover.Checked = true;
                    chkLabelsLC.Checked = false;
                    chkLabelsRC.Checked = false;
                    if (cmbAdvanced.Items.Contains("(Click to Turn Off) Show Left Click Labels"))
                    {
                        cmbAdvanced.Items.Remove("(Click to Turn Off) Show Left Click Labels");
                        cmbAdvanced.Items.Add("(Click to Turn On) Show Left Click Labels");
                    }
                    if (cmbAdvanced.Items.Contains("(Click to Turn Off) Show Right Click Labels"))
                    {
                        cmbAdvanced.Items.Remove("(Click to Turn Off) Show Right Click Labels");
                        cmbAdvanced.Items.Add("(Click to Turn On) Show Right Click Labels");
                    }

                }
                else
                {
                    cmbAdvanced.Items.Insert(index, "(Click to Turn On) Show Mouse Move Labels"); // Insert the new item
                    cmbAdvanced.Items.RemoveAt(index + 1); // Remove the old item
                    chkLabelsHover.Checked = false;
                }
            }
            else if (cmbAdvanced.Items[cmbAdvanced.SelectedIndex].ToString().Contains("Show Left Click Labels"))
            {

                int index = cmbAdvanced.SelectedIndex;
                if (cmbAdvanced.Items[index].ToString().Contains("Click to Turn On"))
                {
                    cmbAdvanced.Items.Insert(index, "(Click to Turn Off) Show Left Click Labels"); // Insert the new item
                    cmbAdvanced.Items.RemoveAt(index + 1); // Remove the old item
                    chkLabelsLC.Checked = true;
                    chkLabelsHover.Checked = false;
                    chkLabelsRC.Checked = false;
                    if (cmbAdvanced.Items.Contains("(Click to Turn Off) Show Mouse Move Labels"))
                    {
                        cmbAdvanced.Items.Remove("(Click to Turn Off) Show Mouse Move Labels");
                        cmbAdvanced.Items.Add("(Click to Turn On) Show Mouse Move Labels");
                    }
                    if (cmbAdvanced.Items.Contains("(Click to Turn Off) Show Right Click Labels"))
                    {
                        cmbAdvanced.Items.Remove("(Click to Turn Off) Show Right Click Labels");
                        cmbAdvanced.Items.Add("(Click to Turn On) Show Right Click Labels");
                    }
                }
                else
                {
                    cmbAdvanced.Items.Insert(index, "(Click to Turn On) Show Left Click Labels"); // Insert the new item
                    cmbAdvanced.Items.RemoveAt(index + 1); // Remove the old item
                    chkLabelsLC.Checked = false;
                }
            }
            else if (cmbAdvanced.Items[cmbAdvanced.SelectedIndex].ToString().Contains("Show Right Click Labels"))
            {
                int index = cmbAdvanced.SelectedIndex;
                if (cmbAdvanced.Items[index].ToString().Contains("Click to Turn On"))
                {
                    cmbAdvanced.Items.Insert(index, "(Click to Turn Off) Show Right Click Labels"); // Insert the new item
                    cmbAdvanced.Items.RemoveAt(index + 1); // Remove the old item
                    chkLabelsRC.Checked = true;
                    chkLabelsHover.Checked = false;
                    chkLabelsLC.Checked = false;
                    if (cmbAdvanced.Items.Contains("(Click to Turn Off) Show Mouse Move Labels"))
                    {
                        cmbAdvanced.Items.Remove("(Click to Turn Off) Show Mouse Move Labels");
                        cmbAdvanced.Items.Add("(Click to Turn On) Show Mouse Move Labels");
                    }
                    if (cmbAdvanced.Items.Contains("(Click to Turn Off) Show Left Click Labels"))
                    {
                        cmbAdvanced.Items.Remove("(Click to Turn Off) Show Left Click Labels");
                        cmbAdvanced.Items.Add("(Click to Turn On) Show Left Click Labels");
                    }
                }
                else
                {
                    cmbAdvanced.Items.Insert(index, "(Click to Turn On) Show Right Click Labels"); // Insert the new item
                    cmbAdvanced.Items.RemoveAt(index + 1); // Remove the old item
                    chkLabelsRC.Checked = false;
                }
            }

            SyncAdvancedFlagsFromCurrentItems();

        }

        private void actTooltip(object sender, EventArgs e)
        {
            Control ctrl = sender as Control;

            if (ctrl != null)
            {
                if (ctrl.Name == "actUp")
                {
                    actUp.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togUpHover.Checked) ? "T" : "") + ((currentTab == "Hover" && varUpHover.Checked && !chkKeyboard.Checked) ? "V" : "") + ((currentTab == "Hover" && onceUp.Checked) ? "O" : "") + ((currentTab == "LC" && togUpLC.Checked) ? "T" : "") + ((currentTab == "LC" && rtcUpLC.Checked) ? "R" : "") + ((currentTab == "RC" && togUpRC.Checked) ? "T" : "") + ((currentTab == "RC" && rtcUpRC.Checked) ? "R" : "") + ")").Replace(" (Current - )", "");
                }
                else if (ctrl.Name == "actUpRight")
                {
                    actUpRight.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togUpRightHover.Checked) ? "T" : "") + ((currentTab == "Hover" && varUpRightHover.Checked && !chkKeyboard.Checked) ? "V" : "") + ((currentTab == "Hover" && onceUpRight.Checked) ? "O" : "") + ((currentTab == "Hover" && diagUpRightHover.Checked) ? "C" : "") + ((currentTab == "LC" && togUpRightLC.Checked) ? "T" : "") + ((currentTab == "LC" && rtcUpRightLC.Checked) ? "R" : "") + ((currentTab == "LC" && diagUpRightLC.Checked) ? "C" : "") + ((currentTab == "RC" && togUpRightRC.Checked) ? "T" : "") + ((currentTab == "RC" && rtcUpRightRC.Checked) ? "R" : "") + ((currentTab == "RC" && diagUpRightRC.Checked) ? "C" : "") + ")").Replace(" (Current - )", "");
                }
                else if (ctrl.Name == "actRight")
                {
                    actRight.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togRightHover.Checked) ? "T" : "") + ((currentTab == "Hover" && varRightHover.Checked && !chkKeyboard.Checked) ? "V" : "") + ((currentTab == "Hover" && onceRight.Checked) ? "O" : "") + ((currentTab == "LC" && togRightLC.Checked) ? "T" : "") + ((currentTab == "LC" && rtcRightLC.Checked) ? "R" : "") + ((currentTab == "RC" && togRightRC.Checked) ? "T" : "") + ((currentTab == "RC" && rtcRightRC.Checked) ? "R" : "") + ")").Replace(" (Current - )", "");
                }
                else if (ctrl.Name == "actDownRight")
                {
                    actDownRight.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togDownRightHover.Checked) ? "T" : "") + ((currentTab == "Hover" && varDownRightHover.Checked && !chkKeyboard.Checked) ? "V" : "") + ((currentTab == "Hover" && onceDownRight.Checked) ? "O" : "") + ((currentTab == "Hover" && diagDownRightHover.Checked) ? "C" : "") + ((currentTab == "LC" && togDownRightLC.Checked) ? "T" : "") + ((currentTab == "LC" && rtcDownRightLC.Checked) ? "R" : "") + ((currentTab == "LC" && diagDownRightLC.Checked) ? "C" : "") + ((currentTab == "RC" && togDownRightRC.Checked) ? "T" : "") + ((currentTab == "RC" && rtcDownRightRC.Checked) ? "R" : "") + ((currentTab == "RC" && diagDownRightRC.Checked) ? "C" : "") + ")").Replace(" (Current - )", "");
                }
                else if (ctrl.Name == "actDown")
                {
                    actDown.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togDownHover.Checked) ? "T" : "") + ((currentTab == "Hover" && varDownHover.Checked && !chkKeyboard.Checked) ? "V" : "") + ((currentTab == "Hover" && onceDown.Checked) ? "O" : "") + ((currentTab == "LC" && togDownLC.Checked) ? "T" : "") + ((currentTab == "LC" && rtcDownLC.Checked) ? "R" : "") + ((currentTab == "RC" && togDownRC.Checked) ? "T" : "") + ((currentTab == "RC" && rtcDownRC.Checked) ? "R" : "") + ")").Replace(" (Current - )", "");
                }
                else if (ctrl.Name == "actDownLeft")
                {

                    actDownLeft.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togDownLeftHover.Checked) ? "T" : "") + ((currentTab == "Hover" && varDownLeftHover.Checked && !chkKeyboard.Checked) ? "V" : "") + ((currentTab == "Hover" && onceDownLeft.Checked) ? "O" : "") + ((currentTab == "Hover" && diagDownLeftHover.Checked) ? "C" : "") + ((currentTab == "LC" && togDownLeftLC.Checked) ? "T" : "") + ((currentTab == "LC" && rtcDownLeftLC.Checked) ? "R" : "") + ((currentTab == "LC" && diagDownLeftLC.Checked) ? "C" : "") + ((currentTab == "RC" && togDownLeftRC.Checked) ? "T" : "") + ((currentTab == "RC" && rtcDownLeftRC.Checked) ? "R" : "") + ((currentTab == "RC" && diagDownLeftRC.Checked) ? "C" : "") + ")").Replace(" (Current - )", "");
                }
                else if (ctrl.Name == "actLeft")
                {
                    actLeft.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togLeftHover.Checked) ? "T" : "") + ((currentTab == "Hover" && varLeftHover.Checked && !chkKeyboard.Checked) ? "V" : "") + ((currentTab == "Hover" && onceLeft.Checked) ? "O" : "") + ((currentTab == "LC" && togLeftLC.Checked) ? "T" : "") + ((currentTab == "LC" && rtcLeftLC.Checked) ? "R" : "") + ((currentTab == "RC" && togLeftRC.Checked) ? "T" : "") + ((currentTab == "RC" && rtcLeftRC.Checked) ? "R" : "") + ")").Replace(" (Current - )", "");
                }
                else if (ctrl.Name == "actUpLeft")
                {
                    actUpLeft.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togUpLeftHover.Checked) ? "T" : "") + ((currentTab == "Hover" && varUpLeftHover.Checked && !chkKeyboard.Checked) ? "V" : "") + ((currentTab == "Hover" && onceUpLeft.Checked) ? "O" : "") + ((currentTab == "Hover" && diagUpLeftHover.Checked) ? "C" : "") + ((currentTab == "LC" && togUpLeftLC.Checked) ? "T" : "") + ((currentTab == "LC" && rtcUpLeftLC.Checked) ? "R" : "") + ((currentTab == "LC" && diagUpLeftLC.Checked) ? "C" : "") + ((currentTab == "RC" && togUpLeftRC.Checked) ? "T" : "") + ((currentTab == "RC" && rtcUpLeftRC.Checked) ? "R" : "") + ((currentTab == "RC" && diagUpLeftRC.Checked) ? "C" : "") + ")").Replace(" (Current - )", "");
                }
                else if (ctrl.Name == "actDZUpper")
                {
                    actDZUpper.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togDZUpperHover.Checked) ? "T" : "") + ((currentTab == "Hover" && onceDZUpper.Checked) ? "O" : "") + ((currentTab == "LC" && togDZUpperLC.Checked) ? "T" : "") + ((currentTab == "RC" && togDZUpperRC.Checked) ? "T" : "") + ")").Replace(" (Current - )", "");
                }
                else if (ctrl.Name == "actDZMiddle")
                {
                    actDZMiddle.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togDZMiddleHover.Checked) ? "T" : "") + ((currentTab == "Hover" && onceDZMiddle.Checked) ? "O" : "") + ((currentTab == "LC" && togDZMiddleLC.Checked) ? "T" : "") + ((currentTab == "RC" && togDZMiddleRC.Checked) ? "T" : "") + ")").Replace(" (Current - )", "");
                }
                else if (ctrl.Name == "actDZLower")
                {
                    actDZLower.ToolTipText = ("Click to Set Options (Current - " + ((currentTab == "Hover" && togDZLowerHover.Checked) ? "T" : "") + ((currentTab == "Hover" && onceDZLower.Checked) ? "O" : "") + ((currentTab == "LC" && togDZLowerLC.Checked) ? "T" : "") + ((currentTab == "RC" && togDZLowerRC.Checked) ? "T" : "") + ")").Replace(" (Current - )", "");
                }
            }
        }

        private void PressKey_MouseClick(object sender, MouseEventArgs e)
        {
            var ctrl = sender as Control;

            ctrl.Text = "Press Key";
            ToggleForm(false);

            if (ctrl.Name.Contains("DZ"))
            {
                DisableAllExceptOne(pnlAllSettings, pnlDeadzone, ctrl);
                btnFocus.Enabled = true;
                ctrl.BackColor = System.Drawing.Color.FromArgb(0, 0, 0);
            }
            else
            {
                DisableAllExceptOne(pnlAllSettings, pnlQuadrants, ctrl);
                btnFocus.Enabled = true;
                ctrl.BackColor = System.Drawing.Color.FromArgb(0, 0, 0);
            }

        }



        private void label2_Click(object sender, EventArgs e)
        {

            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://www.jettfoundation.org/stay-connected/",
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(psi);
        }

        private void lblDonate_Click(object sender, EventArgs e)
        {
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://www.jettfoundation.org/donate/",
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(psi);
        }

        private void btnGames_Click(object sender, EventArgs e)
        {
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://ourodyssey.org/games",
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(psi);
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (isLoadingProfile)
            {
                return;
            }

            if (!chkKeyboard.Checked)
            {
                return;
            }

            pnlDeadzone.SuspendLayout();
            pnlQuadrants.SuspendLayout();

            if (chkXbox.Checked)
            {
                chkXbox.Checked = false;
            }

            if (nintendoSwitch.Checked)
            {
                nintendoSwitch.Checked = false;
            }

            btnHover.PerformClick();

            chkKeyboard.Enabled = false;

            optUp.Items.Remove(optDisable); optUpRight.Items.Remove(optDisable); optRight.Items.Remove(optDisable); optDownRight.Items.Remove(optDisable);
            optDown.Items.Remove(optDisable); optDownLeft.Items.Remove(optDisable); optLeft.Items.Remove(optDisable); optUpLeft.Items.Remove(optDisable);
            optDZUpper.Items.Remove(optDisable); optDZMiddle.Items.Remove(optDisable); optDZLower.Items.Remove(optDisable);

            optUpRight.Items.Remove("Press Up-Right Quadrants");
            optUpRight.Items.Remove("Press Up & Right Quadrants");
            optDownRight.Items.Remove("Press Down & Right Quadrants");
            optUpLeft.Items.Remove("Press Up & Left Quadrants");
            optDownLeft.Items.Remove("Press Down & Left Quadrants");

            //optUpRight.Items.Add("Press Up-Right Quadrants");
            optDownRight.Items.Remove("Press Down-Right Quadrants");
            //optDownRight.Items.Add("Press Down-Right Quadrants");
            optUpLeft.Items.Remove("Press Up-Left Quadrants");
            //optUpLeft.Items.Add("Press Up-Left Quadrants");
            optDownLeft.Items.Remove("Press Down-Left Quadrants");
            //optDownLeft.Items.Add("Press Down-Left Quadrants");

            holdUpLC.Checked = false; holdUpRightLC.Checked = false; holdRightLC.Checked = false; holdDownRightLC.Checked = false;
            holdDownLC.Checked = false; holdDownLeftLC.Checked = false; holdLeftLC.Checked = false; holdUpLeftLC.Checked = false;

            holdUpRC.Checked = false; holdUpRightRC.Checked = false; holdRightRC.Checked = false; holdDownRightRC.Checked = false;
            holdDownRC.Checked = false; holdDownLeftRC.Checked = false; holdLeftRC.Checked = false; holdUpLeftRC.Checked = false;

            if (chkKeyboard.Checked == true && !nintendoSwitch.Checked && !chkXbox.Checked)
            {

                cmbAdvanced.Items.Remove("(Click to Turn On) Mario Kart Mode");
                cmbAdvanced.Items.Remove("(Click to Turn Off) Mario Kart Mode");
                cmbAdvanced.Items.Remove("(Click to Turn On) Keep Mouse Inside Overjoyed");
                cmbAdvanced.Items.Remove("(Click to Turn Off) Keep Mouse Inside Overjoyed");

                cmbAdvanced.Items.Remove("(Click to Turn On) Press PlayStation Buttons");
                cmbAdvanced.Items.Remove("(Click to Turn Off) Press PlayStation Buttons");
                if (advKeepMouse == true)
                {
                    cmbAdvanced.Items.Add("(Click to Turn On) Keep Mouse Inside Overjoyed");
                }
                else
                {
                    cmbAdvanced.Items.Add("(Click to Turn Off) Keep Mouse Inside Overjoyed");
                }
                cmbAdvanced.Items.Remove("(Click to Turn On) Prevent Game From Locking Mouse");
                cmbAdvanced.Items.Remove("(Click to Turn Off) Prevent Game From Locking Mouse");
                if (advMouseLock)
                {
                    cmbAdvanced.Items.Add("(Click to Turn On) Prevent Game From Locking Mouse");
                }
                else
                {
                    cmbAdvanced.Items.Add("(Click to Turn Off) Prevent Game From Locking Mouse");
                }
            }

            pnlDeadzone.ResumeLayout();
            pnlQuadrants.ResumeLayout();


        }



    }
}