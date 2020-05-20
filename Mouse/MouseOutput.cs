/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-01-20
 * Godzina: 19:46
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Mouse
{
    /// <summary>
    /// Description of MyClass.
    /// </summary>
    public class MouseOutput : HomeSimCockpitSDK.IModule, HomeSimCockpitSDK.IOutput, HomeSimCockpitSDK.IModuleConfiguration, HomeSimCockpitSDK.IModuleFunctions
    {
        public string Name
        {
            get { return "MouseOutput"; }
        }

        public string Description
        {
            get { return "Moduł do obsługi myszy, pozwala na ruszanie kursorem, klikanie itp.."; }
        }

        public string Author
        {
            get { return "codeking"; }
        }

        public string Contact
        {
            get { return "codeking@homesimcockpit.com"; }
        }

        private Version _version = new Version(1, 0, 0, 0);

        public Version Version
        {
            get { return _version; }
        }

        private HomeSimCockpitSDK.ILog _log = null;

        private string _xmlConfigurationFilePath = null;
        
        private ModuleConfiguration _configuration = null;

        public void Load(HomeSimCockpitSDK.ILog log)
        {
            _log = log;

            LoadConfiguration();
        }
        
        private void LoadConfiguration()
        {
            // utworzenie ścieżki do pliku konfiguracyjnego
            _xmlConfigurationFilePath = Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, ".xml");

            // wczytanie konfiguracji
            _configuration = ModuleConfiguration.Load(_xmlConfigurationFilePath);
            
            _outputs = _configuration.Clicks;
        }
        
        public void Unload()
        {
            
        }
        
        private volatile bool _working = false;
        
        public void Start(HomeSimCockpitSDK.StartStopType startStopType)
        {
            _working = true;
            for (int i = 0; i < _outputs.Length; i++)
            {
                _outputs[i].Reset();
                _outputs[i].Mouse = this;
            }
        }
        
        public void Stop(HomeSimCockpitSDK.StartStopType startStopType)
        {
            _working = false;
        }
        
        private Click[] _outputs = null;
        
        public HomeSimCockpitSDK.IVariable[] OutputVariables
        {
            get { return _outputs; }
        }
        
        private Dictionary<string, Click> _registered = new Dictionary<string, Click>();
        
        public void RegisterChangableVariable(string variableID, HomeSimCockpitSDK.VariableType type)
        {
            foreach (Click v in _outputs)
            {
                if (v.ID == variableID && v.Type == type)
                {
                    if (!_registered.ContainsKey(variableID))
                    {
                        _registered.Add(variableID, v);
                    }
                    return;
                }
            }
            throw new Exception(string.Format("Brak zmiennej o identyfikatorze '{0}' i type '{1}'.", variableID, type));
        }

        public void UnregisterChangableVariable(string variableID)
        {
            if (_registered.ContainsKey(variableID))
            {
                _registered.Remove(variableID);
                return;
            }
            throw new Exception(string.Format("Brak zarejestrowanej zmiennej o identyfikatorze '{0}'.", variableID));
        }
        
        public void SetVariableValue(string variableID, object value)
        {
            _registered[variableID].SetValue(value);
        }
        
        public bool Configuration(System.Windows.Forms.IWin32Window parent)
        {
            if (_working)
            {
                MessageBox.Show(parent, "Konfiguracja jest niedostępna w trakcie działania skryptu korzystającego z tego modułu.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            ConfigDialog cd = new ConfigDialog(ModuleConfiguration.Load(_xmlConfigurationFilePath));
            if (cd.ShowDialog(parent) == System.Windows.Forms.DialogResult.OK)
            {
                ModuleConfiguration.Save(_xmlConfigurationFilePath, cd.Configuration);
                _configuration = cd.Configuration;
                LoadConfiguration();
                return true;
            }
            return false;
        }
        
        public HomeSimCockpitSDK.ModuleFunctionInfo[] Functions
        {
            get
            {
                return new HomeSimCockpitSDK.ModuleFunctionInfo[]
                {
                    new HomeSimCockpitSDK.ModuleFunctionInfo("MoveTo", "Funkcja przesuwa kursor myszy do wskazanego miejsca (x, y). Przykład: MoveTo(100, 200).", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(MoveTo))
                        ,   new HomeSimCockpitSDK.ModuleFunctionInfo("Move", "Funkcja przesuwa kursor myszy o wskazane odległości. Przykład: Move(50, -100).", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(Move))
                        ,   new HomeSimCockpitSDK.ModuleFunctionInfo("Click", "Funkcja naciska wskazany klawisz myszy. Może przyjąć 0 parametrów (kliknięcie lewym klawiszem w miejscu w którym znajduje się kursror), 1 parametr (numer przycisku (0 - lewy, 1 - środkowy, 2 - prawy, 3 - dodatkowy 1, 4 - dodatkowy 2, 5 - rolka do góry, 6 - rolka w dół) lub identyfikator zmiennej), 3 parametry (numer przycisku i współprzędne (x, y) np. Click(0, 150, 300)).", -1, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(Click))
                };
            }
        }
        
        private object MoveTo(object[] arguments)
        {
            int x = (int)arguments[0];
            int y = (int)arguments[1];
            Cursor.Position = new System.Drawing.Point(x, y);
            return true;
        }
        
        private object Move(object[] arguments)
        {
            int x = (int)arguments[0];
            int y = (int)arguments[1];
            System.Drawing.Point pos = Cursor.Position;
            pos.Offset(x, y);
            Cursor.Position = pos;
            return true;
        }
        
        private object Click(object[] arguments)
        {
            if (arguments != null && arguments.Length > 0 && arguments[0] is string)
            {
                int count = 0;
                for (int i = 0; i < arguments.Length; i++)
                {
                    string id = (string)arguments[i];
                    Click c = Array.Find<Click>(_outputs, delegate(Click o)
                                                {
                                                    return o.ID == id;
                                                });
                    if (c != null)
                    {
                        if (Click(c))
                        {
                            count++;
                        }
                    }
                }
                return count > 0;
            }
            
            // przycisk 0 - lewy, 1 - srodkowy, 2 - prawy
            MouseButton but = MouseButton.Left;
            int x = Cursor.Position.X;
            int y = Cursor.Position.Y;
            if (arguments.Length > 0)
            {
                but = (MouseButton)(int)arguments[0];
            }
            if (arguments.Length > 1)
            {
                x = (int)arguments[1];
            }
            if (arguments.Length > 2)
            {
                y = (int)arguments[2];
            }
            return Click(but, x, y);
        }
        
        private bool Click(MouseButton button, int x, int y)
        {
            System.Drawing.Point previousPosition = Cursor.Position;
            Cursor.Position = new System.Drawing.Point(x, y);
            int flagsDown = MOUSEEVENTF_LEFTDOWN;
            int flagsUp = MOUSEEVENTF_LEFTUP;
            int mouseData = 0;
            switch (button)
            {
                case MouseButton.Middle:
                    flagsDown = MOUSEEVENTF_MIDDLEDOWN;
                    flagsUp = MOUSEEVENTF_MIDDLEUP;
                    break;
                    
                case MouseButton.Right:
                    flagsDown = MOUSEEVENTF_RIGHTDOWN;
                    flagsUp = MOUSEEVENTF_RIGHTUP;
                    break;
                    
                case MouseButton.XButton1:
                    flagsDown = MOUSEEVENTF_XDOWN;
                    flagsUp = MOUSEEVENTF_XUP;
                    mouseData = XBUTTON1;
                    break;
                    
                case MouseButton.XButton2:
                    flagsDown = MOUSEEVENTF_XDOWN;
                    flagsUp = MOUSEEVENTF_XUP;
                    mouseData = XBUTTON2;
                    break;
            }
            
            if (button == MouseButton.WheelDown || button == MouseButton.WheelUp)
            {
                INPUT[] ins = new INPUT[1];
                INPUT inp = new INPUT();
                inp.type = INPUT_MOUSE;
                inp.inputUnion.mi.dwExtraInfo = IntPtr.Zero;
                inp.inputUnion.mi.dwFlags = MOUSEEVENTF_WHEEL;
                inp.inputUnion.mi.dx = 0;
                inp.inputUnion.mi.dy = 0;
                inp.inputUnion.mi.mouseData = button == MouseButton.WheelDown ? -1 : 1;
                inp.inputUnion.mi.time = 0;
                ins[0] = inp;
                SendInput((uint)ins.Length, ins, Marshal.SizeOf(typeof(INPUT)));
            }
            else
            {
                INPUT[] ins = new INPUT[2];
                INPUT inp = new INPUT();
                inp.type = INPUT_MOUSE;
                inp.inputUnion.mi.dwExtraInfo = IntPtr.Zero;
                inp.inputUnion.mi.dwFlags = flagsDown;
                inp.inputUnion.mi.dx = 0;
                inp.inputUnion.mi.dy = 0;
                inp.inputUnion.mi.mouseData = mouseData;
                inp.inputUnion.mi.time = 0;
                ins[0] = inp;
                inp = new INPUT();
                inp.type = INPUT_MOUSE;
                inp.inputUnion.mi.dwExtraInfo = IntPtr.Zero;
                inp.inputUnion.mi.dwFlags = flagsUp;
                inp.inputUnion.mi.dx = 0;
                inp.inputUnion.mi.dy = 0;
                inp.inputUnion.mi.mouseData = mouseData;
                inp.inputUnion.mi.time = 0;
                ins[1] = inp;
                SendInput((uint)ins.Length, ins, Marshal.SizeOf(typeof(INPUT)));
            }
            Cursor.Position = previousPosition;
            return true;
        }
        
        public bool Click(Click click)
        {
            return Click(click.Button, click.X, click.Y);
        }
        
        public const int MOUSEEVENTF_LEFTDOWN = 0x0002; /* left button down */
        public const int MOUSEEVENTF_LEFTUP = 0x0004; /* left button up */
        public const int MOUSEEVENTF_RIGHTDOWN = 0x0008; /* right button down */
        public const int MOUSEEVENTF_RIGHTUP = 0x0010; /* right button up */
        public const int MOUSEEVENTF_MIDDLEDOWN = 0x0020; /* middle button down */
        public const int MOUSEEVENTF_MIDDLEUP = 0x0040; /* middle button up */
        public const int MOUSEEVENTF_XDOWN = 0x0080; /* x button down */
        public const int MOUSEEVENTF_XUP = 0x0100; /* x button up */
        public const int MOUSEEVENTF_WHEEL = 0x0800; /* wheel button rolled */
        
        public const int XBUTTON1 = 0x0001;
        public const int XBUTTON2 = 0x0002;
        
        public const int INPUT_MOUSE = 0;
        public const int INPUT_KEYBOARD = 1;
        public const int INPUT_HARDWARE = 2;
        
        #region WinAPI functions

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);
        
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
    }
}