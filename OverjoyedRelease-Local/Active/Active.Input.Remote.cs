using ExtendedMessageBoxLibrary;
using GregsStack.InputSimulatorStandard.Native;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using SharedVars;
using SIPSorcery.Net;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Wisej.Web;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;


namespace OverjoyedReleaseLocal
{
    public partial class Active : Page
    {
        public async void checkPosition(string remotePosition)
        {
            var parsed = await Task.Run(() =>
            {
                try
                {
                    string title = remotePosition;

                    if (string.IsNullOrEmpty(title))
                        return null;

                    return title;
                }
                catch
                {
                    return null;
                }
            });

            if (parsed == null)
                return;

            string title = parsed;
            Debug.WriteLine($"Title received: {title}");

       

            int prefixSeparator = title.IndexOf('|');
            if (prefixSeparator >= 0 && prefixSeparator < title.Length - 1)
            {
                title = title.Substring(prefixSeparator + 1).Trim();
            }

            try
            {
                if (title.Contains("Overjoyed - X:"))
                {
                    int xStart = title.IndexOf("X: ") + 3;
                    int yStart = title.IndexOf("Y: ") + 3;
                    int actionStart = title.IndexOf("Action: ") + 8;

                    remoteX = (int)((int.Parse(title.Substring(xStart, title.IndexOf(",", xStart) - xStart).Trim())));
                    remoteY = (int)((int.Parse(title.Substring(yStart, title.IndexOf(",", yStart) - yStart).Trim())));
                    remoteXoverlay = (int)((remoteX - 20) * zoomFactor);
                    remoteYoverlay = (int)((remoteY - 20) * zoomFactor);
                    action = NormalizeRemoteActionToken(title.Substring(actionStart).Trim());

                    //Eval(@"document.getElementById('socketSquare').style.left = '" + remoteX + @"px';
                    //document.getElementById('socketSquare').style.top = '" + remoteY + @"px';");

                    SyncOverlayRemotePointerState();

                    if (positionCheck)
                    {
                        ProcessRemoteTitlePositionUpdate(action);
                    }
                }
                else if (title.Contains("activate"))
                {
                    if (chkRemote.Checked)
                    {
                        ActivateFromDeadZone(true);
                    }
                }
                else if (title.Contains("exit"))
                {
                    if (chkRemote.Checked)
                    {
                        DeactivateFromExitHold();
                    }
                }
                else if (title.Contains("qcUp"))
                {
                    if (chkRemote.Checked)
                    {
                        btnUp.PerformClick();
                    }
                }
                else if (title.Contains("qcDown"))
                {
                    if (chkRemote.Checked)
                    {
                        btnDown.PerformClick();
                    }
                }
                else if (title.Contains("qcLeft"))
                {
                    if (chkRemote.Checked)
                    {
                        btnLeft.PerformClick();
                    }
                }
                else if (title.Contains("qcRight"))
                {
                    if (chkRemote.Checked)
                    {
                        btnRight.PerformClick();
                    }
                }
                else if (title.Contains("qcAccept"))
                {
                    if (chkRemote.Checked)
                    {
                        btnAccept.PerformClick();
                    }
                }
                else if (title.Contains("qcBack"))
                {
                    if (chkRemote.Checked)
                    {
                        btnBack.PerformClick();
                    }
                }
                else if (title.Contains("qcStart"))
                {
                    if (chkRemote.Checked)
                    {
                        btnSwitchStart.PerformClick();
                    }
                }
                else if (title.Contains("qcSelect"))
                {
                    if (chkRemote.Checked)
                    {
                        btnSwitchSelect.PerformClick();
                    }
                }
                else if (title.Contains("qcHome"))
                {
                    if (chkRemote.Checked)
                    {
                        btnSwitchHome.PerformClick();
                    }
                }
            }
            catch (JsonException)
            {
                // ignore malformed payloads
            }
        }

        private void ProcessRemoteTitlePositionUpdate(string remoteAction)
        {
            if (!positionCheck)
                return;

            bool positionChanged = remoteX != prevRemoteX || remoteY != prevRemoteY;
            bool shouldQueueControllerWork = positionChanged;
            if (TryParseRemotePointerAction(remoteAction, out long? sequence, out bool? leftDown, out bool? rightDown))
            {
                if (sequence.HasValue)
                {
                    if (sequence.Value <= lastReceivedPointerPacketSequence)
                        return;

                    lastReceivedPointerPacketSequence = sequence.Value;
                }

                if (leftDown.HasValue)
                {
                    shouldQueueControllerWork |= remoteLeftPointerDown != leftDown.Value;
                    ApplyRemotePointerState("left", leftDown.Value);
                }

                if (rightDown.HasValue)
                {
                    shouldQueueControllerWork |= remoteRightPointerDown != rightDown.Value;
                    ApplyRemotePointerState("right", rightDown.Value);
                }

                shouldQueueControllerWork |= remoteLeftPointerDown || remoteRightPointerDown;
            }

            long now = Environment.TickCount64;
            bool actionChanged = !string.Equals(action, prevAction, StringComparison.Ordinal);
            if (actionChanged || now - lastRemoteStatusLabelUpdateTick >= 100)
            {
                //SetLblActiveTextUi($"Found window with coordinates - X: {remoteX}, Y: {remoteY}, Action: {action}");
                lastRemoteStatusLabelUpdateTick = now;
            }

            prevRemoteX = remoteX;
            prevRemoteY = remoteY;
            prevAction = action;

            if (shouldQueueControllerWork)
            {
                QueueMouseMoveActions();
            }
        }

        private void ApplyRemotePointerState(string mouseButton, bool isDown)
        {
            if (mouseButton == "left")
            {
                if (remoteLeftPointerDown == isDown)
                    return;

                if (isDown)
                {
                    HandleRemoteMouseDownAction();
                }
                else
                {
                    mouseUp("left");
                }

                remoteLeftPointerDown = isDown;
                return;
            }

            if (mouseButton == "right")
            {
                if (remoteRightPointerDown == isDown)
                    return;

                if (isDown)
                {
                    mouseDown("right");
                }
                else
                {
                    mouseUp("right");
                }

                remoteRightPointerDown = isDown;
            }
        }

        private void QueueMouseMoveActions()
        {
            bool lookMode = rightLookModeLatched && (localRightPointerDown || remoteRightPointerDown);

            lock (mouseMoveQueueSync)
            {
                queuedMouseMoveFps = chkFPS.Checked;
                queuedMouseMoveLookMode = lookMode;
                mouseMoveQueued = true;

                if (mouseMoveWorkerRunning)
                {
                    return;
                }

                mouseMoveWorkerRunning = true;
            }

            _ = Task.Run(ProcessQueuedMouseMoveActionsAsync);
        }

