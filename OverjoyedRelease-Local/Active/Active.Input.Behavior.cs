using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Wisej.Web;

namespace OverjoyedReleaseLocal
{
    public partial class Active : Page
    {
        private static bool IsLeftClickPrompt(int promptIndex)
        {
            return promptIndex < 16 || promptIndex == 25 || promptIndex == 28 || promptIndex == 31;
        }

        private async Task RunHoldUntilReleaseBehaviorAsync(
            Func<Task> pressAsync,
            Func<Task>? activeTextAsync = null,
            Func<Task>? waitForReleaseAsync = null,
            Func<Task>? releaseAsync = null,
            Func<Task>? idleTextAsync = null)
        {
            await pressAsync();

            if (activeTextAsync != null)
            {
                await activeTextAsync();
            }

            if (waitForReleaseAsync == null || releaseAsync == null)
            {
                return;
            }

            await waitForReleaseAsync();
            await releaseAsync();

            if (idleTextAsync != null)
            {
                await idleTextAsync();
            }
        }

        private async Task RunToggleBehaviorAsync(
            int promptIndex,
            Func<bool> toggleOn,
            Func<bool> toggleOff,
            Func<Task> enableAsync,
            Func<Task> disableAsync,
            Func<Task>? activeTextAsync = null,
            Func<Task>? idleTextAsync = null)
        {
            if (toggleOn())
            {
                await enableAsync();
                activateToggle[promptIndex] = true;
                buttonPressed[promptIndex] = true;

                if (activeTextAsync != null)
                {
                    await activeTextAsync();
                }

                return;
            }

            if (toggleOff())
            {
                await disableAsync();
                activateToggle[promptIndex] = true;
                buttonPressed[promptIndex] = false;

                if (idleTextAsync != null)
                {
                    await idleTextAsync();
                }
            }
        }

        private async Task RunOnceBehaviorAsync(int promptIndex, Func<Task> executeAsync)
        {
            if (oncePressed[promptIndex] != 0)
            {
                return;
            }

            oncePressed[promptIndex] = 1;
            await executeAsync();
        }

        private async Task RunVariableBehaviorAsync(Func<Task> applyAsync, Func<Task>? activeTextAsync = null)
        {
            await applyAsync();

            if (activeTextAsync != null)
            {
                await activeTextAsync();
            }
        }

        private async Task ExecuteInputBehaviorAsync(
            InputActionBehavior behavior,
            int promptIndex,
            Func<Task> normalAsync,
            Func<Task> onceAsync,
            Func<Task>? activeTextAsync = null,
            Func<Task>? waitForReleaseAsync = null,
            Func<Task>? releaseAsync = null,
            Func<Task>? idleTextAsync = null,
            Func<bool>? toggleOn = null,
            Func<bool>? toggleOff = null,
            Func<Task>? toggleEnableAsync = null,
            Func<Task>? toggleDisableAsync = null,
            Func<Task>? variableAsync = null)
        {
            switch (behavior)
            {
                case InputActionBehavior.Toggle:
                    if (toggleOn != null && toggleOff != null && toggleEnableAsync != null && toggleDisableAsync != null)
                    {
                        await RunToggleBehaviorAsync(
                            promptIndex,
                            toggleOn,
                            toggleOff,
                            toggleEnableAsync,
                            toggleDisableAsync,
                            activeTextAsync,
                            idleTextAsync);
                        break;
                    }

                    goto default;

                case InputActionBehavior.Once:
                    await RunOnceBehaviorAsync(promptIndex, onceAsync);
                    break;

                case InputActionBehavior.Variable:
                    if (variableAsync != null)
                    {
                        await RunVariableBehaviorAsync(variableAsync, activeTextAsync);
                        break;
                    }

                    goto default;

                default:
                    await RunHoldUntilReleaseBehaviorAsync(
                        normalAsync,
                        activeTextAsync,
                        waitForReleaseAsync,
                        releaseAsync,
                        idleTextAsync);
                    break;
            }
        }

