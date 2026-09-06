using ExtendedMessageBoxLibrary;
using GregsStack.InputSimulatorStandard.Native;
using Microsoft.Maui.ApplicationModel;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Wisej.Web;


namespace OverjoyedReleaseLocal
{
    public partial class Active : Page
    {
		// Detect button presses and simulate the corresponding button press/release
		private async Task DetectButtons(XINPUT_GAMEPAD gamepad)
		{
			await Task.Run(() =>
			{
				bool isStreamerModeConfigured = false;
				try
				{
					string streamerModePath = Path.Combine(configPath, "StreamerMode.txt");
					if (File.Exists(streamerModePath))
					{
						string streamerModeValue = File.ReadAllText(streamerModePath);
						isStreamerModeConfigured = !string.IsNullOrWhiteSpace(streamerModeValue);
					}
				}
				catch
				{
					isStreamerModeConfigured = false;
				}

				bool ShouldPassPhysicalXboxButton(string token)
				{
					if (!isStreamerModeConfigured)
					{
						return true;
					}

					return !xbuttons.Contains(token);
				}

				bool ShouldPassPhysicalSwitchButton(string token)
				{
					if (!isStreamerModeConfigured)
					{
						return true;
					}

					return !switchbuttons.Contains(token);
				}

				// A button
				if ((gamepad.wButtons & XInputConstants.XINPUT_GAMEPAD_A) != 0) // Button A pressed
				{
					if (!buttonAPressed)
					{
						bool wasIgnored = false;
						buttonAPressed = true;
						if (chkCombineControllers.Checked)
						{
							if (xboxMode)
							{
								if (ShouldPassPhysicalXboxButton("A or X"))
								{
									SimGamePad.Instance.SetControl(12, 1); // 1 corresponds to Button A
								}
								else
								{
									ShowIgnoredPhysicalInput("A button");
									wasIgnored = true;
								}
							}
							else if (switchMode)
							{
								if (ShouldPassPhysicalSwitchButton("A Button"))
								{
									switchGamepad.ButtonA_Down();
								}
								else
								{
									ShowIgnoredPhysicalInput("A button");
									wasIgnored = true;
								}
							}
						}

						if (!wasIgnored)
						{
							lblCombine.Invoke(new Action(() => lblCombine.Text = "A button pressed"));
						}
					}
				}
				else if (buttonAPressed) // Button A released
				{
					bool wasPassed = true;
					buttonAPressed = false;
					if (chkCombineControllers.Checked)
					{
						if (xboxMode)
						{
							if (ShouldPassPhysicalXboxButton("A or X"))
							{
								SimGamePad.Instance.ReleaseControl(12, toggleXbox, 1); // 1 corresponds to Button A
							}
							else
							{
								wasPassed = false;
							}
						}
						else if (switchMode)
						{
							if (ShouldPassPhysicalSwitchButton("A Button"))
							{
								switchGamepad.ButtonA_Up();
							}
							else
							{
								wasPassed = false;
							}
						}
					}

					if (wasPassed)
					{
						lblCombine.Invoke(new Action(() => lblCombine.Text = "A button released"));
					}
				}

				// B button
				if ((gamepad.wButtons & XInputConstants.XINPUT_GAMEPAD_B) != 0) // Button B pressed
				{
					if (!buttonBPressed)
					{
						bool wasIgnored = false;
						buttonBPressed = true;
						if (chkCombineControllers.Checked)
						{
							if (xboxMode)
							{
								if (ShouldPassPhysicalXboxButton("B or O"))
								{
									SimGamePad.Instance.SetControl(13, 1); // 2 corresponds to Button B
								}
								else
								{
									ShowIgnoredPhysicalInput("B button");
									wasIgnored = true;
								}
							}
							else if (switchMode)
							{
								if (ShouldPassPhysicalSwitchButton("B Button"))
								{
									switchGamepad.ButtonB_Down();
								}
								else
								{
									ShowIgnoredPhysicalInput("B button");
									wasIgnored = true;
								}
							}
						}

						if (!wasIgnored)
						{
							lblCombine.Invoke(new Action(() => lblCombine.Text = "B button pressed"));
						}
					}
				}
				else if (buttonBPressed) // Button B released
				{
					bool wasPassed = true;
					buttonBPressed = false;
					if (chkCombineControllers.Checked)
					{
						if (xboxMode)
						{
							if (ShouldPassPhysicalXboxButton("B or O"))
							{
								SimGamePad.Instance.ReleaseControl(13, toggleXbox, 1); // 2 corresponds to Button B
							}
							else
							{
								wasPassed = false;
							}
						}
						else if (switchMode)
						{
							if (ShouldPassPhysicalSwitchButton("B Button"))
							{
								switchGamepad.ButtonB_Up();
							}
							else
							{
								wasPassed = false;
							}
						}
					}

					if (wasPassed)
					{
						lblCombine.Invoke(new Action(() => lblCombine.Text = "B button released"));
					}
				}

				// X button
				if ((gamepad.wButtons & XInputConstants.XINPUT_GAMEPAD_X) != 0) // Button X pressed
				{
					if (!buttonXPressed)
					{
						bool wasIgnored = false;
						buttonXPressed = true;
						if (chkCombineControllers.Checked)
						{
							if (xboxMode)
							{
								if (ShouldPassPhysicalXboxButton("X or ⬜"))
								{
									SimGamePad.Instance.SetControl(14, 1); // 3 corresponds to Button X
								}
								else
								{
									ShowIgnoredPhysicalInput("X button");
									wasIgnored = true;
								}
							}
							else if (switchMode)
							{
								if (ShouldPassPhysicalSwitchButton("X Button"))
								{
									switchGamepad.ButtonX_Down();
								}
								else
								{
									ShowIgnoredPhysicalInput("X button");
									wasIgnored = true;
								}
							}
						}

						if (!wasIgnored)
						{
							lblCombine.Invoke(new Action(() => lblCombine.Text = "X button pressed"));
						}
					}
				}
				else if (buttonXPressed) // Button X released
				{
					bool wasPassed = true;
					buttonXPressed = false;
					if (chkCombineControllers.Checked)
					{
						if (xboxMode)
						{
							if (ShouldPassPhysicalXboxButton("X or ⬜"))
							{
								SimGamePad.Instance.ReleaseControl(14, toggleXbox, 1); // 3 corresponds to Button X
							}
							else
							{
								wasPassed = false;
							}
						}
						else if (switchMode)
						{
							if (ShouldPassPhysicalSwitchButton("X Button"))
							{
								switchGamepad.ButtonX_Up();
							}
							else
							{
								wasPassed = false;
							}
						}
					}

					if (wasPassed)
					{
						lblCombine.Invoke(new Action(() => lblCombine.Text = "X button released"));
					}
				}

				// Y button
				if ((gamepad.wButtons & XInputConstants.XINPUT_GAMEPAD_Y) != 0) // Button Y pressed
				{
					if (!buttonYPressed)
					{
						bool wasIgnored = false;
						buttonYPressed = true;
						if (chkCombineControllers.Checked)
						{
							if (xboxMode)
							{
								if (ShouldPassPhysicalXboxButton("Y or ꕔ"))
								{
									SimGamePad.Instance.SetControl(15, 1); // 4 corresponds to Button Y
								}
								else
								{
									ShowIgnoredPhysicalInput("Y button");
									wasIgnored = true;
								}
							}
							else if (switchMode)
							{
								if (ShouldPassPhysicalSwitchButton("Y Button"))
								{
									switchGamepad.ButtonY_Down();
								}
								else
								{
									ShowIgnoredPhysicalInput("Y button");
									wasIgnored = true;
								}
							}
						}

						if (!wasIgnored)
						{
							lblCombine.Invoke(new Action(() => lblCombine.Text = "Y button pressed"));
						}
					}
				}
				else if (buttonYPressed) // Button Y released
				{
					bool wasPassed = true;
					buttonYPressed = false;
					if (chkCombineControllers.Checked)
					{
						if (xboxMode)
						{
							if (ShouldPassPhysicalXboxButton("Y or ꕔ"))
							{
								SimGamePad.Instance.ReleaseControl(15, toggleXbox, 1); // 4 corresponds to Button Y
							}
							else
							{
								wasPassed = false;
							}
						}
						else if (switchMode)
						{
							if (ShouldPassPhysicalSwitchButton("Y Button"))
							{
								switchGamepad.ButtonY_Up();
							}
							else
							{
								wasPassed = false;
							}
						}
					}

					if (wasPassed)
					{
						lblCombine.Invoke(new Action(() => lblCombine.Text = "Y button released"));
					}
				}

				// Left Bumper
				if ((gamepad.wButtons & XInputConstants.XINPUT_GAMEPAD_LEFT_SHOULDER) != 0) // Left Bumper pressed
				{
					if (!leftBumperPressed)
					{
						bool wasIgnored = false;
						leftBumperPressed = true;
						if (chkCombineControllers.Checked)
						{
							if (xboxMode)
							{
								if (ShouldPassPhysicalXboxButton("LB or L1"))
								{
									SimGamePad.Instance.SetControl(8, 1); // 5 corresponds to Left Bumper
									lblCombine.Invoke(new Action(() => lblCombine.Text = "Left Bumper pressed"));
								}
								else
								{
									ShowIgnoredPhysicalInput("Left Bumper");
									wasIgnored = true;
								}
							}
							else if (switchMode)
							{
								if (ShouldPassPhysicalSwitchButton("L Button"))
								{
									switchGamepad.ButtonL_Down();
									lblCombine.Invoke(new Action(() => lblCombine.Text = "L button pressed"));
								}
								else
								{
									ShowIgnoredPhysicalInput("L button");
									wasIgnored = true;
								}
							}
						}

						if (!chkCombineControllers.Checked && !wasIgnored)
						{
							lblCombine.Invoke(new Action(() => lblCombine.Text = "Left Bumper pressed"));
						}
					}
				}
				else if (leftBumperPressed) // Left Bumper released
				{
					bool wasPassed = true;
					leftBumperPressed = false;
					if (chkCombineControllers.Checked)
					{
						if (xboxMode)
						{
							if (ShouldPassPhysicalXboxButton("LB or L1"))
							{
								SimGamePad.Instance.ReleaseControl(8, toggleXbox, 1); // 5 corresponds to Left Bumper
							}
							else
							{
								wasPassed = false;
							}
						}
						else if (switchMode)
						{
							if (ShouldPassPhysicalSwitchButton("L Button"))
							{
								switchGamepad.ButtonL_Up();
							}
							else
							{
								wasPassed = false;
							}
							if (wasPassed)
							{
								lblCombine.Invoke(new Action(() => lblCombine.Text = "L button released"));
							}
						}
						if (xboxMode && wasPassed)
						{
							lblCombine.Invoke(new Action(() => lblCombine.Text = "Left Bumper released"));
						}
					}

				}

				// Right Bumper
				if ((gamepad.wButtons & XInputConstants.XINPUT_GAMEPAD_RIGHT_SHOULDER) != 0) // Right Bumper pressed
				{
					if (!rightBumperPressed)
					{
						bool wasIgnored = false;
						rightBumperPressed = true;
						if (chkCombineControllers.Checked)
						{
							if (xboxMode)
							{
								if (ShouldPassPhysicalXboxButton("RB or R1"))
								{
									SimGamePad.Instance.SetControl(9, 1); // 6 corresponds to Right Bumper
									lblCombine.Invoke(new Action(() => lblCombine.Text = "Right Bumper pressed"));
								}
								else
								{
									ShowIgnoredPhysicalInput("Right Bumper");
									wasIgnored = true;
								}
							}
							else if (switchMode)
							{
								if (ShouldPassPhysicalSwitchButton("R Button"))
								{
									switchGamepad.ButtonR_Down();
									lblCombine.Invoke(new Action(() => lblCombine.Text = "R button pressed"));
								}
								else
								{
									ShowIgnoredPhysicalInput("R button");
									wasIgnored = true;
								}
							}
						}

						if (!chkCombineControllers.Checked && !wasIgnored)
						{
							lblCombine.Invoke(new Action(() => lblCombine.Text = "Right Bumper pressed"));
						}
					}
				}
				else if (rightBumperPressed) // Right Bumper released
				{
					bool wasPassed = true;
					rightBumperPressed = false;
					if (chkCombineControllers.Checked)
					{
						if (xboxMode)
						{
							if (ShouldPassPhysicalXboxButton("RB or R1"))
							{
								SimGamePad.Instance.ReleaseControl(9, toggleXbox, 1); // 6 corresponds to Right Bumper
							}
							else
							{
								wasPassed = false;
							}
						}
						else if (switchMode)
						{
							if (ShouldPassPhysicalSwitchButton("R Button"))
							{
								switchGamepad.ButtonR_Up();
							}
							else
							{
								wasPassed = false;
							}
							if (wasPassed)
							{
								lblCombine.Invoke(new Action(() => lblCombine.Text = "R button released"));
							}
						}
						if (xboxMode && wasPassed)
						{
							lblCombine.Invoke(new Action(() => lblCombine.Text = "Right Bumper released"));
						}
					}

				}

				// Back button
				if ((gamepad.wButtons & XInputConstants.XINPUT_GAMEPAD_BACK) != 0) // Back button pressed
				{
					if (!backPressed)
					{
						bool wasIgnored = false;
						backPressed = true;
						if (chkCombineControllers.Checked)
						{
							if (xboxMode)
							{
								if (ShouldPassPhysicalXboxButton("View or Select"))
								{
									SimGamePad.Instance.SetControl(5, 1); // 7 corresponds to Back button
								}
								else
								{
									ShowIgnoredPhysicalInput("Select button");
									wasIgnored = true;
								}
							}
							else if (switchMode)
							{
								if (ShouldPassPhysicalSwitchButton("Minus (Select)"))
								{
									switchGamepad.ButtonMinus_Down();
								}
								else
								{
									ShowIgnoredPhysicalInput("Select button");
									wasIgnored = true;
								}
							}
						}

						if (!wasIgnored)
						{
							lblCombine.Invoke(new Action(() => lblCombine.Text = "Select button pressed"));
						}
					}
				}
				else if (backPressed) // Back button released
				{
					bool wasPassed = true;
					backPressed = false;
					if (chkCombineControllers.Checked)
					{
						if (xboxMode)
						{
							if (ShouldPassPhysicalXboxButton("View or Select"))
							{
								SimGamePad.Instance.ReleaseControl(5, toggleXbox, 1); // 7 corresponds to Back button
							}
							else
							{
								wasPassed = false;
							}
						}
						else if (switchMode)
						{
							if (ShouldPassPhysicalSwitchButton("Minus (Select)"))
							{
								switchGamepad.ButtonMinus_Up();
							}
							else
							{
								wasPassed = false;
							}
						}
					}

					if (wasPassed)
					{
						lblCombine.Invoke(new Action(() => lblCombine.Text = "Select button released"));
					}
				}

				// Start button
				if ((gamepad.wButtons & XInputConstants.XINPUT_GAMEPAD_START) != 0) // Start button pressed
				{
					if (!startPressed)
					{
						bool wasIgnored = false;
						startPressed = true;
						if (chkCombineControllers.Checked)
						{
							if (xboxMode)
							{
								if (ShouldPassPhysicalXboxButton("Menu or Start"))
								{
									SimGamePad.Instance.SetControl(4, 1); // 8 corresponds to Start button
								}
								else
								{
									ShowIgnoredPhysicalInput("Start button");
									wasIgnored = true;
								}
							}
							else if (switchMode)
							{
								if (ShouldPassPhysicalSwitchButton("Plus (Start)"))
								{
									switchGamepad.ButtonPlus_Down();
								}
								else
								{
									ShowIgnoredPhysicalInput("Start button");
									wasIgnored = true;
								}
							}
						}
						if (!wasIgnored)
						{
							lblCombine.Invoke(new Action(() => lblCombine.Text = "Start button pressed"));
						}
					}
				}
				else if (startPressed) // Start button released
				{
					bool wasPassed = true;
					startPressed = false;
					if (chkCombineControllers.Checked)
					{
						if (xboxMode)
						{
							if (ShouldPassPhysicalXboxButton("Menu or Start"))
							{
								SimGamePad.Instance.ReleaseControl(4, toggleXbox, 1); // 8 corresponds to Start button
							}
							else
							{
								wasPassed = false;
							}
						}
						else if (switchMode)
						{
							if (ShouldPassPhysicalSwitchButton("Plus (Start)"))
							{
								switchGamepad.ButtonPlus_Up();
							}
							else
							{
								wasPassed = false;
							}
						}
					}

					if (wasPassed)
					{
						lblCombine.Invoke(new Action(() => lblCombine.Text = "Start button released"));
					}
				}

				// Left Stick Click
				if ((gamepad.wButtons & XInputConstants.XINPUT_GAMEPAD_LEFT_THUMB) != 0) // Left Stick Click pressed
				{
					if (!leftStickPressed)
					{
						bool wasIgnored = false;
						leftStickPressed = true;
						if (chkCombineControllers.Checked)
						{
							if (xboxMode)
							{
								if (ShouldPassPhysicalXboxButton("LSB or L3"))
								{
									SimGamePad.Instance.SetControl(6, 1); // 9 corresponds to Left Stick Click
									lblCombine.Invoke(new Action(() => lblCombine.Text = "Left Stick Click pressed"));
								}
								else
								{
									ShowIgnoredPhysicalInput("Left Stick Click");
									wasIgnored = true;
								}
							}
							else if (switchMode)
							{
								if (ShouldPassPhysicalSwitchButton("L3 Button"))
								{
									switchGamepad.ButtonL3_Down();
									lblCombine.Invoke(new Action(() => lblCombine.Text = "L3 button pressed"));
								}
								else
								{
									ShowIgnoredPhysicalInput("L3 button");
									wasIgnored = true;
								}
							}
						}

						if (!chkCombineControllers.Checked && !wasIgnored)
						{
							lblCombine.Invoke(new Action(() => lblCombine.Text = "Left Stick Click pressed"));
						}
					}
				}
				else if (leftStickPressed) // Left Stick Click released
				{
					bool wasPassed = true;
					leftStickPressed = false;
					if (chkCombineControllers.Checked)
					{
						if (xboxMode)
						{
							if (ShouldPassPhysicalXboxButton("LSB or L3"))
							{
								SimGamePad.Instance.ReleaseControl(6, toggleXbox, 1); // 9 corresponds to Left Stick Click
							}
							else
							{
								wasPassed = false;
							}
							lblCombine.Invoke(new Action(() => lblCombine.Text = "Left Stick Click released"));
						}
						else if (switchMode)
						{
							if (ShouldPassPhysicalSwitchButton("L3 Button"))
							{
								switchGamepad.ButtonL3_Up();
							}
							else
							{
								wasPassed = false;
							}
							lblCombine.Invoke(new Action(() => lblCombine.Text = "L3 button released"));
						}
					}

					if (!chkCombineControllers.Checked && wasPassed)
					{
						lblCombine.Invoke(new Action(() => lblCombine.Text = "Left Stick Click released"));
					}

				}

				// Right Stick Click
				if ((gamepad.wButtons & XInputConstants.XINPUT_GAMEPAD_RIGHT_THUMB) != 0) // Right Stick Click pressed
				{
					if (!rightStickPressed)
					{
						bool wasIgnored = false;
						rightStickPressed = true;
						if (chkCombineControllers.Checked)
						{
							if (xboxMode)
							{
								if (ShouldPassPhysicalXboxButton("RSB or R3"))
								{
									SimGamePad.Instance.SetControl(7, 1); // 10 corresponds to Right Stick Click
									lblCombine.Invoke(new Action(() => lblCombine.Text = "Right Stick Click pressed"));
								}
								else
								{
									ShowIgnoredPhysicalInput("Right Stick Click");
									wasIgnored = true;
								}

							}
							else if (switchMode)
							{
								if (ShouldPassPhysicalSwitchButton("R3 Button"))
								{
									switchGamepad.ButtonR3_Down();
									lblCombine.Invoke(new Action(() => lblCombine.Text = "R3 button pressed"));
								}
								else
								{
									ShowIgnoredPhysicalInput("R3 button");
									wasIgnored = true;
								}
							}
						}

						if (!chkCombineControllers.Checked && !wasIgnored)
						{
							lblCombine.Invoke(new Action(() => lblCombine.Text = "Right Stick Click pressed"));
						}
					}
				}
				else if (rightStickPressed) // Right Stick Click released
				{
					bool wasPassed = true;
					rightStickPressed = false;
					if (chkCombineControllers.Checked)
					{
						if (xboxMode)
						{
							if (ShouldPassPhysicalXboxButton("RSB or R3"))
							{
								SimGamePad.Instance.ReleaseControl(7, toggleXbox, 1); // 10 corresponds to Right Stick Click
							}
							else
							{
								wasPassed = false;
							}
							lblCombine.Invoke(new Action(() => lblCombine.Text = "Right Stick Click released"));

						}
						else if (switchMode)
						{
							if (ShouldPassPhysicalSwitchButton("R3 Button"))
							{
								switchGamepad.ButtonR3_Up();
							}
							else
							{
								wasPassed = false;
							}
							lblCombine.Invoke(new Action(() => lblCombine.Text = "R3 button released"));
						}
					}

					if (!chkCombineControllers.Checked && wasPassed)
					{
						lblCombine.Invoke(new Action(() => lblCombine.Text = "Right Stick Click released"));
					}

				}

				// D-Pad buttons
				if ((gamepad.wButtons & XInputConstants.XINPUT_GAMEPAD_DPAD_UP) != 0) // D-Pad Up pressed
				{
					if (!dpadUpPressed)
					{
						bool wasIgnored = false;
						dpadUpPressed = true;
						if (chkCombineControllers.Checked)
						{
							if (xboxMode)
							{
								if (ShouldPassPhysicalXboxButton("DPad Up"))
								{
									SimGamePad.Instance.SetControl(0, 1); // 11 corresponds to D-Pad Up
								}
								else
								{
									ShowIgnoredPhysicalInput("D-Pad Up");
									wasIgnored = true;
								}
							}
							else if (switchMode)
							{
								if (ShouldPassPhysicalSwitchButton("DPad Up"))
								{
									switchGamepad.ButtonDpadUp_Down();
								}
								else
								{
									ShowIgnoredPhysicalInput("D-Pad Up");
									wasIgnored = true;
								}
							}
						}

						if (!wasIgnored)
						{
							lblCombine.Invoke(new Action(() => lblCombine.Text = "D-Pad Up pressed"));
						}
					}
				}
				else if (dpadUpPressed) // D-Pad Up released
				{
					bool wasPassed = true;
					dpadUpPressed = false;
					if (chkCombineControllers.Checked)
					{
						if (xboxMode)
						{
							if (ShouldPassPhysicalXboxButton("DPad Up"))
							{
								SimGamePad.Instance.ReleaseControl(0, toggleXbox, 1); // 11 corresponds to D-Pad Up
							}
							else
							{
								wasPassed = false;
							}
						}
						else if (switchMode)
						{
							if (ShouldPassPhysicalSwitchButton("DPad Up"))
							{
								switchGamepad.ButtonDpadUp_Up();
							}
							else
							{
								wasPassed = false;
							}
						}
					}

					if (wasPassed)
					{
						lblCombine.Invoke(new Action(() => lblCombine.Text = "D-Pad Up released"));
					}
				}

				if ((gamepad.wButtons & XInputConstants.XINPUT_GAMEPAD_DPAD_DOWN) != 0) // D-Pad Down pressed
				{
					if (!dpadDownPressed)
					{
						bool wasIgnored = false;
						dpadDownPressed = true;
						if (chkCombineControllers.Checked)
						{
							if (xboxMode)
							{
								if (ShouldPassPhysicalXboxButton("DPad Down"))
								{
									SimGamePad.Instance.SetControl(1, 1); // 12 corresponds to D-Pad Down
								}
								else
								{
									ShowIgnoredPhysicalInput("D-Pad Down");
									wasIgnored = true;
								}
							}
							else if (switchMode)
							{
								if (ShouldPassPhysicalSwitchButton("DPad Down"))
								{
									switchGamepad.ButtonDpadDown_Down();
								}
								else
								{
									ShowIgnoredPhysicalInput("D-Pad Down");
									wasIgnored = true;
								}
							}
						}

						if (!wasIgnored)
						{
							lblCombine.Invoke(new Action(() => lblCombine.Text = "D-Pad Down pressed"));
						}
					}
				}
				else if (dpadDownPressed) // D-Pad Down released
				{
					bool wasPassed = true;
					dpadDownPressed = false;
					if (chkCombineControllers.Checked)
					{
						if (xboxMode)
						{
							if (ShouldPassPhysicalXboxButton("DPad Down"))
							{
								SimGamePad.Instance.ReleaseControl(1, toggleXbox, 1); // 12 corresponds to D-Pad Down
							}
							else
							{
								wasPassed = false;
							}
						}
						else if (switchMode)
						{
							if (ShouldPassPhysicalSwitchButton("DPad Down"))
							{
								switchGamepad.ButtonDpadDown_Up();
							}
							else
							{
								wasPassed = false;
							}
						}
					}

					if (wasPassed)
					{
						lblCombine.Invoke(new Action(() => lblCombine.Text = "D-Pad Down released"));
					}
				}

				if ((gamepad.wButtons & XInputConstants.XINPUT_GAMEPAD_DPAD_LEFT) != 0) // D-Pad Left pressed
				{
					if (!dpadLeftPressed)
					{
						bool wasIgnored = false;
						dpadLeftPressed = true;
						if (chkCombineControllers.Checked)
						{
							if (xboxMode)
							{
								if (ShouldPassPhysicalXboxButton("DPad Left"))
								{
									SimGamePad.Instance.SetControl(2, 1); // 13 corresponds to D-Pad Left
								}
								else
								{
									ShowIgnoredPhysicalInput("D-Pad Left");
									wasIgnored = true;
								}
							}
							else if (switchMode)
							{
								if (ShouldPassPhysicalSwitchButton("DPad Left"))
								{
									switchGamepad.ButtonDpadLeft_Down();
								}
								else
								{
									ShowIgnoredPhysicalInput("D-Pad Left");
									wasIgnored = true;
								}
							}
						}
						if (!wasIgnored)
						{
							lblCombine.Invoke(new Action(() => lblCombine.Text = "D-Pad Left pressed"));
						}
					}
				}
				else if (dpadLeftPressed) // D-Pad Left released
				{
					bool wasPassed = true;
					dpadLeftPressed = false;
					if (chkCombineControllers.Checked)
					{
						if (xboxMode)
						{
							if (ShouldPassPhysicalXboxButton("DPad Left"))
							{
								SimGamePad.Instance.ReleaseControl(2, toggleXbox, 1); // 13 corresponds to D-Pad Left
							}
							else
							{
								wasPassed = false;
							}
						}
						else if (switchMode)
						{
							if (ShouldPassPhysicalSwitchButton("DPad Left"))
							{
								switchGamepad.ButtonDpadLeft_Up();
							}
							else
							{
								wasPassed = false;
							}
						}
					}

					if (wasPassed)
					{
						lblCombine.Invoke(new Action(() => lblCombine.Text = "D-Pad Left released"));
					}
				}

				if ((gamepad.wButtons & XInputConstants.XINPUT_GAMEPAD_DPAD_RIGHT) != 0) // D-Pad Right pressed
				{
					if (!dpadRightPressed)
					{
						bool wasIgnored = false;
						dpadRightPressed = true;
						if (chkCombineControllers.Checked)
						{
							if (xboxMode)
							{
								if (ShouldPassPhysicalXboxButton("DPad Right"))
								{
									SimGamePad.Instance.SetControl(3, 1); // 14 corresponds to D-Pad Right
								}
								else
								{
									ShowIgnoredPhysicalInput("D-Pad Right");
									wasIgnored = true;
								}
							}
							else if (switchMode)
							{
								if (ShouldPassPhysicalSwitchButton("DPad Right"))
								{
									switchGamepad.ButtonDpadRight_Down();
								}
								else
								{
									ShowIgnoredPhysicalInput("D-Pad Right");
									wasIgnored = true;
								}
							}
						}
						if (!wasIgnored)
						{
							lblCombine.Invoke(new Action(() => lblCombine.Text = "D-Pad Right pressed"));
						}
					}
				}
				else if (dpadRightPressed) // D-Pad Right released
				{
					bool wasPassed = true;
					dpadRightPressed = false;
					if (chkCombineControllers.Checked)
					{
						if (xboxMode)
						{
							if (ShouldPassPhysicalXboxButton("DPad Right"))
							{
								SimGamePad.Instance.ReleaseControl(3, toggleXbox, 1); // 14 corresponds to D-Pad Right
							}
							else
							{
								wasPassed = false;
							}
						}
						else if (switchMode)
						{
							if (ShouldPassPhysicalSwitchButton("DPad Right"))
							{
								switchGamepad.ButtonDpadRight_Up();
							}
							else
							{
								wasPassed = false;
							}
						}
					}

					if (wasPassed)
					{
						lblCombine.Invoke(new Action(() => lblCombine.Text = "D-Pad Right released"));
					}
				}


				// Guide Button
				if ((gamepad.wButtons & XInputConstants.XINPUT_GAMEPAD_GUIDE) != 0) // Guide button pressed
				{
					if (!guidePressed)
					{
						bool wasIgnored = false;
						guidePressed = true;
						if (chkCombineControllers.Checked)
						{
							if (xboxMode)
							{
								if (ShouldPassPhysicalXboxButton("Guide"))
								{
									SimGamePad.Instance.SetControl(10, 1); // 15 corresponds to Guide button
									lblCombine.Invoke(new Action(() => lblCombine.Text = "Guide button pressed"));
								}
								else
								{
									ShowIgnoredPhysicalInput("Guide button");
									wasIgnored = true;
								}
							}
							else if (switchMode)
							{
								if (ShouldPassPhysicalSwitchButton("Home"))
								{
									switchGamepad.ButtonHome_Down();
									lblCombine.Invoke(new Action(() => lblCombine.Text = "Home button pressed"));
								}
								else
								{
									ShowIgnoredPhysicalInput("Home button");
									wasIgnored = true;
								}
							}
						}

						if (!chkCombineControllers.Checked && !wasIgnored)
						{
							lblCombine.Invoke(new Action(() => lblCombine.Text = "Guide button pressed"));
						}

					}
				}
				else if (guidePressed) // Guide button released
				{
					bool wasPassed = true;
					guidePressed = false;
					if (chkCombineControllers.Checked)
					{
						if (xboxMode)
						{
							if (ShouldPassPhysicalXboxButton("Guide"))
							{
								SimGamePad.Instance.ReleaseControl(10, toggleXbox, 1); // 15 corresponds to Guide button
							}
							else
							{
								wasPassed = false;
							}
							lblCombine.Invoke(new Action(() => lblCombine.Text = "Guide button released"));

						}
						else if (switchMode)
						{
							if (ShouldPassPhysicalSwitchButton("Home"))
							{
								switchGamepad.ButtonHome_Up();
							}
							else
							{
								wasPassed = false;
							}
							lblCombine.Invoke(new Action(() => lblCombine.Text = "Home button released"));

						}
					}

					if (!chkCombineControllers.Checked && wasPassed)
					{
						lblCombine.Invoke(new Action(() => lblCombine.Text = "Guide button released"));
					}


				}
			});
		}



