using ExtendedMessageBoxLibrary;
using GregsStack.InputSimulatorStandard.Native;
using Microsoft.Ajax.Utilities;
using Microsoft.Maui.Storage;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Exceptions;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using SharedVars;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Wisej.Core;
using Wisej.Hybrid;
using Wisej.Web;

namespace OverjoyedReleaseLocal
{
    public class SimGamePad
    {
        private static SimGamePad instance;
        private static readonly string NoDriverMessage =
            "<p style='margin:0;padding:0'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 9; -webkit-box-orient: vertical; overflow: hidden;' title='To use Gamepad Mode, the Xbox/PlayStation driver called Vigembus that  simulates GamePad input has been downloaded in your browser. Please open and run the installer after it finishes downloading. Once the driver is detected, this prompt will disappear. If the prompt does not disappear and no Controller Connected tone plays, close Overjoyed and try again.'>To use Gamepad Mode, the Xbox/PlayStation driver called Vigembus that  simulates GamePad input has been downloaded in your browser. Please <strong>open and run the installer</strong> after it finishes downloading.<br><br>Once the driver is detected, this prompt will disappear. If the prompt does not disappear and no Controller Connected tone plays, close Overjoyed and try again.</span></p>";

        private readonly bool[] isPluggedIn = new bool[4];
        private string driverInstalled = "false";
        private static bool suppressDriverMessage = false;

        // Track controllers created by Overjoyed using GUIDs for identification
        private Dictionary<Guid, IXbox360Controller> xboxControllers = new Dictionary<Guid, IXbox360Controller>();
        private Dictionary<Guid, IDualShock4Controller> ds4Controllers = new Dictionary<Guid, IDualShock4Controller>();
        private Guid activeXboxGuid = Guid.Empty;
        private Guid activeDs4Guid = Guid.Empty;
        
        // Track XInput controller indices used by Overjoyed's virtual controllers
        private HashSet<int> overjoydControllerIndices = new HashSet<int>();

        private string[] buttonOptions = new string[26]
        {
            "DPad Up", "DPad Down", "DPad Left", "DPad Right", "Menu or Start",
            "View or Select", "LSB or L3", "RSB or R3", "LB or L1", "RB or R1",
            "Guide", "None", "A or X", "B or O", "X or ⬜", "Y or ꕔ", "LT or L2",
            "RT or R2", "LS Left", "LS Right", "LS Up", "LS Down", "RS Left", "RS Right", "RS Up", "RS Down"
        };

        private bool pluggedOnce = false;

        private ViGEmClient client;
        IXbox360Controller xboxController;
        IDualShock4Controller ds4Controller;

        private string controllerMode = "xbox"; // "xbox" or "ds4"

        private SimGamePad()
        {
            driverInstalled = "false";
            // Scan PnP at startup to identify ViGEm devices
            ScanPnPForViGEmDevices();
            Initialize();
        }

        // P/Invoke for XInput controller detection
        [StructLayout(LayoutKind.Sequential)]
        private struct XINPUT_GAMEPAD
        {
            public ushort wButtons;
            public byte bLeftTrigger;
            public byte bRightTrigger;
            public short sThumbLX;
            public short sThumbLY;
            public short sThumbRX;
            public short sThumbRY;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct XINPUT_STATE
        {
            public uint dwPacketNumber;
            public XINPUT_GAMEPAD Gamepad;
        }

        [DllImport("xinput1_4.dll")]
        private static extern int XInputGetState(int dwUserIndex, out XINPUT_STATE pState);

        // SetupAPI P/Invoke for PnP device enumeration (for filtering, not input)
        [DllImport("setupapi.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SetupDiGetClassDevs(
            ref Guid classGuid,
            string enumerator,
            IntPtr hwndParent,
            uint flags);

        [DllImport("setupapi.dll")]
        private static extern bool SetupDiEnumDeviceInfo(
            IntPtr hDevInfo,
            uint memberIndex,
            ref SP_DEVINFO_DATA deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Unicode)]
        private static extern bool SetupDiGetDeviceRegistryProperty(
            IntPtr hDevInfo,
            ref SP_DEVINFO_DATA deviceInfoData,
            uint property,
            out uint propertyRegDataType,
            byte[] propertyBuffer,
            uint propertyBufferSize,
            out uint requiredSize);

