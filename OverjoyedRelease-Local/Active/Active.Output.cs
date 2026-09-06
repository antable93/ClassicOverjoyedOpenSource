using GregsStack.InputSimulatorStandard.Native;
using Microsoft.Maui.ApplicationModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wisej.Web;

namespace OverjoyedReleaseLocal
{
    public partial class Active : Page
    {
        private static readonly object mouseMoveDebugSync = new object();

        private async Task MouseMoveActions(bool FPSmode, bool lookMode)
        {
            this.Invoke(new Action(() =>
            {
                if (positionCheck)
                {
                    xEnd2 = (int)remoteX;
                    yEnd2 = (int)remoteY;
                }
                else if (chkDisableClicks.Checked && IsMoveMouseUsingLeftClicksEnabled())
                {
                    xEnd2 = mousePosX;
                    yEnd2 = mousePosY;
                }
                else
                {
                    xEnd2 = (int)(Cursor.Position.X / zoomFactor);
                    yEnd2 = (int)(Cursor.Position.Y / zoomFactor);
                }

                mousePosX = (int)xEnd2;
                mousePosY = (int)yEnd2;
            }));

            float magnitudeX = Math.Abs(xStart - xEnd2);
            float magnitudeY = Math.Abs(yStart - yEnd2);
            double distance = Math.Sqrt(Math.Pow(magnitudeX, 2) + Math.Pow(magnitudeY, 2));
            float angle = (float)(180 / Math.PI * Math.Atan2(yEnd2 - yStart, xEnd2 - xStart));
            bool inCenterDeadZone = distance <= deadZone / 2;

            bool useLatchedDeadZoneLookMode = FPSmode && fpsDeadZoneLookModeLatched;

            if (distance > deadZone)
            {
                fpsDeadZoneCenterEntryCount = 0;
            }

            if (useLatchedDeadZoneLookMode && inCenterDeadZone && fpsDeadZoneCenterEntryCount == 0)
            {
                fpsDeadZoneCenterEntryCount = 1;
            }

            if (distance <= deadZone && useLatchedDeadZoneLookMode && fpsDeadZoneCenterEntryCount == 1)
            {
                await ReleaseAllMouseMoveOutputsAsync();

                if (isActive)
                {
                    UpdateRightStickDot(xEnd2, yEnd2);
                }

                await SetHoverButtonsIdleTextAsync();
                previousQuadrant = -1;
                currentQuadrant = -1;
                currentQuadrant2 = -1;
                return;
            }

            if (isActive)
            {
                aimDotX = 300f;
                aimDotY = 350f;
                joyCenterX = 300f;
                joyCenterY = 350f;
            }

            var hoverTarget = ResolveMouseMoveTarget(distance, angle);
            hoverTarget = NormalizeFpsMouseMoveTarget(FPSmode, hoverTarget);
            currentQuadrant = hoverTarget.promptIndex;
            currentQuadrant2 = hoverTarget.promptIndex;

            await SetMouseMoveQuadrantLegendAsync(hoverTarget.promptIndex);

            if (!isActive)
            {
                ResetMouseMoveOnceState();
                previousQuadrant = -1;
                currentQuadrant = -1;
                currentQuadrant2 = -1;
                return;
            }

            bool reenteredQuadrant = previousQuadrant != hoverTarget.promptIndex;
            int safeZoneBuffer = UpdateMouseMoveSafeZoneState(hoverTarget.promptIndex);

            LogMouseMoveDebug(
                "MouseMoveActions",
                $"quadrant={hoverTarget.promptIndex} primary={hoverTarget.primaryIndex} secondary={hoverTarget.secondaryIndex} reentered={reenteredQuadrant} disabled={buttonConfig[hoverTarget.promptIndex][2]} distance={distance:F2} angle={angle:F2} safeBuffer={safeZoneBuffer} fps={FPSmode} look={lookMode}");

            if (reenteredQuadrant)
            {
                oncePressed[hoverTarget.promptIndex] = 0;
            }

            for (int i = 0; i < 33; i++)
            {
                if (i != hoverTarget.promptIndex)
                {
                    oncePressed[i] = 0;
                }
            }

            if (IsHoverMouseMoveTarget(hoverTarget.promptIndex) && safeZoneBuffer > 0 && distance <= deadZone + safeZoneBuffer)
            {
                if (reenteredQuadrant)
                {
                    await SyncMouseMoveQuadrantAsync(hoverTarget.promptIndex, buttonConfig[hoverTarget.promptIndex][2] == "true");
                }

                previousQuadrant = hoverTarget.promptIndex;
                return;
            }

            if (safeZoneBuffer > 0)
            {
                safeZone = false;
                HideSafeZoneOverlay();
            }

            if (buttonConfig[hoverTarget.promptIndex][2] == "false")
            {
                if (reenteredQuadrant)
                {
                    await SyncMouseMoveQuadrantAsync(hoverTarget.promptIndex, true);
                }

                LogMouseMoveDebug(
                    "MouseMoveActions.Dispatch",
                    $"quadrant={hoverTarget.promptIndex} behavior={GetConfiguredInputBehavior(hoverTarget.promptIndex, true)} primary={hoverTarget.primaryIndex} secondary={hoverTarget.secondaryIndex} distance={distance:F2}");

                await HandleSingleMouseMoveBehavior(hoverTarget.promptIndex, hoverTarget.toggleIndex, hoverTarget.primaryIndex, hoverTarget.secondaryIndex, distance);
            }
            else
            {
                LogMouseMoveDebug(
                    "MouseMoveActions.Disabled",
                    $"quadrant={hoverTarget.promptIndex} primary={hoverTarget.primaryIndex} secondary={hoverTarget.secondaryIndex} disabled={buttonConfig[hoverTarget.promptIndex][2]} distance={distance:F2}");

                if (reenteredQuadrant)
                {
                    await SyncMouseMoveQuadrantAsync(hoverTarget.promptIndex, true);
                }

                await SetHoverButtonsIdleTextAsync();
            }

            previousQuadrant = hoverTarget.promptIndex;
        }

        private int UpdateMouseMoveSafeZoneState(int promptIndex)
        {
            if (!chkSafeZone.Checked)
            {
                safeZone = false;
                HideSafeZoneOverlay();
                return 0;
            }

            if (IsDeadZoneMouseMoveTarget(promptIndex))
            {
                safeZone = true;
                ShowSafeZoneOverlay();
                return 0;
            }

            if (IsHoverMouseMoveTarget(promptIndex)
                && safeZone
                && buttonConfig[promptIndex + 8][2] == "false"
                && buttonConfig[promptIndex + 16][2] == "false"
                && !string.Equals(buttonConfig[promptIndex][1], "once", StringComparison.OrdinalIgnoreCase))
            {
                ShowSafeZoneOverlay();
                return 38;
            }

            HideSafeZoneOverlay();
            return 0;
        }

        private static bool IsDeadZoneMouseMoveTarget(int promptIndex)
        {
            return promptIndex == 24 || promptIndex == 27 || promptIndex == 30;
        }

        private static bool IsHoverMouseMoveTarget(int promptIndex)
        {
            return promptIndex >= 0 && promptIndex <= 7;
        }

        private (int promptIndex, int toggleIndex, int primaryIndex, int secondaryIndex) ResolveMouseMoveTarget(double distance, float angle)
        {
            if (distance <= deadZone)
            {
                return ResolveDeadZoneMouseMoveTarget(distance);
            }

            int quadrant = ResolveMouseMoveQuadrant(angle);
            return ResolveHoverMouseMoveTarget(quadrant);
        }

        private (int promptIndex, int toggleIndex, int primaryIndex, int secondaryIndex) ResolveDeadZoneMouseMoveTarget(double distance)
        {
            if (distance <= deadZone / 2)
            {
                return (27, 27, 27, 27);
            }

            return (yStart - yEnd2) > 0 ? (24, 24, 24, 24) : (30, 30, 30, 30);
        }

        private (int promptIndex, int toggleIndex, int primaryIndex, int secondaryIndex) ResolveHoverMouseMoveTarget(int quadrant)
        {
            return quadrant switch
            {
                1 when ShouldUseAdjacentHoverTargets(1, 0, 2) => (1, 1, 0, 2),
                3 when ShouldUseAdjacentHoverTargets(3, 2, 4) => (3, 3, 2, 4),
                5 when ShouldUseAdjacentHoverTargets(5, 4, 6) => (5, 5, 4, 6),
                7 when ShouldUseAdjacentHoverTargets(7, 6, 0) => (7, 7, 6, 0),
                _ => (quadrant, quadrant, quadrant, quadrant)
            };
        }

        private (int promptIndex, int toggleIndex, int primaryIndex, int secondaryIndex) NormalizeFpsMouseMoveTarget(
            bool fpsMode,
            (int promptIndex, int toggleIndex, int primaryIndex, int secondaryIndex) target)
        {
            if (!fpsMode)
            {
                return target;
            }

            return target.promptIndex switch
            {
                2 => (2, 2, 2, 33),
                6 => (6, 6, 6, 34),
                _ => target
            };
        }

        private bool ShouldUseAdjacentHoverTargets(int diagonalQuadrant, int primaryIndex, int secondaryIndex)
        {
            if (!IsAdjacentHoverQuadrant(diagonalQuadrant))
            {
                return false;
            }

            return true;
        }

        private static float SnapQuadrantBoundaryAngleToCardinal(float angle)
        {
            const float boundaryEpsilon = 1.0f;

            if (Math.Abs(angle + 112.5f) < boundaryEpsilon || Math.Abs(angle + 67.5f) < boundaryEpsilon)
            {
                return -90f;
            }

            if (Math.Abs(angle + 22.5f) < boundaryEpsilon || Math.Abs(angle - 22.5f) < boundaryEpsilon)
            {
                return 0f;
            }

            if (Math.Abs(angle - 67.5f) < boundaryEpsilon || Math.Abs(angle - 112.5f) < boundaryEpsilon)
            {
                return 90f;
            }

            if (Math.Abs(angle + 157.5f) < boundaryEpsilon || Math.Abs(angle - 157.5f) < boundaryEpsilon || Math.Abs(Math.Abs(angle) - 180f) < boundaryEpsilon)
            {
                return 179.999f;
            }

            return angle;
        }

        private int ResolveMouseMoveQuadrant(float angle)
        {
            angle = SnapQuadrantBoundaryAngleToCardinal(angle);

            if (angle >= -112.5 && angle < -67.5)
            {
                return 0;
            }

            if (angle >= -67.5 && angle < -22.5)
            {
                return 1;
            }

            if (angle >= -22.5 && angle < 22.5)
            {
                return 2;
            }

            if (angle >= 22.5 && angle < 67.5)
            {
                return 3;
            }

            if (angle >= 67.5 && angle < 112.5)
            {
                return 4;
            }

            if (angle >= 112.5 && angle < 157.5)
            {
                return 5;
            }

            if (angle >= 157.5 || angle < -157.5)
            {
                return 6;
            }

            return 7;
        }

        private async Task SetMouseMoveQuadrantLegendAsync(int quadrant)
        {
            string moveLegend = buttonConfig[quadrant][2] == "false" ? GetMouseMoveLegendToken(quadrant) : "None";
            string leftLegend = "None";
            string rightLegend = "None";

            if (quadrant >= 0 && quadrant <= 7)
            {
                leftLegend = buttonConfig[quadrant + 8][2] == "false" ? GetMouseMoveLegendToken(quadrant + 8) : "None";
                rightLegend = buttonConfig[quadrant + 16][2] == "false" ? GetMouseMoveLegendToken(quadrant + 16) : "None";
            }

            await SetMouseMoveLegendAsync(moveLegend, leftLegend, rightLegend);
        }

        private string GetMouseMoveLegendToken(int promptIndex)
        {
            if (keyboardMode)
            {
                return keyCodes[promptIndex].ToString();
            }

            if (xboxMode)
            {
                return xbuttons[promptIndex].ToString();
            }

            return switchbuttons[promptIndex].ToString();
        }

        bool ToggleOn(int q1)
        {
            bool state = false;
            if (activateToggle[q1] && !buttonPressed[q1]) { state = true; }
            else { state = false; }
            return state;
        }

        bool ToggleOff(int q1)
        {
            bool state = false;
            if (activateToggle[q1] && buttonPressed[q1]) { state = true; }
            else { state = false; }
            return state;
        }

        bool ClickToggleOn(int c0)
        {
            bool state = false;
            if (activateToggle[c0] && !buttonPressed[c0]) { state = true; }
            else { state = false; }
            return state;
        }

        bool ClickToggleOff(int c0)
        {
            bool state = false;
            if (activateToggle[c0] && buttonPressed[c0]) { state = true; }
            else { state = false; }
            return state;
        }

        private void ApplyToggleOnState(int c1, int c2, int c3)
        {
            ApplyToggleTargetState(GetCurrentClickThreadMode(), c1, c2, c3, true);
        }

        private void ApplyToggleOffState(int c1, int c2, int c3)
        {
            ApplyToggleTargetState(GetCurrentClickThreadMode(), c1, c2, c3, false);
        }

        private void MoveCombinedXboxLeftStick(int axis, double amount, bool negative)
        {
            if (chkCombineControllers.Checked && !chkCombineLS.Checked)
            {
                return;
            }

            SimGamePad.Instance.MoveSticks(axis, amount, negative);
        }

        private void MoveCombinedXboxRightStick(int axis, double amount, bool negative)
        {
            if (chkCombineControllers.Checked && !chkCombineRS.Checked)
            {
                return;
            }

            SimGamePad.Instance.MoveSticks(axis, amount, negative);
        }

        private void SetCombinedXboxTrigger(int triggerIndex, double amount)
        {
            if (chkCombineControllers.Checked && ((triggerIndex == 0 && !chkCombineLT.Checked) || (triggerIndex == 1 && !chkCombineRT.Checked)))
            {
                return;
            }

            SimGamePad.Instance.VariableTriggers(triggerIndex, amount);
        }

        private void MoveCombinedSwitchLeftStick(string direction, double amount)
        {
            if (chkCombineControllers.Checked && !chkCombineLS.Checked)
            {
                return;
            }

            if (direction == "up") { switchGamepad.leftStick_MoveUp(amount); }
            else if (direction == "right") { switchGamepad.leftStick_MoveRight(amount); }
            else if (direction == "down") { switchGamepad.leftStick_MoveDown(amount); }
            else if (direction == "left") { switchGamepad.leftStick_MoveLeft(amount); }
        }

        private void MoveCombinedSwitchLeftStickDiagonal(double amount, int angle)
        {
            if (chkCombineControllers.Checked && !chkCombineLS.Checked)
            {
                return;
            }

            switchGamepad.MoveLeftStickDiagonal(amount, angle);
        }

        private void MoveCombinedSwitchRightStick(string direction, double amount)
        {
            if (chkCombineControllers.Checked && !chkCombineRS.Checked)
            {
                return;
            }

            if (direction == "up") { switchGamepad.rightStick_MoveUp(amount); }
            else if (direction == "right") { switchGamepad.rightStick_MoveRight(amount); }
            else if (direction == "down") { switchGamepad.rightStick_MoveDown(amount); }
            else if (direction == "left") { switchGamepad.rightStick_MoveLeft(amount); }
        }

        private void MoveCombinedSwitchRightStickDiagonal(double amount, int angle)
        {
            if (chkCombineControllers.Checked && !chkCombineRS.Checked)
            {
                return;
            }

            switchGamepad.MoveRightStickDiagonal(amount, angle);
        }

        private bool IsBlockedXboxToken(string token)
        {
            if (!chkCombineControllers.Checked || string.IsNullOrEmpty(token))
            {
                return false;
            }

            if (token.StartsWith("LS ", StringComparison.OrdinalIgnoreCase))
            {
                return !chkCombineLS.Checked;
            }

            if (token.StartsWith("RS ", StringComparison.OrdinalIgnoreCase))
            {
                return !chkCombineRS.Checked;
            }

            if (string.Equals(token, "LT or L2", StringComparison.OrdinalIgnoreCase))
            {
                return !chkCombineLT.Checked;
            }

            if (string.Equals(token, "RT or R2", StringComparison.OrdinalIgnoreCase))
            {
                return !chkCombineRT.Checked;
            }

            return false;
        }

        private bool IsBlockedSwitchToken(string token)
        {
            if (!chkCombineControllers.Checked || string.IsNullOrEmpty(token))
            {
                return false;
            }

            if (token.StartsWith("LS ", StringComparison.OrdinalIgnoreCase))
            {
                return !chkCombineLS.Checked;
            }

            if (token.StartsWith("RS ", StringComparison.OrdinalIgnoreCase))
            {
                return !chkCombineRS.Checked;
            }

            if (string.Equals(token, "ZL Button", StringComparison.OrdinalIgnoreCase))
            {
                return !chkCombineLT.Checked;
            }

            if (string.Equals(token, "ZR Button", StringComparison.OrdinalIgnoreCase))
            {
                return !chkCombineRT.Checked;
            }

            return false;
        }

        private bool TryApplySingleXboxVariableToken(string token, int promptIndex, double amount)
        {
            if (kbButtons[promptIndex][5] != "True")
            {
                return false;
            }

            if (IsBlockedXboxToken(token))
            {
                return true;
            }

            switch (token)
            {
                case "RT or R2":
                    SetCombinedXboxTrigger(1, amount);
                    SimGamePad.Instance.Update(1);
                    return true;
                case "LT or L2":
                    SetCombinedXboxTrigger(0, amount);
                    SimGamePad.Instance.Update(1);
                    return true;
                case "RS Up":
                    if (!toggleXbox.Contains("RS Down") && !toggleXbox.Contains("RS Right") && !toggleXbox.Contains("RS Left"))
                    {
                        MoveCombinedXboxRightStick(3, amount, false);
                        SimGamePad.Instance.Update(1);
                    }
                    return true;
                case "RS Right":
                    if (!toggleXbox.Contains("RS Up") && !toggleXbox.Contains("RS Down") && !toggleXbox.Contains("RS Left"))
                    {
                        MoveCombinedXboxRightStick(2, amount, false);
                        SimGamePad.Instance.Update(1);
                    }
                    return true;
                case "RS Down":
                    if (!toggleXbox.Contains("RS Up") && !toggleXbox.Contains("RS Right") && !toggleXbox.Contains("RS Left"))
                    {
                        MoveCombinedXboxRightStick(3, amount, true);
                        SimGamePad.Instance.Update(1);
                    }
                    return true;
                case "RS Left":
                    if (!toggleXbox.Contains("RS Up") && !toggleXbox.Contains("RS Down") && !toggleXbox.Contains("RS Right"))
                    {
                        MoveCombinedXboxRightStick(2, amount, true);
                        SimGamePad.Instance.Update(1);
                    }
                    return true;
                case "LS Up":
                    if (!toggleXbox.Contains("LS Down") && !toggleXbox.Contains("LS Right") && !toggleXbox.Contains("LS Left"))
                    {
                        MoveCombinedXboxLeftStick(1, amount, false);
                        SimGamePad.Instance.Update(1);
                    }
                    return true;
                case "LS Right":
                    if (!toggleXbox.Contains("LS Up") && !toggleXbox.Contains("LS Down") && !toggleXbox.Contains("LS Left"))
                    {
                        MoveCombinedXboxLeftStick(0, amount, false);
                        SimGamePad.Instance.Update(1);
                    }
                    return true;
                case "LS Down":
                    if (!toggleXbox.Contains("LS Up") && !toggleXbox.Contains("LS Right") && !toggleXbox.Contains("LS Left"))
                    {
                        MoveCombinedXboxLeftStick(1, amount, true);
                        SimGamePad.Instance.Update(1);
                    }
                    return true;
                case "LS Left":
                    if (!toggleXbox.Contains("LS Up") && !toggleXbox.Contains("LS Down") && !toggleXbox.Contains("LS Right"))
                    {
                        MoveCombinedXboxLeftStick(0, amount, true);
                        SimGamePad.Instance.Update(1);
                    }
                    return true;
                default:
                    return false;
            }
        }