        private async Task ProcessQueuedMouseMoveActionsAsync()
        {
            while (true)
            {
                bool fpsMode;
                bool lookMode;

                lock (mouseMoveQueueSync)
                {
                    if (!mouseMoveQueued)
                    {
                        mouseMoveWorkerRunning = false;
                        return;
                    }

                    mouseMoveQueued = false;
                    fpsMode = queuedMouseMoveFps;
                    lookMode = queuedMouseMoveLookMode;
                }

                try
                {
                    await MouseMoveActions(fpsMode, lookMode);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[MouseMoveDebug] ProcessQueuedMouseMoveActionsAsync failed: {ex.Message}");
                }
            }
        }

        private static string NormalizeRemoteActionToken(string? rawAction)
        {
            var trimmed = (rawAction ?? "none").Trim();
            if (string.IsNullOrEmpty(trimmed))
                return "none";

            var parts = trimmed.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length == 2 && int.TryParse(parts[0], out _) && int.TryParse(parts[1], out _))
            {
                return $"{parts[0]},{parts[1]}";
            }

            return trimmed.ToLowerInvariant();
        }

        private static bool TryParseRemotePointerAction(string remoteAction, out long? sequence, out bool? leftDown, out bool? rightDown)
        {
            sequence = null;
            leftDown = null;
            rightDown = null;

            var trimmed = (remoteAction ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(trimmed))
                return false;

            var countParts = trimmed.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (countParts.Length == 2
                && int.TryParse(countParts[0], out int downCount)
                && int.TryParse(countParts[1], out int upCount))
            {
                leftDown = downCount > upCount;
                rightDown = false;
                return true;
            }

            switch (trimmed)
            {
                case "down":
                    leftDown = true;
                    return true;
                case "up":
                    leftDown = false;
                    rightDown = false;
                    return true;
                case "leftdown":
                    leftDown = true;
                    return true;
                case "leftup":
                    leftDown = false;
                    return true;
                case "rightdown":
                    rightDown = true;
                    return true;
                case "rightup":
                    rightDown = false;
                    return true;
            }

            var tokens = trimmed.Split(new[] { ' ', ';', ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var token in tokens)
            {
                if (token.StartsWith("seq:", StringComparison.OrdinalIgnoreCase))
                {
                    if (long.TryParse(token.Substring(4), out long parsedSequence))
                    {
                        sequence = parsedSequence;
                    }

                    continue;
                }
                if (token.StartsWith("left:", StringComparison.OrdinalIgnoreCase))
                {
                    string leftValue = token.Substring(5);

                    if (leftValue.Equals("down", StringComparison.OrdinalIgnoreCase))
                    {
                        leftDown = true;
                    }
                    else if (leftValue.Equals("up", StringComparison.OrdinalIgnoreCase))
                    {
                        leftDown = false;
                    }
                }

                if (token.StartsWith("right:", StringComparison.OrdinalIgnoreCase))
                {
                    string rightValue = token.Substring(6);

                    if (rightValue.Equals("down", StringComparison.OrdinalIgnoreCase))
                    {
                        rightDown = true;
                    }
                    else if (rightValue.Equals("up", StringComparison.OrdinalIgnoreCase))
                    {
                        rightDown = false;
                    }
                }
            }

            return sequence.HasValue || leftDown.HasValue || rightDown.HasValue;
        }





        private void HandleRemoteMouseDownAction()
        {
            xEnd = remoteX;
            yEnd = remoteY;

            float _magnitudeX = Math.Abs(xStart - xEnd);
            float _magnitudeY = Math.Abs(yStart - yEnd);
            double distance = Math.Sqrt(Math.Pow(_magnitudeX, 2) + Math.Pow(_magnitudeY, 2));

            if ((distance <= deadZone) && isActive)
            {
                if ((distance > deadZone / 2 && ((yStart - yEnd) > 0)))
                {
                    c1 = 25; c2 = 25; c3 = 25;

                    if (ActivateUpper && !positionCheck && !chkRemote.Checked)
                    {
                        DeactivateFromExitHold();
                    }
                }
                else if ((distance <= deadZone / 2))
                {
                    c1 = 28; c2 = 28; c3 = 28;

                    if (ActivateMiddle && !positionCheck && !chkRemote.Checked)
                    {
                        DeactivateFromExitHold();
                    }
                }
                else if (((yStart - yEnd) < 0) && (distance > deadZone / 2))
                {
                    c1 = 31; c2 = 31; c3 = 31;

                    if (ActivateLower && !positionCheck && !chkRemote.Checked)
                    {
                        DeactivateFromExitHold();
                    }
                }
            }
            else if (!isActive)
            {
                if (positionCheck || chkRemote.Checked)
                {
                    return;
                }

                ActivateFromDeadZone(false);
            }

            mouseDown("left");
        }

        private bool TryHandleRemoteMouseButtonAction(string mouseButton, bool isDown, bool treatAsHotkey = true)
        {
            if (!positionCheck && !chkDisableClicks.Checked)
            {
                return false;
            }

            int endX = positionCheck ? (int)remoteX : mousePosX;
            int endY = positionCheck ? (int)remoteY : mousePosY;

            if (!TryResolveMouseButtonPromptAction(mouseButton, endX, endY, out int resolvedC1, out int resolvedC2, out int resolvedC3))
            {
                return true;
            }

            c1 = resolvedC1;
            c2 = resolvedC2;
            c3 = resolvedC3;

            if (buttonConfig[c1][2] == "false")
            {
                if (treatAsHotkey)
                {
                    HotkeyClickLoop(c1, c2, c3, isDown);
                }
                else
                {
                    if (isDown)
                    {
                        ClickLoop(c1, c2, c3, true);
                    }
                    else
                    {
                        ReleaseRemoteRegularMouseButtonAction(mouseButton, c1, c2, c3);
                    }
                }
            }

            if (!isDown && buttonConfig[c1][4] == "true")
            {
                ReturnToCenter();
            }

            return true;
        }

        // To stop listening
        public void StopListening()
        {
            _cancellationTokenSource?.Cancel();
        }

        public async void StartListening()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            // Start listening for messages
            try
            {
                await ActiveMessenger.StartListeningAsyncActive(HandleMessage, _cancellationTokenSource.Token);
            }
            catch (TimeoutException tex)
            {
                Debug.WriteLine($"[StartListeningActive] Timeout: {tex}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[StartListeningActive] Error: {ex}");
            }
        }


