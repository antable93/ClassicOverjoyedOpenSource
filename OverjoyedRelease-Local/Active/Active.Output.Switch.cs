using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using SharedVars;
using Wisej.Web;

namespace OverjoyedReleaseLocal
{
    public enum KeyMap
    {
        KeyMap_Up = 0x200,
        KeyMap_Down = 0x100,
        KeyMap_Left = 0x80,
        KeyMap_Right = 0x40,
        KeyMap_A = 0x2000,
        KeyMap_B = 0x1000,
        KeyMap_X = 0x10,
        KeyMap_Y = 0x20,
        KeyMap_L1 = 0x400,
        KeyMap_R1 = 0x800,
        KeyMap_L2 = 0x4000,
        KeyMap_R2 = 0x20000,
        KeyMap_L3 = 0x02,
        KeyMap_R3 = 0x04,
        KeyMap_Select = 0x08,
        KeyMap_Start = 0x01,
        KeyMap_Home = 0x8000,
        KeyMap_Screenshot = 0x10000,
        KeyMap_Pair = 0x40000,
        KeyMap_Wakeup = 0x40000000,
    }
    class Mapkey
    {
    }

    public static class SHBuffer
    {
        //// <summary>
        /// Convert a struct to a byte array
        /// </summary>
        public static byte[] StructToBytes(object structObj)
        {
            int size = Marshal.SizeOf(structObj);
            byte[] bytes = new byte[size];
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(structObj, structPtr, false);
            Marshal.Copy(structPtr, bytes, 0, size);
            Marshal.FreeHGlobal(structPtr);
            return bytes;
        }


        public static void memcpy(byte[] dst, int dstOffset, byte[] src, int srcOffset, int len)
        {
            Buffer.BlockCopy(src, srcOffset, dst, dstOffset, len);
        }
    }

    internal static class HidEnumerator
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct SP_DEVICE_INTERFACE_DATA
        {
            public int cbSize;
            public Guid InterfaceClassGuid;
            public int Flags;
            public IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            public int cbSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string DevicePath;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HIDD_ATTRIBUTES
        {
            public int Size;
            public ushort VendorID;
            public ushort ProductID;
            public ushort VersionNumber;
        }

        private const int DIGCF_PRESENT = 0x00000002;
        private const int DIGCF_DEVICEINTERFACE = 0x00000010;
        private const uint GENERIC_READ = 0x80000000;
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint FILE_SHARE_READ = 0x00000001;
        private const uint FILE_SHARE_WRITE = 0x00000002;
        private const uint OPEN_EXISTING = 3;

        [DllImport("hid.dll")]
        private static extern void HidD_GetHidGuid(out Guid HidGuid);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, IntPtr Enumerator, IntPtr hwndParent, int Flags);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiEnumDeviceInterfaces(IntPtr DeviceInfoSet, IntPtr DeviceInfoData, ref Guid InterfaceClassGuid, int MemberIndex, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, IntPtr DeviceInterfaceDetailData, int DeviceInterfaceDetailDataSize, out int RequiredSize, IntPtr DeviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, ref SP_DEVICE_INTERFACE_DETAIL_DATA DeviceInterfaceDetailData, int DeviceInterfaceDetailDataSize, out int RequiredSize, IntPtr DeviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("hid.dll")]
        private static extern bool HidD_GetAttributes(IntPtr HidDeviceObject, ref HIDD_ATTRIBUTES Attributes);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        public static string[] GetReceiverPaths(ushort vid, ushort pid)
        {
            HidD_GetHidGuid(out var hidGuid);
            var devs = SetupDiGetClassDevs(ref hidGuid, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);
            if (devs == IntPtr.Zero || devs == (IntPtr)(-1)) return Array.Empty<string>();

            var paths = new List<string>();
            try
            {
                var did = new SP_DEVICE_INTERFACE_DATA { cbSize = Marshal.SizeOf<SP_DEVICE_INTERFACE_DATA>() };
                int index = 0;
                while (SetupDiEnumDeviceInterfaces(devs, IntPtr.Zero, ref hidGuid, index, ref did))
                {
                    index++;

                    // Query required size then fetch detail with path
                    SetupDiGetDeviceInterfaceDetail(devs, ref did, IntPtr.Zero, 0, out int requiredSize, IntPtr.Zero);
                    var detail = new SP_DEVICE_INTERFACE_DETAIL_DATA
                    {
                        cbSize = IntPtr.Size == 8 ? 8 : 4 + Marshal.SystemDefaultCharSize
                    };
                    if (!SetupDiGetDeviceInterfaceDetail(devs, ref did, ref detail, Marshal.SizeOf<SP_DEVICE_INTERFACE_DETAIL_DATA>(), out _, IntPtr.Zero))
                        continue;

                    // Open to read attributes (no access needed beyond attributes)
                    var handle = CreateFile(detail.DevicePath, 0, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
                    if (handle != (IntPtr)(-1))
                    {
                        var attrib = new HIDD_ATTRIBUTES { Size = Marshal.SizeOf<HIDD_ATTRIBUTES>() };
                        if (HidD_GetAttributes(handle, ref attrib))
                        {
                            if (attrib.VendorID == vid && attrib.ProductID == pid)
                                paths.Add(detail.DevicePath);
                        }
                        CloseHandle(handle);
                    }
                }
            }
            finally
            {
                SetupDiDestroyDeviceInfoList(devs);
            }

            return paths.ToArray();
        }
    }


