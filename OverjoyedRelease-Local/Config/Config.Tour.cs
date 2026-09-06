using ExtendedMessageBoxLibrary;
using GregsStack.InputSimulatorStandard.Native;
using Microsoft.Maui.ApplicationModel;
using Notion.Client;
using SharedVars;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Wisej.Web;
using Wisej.Web.Ext.TourPanel;


namespace OverjoyedReleaseLocal
{
    public partial class Config : Wisej.Web.Page
    {


        private void MicroSetup()
        {
            initialTour = true;

            window2 = new GuidedTour
            {

                Steps = new[]
                {
            new TourStep {Title = @"1) Purchase Adapter for Switch", Text = "<p style='line-height:1.5;padding:0;margin:0;'><span style='-webkit-line-clamp:3;' class='truncate' title='Nintendo Switch Mode for Overjoyed requires the Micro adapter from our amazing partner 8BitDo. A firmware upgrade is required the first time you use the adapter, or when migrating from Switch 1 to Switch 2.'>Nintendo Switch Mode for Overjoyed requires the Micro adapter from our amazing partner 8BitDo. A firmware upgrade is required the first time you use the adapter, or when migrating from Switch 1 to Switch 2.</span></p>", Target = pnlAllSettings, Alignment = Placement.TopLeft},

                        new TourStep {Title = @"2) Update Your Micro Adapter", Text = "<p style='line-height:1.5;padding:0;margin:0;'><span style='-webkit-line-clamp:3;' class='truncate' title='Once you have received your Micro adapter, use the diagram below and the 8BitDo UPDATER window to the right to update the firmware so it recognizes Overjoyed.'>Once you have received your Micro adapter, use the diagram below and the 8BitDo UPDATER window to the right to update the firmware so it recognizes Overjoyed.</span></p><br><p style='line-height:1.5;padding:0;margin:0;'><span style='-webkit-line-clamp:3;' class='truncate' title='Text from Firmware Update Window: Press and hold L + R + HOME (see diagram below) on your MICRO. When light is  yellow, connect MICRO to your computer using USB cable and click USB UPDATE.'><strong>Text from Firmware Update Window:</strong> Press and hold L + R + HOME (see diagram below) on your MICRO. When light is  yellow, connect MICRO to your computer using USB cable and click USB UPDATE.</span></p>", Target = pnlAllSettings, Alignment = Placement.TopLeft},

                         new TourStep {Title = @"3) Pair With Nintendo Switch", Text = "<p style='line-height:1.5;padding:0;margin:0;'><span style='-webkit-line-clamp:2;' class='truncate' title='Finally, follow the steps below to pair your Micro adapter with your Nintendo Switch (you only have to do this once).'>Finally, follow the steps below to pair your Micro adapter with your Nintendo Switch (you only have to do this once).</span></p><p style='line-height:1.5;padding:0;margin:0;'><span style='font-size:80%;'><br><br><br><br><br><br></span></p><p style='line-height:1.5;padding:0;margin:0;'><span style='-webkit-line-clamp:1;font-size:85%;' class='truncate' title='1. Unplug MICRO from your computer. Then turn MODE SWITCH to S.'>1. Unplug MICRO from your computer. Then turn MODE SWITCH to S.</span></p><br><p style='line-height:1.5;padding:0;margin:0;'><span style='-webkit-line-clamp:1;font-size:85%;' class='truncate' title='2. Press home to turn on the gamepad.'>2. Press home to turn on the gamepad.</span></p><br><p style='line-height:1.5;padding:0;margin:0;'><span style='-webkit-line-clamp:1;font-size:85%;' class='truncate' title='3. Hold PAIR button for 1 second to enter pairing mode, LED will blink rapidly.'>3. Hold PAIR button for 1 second to enter pairing mode, LED will blink rapidly.</span></p><br><p style='line-height:1.5;padding:0;margin:0;'><span style='-webkit-line-clamp:2;font-size:85%;' class='truncate' title='4. Turn on Nintendo Switch, then use a Switch Joycon to click on Controllers, then Change Grip/Order. Wait for MICRO to connect.'>4. Turn on Nintendo Switch, then use a Switch Joycon to click on Controllers, then Change Grip/Order. Wait for MICRO to connect.</span></p><br><p style='line-height:1.5;padding:0;margin:0;'><span style='-webkit-line-clamp:1;font-size:85%;' class='truncate' title='5. The LED on your MICRO will become solid when the connection is successful.'>5. The LED on your MICRO will become solid when the connection is successful.</span></p><br><p style='line-height:1.5;padding:0;margin:0;'><span style='-webkit-line-clamp:1;font-size:85%;' class='truncate' title='6. Plug the Micro adapter back into your computer.'>6. Plug the Micro adapter back into your computer.</span></p>", Target = pnlAllSettings, Alignment = Placement.TopLeft},

                         new TourStep {Title = @"4) Enjoy Playing Your Switch!", Text = "<p style='line-height:1.5;padding:0;margin:0;'><span style='-webkit-line-clamp:3;' class='truncate' title='Your 8BitDo Micro adapter is all set up to work with Overjoyed and your Nintendo Switch console! To run this setup guide in the future, simply click the 'Micro Setup' button shown above.'>Your 8BitDo Micro adapter is all set up to work with Overjoyed and your Nintendo Switch console! To run this setup guide in the future, simply click the 'Micro Setup' button shown above.</span></p><br><p style='line-height:1.5;padding:0;margin:0;'><span style='-webkit-line-clamp:3;' class='truncate' title='Turn On Micro Adapter: When you turn on your computer, your Micro adapter should automatically turn on. To manually turn your adapter on, press the Home button (looks like a heart) on your adapter.'><strong>Turn On Micro Adapter:</strong> When you turn on your computer, your Micro adapter should automatically turn on. To manually turn your adapter on, press the Home button (looks like a heart) on your adapter.</span></p><br><p style='line-height:1.5;padding:0;margin:0;'><span style='-webkit-line-clamp:3;' class='truncate' title='Turn Off Micro Adapter: When you turn off your computer, your Micro adapter will automatically turn off. To manually turn your adapter off, hold the Home button (looks like a heart) on your adapter for 8 seconds.'><strong>Turn Off Micro Adapter:</strong> When you turn off your computer, your Micro adapter will automatically turn off. To manually turn your adapter off, hold the Home button (looks like a heart) on your adapter for 8 seconds.</span></p><br><p style='line-height:1.5;padding:0;margin:0;'><span style='-webkit-line-clamp:2;' class='truncate' title='Turn On Your Switch (only works on Switch 1): Click Turn On Switch button in the 'Overjoyed Interface' window that appears after setup.'><strong>Turn On Your Switch (only works on Switch 1):</strong> Click 'Turn On Switch' button in the 'Overjoyed Interface' window that appears after setup.</span></p>", Target = pnlAllSettings, Alignment = Placement.TopLeft}
                }
            };
            window2.BeforeStep += MicroSetup_BeforeStep;
            window2.Closed += MicroSetup_Closed;
            window2.Show();

            window2.CssClass = "GuidedTour";


        }
        private void MicroSetup_Closed(object sender, EventArgs e)
        {
            var closedWindow = window2;

            Control controlToRemove = window2.Controls["MicroImage"];
            if (controlToRemove != null)
            {
                window2.Controls.Remove(controlToRemove);
            }

            controlToRemove = window2.Controls["MicroTextBlue"];
            if (controlToRemove != null)
            {
                window2.Controls.Remove(controlToRemove);
            }

            controlToRemove = window2.Controls["MicroTextGreen"];
            if (controlToRemove != null)
            {
                window2.Controls.Remove(controlToRemove);
            }

            controlToRemove = window2.Controls["MicroDiagramFront"];
            if (controlToRemove != null)
            {
                window2.Controls.Remove(controlToRemove);
            }

            controlToRemove = window2.Controls["MicroDiagramBottom"];
            if (controlToRemove != null)
            {
                window2.Controls.Remove(controlToRemove);
            }

            controlToRemove = window2.Controls["ExitTour2"];
            if (controlToRemove != null)
            {
                window2.Controls.Remove(controlToRemove);
            }

            try
            {



                // Write the updated parameters back to the file with shared read/write access
                using (FileStream fs = new FileStream(tourFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                using (StreamWriter writer = new StreamWriter(fs))
                {

                    writer.WriteLine("tourNo");
                    writer.WriteLine("microNo");
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

            nintendoSwitch.Checked = true;

            Eval(@" const current = document.querySelector('.gt-lang-code').textContent.trim();
App.Config.currentLanguage(current); location.reload();");


            if (tourEnd)
            {
                btnSubmit.PerformClick();
            }
            else
            {
                ExtendedDialogResult dialogResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 3; -webkit-box-orient: vertical; overflow: hidden; ' title='You have exited the Micro Setup. You can access the  Micro Setup at any time by clicking Micro Setup in the bottom left corner of the main Overjoyed window.'>You have exited the Micro Setup. You can access the  Micro Setup at any time by clicking 'Micro Setup' in the bottom left corner of the main Overjoyed window.</span></p>", "Micro Setup Exited");
                if (dialogResult.Result == DialogResult.OK)
                {
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
                    if (currentLang != "en")
                    {
                        Eval(@"setTimeout(() => {
const switcher = document.querySelector('.gt_float_switcher');

if (switcher) {
    // List of events to try
    const events = ['mouseenter', 'mouseover', 'mousemove', 'pointerenter', 'focus'];

    // Dispatch each event
    events.forEach(eventName => {
        const event = new MouseEvent(eventName, { bubbles: true });
        switcher.dispatchEvent(event);
    });
switcher.style.display = 'none';
}



}, 500);");
                    }
                    else
                    {
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
                    }
                }

            }

            if (closedWindow != null)
            {
                closedWindow.BeforeStep -= MicroSetup_BeforeStep;
                closedWindow.Closed -= MicroSetup_Closed;
                closedWindow.Dispose();
            }

            window2 = null;


        }

        private void MicroSetup_BeforeStep(object sender, TourPanelEventArgs e)
        {

            int preferredHeight = 300;

            if (e.StepIndex == 0)
            {
                tourEnd = false;
                preferredHeight = 445;

                Eval(@"
(function tryMoveScrollbar(){
    const scrollbar = document.querySelector('[name=""scrollbar-y""]');
    if (scrollbar) {
        document.body.appendChild(scrollbar);
    } else {
        setTimeout(tryMoveScrollbar, 100);
    }
})();
");

                Control controlToRemove = window2.Controls["MicroImage"];
                if (controlToRemove != null)
                {
                    window2.Controls.Remove(controlToRemove);
                }

                controlToRemove = window2.Controls["MicroTextBlue"];
                if (controlToRemove != null)
                {
                    window2.Controls.Remove(controlToRemove);
                }

                controlToRemove = window2.Controls["MicroTextGreen"];
                if (controlToRemove != null)
                {
                    window2.Controls.Remove(controlToRemove);
                }

                controlToRemove = window2.Controls["MicroDiagramFront"];
                if (controlToRemove != null)
                {
                    window2.Controls.Remove(controlToRemove);
                }

                controlToRemove = window2.Controls["MicroDiagramBottom"];
                if (controlToRemove != null)
                {
                    window2.Controls.Remove(controlToRemove);
                }


                window2.Controls.Add(new PictureBox
                {
                    Image = System.Drawing.Image.FromFile(Path.Combine(configPath, "8BitDo-Micro.png")),
                    Size = new System.Drawing.Size(300, 170),
                    Location = new System.Drawing.Point(110, 235),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Name = "MicroImage"
                });

                window2.Controls.Add(new HtmlPanel
                {
                    Text = "<p style='padding:0;margin:0;text-align:center;'><strong><a href='#'>Click here</a></strong> to purchase a Blue Micro adapter</p>",
                    Size = new System.Drawing.Size(480, 20),
                    Location = new System.Drawing.Point(10, 195),
                    Name = "MicroTextBlue",


                });

                window2.Controls.Add(new HtmlPanel
                {
                    Text = "<p style='padding:0;margin:0;text-align:center;'><strong><a href='#'>Click here</a></strong> to purchase a Green Micro adapter</p>",
                    Size = new System.Drawing.Size(480, 20),
                    Location = new System.Drawing.Point(10, 150),
                    Name = "MicroTextGreen"
                });

                //add click event to above htmlpanel
                window2.Controls["MicroTextBlue"].Click += new System.EventHandler(this.MicroTextBlue_Click);

                window2.Controls["MicroTextBlue"].BringToFront();

                window2.Controls["MicroTextGreen"].Click += new System.EventHandler(this.MicroTextGreen_Click);

                window2.Controls["MicroTextGreen"].BringToFront();



                if (Screen.Bounds.Height >= 1100 && Screen.Bounds.Width > 1920)
                {
                    window2.Controls["MicroImage"].Scale(1.4f);
                    window2.Controls["MicroTextBlue"].Scale(1.4f);
                    window2.Controls["MicroTextGreen"].Scale(1.4f);


                    if (initialTour == true)
                    {
                        ScaleFonts(window2, 1.4f); ScaleFonts(window2.Controls["MicroTextGreen"], 1 / 1.4f);
                        ScaleFonts(window2.Controls["MicroTextBlue"], 1 / 1.4f);
                    }
                }
            }
            if (e.StepIndex == 1)
            {
                preferredHeight = 490;

                initialTour = false;

                Control controlToRemove = window2.Controls["MicroImage"];
                if (controlToRemove != null)
                {
                    window2.Controls.Remove(controlToRemove);
                }

                controlToRemove = window2.Controls["MicroTextBlue"];
                if (controlToRemove != null)
                {
                    window2.Controls.Remove(controlToRemove);
                }

                controlToRemove = window2.Controls["MicroTextGreen"];
                if (controlToRemove != null)
                {
                    window2.Controls.Remove(controlToRemove);
                }

                controlToRemove = window2.Controls["MicroDiagramFront"];
                if (controlToRemove != null)
                {
                    window2.Controls.Remove(controlToRemove);
                }

                controlToRemove = window2.Controls["MicroDiagramBottom"];
                if (controlToRemove != null)
                {
                    window2.Controls.Remove(controlToRemove);
                }


                window2.Controls.Add(new PictureBox
                {
                    Image = System.Drawing.Image.FromFile(Path.Combine(configPath, "8BitDo-MicroDiagram-Front.png")),
                    Size = new System.Drawing.Size(450, 213),
                    Location = new System.Drawing.Point(25, 240),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Name = "MicroDiagramFront"
                });

                if (Screen.Bounds.Height >= 1100 && Screen.Bounds.Width > 1920)
                {
                    window2.Controls["MicroDiagramFront"].Scale(1.4f);

                }

                microCorner();

                // Specify the process name (without extension)
                string processName = "8Bitdo Micro Updater for Overjoyed";

                // Check if the process is already running
                Process[] processes = Process.GetProcessesByName(processName);

                if (processes.Length == 0)
                {
                    // Process is not running, so start it
                    Process.Start(Path.Combine(configPath, "8Bitdo Micro Updater for Overjoyed.exe"));
                }
            }
            if (e.StepIndex == 2)
            {
                preferredHeight = 520;

                Control controlToRemove = window2.Controls["MicroImage"];
                if (controlToRemove != null)
                {
                    window2.Controls.Remove(controlToRemove);
                }

                controlToRemove = window2.Controls["MicroTextBlue"];
                if (controlToRemove != null)
                {
                    window2.Controls.Remove(controlToRemove);
                }

                controlToRemove = window2.Controls["MicroTextGreen"];
                if (controlToRemove != null)
                {
                    window2.Controls.Remove(controlToRemove);
                }

                controlToRemove = window2.Controls["MicroDiagramFront"];
                if (controlToRemove != null)
                {
                    window2.Controls.Remove(controlToRemove);
                }

                controlToRemove = window2.Controls["MicroDiagramBottom"];
                if (controlToRemove != null)
                {
                    window2.Controls.Remove(controlToRemove);
                }

                controlToRemove = window2.Controls["ExitTour2"];
                if (controlToRemove != null)
                {
                    window2.Controls.Remove(controlToRemove);
                }

                window2.Controls.Add(new PictureBox
                {
                    Image = System.Drawing.Image.FromFile(Path.Combine(configPath, "8BitDo-MicroDiagram-Bottom.png")),
                    Size = new System.Drawing.Size(450, 96),
                    Location = new System.Drawing.Point(25, 120),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Name = "MicroDiagramBottom"
                });

                if (Screen.Bounds.Height >= 1100 && Screen.Bounds.Width > 1920)
                {
                    window2.Controls["MicroDiagramBottom"].Scale(1.4f);
                }
            }
            if (e.StepIndex == 3)
            {
                tourEnd = true;
                preferredHeight = 460;

                Control controlToRemove = window2.Controls["MicroImage"];
                if (controlToRemove != null)
                {
                    window2.Controls.Remove(controlToRemove);
                }

                controlToRemove = window2.Controls["MicroTextBlue"];
                if (controlToRemove != null)
                {
                    window2.Controls.Remove(controlToRemove);
                }

                controlToRemove = window2.Controls["MicroTextGreen"];
                if (controlToRemove != null)
                {
                    window2.Controls.Remove(controlToRemove);
                }

                controlToRemove = window2.Controls["MicroDiagramFront"];
                if (controlToRemove != null)
                {
                    window2.Controls.Remove(controlToRemove);
                }

                controlToRemove = window2.Controls["MicroDiagramBottom"];
                if (controlToRemove != null)
                {
                    window2.Controls.Remove(controlToRemove);
                }

                Control controlToRemove2 = window2.Controls["ExitTour2"];
                if (controlToRemove != null)
                {
                    window2.Controls.Remove(controlToRemove2);
                }

                window2.Controls.Add(new Button
                {

                    Size = new System.Drawing.Size(480, 50),
                    Location = new System.Drawing.Point(10, 390),
                    BackColor = System.Drawing.Color.FromArgb(0, 167, 209),
                    ForeColor = System.Drawing.Color.FromArgb(255, 255, 255),
                    Text = "Exit Setup Guide and Start Playing Nintendo Switch!",
                    Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point),
                    Cursor = Cursors.Hand,
                    Name = "ExitTour2"
                });


                window2.Controls["ExitTour2"].Click += new System.EventHandler(this.ExitMicroTour_Click);
                styleSheet1.SetCssClass(window2.Controls["ExitTour2"], "optButtons");


                window2.Controls["ExitTour2"].BringToFront();
                if (Screen.Bounds.Height >= 1100 && Screen.Bounds.Width > 1920)
                {
                    window2.Controls["ExitTour2"].Scale(1.4f);

                    ScaleFonts(window2.Controls["ExitTour2"], 1.4f);

                }

            }



            if (initialTour == false && Screen.Bounds.Height >= 1100 && Screen.Bounds.Width > 1920)
            {
                float inverseScale = 1f / 1.4f;

                window2.TitlePanel.Scale(inverseScale);
                window2.ButtonsPanel.Scale(inverseScale);
                window2.HtmlText.Scale(inverseScale);
            }

            if (Screen.Bounds.Height < 1100 && Screen.Bounds.Width <= 1920)
            {

                window2.MaximumSize = new System.Drawing.Size((int)(501), (int)(preferredHeight));
                window2.MinimumSize = new System.Drawing.Size((int)(501), (int)(preferredHeight));
            }



            if (Screen.Bounds.Height >= 1100 && Screen.Bounds.Width > 1920)
            {
                window2.MaximumSize = new System.Drawing.Size((int)(501), (int)(preferredHeight * 1.4));
                window2.MinimumSize = new System.Drawing.Size((int)(501), (int)(preferredHeight * 1.4));
                window2.TitlePanel.Scale(1.4f);
                window2.ButtonsPanel.Scale(1.4f);
                window2.HtmlText.Scale(1.4f);



                Eval(@"
                (function retrySetTourPanel(){
                    var panel = document.getElementsByName('TourPanel1')[0];
                    if (panel) {
                        panel.style.setProperty('width', '701px', 'important');
                    } else {
                        setTimeout(retrySetTourPanel, 100); // Retry after 100ms
                    }
                })();
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
    let lineHeight = 1.5;
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

            }

        }

        private void ExitMicroTour_Click(object sender, EventArgs e)
        {
            window2.Close();
        }

        private async void microCorner()
        {
            int playerNum = GetAssignedPlayerNum();
            await MinimizeMessenger.SendMessageAsync("corner|" + playerNum);

        }

        private void MicroTextBlue_Click(object sender, EventArgs e)
        {

            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://shop.8bitdo.com/products/8bitdo-micro-bluetooth-gamepad?variant=42849604534449",
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(psi);


        }

        private void MicroTextGreen_Click(object sender, EventArgs e)
        {

            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://shop.8bitdo.com/products/8bitdo-micro-bluetooth-gamepad?variant=42849604567217",
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(psi);
        }



        private void GuidedTour()
        {
            initialTour = true;

            window = new GuidedTour
            {



                Steps = new[]
    {

            new TourStep {Title = @"Get Ready to Be Overjoyed!", Text = "<p style='padding:0;margin:0;line-height:1.5;'><span style='-webkit-line-clamp:3;' class='truncate' title='Thank you for installing Overjoyed Accessible Gaming! This software is a project of Jett Foundation, a nonprofit empowering the neuromuscular disease community with education, adaptive recreation, and advocacy.'>Thank you for installing Overjoyed Accessible Gaming! This software is a project of Jett Foundation, a nonprofit empowering the neuromuscular disease community with education, adaptive recreation, and advocacy.</span></p><br><p style='padding:0;margin:0;line-height:1.5;'><span style='-webkit-line-clamp:5;' class='truncate' title='Overjoyed is a virtual accessibility platform that allows you to play video games using only your mouse, trackpad, eye gaze system, or any other way you move your mouse cursor. It is made up of 8 main quadrants and a deadzone circle in the center. When you Move Your Mouse or Click in a quadrant, it triggers a specific control to be pressed.'>Overjoyed is a virtual accessibility platform that allows you to play video games using only your mouse, trackpad, eye gaze system, or any other way you move your mouse cursor. It is made up of 8 main quadrants and a deadzone circle in the center. When you Move Your Mouse or Click in a quadrant, it triggers a specific control to be pressed.</span></p><br><p style='text-align:center;padding:0;margin:0;line-height:1.5;font-size:135%;'><strong>Special Thanks</strong></p><br><br><br><br><p style='text-align:center;padding:0;margin:0;line-height:1.5;font-size:107%;font-style:italic;'>For Nintendo Switch Support</p><br><br><p style='text-align:center;padding:0;margin:0;line-height:1.5;font-size:107%;font-style:italic;'><span style='-webkit-line-clamp:2;' class='truncate' title='Jonah Monaghan and The Playability Initiative for their support developing the initial version of Overjoyed'>Jonah Monaghan and The Playability Initiative for their amazing support developing the initial version of Overjoyed</p><br><p style='text-align:center;padding:0;margin:0;line-height:1.5;font-size:107%;font-style:italic;'><span style='-webkit-line-clamp:3;' class='truncate' title='Jen_theHuman, Vimlark, MDA_LetsPlay, Storybrooks, Starlinggg, and all of the accepting Twitch communities who motivated me and helped me cope with being unable to use a traditional controller'>Jen_theHuman, Vimlark, MDA_LetsPlay, Storybrooks, Starlinggg, and all of the accepting Twitch communities who motivated me and helped me cope with being unable to use a traditional controller</span></p>", Target = pnlQuadrants, Alignment = Placement.TopLeft},

                        new TourStep {Title = @"Select Game Controls Layout", Text = "<p style='line-height:1.5;padding:0;margin:0;'><span style='-webkit-line-clamp:2;' class='truncate' title='Start by selecting a Game Controls Layout. We recommend using the  Default Layout  when trying Overjoyed the first time.'>Start by selecting a Game Controls Layout. We recommend using the  Default Layout  when trying Overjoyed the first time.</span></p><br><p style='line-height:1.5;padding:0;margin:0;'><span style='-webkit-line-clamp:4;' class='truncate' title='Once you're familiar with Overjoyed, you can create custom layouts for specific games by clicking the  Create & Manage Layouts  button and selecting  Create Layout.  Enter a name, like  Stardew Valley , if you re setting up controls for that game.'>Once you're familiar with Overjoyed, you can create custom layouts for specific games by clicking the  Create & Manage Layouts  button and selecting  Create Layout.  Enter a name, like  Stardew Valley , if you re setting up controls for that game.</span></p><br><p style='line-height:1.5;padding:0;margin:0;'><span style='-webkit-line-clamp:1;' class='truncate' title='Under  Create & Manage Layouts  you will find options to:'><strong>Under  Create & Manage Layouts  you will find options to:</strong></span></p><br><p style='line-height:1.5;padding:0;margin:0;'><span style='-webkit-line-clamp:1;' class='truncate' title='Search/Import   Load a friend s layout or search Game Library'>Search/Import   Load a friend s layout or search Game Library</span></p><br><p style='line-height:1.5;padding:0;margin:0;'><span style='-webkit-line-clamp:1;' class='truncate' title='Share   Export a layout to a friend or Overjoyed Game Library'>Share   Export a layout to a friend or Overjoyed Game Library</span></p><br><p style='line-height:1.5;padding:0;margin:0;'><span style='-webkit-line-clamp:2;' class='truncate' title='Rename, Delete, Reset   Rename, permanently remove, or restore a layout to default settings'>Rename, Delete, Reset   Rename, permanently remove, or restore a layout to default settings</span></p>", Target = pnlStep2},

new TourStep {Title = @"Choose Type of Controls", Text = "<p style='line-height:1.5;padding:0;margin:0;'><span style='-webkit-line-clamp:2;' class='truncate' title='Then choose what type of controls you need -  Basic Keyboard ,  Gamepad (PC / Xbox / PlayStation) , or  Nintendo Switch .'>Then choose what type of controls you need -  Basic Keyboard ,  Gamepad (PC / Xbox / PlayStation) , or  Nintendo Switch .</span></p><br><p style='line-height:1.5;padding:0;margin:0;'><span style='-webkit-line-clamp:3;' class='truncate' title=' Gamepad  supports PC, Xbox Game Pass (in browser, Xbox app, remote play), and PlayStation Plus Premium (on PC only, not remote play) and needs a driver install.  Nintendo Switch  requires 8BitDo's Micro adapter.'> Gamepad  supports PC, Xbox Game Pass (in browser, Xbox app, remote play), and PlayStation Plus Premium (on PC only, not remote play) and needs a driver install.  Nintendo Switch  requires 8BitDo's Micro adapter.</span></p>", Target = pnlModes, Alignment = Placement.BottomLeft},

                        new TourStep {Title = @"Set Up Your Controls", Text = "<p style='line-height:1.5;padding:0;margin:0;'><span style='-webkit-line-clamp:3;' class='truncate' title='The  Mouse Move Controls  tab shows controls triggered when your cursor moves into a quadrant.  Left Click Controls  or  Right Click Controls  tabs show controls triggered when you left or right click in a quadrant.'>The  Mouse Move Controls  tab shows controls triggered when your cursor moves into a quadrant.  Left Click Controls  or  Right Click Controls  tabs show controls triggered when you left or right click in a quadrant.</span></p><br><p style='line-height:1.5;padding:0;margin:0;'><span style='-webkit-line-clamp:4;' class='truncate' title='Click the Assign Button in a specific quadrant, then either choose a control from the dropdown or type the key you want to assign it to using your on-screen keyboard. To disable a specific quadrant from triggering controls, click its blue Power button.'>Click the Assign Button in a specific quadrant, then either choose a control from the dropdown or type the key you want to assign it to using your on-screen keyboard. To disable a specific quadrant from triggering controls, click its blue Power button.</span></p>", Target = pnlTabs, Alignment = Placement.BottomLeft},

                         new TourStep {Title = @"Advanced - Control Options", Text = "<div style=\"margin:0 auto;width:65%;margin-top:-25px;pointer-events:none\"><svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\" id=\"Capa_1\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" width=\"100%\" height=\"auto\" xml:space=\"preserve\" preserveAspectRatio=\"xMidYMid\" viewBox=\"0 0 600 500\" data-app=\"Xyris\">\r\n    <defs id=\"defs4116\">\r\n    </defs>\r\n    <path id=\"path-hNVhUnaSzcyM2qzWzX35kS\" name=\"left\" d=\"M 92.962 233.11 C 90.50200000000001 231.881 78.378 230.185 72.04400000000001 230.185 C 71.54200000000002 230.185 71.10700000000001 230.197 70.73400000000001 230.227 C 69.67500000000001 230.30100000000002 66.468 230.528 66.46300000000001 244.193 C 66.46900000000001 257.902 69.676 258.129 70.72900000000001 258.202 C 71.10800000000002 258.233 71.549 258.245 72.04500000000002 258.245 C 78.38500000000002 258.245 90.50900000000001 256.557 92.96300000000002 255.32 C 94.63400000000003 254.481 99.31600000000002 248.74699999999999 102.63900000000002 244.218 C 98.936 239.181 94.529 233.893 92.962 233.11 Z\" fill=\"rgb(0, 0, 0)\">\r\n    </path>\r\n    <path id=\"path-3BB7fPiwquCZyewHex3HWf\" name=\"down\" d=\"M 114.003 255.595 C 108.96 259.291 103.679 263.685 102.901 265.258 C 101.548 267.963 99.657 282.51 100.012 287.498 C 100.085 288.551 100.318 291.758 113.984 291.764 L 114.026 294.824 L 114.026 291.764 C 127.692 291.758 127.924 288.551 127.998 287.493 C 128.353 282.517 126.468 267.97 125.109 265.259 C 124.407 263.851 119.682 259.762 114.003 255.595 Z\" fill=\"rgb(0, 0, 0)\" stroke=\"rgb(255, 255, 255)\" stroke-width=\"0\" transform=\"\">\r\n    </path>\r\n    <g id=\"g5464\" transform=\"matrix(1 0 0 1 0 0)\">\r\n        <g id=\"g8448\" transform=\"matrix(1 0 0 1 0 0)\">\r\n            <g>\r\n                <path id=\"controller-b14\" name=\"left\" d=\"M 92.962 233.11 C 90.50200000000001 231.881 78.378 230.185 72.04400000000001 230.185 C 71.54200000000002 230.185 71.10700000000001 230.197 70.73400000000001 230.227 C 69.67500000000001 230.30100000000002 66.468 230.528 66.46300000000001 244.193 C 66.46900000000001 257.902 69.676 258.129 70.72900000000001 258.202 C 71.10800000000002 258.233 71.549 258.245 72.04500000000002 258.245 C 78.38500000000002 258.245 90.50900000000001 256.557 92.96300000000002 255.32 C 94.63400000000003 254.481 99.31600000000002 248.74699999999999 102.63900000000002 244.218 C 98.936 239.181 94.529 233.893 92.962 233.11 Z\" fill=\"rgb(0, 167, 209)\">\r\n                </path>\r\n                <path id=\"controller-b13\" name=\"down\" d=\"M 114.003 255.595 C 108.96 259.291 103.679 263.685 102.901 265.258 C 101.548 267.96299999999997 99.657 282.51 100.012 287.498 C 100.085 288.551 100.318 291.758 113.984 291.764 L 114.026 294.824 L 114.026 291.764 C 127.692 291.75800000000004 127.92399999999999 288.551 127.99799999999999 287.493 C 128.35299999999998 282.517 126.46799999999999 267.96999999999997 125.109 265.259 C 124.407 263.851 119.682 259.762 114.003 255.595 Z\" fill=\"rgb(0, 167, 209)\" stroke=\"rgb(255, 255, 255)\" stroke-width=\"0\">\r\n                </path>\r\n                <animate attributeName=\"opacity\" keyTimes=\"0; 0.49975; 0.75; 1\" values=\"0; 1; 0; 0\" begin=\"-0.00001\" dur=\"4.00002\" fill=\"freeze\" calcMode=\"spline\" keySplines=\"0 0 1 1; 0 0 1 1; 0 0 1 1\" repeatCount=\"indefinite\">\r\n                </animate>\r\n            </g>\r\n            <rect id=\"controller-b8\" name=\"select\" x=\"227.994\" y=\"238.771\" width=\"18.898\" height=\"7.711\">\r\n            </rect>\r\n            <path id=\"controller-b12\" name=\"up\" d=\"M 114.003 232.84 C 119.689 228.673 124.407 224.584 125.111 223.171 C 126.464 220.47199999999998 128.35500000000002 205.92499999999998 128 200.93099999999998 C 127.92 199.878 127.694 196.677 114.028 196.671 C 100.32000000000001 196.677 100.093 199.878 100.013 200.93099999999998 C 99.658 205.91799999999998 101.543 220.46599999999998 102.902 223.171 C 103.685 224.744 108.96 229.15 114.003 232.84 Z\" transform=\"matrix(1 0 0 1 0 0)\">\r\n                <animate attributeType=\"auto\" attributeName=\"fill\" values=\"#000000;rgb(0, 0, 0);rgb(0, 0, 0);rgb(0, 0, 0);rgb(0, 0, 0);#000000;#000000\" calcMode=\"spline\" keyTimes=\"0; 0.21666666666666667; 0.25; 0.4166666666666667; 0.7166666666666667; 0.75; 1\" keySplines=\"0 0 1 1;0 0 1 1;0 0 1 1;0 0 1 1;0 0 1 1;0 0 1 1\" dur=\"6.00002\" begin=\"-0.00001\" repeatCount=\"indefinite\" additive=\"replace\" accumulate=\"none\" fill=\"freeze\" id=\"animate3869\">\r\n                </animate>\r\n            </path>\r\n            <path id=\"controller-b15\" name=\"right\" d=\"M 157.284 230.234 C 156.905 230.203 156.464 230.191 155.968 230.191 C 149.628 230.191 137.504 231.88 135.04399999999998 233.116 C 133.47799999999998 233.9 129.07099999999997 239.181 125.37499999999999 244.22400000000002 C 128.69199999999998 248.752 133.374 254.49400000000003 135.04399999999998 255.32600000000002 C 137.504 256.55600000000004 149.628 258.25100000000003 155.962 258.25100000000003 C 156.458 258.25100000000003 156.898 258.23900000000003 157.272 258.20900000000006 C 158.331 258.13500000000005 161.53799999999998 257.90900000000005 161.54299999999998 244.24300000000005 C 161.54299999999998 244.19300000000004 161.54299999999998 244.15100000000004 161.54299999999998 244.11400000000006 C 161.525 230.528 158.336 230.301 157.284 230.234 Z\">\r\n            </path>\r\n            <rect id=\"rect-gfUbPXyvwyzB2qFLJWtrvx\" x=\"99.994\" y=\"50.0\" height=\"50.898\" width=\"35.5\" fill=\"rgb(0, 0, 0)\" rx=\"19\" ry=\"10\">\r\n                <animate attributeName=\"fill-opacity\" keyTimes=\"0; 0.33316666666666667; 0.6549999999999999; 0.84; 1\" values=\"1;1;1;1;1\" begin=\"-0.00001\" dur=\"6.00002\" fill=\"freeze\" calcMode=\"spline\" keySplines=\"0 0 1 1; 0 0 1 1; 0 0 1 1; 0 0 1 1\" repeatCount=\"indefinite\">\r\n                </animate>\r\n            </rect>\r\n            <circle id=\"controller-b3\" cx=\"465.824\" cy=\"194.529\" r=\"19.866\">\r\n                <animate attributeType=\"auto\" attributeName=\"fill\" values=\"#000000;#000000;#00a7d1;#000000;#000000\" calcMode=\"spline\" keyTimes=\"0;0;0.12499999999999997;0.24999999999999994;0.9999999999999998\" keySplines=\"0 0 1 1;0 0 1 1;0 0 1 1;0 0 1 1\" dur=\"4.00002\" begin=\"-0.00001\" repeatCount=\"indefinite\" additive=\"replace\" accumulate=\"none\" fill=\"freeze\" id=\"animate4708\">\r\n                </animate>\r\n            </circle>\r\n            <circle id=\"controller-b1\" cx=\"512.4459838867188\" cy=\"241.15199279785156\" r=\"19.865\">\r\n                <animate attributeType=\"auto\" attributeName=\"fill\" values=\"#000000;#000000;#00a7d1;#000000;#000000\" calcMode=\"spline\" keyTimes=\"0;0.24999999999999994;0.375;0.4999999999999999;0.9999999999999998\" keySplines=\"0 0 1 1;0 0 1 1;0 0 1 1;0 0 1 1\" dur=\"4.00002\" begin=\"-0.00001\" repeatCount=\"indefinite\" additive=\"replace\" accumulate=\"none\" fill=\"freeze\" id=\"animate3577\">\r\n                </animate>\r\n            </circle>\r\n            <circle id=\"controller-b0\" cx=\"465.824\" cy=\"287.774\" r=\"19.865\">\r\n                <animate attributeType=\"auto\" attributeName=\"fill\" values=\"#000000;#000000;#00a7d1;#000000;#000000\" calcMode=\"spline\" keyTimes=\"0;0.5;0.625;0.75;1\" keySplines=\"0 0 1 1;0 0 1 1;0 0 1 1;0 0 1 1\" dur=\"4.00002\" begin=\"-0.00001\" repeatCount=\"indefinite\" additive=\"replace\" accumulate=\"none\" fill=\"freeze\" id=\"animate203\">\r\n                </animate>\r\n            </circle>\r\n            <circle id=\"controller-b2\" cx=\"419.202\" cy=\"241.152\" r=\"19.865\">\r\n                <animate attributeType=\"auto\" attributeName=\"fill\" values=\"#000000;#000000;#00a7d1;#000000\" calcMode=\"spline\" keyTimes=\"0;0.75;0.875;1\" keySplines=\"0 0 1 1;0 0 1 1;0 0 1 1\" dur=\"4.00002\" begin=\"-0.00001\" repeatCount=\"indefinite\" additive=\"replace\" accumulate=\"none\" fill=\"freeze\" id=\"animate6753\">\r\n                </animate>\r\n            </circle>\r\n            <polygon id=\"controller-b9\" name=\"select\" points=\"333.062,246.586              349.171,241.751 333.062,237.381\">\r\n            </polygon>\r\n            <path id=\"layout\" d=\" M 561.234 241.611 C 558.793 223.30599999999998 551.063 189.536 517.7030000000001 166.34699999999998 L 509.5630000000001 135.01899999999998 C 509.5630000000001 135.01899999999998 508.7500000000001 127.28999999999998 498.5780000000001 121.59099999999998 C 498.5780000000001 121.59099999999998 499.1900000000001 116.30299999999998 489.8330000000001 113.86199999999998 C 480.4760000000001 111.41999999999999 458.7060000000001 106.74499999999998 433.2840000000001 115.28799999999998 C 433.2840000000001 115.28799999999998 429.4160000000001 116.30399999999999 429.6240000000001 122.00199999999998 C 429.6240000000001 122.00199999999998 417.6230000000001 127.90099999999998 415.1810000000001 138.88699999999997 C 412.7400000000001 149.87199999999996 409.4840000000001 163.70299999999997 409.4840000000001 163.70299999999997 L 372.5680000000001 163.70299999999997 L 211.195 163.70299999999997 L 171.635 163.70299999999997 C 171.635 163.70299999999997 168.379 149.87199999999999 165.93699999999998 138.88699999999997 S 151.49399999999997 122.00199999999997 151.49399999999997 122.00199999999997 C 151.69599999999997 116.30399999999997 147.83399999999997 115.28799999999997 147.83399999999997 115.28799999999997 C 122.40499999999997 106.74499999999998 100.64299999999997 111.41999999999997 91.28499999999997 113.86199999999997 C 81.92799999999997 116.30299999999997 82.53899999999997 121.59099999999997 82.53899999999997 121.59099999999997 C 72.36799999999997 127.28899999999996 71.55399999999997 135.01899999999998 71.55399999999997 135.01899999999998 L 63.41399999999997 166.34699999999998 C 30.053999999999974 189.53499999999997 22.32399999999997 223.30599999999998 19.881999999999977 241.611 C 17.441 259.921 1.579 380.344 0.354 394.58 C -0.864 408.82099999999997 -1.678 462.114 47.141999999999996 469.844 S 121.592 433.229 121.592 433.229 L 154.548 376.068 C 228.184 433.229 263.172 355.52299999999997 263.172 355.52299999999997 L 317.946 355.52299999999997 C 317.946 355.52299999999997 352.934 433.229 426.57000000000005 376.068 L 459.52600000000007 433.229 C 459.52600000000007 433.229 485.15700000000004 477.57399999999996 533.9760000000001 469.844 S 581.9810000000001 408.816 580.7630000000001 394.58 C 579.546 380.338 563.677 259.916 561.234 241.611 Z M 465.824 168.544 C 480.15000000000003 168.544 491.809 180.202 491.809 194.529 C 491.809 208.855 480.15000000000003 220.514 465.824 220.514 C 451.497 220.514 439.839 208.85500000000002 439.839 194.529 C 439.839 180.203 451.497 168.544 465.824 168.544 Z M 317.322 191.175 C 321.098 191.175 324.874 191.175 328.644 191.175 C 328.68 191.304 328.693 193.15800000000002 328.656 193.44 C 328.594 193.446 328.528 193.453 328.461 193.453 C 328.075 193.453 327.684 193.453 327.298 193.453 C 327.255 193.453 327.212 193.453 327.169 193.459 C 327.017 193.483 326.931 193.58100000000002 326.96099999999996 193.734 C 326.98499999999996 193.85000000000002 327.03399999999993 193.979 327.102 194.077 C 327.21799999999996 194.248 327.35299999999995 194.401 327.49399999999997 194.555 C 328.60799999999995 195.773 329.71599999999995 196.985 330.83 198.197 C 330.85999999999996 198.227 330.89099999999996 198.252 330.93399999999997 198.28900000000002 C 330.98299999999995 198.233 331.03299999999996 198.185 331.08099999999996 198.13600000000002 C 332.342 196.75900000000001 333.59599999999995 195.38200000000003 334.85599999999994 194.00500000000002 C 334.88699999999994 193.97400000000002 334.91799999999995 193.93800000000002 334.9359999999999 193.901 C 335.02199999999993 193.77300000000002 334.99799999999993 193.65 334.87999999999994 193.55200000000002 C 334.7819999999999 193.473 334.6719999999999 193.454 334.5439999999999 193.454 C 334.15199999999993 193.46 333.7539999999999 193.454 333.3619999999999 193.454 C 333.3069999999999 193.454 333.2519999999999 193.441 333.1849999999999 193.435 C 333.1849999999999 192.67600000000002 333.1849999999999 191.929 333.1849999999999 191.17000000000002 C 336.2449999999999 191.17000000000002 339.2979999999999 191.17000000000002 342.3649999999999 191.17000000000002 C 342.3649999999999 191.923 342.3649999999999 192.67000000000002 342.3649999999999 193.441 C 342.2979999999999 193.441 342.2299999999999 193.441 342.1629999999999 193.441 C 341.4169999999999 193.441 340.6699999999999 193.441 339.9299999999999 193.441 C 339.8069999999999 193.441 339.72199999999987 193.478 339.6359999999999 193.564 C 337.2249999999999 196.134 334.8139999999999 198.70499999999998 332.40199999999993 201.275 C 332.3229999999999 201.36100000000002 332.29299999999995 201.44 332.29299999999995 201.556 C 332.2989999999999 202.54700000000003 332.29299999999995 203.53300000000002 332.29299999999995 204.525 C 332.29299999999995 204.567 332.29299999999995 204.61 332.2989999999999 204.653 C 332.3179999999999 204.849 332.4149999999999 204.965 332.6169999999999 204.98999999999998 C 332.7019999999999 205.00199999999998 332.7879999999999 204.98999999999998 332.8799999999999 204.98999999999998 C 333.7189999999999 204.98999999999998 334.5569999999999 204.98999999999998 335.3959999999999 204.98999999999998 C 335.4199999999999 204.98999999999998 335.4449999999999 204.99599999999998 335.4819999999999 205.00199999999998 C 335.4819999999999 205.682 335.4819999999999 206.355 335.4819999999999 207.034 C 331.7729999999999 207.034 328.07699999999994 207.034 324.36799999999994 207.034 C 324.36799999999994 206.35399999999998 324.36799999999994 205.68099999999998 324.36799999999994 204.98399999999998 C 324.4409999999999 204.98399999999998 324.50899999999996 204.98399999999998 324.57599999999996 204.98399999999998 C 325.292 204.98399999999998 326.00199999999995 204.98399999999998 326.71799999999996 204.98399999999998 C 326.792 204.98399999999998 326.871 204.98399999999998 326.94499999999994 204.97699999999998 C 327.17699999999996 204.95299999999997 327.29999999999995 204.81799999999998 327.31899999999996 204.57999999999998 C 327.31899999999996 204.54299999999998 327.31899999999996 204.50699999999998 327.31899999999996 204.46999999999997 C 327.31899999999996 203.43599999999998 327.31899999999996 202.40199999999996 327.31899999999996 201.36699999999996 C 327.31899999999996 201.24499999999995 327.28799999999995 201.15899999999996 327.20199999999994 201.06699999999995 C 324.91999999999996 198.56399999999996 322.6309999999999 196.05999999999995 320.3539999999999 193.55699999999996 C 320.2679999999999 193.45899999999995 320.17599999999993 193.42199999999997 320.04799999999994 193.42199999999997 C 319.2149999999999 193.42899999999997 318.3829999999999 193.42199999999997 317.55099999999993 193.42199999999997 C 317.4839999999999 193.42199999999997 317.41599999999994 193.42199999999997 317.33599999999996 193.42199999999997 C 317.322 192.681 317.322 191.935 317.322 191.175 Z M 289.892 191.169 C 289.971 191.163 290.051 191.163 290.13 191.163 C 292.64 191.163 295.155 191.163 297.665 191.163 C 297.774 191.163 297.892 191.169 298.00100000000003 191.163 C 298.136 191.15 298.23300000000006 191.193 298.338 191.28500000000003 C 299.029 191.91600000000003 299.721 192.54000000000002 300.41200000000003 193.16400000000002 C 303.252 195.723 306.086 198.28000000000003 308.92600000000004 200.83900000000003 C 308.98100000000005 200.88800000000003 309.03600000000006 200.93700000000004 309.12800000000004 201.01700000000002 C 309.12800000000004 200.907 309.134 200.83300000000003 309.134 200.76600000000002 C 309.134 198.734 309.134 196.70800000000003 309.134 194.67600000000002 C 309.134 194.51700000000002 309.134 194.364 309.134 194.205 C 309.14 193.947 309.024 193.764 308.81 193.63000000000002 C 308.614 193.50700000000003 308.4 193.45200000000003 308.16700000000003 193.45200000000003 C 307.463 193.45200000000003 306.76500000000004 193.45200000000003 306.062 193.45200000000003 C 305.995 193.45200000000003 305.927 193.45200000000003 305.84700000000004 193.45200000000003 C 305.84700000000004 192.68600000000004 305.84700000000004 191.94600000000003 305.84700000000004 191.20000000000002 C 305.84700000000004 191.20000000000002 305.85400000000004 191.193 305.86 191.187 S 305.873 191.174 305.873 191.174 C 309.012 191.168 312.145 191.168 315.28499999999997 191.162 C 315.32199999999995 191.162 315.35799999999995 191.168 315.407 191.168 C 315.407 191.921 315.407 192.674 315.407 193.44400000000002 C 315.334 193.44400000000002 315.26599999999996 193.44400000000002 315.19899999999996 193.44400000000002 C 314.52 193.44400000000002 313.84599999999995 193.44400000000002 313.16799999999995 193.43800000000002 C 312.59899999999993 193.43800000000002 312.34799999999996 193.67700000000002 312.3229999999999 194.252 C 312.3159999999999 194.33700000000002 312.3159999999999 194.429 312.3159999999999 194.51500000000001 C 312.3159999999999 198.62800000000001 312.3159999999999 202.746 312.3159999999999 206.859 C 312.3159999999999 206.93900000000002 312.3159999999999 207.018 312.3159999999999 207.11 C 312.24299999999994 207.11700000000002 312.1939999999999 207.12300000000002 312.1389999999999 207.12300000000002 C 311.0739999999999 207.12300000000002 310.0089999999999 207.12300000000002 308.9439999999999 207.12300000000002 C 308.8349999999999 207.12300000000002 308.7549999999999 207.09300000000002 308.6749999999999 207.019 C 304.6049999999999 203.39600000000002 300.5349999999999 199.78 296.4649999999999 196.157 C 296.41599999999994 196.114 296.36099999999993 196.071 296.2809999999999 196.00400000000002 C 296.2809999999999 196.10200000000003 296.2809999999999 196.163 296.2809999999999 196.22500000000002 C 296.2809999999999 198.94800000000004 296.2809999999999 201.67100000000002 296.2809999999999 204.395 C 296.2809999999999 204.49900000000002 296.29399999999987 204.609 296.3049999999999 204.71300000000002 C 296.3239999999999 204.82900000000004 296.3969999999999 204.90200000000002 296.51999999999987 204.92100000000002 C 296.65499999999986 204.94000000000003 296.79499999999985 204.94500000000002 296.9299999999999 204.94500000000002 C 297.6949999999999 204.94500000000002 298.4589999999999 204.94500000000002 299.2249999999999 204.94500000000002 C 299.2919999999999 204.94500000000002 299.3599999999999 204.94500000000002 299.4389999999999 204.94500000000002 C 299.4389999999999 205.67400000000004 299.4389999999999 206.383 299.4389999999999 207.10500000000002 C 296.2689999999999 207.10500000000002 293.11099999999993 207.10500000000002 289.9349999999999 207.10500000000002 C 289.9349999999999 206.389 289.9349999999999 205.67900000000003 289.9349999999999 204.94500000000002 C 290.00799999999987 204.94500000000002 290.0759999999999 204.94500000000002 290.1429999999999 204.94500000000002 C 290.8399999999999 204.94500000000002 291.5319999999999 204.94500000000002 292.2299999999999 204.94500000000002 C 292.3089999999999 204.94500000000002 292.3949999999999 204.94500000000002 292.47399999999993 204.93200000000002 C 292.71899999999994 204.89600000000002 292.85999999999996 204.73700000000002 292.87199999999996 204.48600000000002 C 292.87199999999996 204.437 292.87199999999996 204.388 292.87199999999996 204.33300000000003 C 292.87199999999996 200.86300000000003 292.87199999999996 197.39400000000003 292.87199999999996 193.92300000000003 C 292.87199999999996 193.87400000000002 292.87199999999996 193.82500000000002 292.87199999999996 193.77100000000004 C 292.86499999999995 193.47700000000003 292.756 193.34200000000004 292.47399999999993 193.29300000000003 C 292.33899999999994 193.26900000000003 292.19899999999996 193.25600000000003 292.0639999999999 193.25600000000003 C 291.4159999999999 193.25000000000003 290.7609999999999 193.25600000000003 290.11099999999993 193.25600000000003 C 290.0439999999999 193.25600000000003 289.97599999999994 193.25600000000003 289.88399999999996 193.25600000000003 C 289.892 192.565 289.892 191.879 289.892 191.169 Z M 264.072 195.111 C 264.733 194.034 265.595 193.146 266.612 192.39999999999998 C 267.995 191.384 269.53700000000003 190.69899999999998 271.196 190.25699999999998 C 272.84200000000004 189.81699999999998 274.519 189.65699999999998 276.22 189.73699999999997 C 278.31300000000005 189.83499999999998 280.321 190.28799999999995 282.218 191.19299999999996 C 283.54 191.82399999999996 284.733 192.64399999999995 285.731 193.72699999999995 C 286.61899999999997 194.68799999999996 287.28 195.77099999999996 287.61 197.04299999999995 C 288.063 198.75099999999995 287.861 200.39699999999996 287.06600000000003 201.96399999999994 C 286.40500000000003 203.26199999999994 285.43800000000005 204.30199999999994 284.27500000000003 205.16499999999994 C 282.86100000000005 206.21099999999993 281.28200000000004 206.90899999999993 279.593 207.34999999999994 C 278.61400000000003 207.60699999999994 277.622 207.76599999999993 276.613 207.83299999999994 C 276.221 207.85699999999994 275.823 207.83899999999994 275.432 207.83899999999994 C 275.432 207.84499999999994 275.432 207.85199999999995 275.432 207.85799999999995 C 274.92400000000004 207.83899999999994 274.416 207.85199999999995 273.91400000000004 207.80299999999994 C 271.27000000000004 207.56399999999994 268.81600000000003 206.78099999999995 266.64900000000006 205.21399999999994 C 265.5110000000001 204.39399999999995 264.56800000000004 203.39599999999993 263.88900000000007 202.15299999999993 C 263.28400000000005 201.04599999999994 262.9770000000001 199.86399999999995 263.01400000000007 198.60299999999992 C 263.043 197.351 263.417 196.188 264.072 195.111 Z M 240.008 192.565 C 240.565 192.003 241.214 191.57399999999998 241.917 191.212 C 242.958 190.67999999999998 244.053 190.331 245.191 190.08599999999998 C 246.366 189.83499999999998 247.547 189.694 248.75300000000001 189.706 C 249.69000000000003 189.719 250.614 189.82899999999998 251.532 190.006 C 252.854 190.264 254.145 190.643 255.394 191.157 C 255.644 191.26100000000002 255.883 191.401 256.141 191.51100000000002 C 256.226 191.54800000000003 256.33000000000004 191.57300000000004 256.42900000000003 191.56700000000004 C 256.61800000000005 191.55400000000003 256.67400000000004 191.48700000000002 256.67400000000004 191.29200000000003 C 256.67400000000004 191.10200000000003 256.67400000000004 190.90600000000003 256.67400000000004 190.71600000000004 C 256.82700000000006 190.67400000000004 258.82200000000006 190.66700000000003 259.04300000000006 190.70400000000004 C 259.04300000000006 192.54000000000005 259.04300000000006 194.37600000000003 259.04300000000006 196.22500000000002 C 258.2780000000001 196.22500000000002 257.5250000000001 196.22500000000002 256.7540000000001 196.22500000000002 C 256.7420000000001 196.139 256.7360000000001 196.06600000000003 256.72300000000007 195.98600000000002 C 256.6310000000001 195.31900000000002 256.3190000000001 194.76800000000003 255.84200000000007 194.30300000000003 C 255.38300000000007 193.86300000000003 254.84400000000008 193.55000000000004 254.26300000000006 193.30500000000004 C 253.47300000000007 192.96200000000005 252.64100000000005 192.74800000000005 251.79700000000005 192.60200000000003 C 250.59700000000007 192.38700000000003 249.38500000000005 192.30200000000002 248.16800000000006 192.33800000000002 C 247.26900000000006 192.36900000000003 246.36900000000006 192.44200000000004 245.48700000000005 192.66200000000003 C 245.12600000000006 192.75400000000005 244.77800000000005 192.87000000000003 244.44700000000006 193.04200000000003 C 244.20800000000006 193.16400000000004 243.98200000000006 193.31700000000004 243.80400000000006 193.52000000000004 C 243.59600000000006 193.75200000000004 243.47900000000007 194.02200000000005 243.46100000000007 194.33300000000003 C 243.44900000000007 194.59700000000004 243.54100000000008 194.82300000000004 243.74200000000008 194.97600000000003 C 243.90800000000007 195.10500000000002 244.09700000000007 195.21500000000003 244.28700000000006 195.29400000000004 C 244.73400000000007 195.49000000000004 245.21700000000007 195.58200000000005 245.70100000000005 195.66700000000003 C 246.65000000000006 195.83300000000003 247.61100000000005 195.91800000000003 248.57200000000006 196.01000000000002 C 250.10200000000006 196.163 251.63800000000006 196.30400000000003 253.15500000000006 196.573 C 254.33000000000007 196.781 255.48100000000005 197.05100000000002 256.58900000000006 197.51500000000001 C 257.28700000000003 197.80300000000003 257.94200000000006 198.163 258.51700000000005 198.66500000000002 C 259.41100000000006 199.442 259.91300000000007 200.41500000000002 260.01000000000005 201.597 C 260.14500000000004 203.12 259.61800000000005 204.369 258.46200000000005 205.36100000000002 C 257.75200000000007 205.967 256.93800000000005 206.383 256.0690000000001 206.71400000000003 C 255.05300000000008 207.10000000000002 254.00100000000006 207.34500000000003 252.93000000000006 207.51000000000002 C 252.02400000000006 207.645 251.11800000000005 207.73700000000002 250.20100000000005 207.73700000000002 C 249.63800000000006 207.73700000000002 249.07500000000005 207.67000000000002 248.51800000000006 207.57800000000003 C 246.70000000000005 207.27800000000002 244.91300000000007 206.84900000000002 243.15100000000007 206.31000000000003 C 242.91800000000006 206.23700000000002 242.69200000000006 206.15800000000004 242.47800000000007 206.03500000000003 C 242.27000000000007 205.913 242.02500000000006 205.919 241.76800000000006 205.92600000000002 C 241.76800000000006 206.26200000000003 241.76800000000006 206.59300000000002 241.76800000000006 206.929 C 240.85600000000005 206.929 239.96300000000005 206.929 239.06900000000005 206.929 C 239.02600000000004 206.80100000000002 239.00700000000003 201.024 239.05000000000004 200.668 C 239.18500000000003 200.632 241.42500000000004 200.626 241.61400000000003 200.662 C 241.63800000000003 200.834 241.65100000000004 201.005 241.68700000000004 201.177 C 241.82800000000003 201.899 242.20100000000005 202.487 242.73300000000003 202.976 C 243.32700000000003 203.527 244.02500000000003 203.9 244.76500000000004 204.2 C 245.61600000000004 204.54299999999998 246.50300000000004 204.76299999999998 247.40900000000005 204.91 C 248.60900000000004 205.106 249.82000000000005 205.185 251.03800000000004 205.137 C 251.95600000000005 205.101 252.85500000000005 204.966 253.71800000000005 204.617 C 254.19600000000005 204.427 254.63000000000005 204.176 255.00300000000004 203.821 C 255.19900000000004 203.637 255.37100000000004 203.429 255.49900000000005 203.19 C 255.56000000000006 203.081 255.57200000000006 202.97 255.54800000000006 202.847 C 255.48700000000005 202.53 255.31600000000006 202.285 255.07000000000005 202.08200000000002 C 254.83200000000005 201.88700000000003 254.55600000000004 201.752 254.26800000000006 201.63600000000002 C 253.69300000000007 201.40300000000002 253.08700000000005 201.26900000000003 252.47500000000005 201.15800000000002 C 251.45300000000006 200.96900000000002 250.41900000000004 200.85100000000003 249.39100000000005 200.735 C 247.91600000000005 200.56400000000002 246.44100000000006 200.411 244.98500000000004 200.14100000000002 C 243.81000000000003 199.92100000000002 242.64700000000005 199.65200000000002 241.54600000000005 199.162 C 240.87900000000005 198.868 240.25400000000005 198.495 239.74000000000004 197.96200000000002 C 239.09200000000004 197.28300000000002 238.77300000000002 196.47600000000003 238.76100000000002 195.53900000000002 C 238.741 194.365 239.194 193.391 240.008 192.565 Z M 93.911 200.502 C 94.615 190.56400000000002 108.18900000000001 190.55800000000002 113.97800000000001 190.55100000000002 C 119.82300000000001 190.55800000000002 133.38500000000002 190.56400000000002 134.101 200.496 C 134.48 205.82000000000002 132.687 221.72 130.588 225.906 C 128.813 229.45600000000002 120.78399999999999 235.53900000000002 116.00999999999999 238.942 C 115.87499999999999 239.06400000000002 115.72899999999998 239.168 115.56299999999999 239.25900000000001 C 115.07999999999998 239.54100000000003 114.54099999999998 239.68200000000002 114.00899999999999 239.68200000000002 C 113.80699999999999 239.68200000000002 113.60499999999999 239.65800000000002 113.40899999999999 239.62 C 113.17099999999999 239.571 112.93799999999999 239.498 112.71799999999999 239.4 C 112.45499999999998 239.27100000000002 112.216 239.125 112.002 238.941 C 108.795 236.66400000000002 99.339 229.73 97.42999999999999 225.905 C 95.325 221.72 93.525 205.821 93.911 200.502 Z M 70.288 264.316 C 60.349999999999994 263.61199999999997 60.342999999999996 250.03799999999998 60.342999999999996 244.24299999999997 C 60.342999999999996 238.40399999999997 60.349 224.82999999999996 70.294 224.12599999999998 C 70.78999999999999 224.08999999999997 71.377 224.07099999999997 72.044 224.07099999999997 C 77.699 224.07099999999997 91.573 225.57699999999997 95.704 227.63899999999998 C 99.529 229.54799999999997 106.463 239.016 108.74 242.22299999999998 C 108.942 242.46099999999998 109.11399999999999 242.731 109.23599999999999 243.03099999999998 C 109.40199999999999 243.42799999999997 109.481 243.85099999999997 109.475 244.26699999999997 C 109.469 244.71899999999997 109.365 245.17799999999997 109.157 245.60099999999997 C 109.041 245.82799999999997 108.89999999999999 246.04099999999997 108.741 246.23099999999997 C 106.93599999999999 248.78999999999996 99.69 258.82 95.705 260.80899999999997 C 91.58 262.871 77.7 264.37699999999995 72.045 264.37699999999995 C 71.377 264.371 70.784 264.353 70.288 264.316 Z M 134.101 287.927 C 133.391 297.87100000000004 119.823 297.87800000000004 114.03399999999999 297.884 L 114.03399999999999 297.884 L 114.00999999999999 297.884 C 108.19 297.87800000000004 94.62199999999999 297.87100000000004 93.91799999999999 287.927 C 93.53899999999999 282.615 95.332 266.71500000000003 97.431 262.523 C 99.34 258.704 108.80799999999999 251.764 112.009 249.48700000000002 C 112.144 249.371 112.291 249.26700000000002 112.45 249.175 C 112.682 249.04000000000002 112.927 248.936 113.178 248.863 C 113.973 248.649 114.837 248.735 115.571 249.157 C 115.767 249.27300000000002 115.95 249.40800000000002 116.11 249.554 C 120.908 252.982 128.833 258.997 130.59 262.517 C 132.681 266.721 134.48 282.615 134.101 287.927 Z M 155.968 264.371 C 150.319 264.371 136.439 262.866 132.314 260.804 C 128.32399999999998 258.815 121.07199999999999 248.75999999999996 119.27799999999999 246.21999999999997 C 119.14999999999999 246.07299999999998 119.03299999999999 245.90199999999996 118.93499999999999 245.72399999999996 C 118.64199999999998 245.19199999999995 118.51299999999999 244.59199999999996 118.54999999999998 244.00399999999996 C 118.57399999999998 243.63099999999997 118.66599999999998 243.26399999999995 118.83099999999999 242.90799999999996 C 118.954 242.65099999999995 119.106 242.41299999999995 119.28999999999999 242.19799999999995 C 121.585 238.97899999999996 128.501 229.54899999999995 132.313 227.63899999999995 C 136.444 225.57699999999994 150.31799999999998 224.07099999999994 155.97299999999998 224.07099999999994 C 156.634 224.07099999999994 157.22699999999998 224.08999999999995 157.72899999999998 224.12599999999995 C 167.601 224.82999999999996 167.67399999999998 238.22699999999995 167.67399999999998 244.08399999999995 C 167.67399999999998 244.13299999999995 167.67399999999998 244.17599999999996 167.67399999999998 244.21899999999994 C 167.66799999999998 250.03799999999993 167.66199999999998 263.61299999999994 157.71699999999998 264.31699999999995 C 157.223 264.353 156.629 264.371 155.968 264.371 Z M 200.111 364.892 C 177.785 364.892 159.62099999999998 346.728 159.62099999999998 324.408 C 159.62099999999998 302.083 177.78499999999997 283.91900000000004 200.111 283.91900000000004 S 240.601 302.083 240.601 324.408 C 240.607 346.728 222.437 364.892 200.111 364.892 Z M 253.013 247.712 C 253.013 250.404 250.822 252.60199999999998 248.12300000000002 252.60199999999998 L 226.76500000000001 252.60199999999998 C 224.066 252.60199999999998 221.87500000000003 250.41199999999998 221.87500000000003 247.712 L 221.87500000000003 237.54 C 221.87500000000003 234.84799999999998 224.06600000000003 232.65 226.76500000000001 232.65 L 248.12300000000002 232.65 C 250.82200000000003 232.65 253.013 234.841 253.013 237.54 L 253.013 247.712 Z M 290.559 329.756 C 271.819 329.756 256.56800000000004 314.51199999999994 256.56800000000004 295.76599999999996 C 256.56800000000004 277.02099999999996 271.81300000000005 261.77599999999995 290.559 261.77599999999995 S 324.55 277.02099999999996 324.55 295.76599999999996 C 324.55 314.512 309.305 329.756 290.559 329.756 Z M 332.708 253.08 C 332.427 253.17200000000003 332.12600000000003 253.209 331.826 253.209 C 329.134 253.209 326.93600000000004 251.018 326.93600000000004 248.318 L 326.93600000000004 235.705 C 326.93600000000004 232.73100000000002 329.79400000000004 230.13600000000002 332.627 230.92000000000002 L 358.87 238.043 C 360.204 238.404 361.128 239.616 361.128 240.993 C 361.128 243.50199999999998 359.22499999999997 245.577 356.789 245.852 L 332.708 253.08 Z M 381.362 364.892 C 359.036 364.892 340.872 346.728 340.872 324.408 C 340.872 302.083 359.036 283.91900000000004 381.362 283.91900000000004 C 403.687 283.91900000000004 421.851 302.083 421.851 324.408 C 421.852 346.728 403.688 364.892 381.362 364.892 Z M 419.202 267.137 C 404.875 267.137 393.216 255.479 393.216 241.152 S 404.875 215.16699999999997 419.202 215.16699999999997 C 433.528 215.16699999999997 445.187 226.82499999999996 445.187 241.152 C 445.182 255.479 433.528 267.137 419.202 267.137 Z M 465.824 313.759 C 451.497 313.759 439.839 302.101 439.839 287.774 S 451.497 261.789 465.824 261.789 C 480.15000000000003 261.789 491.809 273.447 491.809 287.774 C 491.804 302.101 480.15 313.759 465.824 313.759 Z M 512.446 267.137 C 498.119 267.137 486.461 255.479 486.461 241.152 S 498.119 215.16699999999997 512.446 215.16699999999997 S 538.431 226.82499999999996 538.431 241.152 C 538.426 255.479 526.772 267.137 512.446 267.137 Z \">\r\n            </path>\r\n            <path d=\" M 270.437,203.63 c 1.407,0.997,2.993,1.444,4.541,1.462 c 1.181,0,2.167 -0.171,3.103 -0.52 c 0.067 -0.024,0.116 -0.074,0.184 -0.099 c 0.012 -0.006,0.031,0,0.043 -0.006 c 1.334 -0.502,2.466 -1.279,3.292 -2.454 c 1.114 -1.579,1.377 -3.292,0.704 -5.116 c -0.472 -1.279 -1.334 -2.265 -2.473 -2.986 c -0.563 -0.355 -1.138 -0.631 -1.72 -0.839 c -1.806 -0.703 -3.703 -0.74 -5.667 -0.11 c -1.396,0.447 -2.577,1.236 -3.464,2.412 c -1.193,1.585 -1.53,3.341 -0.888,5.232 C 268.527,201.874,269.353,202.859,270.437,203.63 z \" id=\"path5693\">\r\n            </path>\r\n            <circle cx=\"290.000\" cy=\"295.000\" r=\"35\" width=\"50.898\" height=\"100.711\" stroke=\"black\" fill=\"black\" id=\"circle7461\">\r\n            </circle>\r\n            <rect x=\"227.994\" y=\"190.0\" width=\"120.898\" height=\"17.5\" stroke=\"black\" fill=\"black\" id=\"rect3729\">\r\n            </rect>\r\n            <rect id=\"controller-b4\" x=\"83.5\" y=\"110.0\" width=\"70\" height=\"20\" rx=\"16.5\" stroke-width=\"3\">\r\n            </rect>\r\n            <rect id=\"controller-b5\" x=\"429.5\" y=\"110.0\" width=\"70\" height=\"20\" rx=\"16.5\" stroke-width=\"3\">\r\n            </rect>\r\n            <rect id=\"controller-b6\" x=\"99.994\" y=\"50.0\" height=\"50.898\" width=\"35.5\" fill=\"rgb(0, 167, 209)\" rx=\"19\" ry=\"10\">\r\n                <animate attributeName=\"fill-opacity\" keyTimes=\"0; 0.19424603174603178; 0.5952380952380952; 0.7936507936507935; 1\" values=\"0; 1; 1; 0; 0\" begin=\"-0.00001\" dur=\"5.04002\" fill=\"freeze\" calcMode=\"spline\" keySplines=\"0 0 1 1; 0 0 1 1; 0 0 1 1; 0 0 1 1\" repeatCount=\"indefinite\">\r\n                </animate>\r\n            </rect>\r\n            <rect id=\"controller-b7\" x=\"448.994\" y=\"50.0\" height=\"50.898\" width=\"35.5\" fill=\"black\" rx=\"19\" ry=\"10\">\r\n            </rect>\r\n            <circle id=\"controller-b10-below\" name=\"left-stick\" cx=\"200.000\" cy=\"325.000\" r=\"15\" width=\"50.898\" height=\"100.711\" stroke=\"black\" fill=\"black\">\r\n            </circle>\r\n            <a href=\"#textboxA\" id=\"a-fQ8TCzJ3NTHAiH9Z2R5Pp6\">\r\n                <text x=\"7.06777\" y=\"92.709\" id=\"textA\" font-size=\"65px\" fill=\"rgb(0, 0, 0)\" font-family=\"Calibri\" transform=\"\">\r\n                    [A]\r\n                </text>\r\n            </a>\r\n            <a href=\"#textboxB\" id=\"a-23JHBwVEKPimXQ3TMdXeYZ\">\r\n                <text x=\"160\" y=\"450\" id=\"textB\" font-size=\"65\" fill=\"rgb(0, 0, 0)\" font-family=\"Calibri\">\r\n                    [C]\r\n                </text>\r\n            </a>\r\n            <a href=\"#textboxC\" id=\"a-fnn2zy4XLpEfwdVdFiX3G9\">\r\n                <text x=\"345\" y=\"450\" id=\"textC\" font-size=\"65\" fill=\"rgb(0, 0, 0)\" font-family=\"Calibri\">\r\n                    [D]\r\n                </text>\r\n            </a>\r\n            <a href=\"#textboxD\" id=\"a-5ZEKVzJhdjxJydgQ91ocK3\">\r\n                <text x=\"489\" y=\"330\" id=\"textD\" font-size=\"65\" fill=\"rgb(0, 160, 201)\" font-family=\"Calibri\">\r\n                    [E]\r\n                </text>\r\n            </a>\r\n            <circle id=\"controller-b10\" name=\"left-stick\" cx=\"200\" cy=\"325\" r=\"35\" width=\"50.898\" height=\"100.711\" fill=\"rgb(105, 4, 4)\" transform-origin=\"center\" style=\"transform-box: fill-box;\">\r\n                <animateTransform attributeName=\"transform\" attributeType=\"auto\" type=\"translate\" values=\"0 0;1.25 -15.44;1.25 15.44;0 0\" calcMode=\"spline\" keyTimes=\"0; 0.3333333333333333; 0.6666666666666666; 1\" keySplines=\"0 0 1 1;0 0 1 1;0 0 1 1\" dur=\"6.00002\" begin=\"-0.00001\" repeatCount=\"indefinite\" additive=\"sum\" accumulate=\"none\" fill=\"freeze\" id=\"animateTransform1567\">\r\n                </animateTransform>\r\n                <animate attributeType=\"auto\" attributeName=\"fill\" values=\"#000000;#00a7d1;#000000;#00a7d1;#000000\" calcMode=\"spline\" keyTimes=\"0; 0.33316666666666667; 0.5; 0.75; 1\" keySplines=\"0 0 1 1;0 0 1 1;0 0 1 1;0 0 1 1\" dur=\"6.00002\" begin=\"-0.00001\" repeatCount=\"indefinite\" additive=\"replace\" accumulate=\"none\" fill=\"freeze\" id=\"animate4627\">\r\n                </animate>\r\n            </circle>\r\n            <circle id=\"controller-b11-bottom\" name=\"stick-right\" cx=\"381.000\" cy=\"325.000\" r=\"15\" width=\"50.898\" height=\"100.711\" stroke=\"black\" fill=\"black\">\r\n            </circle>\r\n            <circle id=\"controller-b11\" name=\"stick-right\" cx=\"381.000\" cy=\"325.000\" r=\"35\" width=\"50.898\" height=\"100.711\" fill=\"rgb(105, 4, 4)\" transform-origin=\"center\" style=\"transform-box: fill-box;\">\r\n                <animateTransform attributeName=\"transform\" attributeType=\"auto\" type=\"translate\" values=\"0 0;-1.99 -13.45;0 0;0 0\" calcMode=\"spline\" keyTimes=\"0;0.6666666666666665;0.7000000000000001;1\" keySplines=\"0 0 1 1;0 0 1 1;0 0 1 1\" dur=\"3.00002\" begin=\"-0.00001\" repeatCount=\"indefinite\" additive=\"sum\" accumulate=\"none\" fill=\"freeze\" id=\"animateTransform6427\">\r\n                </animateTransform>\r\n                <animate attributeType=\"auto\" attributeName=\"fill\" values=\"#000000;#00a7d1;#000000;#000000\" calcMode=\"spline\" keyTimes=\"0;0.6666666666666666;0.7000000000000001;1\" keySplines=\"0 0 1 1;0 0 1 1;0 0 1 1\" dur=\"3.00002\" begin=\"-0.00001\" repeatCount=\"indefinite\" additive=\"replace\" accumulate=\"none\" fill=\"freeze\" id=\"animate8889\">\r\n                </animate>\r\n            </circle>\r\n        </g>\r\n    </g>\r\n    <g id=\"g2473\">\r\n    </g>\r\n    <g id=\"g6682\">\r\n    </g>\r\n    <g id=\"g2475\">\r\n    </g>\r\n    <g id=\"g4965\">\r\n    </g>\r\n    <g id=\"g3017\">\r\n    </g>\r\n    <g id=\"g1284\">\r\n    </g>\r\n    <g id=\"g9276\">\r\n    </g>\r\n    <g id=\"g7311\">\r\n    </g>\r\n    <g id=\"g3181\">\r\n    </g>\r\n    <g id=\"g2320\">\r\n    </g>\r\n    <g id=\"g4450\">\r\n    </g>\r\n    <g id=\"g8574\">\r\n    </g>\r\n    <g id=\"g9668\">\r\n    </g>\r\n    <g id=\"g189\">\r\n    </g>\r\n    <g id=\"g5070\">\r\n    </g>\r\n    <text x=\"17.0268\" y=\"312.709\" font-family=\"Calibri\" font-size=\"65\" fill=\"rgb(0, 160, 201)\" transform=\"\">\r\n        [B]\r\n    </text>\r\n</svg>\r\n</div><p style='line-height:1.5;padding:0;margin:0;margin-bottom:5px;'><span style='-webkit-line-clamp:2;' class='truncate' title='[A] Toggle:</strong> Hold a control down until you Move Your Mouse or Click into it again (great for holding gas button in racing games)'><strong>[A] Toggle:</strong> Hold a control down until you Move Your Mouse or Click into it again (great for holding gas button in racing games)</span></p><p style='line-height:1.5;padding:0;margin:0;margin-bottom:5px;'><span style='-webkit-line-clamp:2;' class='truncate' title='[B] Combine Controls:</strong> Check a Combine checkbox to combine adjacent controls (great for games that need two controls pressed at same time)'><strong>[B] Combine Controls:</strong> Check a Combine checkbox to combine adjacent controls (great for games that need two controls pressed at same time)</span></p><p style='line-height:1.5;padding:0;margin:0;margin-bottom:5px;'><span style='-webkit-line-clamp:2;' class='truncate' title='[C] Variable Triggers/Sticks:</strong> In Gamepad or Nintendo Switch mode, push Analog Sticks/Triggers further as your mouse moves away from center'><strong>[C] Variable Triggers/Sticks:</strong> In Gamepad or Nintendo Switch mode, push Analog Sticks/Triggers further as mouse moves away from center</span></p><p style='line-height:1.5;padding:0;margin:0;margin-bottom:5px;'><span style='-webkit-line-clamp:2;' class='truncate' title='[D] Return to Center On Click:</strong> Return your mouse when you release your Click in it (great for stealth games where you need to stop quickly)'><strong>[D] Return to Center On Click:</strong> Return your mouse when you release your Click in it (great for stealth games where you need to stop quickly)</span></p><p style='line-height:1.5;padding:0;margin:0;margin-bottom:5px;'><span style='-webkit-line-clamp:2;' class='truncate' title='[E] Press Control Once:</strong> Press a control once, instead of holding it, when you Move Your Mouse into the quadrant (great for rhythm games)'><strong>[E] Press Control Once:</strong> Press a control once, instead of holding it, when you Move Your Mouse into the quadrant (great for rhythm games)</span></p>", Target = optUp},

                         new TourStep {Title = @"Configure Deadzone", Text = "<p style='line-height:1.5;padding:0;margin:0;'><span style='-webkit-line-clamp:3;' class='truncate' title='The Deadzone is the center circle of Overjoyed where all Controls are released. Assign controls to the Upper, Middle, or Lower quadrants to press buttons when you are not moving.'>The Deadzone is the center circle of Overjoyed where all Controls are released. Assign controls to the Upper, Middle, or Lower quadrants to press buttons when you are not moving.</span></p><br><p style='line-height:1.5;padding:0;margin:0;'><span style='-webkit-line-clamp:2;' class='truncate' title='Double clicking in the light blue quadrant in the center of the deadzone will activate and deactivate Overjoyed.'>Double clicking in the light blue quadrant in the center of the deadzone will activate and deactivate Overjoyed.</span></p>", Target = pnlDeadzone, Alignment = Placement.TopLeft},

                         new TourStep {Title = @"Advanced - Deactivation Zone", Text = "<p style='line-height:1.5;padding:0;margin:0;'><span style='-webkit-line-clamp:2;' class='truncate' title='Choose which deadzone quadrant deactivates the Overjoyed Interface when you Double Click it by pressing Upper, Middle, or Lower below.'>Choose which deadzone quadrant deactivates the Overjoyed Interface when you Double Click it by pressing Upper, Middle, or Lower below.</span></p>", Target = pnlDZImage, Alignment = Placement.TopCenter},

                         new TourStep {Title = @"You Are Ready To Play!", Text = "<p style='line-height:1.5;padding:0;margin:0;'><span style='-webkit-line-clamp:3;' class='truncate' title='Finally, we are done assigning controls & configuring settings, so please click the  Start Overjoyed  button below! This window will transform into the Overjoyed Overlay window that you control your game from.'>Finally, we are done assigning controls & configuring settings, so please click the  Start Overjoyed  button below! This window will transform into the Overjoyed Overlay window that you control your game from.</span></p><br><p style='line-height:1.5;padding:0;margin:0;'><span style='-webkit-line-clamp:3;' class='truncate' title='When you feel ready, open your game and double click in the center to activate Overjoyed. Enjoy using just your mouse cursor to play your game with minimal body or eye movement!'>When you feel ready, open your game and double click in the center to activate Overjoyed. Enjoy using just your mouse cursor to play your game with minimal body or eye movement!</span></p><br><p style='line-height:1.5;padding:0;margin:0;'><span style='-webkit-line-clamp:3;' class='truncate' title='You can access this Guided Tour at any time by clicking  Guided Tour  in the bottom left corner. Please share your feedback & feature requests using the bottom left  Click To Provide Feedback  button. Thank you!'>You can access this Guided Tour at any time by clicking  Guided Tour  in the bottom left corner. Please share your feedback & feature requests using the bottom left  Click To Provide Feedback  button. Thank you!</span></p>", Target = btnSubmit, Alignment = Placement.TopCenter }


                }
            };


            window.BeforeStep += TourPanel1_BeforeStep;
            window.Closed += TourPanel1_Closed;
            window.Show();

            window.CssClass = "GuidedTour";
            window.Steps[4].Enabled = false;

        }





        private void TourPanel1_Closed(object sender, EventArgs e)
        {
            var closedWindow = window;


            chkKeyboard.Checked = true;


            cmbUpHover.CssStyle = "";
            cmbUpRightHover.CssStyle = "";
            cmbRightHover.CssStyle = "";
            cmbDownRightHover.CssStyle = "";
            cmbDownHover.CssStyle = "";
            cmbDownLeftHover.CssStyle = "";
            cmbLeftHover.CssStyle = "";
            cmbUpLeftHover.CssStyle = "";

            btnUpHover.CssStyle = "";
            btnUpRightHover.CssStyle = "";
            btnRightHover.CssStyle = "";
            btnDownRightHover.CssStyle = "";
            btnDownHover.CssStyle = "";
            btnDownLeftHover.CssStyle = "";
            btnLeftHover.CssStyle = "";
            btnUpLeftHover.CssStyle = "";



            Control controlToRemove = window.Controls["8BitDoLogo"];
            if (controlToRemove != null)
            {
                window.Controls.Remove(controlToRemove);
            }

            Control panelToRemove = window.Controls["AdvancedControls"];
            if (panelToRemove != null)
            {
                window.Controls.Remove(panelToRemove);
            }

            Control panelToRemove2 = window.Controls["DisableControls"];
            if (panelToRemove2 != null)
            {
                window.Controls.Remove(panelToRemove2);
            }

            chkXbox.Checked = false;

            try
            {
                string line2 = "";
                using (FileStream fs = new FileStream(tourFilePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader reader = new StreamReader(fs))
                {
                    reader.ReadLine();
                    line2 = reader.ReadLine();
                }


                // Write the updated parameters back to the file with shared read/write access
                using (FileStream fs = new FileStream(tourFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                using (StreamWriter writer = new StreamWriter(fs))
                {

                    writer.WriteLine("tourNo");
                    writer.WriteLine(line2);
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



            Eval(@" const current = document.querySelector('.gt-lang-code').textContent.trim();
App.Config.currentLanguage(current); location.reload();");

            if (tourEnd)
            {
                btnSubmit.PerformClick();
            }
            else
            {
                ExtendedDialogResult dialogResult = ExtendedMessageBox.Show("<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 3; -webkit-box-orient: vertical; overflow: hidden; ' title='You have exited the Guided Tour. You can access the Guided Tour at any time by clicking 'Guided Tour' in the bottom left corner of the main Overjoyed window.'>You have exited the Guided Tour. You can access the Guided Tour at any time by clicking 'Guided Tour' in the bottom left corner of the main Overjoyed window.</span></p>", "Guided Tour Exited", ExtendedMessageBoxLibrary.MessageBoxButtons.OK, lines: 3);
                if (dialogResult.Result == DialogResult.OK)
                {
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
                    if (currentLang != "en")
                    {
                        Eval(@"setTimeout(() => {
const switcher = document.querySelector('.gt_float_switcher');

if (switcher) {
    // List of events to try
    const events = ['mouseenter', 'mouseover', 'mousemove', 'pointerenter', 'focus'];

    // Dispatch each event
    events.forEach(eventName => {
        const event = new MouseEvent(eventName, { bubbles: true });
        switcher.dispatchEvent(event);
    });
switcher.style.display = 'none';
}



}, 500);");
                    }
                    else
                    {
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
                    }
                }
            }

            if (closedWindow != null)
            {
                closedWindow.BeforeStep -= TourPanel1_BeforeStep;
                closedWindow.Closed -= TourPanel1_Closed;
                closedWindow.Dispose();
            }

            window = null;


        }


        private void TourPanel1_BeforeStep(object sender, TourPanelEventArgs e)
        {
            int preferredHeight = 300;


            if (e.StepIndex == 0)
            {
                Eval(@"
(function tryMoveScrollbar(){
    const scrollbar = document.querySelector('[name=""scrollbar-y""]');
    if (scrollbar) {
        document.body.appendChild(scrollbar);
    } else {
        setTimeout(tryMoveScrollbar, 100);
    }
})();
");
                nintendoSwitch.Checked = false;
                chkXbox.Checked = false;
                preferredHeight = 590;

                Control controlToRemove = window.Controls["8BitDoLogo"];
                if (controlToRemove != null)
                {
                    window.Controls.Remove(controlToRemove);
                }

                window.Controls.Add(new PictureBox
                {
                    Image = System.Drawing.Image.FromFile(Path.Combine(configPath, "8BitDo-Logo.png")),
                    Size = new System.Drawing.Size(180, 49),
                    Location = new System.Drawing.Point(160, 315),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Cursor = Cursors.Hand,
                    Name = "8BitDoLogo"
                });

                window.Controls["8BitDoLogo"].Click += new System.EventHandler(this.BitDoLogo_Click);


                window.Controls["8BitDoLogo"].BringToFront();
                if (Screen.Bounds.Height >= 1100 && Screen.Bounds.Width > 1920)
                {
                    window.Controls["8BitDoLogo"].Scale(1.4f);
                    if (initialTour == true)
                    {
                        ScaleFonts(window, 1.4f);
                    }
                }

                this.VerticalScroll.Value = 0;

            }
            else if (e.StepIndex == 1)
            {
                initialTour = false;
                Control controlToRemove = window.Controls["8BitDoLogo"];
                if (controlToRemove != null)
                {
                    window.Controls.Remove(controlToRemove);
                }

                preferredHeight = 440;


                Eval(@"
                const box = document.getElementsByName('TourPanel1')[0];
                const container = document.getElementsByName('pnlAllSettings')[0];

                if (box && container) {
                    function clampPosition() {
                        let R = box.offsetRight;
                        let L = box.offsetLeft;

                        const maxX = container.width;

                        if (L < 0) L = 0;
                        if (R > maxX) R = maxX;

                        box.style.right = R + 'px';
                        box.style.left = L + 'px';
                    }

                    // ? Run immediately when div is created
                    clampPosition();

                    // ? Watch for style changes (top/left/right/bottom)
                    const observer = new MutationObserver(clampPosition);
                    observer.observe(box, { attributes: true, attributeFilter: ['style'] });
                }
                ");
                this.VerticalScroll.Value = 0;

            }
            else if (e.StepIndex == 2)
            {
                preferredHeight = 220;
                Control controlToRemove = window.Controls["AdvancedControls"];
                if (controlToRemove != null)
                {
                    window.Controls.Remove(controlToRemove);
                }


                chkKeyboard.Enabled = true;

                this.VerticalScroll.Value = 0;

            }

            else if (e.StepIndex == 3)
            {
                preferredHeight = 330;


                Control controlToRemove = window.Controls["AdvancedControls"];
                if (controlToRemove != null)
                {
                    window.Controls.Remove(controlToRemove);
                }

                window.Controls.Add(new Button
                {

                    Size = new System.Drawing.Size(480, 50),
                    Location = new System.Drawing.Point(10, 255),
                    BackColor = System.Drawing.Color.FromArgb(0, 167, 209),
                    ForeColor = System.Drawing.Color.FromArgb(255, 255, 255),
                    Text = "Click Here To Learn About More Advanced Options",
                    Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point),
                    Cursor = Cursors.Hand,
                    Name = "AdvancedControls"
                });


                window.Controls["AdvancedControls"].Click += new System.EventHandler(this.AdvancedControls_Click);
                styleSheet1.SetCssClass(window.Controls["AdvancedControls"], "optButtons");


                window.Controls["AdvancedControls"].BringToFront();
                if (Screen.Bounds.Height >= 1100 && Screen.Bounds.Width > 1920)
                {
                    window.Controls["AdvancedControls"].Scale(1.4f);

                    ScaleFonts(window.Controls["AdvancedControls"], 1.4f);

                }


                chkKeyboard.Enabled = false;
                this.VerticalScroll.Value = 0;


            }

            else if (e.StepIndex == 4)
            {

                Control controlToRemove = window.Controls["AdvancedControls"];
                if (controlToRemove != null)
                {
                    window.Controls.Remove(controlToRemove);
                }
                preferredHeight = 590;

                window.Steps[4].Enabled = false;
                this.VerticalScroll.Value = 10000;
            }
            else if (e.StepIndex == 5)
            {
                Control controlToRemove = window.Controls["AdvancedControls"];
                if (controlToRemove != null)
                {
                    window.Controls.Remove(controlToRemove);
                }

                Control controlToRemove2 = window.Controls["ExitTour"];
                if (controlToRemove2 != null)
                {
                    window.Controls.Remove(controlToRemove2);
                }

                window.Controls.Add(new Button
                {

                    Size = new System.Drawing.Size(480, 50),
                    Location = new System.Drawing.Point(10, 205),
                    BackColor = System.Drawing.Color.FromArgb(0, 167, 209),
                    ForeColor = System.Drawing.Color.FromArgb(255, 255, 255),
                    Text = "Click Here To Learn About More Advanced Options",
                    Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point),
                    Cursor = Cursors.Hand,
                    Name = "AdvancedControls"
                });




                window.Controls["AdvancedControls"].Click += new System.EventHandler(this.AdvancedControls_Click);
                styleSheet1.SetCssClass(window.Controls["AdvancedControls"], "optButtons");


                window.Controls["AdvancedControls"].BringToFront();
                if (Screen.Bounds.Height >= 1100 && Screen.Bounds.Width > 1920)
                {
                    window.Controls["AdvancedControls"].Scale(1.4f);

                    ScaleFonts(window.Controls["AdvancedControls"], 1.4f);

                }

                preferredHeight = 270;

                window.Steps[6].Enabled = false;
                this.VerticalScroll.Value = 10000;
            }

            else if (e.StepIndex == 6)
            {
                tourEnd = false;
                preferredHeight = 135;

                Control controlToRemove = window.Controls["AdvancedControls"];
                if (controlToRemove != null)
                {
                    window.Controls.Remove(controlToRemove);
                }

                Control controlToRemove2 = window.Controls["ExitTour"];
                if (controlToRemove2 != null)
                {
                    window.Controls.Remove(controlToRemove2);
                }
                this.VerticalScroll.Value = 10000;


            }

            else if (e.StepIndex == 7)
            {
                tourEnd = true;
                preferredHeight = 330;

                Control controlToRemove = window.Controls["AdvancedControls"];
                if (controlToRemove != null)
                {
                    window.Controls.Remove(controlToRemove);
                }

                Control controlToRemove2 = window.Controls["ExitTour"];
                if (controlToRemove != null)
                {
                    window.Controls.Remove(controlToRemove2);
                }

                window.Controls.Add(new Button
                {

                    Size = new System.Drawing.Size(305, 50),
                    Location = new System.Drawing.Point(205, 340),
                    BackColor = System.Drawing.Color.FromArgb(0, 167, 209),
                    ForeColor = System.Drawing.Color.FromArgb(255, 255, 255),
                    Text = "Exit Tour and Start Using Overjoyed!",
                    Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point),
                    Cursor = Cursors.Hand,
                    Name = "ExitTour"
                });


                window.Controls["ExitTour"].Click += new System.EventHandler(this.ExitTour_Click);
                styleSheet1.SetCssClass(window.Controls["ExitTour"], "optButtons");


                window.Controls["ExitTour"].BringToFront();
                if (Screen.Bounds.Height >= 1100 && Screen.Bounds.Width > 1920)
                {
                    window.Controls["ExitTour"].Scale(1.4f);

                    ScaleFonts(window.Controls["ExitTour"], 1.4f);

                }



                disMinimize = true;
                chkKeyboard.Checked = true;
                chkXbox.Checked = false;
                disMinimize = false;

                this.VerticalScroll.Value = 10000;

            }

            if (initialTour == false && Screen.Bounds.Height >= 1100 && Screen.Bounds.Width > 1920)
            {
                float inverseScale = 1f / 1.4f;

                window.TitlePanel.Scale(inverseScale);
                window.ButtonsPanel.Scale(inverseScale);
                window.HtmlText.Scale(inverseScale);
            }

            if (Screen.Bounds.Height < 1100 && Screen.Bounds.Width <= 1920)
            {

                window.MaximumSize = new System.Drawing.Size((int)(501), (int)(preferredHeight));
                window.MinimumSize = new System.Drawing.Size((int)(501), (int)(preferredHeight));
            }



            if (Screen.Bounds.Height >= 1100 && Screen.Bounds.Width > 1920)
            {
                window.MaximumSize = new System.Drawing.Size((int)(501), (int)(preferredHeight * 1.4));
                window.MinimumSize = new System.Drawing.Size((int)(501), (int)(preferredHeight * 1.4));
                window.TitlePanel.Scale(1.4f);
                window.ButtonsPanel.Scale(1.4f);
                window.HtmlText.Scale(1.4f);



                Eval(@"
                (function retrySetTourPanel(){
                    var panel = document.getElementsByName('TourPanel1')[0];
                    if (panel) {
                        panel.style.setProperty('width', '701px', 'important');
                    } else {
                        setTimeout(retrySetTourPanel, 100); // Retry after 100ms
                    }
                })();
                ");
            }

            Eval(@"
               function isShowingOverflowEllipsis(el) {
  return el.scrollWidth > el.clientWidth || el.scrollHeight > el.clientHeight;
}

function setupDynamicEllipsisTooltip(selector) {
  document.querySelectorAll(selector).forEach(el => {
  // Apply min-height based on line-height and minLines
    const cs = getComputedStyle(el);
    const minLines = parseInt(cs.getPropertyValue('-webkit-line-clamp')) || 1;
    let lineHeight = 1.5;
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


        }

        private void AdvancedControls_Click(object sender, EventArgs e)
        {
            window.Steps[4].Enabled = true;
            window.Steps[6].Enabled = true;

            window.Next();
            window.Steps[4].Enabled = false;
            window.Steps[6].Enabled = false;
        }

        private void ExitTour_Click(object sender, EventArgs e)
        {
            window.Close();
        }



        private void BitDoLogo_Click(object sender, EventArgs e)
        {

            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://www.8bitdo.com/",
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(psi);


        }


        private void btnFeedback_Click(object sender, EventArgs e)
        {

            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://forms.gle/JBfGJtkcLbDJejmK8",
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(psi);

        }

        private void tourButton_Click(object sender, EventArgs e)
        {
            GuidedTour();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            nintendoSwitch.Checked = true;
            MicroSetup();
        }


    }
}