        private void ReleaseRemoteRegularMouseButtonAction(string mouseButton, int c1, int c2, int c3)
        {
            ClickThreadMode mode = GetCurrentClickThreadMode();
            OutputTarget target = CreateClickTarget(c1, c2, c3, 0);

            if (mode == ClickThreadMode.Switch)
            {
                Output.Switch.Release(target, OutputReleaseMode.Click).GetAwaiter().GetResult();
            }
            else if (mode == ClickThreadMode.Xbox)
            {
                Output.Xbox.Release(target, OutputReleaseMode.Click).GetAwaiter().GetResult();
            }
            else
            {
                Output.Keyboard.Release(target, OutputReleaseMode.Click).GetAwaiter().GetResult();
            }

            clickButtons.Text = "Click: Nothing is being pressed.";
        }

        private void chkSteam_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSteam.Checked)
            {
                if (chkSteam.Text == "Watch Steam Tutorial")
                {
                    System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "https://www.youtube.com/watch?v=bAVdCvSKFRs",
                        UseShellExecute = true
                    };
                    System.Diagnostics.Process.Start(psi);
                }
                else
                {
                    ShowCombineDiagramOverlay();

                    closeDiagram.Visible = true;
                    closeDiagram.BringToFront();
                    hideBG.Visible = false;
                    chkSteam.Visible = false;
                    Eval(@"setTimeout(() => {
    document.querySelectorAll('svg text[data-id]').forEach(svgText => {
      const id = svgText.getAttribute('data-id');
      const htmlText = document.querySelector('#translations [data-id=""'+id+'""]');
      if(htmlText) {
        // Remove existing text nodes that are not <tspan>
        svgText.childNodes.forEach(node => {
          if(node.nodeType === Node.TEXT_NODE) node.remove();
        });
        // Insert translated text before first <tspan>
        const firstTspan = svgText.querySelector('tspan');
        if(firstTspan) {
          svgText.insertBefore(document.createTextNode(htmlText.textContent), firstTspan);
        } else {
          svgText.textContent = htmlText.textContent;
        }

        // Adjust background rect width to fit text
        const parentGroup = svgText.parentNode;
        if(parentGroup && parentGroup.querySelector('rect')) {
          const bbox = svgText.getBBox(); // get rendered size of text
          const paddingX = 10; // horizontal padding
          const paddingY = 6;  // vertical padding
          const rect = parentGroup.querySelector('rect');
          rect.setAttribute('width', bbox.width + paddingX*2);
          rect.setAttribute('height', bbox.height + paddingY*2);
          rect.setAttribute('x', bbox.x - paddingX);
          rect.setAttribute('y', bbox.y - paddingY);
        }
      }
    });
  }, 1000); // wait 5 seconds