		private void StartGamepadDetection()
		{



			if (!scanning)
			{
				scanned = false;
				scanning = true;
				lblCombine.Visible = false;
				lblCombine.Text = "Gamepad detection started. Press A button on your controller.";
				SetLblActiveInactiveUi("Gamepad detection started. Press A button on your controller.");
				ApplyActivationAvailabilityGate();


				// Run the ScanGamepads method on a separate thread.
				Task.Run(() => ScanGamepads());

			}
		}

		private void ShowIgnoredPhysicalInput(string buttonName)
		{
			string target = xboxMode ? "gamepad" : switchMode ? "switch" : "output";
			lblCombine.Invoke(new Action(() => lblCombine.Text = $"{buttonName} ignored by {target}"));
		}

		public static class XInputConstants
		{
			public const int XINPUT_GAMEPAD_A = 0x1000;
			public const int XINPUT_GAMEPAD_B = 0x2000;
			public const int XINPUT_GAMEPAD_X = 0x4000;
			public const int XINPUT_GAMEPAD_Y = 0x8000;
			public const int XINPUT_GAMEPAD_LEFT_SHOULDER = 0x0100;
			public const int XINPUT_GAMEPAD_RIGHT_SHOULDER = 0x0200;
			public const int XINPUT_GAMEPAD_BACK = 0x0020;
			public const int XINPUT_GAMEPAD_START = 0x0010;
			public const int XINPUT_GAMEPAD_LEFT_THUMB = 0x0040;
			public const int XINPUT_GAMEPAD_RIGHT_THUMB = 0x0080;
			public const int XINPUT_GAMEPAD_DPAD_UP = 0x0001;
			public const int XINPUT_GAMEPAD_DPAD_DOWN = 0x0002;
			public const int XINPUT_GAMEPAD_DPAD_LEFT = 0x0004;
			public const int XINPUT_GAMEPAD_DPAD_RIGHT = 0x0008;

