/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-01-06
 * Godzina: 21:04
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace LCDOnLPT
{
    /// <summary>
    /// Description of LPTPort.
    /// </summary>
    class LPTPort
    {
        [DllImport("inpout32.dll", EntryPoint="Out32")]
        private static extern void Output(int adress, int value);
        
        public LPTPort(int lptAddress)
        {
            _lptData = lptAddress;
            _lptControl = lptAddress + 2;
        }
        
        private int _lptData = 0;
        private int _lptControl = 0;
        
        private const int RS = 4;
        private const int RW = 2;
        private const int E1 = 1;
        private const int E2 = 8;
        private const int CTRL_MASK = 11;
        
        public void WriteControl(LCDSeq lcd, int control, int multiplier)
        {
            Output(_lptControl, 0 ^ CTRL_MASK);
            Output(_lptData, control);
            LCDOnLPTModule.Wait(LPTLCD.uiDelayBus, multiplier);
            if (lcd == LCDSeq.LCD1)
            {
                Output(_lptControl, E1 ^ CTRL_MASK);
            }
            else
            {
                Output(_lptControl, E2 ^ CTRL_MASK);
            }
            LCDOnLPTModule.Wait(LPTLCD.uiDelayBus, multiplier);
            Output(_lptControl, 0 ^ CTRL_MASK);
            LCDOnLPTModule.Wait(LPTLCD.uiDelayShort, multiplier);
            
            //Debug.WriteLine(string.Format("LCD = {0}, Command = {1}", lcd, control));
        }
        
        public void WriteData(LCDSeq lcd, int data, int multiplier)
        {
            Output(_lptControl, RS ^ CTRL_MASK);
            Output(_lptData, data);
            LCDOnLPTModule.Wait(LPTLCD.uiDelayBus, multiplier);
            if (lcd == LCDSeq.LCD1)
            {
                Output(_lptControl, (RS | E1) ^ CTRL_MASK);
            }
            else
            {
                Output(_lptControl, (RS | E2) ^ CTRL_MASK);
            }
            LCDOnLPTModule.Wait(LPTLCD.uiDelayBus, multiplier);
            Output(_lptControl, RS ^ CTRL_MASK);
            LCDOnLPTModule.Wait(LPTLCD.uiDelayShort, multiplier);
            
            //Debug.WriteLine(string.Format("LCD = {0}, Data = {1}", lcd, data));
        }
    }
}