        private async Task WaitForClickThreadRelease(bool isLeftButton, CancellationToken cancellationToken)
        {
            bool mouseUpOccurred = false;

            [DllImport("user32.dll")]
            static extern short GetAsyncKeyState(int vKey);

            const int VK_LBUTTON = 0x01;
            const int VK_RBUTTON = 0x02;

            while (!mouseUpOccurred && !cancellationToken.IsCancellationRequested)
            {
                if (isLeftButton)
                {
                    bool leftButtonStillHeld = (GetAsyncKeyState(VK_LBUTTON) & 0x8000) != 0 || syntheticLeftMouseButtonHeld;
                    if (!leftButtonStillHeld)
                    {
                        mouseUpOccurred = true;
                    }
                }
                else
                {
                    bool rightButtonStillHeld = (GetAsyncKeyState(VK_RBUTTON) & 0x8000) != 0 || syntheticRightMouseButtonHeld;
                    if (!rightButtonStillHeld)
                    {
                        mouseUpOccurred = true;
                    }
                }

                await Task.Delay(10, cancellationToken);
            }
        }

        private async Task WaitForClickBehaviorCompletion(InputActionBehavior behavior, bool isLeftButton, CancellationToken cancellationToken)
        {
            if (behavior == InputActionBehavior.Once)
            {
                await Task.Delay(OncePressDurationMs, cancellationToken);
                return;
            }

            await WaitForClickThreadRelease(isLeftButton, cancellationToken);
        }

        private async Task HandleSingleMouseMoveBehavior(int promptIndex, int toggleIndex, int b1, int b2, double distance)
        {
            ClickThreadMode mode = GetCurrentClickThreadMode();
            InputActionBehavior behavior = GetConfiguredInputBehavior(promptIndex, true);

            OutputTarget mouseMoveTarget = CreateMouseMoveTarget(promptIndex, b1, b2, distance);

            LogMouseMoveDebug(
                "HandleSingleMouseMoveBehavior",
                $"prompt={promptIndex} toggle={toggleIndex} primary={b1} secondary={b2} behavior={behavior} mode={mode} distance={distance:F2} target={DescribeTarget(mouseMoveTarget)}");

            await ExecuteInputBehaviorAsync(
                behavior,
                promptIndex,
                normalAsync: () => mode == ClickThreadMode.Keyboard
                    ? Output.Keyboard.Press(mouseMoveTarget, InputActionBehavior.HoldUntilRelease)
                    : mode == ClickThreadMode.Xbox
                        ? Output.Xbox.Press(mouseMoveTarget, InputActionBehavior.HoldUntilRelease)
                        : Output.Switch.Press(mouseMoveTarget, InputActionBehavior.HoldUntilRelease),
                onceAsync: () => mode == ClickThreadMode.Keyboard
                    ? Output.Keyboard.Press(mouseMoveTarget, InputActionBehavior.Once)
                    : mode == ClickThreadMode.Xbox
                        ? Output.Xbox.Press(mouseMoveTarget, InputActionBehavior.Once)
                        : Output.Switch.Press(mouseMoveTarget, InputActionBehavior.Once),
                activeTextAsync: () => SetMouseMoveTextAsync(mode, b1, b2, true),
                idleTextAsync: SetHoverButtonsIdleTextAsync,
                toggleOn: () => ToggleOn(promptIndex),
                toggleOff: () => ToggleOff(toggleIndex),
                toggleEnableAsync: () => mode == ClickThreadMode.Keyboard
                    ? Output.Keyboard.Press(mouseMoveTarget, InputActionBehavior.Toggle)
                    : mode == ClickThreadMode.Xbox
                        ? Output.Xbox.Press(mouseMoveTarget, InputActionBehavior.Toggle)
                        : Output.Switch.Press(mouseMoveTarget, InputActionBehavior.Toggle),
                toggleDisableAsync: () => mode == ClickThreadMode.Keyboard
                    ? Output.Keyboard.Release(mouseMoveTarget, OutputReleaseMode.MouseMove)
                    : mode == ClickThreadMode.Xbox
                        ? Output.Xbox.Release(mouseMoveTarget, OutputReleaseMode.MouseMove)
                        : Output.Switch.Release(mouseMoveTarget, OutputReleaseMode.MouseMove),
                variableAsync: () => mode == ClickThreadMode.Keyboard
                    ? Output.Keyboard.Press(mouseMoveTarget, InputActionBehavior.Variable)
                    : mode == ClickThreadMode.Xbox
                        ? Output.Xbox.Press(mouseMoveTarget, InputActionBehavior.Variable)
                        : Output.Switch.Press(mouseMoveTarget, InputActionBehavior.Variable));
        }