    public partial class Gamepad
    {
        public const int EBITDO_VID = 0x2DC8;
        public const int MICRO_PID = 0x9020; // km per sec.

        //get all device paths containing EBITDO_VID and MICRO_PID
       public string[] paths = HidEnumerator.GetReceiverPaths(Gamepad.EBITDO_VID, Gamepad.MICRO_PID);

        public int playerNum = 1; //default player 1
        public string configPath = Path.Combine(FileSystem.Current.AppDataDirectory, "configs");


        //record keys press
        private int L3 = 0;
        private int R3 = 0;
        private int L2 = 0;
        private int R2 = 0;
        private int L = 0;
        private int R = 0;
        private int Up = 0;
        private int Down = 0;
        private int Left = 0;
        private int Right = 0;
        private int A = 0;
        private int B = 0;
        private int X = 0;
        private int Y = 0;
        private int Select = 0;
        private int Start = 0;
        private int Home = 0;
        private int Screenshot = 0;
        private int Pair = 0;
        private int wakeup_btn = 0;
        private int lx = 127;
        private int ly = 127;
        private int rx = 127;
        private int ry = 127;


        //order
        public enum CUSTOM_INPUT_CMD_E
        {
            CUMTOM_SET_IPNUT_VALUE = 1,
            CUMTOM_REQUST_BATTERY_VALUE,
            CUMTOM_RSPONE_BATTERY_VALUE
        }

        // Delegate signature must match HidLogCallback
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate void HidLogCallback(string message);

        [DllImport("microHIDx64.dll",
        EntryPoint = "SetHidLogger",
        CallingConvention = CallingConvention.StdCall,
        CharSet = CharSet.Unicode)]
        internal static extern void SetHidLogger(HidLogCallback cb);



        [DllImport("microHIDx64.dll",
            EntryPoint = "Find24GHid",
            CallingConvention = CallingConvention.StdCall,
            CharSet = CharSet.Unicode,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true)]
        internal static extern bool Find24GHid(ushort vid, ushort pid,
            [MarshalAs(UnmanagedType.LPWStr)] string fullDevicePath);
    
    

        [DllImport(@"microHIDx64.dll", EntryPoint = "Open_Hid")]
        extern static bool Open_Hid();

        [DllImport(@"microHIDx64.dll", EntryPoint = "WriteHidData")]
        extern static uint WriteHidData(Byte[] data, uint ucTxLength);

        public void Button_Wakeup_Down()
        {
            wakeup_btn = 1;
            SendKey();
        }

        public void Button_Wakeup_Up()
        {
            wakeup_btn = 0;
            SendKey();
        }

        public void ButtonZL_Up()
        {
            L2 = 0;
            SendKey();
        }

        public void ButtonZL_Down()
        {
            L2 = 1;
            SendKey();
        }

        public void ButtonZR_Up()
        {
            R2 = 0;
            SendKey();
        }

        public void ButtonZR_Down()
        {
            R2 = 1;
            SendKey();
        }

        public void ButtonL_Up()
        {
            L = 0;
            SendKey();
        }

        public void ButtonL_Down()
        {
            L = 1;
            SendKey();
        }

        public void ButtonR_Up()
        {
            R = 0;
            SendKey();
        }

        public void ButtonR_Down()
        {
            R = 1;
            SendKey();
        }

        public void ButtonL3_Up()
        {
            L3 = 0;
            SendKey();
        }

