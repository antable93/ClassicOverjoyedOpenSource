using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Wisej.Web;


namespace ExtendedMessageBoxLibrary
{
    /// <summary>
    /// Displays a message to the user in a modal dialog box
    /// blocking other actions in the application until it is
    /// dismissed. This extended messagebox contains everything
    /// provided in the System.Windows Forms.Messagebox dialog
    /// but extends it in two ways: it allows for a chackbox 
    /// and a timeout feature.
    /// </summary>
    public static class ExtendedMessageBox
    {
        #region PINVOKE STUFF FOR ICONS
        private const int MAX_PATH = 260;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);
        [DllImport("Shell32.dll", SetLastError = false)]
        private static extern Int32 SHGetStockIconInfo(SHSTOCKICONID siid, SHGSI uFlags, ref SHSTOCKICONINFO psii);
        [Flags]
        private enum SHGSI : uint
        {
            SHGSI_ICONLOCATION = 0,
            SHGSI_ICON = 0x000000100,
            SHGSI_SYSICONINDEX = 0x000004000,
            SHGSI_LINKOVERLAY = 0x000008000,
            SHGSI_SELECTED = 0x000010000,
            SHGSI_LARGEICON = 0x000000000,
            SHGSI_SMALLICON = 0x000000001,
            SHGSI_SHELLICONSIZE = 0x000000004
        }
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct SHSTOCKICONINFO
        {
            public UInt32 cbSize;
            public IntPtr hIcon;
            public Int32 iSysIconIndex;
            public Int32 iIcon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string szPath;
        }

        private enum SHSTOCKICONID : uint
        {
            SIID_DOCNOASSOC = 0,
            SIID_DOCASSOC = 1,
            SIID_APPLICATION = 2,
            SIID_FOLDER = 3,
            SIID_FOLDEROPEN = 4,
            SIID_DRIVE525 = 5,
            SIID_DRIVE35 = 6,
            SIID_DRIVEREMOVE = 7,
            SIID_DRIVEFIXED = 8,
            SIID_DRIVENET = 9,
            SIID_DRIVENETDISABLED = 10,
            SIID_DRIVECD = 11,
            SIID_DRIVERAM = 12,
            SIID_WORLD = 13,
            SIID_SERVER = 15,
            SIID_PRINTER = 16,
            SIID_MYNETWORK = 17,
            SIID_FIND = 22,
            SIID_HELP = 23,
            SIID_SHARE = 28,
            SIID_LINK = 29,
            SIID_SLOWFILE = 30,
            SIID_RECYCLER = 31,
            SIID_RECYCLERFULL = 32,
            SIID_MEDIACDAUDIO = 40,
            SIID_LOCK = 47,
            SIID_AUTOLIST = 49,
            SIID_PRINTERNET = 50,
            SIID_SERVERSHARE = 51,
            SIID_PRINTERFAX = 52,
            SIID_PRINTERFAXNET = 53,
            SIID_PRINTERFILE = 54,
            SIID_STACK = 55,
            SIID_MEDIASVCD = 56,
            SIID_STUFFEDFOLDER = 57,
            SIID_DRIVEUNKNOWN = 58,
            SIID_DRIVEDVD = 59,
            SIID_MEDIADVD = 60,
            SIID_MEDIADVDRAM = 61,
            SIID_MEDIADVDRW = 62,
            SIID_MEDIADVDR = 63,
            SIID_MEDIADVDROM = 64,
            SIID_MEDIACDAUDIOPLUS = 65,
            SIID_MEDIACDRW = 66,
            SIID_MEDIACDR = 67,
            SIID_MEDIACDBURN = 68,
            SIID_MEDIABLANKCD = 69,
            SIID_MEDIACDROM = 70,
            SIID_AUDIOFILES = 71,
            SIID_IMAGEFILES = 72,
            SIID_VIDEOFILES = 73,
            SIID_MIXEDFILES = 74,
            SIID_FOLDERBACK = 75,
            SIID_FOLDERFRONT = 76,
            SIID_SHIELD = 77,
            SIID_WARNING = 78,
            SIID_INFO = 79,
            SIID_ERROR = 80,
            SIID_KEY = 81,
            SIID_SOFTWARE = 82,
            SIID_RENAME = 83,
            SIID_DELETE = 84,
            SIID_MEDIAAUDIODVD = 85,
            SIID_MEDIAMOVIEDVD = 86,
            SIID_MEDIAENHANCEDCD = 87,
            SIID_MEDIAENHANCEDDVD = 88,
            SIID_MEDIAHDDVD = 89,
            SIID_MEDIABLURAY = 90,
            SIID_MEDIAVCD = 91,
            SIID_MEDIADVDPLUSR = 92,
            SIID_MEDIADVDPLUSRW = 93,
            SIID_DESKTOPPC = 94,
            SIID_MOBILEPC = 95,
            SIID_USERS = 96,
            SIID_MEDIASMARTMEDIA = 97,
            SIID_MEDIACOMPACTFLASH = 98,
            SIID_DEVICECELLPHONE = 99,
            SIID_DEVICECAMERA = 100,
            SIID_DEVICEVIDEOCAMERA = 101,
            SIID_DEVICEAUDIOPLAYER = 102,
            SIID_NETWORKCONNECT = 103,
            SIID_INTERNET = 104,
            SIID_ZIPFILE = 105,
            SIID_SETTINGS = 106,
            SIID_DRIVEHDDVD = 132,
            SIID_DRIVEBD = 133,
            SIID_MEDIAHDDVDROM = 134,
            SIID_MEDIAHDDVDR = 135,
            SIID_MEDIAHDDVDRAM = 136,
            SIID_MEDIABDROM = 137,
            SIID_MEDIABDR = 138,
            SIID_MEDIABDRE = 139,
            SIID_CLUSTEREDDRIVE = 140,
            SIID_MAX_ICONS = 175
        }
        #endregion
        private static int PICTURE_WIDTH = 32;
        private static int PICTURE_HEIGHT = 32;
        private static int BUTTON_WIDTH = 115;
        private static int BUTTON_HEIGHT = 50;
        private static int PADDING = 10;