        async Task ClickThread(int b0, int b1, int b2, double distance, CancellationToken cancellationToken, bool skipToggle, ClickThreadMode mode)
        {
            int originalB0 = b0;
            InputActionBehavior behavior = GetConfiguredInputBehavior(originalB0);
            NormalizeClickThreadTargets(mode, originalB0, ref b0, ref b1, ref b2);
            OutputTarget clickTarget = CreateClickTarget(b0, b1, b2, distance);
            OutputTarget releaseTarget = CreateClickTarget(originalB0, b1, b2, distance);

            try
            {
                Func<Task> pressClickTargetsAsync = () => mode == ClickThreadMode.Keyboard
                    ? Output.Keyboard.Press(clickTarget, InputActionBehavior.HoldUntilRelease)
                    : mode == ClickThreadMode.Xbox
                        ? Output.Xbox.Press(clickTarget, InputActionBehavior.HoldUntilRelease)
                        : Output.Switch.Press(clickTarget, InputActionBehavior.HoldUntilRelease);
                Func<Task> setClickTextAsync = () =>
                {
                    SetClickThreadText(mode, b1, b2);
                    return Task.CompletedTask;
                };
                Func<Task> waitForReleaseAsync = () => WaitForClickThreadRelease(IsLeftClickPrompt(originalB0), cancellationToken);
                Func<Task> releaseClickTargetsAsync = () =>
                {
                    if (mode == ClickThreadMode.Keyboard)
                    {
                        Output.Keyboard.Release(releaseTarget, OutputReleaseMode.Click).GetAwaiter().GetResult();
                    }
                    else if (mode == ClickThreadMode.Xbox)
                    {
                        Output.Xbox.Release(releaseTarget, OutputReleaseMode.Click).GetAwaiter().GetResult();
                    }
                    else
                    {
                        Output.Switch.Release(releaseTarget, OutputReleaseMode.Click).GetAwaiter().GetResult();
                    }

                    SetClickButtonsIdleText();
                    return Task.CompletedTask;
                };
                Func<Task> setClickIdleTextAsync = () =>
                {
                    SetClickButtonsIdleText();
                    return Task.CompletedTask;
                };

                Func<Task> onceClickTargetsAsync = async () =>
                {
                    await setClickTextAsync();

                    if (mode == ClickThreadMode.Keyboard)
                    {
                        await Output.Keyboard.Press(clickTarget, InputActionBehavior.Once, cancellationToken);
                    }
                    else if (mode == ClickThreadMode.Xbox)
                    {
                        await Output.Xbox.Press(clickTarget, InputActionBehavior.Once, cancellationToken);
                    }
                    else
                    {
                        await Output.Switch.Press(clickTarget, InputActionBehavior.Once, cancellationToken);
                    }

                    await setClickIdleTextAsync();
                };

                await ExecuteInputBehaviorAsync(
                    behavior,
                    originalB0,
                    normalAsync: pressClickTargetsAsync,
                    onceAsync: onceClickTargetsAsync,
                    activeTextAsync: setClickTextAsync,
                    waitForReleaseAsync: waitForReleaseAsync,
                    releaseAsync: releaseClickTargetsAsync,
                    idleTextAsync: setClickIdleTextAsync);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                RestoreNormalizedClickThreadTarget(mode);
            }
        }
    }
}