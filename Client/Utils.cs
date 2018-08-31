using OpenTK;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Client {
	public static class Utils {

#if DEBUG
		public static bool IsDebug = true;
#else
		public static bool IsDebug = false;
#endif

		public static bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
		public static bool IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
		public static bool IsOSX = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

		public static float WorldUnitToScreen(float scale) => (scale * Camera.ZoomFactor);
		public static float TextHeight(float scale) => WorldUnitToScreen(scale);
		public static float TextWidth(string text, float scale) => text.Length * WorldUnitToScreen(scale);

		public static float Map(float value, float min, float max, float new_min, float new_max) =>
			(value - min) * (new_max - new_min) / (max - min) + new_min;

		public static Vector2 ScreenToWorld(float x, float y) =>
			new Vector2(Map(x, 0, Window.ClientWidth, -1, 1), Map(y, 0, Window.ClientHeight, -1, 1));
	}
}