			[StructLayout(LayoutKind.Sequential)]
			public struct XINPUT_STATE_EX
			{
				public uint dwPacketNumber;
				public XINPUT_GAMEPAD Gamepad;
				public byte GuideButton; // Extra byte for Guide button (Unofficial)
			}

			// Import the undocumented function
			[DllImport("xinput1_4.dll", EntryPoint = "#100")]
			private static extern int XInputGetStateEx(int dwUserIndex, out XINPUT_STATE_EX pState);

			public const int ERROR_SUCCESS = 0;
			public const int XINPUT_GAMEPAD_GUIDE = 0x0400; // Custom bit flag
		}

		private void ScanGamepads()
		{
			while (scanning || scanned)
			{
				if (selectedController == -1) // No controller selected yet
				{
					bool controllerSelected = false;
					for (int i = 0; i < 4; i++) // XInput supports up to 4 controllers
					{
						if (!scanning)
						{
							return;
						}

						XINPUT_STATE state;
						if (XInputGetState(i, out state) == 0) // Controller is connected
						{
							if (state.Gamepad.wButtons != 0) // If any button is pressed
							{
								selectedController = i;
								statePrev = state;
								scanning = false; // Stop scanning once a controller is selected
								scanned = true; // Set flag to check button presses
								this.Invoke(new Action(() =>
								{
									resetTransparency(false, true);

									ExtendedMessageBox.Show($"<p style='margin:0;padding:0;line-height:1.6;'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 1; -webkit-box-orient: vertical; overflow: hidden; font-size:" + truncateSize + "pt; ' title='Gamepad successfully detected!'>Gamepad successfully detected!</span></p>", "Gamepad Detected", ExtendedMessageBoxLibrary.MessageBoxButtons.OK, lines: 1, zoomFactor: zoomFactor);
									resetTransparency(true, true);
									lblCombine.Visible = true;
									ApplyActivationAvailabilityGate();
									if (isActive)
									{
										SetLblActiveVisibleUi(false);
									}
								}));
								lblCombine.Invoke(new Action(() => lblCombine.Text = "Gamepad successfully detected!"));
								if (!isActive)
								{
									this.Invoke(new Action(() => SetLblActiveInactiveUi("Overjoyed is inactive. Hold Click Here to activate!")));
								}
								controllerSelected = true;
								break;
							}
						}
					}

					if (!controllerSelected)
					{
						Thread.Sleep(GamepadPollingIntervalMs);
					}

					continue;
				}

				if (!scanning && scanned)
				{
					XINPUT_STATE state;
					if (XInputGetState(selectedController, out state) == 0)
					{
						if (state.dwPacketNumber == statePrev.dwPacketNumber)
						{
							Thread.Sleep(GamepadPollingIntervalMs);
							continue;
						}

						var prev = statePrev;
						statePrev = state; // <-- UPDATE FIRST

						if (state.Gamepad.wButtons != prev.Gamepad.wButtons) // If any button is pressed
						{
							Task.Run(() => DetectButtons(state.Gamepad));
						}
						if (state.Gamepad.bLeftTrigger > 0) { leftTriggerPressed = true; }
						if (xboxMode)
						{
							bool allowPhysicalLeftTrigger = chkCombineControllers.Checked && !chkCombineLT.Checked;
							if (state.Gamepad.bLeftTrigger != prev.Gamepad.bLeftTrigger && leftTriggerPressed)
							{

								Task.Run(() => DetectAnalogTriggers(state.Gamepad, "left"));


							}
							else if (state.Gamepad.bLeftTrigger == 0 && leftTriggerPressed)
							{
								if (allowPhysicalLeftTrigger)
								{
									SimGamePad.Instance.VariableTriggers(0, 0);
								}
								leftTriggerPressed = false;
								lblCombine.Invoke(new Action(() => lblCombine.Text = "Left Trigger released"));

							}
						}

						if (switchMode)
						{
							bool allowPhysicalLeftTrigger = chkCombineControllers.Checked && !chkCombineLT.Checked;
							if (state.Gamepad.bLeftTrigger != 0 && leftTriggerPressed && leftTriggerDown == false)
							{

								if (allowPhysicalLeftTrigger)
								{
									switchGamepad.ButtonZL_Down();
									leftTriggerDown = true;
									lblCombine.Invoke(new Action(() => lblCombine.Text = "ZL button pressed"));
								}
								else
								{
									ShowIgnoredPhysicalInput("ZL button");
								}

							}
							else if (state.Gamepad.bLeftTrigger == 0 && leftTriggerPressed)
							{

								if (leftTriggerDown)
								{
									switchGamepad.ButtonZL_Up();
								}
								leftTriggerPressed = false;
								lblCombine.Invoke(new Action(() => lblCombine.Text = "ZL button released"));
								leftTriggerDown = false;

							}
						}

						if (state.Gamepad.bRightTrigger > 0) { rightTriggerPressed = true; }
						if (xboxMode)
						{
							bool allowPhysicalRightTrigger = chkCombineControllers.Checked && !chkCombineRT.Checked;
							if (state.Gamepad.bRightTrigger != prev.Gamepad.bRightTrigger && rightTriggerPressed)
							{

								Task.Run(() => DetectAnalogTriggers(state.Gamepad, "right"));

							}
							else if (state.Gamepad.bRightTrigger == 0 && rightTriggerPressed)
							{

								if (allowPhysicalRightTrigger)
								{
									SimGamePad.Instance.VariableTriggers(1, 0);
								}
								rightTriggerPressed = false;
								lblCombine.Invoke(new Action(() => lblCombine.Text = "Right Trigger released"));

							}
						}
						if (switchMode)
						{
							bool allowPhysicalRightTrigger = chkCombineControllers.Checked && !chkCombineRT.Checked;
							if (state.Gamepad.bRightTrigger != 0 && rightTriggerPressed && rightTriggerDown == false)
							{

								if (allowPhysicalRightTrigger)
								{
									switchGamepad.ButtonZR_Down();
									rightTriggerDown = true;
									lblCombine.Invoke(new Action(() => lblCombine.Text = "ZR button pressed"));
								}
								else
								{
									ShowIgnoredPhysicalInput("ZR button");
								}

							}
							else if (state.Gamepad.bRightTrigger == 0 && rightTriggerPressed)
							{
								if (rightTriggerDown)
								{
									switchGamepad.ButtonZR_Up();
								}
								rightTriggerPressed = false;
								lblCombine.Invoke(new Action(() => lblCombine.Text = "ZR button released"));
								rightTriggerDown = false;

							}
						}




						if (state.Gamepad.sThumbLX <= -8000 || state.Gamepad.sThumbLX >= 8000) { leftStickXMoved = true; }
						if (state.Gamepad.sThumbLY <= -8000 || state.Gamepad.sThumbLY >= 8000) { leftStickYMoved = true; }
						if ((state.Gamepad.sThumbLX != prev.Gamepad.sThumbLX && leftStickXMoved) || (state.Gamepad.sThumbLY != prev.Gamepad.sThumbLY && leftStickYMoved))
						{
							stateCounter++;
							Task.Run(() => DetectThumbsticks(state.Gamepad, "left"));

						}
						bool leftStickXCentered = leftStickXMoved && state.Gamepad.sThumbLX > -8000 && state.Gamepad.sThumbLX < 8000;
						bool leftStickYCentered = leftStickYMoved && state.Gamepad.sThumbLY > -8000 && state.Gamepad.sThumbLY < 8000;
						if (leftStickXCentered || leftStickYCentered)
						{
							bool allowPhysicalLeftStick = chkCombineControllers.Checked && !chkCombineLS.Checked;

							if (leftStickXCentered) leftStickXMoved = false;
							if (leftStickYCentered) leftStickYMoved = false;

							if (allowPhysicalLeftStick)
							{
								if (xboxMode)
								{
									SimGamePad.Instance.MoveSticks(0, 0, false);
									SimGamePad.Instance.MoveSticks(1, 0, false);
								}
								else if (switchMode)
								{
									switchGamepad.leftStick_Stop();
								}
							}
							lblCombine.Invoke(new Action(() => lblCombine.Text = "Left Stick released"));
						}




						if (state.Gamepad.sThumbRX <= -8000 || state.Gamepad.sThumbRX >= 8000) { rightStickXMoved = true; }
						if (state.Gamepad.sThumbRY <= -8000 || state.Gamepad.sThumbRY >= 8000) { rightStickYMoved = true; }
						if ((state.Gamepad.sThumbRX != prev.Gamepad.sThumbRX && rightStickXMoved) || (state.Gamepad.sThumbRY != prev.Gamepad.sThumbRY && rightStickYMoved))
						{
							stateCounter++;
							Task.Run(() => DetectThumbsticks(state.Gamepad, "right"));

						}
						if ((rightStickXMoved && state.Gamepad.sThumbRX > -8000 && state.Gamepad.sThumbRX < 8000) || (rightStickYMoved && state.Gamepad.sThumbRY > -8000 && state.Gamepad.sThumbRY < 8000))
						{
							bool allowPhysicalRightStick = chkCombineControllers.Checked && !chkCombineRS.Checked;

							if (rightStickXMoved && state.Gamepad.sThumbRX > -8000 && state.Gamepad.sThumbRX < 8000) rightStickXMoved = false;
							if (rightStickYMoved && state.Gamepad.sThumbRY > -8000 && state.Gamepad.sThumbRY < 8000) rightStickYMoved = false;

							if (allowPhysicalRightStick)
							{
								if (xboxMode)
								{
									SimGamePad.Instance.MoveSticks(2, 0, false);
									SimGamePad.Instance.MoveSticks(3, 0, false);
								}
								else if (switchMode)
								{
									switchGamepad.rightStick_Stop();
								}
							}
							lblCombine.Invoke(new Action(() => lblCombine.Text = "Right Stick released"));
						}


					}
					else
					{
						scanned = false; // Reset scanned flag
						scanning = false; // Keep scanning stopped until explicitly started again
						this.Invoke(new Action(() =>
						{
							chkCombineControllers.Checked = false;
							scanned = false;
							ApplyActivationAvailabilityGate();
							if (!isActive)
							{
								SetLblActiveVisibleUi(true);
								SetLblActiveInactiveUi("Overjoyed is inactive. Hold Click Here to activate!");
							}
							resetTransparency(false, true);
							ExtendedMessageBox.Show($"<p style='margin:0;padding:0;line-height:1.6;'><span class='truncate' style='display: -webkit-box; -webkit-line-clamp: 3; -webkit-box-orient: vertical; overflow: hidden; font-size:" + truncateSize + "pt; ' title='Gamepad disconnected. Recheck 'Combine Inputs from Other Inputs' under Advanced to reconnect.'>Gamepad disconnected. Recheck 'Combine Inputs from Other Inputs' under Advanced to reconnect.</span></p>", "Gamepad Disconnected", ExtendedMessageBoxLibrary.MessageBoxButtons.OK, lines: 3, zoomFactor: zoomFactor);
							resetTransparency(true, true);
						}));
						lblCombine.Invoke(new Action(() => lblCombine.Text = "Gamepad disconnected. Recheck 'Combine Inputs from Other Inputs' under Advanced to reconnect."));

						selectedController = -1; // Reset selection
						return;
					}

					Thread.Sleep(GamepadPollingIntervalMs);
				}
			}


		}


