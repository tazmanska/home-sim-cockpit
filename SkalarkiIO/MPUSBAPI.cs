/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2009-12-02
 * Godzina: 22:34
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Runtime.InteropServices;

namespace SkalarkiIO
{
	/// <summary>
	/// Description of MPUSBAPI.
	/// </summary>
	public unsafe static class MPUSBAPI
	{
		public static readonly uint MP_WRITE = 0;		
		public static readonly uint MP_READ = 1;
		public static readonly uint MPUSB_FAIL = 0;
		public static readonly uint MPUSB_SUCCESS = 1;
		public static readonly uint MAX_NUM_MPUSB_DEV = 127;
		
		[DllImport("mpusbapi.dll")]
	    public static extern uint _MPUSBGetDLLVersion();
	    
	    [DllImport("mpusbapi.dll")]
	    public static extern uint _MPUSBGetDeviceCount(string pVID_PID);
	    
	    [DllImport("mpusbapi.dll")]
	    public static extern void* _MPUSBOpen(uint instance, string pVID_PID, string pEP, uint dwDir, uint dwReserved);
	    
	    [DllImport("mpusbapi.dll")]
	    public static extern uint _MPUSBRead(void* handle, void* pData, uint dwLen, uint* pLength, uint dwMilliseconds);
	    
	    [DllImport("mpusbapi.dll")]
	    public static extern uint _MPUSBWrite(void* handle, void* pData, uint dwLen, uint* pLength, uint dwMilliseconds);
	    
	    [DllImport("mpusbapi.dll")]
	    public static extern uint _MPUSBReadInt(void* handle, void* pData, uint dwLen, uint* pLength, uint dwMilliseconds);
	    
	    [DllImport("mpusbapi.dll")]
	    public static extern bool _MPUSBClose(void* handle);
	}
}
