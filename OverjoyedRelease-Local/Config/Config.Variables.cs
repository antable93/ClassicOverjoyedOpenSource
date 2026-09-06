using ExtendedMessageBoxLibrary;
using GregsStack.InputSimulatorStandard.Native;
using Microsoft.Maui.ApplicationModel;
using SharedVars;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using Wisej.Web;
using Microsoft.Maui.Storage;


namespace OverjoyedReleaseLocal
{
    public partial class Config : Wisej.Web.Page
    {
        private Panel backupPanel = new Panel() { Left = 50, Top = 200, Width = 500, Height = 400, BorderStyle = BorderStyle.Double, BackColor = System.Drawing.Color.LightGray, MaximumSize = new Size(500, 400), MinimumSize = new Size(500, 400) };

        private ComboBox comboBackups = new ComboBox() { Left = 20, Top = 20, Width = 300, DropDownStyle = ComboBoxStyle.DropDownList, Height = 30, DropDownWidth = 320, MinimumSize = new Size(320, 30), MaximumSize = new Size(320, 30), Font = new Font("default", 10, FontStyle.Regular) };
        private Button btnRestore = new Button() { Left = 350, Top = 20, Width = 130, Text = "Restore Backup", MaximumSize = new Size(130, 30), MinimumSize = new Size(130, 30), Height = 30, Font = new Font("default", 10, FontStyle.Regular) };
        private TextBox txtLog = new TextBox() { Left = 20, Top = 60, Width = 460, Height = 280, Multiline = true, ScrollBars = ScrollBars.Vertical, MaximumSize = new Size(460, 280), MinimumSize = new Size(460, 280), Font = new Font("default", 10, FontStyle.Regular) };
        private string backupRoot;

        private readonly string[] excludedProfiles = { "Default", "DefaultMusic" };
        private static Config _currentConfig;
        public static Config CurrentInstance => _currentConfig;
        private bool advFPS = false;
        private bool advCombine = false;
        private bool advDisableClicks = false;
        private bool advAltRTC = false;
        private bool advMarioKart = false;
        private bool advVoice = false;

        private static int GetAssignedPlayerNum()
        {
            return PlayerAssignment.GetAssignedPlayerOrDefault();
        }

        private bool advKeepMouse = false;
        private bool advMouseLock = false;
        private bool advPSbuttons = false;
        private string currentTab = "";
        private string currentLang = "en";