        private static Form MobjForm = null;
        private static bool MbolChecked = false;
        private static bool MbolTimeout = false;
        private static string MstrMoreText = "";
        private static double MintDPI = 1;
        private static bool isDeadzone = false;
        private static bool isOn = false;

        /// <summary>
        /// Displays a messagebox
        /// </summary>
        /// <param name="text">The text to display in the message box</param>
        /// <param name="caption">Text to display in the title bar of the message box</param>
        /// <param name="buttons">Which buttons to display in the message box</param>
        /// <param name="icon">Which icon to show in the message box</param>
        /// <param name="checkboxtext">Text to display in the checkbox. If left empty, no checkbox will appear.</param>
        /// <param name="hyperlink">A Hyperlink object (text and link), that will appear below the message</param>
        /// <param name="moredata">If provided this will show a More button which will expand the form to show more text</param>
        /// <param name="timeout">A value in seconds until the dialog self dismisses with a dialog result of cancel and timeout as true. No timeout will occur if no value is specified.</param>
        /// <returns>An extended dialog result object wiht a traditional dialog result, the state of the checkbox and whether the form timed out or the user took action.</returns>
        public static ExtendedDialogResult Show(string text,
                                           string caption = "",
                                           MessageBoxButtons buttons = MessageBoxButtons.OK,
                                           MessageBoxIcon icon = MessageBoxIcon.None,
                                           string checkboxtext = "",
                                           Hyperlink hyperlink = null,
                                           string moredata = null,
                                           int timeout = int.MaxValue,
                                           bool isDZ = false,
                                           bool turnOn = false,
                                               int? lines = 1, // NEW PARAMETER: number of lines to make room for
                                               double zoomFactor = 1.0
)
        {
            lines = GetWebkitLineClampValue(text); // clamp == 3

            isDeadzone = isDZ;
            isOn = turnOn;
            MbolChecked = false;
            MbolTimeout = false;
            MstrMoreText = "";
            Form dialogForm = null;

            try
            {
                MobjForm = CreateForm(text, caption, buttons, icon, checkboxtext, hyperlink, moredata, timeout, (int)lines, zoomFactor);
                dialogForm = MobjForm;
                if (Screen.Bounds.Height >= 1100 && Screen.Bounds.Width > 1920)
                {
                    // Replace the line containing Eval with the following code block:

                    MobjForm.Scale(1.4f); // Scale the form to 140% for better visibility
                    ScaleFonts(MobjForm, 1.4f); // Scale the fonts to match the form scaling
                }

                MobjForm.Eval(@"
  (function() {
    function applyMarquee(el) {  if ((el.dataset.marqueeApplied === 'true' && el.querySelector('.marquee-outer')) || el.querySelector('.truncate')){ return;}

      const text = el.textContent.trim();
      if (!text) return;

      el.style.whiteSpace = 'nowrap';
      const cs = getComputedStyle(el);
      const wasCentered = cs.textAlign === 'center';

      el.textContent = '';
      const outer = document.createElement('span');
      outer.className = 'marquee-outer';
      outer.style.display = 'block';
      outer.style.width = '100%';
if (wasCentered) {
        outer.style.textAlign = 'center';
      }
else {
        outer.style.textAlign = 'left';
      }

      const inner = document.createElement('span');
      inner.className = 'marquee-inner';

      const s1 = document.createElement('span');
      s1.textContent = text;
      const s2 = document.createElement('span');
      s2.textContent = text;

      inner.appendChild(s1);
      inner.appendChild(s2);
      outer.appendChild(inner);
      el.appendChild(outer);

      s1.style.display = 'inline-flex';
      if (wasCentered) {
        const space = el.clientWidth - s1.offsetWidth;
        s1.style.paddingLeft = (space / 2) + 'px';
      }

      el.dataset.marqueeApplied = 'true';
    }

    // Run immediately inside this same context
    setTimeout(() => {
      console.log('Applying marquee after 2s...');
      document.querySelectorAll('[name^=""label""], [name^=""title""]').forEach(el => {
        applyMarquee(el);
      });
    }, 2000);
  })();
//");
                MobjForm.Eval(@"
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

                DialogResult LobjResult = dialogForm.ShowDialog();


                ExtendedDialogResult LobjReturnValue = new ExtendedDialogResult(LobjResult, MbolChecked, MbolTimeout);
                return LobjReturnValue;
            }
            catch (Exception PobjEx)
            {
                throw PobjEx; // pass it along
            }
            finally
            {
                if (dialogForm != null)
                {
                    dialogForm.Dispose();
                }

                if (ReferenceEquals(MobjForm, dialogForm))
                {
                    MobjForm = null;
                }
            }
        }

        public static int? GetWebkitLineClampValue(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            var match = System.Text.RegularExpressions.Regex.Match(
                input,
                @"-webkit-line-clamp:\s*(\d+)",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );

            if (match.Success && int.TryParse(match.Groups[1].Value, out int value))
                return value;

            return null;
        }

        private static void ScaleFonts(Control parent, float scale)
        {


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

        /// <summary>
        /// Creates the form for display
        /// </summary>
        /// <param name="PstrText"></param>
        /// <param name="PstrTitle"></param>
        /// <param name="PobjButtons"></param>
        /// <param name="PobjIcon"></param>
        /// <param name="PstrCheckboxText"></param>
        /// <param name="PintTimeout"></param>
        /// <returns></returns>
        private static Form CreateForm(
    string PstrText, string PstrTitle,
    MessageBoxButtons PobjButtons,
    MessageBoxIcon PobjIcon,
    string PstrCheckboxText,
    Hyperlink PobjHyperlink,
    string PstrMore,
    int PintTimeout, int lines, double zoomFactor // NEW PARAMETER
)
        {
            try
            {
                Form LobjForm = new Form();
                MintDPI = 1;
                LobjForm.StartPosition = FormStartPosition.CenterScreen;
                LobjForm.MinimizeBox = false;
                LobjForm.MaximizeBox = false;
                LobjForm.Movable = false;
                LobjForm.FormBorderStyle = FormBorderStyle.Fixed;
                LobjForm.ControlBox = false;
                LobjForm.AllowDrag = false;
                LobjForm.AllowDrop = false;
                LobjForm.AutoSize = true;
                LobjForm.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                LobjForm.Text = ((PintTimeout < int.MaxValue && PintTimeout > 0) ?
                          PstrTitle + " [" + (PintTimeout).ToString() + "]" :
                          PstrTitle);

                // Create the panels
                Panel LobjTopPanel = GetTopPanel(PobjIcon, PstrText, PobjHyperlink, lines, zoomFactor);
                Panel LobjButtonPanel = GetButtonPanel(PobjButtons, PstrCheckboxText, PstrMore);

                LobjForm.Height = LobjTopPanel.Height + LobjButtonPanel.Height + 30;
                LobjForm.MinimumSize = new Size((int)(470*zoomFactor), 0);
                LobjForm.Controls.Add(LobjButtonPanel);
                LobjForm.Controls.Add(LobjTopPanel);

                // On load we need to arrange a few things since we
                // get sized and modified on first paint
                LobjForm.Load += (o, e) => { ArrangeForm(); };

                // create the timer
                if (PintTimeout > 0 && PintTimeout < int.MaxValue)
                {
                    Timer LobjTimer = new Timer();
                    LobjTimer.Interval = 1000;
                    int LintCount = 0;
                    LobjTimer.Tick += (o, e) =>
                    {
                        LintCount++;
                        if (LintCount >= PintTimeout)
                        {
                            MbolTimeout = true;
                            LobjForm.DialogResult = DialogResult.Cancel;
                            LobjForm.Close();
                        }
                        else
                        {
                            // update the title bar
                            LobjForm.Text = PstrTitle + " [" + (PintTimeout - LintCount).ToString() + "]";
                        }
                    };
                    LobjTimer.Start();
                }
                return LobjForm;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// After the form is drawn, we need to arrange
        /// </summary>
        private static void ArrangeForm()
        {
            try
            {
                int buttonsPerRow = 3;
                int minButtonWidth = 110;
                int minButtonHeight = 50;
                int padding = 10;

                Panel panelButtons = MobjForm.Controls["panelButtons"] as Panel;
                if (panelButtons == null) return;

                // Collect all buttons
                List<Button> buttons = new List<Button>();
                foreach (Control control in panelButtons.Controls)
                {
                    if (control is Button button)
                    {
                        button.MinimumSize = new Size(minButtonWidth, minButtonHeight);
                        button.TextAlign = ContentAlignment.MiddleCenter;
                        button.Dock = DockStyle.Fill; // Make button fill its cell
                        buttons.Add(button);
                    }
                }
                if (buttons.Count == 0) return;

                // Remove all controls from panelButtons
                panelButtons.Controls.Clear();
                panelButtons.AutoSize = false; // Ensure panel can be filled

                // Calculate rows and columns
                int rows = (int)Math.Ceiling((double)buttons.Count / buttonsPerRow);

                // Create TableLayoutPanel
                var table = new Wisej.Web.TableLayoutPanel
                {
                    RowCount = rows,
                    ColumnCount = buttonsPerRow,
                    Dock = DockStyle.Fill,
                    Padding = new Padding(padding),
                    BackColor = panelButtons.BackColor,
                    AutoSize = false,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                };

                // Set column and row styles to fill
                table.ColumnStyles.Clear();
                for (int c = 0; c < buttonsPerRow; c++)
                    table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / buttonsPerRow));
                table.RowStyles.Clear();
                for (int r = 0; r < rows; r++)
                    table.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / rows));

                // Add buttons to the table
                int btnIndex = 0;
                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < buttonsPerRow; c++)
                    {
                        if (btnIndex < buttons.Count)
                        {
                            table.Controls.Add(buttons[btnIndex], c, r);
                            btnIndex++;
                        }
                    }
                }

                panelButtons.Controls.Add(table);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// Gets the height of the title bar
        /// </summary>
        /// <param name="PobjForm"></param>
        /// <returns></returns>
        private static int GetTitleBarHeight(Form PobjForm)
        {
            try
            {
                Rectangle LobjScreenRectangle = PobjForm.RectangleToScreen(PobjForm.ClientRectangle);
                return LobjScreenRectangle.Top - PobjForm.Top;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Returns the top panel of the message box
        /// ICON + TEXT
        /// </summary>
        /// <returns></returns>
        private static Panel GetTopPanel(MessageBoxIcon PobjIcon, string PstrText, Hyperlink PobjHyperlink, int lines, double zoomFactor)
        {
            try
            {
                var table = new Wisej.Web.TableLayoutPanel
                {
                    Name = "panelTop",
                    Dock = DockStyle.Top,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    ColumnCount = 2,
                    RowCount = PobjHyperlink != null ? 2 : 1,
                    Padding = new Padding(PADDING, PADDING, PADDING, PADDING),
                    BackColor = Color.White
                };

                // Set column styles: icon (auto), text (fill)
                table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
                table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                if (PobjHyperlink != null)
                    table.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                // Icon
                if (PobjIcon != MessageBoxIcon.None)
                {
                    var iconBox = new PictureBox
                    {
                        Width = PICTURE_WIDTH,
                        Height = PICTURE_HEIGHT,
                        SizeMode = PictureBoxSizeMode.CenterImage,
                        Image = GetIcon(PobjIcon),
                        Anchor = AnchorStyles.Top | AnchorStyles.Left,
                        Margin = new Padding(0, 0, PADDING, 0)
                    };
                    table.Controls.Add(iconBox, 0, 0);
                }
                else
                {
                    // Add an empty panel to keep the text aligned if no icon
                    table.Controls.Add(new Panel { Width = 1, Height = 1, Margin = Padding.Empty }, 0, 0);
                }


                // Calculate minimum height for the label based on lines and font size
                float fontSize = 12F;

                int lineHeight = (int)(fontSize*2*1.2); // Approximate line height
                if (Screen.Bounds.Height >= 1100 && Screen.Bounds.Width > 1920)
                {
                    lineHeight = (int)(fontSize *0.9*1.2); // Adjust line height for larger screens
                }
                    int minHeight = (int)(lineHeight*lines*zoomFactor);


                // Text
                var label = new Label
                {
                    Text = PstrText, // Removed "\n\n" to avoid extra space
                    AutoSize = false,
                    Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point),
                    BorderStyle = BorderStyle.None,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                    Margin = new Padding(0, 10, 0, 10),
                    AllowHtml = true,
                    MinimumSize = new Size(0, minHeight),
                    MaximumSize = new Size(0, minHeight) // Limit max width for better readability


                };
                if (Screen.Bounds.Height >= 1100 && Screen.Bounds.Width > 1920)
                {
                    label.Margin = new Padding(0, 3, 0, 3);
                }
                    table.Controls.Add(label, 1, 0);

                // Hyperlink (optional, on a new row)
                if (PobjHyperlink != null)
                {
                    var link = new LinkLabel
                    {
                        Text = PobjHyperlink.HyperlinkText,
                        AutoSize = true,
                        Anchor = AnchorStyles.Left,
                        Margin = new Padding(0, PADDING / 2, 0, 0)
                    };
                    link.Click += (s, e) =>
                    {
                        try { Process.Start(PobjHyperlink.HyperlinkLinkData); }
                        catch { }
                    };
                    // Add empty cell for icon column, then link in text column
                    table.Controls.Add(new Panel { Width = 1, Height = 1, Margin = Padding.Empty }, 0, 1);
                    table.Controls.Add(link, 1, 1);
                }

                // Wrap in a panel for compatibility with the rest of your code
                var panel = new Panel
                {
                    Name = "panelTop",
                    Dock = DockStyle.Top,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    BackColor = SystemColors.Control,
                    Padding = Padding.Empty // Remove extra padding
                };
                panel.Controls.Add(table);
                return panel;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Creates the button panel for each button
        /// [YES] [NO] [CANCEL]
        /// </summary>
        /// <param name="PobjButtons"></param>
        /// <returns></returns>
        private static Panel GetButtonPanel(MessageBoxButtons PobjButtons,
                                    string PstrCheckboxText,
                                    string PstrMore)
        {
            try
            {
                List<Button> LobjButtonsList = new List<Button>();
                switch (PobjButtons)
                {
                    case MessageBoxButtons.AbortRetryIgnore:
                        if (!isOn)
                        {
                            if (!isDeadzone)
                            {
                                LobjButtonsList.Add(CreateButton(DialogResult.Abort, "<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Turn On for This Quadrant Only'>Turn On for This Quadrant Only</span></p>"));
                                LobjButtonsList.Add(CreateButton(DialogResult.Retry, "<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Turn On for All Outer Quadrants'>Turn On for All Outer Quadrants</span></p>"));
                                LobjButtonsList.Add(CreateButton(DialogResult.Ignore, "No Thanks"));
                            }
                            else
                            {
                                LobjButtonsList.Add(CreateButton(DialogResult.Abort, "<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Turn On for This Quadrant Only'>Turn On for This Quadrant Only</span></p>"));
                                LobjButtonsList.Add(CreateButton(DialogResult.Retry, "<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Turn On for All Inner Quadrants'>Turn On for All Inner Quadrants</span></p>"));
                                LobjButtonsList.Add(CreateButton(DialogResult.Ignore, "No Thanks"));
                            }
                        }
                        else
                        {



                            if (!isDeadzone)
                            {
                                LobjButtonsList.Add(CreateButton(DialogResult.Abort, "<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Turn Off for This Quadrant Only'>Turn Off for This Quadrant Only</span></p>"));
                                LobjButtonsList.Add(CreateButton(DialogResult.Retry, "<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Turn Off for All Outer Quadrants'>Turn Off for All Outer Quadrants</span></p>"));
                                LobjButtonsList.Add(CreateButton(DialogResult.Ignore, "No Thanks"));
                            }
                            else
                            {
                                LobjButtonsList.Add(CreateButton(DialogResult.Abort, "<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Turn Off for This Quadrant Only'>Turn Off for This Quadrant Only</span></p>Turn Off for This Quadrant Only"));
                                LobjButtonsList.Add(CreateButton(DialogResult.Retry, "<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; ' title='Turn Off for All Inner Quadrants'>Turn Off for All Inner Quadrants</span></p>"));
                                LobjButtonsList.Add(CreateButton(DialogResult.Ignore, "No Thanks"));
                            }
                        }
                        break;
                    case MessageBoxButtons.OK:
                        LobjButtonsList.Add(CreateButton(DialogResult.OK, "Okay"));
                        break;
                    case MessageBoxButtons.OKCancel:
                        LobjButtonsList.Add(CreateButton(DialogResult.OK, "Okay"));
                        LobjButtonsList.Add(CreateButton(DialogResult.Cancel, "Cancel"));
                        break;
                    case MessageBoxButtons.RetryCancel:
                        LobjButtonsList.Add(CreateButton(DialogResult.Retry, "Retry"));
                        LobjButtonsList.Add(CreateButton(DialogResult.Cancel, "Cancel"));
                        break;
                    case MessageBoxButtons.YesNo:
                        LobjButtonsList.Add(CreateButton(DialogResult.Yes, "Yes"));
                        LobjButtonsList.Add(CreateButton(DialogResult.No, "No"));
                        break;
                    case MessageBoxButtons.YesNoCancel:
                        LobjButtonsList.Add(CreateButton(DialogResult.Yes, "Yes"));
                        LobjButtonsList.Add(CreateButton(DialogResult.No, "No"));
                        LobjButtonsList.Add(CreateButton(DialogResult.Cancel, "Cancel"));
                        break;
                }

                // Add the "More..." button if needed
                if (!string.IsNullOrEmpty(PstrMore))
                {
                    var moreButton = CreateButton(DialogResult.None);
                    moreButton.Text = PstrMore;
                    LobjButtonsList.Add(moreButton);
                }

                Panel LobjPanel = new Panel
                {
                    Name = "panelButtons",
                    BackColor = SystemColors.ControlLight,
                    Dock = DockStyle.Top,
                    MinimumSize = new Size(0, 80),
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink
                };

                int LintButtonCount = 0;
                int LintLeft = 0;
                int LintHeight = BUTTON_HEIGHT;
                int LintBoxRight = 0;

                // Add checkbox
                if (!string.IsNullOrEmpty(PstrCheckboxText))
                {
                    CheckBox LobjBox = new CheckBox
                    {
                        AutoSize = true,
                        Text = PstrCheckboxText,
                        Left = PADDING,
                        Top = PADDING,
                        Anchor = AnchorStyles.Top | AnchorStyles.Left
                    };
                    LobjBox.Click += (o, e) => { MbolChecked = LobjBox.Checked; };
                    LobjBox.SendToBack();
                    LobjPanel.Controls.Add(LobjBox);
                    LintBoxRight = LobjBox.Left + LobjBox.Width;
                }

                // Add buttons
                foreach (Button LobjButton in LobjButtonsList)
                {
                    LintLeft = (BUTTON_WIDTH) * LintButtonCount;
                    LobjButton.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);

                    // Increase button padding here
                    LobjButton.Padding = new Padding(8, 0, 8, 0); // Example: 16px left/right, 8px top/bottom

                    LobjButton.Left = LintLeft;
                    LobjButton.Top = PADDING / 2;
                    LobjPanel.Controls.Add(LobjButton);
                    LintButtonCount++;
                }

                // Adjust panel height if needed
                //if (LintBoxRight > 0 && LintBoxRight > LobjButtonsList[0].Left)
                //{
                //    foreach (Button LobjButton in LobjButtonsList)
                //    {
                //        LobjButton.Top += BUTTON_HEIGHT;
                //    }
                //    LobjPanel.Height = LobjButtonsList[0].Top + LobjButtonsList[0].Height + (PADDING/2);
                //}
                //else
                //{
                //    LobjPanel.Height = LintHeight + PADDING;
                //}



                return LobjPanel;
            }
            catch
            {
                return null;
            }
        }



        /// <summary>
        /// Expands the form
        /// </summary>
        private static void ExpandForm()
        {
            try
            {
                Panel LobjButtonPanel = MobjForm.Controls.Find("panelButtons", true)[0] as Panel;
                TextBox LobjBox = new TextBox();
                LobjBox.Width = MobjForm.Width - (PADDING * 3);
                LobjBox.Left = PADDING;
                LobjBox.Top = LobjButtonPanel.Height;
                LobjBox.ReadOnly = true;
                LobjBox.ScrollBars = ScrollBars.Vertical;
                LobjBox.Multiline = true;
                LobjBox.MinimumSize = new Size(LobjBox.Width, 120);
                LobjBox.Text = MstrMoreText;
                LobjBox.TabStop = false;
                LobjBox.BackColor = SystemColors.ButtonFace;
                LobjBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                //LobjButtonPanel.MaximumSize = new Size((int)(600), 
                //                                       LobjButtonPanel.Height + LobjBox.Height + PADDING);
                LobjButtonPanel.Height += LobjBox.Height + PADDING;
                // add to the bottom of the button panel
                LobjButtonPanel.Controls.Add(LobjBox);
                // diable the more button
                Button LobjMoreButton = MobjForm.Controls.Find("moreButton", true)[0] as Button;
                LobjMoreButton.Enabled = false;

            }
            catch (Exception PobjEx)
            {
                throw PobjEx;
            }
        }

        /// <summary>
        /// Creates a generic button based on the default text
        /// </summary>
        /// <param name="PobjDialogResult"></param>
        /// <returns></returns>
        private static Button CreateButton(DialogResult PobjDialogResult, string customText = null)
        {
            try
            {
                Button LobjButton = new Button();
                LobjButton.AllowHtml = true;
                if (PobjDialogResult != DialogResult.None)
                {
                    // Use the custom text if provided; otherwise, fall back to default text
                    LobjButton.Text = !string.IsNullOrEmpty(customText)
                                      ? customText
                                      : "&" + PobjDialogResult.ToString();

                    LobjButton.DialogResult = PobjDialogResult;

                    // EVENT to close the form on a button click
                    LobjButton.Click += (o, e) =>
                    {
                        Form ownerForm = LobjButton.FindForm();
                        if (ownerForm == null)
                        {
                            return;
                        }

                        ownerForm.DialogResult = PobjDialogResult;
                        ownerForm.Close();
                    };
                }
                else
                {
                    // Text for "More" button
                    LobjButton.Text = "&More >";
                    LobjButton.Click += (o, e) => { ExpandForm(); };
                    LobjButton.Name = "moreButton";
                }

                // Set properties for size and anchoring
                LobjButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                LobjButton.Width = BUTTON_WIDTH;
                LobjButton.Height = BUTTON_HEIGHT;

                return LobjButton;
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// Returns the bitmap image of the selected icon
        /// </summary>
        /// <param name="PobjIcon"></param>
        /// <returns></returns>
        private static Bitmap GetIconBitmap(MessageBoxIcon PobjIcon)
        {
            try
            {
                Bitmap LobjReturn = null;
                switch (PobjIcon)
                {
                    case MessageBoxIcon.Error:
                        //case MessageBoxIcon.Hand:
                        // case MessageBoxIcon.Stop:
                        LobjReturn = SystemIcons.Error.ToBitmap();
                        break;
                    case MessageBoxIcon.Information:
                        //case MessageBoxIcon.Asterisk:
                        LobjReturn = SystemIcons.Information.ToBitmap();
                        break;
                    case MessageBoxIcon.Warning:
                        //case MessageBoxIcon.Exclamation:
                        LobjReturn = SystemIcons.Warning.ToBitmap();
                        break;
                    case MessageBoxIcon.Question:
                        LobjReturn = SystemIcons.Question.ToBitmap();
                        break;
                }
                return ResizeBitmap(LobjReturn);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Resizes the system icon to a higher resolution
        /// </summary>
        /// <param name="PobjBitmap"></param>
        /// <returns></returns>
        private static Bitmap ResizeBitmap(Bitmap PobjBitmap)
        {
            try
            {
                Size LobjS = new Size(PICTURE_WIDTH, PICTURE_HEIGHT);
                Bitmap LobjBm = new Bitmap(LobjS.Width, LobjS.Height);

                using (Graphics LobjG = Graphics.FromImage(LobjBm))
                {
                    LobjG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    LobjG.DrawImage(PobjBitmap, new Rectangle(Point.Empty, LobjS));
                }

                return LobjBm;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// PINVOKES WINDOWS
        /// This gets the built in icons from Windows to display
        /// a better image for the form
        /// </summary>
        /// <param name="PobjIcon"></param>
        /// <returns></returns>
        private static Bitmap GetIcon(MessageBoxIcon PobjIcon)
        {
            try
            {
                SHSTOCKICONINFO LobjSii = new SHSTOCKICONINFO();
                LobjSii.cbSize = (UInt32)Marshal.SizeOf(typeof(SHSTOCKICONINFO));

                Marshal.ThrowExceptionForHR(SHGetStockIconInfo(ConvertIconCode(PobjIcon),
                     SHGSI.SHGSI_ICON | SHGSI.SHGSI_LARGEICON,
                     ref LobjSii));
                Bitmap LobjReturnBitmap = Icon.FromHandle(LobjSii.hIcon).ToBitmap();
                DestroyIcon(LobjSii.hIcon); // cleanup
                return LobjReturnBitmap;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the bitmap image of the selected icon
        /// </summary>
        /// <param name="PobjIcon"></param>
        /// <returns></returns>
        private static SHSTOCKICONID ConvertIconCode(MessageBoxIcon PobjIcon)
        {
            try
            {
                switch (PobjIcon)
                {
                    case MessageBoxIcon.Error:
                        //case MessageBoxIcon.Hand:
                        // case MessageBoxIcon.Stop:
                        return SHSTOCKICONID.SIID_ERROR;
                    case MessageBoxIcon.Information:
                        //case MessageBoxIcon.Asterisk:
                        return SHSTOCKICONID.SIID_INFO;
                    case MessageBoxIcon.Warning:
                        //case MessageBoxIcon.Exclamation:
                        return SHSTOCKICONID.SIID_WARNING;
                    case MessageBoxIcon.Question:
                        return SHSTOCKICONID.SIID_HELP;
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }
    }

    public enum MessageBoxButtons
    {
        None = 0, // <-- Add this line
        OK = 1,
        OKCancel = 2,
        AbortRetryIgnore = 3,
        YesNoCancel = 4,
        YesNo = 5,
        RetryCancel = 6
    }
}
