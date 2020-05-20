using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Keyboard
{
    using Keys = System.Windows.Forms.Keys;

    public class KeyboardOutput : HomeSimCockpitSDK.IModule, HomeSimCockpitSDK.IOutput, HomeSimCockpitSDK.IModuleFunctions//, HomeSimCockpitSDK.IModuleHelp
    {

        #region Inner classes

        private class SKEvent : IEquatable<SKEvent>
        {
            internal IntPtr hwnd;
            internal int paramH;
            internal int paramL;
            internal int wm;
            internal int scan;

            public SKEvent(int a, int b, bool c, IntPtr hwnd, int scan)
            {
                this.wm = a;
                this.paramL = b;
                this.paramH = c ? 1 : 0;
                this.hwnd = hwnd;
                this.scan = scan;
            }

            public SKEvent(int a, int b, int c, IntPtr hwnd, int scan)
            {
                this.wm = a;
                this.paramL = b;
                this.paramH = c;
                this.hwnd = hwnd;
                this.scan = scan;
            }

            public bool IsDown
            {
                get
                {
                    return wm == WM_KEYDOWN || wm == WM_CHAR || wm == WM_SYSKEYDOWN;
                }
            }

            #region IEquatable<SKEvent> Members

            public bool Equals(SKEvent other)
            {
                return wm == other.wm && paramL == other.paramL && paramH == other.paramH;
            }

            #endregion

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override string ToString()
            {
                return string.Format("IsDown = {3}, WM = {0}, L = {1}, H = {2}", wm, paramL, paramH, IsDown);
            }
        }

        private class KeywordVk
        {
            internal string keyword;
            internal int vk;

            public KeywordVk(string key, int v)
            {
                this.keyword = key;
                this.vk = v;
            }
        }

        #endregion

        #region WinAPI functions

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        // For Windows Mobile, replace user32.dll with coredll.dll
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool BlockInput([In, MarshalAs(UnmanagedType.Bool)] bool fBlockIt);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int SetKeyboardState(byte[] keystate);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int GetKeyboardState(byte[] keystate);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern short VkKeyScan(char key);

        [DllImport("user32.dll")]
        public static extern int OemKeyScan(short wAsciiVal);        
        
        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern int MapVirtualKey(int uCode, int nMapType);

        #endregion

        #region WinAPI structures

        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public short wVk;
            public short wScan;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct INPUTUNION
        {
            [FieldOffset(0)]
            public HARDWAREINPUT hi;
            [FieldOffset(0)]
            public KEYBDINPUT ki;
            [FieldOffset(0)]
            public MOUSEINPUT mi;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            public int type;
            public INPUTUNION inputUnion;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        #endregion

        #region WinAPI defines

        public static readonly int KEYEVENTF_EXTENDEDKEY = 0x0001;
        public static readonly int KEYEVENTF_KEYUP = 0x0002;
        public static readonly int KEYEVENTF_UNICODE = 0x0004;
        public static readonly int KEYEVENTF_SCANCODE = 0x0008;

        public static readonly int WM_KEYDOWN = 0x0100; // 256
        public static readonly int WM_KEYUP = 0x0101; // 257
        public static readonly int WM_CHAR = 0x0102; // 258
        public static readonly int WM_SYSKEYDOWN = 0x0104; // 260
        public static readonly int WM_SYSKEYUP = 0x0105; // 261

        public static readonly int INPUT_MOUSE = 0;
        public static readonly int INPUT_KEYBOARD = 1;
        public static readonly int INPUT_HARDWARE = 2;

        #endregion

        public KeyboardOutput()
        {
        }

        #region IModule Members

        public string Name
        {
            get { return "KeyboardOutput"; }
        }

        public string Description
        {
            get { return "Moduł do emulowania klawiszy klawiatury."; }
        }

        public string Author
        {
            get { return "codeking"; }
        }

        public string Contact
        {
            get { return "codeking@homesimcockpit.com"; }
        }

        private Version _version = new Version(1, 0, 0, 1);

        public Version Version
        {
            get { return _version; }
        }

        private HomeSimCockpitSDK.ILog _log = null;

        public void Load(HomeSimCockpitSDK.ILog log)
        {
            _log = log;
        }

        public void Unload()
        {
            Stop(HomeSimCockpitSDK.StartStopType.Output);
        }

        public void Start(HomeSimCockpitSDK.StartStopType startType)
        {
        }

        public void Stop(HomeSimCockpitSDK.StartStopType stopType)
        {
        }

        #endregion

        #region IOutput Members

        public HomeSimCockpitSDK.IVariable[] OutputVariables
        {
            get { return new HomeSimCockpitSDK.IVariable[0]; }
        }

        public void RegisterChangableVariable(string variableID, HomeSimCockpitSDK.VariableType type)
        {
            throw new Exception("Moduł nie udostępnia żadnych zmiennych.");
        }

        public void UnregisterChangableVariable(string variableID)
        {
            throw new Exception("Moduł nie udostępnia żadnych zmiennych.");
        }

        public void SetVariableValue(string variableID, object value)
        {
            throw new Exception("Moduł nie udostępnia żadnych zmiennych.");
        }

        #endregion

        #region IModuleFunctions Members

        public HomeSimCockpitSDK.ModuleFunctionInfo[] Functions
        {
            get
            {
                return new HomeSimCockpitSDK.ModuleFunctionInfo[] 
                { 
                        new HomeSimCockpitSDK.ModuleFunctionInfo("SendKeys", "Funkcja naciska po kolei każdy klawisz oddzielnie.", -1, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(SendKeys))
                    ,   new HomeSimCockpitSDK.ModuleFunctionInfo("SendShortcut", "Funkcja naciska wszystkie klawisze razem.", -1, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(SendShortcut))
                    ,   new HomeSimCockpitSDK.ModuleFunctionInfo("SendKeysDown", "Funkcja naciska wszystkie klawisze razem ale nie zwalnia ich.", -1, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(SendKeysDown))
                    ,   new HomeSimCockpitSDK.ModuleFunctionInfo("SendKeysUp", "Funkcja zwalnia po kolei wszystkie klawisze.", -1, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(SendKeysUp))
                };
            }
        }

        #endregion

        private object Send(object[] arguments, bool sequence, bool downs, bool ups)
        {
            if (arguments == null || arguments.Length == 0)
            {
                return true;
            }
            string keys = (string)arguments[0];
            if (string.IsNullOrEmpty(keys))
            {
                return true;
            }
            string windowClass = null;
            if (arguments.Length > 1)
            {
                windowClass = arguments[1] as string;
            }
            string windowName = null;
            if (arguments.Length > 2)
            {
                windowName = arguments[2] as string;
            }
            bool getPreviousWindow = true;
            if (arguments.Length > 3)
            {
                getPreviousWindow = (bool)arguments[3];
            }
            IntPtr prevoius = IntPtr.Zero;
            IntPtr found = IntPtr.Zero;
            if (windowClass != null || windowName != null)
            {
                prevoius = GetForegroundWindow();
                found = FindWindow(windowClass, windowName);
                if (found != IntPtr.Zero)
                {
                    getPreviousWindow = getPreviousWindow && SetForegroundWindow(found);
                }
                else
                {
                    getPreviousWindow = false;
                }
            }
            Queue<SKEvent> k = ParseKeys(keys, IntPtr.Zero);
            byte[] keyboardState = GetKeyboardState();
            if (!sequence)
            {
                Queue<SKEvent> vkdowns = null;
                Queue<SKEvent> vkups = null;
                GetEvents(k, out vkdowns, out vkups);
                if (downs)
                {
                    SendInput(keyboardState, vkdowns);
                }
                if (ups)
                {
                    SendInput(keyboardState, vkups);
                }
            }
            else
            {
                SendInput(keyboardState, k);
            }
            if (getPreviousWindow && prevoius != IntPtr.Zero)
            {
                SetForegroundWindow(prevoius);
            }
            return true;
        }

        private object SendKeys(object[] arguments)
        {
            return Send(arguments, true, false, false);
        }

        private object SendShortcut(object[] arguments)
        {
            return Send(arguments, false, true, true);
        }

        private object SendKeysDown(object[] arguments)
        {
            return Send(arguments, false, true, false);
        }

        private object SendKeysUp(object[] arguments)
        {
            return Send(arguments, false, false, true);
        }

        private void GetEvents(Queue<SKEvent> events, out Queue<SKEvent> keysDown, out Queue<SKEvent> keysUp)
        {
            keysDown = new Queue<SKEvent>();
            keysUp = new Queue<SKEvent>();
            SKEvent[] evs = events.ToArray();
            for (int i = 0; i < evs.Length; i++)
            {
                SKEvent ev = evs[i];
                if (ev.IsDown)
                {
                    if (!keysDown.Contains(ev))
                    {
                        keysDown.Enqueue(ev);
                    }
                }
                ev = evs[evs.Length - i - 1];
                if (!ev.IsDown)
                {
                    if (!keysUp.Contains(ev))
                    {
                        keysUp.Enqueue(ev);
                    }
                }
            }
        }

        private void ClearGlobalKeys()
        {
            capslockChanged = false;
            numlockChanged = false;
            scrollLockChanged = false;
            kanaChanged = false;
        } 

        private bool capslockChanged = false;
        private bool numlockChanged = false;
        private bool scrollLockChanged = false;
        private bool kanaChanged = false;

        private void SendInput(byte[] oldKeyboardState, Queue<SKEvent> events)
        {
            int count;
            //AddCancelModifiersForPreviousEvents(previousEvents);
            INPUT[] pInputs = new INPUT[2];
            pInputs[0].type = INPUT_KEYBOARD;
            pInputs[0].inputUnion.ki.dwExtraInfo = IntPtr.Zero;
            pInputs[0].inputUnion.ki.time = 0;
            pInputs[1].type = INPUT_KEYBOARD;
            pInputs[1].inputUnion.ki.wVk = 0;
            pInputs[1].inputUnion.ki.dwFlags = KEYEVENTF_KEYUP | KEYEVENTF_UNICODE;
            pInputs[1].inputUnion.ki.dwExtraInfo = IntPtr.Zero;
            pInputs[1].inputUnion.ki.time = 0;
            int cbSize = Marshal.SizeOf(typeof(INPUT));
            uint num2 = 0;
            lock (events)
            {
                bool flag = BlockInput(true);
                try
                {
                    count = events.Count;
                    ClearGlobalKeys();
                    for (int i = 0; i < count; i++)
                    {
                        SKEvent skEvent = (SKEvent)events.Dequeue();
                        pInputs[0].inputUnion.ki.dwFlags = 0;
                        if (skEvent.wm == WM_CHAR)
                        {
                            int vk = pInputs[0].inputUnion.ki.wVk;
                            pInputs[0].inputUnion.ki.wVk = 0;
                            pInputs[0].inputUnion.ki.wScan = (short)MapVirtualKey( vk , 0 /* MAPVK_VK_TO_VSC */);// (short)skEvent.paramL;
                            pInputs[0].inputUnion.ki.dwFlags = KEYEVENTF_UNICODE;
                            pInputs[1].inputUnion.ki.wScan = (short)MapVirtualKey( vk , 0 /* MAPVK_VK_TO_VSC */);// (short)skEvent.paramL;
                            num2 += SendInput(2, pInputs, cbSize) - 1;
                        }
                        else
                        {
                            //pInputs[0].inputUnion.ki.wScan = 0;
                            pInputs[0].inputUnion.ki.wScan = (short)MapVirtualKey( skEvent.paramL , 0 /* MAPVK_VK_TO_VSC */);
                            if ((skEvent.wm == WM_KEYUP) || (skEvent.wm == WM_SYSKEYUP))
                            {
                                pInputs[0].inputUnion.ki.dwFlags |= KEYEVENTF_KEYUP;
                            }
                            if (IsExtendedKey(skEvent))
                            {
                                pInputs[0].inputUnion.ki.dwFlags |= KEYEVENTF_EXTENDEDKEY;
                            }
                            pInputs[0].inputUnion.ki.wVk = (short)skEvent.paramL;
                            num2 += SendInput(1, pInputs, cbSize);
                            CheckGlobalKeys(skEvent);
                        }
                        Thread.Sleep(1);
                    }
                    ResetKeyboardUsingSendInput(cbSize);
                }
                finally
                {
                    SetKeyboardState(oldKeyboardState);
                    if (flag)
                    {
                        BlockInput(false);
                    }
                }
            }
            if (num2 != count)
            {
                throw new System.ComponentModel.Win32Exception();
            }
        }
 
        private void ResetKeyboardUsingSendInput(int INPUTSize)
        {
            if ((capslockChanged || numlockChanged) || (scrollLockChanged || kanaChanged))
            {
                INPUT[] pInputs = new INPUT[2];
                pInputs[0].type = INPUT_KEYBOARD;
                pInputs[0].inputUnion.ki.dwFlags = 0;
                pInputs[1].type = INPUT_KEYBOARD;
                pInputs[1].inputUnion.ki.dwFlags = KEYEVENTF_KEYUP;
                if (capslockChanged)
                {
                    pInputs[0].inputUnion.ki.wVk =  20;
                    pInputs[1].inputUnion.ki.wVk = 20;
                    SendInput(2, pInputs, INPUTSize);
                }
                if (numlockChanged)
                {
                    pInputs[0].inputUnion.ki.wVk = 144;
                    pInputs[1].inputUnion.ki.wVk = 144;
                    SendInput(2, pInputs, INPUTSize);
                }
                if (scrollLockChanged)
                {
                    pInputs[0].inputUnion.ki.wVk = 145;
                    pInputs[1].inputUnion.ki.wVk = 145;
                    SendInput(2, pInputs, INPUTSize);
                }
                if (kanaChanged)
                {
                    pInputs[0].inputUnion.ki.wVk = 21;
                    pInputs[1].inputUnion.ki.wVk = 21;
                    SendInput(2, pInputs, INPUTSize);
                }
            }
        }

        private void CheckGlobalKeys(SKEvent skEvent)
        {
            if (skEvent.wm == WM_KEYDOWN)
            {
                switch (skEvent.paramL)
                {
                    case 20:
                        capslockChanged = !capslockChanged;
                        return;

                    case 21:
                        kanaChanged = !kanaChanged;
                        return;

                    case 144:
                        numlockChanged = !numlockChanged;
                        return;

                    case 145:
                        scrollLockChanged = !scrollLockChanged;
                        return;

                    default:
                        return;
                }
            }
        }

        private bool IsExtendedKey(SKEvent skEvent)
        {
            if (((((skEvent.paramL != 38) && (skEvent.paramL != 40)) && ((skEvent.paramL != 37) && (skEvent.paramL != 39))) && (((skEvent.paramL != 33) && (skEvent.paramL != 34)) && ((skEvent.paramL != 36) && (skEvent.paramL != 35)))) && (skEvent.paramL != 45))
            {
                return (skEvent.paramL == 46);
            }
            return true;
        }
 
        private byte[] GetKeyboardState()
        {
            byte[] keystate = new byte[256];
            GetKeyboardState(keystate);
            return keystate;
        }

        private Queue<SKEvent> ParseKeys(string keys, IntPtr hwnd)
        {
            Queue<SKEvent> events = new Queue<SKEvent>();
            int num = 0;
            int[] haveKeys = new int[3];
            int cGrp = 0;
            bool fStartNewChar = true;
            int length = keys.Length;
            while (num < length)
            {
                int num6;
                int num7;
                int repeat = 1;
                char ch = keys[num];
                int vk = 0;
                int scan = MapVirtualKey(ch, 0 /* MAPVK_VK_TO_VSC */);
                switch (ch)
                {
                    case '%':
                        if (haveKeys[2] != 0)
                        {
                            throw new Exception(string.Format("Niepoprawny ciąg klawiaturowy '{0}'.", keys));
                        }
                        goto Label_03C9;

                    case '(':
                        cGrp++;
                        if (cGrp > 3)
                        {
                            throw new Exception(string.Format("Maksymalna ilość grupowań wynosi 3."));
                        }
                        goto Label_0414;

                    case ')':
                        if (cGrp < 1)
                        {
                            throw new Exception(string.Format("Niepoprawny ciąg klawiaturowy '{0}'.", keys));
                        }
                        goto Label_045A;

                    case '+':
                        if (haveKeys[0] != 0)
                        {
                            throw new Exception(string.Format("Niepoprawny ciąg klawiaturowy '{0}'.", keys));
                        }
                        goto Label_0333;

                    case '^':
                        if (haveKeys[1] != 0)
                        {
                            throw new Exception(string.Format("Niepoprawny ciąg klawiaturowy '{0}'.", keys));
                        }
                        events.Enqueue(new SKEvent(WM_KEYDOWN, 17, fStartNewChar, hwnd, scan));
                        fStartNewChar = false;
                        haveKeys[1] = 10;
                        goto Label_04AB;

                    case '{':
                        num6 = num + 1;
                        if (((num6 + 1) >= length) || (keys[num6] != '}'))
                        {
                            goto Label_00EB;
                        }
                        num7 = num6 + 1;
                        goto Label_00C7;

                    case '}':
                        throw new Exception(string.Format("Niepoprawny ciąg klawiaturowy '{0}'.", keys));

                    case '~':
                        vk = 13;
                        AddMsgsForVK(events, vk, repeat, (haveKeys[2] > 0) && (haveKeys[1] == 0), hwnd, fStartNewChar);
                        goto Label_04AB;

                    default:
                        fStartNewChar = AddSimpleKey(events, keys[num], repeat, hwnd, haveKeys, fStartNewChar, cGrp);
                        goto Label_04AB;
                }
            Label_00C1:
                num7++;
            Label_00C7:
                if ((num7 < length) && (keys[num7] != '}'))
                {
                    goto Label_00C1;
                }
                if (num7 < length)
                {
                    num6++;
                }
            Label_00EB:
                while (((num6 < length) && (keys[num6] != '}')) && !char.IsWhiteSpace(keys[num6]))
                {
                    num6++;
                }
                if (num6 >= length)
                {
                    throw new Exception("Brak znaku oddzielającego.");
                }
                string keyword = keys.Substring(num + 1, num6 - (num + 1));
                if (char.IsWhiteSpace(keys[num6]))
                {
                    while ((num6 < length) && char.IsWhiteSpace(keys[num6]))
                    {
                        num6++;
                    }
                    if (num6 >= length)
                    {
                        throw new Exception("Brak znaku oddzielającego.");
                    }
                    if (char.IsDigit(keys[num6]))
                    {
                        int startIndex = num6;
                        while ((num6 < length) && char.IsDigit(keys[num6]))
                        {
                            num6++;
                        }
                        repeat = int.Parse(keys.Substring(startIndex, num6 - startIndex), System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                if (num6 >= length)
                {
                    throw new Exception("Brak znaku oddzielającego.");
                }
                if (keys[num6] != '}')
                {
                    throw new Exception("Podana wartość powtórzeń jest niepoprawna.");
                }
                vk = MatchKeyword(keyword);
                if (vk != -1)
                {
                    if ((haveKeys[0] == 0) && ((vk & 65536) != 0))
                    {
                        events.Enqueue(new SKEvent(WM_KEYDOWN, 16, fStartNewChar, hwnd, scan));
                        fStartNewChar = false;
                        haveKeys[0] = 10;
                    }
                    if ((haveKeys[1] == 0) && ((vk & 131072) != 0))
                    {
                        events.Enqueue(new SKEvent(WM_KEYDOWN, 17, fStartNewChar, hwnd, scan));
                        fStartNewChar = false;
                        haveKeys[1] = 10;
                    }
                    if ((haveKeys[2] == 0) && ((vk & 262144) != 0))
                    {
                        events.Enqueue(new SKEvent(WM_KEYDOWN, 18, fStartNewChar, hwnd, scan));
                        fStartNewChar = false;
                        haveKeys[2] = 10;
                    }
                    AddMsgsForVK(events, vk, repeat, (haveKeys[2] > 0) && (haveKeys[1] == 0), hwnd, fStartNewChar);
                    CancelMods(events, haveKeys, 10, hwnd, scan);
                }
                else
                {
                    if (keyword.Length != 1)
                    {
                        throw new Exception(string.Format("Klawisz '{0}' jest niepoprawny.", keys.Substring(num + 1, num6 - (num + 1))));
                    }
                    fStartNewChar = AddSimpleKey(events, keyword[0], repeat, hwnd, haveKeys, fStartNewChar, cGrp);
                }
                num = num6;
                goto Label_04AB;
            Label_0333:
                events.Enqueue(new SKEvent(WM_KEYDOWN, 16, fStartNewChar, hwnd, scan));
                fStartNewChar = false;
                haveKeys[0] = 10;
                goto Label_04AB;
            Label_03C9:
                events.Enqueue(new SKEvent((haveKeys[1] != 0) ? WM_KEYDOWN : WM_SYSKEYDOWN, 18, fStartNewChar, hwnd, scan));
                fStartNewChar = false;
                haveKeys[2] = 10;
                goto Label_04AB;
            Label_0414:
                if (haveKeys[0] == 10)
                {
                    haveKeys[0] = cGrp;
                }
                if (haveKeys[1] == 10)
                {
                    haveKeys[1] = cGrp;
                }
                if (haveKeys[2] == 10)
                {
                    haveKeys[2] = cGrp;
                }
                goto Label_04AB;
            Label_045A:
                CancelMods(events, haveKeys, cGrp, hwnd, scan);
                cGrp--;
                if (cGrp == 0)
                {
                    fStartNewChar = true;
                }
            Label_04AB:
                num++;
            }
            if (cGrp != 0)
            {
                throw new Exception("Brak znaku oddzielającego.");
            }
            CancelMods(events, haveKeys, 10, hwnd, 0);
            return events;
        }

        private void AddMsgsForVK(Queue<SKEvent> events, int vk, int repeat, bool altnoctrldown, IntPtr hwnd, bool fStartNewChar)
        {            
            int scan = MapVirtualKey(vk, 0 /* MAPVK_VK_TO_VSC */);
            for (int i = 0; i < repeat; i++)
            {
                events.Enqueue(new SKEvent(altnoctrldown ? WM_SYSKEYDOWN : WM_KEYDOWN, vk, fStartNewChar, hwnd, scan));
                events.Enqueue(new SKEvent(altnoctrldown ? WM_SYSKEYUP : WM_KEYUP, vk, fStartNewChar, hwnd, scan));
            }
        }

        private bool AddSimpleKey(Queue<SKEvent> events, char character, int repeat, IntPtr hwnd, int[] haveKeys, bool fStartNewChar, int cGrp)
        {
            int scan = MapVirtualKey(character, 0 /* MAPVK_VK_TO_VSC */);
            int num = VkKeyScan(character);
            if (num != -1)
            {
                if ((haveKeys[0] == 0) && ((num & 256) != 0))
                {
                    events.Enqueue(new SKEvent(WM_KEYDOWN, 16, fStartNewChar, hwnd, scan));
                    fStartNewChar = false;
                    haveKeys[0] = 10;
                }
                if ((haveKeys[1] == 0) && ((num & 512) != 0))
                {
                    events.Enqueue(new SKEvent(WM_KEYDOWN, 17, fStartNewChar, hwnd, scan));
                    fStartNewChar = false;
                    haveKeys[1] = 10;
                }
                if ((haveKeys[2] == 0) && ((num & 1024) != 0))
                {
                    events.Enqueue(new SKEvent(WM_KEYDOWN, 18, fStartNewChar, hwnd, scan));
                    fStartNewChar = false;
                    haveKeys[2] = 10;
                }
                AddMsgsForVK(events, num & 255, repeat, (haveKeys[2] > 0) && (haveKeys[1] == 0), hwnd, fStartNewChar);
                CancelMods(events, haveKeys, 10, hwnd, scan);
            }
            else
            {
                int num2 = OemKeyScan((short)('\x00ff' & character));
                for (int i = 0; i < repeat; i++)
                {
                    events.Enqueue(new SKEvent(WM_CHAR, VkKeyScan(character), num2 & 65535, hwnd, scan));
                    //events.Enqueue(new SKEvent(WM_CHAR, character, num2 & 65535, hwnd, scan));
                }
            }
            if (cGrp != 0)
            {
                fStartNewChar = true;
            }
            return fStartNewChar;
        }

        private void CancelMods(Queue<SKEvent> events, int[] haveKeys, int level, IntPtr hwnd, int scan)
        {
            if (haveKeys[0] == level)
            {
                events.Enqueue(new SKEvent(WM_KEYUP, 16, false, hwnd, MapVirtualKey(16, 0 /* MAPVK_VK_TO_VSC */)));
                haveKeys[0] = 0;
            }
            if (haveKeys[1] == level)
            {
                events.Enqueue(new SKEvent(WM_KEYUP, 17, false, hwnd, MapVirtualKey(17, 0 /* MAPVK_VK_TO_VSC */)));
                haveKeys[1] = 0;
            }
            if (haveKeys[2] == level)
            {
                events.Enqueue(new SKEvent(WM_SYSKEYUP, 18, false, hwnd, MapVirtualKey(18, 0 /* MAPVK_VK_TO_VSC */)));
                haveKeys[2] = 0;
            }
        }

        private int MatchKeyword(string keyword)
        {
            for (int i = 0; i < keywords.Length; i++)
            {
                if (string.Equals(keywords[i].keyword, keyword, StringComparison.OrdinalIgnoreCase))
                {
                    return keywords[i].vk;
                }
            }
            return -1;
        }
 
        private static KeywordVk[] keywords;

        static KeyboardOutput()
        {
            keywords = new KeywordVk[] { 
                new KeywordVk("ENTER", 13), new KeywordVk("TAB", 9), new KeywordVk("ESC", 27), new KeywordVk("ESCAPE", 27), new KeywordVk("HOME", 36), new KeywordVk("END", 35), new KeywordVk("LEFT", 37), new KeywordVk("RIGHT", 39), new KeywordVk("UP", 38), new KeywordVk("DOWN", 40), new KeywordVk("PGUP", 33), new KeywordVk("PGDN", 34), new KeywordVk("NUMLOCK", 144), new KeywordVk("SCROLLLOCK", 145), new KeywordVk("PRTSC", 44), new KeywordVk("BREAK", 3), 
                new KeywordVk("BACKSPACE", 8), new KeywordVk("BKSP", 8), new KeywordVk("BS", 8), new KeywordVk("CLEAR", 12), new KeywordVk("CAPSLOCK", 20), new KeywordVk("INS", 45), new KeywordVk("INSERT", 45), new KeywordVk("DEL", 46), new KeywordVk("DELETE", 46), new KeywordVk("HELP", 47), new KeywordVk("F1", 112), new KeywordVk("F2", 113), new KeywordVk("F3", 114), new KeywordVk("F4", 115), new KeywordVk("F5", 116), new KeywordVk("F6", 117), 
                new KeywordVk("F7", 118), new KeywordVk("F8", 119), new KeywordVk("F9", 120), new KeywordVk("F10", 121), new KeywordVk("F11", 122), new KeywordVk("F12", 123), new KeywordVk("F13", 124), new KeywordVk("F14", 125), new KeywordVk("F15", 126), new KeywordVk("F16", 127), new KeywordVk("MULTIPLY", 106), new KeywordVk("ADD", 107), new KeywordVk("SUBTRACT", 109), new KeywordVk("DIVIDE", 111), new KeywordVk("+", 107), new KeywordVk("%", 65589), 
                new KeywordVk("^", 65590)
             };
        }

        #region IModuleHelp Members

        public void Help(System.Windows.Forms.IWin32Window parent)
        {
            System.Diagnostics.Process.Start("iexplore.exe", System.IO.Path.ChangeExtension(System.Reflection.Assembly.GetExecutingAssembly().Location, ".html"));
        }

        #endregion
    }
}