		private async Task DetectThumbsticks(XINPUT_GAMEPAD gamepad, string direction)
		{

			await Task.Run(() =>
			{


				if (direction == "left")
				{
					if (chkCombineLS.Checked)
					{
						ShowIgnoredPhysicalInput("Left Stick");
						return;
					}

					const double deadzone = 8000.0;
					const double maxVal = 32767.0;
					double rawX = gamepad.sThumbLX;
					double rawY = gamepad.sThumbLY;
					double leftX = rawX > deadzone ? (rawX - deadzone) / (maxVal - deadzone)
								 : rawX < -deadzone ? (rawX + deadzone) / (maxVal - deadzone) : 0.0;
					double leftY = rawY > deadzone ? (rawY - deadzone) / (maxVal - deadzone)
								 : rawY < -deadzone ? (rawY + deadzone) / (maxVal - deadzone) : 0.0;

					lblCombine.Invoke(new Action(() => lblCombine.Text = $"LX: {gamepad.sThumbLX}, LY: {gamepad.sThumbLY}, leftX: {leftX:F2}, leftY: {leftY:F2}"));

					if (chkCombineControllers.Checked)
					{
						if (xboxMode)
						{
							SimGamePad.Instance.MoveSticks(0, Math.Abs(leftX), leftX < 0);
							SimGamePad.Instance.MoveSticks(1, Math.Abs(leftY), leftY < 0);
						}
						else if (switchMode)
						{
							// Map deadzone-remapped value to Switch (0..254), Y inverted.
							int lxVal = (int)(127 + leftX * 127);
							int lyVal = (int)(127 - leftY * 127);
							switchGamepad.MoveLeftStick(lxVal, lyVal);
						}
					}

				}
				else if (direction == "right")
				{
					if (chkCombineRS.Checked)
					{
						ShowIgnoredPhysicalInput("Right Stick");
						return;
					}

					const double deadzone = 8000.0;
					const double maxVal = 32767.0;
					double rawX = gamepad.sThumbRX;
					double rawY = gamepad.sThumbRY;
					double rightX = rawX > deadzone ? (rawX - deadzone) / (maxVal - deadzone)
								  : rawX < -deadzone ? (rawX + deadzone) / (maxVal - deadzone) : 0.0;
					double rightY = rawY > deadzone ? (rawY - deadzone) / (maxVal - deadzone)
								  : rawY < -deadzone ? (rawY + deadzone) / (maxVal - deadzone) : 0.0;

					lblCombine.Invoke(new Action(() => lblCombine.Text = $"RX: {gamepad.sThumbRX}, RY: {gamepad.sThumbRY}, rightX: {rightX:F2}, rightY: {rightY:F2}"));

					if (chkCombineControllers.Checked)
					{
						if (xboxMode)
						{
							SimGamePad.Instance.MoveSticks(2, Math.Abs(rightX), rightX < 0);
							SimGamePad.Instance.MoveSticks(3, Math.Abs(rightY), rightY < 0);
						}
						else if (switchMode)
						{
							// Map deadzone-remapped value to Switch (0..254), Y inverted.
							int rxVal = (int)(127 + rightX * 127);
							int ryVal = (int)(127 - rightY * 127);
							switchGamepad.MoveRightStick(rxVal, ryVal);
						}
					}

				}
			});
		}



