using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Defsite.Core;

public class Input {
	public static MouseState MouseState { get; protected set; }
	public static KeyboardState KeyboardState { get; protected set; }
	public static IReadOnlyList<JoystickState> JoystickStates { get; protected set; }

	public static Vector2 MousePosition => MouseState.Position;
	public static Vector2 MouseScroll => MouseState.Scroll;
	public static Vector2 MouseScrollDelta => MouseState.ScrollDelta;

	public static void OnKeyPress(Keys key, Action callback) {
		if(KeyboardState.IsKeyPressed(key)) {
			callback.Invoke();
		}
	}

	public static void OnKeyRelease(Keys key, Action callback) {
		if(KeyboardState.IsKeyReleased(key)) {
			callback.Invoke();
		}
	}

	public static bool KeyDown(Keys key) => KeyboardState.IsKeyDown(key);

	public static void OnButtonPress(MouseButton button, Action callback) {
		if(MouseState.WasButtonDown(button) == false && MouseState.IsButtonDown(button)) {
			callback.Invoke();
		}
	}

	public static void OnButtonRelease(MouseButton button, Action callback) {
		if(MouseState.WasButtonDown(button) && MouseState.IsButtonDown(button) == false) {
			callback.Invoke();
		}
	}

	public static bool ButtonDown(MouseButton button) => MouseState.IsButtonDown(button);
}

public sealed class InputController : Input {
	static readonly Lazy<InputController> lazy_instance = new(() => new InputController());

	public static InputController Instance => lazy_instance.Value;

	InputController() { }

	public void SetState(MouseState state) => MouseState = state;
	public void SetState(KeyboardState state) => KeyboardState = state;
	public void SetState(IReadOnlyList<JoystickState> states) => JoystickStates = states;
}