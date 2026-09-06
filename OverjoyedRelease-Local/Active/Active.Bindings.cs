using GregsStack.InputSimulatorStandard.Native;
using System;
using System.Collections.Generic;
using Wisej.Web;

namespace OverjoyedReleaseLocal
{
    public partial class Active : Page
    {
        private sealed class PromptBinding
        {
            public PromptBinding(int index, IReadOnlyList<string> configValues, IReadOnlyList<string> keyboardValues, VirtualKeyCode keyCode)
            {
                Index = index;
                KeyboardToken = GetValue(configValues, 0);
                KeyCode = keyCode;
                ToggleMode = GetValue(configValues, 1);
                DisabledFlag = GetValue(configValues, 2);
                XboxToken = GetValue(keyboardValues, 3);
                ReturnToCenterFlag = GetValue(configValues, 4);
                VariableFlag = GetValue(configValues, 5);
                SwitchToken = GetValue(keyboardValues, 6);
            }

            public int Index { get; }

            public string KeyboardToken { get; }

            public VirtualKeyCode KeyCode { get; }

            public string ToggleMode { get; }

            public string DisabledFlag { get; }

            public string XboxToken { get; }

            public string ReturnToCenterFlag { get; }

            public string VariableFlag { get; }

            public string SwitchToken { get; }

            public bool IsToggle => string.Equals(ToggleMode, "true", StringComparison.OrdinalIgnoreCase);

            public bool IsOnce => string.Equals(ToggleMode, "once", StringComparison.OrdinalIgnoreCase);

            public bool IsDisabled => string.Equals(DisabledFlag, "true", StringComparison.OrdinalIgnoreCase);

            public bool ReturnToCenter => string.Equals(ReturnToCenterFlag, "true", StringComparison.OrdinalIgnoreCase);

            public bool IsVariable => string.Equals(VariableFlag, "True", StringComparison.OrdinalIgnoreCase);

            private static string GetValue(IReadOnlyList<string> values, int index)
            {
                return index < values.Count ? values[index] : string.Empty;
            }
        }

        private PromptBinding GetPromptBinding(int promptIndex)
        {
            IReadOnlyList<string> configValues =
                promptIndex >= 0 && promptIndex < buttonConfig.Count
                    ? buttonConfig[promptIndex]
                    : Array.Empty<string>();

            IReadOnlyList<string> keyboardValues =
                promptIndex >= 0 && promptIndex < kbButtons.Count
                    ? kbButtons[promptIndex]
                    : Array.Empty<string>();

            VirtualKeyCode keyCode =
                promptIndex >= 0 && promptIndex < keyCodes.Count
                    ? keyCodes[promptIndex]
                    : default;

            return new PromptBinding(promptIndex, configValues, keyboardValues, keyCode);
        }

        private InputActionBehavior GetConfiguredInputBehavior(int promptIndex, bool forMouseMove = false)
        {
            PromptBinding binding = GetPromptBinding(promptIndex);

            if (binding.IsToggle)
            {
                return InputActionBehavior.Toggle;
            }

            if (binding.IsOnce)
            {
                return InputActionBehavior.Once;
            }

            if (!forMouseMove)
            {
                return InputActionBehavior.HoldUntilRelease;
            }

            if (GetCurrentClickThreadMode() != ClickThreadMode.Keyboard && binding.IsVariable)
            {
                return InputActionBehavior.Variable;
            }

            return InputActionBehavior.HoldUntilRelease;
        }
    }
}