		private async Task DetectAnalogTriggers(XINPUT_GAMEPAD gamepad, string direction)
		{
			await Task.Run(() =>
			{
				if (direction == "left")
				{
					if (chkCombineLT.Checked)
					{
						ShowIgnoredPhysicalInput("Left Trigger");
						return;
					}

					// Left trigger detection
					double leftTriggerValue = gamepad.bLeftTrigger / 255.0;


					lblCombine.Invoke(new Action(() => lblCombine.Text = $"Left Trigger: {gamepad.bLeftTrigger / 255}%"));
					if (chkCombineControllers.Checked && !chkCombineLT.Checked)
					{
						SimGamePad.Instance.VariableTriggers(
								0,
								leftTriggerValue
							);
					}
					else if (chkCombineControllers.Checked)
					{
						ShowIgnoredPhysicalInput("Left Trigger");
					}

				}

				else if (direction == "right")
				{
					if (chkCombineRT.Checked)
					{
						ShowIgnoredPhysicalInput("Right Trigger");
						return;
					}


					// Right trigger detection
					double rightTriggerValue = gamepad.bRightTrigger / 255.0;
					lblCombine.Invoke(new Action(() => lblCombine.Text = $"Right Trigger: {gamepad.bRightTrigger / 255}%"));
					if (xboxMode && chkCombineControllers.Checked && !chkCombineRT.Checked)
					{
						SimGamePad.Instance.VariableTriggers(
							1,
							rightTriggerValue
						);
					}
					else if (xboxMode && chkCombineControllers.Checked)
					{
						ShowIgnoredPhysicalInput("Right Trigger");
					}


				}
			});
		}
	}
}