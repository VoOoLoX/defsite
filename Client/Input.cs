using OpenTK;
using OpenTK.Input;

namespace Client {

	public static class Input {
		static bool[] active_buttons = new bool[(int)MouseButton.LastButton];
		static bool[] active_keys = new bool[(int)Key.LastKey];
		static int scroll_wheel;

		public static Point MousePos { get; private set; }

		public static int ScrollWheel {
			get {
				try {
					return scroll_wheel;
				} finally {
					scroll_wheel = 0;
				}
			}
		}

		public static bool IsActive(Key key) => active_keys[(int)key];

		public static bool IsActive(MouseButton button) => active_buttons[(int)button];

		public static void Set(Key key, bool value) => active_keys[(int)key] = value;

		public static void Set(MouseButton button, bool value) => active_buttons[(int)button] = value;

		public static void Set(Point pos) => MousePos = pos;

		public static void Set(int value) => scroll_wheel = value;
	}
}