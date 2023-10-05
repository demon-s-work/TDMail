using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace TDMail.WinApiImpl
{
	public static partial class WinApi
	{
		[LibraryImport("user32.dll")]
		private static partial uint SendInput(uint nInputs, Input[] pInputs, int cbSize);

		public static void SendCtrlV()
		{
			ForegroundKeyDown(VKeys.KeyControl);
			Thread.Sleep(50);
			ForegroundKeyDown(VKeys.KeyV);
			Thread.Sleep(50);
			ForegroundKeyUp(VKeys.KeyV);
			Thread.Sleep(50);
			ForegroundKeyUp(VKeys.KeyControl);
		}

		private static void ForegroundKeyDown(VKeys key)
		{
			var structInput = new Input
			{
				type = 1
			};

			structInput.u.ki.wScan = 0;
			structInput.u.ki.time = 0;
			structInput.u.ki.dwFlags = 0;
			structInput.u.ki.wVk = (ushort)key;

			_ = SendInput(1, new[] { structInput }, Marshal.SizeOf(new Input()));
		}

		private static void ForegroundKeyUp(VKeys key)
		{
			var structInput = new Input
			{
				type = 1
			};

			structInput.u.ki.wScan = 0;
			structInput.u.ki.time = 0;
			structInput.u.ki.dwFlags = 0;
			structInput.u.ki.wVk = (ushort)key;

			structInput.u.ki.dwFlags = 0x0002;
			_ = SendInput(1, new[] { structInput }, Marshal.SizeOf(new Input()));
		}

		private struct Input
		{
			public int type;
			public InputUnion u;
		};

		[StructLayout(LayoutKind.Explicit)]
		private struct InputUnion
		{
			[FieldOffset(0)]
			public MouseInput mi;
			[FieldOffset(0)]
			public KeybdInput ki;
			[FieldOffset(0)]
			public HardwareInput hi;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct MouseInput
		{
			public int dx;
			public int dy;
			public uint mouseData;
			public uint dwFlags;
			public uint time;
			public IntPtr dwExtraInfo;
		};

		[StructLayout(LayoutKind.Sequential)]
		private struct KeybdInput
		{
			public ushort wVk;
			public ushort wScan;
			public uint dwFlags;
			public uint time;
			public IntPtr dwExtraInfo;
		};

		[StructLayout(LayoutKind.Sequential)]
		private struct HardwareInput
		{
			public uint uMsg;
			public ushort wParamL;
			public ushort wParamH;
		};
	}
}