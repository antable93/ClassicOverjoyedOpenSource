using System.Collections.Generic;
using GregsStack.InputSimulatorStandard.Native;

namespace OverjoyedReleaseLocal
{
    class DictionaryEnums
    {
        //Enums
        public enum directions
        {
            upHover,
            uprightHover,
            rightHover,
            downrightHover,
            downHover,
            downleftHover,
            leftHover,
            upleftHover,
            upLclick,
            uprightLclick,
            rightLclick,
            downrightLclick,
            downLclick,
            downleftLclick,
            leftLclick,
            upleftLclick,
            upRclick,
            uprightRclick,
            rightRclick,
            downrightRclick,
            downRclick,
            downleftRclick,
            leftRclick,
            upleftRclick,
            deadzoneUpperHover,
            deadzoneUpperLclick,
            deadzoneUpperRclick,
            deadzoneMiddleHover,
            deadzoneMiddleLclick,
            deadzoneMiddleRclick,
            deadzoneLowerHover,
            deadzoneLowerLclick,
            deadzoneLowerRclick
        }


        public static Dictionary<string, VirtualKeyCode> strToKey = new Dictionary<string, VirtualKeyCode>()
        {
            //Special Keys
            { "up", VirtualKeyCode.UP },
            { "down", VirtualKeyCode.DOWN},
            { "left", VirtualKeyCode.LEFT},
            { "right", VirtualKeyCode.RIGHT},
            { "space", VirtualKeyCode.SPACE},
            { "lshift", VirtualKeyCode.LSHIFT},
            { "rshift", VirtualKeyCode.RSHIFT},
            { "lctrl", VirtualKeyCode.LCONTROL},
            { "rctrl", VirtualKeyCode.RCONTROL},
            { "tab", VirtualKeyCode.TAB},
            { "esc", VirtualKeyCode.ESCAPE},
            { "capslk", VirtualKeyCode.CAPITAL},
            {"lalt", VirtualKeyCode.LMENU },
            {"ralt", VirtualKeyCode.RMENU },
            {"win", VirtualKeyCode.LWIN },
            {"pause", VirtualKeyCode.PAUSE },
            {"printscreen", VirtualKeyCode.SNAPSHOT },
            {"scrolllock", VirtualKeyCode.SCROLL },
            {"add", VirtualKeyCode.ADD },
            {"subtract", VirtualKeyCode.SUBTRACT },
            {"multiply", VirtualKeyCode.MULTIPLY },
            {"divide", VirtualKeyCode.DIVIDE },

            //punctuation
            {"semicolon", VirtualKeyCode.OEM_1 },
            {"backslash", VirtualKeyCode.OEM_5 },
            {"slash", VirtualKeyCode.OEM_2 },
            {"backtick", VirtualKeyCode.OEM_3 },
            {"lbracket", VirtualKeyCode.OEM_4 },
            {"rbracket", VirtualKeyCode.OEM_6 },
            {"singlequote", VirtualKeyCode.OEM_7 },
            {"enter", VirtualKeyCode.RETURN },
            {"backspace", VirtualKeyCode.BACK },
            {"insert", VirtualKeyCode.INSERT },
            {"delete", VirtualKeyCode.DELETE },
            {"home", VirtualKeyCode.HOME },
            {"end", VirtualKeyCode.END },
            {"pageup", VirtualKeyCode.PRIOR },
            {"pagedown", VirtualKeyCode.NEXT },
            {"numlock", VirtualKeyCode.NUMLOCK },
            
            //Symbols
            { "comma", VirtualKeyCode.OEM_COMMA},
            { "period", VirtualKeyCode.OEM_PERIOD},
            { "plus", VirtualKeyCode.OEM_PLUS},
            { "minus", VirtualKeyCode.OEM_MINUS},

            //Letters
            { "q", VirtualKeyCode.VK_Q},
            { "w", VirtualKeyCode.VK_W},
            { "e", VirtualKeyCode.VK_E},
            { "r", VirtualKeyCode.VK_R},
            { "t", VirtualKeyCode.VK_T},
            { "y", VirtualKeyCode.VK_Y},
            { "u", VirtualKeyCode.VK_U},
            { "i", VirtualKeyCode.VK_I},
            { "o", VirtualKeyCode.VK_O},
            { "p", VirtualKeyCode.VK_P},
            { "a", VirtualKeyCode.VK_A},
            { "s", VirtualKeyCode.VK_S},
            { "d", VirtualKeyCode.VK_D},
            { "f", VirtualKeyCode.VK_F},
            { "g", VirtualKeyCode.VK_G},
            { "h", VirtualKeyCode.VK_H},
            { "j", VirtualKeyCode.VK_J},
            { "k", VirtualKeyCode.VK_K},
            { "l", VirtualKeyCode.VK_L},
            { "z", VirtualKeyCode.VK_Z},
            { "x", VirtualKeyCode.VK_X},
            { "c", VirtualKeyCode.VK_C},
            { "v", VirtualKeyCode.VK_V},
            { "b", VirtualKeyCode.VK_B},
            { "n", VirtualKeyCode.VK_N},
            { "m", VirtualKeyCode.VK_M},

            //Function Keys
            { "f1", VirtualKeyCode.F1},
            { "f2", VirtualKeyCode.F2},
            { "f3", VirtualKeyCode.F3},
            { "f4", VirtualKeyCode.F4},
            { "f5", VirtualKeyCode.F5},
            { "f6", VirtualKeyCode.F6},
            { "f7", VirtualKeyCode.F7},
            { "f8", VirtualKeyCode.F8},
            { "f9", VirtualKeyCode.F9},
            { "f10", VirtualKeyCode.F10},
            { "f11", VirtualKeyCode.F11},
            { "f12", VirtualKeyCode.F12},

            //Alpha Numbers
            { "1", VirtualKeyCode.VK_1},
            { "2", VirtualKeyCode.VK_2},
            { "3", VirtualKeyCode.VK_3},
            { "4", VirtualKeyCode.VK_4},
            { "5", VirtualKeyCode.VK_5},
            { "6", VirtualKeyCode.VK_6},
            { "7", VirtualKeyCode.VK_7},
            { "8", VirtualKeyCode.VK_8},
            { "9", VirtualKeyCode.VK_9},
            { "0", VirtualKeyCode.VK_0},

            //Numpad Numbers
            //{ "numpad1", VirtualKeyCode.NUMPAD1},
            //{ "numpad2", VirtualKeyCode.NUMPAD2},
            //{ "numpad3", VirtualKeyCode.NUMPAD3},
            //{ "numpad4", VirtualKeyCode.NUMPAD4},
            //{ "numpad5", VirtualKeyCode.NUMPAD5},
            //{ "numpad6", VirtualKeyCode.NUMPAD6},
            //{ "numpad7", VirtualKeyCode.NUMPAD7},
            //{ "numpad8", VirtualKeyCode.NUMPAD8},
            //{ "numpad9", VirtualKeyCode.NUMPAD9},
            //{ "numpad0", VirtualKeyCode.NUMPAD0}

        };
    }
}