        public void ButtonL3_Down()
        {
            L3 = 1;
            SendKey();
        }

        public void ButtonR3_Up()
        {
            R3 = 0;
            SendKey();
        }

        public void ButtonR3_Down()
        {
            R3 = 1;
            SendKey();
        }

        public void ButtonDpadUp_Up()
        {
            Up = 0;
            SendKey();
        }

        public void ButtonDpadUp_Down()
        {
            Up = 1;
            SendKey();
        }

        public void ButtonDpadDown_Up()
        {
            Down = 0;
            SendKey();
        }

        public void ButtonDpadDown_Down()
        {
            Down = 1;
            SendKey();
        }

        public void ButtonDpadLeft_Up()
        {
            Left = 0;
            SendKey();
        }

        public void ButtonDpadLeft_Down()
        {
            Left = 1;
            SendKey();
        }

        public void ButtonDpadRight_Up()
        {
            Right = 0;
            SendKey();
        }

        public void ButtonDpadRight_Down()
        {
            Right = 1;
            SendKey();
        }

        public void ButtonA_Up()
        {
            A = 0;
            SendKey();
        }

        public void ButtonA_Down()
        {
            A = 1;
            SendKey();
        }

        public void ButtonB_Up()
        {
            B = 0;
            SendKey();
        }

        public void ButtonB_Down()
        {
            B = 1;
            SendKey();
        }

        public void ButtonX_Up()
        {
            X = 0;
            SendKey();
        }

        public void ButtonX_Down()
        {
            X = 1;
            SendKey();
        }

        public void ButtonY_Up()
        {
            Y = 0;
            SendKey();
        }

        public void ButtonY_Down()
        {
            Y = 1;
            SendKey();
        }

        public void ButtonMinus_Up()
        {
            Select = 0;
            SendKey();
        }

        public void ButtonMinus_Down()
        {
            Select = 1;
            SendKey();
        }

        public void ButtonPlus_Up()
        {
            Start = 0;
            SendKey();
        }

        public void ButtonPlus_Down()
        {
            Start = 1;
            SendKey();
        }

        public void ButtonHome_Up()
        {
            Home = 0;
            SendKey();
        }

        public void ButtonHome_Down()
        {
            Home = 1;
            SendKey();
        }

        public void ButtonCapture_Up()
        {
            Screenshot = 0;
            SendKey();
        }

        public void ButtonCapture_Down()
        {
            Screenshot = 1;
            SendKey();
        }

        public void ButtonSync_Up()
        {
            Pair = 0;
            SendKey();
        }

        public void ButtonSync_Down()
        {
            Pair = 1;
            SendKey();
        }

        public void rightStick_Stop()
        {
            rx = 127;
            ry = 127;
            SendKey();
        }

        public void leftStick_Stop()
        {
            lx = 127;
            ly = 127;
            SendKey();
        }

        public void rightStick_MoveUp(double percentage)
        {
            if (percentage < 0) { percentage = 0; } else if (percentage > 100) { percentage = 100; }
            rx = 127;
            ry = (int)(127 * (1 - (percentage / 100)));
            SendKey();
        }

        public void rightStick_MoveRight(double percentage)
        {
            if (percentage < 0) { percentage = 0; } else if (percentage > 100) { percentage = 100; }
            rx = 127 + (int)(127 * (percentage / 100));
            ry = 127;
            SendKey();
        }

        public void rightStick_MoveDown(double percentage)
        {
            if (percentage < 0) { percentage = 0; } else if (percentage > 100) { percentage = 100; }
            rx = 127;
            ry = 127 + (int)(127 * (percentage / 100));
            SendKey();
        }

        public void rightStick_MoveLeft(double percentage)
        {
            if (percentage < 0) { percentage = 0; } else if (percentage > 100) { percentage = 100; }
            rx = (int)(127 * (1 - (percentage / 100)));
            ry = 127;
            SendKey();
        }

        public void leftStick_MoveUp(double percentage)
        {
            if (percentage < 0) { percentage = 0; } else if (percentage > 100) { percentage = 100; }
            lx = 127;
            ly = (int)(127 * (1 - (percentage / 100)));
            SendKey();
        }

        public void leftStick_MoveRight(double percentage)
        {
            if (percentage < 0) { percentage = 0; } else if (percentage > 100) { percentage = 100; }
            lx = 127 + (int)(127 * (percentage / 100));
            ly = 127;
            SendKey();
        }

