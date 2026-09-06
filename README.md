# ClassicOverjoyedOpenSource

## Review-only source

This code is provided for review only. It is intended to inspire new features for the new [Overjoyed Accessible Control Hub](https://github.com/antable93/OverjoyedAccessibleControlHub) and is not expected to build easily.

To try a build version, install **Overjoyed Accessible Gaming** from the Microsoft Store.

For more information, visit [getoverjoyed.com](https://getoverjoyed.com) or join our [Discord](https://discord.gg/yUaPn4s5pD)!

## OverjoyedRelease-Local/Active/

The Active directory contains the core runtime behavior and user interaction logic:

- **Active.cs** – Main page class for the active/runtime state; handles initialization and core logic
  - Mouse event handling (down, up, click, double-click, leave)
  - Initialization and startup sequence
  - Main control flow coordination
  
- **Active.AdvancedModes.cs** – Advanced input modes and specialized control schemes
  - FPS mode and look mode handling
  - Advanced motion and analog stick controls
  - Special input processing modes
  
- **Active.Bindings.cs** – Input binding and key mapping configuration
  - PromptBinding class for mapping button inputs to actions
  - Supports toggle, once, hold-until-release, and variable input behaviors
  - Maps keyboard tokens, Xbox tokens, and Switch tokens
  - Handles return-to-center and disabled input flags
  
- **Active.Designer.cs** – UI designer-generated code (form layout)
  - Auto-generated Windows Forms designer code
  - Component initialization and property setup
  
- **Active.Input.Behavior.cs** – Input behavior logic and event handling
  - Core event handling for input triggers
  - Behavior mode evaluation and processing
  
- **Active.Input.CombineControllers.cs** – Multi-controller aggregation and input fusion
  - Detects and processes physical Xbox gamepad button presses
  - Simulates button outputs (A, B, X, Y, triggers, etc.)
  - Handles streamer mode (blocks certain physical inputs)
  - Manages simultaneous input from multiple controllers
  
- **Active.Input.Hotkeys.cs** – Hotkey registration and management
  - Hotkey down/up event handling with debouncing
  - Quadrant hotkey support for combined controller mode
  - Gamepad detection and retry logic
  - Disable clicks hotkey handling
  - Voice input hotkey integration
  
- **Active.Input.MenuControls.cs** – Menu navigation and control
  - Menu-specific input routing
  - Navigation state management
  
- **Active.Input.Remote.cs** – Remote input handling and network communication
  - Remote device communication
  - Network input reception and processing
  
- **Active.Output.cs** – Output routing and result dispatch
  - Mouse movement actions with FPS/look modes
  - Dead zone calculation and angle computation
  - Cursor position tracking and scaling
  - Zoom factor handling for mouse movements
  
- **Active.Output.Switch.cs** – Nintendo Switch output integration
  - Switch-specific button mapping
  - Switch controller simulation
  
- **Active.Output.Xbox.cs** – Xbox controller output integration
  - Xbox gamepad simulation and button control
  - Stick movement handling
  
- **Active.resx** – Resource file for localized strings and assets
  - Embedded strings, icons, and images
  - Localization support
  
- **Active.UI.Core.cs** – Core UI rendering and display logic
  - Reset state and cleanup
  - Toggle state management for Xbox and Switch
  - Input state clearing and initialization
  
- **Active.UI.Helpers.cs** – UI utility functions and helpers
  - Common UI operations
  - State validation helpers
  
- **Active.UI.MenuBar.cs** – Menu bar implementation and rendering
  - Menu bar display and updates
  - Menu item management
  
- **Active.Variables.cs** – Runtime state variables and data storage
  - Global state variables for input processing
  - Configuration caching
  - Toggle and mode flags

## OverjoyedRelease-Local/Config/

The Config directory contains configuration UI and settings management:

- **Config.cs** – Main configuration page; handles settings UI and user preferences
  - Page initialization and loading
  - Notion database integration for game list retrieval
  - Player assignment and messaging
  - Settings page lifecycle management
  
- **Config.Designer.cs** – UI designer-generated code (form layout)
  - Auto-generated Wisej.Web designer code
  - Control initialization and layout
  
- **Config.Profiles.cs** – Profile management and persistence
  - Notion client integration for game database queries
  - Game configuration retrieval and caching
  - Property extraction from Notion database
  - Game name, settings code, contributors, and notes management
  - Support status checking (Yes/No/Checkbox/Select properties)
  - Profile data serialization and deserialization
  
- **Config.resx** – Resource file for localized strings and assets
  - Embedded UI strings and resources
  - Localization support
  
- **Config.Tour.cs** – Guided tour and onboarding UI
  - Tour panel integration (Wisej.Web.Ext.TourPanel)
  - Step-by-step onboarding experience
  - User guidance through configuration
  
- **Config.UI.cs** – Configuration UI components and rendering
  - Control enable/disable logic with exception handling
  - Panel state management
  - Form control state preservation
  - Dynamic UI updates based on configuration
  
- **Config.Variables.cs** – Configuration state variables
  - Configuration state caching
  - Current config instance tracking
  - Runtime variables for settings management
  
- **ConfigClass.cs** – Configuration data model and serialization
  - Configuration data structure definition
  - Serialization/deserialization logic
  - Property mappings and data validation

## SharedVars/

The SharedVars directory contains shared inter-process communication (IPC) and common utilities:

- **SharedVars.cs** – MinimizeMessenger for Wisej server ↔ MAUI client communication
  - Named pipe server listening on "MinimizePipe"
  - Named pipe client connection with retry logic (up to 10 retries, 200ms delay)
  - Async message sending from MAUI to Wisej with error handling
  - Firmware visibility toggle support (`showFirmware` flag)
  - Connection timeout handling (1000ms timeout)
  - Graceful shutdown and cancellation token support
  
- **SharedActive.cs** – ActiveMessenger for Wisej server ↔ MAUI client communication
  - Named pipe server listening on "ActivePipe"
  - Similar retry and timeout logic as MinimizeMessenger
  - Handles active state messages between server and client
  - Operation cancellation with graceful exception handling
  - Debug logging for troubleshooting connection issues
  
- **SharedVars.csproj** – Project file for the SharedVars library
  - Shared class library for cross-process communication
  - Dependency configuration for IPC components

## HTML Components

The application uses embedded HTML resources (stored in .resx resource files) for dynamic UI rendering:

### Active Page HTML Resources:
- **quadrantsSVG.Html** – SVG visualization of input quadrants
  - Displays visual quadrant layout for Disable Clicks mode
  - Shows input zones and state indicators
  
- **pnlBaseOverlay.Html** – Main base overlay HTML panel
  - Primary rendering surface for the active page
  - Contains main UI layout and interactive elements
  
- **combineDiagram.Html** – Controller combination diagram
  - Visual representation of combined controller state
  - Displays multi-controller input aggregation

### GuidedTour HTML:
- **HtmlText** (in GuidedTour.Designer.cs)
  - Rendered HTML content for guided tour steps
  - Tour panel integration via Wisej.Web.Ext.TourPanel
  - Step-by-step onboarding instructions
  - Scrollable non-scrollbar rendering

### External HTML Reference:
- **OBS.html** – External link to https://overjoyed.vercel.app/OBS.html
  - OBS (Open Broadcaster Software) integration guide
  - Opened when users access OBS settings from the application

