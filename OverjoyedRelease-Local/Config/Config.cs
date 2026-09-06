using ExtendedMessageBoxLibrary;
using Microsoft.Maui.Storage;
using Microsoft.UI.Xaml.Documents;
using Notion.Client;
using SharedVars;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Gaming.Preview.GamesEnumeration;
using Wisej.Core;
using Wisej.Web;
using Wisej.Web.Ext.TourPanel;
using Wisej.Web.Markup;

namespace OverjoyedReleaseLocal
{
    public partial class Config : Wisej.Web.Page
    {


        public Config()
        {
            InitializeComponent();
            _currentConfig = this;
        }



        private async void Config_Load()
        {
            int playerNum = GetAssignedPlayerNum();

            await MinimizeMessenger.SendMessageAsync("config|" + playerNum);

            if (gameList.Count == 0)
            {

                //run getNotion();
                await getNotion().ContinueWith(task =>
                   {
                       if (task.IsFaulted)
                       {
                           Console.WriteLine("Error fetching Notion data: " + task.Exception?.Message);
                       }
                       else
                       {
                           Console.WriteLine("Successfully fetched Notion data.");
                       }
                   });
            }
        }


        private void Form2_Load(object sender, EventArgs e)
        {

            Config_Load();
            RegisterNonHoverDisableSelectionHandlers();
            InitializeAssignmentComboPreviousIndices();
            backupPanel.Visible = false;
            backupPanel.Controls.Add(comboBackups);
            backupPanel.Controls.Add(btnRestore);
            backupPanel.Controls.Add(txtLog);
            pnlAllSettings.Controls.Add(backupPanel);
            backupPanel.BringToFront();

            comboBackups.SelectedIndexChanged += (s, e) => ShowBackupFileList(comboBackups.SelectedIndex);
            btnRestore.Click += (s, e) => RestoreSelectedBackup();
            LoadBackupList();

            Eval(@"(function initGTranslate() {
  function loadWidget() {
    // Prevent duplicates
    if (document.querySelector("".gtranslate_wrapper"")) return;
    if (window.gtranslateSettings) return;

    // Create wrapper div
    const wrapper = document.createElement(""div"");
    wrapper.className = ""gtranslate_wrapper"";
    document.body.appendChild(wrapper);

    // Set settings
    window.gtranslateSettings = {
      default_language: ""en"",
      native_language_names: true,
detect_browser_language:true,
      wrapper_selector: "".gtranslate_wrapper"",
      switcher_horizontal_position: ""right"",
      switcher_vertical_position: ""top"",
      float_switcher_open_direction: ""bottom"",
      alt_flags: { ""en"": ""usa"" }
    };

    // Add script if not already added
    if (!document.querySelector('script[src*=""gtranslate.net/widgets/latest/float.js""]')) {
      const script = document.createElement(""script"");
      script.src = ""https://cdn.gtranslate.net/widgets/latest/float.js"";
      script.defer = true;
      document.body.appendChild(script);
    }
  }

  // Run when body is ready
  if (document.readyState === ""loading"") {
    document.addEventListener(""DOMContentLoaded"", loadWidget);
  } else {
    loadWidget();
  }
})();");

            Eval(@"function watchLangCode() {
  const span = document.querySelector('.gt-lang-code');

  // Retry until the element exists
  if (!span) {
    setTimeout(watchLangCode, 200); // ?? retry every 200ms
    return;
  }

  const initial = span.textContent?.trim?.() ?? '';
  if (initial !== 'en') {
    console.log('Initial text is not ""en""; observer not started.');
    return;
  }

  const observer = new MutationObserver(() => {
    const newValue = span.textContent?.trim?.() ?? '';
    if (newValue && newValue !== 'en') {
      console.log('Language changed to:', newValue);
App.Config.onLanguageChange();
observer.disconnect(); // ? stop after first change

    }
  });

  observer.observe(span, {
    childList: true,
    characterData: true,
    subtree: true
  });

  console.log('Watching .gt-lang-code for language change...');
}

// ?? Start polling immediately until .gt-lang-code exists
watchLangCode();");



            actUp.MouseEnter += actTooltip;
            actUpRight.MouseEnter += actTooltip;
            actRight.MouseEnter += actTooltip;
            actDownRight.MouseEnter += actTooltip;
            actDown.MouseEnter += actTooltip;
            actDownLeft.MouseEnter += actTooltip;
            actLeft.MouseEnter += actTooltip;
            actUpLeft.MouseEnter += actTooltip;
            actDZUpper.MouseEnter += actTooltip;
            actDZMiddle.MouseEnter += actTooltip;
            actDZLower.MouseEnter += actTooltip;



            this.AutoScaleDimensions = new System.Drawing.SizeF(600F, 900F);
            this.AutoScaleMode = Wisej.Web.AutoScaleMode.Font;


