using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client {
	public class InputManager {
		static bool[] active_keys = new bool[(int)Key.LastKey];
		static bool[] active_buttons = new bool[(int)MouseButton.LastButton];
		static Vector2 mouse_pos = Vector2.Zero;
		static int scroll_weel = 0;

		public void SetKey(Key key, bool value) => active_keys[(int)key] = value;
		public void SetMouseButton(MouseButton button, bool value) => active_buttons[(int)button] = value;
		public void SetMousePos(Vector2 pos) => mouse_pos = pos;
		public void SetScrollWeel(int value) => scroll_weel = value;
		public static bool IsKeyActive(Key key) => active_keys[(int)key];
		public static bool IsButtonActive(MouseButton button) => active_buttons[(int)button];
		public static Vector2 MousePos() => mouse_pos;
		public static int ScrollWeel() {
			try {
				return scroll_weel;
			} finally {
				scroll_weel = 0;
			}
		}
	}
}