        private void PressSwitchToken(string token, bool skipToggled)
        {
            if (skipToggled && toggleSwitch.Contains(token))
            {
                return;
            }

            if (IsBlockedSwitchToken(token))
            {
                return;
            }

            switch (token)
            {
                case "A Button": switchGamepad.ButtonA_Down(); break;
                case "B Button": switchGamepad.ButtonB_Down(); break;
                case "X Button": switchGamepad.ButtonX_Down(); break;
                case "Y Button": switchGamepad.ButtonY_Down(); break;
                case "L Button": switchGamepad.ButtonL_Down(); break;
                case "ZL Button": switchGamepad.ButtonZL_Down(); break;
                case "L3 Button": switchGamepad.ButtonL3_Down(); break;
                case "R Button": switchGamepad.ButtonR_Down(); break;
                case "ZR Button": switchGamepad.ButtonZR_Down(); break;
                case "R3 Button": switchGamepad.ButtonR3_Down(); break;
                case "DPad Up": switchGamepad.ButtonDpadUp_Down(); break;
                case "DPad Right": switchGamepad.ButtonDpadRight_Down(); break;
                case "DPad Down": switchGamepad.ButtonDpadDown_Down(); break;
                case "DPad Left": switchGamepad.ButtonDpadLeft_Down(); break;
                case "Home": switchGamepad.ButtonHome_Down(); break;
                case "Plus (Start)": switchGamepad.ButtonPlus_Down(); break;
                case "Minus (Select)": switchGamepad.ButtonMinus_Down(); break;
                case "Capture": switchGamepad.ButtonCapture_Down(); break;
            }
        }

        private void PressSwitchTokens(bool skipToggled, params int[] promptIndices)
        {
            HashSet<string> tokens = new HashSet<string>(promptIndices.Select(index => switchbuttons[index].ToString()));
            foreach (string token in tokens)
            {
                PressSwitchToken(token, skipToggled);
            }
        }

        private void StopCombinedSwitchLeftStick()
        {
            switchGamepad.leftStick_Stop();
        }

        private void StopCombinedSwitchRightStick()
        {
            switchGamepad.rightStick_Stop();
        }

        private void SetCombinedSwitchTrigger(bool leftTrigger, bool pressed)
        {
            if (chkCombineControllers.Checked && ((leftTrigger && !chkCombineLT.Checked) || (!leftTrigger && !chkCombineRT.Checked)))
            {
                return;
            }

            if (leftTrigger)
            {
                if (pressed) { switchGamepad.ButtonZL_Down(); }
                else { switchGamepad.ButtonZL_Up(); }
            }
            else
            {
                if (pressed) { switchGamepad.ButtonZR_Down(); }
                else { switchGamepad.ButtonZR_Up(); }
            }
        }