        [DllImport("setupapi.dll")]
        private static extern bool SetupDiDestroyDeviceInfoList(IntPtr hDevInfo);

        private const uint DIGCF_PRESENT = 0x00000002;
        private const uint SPDRP_HARDWAREID = 0x00000001;

        [StructLayout(LayoutKind.Sequential)]
        private struct SP_DEVINFO_DATA
        {
            public uint cbSize;
            public Guid classGuid;
            public uint devInst;
            public IntPtr reserved;
        }

        // ViGEm device identifiers (constant across all ViGEm versions)
        private const ushort VIGEM_VID = 0x1234;
        private const ushort VIGEM_PID_XBOX360 = 0x5678;
        private const ushort VIGEM_PID_DS4 = 0x5680;

        public static void resetFlag()
        {
            suppressDriverMessage = false;
        }

        public static SimGamePad Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SimGamePad();
                }
                return instance;
            }
        }

        /// <summary>
        /// Check if a controller GUID belongs to an Overjoyed-created ViGEm controller.
        /// Used by gamepad detection to ignore virtual controllers created by this application.
        /// </summary>
        public bool IsOverjoyedController(Guid controllerGuid)
        {
            return xboxControllers.ContainsKey(controllerGuid) || ds4Controllers.ContainsKey(controllerGuid);
        }

        /// <summary>
        /// Get all active Overjoyed-created controller GUIDs.
        /// </summary>
        public IEnumerable<Guid> GetActiveOverjoyedControllerGuids()
        {
            return xboxControllers.Keys.Concat(ds4Controllers.Keys);
        }

        /// <summary>
        /// Check if a specific XInput controller index belongs to an Overjoyed virtual controller.
        /// Used by gamepad detection to filter out only Overjoyed's virtual controllers,
        /// allowing detection of physical controllers and other ViGEm controllers.
        /// </summary>
        public bool IsOverjoyedControllerIndex(int xinputIndex)
        {
            return overjoydControllerIndices.Contains(xinputIndex);
        }

        /// <summary>
        /// Register an XInput controller index as belonging to Overjoyed.
        /// Called when a virtual controller is successfully connected.
        /// </summary>
        private void RegisterOverjoyedControllerIndex(int xinputIndex)
        {
            overjoydControllerIndices.Add(xinputIndex);
        }

        /// <summary>
        /// Unregister an XInput controller index.
        /// Called when a virtual controller is disconnected.
        /// </summary>
        private void UnregisterOverjoyedControllerIndex(int xinputIndex)
        {
            overjoydControllerIndices.Remove(xinputIndex);
        }

        /// <summary>
        /// Scan PnP devices to identify ViGEm controllers by VID/PID.
        /// Filters out ViGEm devices from gamepad detection without relying on XInput state.
        /// Called once at startup.
        /// </summary>
        private void ScanPnPForViGEmDevices()
        {
            try
            {
                // GUID for HID devices
                Guid hidGuid = Guid.Parse("4D1E55B2-F16F-11CF-88CB-001111000030");

                IntPtr hDevInfo = SetupDiGetClassDevs(
                    ref hidGuid,
                    null,
                    IntPtr.Zero,
                    DIGCF_PRESENT);

                if (hDevInfo == IntPtr.Zero)
                {
                    Debug.WriteLine("SetupDiGetClassDevs failed");
                    return;
                }

                try
                {
                    SP_DEVINFO_DATA deviceInfoData = new SP_DEVINFO_DATA();
                    deviceInfoData.cbSize = (uint)Marshal.SizeOf(deviceInfoData);

                    for (uint i = 0; SetupDiEnumDeviceInfo(hDevInfo, i, ref deviceInfoData); i++)
                    {
                        if (TryExtractViGEmDevice(hDevInfo, ref deviceInfoData))
                        {
                            Debug.WriteLine($"Found ViGEm device at enumeration index {i}");
                        }
                    }
                }
                finally
                {
                    SetupDiDestroyDeviceInfoList(hDevInfo);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error scanning PnP devices: {ex.Message}");
            }
        }

        /// <summary>
        /// Extract ViGEm device info from PnP and register its XInput index.
        /// Returns true if device is a ViGEm controller.
        /// </summary>
        private bool TryExtractViGEmDevice(IntPtr hDevInfo, ref SP_DEVINFO_DATA deviceInfoData)
        {
            try
            {
                byte[] hardwareIdBuffer = new byte[1024];
                uint propertyType;
                uint requiredSize;

                if (!SetupDiGetDeviceRegistryProperty(
                    hDevInfo,
                    ref deviceInfoData,
                    SPDRP_HARDWAREID,
                    out propertyType,
                    hardwareIdBuffer,
                    (uint)hardwareIdBuffer.Length,
                    out requiredSize))
                {
                    return false;
                }

                string hardwareId = Encoding.Unicode.GetString(hardwareIdBuffer, 0, (int)requiredSize).TrimEnd('\0');
                
                // ViGEm devices have hardware IDs like: USB\VID_1234&PID_5678\...
                if (hardwareId.Contains("VID_1234"))
                {
                    if (hardwareId.Contains("PID_5678") || hardwareId.Contains("PID_5680"))
                    {
                        // This is a ViGEm device, but we can't directly map to XInput index from PnP
                        // Instead, we'll use this as confirmation and let XInput scanning handle it
                        Debug.WriteLine($"Identified ViGEm device: {hardwareId}");
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error extracting device info: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Scan to find which XInput index a newly connected ViGEm controller was assigned to.
        /// Uses XInput since we only need to track active indices, not identify controllers.
        /// Returns true if a new controller was found and registered.
        /// </summary>
        private bool TryRegisterNewVirtualController()
        {
            // Scan all indices to find a newly connected controller
            for (int i = 0; i < 4; i++)
            {
                // Already tracking this index
                if (overjoydControllerIndices.Contains(i))
                    continue;

                XINPUT_STATE state;
                if (XInputGetState(i, out state) == 0) // Controller is connected
                {
                    // Register this as an Overjoyed controller
                    RegisterOverjoyedControllerIndex(i);
                    Debug.WriteLine($"Registered Overjoyed virtual controller at XInput index {i}");
                    return true;
                }
            }
            return false;
        }

        private void resetTransparency()
        {
            int opacity = 255;
            string configPath = Path.Combine(FileSystem.Current.AppDataDirectory, "configs");
            string alphaFilePath = Path.Combine(configPath, "Transparency.txt");

            try
            {
                using (FileStream fs = new FileStream(alphaFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                using (StreamWriter file2 = new StreamWriter(fs))
                {
                    file2.WriteLine(opacity);
                    file2.WriteLine("false");
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

        public void Initialize(bool mayAutoInstallDriver = true, string mode = "xbox")
        {
            controllerMode = mode;
            if (controllerMode == "xbox")
                xboxOff();
            else if (controllerMode == "ds4")
                ds4Off();

            if (suppressDriverMessage)
            {
                Debug.WriteLine("Driver installation message suppressed.");
                return;
            }

            if (driverInstalled != "true")
            {
                try
                {
                    client = new ViGEmClient();
                    Debug.WriteLine("ViGEmBus driver is installed.");
                    driverInstalled = "true";

                    if (controllerMode == "xbox")
                        xboxOn();
                    else if (controllerMode == "ds4")
                        ds4On();
                }
                catch (VigemBusNotFoundException)
                {
                    Debug.WriteLine("ViGEmBus driver not found.");

                    if (!mayAutoInstallDriver)
                    {
                        driverInstalled = "canceled";
                        return;
                    }

                    // Non-blocking UI: Show floating panel, start download, poll for install completion
                    resetTransparency();
                    suppressDriverMessage = true;
                    driverInstalled = "messaged";
                    Debug.WriteLine("Prompting user to install ViGEmBus driver.");

                    setCorner();

                    System.Diagnostics.Process.Start(
                        "explorer.exe",
                        "https://github.com/nefarius/ViGEmBus/releases/download/v1.22.0/ViGEmBus_1.22.0_x64_x86_arm64.exe"
                    );

                    var closePanel = ShowFloatingPanel(
                        NoDriverMessage,
                        "Download & Install Gamepad Driver"
                    );

                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        bool installed = false;
                        while (!installed)
                        {
                            Thread.Sleep(2000);
                            try
                            {
                                client = new ViGEmClient();

                                if (controllerMode == "xbox")
                                {
                                    Guid xboxGuid = Guid.NewGuid();
                                    xboxController = client.CreateXbox360Controller();
                                    xboxController.AutoSubmitReport = true;
                                    xboxController.Connect();
                                    xboxControllers[xboxGuid] = xboxController;
                                    activeXboxGuid = xboxGuid;
                                    Thread.Sleep(100); // Brief delay to ensure XInput detects the controller
                                    TryRegisterNewVirtualController();
                                    installed = true;
                                    driverInstalled = "true";
                                }
                                else if (controllerMode == "ds4")
                                {
                                    Guid ds4Guid = Guid.NewGuid();
                                    ds4Controller = client.CreateDualShock4Controller();
                                    ds4Controller.Connect();
                                    ds4Controllers[ds4Guid] = ds4Controller;
                                    activeDs4Guid = ds4Guid;
                                    Thread.Sleep(100); // Brief delay to ensure XInput detects the controller
                                    TryRegisterNewVirtualController();
                                    installed = true;
                                    driverInstalled = "true";
                                }
                            }
                            catch
                            {
                                // keep polling until installed
                            }
                        }

                        // Close panel and notify mode
                        closePanel();

                        if (controllerMode == "xbox")
                            xboxOn();
                        else if (controllerMode == "ds4")
                            ds4On();
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("An error occurred: " + ex.Message);
                }
            }
            else
            {
                try
                {
                    client = new ViGEmClient();
                    if (controllerMode == "xbox")
                        xboxOn();
                    else if (controllerMode == "ds4")
                        ds4On();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("An error occurred while initializing the ViGEmClient: " + ex.Message);
                }
            }
        }

        private async void setCorner()
        {
            int playerNum = PlayerAssignment.GetAssignedPlayerOrDefault();

            await MinimizeMessenger.SendMessageAsync("corner|" + playerNum);
        }

        private Action ShowFloatingPanel(string message, string title = "Download Started")
        {
            var panel = new Wisej.Web.Panel
            {
                Width = 470,
                Height = 260,
                HeaderSize = 40,
                BorderStyle = Wisej.Web.BorderStyle.Solid,
                ShowCloseButton = false,
                ShowHeader = true,
                
                Text = title,
                Anchor = AnchorStyles.None,
                Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point),
                Location = new System.Drawing.Point(
                    (Application.MainPage.Width - 470) / 2,
                    (Application.MainPage.Height - 210) / 2),
                Padding = new Wisej.Web.Padding(0, 0, 0, 0)
            };

            var contentPanel = new Wisej.Web.Panel
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.White,
                Padding = new Wisej.Web.Padding(20, 20, 20, 10)
            };

            var label = new Wisej.Web.Label
            {
                Text = message,
                Dock = DockStyle.Top,
                AutoSize = false,
                AllowHtml = true,
                Height = 200,
                TextAlign = System.Drawing.ContentAlignment.TopLeft,
                Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point),
                Padding = new Wisej.Web.Padding(0, 0, 0, 10)
            };

            contentPanel.Controls.Add(label);
            panel.Controls.Add(contentPanel);

            Application.MainPage.Controls.Add(panel);

            if (Screen.Bounds.Height >= 1100 && Screen.Bounds.Width > 1920)
            {
                panel.Scale(1.4f);
                ScaleFonts(panel, 1.4f);
            }
            panel.BringToFront();

            panel.Anchor = AnchorStyles.None;
            panel.Location = new System.Drawing.Point(
                (Application.MainPage.Width - panel.Width) / 2,
                (Application.MainPage.Height - panel.Height) / 2
            );

            return () =>
            {
                if (!panel.IsDisposed)
                    panel.Dispose();
            };
        }

        private void ScaleFonts(Control parent, float scale)
        {
            parent.Font = new System.Drawing.Font(
                parent.Font.Name,
                parent.Font.Size * scale,
                parent.Font.Style
            );

            foreach (Control ctrl in parent.Controls)
            {
                ctrl.Font = new System.Drawing.Font(
                    ctrl.Font.Name,
                    ctrl.Font.Size * scale,
                    ctrl.Font.Style
                );

                if (ctrl.HasChildren)
                    ScaleFonts(ctrl, scale);
            }
        }

        private async void xboxOn()
        {
            int playerNum = PlayerAssignment.GetAssignedPlayerOrDefault();

            await ActiveMessenger.SendMessageAsyncActive("xbox-on|" + playerNum);
        }

        private async void xboxOff()
        {
            int playerNum = PlayerAssignment.GetAssignedPlayerOrDefault();

            await ActiveMessenger.SendMessageAsyncActive("xbox-off|" + playerNum);
        }

        public void ShutDown()
        {
            try
            {
                ds4Controller?.Disconnect();
                xboxController?.Disconnect();
                client?.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error during shutdown: " + ex.Message);
            }
            finally
            {
                // Cleanup controller tracking
                if (activeXboxGuid != Guid.Empty && xboxControllers.ContainsKey(activeXboxGuid))
                {
                    xboxControllers.Remove(activeXboxGuid);
                    activeXboxGuid = Guid.Empty;
                }
                if (activeDs4Guid != Guid.Empty && ds4Controllers.ContainsKey(activeDs4Guid))
                {
                    ds4Controllers.Remove(activeDs4Guid);
                    activeDs4Guid = Guid.Empty;
                }
                ds4Controller = null;
                xboxController = null;
                client = null;
            }
        }

        public void PlugIn(int controllerIndex = 0)
        {
            try
            {
                if (driverInstalled == "true")
                {
                    client = new ViGEmClient();

                    if (controllerMode == "xbox")
                    {
                        Guid xboxGuid = Guid.NewGuid();
                        xboxController = client.CreateXbox360Controller();
                        xboxController.AutoSubmitReport = true;
                        xboxController.Connect();
                        xboxControllers[xboxGuid] = xboxController;
                        activeXboxGuid = xboxGuid;
                        Thread.Sleep(100); // Brief delay to ensure XInput detects the controller
                        TryRegisterNewVirtualController();
                        xboxOn();
                    }
                    else if (controllerMode == "ds4")
                    {
                        Guid ds4Guid = Guid.NewGuid();
                        ds4Controller = client.CreateDualShock4Controller();
                        ds4Controller.Connect();
                        ds4Controllers[ds4Guid] = ds4Controller;
                        activeDs4Guid = ds4Guid;
                        Thread.Sleep(100); // Brief delay to ensure XInput detects the controller
                        TryRegisterNewVirtualController();
                        ds4On();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occurred while plugging in the controller: " + ex.Message);
            }
        }

        private async void ds4On()
        {
            int playerNum = PlayerAssignment.GetAssignedPlayerOrDefault();

            await ActiveMessenger.SendMessageAsyncActive("ds4-on|" + playerNum);
        }

        private async void ds4Off()
        {
            int playerNum = PlayerAssignment.GetAssignedPlayerOrDefault();

            await ActiveMessenger.SendMessageAsyncActive("ds4-off|" + playerNum);
        }

        public void Unplug(int controllerIndex = 0)
        {
            try
            {
                if (controllerMode == "xbox" && xboxController != null)
                {
                    xboxController.Disconnect();
                    if (activeXboxGuid != Guid.Empty && xboxControllers.ContainsKey(activeXboxGuid))
                    {
                        xboxControllers.Remove(activeXboxGuid);
                        activeXboxGuid = Guid.Empty;
                    }
                }
                else if (controllerMode == "ds4" && ds4Controller != null)
                {
                    ds4Controller.Disconnect();
                    if (activeDs4Guid != Guid.Empty && ds4Controllers.ContainsKey(activeDs4Guid))
                    {
                        ds4Controllers.Remove(activeDs4Guid);
                        activeDs4Guid = Guid.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error during unplug: " + ex.Message);
            }
        }

        public void Use(int control, ConcurrentBag<string> toggleXbox, int controllerIndex = 0, int holdTimeMS = 1000)
        {
            SetControl(control, controllerIndex);
            Thread.Sleep(holdTimeMS);
            ReleaseControl(control, toggleXbox, controllerIndex);
        }

        private static int GetXboxButtonId(int control)
        {
            switch (control)
            {
                case 12: return 11;
                case 13: return 12;
                case 14: return 13;
                case 15: return 14;
                default: return control;
            }
        }

        public void SetControl(int control, int controllerIndex = 0)
        {
            if (controllerMode == "xbox" && xboxController != null)
            {
                if (control > 11 && control < 16)
                {
                    xboxController.SetButtonState(GetXboxButtonId(control), true);
                }
                if (control < 11) xboxController.SetButtonState(control, true);

                if (control == 16) VariableTriggers(0, 1);
                if (control == 17) VariableTriggers(1, 1);

                switch (control)
                {
                    case 18: MoveSticks(0, -1, false); break; // LS Left
                    case 19: MoveSticks(0, 1, false); break; // LS Right
                    case 20: MoveSticks(1, 1, false); break; // LS Up
                    case 21: MoveSticks(1, -1, false); break; // LS Down
                    case 22: MoveSticks(2, -1, false); break; // RS Left
                    case 23: MoveSticks(2, 1, false); break; // RS Right
                    case 24: MoveSticks(3, 1, false); break; // RS Up
                    case 25: MoveSticks(3, -1, false); break; // RS Down
                }
            }
            else if (controllerMode == "ds4" && ds4Controller != null)
            {
                if (control > 11 && control < 16)
                {
                    switch (control)
                    {
                        case 12: ds4Controller.SetButtonState(DualShock4Button.Cross, true); break;
                        case 13: ds4Controller.SetButtonState(DualShock4Button.Circle, true); break;
                        case 14: ds4Controller.SetButtonState(DualShock4Button.Square, true); break;
                        case 15: ds4Controller.SetButtonState(DualShock4Button.Triangle, true); break;
                    }
                }

                if (control < 11)
                {
                    switch (control)
                    {
                        case 0: ds4Controller.SetDPadDirection(DualShock4DPadDirection.North); break;
                        case 1: ds4Controller.SetDPadDirection(DualShock4DPadDirection.South); break;
                        case 2: ds4Controller.SetDPadDirection(DualShock4DPadDirection.West); break;
                        case 3: ds4Controller.SetDPadDirection(DualShock4DPadDirection.East); break;
                        case 4: ds4Controller.SetButtonState(DualShock4Button.Options, true); break;
                        case 5: ds4Controller.SetButtonState(DualShock4Button.Share, true); break;
                        case 6: ds4Controller.SetButtonState(DualShock4Button.ThumbLeft, true); break;
                        case 7: ds4Controller.SetButtonState(DualShock4Button.ThumbRight, true); break;
                        case 8: ds4Controller.SetButtonState(DualShock4Button.ShoulderLeft, true); break;
                        case 9: ds4Controller.SetButtonState(DualShock4Button.ShoulderRight, true); break;
                        case 10: ds4Controller.SetSpecialButtonsFull(0x01); break; // PS
                    }
                }

                if (control == 16) VariableTriggers(0, 1);
                if (control == 17) VariableTriggers(1, 1);

                switch (control)
                {
                    case 18: MoveSticks(0, -1, false); break;
                    case 19: MoveSticks(0, 1, false); break;
                    case 20: MoveSticks(1, 1, false); break;
                    case 21: MoveSticks(1, -1, false); break;
                    case 22: MoveSticks(2, -1, false); break;
                    case 23: MoveSticks(2, 1, false); break;
                    case 24: MoveSticks(3, 1, false); break;
                    case 25: MoveSticks(3, -1, false); break;
                }

                ds4Controller.SubmitReport();
            }
        }

        public void ReleaseControl(int control, ConcurrentBag<string> toggleXbox, int controllerIndex = 0)
        {
            if (controllerMode == "xbox" && xboxController != null)
            {
                if (control > 11 && control < 16 && !toggleXbox.Contains(buttonOptions[control - 1]))
                {
                    xboxController.SetButtonState(GetXboxButtonId(control), false);
                }

                if (control < 11 && !toggleXbox.Contains(buttonOptions[control]))
                    xboxController.SetButtonState(control, false);

                if (control == 16 && !toggleXbox.Contains(buttonOptions[control])) VariableTriggers(0, 0);
                if (control == 17 && !toggleXbox.Contains(buttonOptions[control])) VariableTriggers(1, 0);

                if (control >= 18 && control <= 21)
                {
                    if (!(toggleXbox.Contains("LS Up") || toggleXbox.Contains("LS Down") ||
                          toggleXbox.Contains("LS Left") || toggleXbox.Contains("LS Right")))
                    {
                        MoveSticks(0, 0, false);
                        MoveSticks(1, 0, false);
                    }
                }

                if (control >= 22 && control <= 25)
                {
                    if (!(toggleXbox.Contains("RS Up") || toggleXbox.Contains("RS Down") ||
                          toggleXbox.Contains("RS Left") || toggleXbox.Contains("RS Right")))
                    {
                        MoveSticks(2, 0, false);
                        MoveSticks(3, 0, false);
                    }
                }
            }
            else if (controllerMode == "ds4" && ds4Controller != null)
            {
                if (control > 11 && control < 16 && !toggleXbox.Contains(buttonOptions[control - 1]))
                {
                    switch (control)
                    {
                        case 12: ds4Controller.SetButtonState(DualShock4Button.Cross, false); break;
                        case 13: ds4Controller.SetButtonState(DualShock4Button.Circle, false); break;
                        case 14: ds4Controller.SetButtonState(DualShock4Button.Square, false); break;
                        case 15: ds4Controller.SetButtonState(DualShock4Button.Triangle, false); break;
                    }
                }

                if (control < 11 && !toggleXbox.Contains(buttonOptions[control]))
                {
                    switch (control)
                    {
                        case 0: ds4Controller.SetDPadDirection(DualShock4DPadDirection.None); break;
                        case 1: ds4Controller.SetDPadDirection(DualShock4DPadDirection.None); break;
                        case 2: ds4Controller.SetDPadDirection(DualShock4DPadDirection.None); break;
                        case 3: ds4Controller.SetDPadDirection(DualShock4DPadDirection.None); break;
                        case 4: ds4Controller.SetButtonState(DualShock4Button.Options, false); break;
                        case 5: ds4Controller.SetButtonState(DualShock4Button.Share, false); break;
                        case 6: ds4Controller.SetButtonState(DualShock4Button.ThumbLeft, false); break;
                        case 7: ds4Controller.SetButtonState(DualShock4Button.ThumbRight, false); break;
                        case 8: ds4Controller.SetButtonState(DualShock4Button.ShoulderLeft, false); break;
                        case 9: ds4Controller.SetButtonState(DualShock4Button.ShoulderRight, false); break;
                        case 10: ds4Controller.SetSpecialButtonsFull(0x00); break; // PS off
                    }
                }

                if (control == 16 && !toggleXbox.Contains(buttonOptions[control])) VariableTriggers(0, 0);
                if (control == 17 && !toggleXbox.Contains(buttonOptions[control])) VariableTriggers(1, 0);

                if (control >= 18 && control <= 21)
                {
                    if (!(toggleXbox.Contains("LS Up") || toggleXbox.Contains("LS Down") ||
                          toggleXbox.Contains("LS Left") || toggleXbox.Contains("LS Right")))
                    {
                        MoveSticks(0, 0, false);
                        MoveSticks(1, 0, false);
                    }
                }

                if (control >= 22 && control <= 25)
                {
                    if (!(toggleXbox.Contains("RS Up") || toggleXbox.Contains("RS Down") ||
                          toggleXbox.Contains("RS Left") || toggleXbox.Contains("RS Right")))
                    {
                        MoveSticks(2, 0, false);
                        MoveSticks(3, 0, false);
                    }
                }

                ds4Controller.SubmitReport();
            }
        }

        public void VariableTriggers(int trigger, double value)
        {
            byte byteValue = (byte)(255 * value);

            if (controllerMode == "xbox" && xboxController != null)
            {
                if (trigger == 0) xboxController.SetSliderValue(Xbox360Slider.LeftTrigger, byteValue);
                if (trigger == 1) xboxController.SetSliderValue(Xbox360Slider.RightTrigger, byteValue);
                xboxController.SubmitReport();
            }
            else if (controllerMode == "ds4" && ds4Controller != null)
            {
                if (trigger == 0) ds4Controller.SetSliderValue(DualShock4Slider.LeftTrigger, byteValue);
                if (trigger == 1) ds4Controller.SetSliderValue(DualShock4Slider.RightTrigger, byteValue);
                ds4Controller.SubmitReport();
            }
        }

        public void MoveSticks(int axisIndex, double value, bool invert)
        {
            if (invert) value = -value;

            if (controllerMode == "xbox" && xboxController != null)
            {
                short shortValue = (short)(32767 * value);
                switch (axisIndex)
                {
                    case 0: xboxController.SetAxisValue(Xbox360Axis.LeftThumbX, shortValue); break;
                    case 1: xboxController.SetAxisValue(Xbox360Axis.LeftThumbY, shortValue); break;
                    case 2: xboxController.SetAxisValue(Xbox360Axis.RightThumbX, shortValue); break;
                    case 3: xboxController.SetAxisValue(Xbox360Axis.RightThumbY, shortValue); break;
                }
                xboxController.SubmitReport();
            }
            else if (controllerMode == "ds4" && ds4Controller != null)
            {
                byte MapAxis(double v) => (byte)(128 + 127 * v);

                switch (axisIndex)
                {
                    case 0: ds4Controller.SetAxisValue(DualShock4Axis.LeftThumbX, MapAxis(value)); break;
                    case 1: ds4Controller.SetAxisValue(DualShock4Axis.LeftThumbY, MapAxis(-value)); break;
                    case 2: ds4Controller.SetAxisValue(DualShock4Axis.RightThumbX, MapAxis(value)); break;
                    case 3: ds4Controller.SetAxisValue(DualShock4Axis.RightThumbY, MapAxis(-value)); break;
                }

                ds4Controller.SubmitReport();
            }
        }

        public void Update(int controllerIndex = 0)
        {
        }

        public bool IsControllerPluggedIn(int controllerIndex = 0)
        {
            return isPluggedIn[controllerIndex];
        }
    }
}