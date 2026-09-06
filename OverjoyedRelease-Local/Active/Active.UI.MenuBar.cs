using ExtendedMessageBoxLibrary;
using Microsoft.Maui.Storage;
using SharedVars;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Wisej.Web;

namespace OverjoyedReleaseLocal
{
    public partial class Active : Page
    {
        private TrackBar transparencySlider;
        private Panel menuBar;
        private double currentOpacity = 0.85;
        private int opacity;
        private float sizeScale = 1.0f;
        private Dictionary<string, Panel> dropdowns = new Dictionary<string, Panel>();
        private bool AltRTC = false;
        private bool Voice = false;
        private bool MarioKart = false;
        private bool FPS = false;
        private bool MouseLock = false;
        private bool FocusMouse = false;
        private bool suppressStreamerCheckedChanged = false;

        private void SetMenuBarVisibleForActiveState(bool overjoyedActive)
        {
            if (menuBar == null)
            {
                return;
            }

            menuBar.Visible = chkRemote.Checked || !overjoyedActive;

            if (overjoyedActive && !chkRemote.Checked)
            {
                foreach (var dropdown in dropdowns.Values)
                {
                    dropdown.Visible = false;
                }
            }
        }

        private void CreateMenuBar()
        {
            menuBar = new Panel
            {
                Dock = DockStyle.Top,
                BackColor = Color.White,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            pnlBaseOverlay.Controls.Add(menuBar);

            AddMenuItem("About", null);
            AddMenuItem("View", CreateViewDropdown());
            AddMenuItem("Advanced", CreateAdvancedDropdown());
            AddMenuItem("Help", null);
            AddMenuItem("Donate", null);

            SetMenuBarVisibleForActiveState(isActive);
        }

        private void AddMenuItem(string text, Panel dropdown)
        {
            var label = new Label
            {
                Text = text,
                AutoSize = true,
                ForeColor = Color.Black,
                Cursor = Cursors.Hand,
                Margin = new Padding(20, 10, 20, 10),
                Location = new Point(menuBar.Controls.Count * 100 + 60, 6),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Regular),
            };

            if (label.Text == "About")
            {
                label.Click += (s, e) =>
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "https://jettfoundation.org/",
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                };
            }
            else if (label.Text == "Help")
            {
                label.Click += (s, e) =>
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "https://forms.gle/JBfGJtkcLbDJejmK8",
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                };
            }
            else if (label.Text == "Advanced")
            {
                label.CssStyle += "margin-left:-3%!important;";
            }
            else if (label.Text == "Donate")
            {
                label.Click += (s, e) =>
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "https://ourodyssey.org/donate",
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                };
            }

            Wisej.Web.Timer closeTimer = new Wisej.Web.Timer();
            closeTimer.Interval = 150;
            closeTimer.Tick += (s, e) =>
            {
                closeTimer.Stop();
                if (dropdown != null && !(dropdown.Tag as bool? ?? false))
                {
                    dropdown.Visible = false;
                }
            };

            void SetDropdownMouseState(bool over)
            {
                if (dropdown != null)
                {
                    dropdown.Tag = over;
                    if (over)
                    {
                        closeTimer.Stop();
                    }
                    else
                    {
                        closeTimer.Start();
                    }
                }
            }

            if (dropdown != null)
            {
                label.MouseEnter += (s, e) =>
                {
                    foreach (var existingDropdown in dropdowns.Values)
                    {
                        existingDropdown.Visible = false;
                    }

                    SetDropdownMouseState(true);
                    dropdown.Location = new Point(label.Left, menuBar.Bottom);
                    dropdown.Visible = true;
                    dropdown.BringToFront();

                    Eval(@"function applyMarquee(el) {  if ((el.dataset.marqueeApplied === 'true' && el.querySelector('.marquee-outer')) || el.querySelector('.truncate')){ return;}

  let text = el.textContent.trim();
if (!text || text.length === 1) return;

el.style.whiteSpace = 'nowrap';

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

  s1.style.display = 'inline-flex';
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
}, 2000);
");
                };

                label.MouseLeave += (s, e) => SetDropdownMouseState(false);
            }

            menuBar.Controls.Add(label);
            if (dropdown != null)
            {
                dropdowns[text] = dropdown;
                dropdown.ForeColor = Color.White;
                dropdown.BackColor = Color.Black;
                dropdown.AutoSize = false;
                dropdown.Width = 250;
                Controls.Add(dropdown);

                EventHandler mouseEnter = (s, e) => SetDropdownMouseState(true);
                EventHandler mouseLeave = (s, e) => SetDropdownMouseState(false);

                dropdown.MouseEnter += mouseEnter;
                dropdown.MouseLeave += mouseLeave;

                foreach (Control control in dropdown.Controls)
                {
                    control.MouseEnter += mouseEnter;
                    control.MouseLeave += mouseLeave;
                }
            }
        }

        private void initializeTransparency()
        {
            if (transparencySlider.Value == 0)
            {
                opacity = 255;
            }
            else if (transparencySlider.Value == 100)
            {
                opacity = 100;
            }
            else
            {
                opacity = 255 - (int)((transparencySlider.Value / 100.0) * (255 - 100));
            }

            string alphaFilePath = Path.Combine(configPath, "Transparency.txt");

            try
            {
                using (FileStream fs = new FileStream(alphaFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                using (StreamWriter file2 = new StreamWriter(fs))
                {
                    file2.WriteLine(opacity);
                    file2.WriteLine(switchMode);
                    file2.WriteLine("true");
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
        }

        public void resetTransparency(bool oldOpacity, bool freeze = false)
        {
            if (freeze)
            {
                freezeSize();
            }

            if (transparencySlider != null)
            {
                if (oldOpacity)
                {
                    if (transparencySlider.Value == 0)
                    {
                        opacity = 255;
                    }
                    else if (transparencySlider.Value == 100)
                    {
                        opacity = 100;
                    }
                    else
                    {
                        opacity = 255 - (int)((transparencySlider.Value / 100.0) * (255 - 100));
                    }
                }
                else
                {
                    opacity = 255;
                }

                string alphaFilePath = Path.Combine(configPath, "Transparency.txt");

                try
                {
                    using (FileStream fs = new FileStream(alphaFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                    using (StreamWriter file2 = new StreamWriter(fs))
                    {
                        file2.WriteLine(opacity);
                        file2.WriteLine(switchMode);
                        file2.WriteLine("true");
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
            }
        }

        private async void freezeSize()
        {
            await MinimizeMessenger.SendMessageAsync("freeze|" + playerNum);
        }

        private string ReadStreamerModeValue()
        {
            try
            {
                string streamerModePath = Path.Combine(configPath, "StreamerMode.txt");
                if (!File.Exists(streamerModePath))
                {
                    return string.Empty;
                }

                using var fs = new FileStream(streamerModePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = new StreamReader(fs);
                return reader.ReadToEnd() ?? string.Empty;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading StreamerMode.txt for chkStreamer: {ex.Message}");
                return string.Empty;
            }
        }

        private bool ShouldHideStreamerCheckbox()
        {
            string streamerModeValue = ReadStreamerModeValue();
            return streamerModeValue.IndexOf("true", StringComparison.OrdinalIgnoreCase) >= 0
                && streamerModeValue.IndexOf("remote", StringComparison.OrdinalIgnoreCase) < 0;
        }

        private void chkStreamer_CheckedChanged(object sender, EventArgs e)
        {
            if (suppressStreamerCheckedChanged)
            {
                return;
            }

            string streamerModePath = Path.Combine(configPath, "StreamerMode.txt");

            if (chkStreamer.Checked)
            {
                ExtendedDialogResult result = ExtendedMessageBox.Show(
                    $"<p style='margin:0;padding:0;line-height:1.6;'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 1; -webkit-box-orient: vertical; overflow: hidden; font-size:{truncateSize}pt;' title='Are you sure you want to enable Admin Mode?'>Are you sure you want to enable Admin Mode?</span></p>",
                    "Enable Admin Mode?",
                    ExtendedMessageBoxLibrary.MessageBoxButtons.YesNo,
                    lines: 1,
                    zoomFactor: zoomFactor);

                if (result.Result != DialogResult.Yes)
                {
                    suppressStreamerCheckedChanged = true;
                    chkStreamer.Checked = false;
                    suppressStreamerCheckedChanged = false;
                    chkStreamer.Enabled = true;
                    chkStreamer.Visible = !ShouldHideStreamerCheckbox();
                    return;
                }

                try
                {
                    using (FileStream fs = new FileStream(streamerModePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        writer.Write("true");
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"An error occurred while writing to the file: {ex.Message}");
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine($"Access denied: {ex.Message}");
                }
            }
            else
            {
                try
                {
                    using (FileStream fs = new FileStream(streamerModePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        writer.Write(string.Empty);
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"An error occurred while writing to the file: {ex.Message}");
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine($"Access denied: {ex.Message}");
                }

            }

            chkStreamer.Enabled = !chkStreamer.Checked;
            chkStreamer.Visible = !ShouldHideStreamerCheckbox();

        }

        private void transparencySlider_ValueChanged(object sender, EventArgs e)
        {
            if (transparencySlider.Value == 0)
            {
                opacity = 255;
            }
            else if (transparencySlider.Value == 100)
            {
                opacity = 100;
            }
            else
            {
                opacity = 255 - (int)((transparencySlider.Value / 100.0) * (255 - 100));
            }

            string alphaFilePath = Path.Combine(configPath, "Transparency.txt");

            try
            {
                using (FileStream fs = new FileStream(alphaFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                using (StreamWriter file2 = new StreamWriter(fs))
                {
                    file2.WriteLine(opacity);
                    file2.WriteLine(switchMode);
                    file2.WriteLine("true");
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
        }

        private async void isLocked()
        {
            await MinimizeMessenger.SendMessageAsync("isLocked|" + playerNum);
        }

        private Panel CreateViewDropdown()
        {
            var panel = CreateDropdownPanel();

            panel.Height = 200;
            panel.MaximumSize = new Size(280, 200);
            panel.MinimumSize = new Size(280, 200);
            panel.Name = "viewPanel";

            void AddMouseHandlers(Control control)
            {
                control.MouseEnter += (s, e) => { panel.Tag = true; };
                control.MouseLeave += (s, e) => { panel.Tag = false; };
            }
  if (chkStreamer.Parent != null)
                chkStreamer.Parent.Controls.Remove(chkStreamer);
            chkStreamer.Dock = DockStyle.Top;
            panel.Controls.Add(chkStreamer);
            chkStreamer.Visible = !ShouldHideStreamerCheckbox();
            chkStreamer.Enabled = !chkStreamer.Checked;
            isLocked();

            if (chkRemote.Parent != null)
                chkRemote.Parent.Controls.Remove(chkRemote);
            chkRemote.Dock = DockStyle.Top;
            panel.Controls.Add(chkRemote);
            chkRemote.Visible = true;

          
            if (hideBG.Parent != null)
                hideBG.Parent.Controls.Remove(hideBG);
            hideBG.Dock = DockStyle.Top;
            panel.Controls.Add(hideBG);
            hideBG.Visible = false;

            if (showRightClickLabels.Parent != null)
                showRightClickLabels.Parent.Controls.Remove(showRightClickLabels);
            showRightClickLabels.Dock = DockStyle.Top;
            panel.Controls.Add(showRightClickLabels);

            if (showLeftClickLabels.Parent != null)
                showLeftClickLabels.Parent.Controls.Remove(showLeftClickLabels);
            showLeftClickLabels.Dock = DockStyle.Top;
            panel.Controls.Add(showLeftClickLabels);

            if (showMouseMoveLabels.Parent != null)
                showMouseMoveLabels.Parent.Controls.Remove(showMouseMoveLabels);
            showMouseMoveLabels.Dock = DockStyle.Top;
            panel.Controls.Add(showMouseMoveLabels);

            AddMouseHandlers(showRightClickLabels);
            panel.Controls.Add(showRightClickLabels);

            AddMouseHandlers(showLeftClickLabels);
            panel.Controls.Add(showLeftClickLabels);

            AddMouseHandlers(showMouseMoveLabels);
            panel.Controls.Add(showMouseMoveLabels);
            AddMouseHandlers(panel);

            var transparencyRow = new FlowLayoutPanel
            {
                FlowDirection = Wisej.Web.FlowDirection.LeftToRight,
                AutoSize = true,
                WrapContents = false,
                Dock = DockStyle.Top,
                Padding = new Padding(0, 5, 0, 5),
                BackColor = Color.Transparent
            };
            AddMouseHandlers(transparencyRow);

            var transparencyLabel = new Label
            {
                Text = "Transparency",
                ForeColor = Color.White,
                Font = new System.Drawing.Font("Segoe UI", 11, FontStyle.Regular),
                AutoSize = true,
                Margin = new Padding(5, 5, 10, 0)
            };
            AddMouseHandlers(transparencyLabel);

            transparencySlider = new TrackBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = 0,
                AutoSize = true,
                TickStyle = TickStyle.None,
                Name = "transparencySlider"
            };
            AddMouseHandlers(transparencySlider);

            int alpha;

            try
            {
                string alphaFilePath = Path.Combine(configPath, "Transparency.txt");
                alpha = int.Parse(System.IO.File.ReadLines(alphaFilePath).ElementAtOrDefault(0));
            }
            catch (Exception)
            {
                alpha = 255;
            }

            int sliderValue;
            if (alpha == 255)
            {
                sliderValue = 0;
            }
            else if (alpha == 100)
            {
                sliderValue = 100;
            }
            else
            {
                sliderValue = (int)Math.Round(100 - ((alpha - 100) / 155.0 * 100));
            }

            transparencySlider.Value = sliderValue;

            initializeTransparency();

            transparencySlider.ValueChanged += transparencySlider_ValueChanged;

            transparencyRow.Controls.Add(transparencyLabel);
            transparencyRow.Controls.Add(transparencySlider);
            panel.Controls.Add(transparencyRow);
            return panel;
        }

        private void showMouseMoveLabels_CheckedChanged(object sender, EventArgs e)
        {
            if (showMouseMoveLabels.Checked)
            {
                showLabelsLC = false;
                showLabelsRC = false;
                showLeftClickLabels.Checked = false;
                showRightClickLabels.Checked = false;

                showLabelsHover = true;
                pnlPrompts.Visible = true;
                drawPrompts();
                pnlPrompts.Scale((float)(browserWidth / 600));
            }
            else
            {
                showLabelsHover = false;
                drawPrompts();
            }
        }

        private void showLeftClickLabels_CheckedChanged(object sender, EventArgs e)
        {
            if (showLeftClickLabels.Checked)
            {
                showLabelsHover = false;
                showLabelsRC = false;
                showMouseMoveLabels.Checked = false;
                showRightClickLabels.Checked = false;

                showLabelsLC = true;
                pnlPrompts.Visible = true;
                drawPrompts();
                pnlPrompts.Scale((float)(browserWidth / 600));
            }
            else
            {
                showLabelsLC = false;
                drawPrompts();
            }
        }

        private void showRightClickLabels_CheckedChanged(object sender, EventArgs e)
        {
            if (showRightClickLabels.Checked)
            {
                showLabelsLC = false;
                showLabelsHover = false;
                showLeftClickLabels.Checked = false;
                showMouseMoveLabels.Checked = false;

                showLabelsRC = true;
                pnlPrompts.Visible = true;
                drawPrompts();
                pnlPrompts.Scale((float)(browserWidth / 600));
            }
            else
            {
                showLabelsRC = false;
                drawPrompts();
            }
        }

        private Panel CreateAdvancedDropdown()
        {
            var panel = advancedPanel;

            panel.Visible = false;
            panel.Width = 250;
            panel.AutoSize = true;
            panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel.BackColor = Color.FromArgb(30, 30, 30);
            panel.Padding = new Padding(10);

            panel.Height = 200;
            panel.MaximumSize = new Size(280, 200);
            panel.MinimumSize = new Size(280, 200);

            void AddMouseHandlers(Control control)
            {
                control.MouseEnter += (s, e) =>
                {
                    panel.Tag = true;
                };
                control.MouseLeave += (s, e) => { panel.Tag = false; };
            }

            if (chkAltRTC.Parent != null)
                chkAltRTC.Parent.Controls.Remove(chkAltRTC);
            chkAltRTC.Visible = true;
            chkAltRTC.Dock = DockStyle.Top;
            chkAltRTC.Text = "Alternative Return To Center";
            panel.Controls.Add(chkAltRTC);

            if (chkDisableClicks.Parent != null)
                chkDisableClicks.Parent.Controls.Remove(chkDisableClicks);
            chkDisableClicks.Visible = true;
            chkDisableClicks.Dock = DockStyle.Top;
            chkDisableClicks.Text = "Alternative Mouse Controls";
            panel.Controls.Add(chkDisableClicks);

            if (chkSafeZone.Parent != null)
                chkSafeZone.Parent.Controls.Remove(chkSafeZone);
            chkSafeZone.Visible = true;
            chkSafeZone.Dock = DockStyle.Top;
            chkSafeZone.Text = "Use Click Safe Zone";
            panel.Controls.Add(chkSafeZone);

            if (chkVoice.Parent != null)
                chkVoice.Parent.Controls.Remove(chkVoice);
            chkVoice.Visible = true;
            chkVoice.Dock = DockStyle.Top;
            chkVoice.Text = "Voice Control Mode";
            panel.Controls.Add(chkVoice);

            if (switchMode)
            {
                if (chkMarioKart.Parent != null)
                    chkMarioKart.Parent.Controls.Remove(chkMarioKart);
                chkMarioKart.Visible = true;
                chkMarioKart.Dock = DockStyle.Top;
                chkMarioKart.Text = "Mario Kart Mode";
                panel.Controls.Add(chkMarioKart);
            }

            if (chkCombineControllers.Parent != null)
                chkCombineControllers.Parent.Controls.Remove(chkCombineControllers);
            chkCombineControllers.Visible = true;
            chkCombineControllers.Dock = DockStyle.Top;
            chkCombineControllers.Text = "Combine Inputs from Other Systems";
            panel.Controls.Add(chkCombineControllers);

            if (chkFPS.Parent != null)
                chkFPS.Parent.Controls.Remove(chkFPS);
            chkFPS.Visible = true;
            chkFPS.Dock = DockStyle.Top;
            chkFPS.Text = "Twin Stick Mode";
            panel.Controls.Add(chkFPS);

            if (chkKeepMouseInside.Parent != null)
                chkKeepMouseInside.Parent.Controls.Remove(chkKeepMouseInside);
            chkKeepMouseInside.Visible = true;
            chkKeepMouseInside.Dock = DockStyle.Top;
            panel.Controls.Add(chkKeepMouseInside);

            if (!switchMode)
            {
                if (chkMouseLock.Parent != null)
                    chkMouseLock.Parent.Controls.Remove(chkMouseLock);
                chkMouseLock.Visible = true;
                chkMouseLock.Dock = DockStyle.Top;
                panel.Controls.Add(chkMouseLock);
            }

            AddMouseHandlers(panel);

            return panel;
        }

        private Panel CreateDropdownPanel()
        {
            return new Panel
            {
                Visible = false,
                Width = 250,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.FromArgb(30, 30, 30),
                Padding = new Padding(10)
            };
        }

    }
}