        public void leftStick_MoveDown(double percentage)
        {
            if (percentage < 0) { percentage = 0; } else if (percentage > 100) { percentage = 100; }
            lx = 127;
            ly = 127 + (int)(127 * (percentage / 100));
            SendKey();
        }

        public void leftStick_MoveLeft(double percentage)
        {
            if (percentage < 0) { percentage = 0; } else if (percentage > 100) { percentage = 100; }
            lx = (int)(127 * (1 - (percentage / 100)));
            ly = 127;
            SendKey();
        }

        public void MoveRightStickDiagonal(double percentage, double angleDegrees)
        {
            var (x, y) = CalculateDiagonalPosition(percentage, angleDegrees);
            rx = x;
            ry = y;
            SendKey();
        }

        public void MoveRightStick(int x, int y)
        {
            rx = x;
            ry = y;
            SendKey();
        }

        public void MoveLeftStickDiagonal(double percentage, double angleDegrees)
        {
            var (x, y) = CalculateDiagonalPosition(percentage, angleDegrees);
            lx = x;
            ly = y;
            SendKey();
        }

        public void MoveLeftStick(int x, int y)
        {
            lx = Math.Clamp(x, 0, 254);
            ly = Math.Clamp(y, 0, 254);
            SendKey();
        }


        // Utility method to calculate stick position based on percentage and direction
        private (int x, int y) CalculateDiagonalPosition(double percentage, double angleDegrees)
        {
            if (percentage < 0) percentage = 0;
            else if (percentage > 100) percentage = 100;

            double radians = Math.PI * angleDegrees / 180.0;
            int magnitude = (int)(127 * (percentage / 100));

            int x = 127 + (int)(magnitude * Math.Cos(radians));
            int y = 127 + (int)(magnitude * Math.Sin(radians));

            return (x, y);
        }




        private Wisej.Web.Timer timer;
        public void Form1_Load()
        {
                        Gamepad.SetHidLogger(msg => Debug.WriteLine(msg));

            playerNum = PlayerAssignment.GetAssignedPlayerOrDefault(playerNum);

            timer = new Wisej.Web.Timer
            {
                Interval = 1000,
                Enabled = true
            };
            timer.Tick += new EventHandler(initTimer);
    

        }

        //Data structure for transmission
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct cumstom_header_t
        {
            public UInt16 cmd;
            public UInt16 len;          //The length for current sendding.
            public custom_input_t custom_Input_T;
        }

