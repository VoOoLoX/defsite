using OpenTK;
using OpenTK.Input;

namespace Client {
	public static class Input {
		static readonly bool[] active_keys = new bool[(int) Key.LastKey];
		static readonly bool[] active_buttons = new bool[(int) MouseButton.LastButton];
		static int scroll_wheel;

		public static Point MousePos { get; set; } = Point.Zero;

		public static int ScrollWheel {
			get {
				try {
					return scroll_wheel;
				}
				finally {
					scroll_wheel = 0;
				}
			}
		}

		public static void Set(Key key, bool value) {
			active_keys[(int) key] = value;
		}

		public static void Set(MouseButton button, bool value) {
			active_buttons[(int) button] = value;
		}

		public static void Set(Point pos) {
			MousePos = pos;
		}

		public static void Set(int value) {
			scroll_wheel = value;
		}

		public static bool IsActive(Key key) {
			return active_keys[(int) key];
		}

		public static bool IsActive(MouseButton button) {
			return active_buttons[(int) button];
		}
	}
}