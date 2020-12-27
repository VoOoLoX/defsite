using System.Drawing;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Defsite {

	public static class Input {
		static readonly bool[] active_buttons = new bool[(int)MouseButton.Last];
		static readonly bool[] active_keys = new bool[(int)Keys.LastKey];
		static float scroll_wheel;

		public static Point MousePos { get; private set; }

		public static float ScrollWheel {
			get {
				try {
					return scroll_wheel;
				} finally {
					scroll_wheel = 0;
				}
			}
		}

		public static bool IsActive(Keys key) => active_keys[(int)key];

		public static bool IsActive(MouseButton button) => active_buttons[(int)button];

		public static void Set(Keys key, bool value) => active_keys[(int)key] = value;

		public static void Set(MouseButton button, bool value) => active_buttons[(int)button] = value;

		public static void Set(Point pos) => MousePos = pos;

		public static void Set(float value) => scroll_wheel = value;
	}
}