");
                }
            }
            chkSteam.Checked = false;

        }

        public async Task remotePlay(string firstLine)
        {
            if (!string.IsNullOrWhiteSpace(firstLine))
            {
                bool remotePlayEnabled = remoteActive;

                if (!remoteActive)
                {

                    resetTransparency(false, true);

                    ExtendedDialogResult result = ExtendedMessageBox.Show($"<p style='margin:0;padding:0;line-height:1.6;'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 1; -webkit-box-orient: vertical; overflow: hidden; font-size:" + truncateSize + $"pt; ' title='Do you want to enable Remote Play features?'>Do you want to enable Remote Play features?</span></p>", "Enable Remote Play?", ExtendedMessageBoxLibrary.MessageBoxButtons.YesNo, lines: 1, zoomFactor: zoomFactor);

                    resetTransparency(true, true);

                    if (result.Result == DialogResult.Yes)
                    {
                        await BasicRemoteClient.Initialize(this);
                        BasicRemoteClient.StartWebRTC();
                        remotePlayEnabled = true;
                    }

                }

                if (!remotePlayEnabled)
                {
                    return;
                }

                try
                {
                    using (FileStream fs = new FileStream(Path.Combine(configPath, "RemoteActive.txt"), FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        writer.WriteLine("true");
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
                if (!remoteActive)
                {
                    await BasicRemoteClient.Initialize(this);
                    BasicRemoteClient.StartWebRTC();
                }

                try
                {
                    using (FileStream fs = new FileStream(Path.Combine(configPath, "RemoteActive.txt"), FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        writer.WriteLine("true");
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

        private void SetRemotePlayPersistedState(bool isEnabled)
        {
            remoteActive = isEnabled;

            try
            {
                using (FileStream fs = new FileStream(Path.Combine(configPath, "RemoteActive.txt"), FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.WriteLine(isEnabled ? "true" : string.Empty);
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

        private void RestoreRemoteToggleButtonText()
        {
            if (xboxMode)
            {
                btnSwitchHome.Text = psLabels ? "PS Button" : "Xbox Button";
            }
            else if (keyboardMode)
            {
                btnSwitchHome.Text = "Alt + Tab";
            }
            else if (switchMode)
            {
                btnSwitchHome.Text = "Home Button";
            }
        }

        public void HandleRemotePlayConnected()
        {
            SetRemotePlayPersistedState(true);

            if (chkRemote.Checked)
            {
                if (!isActive)
                {
                    ActivateFromDeadZone(true);
                }

                SetLblActiveTextUi("Overjoyed Remote Play connected and activated.");
            }
        }

        public void HandleRemotePlayConnectionFailure(string? details = null)
        {
            BasicRemoteClient.Stop();
            SetRemotePlayPersistedState(false);

            suppressRemoteCheckedChanged = true;
            try
            {
                chkRemote.Checked = false;
            }
            finally
            {
                suppressRemoteCheckedChanged = false;
            }

            remotePositionCheck = false;
            drawPrompts();
            RefreshOverlayPointerState();
            RestoreRemoteToggleButtonText();

            resetTransparency(false, true);
            string failureText = string.IsNullOrWhiteSpace(details)
                ? "Remote Play failed to connect. Please try again."
                : $"Remote Play failed to connect. {details}";
            ExtendedMessageBox.Show($"<p style='margin:0;padding:0;line-height:1.6;'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; font-size:" + truncateSize + $"pt; ' title='" + failureText + "'>" + failureText + "</span></p>", "Remote Play Connection Failed", ExtendedMessageBoxLibrary.MessageBoxButtons.OK, lines: 2, zoomFactor: zoomFactor);
            resetTransparency(true, true);

            SetLblActiveInactiveUi("Overjoyed Remote Play failed to connect.");
        }

        public void HandleRemotePlayDisconnected(string? details = null)
        {
            BasicRemoteClient.Stop();
            SetRemotePlayPersistedState(false);

            suppressRemoteCheckedChanged = true;
            try
            {
                chkRemote.Checked = false;
            }
            finally
            {
                suppressRemoteCheckedChanged = false;
            }

            remotePositionCheck = false;
            drawPrompts();
            RefreshOverlayPointerState();
            RestoreRemoteToggleButtonText();

            if (isActive)
            {
                DeactivateFromExitHold();
            }

            string statusText = string.IsNullOrWhiteSpace(details)
                ? "Overjoyed Remote Play disconnected."
                : $"Overjoyed Remote Play disconnected. {details}";

            resetTransparency(false, true);
            ExtendedMessageBox.Show($"<p style='margin:0;padding:0;line-height:1.6;'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 1; -webkit-box-orient: vertical; overflow: hidden; font-size:" + truncateSize + $"pt; ' title='" + statusText + "'>" + statusText + "</span></p>", "Remote Play Disconnected", ExtendedMessageBoxLibrary.MessageBoxButtons.OK, lines: 1, zoomFactor: zoomFactor);
            resetTransparency(true, true);

            SetLblActiveInactiveUi(statusText);
        }

        private void chkRemote_CheckedChanged(object sender, EventArgs e)
        {
            if (suppressRemoteCheckedChanged)
            {
                return;
            }

            var streamerModePath = Path.Combine(configPath, "StreamerMode.txt");
            string? firstLine = null;
            bool showRemotePlayFallback = false;

            if (System.IO.File.Exists(streamerModePath))
            {
                using var fs = new FileStream(streamerModePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = new StreamReader(fs);
                firstLine = reader.ReadLine();
            }

            bool webfuseActivated = !string.IsNullOrWhiteSpace(firstLine);
            bool useWebfuseMode = false;

            if (chkRemote.Checked)
            {
                chkKeepMouseInside.Checked = false;
                if (webfuseActivated)
                {

                    if (!remoteActive)
                    {

                        SetLblActiveInactiveUi(GetEffectiveNintendoStreamerPlayerStatusText(false));


                        resetTransparency(false, true);

                        ExtendedDialogResult result = ExtendedMessageBox.Show($"<p style='margin:0;padding:0;line-height:1.6;'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 1; -webkit-box-orient: vertical; overflow: hidden; font-size:" + truncateSize + $"pt; ' title='Do you want to enable Webfuse features?'>Do you want to enable Webfuse features?</span></p>", "Enable Webfuse?", ExtendedMessageBoxLibrary.MessageBoxButtons.YesNo, lines: 1, zoomFactor: zoomFactor);

                        resetTransparency(true, true);

                        if (result.Result == DialogResult.Yes)
                        {
                            btnSwitchHome.Text = "Webfuse OFF";
                            useWebfuseMode = true;
                            ShowWebfusePanel();

                        }
                        else
                        {
                            showRemotePlayFallback = true;
                            HideWebfusePanel();
                        }
                    }
                    else
                    {
                        useWebfuseMode = true;
                        ShowWebfusePanel();
                    }
                }

                positionCheck = true;
                remotePositionCheck = !useWebfuseMode;

                if (!useWebfuseMode)
                {
                    HideWebfusePanel();

                    if (xboxMode)
                    {
                        btnSwitchHome.Text = "Xbox Button";
                        if (psLabels)
                        {
                            btnSwitchHome.Text = "PS Button";
                        }
                    }
                    else if (keyboardMode)
                    {
                        btnSwitchHome.Text = "Alt + Tab";
                    }
                    else if (switchMode)
                    {
                        btnSwitchHome.Text = "Home Button";
                    }
                }

                if (remoteActive)
                {
                    SetLblActiveTextUi(GetEffectiveNintendoStreamerPlayerStatusText(false));
                }

                drawPrompts();
                RefreshOverlayPointerState();
            }
            else
            {

                if (xboxMode)
                {
                    btnSwitchHome.Text = "Xbox Button";
                }
                else if (switchMode)
                {
                    btnSwitchHome.Text = "Switch Home";
                }
                    else if (keyboardMode)
                    {
                        btnSwitchHome.Text = "Alt + Tab";
                }

                if (webfuseActivated)
                {
                    if (!remoteActive)
                    {


                        resetTransparency(false, true);

                        ExtendedDialogResult result = ExtendedMessageBox.Show($" <p style='margin:0;padding:0;line-height:1.6;'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 1; -webkit-box-orient: vertical; overflow: hidden; font-size:" + truncateSize + $"pt; ' title='Do you want to disable Webfuse features?'>Do you want to disable Webfuse features?</span></p>", "Disable Webfuse?", ExtendedMessageBoxLibrary.MessageBoxButtons.YesNo, lines: 1, zoomFactor: zoomFactor);

                        resetTransparency(true, true);

                        if (result.Result == DialogResult.Yes)
                        {
                            HideWebfusePanel();
                        }
                    }
                }

                HideWebfusePanel();

                positionCheck = false;
                remotePositionCheck = false;
                drawPrompts();
                RefreshOverlayPointerState();

                if (xboxMode)
                {
                    btnSwitchHome.Text = "Xbox Button";
                    if (psLabels)
                    {
                        btnSwitchHome.Text = "PS Button";
                    }
                }
                else if (keyboardMode)
                {
                    btnSwitchHome.Text = "Alt + Tab";
                }
                else if (switchMode)
                {
                    btnSwitchHome.Text = "Home Button";
                }


            }

            if (playerNum == 1)
            {
                if (chkRemote.Checked)
                {
                    if (!webfuseActivated || showRemotePlayFallback)
                    {
                        remotePlay(firstLine);
                    }

                }
                else
                {
                    if (!webfuseActivated)
                    {


                        resetTransparency(false, true);

                        ExtendedDialogResult result = ExtendedMessageBox.Show($"<p style='margin:0;padding:0;line-height:1.6;'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 1; -webkit-box-orient: vertical; overflow: hidden; font-size:" + truncateSize + $"pt; ' title='Do you want to disable Remote Play features?'>Do you want to disable Remote Play features?</span></p>", "Disable Remote Play?", ExtendedMessageBoxLibrary.MessageBoxButtons.YesNo, lines: 1, zoomFactor: zoomFactor);

                        resetTransparency(true, true);

                        if (result.Result == DialogResult.Yes)
                        {
                            BasicRemoteClient.Stop();
                        }
                        try
                        {
                            using (FileStream fs = new FileStream(Path.Combine(configPath, "RemoteActive.txt"), FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                            using (StreamWriter writer = new StreamWriter(fs))
                            {
                                writer.WriteLine("");
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
                        BasicRemoteClient.Stop();
                        try
                        {
                            using (FileStream fs = new FileStream(Path.Combine(configPath, "RemoteActive.txt"), FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                            using (StreamWriter writer = new StreamWriter(fs))
                            {
                                writer.WriteLine("");
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

            SetMenuBarVisibleForActiveState(isActive);

        }

    }

    internal static class NetworkEndpointHelper
    {
        public static bool TryGetReachableTailscaleIPv4(out IPAddress? address)
        {
            address = null;

            try
            {
                if (!IsTailscaleBackendRunning())
                {
                    return false;
                }

                var adapters = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(nic => nic.OperationalStatus == OperationalStatus.Up)
                    .Where(nic =>
                        (!string.IsNullOrWhiteSpace(nic.Name) && nic.Name.IndexOf("tailscale", StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (!string.IsNullOrWhiteSpace(nic.Description) && nic.Description.IndexOf("tailscale", StringComparison.OrdinalIgnoreCase) >= 0));

                foreach (var adapter in adapters)
                {
                    var ipProps = adapter.GetIPProperties();
                    foreach (var uni in ipProps.UnicastAddresses)
                    {
                        if (uni?.Address == null)
                        {
                            continue;
                        }

                        if (uni.Address.AddressFamily != AddressFamily.InterNetwork)
                        {
                            continue;
                        }

                        if (IPAddress.IsLoopback(uni.Address))
                        {
                            continue;
                        }

                        address = uni.Address;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to resolve Tailscale adapter: {ex.Message}");
            }

            return false;
        }

        private static bool IsTailscaleBackendRunning()
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "tailscale",
                    Arguments = "status --json",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(startInfo);
                if (process == null)
                {
                    return false;
                }

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                if (!process.WaitForExit(2000) || process.ExitCode != 0 || string.IsNullOrWhiteSpace(output))
                {
                    Debug.WriteLine($"Tailscale status check failed: exit={process.ExitCode}, error={error.Trim()}");
                    return false;
                }

                using var document = JsonDocument.Parse(output);
                if (document.RootElement.TryGetProperty("BackendState", out var backendState) &&
                    string.Equals(backendState.GetString(), "Running", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to query Tailscale status: {ex.Message}");
            }

            return false;
        }
    }

    public static class BasicRemoteClient
    {
        private static Active? _form;
        private static CancellationTokenSource? _cts;
        private static volatile bool _isRunning;
        private static readonly object _sync = new object();
        private static bool _connectionEstablished;
        private static bool _connectionFailureHandled;
        private static bool _suppressDisconnectNotification;
        private static int _connectionAttemptId;
        private static long _lastHeartbeatTick;
        private const int HeartbeatTimeoutMs = 3000;

        private static RTCPeerConnection? _peer;
        private static RTCDataChannel? _channel;

        private static ConnectionMultiplexer? _redis;
        private static ISubscriber? _subscriber;

        private static string? _pairCode;
        private static bool _allowAllCandidates = false;

        private static string UpstashUrl = "YOUR_UPSTASH_URL";
        private static string UpstashToken = "YOUR_UPSTASH_TOKEN";

        // 🔥 FIX: buffering
        private static List<string> _pendingCandidates = new();
        private static bool _remoteDescriptionSet = false;
        private static readonly RTCDataChannelInit _remoteDataChannelInit = new RTCDataChannelInit
        {
            ordered = false,
            maxRetransmits = 0
        };
        private static readonly object _remoteMessageSync = new object();
        private static string _pendingRemotePositionMessage = string.Empty;
        private static bool _remoteMessagePumpQueued;

        public static bool IsRunning => _isRunning;

        public static void AttachForm(Active form)
        {
            lock (_sync)
            {
                _form = form;
            }
        }

        public static void DetachForm(Active form)
        {
            lock (_sync)
            {
                if (ReferenceEquals(_form, form))
                {
                    _form = null;
                }
            }
        }

        public static async Task Initialize(Active form)
        {
            _form = form;

            var options = ConfigurationOptions.Parse(UpstashUrl);
            options.Password = UpstashToken;
            options.Ssl = true;

            _redis = ConnectionMultiplexer.Connect(options);
            _subscriber = _redis.GetSubscriber();
        }



        public static string GeneratePairingCode()
        {
            var rng = new Random();
            _pairCode = rng.Next(100000, 999999).ToString();
            Debug.WriteLine("Pairing code: " + _pairCode);
            return _pairCode;
        }

        public async static void StartWebRTC()
        {
            int connectionAttemptId;

            lock (_sync)
            {
                if (_isRunning) return;
                _cts?.Dispose();
                _cts = new CancellationTokenSource();
                _isRunning = true;
                _connectionEstablished = false;
                _connectionFailureHandled = false;
                _suppressDisconnectNotification = false;
                _lastHeartbeatTick = 0;
                connectionAttemptId = ++_connectionAttemptId;
            }

            try
            {
                // 🔥 reset state
                _pendingCandidates.Clear();
                _remoteDescriptionSet = false;

                LoadStreamerMode();

                if (_pairCode == null) GeneratePairingCode();

                SetupPeer();

                _form.resetTransparency(false, true);

                ExtendedMessageBox.Show($"<p style='margin:0;padding:0;line-height:1.6;'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 3; -webkit-box-orient: vertical; overflow: hidden; font-size:" + _form.truncateSize + $"pt; ' title='Your pairing code is: {_pairCode}. Please visit Overjoyed.Vercel.App on your other device, enter the code and tap Connect, then immediately press OK on this message box to connect!'>Your pairing code is: <strong>{_pairCode}</strong>. Please visit <span style='color:#0066cc;'><u>Overjoyed.Vercel.App</u></span> on your other device, enter the code and tap Connect, then immediately press OK on this message box to connect!</span></p>", "Remote Pairing Code - Keep This Popup Open", lines: 3, zoomFactor: _form.zoomFactor);

                _form.resetTransparency(true, true);

                SubscribeToChannel(_pairCode);

                _ = WatchForConnectionTimeoutAsync(connectionAttemptId, _cts!.Token);

                // recovery (offer might already exist)
                _ = Task.Run(async () =>
                {
                    var offer = await TryGetStoredOffer();
                    if (offer != null)
                    {
                        Debug.WriteLine("[Recovery] Using stored offer");
                        await HandleOffer(offer);
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to start Remote Play: " + ex);
                NotifyConnectionFailure("Please verify the other device is ready and try again.");
            }
        }

        private static async Task WatchForConnectionTimeoutAsync(int connectionAttemptId, CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(20), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            lock (_sync)
            {
                if (!_isRunning || _connectionEstablished || _connectionAttemptId != connectionAttemptId)
                {
                    return;
                }
            }

            NotifyConnectionFailure("The remote device did not complete the connection in time.");
        }

        private static void NotifyConnectionEstablished()
        {
            Active? form;

            lock (_sync)
            {
                if (_connectionEstablished)
                {
                    return;
                }

                _connectionEstablished = true;
                _connectionFailureHandled = false;
                _lastHeartbeatTick = Environment.TickCount;
                form = _form;
            }

            if (form == null)
            {
                return;
            }

            try
            {
                form.Invoke((Action)(() => form.HandleRemotePlayConnected()));
                _ = Task.Run(() => MonitorHeartbeatAsync(form, _cts?.Token ?? CancellationToken.None));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Remote Play connected UI update failed: " + ex.Message);
            }
        }

        private static void NotifyConnectionFailure(string details)
        {
            Active? form;

            lock (_sync)
            {
                if (_connectionEstablished || _connectionFailureHandled)
                {
                    return;
                }

                _connectionFailureHandled = true;
                form = _form;
            }

            if (form == null)
            {
                Stop();
                return;
            }

            try
            {
                form.Invoke((Action)(() => form.HandleRemotePlayConnectionFailure(details)));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Remote Play failure UI update failed: " + ex.Message);
                Stop();
            }
        }

        private static void NotifyConnectionLost(string details)
        {
            Active? form;

            lock (_sync)
            {
                if (!_connectionEstablished || _connectionFailureHandled || _suppressDisconnectNotification)
                {
                    return;
                }

                _connectionFailureHandled = true;
                form = _form;
            }

            if (form == null)
            {
                Stop();
                return;
            }

            try
            {
                form.Invoke((Action)(() => form.HandleRemotePlayDisconnected(details)));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Remote Play disconnect UI update failed: " + ex.Message);
                Stop();
            }
        }

        private static async Task<string?> TryGetStoredOffer()
        {
            if (_redis == null || _pairCode == null) return null;

            try
            {
                var db = _redis.GetDatabase();
                var value = await db.StringGetAsync($"room:{_pairCode}:offer");
                if (!value.IsNullOrEmpty)
                {
                    // Parse JSON offer format: { "type":"offer", "sdp":"..." }
                    try
                    {
                        using var doc = JsonDocument.Parse(value.ToString());
                        if (doc.RootElement.TryGetProperty("sdp", out var sdpProp))
                            return sdpProp.GetString();
                    }
                    catch
                    {
                        Debug.WriteLine("[Recovery] Offer not JSON, using raw SDP");
                        return value; // fallback for legacy
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("TryGetStoredOffer failed: " + ex);
            }

            return null;
        }

        private static void SubscribeToChannel(string channel)
        {
            if (_subscriber == null) return;

            // Listen for structured offer/answer messages
            _subscriber.Subscribe(channel, async (redisChannel, value) =>
            {
                var msg = value.ToString();
                if (string.IsNullOrEmpty(msg)) return;

                try
                {
                    using var doc = JsonDocument.Parse(msg);
                    if (doc.RootElement.TryGetProperty("type", out var typeProp))
                    {
                        var type = typeProp.GetString();
                        if (type == "offer" && doc.RootElement.TryGetProperty("sdp", out var sdpProp))
                        {
                            var sdp = sdpProp.GetString();
                            if (!string.IsNullOrEmpty(sdp))
                                await HandleOffer(sdp);
                        }
                    }
                }
                catch
                {
                    // fallback: treat as raw candidate
                    if (!_remoteDescriptionSet)
                    {
                        _pendingCandidates.Add(msg);
                        return;
                    }
                    AddCandidateSafe(msg);
                }
            });

            // Subscribe to candidate channel
            _subscriber.Subscribe($"room:{channel}:candidates", (redisChannel, value) =>
            {
                var candStr = value.ToString()?.Trim('"');
                if (string.IsNullOrEmpty(candStr)) return;

                Debug.WriteLine($"[RECEIVED CANDIDATE] {candStr}");

                if (!_remoteDescriptionSet)
                {
                    _pendingCandidates.Add(candStr);
                    Debug.WriteLine($"[Buffered] {candStr}");
                    return;
                }

                AddCandidateSafe(candStr);
            });
        }

        private static void LoadStreamerMode()
        {
            string configPath = Path.Combine(FileSystem.Current.AppDataDirectory, "configs");
            var streamerModePath = Path.Combine(configPath, "StreamerMode.txt");

            string? firstLine = System.IO.File.Exists(streamerModePath)
                ? System.IO.File.ReadLines(streamerModePath).FirstOrDefault()
                : null;

            bool remoteRequested = (firstLine?.IndexOf("remote", StringComparison.OrdinalIgnoreCase) ?? -1) >= 0;

            _allowAllCandidates = remoteRequested;
        }

        private static void SetupPeer()
        {
            _peer = new RTCPeerConnection(new RTCConfiguration
            {
                iceServers = new List<RTCIceServer>
            {
        new RTCIceServer
        {
            urls = "stun:stun.l.google.com:19302"
        },
        new RTCIceServer
       {
        urls = "turn:YOUR_TURN_SERVER_URL",
        username = "YOUR_TURN_USERNAME",
        credential = "YOUR_TURN_CREDENTIAL"
      }

    }
            });


            _peer.onicecandidate += async candidate =>
            {
                if (candidate == null) return;


                await _subscriber!.PublishAsync($"room:{_pairCode}:candidates", candidate.candidate);
                Debug.WriteLine("Sent candidate: " + candidate.candidate);
            };

            _peer.ondatachannel += dc => AttachRemoteDataChannel(dc);
        }

        private static void AttachRemoteDataChannel(RTCDataChannel channel)
        {
            _channel = channel;

            _channel.onopen += () =>
            {
                Debug.WriteLine($"DataChannel OPEN ({_channel.label}) ordered={_channel.ordered} maxRetransmits={_channel.maxRetransmits}");
                NotifyConnectionEstablished();
            };

            _channel.onclose += () =>
            {
                Debug.WriteLine($"DataChannel CLOSED ({_channel.label})");
                NotifyConnectionLost("The remote device disconnected.");
            };

            _channel.onmessage += (dataChannel, protocol, data) =>
            {
                var form = _form;
                if (data == null || data.Length == 0 || form == null)
                {
                    return;
                }

                var message = Encoding.UTF8.GetString(data);
                MarkHeartbeatReceived();

                if (!message.Contains("|"))
                {
                    return;
                }

                int separatorIndex = message.IndexOf('|');
                if (separatorIndex <= 0 || separatorIndex >= message.Length - 1)
                {
                    return;
                }

                string prefix = message.Substring(0, separatorIndex);
                var playerPrefix = form.playerNum.ToString();
                if (!prefix.Equals(playerPrefix, StringComparison.Ordinal))
                {
                    return;
                }

                string payload = message.Substring(separatorIndex + 1).Trim();
                if (payload.Equals("heartbeat", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                QueueLatestRemotePositionMessage(message);
            };
        }

        private static void MarkHeartbeatReceived()
        {
            Interlocked.Exchange(ref _lastHeartbeatTick, Environment.TickCount);
        }

        private static async Task MonitorHeartbeatAsync(Active form, CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, cancellationToken);

                    lock (_sync)
                    {
                        if (!_isRunning || !_connectionEstablished || _connectionFailureHandled)
                        {
                            return;
                        }
                    }

                    int lastHeartbeatTick = Interlocked.Read(ref _lastHeartbeatTick) switch
                    {
                        long value => unchecked((int)value)
                    };

                    if (lastHeartbeatTick == 0)
                    {
                        continue;
                    }

                    int elapsedMs = unchecked(Environment.TickCount - lastHeartbeatTick);
                    if (elapsedMs >= HeartbeatTimeoutMs)
                    {
                        NotifyConnectionLost("Response timed out.");
                        return;
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Heartbeat monitor failed: " + ex.Message);
            }
        }

        private static void QueueLatestRemotePositionMessage(string message)
        {
            bool startPump = false;

            lock (_remoteMessageSync)
            {
                _pendingRemotePositionMessage = message;

                if (!_remoteMessagePumpQueued)
                {
                    _remoteMessagePumpQueued = true;
                    startPump = true;
                }
            }

            if (startPump)
            {
                _ = Task.Run(ProcessLatestRemotePositionMessages);
            }
        }

        private static void ProcessLatestRemotePositionMessages()
        {
            while (true)
            {
                string message;

                lock (_remoteMessageSync)
                {
                    message = _pendingRemotePositionMessage;
                    _pendingRemotePositionMessage = string.Empty;

                    if (string.IsNullOrEmpty(message))
                    {
                        _remoteMessagePumpQueued = false;
                        return;
                    }
                }

                var form = _form;
                if (form == null)
                {
                    continue;
                }

                try
                {
                    form.Invoke((Action)(() => form.checkPosition(message)));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Remote data channel dispatch failed: " + ex.Message);

                    lock (_sync)
                    {
                        if (ReferenceEquals(_form, form))
                        {
                            _form = null;
                        }
                    }
                }
            }
        }

        private static async Task HandleOffer(string sdp)
        {
            try
            {
                var remoteDesc = new RTCSessionDescriptionInit
                {
                    type = RTCSdpType.offer,
                    sdp = sdp
                };

                _peer!.setRemoteDescription(remoteDesc);

                // Create data channel if not already
                if (_channel == null)
                {
                    var dataChannel = await _peer.createDataChannel("remote", _remoteDataChannelInit);
                    AttachRemoteDataChannel(dataChannel);
                }
                _remoteDescriptionSet = true;

                // 🔥 flush buffered candidates
                foreach (var cand in _pendingCandidates)
                {
                    AddCandidateSafe(cand);
                    Debug.WriteLine($"[Flushed] {cand}");
                }
                _pendingCandidates.Clear();

                var answer = _peer.createAnswer(null);

                // Replace actpass with active
                answer.sdp = answer.sdp.Replace("a=setup:actpass", "a=setup:active");

                await _peer.setLocalDescription(answer);

                var db = _redis!.GetDatabase();
                await db.StringSetAsync($"room:{_pairCode}:answer", answer.sdp);

                await PublishMessageAsync(new { type = "answer", sdp = answer.sdp });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("HandleOffer failed: " + ex);
                NotifyConnectionFailure("An error occurred while completing the pairing handshake.");
            }
        }

        private static void AddCandidateSafe(string candStr)
        {
            try
            {
                if (_allowAllCandidates)
                {
                    _peer!.addIceCandidate(new RTCIceCandidateInit { candidate = candStr });
                    Debug.WriteLine($"[ALLOW ALL] {candStr}");
                }
                else if (candStr.Contains(" typ host "))
                {
                    _peer!.addIceCandidate(new RTCIceCandidateInit { candidate = candStr });
                    Debug.WriteLine($"[HOST ONLY] {candStr}");
                }
                else
                {
                    Debug.WriteLine($"[Filtered] {candStr}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("AddCandidate failed: " + ex.Message);
            }
        }

        private static async Task PublishMessageAsync(object payload)
        {
            var msg = JsonSerializer.Serialize(payload);
            await _subscriber!.PublishAsync(_pairCode!, msg);
        }

        public static void SendMessage(string msg)
        {
            if (_channel != null && _channel.readyState == RTCDataChannelState.open)
                _channel.send(Encoding.UTF8.GetBytes(msg));
        }

        public static void Stop()
        {
            lock (_sync)
            {
                if (!_isRunning) return;
                _suppressDisconnectNotification = true;
                try { _cts?.Cancel(); } catch { }
            }

            try { _peer?.Close("Stopping"); } catch { }

            _peer = null;
            _channel = null;

            _cts?.Dispose();
            _cts = null;

            lock (_sync)
            {
                _isRunning = false;
                _connectionEstablished = false;
                _connectionFailureHandled = false;
            }

        }
    }

    public class OBSWebSocket
    {
        private static IHost? _host;
        private static CancellationTokenSource? _cts;
        private static Active? _form;
        private static volatile bool _isRunning;
        private static readonly object _sync = new();
        private static int _port;

        public static bool IsRunning => _isRunning;

        public static void Initialize(Active form) { _form = form; }

        public static void DetachForm(Active form)
        {
            lock (_sync)
            {
                if (ReferenceEquals(_form, form))
                {
                    _form = null;
                }
            }
        }

        public static async Task StartAsync()
        {
            lock (_sync)
            {
                if (_isRunning) return;
                _cts = new CancellationTokenSource();
            }

            _port = ChoosePortFromConfig();

            var builder = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder => {

                    string configPath = Path.Combine(FileSystem.Current.AppDataDirectory, "configs");
                    var streamerModePath = Path.Combine(configPath, "StreamerMode.txt");
                    string? streamerMode = File.Exists(streamerModePath)
                        ? File.ReadAllText(streamerModePath).Trim()
                        : null;

                    if ((streamerMode?.IndexOf("true", StringComparison.OrdinalIgnoreCase) ?? -1) >= 0
                        && NetworkEndpointHelper.TryGetReachableTailscaleIPv4(out var tailscaleIp))
                    {
                        webBuilder.UseKestrel(options =>
                        {
                            options.Listen(tailscaleIp!, _port);
                        });
                        Debug.WriteLine($"OBSWebSocket binding to Tailscale endpoint {tailscaleIp}:{_port}");
                    }
                    else if ((streamerMode?.IndexOf("true", StringComparison.OrdinalIgnoreCase) ?? -1) >= 0)
                    {
                        webBuilder.UseKestrel(options => options.ListenLocalhost(_port));
                        Debug.WriteLine($"OBSWebSocket falling back to localhost:{_port} because Tailscale is not reachable");
                    }
                    else
                    {
                        webBuilder.UseKestrel(options => options.ListenLocalhost(_port));
                    }

                    webBuilder.Configure(app => {
                        var wsOptions = new WebSocketOptions { KeepAliveInterval = TimeSpan.FromSeconds(30) };
                        app.UseWebSockets(wsOptions);

                        app.Map("/ws", branch => branch.Run(async ctx => {
                            if (!ctx.WebSockets.IsWebSocketRequest)
                            {
                                ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
                                return;
                            }

                            using var socket = await ctx.WebSockets.AcceptWebSocketAsync();

                            var senderTask = Task.Run(async () => {
                                float lastX = float.NaN;
                                float lastY = float.NaN;
                                bool? lastLeftDown = null;
                                bool? lastRightDown = null;
                                const int tickMs = 16;

                                while (socket.State == WebSocketState.Open && !_cts!.IsCancellationRequested)
                                {
                                    if (_form != null)
                                    {
                                        float x = _form.xEnd2;
                                        float y = _form.yEnd2;
                                        bool leftDown = _form.localLeftPointerDown;
                                        bool rightDown = _form.localRightPointerDown;

                                        if (!x.Equals(lastX) || !y.Equals(lastY) || leftDown != lastLeftDown || rightDown != lastRightDown)
                                        {
                                            lastX = x;
                                            lastY = y;
                                            lastLeftDown = leftDown;
                                            lastRightDown = rightDown;

                                            long seq = Interlocked.Increment(ref _form.outboundPointerPacketSequence);

                                            string payload = $"Overjoyed - X: {(int)Math.Round(x) + 20}, Y: {(int)Math.Round(y) + 20}, Action: seq:{seq} left:{(leftDown ? "down" : "up")} right:{(rightDown ? "down" : "up")}";
                                            if (_form.UsesNintendoStreamerPlayerRouting())
                                            {
                                                payload = $"{_form.GetEffectiveNintendoStreamerPlayer()}|{payload}";
                                            }
                                            var msg = Encoding.UTF8.GetBytes(payload);

                                            Debug.WriteLine("OBSWebSocket: Sending " + payload);

                                            try
                                            {
                                                await socket.SendAsync(new ArraySegment<byte>(msg), WebSocketMessageType.Text, true, CancellationToken.None);
                                            }
                                            catch (WebSocketException)
                                            {
                                                break;
                                            }
                                        }
                                    }

                                    await Task.Delay(tickMs);
                                }
                            });

                            var buffer = new byte[1024];
                            try
                            {
                                while (!_cts!.IsCancellationRequested && socket.State == WebSocketState.Open)
                                {
                                    var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);
                                    if (result.MessageType == WebSocketMessageType.Close)
                                    {
                                        break;
                                    }
                                }
                            }
                            catch { }
                            finally
                            {
                                try { await senderTask.ConfigureAwait(false); } catch { }
                                try
                                {
                                    if (socket.State == WebSocketState.Open)
                                    {
                                        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                                    }
                                }
                                catch { }
                            }
                        }));
                    });
                });

            _host = builder.Build();

            try
            {
                await _host.StartAsync(_cts!.Token);
                _isRunning = true;
            }
            catch
            {
                try { await (_host?.StopAsync(_cts!.Token) ?? Task.CompletedTask); } catch { }
                try { _host?.Dispose(); } catch { }
                _host = null;
                _isRunning = false;
                throw;
            }
        }

        public static void Stop()
        {
            IHost? hostToStop;
            CancellationTokenSource? localCts;

            lock (_sync)
            {
                if (!_isRunning) return;
                hostToStop = _host;
                localCts = _cts;
                try { localCts?.Cancel(); } catch { }
            }

            try { hostToStop?.StopAsync(CancellationToken.None).GetAwaiter().GetResult(); } catch { }
            try { hostToStop?.Dispose(); } catch { }

            lock (_sync)
            {
                _host = null;
                _cts = null;
                _isRunning = false;
            }
        }

        private static int ChoosePortFromConfig()
        {
            if (_form != null)
            {
                int routedPlayer = _form.UsesNintendoStreamerPlayerRouting()
                    ? _form.GetEffectiveNintendoStreamerPlayer()
                    : _form.playerNum;

                if (routedPlayer >= 1 && routedPlayer <= 4)
                {
                    return 60000 + routedPlayer;
                }
            }

            string configPath = Path.Combine(FileSystem.Current.AppDataDirectory, "configs");
            int port = 60001;

            try
            {
                var content = File.ReadAllText(Path.Combine(configPath, "player.txt"));

                if (content?.IndexOf("player1", StringComparison.OrdinalIgnoreCase) >= 0 &&
                    content?.IndexOf("player2", StringComparison.OrdinalIgnoreCase) >= 0 &&
                    content?.IndexOf("player3", StringComparison.OrdinalIgnoreCase) < 0 &&
                    content?.IndexOf("player4", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    port = 60002;
                }
                else if (content?.IndexOf("player1", StringComparison.OrdinalIgnoreCase) >= 0 &&
                         content?.IndexOf("player2", StringComparison.OrdinalIgnoreCase) >= 0 &&
                         content?.IndexOf("player3", StringComparison.OrdinalIgnoreCase) >= 0 &&
                         content?.IndexOf("player4", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    port = 60003;
                }
                else if (content?.IndexOf("player1", StringComparison.OrdinalIgnoreCase) >= 0 &&
                         content?.IndexOf("player2", StringComparison.OrdinalIgnoreCase) >= 0 &&
                         content?.IndexOf("player3", StringComparison.OrdinalIgnoreCase) >= 0 &&
                         content?.IndexOf("player4", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    port = 60004;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to assign OBS port {ex.Message}");
            }

            return port;
        }
    }

}