/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2012-03-19
 * Godzina: 21:52
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace HomeSimCockpit
{
	/// <summary>
	/// Description of WinAPI.
	/// </summary>
	public static class WinAPI
	{
		public struct RECT
		{
			int left;
			int top;
			int right;
			int bottom;

			public int Left
			{
				get { return this.left; }
			}

			public int Top
			{
				get { return this.top; }
			}

			public int Width
			{
				get { return right - left; }
			}

			public int Height
			{
				get { return bottom - top; }
			}

			public static implicit operator Rectangle(RECT rect)
			{
				return new Rectangle(rect.left, rect.top, rect.Width, rect.Height);
			}
		}
		
		[DllImport("USER32.DLL")]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
		
		[DllImport("user32")]
		public static extern int GetWindowRect(IntPtr hWnd, ref RECT rect);
		
		[DllImport("user32")]
		public static extern int PrintWindow(IntPtr hWnd, IntPtr dc, uint flags);
	}
}