        private bool tourEnd = false;
        private bool initialTour = true;
        public string configPath = Path.Combine(FileSystem.Current.AppDataDirectory, "configs");
        public string profilesPath = Path.Combine(FileSystem.Current.AppDataDirectory, "profiles");
        public string parametersFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, "configs", "parameters.txt");
        public string tourFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, "configs", "Tour.txt");
        public string appFolderPath = Path.GetDirectoryName(AppContext.BaseDirectory);
        public static List<string> settingsCodes = new List<string>();
        public static List<string> gameList = new List<string>();
        public static List<string> gameNames = new List<string>();
        public static List<string> gameNotes = new List<string>();

        private string actionType = "";

        public int repeat = 1;
        public bool disMinimize = false;

        string keyboardAction, xboxAction, switchAction;

        public Placement alignment;
        public Placement topAlignment;
        public double zoomFactor;
        public GuidedTour window;
        public GuidedTour window2;

        int start = 0;
        string[] directions = new string[33];
        string[][] buttonConfig = new string[33][];

        string configName;
        private Dictionary<Wisej.Web.Control, bool> originalEnabledStates = new();
        private bool isLoadingProfile = false;
        private bool isRefreshingDisableButtons = false;
        private bool isApplyingDisableButtonState = false;
        private int lastProfileIndex = -1;

        [DllImport("user32.dll")]
        private static extern short GetKeyState(int keyCode);

        Control[] keyboardButtons;

        double browserHeight;

        List<string> nonVariable = new List<string> { "DPad Up", "DPad Down", "DPad Left", "DPad Right", "Menu or Start",
            "View or Select", "LSB or L3", "RSB or R3", "LB or L1", "RB or R1",
             "Guide", "None","A or X", "B or O", "X or ⬜", "Y or ꕔ" };

        List<string> nonVariableSwitch = new List<string> { "DPad Up", "DPad Down", "DPad Left", "DPad Right", "Plus (Start)",
            "Minus (Select)", "Home","Capture", "L3 Button", "R3 Button", "L Button", "R Button",
             "None","A Button", "B Button", "X Button", "Y Button",  "ZL Button", "ZR Button" };

        string[] buttonOptions = new string[31] { "None","A or X", "B or O", "X or ⬜", "Y or ꕔ",
            "LB or L1","LT or L2", "LSB or L3", "RB or R1","RT or R2","RSB or R3",
            "LS Left", "LS Right", "LS Up", "LS Down",
            "RS Left", "RS Right", "RS Up", "RS Down","Menu or Start", "View or Select",
           "Guide","Guide", "DPad Up", "DPad Down", "DPad Left", "DPad Right", "Up-Right","Up-Left","Down-Right","Down-Left" };

        string[] switchButtonOptions = new string[31] { "None","A Button", "B Button", "X Button", "Y Button",
            "L Button","ZL Button", "L3 Button", "R Button","ZR Button","R3 Button",
            "LS Left", "LS Right", "LS Up", "LS Down",
            "RS Left", "RS Right", "RS Up", "RS Down","Plus (Start)",
            "Minus (Select)", "Home","Capture", "DPad Up", "DPad Down", "DPad Left", "DPad Right", "Up-Right","Up-Left","Down-Right","Down-Left" };

        List<string> keyboardOptions = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12", "Tab", "CapsLk", "Shift", "Ctrl", "Alt", "Space", "Backspace", "Enter", "Esc", "LCtrl", "Backtick", "Minus", "Equal", "Backspace", "RCtrl", "Home", "PageUp", "PageDown", "End", "Insert", "Pause", "Scroll", "Delete", "Enter", "LBracket", "RBracket", "Backslash", "Semicolon", "Quote", "LShift", "RShift", "Comma", "Period", "Slash", "Up", "Down", "Left", "Right", "Win", "LAlt", "RAlt", "Space", "Divide", "Clear", "Up-Right", "Up-Left", "Down-Right", "Down-Left" };

        string optDisable = "Disable Quadrant";
        string optToggle = "Toggle On and Off";
        string optVariable = "Variable Triggers / Sticks";
        string optHold = "Hold Until Click Released";
        string optOnce = "Press Button Once";
        string optActivate = "Click to Activate";
        string optRTC = "Return to Center On Click";

        List<string> defaultKB = new List<string> { "W", "E", "D", "T", "S", "R", "A", "Q", "Space", "Space", "Space", "Space", "Space", "Space", "Space", "Space", "X", "X", "X", "X", "X", "X", "X", "X", "Z", "Space", "X", "Z", "Space", "X", "Z", "Space", "X" };

        List<string> defaultXbox = new List<string> { "LS Up", "LS Right", "LS Right", "LS Right", "LS Down", "LS Left", "LS Left", "LS Left", "A or X", "A or X", "A or X", "A or X", "A or X", "A or X", "A or X", "A or X", "RS Up", "RS Right", "RS Right", "RS Right", "RS Down", "RS Left", "RS Left", "RS Left", "LSB or L3", "B or O", "Y or ꕔ", "RSB or R3", "A or X", "X or ⬜", "Menu or Start", "LB or L1", "RB or R1" };

        List<string> defaultSwitch = new List<string> { "LS Up", "LS Right", "LS Right", "LS Right", "LS Down", "LS Left", "LS Left", "LS Left", "A Button", "A Button", "A Button", "A Button", "A Button", "A Button", "A Button", "A Button", "RS Up", "RS Right", "RS Right", "RS Right", "RS Down", "RS Left", "RS Left", "RS Left", "L3 Button", "B Button", "Y Button", "R3 Button", "A Button", "X Button", "Plus (Start)", "L Button", "R Button" };

        string configReader2;

        bool defaultConfig = false;
        bool defaultMusic = false;


    }
}