        //Button data
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct custom_input_t
        {
            public UInt32 keys;
            public UInt16 left_joy;          //The length for current sendding.
            public UInt16 right_joy;
        }
    ;
        //Symbol of whether the device is connected or not
        private int hasdevice = 0;
        #region Timer
        //Continuously get whether the device is connected or not
        public void initTimer(object source, EventArgs e)
        {
            try
            {

                //if (Find24GHid(0x2DC8, 0x9017))
                if (Find24GHid(EBITDO_VID, MICRO_PID, paths[playerNum - 1]))
                {
                    if (hasdevice == 0)
                    {
                        hasdevice = 1;
                        timer.Enabled = false;

                    }
                }
                else
                {
                    if (hasdevice == 1)
                    {
                        hasdevice = 0;
                        rx = 127;
                        ry = 127;
                        lx = 127;
                        ly = 127;
                        timer.Enabled = true;

                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void SendKey()
        {
            // Do NOT call Find24GHid here; rely on connection state from initTimer
            if (hasdevice != 1)
                return;

            if (Open_Hid())
                    {
                        cumstom_header_t header_ptr = new cumstom_header_t();

                        custom_input_t custom_Input_T = new custom_input_t();
                        custom_Input_T.keys = getKey();
                        custom_Input_T.left_joy = (ushort)(lx | ly << 8);
                        custom_Input_T.right_joy = (ushort)(rx | ry << 8);

                        byte[] buffer = new byte[64];

                        header_ptr.cmd = (ushort)CUSTOM_INPUT_CMD_E.CUMTOM_SET_IPNUT_VALUE;
                        header_ptr.len = 8;//length of custom_Input_T
                        header_ptr.custom_Input_T = custom_Input_T;

                        buffer[0] = (byte)0x81;//reportID
                        buffer[1] = (byte)0x06;

                        byte[] header_byte = SHBuffer.StructToBytes(header_ptr);
                        SHBuffer.memcpy(buffer, 2, header_byte, 0, header_byte.Length);
                        WriteHidData(buffer, 64);// Send data
                    }
           
        }

        private UInt32 getKey()
        {
            UInt32 keys = 0;
            if (A == 1)
            {
                keys |= (uint)KeyMap.KeyMap_A;
            }
            if (B == 1)
            {
                keys |= (uint)KeyMap.KeyMap_B;
            }
            if (X == 1)
            {
                keys |= (uint)KeyMap.KeyMap_X;
            }
            if (Y == 1)
            {
                keys |= (uint)KeyMap.KeyMap_Y;
            }
            if (Up == 1)
            {
                keys |= (uint)KeyMap.KeyMap_Up;
            }
            if (Down == 1)
            {
                keys |= (uint)KeyMap.KeyMap_Down;
            }
            if (Left == 1)
            {
                keys |= (uint)KeyMap.KeyMap_Left;
            }
            if (Right == 1)
            {
                keys |= (uint)KeyMap.KeyMap_Right;
            }
            if (L == 1)
            {
                keys |= (uint)KeyMap.KeyMap_L1;
            }
            if (R == 1)
            {
                keys |= (uint)KeyMap.KeyMap_R1;
            }
            if (L2 == 1)
            {
                keys |= (uint)KeyMap.KeyMap_L2;
            }
            if (R2 == 1)
            {
                keys |= (uint)KeyMap.KeyMap_R2;
            }
            if (L3 == 1)
            {
                keys |= (uint)KeyMap.KeyMap_L3;
            }
            if (R3 == 1)
            {
                keys |= (uint)KeyMap.KeyMap_R3;
            }
            if (Select == 1)
            {
                keys |= (uint)KeyMap.KeyMap_Select;
            }
            if (Start == 1)
            {
                keys |= (uint)KeyMap.KeyMap_Start;
            }
            if (Home == 1)
            {
                keys |= (uint)KeyMap.KeyMap_Home;
            }
            if (Screenshot == 1)
            {
                keys |= (uint)KeyMap.KeyMap_Screenshot;
            }
            if (Pair == 1)
            {
                keys |= (uint)KeyMap.KeyMap_Pair;
            }

            if (wakeup_btn == 1)
            {
                keys |= (uint)KeyMap.KeyMap_Wakeup;
            }
            return keys;
        }
        #endregion
    }

        public class PlayerSelectDialog : Wisej.Web.Form
        {
            private Wisej.Web.ComboBox comboPlayers;
            private Wisej.Web.Button btnOk;
            private Wisej.Web.Button btnCancel;

            public int SelectedPlayer { get; private set; }

            public PlayerSelectDialog(int currentPlayer)
            {
                this.Text = "Player Configuration (Currently " + currentPlayer + ")";
            this.Size = new System.Drawing.Size(280, 140);
                this.StartPosition = Wisej.Web.FormStartPosition.CenterParent;
                this.ShowInTaskbar = false;
                this.ControlBox = false;

                comboPlayers = new Wisej.Web.ComboBox()
                {
                    Location = new System.Drawing.Point(20, 20),
                    Width = 220,
                    DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList
                };
                comboPlayers.Items.AddRange(new object[] { "1", "2", "3", "4" });

                // Select current player if valid, otherwise 1
                int index = (currentPlayer >= 1 && currentPlayer <= 4) ? currentPlayer - 1 : 0;
                comboPlayers.SelectedIndex = index;

                btnOk = new Wisej.Web.Button()
                {
                    Text = "OK",
                    DialogResult = Wisej.Web.DialogResult.OK,
                    Location = new System.Drawing.Point(40, 60),
                    Width = 80
                };

                btnCancel = new Wisej.Web.Button()
                {
                    Text = "Cancel",
                    DialogResult = Wisej.Web.DialogResult.Cancel,
                    Location = new System.Drawing.Point(140, 60),
                    Width = 80
                };

                btnOk.Click += (s, e) =>
                {
                    if (comboPlayers.SelectedItem != null &&
                        int.TryParse(comboPlayers.SelectedItem.ToString(), out int num) &&
                        num >= 1 && num <= 4)
                    {
                        SelectedPlayer = num;
                        this.DialogResult = Wisej.Web.DialogResult.OK;
                        this.Close();
                    }
                };

                btnCancel.Click += (s, e) =>
                {
                    this.DialogResult = Wisej.Web.DialogResult.Cancel;
                    this.Close();
                };

                this.Controls.Add(comboPlayers);
                this.Controls.Add(btnOk);
                this.Controls.Add(btnCancel);
            }
        }
}