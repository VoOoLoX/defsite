using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client {
	public class InputManager {
		static bool[] active_keys = new bool[(int)Key.LastKey];
		static bool[] active_buttons = new bool[(int)MouseButton.LastButton];
		static Point mouse_pos = Point.Zero;
		static int scroll_weel = 0;

		public void Set(Key key, bool value) => active_keys[(int)key] = value;

		public void Set(MouseButton button, bool value) => active_buttons[(int)button] = value;

		public void Set(Point pos) => mouse_pos = pos;

		public void Set(int value) => scroll_weel = value;

		public static bool IsActive(Key key) => active_keys[(int)key];

		public static bool IsActive(MouseButton button) => active_buttons[(int)button];

		public static Point MousePos => mouse_pos;

		public static int ScrollWeel {
			get {
				try {
					return scroll_weel;
				} finally {
					scroll_weel = 0;
				}
			}
		}
	}
}
