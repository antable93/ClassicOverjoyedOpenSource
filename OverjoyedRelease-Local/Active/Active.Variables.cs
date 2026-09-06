using GregsStack.InputSimulatorStandard;
using GregsStack.InputSimulatorStandard.Native;
using ExtendedMessageBoxLibrary;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using SharedVars;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Wisej.Web;
using static OverjoyedReleaseLocal.DictionaryEnums;


namespace OverjoyedReleaseLocal
{
    public partial class Active : Page
    {
        public int truncateSize = 12;
        private int initWidth = 600;
        private string lastUpperFill = "none";
        private string lastMiddleFill = "none";
        private string lastLowerFill = "none";
        private string previousMessage;
        private CancellationTokenSource _cancellationTokenSource;
        private FileSystemWatcher _toggleWatcher;
        private bool webfuseBridgeInjected;
        public static bool toggleRemote = false;
        public bool recentering = false;
        private bool suppressRemoteCheckedChanged = false;

        private static Active _currentInstance; // Holds the current instance
        public static Active CurrentInstance => _currentInstance;

        private double oldWindowWidth = 600; // Store the old window width


        private bool initialAltTab = true; // Flag to track if the Alt+Tab has been triggered once

        public bool safeZone = false; // Flag to track if the safe zone is active

        private IntPtr matchedWindowHandle = IntPtr.Zero; // Store the matched window handle

        public int dz = 0;

        // Delegate for EnumWindows
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        // Import EnumWindows from user32.dll
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        // Import GetWindowText to retrieve window titles
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        public int previousQuadrant = 0;
        public int prevRemoteX = 0;
        public int prevRemoteY = 0;
        public string prevAction = "none";
        public int remoteX = 0;
        public int remoteY = 0;
        public int remoteXoverlay = 0;
        public int remoteYoverlay = 0;
        public string action = "none";
        public bool localLeftPointerDown = false;
        public bool localRightPointerDown = false;
        public bool remoteLeftPointerDown = false;
        public bool remoteRightPointerDown = false;
        public long outboundPointerPacketSequence = 0;
        public long lastReceivedPointerPacketSequence = -1;
        public long lastRemoteStatusLabelUpdateTick = 0;
        public bool tailscale = false;
        public bool positionCheck = false;
        public bool remotePositionCheck = false;
        public int webfuseCount = 0;
        public bool remoteActive = false;
        private ComboBox cmbNintendoStreamerPlayer;
        private Button btnNintendoStreamerX;
        private Button btnNintendoStreamerY;
        private CheckBox chkMoveMouseUsingLeftClicks;
        private CheckBox chkAltLeftClick;
        private CheckBox chkAltRightClick;
        private bool suppressNintendoStreamerPlayerChanged;
        private int lastObsNintendoRoutePlayer;
        int stateCounter = 0;
        public bool autoCombine = true;
        public bool keyboardCombine = false;
        public XINPUT_STATE statePrev;
        private bool leftTriggerPressed = false;
        private bool rightTriggerPressed = false;
        private bool leftTriggerDown = false;
        private bool rightTriggerDown = false;
        private bool leftStickXMoved = false;
        private bool rightStickXMoved = false;
        private bool leftStickYMoved = false;
        private bool rightStickYMoved = false;

        private int lastMouseX;
        private int lastMouseY;
        private bool lastMouseValid;
        private bool scanned = false;
        [StructLayout(LayoutKind.Sequential)]
        public struct XINPUT_GAMEPAD
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
        public struct XINPUT_STATE
        {
            public uint dwPacketNumber;
            public XINPUT_GAMEPAD Gamepad;
        }

        [DllImport("xinput1_4.dll")]
        private static extern int XInputGetState(int dwUserIndex, out XINPUT_STATE pState);

        private int selectedController = -1;
        private bool scanning = false;
        private const int GamepadPollingIntervalMs = 4;

        public string configPath = Path.Combine(FileSystem.Current.AppDataDirectory, "configs");
        public string profilesPath = Path.Combine(FileSystem.Current.AppDataDirectory, "profiles");
        public string parametersFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, "configs", "parameters.txt");

