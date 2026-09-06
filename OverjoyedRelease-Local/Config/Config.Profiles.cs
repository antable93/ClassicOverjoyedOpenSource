using ExtendedMessageBoxLibrary;
using GregsStack.InputSimulatorStandard.Native;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using Notion.Client;
using SharedVars;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
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

        public async Task getNotion()
        {
            // Create Notion client
            var client = NotionClientFactory.Create(new ClientOptions
            {
                AuthToken = "YOUR_NOTION_API_TOKEN"
            });

            // Query parameters (no filter/sort)
            var queryParams = new DatabasesQueryParameters
            {
                Filter = null,
                Sorts = null
            };

            // Query the database
            var queryResult = await client.Databases.QueryAsync(
                "YOUR_NOTION_DATABASE_ID",
                queryParams
            );

            // Loop through each page
            foreach (var r in queryResult.Results)
            {
                var page = await client.Pages.RetrieveAsync(r.Id);

                string settingsCode = "";
                string gameName = "";
                string contributors = "";
                string notes = "";

                // Helper method to extract text
                static string GetPropertyText(PropertyValue prop)
                {
                    switch (prop)
                    {
                        case TitlePropertyValue titleProp:
                            return string.Concat(titleProp.Title.Select(t => t.PlainText));
                        case RichTextPropertyValue richTextProp:
                            return string.Concat(richTextProp.RichText.Select(t => t.PlainText));
                        default:
                            return "";
                    }
                }

                static bool IsSupportedYes(PropertyValue prop)
                {
                    switch (prop)
                    {
                        case CheckboxPropertyValue checkboxProp:
                            return checkboxProp.Checkbox;
                        case SelectPropertyValue selectProp:
                            return string.Equals(selectProp.Select?.Name, "Yes", StringComparison.OrdinalIgnoreCase);
                        case StatusPropertyValue statusProp:
                            return string.Equals(statusProp.Status?.Name, "Yes", StringComparison.OrdinalIgnoreCase);
                        case RichTextPropertyValue richTextProp:
                            return string.Equals(string.Concat(richTextProp.RichText.Select(t => t.PlainText)).Trim(), "Yes", StringComparison.OrdinalIgnoreCase);
                        case TitlePropertyValue titleProp:
                            return string.Equals(string.Concat(titleProp.Title.Select(t => t.PlainText)).Trim(), "Yes", StringComparison.OrdinalIgnoreCase);
                        default:
                            return false;
                    }
                }

                // Extract all fields safely
                if (page.Properties.TryGetValue("Overjoyed Settings Code", out var prop1))
                    settingsCode = GetPropertyText(prop1);

                if (page.Properties.TryGetValue("Title of Game", out var prop2))
                    gameName = GetPropertyText(prop2);

                if (page.Properties.TryGetValue("Contributor(s)", out var prop3))
                    contributors = GetPropertyText(prop3);

                if (page.Properties.TryGetValue("Notes", out var prop4))
                    notes = GetPropertyText(prop4);

                // Only include items explicitly marked as supported.
                if (!page.Properties.TryGetValue("Supported?", out var supportedProp) || !IsSupportedYes(supportedProp))
                    continue;

                // Add to lists if settingsCode exists
                if (!string.IsNullOrWhiteSpace(settingsCode))
                {
                    settingsCodes.Add(settingsCode);
                    gameNames.Add(gameName);
                    gameNotes.Add(notes);
                    gameList.Add($"{gameName} - {contributors}");
                }
            }
        }



        private void LoadBackupList()
        {
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            backupRoot = Path.Combine(userProfile, "OverjoyedBackup");


            // Ensure backup root exists
            if (!Directory.Exists(backupRoot))
            {
                Directory.CreateDirectory(backupRoot);
            }


            var backups = Directory.GetDirectories(backupRoot)
                                   .Select(d => new DirectoryInfo(d))
                                   .OrderByDescending(d => d.LastWriteTime)
                                   .ToList();

            comboBackups.Items.Clear();
            comboBackups.Tag = backups;

            foreach (var dir in backups)
            {
                string version = "Unknown";
                if (dir.Name.StartsWith("OverjoyedRelease-"))
                {
                    var parts = dir.Name.Split('-');
                    if (parts.Length >= 2)
                        version = parts[1];
                }
                comboBackups.Items.Add($"{dir.LastWriteTime:MMMM d, yyyy - h:mm tt} - Version {version}");
            }

            if (comboBackups.Items.Count > 0)
                comboBackups.SelectedIndex = 0;
        }

        private void ShowBackupFileList(int index)
        {
            txtLog.Clear();
            if (index < 0) return;

            var backups = comboBackups.Tag as List<DirectoryInfo>;
            if (backups == null || index >= backups.Count) return;

            var selectedDir = backups[index];
            string profilesPath = Path.Combine(selectedDir.FullName, "profiles");

            if (!Directory.Exists(profilesPath))
            {
                txtLog.AppendText("No profiles folder in this backup.");
                return;
            }

            foreach (var file in Directory.GetFiles(profilesPath, "*", SearchOption.TopDirectoryOnly).OrderBy(f => f))
            {
                string nameWithoutExt = Path.GetFileNameWithoutExtension(file);
                if (excludedProfiles.Contains(nameWithoutExt, StringComparer.OrdinalIgnoreCase))
                    continue;
                txtLog.AppendText(nameWithoutExt + Environment.NewLine);
            }

            foreach (var dir in Directory.GetDirectories(profilesPath, "*", SearchOption.TopDirectoryOnly).OrderBy(d => d))
            {
                string name = Path.GetFileName(dir);
                if (excludedProfiles.Contains(name, StringComparer.OrdinalIgnoreCase))
                    continue;
                txtLog.AppendText(name + Environment.NewLine);
            }
        }

        private void RestoreSelectedBackup()
        {
            if (comboBackups.SelectedIndex < 0) return;

            var backups = comboBackups.Tag as List<DirectoryInfo>;
            if (backups == null || comboBackups.SelectedIndex >= backups.Count) return;

            var selectedDirInfo = backups[comboBackups.SelectedIndex];
            string datePrefix = selectedDirInfo.LastWriteTime.ToString("yyyyMMdd");
            string sourceDir = Path.Combine(selectedDirInfo.FullName, "profiles");
            string destDir = Path.Combine(FileSystem.Current.AppDataDirectory, "profiles");

            if (!Directory.Exists(sourceDir))
            {
                ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 1; -webkit-box-orient: vertical; overflow: hidden;' title='Selected backup has no profiles folder.'>Selected backup has no profiles folder.</span></p>", "No Profiles Folder", ExtendedMessageBoxLibrary.MessageBoxButtons.OK);
                return;
            }

            Directory.CreateDirectory(destDir);
            txtLog.Clear();
            CopyDirectoryWithPrefix(sourceDir, destDir, datePrefix);
            ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden;' title='Backup {selectedDirInfo.Name} restored with prefix {datePrefix}_.'>Backup '{selectedDirInfo.Name}' restored with prefix '{datePrefix}_'.</span></p>", "Backup Restored", ExtendedMessageBoxLibrary.MessageBoxButtons.OK, lines:2);
            backupPanel.Visible = false;

            UpdateConfigProfiles(destDir);
        }

        private void CopyDirectoryWithPrefix(string sourceDir, string destDir, string prefix)
        {
            foreach (var file in Directory.GetFiles(sourceDir))
            {
                string nameWithoutExt = Path.GetFileNameWithoutExtension(file);
                if (excludedProfiles.Contains(nameWithoutExt, StringComparer.OrdinalIgnoreCase))
                    continue;

                string destFile = Path.Combine(destDir, $"{prefix}_{Path.GetFileName(file)}");
                try
                {
                    File.Copy(file, destFile, true);
                    txtLog.AppendText($"Copied file: {destFile}{Environment.NewLine}");
                }
                catch (Exception ex)
                {
                    txtLog.AppendText($"Error copying file {file}: {ex.Message}{Environment.NewLine}");
                }
            }

            foreach (var directory in Directory.GetDirectories(sourceDir))
            {
                string name = Path.GetFileName(directory);
                if (excludedProfiles.Contains(name, StringComparer.OrdinalIgnoreCase))
                    continue;

                string destSubDir = Path.Combine(destDir, $"{prefix}_{name}");
                try
                {
                    Directory.CreateDirectory(destSubDir);
                    CopyDirectoryWithPrefix(directory, destSubDir, prefix);
                }
                catch (Exception ex)
                {
                    txtLog.AppendText($"Error copying directory {directory}: {ex.Message}{Environment.NewLine}");
                }
            }
        }

        private void UpdateConfigProfiles(string profilesPath)
        {
            cmbProfile.Items.Clear();
            cmbProfile.Items.Add("Music Mode");
            cmbProfile.Items.Add("Default Layout");
            // Get all files in the folder (optionally filter by extension, e.g., "*.txt")
            string[] files = Directory.GetFiles(profilesPath);
            foreach (var file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);

                if (fileName != "DefaultMusic" && fileName != "Default" && fileName != "Music" && fileName != "Default Layout")
                {
                    cmbProfile.Items.Add(fileName);
                }


            }

            cmbProfile.SelectedIndex = 1;
        }


        private async void NewBackup()
        {
            int playerNum = GetAssignedPlayerNum();

            await MinimizeMessenger.SendMessageAsync("backup|" + playerNum);
        }



        private void SettingsCode()
        {
            //string[] data = { "True", "True", "False", "Once", "True", "Once", "False" };

            var activationFlags = GetNormalizedCurrentActivationFlags();

            string[] data = {buttonConfig[0][1].ToLower().Replace("hold", "false"),
                            buttonConfig[0][2].ToLower().Replace("hold", "false"),
                            buttonConfig[0][5].ToLower().Replace("hold", "false"),
                            buttonConfig[1][1].ToLower().Replace("hold", "false"),
            buttonConfig[1][2].ToLower().Replace("hold", "false"),
            buttonConfig[1][5].ToLower().Replace("hold", "false"),
            buttonConfig[2][1].ToLower().Replace("hold", "false"),
            buttonConfig[2][2].ToLower().Replace("hold", "false"),
            buttonConfig[2][5].ToLower().Replace("hold", "false"),
            buttonConfig[3][1].ToLower().Replace("hold", "false"),
            buttonConfig[3][2].ToLower().Replace("hold", "false"),
            buttonConfig[3][5].ToLower().Replace("hold", "false"),
            buttonConfig[4][1].ToLower().Replace("hold", "false"),
            buttonConfig[4][2].ToLower().Replace("hold", "false"),
            buttonConfig[4][5].ToLower().Replace("hold", "false"),
            buttonConfig[5][1].ToLower().Replace("hold", "false"),
            buttonConfig[5][2].ToLower().Replace("hold", "false"),
            buttonConfig[5][5].ToLower().Replace("hold", "false"),
            buttonConfig[6][1].ToLower().Replace("hold", "false"),
            buttonConfig[6][2].ToLower().Replace("hold", "false"),
            buttonConfig[6][5].ToLower().Replace("hold", "false"),
            buttonConfig[7][1].ToLower().Replace("hold", "false"),
            buttonConfig[7][2].ToLower().Replace("hold", "false"),
            buttonConfig[7][5].ToLower().Replace("hold", "false"),
            buttonConfig[8][1].ToLower().Replace("hold", "false"),
            buttonConfig[8][2].ToLower().Replace("hold", "false"),
            buttonConfig[8][4].ToLower().Replace("hold", "false"),
            buttonConfig[9][1].ToLower().Replace("hold", "false"),
            buttonConfig[9][2].ToLower().Replace("hold", "false"),
            buttonConfig[9][4].ToLower().Replace("hold", "false"),
            buttonConfig[10][1].ToLower().Replace("hold", "false"),
            buttonConfig[10][2].ToLower().Replace("hold", "false"),
            buttonConfig[10][4].ToLower().Replace("hold", "false"),
            buttonConfig[11][1].ToLower().Replace("hold", "false"),
            buttonConfig[11][2].ToLower().Replace("hold", "false"),
            buttonConfig[11][4].ToLower().Replace("hold", "false"),
            buttonConfig[12][1].ToLower().Replace("hold", "false"),
            buttonConfig[12][2].ToLower().Replace("hold", "false"),
            buttonConfig[12][4].ToLower().Replace("hold", "false"),
            buttonConfig[13][1].ToLower().Replace("hold", "false"),
            buttonConfig[13][2].ToLower().Replace("hold", "false"),
            buttonConfig[13][4].ToLower().Replace("hold", "false"),
            buttonConfig[14][1].ToLower().Replace("hold", "false"),
            buttonConfig[14][2].ToLower().Replace("hold", "false"),
            buttonConfig[14][4].ToLower().Replace("hold", "false"),
            buttonConfig[15][1].ToLower().Replace("hold", "false"),
            buttonConfig[15][2].ToLower().Replace("hold", "false"),
            buttonConfig[15][4].ToLower().Replace("hold", "false"),
            buttonConfig[16][1].ToLower().Replace("hold", "false"),
            buttonConfig[16][2].ToLower().Replace("hold", "false"),
            buttonConfig[16][4].ToLower().Replace("hold", "false"),
            buttonConfig[17][1].ToLower().Replace("hold", "false"),
            buttonConfig[17][2].ToLower().Replace("hold", "false"),
            buttonConfig[17][4].ToLower().Replace("hold", "false"),
            buttonConfig[18][1].ToLower().Replace("hold", "false"),
            buttonConfig[18][2].ToLower().Replace("hold", "false"),
            buttonConfig[18][4].ToLower().Replace("hold", "false"),
            buttonConfig[19][1].ToLower().Replace("hold", "false"),
            buttonConfig[19][2].ToLower().Replace("hold", "false"),
            buttonConfig[19][4].ToLower().Replace("hold", "false"),
            buttonConfig[20][1].ToLower().Replace("hold", "false"),
            buttonConfig[20][2].ToLower().Replace("hold", "false"),
            buttonConfig[20][4].ToLower().Replace("hold", "false"),
            buttonConfig[21][1].ToLower().Replace("hold", "false"),
            buttonConfig[21][2].ToLower().Replace("hold", "false"),
            buttonConfig[21][4].ToLower().Replace("hold", "false"),
            buttonConfig[22][1].ToLower().Replace("hold", "false"),
            buttonConfig[22][2].ToLower().Replace("hold", "false"),
            buttonConfig[22][4].ToLower().Replace("hold", "false"),
            buttonConfig[23][1].ToLower().Replace("hold", "false"),
            buttonConfig[23][2].ToLower().Replace("hold", "false"),
            buttonConfig[23][4].ToLower().Replace("hold", "false"),
            buttonConfig[24][1].ToLower().Replace("hold", "false"),
            buttonConfig[24][2].ToLower().Replace("hold", "false"),
            buttonConfig[25][1].ToLower().Replace("hold", "false"),
            buttonConfig[25][2].ToLower().Replace("hold", "false"),
            buttonConfig[26][1].ToLower().Replace("hold", "false"),
            buttonConfig[26][2].ToLower().Replace("hold", "false"),
            buttonConfig[27][1].ToLower().Replace("hold", "false"),
            buttonConfig[27][2].ToLower().Replace("hold", "false"),
            buttonConfig[28][1].ToLower().Replace("hold", "false"),
            buttonConfig[28][2].ToLower().Replace("hold", "false"),
            buttonConfig[29][1].ToLower().Replace("hold", "false"),
            buttonConfig[29][2].ToLower().Replace("hold", "false"),
            buttonConfig[30][1].ToLower().Replace("hold", "false"),
            buttonConfig[30][2].ToLower().Replace("hold", "false"),
            buttonConfig[31][1].ToLower().Replace("hold", "false"),
            buttonConfig[31][2].ToLower().Replace("hold", "false"),
            buttonConfig[32][1].ToLower().Replace("hold", "false"),
            buttonConfig[32][2].ToLower().Replace("hold", "false"),
            activationFlags.upper.ToString().ToLower(),
            activationFlags.middle.ToString().ToLower(),
            activationFlags.lower.ToString().ToLower(),
            chkLabelsHover.Checked.ToString().ToLower(),
            chkLabelsLC.Checked.ToString().ToLower(),
            chkLabelsRC.Checked.ToString().ToLower(),
            diagUpRightHover.Checked.ToString().ToLower(),
            diagUpRightLC.Checked.ToString().ToLower(),
            diagUpRightRC.Checked.ToString().ToLower(),
            diagDownRightHover.Checked.ToString().ToLower(),
            diagDownRightLC.Checked.ToString().ToLower(),
            diagDownRightRC.Checked.ToString().ToLower(),
            diagUpLeftHover.Checked.ToString().ToLower(),
            diagUpLeftLC.Checked.ToString().ToLower(),
            diagUpLeftRC.Checked.ToString().ToLower(),
            diagDownLeftHover.Checked.ToString().ToLower(),
            diagDownLeftLC.Checked.ToString().ToLower(),
            diagDownLeftRC.Checked.ToString().ToLower() };

            for (int row = 8; row <= 23; row++)
            {
                if (string.Equals(buttonConfig[row][1], "Hold", StringComparison.OrdinalIgnoreCase))
                {
                    data[24 + ((row - 8) * 3)] = "hold";
                }
            }

            int[] holdCapableDeadzoneRows = { 25, 26, 28, 29, 31, 32 };
            int[] holdCapableDeadzoneDataIndexes = { 74, 76, 80, 82, 86, 88 };
            for (int holdIndex = 0; holdIndex < holdCapableDeadzoneRows.Length; holdIndex++)
            {
                if (string.Equals(buttonConfig[holdCapableDeadzoneRows[holdIndex]][1], "Hold", StringComparison.OrdinalIgnoreCase))
                {
                    data[holdCapableDeadzoneDataIndexes[holdIndex]] = "hold";
                }
            }

            Debug.WriteLine(string.Join(", ", data));


            string EncodeExactSettingsCodeToken(string token)
            {
                if (string.Equals(token, "True", StringComparison.OrdinalIgnoreCase))
                {
                    return "T";
                }

                if (string.Equals(token, "Once", StringComparison.OrdinalIgnoreCase))
                {
                    return "O";
                }

                if (string.Equals(token, "Hold", StringComparison.OrdinalIgnoreCase))
                {
                    return "H";
                }

                return "F";
            }

            string FormatCountB(int count)
            {
                string countString = count.ToString();
                if (countString.Length == 2)
                {

                    char firstDigit = (char)(countString[0] - '0' + 'A');
                    char secondDigit = (char)(countString[1] - '0' + 'A');
                    return firstDigit.ToString() + secondDigit.ToString();

                }
                else
                {

                    return countString;
                }
            }

            // Convert counts to the specified format
            string output = "Z" + string.Concat(data.Select(EncodeExactSettingsCodeToken));

            string[] GetSettings(string[][] buttonConfig, int column)
            {
                return buttonConfig.Select(row => row[column]).ToArray();
            }

            string[] kbSettings = GetSettings(buttonConfig, 0);
            string[] xboxSettings = GetSettings(buttonConfig, 3);
            string[] switchSettings = GetSettings(buttonConfig, 6);

            string FormatOutput(string[] settings, IList<string> options)
            {
                int[] positions = new int[settings.Length];
                for (int k = 0; k < settings.Length; k++)
                {
                    string setting = settings[k];
                    for (int j = 0; j < options.Count; j++)
                    {
                        if (string.Equals(setting, options[j], StringComparison.OrdinalIgnoreCase))
                        {
                            positions[k] = j;
                            break;
                        }
                    }
                }
                return string.Join("", positions.Select(FormatCountB));
            }

            string output2;
            if (nintendoSwitch.Checked)
            {
                output2 = FormatOutput(switchSettings, switchButtonOptions);
                output = "S-" + output;
            }
            else if (chkXbox.Checked)
            {
                output2 = FormatOutput(xboxSettings, buttonOptions);
                output = "X-" + output;
            }
            else
            {
                output2 = FormatOutput(kbSettings, keyboardOptions);
                output = "K-" + output;
            }

            string text;

            if (buttonConfig[0][1].ToLower().Replace("hold", "false") == "false")
            {
                text = "N" + output + "-" + output2;
            }
            else
            {
                text = "T" + output + "-" + output2;

            }

            string advanced = "-" + BuildAdvancedSettingsCode();


            text += advanced;


            try
            {
                Debug.WriteLine(text);

                setExport(text);

                //string filePath = Path.Combine(configPath, "Export.txt");


                //// Open or create the file with read/write permissions and allow other processes to access it
                //using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                //{
                //    using (StreamWriter writer = new StreamWriter(fileStream))
                //    {
                //        writer.WriteLine(text);
                //    }
                //}

                //Debug.WriteLine($"File written successfully to: {filePath}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred: {ex.Message}");
                // Optionally, log more detailed error information or take additional recovery actions
            }

            var answer = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 4; -webkit-box-orient: vertical; overflow: hidden;' title='Click Yes to submit your Game Controls Layout to the Overjoyed Game Library so people can play it with #NoSetupNecessary. Or click No to just copy the settings code to your clipboard.'>Click Yes to submit your Game Controls Layout to the Overjoyed Game Library so people can play it with #NoSetupNecessary. Or click No to just copy the settings code to your clipboard.</span></p>", "Submit Game Controls Layout?", ExtendedMessageBoxLibrary.MessageBoxButtons.YesNo, lines:4);

            if (answer.Result == DialogResult.Yes)
            {
                string game = Uri.EscapeDataString(cmbProfile.SelectedItem.ToString());

                string url = "https://forms.fillout.com/t/r5qFHGMShnus?game=" + game + "&code=" + Uri.EscapeDataString(text);
                // Open the URL in the default web browser
                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(psi);
            }
            else
            {
                ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 1; -webkit-box-orient: vertical; overflow: hidden; ' title='Settings Code was copied to your clipboard!'>Settings Code was copied to your clipboard!</span></p>", "Settings Code Copied", ExtendedMessageBoxLibrary.MessageBoxButtons.OK);

            }

        }

        private async void setExport(string text)
        {

            int playerNum = 1;

            playerNum = GetAssignedPlayerNum();

            await MinimizeMessenger.SendMessageAsync("export-" + text + "|" + playerNum);

        }



        private void btnNew_Click(object sender, EventArgs e)
        {
            btnAdd.Visible = true; btnCancel.Visible = true;
            btnProfileSettings.Visible = false;

            cmbProfile.Visible = false; cmbProfile.Enabled = false;
            txtConfigName.Visible = true; txtConfigName.Enabled = true;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            btnAdd.Visible = false; btnCancel.Visible = false;
            cmbProfile.Visible = true; cmbProfile.Enabled = true;
            txtConfigName.Visible = false; txtConfigName.Enabled = false;
            btnProfileSettings.Visible = true;

            string safeFileName = GetSafeFileName(txtConfigName);

            if (safeFileName != "")
            {
                cmbProfile.Items.Add(safeFileName);
            }

            txtConfigName.Text = "";


            cmbProfile.SelectedIndex = cmbProfile.Items.Count - 1;


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
            for (int i = 0; i <= cmbProfile.Items.Count; i++)
            {
                if (cmbProfile.SelectedIndex == i && cmbProfile.SelectedIndex > 0) { string profileText3 = cmbProfile.Items[i].ToString(); configName = profileText3 + ".txt"; }
                else if (cmbProfile.SelectedIndex == 0) { configName = "Music.txt"; }
            }

            try
            {

                // Open the file with shared read/write access
                using (FileStream fs = new FileStream(Path.Combine(profilesPath, configName), FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                using (StreamWriter file = new StreamWriter(fs))
                {
                    PrepareSettingsForSave();

                    WriteCurrentButtonConfigRows(file);


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

            defaultConfig = true;

            DefaultValues();

        }

        private void WriteCurrentButtonConfigRows(StreamWriter file)
        {
            for (int i = 0; i < 33; i++)
            {
                string state;
                if (i == 0)
                {
                    if (togUpHover.Checked) { state = "True"; }
                    else if (onceUp.Checked) { state = "Once"; }
                    else { state = "False"; }

                    if ((nintendoSwitch != null && nintendoSwitch.Checked && (switchUpHover == null || switchUpHover.SelectedItem == null || switchUpHover.SelectedItem.ToString() == "Click to Set" || switchUpHover.SelectedItem.ToString() == "Backtick")) || (chkXbox != null && chkXbox.Checked && (cmbUpHover == null || cmbUpHover.SelectedItem == null || cmbUpHover.SelectedItem.ToString() == "Click to Set" || cmbUpHover.SelectedItem.ToString() == "Backtick")) || (chkKeyboard != null && chkKeyboard.Checked && (btnUpHover == null || string.IsNullOrEmpty(btnUpHover.Text) || btnUpHover.Text == "Click to Set" || btnUpHover.Text == "Backtick"))) { }

                    string xboxAction = cmbUpHover.SelectedItem != null ? cmbUpHover.SelectedItem.ToString() : "None"; xboxAction = string.IsNullOrEmpty(xboxAction) || xboxAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : xboxAction;
                    string switchAction = switchUpHover.SelectedItem != null ? switchUpHover.SelectedItem.ToString() : "None"; switchAction = string.IsNullOrEmpty(switchAction) || switchAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : switchAction;

                    if (xboxAction != "None" && switchAction == "None") { switchAction = switchButtonOptions[Array.IndexOf(buttonOptions, xboxAction)]; switchUpHover.SelectedItem = switchAction; }
                    else if (switchAction != "None" && xboxAction == "None") { xboxAction = buttonOptions[Array.IndexOf(switchButtonOptions, switchAction)]; cmbUpHover.SelectedItem = xboxAction; }

                    buttonConfig[i] = new string[] { directions != null && i < directions.Length && directions[i] != null ? directions[i] : "Backtick", state ?? "False", disUpHover != null ? disUpHover.Checked.ToString() : "False", cmbUpHover != null && cmbUpHover.SelectedItem != null ? cmbUpHover.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None", "False", varUpHover != null ? varUpHover.Checked.ToString() : "False", switchUpHover != null && switchUpHover.SelectedItem != null ? switchUpHover.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None" };
                }
                else if (i == 1)
                {
                    if (togUpRightHover.Checked) { state = "True"; } else if (onceUpRight.Checked) { state = "Once"; } else { state = "False"; }

                    if ((nintendoSwitch != null && nintendoSwitch.Checked && (switchUpRightHover == null || switchUpRightHover.SelectedItem == null || switchUpRightHover.SelectedItem.ToString() == "Click to Set" || switchUpRightHover.SelectedItem.ToString() == "Backtick")) || (chkXbox != null && chkXbox.Checked && (cmbUpRightHover == null || cmbUpRightHover.SelectedItem == null || cmbUpRightHover.SelectedItem.ToString() == "Click to Set" || cmbUpRightHover.SelectedItem.ToString() == "Backtick")) || (chkKeyboard != null && chkKeyboard.Checked && (btnUpRightHover == null || string.IsNullOrEmpty(btnUpRightHover.Text) || btnUpRightHover.Text == "Click to Set" || btnUpRightHover.Text == "Backtick"))) { }


                    string xboxAction = cmbUpRightHover.SelectedItem != null ? cmbUpRightHover.SelectedItem.ToString() : "None"; xboxAction = string.IsNullOrEmpty(xboxAction) || xboxAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : xboxAction;
                    string switchAction = switchUpRightHover.SelectedItem != null ? switchUpRightHover.SelectedItem.ToString() : "None"; switchAction = string.IsNullOrEmpty(switchAction) || switchAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : switchAction;


                    if (xboxAction != "None" && switchAction == "None") { switchAction = switchButtonOptions[Array.IndexOf(buttonOptions, xboxAction)]; switchUpRightHover.SelectedItem = switchAction; }
                    else if (switchAction != "None" && xboxAction == "None") { xboxAction = buttonOptions[Array.IndexOf(switchButtonOptions, switchAction)]; cmbUpRightHover.SelectedItem = xboxAction; }


                    buttonConfig[i] = new string[] { directions != null && i < directions.Length && directions[i] != null ? directions[i] : "Backtick", state ?? "False", disUpRightHover != null ? disUpRightHover.Checked.ToString() : "False", cmbUpRightHover != null && cmbUpRightHover.SelectedItem != null ? cmbUpRightHover.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None", "False", varUpRightHover != null ? varUpRightHover.Checked.ToString() : "False", switchUpRightHover != null && switchUpRightHover.SelectedItem != null ? switchUpRightHover.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None" };
                }
                else if (i == 2)
                {
                    if (togRightHover.Checked) { state = "True"; } else if (onceRight.Checked) { state = "Once"; } else { state = "False"; }

                    if ((nintendoSwitch != null && nintendoSwitch.Checked && (switchRightHover == null || switchRightHover.SelectedItem == null || switchRightHover.SelectedItem.ToString() == "Click to Set" || switchRightHover.SelectedItem.ToString() == "Backtick")) || (chkXbox != null && chkXbox.Checked && (cmbRightHover == null || cmbRightHover.SelectedItem == null || cmbRightHover.SelectedItem.ToString() == "Click to Set" || cmbRightHover.SelectedItem.ToString() == "Backtick")) || (chkKeyboard != null && chkKeyboard.Checked && (btnRightHover == null || string.IsNullOrEmpty(btnRightHover.Text) || btnRightHover.Text == "Click to Set" || btnRightHover.Text == "Backtick"))) { }

                    string xboxAction = cmbRightHover.SelectedItem != null ? cmbRightHover.SelectedItem.ToString() : "None"; xboxAction = string.IsNullOrEmpty(xboxAction) || xboxAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : xboxAction;
                    string switchAction = switchRightHover.SelectedItem != null ? switchRightHover.SelectedItem.ToString() : "None"; switchAction = string.IsNullOrEmpty(switchAction) || switchAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : switchAction;

                    if (xboxAction != "None" && switchAction == "None") { switchAction = switchButtonOptions[Array.IndexOf(buttonOptions, xboxAction)]; switchRightHover.SelectedItem = switchAction; }
                    else if (switchAction != "None" && xboxAction == "None") { xboxAction = buttonOptions[Array.IndexOf(switchButtonOptions, switchAction)]; cmbRightHover.SelectedItem = xboxAction; }

                    buttonConfig[i] = new string[] { directions != null && i < directions.Length && directions[i] != null ? directions[i] : "Backtick", state ?? "False", disRightHover != null ? disRightHover.Checked.ToString() : "False", cmbRightHover != null && cmbRightHover.SelectedItem != null ? cmbRightHover.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None", "False", varRightHover != null ? varRightHover.Checked.ToString() : "False", switchRightHover != null && switchRightHover.SelectedItem != null ? switchRightHover.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None" };
                }
                else if (i == 3)
                {
                    if (togDownRightHover.Checked) { state = "True"; } else if (onceDownRight.Checked) { state = "Once"; } else { state = "False"; }

                    if ((nintendoSwitch != null && nintendoSwitch.Checked && (switchDownRightHover == null || switchDownRightHover.SelectedItem == null || switchDownRightHover.SelectedItem.ToString() == "Click to Set" || switchDownRightHover.SelectedItem.ToString() == "Backtick")) || (chkXbox != null && chkXbox.Checked && (cmbDownRightHover == null || cmbDownRightHover.SelectedItem == null || cmbDownRightHover.SelectedItem.ToString() == "Click to Set" || cmbDownRightHover.SelectedItem.ToString() == "Backtick")) || (chkKeyboard != null && chkKeyboard.Checked && (btnDownRightHover == null || string.IsNullOrEmpty(btnDownRightHover.Text) || btnDownRightHover.Text == "Click to Set" || btnDownRightHover.Text == "Backtick"))) { }

                    string xboxAction = cmbDownRightHover.SelectedItem != null ? cmbDownRightHover.SelectedItem.ToString() : "None"; xboxAction = string.IsNullOrEmpty(xboxAction) || xboxAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : xboxAction;
                    string switchAction = switchDownRightHover.SelectedItem != null ? switchDownRightHover.SelectedItem.ToString() : "None"; switchAction = string.IsNullOrEmpty(switchAction) || switchAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : switchAction;

                    if (xboxAction != "None" && switchAction == "None") { switchAction = switchButtonOptions[Array.IndexOf(buttonOptions, xboxAction)]; switchDownRightHover.SelectedItem = switchAction; }
                    else if (switchAction != "None" && xboxAction == "None") { xboxAction = buttonOptions[Array.IndexOf(switchButtonOptions, switchAction)]; cmbDownRightHover.SelectedItem = xboxAction; }

                    buttonConfig[i] = new string[] { directions != null && i < directions.Length && directions[i] != null ? directions[i] : "Backtick", state ?? "False", disDownRightHover != null ? disDownRightHover.Checked.ToString() : "False", cmbDownRightHover != null && cmbDownRightHover.SelectedItem != null ? cmbDownRightHover.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None", "False", varDownRightHover != null ? varDownRightHover.Checked.ToString() : "False", switchDownRightHover != null && switchDownRightHover.SelectedItem != null ? switchDownRightHover.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None" };
                }
                else if (i == 4)
                {
                    if (togDownHover.Checked) { state = "True"; } else if (onceDown.Checked) { state = "Once"; } else { state = "False"; }

                    if ((nintendoSwitch != null && nintendoSwitch.Checked && (switchDownHover == null || switchDownHover.SelectedItem == null || switchDownHover.SelectedItem.ToString() == "Click to Set" || switchDownHover.SelectedItem.ToString() == "Backtick")) || (chkXbox != null && chkXbox.Checked && (cmbDownHover == null || cmbDownHover.SelectedItem == null || cmbDownHover.SelectedItem.ToString() == "Click to Set" || cmbDownHover.SelectedItem.ToString() == "Backtick")) || (chkKeyboard != null && chkKeyboard.Checked && (btnDownHover == null || string.IsNullOrEmpty(btnDownHover.Text) || btnDownHover.Text == "Click to Set" || btnDownHover.Text == "Backtick"))) { }

                    string xboxAction = cmbDownHover.SelectedItem != null ? cmbDownHover.SelectedItem.ToString() : "None"; xboxAction = string.IsNullOrEmpty(xboxAction) || xboxAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : xboxAction;
                    string switchAction = switchDownHover.SelectedItem != null ? switchDownHover.SelectedItem.ToString() : "None"; switchAction = string.IsNullOrEmpty(switchAction) || switchAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : switchAction;

                    if (xboxAction != "None" && switchAction == "None") { switchAction = switchButtonOptions[Array.IndexOf(buttonOptions, xboxAction)]; switchDownHover.SelectedItem = switchAction; }
                    else if (switchAction != "None" && xboxAction == "None") { xboxAction = buttonOptions[Array.IndexOf(switchButtonOptions, switchAction)]; cmbDownHover.SelectedItem = xboxAction; }


                    buttonConfig[i] = new string[] { directions != null && i < directions.Length && directions[i] != null ? directions[i] : "Backtick", state ?? "False", disDownHover != null ? disDownHover.Checked.ToString() : "False", cmbDownHover != null && cmbDownHover.SelectedItem != null ? cmbDownHover.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None", "False", varDownHover != null ? varDownHover.Checked.ToString() : "False", switchDownHover != null && switchDownHover.SelectedItem != null ? switchDownHover.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None" };
                }
                else if (i == 5)
                {
                    if (togDownLeftHover.Checked) { state = "True"; } else if (onceDownLeft.Checked) { state = "Once"; } else { state = "False"; }

                    if ((nintendoSwitch != null && nintendoSwitch.Checked && (switchDownLeftHover == null || switchDownLeftHover.SelectedItem == null || switchDownLeftHover.SelectedItem.ToString() == "Click to Set" || switchDownLeftHover.SelectedItem.ToString() == "Backtick")) || (chkXbox != null && chkXbox.Checked && (cmbDownLeftHover == null || cmbDownLeftHover.SelectedItem == null || cmbDownLeftHover.SelectedItem.ToString() == "Click to Set" || cmbDownLeftHover.SelectedItem.ToString() == "Backtick")) || (chkKeyboard != null && chkKeyboard.Checked && (btnDownLeftHover == null || string.IsNullOrEmpty(btnDownLeftHover.Text) || btnDownLeftHover.Text == "Click to Set" || btnDownLeftHover.Text == "Backtick"))) { }

                    string xboxAction = cmbDownLeftHover.SelectedItem != null ? cmbDownLeftHover.SelectedItem.ToString() : "None"; xboxAction = string.IsNullOrEmpty(xboxAction) || xboxAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : xboxAction;
                    string switchAction = switchDownLeftHover.SelectedItem != null ? switchDownLeftHover.SelectedItem.ToString() : "None"; switchAction = string.IsNullOrEmpty(switchAction) || switchAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : switchAction;

                    if (xboxAction != "None" && switchAction == "None") { switchAction = switchButtonOptions[Array.IndexOf(buttonOptions, xboxAction)]; switchDownLeftHover.SelectedItem = switchAction; }
                    else if (switchAction != "None" && xboxAction == "None") { xboxAction = buttonOptions[Array.IndexOf(switchButtonOptions, switchAction)]; cmbDownLeftHover.SelectedItem = xboxAction; }

                    buttonConfig[i] = new string[] { directions != null && i < directions.Length && directions[i] != null ? directions[i] : "Backtick", state ?? "False", disDownLeftHover != null ? disDownLeftHover.Checked.ToString() : "False", cmbDownLeftHover != null && cmbDownLeftHover.SelectedItem != null ? cmbDownLeftHover.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None", "False", varDownLeftHover != null ? varDownLeftHover.Checked.ToString() : "False", switchDownLeftHover != null && switchDownLeftHover.SelectedItem != null ? switchDownLeftHover.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None" };
                }
                else if (i == 6)
                {
                    if (togLeftHover.Checked) { state = "True"; } else if (onceLeft.Checked) { state = "Once"; } else { state = "False"; }

                    if ((nintendoSwitch != null && nintendoSwitch.Checked && (switchLeftHover == null || switchLeftHover.SelectedItem == null || switchLeftHover.SelectedItem.ToString() == "Click to Set" || switchLeftHover.SelectedItem.ToString() == "Backtick")) || (chkXbox != null && chkXbox.Checked && (cmbLeftHover == null || cmbLeftHover.SelectedItem == null || cmbLeftHover.SelectedItem.ToString() == "Click to Set" || cmbLeftHover.SelectedItem.ToString() == "Backtick")) || (chkKeyboard != null && chkKeyboard.Checked && (btnLeftHover == null || string.IsNullOrEmpty(btnLeftHover.Text) || btnLeftHover.Text == "Click to Set" || btnLeftHover.Text == "Backtick"))) { }

                    string xboxAction = cmbLeftHover.SelectedItem != null ? cmbLeftHover.SelectedItem.ToString() : "None"; xboxAction = string.IsNullOrEmpty(xboxAction) || xboxAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : xboxAction;
                    string switchAction = switchLeftHover.SelectedItem != null ? switchLeftHover.SelectedItem.ToString() : "None"; switchAction = string.IsNullOrEmpty(switchAction) || switchAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : switchAction;

                    if (xboxAction != "None" && switchAction == "None") { switchAction = switchButtonOptions[Array.IndexOf(buttonOptions, xboxAction)]; switchLeftHover.SelectedItem = switchAction; }
                    else if (switchAction != "None" && xboxAction == "None") { xboxAction = buttonOptions[Array.IndexOf(switchButtonOptions, switchAction)]; cmbLeftHover.SelectedItem = xboxAction; }

                    buttonConfig[i] = new string[] { directions != null && i < directions.Length && directions[i] != null ? directions[i] : "Backtick", state ?? "False", disLeftHover != null ? disLeftHover.Checked.ToString() : "False", cmbLeftHover != null && cmbLeftHover.SelectedItem != null ? cmbLeftHover.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None", "False", varLeftHover != null ? varLeftHover.Checked.ToString() : "False", switchLeftHover != null && switchLeftHover.SelectedItem != null ? switchLeftHover.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None" };
                }
                else if (i == 7)
                {
                    if (togUpLeftHover.Checked) { state = "True"; } else if (onceUpLeft.Checked) { state = "Once"; } else { state = "False"; }

                    if ((nintendoSwitch != null && nintendoSwitch.Checked && (switchUpLeftHover == null || switchUpLeftHover.SelectedItem == null || switchUpLeftHover.SelectedItem.ToString() == "Click to Set" || switchUpLeftHover.SelectedItem.ToString() == "Backtick")) || (chkXbox != null && chkXbox.Checked && (cmbUpLeftHover == null || cmbUpLeftHover.SelectedItem == null || cmbUpLeftHover.SelectedItem.ToString() == "Click to Set" || cmbUpLeftHover.SelectedItem.ToString() == "Backtick")) || (chkKeyboard != null && chkKeyboard.Checked && (btnUpLeftHover == null || string.IsNullOrEmpty(btnUpLeftHover.Text) || btnUpLeftHover.Text == "Click to Set" || btnUpLeftHover.Text == "Backtick"))) { }
                    string xboxAction = cmbUpLeftHover.SelectedItem != null ? cmbUpLeftHover.SelectedItem.ToString() : "None"; xboxAction = string.IsNullOrEmpty(xboxAction) || xboxAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : xboxAction;
                    string switchAction = switchUpLeftHover.SelectedItem != null ? switchUpLeftHover.SelectedItem.ToString() : "None"; switchAction = string.IsNullOrEmpty(switchAction) || switchAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : switchAction;

                    if (xboxAction != "None" && switchAction == "None") { switchAction = switchButtonOptions[Array.IndexOf(buttonOptions, xboxAction)]; switchUpLeftHover.SelectedItem = switchAction; }
                    else if (switchAction != "None" && xboxAction == "None") { xboxAction = buttonOptions[Array.IndexOf(switchButtonOptions, switchAction)]; cmbUpLeftHover.SelectedItem = xboxAction; }

                    buttonConfig[i] = new string[] { directions != null && i < directions.Length && directions[i] != null ? directions[i] : "Backtick", state ?? "False", disUpLeftHover != null ? disUpLeftHover.Checked.ToString() : "False", cmbUpLeftHover != null && cmbUpLeftHover.SelectedItem != null ? cmbUpLeftHover.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None", "False", varUpLeftHover != null ? varUpLeftHover.Checked.ToString() : "False", switchUpLeftHover != null && switchUpLeftHover.SelectedItem != null ? switchUpLeftHover.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None" };
                }
                else if (i == 8)
                {
                    if (togUpLC.Checked) { state = "True"; } else if (onceUpLC.Checked) { state = "Once"; } else if (holdUpLC.Checked) { state = "Hold"; } else { state = "Hold"; }

                    if ((nintendoSwitch != null && nintendoSwitch.Checked && (switchUpLC == null || switchUpLC.SelectedItem == null || switchUpLC.SelectedItem.ToString() == "Click to Set" || switchUpLC.SelectedItem.ToString() == "Backtick")) || (chkXbox != null && chkXbox.Checked && (cmbUpLclick == null || cmbUpLclick.SelectedItem == null || cmbUpLclick.SelectedItem.ToString() == "Click to Set" || cmbUpLclick.SelectedItem.ToString() == "Backtick")) || (chkKeyboard != null && chkKeyboard.Checked && (btnUpLclick == null || string.IsNullOrEmpty(btnUpLclick.Text) || btnUpLclick.Text == "Click to Set" || btnUpLclick.Text == "Backtick"))) { }

                    string xboxAction = cmbUpLclick.SelectedItem != null ? cmbUpLclick.SelectedItem.ToString() : "None"; xboxAction = string.IsNullOrEmpty(xboxAction) || xboxAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : xboxAction;
                    string switchAction = switchUpLC.SelectedItem != null ? switchUpLC.SelectedItem.ToString() : "None"; switchAction = string.IsNullOrEmpty(switchAction) || switchAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : switchAction;


                    if (xboxAction != "None" && switchAction == "None") { switchAction = switchButtonOptions[Array.IndexOf(buttonOptions, xboxAction)]; switchUpLC.SelectedItem = switchAction; }
                    else if (switchAction != "None" && xboxAction == "None") { xboxAction = buttonOptions[Array.IndexOf(switchButtonOptions, switchAction)]; cmbUpLclick.SelectedItem = xboxAction; }

                    buttonConfig[i] = new string[] { directions != null && i < directions.Length && directions[i] != null ? directions[i] : "Backtick", state ?? "False", disUpLC != null ? disUpLC.Checked.ToString() : "False", cmbUpLclick != null && cmbUpLclick.SelectedItem != null ? cmbUpLclick.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None", rtcUpLC != null ? rtcUpLC.Checked.ToString() : "False", "False", switchUpLC != null && switchUpLC.SelectedItem != null ? switchUpLC.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None" };
                }
                else if (i == 9)
                {
                    if (togUpRightLC.Checked) { state = "True"; } else if (onceUpRightLC.Checked) { state = "Once"; } else if (holdUpRightLC.Checked) { state = "Hold"; } else { state = "Hold"; }

                    if ((nintendoSwitch != null && nintendoSwitch.Checked && (switchUpRightLC == null || switchUpRightLC.SelectedItem == null || switchUpRightLC.SelectedItem.ToString() == "Click to Set" || switchUpRightLC.SelectedItem.ToString() == "Backtick")) || (chkXbox != null && chkXbox.Checked && (cmbUpRightLclick == null || cmbUpRightLclick.SelectedItem == null || cmbUpRightLclick.SelectedItem.ToString() == "Click to Set" || cmbUpRightLclick.SelectedItem.ToString() == "Backtick")) || (chkKeyboard != null && chkKeyboard.Checked && (btnUpRightLclick == null || string.IsNullOrEmpty(btnUpRightLclick.Text) || btnUpRightLclick.Text == "Click to Set" || btnUpRightLclick.Text == "Backtick"))) { }


                    string xboxAction = cmbUpRightLclick.SelectedItem != null ? cmbUpRightLclick.SelectedItem.ToString() : "None"; xboxAction = string.IsNullOrEmpty(xboxAction) || xboxAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : xboxAction;
                    string switchAction = switchUpRightLC.SelectedItem != null ? switchUpRightLC.SelectedItem.ToString() : "None"; switchAction = string.IsNullOrEmpty(switchAction) || switchAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : switchAction;

                    if (xboxAction != "None" && switchAction == "None") { switchAction = switchButtonOptions[Array.IndexOf(buttonOptions, xboxAction)]; switchUpRightLC.SelectedItem = switchAction; }
                    else if (switchAction != "None" && xboxAction == "None") { xboxAction = buttonOptions[Array.IndexOf(switchButtonOptions, switchAction)]; cmbUpRightLclick.SelectedItem = xboxAction; }

                    buttonConfig[i] = new string[] { directions != null && i < directions.Length && directions[i] != null ? directions[i] : "Backtick", state ?? "False", disUpRightLC != null ? disUpRightLC.Checked.ToString() : "False", cmbUpRightLclick != null && cmbUpRightLclick.SelectedItem != null ? cmbUpRightLclick.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None", rtcUpRightLC != null ? rtcUpRightLC.Checked.ToString() : "False", "False", switchUpRightLC != null && switchUpRightLC.SelectedItem != null ? switchUpRightLC.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None" };
                }
                else if (i == 10)
                {
                    if (togRightLC.Checked) { state = "True"; } else if (onceRightLC.Checked) { state = "Once"; } else if (holdRightLC.Checked) { state = "Hold"; } else { state = "Hold"; }

                    if ((nintendoSwitch != null && nintendoSwitch.Checked && (switchRightLC == null || switchRightLC.SelectedItem == null || switchRightLC.SelectedItem.ToString() == "Click to Set" || switchRightLC.SelectedItem.ToString() == "Backtick")) || (chkXbox != null && chkXbox.Checked && (cmbRightLclick == null || cmbRightLclick.SelectedItem == null || cmbRightLclick.SelectedItem.ToString() == "Click to Set" || cmbRightLclick.SelectedItem.ToString() == "Backtick")) || (chkKeyboard != null && chkKeyboard.Checked && (btnRightLclick == null || string.IsNullOrEmpty(btnRightLclick.Text) || btnRightLclick.Text == "Click to Set" || btnRightLclick.Text == "Backtick"))) { }

                    string xboxAction = cmbRightLclick.SelectedItem != null ? cmbRightLclick.SelectedItem.ToString() : "None"; xboxAction = string.IsNullOrEmpty(xboxAction) || xboxAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : xboxAction;
                    string switchAction = switchRightLC.SelectedItem != null ? switchRightLC.SelectedItem.ToString() : "None"; switchAction = string.IsNullOrEmpty(switchAction) || switchAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : switchAction;

                    if (xboxAction != "None" && switchAction == "None") { switchAction = switchButtonOptions[Array.IndexOf(buttonOptions, xboxAction)]; switchRightLC.SelectedItem = switchAction; }
                    else if (switchAction != "None" && xboxAction == "None") { xboxAction = buttonOptions[Array.IndexOf(switchButtonOptions, switchAction)]; cmbRightLclick.SelectedItem = xboxAction; }
                    buttonConfig[i] = new string[] { directions != null && i < directions.Length && directions[i] != null ? directions[i] : "Backtick", state ?? "False", disRightLC != null ? disRightLC.Checked.ToString() : "False", cmbRightLclick != null && cmbRightLclick.SelectedItem != null ? cmbRightLclick.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None", rtcRightLC != null ? rtcRightLC.Checked.ToString() : "False", "False", switchRightLC != null && switchRightLC.SelectedItem != null ? switchRightLC.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None" };
                }
                else if (i == 11)
                {
                    if (togDownRightLC.Checked) { state = "True"; } else if (onceDownRightLC.Checked) { state = "Once"; } else if (holdDownRightLC.Checked) { state = "Hold"; } else { state = "Hold"; }


                    if ((nintendoSwitch != null && nintendoSwitch.Checked && (switchDownRightLC == null || switchDownRightLC.SelectedItem == null || switchDownRightLC.SelectedItem.ToString() == "Click to Set" || switchDownRightLC.SelectedItem.ToString() == "Backtick")) || (chkXbox != null && chkXbox.Checked && (cmbDownRightLclick == null || cmbDownRightLclick.SelectedItem == null || cmbDownRightLclick.SelectedItem.ToString() == "Click to Set" || cmbDownRightLclick.SelectedItem.ToString() == "Backtick")) || (chkKeyboard != null && chkKeyboard.Checked && (btnDownRightLclick == null || string.IsNullOrEmpty(btnDownRightLclick.Text) || btnDownRightLclick.Text == "Click to Set" || btnDownRightLclick.Text == "Backtick"))) { }

                    string xboxAction = cmbDownRightLclick.SelectedItem != null ? cmbDownRightLclick.SelectedItem.ToString() : "None"; xboxAction = string.IsNullOrEmpty(xboxAction) || xboxAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : xboxAction;
                    string switchAction = switchDownRightLC.SelectedItem != null ? switchDownRightLC.SelectedItem.ToString() : "None"; switchAction = string.IsNullOrEmpty(switchAction) || switchAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : switchAction;

                    if (xboxAction != "None" && switchAction == "None") { switchAction = switchButtonOptions[Array.IndexOf(buttonOptions, xboxAction)]; switchDownRightLC.SelectedItem = switchAction; }
                    else if (switchAction != "None" && xboxAction == "None") { xboxAction = buttonOptions[Array.IndexOf(switchButtonOptions, switchAction)]; cmbDownRightLclick.SelectedItem = xboxAction; }
                    buttonConfig[i] = new string[] { directions != null && i < directions.Length && directions[i] != null ? directions[i] : "Backtick", state ?? "False", disDownRightLC != null ? disDownRightLC.Checked.ToString() : "False", cmbDownRightLclick != null && cmbDownRightLclick.SelectedItem != null ? cmbDownRightLclick.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None", rtcDownRightLC != null ? rtcDownRightLC.Checked.ToString() : "False", "False", switchDownRightLC != null && switchDownRightLC.SelectedItem != null ? switchDownRightLC.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None" };
                }
                else if (i == 12)
                {
                    if (togDownLC.Checked) { state = "True"; } else if (onceDownLC.Checked) { state = "Once"; } else if (holdDownLC.Checked) { state = "Hold"; } else { state = "Hold"; }


                    if ((nintendoSwitch != null && nintendoSwitch.Checked && (switchDownLC == null || switchDownLC.SelectedItem == null || switchDownLC.SelectedItem.ToString() == "Click to Set" || switchDownLC.SelectedItem.ToString() == "Backtick")) || (chkXbox != null && chkXbox.Checked && (cmbDownLclick == null || cmbDownLclick.SelectedItem == null || cmbDownLclick.SelectedItem.ToString() == "Click to Set" || cmbDownLclick.SelectedItem.ToString() == "Backtick")) || (chkKeyboard != null && chkKeyboard.Checked && (btnDownLclick == null || string.IsNullOrEmpty(btnDownLclick.Text) || btnDownLclick.Text == "Click to Set" || btnDownLclick.Text == "Backtick"))) { }

                    string xboxAction = cmbDownLclick.SelectedItem != null ? cmbDownLclick.SelectedItem.ToString() : "None"; xboxAction = string.IsNullOrEmpty(xboxAction) || xboxAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : xboxAction;
                    string switchAction = switchDownLC.SelectedItem != null ? switchDownLC.SelectedItem.ToString() : "None"; switchAction = string.IsNullOrEmpty(switchAction) || switchAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : switchAction;

                    if (xboxAction != "None" && switchAction == "None") { switchAction = switchButtonOptions[Array.IndexOf(buttonOptions, xboxAction)]; switchDownLC.SelectedItem = switchAction; }
                    else if (switchAction != "None" && xboxAction == "None") { xboxAction = buttonOptions[Array.IndexOf(switchButtonOptions, switchAction)]; cmbDownLclick.SelectedItem = xboxAction; }
                    buttonConfig[i] = new string[] { directions != null && i < directions.Length && directions[i] != null ? directions[i] : "Backtick", state ?? "False", disDownLC != null ? disDownLC.Checked.ToString() : "False", cmbDownLclick != null && cmbDownLclick.SelectedItem != null ? cmbDownLclick.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None", rtcDownLC != null ? rtcDownLC.Checked.ToString() : "False", "False", switchDownLC != null && switchDownLC.SelectedItem != null ? switchDownLC.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None" };
                }
                else if (i == 13)
                {
                    if (togDownLeftLC.Checked) { state = "True"; } else if (onceDownLeftLC.Checked) { state = "Once"; } else if (holdDownLeftLC.Checked) { state = "Hold"; } else { state = "Hold"; }

                    if ((nintendoSwitch != null && nintendoSwitch.Checked && (switchDownLeftLC == null || switchDownLeftLC.SelectedItem == null || switchDownLeftLC.SelectedItem.ToString() == "Click to Set" || switchDownLeftLC.SelectedItem.ToString() == "Backtick")) || (chkXbox != null && chkXbox.Checked && (cmbDownLeftLclick == null || cmbDownLeftLclick.SelectedItem == null || cmbDownLeftLclick.SelectedItem.ToString() == "Click to Set" || cmbDownLeftLclick.SelectedItem.ToString() == "Backtick")) || (chkKeyboard != null && chkKeyboard.Checked && (btnDownLeftLclick == null || string.IsNullOrEmpty(btnDownLeftLclick.Text) || btnDownLeftLclick.Text == "Click to Set" || btnDownLeftLclick.Text == "Backtick"))) { }

                    string xboxAction = cmbDownLeftLclick.SelectedItem != null ? cmbDownLeftLclick.SelectedItem.ToString() : "None"; xboxAction = string.IsNullOrEmpty(xboxAction) || xboxAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : xboxAction;
                    string switchAction = switchDownLeftLC.SelectedItem != null ? switchDownLeftLC.SelectedItem.ToString() : "None"; switchAction = string.IsNullOrEmpty(switchAction) || switchAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : switchAction;

                    if (xboxAction != "None" && switchAction == "None") { switchAction = switchButtonOptions[Array.IndexOf(buttonOptions, xboxAction)]; switchDownLeftLC.SelectedItem = switchAction; }
                    else if (switchAction != "None" && xboxAction == "None") { xboxAction = buttonOptions[Array.IndexOf(switchButtonOptions, switchAction)]; cmbDownLeftLclick.SelectedItem = xboxAction; }
                    buttonConfig[i] = new string[] { directions != null && i < directions.Length && directions[i] != null ? directions[i] : "Backtick", state ?? "False", disDownLeftLC != null ? disDownLeftLC.Checked.ToString() : "False", cmbDownLeftLclick != null && cmbDownLeftLclick.SelectedItem != null ? cmbDownLeftLclick.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None", rtcDownLeftLC != null ? rtcDownLeftLC.Checked.ToString() : "False", "False", switchDownLeftLC != null && switchDownLeftLC.SelectedItem != null ? switchDownLeftLC.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None" };
                }
                else if (i == 14)
                {
                    if (togLeftLC.Checked) { state = "True"; } else if (onceLeftLC.Checked) { state = "Once"; } else if (holdLeftLC.Checked) { state = "Hold"; } else { state = "Hold"; }

                    if ((nintendoSwitch != null && nintendoSwitch.Checked && (switchLeftLC == null || switchLeftLC.SelectedItem == null || switchLeftLC.SelectedItem.ToString() == "Click to Set" || switchLeftLC.SelectedItem.ToString() == "Backtick")) || (chkXbox != null && chkXbox.Checked && (cmbLeftLclick == null || cmbLeftLclick.SelectedItem == null || cmbLeftLclick.SelectedItem.ToString() == "Click to Set" || cmbLeftLclick.SelectedItem.ToString() == "Backtick")) || (chkKeyboard != null && chkKeyboard.Checked && (btnLeftLclick == null || string.IsNullOrEmpty(btnLeftLclick.Text) || btnLeftLclick.Text == "Click to Set" || btnLeftLclick.Text == "Backtick"))) { }

                    string xboxAction = cmbLeftLclick.SelectedItem != null ? cmbLeftLclick.SelectedItem.ToString() : "None"; xboxAction = string.IsNullOrEmpty(xboxAction) || xboxAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : xboxAction;
                    string switchAction = switchLeftLC.SelectedItem != null ? switchLeftLC.SelectedItem.ToString() : "None"; switchAction = string.IsNullOrEmpty(switchAction) || switchAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : switchAction;

                    if (xboxAction != "None" && switchAction == "None") { switchAction = switchButtonOptions[Array.IndexOf(buttonOptions, xboxAction)]; switchLeftLC.SelectedItem = switchAction; }
                    else if (switchAction != "None" && xboxAction == "None") { xboxAction = buttonOptions[Array.IndexOf(switchButtonOptions, switchAction)]; cmbLeftLclick.SelectedItem = xboxAction; }
                    buttonConfig[i] = new string[] { directions != null && i < directions.Length && directions[i] != null ? directions[i] : "Backtick", state ?? "False", disLeftLC != null ? disLeftLC.Checked.ToString() : "False", cmbLeftLclick != null && cmbLeftLclick.SelectedItem != null ? cmbLeftLclick.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None", rtcLeftLC != null ? rtcLeftLC.Checked.ToString() : "False", "False", switchLeftLC != null && switchLeftLC.SelectedItem != null ? switchLeftLC.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None" };
                }
                else if (i == 15)
                {
                    if (togUpLeftLC.Checked) { state = "True"; } else if (onceUpLeftLC.Checked) { state = "Once"; } else if (holdUpLeftLC.Checked) { state = "Hold"; } else { state = "Hold"; }

                    if ((nintendoSwitch != null && nintendoSwitch.Checked && (switchUpLeftLC == null || switchUpLeftLC.SelectedItem == null || switchUpLeftLC.SelectedItem.ToString() == "Click to Set" || switchUpLeftLC.SelectedItem.ToString() == "Backtick")) || (chkXbox != null && chkXbox.Checked && (cmbUpLeftLclick == null || cmbUpLeftLclick.SelectedItem == null || cmbUpLeftLclick.SelectedItem.ToString() == "Click to Set" || cmbUpLeftLclick.SelectedItem.ToString() == "Backtick")) || (chkKeyboard != null && chkKeyboard.Checked && (btnUpLeftLclick == null || string.IsNullOrEmpty(btnUpLeftLclick.Text) || btnUpLeftLclick.Text == "Click to Set" || btnUpLeftLclick.Text == "Backtick"))) { }

                    string xboxAction = cmbUpLeftLclick.SelectedItem != null ? cmbUpLeftLclick.SelectedItem.ToString() : "None"; xboxAction = string.IsNullOrEmpty(xboxAction) || xboxAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : xboxAction;
                    string switchAction = switchUpLeftLC.SelectedItem != null ? switchUpLeftLC.SelectedItem.ToString() : "None"; switchAction = string.IsNullOrEmpty(switchAction) || switchAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : switchAction;

                    if (xboxAction != "None" && switchAction == "None") { switchAction = switchButtonOptions[Array.IndexOf(buttonOptions, xboxAction)]; switchUpLeftLC.SelectedItem = switchAction; }
                    else if (switchAction != "None" && xboxAction == "None") { xboxAction = buttonOptions[Array.IndexOf(switchButtonOptions, switchAction)]; cmbUpLeftLclick.SelectedItem = xboxAction; }
                    buttonConfig[i] = new string[] { directions != null && i < directions.Length && directions[i] != null ? directions[i] : "Backtick", state ?? "False", disUpLeftLC != null ? disUpLeftLC.Checked.ToString() : "False", cmbUpLeftLclick != null && cmbUpLeftLclick.SelectedItem != null ? cmbUpLeftLclick.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None", rtcUpLeftLC != null ? rtcUpLeftLC.Checked.ToString() : "False", "False", switchUpLeftLC != null && switchUpLeftLC.SelectedItem != null ? switchUpLeftLC.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None" };
                }
                else if (i == 16)
                {
                    if (togUpRC.Checked) { state = "True"; } else if (onceUpRC.Checked) { state = "Once"; } else if (holdUpRC.Checked) { state = "Hold"; } else { state = "Hold"; }

                    if ((nintendoSwitch != null && nintendoSwitch.Checked && (switchUpRC == null || switchUpRC.SelectedItem == null || switchUpRC.SelectedItem.ToString() == "Click to Set" || switchUpRC.SelectedItem.ToString() == "Backtick")) || (chkXbox != null && chkXbox.Checked && (cmbUpRclick == null || cmbUpRclick.SelectedItem == null || cmbUpRclick.SelectedItem.ToString() == "Click to Set" || cmbUpRclick.SelectedItem.ToString() == "Backtick")) || (chkKeyboard != null && chkKeyboard.Checked && (btnUpRclick == null || string.IsNullOrEmpty(btnUpRclick.Text) || btnUpRclick.Text == "Click to Set" || btnUpRclick.Text == "Backtick"))) { }

                    string xboxAction = cmbUpRclick.SelectedItem != null ? cmbUpRclick.SelectedItem.ToString() : "None"; xboxAction = string.IsNullOrEmpty(xboxAction) || xboxAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : xboxAction;
                    string switchAction = switchUpRC.SelectedItem != null ? switchUpRC.SelectedItem.ToString() : "None"; switchAction = string.IsNullOrEmpty(switchAction) || switchAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : switchAction;

                    if (xboxAction != "None" && switchAction == "None") { switchAction = switchButtonOptions[Array.IndexOf(buttonOptions, xboxAction)]; switchUpRC.SelectedItem = switchAction; }
                    else if (switchAction != "None" && xboxAction == "None") { xboxAction = buttonOptions[Array.IndexOf(switchButtonOptions, switchAction)]; cmbUpRclick.SelectedItem = xboxAction; }
                    buttonConfig[i] = new string[] { directions != null && i < directions.Length && directions[i] != null ? directions[i] : "Backtick", state ?? "False", disUpRC != null ? disUpRC.Checked.ToString() : "False", cmbUpRclick != null && cmbUpRclick.SelectedItem != null ? cmbUpRclick.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None", rtcUpRC != null ? rtcUpRC.Checked.ToString() : "False", "False", switchUpRC != null && switchUpRC.SelectedItem != null ? switchUpRC.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None" };
                }
                else if (i == 17)
                {
                    if (togUpRightRC.Checked) { state = "True"; } else if (onceUpRightRC.Checked) { state = "Once"; } else if (holdUpRightRC.Checked) { state = "Hold"; } else { state = "Hold"; }

                    if ((nintendoSwitch != null && nintendoSwitch.Checked && (switchUpRightRC == null || switchUpRightRC.SelectedItem == null || switchUpRightRC.SelectedItem.ToString() == "Click to Set" || switchUpRightRC.SelectedItem.ToString() == "Backtick")) || (chkXbox != null && chkXbox.Checked && (cmbUpRightRclick == null || cmbUpRightRclick.SelectedItem == null || cmbUpRightRclick.SelectedItem.ToString() == "Click to Set" || cmbUpRightRclick.SelectedItem.ToString() == "Backtick")) || (chkKeyboard != null && chkKeyboard.Checked && (btnUpRightRclick == null || string.IsNullOrEmpty(btnUpRightRclick.Text) || btnUpRightRclick.Text == "Click to Set" || btnUpRightRclick.Text == "Backtick"))) { }

                    string xboxAction = cmbUpRightRclick.SelectedItem != null ? cmbUpRightRclick.SelectedItem.ToString() : "None"; xboxAction = string.IsNullOrEmpty(xboxAction) || xboxAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : xboxAction;
                    string switchAction = switchUpRightRC.SelectedItem != null ? switchUpRightRC.SelectedItem.ToString() : "None"; switchAction = string.IsNullOrEmpty(switchAction) || switchAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : switchAction;

                    if (xboxAction != "None" && switchAction == "None") { switchAction = switchButtonOptions[Array.IndexOf(buttonOptions, xboxAction)]; switchUpRightRC.SelectedItem = switchAction; }
                    else if (switchAction != "None" && xboxAction == "None") { xboxAction = buttonOptions[Array.IndexOf(switchButtonOptions, switchAction)]; cmbUpRightRclick.SelectedItem = xboxAction; }
                    buttonConfig[i] = new string[] { directions != null && i < directions.Length && directions[i] != null ? directions[i] : "Backtick", state ?? "False", disUpRightRC != null ? disUpRightRC.Checked.ToString() : "False", cmbUpRightRclick != null && cmbUpRightRclick.SelectedItem != null ? cmbUpRightRclick.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None", rtcUpRightRC != null ? rtcUpRightRC.Checked.ToString() : "False", "False", switchUpRightRC != null && switchUpRightRC.SelectedItem != null ? switchUpRightRC.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None" };
                }
                else if (i == 18)
                {
                    if (togRightRC.Checked) { state = "True"; } else if (onceRightRC.Checked) { state = "Once"; } else if (holdRightRC.Checked) { state = "Hold"; } else { state = "Hold"; }

                    if ((nintendoSwitch != null && nintendoSwitch.Checked && (switchRightRC == null || switchRightRC.SelectedItem == null || switchRightRC.SelectedItem.ToString() == "Click to Set" || switchRightRC.SelectedItem.ToString() == "Backtick")) || (chkXbox != null && chkXbox.Checked && (cmbRightRclick == null || cmbRightRclick.SelectedItem == null || cmbRightRclick.SelectedItem.ToString() == "Click to Set" || cmbRightRclick.SelectedItem.ToString() == "Backtick")) || (chkKeyboard != null && chkKeyboard.Checked && (btnRightRclick == null || string.IsNullOrEmpty(btnRightRclick.Text) || btnRightRclick.Text == "Click to Set" || btnRightRclick.Text == "Backtick"))) { }

                    string xboxAction = cmbRightRclick.SelectedItem != null ? cmbRightRclick.SelectedItem.ToString() : "None"; xboxAction = string.IsNullOrEmpty(xboxAction) || xboxAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : xboxAction;
                    string switchAction = switchRightRC.SelectedItem != null ? switchRightRC.SelectedItem.ToString() : "None"; switchAction = string.IsNullOrEmpty(switchAction) || switchAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : switchAction;

                    if (xboxAction != "None" && switchAction == "None") { switchAction = switchButtonOptions[Array.IndexOf(buttonOptions, xboxAction)]; switchRightRC.SelectedItem = switchAction; }
                    else if (switchAction != "None" && xboxAction == "None") { xboxAction = buttonOptions[Array.IndexOf(switchButtonOptions, switchAction)]; cmbRightRclick.SelectedItem = xboxAction; }
                    buttonConfig[i] = new string[] { directions != null && i < directions.Length && directions[i] != null ? directions[i] : "Backtick", state ?? "False", disRightRC != null ? disRightRC.Checked.ToString() : "False", cmbRightRclick != null && cmbRightRclick.SelectedItem != null ? cmbRightRclick.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None", rtcRightRC != null ? rtcRightRC.Checked.ToString() : "False", "False", switchRightRC != null && switchRightRC.SelectedItem != null ? switchRightRC.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None" };
                }
                else if (i == 19)
                {
                    if (togDownRightRC.Checked) { state = "True"; } else if (onceDownRightRC.Checked) { state = "Once"; } else if (holdDownRightRC.Checked) { state = "Hold"; } else { state = "Hold"; }

                    if ((nintendoSwitch != null && nintendoSwitch.Checked && (switchDownRightRC == null || switchDownRightRC.SelectedItem == null || switchDownRightRC.SelectedItem.ToString() == "Click to Set" || switchDownRightRC.SelectedItem.ToString() == "Backtick")) || (chkXbox != null && chkXbox.Checked && (cmbDownRightRclick == null || cmbDownRightRclick.SelectedItem == null || cmbDownRightRclick.SelectedItem.ToString() == "Click to Set" || cmbDownRightRclick.SelectedItem.ToString() == "Backtick")) || (chkKeyboard != null && chkKeyboard.Checked && (btnDownRightRclick == null || string.IsNullOrEmpty(btnDownRightRclick.Text) || btnDownRightRclick.Text == "Click to Set" || btnDownRightRclick.Text == "Backtick"))) { }

                    string xboxAction = cmbDownRightRclick.SelectedItem != null ? cmbDownRightRclick.SelectedItem.ToString() : "None"; xboxAction = string.IsNullOrEmpty(xboxAction) || xboxAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : xboxAction;
                    string switchAction = switchDownRightRC.SelectedItem != null ? switchDownRightRC.SelectedItem.ToString() : "None"; switchAction = string.IsNullOrEmpty(switchAction) || switchAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : switchAction;

                    if (xboxAction != "None" && switchAction == "None") { switchAction = switchButtonOptions[Array.IndexOf(buttonOptions, xboxAction)]; switchDownRightRC.SelectedItem = switchAction; }
                    else if (switchAction != "None" && xboxAction == "None") { xboxAction = buttonOptions[Array.IndexOf(switchButtonOptions, switchAction)]; cmbDownRightRclick.SelectedItem = xboxAction; }
                    buttonConfig[i] = new string[] { directions != null && i < directions.Length && directions[i] != null ? directions[i] : "Backtick", state ?? "False", disDownRightRC != null ? disDownRightRC.Checked.ToString() : "False", cmbDownRightRclick != null && cmbDownRightRclick.SelectedItem != null ? cmbDownRightRclick.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None", rtcDownRightRC != null ? rtcDownRightRC.Checked.ToString() : "False", "False", switchDownRightRC != null && switchDownRightRC.SelectedItem != null ? switchDownRightRC.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None" };
                }
                else if (i == 20)
                {
                    if (togDownRC.Checked) { state = "True"; } else if (onceDownRC.Checked) { state = "Once"; } else if (holdDownRC.Checked) { state = "Hold"; } else { state = "Hold"; }

                    if ((nintendoSwitch != null && nintendoSwitch.Checked && (switchDownRC == null || switchDownRC.SelectedItem == null || switchDownRC.SelectedItem.ToString() == "Click to Set" || switchDownRC.SelectedItem.ToString() == "Backtick")) || (chkXbox != null && chkXbox.Checked && (cmbDownRclick == null || cmbDownRclick.SelectedItem == null || cmbDownRclick.SelectedItem.ToString() == "Click to Set" || cmbDownRclick.SelectedItem.ToString() == "Backtick")) || (chkKeyboard != null && chkKeyboard.Checked && (btnDownRclick == null || string.IsNullOrEmpty(btnDownRclick.Text) || btnDownRclick.Text == "Click to Set" || btnDownRclick.Text == "Backtick"))) { }


                    string xboxAction = cmbDownRclick.SelectedItem != null ? cmbDownRclick.SelectedItem.ToString() : "None"; xboxAction = string.IsNullOrEmpty(xboxAction) || xboxAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : xboxAction;
                    string switchAction = switchDownRC.SelectedItem != null ? switchDownRC.SelectedItem.ToString() : "None"; switchAction = string.IsNullOrEmpty(switchAction) || switchAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : switchAction;

                    if (xboxAction != "None" && switchAction == "None") { switchAction = switchButtonOptions[Array.IndexOf(buttonOptions, xboxAction)]; switchDownRC.SelectedItem = switchAction; }
                    else if (switchAction != "None" && xboxAction == "None") { xboxAction = buttonOptions[Array.IndexOf(switchButtonOptions, switchAction)]; cmbDownRclick.SelectedItem = xboxAction; }
                    buttonConfig[i] = new string[] { directions != null && i < directions.Length && directions[i] != null ? directions[i] : "Backtick", state ?? "False", disDownRC != null ? disDownRC.Checked.ToString() : "False", cmbDownRclick != null && cmbDownRclick.SelectedItem != null ? cmbDownRclick.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None", rtcDownRC != null ? rtcDownRC.Checked.ToString() : "False", "False", switchDownRC != null && switchDownRC.SelectedItem != null ? switchDownRC.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None" };
                }
                else if (i == 21)
                {
                    if (togDownLeftRC.Checked) { state = "True"; } else if (onceDownLeftRC.Checked) { state = "Once"; } else if (holdDownLeftRC.Checked) { state = "Hold"; } else { state = "Hold"; }

                    if ((nintendoSwitch != null && nintendoSwitch.Checked && (switchDownLeftRC == null || switchDownLeftRC.SelectedItem == null || switchDownLeftRC.SelectedItem.ToString() == "Click to Set" || switchDownLeftRC.SelectedItem.ToString() == "Backtick")) || (chkXbox != null && chkXbox.Checked && (cmbDownLeftRclick == null || cmbDownLeftRclick.SelectedItem == null || cmbDownLeftRclick.SelectedItem.ToString() == "Click to Set" || cmbDownLeftRclick.SelectedItem.ToString() == "Backtick")) || (chkKeyboard != null && chkKeyboard.Checked && (btnDownLeftRclick == null || string.IsNullOrEmpty(btnDownLeftRclick.Text) || btnDownLeftRclick.Text == "Click to Set" || btnDownLeftRclick.Text == "Backtick"))) { }

                    string xboxAction = cmbDownLeftRclick.SelectedItem != null ? cmbDownLeftRclick.SelectedItem.ToString() : "None"; xboxAction = string.IsNullOrEmpty(xboxAction) || xboxAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : xboxAction;
                    string switchAction = switchDownLeftRC.SelectedItem != null ? switchDownLeftRC.SelectedItem.ToString() : "None"; switchAction = string.IsNullOrEmpty(switchAction) || switchAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : switchAction;

                    if (xboxAction != "None" && switchAction == "None") { switchAction = switchButtonOptions[Array.IndexOf(buttonOptions, xboxAction)]; switchDownLeftRC.SelectedItem = switchAction; }
                    else if (switchAction != "None" && xboxAction == "None") { xboxAction = buttonOptions[Array.IndexOf(switchButtonOptions, switchAction)]; cmbDownLeftRclick.SelectedItem = xboxAction; }
                    buttonConfig[i] = new string[] { directions != null && i < directions.Length && directions[i] != null ? directions[i] : "Backtick", state ?? "False", disDownLeftRC != null ? disDownLeftRC.Checked.ToString() : "False", cmbDownLeftRclick != null && cmbDownLeftRclick.SelectedItem != null ? cmbDownLeftRclick.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None", rtcDownLeftRC != null ? rtcDownLeftRC.Checked.ToString() : "False", "False", switchDownLeftRC != null && switchDownLeftRC.SelectedItem != null ? switchDownLeftRC.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None" };
                }
                else if (i == 22)
                {
                    if (togLeftRC.Checked) { state = "True"; } else if (onceLeftRC.Checked) { state = "Once"; } else if (holdLeftRC.Checked) { state = "Hold"; } else { state = "Hold"; }

                    if ((nintendoSwitch != null && nintendoSwitch.Checked && (switchLeftRC == null || switchLeftRC.SelectedItem == null || switchLeftRC.SelectedItem.ToString() == "Click to Set" || switchLeftRC.SelectedItem.ToString() == "Backtick")) || (chkXbox != null && chkXbox.Checked && (cmbLeftRclick == null || cmbLeftRclick.SelectedItem == null || cmbLeftRclick.SelectedItem.ToString() == "Click to Set" || cmbLeftRclick.SelectedItem.ToString() == "Backtick")) || (chkKeyboard != null && chkKeyboard.Checked && (btnLeftRclick == null || string.IsNullOrEmpty(btnLeftRclick.Text) || btnLeftRclick.Text == "Click to Set" || btnLeftRclick.Text == "Backtick"))) { }

                    string xboxAction = cmbLeftRclick.SelectedItem != null ? cmbLeftRclick.SelectedItem.ToString() : "None"; xboxAction = string.IsNullOrEmpty(xboxAction) || xboxAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : xboxAction;
                    string switchAction = switchLeftRC.SelectedItem != null ? switchLeftRC.SelectedItem.ToString() : "None"; switchAction = string.IsNullOrEmpty(switchAction) || switchAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : switchAction;

                    if (xboxAction != "None" && switchAction == "None") { switchAction = switchButtonOptions[Array.IndexOf(buttonOptions, xboxAction)]; switchLeftRC.SelectedItem = switchAction; }
                    else if (switchAction != "None" && xboxAction == "None") { xboxAction = buttonOptions[Array.IndexOf(switchButtonOptions, switchAction)]; cmbLeftRclick.SelectedItem = xboxAction; }
                    buttonConfig[i] = new string[] { directions != null && i < directions.Length && directions[i] != null ? directions[i] : "Backtick", state ?? "False", disLeftRC != null ? disLeftRC.Checked.ToString() : "False", cmbLeftRclick != null && cmbLeftRclick.SelectedItem != null ? cmbLeftRclick.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None", rtcLeftRC != null ? rtcLeftRC.Checked.ToString() : "False", "False", switchLeftRC != null && switchLeftRC.SelectedItem != null ? switchLeftRC.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None" };
                }
                else if (i == 23)
                {
                    if (togUpLeftRC.Checked) { state = "True"; } else if (onceUpLeftRC.Checked) { state = "Once"; } else if (holdUpLeftRC.Checked) { state = "Hold"; } else { state = "Hold"; }

                    if ((nintendoSwitch != null && nintendoSwitch.Checked && (switchUpLeftRC == null || switchUpLeftRC.SelectedItem == null || switchUpLeftRC.SelectedItem.ToString() == "Click to Set" || switchUpLeftRC.SelectedItem.ToString() == "Backtick")) || (chkXbox != null && chkXbox.Checked && (cmbUpLeftRclick == null || cmbUpLeftRclick.SelectedItem == null || cmbUpLeftRclick.SelectedItem.ToString() == "Click to Set" || cmbUpLeftRclick.SelectedItem.ToString() == "Backtick")) || (chkKeyboard != null && chkKeyboard.Checked && (btnUpLeftRclick == null || string.IsNullOrEmpty(btnUpLeftRclick.Text) || btnUpLeftRclick.Text == "Click to Set" || btnUpLeftRclick.Text == "Backtick"))) { }

                    string xboxAction = cmbUpLeftRclick.SelectedItem != null ? cmbUpLeftRclick.SelectedItem.ToString() : "None"; xboxAction = string.IsNullOrEmpty(xboxAction) || xboxAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : xboxAction;
                    string switchAction = switchUpLeftRC.SelectedItem != null ? switchUpLeftRC.SelectedItem.ToString() : "None"; switchAction = string.IsNullOrEmpty(switchAction) || switchAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : switchAction;

                    if (xboxAction != "None" && switchAction == "None") { switchAction = switchButtonOptions[Array.IndexOf(buttonOptions, xboxAction)]; switchUpLeftRC.SelectedItem = switchAction; }
                    else if (switchAction != "None" && xboxAction == "None") { xboxAction = buttonOptions[Array.IndexOf(switchButtonOptions, switchAction)]; cmbUpLeftRclick.SelectedItem = xboxAction; }
                    buttonConfig[i] = new string[] { directions != null && i < directions.Length && directions[i] != null ? directions[i] : "Backtick", state ?? "False", disUpLeftRC != null ? disUpLeftRC.Checked.ToString() : "False", cmbUpLeftRclick != null && cmbUpLeftRclick.SelectedItem != null ? cmbUpLeftRclick.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None", rtcUpLeftRC != null ? rtcUpLeftRC.Checked.ToString() : "False", "False", switchUpLeftRC != null && switchUpLeftRC.SelectedItem != null ? switchUpLeftRC.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None" };
                }
                else if (i == 24)
                {
                    if (togDZUpperHover.Checked) { state = "True"; } else if (onceDZUpper.Checked) { state = "Once"; } else { state = "False"; }

                    if ((nintendoSwitch != null && nintendoSwitch.Checked && (switchDZUpperHover == null || switchDZUpperHover.SelectedItem == null || switchDZUpperHover.SelectedItem.ToString() == "Click to Set" || switchDZUpperHover.SelectedItem.ToString() == "Backtick")) || (chkXbox != null && chkXbox.Checked && (cmbDZUpperHover == null || cmbDZUpperHover.SelectedItem == null || cmbDZUpperHover.SelectedItem.ToString() == "Click to Set" || cmbDZUpperHover.SelectedItem.ToString() == "Backtick")) || (chkKeyboard != null && chkKeyboard.Checked && (btnDZUpperHover == null || string.IsNullOrEmpty(btnDZUpperHover.Text) || btnDZUpperHover.Text == "Click to Set" || btnDZUpperHover.Text == "Backtick"))) { }

                    string xboxAction = cmbDZUpperHover.SelectedItem != null ? cmbDZUpperHover.SelectedItem.ToString() : "None"; xboxAction = string.IsNullOrEmpty(xboxAction) || xboxAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : xboxAction;
                    string switchAction = switchDZUpperHover.SelectedItem != null ? switchDZUpperHover.SelectedItem.ToString() : "None"; switchAction = string.IsNullOrEmpty(switchAction) || switchAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : switchAction;

                    if (xboxAction != "None" && switchAction == "None") { switchAction = switchButtonOptions[Array.IndexOf(buttonOptions, xboxAction)]; switchDZUpperHover.SelectedItem = switchAction; }
                    else if (switchAction != "None" && xboxAction == "None") { xboxAction = buttonOptions[Array.IndexOf(switchButtonOptions, switchAction)]; cmbDZUpperHover.SelectedItem = xboxAction; }



                    buttonConfig[i] = new string[] { directions != null && i < directions.Length && directions[i] != null ? directions[i] : "Backtick", state ?? "False", disDZUpperHover != null ? disDZUpperHover.Checked.ToString() : "False", cmbDZUpperHover != null && cmbDZUpperHover.SelectedItem != null ? cmbDZUpperHover.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None", "False", "False", switchDZUpperHover != null && switchDZUpperHover.SelectedItem != null ? switchDZUpperHover.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None" };
                }
                else if (i == 25)

                {
                    if (togDZUpperLC.Checked) { state = "True"; } else if (onceDZUpperLC.Checked) { state = "Once"; } else if (holdDZUpperLC.Checked) { state = "Hold"; } else { state = "Hold"; }

                    if ((nintendoSwitch != null && nintendoSwitch.Checked && (switchDZUpperLC == null || switchDZUpperLC.SelectedItem == null || switchDZUpperLC.SelectedItem.ToString() == "Click to Set" || switchDZUpperLC.SelectedItem.ToString() == "Backtick")) || (chkXbox != null && chkXbox.Checked && (cmbDZUpperLC == null || cmbDZUpperLC.SelectedItem == null || cmbDZUpperLC.SelectedItem.ToString() == "Click to Set" || cmbDZUpperLC.SelectedItem.ToString() == "Backtick")) || (chkKeyboard != null && chkKeyboard.Checked && (btnDZUpperLC == null || string.IsNullOrEmpty(btnDZUpperLC.Text) || btnDZUpperLC.Text == "Click to Set" || btnDZUpperLC.Text == "Backtick"))) { }

                    string xboxAction = cmbDZUpperLC.SelectedItem != null ? cmbDZUpperLC.SelectedItem.ToString() : "None"; xboxAction = string.IsNullOrEmpty(xboxAction) || xboxAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : xboxAction;
                    string switchAction = switchDZUpperLC.SelectedItem != null ? switchDZUpperLC.SelectedItem.ToString() : "None"; switchAction = string.IsNullOrEmpty(switchAction) || switchAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : switchAction;

                    if (xboxAction != "None" && switchAction == "None") { switchAction = switchButtonOptions[Array.IndexOf(buttonOptions, xboxAction)]; switchDZUpperLC.SelectedItem = switchAction; }
                    else if (switchAction != "None" && xboxAction == "None") { xboxAction = buttonOptions[Array.IndexOf(switchButtonOptions, switchAction)]; cmbDZUpperLC.SelectedItem = xboxAction; }


                    buttonConfig[i] = new string[] { directions != null && i < directions.Length && directions[i] != null ? directions[i] : "Backtick", state ?? "False", disDZUpperLC != null ? disDZUpperLC.Checked.ToString() : "False", cmbDZUpperLC != null && cmbDZUpperLC.SelectedItem != null ? cmbDZUpperLC.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None", "False", "False", switchDZUpperLC != null && switchDZUpperLC.SelectedItem != null ? switchDZUpperLC.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None" };
                }
                else if (i == 26)
                {
                    if (togDZUpperRC.Checked) { state = "True"; } else if (onceDZUpperRC.Checked) { state = "Once"; } else if (holdDZUpperRC.Checked) { state = "Hold"; } else { state = "Hold"; }

                    if ((nintendoSwitch != null && nintendoSwitch.Checked && (switchDZUpperRC == null || switchDZUpperRC.SelectedItem == null || switchDZUpperRC.SelectedItem.ToString() == "Click to Set" || switchDZUpperRC.SelectedItem.ToString() == "Backtick")) || (chkXbox != null && chkXbox.Checked && (cmbDZUpperRC == null || cmbDZUpperRC.SelectedItem == null || cmbDZUpperRC.SelectedItem.ToString() == "Click to Set" || cmbDZUpperRC.SelectedItem.ToString() == "Backtick")) || (chkKeyboard != null && chkKeyboard.Checked && (btnDZUpperRC == null || string.IsNullOrEmpty(btnDZUpperRC.Text) || btnDZUpperRC.Text == "Click to Set" || btnDZUpperRC.Text == "Backtick"))) { }

                    string xboxAction = cmbDZUpperRC.SelectedItem != null ? cmbDZUpperRC.SelectedItem.ToString() : "None"; xboxAction = string.IsNullOrEmpty(xboxAction) || xboxAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : xboxAction;
                    string switchAction = switchDZUpperRC.SelectedItem != null ? switchDZUpperRC.SelectedItem.ToString() : "None"; switchAction = string.IsNullOrEmpty(switchAction) || switchAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : switchAction;

                    if (xboxAction != "None" && switchAction == "None") { switchAction = switchButtonOptions[Array.IndexOf(buttonOptions, xboxAction)]; switchDZUpperRC.SelectedItem = switchAction; }
                    else if (switchAction != "None" && xboxAction == "None") { xboxAction = buttonOptions[Array.IndexOf(switchButtonOptions, switchAction)]; cmbDZUpperRC.SelectedItem = xboxAction; }
                    buttonConfig[i] = new string[] { directions != null && i < directions.Length && directions[i] != null ? directions[i] : "Backtick", state ?? "False", disDZUpperRC != null ? disDZUpperRC.Checked.ToString() : "False", cmbDZUpperRC != null && cmbDZUpperRC.SelectedItem != null ? cmbDZUpperRC.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None", "False", "False", switchDZUpperRC != null && switchDZUpperRC.SelectedItem != null ? switchDZUpperRC.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None" };
                }
                else if (i == 27)
                {
                    if (togDZMiddleHover.Checked) { state = "True"; } else if (onceDZMiddle.Checked) { state = "Once"; } else { state = "False"; }

                    if ((nintendoSwitch != null && nintendoSwitch.Checked && (switchDZMiddleHover == null || switchDZMiddleHover.SelectedItem == null || switchDZMiddleHover.SelectedItem.ToString() == "Click to Set" || switchDZMiddleHover.SelectedItem.ToString() == "Backtick")) || (chkXbox != null && chkXbox.Checked && (cmbDZMiddleHover == null || cmbDZMiddleHover.SelectedItem == null || cmbDZMiddleHover.SelectedItem.ToString() == "Click to Set" || cmbDZMiddleHover.SelectedItem.ToString() == "Backtick")) || (chkKeyboard != null && chkKeyboard.Checked && (btnDZMiddleHover == null || string.IsNullOrEmpty(btnDZMiddleHover.Text) || btnDZMiddleHover.Text == "Click to Set" || btnDZMiddleHover.Text == "Backtick"))) { }

                    string xboxAction = cmbDZMiddleHover.SelectedItem != null ? cmbDZMiddleHover.SelectedItem.ToString() : "None"; xboxAction = string.IsNullOrEmpty(xboxAction) || xboxAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : xboxAction;
                    string switchAction = switchDZMiddleHover.SelectedItem != null ? switchDZMiddleHover.SelectedItem.ToString() : "None"; switchAction = string.IsNullOrEmpty(switchAction) || switchAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : switchAction;

                    if (xboxAction != "None" && switchAction == "None") { switchAction = switchButtonOptions[Array.IndexOf(buttonOptions, xboxAction)]; switchDZMiddleHover.SelectedItem = switchAction; }
                    else if (switchAction != "None" && xboxAction == "None") { xboxAction = buttonOptions[Array.IndexOf(switchButtonOptions, switchAction)]; cmbDZMiddleHover.SelectedItem = xboxAction; }

                    buttonConfig[i] = new string[] { directions != null && i < directions.Length && directions[i] != null ? directions[i] : "Backtick", state ?? "False", disDZMiddleHover != null ? disDZMiddleHover.Checked.ToString() : "False", cmbDZMiddleHover != null && cmbDZMiddleHover.SelectedItem != null ? cmbDZMiddleHover.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None", "False", "False", switchDZMiddleHover != null && switchDZMiddleHover.SelectedItem != null ? switchDZMiddleHover.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None" };
                }
                else if (i == 28)
                {
                    if (togDZMiddleLC.Checked) { state = "True"; } else if (onceDZMiddleLC.Checked) { state = "Once"; } else if (holdDZMiddleLC.Checked) { state = "Hold"; } else { state = "Hold"; }

                    if ((nintendoSwitch != null && nintendoSwitch.Checked && (switchDZMiddleLC == null || switchDZMiddleLC.SelectedItem == null || switchDZMiddleLC.SelectedItem.ToString() == "Click to Set" || switchDZMiddleLC.SelectedItem.ToString() == "Backtick")) || (chkXbox != null && chkXbox.Checked && (cmbDZMiddleLC == null || cmbDZMiddleLC.SelectedItem == null || cmbDZMiddleLC.SelectedItem.ToString() == "Click to Set" || cmbDZMiddleLC.SelectedItem.ToString() == "Backtick")) || (chkKeyboard != null && chkKeyboard.Checked && (btnDZMiddleLC == null || string.IsNullOrEmpty(btnDZMiddleLC.Text) || btnDZMiddleLC.Text == "Click to Set" || btnDZMiddleLC.Text == "Backtick"))) { }

                    string xboxAction = cmbDZMiddleLC.SelectedItem != null ? cmbDZMiddleLC.SelectedItem.ToString() : "None"; xboxAction = string.IsNullOrEmpty(xboxAction) || xboxAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : xboxAction;
                    string switchAction = switchDZMiddleLC.SelectedItem != null ? switchDZMiddleLC.SelectedItem.ToString() : "None"; switchAction = string.IsNullOrEmpty(switchAction) || switchAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : switchAction;

                    if (xboxAction != "None" && switchAction == "None") { switchAction = switchButtonOptions[Array.IndexOf(buttonOptions, xboxAction)]; switchDZMiddleLC.SelectedItem = switchAction; }
                    else if (switchAction != "None" && xboxAction == "None") { xboxAction = buttonOptions[Array.IndexOf(switchButtonOptions, switchAction)]; cmbDZMiddleLC.SelectedItem = xboxAction; }
                    buttonConfig[i] = new string[] { directions != null && i < directions.Length && directions[i] != null ? directions[i] : "Backtick", state ?? "False", disDZMiddleLC != null ? disDZMiddleLC.Checked.ToString() : "False", cmbDZMiddleLC != null && cmbDZMiddleLC.SelectedItem != null ? cmbDZMiddleLC.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None", "False", "False", switchDZMiddleLC != null && switchDZMiddleLC.SelectedItem != null ? switchDZMiddleLC.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None" };
                }
                else if (i == 29)
                {
                    if (togDZMiddleRC.Checked) { state = "True"; } else if (onceDZMiddleRC.Checked) { state = "Once"; } else if (holdDZMiddleRC.Checked) { state = "Hold"; } else { state = "Hold"; }

                    if ((nintendoSwitch != null && nintendoSwitch.Checked && (switchDZMiddleRC == null || switchDZMiddleRC.SelectedItem == null || switchDZMiddleRC.SelectedItem.ToString() == "Click to Set" || switchDZMiddleRC.SelectedItem.ToString() == "Backtick")) || (chkXbox != null && chkXbox.Checked && (cmbDZMiddleRC == null || cmbDZMiddleRC.SelectedItem == null || cmbDZMiddleRC.SelectedItem.ToString() == "Click to Set" || cmbDZMiddleRC.SelectedItem.ToString() == "Backtick")) || (chkKeyboard != null && chkKeyboard.Checked && (btnDZMiddleRC == null || string.IsNullOrEmpty(btnDZMiddleRC.Text) || btnDZMiddleRC.Text == "Click to Set" || btnDZMiddleRC.Text == "Backtick"))) { }

                    string xboxAction = cmbDZMiddleRC.SelectedItem != null ? cmbDZMiddleRC.SelectedItem.ToString() : "None"; xboxAction = string.IsNullOrEmpty(xboxAction) || xboxAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : xboxAction;
                    string switchAction = switchDZMiddleRC.SelectedItem != null ? switchDZMiddleRC.SelectedItem.ToString() : "None"; switchAction = string.IsNullOrEmpty(switchAction) || switchAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : switchAction;

                    if (xboxAction != "None" && switchAction == "None") { switchAction = switchButtonOptions[Array.IndexOf(buttonOptions, xboxAction)]; switchDZMiddleRC.SelectedItem = switchAction; }
                    else if (switchAction != "None" && xboxAction == "None") { xboxAction = buttonOptions[Array.IndexOf(switchButtonOptions, switchAction)]; cmbDZMiddleRC.SelectedItem = xboxAction; }
                    buttonConfig[i] = new string[] { directions != null && i < directions.Length && directions[i] != null ? directions[i] : "Backtick", state ?? "False", disDZMiddleRC != null ? disDZMiddleRC.Checked.ToString() : "False", cmbDZMiddleRC != null && cmbDZMiddleRC.SelectedItem != null ? cmbDZMiddleRC.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None", "False", "False", switchDZMiddleRC != null && switchDZMiddleRC.SelectedItem != null ? switchDZMiddleRC.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None" };
                }
                else if (i == 30)
                {
                    if (togDZLowerHover.Checked) { state = "True"; } else if (onceDZLower.Checked) { state = "Once"; } else { state = "False"; }

                    if ((nintendoSwitch != null && nintendoSwitch.Checked && (switchDZLowerHover == null || switchDZLowerHover.SelectedItem == null || switchDZLowerHover.SelectedItem.ToString() == "Click to Set" || switchDZLowerHover.SelectedItem.ToString() == "Backtick")) || (chkXbox != null && chkXbox.Checked && (cmbDZLowerHover == null || cmbDZLowerHover.SelectedItem == null || cmbDZLowerHover.SelectedItem.ToString() == "Click to Set" || cmbDZLowerHover.SelectedItem.ToString() == "Backtick")) || (chkKeyboard != null && chkKeyboard.Checked && (btnDZLowerHover == null || string.IsNullOrEmpty(btnDZLowerHover.Text) || btnDZLowerHover.Text == "Click to Set" || btnDZLowerHover.Text == "Backtick"))) { }

                    string xboxAction = cmbDZLowerHover.SelectedItem != null ? cmbDZLowerHover.SelectedItem.ToString() : "None"; xboxAction = string.IsNullOrEmpty(xboxAction) || xboxAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : xboxAction;
                    string switchAction = switchDZLowerHover.SelectedItem != null ? switchDZLowerHover.SelectedItem.ToString() : "None"; switchAction = string.IsNullOrEmpty(switchAction) || switchAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : switchAction;

                    if (xboxAction != "None" && switchAction == "None") { switchAction = switchButtonOptions[Array.IndexOf(buttonOptions, xboxAction)]; switchDZLowerHover.SelectedItem = switchAction; }
                    else if (switchAction != "None" && xboxAction == "None") { xboxAction = buttonOptions[Array.IndexOf(switchButtonOptions, switchAction)]; cmbDZLowerHover.SelectedItem = xboxAction; }

                    buttonConfig[i] = new string[] { directions != null && i < directions.Length && directions[i] != null ? directions[i] : "Backtick", state ?? "False", disDZLowerHover != null ? disDZLowerHover.Checked.ToString() : "False", cmbDZLowerHover != null && cmbDZLowerHover.SelectedItem != null ? cmbDZLowerHover.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None", "False", "False", switchDZLowerHover != null && switchDZLowerHover.SelectedItem != null ? switchDZLowerHover.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None" };
                }
                else if (i == 31)
                {
                    if (togDZLowerLC.Checked) { state = "True"; } else if (onceDZLowerLC.Checked) { state = "Once"; } else if (holdDZLowerLC.Checked) { state = "Hold"; } else { state = "Hold"; }

                    if ((nintendoSwitch != null && nintendoSwitch.Checked && (switchDZLowerLC == null || switchDZLowerLC.SelectedItem == null || switchDZLowerLC.SelectedItem.ToString() == "Click to Set" || switchDZLowerLC.SelectedItem.ToString() == "Backtick")) || (chkXbox != null && chkXbox.Checked && (cmbDZLowerLC == null || cmbDZLowerLC.SelectedItem == null || cmbDZLowerLC.SelectedItem.ToString() == "Click to Set" || cmbDZLowerLC.SelectedItem.ToString() == "Backtick")) || (chkKeyboard != null && chkKeyboard.Checked && (btnDZLowerLC == null || string.IsNullOrEmpty(btnDZLowerLC.Text) || btnDZLowerLC.Text == "Click to Set" || btnDZLowerLC.Text == "Backtick"))) { }

                    string xboxAction = cmbDZLowerLC.SelectedItem != null ? cmbDZLowerLC.SelectedItem.ToString() : "None"; xboxAction = string.IsNullOrEmpty(xboxAction) || xboxAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : xboxAction;
                    string switchAction = switchDZLowerLC.SelectedItem != null ? switchDZLowerLC.SelectedItem.ToString() : "None"; switchAction = string.IsNullOrEmpty(switchAction) || switchAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : switchAction;

                    if (xboxAction != "None" && switchAction == "None") { switchAction = switchButtonOptions[Array.IndexOf(buttonOptions, xboxAction)]; switchDZLowerLC.SelectedItem = switchAction; }
                    else if (switchAction != "None" && xboxAction == "None") { xboxAction = buttonOptions[Array.IndexOf(switchButtonOptions, switchAction)]; cmbDZLowerLC.SelectedItem = xboxAction; }
                    buttonConfig[i] = new string[] { directions != null && i < directions.Length && directions[i] != null ? directions[i] : "Backtick", state ?? "False", disDZLowerLC != null ? disDZLowerLC.Checked.ToString() : "False", cmbDZLowerLC != null && cmbDZLowerLC.SelectedItem != null ? cmbDZLowerLC.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None", "False", "False", switchDZLowerLC != null && switchDZLowerLC.SelectedItem != null ? switchDZLowerLC.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None" };
                }
                else if (i == 32)
                {
                    if (togDZLowerRC.Checked) { state = "True"; } else if (onceDZLowerRC.Checked) { state = "Once"; } else if (holdDZLowerRC.Checked) { state = "Hold"; } else { state = "Hold"; }

                    if ((nintendoSwitch != null && nintendoSwitch.Checked && (switchDZLowerRC == null || switchDZLowerRC.SelectedItem == null || switchDZLowerRC.SelectedItem.ToString() == "Click to Set" || switchDZLowerRC.SelectedItem.ToString() == "Backtick")) || (chkXbox != null && chkXbox.Checked && (cmbDZLowerRC == null || cmbDZLowerRC.SelectedItem == null || cmbDZLowerRC.SelectedItem.ToString() == "Click to Set" || cmbDZLowerRC.SelectedItem.ToString() == "Backtick")) || (chkKeyboard != null && chkKeyboard.Checked && (btnDZLowerRC == null || string.IsNullOrEmpty(btnDZLowerRC.Text) || btnDZLowerRC.Text == "Click to Set" || btnDZLowerRC.Text == "Backtick"))) { }

                    string xboxAction = cmbDZLowerRC.SelectedItem != null ? cmbDZLowerRC.SelectedItem.ToString() : "None"; xboxAction = string.IsNullOrEmpty(xboxAction) || xboxAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : xboxAction;
                    string switchAction = switchDZLowerRC.SelectedItem != null ? switchDZLowerRC.SelectedItem.ToString() : "None"; switchAction = string.IsNullOrEmpty(switchAction) || switchAction.Equals("Click to Set", StringComparison.OrdinalIgnoreCase) ? "None" : switchAction;

                    if (xboxAction != "None" && switchAction == "None") { switchAction = switchButtonOptions[Array.IndexOf(buttonOptions, xboxAction)]; switchDZLowerRC.SelectedItem = switchAction; }
                    else if (switchAction != "None" && xboxAction == "None") { xboxAction = buttonOptions[Array.IndexOf(switchButtonOptions, switchAction)]; cmbDZLowerRC.SelectedItem = xboxAction; }
                    buttonConfig[i] = new string[] { directions != null && i < directions.Length && directions[i] != null ? directions[i] : "Backtick", state ?? "False", disDZLowerRC != null ? disDZLowerRC.Checked.ToString() : "False", cmbDZLowerRC != null && cmbDZLowerRC.SelectedItem != null ? cmbDZLowerRC.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None", "False", "False", switchDZLowerRC != null && switchDZLowerRC.SelectedItem != null ? switchDZLowerRC.SelectedItem.ToString().Replace("Click to Set", "None", StringComparison.OrdinalIgnoreCase) : "None" };
                }

                NormalizeDisabledButtonConfigRow(buttonConfig[i]);
                file.WriteLine(string.Join(",", buttonConfig[i]));

            }

            var activationFlags = GetNormalizedCurrentActivationFlags();

            file.WriteLine(activationFlags.upper);
            file.WriteLine(activationFlags.middle);
            file.WriteLine(activationFlags.lower);
            file.WriteLine(chkLabelsHover.Checked);
            file.WriteLine(chkLabelsLC.Checked);
            file.WriteLine(chkLabelsRC.Checked);
            if (cmbProfile.SelectedIndex == 0)
            {
                file.WriteLine("True");
            }
            else { file.WriteLine("False"); }
            if (chkXbox.Checked == true) { file.WriteLine("Xbox"); }
            else if (nintendoSwitch.Checked == true) { file.WriteLine("Switch"); }
            else { file.WriteLine("Keyboard"); }

            file.WriteLine("True");
            file.WriteLine("True");


            file.WriteLine(diagUpRightHover.Checked);
            file.WriteLine(diagUpRightLC.Checked);
            file.WriteLine(diagUpRightRC.Checked);
            file.WriteLine(diagDownRightHover.Checked);
            file.WriteLine(diagDownRightLC.Checked);
            file.WriteLine(diagDownRightRC.Checked);
            file.WriteLine(diagUpLeftHover.Checked);
            file.WriteLine(diagUpLeftLC.Checked);
            file.WriteLine(diagUpLeftRC.Checked);
            file.WriteLine(diagDownLeftHover.Checked);
            file.WriteLine(diagDownLeftLC.Checked);
            file.WriteLine(diagDownLeftRC.Checked);
            file.WriteLine("True");
            file.WriteLine("08-27-2025");
            string advanced = BuildAdvancedSettingsCode();

            file.WriteLine(advanced);
        }


        private static void NormalizeDisabledButtonConfigRow(string[] configRow)
        {
            if (configRow == null || configRow.Length < 7)
            {
                return;
            }

            if (!bool.TryParse(configRow[2], out bool isDisabled) || !isDisabled)
            {
                return;
            }

            configRow[3] = "None";
            configRow[6] = "None";
        }



        private void cmbProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoadingProfile)
            {
                return;
            }

            if (lastProfileIndex >= 0 && lastProfileIndex != cmbProfile.SelectedIndex)
            {
                SaveProfileButtonConfig(lastProfileIndex);
            }

            lastProfileIndex = cmbProfile.SelectedIndex;

            chkXbox.Enabled = cmbProfile.SelectedIndex != 0;
            nintendoSwitch.Enabled = cmbProfile.SelectedIndex != 0;

            LoadSelectedProfileIntoUi();
        }

        private void cmbProfile_MouseClick(object sender, MouseEventArgs e)
        {
            SaveCurrentProfileButtonConfig();
        }

        private string GetConfigNameForProfileIndex(int profileIndex)
        {
            if (profileIndex == 0)
            {
                return "Music.txt";
            }

            if (profileIndex > 0 && profileIndex < cmbProfile.Items.Count)
            {
                return cmbProfile.Items[profileIndex].ToString() + ".txt";
            }

            return null;
        }

        private void SaveCurrentProfileButtonConfig()
        {
            SaveProfileButtonConfig(cmbProfile.SelectedIndex);
        }

        private void SaveProfileButtonConfig(int profileIndex)
        {
            string targetConfigName = GetConfigNameForProfileIndex(profileIndex);
            if (string.IsNullOrEmpty(targetConfigName))
            {
                return;
            }

            configName = targetConfigName;

            try
            {
                using (FileStream fs = new FileStream(Path.Combine(profilesPath, configName), FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                using (StreamWriter file = new StreamWriter(fs))
                {
                    PrepareSettingsForSave();
                    WriteCurrentButtonConfigRows(file);
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


        private void btnSave_Click(object sender, EventArgs e)
        {
            string oldConfig = cmbProfile.Items[cmbProfile.SelectedIndex].ToString(); System.IO.File.Move(Path.Combine(profilesPath, oldConfig + ".txt"), Path.Combine(profilesPath, txtConfigName.Text + ".txt"));
            btnProfileSettings.Visible = true;
            cmbProfile.Items[cmbProfile.SelectedIndex] = txtConfigName.Text; btnSave.Visible = false; btnSave.Enabled = false; txtConfigName.Visible = false; cmbProfile.Visible = true; cmbProfile.Enabled = true;

            txtConfigName.Text = "";


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
        }

        private void btnDefault_Click(object sender, EventArgs e)
        {
            if (cmbProfile.SelectedIndex > 0)
            {
                if (ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Are you sure you want to set the \"" + cmbProfile.SelectedItem.ToString() + "\" profile to the defaults?'>Are you sure you want to set the \"" + cmbProfile.SelectedItem.ToString() + "\" profile to the defaults?</span></p>", "Set Defaults Verification", ExtendedMessageBoxLibrary.MessageBoxButtons.YesNo, lines: 2).Result ==
    Wisej.Web.DialogResult.Yes) { defaultConfig = true; LoadSelectedProfileIntoUi(); }
            }
            else if (cmbProfile.SelectedIndex == 0)
            {
                if (ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Are you sure you want to set the \"" + cmbProfile.SelectedItem.ToString() + "\" profile to the defaults?'>Are you sure you want to set the \"" + cmbProfile.SelectedItem.ToString() + "\" profile to the defaults?</span></p>", "Set Defaults Verification", ExtendedMessageBoxLibrary.MessageBoxButtons.YesNo, lines: 2).Result ==
    Wisej.Web.DialogResult.Yes) { defaultMusic = true; LoadSelectedProfileIntoUi(); }
            }
        }


        private void LoadSelectedProfileIntoUi()
        {
            string activeTab = string.IsNullOrEmpty(currentTab) ? "Hover" : currentTab;
            bool wasKeyboardMode = chkKeyboard.Checked;
            bool wasXboxMode = chkXbox.Checked;
            bool wasSwitchMode = nintendoSwitch.Checked;

            isLoadingProfile = true;
            try
            {
                DefaultValues();
            }
            finally
            {
                isLoadingProfile = false;
            }

            ApplyLoadedInputModeState();

            bool modeChanged = wasKeyboardMode != chkKeyboard.Checked
                || wasXboxMode != chkXbox.Checked
                || wasSwitchMode != nintendoSwitch.Checked;

            if (modeChanged)
            {
                activeTab = "Hover";
            }

            NormalizeLoadedAssignmentControls();
            ApplyLoadedDiagonalCombineVisualState();
            RestoreCurrentTabAfterLoad(activeTab);
            RefreshDisableButtonsByClick();
        }

        private void ApplyLoadedInputModeState()
        {
            if (nintendoSwitch.Checked)
            {
                nintendoSwitch_CheckedChanged(nintendoSwitch, EventArgs.Empty);
            }
            else if (chkXbox.Checked)
            {
                chkXbox_CheckedChanged(chkXbox, EventArgs.Empty);
            }
            else
            {
                checkBox1_CheckedChanged_1(chkKeyboard, EventArgs.Empty);
            }
        }

        private void RestoreCurrentTabAfterLoad(string activeTab)
        {
            if (string.Equals(activeTab, "LC", StringComparison.OrdinalIgnoreCase))
            {
                btnLeftClick.PerformClick();
            }
            else if (string.Equals(activeTab, "RC", StringComparison.OrdinalIgnoreCase))
            {
                btnRightClick.PerformClick();
            }
            else
            {
                btnHover.PerformClick();
            }
        }

        private void ApplyLoadedDiagonalCombineVisualState()
        {
            ApplyLoadedDiagonalCombineControlState(diagUpRightHover, btnUpRightHover, cmbUpRightHover, switchUpRightHover, "Up-Right");
            ApplyLoadedDiagonalCombineControlState(diagUpRightLC, btnUpRightLclick, cmbUpRightLclick, switchUpRightLC, "Up-Right");
            ApplyLoadedDiagonalCombineControlState(diagUpRightRC, btnUpRightRclick, cmbUpRightRclick, switchUpRightRC, "Up-Right");

            ApplyLoadedDiagonalCombineControlState(diagDownRightHover, btnDownRightHover, cmbDownRightHover, switchDownRightHover, "Down-Right");
            ApplyLoadedDiagonalCombineControlState(diagDownRightLC, btnDownRightLclick, cmbDownRightLclick, switchDownRightLC, "Down-Right");
            ApplyLoadedDiagonalCombineControlState(diagDownRightRC, btnDownRightRclick, cmbDownRightRclick, switchDownRightRC, "Down-Right");

            ApplyLoadedDiagonalCombineControlState(diagDownLeftHover, btnDownLeftHover, cmbDownLeftHover, switchDownLeftHover, "Down-Left");
            ApplyLoadedDiagonalCombineControlState(diagDownLeftLC, btnDownLeftLclick, cmbDownLeftLclick, switchDownLeftLC, "Down-Left");
            ApplyLoadedDiagonalCombineControlState(diagDownLeftRC, btnDownLeftRclick, cmbDownLeftRclick, switchDownLeftRC, "Down-Left");

            ApplyLoadedDiagonalCombineControlState(diagUpLeftHover, btnUpLeftHover, cmbUpLeftHover, switchUpLeftHover, "Up-Left");
            ApplyLoadedDiagonalCombineControlState(diagUpLeftLC, btnUpLeftLclick, cmbUpLeftLclick, switchUpLeftLC, "Up-Left");
            ApplyLoadedDiagonalCombineControlState(diagUpLeftRC, btnUpLeftRclick, cmbUpLeftRclick, switchUpLeftRC, "Up-Left");

            ApplyLoadedDiagonalMergeState(mergeUpRight, diagUpRightHover.Checked || diagUpRightLC.Checked || diagUpRightRC.Checked, "Up-Right");
            ApplyLoadedDiagonalMergeState(mergeDownRight, diagDownRightHover.Checked || diagDownRightLC.Checked || diagDownRightRC.Checked, "Down-Right");
            ApplyLoadedDiagonalMergeState(mergeDownLeft, diagDownLeftHover.Checked || diagDownLeftLC.Checked || diagDownLeftRC.Checked, "Down-Left");
            ApplyLoadedDiagonalMergeState(mergeUpLeft, diagUpLeftHover.Checked || diagUpLeftLC.Checked || diagUpLeftRC.Checked, "Up-Left");
        }

        private void NormalizeLoadedAssignmentControls()
        {
            string[] disableButtonNames =
            {
                "disableUpHover", "disableUpRightHover", "disableRightHover", "disableDownRightHover", "disableDownHover", "disableDownLeftHover", "disableLeftHover", "disableUpLeftHover",
                "disableUpLC", "disableUpRightLC", "disableRightLC", "disableDownRightLC", "disableDownLC", "disableDownLeftLC", "disableLeftLC", "disableUpLeftLC",
                "disableUpRC", "disableUpRightRC", "disableRightRC", "disableDownRightRC", "disableDownRC", "disableDownLeftRC", "disableLeftRC", "disableUpLeftRC",
                "disableDZUpperHover", "disableDZUpperLC", "disableDZUpperRC", "disableDZMiddleHover", "disableDZMiddleLC", "disableDZMiddleRC", "disableDZLowerHover", "disableDZLowerLC", "disableDZLowerRC"
            };

            for (int index = 0; index < disableButtonNames.Length; index++)
            {
                if (buttonConfig == null || index >= buttonConfig.Length || buttonConfig[index] == null)
                {
                    continue;
                }

                string disableButtonName = disableButtonNames[index];
                string xboxControlName = disableButtonName.Contains("DZ")
                    ? disableButtonName.Replace("disable", "cmb")
                    : disableButtonName.Replace("LC", "Lclick").Replace("disable", "cmb").Replace("RC", "Rclick");
                string switchControlName = disableButtonName.Replace("disable", "switch");
                string keyboardControlName = disableButtonName.Contains("DZ")
                    ? disableButtonName.Replace("disable", "btn")
                    : disableButtonName.Replace("LC", "Lclick").Replace("disable", "btn").Replace("RC", "Rclick");

                Control[] xboxControls = Controls.Find(xboxControlName, true);
                if (xboxControls.Length > 0 && xboxControls[0] is ComboBox xboxCombo)
                {
                    ApplyLoadedComboValue(xboxCombo, buttonConfig[index].Length > 3 ? buttonConfig[index][3] : "None");
                }

                Control[] switchControls = Controls.Find(switchControlName, true);
                if (switchControls.Length > 0 && switchControls[0] is ComboBox switchCombo)
                {
                    ApplyLoadedComboValue(switchCombo, buttonConfig[index].Length > 6 ? buttonConfig[index][6] : "None");
                }

                Control[] keyboardControls = Controls.Find(keyboardControlName, true);
                if (keyboardControls.Length > 0 && keyboardControls[0] is Button keyboardButton)
                {
                    ApplyLoadedButtonValue(keyboardButton, buttonConfig[index].Length > 0 ? buttonConfig[index][0] : "None");
                }
            }
        }

        private void ApplyLoadedComboValue(ComboBox comboBox, string loadedValue)
        {
            if (comboBox == null)
            {
                return;
            }

            string normalizedValue = string.IsNullOrWhiteSpace(loadedValue)
                ? "None"
                : loadedValue.Replace("/", "-");

            if (!HasAssignedActionValue(normalizedValue))
            {
                if (comboBox.Items.Contains("None"))
                {
                    comboBox.SelectedItem = "None";
                }
                else
                {
                    comboBox.SelectedIndex = -1;
                }
                comboBox.Text = "None";
                return;
            }

            if (comboBox.Items.Contains(normalizedValue))
            {
                comboBox.SelectedItem = normalizedValue;
            }
            else
            {
                comboBox.Items.Add(normalizedValue);
                comboBox.SelectedItem = normalizedValue;
            }

            comboBox.Text = normalizedValue;
        }

        private void ApplyLoadedButtonValue(Button button, string loadedValue)
        {
            if (button == null)
            {
                return;
            }

            string normalizedValue = string.IsNullOrWhiteSpace(loadedValue)
                ? "None"
                : loadedValue.Replace("/", "-");

            button.Text = HasAssignedActionValue(normalizedValue) ? normalizedValue : "None";
        }

        private void ApplyLoadedDiagonalCombineControlState(CheckBox combineCheckBox, Button targetButton, ComboBox xboxCombo, ComboBox switchCombo, string combinedLabel)
        {
            bool isCombined = combineCheckBox.Checked;

            static void SetFirstItemLabel(ComboBox comboBox, string label)
            {
                if (comboBox == null)
                {
                    return;
                }

                if (comboBox.Items.Count == 0)
                {
                    comboBox.Items.Add(label);
                }
                else
                {
                    comboBox.Items[0] = label;
                }

                comboBox.SelectedIndex = 0;
                comboBox.Text = label;
            }

            combineCheckBox.ToolTipText = isCombined ? "Click to Uncombine" : "Click to Combine";

            if (isCombined)
            {
                targetButton.Text = combinedLabel;
                SetFirstItemLabel(xboxCombo, combinedLabel);
                SetFirstItemLabel(switchCombo, combinedLabel);
            }
            else
            {
                if (string.Equals(xboxCombo.Text, combinedLabel, StringComparison.OrdinalIgnoreCase)
                    || (xboxCombo.SelectedItem != null && string.Equals(xboxCombo.SelectedItem.ToString(), combinedLabel, StringComparison.OrdinalIgnoreCase)))
                {
                    SetFirstItemLabel(xboxCombo, "None");
                }

                if (string.Equals(switchCombo.Text, combinedLabel, StringComparison.OrdinalIgnoreCase)
                    || (switchCombo.SelectedItem != null && string.Equals(switchCombo.SelectedItem.ToString(), combinedLabel, StringComparison.OrdinalIgnoreCase)))
                {
                    SetFirstItemLabel(switchCombo, "None");
                }
            }

            targetButton.Enabled = !isCombined;
            xboxCombo.Enabled = !isCombined;
            switchCombo.Enabled = !isCombined;
        }

        private void btnImport_Click(object sender, EventArgs e)
        {

            string configPath = Path.Combine(FileSystem.Current.AppDataDirectory, "configs");
            string profilePath = Path.Combine(FileSystem.Current.AppDataDirectory, "profiles");
            string parametersFilePath = Path.Combine(configPath, "parameters.txt");

            for (int i = 0; i <= cmbProfile.Items.Count; i++)
            {
                if (cmbProfile.SelectedIndex == i && cmbProfile.SelectedIndex > 0) { string profileText3 = cmbProfile.Items[i].ToString(); configName = profileText3 + ".txt"; }
                else if (cmbProfile.SelectedIndex == 0) { configName = "Music.txt"; }
            }

            using (StreamWriter file = new StreamWriter(new FileStream(Path.Combine(profilePath, configName), FileMode.OpenOrCreate)))
            {
                PrepareSettingsForSave();

                WriteCurrentButtonConfigRows(file);

            }


            string settingsCode = "";
            string profileName = "";
            string extracted = "";

            InputForm inputForm = new InputForm();
            var result = inputForm.ShowDialog();

            if (result == DialogResult.Abort)
            {

                btnNew.PerformClick();
                ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 1; -webkit-box-orient: vertical; overflow: hidden; ' title='Name your layout in the textbox upper left, then press Add.'>Name your layout in the textbox upper left, then press Add.</span></p>", "Create New Layout");
                return;

            }
            else if (result == DialogResult.OK)
            {
                settingsCode = inputForm.GetSettingsCode();
                profileName = inputForm.GetProfileName();
                string importedConfigName = profileName + ".txt";
                // Do something with settingsCode and profileName

                // Find the index of the third dash
                int firstDashIndex = settingsCode.IndexOf('-');
                int secondDashIndex = settingsCode.IndexOf('-', firstDashIndex + 1);
                int thirdDashIndex = settingsCode.IndexOf('-', secondDashIndex + 1);

                // Check if the third dash exists
                if (thirdDashIndex != -1)
                {
                    // Extract everything after the third dash
                    extracted = settingsCode.Substring(thirdDashIndex + 1);

                    // Remove the extracted part and the third dash
                    string modified = settingsCode.Substring(0, thirdDashIndex);

                    settingsCode = modified;
                }
                else
                {
                    // Handle cases where there are fewer than three dashes
                    extracted = "0"; // No change if there aren't enough dashes
                }
                // Example encoded string
                string encodedString = settingsCode;

                //get encodedString from the user via message box
                //encodedString = Microsoft.VisualBasic.Interaction.InputBox("Enter the encoded string", "Encoded String", "", 0, 0);

                //if first character of encodedString is N, then boolStart = false
                bool boolStart = encodedString[0] == 'N' ? false : true;
                string configMode = "";
                if (encodedString[1] == 'K') { configMode = "Keyboard"; }
                else if (encodedString[1] == 'X') { configMode = "Xbox"; }
                else if (encodedString[1] == 'S') { configMode = "Switch"; }


                //split the encoded string into an array using the '-' character
                string[] encodedArray = encodedString.Split('-');

                string boolString = encodedArray[1];
                bool isExactSettingsCodeEncoding = boolString.StartsWith("Z", StringComparison.Ordinal);

                List<string> decodedArray = new List<string>();

                if (isExactSettingsCodeEncoding)
                {
                    for (int i = 1; i < boolString.Length; i++)
                    {
                        switch (boolString[i])
                        {
                            case 'T':
                                decodedArray.Add("True");
                                break;
                            case 'O':
                                decodedArray.Add("Once");
                                break;
                            case 'H':
                                decodedArray.Add("Hold");
                                break;
                            default:
                                decodedArray.Add("False");
                                break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < boolString.Length; i++)
                    {
                        if (char.IsDigit(boolString[i]))
                        {
                            decodedArray.Add(boolString[i].ToString());
                        }
                        else if (char.IsLetter(boolString[i]))
                        {
                            bool isCurrentAlphaDigit = boolString[i] >= 'A' && boolString[i] <= 'J';
                            bool hasNext = i < boolString.Length - 1;
                            bool isNextAlphaDigit = hasNext && boolString[i + 1] >= 'A' && boolString[i + 1] <= 'J';

                            // Prefer A-J pair decoding first so legacy H/I markers don't split valid count pairs.
                            if (isCurrentAlphaDigit && isNextAlphaDigit)
                            {
                                decodedArray.Add(boolString[i].ToString() + boolString[i + 1].ToString());
                                i++; // Skip the next letter
                            }
                            else if (boolString[i] == 'O')
                            {
                                decodedArray.Add("O");
                            }
                            else if (boolString[i] == 'P')
                            {
                                decodedArray.Add("P");
                            }
                            else if (boolString[i] == 'H')
                            {
                                decodedArray.Add("H");
                            }
                            else if (boolString[i] == 'I')
                            {
                                decodedArray.Add("I");
                            }
                            else if (boolString[i] == 'Q')
                            {
                                decodedArray.Add("Q");
                            }
                            else if (boolString[i] == 'R')
                            {
                                decodedArray.Add("R");
                            }
                            else
                            {
                                decodedArray.Add(boolString[i].ToString());
                            }
                        }
                    }
                }

                // Print the decoded array
                foreach (string item in decodedArray)
                {
                    Debug.Write(item + " ");
                }


                boolString = encodedArray[2];


                List<string> decodedArray2 = new List<string>();

                for (int i = 0; i < boolString.Length; i++)
                {
                    if (char.IsDigit(boolString[i]))
                    {
                        decodedArray2.Add(boolString[i].ToString());
                    }
                    else if (char.IsLetter(boolString[i]))
                    {
                        if (i < boolString.Length - 1 && char.IsLetter(boolString[i + 1]))
                        {
                            decodedArray2.Add(boolString[i].ToString() + boolString[i + 1].ToString());
                            i++; // Skip the next letter
                        }
                        else
                        {
                            // Handle the case where the last character is a letter
                            decodedArray2.Add(boolString[i].ToString());
                        }
                    }

                }

                // Print the decoded array
                foreach (string item in decodedArray2)
                {
                    Debug.Write(item + " ");
                }

                List<string> decodedValues = new List<string>();

                foreach (var part in decodedArray)
                {
                    if (part == "True" || part == "False" || part == "Once" || part == "Hold")
                    {
                        decodedValues.Add(part);
                    }
                    else if (int.TryParse(part, out int number))
                    {
                        decodedValues.Add(number.ToString());
                    }
                    else if (part.Length == 1 && (part == "P" || part == "O" || part == "H" || part == "I" || part == "Q" || part == "R"))
                    {
                        decodedValues.Add(part);
                    }
                    else
                    {
                        int value = 0;
                        foreach (char c in part)
                        {
                            value = value * 10 + (c - 'A');
                        }
                        decodedValues.Add(value.ToString());
                    }
                }



                Debug.WriteLine(System.Environment.NewLine + string.Join(", ", decodedValues));

                List<int> settingsArray = new List<int>();

                foreach (var part in decodedArray2)
                {
                    if (int.TryParse(part, out int number))
                    {
                        settingsArray.Add(number);
                    }

                    else
                    {
                        int value = 0;
                        foreach (char c in part)
                        {
                            value = value * 10 + (c - 'A');
                        }
                        settingsArray.Add(value);
                    }
                }

                Debug.WriteLine(System.Environment.NewLine + string.Join(", ", settingsArray));

                List<string> boolArray = new List<string>();
                bool isLegacyBooleanEncoding = !isExactSettingsCodeEncoding;

                for (int i = 0; i < decodedValues.Count; i++)
                {
                    if (decodedValues[i] == "True" || decodedValues[i] == "False" || decodedValues[i] == "Once" || decodedValues[i] == "Hold")
                    {
                        boolArray.Add(decodedValues[i]);
                    }
                    else if (int.TryParse(decodedValues[i], out int number))
                    {
                        for (int j = 0; j < number; j++)
                        {
                            boolArray.Add(boolStart ? "True" : "False");
                        }
                        boolStart = !boolStart; // Toggle the flag after processing the number
                    }
                    else if (decodedValues[i] == "O")
                    {
                        boolArray.Add("Once");
                        if (i + 1 < decodedValues.Count && int.TryParse(decodedValues[i + 1], out int trueCount))
                        {
                            for (int j = 0; j < trueCount; j++)
                            {
                                boolArray.Add("True");
                            }
                            i++; // Skip the next number
                        }
                    }
                    else if (decodedValues[i] == "P")
                    {
                        boolArray.Add("Once");
                        if (i + 1 < decodedValues.Count && int.TryParse(decodedValues[i + 1], out int falseCount))
                        {
                            for (int j = 0; j < falseCount; j++)
                            {
                                boolArray.Add("False");
                            }
                            i++; // Skip the next number
                        }
                    }
                    else if (decodedValues[i] == "H")
                    {
                        boolArray.Add("Hold");
                        if (i + 1 < decodedValues.Count && int.TryParse(decodedValues[i + 1], out int holdFalseCount))
                        {
                            for (int j = 0; j < holdFalseCount; j++)
                            {
                                boolArray.Add("False");
                            }
                            i++; // Skip the next number
                        }
                    }
                    else if (decodedValues[i] == "I")
                    {
                        boolArray.Add("Hold");
                        if (i + 1 < decodedValues.Count && int.TryParse(decodedValues[i + 1], out int holdTrueCount))
                        {
                            for (int j = 0; j < holdTrueCount; j++)
                            {
                                boolArray.Add("True");
                            }
                            i++; // Skip the next number
                        }
                    }
                    else if (decodedValues[i] == "Q")
                    {
                        boolArray.Add("Once");
                    }
                    else if (decodedValues[i] == "R")
                    {
                        boolArray.Add("Hold");
                    }
                }

                Debug.WriteLine(System.Environment.NewLine + string.Join(", ", boolArray));

                if (isLegacyBooleanEncoding)
                {
                    // Legacy imports can carry diagonal-combine flags that overwrite explicit LC/RC assignments.
                    // Force these four quadrants to remain uncombined so their mapped buttons persist.
                    int[] forcedFalseDiagonalFlags = { 97, 98, 103, 104 };
                    foreach (int flagIndex in forcedFalseDiagonalFlags)
                    {
                        if (flagIndex >= 0 && flagIndex < boolArray.Count)
                        {
                            boolArray[flagIndex] = "False";
                        }
                    }

                    int[] forcedTrueStates = { 27, 45, 51, 69 };
                    foreach (int stateIndex in forcedTrueStates)
                    {
                        if (stateIndex >= 0 && stateIndex < boolArray.Count
                            && string.Equals(boolArray[stateIndex], "Hold", StringComparison.OrdinalIgnoreCase))
                        {
                            boolArray[stateIndex] = "True";
                        }
                    }
                }

                var importedActivationFlags = NormalizeImportedActivationFlags(boolArray);

                try
                {

                    using (StreamWriter file = new StreamWriter(new FileStream(Path.Combine(profilePath, importedConfigName), FileMode.Create, FileAccess.Write, FileShare.ReadWrite)))
                    {
                        for (int i = 0; i < 33; i++)
                        {
                            if (configMode == "Keyboard")
                            {
                                keyboardAction = keyboardOptions[settingsArray[i]];
                            }
                            else if (configMode == "Xbox")
                            {
                                xboxAction = buttonOptions[settingsArray[i]];
                            }
                            else if (configMode == "Switch")
                            {
                                switchAction = switchButtonOptions[settingsArray[i]];
                            }

                            string state;
                            if (i == 0)
                            {
                                if (boolArray[0] == "True") { state = "True"; }
                                else if (boolArray[0] == "Once") { state = "Once"; }
                                else { state = "False"; }

                                if (configMode == "Keyboard") { xboxAction = defaultXbox[i]; switchAction = defaultSwitch[i]; }
                                else if (configMode == "Xbox") { switchAction = defaultSwitch[i]; keyboardAction = defaultKB[i]; }
                                else if (configMode == "Switch") { xboxAction = defaultXbox[i]; keyboardAction = defaultKB[i]; }

                                buttonConfig[i] = new string[] { keyboardAction, state, boolArray[1], xboxAction, "False", boolArray[2], switchAction };
                            }
                            else if (i == 1)
                            {
                                if (boolArray[3] == "True") { state = "True"; }
                                else if (boolArray[3] == "Once") { state = "Once"; }
                                else { state = "False"; }

                                if (configMode == "Keyboard") { xboxAction = defaultXbox[i]; switchAction = defaultSwitch[i]; }
                                else if (configMode == "Xbox") { switchAction = defaultSwitch[i]; keyboardAction = defaultKB[i]; }
                                else if (configMode == "Switch") { xboxAction = defaultXbox[i]; keyboardAction = defaultKB[i]; }

                                buttonConfig[i] = new string[] { keyboardAction, state, boolArray[4], xboxAction, "False", boolArray[5], switchAction };
                            }
                            else if (i == 2)
                            {
                                if (boolArray[6] == "True") { state = "True"; }
                                else if (boolArray[6] == "Once") { state = "Once"; }
                                else { state = "False"; }

                                if (configMode == "Keyboard") { xboxAction = defaultXbox[i]; switchAction = defaultSwitch[i]; }
                                else if (configMode == "Xbox") { switchAction = defaultSwitch[i]; keyboardAction = defaultKB[i]; }
                                else if (configMode == "Switch") { xboxAction = defaultXbox[i]; keyboardAction = defaultKB[i]; }

                                buttonConfig[i] = new string[] { keyboardAction, state, boolArray[7], xboxAction, "False", boolArray[8], switchAction };
                            }
                            else if (i == 3)
                            {
                                if (boolArray[9] == "True") { state = "True"; }
                                else if (boolArray[9] == "Once") { state = "Once"; }
                                else { state = "False"; }

                                if (configMode == "Keyboard") { xboxAction = defaultXbox[i]; switchAction = defaultSwitch[i]; }
                                else if (configMode == "Xbox") { switchAction = defaultSwitch[i]; keyboardAction = defaultKB[i]; }
                                else if (configMode == "Switch") { xboxAction = defaultXbox[i]; keyboardAction = defaultKB[i]; }

                                buttonConfig[i] = new string[] { keyboardAction, state, boolArray[10], xboxAction, "False", boolArray[11], switchAction };
                            }
                            else if (i == 4)
                            {
                                if (boolArray[12] == "True") { state = "True"; }
                                else if (boolArray[12] == "Once") { state = "Once"; }
                                else { state = "False"; }

                                if (configMode == "Keyboard") { xboxAction = defaultXbox[i]; switchAction = defaultSwitch[i]; }
                                else if (configMode == "Xbox") { switchAction = defaultSwitch[i]; keyboardAction = defaultKB[i]; }
                                else if (configMode == "Switch") { xboxAction = defaultXbox[i]; keyboardAction = defaultKB[i]; }

                                buttonConfig[i] = new string[] { keyboardAction, state, boolArray[13], xboxAction, "False", boolArray[14], switchAction };
                            }
                            else if (i == 5)
                            {
                                if (boolArray[15] == "True") { state = "True"; }
                                else if (boolArray[15] == "Once") { state = "Once"; }
                                else { state = "False"; }

                                if (configMode == "Keyboard") { xboxAction = defaultXbox[i]; switchAction = defaultSwitch[i]; }
                                else if (configMode == "Xbox") { switchAction = defaultSwitch[i]; keyboardAction = defaultKB[i]; }
                                else if (configMode == "Switch") { xboxAction = defaultXbox[i]; keyboardAction = defaultKB[i]; }

                                buttonConfig[i] = new string[] { keyboardAction, state, boolArray[16], xboxAction, "False", boolArray[17], switchAction };
                            }
                            else if (i == 6)
                            {
                                if (boolArray[18] == "True") { state = "True"; }
                                else if (boolArray[18] == "Once") { state = "Once"; }
                                else { state = "False"; }

                                if (configMode == "Keyboard") { xboxAction = defaultXbox[i]; switchAction = defaultSwitch[i]; }
                                else if (configMode == "Xbox") { switchAction = defaultSwitch[i]; keyboardAction = defaultKB[i]; }
                                else if (configMode == "Switch") { xboxAction = defaultXbox[i]; keyboardAction = defaultKB[i]; }

                                buttonConfig[i] = new string[] { keyboardAction, state, boolArray[19], xboxAction, "False", boolArray[20], switchAction };
                            }
                            else if (i == 7)
                            {
                                if (boolArray[21] == "True") { state = "True"; }
                                else if (boolArray[21] == "Once") { state = "Once"; }
                                else { state = "False"; }

                                if (configMode == "Keyboard") { xboxAction = defaultXbox[i]; switchAction = defaultSwitch[i]; }
                                else if (configMode == "Xbox") { switchAction = defaultSwitch[i]; keyboardAction = defaultKB[i]; }
                                else if (configMode == "Switch") { xboxAction = defaultXbox[i]; keyboardAction = defaultKB[i]; }

                                buttonConfig[i] = new string[] { keyboardAction, state, boolArray[22], xboxAction, "False", boolArray[23], switchAction };
                            }
                            else if (i == 8)
                            {

                                if (boolArray[24] == "True") { state = "True"; }
                                else if (boolArray[24] == "Once") { state = "Once"; }
                                else { state = "Hold"; }

                                if (configMode == "Keyboard") { xboxAction = defaultXbox[i]; switchAction = switchButtonOptions[2]; }
                                else if (configMode == "Xbox") { switchAction = switchButtonOptions[2]; keyboardAction = defaultKB[i]; }
                                else if (configMode == "Switch") { xboxAction = defaultXbox[i]; keyboardAction = defaultKB[i]; }

                                buttonConfig[i] = new string[] { keyboardAction, state, boolArray[25], xboxAction, boolArray[26], "False", switchAction };

                            }
                            else if (i == 9)
                            {
                                if (boolArray[27] == "True") { state = "True"; }
                                else if (boolArray[27] == "Once") { state = "Once"; }
                                else { state = "Hold"; }

                                bool combineUpRightLC = boolArray.Count > 97 && string.Equals(boolArray[97], "True", StringComparison.OrdinalIgnoreCase);

                                if (configMode == "Keyboard") { xboxAction = defaultXbox[i]; switchAction = switchButtonOptions[2]; }
                                else if (configMode == "Xbox") { switchAction = switchButtonOptions[2]; keyboardAction = defaultKB[i]; }
                                else if (configMode == "Switch") { xboxAction = defaultXbox[i]; keyboardAction = defaultKB[i]; }

                                buttonConfig[i] = new string[] { keyboardAction, state, boolArray[28], xboxAction, boolArray[29], "False", switchAction };
                                if (string.Equals(state, "Hold", StringComparison.OrdinalIgnoreCase) && combineUpRightLC)
                                {
                                    buttonConfig[i][0] = "Up-Right";
                                    buttonConfig[i][3] = "Up-Right";
                                    buttonConfig[i][6] = "Up-Right";
                                }

                                if (isLegacyBooleanEncoding)
                                {
                                    if (string.Equals(state, "Hold", StringComparison.OrdinalIgnoreCase))
                                    {
                                        state = "True";
                                        buttonConfig[i][1] = state;
                                        buttonConfig[i][0] = keyboardAction;
                                    }
                                    buttonConfig[i][3] = "LT or L2";
                                    buttonConfig[i][6] = "ZL Button";
                                }
                            }
                            else if (i == 10)
                            {
                                if (boolArray[30] == "True") { state = "True"; }
                                else if (boolArray[30] == "Once") { state = "Once"; }
                                else { state = "Hold"; }

                                if (configMode == "Keyboard") { xboxAction = defaultXbox[i]; switchAction = defaultSwitch[i]; }
                                else if (configMode == "Xbox") { switchAction = defaultSwitch[i]; keyboardAction = defaultKB[i]; }
                                else if (configMode == "Switch") { xboxAction = defaultXbox[i]; keyboardAction = defaultKB[i]; }

                                buttonConfig[i] = new string[] { keyboardAction, state, boolArray[31], xboxAction, boolArray[32], "False", switchAction };
                            }
                            else if (i == 11)
                            {
                                if (boolArray[33] == "True") { state = "True"; }
                                else if (boolArray[33] == "Once") { state = "Once"; }
                                else { state = "Hold"; }

                                if (configMode == "Keyboard") { xboxAction = defaultXbox[i]; switchAction = defaultSwitch[i]; }
                                else if (configMode == "Xbox") { switchAction = defaultSwitch[i]; keyboardAction = defaultKB[i]; }
                                else if (configMode == "Switch") { xboxAction = defaultXbox[i]; keyboardAction = defaultKB[i]; }

                                buttonConfig[i] = new string[] { keyboardAction, state, boolArray[34], xboxAction, boolArray[35], "False", switchAction };
                                if (string.Equals(state, "Once", StringComparison.OrdinalIgnoreCase)
                                    && string.Equals(buttonConfig[i][3], "None", StringComparison.OrdinalIgnoreCase)
                                    && string.Equals(buttonConfig[i][6], "None", StringComparison.OrdinalIgnoreCase))
                                {
                                    buttonConfig[i][3] = "Y or ꕔ";
                                    buttonConfig[i][6] = "Y Button";
                                }
                            }
                            else if (i == 12)
                            {
                                if (boolArray[36] == "True") { state = "True"; }
                                else if (boolArray[36] == "Once") { state = "Once"; }
                                else { state = "Hold"; }

                                if (configMode == "Keyboard") { xboxAction = defaultXbox[i]; switchAction = defaultSwitch[i]; }
                                else if (configMode == "Xbox") { switchAction = defaultSwitch[i]; keyboardAction = defaultKB[i]; }
                                else if (configMode == "Switch") { xboxAction = defaultXbox[i]; keyboardAction = defaultKB[i]; }

                                buttonConfig[i] = new string[] { keyboardAction, state, boolArray[37], xboxAction, boolArray[38], "False", switchAction };
                            }
                            else if (i == 13)
                            {
                                if (boolArray[39] == "True") { state = "True"; }
                                else if (boolArray[39] == "Once") { state = "Once"; }
                                else { state = "Hold"; }

                                if (configMode == "Keyboard") { xboxAction = defaultXbox[i]; switchAction = defaultSwitch[i]; }
                                else if (configMode == "Xbox") { switchAction = defaultSwitch[i]; keyboardAction = defaultKB[i]; }
                                else if (configMode == "Switch") { xboxAction = defaultXbox[i]; keyboardAction = defaultKB[i]; }

                                buttonConfig[i] = new string[] { keyboardAction, state, boolArray[40], xboxAction, boolArray[41], "False", switchAction };
                                if (string.Equals(state, "Once", StringComparison.OrdinalIgnoreCase)
                                    && string.Equals(buttonConfig[i][3], "None", StringComparison.OrdinalIgnoreCase)
                                    && string.Equals(buttonConfig[i][6], "None", StringComparison.OrdinalIgnoreCase))
                                {
                                    buttonConfig[i][3] = "X or ⬜";
                                    buttonConfig[i][6] = "X Button";
                                }
                            }
                            else if (i == 14)
                            {
                                if (boolArray[42] == "True") { state = "True"; }
                                else if (boolArray[42] == "Once") { state = "Once"; }
                                else { state = "Hold"; }

                                if (configMode == "Keyboard") { xboxAction = defaultXbox[i]; switchAction = defaultSwitch[i]; }
                                else if (configMode == "Xbox") { switchAction = defaultSwitch[i]; keyboardAction = defaultKB[i]; }
                                else if (configMode == "Switch") { xboxAction = defaultXbox[i]; keyboardAction = defaultKB[i]; }

                                buttonConfig[i] = new string[] { keyboardAction, state, boolArray[43], xboxAction, boolArray[44], "False", switchAction };
                            }
                            else if (i == 15)
                            {
                                if (boolArray[45] == "True") { state = "True"; }
                                else if (boolArray[45] == "Once") { state = "Once"; }
                                else { state = "Hold"; }

                                bool combineUpLeftLC = boolArray.Count > 103 && string.Equals(boolArray[103], "True", StringComparison.OrdinalIgnoreCase);

                                if (configMode == "Keyboard") { xboxAction = defaultXbox[i]; switchAction = defaultSwitch[i]; }
                                else if (configMode == "Xbox") { switchAction = defaultSwitch[i]; keyboardAction = defaultKB[i]; }
                                else if (configMode == "Switch") { xboxAction = defaultXbox[i]; keyboardAction = defaultKB[i]; }

                                buttonConfig[i] = new string[] { keyboardAction, state, boolArray[46], xboxAction, boolArray[47], "False", switchAction };
                                if (string.Equals(state, "Hold", StringComparison.OrdinalIgnoreCase) && combineUpLeftLC)
                                {
                                    buttonConfig[i][0] = "Up-Left";
                                    buttonConfig[i][3] = "Up-Left";
                                    buttonConfig[i][6] = "Up-Left";
                                }

                                if (isLegacyBooleanEncoding)
                                {
                                    if (string.Equals(state, "Hold", StringComparison.OrdinalIgnoreCase))
                                    {
                                        state = "True";
                                        buttonConfig[i][1] = state;
                                        buttonConfig[i][0] = keyboardAction;
                                    }
                                    buttonConfig[i][3] = "B or O";
                                    buttonConfig[i][6] = "B Button";
                                }
                            }
                            else if (i == 16)
                            {
                                if (boolArray[48] == "True") { state = "True"; }
                                else if (boolArray[48] == "Once") { state = "Once"; }
                                else { state = "Hold"; }

                                if (configMode == "Keyboard") { xboxAction = defaultXbox[i]; switchAction = defaultSwitch[i]; }
                                else if (configMode == "Xbox") { switchAction = defaultSwitch[i]; keyboardAction = defaultKB[i]; }
                                else if (configMode == "Switch") { xboxAction = defaultXbox[i]; keyboardAction = defaultKB[i]; }

                                buttonConfig[i] = new string[] { keyboardAction, state, boolArray[49], xboxAction, boolArray[50], "False", switchAction };
                            }
                            else if (i == 17)
                            {
                                if (boolArray[51] == "True") { state = "True"; }
                                else if (boolArray[51] == "Once") { state = "Once"; }
                                else { state = "Hold"; }

                                bool combineUpRightRC = boolArray.Count > 98 && string.Equals(boolArray[98], "True", StringComparison.OrdinalIgnoreCase);

                                if (configMode == "Keyboard") { xboxAction = defaultXbox[i]; switchAction = defaultSwitch[i]; }
                                else if (configMode == "Xbox") { switchAction = defaultSwitch[i]; keyboardAction = defaultKB[i]; }
                                else if (configMode == "Switch") { xboxAction = defaultXbox[i]; keyboardAction = defaultKB[i]; }

                                buttonConfig[i] = new string[] { keyboardAction, state, boolArray[52], xboxAction, boolArray[53], "False", switchAction };
                                if (string.Equals(state, "Hold", StringComparison.OrdinalIgnoreCase) && combineUpRightRC)
                                {
                                    buttonConfig[i][0] = "Up-Right";
                                    buttonConfig[i][3] = "Up-Right";
                                    buttonConfig[i][6] = "Up-Right";
                                }

                                if (isLegacyBooleanEncoding)
                                {
                                    if (string.Equals(state, "Hold", StringComparison.OrdinalIgnoreCase))
                                    {
                                        state = "True";
                                        buttonConfig[i][1] = state;
                                        buttonConfig[i][0] = keyboardAction;
                                    }
                                    buttonConfig[i][3] = "B or O";
                                    buttonConfig[i][6] = "B Button";
                                }
                            }
                            else if (i == 18)
                            {
                                if (boolArray[54] == "True") { state = "True"; }
                                else if (boolArray[54] == "Once") { state = "Once"; }
                                else { state = "Hold"; }

                                if (configMode == "Keyboard") { xboxAction = defaultXbox[i]; switchAction = defaultSwitch[i]; }
                                else if (configMode == "Xbox") { switchAction = defaultSwitch[i]; keyboardAction = defaultKB[i]; }
                                else if (configMode == "Switch") { xboxAction = defaultXbox[i]; keyboardAction = defaultKB[i]; }

                                buttonConfig[i] = new string[] { keyboardAction, state, boolArray[55], xboxAction, boolArray[56], "False", switchAction };
                            }
                            else if (i == 19)
                            {
                                if (boolArray[57] == "True") { state = "True"; }
                                else if (boolArray[57] == "Once") { state = "Once"; }
                                else { state = "Hold"; }

                                if (configMode == "Keyboard") { xboxAction = defaultXbox[i]; switchAction = defaultSwitch[i]; }
                                else if (configMode == "Xbox") { switchAction = defaultSwitch[i]; keyboardAction = defaultKB[i]; }
                                else if (configMode == "Switch") { xboxAction = defaultXbox[i]; keyboardAction = defaultKB[i]; }

                                buttonConfig[i] = new string[] { keyboardAction, state, boolArray[58], xboxAction, boolArray[59], "False", switchAction };
                                if (string.Equals(state, "Once", StringComparison.OrdinalIgnoreCase)
                                    && string.Equals(buttonConfig[i][3], "None", StringComparison.OrdinalIgnoreCase)
                                    && string.Equals(buttonConfig[i][6], "None", StringComparison.OrdinalIgnoreCase))
                                {
                                    buttonConfig[i][3] = "Y or ꕔ";
                                    buttonConfig[i][6] = "Y Button";
                                }
                            }
                            else if (i == 20)
                            {
                                if (boolArray[60] == "True") { state = "True"; }
                                else if (boolArray[60] == "Once") { state = "Once"; }
                                else { state = "Hold"; }

                                if (configMode == "Keyboard") { xboxAction = defaultXbox[i]; switchAction = defaultSwitch[i]; }
                                else if (configMode == "Xbox") { switchAction = defaultSwitch[i]; keyboardAction = defaultKB[i]; }
                                else if (configMode == "Switch") { xboxAction = defaultXbox[i]; keyboardAction = defaultKB[i]; }

                                buttonConfig[i] = new string[] { keyboardAction, state, boolArray[61], xboxAction, boolArray[62], "False", switchAction };
                            }
                            else if (i == 21)
                            {
                                if (boolArray[63] == "True") { state = "True"; }
                                else if (boolArray[63] == "Once") { state = "Once"; }
                                else { state = "Hold"; }

                                if (configMode == "Keyboard") { xboxAction = defaultXbox[i]; switchAction = defaultSwitch[i]; }
                                else if (configMode == "Xbox") { switchAction = defaultSwitch[i]; keyboardAction = defaultKB[i]; }
                                else if (configMode == "Switch") { xboxAction = defaultXbox[i]; keyboardAction = defaultKB[i]; }

                                buttonConfig[i] = new string[] { keyboardAction, state, boolArray[64], xboxAction, boolArray[65], "False", switchAction };
                                if (string.Equals(state, "Once", StringComparison.OrdinalIgnoreCase)
                                    && string.Equals(buttonConfig[i][3], "None", StringComparison.OrdinalIgnoreCase)
                                    && string.Equals(buttonConfig[i][6], "None", StringComparison.OrdinalIgnoreCase))
                                {
                                    buttonConfig[i][3] = "X or ⬜";
                                    buttonConfig[i][6] = "X Button";
                                }
                            }
                            else if (i == 22)
                            {
                                if (boolArray[66] == "True") { state = "True"; }
                                else if (boolArray[66] == "Once") { state = "Once"; }
                                else { state = "Hold"; }

                                if (configMode == "Keyboard") { xboxAction = defaultXbox[i]; switchAction = defaultSwitch[i]; }
                                else if (configMode == "Xbox") { switchAction = defaultSwitch[i]; keyboardAction = defaultKB[i]; }
                                else if (configMode == "Switch") { xboxAction = defaultXbox[i]; keyboardAction = defaultKB[i]; }

                                buttonConfig[i] = new string[] { keyboardAction, state, boolArray[67], xboxAction, boolArray[68], "False", switchAction };
                            }
                            else if (i == 23)
                            {
                                if (boolArray[69] == "True") { state = "True"; }
                                else if (boolArray[69] == "Once") { state = "Once"; }
                                else { state = "Hold"; }

                                bool combineUpLeftRC = boolArray.Count > 104 && string.Equals(boolArray[104], "True", StringComparison.OrdinalIgnoreCase);

                                if (configMode == "Keyboard") { xboxAction = defaultXbox[i]; switchAction = defaultSwitch[i]; }
                                else if (configMode == "Xbox") { switchAction = defaultSwitch[i]; keyboardAction = defaultKB[i]; }
                                else if (configMode == "Switch") { xboxAction = defaultXbox[i]; keyboardAction = defaultKB[i]; }

                                buttonConfig[i] = new string[] { keyboardAction, state, boolArray[70], xboxAction, boolArray[71], "False", switchAction };
                                if (string.Equals(state, "Hold", StringComparison.OrdinalIgnoreCase) && combineUpLeftRC)
                                {
                                    buttonConfig[i][0] = "Up-Left";
                                    buttonConfig[i][3] = "Up-Left";
                                    buttonConfig[i][6] = "Up-Left";
                                }

                                if (isLegacyBooleanEncoding)
                                {
                                    if (string.Equals(state, "Hold", StringComparison.OrdinalIgnoreCase))
                                    {
                                        state = "True";
                                        buttonConfig[i][1] = state;
                                        buttonConfig[i][0] = keyboardAction;
                                    }
                                    buttonConfig[i][3] = "A or X";
                                    buttonConfig[i][6] = "A Button";
                                }
                            }
                            else if (i == 24)
                            {
                                if (boolArray[72] == "True") { state = "True"; }
                                else if (boolArray[72] == "Once") { state = "Once"; }
                                else { state = "False"; }

                                if (configMode == "Keyboard") { xboxAction = defaultXbox[i]; switchAction = defaultSwitch[i]; }
                                else if (configMode == "Xbox") { switchAction = defaultSwitch[i]; keyboardAction = defaultKB[i]; }
                                else if (configMode == "Switch") { xboxAction = defaultXbox[i]; keyboardAction = defaultKB[i]; }

                                buttonConfig[i] = new string[] { keyboardAction, state, boolArray[73], xboxAction, "False", "False", switchAction };
                            }
                            else if (i == 25)

                            {
                                if (boolArray[74] == "True") { state = "True"; }
                                else if (boolArray[74] == "Once") { state = "Once"; }
                                else { state = "Hold"; }

                                if (configMode == "Keyboard") { xboxAction = defaultXbox[i]; switchAction = defaultSwitch[i]; }
                                else if (configMode == "Xbox") { switchAction = defaultSwitch[i]; keyboardAction = defaultKB[i]; }
                                else if (configMode == "Switch") { xboxAction = defaultXbox[i]; keyboardAction = defaultKB[i]; }

                                buttonConfig[i] = new string[] { keyboardAction, state, boolArray[75], xboxAction, "False", "False", switchAction };
                            }
                            else if (i == 26)
                            {
                                if (boolArray[76] == "True") { state = "True"; }
                                else if (boolArray[76] == "Once") { state = "Once"; }
                                else { state = "Hold"; }

                                if (configMode == "Keyboard") { xboxAction = defaultXbox[i]; switchAction = defaultSwitch[i]; }
                                else if (configMode == "Xbox") { switchAction = defaultSwitch[i]; keyboardAction = defaultKB[i]; }
                                else if (configMode == "Switch") { xboxAction = defaultXbox[i]; keyboardAction = defaultKB[i]; }

                                buttonConfig[i] = new string[] { keyboardAction, state, boolArray[77], xboxAction, "False", "False", switchAction };
                            }
                            else if (i == 27)
                            {
                                if (boolArray[78] == "True") { state = "True"; }
                                else if (boolArray[78] == "Once") { state = "Once"; }
                                else { state = "False"; }

                                if (configMode == "Keyboard") { xboxAction = defaultXbox[i]; switchAction = defaultSwitch[i]; }
                                else if (configMode == "Xbox") { switchAction = defaultSwitch[i]; keyboardAction = defaultKB[i]; }
                                else if (configMode == "Switch") { xboxAction = defaultXbox[i]; keyboardAction = defaultKB[i]; }

                                buttonConfig[i] = new string[] { keyboardAction, state, boolArray[79], xboxAction, "False", "False", switchAction };
                            }
                            else if (i == 28)
                            {
                                if (boolArray[80] == "True") { state = "True"; }
                                else if (boolArray[80] == "Once") { state = "Once"; }
                                else { state = "Hold"; }

                                if (configMode == "Keyboard") { xboxAction = defaultXbox[i]; switchAction = defaultSwitch[i]; }
                                else if (configMode == "Xbox") { switchAction = defaultSwitch[i]; keyboardAction = defaultKB[i]; }
                                else if (configMode == "Switch") { xboxAction = defaultXbox[i]; keyboardAction = defaultKB[i]; }

                                buttonConfig[i] = new string[] { keyboardAction, state, boolArray[81], xboxAction, "False", "False", switchAction };
                            }
                            else if (i == 29)
                            {
                                if (boolArray[82] == "True") { state = "True"; }
                                else if (boolArray[82] == "Once") { state = "Once"; }
                                else { state = "Hold"; }

                                if (configMode == "Keyboard") { xboxAction = defaultXbox[i]; switchAction = defaultSwitch[i]; }
                                else if (configMode == "Xbox") { switchAction = defaultSwitch[i]; keyboardAction = defaultKB[i]; }
                                else if (configMode == "Switch") { xboxAction = defaultXbox[i]; keyboardAction = defaultKB[i]; }

                                buttonConfig[i] = new string[] { keyboardAction, state, boolArray[83], xboxAction, "False", "False", switchAction };
                            }
                            else if (i == 30)
                            {
                                if (boolArray[84] == "True") { state = "True"; }
                                else if (boolArray[84] == "Once") { state = "Once"; }
                                else { state = "False"; }

                                if (configMode == "Keyboard") { xboxAction = defaultXbox[i]; switchAction = defaultSwitch[i]; }
                                else if (configMode == "Xbox") { switchAction = defaultSwitch[i]; keyboardAction = defaultKB[i]; }
                                else if (configMode == "Switch") { xboxAction = defaultXbox[i]; keyboardAction = defaultKB[i]; }

                                buttonConfig[i] = new string[] { keyboardAction, state, boolArray[85], xboxAction, "False", "False", switchAction };
                            }
                            else if (i == 31)
                            {
                                if (boolArray[86] == "True") { state = "True"; }
                                else if (boolArray[86] == "Once") { state = "Once"; }
                                else { state = "Hold"; }

                                if (configMode == "Keyboard") { xboxAction = defaultXbox[i]; switchAction = defaultSwitch[i]; }
                                else if (configMode == "Xbox") { switchAction = defaultSwitch[i]; keyboardAction = defaultKB[i]; }
                                else if (configMode == "Switch") { xboxAction = defaultXbox[i]; keyboardAction = defaultKB[i]; }

                                buttonConfig[i] = new string[] { keyboardAction, state, boolArray[87], xboxAction, "False", "False", switchAction };
                            }
                            else if (i == 32)
                            {
                                if (boolArray[88] == "True") { state = "True"; }
                                else if (boolArray[88] == "Once") { state = "Once"; }
                                else { state = "Hold"; }

                                if (configMode == "Keyboard") { xboxAction = defaultXbox[i]; switchAction = defaultSwitch[i]; }
                                else if (configMode == "Xbox") { switchAction = defaultSwitch[i]; keyboardAction = defaultKB[i]; }
                                else if (configMode == "Switch") { xboxAction = defaultXbox[i]; keyboardAction = defaultKB[i]; }

                                buttonConfig[i] = new string[] { keyboardAction, state, boolArray[89], xboxAction, "False", "False", switchAction };
                            }

                            file.WriteLine(string.Join(",", buttonConfig[i]));

                        }

                        file.WriteLine(importedActivationFlags.upper);
                        file.WriteLine(importedActivationFlags.middle);
                        file.WriteLine(importedActivationFlags.lower);
                        file.WriteLine(boolArray[93]);
                        file.WriteLine(boolArray[94]);
                        file.WriteLine(boolArray[95]);
                        if (cmbProfile.SelectedIndex == 0)
                        {
                            file.WriteLine("True");
                        }
                        else { file.WriteLine("False"); }
                        if (configMode == "Xbox") { file.WriteLine("Xbox"); }
                        else if (configMode == "Switch") { file.WriteLine("Switch"); }
                        else { file.WriteLine("Keyboard"); }

                        file.WriteLine("True");
                        file.WriteLine("True");


                        file.WriteLine(boolArray[96]);
                        file.WriteLine(boolArray[97]);
                        file.WriteLine(boolArray[98]);
                        file.WriteLine(boolArray[99]);
                        file.WriteLine(boolArray[100]);
                        file.WriteLine(boolArray[101]);
                        file.WriteLine(boolArray[102]);
                        file.WriteLine(boolArray[103]);
                        file.WriteLine(boolArray[104]);
                        file.WriteLine(boolArray[105]);
                        file.WriteLine(boolArray[106]);
                        file.WriteLine(boolArray[107]);
                        file.WriteLine("True");
                        file.WriteLine("08-27-2025");
                        file.WriteLine(extracted);
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
                int importedProfileIndex = -1;
                for (int i = 0; i < cmbProfile.Items.Count; i++)
                {
                    if (string.Equals(cmbProfile.Items[i].ToString(), profileName, StringComparison.OrdinalIgnoreCase))
                    {
                        importedProfileIndex = i;
                        break;
                    }
                }

                if (importedProfileIndex < 0)
                {
                    cmbProfile.Items.Add(profileName);
                    importedProfileIndex = cmbProfile.Items.Count - 1;
                }

                cmbProfile.SelectedIndex = importedProfileIndex;

                using (StreamWriter file2 = new StreamWriter(new FileStream(parametersFilePath, FileMode.OpenOrCreate)))
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


        }


        private void btnExport_Click(object sender, EventArgs e)
        {
        


            for (int i = 0; i <= cmbProfile.Items.Count; i++)
            {
                if (cmbProfile.SelectedIndex == i && cmbProfile.SelectedIndex > 0) { string profileText3 = cmbProfile.Items[i].ToString(); configName = profileText3 + ".txt"; }
                else if (cmbProfile.SelectedIndex == 0) { configName = "Music.txt"; }
            }

            try
            {

                // Open the file with shared read/write access
                using (FileStream fs = new FileStream(Path.Combine(profilesPath, configName), FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))

                using (StreamWriter file = new StreamWriter(fs))
                {
                    PrepareSettingsForSave();

                    WriteCurrentButtonConfigRows(file);

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
            SettingsCode();
        }


        private void PrepareSettingsForSave()
        {
            for (int i = 0; i < keyboardButtons.Length; i++)
            {
                directions[i] = keyboardButtons[i].Text;
            }

            NormalizeHoverVariableCheckBoxes();
            SyncAdvancedFlagsFromCurrentItems();
        }

        private (bool upper, bool middle, bool lower) GetNormalizedCurrentActivationFlags()
        {
            if (lblActiveLower.CssStyle.Contains("bold", StringComparison.OrdinalIgnoreCase))
            {
                return (false, false, true);
            }

            if (lblActiveMiddle.CssStyle.Contains("bold", StringComparison.OrdinalIgnoreCase))
            {
                return (false, true, false);
            }

            if (lblActiveUpper.CssStyle.Contains("bold", StringComparison.OrdinalIgnoreCase))
            {
                return (true, false, false);
            }


            return (false, false, true);
        }

        private void ApplyLoadedActivationZoneState(bool loadedUpper, bool loadedMiddle, bool loadedLower)
        {
       

            if (!loadedUpper && !loadedMiddle && !loadedLower)
            {
                loadedLower = true;
            }


            if (loadedUpper)
            {
              
                lblActiveLower.CssStyle = "font-weight: normal !important;";
                lblActiveMiddle.CssStyle = "font-weight: normal !important;";
                lblActiveUpper.CssStyle = "font-weight: bold !important;";
            }
            else if (loadedMiddle)
            {
              
                lblActiveLower.CssStyle = "font-weight: normal !important;";
                lblActiveMiddle.CssStyle = "font-weight: bold !important;";
                lblActiveUpper.CssStyle = "font-weight: normal !important;";
            }
            else if (loadedLower)
            {
               
                lblActiveLower.CssStyle = "font-weight: bold !important;";
                lblActiveMiddle.CssStyle = "font-weight: normal !important;";
                lblActiveUpper.CssStyle = "font-weight: normal !important;";
            }
        }

        private static (string upper, string middle, string lower) NormalizeImportedActivationFlags(IReadOnlyList<string> boolArray)
        {
            bool? upper = TryGetImportedBoolean(boolArray, 90);
            bool? middle = TryGetImportedBoolean(boolArray, 91);
            bool? lower = TryGetImportedBoolean(boolArray, 92);

            if (upper.HasValue || middle.HasValue || lower.HasValue)
            {
                return (
                    (upper ?? false).ToString(),
                    (middle ?? false).ToString(),
                    (lower ?? false).ToString());
            }

            return ("False", "False", "True");
        }

        private static bool? TryGetImportedBoolean(IReadOnlyList<string> values, int index)
        {
            if (index < 0 || index >= values.Count)
            {
                return null;
            }

            if (bool.TryParse(values[index], out bool parsed))
            {
                return parsed;
            }

            return null;
        }

        private void SyncAdvancedFlagsFromCurrentItems()
        {
            advFPS = cmbAdvanced.Items.Contains("(Click to Turn Off) Twin Stick Mode");
            advCombine = cmbAdvanced.Items.Contains("(Click to Turn Off) Combine Inputs from Other Systems");
            advDisableClicks = cmbAdvanced.Items.Contains("(Click to Turn Off) Alternative Mouse Controls");
            advAltRTC = cmbAdvanced.Items.Contains("(Click to Turn Off) Alternative Return To Center");
            advMarioKart = cmbAdvanced.Items.Contains("(Click to Turn Off) Mario Kart Mode");
            advVoice = cmbAdvanced.Items.Contains("(Click to Turn Off) Voice Control Mode");
            advKeepMouse = cmbAdvanced.Items.Contains("(Click to Turn On) Keep Mouse Inside Overjoyed");
            advMouseLock = cmbAdvanced.Items.Contains("(Click to Turn On) Prevent Game From Locking Mouse");
            advPSbuttons = cmbAdvanced.Items.Contains("(Click to Turn Off) Press PlayStation Buttons");
        }

        private string BuildAdvancedSettingsCode()
        {
            SyncAdvancedFlagsFromCurrentItems();

            string advanced = "0";

            if (advFPS) { advanced += "1"; }
            if (advCombine) { advanced += "2"; }
            if (advDisableClicks) { advanced += "3"; }
            if (advAltRTC) { advanced += "4"; }
            if (advMarioKart) { advanced += "5"; }
            if (advVoice) { advanced += "6"; }
            if (advKeepMouse) { advanced += "7"; }
            if (advMouseLock) { advanced += "8"; }
            if (advPSbuttons) { advanced += "9"; }
            if (cmbAdvanced.Items.Contains("(Click to Turn Off) Use Click Safe Zone")) { advanced += "A"; }

            return advanced;
        }


        private string GetFriendlyKeyName(Keys keyCode)
        {
            switch (keyCode)
            {
                case Keys.Oem1: return "Semicolon";
                case Keys.Oem2: return "Slash";
                case Keys.Oem3: return "Backtick";
                case Keys.Oem4: return "LBracket";
                case Keys.Oem5: return "Backslash";
                case Keys.Oem6: return "RBracket";
                case Keys.Oem7: return "Quote";
                case Keys.LWin: return "Win";
                case Keys.OemComma: return "Comma";
                case Keys.OemPeriod: return "Period";
                case Keys.OemPlus: return "Equal";
                case Keys.OemMinus: return "Minus";
                case Keys.CapsLock: return "CapsLk";
                case Keys.Escape: return "Esc";
                case Keys.PageUp: return "PageUp";
                case Keys.NumPad0: return "0";
                case Keys.NumPad1: return "1";
                case Keys.NumPad2: return "2";
                case Keys.NumPad3: return "3";
                case Keys.NumPad4: return "4";
                case Keys.NumPad5: return "5";
                case Keys.NumPad6: return "6";
                case Keys.NumPad7: return "7";
                case Keys.NumPad8: return "8";
                case Keys.NumPad9: return "9";
                case Keys.D0: return "0";
                case Keys.D1: return "1";
                case Keys.D2: return "2";
                case Keys.D3: return "3";
                case Keys.D4: return "4";
                case Keys.D5: return "5";
                case Keys.D6: return "6";
                case Keys.D7: return "7";
                case Keys.D8: return "8";
                case Keys.D9: return "9";
                case Keys.Space: return "Space";
                // Add more cases as needed
                default: return keyCode.ToString();
            }
        }

        private void btnAssign_KeyDown(object sender, KeyEventArgs e)
        {


            var ctrl = sender as Control;
            bool leftCtrlPressed = (GetKeyState((int)Keys.LControlKey) & 0x8000) != 0;
            bool rightCtrlPressed = (GetKeyState((int)Keys.RControlKey) & 0x8000) != 0;
            bool leftAltPressed = (GetKeyState((int)Keys.LMenu) & 0x8000) != 0;
            bool rightAltPressed = (GetKeyState((int)Keys.RMenu) & 0x8000) != 0;
            bool leftShiftPressed = (GetKeyState((int)Keys.LShiftKey) & 0x8000) != 0;
            bool rightShiftPressed = (GetKeyState((int)Keys.RShiftKey) & 0x8000) != 0;
            bool enterPressed = (GetKeyState((int)Keys.Enter) & 0x8000) != 0;

            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space || e.KeyCode == Keys.Control || e.KeyCode == Keys.Menu)
            {
                // Prevent the button from being "clicked" when the specified keys are pressed
                e.Handled = true;

            }

            KeysConverter kc = new KeysConverter();


            if (leftCtrlPressed)
            {
                ctrl.Text = "LCtrl";
            }
            else if (enterPressed)
            {
                ctrl.Text = "Enter";
            }
            else if (rightCtrlPressed)
            {
                ctrl.Text = "RCtrl";
            }
            else if (leftAltPressed)
            {
                ctrl.Text = "LAlt";
            }
            else if (rightAltPressed)
            {
                ctrl.Text = "RAlt";
            }
            else if (leftShiftPressed)
            {
                ctrl.Text = "LShift";
            }
            else if (rightShiftPressed)
            {
                ctrl.Text = "RShift";
            }
            else
            {
                ctrl.Text = kc.ConvertToString(GetFriendlyKeyName(e.KeyCode));
            }
            btnFocus.Select();

            if (pnlDeadzone.Enabled == true)
            {
                EnableAllControls(pnlAllSettings, pnlDeadzone);
            }
            else if (pnlQuadrants.Enabled == true)
            {
                EnableAllControls(pnlAllSettings, pnlQuadrants);
            }

            ctrl.BackColor = System.Drawing.Color.FromArgb(0, 167, 209);
        }

        private void renameProfile()
        {
            cmbProfile.Visible = false; cmbProfile.Enabled = false; btnSave.Visible = true; txtConfigName.Enabled = true;
            txtConfigName.Text = cmbProfile.SelectedItem.ToString(); txtConfigName.Visible = true;
            btnCancel.Visible = true;
            btnProfileSettings.Visible = false;
        }
        private void deleteProfile()
        {

            if (cmbProfile.SelectedIndex >= 0) // Ensure an item is selected
            {
                string selectedProfile = cmbProfile.SelectedItem.ToString(); // Get the selected profile
                string profileFilePath = Path.Combine(profilesPath, selectedProfile + ".txt"); // Construct file path

                if (ExtendedMessageBox.Show($"<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 1; -webkit-box-orient: vertical; overflow: hidden; ' title='Are you sure you want to delete the \"{selectedProfile}\" profile?'>Are you sure you want to delete the \"{selectedProfile}\" profile?</span></p>",
                                    "Profile Deletion Verification",
                                    ExtendedMessageBoxLibrary.MessageBoxButtons.YesNo).Result == Wisej.Web.DialogResult.Yes)
                {
                    try
                    {


                        // Remove from combo box
                        cmbProfile.Items.RemoveAt(cmbProfile.SelectedIndex);

                        // Optionally, set the new selected index
                        if (cmbProfile.Items.Count > 0)
                        {
                            cmbProfile.SelectedIndex = 1; // Select the first item
                        }
                        else
                        {
                            cmbProfile.SelectedIndex = -1; // No selection if list is empty
                        }

                        // Delete the file
                        if (File.Exists(profileFilePath))
                        {
                            File.Delete(profileFilePath);
                            Debug.WriteLine($"Profile file deleted: {profileFilePath}");
                        }
                        else
                        {
                            Debug.WriteLine("The profile file does not exist.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"An error occurred: {ex.Message}");
                    }
                }
            }
            else
            {
                ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 1; -webkit-box-orient: vertical; overflow: hidden; ' title='Please select a profile to delete.'>Please select a profile to delete.</span></p>", "No Profile Selected", ExtendedMessageBoxLibrary.MessageBoxButtons.OK);
            }

            for (int i = 1; i <= cmbProfile.Items.Count - 1; i++)
            {
                if (cmbProfile.SelectedIndex == i) { string profileText2 = cmbProfile.Items[i].ToString(); configName = profileText2 + ".txt"; }
            }

            if (cmbProfile.SelectedIndex == 0) { configName = "Music.txt"; }

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
        }
        public static string GetSafeFileName(TextBox textBox)
        {
            // Get the text from the TextBox and trim spaces
            string input = textBox.Text.Trim();

            // Replace invalid characters with underscores
            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char invalidChar in invalidChars)
            {
                input = input.Replace(invalidChar, '_');
            }

            return input;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            btnAdd.Visible = false;
            btnSave.Visible = false; txtConfigName.Visible = false; cmbProfile.Visible = true;
            txtConfigName.Text = ""; btnCancel.Visible = false; cmbProfile.Enabled = true;
            btnProfileSettings.Visible = true;
        }

        private void txtConfigName_KeyDown(object sender, KeyEventArgs e)
        {
            bool fileExists = false;
            try
            {
                // Verify if the profiles directory exists
                if (Directory.Exists(profilesPath))
                {
                    // Get all .txt files in the profiles directory
                    string[] files = Directory.GetFiles(profilesPath, "*.txt");

                    // Check if any file name (without extension) matches the entered text
                    foreach (string file in files)
                    {
                        if (Path.GetFileNameWithoutExtension(file).Equals(txtConfigName.Text.Trim(), StringComparison.OrdinalIgnoreCase))
                        {
                            fileExists = true;
                            break;
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"The directory '{profilesPath}' does not exist.");
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


            // Enable the add button if the file does not exist
            btnAdd.Enabled = false;
            btnSave.Enabled = false;

            if (!fileExists && !string.IsNullOrEmpty(txtConfigName.Text) && !txtConfigName.Text.Equals("Default Layout", StringComparison.OrdinalIgnoreCase))
            {


                btnAdd.Enabled = true;
                btnSave.Enabled = true;
            }
        }

        private void btnProfileSettings_Click(object sender, EventArgs e)
        {
            Eval("(document.doctype ? new XMLSerializer().serializeToString(document.doctype) + '\\n' : '') + document.documentElement.outerHTML", new Action<object>(SaveCurrentHtmlSnapshot));

            string choice = "";
            if (cmbProfile.SelectedIndex < 2)
            {
                choice = CustomMessageBox4.Show(
                           "",
                           "Create / Import", "Share Layout", "Reset Layout", "Restore Layouts"
                       );
            }
            else
            {
                choice = CustomMessageBox6.Show(

               "Create / Import", "Share Layout", "Reset Layout", "Rename Layout", "Delete Layout", "Restore Layouts"
           );
            }

            if (choice == "Create / Import")
            {
                btnImport.PerformClick();
            }

            else if (choice == "Share Layout")
            {
                btnExport.PerformClick();
            }
            else if (choice == "Reset Layout")
            {
                btnDefault.PerformClick();
            }
            else if (choice == "Rename Layout")
            {
                renameProfile();
            }
            else if (choice == "Delete Layout")
            {
                deleteProfile();
            }
            else if (choice == "Restore Layouts")
            {
                backupPanel.Visible = true;  // show


            }

        }



        private void DefaultValues()
        {
            pnlDeadzone.SuspendLayout();
            pnlQuadrants.SuspendLayout();

            NormalizeProfileUiBeforeLoad();


            lblActiveUpper.CssStyle = "font-weight:normal!important";
            lblActiveMiddle.CssStyle = "font-weight:normal!important";
            lblActiveLower.CssStyle = "font-weight:normal!important";

            diagUpRightHover.Checked = false;
            diagDownRightHover.Checked = false;
            diagDownLeftHover.Checked = false;
            diagUpLeftHover.Checked = false;
            diagUpRightLC.Checked = false;
            diagDownRightLC.Checked = false;
            diagDownLeftLC.Checked = false;
            diagUpLeftLC.Checked = false;
            diagUpRightRC.Checked = false;
            diagDownRightRC.Checked = false;
            diagDownLeftRC.Checked = false;
            diagUpLeftRC.Checked = false;

            //string line = File.ReadLines(parametersFilePath).ElementAtOrDefault(2).ToString();

            cmbUpLeftHover.SelectedIndex = 0;
            cmbUpHover.SelectedIndex = 0;
            cmbUpRightHover.SelectedIndex = 0;
            cmbRightHover.SelectedIndex = 0;
            cmbDownRightHover.SelectedIndex = 0;
            cmbDownHover.SelectedIndex = 0;
            cmbDownLeftHover.SelectedIndex = 0;
            cmbLeftHover.SelectedIndex = 0;
            cmbUpLeftLclick.SelectedIndex = 0;
            cmbUpLclick.SelectedIndex = 0;
            cmbUpRightLclick.SelectedIndex = 0;
            cmbRightLclick.SelectedIndex = 0;
            cmbDownRightLclick.SelectedIndex = 0;
            cmbDownLclick.SelectedIndex = 0;
            cmbDownLeftLclick.SelectedIndex = 0;
            cmbLeftLclick.SelectedIndex = 0;
            cmbUpLeftRclick.SelectedIndex = 0;
            cmbUpRclick.SelectedIndex = 0;
            cmbUpRightRclick.SelectedIndex = 0;
            cmbRightRclick.SelectedIndex = 0;
            cmbDownRightRclick.SelectedIndex = 0;
            cmbDownRclick.SelectedIndex = 0;
            cmbDownLeftRclick.SelectedIndex = 0;
            cmbLeftRclick.SelectedIndex = 0;
            cmbDZUpperHover.SelectedIndex = 0;
            cmbDZMiddleHover.SelectedIndex = 0;
            cmbDZLowerHover.SelectedIndex = 0;
            cmbDZUpperLC.SelectedIndex = 0;
            cmbDZMiddleLC.SelectedIndex = 0;
            cmbDZLowerLC.SelectedIndex = 0;
            cmbDZUpperRC.SelectedIndex = 0;
            cmbDZMiddleRC.SelectedIndex = 0;
            cmbDZLowerRC.SelectedIndex = 0;

            //same for switch
            switchUpHover.SelectedIndex = 0;
            switchUpRightHover.SelectedIndex = 0;
            switchRightHover.SelectedIndex = 0;
            switchDownRightHover.SelectedIndex = 0;
            switchDownHover.SelectedIndex = 0;
            switchDownLeftHover.SelectedIndex = 0;
            switchLeftHover.SelectedIndex = 0;
            switchUpLeftHover.SelectedIndex = 0;
            switchUpLC.SelectedIndex = 0;
            switchUpRightLC.SelectedIndex = 0;
            switchRightLC.SelectedIndex = 0;
            switchDownRightLC.SelectedIndex = 0;
            switchDownLC.SelectedIndex = 0;
            switchDownLeftLC.SelectedIndex = 0;
            switchLeftLC.SelectedIndex = 0;
            switchUpLeftLC.SelectedIndex = 0;
            switchUpRC.SelectedIndex = 0;
            switchUpRightRC.SelectedIndex = 0;
            switchRightRC.SelectedIndex = 0;
            switchDownRightRC.SelectedIndex = 0;
            switchDownRC.SelectedIndex = 0;
            switchDownLeftRC.SelectedIndex = 0;
            switchLeftRC.SelectedIndex = 0;
            switchUpLeftRC.SelectedIndex = 0;
            switchDZLowerHover.SelectedIndex = 0;
            switchDZMiddleHover.SelectedIndex = 0;
            switchDZUpperHover.SelectedIndex = 0;
            switchDZLowerLC.SelectedIndex = 0;
            switchDZMiddleLC.SelectedIndex = 0;
            switchDZUpperLC.SelectedIndex = 0;
            switchDZLowerRC.SelectedIndex = 0;
            switchDZMiddleRC.SelectedIndex = 0;
            switchDZUpperRC.SelectedIndex = 0;








            //NEW STUFF YAY!
            int counter = 0;
            bool hasExplicitCombineStates = false;
            bool loadedActivateUpper = false;
            bool loadedActivateMiddle = false;
            bool loadedActivateLower = false;

            if (defaultConfig == true) { configName = "Default.txt"; defaultConfig = false; }
            else if (defaultMusic == true) { configName = "DefaultMusic.txt"; defaultMusic = false; }
            else
            {
                for (int i = 0; i <= cmbProfile.Items.Count; i++)
                {

                    if (cmbProfile.SelectedIndex == i && cmbProfile.SelectedIndex > 0) { string profileText1 = cmbProfile.Items[i].ToString(); configName = profileText1 + ".txt"; }
                    else if (cmbProfile.SelectedIndex == 0) { configName = "Music.txt"; }

                }
            }

            try
            {

                // Open the file with shared read/write access
                using (FileStream fs = new FileStream(Path.Combine(profilesPath, configName), FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader file = new StreamReader(fs))

                {


                    while ((configReader2 = file.ReadLine()) != null)
                    {

                        if (counter == 0)
                        {
                            buttonConfig[counter] = configReader2.Split(',');
                            btnUpHover.Text = buttonConfig[counter][0].Replace("/", "-");
                            cmbUpHover.SelectedItem = buttonConfig[counter][3].Replace("/", "-");
                            switchUpHover.SelectedItem = buttonConfig[counter][6].Replace("/", "-");
                            directions[counter] = buttonConfig[counter][0].Replace("/", "-");

                            if (buttonConfig[counter][1].ToString() == "True")
                            {
                                togUpHover.Checked = true;
                                onceUp.Checked = false;
                            }
                            else if (buttonConfig[counter][1].ToString() == "Once")
                            {
                                onceUp.Checked = true;
                                togUpHover.Checked = false;
                            }
                            else { togUpHover.Checked = false; onceUp.Checked = false; }


                            disUpHover.Checked = bool.Parse(buttonConfig[counter][2]);
                            varUpHover.Checked = bool.Parse(buttonConfig[counter][5]);


                            if (disUpHover.Checked == true)
                            {
                                disableUpHover.RemoveCssClass("disableButtonOff"); disableUpHover.AddCssClass("disableButtonOn");
                                disableUpHover.PerformClick();

                            }
                            else { disableUpHover.ToolTipText = "Click to Disable"; btnUpHover.Enabled = true; cmbUpHover.Enabled = true; switchUpHover.Enabled = true; }


                        }
                        else if (counter == 1)
                        {
                            buttonConfig[counter] = configReader2.Split(',');
                            btnUpRightHover.Text = buttonConfig[counter][0].Replace("/", "-");
                            cmbUpRightHover.SelectedItem = buttonConfig[counter][3].Replace("/", "-");
                            switchUpRightHover.SelectedItem = buttonConfig[counter][6].Replace("/", "-");
                            directions[counter] = buttonConfig[counter][0].Replace("/", "-");


                            if (buttonConfig[counter][1].ToString() == "True")
                            {
                                togUpRightHover.Checked = true;
                                onceUpRight.Checked = false;
                            }
                            else if (buttonConfig[counter][1].ToString() == "Once")
                            {
                                onceUpRight.Checked = true;
                                togUpRightHover.Checked = false;
                            }
                            else { togUpRightHover.Checked = false; onceUpRight.Checked = false; }


                            disUpRightHover.Checked = bool.Parse(buttonConfig[counter][2]);
                            varUpRightHover.Checked = bool.Parse(buttonConfig[counter][5]);

                            if (disUpRightHover.Checked == true)
                            {
                                disableUpRightHover.RemoveCssClass("disableButtonOff"); disableUpRightHover.AddCssClass("disableButtonOn");
                                disableUpRightHover.PerformClick();

                            }
                            else if (diagUpRightHover.Checked == false) { disableUpRightHover.ToolTipText = "Click to Disable"; btnUpRightHover.Enabled = true; cmbUpRightHover.Enabled = true; switchUpRightHover.Enabled = true; }
                            else { disableUpRightHover.ToolTipText = "Click to Disable"; }

                        }
                        else if (counter == 2)
                        {
                            buttonConfig[counter] = configReader2.Split(',');
                            btnRightHover.Text = buttonConfig[counter][0].Replace("/", "-");
                            cmbRightHover.SelectedItem = buttonConfig[counter][3].Replace("/", "-");
                            switchRightHover.SelectedItem = buttonConfig[counter][6].Replace("/", "-");
                            directions[counter] = buttonConfig[counter][0].Replace("/", "-");

                            if (buttonConfig[counter][1].ToString() == "True")
                            {
                                togRightHover.Checked = true;
                                onceRight.Checked = false;
                            }
                            else if (buttonConfig[counter][1].ToString() == "Once")
                            {
                                onceRight.Checked = true;
                                togRightHover.Checked = false;
                            }
                            else { togRightHover.Checked = false; onceRight.Checked = false; }
                            disRightHover.Checked = bool.Parse(buttonConfig[counter][2]);
                            varRightHover.Checked = bool.Parse(buttonConfig[counter][5]);

                            if (disRightHover.Checked == true)
                            {
                                disableRightHover.RemoveCssClass("disableButtonOff"); disableRightHover.AddCssClass("disableButtonOn");
                                disableRightHover.PerformClick();

                            }
                            else { disableRightHover.ToolTipText = "Click to Disable"; btnRightHover.Enabled = true; cmbRightHover.Enabled = true; switchRightHover.Enabled = true; }
                        }
                        else if (counter == 3)
                        {
                            buttonConfig[counter] = configReader2.Split(',');
                            btnDownRightHover.Text = buttonConfig[counter][0].Replace("/", "-");
                            cmbDownRightHover.SelectedItem = buttonConfig[counter][3].Replace("/", "-");
                            switchDownRightHover.SelectedItem = buttonConfig[counter][6].Replace("/", "-");
                            directions[counter] = buttonConfig[counter][0].Replace("/", "-");

                            if (buttonConfig[counter][1].ToString() == "True")
                            {
                                togDownRightHover.Checked = true;
                                onceDownRight.Checked = false;
                            }
                            else if (buttonConfig[counter][1].ToString() == "Once")
                            {
                                onceDownRight.Checked = true;
                                togDownRightHover.Checked = false;
                            }
                            else { togDownRightHover.Checked = false; onceDownRight.Checked = false; }
                            disDownRightHover.Checked = bool.Parse(buttonConfig[counter][2]);
                            varDownRightHover.Checked = bool.Parse(buttonConfig[counter][5]);

                            if (disDownRightHover.Checked == true)
                            {
                                disableDownRightHover.RemoveCssClass("disableButtonOff"); disableDownRightHover.AddCssClass("disableButtonOn");
                                disableDownRightHover.PerformClick();

                            }
                            else if (diagDownRightHover.Checked == false) { disableDownRightHover.ToolTipText = "Click to Disable"; btnDownRightHover.Enabled = true; cmbDownRightHover.Enabled = true; switchDownRightHover.Enabled = true; }
                            else { disableDownRightHover.ToolTipText = "Click to Disable"; }
                        }
                        else if (counter == 4)
                        {
                            buttonConfig[counter] = configReader2.Split(',');
                            btnDownHover.Text = buttonConfig[counter][0].Replace("/", "-");
                            cmbDownHover.SelectedItem = buttonConfig[counter][3].Replace("/", "-");
                            switchDownHover.SelectedItem = buttonConfig[counter][6].Replace("/", "-");
                            directions[counter] = buttonConfig[counter][0].Replace("/", "-");

                            if (buttonConfig[counter][1].ToString() == "True")
                            {
                                togDownHover.Checked = true;
                                onceDown.Checked = false;
                            }
                            else if (buttonConfig[counter][1].ToString() == "Once")
                            {
                                onceDown.Checked = true;
                                togDownHover.Checked = false;
                            }
                            else { togDownHover.Checked = false; onceDown.Checked = false; }
                            disDownHover.Checked = bool.Parse(buttonConfig[counter][2]);
                            varDownHover.Checked = bool.Parse(buttonConfig[counter][5]);

                            if (disDownHover.Checked == true)
                            {
                                disableDownHover.RemoveCssClass("disableButtonOff"); disableDownHover.AddCssClass("disableButtonOn");
                                disableDownHover.PerformClick();

                            }
                            else { disableDownHover.ToolTipText = "Click to Disable"; btnDownHover.Enabled = true; cmbDownHover.Enabled = true; switchDownHover.Enabled = true; }
                        }
                        else if (counter == 5)
                        {
                            buttonConfig[counter] = configReader2.Split(',');
                            btnDownLeftHover.Text = buttonConfig[counter][0].Replace("/", "-");
                            cmbDownLeftHover.SelectedItem = buttonConfig[counter][3].Replace("/", "-");
                            switchDownLeftHover.SelectedItem = buttonConfig[counter][6].Replace("/", "-");
                            directions[counter] = buttonConfig[counter][0].Replace("/", "-");

                            if (buttonConfig[counter][1].ToString() == "True")
                            {
                                togDownLeftHover.Checked = true;
                                onceDownLeft.Checked = false;
                            }
                            else if (buttonConfig[counter][1].ToString() == "Once")
                            {
                                onceDownLeft.Checked = true;
                                togDownLeftHover.Checked = false;
                            }
                            else { togDownLeftHover.Checked = false; onceDownLeft.Checked = false; }
                            disDownLeftHover.Checked = bool.Parse(buttonConfig[counter][2]);
                            varDownLeftHover.Checked = bool.Parse(buttonConfig[counter][5]);

                            if (disDownLeftHover.Checked == true)
                            {
                                disableDownLeftHover.RemoveCssClass("disableButtonOff"); disableDownLeftHover.AddCssClass("disableButtonOn");
                                disableDownLeftHover.PerformClick();

                            }
                            else if (diagDownLeftHover.Checked == false) { disableDownLeftHover.ToolTipText = "Click to Disable"; btnDownLeftHover.Enabled = true; cmbDownLeftHover.Enabled = true; switchDownLeftHover.Enabled = true; }
                            else { disableDownLeftHover.ToolTipText = "Click to Disable"; }
                        }
                        else if (counter == 6)
                        {
                            buttonConfig[counter] = configReader2.Split(',');
                            btnLeftHover.Text = buttonConfig[counter][0].Replace("/", "-");
                            cmbLeftHover.SelectedItem = buttonConfig[counter][3].Replace("/", "-");
                            switchLeftHover.SelectedItem = buttonConfig[counter][6].Replace("/", "-");
                            directions[counter] = buttonConfig[counter][0].Replace("/", "-");

                            if (buttonConfig[counter][1].ToString() == "True")
                            {
                                togLeftHover.Checked = true;
                                onceLeft.Checked = false;
                            }
                            else if (buttonConfig[counter][1].ToString() == "Once")
                            {
                                onceLeft.Checked = true;
                                togLeftHover.Checked = false;
                            }
                            else { togLeftHover.Checked = false; onceLeft.Checked = false; }
                            disLeftHover.Checked = bool.Parse(buttonConfig[counter][2]);
                            varLeftHover.Checked = bool.Parse(buttonConfig[counter][5]);

                            if (disLeftHover.Checked == true)
                            {
                                disableLeftHover.RemoveCssClass("disableButtonOff"); disableLeftHover.AddCssClass("disableButtonOn");
                                disableLeftHover.PerformClick();

                            }
                            else { disableLeftHover.ToolTipText = "Click to Disable"; btnLeftHover.Enabled = true; cmbLeftHover.Enabled = true; switchLeftHover.Enabled = true; }
                        }
                        else if (counter == 7)
                        {
                            buttonConfig[counter] = configReader2.Split(',');
                            btnUpLeftHover.Text = buttonConfig[counter][0].Replace("/", "-");
                            cmbUpLeftHover.SelectedItem = buttonConfig[counter][3].Replace("/", "-");
                            switchUpLeftHover.SelectedItem = buttonConfig[counter][6].Replace("/", "-");
                            directions[counter] = buttonConfig[counter][0].Replace("/", "-");

                            if (buttonConfig[counter][1].ToString() == "True")
                            {
                                togUpLeftHover.Checked = true;
                                onceUpLeft.Checked = false;
                            }
                            else if (buttonConfig[counter][1].ToString() == "Once")
                            {
                                onceUpLeft.Checked = true;
                                togUpLeftHover.Checked = false;
                            }
                            else { togUpLeftHover.Checked = false; onceUpLeft.Checked = false; }
                            disUpLeftHover.Checked = bool.Parse(buttonConfig[counter][2]);
                            varUpLeftHover.Checked = bool.Parse(buttonConfig[counter][5]);

                            if (disUpLeftHover.Checked == true)
                            {
                                disableUpLeftHover.RemoveCssClass("disableButtonOff"); disableUpLeftHover.AddCssClass("disableButtonOn");
                                disableUpLeftHover.PerformClick();

                            }
                            else if (diagUpLeftHover.Checked == false) { disableUpLeftHover.ToolTipText = "Click to Disable"; btnUpLeftHover.Enabled = true; cmbUpLeftHover.Enabled = true; switchUpLeftHover.Enabled = true; }
                            else { disableUpLeftHover.ToolTipText = "Click to Disable"; }
                        }
                        else if (counter == 8)
                        {
                            buttonConfig[counter] = configReader2.Split(',');
                            btnUpLclick.Text = buttonConfig[counter][0].Replace("/", "-");
                            cmbUpLclick.SelectedItem = buttonConfig[counter][3].Replace("/", "-");
                            switchUpLC.SelectedItem = buttonConfig[counter][6].Replace("/", "-");
                            directions[counter] = buttonConfig[counter][0].Replace("/", "-");
                            if (buttonConfig[counter][1].ToString() == "True")
                            {
                                togUpLC.Checked = true;
                                onceUpLC.Checked = false;
                                holdUpLC.Checked = false;
                            }
                            else if (buttonConfig[counter][1].ToString() == "Once")
                            {
                                onceUpLC.Checked = true;
                                togUpLC.Checked = false;
                                holdUpLC.Checked = false;
                            }
                            else { togUpLC.Checked = false; onceUpLC.Checked = false; holdUpLC.Checked = true; }


                            disUpLC.Checked = bool.Parse(buttonConfig[counter][2]);
                            rtcUpLC.Checked = bool.Parse(buttonConfig[counter][4]);

                            if (disUpLC.Checked == true)
                            {
                                disableUpLC.RemoveCssClass("disableButtonOff"); disableUpLC.AddCssClass("disableButtonOn");

                                disableUpLC.PerformClick();
                            }
                            else { disableUpLC.ToolTipText = "Click to Disable"; btnUpLclick.Enabled = true; cmbUpLclick.Enabled = true; switchUpLC.Enabled = true; }
                        }
                        else if (counter == 9)
                        {
                            buttonConfig[counter] = configReader2.Split(',');
                            btnUpRightLclick.Text = buttonConfig[counter][0].Replace("/", "-");
                            cmbUpRightLclick.SelectedItem = buttonConfig[counter][3].Replace("/", "-");
                            switchUpRightLC.SelectedItem = buttonConfig[counter][6].Replace("/", "-");
                            directions[counter] = buttonConfig[counter][0].Replace("/", "-");


                            if (buttonConfig[counter][1].ToString() == "True")
                            {

                                togUpRightLC.Checked = true;
                                onceUpRightLC.Checked = false;
                                holdUpRightLC.Checked = false;
                            }
                            else if (buttonConfig[counter][1].ToString() == "Once")
                            {

                                onceUpRightLC.Checked = true;
                                togUpRightLC.Checked = false;
                                holdUpRightLC.Checked = false;
                            }
                            else
                            {
                                togUpRightLC.Checked = false; onceUpRightLC.Checked = false; holdUpRightLC.Checked = true;
                            }


                            disUpRightLC.Checked = bool.Parse(buttonConfig[counter][2]);
                            rtcUpRightLC.Checked = bool.Parse(buttonConfig[counter][4]);

                            if (disUpRightLC.Checked == true)
                            {
                                disableUpRightLC.RemoveCssClass("disableButtonOff"); disableUpRightLC.AddCssClass("disableButtonOn");
                                disableUpRightLC.PerformClick();

                            }
                            else if (diagUpRightLC.Checked == false) { disableUpRightLC.ToolTipText = "Click to Disable"; btnUpRightLclick.Enabled = true; cmbUpRightLclick.Enabled = true; switchUpRightLC.Enabled = true; }
                            else { disableUpRightLC.ToolTipText = "Click to Disable"; }
                        }
                        else if (counter == 10)
                        {
                            buttonConfig[counter] = configReader2.Split(',');
                            btnRightLclick.Text = buttonConfig[counter][0].Replace("/", "-");
                            cmbRightLclick.SelectedItem = buttonConfig[counter][3].Replace("/", "-");
                            switchRightLC.SelectedItem = buttonConfig[counter][6].Replace("/", "-");
                            directions[counter] = buttonConfig[counter][0].Replace("/", "-");


                            if (buttonConfig[counter][1].ToString() == "True")
                            {
                                togRightLC.Checked = true;
                                holdRightLC.Checked = false;
                                onceRightLC.Checked = false;
                            }
                            else if (buttonConfig[counter][1].ToString() == "Once")
                            {
                                onceRightLC.Checked = true;
                                togRightLC.Checked = false;
                                holdRightLC.Checked = false;
                            }
                            else { holdRightLC.Checked = true; togRightLC.Checked = false; onceRightLC.Checked = false; }
                            disRightLC.Checked = bool.Parse(buttonConfig[counter][2]);
                            rtcRightLC.Checked = bool.Parse(buttonConfig[counter][4]);

                            if (disRightLC.Checked == true)
                            {
                                disableRightLC.RemoveCssClass("disableButtonOff"); disableRightLC.AddCssClass("disableButtonOn");
                                disableRightLC.PerformClick();

                            }
                            else { disableRightLC.ToolTipText = "Click to Disable"; btnRightLclick.Enabled = true; cmbRightLclick.Enabled = true; switchRightLC.Enabled = true; }
                        }
                        else if (counter == 11)
                        {
                            buttonConfig[counter] = configReader2.Split(',');
                            btnDownRightLclick.Text = buttonConfig[counter][0].Replace("/", "-");
                            cmbDownRightLclick.SelectedItem = buttonConfig[counter][3].Replace("/", "-");
                            switchDownRightLC.SelectedItem = buttonConfig[counter][6].Replace("/", "-");
                            directions[counter] = buttonConfig[counter][0].Replace("/", "-");


                            if (buttonConfig[counter][1].ToString() == "True")
                            {
                                togDownRightLC.Checked = true;
                                holdDownRightLC.Checked = false;
                                onceDownRightLC.Checked = false;
                            }
                            else if (buttonConfig[counter][1].ToString() == "Once")
                            {
                                onceDownRightLC.Checked = true;
                                togDownRightLC.Checked = false;
                                holdDownRightLC.Checked = false;
                            }

                            else { holdDownRightLC.Checked = true; togDownRightLC.Checked = false; onceDownRightLC.Checked = false; }
                            disDownRightLC.Checked = bool.Parse(buttonConfig[counter][2]);
                            rtcDownRightLC.Checked = bool.Parse(buttonConfig[counter][4]);

                            if (disDownRightLC.Checked == true)
                            {
                                disableDownRightLC.RemoveCssClass("disableButtonOff"); disableDownRightLC.AddCssClass("disableButtonOn");
                                disableDownRightLC.PerformClick();

                            }
                            else if (diagDownRightLC.Checked == false) { disableDownRightLC.ToolTipText = "Click to Disable"; btnDownRightLclick.Enabled = true; cmbDownRightLclick.Enabled = true; switchDownRightLC.Enabled = true; }
                            else { disableDownRightLC.ToolTipText = "Click to Disable"; }
                        }
                        else if (counter == 12)
                        {
                            buttonConfig[counter] = configReader2.Split(',');
                            btnDownLclick.Text = buttonConfig[counter][0].Replace("/", "-");
                            cmbDownLclick.SelectedItem = buttonConfig[counter][3].Replace("/", "-");
                            switchDownLC.SelectedItem = buttonConfig[counter][6].Replace("/", "-");
                            directions[counter] = buttonConfig[counter][0].Replace("/", "-");

                            if (buttonConfig[counter][1].ToString() == "True")
                            {
                                togDownLC.Checked = true;
                                holdDownLC.Checked = false;
                                onceDownLC.Checked = false;
                            }
                            else if (buttonConfig[counter][1].ToString() == "Once")
                            {
                                onceDownLC.Checked = true;
                                togDownLC.Checked = false;
                                holdDownLC.Checked = false;
                            }
                            else { holdDownLC.Checked = true; togDownLC.Checked = false; onceDownLC.Checked = false; }
                            disDownLC.Checked = bool.Parse(buttonConfig[counter][2]);
                            rtcDownLC.Checked = bool.Parse(buttonConfig[counter][4]);

                            if (disDownLC.Checked == true)
                            {
                                disableDownLC.RemoveCssClass("disableButtonOff"); disableDownLC.AddCssClass("disableButtonOn");
                                disableDownLC.PerformClick();

                            }
                            else { disableDownLC.ToolTipText = "Click to Disable"; btnDownLclick.Enabled = true; cmbDownLclick.Enabled = true; switchDownLC.Enabled = true; }
                        }
                        else if (counter == 13)
                        {
                            buttonConfig[counter] = configReader2.Split(',');
                            btnDownLeftLclick.Text = buttonConfig[counter][0].Replace("/", "-");
                            cmbDownLeftLclick.SelectedItem = buttonConfig[counter][3].Replace("/", "-");
                            switchDownLeftLC.SelectedItem = buttonConfig[counter][6].Replace("/", "-");
                            directions[counter] = buttonConfig[counter][0].Replace("/", "-");

                            if (buttonConfig[counter][1].ToString() == "True")
                            {
                                togDownLeftLC.Checked = true;
                                holdDownLeftLC.Checked = false;
                                onceDownLeftLC.Checked = false;
                            }
                            else if (buttonConfig[counter][1].ToString() == "Once")
                            {
                                onceDownLeftLC.Checked = true;
                                togDownLeftLC.Checked = false;
                                holdDownLeftLC.Checked = false;
                            }
                            else { holdDownLeftLC.Checked = true; togDownLeftLC.Checked = false; onceDownLeftLC.Checked = false; }
                            disDownLeftLC.Checked = bool.Parse(buttonConfig[counter][2]);
                            rtcDownLeftLC.Checked = bool.Parse(buttonConfig[counter][4]);

                            if (disDownLeftLC.Checked == true)
                            {
                                disableDownLeftLC.RemoveCssClass("disableButtonOff"); disableDownLeftLC.AddCssClass("disableButtonOn");
                                disableDownLeftLC.PerformClick();

                            }
                            else if (diagDownLeftLC.Checked == false) { disableDownLeftLC.ToolTipText = "Click to Disable"; btnDownLeftLclick.Enabled = true; cmbDownLeftLclick.Enabled = true; switchDownLeftLC.Enabled = true; }
                            else { disableDownLeftLC.ToolTipText = "Click to Disable"; }
                        }
                        else if (counter == 14)
                        {
                            buttonConfig[counter] = configReader2.Split(',');
                            btnLeftLclick.Text = buttonConfig[counter][0].Replace("/", "-");
                            cmbLeftLclick.SelectedItem = buttonConfig[counter][3].Replace("/", "-");
                            switchLeftLC.SelectedItem = buttonConfig[counter][6].Replace("/", "-");
                            directions[counter] = buttonConfig[counter][0].Replace("/", "-");

                            if (buttonConfig[counter][1].ToString() == "True")
                            {
                                togLeftLC.Checked = true;
                                holdLeftLC.Checked = false;
                                onceLeftLC.Checked = false;
                            }
                            else if (buttonConfig[counter][1].ToString() == "Once")
                            {
                                onceLeftLC.Checked = true;
                                togLeftLC.Checked = false;
                                holdLeftLC.Checked = false;
                            }
                            else { holdLeftLC.Checked = true; togLeftLC.Checked = false; onceLeftLC.Checked = false; }
                            disLeftLC.Checked = bool.Parse(buttonConfig[counter][2]);
                            rtcLeftLC.Checked = bool.Parse(buttonConfig[counter][4]);

                            if (disLeftLC.Checked == true)
                            {
                                disableLeftLC.RemoveCssClass("disableButtonOff"); disableLeftLC.AddCssClass("disableButtonOn");
                                disableLeftLC.PerformClick();

                            }
                            else { disableLeftLC.ToolTipText = "Click to Disable"; btnLeftLclick.Enabled = true; cmbLeftLclick.Enabled = true; switchLeftLC.Enabled = true; }
                        }
                        else if (counter == 15)
                        {
                            buttonConfig[counter] = configReader2.Split(',');
                            btnUpLeftLclick.Text = buttonConfig[counter][0].Replace("/", "-");
                            cmbUpLeftLclick.SelectedItem = buttonConfig[counter][3].Replace("/", "-");
                            switchUpLeftLC.SelectedItem = buttonConfig[counter][6].Replace("/", "-");
                            directions[counter] = buttonConfig[counter][0].Replace("/", "-");


                            if (buttonConfig[counter][1].ToString() == "True")
                            {
                                togUpLeftLC.Checked = true;
                                holdUpLeftLC.Checked = false;
                                onceUpLeftLC.Checked = false;
                            }
                            else if (buttonConfig[counter][1].ToString() == "Once")
                            {
                                onceUpLeftLC.Checked = true;
                                togUpLeftLC.Checked = false;
                                holdUpLeftLC.Checked = false;
                            }
                            else { holdUpLeftLC.Checked = true; togUpLeftLC.Checked = false; onceUpLeftLC.Checked = false; }
                            disUpLeftLC.Checked = bool.Parse(buttonConfig[counter][2]);
                            rtcUpLeftLC.Checked = bool.Parse(buttonConfig[counter][4]);

                            if (disUpLeftLC.Checked == true)
                            {
                                disableUpLeftLC.RemoveCssClass("disableButtonOff"); disableUpLeftLC.AddCssClass("disableButtonOn");
                                disableUpLeftLC.PerformClick();

                            }
                            else if (diagUpLeftLC.Checked == false) { disableUpLeftLC.ToolTipText = "Click to Disable"; btnUpLeftLclick.Enabled = true; cmbUpLeftLclick.Enabled = true; switchUpLeftLC.Enabled = true; }
                            else { disableUpLeftLC.ToolTipText = "Click to Disable"; }
                        }
                        else if (counter == 16)
                        {
                            buttonConfig[counter] = configReader2.Split(',');
                            btnUpRclick.Text = buttonConfig[counter][0].Replace("/", "-");
                            cmbUpRclick.SelectedItem = buttonConfig[counter][3].Replace("/", "-");
                            switchUpRC.SelectedItem = buttonConfig[counter][6].Replace("/", "-");
                            directions[counter] = buttonConfig[counter][0].Replace("/", "-");


                            if (buttonConfig[counter][1].ToString() == "True")
                            {
                                togUpRC.Checked = true;
                                holdUpRC.Checked = false;
                                onceUpRC.Checked = false;
                            }
                            else if (buttonConfig[counter][1].ToString() == "Once")
                            {
                                onceUpRC.Checked = true;
                                togUpRC.Checked = false;
                                holdUpRC.Checked = false;
                            }

                            else { holdUpRC.Checked = true; togUpRC.Checked = false; onceUpRC.Checked = false; }
                            disUpRC.Checked = bool.Parse(buttonConfig[counter][2]);
                            rtcUpRC.Checked = bool.Parse(buttonConfig[counter][4]);

                            if (disUpRC.Checked == true)
                            {
                                disableUpRC.RemoveCssClass("disableButtonOff"); disableUpRC.AddCssClass("disableButtonOn");
                                disableUpRC.PerformClick();

                            }
                            else { disableUpRC.ToolTipText = "Click to Disable"; btnUpRclick.Enabled = true; cmbUpRclick.Enabled = true; switchUpRC.Enabled = true; }
                        }
                        else if (counter == 17)
                        {
                            buttonConfig[counter] = configReader2.Split(',');
                            btnUpRightRclick.Text = buttonConfig[counter][0].Replace("/", "-");
                            cmbUpRightRclick.SelectedItem = buttonConfig[counter][3].Replace("/", "-");
                            switchUpRightRC.SelectedItem = buttonConfig[counter][6].Replace("/", "-");
                            directions[counter] = buttonConfig[counter][0].Replace("/", "-");

                            if (buttonConfig[counter][1].ToString() == "True")
                            {
                                togUpRightRC.Checked = true;
                                holdUpRightRC.Checked = false;
                                onceUpRightRC.Checked = false;
                            }
                            else if (buttonConfig[counter][1].ToString() == "Once")
                            {
                                onceUpRightRC.Checked = true;
                                togUpRightRC.Checked = false;
                                holdUpRightRC.Checked = false;
                            }
                            else { holdUpRightRC.Checked = true; togUpRightRC.Checked = false; onceUpRightRC.Checked = false; }
                            disUpRightRC.Checked = bool.Parse(buttonConfig[counter][2]);
                            rtcUpRightRC.Checked = bool.Parse(buttonConfig[counter][4]);

                            if (disUpRightRC.Checked == true)
                            {
                                disableUpRightRC.RemoveCssClass("disableButtonOff"); disableUpRightRC.AddCssClass("disableButtonOn");
                                disableUpRightRC.PerformClick();

                            }
                            else if (diagUpRightRC.Checked == false) { disableUpRightRC.ToolTipText = "Click to Disable"; btnUpRightRclick.Enabled = true; cmbUpRightRclick.Enabled = true; switchUpRightRC.Enabled = true; }
                            else { disableUpRightRC.ToolTipText = "Click to Disable"; }
                        }
                        else if (counter == 18)
                        {
                            buttonConfig[counter] = configReader2.Split(',');
                            btnRightRclick.Text = buttonConfig[counter][0].Replace("/", "-");
                            cmbRightRclick.SelectedItem = buttonConfig[counter][3].Replace("/", "-");
                            switchRightRC.SelectedItem = buttonConfig[counter][6].Replace("/", "-");
                            directions[counter] = buttonConfig[counter][0].Replace("/", "-");

                            if (buttonConfig[counter][1].ToString() == "True")
                            {
                                togRightRC.Checked = true;
                                holdRightRC.Checked = false;
                                onceRightRC.Checked = false;
                            }
                            else if (buttonConfig[counter][1].ToString() == "Once")
                            {
                                onceRightRC.Checked = true;
                                togRightRC.Checked = false;
                                holdRightRC.Checked = false;
                            }

                            else { holdRightRC.Checked = true; togRightRC.Checked = false; onceRightRC.Checked = false; }
                            disRightRC.Checked = bool.Parse(buttonConfig[counter][2]);
                            rtcRightRC.Checked = bool.Parse(buttonConfig[counter][4]);

                            if (disRightRC.Checked == true)
                            {
                                disableRightRC.RemoveCssClass("disableButtonOff"); disableRightRC.AddCssClass("disableButtonOn");
                                disableRightRC.PerformClick();

                            }
                            else { disableRightRC.ToolTipText = "Click to Disable"; btnRightRclick.Enabled = true; cmbRightRclick.Enabled = true; switchRightRC.Enabled = true; }
                        }
                        else if (counter == 19)
                        {
                            buttonConfig[counter] = configReader2.Split(',');
                            btnDownRightRclick.Text = buttonConfig[counter][0].Replace("/", "-");
                            cmbDownRightRclick.SelectedItem = buttonConfig[counter][3].Replace("/", "-");
                            switchDownRightRC.SelectedItem = buttonConfig[counter][6].Replace("/", "-");
                            directions[counter] = buttonConfig[counter][0].Replace("/", "-");

                            if (buttonConfig[counter][1].ToString() == "True")
                            {
                                togDownRightRC.Checked = true;
                                holdDownRightRC.Checked = false;
                                onceDownRightRC.Checked = false;
                            }
                            else if (buttonConfig[counter][1].ToString() == "Once")
                            {
                                onceDownRightRC.Checked = true;
                                togDownRightRC.Checked = false;
                                holdDownRightRC.Checked = false;
                            }
                            else { holdDownRightRC.Checked = true; togDownRightRC.Checked = false; onceDownRightRC.Checked = false; }
                            disDownRightRC.Checked = bool.Parse(buttonConfig[counter][2]);
                            rtcDownRightRC.Checked = bool.Parse(buttonConfig[counter][4]);

                            if (disDownRightRC.Checked == true)
                            {
                                disableDownRightRC.RemoveCssClass("disableButtonOff"); disableDownRightRC.AddCssClass("disableButtonOn");
                                disableDownRightRC.PerformClick();

                            }
                            else if (diagDownRightRC.Checked == false) { disableDownRightRC.ToolTipText = "Click to Disable"; btnDownRightRclick.Enabled = true; cmbDownRightRclick.Enabled = true; switchDownRightRC.Enabled = true; }
                            else { disableDownRightRC.ToolTipText = "Click to Disable"; }
                        }
                        else if (counter == 20)
                        {
                            buttonConfig[counter] = configReader2.Split(',');
                            btnDownRclick.Text = buttonConfig[counter][0].Replace("/", "-");
                            cmbDownRclick.SelectedItem = buttonConfig[counter][3].Replace("/", "-");
                            switchDownRC.SelectedItem = buttonConfig[counter][6].Replace("/", "-");
                            directions[counter] = buttonConfig[counter][0].Replace("/", "-");


                            if (buttonConfig[counter][1].ToString() == "True")
                            {
                                togDownRC.Checked = true;
                                holdDownRC.Checked = false;
                                onceDownRC.Checked = false;
                            }
                            else if (buttonConfig[counter][1].ToString() == "Once")
                            {
                                onceDownRC.Checked = true;
                                togDownRC.Checked = false;
                                holdDownRC.Checked = false;
                            }
                            else { holdDownRC.Checked = true; togDownRC.Checked = false; onceDownRC.Checked = false; }
                            disDownRC.Checked = bool.Parse(buttonConfig[counter][2]);
                            rtcDownRC.Checked = bool.Parse(buttonConfig[counter][4]);

                            if (disDownRC.Checked == true)
                            {
                                disableDownRC.RemoveCssClass("disableButtonOff"); disableDownRC.AddCssClass("disableButtonOn");
                                disableDownRC.PerformClick();

                            }
                            else { disableDownRC.ToolTipText = "Click to Disable"; btnDownRclick.Enabled = true; cmbDownRclick.Enabled = true; switchDownRC.Enabled = true; }
                        }
                        else if (counter == 21)
                        {
                            buttonConfig[counter] = configReader2.Split(',');
                            btnDownLeftRclick.Text = buttonConfig[counter][0].Replace("/", "-");
                            cmbDownLeftRclick.SelectedItem = buttonConfig[counter][3].Replace("/", "-");
                            switchDownLeftRC.SelectedItem = buttonConfig[counter][6].Replace("/", "-");
                            directions[counter] = buttonConfig[counter][0].Replace("/", "-");

                            if (buttonConfig[counter][1].ToString() == "True")
                            {
                                togDownLeftRC.Checked = true;
                                holdDownLeftRC.Checked = false;
                                onceDownLeftRC.Checked = false;
                            }
                            else if (buttonConfig[counter][1].ToString() == "Once")
                            {
                                onceDownLeftRC.Checked = true;
                                togDownLeftRC.Checked = false;
                                holdDownLeftRC.Checked = false;
                            }
                            else { holdDownLeftRC.Checked = true; togDownLeftRC.Checked = false; onceDownLeftRC.Checked = false; }
                            disDownLeftRC.Checked = bool.Parse(buttonConfig[counter][2]);
                            rtcDownLeftRC.Checked = bool.Parse(buttonConfig[counter][4]);

                            if (disDownLeftRC.Checked == true)
                            {
                                disableDownLeftRC.RemoveCssClass("disableButtonOff"); disableDownLeftRC.AddCssClass("disableButtonOn");
                                disableDownLeftRC.PerformClick();

                            }
                            else if (diagDownLeftRC.Checked == false) { disableDownLeftRC.ToolTipText = "Click to Disable"; btnDownLeftRclick.Enabled = true; cmbDownLeftRclick.Enabled = true; switchDownLeftRC.Enabled = true; }
                            else { disableDownLeftRC.ToolTipText = "Click to Disable"; }
                        }
                        else if (counter == 22)
                        {
                            buttonConfig[counter] = configReader2.Split(',');
                            btnLeftRclick.Text = buttonConfig[counter][0].Replace("/", "-");
                            cmbLeftRclick.SelectedItem = buttonConfig[counter][3].Replace("/", "-");
                            switchLeftRC.SelectedItem = buttonConfig[counter][6].Replace("/", "-");
                            directions[counter] = buttonConfig[counter][0].Replace("/", "-");

                            if (buttonConfig[counter][1].ToString() == "True")
                            {
                                togLeftRC.Checked = true;
                                holdLeftRC.Checked = false;
                                onceLeftRC.Checked = false;
                            }
                            else if (buttonConfig[counter][1].ToString() == "Once")
                            {
                                onceLeftRC.Checked = true;
                                togLeftRC.Checked = false;
                                holdLeftRC.Checked = false;
                            }
                            else { holdLeftRC.Checked = true; togLeftRC.Checked = false; onceLeftRC.Checked = false; }
                            disLeftRC.Checked = bool.Parse(buttonConfig[counter][2]);
                            rtcLeftRC.Checked = bool.Parse(buttonConfig[counter][4]);

                            if (disLeftRC.Checked == true)
                            {
                                disableLeftRC.RemoveCssClass("disableButtonOff"); disableLeftRC.AddCssClass("disableButtonOn");
                                disableLeftRC.PerformClick();

                            }
                            else { disableLeftRC.ToolTipText = "Click to Disable"; btnLeftRclick.Enabled = true; cmbLeftRclick.Enabled = true; switchLeftRC.Enabled = true; }
                        }
                        else if (counter == 23)
                        {
                            buttonConfig[counter] = configReader2.Split(',');
                            btnUpLeftRclick.Text = buttonConfig[counter][0].Replace("/", "-");
                            cmbUpLeftRclick.SelectedItem = buttonConfig[counter][3].Replace("/", "-");
                            switchUpLeftRC.SelectedItem = buttonConfig[counter][6].Replace("/", "-");
                            directions[counter] = buttonConfig[counter][0].Replace("/", "-");

                            if (buttonConfig[counter][1].ToString() == "True")
                            {
                                togUpLeftRC.Checked = true;
                                holdUpLeftRC.Checked = false;
                                onceUpLeftRC.Checked = false;
                            }
                            else if (buttonConfig[counter][1].ToString() == "Once")
                            {
                                onceUpLeftRC.Checked = true;
                                togUpLeftRC.Checked = false;
                                holdUpLeftRC.Checked = false;
                            }
                            else { holdUpLeftRC.Checked = true; togUpLeftRC.Checked = false; onceUpLeftRC.Checked = false; }

                            disUpLeftRC.Checked = bool.Parse(buttonConfig[counter][2]);
                            rtcUpLeftRC.Checked = bool.Parse(buttonConfig[counter][4]);

                            if (disUpLeftRC.Checked == true)
                            {
                                disableUpLeftRC.RemoveCssClass("disableButtonOff"); disableUpLeftRC.AddCssClass("disableButtonOn");
                                disableUpLeftRC.PerformClick();

                            }
                            else if (diagUpLeftRC.Checked == false) { disableUpLeftRC.ToolTipText = "Click to Disable"; btnUpLeftRclick.Enabled = true; cmbUpLeftRclick.Enabled = true; switchUpLeftRC.Enabled = true; }
                            else { disableUpLeftRC.ToolTipText = "Click to Disable"; }
                        }
                        else if (counter == 24)
                        {
                            buttonConfig[counter] = configReader2.Split(',');
                            btnDZUpperHover.Text = buttonConfig[counter][0].Replace("/", "-");
                            cmbDZUpperHover.SelectedItem = buttonConfig[counter][3].Replace("/", "-");
                            switchDZUpperHover.SelectedItem = buttonConfig[counter][6].Replace("/", "-");
                            directions[counter] = buttonConfig[counter][0].Replace("/", "-");

                            if (buttonConfig[counter][1].ToString() == "True")
                            {
                                togDZUpperHover.Checked = true;
                                onceDZUpper.Checked = false;
                            }
                            else if (buttonConfig[counter][1].ToString() == "Once") { togDZUpperHover.Checked = false; onceDZUpper.Checked = true; }
                            else { togDZUpperHover.Checked = false; onceDZUpper.Checked = false; }
                            disDZUpperHover.Checked = bool.Parse(buttonConfig[counter][2]);

                            if (disDZUpperHover.Checked == true)
                            {
                                ApplyDisableButtonState(disableDZUpperHover, true);

                            }
                            else { disableDZUpperHover.ToolTipText = "Click to Disable"; btnDZUpperHover.Enabled = true; cmbDZUpperHover.Enabled = true; switchDZUpperHover.Enabled = true; }

                        }
                        else if (counter == 25)
                        {
                            buttonConfig[counter] = configReader2.Split(',');
                            btnDZUpperLC.Text = buttonConfig[counter][0].Replace("/", "-");
                            cmbDZUpperLC.SelectedItem = buttonConfig[counter][3].Replace("/", "-");
                            switchDZUpperLC.SelectedItem = buttonConfig[counter][6].Replace("/", "-");
                            directions[counter] = buttonConfig[counter][0].Replace("/", "-");

                            if (buttonConfig[counter][1].ToString() == "True")
                            {
                                togDZUpperLC.Checked = true;
                                holdDZUpperLC.Checked = false;
                                onceDZUpperLC.Checked = false;
                            }
                            else if (buttonConfig[counter][1].ToString() == "Once")
                            {
                                onceDZUpperLC.Checked = true;
                                togDZUpperLC.Checked = false;
                                holdDZUpperLC.Checked = false;
                            }

                            else { holdDZUpperLC.Checked = true; togDZUpperLC.Checked = false; onceDZUpperLC.Checked = false; }
                            disDZUpperLC.Checked = bool.Parse(buttonConfig[counter][2]);

                            if (disDZUpperLC.Checked == true)
                            {
                                ApplyDisableButtonState(disableDZUpperLC, true);

                            }
                            else { disableDZUpperLC.ToolTipText = "Click to Disable"; btnDZUpperLC.Enabled = true; cmbDZUpperLC.Enabled = true; switchDZUpperLC.Enabled = true; }
                        }
                        else if (counter == 26)
                        {
                            buttonConfig[counter] = configReader2.Split(',');
                            btnDZUpperRC.Text = buttonConfig[counter][0].Replace("/", "-");
                            cmbDZUpperRC.SelectedItem = buttonConfig[counter][3].Replace("/", "-");
                            switchDZUpperRC.SelectedItem = buttonConfig[counter][6].Replace("/", "-");
                            directions[counter] = buttonConfig[counter][0].Replace("/", "-");

                            if (buttonConfig[counter][1].ToString() == "True")
                            {
                                togDZUpperRC.Checked = true;
                                holdDZUpperRC.Checked = false;
                                onceDZUpperRC.Checked = false;
                            }
                            else if (buttonConfig[counter][1].ToString() == "Once")
                            {
                                onceDZUpperRC.Checked = true;
                                togDZUpperRC.Checked = false;
                                holdDZUpperRC.Checked = false;
                            }
                            else { holdDZUpperRC.Checked = true; togDZUpperRC.Checked = false; onceDZUpperRC.Checked = false; }
                            disDZUpperRC.Checked = bool.Parse(buttonConfig[counter][2]);

                            if (disDZUpperRC.Checked == true)
                            {
                                ApplyDisableButtonState(disableDZUpperRC, true);

                            }
                            else { disableDZUpperRC.ToolTipText = "Click to Disable"; btnDZUpperRC.Enabled = true; cmbDZUpperRC.Enabled = true; switchDZUpperRC.Enabled = true; }
                        }
                        else if (counter == 27)
                        {
                            buttonConfig[counter] = configReader2.Split(',');
                            btnDZMiddleHover.Text = buttonConfig[counter][0].Replace("/", "-");
                            cmbDZMiddleHover.SelectedItem = buttonConfig[counter][3].Replace("/", "-");
                            switchDZMiddleHover.SelectedItem = buttonConfig[counter][6].Replace("/", "-");
                            directions[counter] = buttonConfig[counter][0].Replace("/", "-");
                            if (buttonConfig[counter][1].ToString() == "True")
                            {
                                togDZMiddleHover.Checked = true;
                                onceDZMiddle.Checked = false;
                            }
                            else if (buttonConfig[counter][1].ToString() == "Once") { togDZMiddleHover.Checked = false; onceDZMiddle.Checked = true; }
                            else { togDZMiddleHover.Checked = false; onceDZMiddle.Checked = false; }
                            disDZMiddleHover.Checked = bool.Parse(buttonConfig[counter][2]);

                            if (disDZMiddleHover.Checked == true)
                            {
                                ApplyDisableButtonState(disableDZMiddleHover, true);

                            }
                            else { disableDZMiddleHover.ToolTipText = "Click to Disable"; btnDZMiddleHover.Enabled = true; cmbDZMiddleHover.Enabled = true; switchDZMiddleHover.Enabled = true; }
                        }
                        else if (counter == 28)
                        {
                            buttonConfig[counter] = configReader2.Split(',');
                            btnDZMiddleLC.Text = buttonConfig[counter][0].Replace("/", "-");
                            cmbDZMiddleLC.SelectedItem = buttonConfig[counter][3].Replace("/", "-");
                            switchDZMiddleLC.SelectedItem = buttonConfig[counter][6].Replace("/", "-");
                            directions[counter] = buttonConfig[counter][0].Replace("/", "-");

                            if (buttonConfig[counter][1].ToString() == "True")
                            {
                                togDZMiddleLC.Checked = true;
                                holdDZMiddleLC.Checked = false;
                                onceDZMiddleLC.Checked = false;
                            }

                            else if (buttonConfig[counter][1].ToString() == "Once")
                            {
                                onceDZMiddleLC.Checked = true;
                                togDZMiddleLC.Checked = false;
                                holdDZMiddleLC.Checked = false;
                            }
                            else { holdDZMiddleLC.Checked = true; togDZMiddleLC.Checked = false; onceDZMiddleLC.Checked = false; }
                            disDZMiddleLC.Checked = bool.Parse(buttonConfig[counter][2]);

                            if (disDZMiddleLC.Checked == true)
                            {
                                ApplyDisableButtonState(disableDZMiddleLC, true);

                            }
                            else { disableDZMiddleLC.ToolTipText = "Click to Disable"; btnDZMiddleLC.Enabled = true; cmbDZMiddleLC.Enabled = true; switchDZMiddleLC.Enabled = true; }
                        }
                        else if (counter == 29)
                        {
                            buttonConfig[counter] = configReader2.Split(',');
                            btnDZMiddleRC.Text = buttonConfig[counter][0].Replace("/", "-");
                            cmbDZMiddleRC.SelectedItem = buttonConfig[counter][3].Replace("/", "-");
                            switchDZMiddleRC.SelectedItem = buttonConfig[counter][6].Replace("/", "-");
                            directions[counter] = buttonConfig[counter][0].Replace("/", "-");

                            if (buttonConfig[counter][1].ToString() == "True")
                            {
                                togDZMiddleRC.Checked = true;
                                holdDZMiddleRC.Checked = false;
                                onceDZMiddleRC.Checked = false;
                            }
                            else if (buttonConfig[counter][1].ToString() == "Once")
                            {
                                onceDZMiddleRC.Checked = true;
                                togDZMiddleRC.Checked = false;
                                holdDZMiddleRC.Checked = false;
                            }
                            else { holdDZMiddleRC.Checked = true; togDZMiddleRC.Checked = false; onceDZMiddleRC.Checked = false; }
                            disDZMiddleRC.Checked = bool.Parse(buttonConfig[counter][2]);

                            if (disDZMiddleRC.Checked == true)
                            {
                                ApplyDisableButtonState(disableDZMiddleRC, true);

                            }
                            else { disableDZMiddleRC.ToolTipText = "Click to Disable"; btnDZMiddleRC.Enabled = true; cmbDZMiddleRC.Enabled = true; switchDZMiddleRC.Enabled = true; }
                        }
                        else if (counter == 30)
                        {
                            buttonConfig[counter] = configReader2.Split(',');
                            btnDZLowerHover.Text = buttonConfig[counter][0].Replace("/", "-");
                            cmbDZLowerHover.SelectedItem = buttonConfig[counter][3].Replace("/", "-");
                            switchDZLowerHover.SelectedItem = buttonConfig[counter][6].Replace("/", "-");
                            directions[counter] = buttonConfig[counter][0].Replace("/", "-");

                            if (buttonConfig[counter][1].ToString() == "True")
                            {
                                togDZLowerHover.Checked = true;
                                onceDZLower.Checked = false;
                            }
                            else if (buttonConfig[counter][1].ToString() == "Once") { togDZLowerHover.Checked = false; onceDZLower.Checked = true; }
                            else { togDZLowerHover.Checked = false; onceDZLower.Checked = false; }
                            disDZLowerHover.Checked = bool.Parse(buttonConfig[counter][2]);

                            if (disDZLowerHover.Checked == true)
                            {
                                ApplyDisableButtonState(disableDZLowerHover, true);

                            }
                            else { disableDZLowerHover.ToolTipText = "Click to Disable"; btnDZLowerHover.Enabled = true; cmbDZLowerHover.Enabled = true; switchDZLowerHover.Enabled = true; }
                        }
                        else if (counter == 31)
                        {
                            buttonConfig[counter] = configReader2.Split(',');
                            btnDZLowerLC.Text = buttonConfig[counter][0].Replace("/", "-");
                            cmbDZLowerLC.SelectedItem = buttonConfig[counter][3].Replace("/", "-");
                            switchDZLowerLC.SelectedItem = buttonConfig[counter][6].Replace("/", "-");
                            directions[counter] = buttonConfig[counter][0].Replace("/", "-");

                            if (buttonConfig[counter][1].ToString() == "True")
                            {
                                togDZLowerLC.Checked = true;
                                holdDZLowerLC.Checked = false;
                                onceDZLowerLC.Checked = false;
                            }
                            else if (buttonConfig[counter][1].ToString() == "Once")
                            {
                                onceDZLowerLC.Checked = true;
                                togDZLowerLC.Checked = false;
                                holdDZLowerLC.Checked = false;
                            }
                            else { holdDZLowerLC.Checked = true; togDZLowerLC.Checked = false; onceDZLowerLC.Checked = false; }
                            disDZLowerLC.Checked = bool.Parse(buttonConfig[counter][2]);

                            if (disDZLowerLC.Checked == true)
                            {
                                ApplyDisableButtonState(disableDZLowerLC, true);

                            }
                            else { disableDZLowerLC.ToolTipText = "Click to Disable"; btnDZLowerLC.Enabled = true; cmbDZLowerLC.Enabled = true; switchDZLowerLC.Enabled = true; }
                        }
                        else if (counter == 32)
                        {
                            buttonConfig[counter] = configReader2.Split(',');
                            btnDZLowerRC.Text = buttonConfig[counter][0].Replace("/", "-");
                            cmbDZLowerRC.SelectedItem = buttonConfig[counter][3].Replace("/", "-");
                            switchDZLowerRC.SelectedItem = buttonConfig[counter][6].Replace("/", "-");
                            directions[counter] = buttonConfig[counter][0].Replace("/", "-");

                            if (buttonConfig[counter][1].ToString() == "True")
                            {
                                togDZLowerRC.Checked = true;
                                holdDZLowerRC.Checked = false;
                                onceDZLowerRC.Checked = false;
                            }

                            else if (buttonConfig[counter][1].ToString() == "Once")
                            {
                                onceDZLowerRC.Checked = true;
                                togDZLowerRC.Checked = false;
                                holdDZLowerRC.Checked = false;
                            }
                            else { holdDZLowerRC.Checked = true; togDZLowerRC.Checked = false; onceDZLowerRC.Checked = false; }
                            disDZLowerRC.Checked = bool.Parse(buttonConfig[counter][2]);

                            if (disDZLowerRC.Checked == true)
                            {
                                ApplyDisableButtonState(disableDZLowerRC, true);

                            }
                            else { disableDZLowerRC.ToolTipText = "Click to Disable"; btnDZLowerRC.Enabled = true; cmbDZLowerRC.Enabled = true; switchDZLowerRC.Enabled = true; }
                        }
                        else if (counter == 33)
                        {
                            loadedActivateUpper = bool.Parse(configReader2);
                        }
                        else if (counter == 34)
                        {
                            loadedActivateMiddle = bool.Parse(configReader2);
                        }
                        else if (counter == 35)
                        {
                            loadedActivateLower = bool.Parse(configReader2);
                        }
                        else if (counter == 36)
                        {
                            chkLabelsHover.Checked = bool.Parse(configReader2);
                        }
                        else if (counter == 37)
                        {
                            chkLabelsLC.Checked = bool.Parse(configReader2);
                        }
                        else if (counter == 38)
                        {
                            chkLabelsRC.Checked = bool.Parse(configReader2);
                        }
                        else if (counter == 39)
                        {
                        }
                        else if (counter == 40)
                        {
                            if (configReader2 == "Xbox") { chkXbox.Checked = true; nintendoSwitch.Checked = false; chkKeyboard.Checked = false; }
                            else if (configReader2 == "Switch") { nintendoSwitch.Checked = true; chkXbox.Checked = false; chkKeyboard.Checked = false; }
                            else { nintendoSwitch.Checked = false; chkXbox.Checked = false; chkKeyboard.Checked = true; }
                        }
                        else if (counter == 41)
                        {

                        }
                        else if (counter == 42)
                        { }
                        else if (counter == 43)
                        {
                            hasExplicitCombineStates = true;
                            diagUpRightHover.Checked = bool.Parse(configReader2);
                        }
                        else if (counter == 44)
                        {
                            hasExplicitCombineStates = true;
                            diagUpRightLC.Checked = bool.Parse(configReader2);
                        }
                        else if (counter == 45)
                        {
                            hasExplicitCombineStates = true;
                            diagUpRightRC.Checked = bool.Parse(configReader2);
                        }
                        else if (counter == 46)
                        {
                            hasExplicitCombineStates = true;
                            diagDownRightHover.Checked = bool.Parse(configReader2);
                        }
                        else if (counter == 47)
                        {
                            hasExplicitCombineStates = true;
                            diagDownRightLC.Checked = bool.Parse(configReader2);
                        }
                        else if (counter == 48)
                        {
                            hasExplicitCombineStates = true;
                            diagDownRightRC.Checked = bool.Parse(configReader2);
                        }
                        else if (counter == 49)
                        {
                            hasExplicitCombineStates = true;
                            diagUpLeftHover.Checked = bool.Parse(configReader2);
                        }
                        else if (counter == 50)
                        {
                            hasExplicitCombineStates = true;
                            diagUpLeftLC.Checked = bool.Parse(configReader2);
                        }
                        else if (counter == 51)
                        {
                            hasExplicitCombineStates = true;
                            diagUpLeftRC.Checked = bool.Parse(configReader2);
                        }
                        else if (counter == 52)
                        {
                            hasExplicitCombineStates = true;
                            diagDownLeftHover.Checked = bool.Parse(configReader2);
                        }
                        else if (counter == 53)
                        {
                            hasExplicitCombineStates = true;
                            diagDownLeftLC.Checked = bool.Parse(configReader2);
                        }
                        else if (counter == 54)
                        {
                            hasExplicitCombineStates = true;
                            diagDownLeftRC.Checked = bool.Parse(configReader2);
                        }
                        else if (counter == 57)
                        {


                            if (configReader2.Contains("1")) { cmbAdvanced.Items.Remove("(Click to Turn Off) Twin Stick Mode"); cmbAdvanced.Items.Remove("(Click to Turn On) Twin Stick Mode"); cmbAdvanced.Items.Add("(Click to Turn Off) Twin Stick Mode"); advFPS = true; }
                            else { cmbAdvanced.Items.Remove("(Click to Turn Off) Twin Stick Mode"); cmbAdvanced.Items.Remove("(Click to Turn On) Twin Stick Mode"); cmbAdvanced.Items.Add("(Click to Turn On) Twin Stick Mode"); advFPS = false; }
                            if (configReader2.Contains("2")) { cmbAdvanced.Items.Remove("(Click to Turn On) Combine Inputs from Other Systems"); cmbAdvanced.Items.Remove("(Click to Turn Off) Combine Inputs from Other Systems"); cmbAdvanced.Items.Add("(Click to Turn Off) Combine Inputs from Other Systems"); advCombine = true; }
                            else { cmbAdvanced.Items.Remove("(Click to Turn On) Combine Inputs from Other Systems"); cmbAdvanced.Items.Remove("(Click to Turn Off) Combine Inputs from Other Systems"); cmbAdvanced.Items.Add("(Click to Turn On) Combine Inputs from Other Systems"); advCombine = false; }
                            if (configReader2.Contains("3")) { cmbAdvanced.Items.Remove("(Click to Turn On) Alternative Mouse Controls"); cmbAdvanced.Items.Remove("(Click to Turn Off) Alternative Mouse Controls"); cmbAdvanced.Items.Add("(Click to Turn Off) Alternative Mouse Controls"); advDisableClicks = true; }
                            else { cmbAdvanced.Items.Remove("(Click to Turn On) Alternative Mouse Controls"); cmbAdvanced.Items.Remove("(Click to Turn Off) Alternative Mouse Controls"); cmbAdvanced.Items.Add("(Click to Turn On) Alternative Mouse Controls"); advDisableClicks = false; }
                            if (configReader2.Contains("4")) { cmbAdvanced.Items.Remove("(Click to Turn On) Alternative Return To Center"); cmbAdvanced.Items.Remove("(Click to Turn Off) Alternative Return To Center"); cmbAdvanced.Items.Add("(Click to Turn Off) Alternative Return To Center"); advAltRTC = true; }
                            else { cmbAdvanced.Items.Remove("(Click to Turn On) Alternative Return To Center"); cmbAdvanced.Items.Remove("(Click to Turn Off) Alternative Return To Center"); cmbAdvanced.Items.Add("(Click to Turn On) Alternative Return To Center"); advAltRTC = false; }
                            if (configReader2.Contains("5") && nintendoSwitch.Checked) { cmbAdvanced.Items.Remove("(Click to Turn On) Mario Kart Mode"); cmbAdvanced.Items.Remove("(Click to Turn Off) Mario Kart Mode"); cmbAdvanced.Items.Add("(Click to Turn Off) Mario Kart Mode"); advMarioKart = true; }
                            else
                            {
                                if (nintendoSwitch.Checked) { cmbAdvanced.Items.Remove("(Click to Turn On) Mario Kart Mode"); cmbAdvanced.Items.Remove("(Click to Turn Off) Mario Kart Mode"); cmbAdvanced.Items.Add("(Click to Turn On) Mario Kart Mode"); advMarioKart = false; }
                            }
                            if (configReader2.Contains("6")) { cmbAdvanced.Items.Remove("(Click to Turn On) Voice Control Mode"); cmbAdvanced.Items.Remove("(Click to Turn Off) Voice Control Mode"); cmbAdvanced.Items.Add("(Click to Turn Off) Voice Control Mode"); advVoice = true; }
                            else { cmbAdvanced.Items.Remove("(Click to Turn On) Voice Control Mode"); cmbAdvanced.Items.Remove("(Click to Turn Off) Voice Control Mode"); cmbAdvanced.Items.Add("(Click to Turn On) Voice Control Mode"); advVoice = false; }
                            if (configReader2.Contains("7")) { cmbAdvanced.Items.Remove("(Click to Turn On) Keep Mouse Inside Overjoyed"); cmbAdvanced.Items.Remove("(Click to Turn Off) Keep Mouse Inside Overjoyed"); cmbAdvanced.Items.Add("(Click to Turn On) Keep Mouse Inside Overjoyed"); advKeepMouse = true; }
                            else
                            {
                                cmbAdvanced.Items.Remove("(Click to Turn On) Keep Mouse Inside Overjoyed"); cmbAdvanced.Items.Remove("(Click to Turn Off) Keep Mouse Inside Overjoyed"); cmbAdvanced.Items.Add("(Click to Turn Off) Keep Mouse Inside Overjoyed"); advKeepMouse = false;
                            }
                            if (configReader2.Contains("8")) { cmbAdvanced.Items.Remove("(Click to Turn On) Prevent Game From Locking Mouse"); cmbAdvanced.Items.Remove("(Click to Turn Off) Prevent Game From Locking Mouse"); cmbAdvanced.Items.Add("(Click to Turn On) Prevent Game From Locking Mouse"); advMouseLock = true; }
                            else { if (!nintendoSwitch.Checked) { cmbAdvanced.Items.Remove("(Click to Turn On) Prevent Game From Locking Mouse"); cmbAdvanced.Items.Remove("(Click to Turn Off) Prevent Game From Locking Mouse"); cmbAdvanced.Items.Add("(Click to Turn Off) Prevent Game From Locking Mouse"); advMouseLock = false; } }
                            if (configReader2.Contains("9")) { cmbAdvanced.Items.Remove("(Click to Turn On) Press PlayStation Buttons"); cmbAdvanced.Items.Remove("(Click to Turn Off) Press PlayStation Buttons"); cmbAdvanced.Items.Add("(Click to Turn Off) Press PlayStation Buttons"); advPSbuttons = true; }
                            else { cmbAdvanced.Items.Remove("(Click to Turn On) Press PlayStation Buttons"); cmbAdvanced.Items.Remove("(Click to Turn Off) Press PlayStation Buttons"); cmbAdvanced.Items.Add("(Click to Turn On) Press PlayStation Buttons"); advPSbuttons = false; }
                            if (configReader2.Contains("A")) { cmbAdvanced.Items.Remove("(Click to Turn On) Use Click Safe Zone"); cmbAdvanced.Items.Remove("(Click to Turn Off) Use Click Safe Zone"); cmbAdvanced.Items.Add("(Click to Turn Off) Use Click Safe Zone"); }
                            else { cmbAdvanced.Items.Remove("(Click to Turn On) Use Click Safe Zone"); cmbAdvanced.Items.Remove("(Click to Turn Off) Use Click Safe Zone"); cmbAdvanced.Items.Add("(Click to Turn On) Use Click Safe Zone"); }

                            if (chkLabelsHover.Checked)
                            {
                                cmbAdvanced.Items.Remove("(Click to Turn On) Show Mouse Move Labels");
                                cmbAdvanced.Items.Remove("(Click to Turn Off) Show Mouse Move Labels");
                                cmbAdvanced.Items.Add("(Click to Turn Off) Show Mouse Move Labels");
                            }
                            else
                            {
                                cmbAdvanced.Items.Remove("(Click to Turn On) Show Mouse Move Labels");
                                cmbAdvanced.Items.Remove("(Click to Turn Off) Show Mouse Move Labels");
                                cmbAdvanced.Items.Add("(Click to Turn On) Show Mouse Move Labels");

                            }

                            if (chkLabelsLC.Checked)
                            {
                                cmbAdvanced.Items.Remove("(Click to Turn On) Show Left Click Labels");
                                cmbAdvanced.Items.Remove("(Click to Turn Off) Show Left Click Labels");
                                cmbAdvanced.Items.Add("(Click to Turn Off) Show Left Click Labels");
                            }
                            else
                            {
                                cmbAdvanced.Items.Remove("(Click to Turn On) Show Left Click Labels");
                                cmbAdvanced.Items.Remove("(Click to Turn Off) Show Left Click Labels");
                                cmbAdvanced.Items.Add("(Click to Turn On) Show Left Click Labels");
                            }

                            if (chkLabelsRC.Checked)
                            {
                                cmbAdvanced.Items.Remove("(Click to Turn On) Show Right Click Labels");
                                cmbAdvanced.Items.Remove("(Click to Turn Off) Show Right Click Labels");
                                cmbAdvanced.Items.Add("(Click to Turn Off) Show Right Click Labels");
                            }
                            else
                            {
                                cmbAdvanced.Items.Remove("(Click to Turn On) Show Right Click Labels");
                                cmbAdvanced.Items.Remove("(Click to Turn Off) Show Right Click Labels");
                                cmbAdvanced.Items.Add("(Click to Turn On) Show Right Click Labels");
                            }
                        }
                        counter++;
                    }
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

            if (!hasExplicitCombineStates)
            {
                ApplyLegacyCombineDefaults();
            }

            ApplyLoadedActivationZoneState(loadedActivateUpper, loadedActivateMiddle, loadedActivateLower);
            if (!hasExplicitCombineStates)
            {
                SyncDiagonalCombineStatesFromLoadedActions();
            }

            //string alphaFilePath = Path.Combine(configPath, "Transparency.txt");
            //int alpha = (int)((255 - int.Parse(File.ReadLines(alphaFilePath).ElementAtOrDefault(0))) / 2.55);


            //txtAlpha.Text = alpha.ToString();

            pnlDeadzone.ResumeLayout();
            pnlQuadrants.ResumeLayout();




        }


    }

    public class InputForm : Form
    {
        private TextBox settingsCodeTextBox;
        private TextBox profileNameTextBox;
        private Button okButton;
        private Button cancelButton;
        private ComboBox formatComboBox;
        private TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
        private Button createNewLayoutButton;


        public InputForm()
        {
            InitializeComponent();
        }

        private void CreateNewLayoutButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Abort;
            this.Close();
        }

        private void InitializeComponent()
        {

            // Create the "Create New Layout" button
            createNewLayoutButton = new Button();
            createNewLayoutButton.Text = "Create New Layout";
            createNewLayoutButton.Click += CreateNewLayoutButton_Click;
            createNewLayoutButton.Dock = DockStyle.Top;
            createNewLayoutButton.Padding = new Padding(0, 0, 0, 0);
            createNewLayoutButton.Font = new Font("Segoe UI", 12f, FontStyle.Regular, GraphicsUnit.Point);


            //// Insert the button at the top of the layout
            //tableLayoutPanel.Controls.Add(createNewLayoutButton, 0, 0);
            //tableLayoutPanel.SetColumnSpan(createNewLayoutButton, tableLayoutPanel.ColumnCount);


            this.Text = "Import Game Layout";
            this.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new System.Drawing.Size(400, 340);

            this.Padding = new Padding(10);


            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ControlBox = true;
            this.FormBorderStyle = FormBorderStyle.Fixed;
            this.Movable = false;

            // New label for dropdown
            Label formatLabel = new Label();
            formatLabel.Text = "Or Choose Game To Import:";
            formatLabel.Dock = DockStyle.Top;
            formatLabel.Padding = new Padding(0, 0, 0, 0);
            formatLabel.Font = new Font("Segoe UI", 12f, FontStyle.Regular, GraphicsUnit.Point);

            // ComboBox for format selection
            formatComboBox = new ComboBox();
            formatComboBox.Dock = DockStyle.Top;
            formatComboBox.SelectedIndex = -1; // No selection by default
            formatComboBox.TabIndex = 0;
            //autocompleteMode
            formatComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            string[] gameArray = Config.gameList.ToArray();
            formatComboBox.Items.AddRange(gameArray);
            formatComboBox.SelectedIndexChanged += ChangeGame;
            formatComboBox.Font = new Font("Segoe UI", 12f, FontStyle.Regular, GraphicsUnit.Point);


            Label settingsCodeLabel = new Label();
            settingsCodeLabel.Text = "Or Paste Layout Code Here:";
            settingsCodeLabel.Dock = DockStyle.Top;
            settingsCodeLabel.Padding = new Padding(0, 0, 0, 0);
            settingsCodeLabel.Font = new Font("Segoe UI", 12f, FontStyle.Regular, GraphicsUnit.Point);

            this.settingsCodeTextBox = new TextBox();
            this.settingsCodeTextBox.Dock = DockStyle.Top;
            this.settingsCodeTextBox.Margin = new Padding(0, 0, 0, 0);
            this.settingsCodeTextBox.TabIndex = 1;
            this.settingsCodeTextBox.Font = new Font("Segoe UI", 12f, FontStyle.Regular, GraphicsUnit.Point);

            Label profileNameLabel = new Label();
            profileNameLabel.Text = "Name Your Imported Layout:";
            profileNameLabel.Dock = DockStyle.Top;
            profileNameLabel.Padding = new Padding(0, 0, 0, 0);
            profileNameLabel.Font = new Font("Segoe UI", 12f, FontStyle.Regular, GraphicsUnit.Point);


            this.profileNameTextBox = new TextBox();
            this.profileNameTextBox.Dock = DockStyle.Top;
            this.profileNameTextBox.Margin = new Padding(0, 0, 0, 0);
            this.profileNameTextBox.TabIndex = 2;
            this.profileNameTextBox.KeyUp += txtProfileName_KeyUp;
            this.profileNameTextBox.Font = new Font("Segoe UI", 12f, FontStyle.Regular, GraphicsUnit.Point);

            this.okButton = new Button();
            this.okButton.Text = "OK";
            this.okButton.Dock = DockStyle.Bottom;
            this.okButton.TabIndex = 3;
            this.okButton.Enabled = false;
            this.okButton.Click += OkButton_Click;

            this.cancelButton = new Button();
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Dock = DockStyle.Bottom;
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Click += CancelButton_Click;

            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.ColumnCount = 2;
            tableLayoutPanel.RowCount = 7;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 16F)); // createNewLayoutButton
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 11F)); // formatLabel
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 16F)); // formatComboBox
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 11F)); // settingsCodeLabel
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 16F)); // settingsCodeTextBox
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 11F)); // profileNameLabel
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 16F)); // profileNameTextBox
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 16F)); // buttons

            tableLayoutPanel.Controls.Add(createNewLayoutButton, 0, 0);
            tableLayoutPanel.SetColumnSpan(createNewLayoutButton, 2);
            tableLayoutPanel.Controls.Add(formatLabel, 0, 1);
            formatLabel.CssStyle = "overflow:visible!important;";
            tableLayoutPanel.SetColumnSpan(formatLabel, 2);
            tableLayoutPanel.Controls.Add(formatComboBox, 0, 2);
            tableLayoutPanel.SetColumnSpan(formatComboBox, 2);
            tableLayoutPanel.Controls.Add(settingsCodeLabel, 0, 3);
            settingsCodeLabel.CssStyle = "overflow:visible!important;";
            tableLayoutPanel.SetColumnSpan(settingsCodeLabel, 2);
            tableLayoutPanel.Controls.Add(this.settingsCodeTextBox, 0, 4);
            tableLayoutPanel.SetColumnSpan(this.settingsCodeTextBox, 2);
            tableLayoutPanel.Controls.Add(profileNameLabel, 0, 5);
            profileNameLabel.CssStyle = "overflow:visible!important;";
            tableLayoutPanel.SetColumnSpan(profileNameLabel, 2);
            tableLayoutPanel.Controls.Add(this.profileNameTextBox, 0, 6);
            tableLayoutPanel.SetColumnSpan(this.profileNameTextBox, 2);
            tableLayoutPanel.Controls.Add(this.okButton, 0, 7);
            tableLayoutPanel.Controls.Add(this.cancelButton, 1, 7);

            this.Controls.Add(tableLayoutPanel);

            this.AcceptButton = this.okButton;

            this.Load += InputForm_Load;


        }

        private void InputForm_Load(object sender, EventArgs e)
        {
            if (Screen.Bounds.Height >= 1100 && Screen.Bounds.Width > 1920)
            {

                tableLayoutPanel.Scale(1.1f);

                ScaleFonts(tableLayoutPanel, 1.1f);


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
document.querySelectorAll('[class^=""qx-textlabel-borderNone""]').forEach(el => {
    applyMarquee(el);
});

document.querySelectorAll('[name^=""label""]').forEach(el => {
    applyMarquee(el);
});

document.querySelectorAll('[name^=""title""]').forEach(el => {
    applyMarquee(el);
});
}, 2000); // 2000 milliseconds = 2 seconds
");
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ChangeGame(object sender, EventArgs e)
        {
            settingsCodeTextBox.Text = Config.settingsCodes[formatComboBox.SelectedIndex];
            profileNameTextBox.Text = Config.gameNames[formatComboBox.SelectedIndex];
            checkGameName();
        }

        private void checkGameName()
        {
            bool fileExists = false;
            string profilesPath = Path.Combine(FileSystem.Current.AppDataDirectory, "profiles");

            try
            {
                // Verify if the profiles directory exists
                if (Directory.Exists(profilesPath))
                {
                    // Get all .txt files in the profiles directory
                    string[] files = Directory.GetFiles(profilesPath, "*.txt");

                    // Check if any file name (without extension) matches the entered text
                    foreach (string file in files)
                    {
                        if (Path.GetFileNameWithoutExtension(file).Equals(profileNameTextBox.Text.Trim(), StringComparison.OrdinalIgnoreCase))
                        {
                            fileExists = true;
                            break;
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"The directory '{profilesPath}' does not exist.");
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


            // Enable the add button if the file does not exist
            okButton.Enabled = false;

            if (!fileExists && !string.IsNullOrEmpty(profileNameTextBox.Text) && !profileNameTextBox.Text.Equals("Default Layout", StringComparison.OrdinalIgnoreCase))
            {


                okButton.Enabled = true;
            }
        }

        private void txtProfileName_KeyUp(object sender, KeyEventArgs e)
        {
            checkGameName();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.settingsCodeTextBox.Text))
            {
                ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Please paste the settings code.'>Please paste the settings code.</span></p>", "Validation", ExtendedMessageBoxLibrary.MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(this.profileNameTextBox.Text))
            {
                ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Please name your imported profile.'>Please name your imported profile.</span></p>", "Validation", ExtendedMessageBoxLibrary.MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.DialogResult = DialogResult.OK;

            this.Close();

            ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 1; -webkit-box-orient: vertical; overflow: hidden; ' title='Settings imported successfully!'>Settings imported successfully!</span></p>", "Success", ExtendedMessageBoxLibrary.MessageBoxButtons.OK);

            if (formatComboBox.SelectedIndex >= 0 && formatComboBox.SelectedIndex < Config.gameNotes.Count)
            {
                if (string.IsNullOrEmpty(Config.gameNotes[formatComboBox.SelectedIndex]) == false)
                {

                    ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 3; -webkit-box-orient: vertical; overflow: hidden; ' title='" + Config.gameNotes[formatComboBox.SelectedIndex] + "'>" + Config.gameNotes[formatComboBox.SelectedIndex] + "</span></p>", "Notes From Layout Contributor(s)", ExtendedMessageBoxLibrary.MessageBoxButtons.OK);
                }
            }
        }

        public string GetSettingsCode()
        {

            return this.settingsCodeTextBox.Text.Trim().ToUpper();
        }

        public string GetProfileName()
        {
            string safeFileName = Config.GetSafeFileName(profileNameTextBox);

            return safeFileName;
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
    }


    public class CustomMessageBox6 : Form
    {
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SelectedOption { get; private set; }

        public CustomMessageBox6(params string[] buttonLabels)
        {
            if (buttonLabels.Length != 6)
                throw new ArgumentException("Exactly 6 button labels are required.");

            this.Text = "Choose an Option";
            this.Font = new Font("Segoe UI", 14f, FontStyle.Regular, GraphicsUnit.Point);

            this.Size = new Size(420, 220);
            if (Screen.Bounds.Height >= 1100 && Screen.Bounds.Width > 1920)
            {
                this.Scale(1.4f);
            }

            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Fixed;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ControlBox = true;

            var buttonTable = new TableLayoutPanel()
            {
                RowCount = 2,
                ColumnCount = 3,
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
            };

            for (int i = 0; i < 3; i++)
                buttonTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            for (int i = 0; i < 2; i++)
                buttonTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));

            for (int i = 0; i < 6; i++)
            {
                int index = i; // ? Safe closure
                var btn = new Button()
                {
                    Text = buttonLabels[index],
                    Dock = DockStyle.Fill,
                    Margin = new Padding(8),
                    Font = new Font("Segoe UI", 10f, FontStyle.Regular, GraphicsUnit.Point)
                };

                int row = index / 3;
                int col = index % 3;

                btn.Click += (s, e) =>
                {
                    SelectedOption = buttonLabels[index];
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                };

                buttonTable.Controls.Add(btn, col, row);
            }

            this.Controls.Add(buttonTable);
            if (Screen.Bounds.Height >= 1100 && Screen.Bounds.Width > 1920)
            {
                ScaleFonts(this, 1.4f);

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
document.querySelectorAll('[class^=""qx-textlabel-borderNone""]').forEach(el => {
    applyMarquee(el);
});

document.querySelectorAll('[name^=""label""]').forEach(el => {
    applyMarquee(el);
});

document.querySelectorAll('[name^=""title""]').forEach(el => {
    applyMarquee(el);
});
}, 2000); // 2000 milliseconds = 2 seconds
");
        }

        public static string Show(params string[] buttonLabels)
        {
            using (var msgBox = new CustomMessageBox6(buttonLabels))
            {
                msgBox.ShowDialog();
                return msgBox.SelectedOption;
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
    }




public class CustomMessageBox4 : Form
{
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string SelectedOption { get; private set; }

    public CustomMessageBox4(string message, params string[] buttonLabels)
    {
        if (buttonLabels.Length != 4)
            throw new ArgumentException("Exactly 4 button labels are required.");

        this.Text = "Choose an Option";
        this.Font = new Font("Segoe UI", 14f, FontStyle.Regular, GraphicsUnit.Point);

        this.Size = new Size(360, 220);
        if (Screen.Bounds.Height >= 1100 && Screen.Bounds.Width > 1920)
        {
            this.Scale(1.4f);
        }
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.Fixed;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.ControlBox = true;



        // Panel to hold everything
        var mainPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(10),
            Name = "mainPanel"
        };
        // 2x2 Button grid
        var table = new TableLayoutPanel()
        {
            RowCount = 2,
            ColumnCount = 2,
            Dock = DockStyle.Fill,
        };

        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
        table.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
        table.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));

        for (int i = 0; i < 4; i++)
        {
            int index = i; // ? Safe closure
            var btn = new Button()
            {
                Text = buttonLabels[index],
                Dock = DockStyle.Fill,
                Margin = new Padding(8),
                Font = new Font("Segoe UI", 10f, FontStyle.Regular, GraphicsUnit.Point)
            };

            int row = index / 2;
            int col = index % 2;

            btn.Click += (s, e) =>
            {
                SelectedOption = buttonLabels[index];
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            table.Controls.Add(btn, col, row);
        }


        mainPanel.Controls.Add(table);
        this.Controls.Add(mainPanel);
        if (Screen.Bounds.Height >= 1100 && Screen.Bounds.Width > 1920)
        {
            ScaleFonts(this, 1.4f);

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
document.querySelectorAll('[class^=""qx-textlabel-borderNone""]').forEach(el => {
    applyMarquee(el);
});

document.querySelectorAll('[name^=""label""]').forEach(el => {
    applyMarquee(el);
});

document.querySelectorAll('[name^=""title""]').forEach(el => {
    applyMarquee(el);
});
}, 2000); // 2000 milliseconds = 2 seconds
");

    }

    public static string Show(string message, params string[] buttonLabels)
    {
        using (var msgBox = new CustomMessageBox4(message, buttonLabels))
        {
            msgBox.ShowDialog();
            return msgBox.SelectedOption;
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
}

    }