            //            Eval(@"const style = document.createElement('style');
            //    style.textContent = `
            //     [name= 'ExitTour']{ opacity: 0!important; }
            //                     [name= 'CloseButton']{ background-color:black!important; }
            //`;
            //    document.head.appendChild(style);");

            //get screen width and height
            int screenWidth = Screen.Bounds.Width;
            float scaleFactor = 1.0f;
            if (Screen.Bounds.Height < 1100 && Screen.Bounds.Width <= 1920)
            {

                pnlAllSettings.Height = 870;
                pnlAllSettings.MinimumSize = new System.Drawing.Size(0, 870);
                pnlAllSettings.MaximumSize = new System.Drawing.Size(0, 870);
                scaleFactor = 588f / 600f;

                Eval(@"const style = document.createElement('style');
    style.textContent = `
     
                     [name= 'chkXbox'] [name= 'icon'], [name= 'chkKeyboard'] [name= 'icon'], [name= 'nintendoSwitch'] [name= 'icon']{ margin-top:-3px!important; }
`;
    document.head.appendChild(style);");




            }
            else
            {

                Eval(@"var s = document.createElement('style');
                s.textContent = '[name=""captionbar""] [name=""title""] { overflow: visible !important; font-size: 16pt !important; }';
                document.head.appendChild(s);");
                pnlAllSettings.Height = 1230;
                pnlAllSettings.MinimumSize = new System.Drawing.Size(0, 1230);
                pnlAllSettings.MaximumSize = new System.Drawing.Size(0, 1230);
                scaleFactor = 828f / 600f;
                Eval(@"const style = document.createElement('style');
    style.textContent = `
     
      [name='lblActiveUpper'], [name='lblActiveMiddle'], [name='lblActiveLower'] {
        margin-top: 7px!important;
      }`;
    document.head.appendChild(style);");

            }

            Eval(@"var existingDisabledComboStyle = document.getElementById('globalDisabledAssignmentCombos');
if (!existingDisabledComboStyle) {
        var disabledComboStyle = document.createElement('style');
        disabledComboStyle.id = 'globalDisabledAssignmentCombos';
        disabledComboStyle.textContent = `
            .border-button.qx-combobox-disabled-borderSolid-dropDownList,
            .qx-combobox-disabled-borderSolid-dropDownList.border-button,
            .border-button.qx-button-disabled-borderSolid,
            .qx-button-disabled-borderSolid.border-button,
            .border-button.qx-textbox-disabled-borderSolid,
            .qx-textbox-disabled-borderSolid.border-button {
                opacity: 0.8 !important;
                background-color: #60707c !important;
                color: #eef4f7 !important;
                cursor: not-allowed !important;
            }
        `;
        document.head.appendChild(disabledComboStyle);
}");

            pnlQuadrants.Scale(scaleFactor);
            pnlTop.Scale(scaleFactor);
            pnlMiddle.Scale(scaleFactor);
            pnlBottom.Scale(scaleFactor);
            pnlDZImage.Scale(scaleFactor);
            pnlDeadzone.Scale(scaleFactor);
            pnlVersion.Scale(scaleFactor);
            Logo.Scale(scaleFactor);
            pnlStep1.Scale(scaleFactor);
            pnlStep2.Scale(scaleFactor);
            pnlModes.Scale(scaleFactor);
            pnlDonate.Scale(scaleFactor);
            pnlOurOdyssey.Scale(scaleFactor);
            pnlControls.Scale(scaleFactor);
            pnlTabs.Scale(scaleFactor);
            backupPanel.Scale(scaleFactor);

            ScaleFonts(backupPanel, scaleFactor);
            ScaleFonts(pnlStep1, scaleFactor);
            ScaleFonts(pnlStep2, scaleFactor);
            ScaleFonts(pnlModes, scaleFactor);
            ScaleFonts(pnlDonate, scaleFactor);
            ScaleFonts(pnlQuadrants, scaleFactor);
            ScaleFonts(pnlDZImage, scaleFactor);
            ScaleFonts(pnlDeadzone, scaleFactor);
            ScaleFonts(pnlTop, scaleFactor);
            ScaleFonts(pnlMiddle, scaleFactor);
            ScaleFonts(pnlBottom, scaleFactor);
            ScaleFonts(pnlVersion, scaleFactor);
            ScaleFonts(pnlOurOdyssey, scaleFactor);
            ScaleFonts(pnlControls, scaleFactor);

            btnSubmit.CssClass = "highlight";
            txtConfigName.CssClass = "highlight";


            Eval("document.title = 'Overjoyed Settings';");

            SetSvgBackgroundButton(disableUpRightHover, appFolderPath + "\\images\\powersymbol.svg", "contain");
            SetSvgBackgroundButton(disableRightHover, appFolderPath + "\\images\\powersymbol.svg", "contain");
            SetSvgBackgroundButton(disableDownRightHover, appFolderPath + "\\images\\powersymbol.svg", "contain");
            SetSvgBackgroundButton(disableDownHover, appFolderPath + "\\images\\powersymbol.svg", "contain");
            SetSvgBackgroundButton(disableDownLeftHover, appFolderPath + "\\images\\powersymbol.svg", "contain");
            SetSvgBackgroundButton(disableLeftHover, appFolderPath + "\\images\\powersymbol.svg", "contain");
            SetSvgBackgroundButton(disableUpLeftHover, appFolderPath + "\\images\\powersymbol.svg", "contain");
            SetSvgBackgroundButton(disableUpHover, appFolderPath + "\\images\\powersymbol.svg", "contain");
            SetSvgBackgroundButton(disableDZUpperHover, appFolderPath + "\\images\\powersymbol.svg", "contain");
            SetSvgBackgroundButton(disableDZMiddleHover, appFolderPath + "\\images\\powersymbol.svg", "contain");
            SetSvgBackgroundButton(disableDZLowerHover, appFolderPath + "\\images\\powersymbol.svg", "contain");

            SetSvgBackgroundButton(disableUpRightLC, appFolderPath + "\\images\\powersymbol.svg", "contain");
            SetSvgBackgroundButton(disableRightLC, appFolderPath + "\\images\\powersymbol.svg", "contain");
            SetSvgBackgroundButton(disableDownRightLC, appFolderPath + "\\images\\powersymbol.svg", "contain");
            SetSvgBackgroundButton(disableDownLC, appFolderPath + "\\images\\powersymbol.svg", "contain");
            SetSvgBackgroundButton(disableDownLeftLC, appFolderPath + "\\images\\powersymbol.svg", "contain");
            SetSvgBackgroundButton(disableLeftLC, appFolderPath + "\\images\\powersymbol.svg", "contain");
            SetSvgBackgroundButton(disableUpLeftLC, appFolderPath + "\\images\\powersymbol.svg", "contain");
            SetSvgBackgroundButton(disableUpLC, appFolderPath + "\\images\\powersymbol.svg", "contain");
            SetSvgBackgroundButton(disableDZUpperLC, appFolderPath + "\\images\\powersymbol.svg", "contain");
            SetSvgBackgroundButton(disableDZMiddleLC, appFolderPath + "\\images\\powersymbol.svg", "contain");
            SetSvgBackgroundButton(disableDZLowerLC, appFolderPath + "\\images\\powersymbol.svg", "contain");

            SetSvgBackgroundButton(disableUpRightRC, appFolderPath + "\\images\\powersymbol.svg", "contain");
            SetSvgBackgroundButton(disableRightRC, appFolderPath + "\\images\\powersymbol.svg", "contain");
            SetSvgBackgroundButton(disableDownRightRC, appFolderPath + "\\images\\powersymbol.svg", "contain");
            SetSvgBackgroundButton(disableDownRC, appFolderPath + "\\images\\powersymbol.svg", "contain");
            SetSvgBackgroundButton(disableDownLeftRC, appFolderPath + "\\images\\powersymbol.svg", "contain");
            SetSvgBackgroundButton(disableLeftRC, appFolderPath + "\\images\\powersymbol.svg", "contain");
            SetSvgBackgroundButton(disableUpLeftRC, appFolderPath + "\\images\\powersymbol.svg", "contain");
            SetSvgBackgroundButton(disableUpRC, appFolderPath + "\\images\\powersymbol.svg", "contain");
            SetSvgBackgroundButton(disableDZUpperRC, appFolderPath + "\\images\\powersymbol.svg", "contain");
            SetSvgBackgroundButton(disableDZMiddleRC, appFolderPath + "\\images\\powersymbol.svg", "contain");
            SetSvgBackgroundButton(disableDZLowerRC, appFolderPath + "\\images\\powersymbol.svg", "contain");


            SetSvgBackground(pnlQuadrants, appFolderPath + "\\images\\Overjoyed-Interface.svg", "quadrants");

            SetSvgBackground(pnlDZImage, appFolderPath + "\\images\\Overjoyed-DZ.svg", "special");

            SetSvgBackground(Logo, appFolderPath + "\\images\\Overjoyed-Logo.svg", "cover");

            SetSvgBackground(btnRename, appFolderPath + "\\images\\btnRenameImage.svg", "cover");

            SetSvgBackground(btnDelete, appFolderPath + "\\images\\btnDeleteImage.svg", "cover");

            keyboardButtons = new Control[] { btnUpHover, btnUpRightHover, btnRightHover, btnDownRightHover, btnDownHover, btnDownLeftHover, btnLeftHover, btnUpLeftHover, btnUpLclick, btnUpRightLclick, btnRightLclick, btnDownRightLclick, btnDownLclick, btnDownLeftLclick, btnLeftLclick, btnUpLeftLclick, btnUpRclick, btnUpRightRclick, btnRightRclick, btnDownRightRclick, btnDownRclick, btnDownLeftRclick, btnLeftRclick, btnUpLeftRclick, btnDZUpperHover, btnDZUpperLC, btnDZUpperRC, btnDZMiddleHover, btnDZMiddleLC, btnDZMiddleRC, btnDZLowerHover, btnDZLowerLC, btnDZLowerRC };

            string line = null; string[] linesArray = null;

            try
            {

                // Open the file for read/write operations
                using (FileStream fileStream = new FileStream(parametersFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    // Read all lines into a list and convert it to an array
                    var linesList = new List<string>();
                    while ((line = reader.ReadLine()) != null)
                    {
                        linesList.Add(line);
                    }
                    linesArray = linesList.ToArray();

                    // Optional: Output the lines for verification
                    Console.WriteLine("Lines read from the file:");
                    foreach (var l in linesArray)
                    {
                        Console.WriteLine(l);
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IO Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }

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

            cmbProfile.SelectedIndex = int.Parse(linesArray[0]);
            lastProfileIndex = cmbProfile.SelectedIndex;

            this.Show();
            string lineTour = File.ReadLines(tourFilePath).ElementAtOrDefault(0);

            if (lineTour == "tourYes")
            {
                NewBackup();
                GuidedTour();
            }


            optUp.SelectedIndex = 0;
            optUpRight.SelectedIndex = 0;
            optRight.SelectedIndex = 0;
            optDownRight.SelectedIndex = 0;
            optDown.SelectedIndex = 0;
            optDownLeft.SelectedIndex = 0;
            optLeft.SelectedIndex = 0;
            optUpLeft.SelectedIndex = 0;
            optDZUpper.SelectedIndex = 0;
            optDZMiddle.SelectedIndex = 0;
            optDZLower.SelectedIndex = 0;

            btnHover.PerformClick();


        }

        [WebMethod]
        public void onLanguageChange()
        {
            ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 6; -webkit-box-orient: vertical; overflow: hidden;' title='Your language has been changed. If a word is cut off or a paragraph has 3 dots (...) at the end of it, please hover your mouse over it to see the full text. If you have any issues, please press the Click To Provide Feedback button bottom left. Thank you!'>Your language has been changed. If a word is cut off or a paragraph has 3 dots (...) at the end of it, please hover your mouse over it to see the full text. If you have any issues, please press the Click To Provide Feedback button bottom left. Thank you!</span></p>", "Language Changed");
        }

        [WebMethod]
        public void currentLanguage(string current)
        {
            currentLang = current;
        }







        private void SaveCurrentHtmlSnapshot(object result)
        {
            //if (result is not string htmlSource || string.IsNullOrWhiteSpace(htmlSource))
            //{
            //    return;
            //}

            //try
            //{
            //    string sourceProfilesPath = GetSourceProfilesPath();
            //    if (string.IsNullOrEmpty(sourceProfilesPath))
            //    {
            //        Console.WriteLine("Failed to resolve source profiles folder for Current.txt.");
            //        return;
            //    }

            //    Directory.CreateDirectory(sourceProfilesPath);
            //    File.WriteAllText(Path.Combine(sourceProfilesPath, "Current.txt"), htmlSource);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"Failed to write Current.txt: {ex.Message}");
            //}
        }

        private string GetSourceProfilesPath()
        {
            var currentDirectory = new DirectoryInfo(appFolderPath);

            while (currentDirectory != null)
            {
                string solutionPath = Path.Combine(currentDirectory.FullName, "OverjoyedRelease.sln");
                string clientProfilesPath = Path.Combine(currentDirectory.FullName, "OverjoyedRelease-Client", "profiles");

                if (File.Exists(solutionPath) && Directory.Exists(Path.GetDirectoryName(clientProfilesPath)))
                {
                    return clientProfilesPath;
                }

                currentDirectory = currentDirectory.Parent;
            }

            return null;
        }











        private void Config_FormClosing(object sender, FormClosingEventArgs e)
        {
            string line1, line2;

            line1 = File.ReadLines(parametersFilePath).ElementAtOrDefault(0);
            line2 = File.ReadLines(parametersFilePath).ElementAtOrDefault(1);

            try
            {

                // Open the file with shared read/write access
                using (FileStream fs = new FileStream(parametersFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                using (StreamWriter file2 = new StreamWriter(fs))
                {
                    if (line1 != null)
                        file2.WriteLine(line1);
                    if (line2 != null)
                        file2.WriteLine(line2);
                    file2.WriteLine("config");
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