        //public bool isLeftMouseButtonDown = false;
        public bool psLabels = false;
        public bool rtcDraw = false;
        private DateTime lastLclickTime = DateTime.MinValue;
        private DateTime lastRclickTime = DateTime.MinValue;
        private DateTime lastLeftMouseDownAt = DateTime.MinValue;
        private DateTime lastRightMouseDownAt = DateTime.MinValue;
        private CancellationTokenSource altMouseDeactivateHoldCts;
        private const int AltMouseDeactivateHoldMs = 2000;
        private volatile bool rightLookModeLatched;
        private volatile bool fpsDeadZoneLookModeLatched;
        private int fpsDeadZoneCenterEntryCount;
        private const int MinimumPointerGlowMs = 200;
        private DateTime suppressBaseOverlayDoubleClickUntil = DateTime.MinValue;
        private volatile bool syntheticLeftMouseButtonHeld;
        private volatile bool syntheticRightMouseButtonHeld;

        private enum ClickThreadMode
        {
            Keyboard,
            Xbox,
            Switch
        }

        private enum InputActionBehavior
        {
            HoldUntilRelease,
            Once,
            Toggle,
            Variable
        }

        private ConcurrentBag<string> toggleXbox = new ConcurrentBag<string>();
        private ConcurrentBag<string> toggleXboxFPS = new ConcurrentBag<string>();
        readonly private List<string> releaseSwitchHover = new List<string>();
        readonly private List<string> releaseXboxHover = new List<string>();
        readonly private List<VirtualKeyCode> releaseKBHover = new List<VirtualKeyCode>();
        readonly private List<VirtualKeyCode> releaseKBClick = new List<VirtualKeyCode>();
        readonly private List<string> releaseSwitchClick = new List<string>();
        readonly private List<string> releaseXboxClick = new List<string>();

        // Declare variables to track the double-click timing and button state
        private DateTime _lastClickTime = DateTime.MinValue;
        private MouseButtons _lastButtonClicked;
        private const int DoubleClickThreshold = 500; // Milliseconds


        public int currentQuadrant = 0;
        public int currentQuadrant2 = -1;
        public int prevQuadrant = 0;

        private readonly object mouseMoveQueueSync = new object();
        private bool mouseMoveWorkerRunning;
        private bool mouseMoveQueued;
        private bool queuedMouseMoveFps;
        private bool queuedMouseMoveLookMode;

        int modeCounter = 0;
        string previousMode, newMode;


        public Gamepad switchGamepad = new Gamepad();


        readonly private FileSystemWatcher watcher;
        readonly private FileSystemWatcher watcher2;


        public double density;

        //public PictureBox[] pictureBox = new PictureBox[16];
        //public Label[] lblPrompts = new Label[3];

        public static bool foo3 = false;
        public static bool settingsClosed = false;
        public bool clickFocus = false;

        //private RateLimiter rateLimiter = new RateLimiter(2, TimeSpan.FromSeconds(2)); // 10 calls per second

        public string appFolderPath;

        public int mousePosX;
        public int mousePosY;

        public double jscriptmousePosX;
        public double jscriptmousePosY;

        //Variable Speed
        public float setspeed, setspeed2;
        public int clicked = 0;
        public int unclicked = 1;
        public int unclicked2;
        public List<Int32> music = new List<Int32>();
        public int keyOrder = 8;
        public int keyOrder2 = 7;
        public bool musicMode = false;
        public bool first = true;
        public bool xboxMode;
        public bool switchMode;
        public bool keyboardMode;
        int buttonMap;

        bool diagUpRightHover, diagUpLeftHover, diagDownRightHover, diagDownLeftHover;
        bool diagUpRightLC, diagUpLeftLC, diagDownRightLC, diagDownLeftLC;
        bool diagUpRightRC, diagUpLeftRC, diagDownRightRC, diagDownLeftRC;



        public float scaleDZ;

        public double zoomFactor;

        public bool exit = false;