        private static bool IsEnabledFlag(string value)
        {
            return string.Equals(value, "True", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsXboxVariableEligibleToken(string token)
        {
            return !string.IsNullOrEmpty(token) &&
                (token.StartsWith("LS ", StringComparison.OrdinalIgnoreCase)
                || token.StartsWith("RS ", StringComparison.OrdinalIgnoreCase)
                || string.Equals(token, "LT or L2", StringComparison.OrdinalIgnoreCase)
                || string.Equals(token, "RT or R2", StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsSwitchVariableEligibleToken(string token)
        {
            return !string.IsNullOrEmpty(token) &&
                (token.StartsWith("LS ", StringComparison.OrdinalIgnoreCase)
                || token.StartsWith("RS ", StringComparison.OrdinalIgnoreCase));
        }

        private bool IsResolvedXboxVariableTarget(int promptIndex, int resolvedIndex, string token)
        {
            return GetPromptBinding(resolvedIndex).IsVariable
                || (promptIndex != resolvedIndex && IsEnabledFlag(buttonConfig[promptIndex][5]) && IsXboxVariableEligibleToken(token));
        }

        private bool IsResolvedSwitchVariableTarget(int promptIndex, int resolvedIndex, string token)
        {
            return GetPromptBinding(resolvedIndex).IsVariable
                || (promptIndex != resolvedIndex && IsEnabledFlag(buttonConfig[promptIndex][5]) && IsSwitchVariableEligibleToken(token));
        }

        private static bool IsCurrentVariableTokenIteration(string currentOption, string firstToken, bool firstVariable, string secondToken, bool secondVariable, Func<string, bool> isEligibleToken)
        {
            return (firstVariable && isEligibleToken(firstToken) && string.Equals(firstToken, currentOption, StringComparison.OrdinalIgnoreCase))
                || (secondVariable && isEligibleToken(secondToken) && string.Equals(secondToken, currentOption, StringComparison.OrdinalIgnoreCase));
        }

        private static int GetMixedVariablePartnerIndex(int firstIndex, bool firstVariable, int secondIndex, bool secondVariable)
        {
            if (firstVariable && !secondVariable)
            {
                return secondIndex;
            }

            if (secondVariable && !firstVariable)
            {
                return firstIndex;
            }

            return secondIndex;
        }

        private void ApplyXboxVariableTarget(OutputTarget target, bool resetSticks, bool resetButtons = true)
        {
            int x0 = target.PromptIndex;
            int x1 = target.PrimaryIndex;
            int x2 = target.SecondaryIndex;
            VariableInputState variableState = CreateVariableInputState(x0, target.Distance, !resetSticks);
            GamepadMouseX1 = mousePosX;
            GamepadMouseY1 = mousePosY;
            double disPushed = variableState.UnitMagnitude;
            bool x1Variable = IsResolvedXboxVariableTarget(x0, x1, xbuttons[x1].ToString());
            bool x2Variable = IsResolvedXboxVariableTarget(x0, x2, xbuttons[x2].ToString());
            if (variableState.ShouldProcess)
            {



                if (resetSticks)
                {

                    if (!(toggleXbox.Contains("LS Up") == true) && !(toggleXbox.Contains("LS Down") == true) && !(toggleXbox.Contains("LS Right") == true) && !(toggleXbox.Contains("LS Left") == true))
                    {
                        if (chkCombineLS.Checked)
                        {
                            SimGamePad.Instance.MoveSticks(0, 0, false);
                            SimGamePad.Instance.MoveSticks(1, 0, false);
                        }
                    }
                    if (!(toggleXbox.Contains("RS Up") == true) && !(toggleXbox.Contains("RS Down") == true) && !(toggleXbox.Contains("RS Right") == true) && !(toggleXbox.Contains("RS Left") == true))
                    {
                        if (chkCombineRS.Checked)
                        {
                            MoveCombinedXboxRightStick(2, 0, false);
                            MoveCombinedXboxRightStick(3, 0, false);
                        }

                    }
                    //if (!chkFPS.Checked)
                    //{

                    if (toggleXbox.Contains("RT or R2") == false)
                    {
                        if (chkCombineRT.Checked)
                        {
                            SetCombinedXboxTrigger(1, 0);
                        }
                    }
                    if (toggleXbox.Contains("LT or L2") == false)
                    {
                        if (chkCombineLT.Checked)
                        {
                            SetCombinedXboxTrigger(0, 0);
                        }
                    }
                    //}
                }

                if (resetButtons)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        if (i != 11)
                        {
                            if (toggleXbox.Contains(buttonOptions[i].ToString()) == false)
                            {
                                SimGamePad.Instance.ReleaseControl(i, toggleXbox, 1);



                            }
                        }
                    }

                }

                for (int i = 0; i < 26; i++)
                {

                    if ((xbuttons[x1].ToString() == buttonOptions[i].ToString() || xbuttons[x2].ToString() == buttonOptions[i].ToString()))
                    {
                        if (x1 == x2)
                        {
                            if (!TryApplySingleXboxVariableToken(xbuttons[x0].ToString(), x0, disPushed))
                            {
                                if (!IsBlockedXboxToken(xbuttons[x0].ToString()))
                                {
                                    SimGamePad.Instance.SetControl(i, 1);
                                }
                            }
                        }

                        else
                        {
                            if (x1Variable && x2Variable)
                            {
                                if ((xbuttons[x1].ToString() == "RS Right" || xbuttons[x2].ToString() == "RS Right") && (xbuttons[x1].ToString() == "RS Up" || xbuttons[x2].ToString() == "RS Up"))
                                {
                                    if (!(toggleXbox.Contains("RS Down") == true) && !(toggleXbox.Contains("RS Left") == true))
                                    {
                                        MoveCombinedXboxRightStick(2, disPushed, false);
                                        MoveCombinedXboxRightStick(3, disPushed, false);
                                        SimGamePad.Instance.Update(1);
                                    }
                                }

                                else if ((xbuttons[x1].ToString() == "RS Right" || xbuttons[x2].ToString() == "RS Right") && (xbuttons[x1].ToString() == "RS Down" || xbuttons[x2].ToString() == "RS Down"))
                                {
                                    if (!(toggleXbox.Contains("RS Up") == true) && !(toggleXbox.Contains("RS Left") == true))
                                    {
                                        MoveCombinedXboxRightStick(2, disPushed, false);
                                        MoveCombinedXboxRightStick(3, disPushed, true);
                                        SimGamePad.Instance.Update(1);
                                    }
                                }

                                else if ((xbuttons[x1].ToString() == "RS Left" || xbuttons[x2].ToString() == "RS Left") && (xbuttons[x1].ToString() == "RS Down" || xbuttons[x2].ToString() == "RS Down"))
                                {
                                    if (!(toggleXbox.Contains("RS Up") == true) && !(toggleXbox.Contains("RS Right") == true))
                                    {
                                        MoveCombinedXboxRightStick(2, disPushed, true);
                                        MoveCombinedXboxRightStick(3, disPushed, true);
                                        SimGamePad.Instance.Update(1);
                                    }
                                }

                                else if ((xbuttons[x1].ToString() == "RS Left" || xbuttons[x2].ToString() == "RS Left") && (xbuttons[x1].ToString() == "RS Up" || xbuttons[x2].ToString() == "RS Up"))
                                {
                                    if (!(toggleXbox.Contains("RS Down") == true) && !(toggleXbox.Contains("RS Right") == true))
                                    {
                                        MoveCombinedXboxRightStick(2, disPushed, true);
                                        MoveCombinedXboxRightStick(3, disPushed, false);
                                        SimGamePad.Instance.Update(1);
                                    }
                                }


                                else if ((xbuttons[x1].ToString() == "LS Right" || xbuttons[x2].ToString() == "LS Right") && (xbuttons[x1].ToString() == "LS Up" || xbuttons[x2].ToString() == "LS Up"))
                                {
                                    if (!(toggleXbox.Contains("LS Down") == true) && !(toggleXbox.Contains("LS Left") == true))
                                    {
                                        MoveCombinedXboxLeftStick(0, disPushed, false);
                                        MoveCombinedXboxLeftStick(1, disPushed, false);
                                        SimGamePad.Instance.Update(1);
                                    }
                                }

                                else if ((xbuttons[x1].ToString() == "LS Right" || xbuttons[x2].ToString() == "LS Right") && (xbuttons[x1].ToString() == "LS Down" || xbuttons[x2].ToString() == "LS Down"))
                                {
                                    if (!(toggleXbox.Contains("LS Up") == true) && !(toggleXbox.Contains("LS Left") == true))
                                    {
                                        MoveCombinedXboxLeftStick(0, disPushed, false);
                                        MoveCombinedXboxLeftStick(1, disPushed, true);
                                        SimGamePad.Instance.Update(1);
                                    }
                                }

                                else if ((xbuttons[x1].ToString() == "LS Left" || xbuttons[x2].ToString() == "LS Left") && (xbuttons[x1].ToString() == "LS Down" || xbuttons[x2].ToString() == "LS Down"))
                                {
                                    if (!(toggleXbox.Contains("LS Up") == true) && !(toggleXbox.Contains("LS Right") == true))
                                    {
                                        MoveCombinedXboxLeftStick(0, disPushed, true);
                                        MoveCombinedXboxLeftStick(1, disPushed, true);
                                        SimGamePad.Instance.Update(1);
                                    }
                                }

                                else if ((xbuttons[x1].ToString() == "LS Left" || xbuttons[x2].ToString() == "LS Left") && (xbuttons[x1].ToString() == "LS Up" || xbuttons[x2].ToString() == "LS Up"))
                                {
                                    if (!(toggleXbox.Contains("LS Down") == true) && !(toggleXbox.Contains("LS Right") == true))
                                    {
                                        MoveCombinedXboxLeftStick(0, disPushed, true);
                                        MoveCombinedXboxLeftStick(1, disPushed, false);
                                        SimGamePad.Instance.Update(1);
                                    }
                                }

                                else if ((xbuttons[x1].ToString() == "RS Left" || xbuttons[x2].ToString() == "RS Left") && (xbuttons[x1].ToString() == "LS Left" || xbuttons[x2].ToString() == "LS Left"))
                                {
                                    if (!(toggleXbox.Contains("LS Up") == true) && !(toggleXbox.Contains("LS Down") == true) && !(toggleXbox.Contains("LS Right") == true))
                                    {
                                        if (!(toggleXbox.Contains("RS Up") == true) && !(toggleXbox.Contains("RS Down") == true) && !(toggleXbox.Contains("RS Right") == true))
                                        {
                                            MoveCombinedXboxLeftStick(0, disPushed, true);
                                            MoveCombinedXboxRightStick(2, disPushed, true);
                                            SimGamePad.Instance.Update(1);
                                        }
                                    }
                                }

                                else if ((xbuttons[x1].ToString() == "RS Right" || xbuttons[x2].ToString() == "RS Right") && (xbuttons[x1].ToString() == "LS Right" || xbuttons[x2].ToString() == "LS Right"))
                                {
                                    if (!(toggleXbox.Contains("LS Up") == true) && !(toggleXbox.Contains("LS Down") == true) && !(toggleXbox.Contains("LS Left") == true))
                                    {
                                        if (!(toggleXbox.Contains("RS Up") == true) && !(toggleXbox.Contains("RS Down") == true) && !(toggleXbox.Contains("RS Left") == true))
                                        {
                                            MoveCombinedXboxLeftStick(0, disPushed, false);
                                            MoveCombinedXboxRightStick(2, disPushed, false);
                                            SimGamePad.Instance.Update(1);
                                        }
                                    }
                                }

                                else if ((xbuttons[x1].ToString() == "RS Up" || xbuttons[x2].ToString() == "RS Up") && (xbuttons[x1].ToString() == "LS Up" || xbuttons[x2].ToString() == "LS Up"))
                                {
                                    if (!(toggleXbox.Contains("LS Down") == true) && !(toggleXbox.Contains("LS Right") == true) && !(toggleXbox.Contains("LS Left") == true))
                                    {
                                        if (!(toggleXbox.Contains("RS Down") == true) && !(toggleXbox.Contains("RS Right") == true) && !(toggleXbox.Contains("RS Left") == true))
                                        {
                                            MoveCombinedXboxLeftStick(1, disPushed, false);
                                            MoveCombinedXboxRightStick(3, disPushed, false);
                                            SimGamePad.Instance.Update(1);
                                        }
                                    }
                                }

                                else if ((xbuttons[x1].ToString() == "RS Down" || xbuttons[x2].ToString() == "RS Down") && (xbuttons[x1].ToString() == "LS Down" || xbuttons[x2].ToString() == "LS Down"))
                                {
                                    if (!(toggleXbox.Contains("LS Up") == true) && !(toggleXbox.Contains("LS Right") == true) && !(toggleXbox.Contains("LS Left") == true))
                                    {
                                        if (!(toggleXbox.Contains("RS Up") == true) && !(toggleXbox.Contains("RS Right") == true) && !(toggleXbox.Contains("RS Left") == true))
                                        {
                                            MoveCombinedXboxLeftStick(1, disPushed, true);
                                            MoveCombinedXboxRightStick(3, disPushed, true);
                                            SimGamePad.Instance.Update(1);
                                        }
                                    }
                                }



                                else if ((xbuttons[x1].ToString() == "RT or R2" || xbuttons[x2].ToString() == "RT or R2") && (xbuttons[x1].ToString() == "LS Up" || xbuttons[x2].ToString() == "LS Up"))
                                {
                                    SetCombinedXboxTrigger(1, disPushed);
                                    if (!(toggleXbox.Contains("LS Down") == true) && !(toggleXbox.Contains("LS Right") == true) && !(toggleXbox.Contains("LS Left") == true))
                                    {
                                        MoveCombinedXboxLeftStick(1, disPushed, true);
                                    }
                                    SimGamePad.Instance.Update(1);
                                }

                                else if ((xbuttons[x1].ToString() == "RT or R2" || xbuttons[x2].ToString() == "RT or R2") && (xbuttons[x1].ToString() == "LS Down" || xbuttons[x2].ToString() == "LS Down"))
                                {
                                    SetCombinedXboxTrigger(1, disPushed);
                                    if (!(toggleXbox.Contains("LS Up") == true) && !(toggleXbox.Contains("LS Right") == true) && !(toggleXbox.Contains("LS Left") == true))
                                    {
                                        MoveCombinedXboxLeftStick(1, disPushed, true);
                                    }
                                    SimGamePad.Instance.Update(1);
                                }
                                else if ((xbuttons[x1].ToString() == "RT or R2" || xbuttons[x2].ToString() == "RT or R2") && (xbuttons[x1].ToString() == "LS Left" || xbuttons[x2].ToString() == "LS Left"))
                                {
                                    SetCombinedXboxTrigger(1, disPushed);
                                    if (!(toggleXbox.Contains("LS Up") == true) && !(toggleXbox.Contains("LS Down") == true) && !(toggleXbox.Contains("LS Right") == true))
                                    {



                                        MoveCombinedXboxLeftStick(0, disPushed, true);
                                    }
                                    SimGamePad.Instance.Update(1);
                                }

                                else if ((xbuttons[x1].ToString() == "RT or R2" || xbuttons[x2].ToString() == "RT or R2") && (xbuttons[x1].ToString() == "LS Right" || xbuttons[x2].ToString() == "LS Right"))
                                {
                                    SetCombinedXboxTrigger(1, disPushed);
                                    if (!(toggleXbox.Contains("LS Up") == true) && !(toggleXbox.Contains("LS Down") == true) && !(toggleXbox.Contains("LS Left") == true))
                                    {
                                        MoveCombinedXboxLeftStick(0, disPushed, false);
                                    }
                                    SimGamePad.Instance.Update(1);
                                }






                                else if ((xbuttons[x1].ToString() == "RT or R2" || xbuttons[x2].ToString() == "RT or R2") && (xbuttons[x1].ToString() == "RS Up" || xbuttons[x2].ToString() == "RS Up"))
                                {
                                    SetCombinedXboxTrigger(1, disPushed);
                                    if (!(toggleXbox.Contains("RS Down") == true) && !(toggleXbox.Contains("RS Right") == true) && !(toggleXbox.Contains("RS Left") == true))
                                    {
                                        MoveCombinedXboxRightStick(3, disPushed, false);
                                    }
                                    SimGamePad.Instance.Update(1);
                                }

                                else if ((xbuttons[x1].ToString() == "RT or R2" || xbuttons[x2].ToString() == "RT or R2") && (xbuttons[x1].ToString() == "RS Down" || xbuttons[x2].ToString() == "RS Down"))
                                {
                                    SetCombinedXboxTrigger(1, disPushed);
                                    if (!(toggleXbox.Contains("RS Up") == true) && !(toggleXbox.Contains("RS Right") == true) && !(toggleXbox.Contains("RS Left") == true))
                                    {
                                        MoveCombinedXboxRightStick(3, disPushed, true);
                                    }
                                    SimGamePad.Instance.Update(1);
                                }
                                else if ((xbuttons[x1].ToString() == "RT or R2" || xbuttons[x2].ToString() == "RT or R2") && (xbuttons[x1].ToString() == "RS Left" || xbuttons[x2].ToString() == "RS Left"))
                                {
                                    SetCombinedXboxTrigger(1, disPushed);
                                    if (!(toggleXbox.Contains("RS Left") == true))
                                    {
                                        MoveCombinedXboxRightStick(2, disPushed, true);
                                    }
                                    SimGamePad.Instance.Update(1);
                                }

                                else if ((xbuttons[x1].ToString() == "RT or R2" || xbuttons[x2].ToString() == "RT or R2") && (xbuttons[x1].ToString() == "RS Right" || xbuttons[x2].ToString() == "RS Right"))
                                {
                                    SetCombinedXboxTrigger(1, disPushed);
                                    if (!(toggleXbox.Contains("RS Down") == true) && !(toggleXbox.Contains("RS Right") == true) && !(toggleXbox.Contains("RS Left") == true))
                                    {
                                        MoveCombinedXboxRightStick(2, disPushed, false);
                                    }
                                    SimGamePad.Instance.Update(1);
                                }

                                else if ((xbuttons[x1].ToString() == "RT or R2" || xbuttons[x2].ToString() == "RT or R2") && (xbuttons[x1].ToString() == "LT or L2" || xbuttons[x2].ToString() == "LT or L2"))
                                {
                                    SetCombinedXboxTrigger(1, disPushed);
                                    SetCombinedXboxTrigger(0, disPushed);
                                    SimGamePad.Instance.Update(1);
                                }



                                else if ((xbuttons[x1].ToString() == "LT or L2" || xbuttons[x2].ToString() == "LT or L2") && (xbuttons[x1].ToString() == "LS Up" || xbuttons[x2].ToString() == "LS Up"))
                                {
                                    SetCombinedXboxTrigger(0, disPushed);
                                    if (!(toggleXbox.Contains("LS Down") == true) && !(toggleXbox.Contains("LS Right") == true) && !(toggleXbox.Contains("LS Left") == true))
                                    {
                                        MoveCombinedXboxLeftStick(1, disPushed, false);
                                    }
                                    SimGamePad.Instance.Update(1);
                                }

                                else if ((xbuttons[x1].ToString() == "LT or L2" || xbuttons[x2].ToString() == "LT or L2") && (xbuttons[x1].ToString() == "LS Down" || xbuttons[x2].ToString() == "LS Down"))
                                {
                                    SetCombinedXboxTrigger(0, disPushed);
                                    if (!(toggleXbox.Contains("LS Up") == true) && !(toggleXbox.Contains("LS Right") == true) && !(toggleXbox.Contains("LS Left") == true))
                                    {
                                        MoveCombinedXboxLeftStick(1, disPushed, true);
                                    }
                                    SimGamePad.Instance.Update(1);
                                }
                                else if ((xbuttons[x1].ToString() == "LT or L2" || xbuttons[x2].ToString() == "LT or L2") && (xbuttons[x1].ToString() == "LS Left" || xbuttons[x2].ToString() == "LS Left"))
                                {
                                    SetCombinedXboxTrigger(0, disPushed);
                                    if (!(toggleXbox.Contains("LS Up") == true) && !(toggleXbox.Contains("LS Down") == true) && !(toggleXbox.Contains("LS Right") == true))
                                    {
                                        MoveCombinedXboxLeftStick(0, disPushed, true);
                                    }
                                    SimGamePad.Instance.Update(1);
                                }

                                else if ((xbuttons[x1].ToString() == "LT or L2" || xbuttons[x2].ToString() == "LT or L2") && (xbuttons[x1].ToString() == "LS Right" || xbuttons[x2].ToString() == "LS Right"))
                                {
                                    SetCombinedXboxTrigger(0, disPushed);
                                    if (!(toggleXbox.Contains("LS Up") == true) && !(toggleXbox.Contains("LS Down") == true) && !(toggleXbox.Contains("LS Left") == true))
                                    {
                                        MoveCombinedXboxLeftStick(0, disPushed, false);
                                    }
                                    SimGamePad.Instance.Update(1);
                                }

                                else if ((xbuttons[x1].ToString() == "LT or L2" || xbuttons[x2].ToString() == "LT or L2") && (xbuttons[x1].ToString() == "RS Up" || xbuttons[x2].ToString() == "RS Up"))
                                {
                                    SetCombinedXboxTrigger(0, disPushed);
                                    if (!(toggleXbox.Contains("RS Down") == true) && !(toggleXbox.Contains("RS Right") == true) && !(toggleXbox.Contains("RS Left") == true))
                                    {
                                        MoveCombinedXboxRightStick(3, disPushed, false);
                                    }
                                    SimGamePad.Instance.Update(1);
                                }

                                else if ((xbuttons[x1].ToString() == "LT or L2" || xbuttons[x2].ToString() == "LT or L2") && (xbuttons[x1].ToString() == "RS Down" || xbuttons[x2].ToString() == "RS Down"))
                                {
                                    SetCombinedXboxTrigger(0, disPushed);
                                    if (!(toggleXbox.Contains("RS Up") == true) && !(toggleXbox.Contains("RS Right") == true) && !(toggleXbox.Contains("RS Left") == true))
                                    {
                                        MoveCombinedXboxRightStick(3, disPushed, true);
                                    }
                                    SimGamePad.Instance.Update(1);
                                }
                                else if ((xbuttons[x1].ToString() == "LT or L2" || xbuttons[x2].ToString() == "LT or L2") && (xbuttons[x1].ToString() == "RS Left" || xbuttons[x2].ToString() == "RS Left"))
                                {
                                    SetCombinedXboxTrigger(0, disPushed);
                                    if (!(toggleXbox.Contains("RS Up") == true) && !(toggleXbox.Contains("RS Down") == true) && !(toggleXbox.Contains("RS Right") == true))
                                    {
                                        MoveCombinedXboxRightStick(2, disPushed, true);
                                    }
                                    SimGamePad.Instance.Update(1);
                                }

                                else if ((xbuttons[x1].ToString() == "LT or L2" || xbuttons[x2].ToString() == "LT or L2") && (xbuttons[x1].ToString() == "RS Right" || xbuttons[x2].ToString() == "RS Right"))
                                {
                                    SetCombinedXboxTrigger(0, disPushed);
                                    if (!(toggleXbox.Contains("RS Up") == true) && !(toggleXbox.Contains("RS Down") == true) && !(toggleXbox.Contains("RS Left") == true))
                                    {
                                        MoveCombinedXboxRightStick(2, disPushed, false);
                                    }
                                    SimGamePad.Instance.Update(1);
                                }
                            }
                            else if (x1Variable || x2Variable)
                            {
                                string currentOption = buttonOptions[i].ToString();
                                if (IsCurrentVariableTokenIteration(currentOption, xbuttons[x1].ToString(), x1Variable, xbuttons[x2].ToString(), x2Variable, IsXboxVariableEligibleToken))
                                {
                                    continue;
                                }

                                if (nonVariable.Contains(xbuttons[x1].ToString()) == false && nonVariable.Contains(xbuttons[x2].ToString()) == false)
                                {
                                    if ((xbuttons[x1].ToString() == "RS Right" && x1Variable && xbuttons[x2].ToString() == buttonOptions[i].ToString()) || (xbuttons[x2].ToString() == "RS Right" && x2Variable && xbuttons[x1].ToString() == buttonOptions[i].ToString()))
                                    {
                                        if (!(toggleXbox.Contains("RS Up") == true) && !(toggleXbox.Contains("RS Down") == true) && !(toggleXbox.Contains("RS Left") == true))
                                        {
                                            MoveCombinedXboxRightStick(2, disPushed, false);
                                            SimGamePad.Instance.Update(1);
                                        }
                                        SimGamePad.Instance.SetControl(i, 1);
                                    }

                                    else if ((xbuttons[x1].ToString() == "RS Left" && x1Variable && xbuttons[x2].ToString() == buttonOptions[i].ToString()) || (xbuttons[x2].ToString() == "RS Left" && x2Variable && xbuttons[x1].ToString() == buttonOptions[i].ToString()))
                                    {
                                        if (!(toggleXbox.Contains("RS Up") == true) && !(toggleXbox.Contains("RS Down") == true) && !(toggleXbox.Contains("RS Right") == true))
                                        {
                                            MoveCombinedXboxRightStick(2, disPushed, true);
                                            SimGamePad.Instance.Update(1);
                                        }
                                        SimGamePad.Instance.SetControl(i, 1);
                                    }
                                    else if ((xbuttons[x1].ToString() == "RS Up" && x1Variable && xbuttons[x2].ToString() == buttonOptions[i].ToString()) || (xbuttons[x2].ToString() == "RS Up" && x2Variable && xbuttons[x1].ToString() == buttonOptions[i].ToString()))
                                    {
                                        if (!(toggleXbox.Contains("RS Down") == true) && !(toggleXbox.Contains("RS Right") == true) && !(toggleXbox.Contains("RS Left") == true))
                                        {
                                            MoveCombinedXboxRightStick(3, disPushed, false);
                                            SimGamePad.Instance.Update(1);
                                        }
                                        SimGamePad.Instance.SetControl(i, 1);
                                    }
                                    else if ((xbuttons[x1].ToString() == "RS Down" && x1Variable && xbuttons[x2].ToString() == buttonOptions[i].ToString()) || (xbuttons[x2].ToString() == "RS Down" && x2Variable && xbuttons[x1].ToString() == buttonOptions[i].ToString()))
                                    {
                                        if (!(toggleXbox.Contains("RS Up") == true) && !(toggleXbox.Contains("RS Right") == true) && !(toggleXbox.Contains("RS Left") == true))
                                        {

                                            MoveCombinedXboxRightStick(3, disPushed, true);
                                            SimGamePad.Instance.Update(1);
                                        }
                                        SimGamePad.Instance.SetControl(i, 1);
                                    }
                                    else if ((xbuttons[x1].ToString() == "LS Right" && x1Variable && xbuttons[x2].ToString() == buttonOptions[i].ToString()) || (xbuttons[x2].ToString() == "LS Right" && x2Variable && xbuttons[x1].ToString() == buttonOptions[i].ToString()))
                                    {
                                        if (!(toggleXbox.Contains("LS Up") == true) && !(toggleXbox.Contains("LS Down") == true) && !(toggleXbox.Contains("LS Left") == true))
                                        {
                                            MoveCombinedXboxLeftStick(0, disPushed, false);
                                            SimGamePad.Instance.Update(1);
                                        }
                                        SimGamePad.Instance.SetControl(i, 1);
                                    }
                                    else if ((xbuttons[x1].ToString() == "LS Left" && x1Variable && xbuttons[x2].ToString() == buttonOptions[i].ToString()) || (xbuttons[x2].ToString() == "LS Left" && x2Variable && xbuttons[x1].ToString() == buttonOptions[i].ToString()))
                                    {
                                        if (!(toggleXbox.Contains("LS Up") == true) && !(toggleXbox.Contains("LS Down") == true) && !(toggleXbox.Contains("LS Right") == true))
                                        {
                                            MoveCombinedXboxLeftStick(0, disPushed, true);
                                            SimGamePad.Instance.Update(1);
                                        }
                                        SimGamePad.Instance.SetControl(i, 1);
                                    }
                                    else if ((xbuttons[x1].ToString() == "LS Up" && x1Variable && xbuttons[x2].ToString() == buttonOptions[i].ToString()) || (xbuttons[x2].ToString() == "LS Up" && x2Variable && xbuttons[x1].ToString() == buttonOptions[i].ToString()))
                                    {
                                        if (!(toggleXbox.Contains("LS Down") == true) && !(toggleXbox.Contains("LS Right") == true) && !(toggleXbox.Contains("LS Left") == true))
                                        {
                                            MoveCombinedXboxLeftStick(1, disPushed, false);
                                            SimGamePad.Instance.Update(1);
                                        }
                                        SimGamePad.Instance.SetControl(i, 1);
                                    }
                                    else if ((xbuttons[x1].ToString() == "LS Down" && x1Variable && xbuttons[x2].ToString() == buttonOptions[i].ToString()) || (xbuttons[x2].ToString() == "LS Down" && x2Variable && xbuttons[x1].ToString() == buttonOptions[i].ToString()))
                                    {
                                        if (!(toggleXbox.Contains("LS Up") == true) && !(toggleXbox.Contains("LS Right") == true) && !(toggleXbox.Contains("LS Left") == true))
                                        {
                                            MoveCombinedXboxLeftStick(1, disPushed, true);
                                            SimGamePad.Instance.Update(1);
                                        }
                                        SimGamePad.Instance.SetControl(i, 1);
                                    }
                                    else if ((xbuttons[x1].ToString() == "LT or L2" && x1Variable && xbuttons[x2].ToString() == buttonOptions[i].ToString()) || (xbuttons[x2].ToString() == "LT or L2" && x2Variable && xbuttons[x1].ToString() == buttonOptions[i].ToString()))
                                    {
                                        SetCombinedXboxTrigger(0, disPushed);
                                        SimGamePad.Instance.Update(1);
                                        SimGamePad.Instance.SetControl(i, 1);
                                    }
                                    else if ((xbuttons[x1].ToString() == "RT or R2" && x1Variable && xbuttons[x2].ToString() == buttonOptions[i].ToString()) || (xbuttons[x2].ToString() == "RT or R2" && x2Variable && xbuttons[x1].ToString() == buttonOptions[i].ToString()))
                                    {
                                        SetCombinedXboxTrigger(1, disPushed);
                                        SimGamePad.Instance.Update(1);
                                        SimGamePad.Instance.SetControl(i, 1);
                                    }
                                    else
                                    {

                                        SimGamePad.Instance.SetControl(i, 1);
                                    }

                                }
                                else
                                {

                                    if (xbuttons[x2].ToString() == "RT or R2" && nonVariable.Contains(xbuttons[x1].ToString()) == true && xbuttons[x1].ToString() == buttonOptions[i].ToString())
                                    {
                                        SetCombinedXboxTrigger(1, disPushed);
                                        SimGamePad.Instance.Update(1);
                                        SimGamePad.Instance.SetControl(i, 1);
                                    }

                                    else if (xbuttons[x2].ToString() == "LT or L2" && nonVariable.Contains(xbuttons[x1].ToString()) == true && xbuttons[x1].ToString() == buttonOptions[i].ToString())
                                    {
                                        SetCombinedXboxTrigger(0, disPushed);
                                        SimGamePad.Instance.Update(1);
                                        SimGamePad.Instance.SetControl(i, 1);

                                    }

                                    else if (xbuttons[x2].ToString() == "RS Up" && nonVariable.Contains(xbuttons[x1].ToString()) == true && xbuttons[x1].ToString() == buttonOptions[i].ToString())
                                    {
                                        if (!(toggleXbox.Contains("RS Down") == true) && !(toggleXbox.Contains("RS Right") == true) && !(toggleXbox.Contains("RS Left") == true))
                                        {
                                            MoveCombinedXboxRightStick(3, disPushed, false);
                                            SimGamePad.Instance.Update(1);
                                        }
                                        SimGamePad.Instance.SetControl(i, 1);

                                    }

                                    else if (xbuttons[x2].ToString() == "RS Right" && nonVariable.Contains(xbuttons[x1].ToString()) == true && xbuttons[x1].ToString() == buttonOptions[i].ToString())
                                    {
                                        if (!(toggleXbox.Contains("RS Up") == true) && !(toggleXbox.Contains("RS Down") == true) && !(toggleXbox.Contains("RS Left") == true))
                                        {
                                            MoveCombinedXboxRightStick(2, disPushed, false);
                                            SimGamePad.Instance.Update(1);
                                        }
                                        SimGamePad.Instance.SetControl(i, 1);

                                    }
                                    else if (xbuttons[x2].ToString() == "RS Down" && nonVariable.Contains(xbuttons[x1].ToString()) == true && xbuttons[x1].ToString() == buttonOptions[i].ToString())
                                    {
                                        if (!(toggleXbox.Contains("RS Up") == true) && !(toggleXbox.Contains("RS Right") == true) && !(toggleXbox.Contains("RS Left") == true))
                                        {
                                            MoveCombinedXboxRightStick(3, disPushed, true);
                                            SimGamePad.Instance.Update(1);
                                        }
                                        SimGamePad.Instance.SetControl(i, 1);

                                    }
                                    else if (xbuttons[x2].ToString() == "RS Left" && nonVariable.Contains(xbuttons[x1].ToString()) == true && xbuttons[x1].ToString() == buttonOptions[i].ToString())
                                    {
                                        if (!(toggleXbox.Contains("RS Up") == true) && !(toggleXbox.Contains("RS Down") == true) && !(toggleXbox.Contains("RS Right") == true))
                                        {
                                            MoveCombinedXboxRightStick(2, disPushed, true);
                                            SimGamePad.Instance.Update(1);
                                        }
                                        SimGamePad.Instance.SetControl(i, 1);

                                    }
                                    else if (xbuttons[x2].ToString() == "LS Up" && nonVariable.Contains(xbuttons[x1].ToString()) == true && xbuttons[x1].ToString() == buttonOptions[i].ToString())
                                    {
                                        if (!(toggleXbox.Contains("LS Down") == true) && !(toggleXbox.Contains("LS Right") == true) && !(toggleXbox.Contains("LS Left") == true))
                                        {
                                            MoveCombinedXboxLeftStick(1, disPushed, false);
                                            SimGamePad.Instance.Update(1);
                                        }
                                        SimGamePad.Instance.SetControl(i, 1);

                                    }
                                    else if (xbuttons[x2].ToString() == "LS Right" && nonVariable.Contains(xbuttons[x1].ToString()) == true && xbuttons[x1].ToString() == buttonOptions[i].ToString())
                                    {
                                        if (!(toggleXbox.Contains("LS Up") == true) && !(toggleXbox.Contains("LS Down") == true) && !(toggleXbox.Contains("LS Left") == true))
                                        {

                                            MoveCombinedXboxLeftStick(0, disPushed, false);
                                            SimGamePad.Instance.Update(1);
                                        }
                                        SimGamePad.Instance.SetControl(i, 1);

                                    }
                                    else if (xbuttons[x2].ToString() == "LS Down" && nonVariable.Contains(xbuttons[x1].ToString()) == true && xbuttons[x1].ToString() == buttonOptions[i].ToString())
                                    {
                                        if (!(toggleXbox.Contains("LS Up") == true) && !(toggleXbox.Contains("LS Right") == true) && !(toggleXbox.Contains("LS Left") == true))
                                        {
                                            MoveCombinedXboxLeftStick(1, disPushed, true);
                                            SimGamePad.Instance.Update(1);
                                        }
                                        SimGamePad.Instance.SetControl(i, 1);

                                    }
                                    else if (xbuttons[x2].ToString() == "LS Left" && nonVariable.Contains(xbuttons[x1].ToString()) == true && xbuttons[x1].ToString() == buttonOptions[i].ToString())
                                    {
                                        if (!(toggleXbox.Contains("LS Up") == true) && !(toggleXbox.Contains("LS Down") == true) && !(toggleXbox.Contains("LS Right") == true))
                                        {
                                            MoveCombinedXboxLeftStick(0, disPushed, true);
                                            SimGamePad.Instance.Update(1);
                                        }
                                        SimGamePad.Instance.SetControl(i, 1);

                                    }


                                    else if (xbuttons[x1].ToString() == "RT or R2" && nonVariable.Contains(xbuttons[x2].ToString()) == true && xbuttons[x2].ToString() == buttonOptions[i].ToString())
                                    {
                                        SetCombinedXboxTrigger(1, disPushed);
                                        SimGamePad.Instance.Update(1);
                                        SimGamePad.Instance.SetControl(i, 1);
                                    }

                                    else if (xbuttons[x1].ToString() == "LT or L2" && nonVariable.Contains(xbuttons[x2].ToString()) == true && xbuttons[x2].ToString() == buttonOptions[i].ToString())
                                    {
                                        SetCombinedXboxTrigger(0, disPushed);
                                        SimGamePad.Instance.Update(1);
                                        SimGamePad.Instance.SetControl(i, 1);

                                    }

                                    else if (xbuttons[x1].ToString() == "RS Up" && nonVariable.Contains(xbuttons[x2].ToString()) == true && xbuttons[x2].ToString() == buttonOptions[i].ToString())
                                    {
                                        if (!(toggleXbox.Contains("RS Down") == true) && !(toggleXbox.Contains("RS Right") == true) && !(toggleXbox.Contains("RS Left") == true))
                                        {
                                            MoveCombinedXboxRightStick(3, disPushed, false);
                                            SimGamePad.Instance.Update(1);
                                        }
                                        SimGamePad.Instance.SetControl(i, 1);

                                    }

                                    else if (xbuttons[x1].ToString() == "RS Right" && nonVariable.Contains(xbuttons[x2].ToString()) == true && xbuttons[x2].ToString() == buttonOptions[i].ToString())
                                    {
                                        if (!(toggleXbox.Contains("RS Up") == true) && !(toggleXbox.Contains("RS Down") == true) && !(toggleXbox.Contains("RS Left") == true))
                                        {
                                            MoveCombinedXboxRightStick(2, disPushed, false);
                                            SimGamePad.Instance.Update(1);
                                        }
                                        SimGamePad.Instance.SetControl(i, 1);

                                    }
                                    else if (xbuttons[x1].ToString() == "RS Down" && nonVariable.Contains(xbuttons[x2].ToString()) == true && xbuttons[x2].ToString() == buttonOptions[i].ToString())
                                    {
                                        if (!(toggleXbox.Contains("RS Up") == true) && !(toggleXbox.Contains("RS Right") == true) && !(toggleXbox.Contains("RS Left") == true))
                                        {
                                            MoveCombinedXboxRightStick(3, disPushed, true);
                                            SimGamePad.Instance.Update(1);
                                        }
                                        SimGamePad.Instance.SetControl(i, 1);

                                    }
                                    else if (xbuttons[x1].ToString() == "RS Left" && nonVariable.Contains(xbuttons[x2].ToString()) == true && xbuttons[x2].ToString() == buttonOptions[i].ToString())
                                    {
                                        if (!(toggleXbox.Contains("RS Up") == true) && !(toggleXbox.Contains("RS Down") == true) && !(toggleXbox.Contains("RS Right") == true))
                                        {
                                            SimGamePad.Instance.MoveSticks(2, disPushed, true);
                                            SimGamePad.Instance.Update(1);
                                        }
                                        SimGamePad.Instance.SetControl(i, 1);

                                    }
                                    else if (xbuttons[x1].ToString() == "LS Up" && nonVariable.Contains(xbuttons[x2].ToString()) == true && xbuttons[x2].ToString() == buttonOptions[i].ToString())
                                    {
                                        if (!(toggleXbox.Contains("LS Down") == true) && !(toggleXbox.Contains("LS Right") == true) && !(toggleXbox.Contains("LS Left") == true))
                                        {
                                            MoveCombinedXboxLeftStick(1, disPushed, false);
                                            SimGamePad.Instance.Update(1);
                                        }
                                        SimGamePad.Instance.SetControl(i, 1);

                                    }
                                    else if (xbuttons[x1].ToString() == "LS Right" && nonVariable.Contains(xbuttons[x2].ToString()) == true && xbuttons[x2].ToString() == buttonOptions[i].ToString())
                                    {
                                        if (!(toggleXbox.Contains("LS Up") == true) && !(toggleXbox.Contains("LS Down") == true) && !(toggleXbox.Contains("LS Left") == true))
                                        {
                                            MoveCombinedXboxLeftStick(0, disPushed, false);
                                            SimGamePad.Instance.Update(1);
                                        }
                                        SimGamePad.Instance.SetControl(i, 1);

                                    }
                                    else if (xbuttons[x1].ToString() == "LS Down" && nonVariable.Contains(xbuttons[x2].ToString()) == true && xbuttons[x2].ToString() == buttonOptions[i].ToString())
                                    {
                                        if (!(toggleXbox.Contains("LS Up") == true) && !(toggleXbox.Contains("LS Right") == true) && !(toggleXbox.Contains("LS Left") == true))
                                        {
                                            MoveCombinedXboxLeftStick(1, disPushed, true);
                                            SimGamePad.Instance.Update(1);
                                        }
                                        SimGamePad.Instance.SetControl(i, 1);

                                    }
                                    else if (xbuttons[x1].ToString() == "LS Left" && nonVariable.Contains(xbuttons[x2].ToString()) == true && xbuttons[x2].ToString() == buttonOptions[i].ToString())
                                    {
                                        if (!(toggleXbox.Contains("LS Up") == true) && !(toggleXbox.Contains("LS Down") == true) && !(toggleXbox.Contains("LS Right") == true))
                                        {
                                            MoveCombinedXboxLeftStick(0, disPushed, true);
                                            SimGamePad.Instance.Update(1);
                                        }
                                        SimGamePad.Instance.SetControl(i, 1);

                                    }
                                    else
                                    {

                                        SimGamePad.Instance.SetControl(i, 1);
                                    }
                                }
                            }
                            else if (!x1Variable && !x2Variable)

                            {
                                if (!(toggleXbox.Contains("LS Up") == true) && !(toggleXbox.Contains("LS Down") == true) && !(toggleXbox.Contains("LS Right") == true) && !(toggleXbox.Contains("LS Left") == true))
                                {
                                    if (((xbuttons[x1].ToString() == "LS Down" && xbuttons[x2].ToString() == "LS Right") || (xbuttons[x2].ToString() == "LS Down" && xbuttons[x1].ToString() == "LS Right")))
                                    {
                                        MoveCombinedXboxLeftStick(0, 1, false);
                                        MoveCombinedXboxLeftStick(1, 1, true);
                                        SimGamePad.Instance.Update(1);
                                    }
                                    else if (((xbuttons[x1].ToString() == "LS Up" && xbuttons[x2].ToString() == "LS Right") || (xbuttons[x2].ToString() == "LS Up" && xbuttons[x1].ToString() == "LS Right")))
                                    {
                                        MoveCombinedXboxLeftStick(0, 1, false);
                                        MoveCombinedXboxLeftStick(1, 1, false);
                                        SimGamePad.Instance.Update(1);
                                    }
                                    else if (((xbuttons[x1].ToString() == "LS Down" && xbuttons[x2].ToString() == "LS Left") || (xbuttons[x2].ToString() == "LS Down" && xbuttons[x1].ToString() == "LS Left")))
                                    {
                                        MoveCombinedXboxLeftStick(0, 1, true);
                                        MoveCombinedXboxLeftStick(1, 1, true);
                                        SimGamePad.Instance.Update(1);
                                    }
                                    else if (((xbuttons[x1].ToString() == "LS Up" && xbuttons[x2].ToString() == "LS Left") || (xbuttons[x2].ToString() == "LS Up" && xbuttons[x1].ToString() == "LS Left")))
                                    {
                                        MoveCombinedXboxLeftStick(0, 1, true);
                                        MoveCombinedXboxLeftStick(1, 1, false);
                                        SimGamePad.Instance.Update(1);
                                    }
                                    else
                                    {
                                        SimGamePad.Instance.SetControl(i, 1);
                                    }
                                }
                                else if (!(toggleXbox.Contains("RS Up") == true) && !(toggleXbox.Contains("RS Down") == true) && !(toggleXbox.Contains("RS Right") == true) && !(toggleXbox.Contains("RS Left") == true))
                                {
                                    if (((xbuttons[x1].ToString() == "RS Down" && xbuttons[x2].ToString() == "RS Right") || (xbuttons[x2].ToString() == "RS Down" && xbuttons[x1].ToString() == "RS Right")))
                                    {
                                        MoveCombinedXboxRightStick(2, 1, false);
                                        MoveCombinedXboxRightStick(3, 1, true);
                                        SimGamePad.Instance.Update(1);
                                    }
                                    else if (((xbuttons[x1].ToString() == "RS Up" && xbuttons[x2].ToString() == "RS Right") || (xbuttons[x2].ToString() == "RS Up" && xbuttons[x1].ToString() == "RS Right")))
                                    {
                                        MoveCombinedXboxRightStick(2, 1, false);
                                        MoveCombinedXboxRightStick(3, 1, false);
                                        SimGamePad.Instance.Update(1);
                                    }
                                    else if (((xbuttons[x1].ToString() == "RS Down" && xbuttons[x2].ToString() == "RS Left") || (xbuttons[x2].ToString() == "RS Down" && xbuttons[x1].ToString() == "RS Left")))
                                    {
                                        MoveCombinedXboxRightStick(2, 1, true);
                                        MoveCombinedXboxRightStick(3, 1, true);
                                        SimGamePad.Instance.Update(1);
                                    }
                                    else if (((xbuttons[x1].ToString() == "RS Up" && xbuttons[x2].ToString() == "RS Left") || (xbuttons[x2].ToString() == "RS Up" && xbuttons[x1].ToString() == "RS Left")))
                                    {
                                        MoveCombinedXboxRightStick(2, 1, true);
                                        MoveCombinedXboxRightStick(3, 1, false);
                                        SimGamePad.Instance.Update(1);
                                    }
                                    else
                                    {
                                        SimGamePad.Instance.SetControl(i, 1);
                                    }
                                }
                                else
                                {
                                    SimGamePad.Instance.SetControl(i, 1);
                                }
                            }

                        }

                    }
                }

            }

            GamepadMouseX2 = mousePosX;
            GamepadMouseY2 = mousePosY;
            oldQuadrant = x0;


        }

        private void ApplySwitchVariableTarget(OutputTarget target, VariableInputMode mode, bool resetButtons = true)
        {
            int s0 = target.PromptIndex;
            int s1 = target.PrimaryIndex;
            int s2 = target.SecondaryIndex;
            VariableInputState variableState = CreateVariableInputState(s0, target.Distance, mode == VariableInputMode.Click);
            GamepadMouseX1 = mousePosX;
            GamepadMouseY1 = mousePosY;
            double disPushed = variableState.PercentMagnitude;
            bool s1Variable = IsResolvedSwitchVariableTarget(s0, s1, switchbuttons[s1].ToString());
            bool s2Variable = IsResolvedSwitchVariableTarget(s0, s2, switchbuttons[s2].ToString());

            LogMouseMoveDebug(
                "ApplySwitchVariableTarget",
                $"mode={mode} prompt={s0} primary={s1}:{switchbuttons[s1]} secondary={s2}:{switchbuttons[s2]} s1Variable={s1Variable} s2Variable={s2Variable} percent={disPushed:F2} shouldProcess={variableState.ShouldProcess} adjacent={variableState.AdjacentHover} resetButtons={resetButtons}");

            if (variableState.ShouldProcess)
            {
                bool adjacentHover = variableState.AdjacentHover;
                if (!adjacentHover)
                {
                    if (resetButtons)
                    {
                        PressSwitchTokens(true, s0);
                    }

                    if (mode == VariableInputMode.Hover)
                    {
                        if (switchbuttons.Contains("LS Up") || switchbuttons.Contains("LS Right") || switchbuttons.Contains("LS Down") || switchbuttons.Contains("LS Left"))
                        {
                            if (switchbuttons[s0].ToString() != "LS Up" && switchbuttons[s0].ToString() != "LS Right" && switchbuttons[s0].ToString() != "LS Down" && switchbuttons[s0].ToString() != "LS Left")
                            {
                                if (!toggleSwitch.Contains("LS Up") &&
                                                 !toggleSwitch.Contains("LS Right") &&
                                                 !toggleSwitch.Contains("LS Down") &&
                                                 !toggleSwitch.Contains("LS Left"))
                                {
                                    StopCombinedSwitchLeftStick();
                                }
                            }
                        }

                        if (switchbuttons.Contains("RS Up") || switchbuttons.Contains("RS Right") || switchbuttons.Contains("RS Down") || switchbuttons.Contains("RS Left"))
                        {
                            if (switchbuttons[s0].ToString() != "RS Up" && switchbuttons[s0].ToString() != "RS Right" && switchbuttons[s0].ToString() != "RS Down" && switchbuttons[s0].ToString() != "RS Left")
                            {
                                if (!toggleSwitch.Contains("RS Up") &&
                                                 !toggleSwitch.Contains("RS Right") &&
                                                 !toggleSwitch.Contains("RS Down") &&
                                                 !toggleSwitch.Contains("RS Left"))
                                {
                                    if (chkCombineRS.Checked)
                                    {
                                        StopCombinedSwitchRightStick();
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    PressSwitchTokens(true, s1, s2);

                    if (mode == VariableInputMode.Hover)
                    {

                        if (switchbuttons.Contains("LS Up") || switchbuttons.Contains("LS Right") || switchbuttons.Contains("LS Down") || switchbuttons.Contains("LS Left"))
                        {

                            if (!toggleSwitch.Contains("LS Up") &&
                                             !toggleSwitch.Contains("LS Right") &&
                                             !toggleSwitch.Contains("LS Down") &&
                                             !toggleSwitch.Contains("LS Left"))
                            {
                                StopCombinedSwitchLeftStick();
                            }

                        }

                        if (switchbuttons.Contains("RS Up") || switchbuttons.Contains("RS Right") || switchbuttons.Contains("RS Down") || switchbuttons.Contains("RS Left"))
                        {

                            if (!toggleSwitch.Contains("RS Up") &&
                                             !toggleSwitch.Contains("RS Right") &&
                                             !toggleSwitch.Contains("RS Down") &&
                                             !toggleSwitch.Contains("RS Left"))
                            {
                                StopCombinedSwitchRightStick();
                            }

                        }

                    }
                }

                if (s1 == s2)
                {

                    if (switchbuttons[s1].ToString() == "RS Up" && s1Variable)
                    {
                        if (!(toggleSwitch.Contains("RS Down") == true) && !(toggleSwitch.Contains("RS Right") == true) && !(toggleSwitch.Contains("RS Left") == true))
                        {
                            MoveCombinedSwitchRightStick("up", disPushed);
                        }
                    }

                    else if (switchbuttons[s1].ToString() == "RS Right" && s1Variable)
                    {
                        if (!(toggleSwitch.Contains("RS Up") == true) && !(toggleSwitch.Contains("RS Down") == true) && !(toggleSwitch.Contains("RS Left") == true))
                        {
                            MoveCombinedSwitchRightStick("right", disPushed);
                        }
                    }
                    else if (switchbuttons[s1].ToString() == "RS Down" && s1Variable)
                    {
                        if (!(toggleSwitch.Contains("RS Up") == true) && !(toggleSwitch.Contains("RS Right") == true) && !(toggleSwitch.Contains("RS Left") == true))
                        {
                            MoveCombinedSwitchRightStick("down", disPushed);
                        }
                    }
                    else if (switchbuttons[s1].ToString() == "RS Left" && s1Variable)
                    {
                        if (!(toggleSwitch.Contains("RS Up") == true) && !(toggleSwitch.Contains("RS Down") == true) && !(toggleSwitch.Contains("RS Right") == true))
                        {
                            MoveCombinedSwitchRightStick("left", disPushed);
                        }
                    }
                    else if (switchbuttons[s1].ToString() == "LS Up" && s1Variable)
                    {
                        if (!(toggleSwitch.Contains("LS Down") == true) && !(toggleSwitch.Contains("LS Right") == true) && !(toggleSwitch.Contains("LS Left") == true))
                        {
                            MoveCombinedSwitchLeftStick("up", disPushed);
                        }

                    }
                    else if (switchbuttons[s1].ToString() == "LS Right" && s1Variable)
                    {
                        if (!(toggleSwitch.Contains("LS Up") == true) && !(toggleSwitch.Contains("LS Down") == true) && !(toggleSwitch.Contains("LS Left") == true))
                        {
                            MoveCombinedSwitchLeftStick("right", disPushed);
                        }

                    }
                    else if (switchbuttons[s1].ToString() == "LS Down" && s1Variable)
                    {
                        if (!(toggleSwitch.Contains("LS Up") == true) && !(toggleSwitch.Contains("LS Right") == true) && !(toggleSwitch.Contains("LS Left") == true))
                        {
                            MoveCombinedSwitchLeftStick("down", disPushed);
                        }

                    }
                    else if (switchbuttons[s1].ToString() == "LS Left" && s1Variable)
                    {
                        if (!(toggleSwitch.Contains("LS Up") == true) && !(toggleSwitch.Contains("LS Down") == true) && !(toggleSwitch.Contains("LS Right") == true))
                        {
                            MoveCombinedSwitchLeftStick("left", disPushed);
                        }

                    }
                    else
                    {
                        PressSwitchToken(switchbuttons[s1].ToString(), false);
                        if (!(toggleSwitch.Contains("LS Up") == true) && !(toggleSwitch.Contains("LS Down") == true) && !(toggleSwitch.Contains("LS Right") == true) && !(toggleSwitch.Contains("LS Left") == true))
                        {
                            if (switchbuttons[s1].ToString() == "LS Up") { MoveCombinedSwitchLeftStick("up", 100); }
                            if (switchbuttons[s1].ToString() == "LS Right") { MoveCombinedSwitchLeftStick("right", 100); }
                            if (switchbuttons[s1].ToString() == "LS Down") { MoveCombinedSwitchLeftStick("down", 100); }
                            if (switchbuttons[s1].ToString() == "LS Left") { MoveCombinedSwitchLeftStick("left", 100); }
                        }
                        if (!(toggleSwitch.Contains("RS Up") == true) && !(toggleSwitch.Contains("RS Down") == true) && !(toggleSwitch.Contains("RS Right") == true) && !(toggleSwitch.Contains("RS Left") == true))
                        {
                            if (switchbuttons[s1].ToString() == "RS Up") { MoveCombinedSwitchRightStick("up", 100); }
                            if (switchbuttons[s1].ToString() == "RS Right") { MoveCombinedSwitchRightStick("right", 100); }
                            if (switchbuttons[s1].ToString() == "RS Down") { MoveCombinedSwitchRightStick("down", 100); }
                            if (switchbuttons[s1].ToString() == "RS Left") { MoveCombinedSwitchRightStick("left", 100); }
                        }
                    }
                }

                else
                {
                    if (s1Variable && s2Variable)
                    {
                        if ((switchbuttons[s1].ToString() == "RS Right" || switchbuttons[s2].ToString() == "RS Right") && (switchbuttons[s1].ToString() == "RS Up" || switchbuttons[s2].ToString() == "RS Up"))
                        {
                            if (!(toggleSwitch.Contains("RS Down") == true) && !(toggleSwitch.Contains("RS Left") == true))
                            {
                                MoveCombinedSwitchRightStickDiagonal(disPushed, 315);
                            }
                        }

                        else if ((switchbuttons[s1].ToString() == "RS Right" || switchbuttons[s2].ToString() == "RS Right") && (switchbuttons[s1].ToString() == "RS Down" || switchbuttons[s2].ToString() == "RS Down"))
                        {
                            if (!(toggleSwitch.Contains("RS Up") == true) && !(toggleSwitch.Contains("RS Left") == true))
                            {
                                MoveCombinedSwitchRightStickDiagonal(disPushed, 45);
                            }
                        }

                        else if ((switchbuttons[s1].ToString() == "RS Left" || switchbuttons[s2].ToString() == "RS Left") && (switchbuttons[s1].ToString() == "RS Down" || switchbuttons[s2].ToString() == "RS Down"))
                        {
                            if (!(toggleSwitch.Contains("RS Up") == true) && !(toggleSwitch.Contains("RS Right") == true))
                            {
                                MoveCombinedSwitchRightStickDiagonal(disPushed, 135);
                            }

                        }

                        else if ((switchbuttons[s1].ToString() == "RS Left" || switchbuttons[s2].ToString() == "RS Left") && (switchbuttons[s1].ToString() == "RS Up" || switchbuttons[s2].ToString() == "RS Up"))
                        {
                            if (!(toggleSwitch.Contains("RS Down") == true) && !(toggleSwitch.Contains("RS Right") == true))
                            {
                                MoveCombinedSwitchRightStickDiagonal(disPushed, 225);
                            }

                        }


                        else if ((switchbuttons[s1].ToString() == "LS Right" || switchbuttons[s2].ToString() == "LS Right") && (switchbuttons[s1].ToString() == "LS Up" || switchbuttons[s2].ToString() == "LS Up"))
                        {
                            if (!(toggleSwitch.Contains("LS Down") == true) && !(toggleSwitch.Contains("LS Left") == true))
                            {
                                MoveCombinedSwitchLeftStickDiagonal(disPushed, 315);
                            }

                        }

                        else if ((switchbuttons[s1].ToString() == "LS Right" || switchbuttons[s2].ToString() == "LS Right") && (switchbuttons[s1].ToString() == "LS Down" || switchbuttons[s2].ToString() == "LS Down"))
                        {
                            if (!(toggleSwitch.Contains("LS Up") == true) && !(toggleSwitch.Contains("LS Left") == true))
                            {

                                MoveCombinedSwitchLeftStickDiagonal(disPushed, 45);
                            }

                        }

                        else if ((switchbuttons[s1].ToString() == "LS Left" || switchbuttons[s2].ToString() == "LS Left") && (switchbuttons[s1].ToString() == "LS Down" || switchbuttons[s2].ToString() == "LS Down"))
                        {
                            if (!(toggleSwitch.Contains("LS Up") == true) && !(toggleSwitch.Contains("LS Right") == true))
                            {
                                MoveCombinedSwitchLeftStickDiagonal(disPushed, 135);
                            }
                        }
                        else if ((switchbuttons[s1].ToString() == "LS Left" || switchbuttons[s2].ToString() == "LS Left") && (switchbuttons[s1].ToString() == "LS Up" || switchbuttons[s2].ToString() == "LS Up"))
                        {
                            if (!(toggleSwitch.Contains("LS Down") == true) && !(toggleSwitch.Contains("LS Right") == true))
                            {
                                MoveCombinedSwitchLeftStickDiagonal(disPushed, 225);
                            }
                        }
                        else if ((switchbuttons[s1].ToString() == "LS Right" || switchbuttons[s2].ToString() == "LS Right") && (switchbuttons[s1].ToString() == "LS Left" || switchbuttons[s2].ToString() == "LS Left"))
                        {
                            if (!(toggleSwitch.Contains("LS Up") == true) && !(toggleSwitch.Contains("LS Down") == true))
                            {
                                StopCombinedSwitchLeftStick();
                            }
                        }
                        else if ((switchbuttons[s1].ToString() == "LS Up" || switchbuttons[s2].ToString() == "LS Up") && (switchbuttons[s1].ToString() == "LS Down" || switchbuttons[s2].ToString() == "LS Down"))
                        {
                            if (!(toggleSwitch.Contains("LS Right") == true) && !(toggleSwitch.Contains("LS Left") == true))
                            {
                                StopCombinedSwitchLeftStick();
                            }
                        }


                        else if ((switchbuttons[s1].ToString() == "LS Left" || switchbuttons[s2].ToString() == "LS Left") && (switchbuttons[s1].ToString() == "RS Left" || switchbuttons[s2].ToString() == "RS Left"))
                        {
                            if (!(toggleSwitch.Contains("LS Up") == true) && !(toggleSwitch.Contains("LS Down") == true) && !(toggleSwitch.Contains("LS Right") == true))
                            {
                                if (!(toggleSwitch.Contains("RS Up") == true) && !(toggleSwitch.Contains("RS Down") == true) && !(toggleSwitch.Contains("RS Right") == true))
                                {
                                    MoveCombinedSwitchRightStick("left", disPushed);
                                    MoveCombinedSwitchLeftStick("left", disPushed);
                                }
                            }
                        }

                        else if ((switchbuttons[s1].ToString() == "LS Right" || switchbuttons[s2].ToString() == "LS Right") && (switchbuttons[s1].ToString() == "RS Right" || switchbuttons[s2].ToString() == "RS Right"))
                        {
                            if (!(toggleSwitch.Contains("LS Up") == true) && !(toggleSwitch.Contains("LS Down") == true) && !(toggleSwitch.Contains("LS Left") == true))
                            {
                                if (!(toggleSwitch.Contains("RS Up") == true) && !(toggleSwitch.Contains("RS Down") == true) && !(toggleSwitch.Contains("RS Left") == true))
                                {
                                    MoveCombinedSwitchRightStick("right", disPushed);
                                    MoveCombinedSwitchLeftStick("right", disPushed);
                                }
                            }
                        }

                        else if ((switchbuttons[s1].ToString() == "LS Up" || switchbuttons[s2].ToString() == "LS Up") && (switchbuttons[s1].ToString() == "RS Up" || switchbuttons[s2].ToString() == "RS Up"))
                        {
                            if (!(toggleSwitch.Contains("LS Down") == true) && !(toggleSwitch.Contains("LS Right") == true) && !(toggleSwitch.Contains("LS Left") == true))
                            {
                                if (!(toggleSwitch.Contains("RS Down") == true) && !(toggleSwitch.Contains("RS Right") == true) && !(toggleSwitch.Contains("RS Left") == true))
                                {
                                    MoveCombinedSwitchRightStick("up", disPushed);
                                    MoveCombinedSwitchLeftStick("up", disPushed);
                                }
                            }
                        }

                        else if ((switchbuttons[s1].ToString() == "LS Down" || switchbuttons[s2].ToString() == "LS Down") && (switchbuttons[s1].ToString() == "RS Down" || switchbuttons[s2].ToString() == "RS Down"))
                        {
                            if (!(toggleSwitch.Contains("LS Up") == true) && !(toggleSwitch.Contains("LS Right") == true) && !(toggleSwitch.Contains("LS Left") == true))
                            {
                                if (!(toggleSwitch.Contains("RS Up") == true) && !(toggleSwitch.Contains("RS Right") == true) && !(toggleSwitch.Contains("RS Left") == true))
                                {
                                    MoveCombinedSwitchRightStick("down", disPushed);
                                    MoveCombinedSwitchLeftStick("down", disPushed);
                                }
                            }
                        }


                    }
                    else if (s1Variable || s2Variable)
                    {
                        int partnerIndex = GetMixedVariablePartnerIndex(s1, s1Variable, s2, s2Variable);

                        if (nonVariable.Contains(switchbuttons[s1].ToString()) == false && nonVariable.Contains(switchbuttons[s2].ToString()) == false)
                        {
                            if ((switchbuttons[s1].ToString() == "RS Right" && s1Variable) || (switchbuttons[s2].ToString() == "RS Right" && s2Variable))
                            {
                                if (!(toggleSwitch.Contains("RS Up") == true) && !(toggleSwitch.Contains("RS Down") == true) && !(toggleSwitch.Contains("RS Left") == true))
                                {
                                    MoveCombinedSwitchRightStick("right", disPushed);
                                }
                                Output.Switch.Press(CreateHotkeyTarget(partnerIndex, partnerIndex, partnerIndex), InputActionBehavior.HoldUntilRelease, trackTargets: false).GetAwaiter().GetResult();

                            }

                            else if ((switchbuttons[s1].ToString() == "RS Left" && s1Variable) || (switchbuttons[s2].ToString() == "RS Left" && s2Variable))
                            {
                                if (!(toggleSwitch.Contains("RS Up") == true) && !(toggleSwitch.Contains("RS Down") == true) && !(toggleSwitch.Contains("RS Right") == true))
                                {
                                    MoveCombinedSwitchRightStick("left", disPushed);
                                }

                                Output.Switch.Press(CreateHotkeyTarget(partnerIndex, partnerIndex, partnerIndex), InputActionBehavior.HoldUntilRelease, trackTargets: false).GetAwaiter().GetResult();
                            }
                            else if ((switchbuttons[s1].ToString() == "RS Up" && s1Variable) || (switchbuttons[s2].ToString() == "RS Up" && s2Variable))
                            {
                                if (!(toggleSwitch.Contains("RS Down") == true) && !(toggleSwitch.Contains("RS Right") == true) && !(toggleSwitch.Contains("RS Left") == true))
                                {
                                    MoveCombinedSwitchRightStick("up", disPushed);
                                }

                                Output.Switch.Press(CreateHotkeyTarget(partnerIndex, partnerIndex, partnerIndex), InputActionBehavior.HoldUntilRelease, trackTargets: false).GetAwaiter().GetResult();
                            }
                            else if ((switchbuttons[s1].ToString() == "RS Down" && s1Variable) || (switchbuttons[s2].ToString() == "RS Down" && s2Variable))
                            {
                                if (!(toggleSwitch.Contains("RS Up") == true) && !(toggleSwitch.Contains("RS Right") == true) && !(toggleSwitch.Contains("RS Left") == true))
                                {
                                    MoveCombinedSwitchRightStick("down", disPushed);
                                }

                                Output.Switch.Press(CreateHotkeyTarget(partnerIndex, partnerIndex, partnerIndex), InputActionBehavior.HoldUntilRelease, trackTargets: false).GetAwaiter().GetResult();
                            }
                            else if ((switchbuttons[s1].ToString() == "LS Right" && s1Variable) || (switchbuttons[s2].ToString() == "LS Right" && s2Variable))
                            {
                                if (!(toggleSwitch.Contains("LS Up") == true) && !(toggleSwitch.Contains("LS Down") == true) && !(toggleSwitch.Contains("LS Left") == true))
                                {
                                    MoveCombinedSwitchLeftStick("right", disPushed);
                                }

                                Output.Switch.Press(CreateHotkeyTarget(partnerIndex, partnerIndex, partnerIndex), InputActionBehavior.HoldUntilRelease, trackTargets: false).GetAwaiter().GetResult();
                            }
                            else if ((switchbuttons[s1].ToString() == "LS Left" && s1Variable) || (switchbuttons[s2].ToString() == "LS Left" && s2Variable))
                            {
                                if (!(toggleSwitch.Contains("LS Up") == true) && !(toggleSwitch.Contains("LS Down") == true) && !(toggleSwitch.Contains("LS Right") == true))
                                {
                                    MoveCombinedSwitchLeftStick("left", disPushed);
                                }

                                Output.Switch.Press(CreateHotkeyTarget(partnerIndex, partnerIndex, partnerIndex), InputActionBehavior.HoldUntilRelease, trackTargets: false).GetAwaiter().GetResult();
                            }
                            else if ((switchbuttons[s1].ToString() == "LS Up" && s1Variable) || (switchbuttons[s2].ToString() == "LS Up" && s2Variable))
                            {
                                if (!(toggleSwitch.Contains("LS Down") == true) && !(toggleSwitch.Contains("LS Right") == true) && !(toggleSwitch.Contains("LS Left") == true))
                                {
                                    MoveCombinedSwitchLeftStick("up", disPushed);
                                }

                                Output.Switch.Press(CreateHotkeyTarget(partnerIndex, partnerIndex, partnerIndex), InputActionBehavior.HoldUntilRelease, trackTargets: false).GetAwaiter().GetResult();
                            }
                            else if ((switchbuttons[s1].ToString() == "LS Down" && s1Variable) || (switchbuttons[s2].ToString() == "LS Down" && s2Variable))
                            {
                                if (!(toggleSwitch.Contains("LS Up") == true) && !(toggleSwitch.Contains("LS Right") == true) && !(toggleSwitch.Contains("LS Left") == true))
                                {
                                    MoveCombinedSwitchLeftStick("down", disPushed);
                                }

                                Output.Switch.Press(CreateHotkeyTarget(partnerIndex, partnerIndex, partnerIndex), InputActionBehavior.HoldUntilRelease, trackTargets: false).GetAwaiter().GetResult();
                            }
                            else
                            {

                                Output.Switch.Press(CreateHotkeyTarget(s1, s1, s1), InputActionBehavior.HoldUntilRelease, trackTargets: false).GetAwaiter().GetResult();
                            }


                        }
                        else
                        {


                            if (switchbuttons[s2].ToString() == "RS Up" && nonVariable.Contains(switchbuttons[s1].ToString()) == true)
                            {
                                if (!(toggleSwitch.Contains("RS Down") == true) && !(toggleSwitch.Contains("RS Right") == true) && !(toggleSwitch.Contains("RS Left") == true))
                                {
                                    MoveCombinedSwitchRightStick("up", disPushed);
                                }

                                Output.Switch.Press(CreateHotkeyTarget(s1, s1, s1), InputActionBehavior.HoldUntilRelease, trackTargets: false).GetAwaiter().GetResult();

                            }

                            else if (switchbuttons[s2].ToString() == "RS Right" && nonVariable.Contains(switchbuttons[s1].ToString()) == true)
                            {
                                if (!(toggleSwitch.Contains("RS Up") == true) && !(toggleSwitch.Contains("RS Down") == true) && !(toggleSwitch.Contains("RS Left") == true))
                                {
                                    MoveCombinedSwitchRightStick("right", disPushed);
                                }

                                Output.Switch.Press(CreateHotkeyTarget(s1, s1, s1), InputActionBehavior.HoldUntilRelease, trackTargets: false).GetAwaiter().GetResult();

                            }
                            else if (switchbuttons[s2].ToString() == "RS Down" && nonVariable.Contains(switchbuttons[s1].ToString()) == true)
                            {
                                if (!(toggleSwitch.Contains("RS Up") == true) && !(toggleSwitch.Contains("RS Right") == true) && !(toggleSwitch.Contains("RS Left") == true))
                                {
                                    MoveCombinedSwitchRightStick("down", disPushed);
                                }

                                Output.Switch.Press(CreateHotkeyTarget(s1, s1, s1), InputActionBehavior.HoldUntilRelease, trackTargets: false).GetAwaiter().GetResult();

                            }
                            else if (switchbuttons[s2].ToString() == "RS Left" && nonVariable.Contains(switchbuttons[s1].ToString()) == true)
                            {
                                if (!(toggleSwitch.Contains("RS Up") == true) && !(toggleSwitch.Contains("RS Down") == true) && !(toggleSwitch.Contains("RS Right") == true))
                                {
                                    MoveCombinedSwitchRightStick("left", disPushed);
                                }

                                Output.Switch.Press(CreateHotkeyTarget(s1, s1, s1), InputActionBehavior.HoldUntilRelease, trackTargets: false).GetAwaiter().GetResult();

                            }
                            else if (switchbuttons[s2].ToString() == "LS Up" && nonVariable.Contains(switchbuttons[s1].ToString()) == true)
                            {
                                if (!(toggleSwitch.Contains("LS Down") == true) && !(toggleSwitch.Contains("LS Right") == true) && !(toggleSwitch.Contains("LS Left") == true))
                                {
                                    MoveCombinedSwitchLeftStick("up", disPushed);
                                }

                                Output.Switch.Press(CreateHotkeyTarget(s1, s1, s1), InputActionBehavior.HoldUntilRelease, trackTargets: false).GetAwaiter().GetResult();

                            }
                            else if (switchbuttons[s2].ToString() == "LS Right" && nonVariable.Contains(switchbuttons[s1].ToString()) == true)
                            {
                                if (!(toggleSwitch.Contains("LS Up") == true) && !(toggleSwitch.Contains("LS Down") == true) && !(toggleSwitch.Contains("LS Left") == true))
                                {
                                    MoveCombinedSwitchLeftStick("right", disPushed);
                                }

                                Output.Switch.Press(CreateHotkeyTarget(s1, s1, s1), InputActionBehavior.HoldUntilRelease, trackTargets: false).GetAwaiter().GetResult();

                            }
                            else if (switchbuttons[s2].ToString() == "LS Down" && nonVariable.Contains(switchbuttons[s1].ToString()) == true)
                            {
                                if (!(toggleSwitch.Contains("LS Up") == true) && !(toggleSwitch.Contains("LS Right") == true) && !(toggleSwitch.Contains("LS Left") == true))
                                {
                                    MoveCombinedSwitchLeftStick("down", disPushed);
                                }

                                Output.Switch.Press(CreateHotkeyTarget(s1, s1, s1), InputActionBehavior.HoldUntilRelease, trackTargets: false).GetAwaiter().GetResult();

                            }
                            else if (switchbuttons[s2].ToString() == "LS Left" && nonVariable.Contains(switchbuttons[s1].ToString()) == true)
                            {
                                if (!(toggleSwitch.Contains("LS Up") == true) && !(toggleSwitch.Contains("LS Down") == true) && !(toggleSwitch.Contains("LS Right") == true))
                                {
                                    MoveCombinedSwitchLeftStick("left", disPushed);
                                }

                                Output.Switch.Press(CreateHotkeyTarget(s1, s1, s1), InputActionBehavior.HoldUntilRelease, trackTargets: false).GetAwaiter().GetResult();

                            }



                            else if (switchbuttons[s1].ToString() == "RS Up" && nonVariable.Contains(switchbuttons[s2].ToString()) == true)
                            {
                                if (!(toggleSwitch.Contains("RS Down") == true) && !(toggleSwitch.Contains("RS Right") == true) && !(toggleSwitch.Contains("RS Left") == true))
                                {
                                    MoveCombinedSwitchRightStick("up", disPushed);
                                }
                                Output.Switch.Press(CreateHotkeyTarget(s2, s2, s2), InputActionBehavior.HoldUntilRelease, trackTargets: false).GetAwaiter().GetResult();

                            }

                            else if (switchbuttons[s1].ToString() == "RS Right" && nonVariable.Contains(switchbuttons[s2].ToString()) == true)
                            {
                                if (!(toggleSwitch.Contains("RS Up") == true) && !(toggleSwitch.Contains("RS Down") == true) && !(toggleSwitch.Contains("RS Left") == true))
                                {
                                    MoveCombinedSwitchRightStick("right", disPushed);
                                }

                                Output.Switch.Press(CreateHotkeyTarget(s2, s2, s2), InputActionBehavior.HoldUntilRelease, trackTargets: false).GetAwaiter().GetResult();

                            }
                            else if (switchbuttons[s1].ToString() == "RS Down" && nonVariable.Contains(switchbuttons[s2].ToString()) == true)
                            {
                                if (!(toggleSwitch.Contains("RS Up") == true) && !(toggleSwitch.Contains("RS Right") == true) && !(toggleSwitch.Contains("RS Left") == true))
                                {
                                    MoveCombinedSwitchRightStick("down", disPushed);
                                }

                                Output.Switch.Press(CreateHotkeyTarget(s2, s2, s2), InputActionBehavior.HoldUntilRelease, trackTargets: false).GetAwaiter().GetResult();

                            }
                            else if (switchbuttons[s1].ToString() == "RS Left" && nonVariable.Contains(switchbuttons[s2].ToString()) == true)
                            {
                                if (!(toggleSwitch.Contains("RS Up") == true) && !(toggleSwitch.Contains("RS Down") == true) && !(toggleSwitch.Contains("RS Right") == true))
                                {
                                    MoveCombinedSwitchRightStick("left", disPushed);
                                }

                                Output.Switch.Press(CreateHotkeyTarget(s2, s2, s2), InputActionBehavior.HoldUntilRelease, trackTargets: false).GetAwaiter().GetResult();

                            }
                            else if (switchbuttons[s1].ToString() == "LS Up" && nonVariable.Contains(switchbuttons[s2].ToString()) == true)
                            {
                                if (!(toggleSwitch.Contains("LS Down") == true) && !(toggleSwitch.Contains("LS Right") == true) && !(toggleSwitch.Contains("LS Left") == true))
                                {
                                    MoveCombinedSwitchLeftStick("up", disPushed);
                                }

                                Output.Switch.Press(CreateHotkeyTarget(s2, s2, s2), InputActionBehavior.HoldUntilRelease, trackTargets: false).GetAwaiter().GetResult();

                            }
                            else if (switchbuttons[s1].ToString() == "LS Right" && nonVariable.Contains(switchbuttons[s2].ToString()) == true)
                            {
                                if (!(toggleSwitch.Contains("LS Up") == true) && !(toggleSwitch.Contains("LS Down") == true) && !(toggleSwitch.Contains("LS Left") == true))
                                {
                                    MoveCombinedSwitchLeftStick("right", disPushed);
                                }

                                Output.Switch.Press(CreateHotkeyTarget(s2, s2, s2), InputActionBehavior.HoldUntilRelease, trackTargets: false).GetAwaiter().GetResult();

                            }
                            else if (switchbuttons[s1].ToString() == "LS Down" && nonVariable.Contains(switchbuttons[s2].ToString()) == true)
                            {
                                if (!(toggleSwitch.Contains("LS Up") == true) && !(toggleSwitch.Contains("LS Right") == true) && !(toggleSwitch.Contains("LS Left") == true))
                                {
                                    MoveCombinedSwitchLeftStick("down", disPushed);
                                }

                                Output.Switch.Press(CreateHotkeyTarget(s2, s2, s2), InputActionBehavior.HoldUntilRelease, trackTargets: false).GetAwaiter().GetResult();

                            }
                            else if (switchbuttons[s1].ToString() == "LS Left" && nonVariable.Contains(switchbuttons[s2].ToString()) == true)
                            {
                                if (!(toggleSwitch.Contains("LS Up") == true) && !(toggleSwitch.Contains("LS Down") == true) && !(toggleSwitch.Contains("LS Right") == true))
                                {
                                    MoveCombinedSwitchLeftStick("left", disPushed);
                                }

                                Output.Switch.Press(CreateHotkeyTarget(s2, s2, s2), InputActionBehavior.HoldUntilRelease, trackTargets: false).GetAwaiter().GetResult();

                            }
                            else
                            {

                                if (((switchbuttons[s1].ToString() == "LS Down" && switchbuttons[s2].ToString() == "LS Right") || (switchbuttons[s2].ToString() == "LS Down" && switchbuttons[s1].ToString() == "LS Right")) && (!(toggleSwitch.Contains("LS Up") == true) && !(toggleSwitch.Contains("LS Left") == true))) { MoveCombinedSwitchLeftStickDiagonal(disPushed, 45); }
                                else if (((switchbuttons[s1].ToString() == "LS Up" && switchbuttons[s2].ToString() == "LS Right") || (switchbuttons[s2].ToString() == "LS Up" && switchbuttons[s1].ToString() == "LS Right")) && (!(toggleSwitch.Contains("LS Down") == true) && !(toggleSwitch.Contains("LS Left") == true))) { MoveCombinedSwitchLeftStickDiagonal(disPushed, 315); }

                                else if (((switchbuttons[s1].ToString() == "LS Down" && switchbuttons[s2].ToString() == "LS Left") || (switchbuttons[s2].ToString() == "LS Down" && switchbuttons[s1].ToString() == "LS Left")) && (!(toggleSwitch.Contains("LS Up") == true) && !(toggleSwitch.Contains("LS Right") == true))) { MoveCombinedSwitchLeftStickDiagonal(disPushed, 135); }
                                else if (((switchbuttons[s1].ToString() == "LS Up" && switchbuttons[s2].ToString() == "LS Left") || (switchbuttons[s2].ToString() == "LS Up" && switchbuttons[s1].ToString() == "LS Left")) && (!(toggleSwitch.Contains("LS Down") == true) && !(toggleSwitch.Contains("LS Right") == true))) { MoveCombinedSwitchLeftStickDiagonal(disPushed, 225); }
                                else { Output.Switch.Press(CreateHotkeyTarget(s1, s1, s2), InputActionBehavior.HoldUntilRelease, trackTargets: false).GetAwaiter().GetResult(); }



                                if (((switchbuttons[s1].ToString() == "RS Down" && switchbuttons[s2].ToString() == "RS Right") || (switchbuttons[s2].ToString() == "RS Down" && switchbuttons[s1].ToString() == "RS Right")) && (!(toggleSwitch.Contains("RS Up") == true) && !(toggleSwitch.Contains("RS Left") == true))) { MoveCombinedSwitchRightStickDiagonal(disPushed, 45); }
                                else if (((switchbuttons[s1].ToString() == "RS Up" && switchbuttons[s2].ToString() == "RS Right") || (switchbuttons[s2].ToString() == "RS Up" && switchbuttons[s1].ToString() == "RS Right")) && (!(toggleSwitch.Contains("RS Down") == true) && !(toggleSwitch.Contains("RS Left") == true))) { MoveCombinedSwitchRightStickDiagonal(disPushed, 315); }
                                else if (((switchbuttons[s1].ToString() == "RS Down" && switchbuttons[s2].ToString() == "RS Left") || (switchbuttons[s2].ToString() == "RS Down" && switchbuttons[s1].ToString() == "RS Left")) && (!(toggleSwitch.Contains("RS Up") == true) && !(toggleSwitch.Contains("RS Right") == true))) { MoveCombinedSwitchRightStickDiagonal(disPushed, 135); }
                                else if (((switchbuttons[s1].ToString() == "RS Up" && switchbuttons[s2].ToString() == "RS Left") || (switchbuttons[s2].ToString() == "RS Up" && switchbuttons[s1].ToString() == "RS Left")) && (!(toggleSwitch.Contains("RS Down") == true) && !(toggleSwitch.Contains("RS Right") == true))) { MoveCombinedSwitchRightStickDiagonal(disPushed, 225); }
                                else { Output.Switch.Press(CreateHotkeyTarget(s1, s1, s2), InputActionBehavior.HoldUntilRelease, trackTargets: false).GetAwaiter().GetResult(); }

                            }
                        }
                    }
                    else if (!s1Variable && !s2Variable)

                    {
                        if (!(toggleSwitch.Contains("LS Up") == true) && !(toggleSwitch.Contains("LS Down") == true) && !(toggleSwitch.Contains("LS Right") == true) && !(toggleSwitch.Contains("LS Left") == true))
                        {
                            if (((switchbuttons[s1].ToString() == "LS Down" && switchbuttons[s2].ToString() == "LS Right") || (switchbuttons[s2].ToString() == "LS Down" && switchbuttons[s1].ToString() == "LS Right"))) { MoveCombinedSwitchLeftStickDiagonal(100, 45); }
                            else if (((switchbuttons[s1].ToString() == "LS Up" && switchbuttons[s2].ToString() == "LS Right") || (switchbuttons[s2].ToString() == "LS Up" && switchbuttons[s1].ToString() == "LS Right"))) { MoveCombinedSwitchLeftStickDiagonal(100, 315); }

                            else if (((switchbuttons[s1].ToString() == "LS Down" && switchbuttons[s2].ToString() == "LS Left") || (switchbuttons[s2].ToString() == "LS Down" && switchbuttons[s1].ToString() == "LS Left"))) { MoveCombinedSwitchLeftStickDiagonal(100, 135); }
                            else if (((switchbuttons[s1].ToString() == "LS Up" && switchbuttons[s2].ToString() == "LS Left") || (switchbuttons[s2].ToString() == "LS Up" && switchbuttons[s1].ToString() == "LS Left"))) { MoveCombinedSwitchLeftStickDiagonal(100, 225); }
                            else { Output.Switch.Press(CreateHotkeyTarget(s1, s1, s2), InputActionBehavior.HoldUntilRelease, trackTargets: false).GetAwaiter().GetResult(); }
                        }

                        else if (!(toggleSwitch.Contains("RS Up") == true) && !(toggleSwitch.Contains("RS Down") == true) && !(toggleSwitch.Contains("RS Right") == true) && !(toggleSwitch.Contains("RS Left") == true))
                        {
                            if (((switchbuttons[s1].ToString() == "RS Down" && switchbuttons[s2].ToString() == "RS Right") || (switchbuttons[s2].ToString() == "RS Down" && switchbuttons[s1].ToString() == "RS Right"))) { MoveCombinedSwitchRightStickDiagonal(100, 45); }
                            else if (((switchbuttons[s1].ToString() == "RS Up" && switchbuttons[s2].ToString() == "RS Right") || (switchbuttons[s2].ToString() == "RS Up" && switchbuttons[s1].ToString() == "RS Right"))) { MoveCombinedSwitchRightStickDiagonal(100, 315); }
                            else if (((switchbuttons[s1].ToString() == "RS Down" && switchbuttons[s2].ToString() == "RS Left") || (switchbuttons[s2].ToString() == "RS Down" && switchbuttons[s1].ToString() == "RS Left"))) { MoveCombinedSwitchRightStickDiagonal(100, 135); }
                            else if (((switchbuttons[s1].ToString() == "RS Up" && switchbuttons[s2].ToString() == "RS Left") || (switchbuttons[s2].ToString() == "RS Up" && switchbuttons[s1].ToString() == "RS Left"))) { MoveCombinedSwitchRightStickDiagonal(100, 225); }
                            else { Output.Switch.Press(CreateHotkeyTarget(s1, s1, s2), InputActionBehavior.HoldUntilRelease, trackTargets: false).GetAwaiter().GetResult(); }
                        }

                    }


                }



            }

            GamepadMouseX2 = mousePosX;
            GamepadMouseY2 = mousePosY;
            oldQuadrant = s0;

        }



        private async Task SyncMouseMoveQuadrantAsync(int promptIndex, bool releaseOutputs)
        {
            if (IsAdjacentHoverQuadrant(promptIndex))
            {
                LogMouseMoveDebug("SyncMouseMoveQuadrantAsync", $"promptIndex={promptIndex} releaseOutputs={releaseOutputs}");
            }

            if (releaseOutputs)
            {
                await ReleaseMouseMoveOutputsAsync(null);
            }

            UpdateMouseMoveToggleState(promptIndex);
        }

        private async Task ReleaseAllMouseMoveOutputsAsync()
        {
            await ReleaseMouseMoveOutputsAsync(null);
        }

        private async Task ReleaseMouseMoveOutputsAsync(int? preservedPromptIndex)
        {
            HashSet<int> preservedActionIndices = GetPreservedMouseMoveToggleActionIndices();

            if (preservedPromptIndex.HasValue)
            {
                var preservedTarget = ResolveMouseMoveReleaseTarget(preservedPromptIndex.Value);
                foreach (int actionIndex in EnumerateMouseMoveActionIndices(preservedTarget.primaryIndex, preservedTarget.secondaryIndex))
                {
                    preservedActionIndices.Add(actionIndex);
                }
            }

            foreach (int promptIndex in GetHoverPromptIndices())
            {
                var hoverTarget = ResolveMouseMoveReleaseTarget(promptIndex);
                foreach (int actionIndex in EnumerateMouseMoveActionIndices(hoverTarget.primaryIndex, hoverTarget.secondaryIndex))
                {
                    if (preservedActionIndices.Contains(actionIndex))
                    {
                        if (IsAdjacentHoverQuadrant(promptIndex))
                        {
                            LogMouseMoveDebug("ReleaseAllMouseMoveOutputsAsync.Skip", $"promptIndex={promptIndex} actionIndex={actionIndex} preserved=true");
                        }

                        continue;
                    }

                    if (IsAdjacentHoverQuadrant(promptIndex))
                    {
                        LogMouseMoveDebug("ReleaseAllMouseMoveOutputsAsync.Release", $"promptIndex={promptIndex} actionIndex={actionIndex}");
                    }

                    OutputTarget target = CreateMouseMoveTarget(actionIndex, actionIndex, actionIndex, 0);

                    if (keyboardMode)
                    {
                        await Output.Keyboard.Release(target, OutputReleaseMode.MouseMove);
                        continue;
                    }

                    if (xboxMode)
                    {
                        await Output.Xbox.Release(target, OutputReleaseMode.MouseMove);
                        continue;
                    }

                    if (switchMode)
                    {
                        await Output.Switch.Release(target, OutputReleaseMode.MouseMove);
                    }
                }
            }

            ResetMouseMoveOnceState();
        }

        private HashSet<int> GetPreservedMouseMoveToggleActionIndices()
        {
            HashSet<int> preserved = new HashSet<int>();

            foreach (int promptIndex in GetHoverPromptIndices())
            {
                if (!buttonPressed[promptIndex] || GetConfiguredInputBehavior(promptIndex, true) != InputActionBehavior.Toggle)
                {
                    continue;
                }

                var hoverTarget = ResolveMouseMoveReleaseTarget(promptIndex);
                foreach (int actionIndex in EnumerateMouseMoveActionIndices(hoverTarget.primaryIndex, hoverTarget.secondaryIndex))
                {
                    preserved.Add(actionIndex);
                }
            }

            return preserved;
        }

        private (int promptIndex, int toggleIndex, int primaryIndex, int secondaryIndex) ResolveMouseMoveReleaseTarget(int promptIndex)
        {
            var target = IsHoverMouseMoveTarget(promptIndex)
                ? ResolveHoverMouseMoveTarget(promptIndex)
                : (promptIndex, promptIndex, promptIndex, promptIndex);

            return NormalizeFpsMouseMoveTarget(chkFPS.Checked, target);
        }

        private static int[] GetHoverPromptIndices()
        {
            return new[] { 0, 1, 2, 3, 4, 5, 6, 7, 24, 27, 30 };
        }

        private void ResetMouseMoveOnceState()
        {
            foreach (int promptIndex in GetHoverPromptIndices())
            {
                oncePressed[promptIndex] = 0;
            }
        }

        private void UpdateMouseMoveToggleState(int promptIndex)
        {
            foreach (int index in GetHoverPromptIndices())
            {
                if (index != promptIndex)
                {
                    activateToggle[index] = true;
                }
            }
        }

        private void ReleaseKeyboardHoverTargets()
        {
            if (!keyboardMode)
            {
                return;
            }

            foreach (VirtualKeyCode keyCode in new[]
            {
                keyCodes[0],
                keyCodes[1],
                keyCodes[2],
                keyCodes[3],
                keyCodes[4],
                keyCodes[5],
                keyCodes[6],
                keyCodes[7],
                keyCodes[24],
                keyCodes[27],
                keyCodes[30]
            })
            {
                if (releaseKBHover.Contains(keyCode))
                {
                    inputSimulator.Keyboard.KeyUp(keyCode);
                }
            }
        }

        private void HotkeyClickLoop(int c1, int c2, int c3, bool isKeyDown)
        {
            ClickThreadMode mode = GetCurrentClickThreadMode();

            if (isKeyDown)
            {
                if (buttonConfig[c1][1] == "true")
                {
                    return;
                }

                PressHotkeyTargets(mode, c1, c2, c3);
                activateToggle[c1] = true;
                buttonPressed[c1] = true;
                SetClickButtonsText(mode, c1, c2, c3, true);
            }

            else
            {
                if (buttonConfig[c1][1] == "true")
                {
                    if (ClickToggleOn(c1))
                    {
                        ApplyToggleOnState(c1, c2, c3);
                    }
                    else if (ClickToggleOff(c1))
                    {
                        ApplyToggleOffState(c1, c2, c3);
                    }

                    return;
                }

                ReleaseHotkeyTargets(mode, c1, c2, c3);
                activateToggle[c1] = true;
                buttonPressed[c1] = false;
                SetClickButtonsIdleText();
            }
        }

        private void ClickLoop(int c1, int c2, int c3, bool skipToggle)
        {
            if (buttonConfig[c1][1] == "true")
            {
                ClickThreadMode mode = GetCurrentClickThreadMode();
                _ = RunToggleBehaviorAsync(
                    c1,
                    () => ClickToggleOn(c1),
                    () => ClickToggleOff(c1),
                    enableAsync: () =>
                    {
                        PressHotkeyTargets(mode, c1, c2, c3);
                        return Task.CompletedTask;
                    },
                    disableAsync: () =>
                    {
                        ReleaseHotkeyTargets(mode, c1, c2, c3);
                        return Task.CompletedTask;
                    },
                    activeTextAsync: () =>
                    {
                        SetClickButtonsText(mode, c1, c2, c3, true);
                        return Task.CompletedTask;
                    },
                    idleTextAsync: () =>
                    {
                        SetClickButtonsIdleText();
                        return Task.CompletedTask;
                    });
            }
            else
            {
                ClickThreadMode clickThreadMode = GetCurrentClickThreadMode();
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                Task.Run(async () => await ClickThread(c1, c2, c3, 190, cancellationTokenSource.Token, skipToggle, clickThreadMode))
                    .ContinueWith(task =>
                    {
                        cancellationTokenSource.Dispose();
                    });
            }
        }

        private const int OncePressDurationMs = 200;

        private enum OutputTargetKind
        {
            Hotkey,
            Click,
            MouseMove
        }

        private enum OutputReleaseMode
        {
            Hotkey,
            Click,
            MouseMove,
            All
        }

        private enum VariableInputMode
        {
            Hover,
            Click
        }

        private readonly struct OutputTarget
        {
            public OutputTarget(OutputTargetKind kind, int promptIndex, int primaryIndex, int secondaryIndex, double distance = 0)
            {
                Kind = kind;
                PromptIndex = promptIndex;
                PrimaryIndex = primaryIndex;
                SecondaryIndex = secondaryIndex;
                Distance = distance;
            }

            public OutputTargetKind Kind { get; }

            public int PromptIndex { get; }

            public int PrimaryIndex { get; }

            public int SecondaryIndex { get; }

            public double Distance { get; }

            public static OutputTarget Hotkey(int promptIndex, int primaryIndex, int secondaryIndex)
            {
                return new OutputTarget(OutputTargetKind.Hotkey, promptIndex, primaryIndex, secondaryIndex);
            }

            public static OutputTarget Click(int promptIndex, int primaryIndex, int secondaryIndex, double distance)
            {
                return new OutputTarget(OutputTargetKind.Click, promptIndex, primaryIndex, secondaryIndex, distance);
            }

            public static OutputTarget MouseMove(int promptIndex, int primaryIndex, int secondaryIndex, double distance)
            {
                return new OutputTarget(OutputTargetKind.MouseMove, promptIndex, primaryIndex, secondaryIndex, distance);
            }
        }

        private readonly struct VariableInputState
        {
            public VariableInputState(double unitMagnitude, bool shouldProcess, bool adjacentHover)
            {
                UnitMagnitude = unitMagnitude;
                PercentMagnitude = unitMagnitude * 100;
                ShouldProcess = shouldProcess;
                AdjacentHover = adjacentHover;
            }

            public double UnitMagnitude { get; }

            public double PercentMagnitude { get; }

            public bool ShouldProcess { get; }

            public bool AdjacentHover { get; }
        }

        private sealed class OutputFacade
        {
            public OutputFacade(Active owner)
            {
                Keyboard = new KeyboardOutput(owner);
                Xbox = new XboxOutput(owner);
                Switch = new SwitchOutput(owner);
            }

            public KeyboardOutput Keyboard { get; }

            public XboxOutput Xbox { get; }

            public SwitchOutput Switch { get; }

        }

        private sealed class KeyboardOutput
        {
            private readonly Active owner;

            public KeyboardOutput(Active owner)
            {
                this.owner = owner;
            }

            public Task Press(OutputTarget target, InputActionBehavior pressMode, CancellationToken cancellationToken = default)
            {
                if (pressMode == InputActionBehavior.Once)
                {
                    return PressOnceAsync(target, cancellationToken);
                }

                PressCore(target);
                return Task.CompletedTask;
            }

            public Task Release(OutputTarget target, OutputReleaseMode releaseMode)
            {
                switch (releaseMode)
                {
                    case OutputReleaseMode.MouseMove:
                        ReleaseMouseMove(target.PrimaryIndex, target.SecondaryIndex);
                        break;

                    case OutputReleaseMode.All:
                    case OutputReleaseMode.Click:
                    case OutputReleaseMode.Hotkey:
                    default:
                        ReleaseCore(target);
                        break;
                }

                return Task.CompletedTask;
            }

            private void PressCore(OutputTarget target)
            {
                foreach (PromptBinding binding in EnumerateTargets(target))
                {
                    if (!owner.CheckKey(binding.KeyCode))
                    {
                        owner.inputSimulator.Keyboard.KeyDown(binding.KeyCode);
                    }
                }
            }

            private async Task PressOnceAsync(OutputTarget target, CancellationToken cancellationToken)
            {
                PressCore(target);
                await Task.Delay(OncePressDurationMs, cancellationToken);

                if (target.Kind == OutputTargetKind.MouseMove)
                {
                    ReleaseMouseMove(target.PrimaryIndex, target.SecondaryIndex);
                    await owner.SetMouseMoveTextAsync(ClickThreadMode.Keyboard, target.PrimaryIndex, target.SecondaryIndex, false);
                    return;
                }

                ReleaseCore(target);
            }

            private void ReleaseMouseMove(int primaryIndex, int secondaryIndex)
            {
                if (primaryIndex == secondaryIndex)
                {
                    ReleaseKey(primaryIndex);
                    return;
                }

                ReleaseKey(primaryIndex);
                ReleaseKey(secondaryIndex);
            }

            private IEnumerable<PromptBinding> EnumerateTargets(OutputTarget target)
            {
                if (target.Kind == OutputTargetKind.MouseMove)
                {
                    return EnumerateMouseMoveActionIndices(target.PrimaryIndex, target.SecondaryIndex)
                        .Select(owner.GetPromptBinding);
                }

                return owner.EnumerateActionIndices(target.PromptIndex, target.PrimaryIndex, target.SecondaryIndex)
                    .Select(owner.GetPromptBinding);
            }

            private void ReleaseCore(OutputTarget target)
            {
                foreach (PromptBinding binding in EnumerateTargets(target))
                {
                    if (owner.CheckKey(binding.KeyCode))
                    {
                        owner.inputSimulator.Keyboard.KeyUp(binding.KeyCode);
                    }
                }
            }

            private void ReleaseKey(int promptIndex)
            {
                PromptBinding binding = owner.GetPromptBinding(promptIndex);
                if (!owner.buttonPressed[promptIndex] && owner.CheckKey(binding.KeyCode))
                {
                    owner.inputSimulator.Keyboard.KeyUp(binding.KeyCode);
                }
            }
        }

        private sealed class XboxOutput
        {
            private readonly Active owner;

            public XboxOutput(Active owner)
            {
                this.owner = owner;
            }

            public Task Press(OutputTarget target, InputActionBehavior pressMode, CancellationToken cancellationToken = default)
            {
                if (pressMode == InputActionBehavior.Once)
                {
                    return PressOnceAsync(target, cancellationToken);
                }

                switch (target.Kind)
                {
                    case OutputTargetKind.Hotkey:
                        AddTargets(target.PrimaryIndex, target.SecondaryIndex);
                        ApplyHotkeyTargets(target.PrimaryIndex, target.SecondaryIndex);
                        break;

                    case OutputTargetKind.Click:
                        AddTargets(target.PrimaryIndex, target.SecondaryIndex);
                        owner.ApplyXboxVariableTarget(target, false);
                        break;

                    case OutputTargetKind.MouseMove:
                        if (pressMode == InputActionBehavior.Toggle)
                        {
                            AddTargets(target.PrimaryIndex, target.SecondaryIndex);
                            ApplyHotkeyTargets(target.PrimaryIndex, target.SecondaryIndex);
                        }
                        else if (owner.UsesDiscreteMouseMoveTargets(target))
                        {
                            owner.LogMouseMoveDebug("XboxOutput.Press.Discrete", owner.DescribeTarget(target));
                            owner.ApplyDiscreteXboxMouseMoveTargets(target, false);
                        }
                        else
                        {
                            owner.ApplyXboxVariableTarget(
                                pressMode == InputActionBehavior.Variable ? target : owner.CreateMaxMouseMoveTarget(target),
                                true);
                        }

                        break;
                }

                return Task.CompletedTask;
            }

            public Task Release(OutputTarget target, OutputReleaseMode releaseMode)
            {
                RemoveTargets(target.PrimaryIndex, target.SecondaryIndex);
                ReleaseTargets(target.PrimaryIndex, target.SecondaryIndex);
                return Task.CompletedTask;
            }

            private async Task PressOnceAsync(OutputTarget target, CancellationToken cancellationToken)
            {
                if (target.Kind == OutputTargetKind.MouseMove)
                {
                    if (owner.UsesDiscreteMouseMoveTargets(target))
                    {
                        owner.LogMouseMoveDebug("XboxOutput.PressOnce.Discrete", owner.DescribeTarget(target));
                        owner.ApplyDiscreteXboxMouseMoveTargets(target, true);
                    }
                    else
                    {
                        owner.ApplyXboxVariableTarget(target, true, false);
                    }

                    await owner.SetMouseMoveTextAsync(ClickThreadMode.Xbox, target.PrimaryIndex, target.SecondaryIndex, false);
                    await Task.Delay(OncePressDurationMs, cancellationToken);
                    ReleaseTargets(target.PrimaryIndex, target.SecondaryIndex);
                    return;
                }

                AddTargets(target.PrimaryIndex, target.SecondaryIndex);
                owner.ApplyXboxVariableTarget(target, false);
                await Task.Delay(OncePressDurationMs, cancellationToken);
                RemoveTargets(target.PrimaryIndex, target.SecondaryIndex);
                ReleaseTargets(target.PrimaryIndex, target.SecondaryIndex);
            }

            private void AddTargets(int primaryIndex, int secondaryIndex)
            {
                foreach (string target in EnumerateDistinctTargets(primaryIndex, secondaryIndex)
                    .Select(index => owner.GetPromptBinding(index).XboxToken))
                {
                    owner.toggleXbox.Add(target);
                }
            }

            private void RemoveTargets(int primaryIndex, int secondaryIndex)
            {
                HashSet<string> targets = new HashSet<string>(EnumerateDistinctTargets(primaryIndex, secondaryIndex)
                    .Select(index => owner.GetPromptBinding(index).XboxToken));
                owner.toggleXbox = new ConcurrentBag<string>(owner.toggleXbox.Where(value => !targets.Contains(value)));
            }

            private void ApplyHotkeyTargets(int primaryIndex, int secondaryIndex)
            {
                owner.ApplyXboxCombinedTargets(primaryIndex, secondaryIndex);
            }

            private void ReleaseTargets(int primaryIndex, int secondaryIndex)
            {
                if (!owner.xboxMode)
                {
                    return;
                }

                for (int i = 0; i < 26; i++)
                {
                    if (owner.xbuttons[primaryIndex].ToString() == owner.buttonOptions[i].ToString() || owner.xbuttons[secondaryIndex].ToString() == owner.buttonOptions[i].ToString())
                    {
                        SimGamePad.Instance.ReleaseControl(i, owner.toggleXbox, 1);
                    }
                }

                if ((owner.xbuttons[primaryIndex].Contains("LS ") || owner.xbuttons[secondaryIndex].Contains("LS ")) &&
                    !owner.toggleXbox.Contains("LS Up") &&
                    !owner.toggleXbox.Contains("LS Right") &&
                    !owner.toggleXbox.Contains("LS Down") &&
                    !owner.toggleXbox.Contains("LS Left"))
                {
                    owner.MoveCombinedXboxLeftStick(0, 0, false);
                    owner.MoveCombinedXboxLeftStick(1, 0, false);
                }

                if ((owner.xbuttons[primaryIndex].Contains("RS ") || owner.xbuttons[secondaryIndex].Contains("RS ")) &&
                    !owner.toggleXbox.Contains("RS Up") &&
                    !owner.toggleXbox.Contains("RS Right") &&
                    !owner.toggleXbox.Contains("RS Down") &&
                    !owner.toggleXbox.Contains("RS Left"))
                {
                    owner.MoveCombinedXboxRightStick(2, 0, false);
                    owner.MoveCombinedXboxRightStick(3, 0, false);
                }

                if ((owner.xbuttons[primaryIndex] == "RT or R2" || owner.xbuttons[secondaryIndex] == "RT or R2") && !owner.toggleXbox.Contains("RT or R2"))
                {
                    owner.SetCombinedXboxTrigger(1, 0);
                }

                if ((owner.xbuttons[primaryIndex] == "LT or L2" || owner.xbuttons[secondaryIndex] == "LT or L2") && !owner.toggleXbox.Contains("LT or L2"))
                {
                    owner.SetCombinedXboxTrigger(0, 0);
                }
            }
        }

        private sealed class SwitchOutput
        {
            private readonly Active owner;

            public SwitchOutput(Active owner)
            {
                this.owner = owner;
            }

            public Task Press(OutputTarget target, InputActionBehavior pressMode, CancellationToken cancellationToken = default, bool trackTargets = true)
            {
                if (pressMode == InputActionBehavior.Once)
                {
                    return PressOnceAsync(target, cancellationToken, trackTargets);
                }

                switch (target.Kind)
                {
                    case OutputTargetKind.Hotkey:
                        if (trackTargets)
                        {
                            AddTargets(target.PrimaryIndex, target.SecondaryIndex);
                        }
                        ApplyHotkeyTargets(target.PrimaryIndex, target.SecondaryIndex);
                        return Task.CompletedTask;

                    case OutputTargetKind.Click:
                        return PressClickAsync(target);

                    case OutputTargetKind.MouseMove:
                        if (pressMode == InputActionBehavior.Toggle)
                        {
                            if (trackTargets)
                            {
                                AddTargets(target.PrimaryIndex, target.SecondaryIndex);
                            }
                            ApplyHotkeyTargets(target.PrimaryIndex, target.SecondaryIndex);
                        }
                        else if (owner.UsesDiscreteMouseMoveTargets(target))
                        {
                            owner.LogMouseMoveDebug("SwitchOutput.Press.Discrete", owner.DescribeTarget(target));
                            owner.ApplyDiscreteSwitchMouseMoveTargets(target);
                        }
                        else
                        {
                            owner.ApplySwitchVariableTarget(
                                pressMode == InputActionBehavior.Variable ? target : owner.CreateMaxMouseMoveTarget(target),
                                VariableInputMode.Hover);
                        }

                        return Task.CompletedTask;

                    default:
                        return Task.CompletedTask;
                }
            }

            public Task Release(OutputTarget target, OutputReleaseMode releaseMode, bool trackTargets = true)
            {
                if (trackTargets)
                {
                    RemoveTargets(target.PrimaryIndex, target.SecondaryIndex);
                }

                switch (releaseMode)
                {
                    case OutputReleaseMode.Hotkey:
                    case OutputReleaseMode.MouseMove:
                        ReleaseHotkeyTargets(target.PrimaryIndex, target.SecondaryIndex);
                        break;

                    case OutputReleaseMode.All:
                        ReleaseHotkeyTargets(target.PrimaryIndex, target.SecondaryIndex);
                        ReleaseClickTargets(target.PrimaryIndex, target.SecondaryIndex);
                        break;

                    case OutputReleaseMode.Click:
                    default:
                        ReleaseClickTargets(target.PrimaryIndex, target.SecondaryIndex);
                        break;
                }

                return Task.CompletedTask;
            }

            private async Task PressOnceAsync(OutputTarget target, CancellationToken cancellationToken, bool trackTargets)
            {
                if (target.Kind == OutputTargetKind.MouseMove)
                {
                    if (owner.UsesDiscreteMouseMoveTargets(target))
                    {
                        owner.LogMouseMoveDebug("SwitchOutput.PressOnce.Discrete", owner.DescribeTarget(target));
                        owner.ApplyDiscreteSwitchMouseMoveTargets(target);
                    }
                    else
                    {
                        owner.ApplySwitchVariableTarget(target, VariableInputMode.Hover);
                    }

                    await owner.SetMouseMoveTextAsync(ClickThreadMode.Switch, target.PrimaryIndex, target.SecondaryIndex, false);
                    await Task.Delay(OncePressDurationMs, cancellationToken);
                    ReleaseHotkeyTargets(target.PrimaryIndex, target.SecondaryIndex);
                    return;
                }

                await PressClickAsync(target);
                await Task.Delay(OncePressDurationMs, cancellationToken);
                if (trackTargets)
                {
                    RemoveTargets(target.PrimaryIndex, target.SecondaryIndex);
                }
                ReleaseClickTargets(target.PrimaryIndex, target.SecondaryIndex);
            }

            private async Task PressClickAsync(OutputTarget target)
            {
                AddTargets(target.PrimaryIndex, target.SecondaryIndex);
                owner.ApplySwitchVariableTarget(OutputTarget.MouseMove(target.PrimaryIndex, target.PrimaryIndex, target.PrimaryIndex, target.Distance), VariableInputMode.Click);

                if (target.PrimaryIndex != target.SecondaryIndex)
                {
                    await Task.Delay(10, CancellationToken.None);
                    owner.ApplySwitchVariableTarget(OutputTarget.MouseMove(target.SecondaryIndex, target.SecondaryIndex, target.SecondaryIndex, target.Distance), VariableInputMode.Click);
                }
            }

            private void AddTargets(int primaryIndex, int secondaryIndex)
            {
                foreach (string target in EnumerateDistinctTargets(primaryIndex, secondaryIndex)
                    .Select(index => owner.GetPromptBinding(index).SwitchToken))
                {
                    owner.toggleSwitch.Add(target);
                }
            }

            private void RemoveTargets(int primaryIndex, int secondaryIndex)
            {
                HashSet<string> targets = new HashSet<string>(EnumerateDistinctTargets(primaryIndex, secondaryIndex)
                    .Select(index => owner.GetPromptBinding(index).SwitchToken));
                owner.toggleSwitch = new ConcurrentBag<string>(owner.toggleSwitch.Where(value => !targets.Contains(value)));
            }

            private void ApplyHotkeyTargets(int primaryIndex, int secondaryIndex)
            {
                owner.ApplySwitchCombinedTargets(primaryIndex, secondaryIndex);
            }

            private void ReleaseHotkeyTargets(int primaryIndex, int secondaryIndex)
            {
                if (owner.switchbuttons[primaryIndex].ToString() == "A Button" || owner.switchbuttons[secondaryIndex].ToString() == "A Button") { owner.switchGamepad.ButtonA_Up(); }
                if (owner.switchbuttons[primaryIndex].ToString() == "B Button" || owner.switchbuttons[secondaryIndex].ToString() == "B Button") { owner.switchGamepad.ButtonB_Up(); }
                if (owner.switchbuttons[primaryIndex].ToString() == "X Button" || owner.switchbuttons[secondaryIndex].ToString() == "X Button") { owner.switchGamepad.ButtonX_Up(); }
                if (owner.switchbuttons[primaryIndex].ToString() == "Y Button" || owner.switchbuttons[secondaryIndex].ToString() == "Y Button") { owner.switchGamepad.ButtonY_Up(); }
                if (owner.switchbuttons[primaryIndex].ToString() == "L Button" || owner.switchbuttons[secondaryIndex].ToString() == "L Button") { owner.switchGamepad.ButtonL_Up(); }
                if (owner.switchbuttons[primaryIndex].ToString() == "ZL Button" || owner.switchbuttons[secondaryIndex].ToString() == "ZL Button") { owner.switchGamepad.ButtonZL_Up(); }
                if (owner.switchbuttons[primaryIndex].ToString() == "L3 Button" || owner.switchbuttons[secondaryIndex].ToString() == "L3 Button") { owner.switchGamepad.ButtonL3_Up(); }
                if (owner.switchbuttons[primaryIndex].ToString() == "R Button" || owner.switchbuttons[secondaryIndex].ToString() == "R Button") {  owner.switchGamepad.ButtonR_Up(); }
                if (owner.switchbuttons[primaryIndex].ToString() == "ZR Button" || owner.switchbuttons[secondaryIndex].ToString() == "ZR Button") { owner.switchGamepad.ButtonZR_Up(); }
                if (owner.switchbuttons[primaryIndex].ToString() == "R3 Button" || owner.switchbuttons[secondaryIndex].ToString() == "R3 Button") { owner.switchGamepad.ButtonR3_Up(); }
                if (owner.switchbuttons[primaryIndex].ToString() == "DPad Up" || owner.switchbuttons[secondaryIndex].ToString() == "DPad Up") { owner.switchGamepad.ButtonDpadUp_Up(); }
                if (owner.switchbuttons[primaryIndex].ToString() == "DPad Right" || owner.switchbuttons[secondaryIndex].ToString() == "DPad Right") { owner.switchGamepad.ButtonDpadRight_Up(); }
                if (owner.switchbuttons[primaryIndex].ToString() == "DPad Down" || owner.switchbuttons[secondaryIndex].ToString() == "DPad Down") { owner.switchGamepad.ButtonDpadDown_Up(); }
                if (owner.switchbuttons[primaryIndex].ToString() == "DPad Left" || owner.switchbuttons[secondaryIndex].ToString() == "DPad Left") { owner.switchGamepad.ButtonDpadLeft_Up(); }
                if (owner.switchbuttons[primaryIndex].ToString() == "Home" || owner.switchbuttons[secondaryIndex].ToString() == "Home") { owner.switchGamepad.ButtonHome_Up(); }
                if (owner.switchbuttons[primaryIndex].ToString() == "Plus (Start)" || owner.switchbuttons[secondaryIndex].ToString() == "Plus (Start)") { owner.switchGamepad.ButtonPlus_Up(); }
                if (owner.switchbuttons[primaryIndex].ToString() == "Minus (Select)" || owner.switchbuttons[secondaryIndex].ToString() == "Minus (Select)") { owner.switchGamepad.ButtonMinus_Up(); }
                if (owner.switchbuttons[primaryIndex].ToString() == "Capture" || owner.switchbuttons[secondaryIndex].ToString() == "Capture") { owner.switchGamepad.ButtonCapture_Up(); }

                if ((owner.switchbuttons[primaryIndex].Contains("LS ") || owner.switchbuttons[secondaryIndex].Contains("LS ")) &&
                    !owner.toggleSwitch.Contains("LS Up") &&
                    !owner.toggleSwitch.Contains("LS Right") &&
                    !owner.toggleSwitch.Contains("LS Down") &&
                    !owner.toggleSwitch.Contains("LS Left"))
                {
                    owner.switchGamepad.leftStick_Stop();
                }

                if ((owner.switchbuttons[primaryIndex].Contains("RS ") || owner.switchbuttons[secondaryIndex].Contains("RS ")) &&
                    !owner.toggleSwitch.Contains("RS Up") &&
                    !owner.toggleSwitch.Contains("RS Right") &&
                    !owner.toggleSwitch.Contains("RS Down") &&
                    !owner.toggleSwitch.Contains("RS Left"))
                {
                    owner.switchGamepad.rightStick_Stop();
                }
            }

            private void ReleaseClickTargets(int primaryIndex, int secondaryIndex)
            {
                if ((owner.switchbuttons[primaryIndex].ToString() == "A Button" || owner.switchbuttons[secondaryIndex].ToString() == "A Button") && !owner.toggleSwitch.Contains("A Button")) { owner.switchGamepad.ButtonA_Up(); }
                if ((owner.switchbuttons[primaryIndex].ToString() == "B Button" || owner.switchbuttons[secondaryIndex].ToString() == "B Button") && !owner.toggleSwitch.Contains("B Button")) { owner.switchGamepad.ButtonB_Up(); }
                if ((owner.switchbuttons[primaryIndex].ToString() == "X Button" || owner.switchbuttons[secondaryIndex].ToString() == "X Button") && !owner.toggleSwitch.Contains("X Button")) { owner.switchGamepad.ButtonX_Up(); }
                if ((owner.switchbuttons[primaryIndex].ToString() == "Y Button" || owner.switchbuttons[secondaryIndex].ToString() == "Y Button") && !owner.toggleSwitch.Contains("Y Button")) { owner.switchGamepad.ButtonY_Up(); }
                if ((owner.switchbuttons[primaryIndex].ToString() == "L Button" || owner.switchbuttons[secondaryIndex].ToString() == "L Button") && !owner.toggleSwitch.Contains("L Button")) { owner.switchGamepad.ButtonL_Up(); }
                if ((owner.switchbuttons[primaryIndex].ToString() == "ZL Button" || owner.switchbuttons[secondaryIndex].ToString() == "ZL Button") && !owner.toggleSwitch.Contains("ZL Button")) { owner.SetCombinedSwitchTrigger(true, false); }
                if ((owner.switchbuttons[primaryIndex].ToString() == "L3 Button" || owner.switchbuttons[secondaryIndex].ToString() == "L3 Button") && !owner.toggleSwitch.Contains("L3 Button")) { owner.switchGamepad.ButtonL3_Up(); }
                if ((owner.switchbuttons[primaryIndex].ToString() == "R Button" || owner.switchbuttons[secondaryIndex].ToString() == "R Button") && !owner.toggleSwitch.Contains("R Button")) { Debug.WriteLine("R Button Released"); owner.switchGamepad.ButtonR_Up(); }
                if ((owner.switchbuttons[primaryIndex].ToString() == "ZR Button" || owner.switchbuttons[secondaryIndex].ToString() == "ZR Button") && !owner.toggleSwitch.Contains("ZR Button")) { owner.SetCombinedSwitchTrigger(false, false); }
                if ((owner.switchbuttons[primaryIndex].ToString() == "R3 Button" || owner.switchbuttons[secondaryIndex].ToString() == "R3 Button") && !owner.toggleSwitch.Contains("R3 Button")) { owner.switchGamepad.ButtonR3_Up(); }
                if ((owner.switchbuttons[primaryIndex].ToString() == "DPad Up" || owner.switchbuttons[secondaryIndex].ToString() == "DPad Up") && !owner.toggleSwitch.Contains("DPad Up")) { owner.switchGamepad.ButtonDpadUp_Up(); }
                if ((owner.switchbuttons[primaryIndex].ToString() == "DPad Right" || owner.switchbuttons[secondaryIndex].ToString() == "DPad Right") && !owner.toggleSwitch.Contains("DPad Right")) { owner.switchGamepad.ButtonDpadRight_Up(); }
                if ((owner.switchbuttons[primaryIndex].ToString() == "DPad Down" || owner.switchbuttons[secondaryIndex].ToString() == "DPad Down") && !owner.toggleSwitch.Contains("DPad Down")) { owner.switchGamepad.ButtonDpadDown_Up(); }
                if ((owner.switchbuttons[primaryIndex].ToString() == "DPad Left" || owner.switchbuttons[secondaryIndex].ToString() == "DPad Left") && !owner.toggleSwitch.Contains("DPad Left")) { owner.switchGamepad.ButtonDpadLeft_Up(); }
                if ((owner.switchbuttons[primaryIndex].ToString() == "Home" || owner.switchbuttons[secondaryIndex].ToString() == "Home") && !owner.toggleSwitch.Contains("Home")) { owner.switchGamepad.ButtonHome_Up(); }
                if ((owner.switchbuttons[primaryIndex].ToString() == "Plus (Start)" || owner.switchbuttons[secondaryIndex].ToString() == "Plus (Start)") && !owner.toggleSwitch.Contains("Plus (Start)")) { owner.switchGamepad.ButtonPlus_Up(); }
                if ((owner.switchbuttons[primaryIndex].ToString() == "Minus (Select)" || owner.switchbuttons[secondaryIndex].ToString() == "Minus (Select)") && !owner.toggleSwitch.Contains("Minus (Select)")) { owner.switchGamepad.ButtonMinus_Up(); }
                if ((owner.switchbuttons[primaryIndex].ToString() == "Capture" || owner.switchbuttons[secondaryIndex].ToString() == "Capture") && !owner.toggleSwitch.Contains("Capture")) { owner.switchGamepad.ButtonCapture_Up(); }

                if ((owner.switchbuttons[primaryIndex].Contains("LS ") || owner.switchbuttons[secondaryIndex].Contains("LS ")) && !owner.toggleSwitch.Contains("LS Left") && !owner.toggleSwitch.Contains("LS Right") && !owner.toggleSwitch.Contains("LS Up") && !owner.toggleSwitch.Contains("LS Down")) { owner.StopCombinedSwitchLeftStick(); }
                if ((owner.switchbuttons[primaryIndex].Contains("RS ") || owner.switchbuttons[secondaryIndex].Contains("RS ")) && !owner.toggleSwitch.Contains("RS Left") && !owner.toggleSwitch.Contains("RS Right") && !owner.toggleSwitch.Contains("RS Up") && !owner.toggleSwitch.Contains("RS Down")) { owner.StopCombinedSwitchRightStick(); }
            }
        }

        private OutputFacade output;

        private OutputFacade Output => output ??= new OutputFacade(this);

        private ClickThreadMode GetCurrentClickThreadMode()
        {
            if (keyboardMode)
            {
                return ClickThreadMode.Keyboard;
            }

            return xboxMode ? ClickThreadMode.Xbox : ClickThreadMode.Switch;
        }

        private IEnumerable<int> EnumerateActionIndices(int c1, int c2, int c3)
        {
            if (c1 == c2)
            {
                yield return c1;
                yield break;
            }

            yield return c2;
            if (c3 != c2)
            {
                yield return c3;
            }
        }

        private static IEnumerable<int> EnumerateMouseMoveActionIndices(int primaryIndex, int secondaryIndex)
        {
            yield return primaryIndex;

            if (secondaryIndex != primaryIndex)
            {
                yield return secondaryIndex;
            }
        }

        private static IEnumerable<int> EnumerateDistinctTargets(int primaryIndex, int secondaryIndex)
        {
            yield return primaryIndex;

            if (secondaryIndex != primaryIndex)
            {
                yield return secondaryIndex;
            }
        }

        private static OutputTarget CreateHotkeyTarget(int promptIndex, int primaryIndex, int secondaryIndex)
        {
            return OutputTarget.Hotkey(promptIndex, primaryIndex, secondaryIndex);
        }

        private static OutputTarget CreateClickTarget(int promptIndex, int primaryIndex, int secondaryIndex, double distance)
        {
            return OutputTarget.Click(promptIndex, primaryIndex, secondaryIndex, distance);
        }

        private static OutputTarget CreateMouseMoveTarget(int promptIndex, int primaryIndex, int secondaryIndex, double distance)
        {
            return OutputTarget.MouseMove(promptIndex, primaryIndex, secondaryIndex, distance);
        }

        private bool UsesDiscreteMouseMoveTargets(OutputTarget target)
        {
            // Use discrete mode for both diagonals (primary != secondary) and pure cardinals (primary == secondary)
            // This avoids ApplyXboxVariableTarget's toggleXbox guards which can intermittently block cardinal directions
            return target.Kind == OutputTargetKind.MouseMove
                && !GetPromptBinding(target.PrimaryIndex).IsVariable
                && !GetPromptBinding(target.SecondaryIndex).IsVariable;
        }

        private string DescribeTarget(OutputTarget target)
        {
            return $"prompt={target.PromptIndex} primary={target.PrimaryIndex} secondary={target.SecondaryIndex} distance={target.Distance:F2} discrete={UsesDiscreteMouseMoveTargets(target)}";
        }

        private void ApplyXboxCombinedTargets(int primaryIndex, int secondaryIndex)
        {
            if (!xboxMode)
            {
                return;
            }

            string primaryToken = xbuttons[primaryIndex].ToString();
            string secondaryToken = xbuttons[secondaryIndex].ToString();

            for (int i = 0; i < 26; i++)
            {
                if ((primaryToken == buttonOptions[i].ToString() && !IsBlockedXboxToken(primaryToken))
                    || (secondaryToken == buttonOptions[i].ToString() && !IsBlockedXboxToken(secondaryToken)))
                {
                    SimGamePad.Instance.SetControl(i, 1);
                }
            }

            if ((primaryToken == "LT or L2" && !IsBlockedXboxToken(primaryToken)) || (secondaryToken == "LT or L2" && !IsBlockedXboxToken(secondaryToken)))
            {
                SetCombinedXboxTrigger(0, 1);
            }

            if ((primaryToken == "RT or R2" && !IsBlockedXboxToken(primaryToken)) || (secondaryToken == "RT or R2" && !IsBlockedXboxToken(secondaryToken)))
            {
                SetCombinedXboxTrigger(1, 1);
            }

            if ((primaryToken == "LS Up" && !IsBlockedXboxToken(primaryToken)) || (secondaryToken == "LS Up" && !IsBlockedXboxToken(secondaryToken))) { MoveCombinedXboxLeftStick(1, 1, false); }
            if ((primaryToken == "LS Right" && !IsBlockedXboxToken(primaryToken)) || (secondaryToken == "LS Right" && !IsBlockedXboxToken(secondaryToken))) { MoveCombinedXboxLeftStick(0, 1, false); }
            if ((primaryToken == "LS Down" && !IsBlockedXboxToken(primaryToken)) || (secondaryToken == "LS Down" && !IsBlockedXboxToken(secondaryToken))) { MoveCombinedXboxLeftStick(1, 1, true); }
            if ((primaryToken == "LS Left" && !IsBlockedXboxToken(primaryToken)) || (secondaryToken == "LS Left" && !IsBlockedXboxToken(secondaryToken))) { MoveCombinedXboxLeftStick(0, 1, true); }
            if ((primaryToken == "RS Up" && !IsBlockedXboxToken(primaryToken)) || (secondaryToken == "RS Up" && !IsBlockedXboxToken(secondaryToken))) { MoveCombinedXboxRightStick(3, 1, false); }
            if ((primaryToken == "RS Right" && !IsBlockedXboxToken(primaryToken)) || (secondaryToken == "RS Right" && !IsBlockedXboxToken(secondaryToken))) { MoveCombinedXboxRightStick(2, 1, false); }
            if ((primaryToken == "RS Down" && !IsBlockedXboxToken(primaryToken)) || (secondaryToken == "RS Down" && !IsBlockedXboxToken(secondaryToken))) { MoveCombinedXboxRightStick(3, 1, true); }
            if ((primaryToken == "RS Left" && !IsBlockedXboxToken(primaryToken)) || (secondaryToken == "RS Left" && !IsBlockedXboxToken(secondaryToken))) { MoveCombinedXboxRightStick(2, 1, true); }

            SimGamePad.Instance.Update(1);
        }

        private void ApplySwitchCombinedTargets(int primaryIndex, int secondaryIndex)
        {
            bool hasLsUp = switchbuttons[primaryIndex].ToString() == "LS Up" || switchbuttons[secondaryIndex].ToString() == "LS Up";
            bool hasLsRight = switchbuttons[primaryIndex].ToString() == "LS Right" || switchbuttons[secondaryIndex].ToString() == "LS Right";
            bool hasLsDown = switchbuttons[primaryIndex].ToString() == "LS Down" || switchbuttons[secondaryIndex].ToString() == "LS Down";
            bool hasLsLeft = switchbuttons[primaryIndex].ToString() == "LS Left" || switchbuttons[secondaryIndex].ToString() == "LS Left";
            bool hasRsUp = switchbuttons[primaryIndex].ToString() == "RS Up" || switchbuttons[secondaryIndex].ToString() == "RS Up";
            bool hasRsRight = switchbuttons[primaryIndex].ToString() == "RS Right" || switchbuttons[secondaryIndex].ToString() == "RS Right";
            bool hasRsDown = switchbuttons[primaryIndex].ToString() == "RS Down" || switchbuttons[secondaryIndex].ToString() == "RS Down";
            bool hasRsLeft = switchbuttons[primaryIndex].ToString() == "RS Left" || switchbuttons[secondaryIndex].ToString() == "RS Left";

            if (switchbuttons[primaryIndex].ToString() == "A Button" || switchbuttons[secondaryIndex].ToString() == "A Button") { switchGamepad.ButtonA_Down(); }
            if (switchbuttons[primaryIndex].ToString() == "B Button" || switchbuttons[secondaryIndex].ToString() == "B Button") { switchGamepad.ButtonB_Down(); }
            if (switchbuttons[primaryIndex].ToString() == "X Button" || switchbuttons[secondaryIndex].ToString() == "X Button") { switchGamepad.ButtonX_Down(); }
            if (switchbuttons[primaryIndex].ToString() == "Y Button" || switchbuttons[secondaryIndex].ToString() == "Y Button") { switchGamepad.ButtonY_Down(); }
            if (switchbuttons[primaryIndex].ToString() == "L Button" || switchbuttons[secondaryIndex].ToString() == "L Button") { switchGamepad.ButtonL_Down(); }
            if ((switchbuttons[primaryIndex].ToString() == "ZL Button" || switchbuttons[secondaryIndex].ToString() == "ZL Button") && !IsBlockedSwitchToken("ZL Button")) { switchGamepad.ButtonZL_Down(); }
            if (switchbuttons[primaryIndex].ToString() == "L3 Button" || switchbuttons[secondaryIndex].ToString() == "L3 Button") { switchGamepad.ButtonL3_Down(); }
            if (switchbuttons[primaryIndex].ToString() == "R Button" || switchbuttons[secondaryIndex].ToString() == "R Button") { switchGamepad.ButtonR_Down(); }
            if ((switchbuttons[primaryIndex].ToString() == "ZR Button" || switchbuttons[secondaryIndex].ToString() == "ZR Button") && !IsBlockedSwitchToken("ZR Button")) { switchGamepad.ButtonZR_Down(); }
            if (switchbuttons[primaryIndex].ToString() == "R3 Button" || switchbuttons[secondaryIndex].ToString() == "R3 Button") { switchGamepad.ButtonR3_Down(); }
            if (switchbuttons[primaryIndex].ToString() == "DPad Up" || switchbuttons[secondaryIndex].ToString() == "DPad Up") { switchGamepad.ButtonDpadUp_Down(); }
            if (switchbuttons[primaryIndex].ToString() == "DPad Right" || switchbuttons[secondaryIndex].ToString() == "DPad Right") { switchGamepad.ButtonDpadRight_Down(); }
            if (switchbuttons[primaryIndex].ToString() == "DPad Down" || switchbuttons[secondaryIndex].ToString() == "DPad Down") { switchGamepad.ButtonDpadDown_Down(); }
            if (switchbuttons[primaryIndex].ToString() == "DPad Left" || switchbuttons[secondaryIndex].ToString() == "DPad Left") { switchGamepad.ButtonDpadLeft_Down(); }
            if (switchbuttons[primaryIndex].ToString() == "Home" || switchbuttons[secondaryIndex].ToString() == "Home") { switchGamepad.ButtonHome_Down(); }
            if (switchbuttons[primaryIndex].ToString() == "Plus (Start)" || switchbuttons[secondaryIndex].ToString() == "Plus (Start)") { switchGamepad.ButtonPlus_Down(); }
            if (switchbuttons[primaryIndex].ToString() == "Minus (Select)" || switchbuttons[secondaryIndex].ToString() == "Minus (Select)") { switchGamepad.ButtonMinus_Down(); }
            if (switchbuttons[primaryIndex].ToString() == "Capture" || switchbuttons[secondaryIndex].ToString() == "Capture") { switchGamepad.ButtonCapture_Down(); }

            if (hasLsUp && hasLsRight) { switchGamepad.MoveLeftStickDiagonal(100, 315); }
            else if (hasLsRight && hasLsDown) { switchGamepad.MoveLeftStickDiagonal(100, 45); }
            else if (hasLsDown && hasLsLeft) { switchGamepad.MoveLeftStickDiagonal(100, 135); }
            else if (hasLsLeft && hasLsUp) { switchGamepad.MoveLeftStickDiagonal(100, 225); }
            else
            {
                if (hasLsUp) { switchGamepad.leftStick_MoveUp(100); }
                if (hasLsRight) { switchGamepad.leftStick_MoveRight(100); }
                if (hasLsDown) { switchGamepad.leftStick_MoveDown(100); }
                if (hasLsLeft) { switchGamepad.leftStick_MoveLeft(100); }
            }

            if (hasRsUp && hasRsRight) { switchGamepad.MoveRightStickDiagonal(100, 315); }
            else if (hasRsRight && hasRsDown) { switchGamepad.MoveRightStickDiagonal(100, 45); }
            else if (hasRsDown && hasRsLeft) { switchGamepad.MoveRightStickDiagonal(100, 135); }
            else if (hasRsLeft && hasRsUp) { switchGamepad.MoveRightStickDiagonal(100, 225); }
            else
            {
                if (hasRsUp) { switchGamepad.rightStick_MoveUp(100); }
                if (hasRsRight) { switchGamepad.rightStick_MoveRight(100); }
                if (hasRsDown) { switchGamepad.rightStick_MoveDown(100); }
                if (hasRsLeft) { switchGamepad.rightStick_MoveLeft(100); }
            }
        }

        private void ApplyDiscreteXboxMouseMoveTargets(OutputTarget target, bool onceMode)
        {
            LogMouseMoveDebug("ApplyDiscreteXboxMouseMoveTargets", $"onceMode={onceMode} {DescribeTarget(target)}");
            ApplyXboxCombinedTargets(target.PrimaryIndex, target.SecondaryIndex);
        }

        private void ApplyDiscreteSwitchMouseMoveTargets(OutputTarget target)
        {
            LogMouseMoveDebug("ApplyDiscreteSwitchMouseMoveTargets", DescribeTarget(target));
            ApplySwitchCombinedTargets(target.PrimaryIndex, target.SecondaryIndex);
        }

        private void LogMouseMoveDebug(string stage, string message)
        {
            string line = $"[{DateTime.Now:HH:mm:ss.fff}] [MouseMoveDebug] {stage} {message}";
            //Debug.WriteLine(line);

            try
            {
                Directory.CreateDirectory(configPath);

                lock (mouseMoveDebugSync)
                {
                    File.AppendAllText(Path.Combine(configPath, "MouseMoveDebug.log"), line + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MouseMoveDebug] Failed to write log: {ex.Message}");
            }
        }

        private OutputTarget CreateMaxMouseMoveTarget(OutputTarget target)
        {
            return CreateMouseMoveTarget(target.PromptIndex, target.PrimaryIndex, target.SecondaryIndex, 190);
        }

        private VariableInputState CreateVariableInputState(int promptIndex, double distance, bool forceUpdate)
        {
            double unitMagnitude = (distance - 60) / 130;
            bool promptChanged = promptIndex != oldQuadrant;

            if (unitMagnitude >= 1)
            {
                unitMagnitude = 1;
            }
            else if (unitMagnitude <= 0.2)
            {
                unitMagnitude = promptChanged && distance > deadZone ? 0.2 : 0;
            }

            bool pointerMoved = Math.Abs(mousePosX - GamepadMouseX2) > 2 || Math.Abs(mousePosY - GamepadMouseY2) > 2;
            bool shouldProcess = pointerMoved || promptChanged || forceUpdate;
            return new VariableInputState(unitMagnitude, shouldProcess, IsAdjacentHoverQuadrant(promptIndex));
        }

        private bool IsAdjacentHoverQuadrant(int promptIndex)
        {
            if (promptIndex == 1)
            {
                return diagUpRightHover;
            }

            if (promptIndex == 3)
            {
                return diagDownRightHover;
            }

            if (promptIndex == 5)
            {
                return diagDownLeftHover;
            }

            if (promptIndex == 7)
            {
                return diagUpLeftHover;
            }

            return false;
        }

        private void PressHotkeyTargets(ClickThreadMode mode, int c1, int c2, int c3)
        {
            OutputTarget target = CreateHotkeyTarget(c1, c2, c3);

            switch (mode)
            {
                case ClickThreadMode.Keyboard:
                    Output.Keyboard.Press(target, InputActionBehavior.HoldUntilRelease).GetAwaiter().GetResult();
                    break;

                case ClickThreadMode.Xbox:
                    Output.Xbox.Press(target, InputActionBehavior.HoldUntilRelease).GetAwaiter().GetResult();
                    break;

                default:
                    Output.Switch.Press(target, InputActionBehavior.HoldUntilRelease).GetAwaiter().GetResult();
                    break;
            }
        }

        private void ReleaseHotkeyTargets(ClickThreadMode mode, int c1, int c2, int c3)
        {
            OutputTarget target = CreateHotkeyTarget(c1, c2, c3);

            switch (mode)
            {
                case ClickThreadMode.Keyboard:
                    Output.Keyboard.Release(target, OutputReleaseMode.Hotkey).GetAwaiter().GetResult();
                    break;

                case ClickThreadMode.Xbox:
                    Output.Xbox.Release(target, OutputReleaseMode.Hotkey).GetAwaiter().GetResult();
                    break;

                default:
                    Output.Switch.Release(target, OutputReleaseMode.Hotkey).GetAwaiter().GetResult();
                    break;
            }
        }

        private void ApplyToggleTargetState(ClickThreadMode mode, int c1, int c2, int c3, bool enabled)
        {
            if (enabled)
            {
                PressHotkeyTargets(mode, c1, c2, c3);
                SetClickButtonsText(mode, c1, c2, c3, true);
                activateToggle[c1] = true;
                buttonPressed[c1] = true;
                return;
            }

            ReleaseHotkeyTargets(mode, c1, c2, c3);
            activateToggle[c1] = true;
            buttonPressed[c1] = false;
            SetClickButtonsIdleText();
        }
    }
}