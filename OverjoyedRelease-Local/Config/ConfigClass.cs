using System.Collections.Generic;
using Wisej.Web;
using GregsStack.InputSimulatorStandard.Native;
using static OverjoyedReleaseLocal.DictionaryEnums;
using System.Globalization;
using System.ComponentModel;

namespace OverjoyedReleaseLocal
{
    partial class Config
    {
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Name2 { get; set; }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<VirtualKeyCode> KeyCodes { get; set; }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsVector { get; set; }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float XStart { get; set; }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float YStart { get; set; }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float DeadZone { get; set; }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool RtcLC { get; set; }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool RtcRC { get; set; }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SwitchConfig { get; set; }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Config AltConfig { get; set; }

        public Config(string n)
        {
            Name2 = n;
            KeyCodes.Add(VirtualKeyCode.VK_W);
            KeyCodes.Add(VirtualKeyCode.VK_S);
            KeyCodes.Add(VirtualKeyCode.VK_A);
            KeyCodes.Add(VirtualKeyCode.VK_D);
            KeyCodes.Add(VirtualKeyCode.VK_Z);
            KeyCodes.Add(VirtualKeyCode.VK_X);
            IsVector = true;
            XStart = Screen.Bounds.Width / 2;
            YStart = Screen.Bounds.Height / 2;
            DeadZone = 100f;
            RtcLC = false;
            RtcRC = false;
        }

        public Config(string n, List<string> lst)
        {

            Name2 = n;
            VirtualKeyCode code;
            foreach (string s in lst)
            {
                strToKey.TryGetValue(s, out code);
                KeyCodes.Add(code);
            }
            IsVector = true;
            XStart = Screen.Bounds.Width / 2;
            YStart = Screen.Bounds.Height / 2;
            DeadZone = 100f;
            RtcLC = false;
            RtcRC = false;
        }

        public Config(string n, List<string> lst, bool iV)
        {
            Name2 = n;
            VirtualKeyCode code;
            foreach (string s in lst)
            {
                strToKey.TryGetValue(s, out code);
                KeyCodes.Add(code);
            }
            IsVector = iV;
            XStart = Screen.Bounds.Width / 2;
            YStart = Screen.Bounds.Height / 2;
            DeadZone = 100f;
            RtcLC = false;
            RtcRC = false;
        }

        public Config(string n, List<string> lst, bool iV, float xS, float yS, float dZ)
        {
            Name2 = n;
            VirtualKeyCode code;
            foreach (string s in lst)
            {
                strToKey.TryGetValue(s, out code);
                KeyCodes.Add(code);
            }
            IsVector = iV;
            XStart = xS;
            YStart = yS;
            DeadZone = dZ;
            RtcLC = false;
            RtcRC = false;
        }

        public Config(string n, List<string> lst, bool iV, float xS, float yS, float dZ, bool l, bool r)
        {
            Name2 = n;
            VirtualKeyCode code;
            foreach (string s in lst)
            {
                strToKey.TryGetValue(s, out code);
                KeyCodes.Add(code);
            }
            IsVector = iV;
            XStart = xS;
            YStart = yS;
            DeadZone = dZ;
            RtcLC = l;
            RtcRC = r;
        }

        public Config(string n, List<string> lst, bool iV, float xS, float yS, float dZ, bool l, bool r, Config c)
        {
            Name2 = n;
            VirtualKeyCode code;
            foreach (string s in lst)
            {
                strToKey.TryGetValue(s, out code);
                KeyCodes.Add(code);
            }
            IsVector = iV;
            XStart = xS;
            YStart = yS;
            DeadZone = dZ;
            RtcLC = l;
            RtcRC = r;
            SwitchConfig = true;
            AltConfig = c;
        }

    }
}