        private bool showLabelsHover, showLabelsLC, showLabelsRC;

        private bool ActivateUpper = false;

        private bool ActivateMiddle = false;

        private bool ActivateLower = false;

        readonly List<List<string>> buttonConfig = new List<List<string>>(
             Enumerable.Range(0, 100)
                       .Select(_ => new List<string> { "d", "false", "false", "rs right", "false", "true", "rs right" })
                       .ToList()
         );

        readonly List<List<string>> kbButtons = new List<List<string>>(
            Enumerable.Range(0, 100)
                      .Select(_ => new List<string> { "D", "False", "False", "RS Right", "False", "True", "RS Right" })
                      .ToList()
        );

        int[] v;

        //private LowLevelKeyboardListener _listener; //Keyboard Hook, DONE

        public double X;
        public double Y;
        public double Z;
        public double Yaw;
        public double Pitch;
        public double Roll;

        public double browserWidth = 600;

        public double formWidth = 600;

        //Settings booleans
        private bool isActive = false;

        //Vector variables
        public float xStart = 300;
        public float yStart = 350;
        public float xEnd, yEnd; //DONE
        public float xEnd2, yEnd2; //DONE

        public bool SafeZone = false;

        //Needs config
        private float deadZone = 60f;

        InputSimulator inputSimulator = new InputSimulator(); //Input Simulator , DONE

        public List<VirtualKeyCode> keyCodes = new List<VirtualKeyCode>(); //DON

        private ConcurrentBag<string> toggleSwitch = new ConcurrentBag<string>();
        private readonly ConcurrentDictionary<VirtualKeyCode, int> keyboardHeldCounts = new ConcurrentDictionary<VirtualKeyCode, int>();

        List<bool> activateToggle = new List<bool>(new bool[100]);

        List<bool> buttonPressed = new List<bool>(new bool[100]);

        string[] buttons = new string[12];

        List<string> xbuttons = new List<string>(Enumerable.Repeat("RS Right", 100));

        List<string> switchbuttons = new List<string>(Enumerable.Repeat("RS Right", 100));


        int GamepadMouseX1, GamepadMouseY1, GamepadMouseX2, GamepadMouseY2;

        int oldQuadrant = 0;

        int[] oncePressed = new int[33] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        string[] buttonOptions = new string[26] { "DPad Up", "DPad Down", "DPad Left", "DPad Right", "Menu or Start",
            "View or Select", "LSB or L3", "RSB or R3", "LB or L1", "RB or R1",
             "Guide", "None","A or X", "B or O", "X or ⬜", "Y or ꕔ",  "LT or L2",
            "RT or R2",  "LS Left", "LS Right", "LS Up", "LS Down", "RS Left", "RS Right", "RS Up", "RS Down" };



        List<string> nonVariable = new List<string> {"DPad Up", "DPad Down", "DPad Left", "DPad Right", "Menu or Start",
            "View or Select", "LSB or L3", "RSB or R3", "LB or L1", "RB or R1",
             "Guide", "None","A or X", "B or O", "X or ⬜", "Y or ꕔ" };



        int b0, b1, b2, c1, c2, c3;

        //Drawing Variables
        Pen penA = new Pen(Color.FromArgb(255, 255, 255, 255));
        Pen penB = new Pen(Color.FromArgb(255, 0, 167, 209));
        Pen penC = new Pen(Color.FromArgb(255, 255, 0, 0));

        // Track the state of buttons
        private bool buttonAPressed = false;
        private bool buttonBPressed = false;
        private bool buttonXPressed = false;
        private bool buttonYPressed = false;
        private bool leftBumperPressed = false;
        private bool rightBumperPressed = false;
        private bool backPressed = false;
        private bool startPressed = false;
        private bool leftStickPressed = false;
        private bool rightStickPressed = false;
        private bool dpadUpPressed = false;
        private bool dpadDownPressed = false;
        private bool dpadLeftPressed = false;
        private bool dpadRightPressed = false;
        private bool guidePressed = false;

        public int playerNum = 1;

        public bool specialMode = false;

      
    }

}