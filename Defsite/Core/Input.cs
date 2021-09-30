using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Defsite.Core;
public static class Input {
	static readonly bool[] active_buttons = new bool[(int)MouseButton.Last];
	static readonly bool[] active_keys = new bool[(int)Keys.LastKey];
	static float scroll_wheel;

	public static Vector2 MousePos { get; private set; }

	public static float ScrollWheel {
		get {
			try {
				return scroll_wheel;
			} finally {
				scroll_wheel = 0;
			}
		}
	}

	public static void KeyDown(Keys key, Action callback) {
		if(IsActive(key)) {
			callback.Invoke();
		}
	}

	public static void KeyUp(Keys key, Action callback) {
		if(!IsActive(key)) {
			callback.Invoke();
		}
	}

	public static bool IsActive(Keys key) => active_keys[(int)key];

	public static bool IsActive(MouseButton button) => active_buttons[(int)button];

	public static void Set(Keys key, bool value) => active_keys[(int)key] = value;

	public static void Set(MouseButton button, bool value) => active_buttons[(int)button] = value;

	public static void Set(Vector2 position) => MousePos = position;

	public static void Set(float value) => scroll_wheel = value;
}
