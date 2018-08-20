using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Client {
	public partial class Window : GameWindow {
		public Window(int width, int height, GraphicsMode mode, string title, GameWindowFlags window_flags, DisplayDevice device, int major, int minor, GraphicsContextFlags context_flags)
			: base(width, height, mode, title, window_flags, device, major, minor, context_flags) {
		}

		public static int ClientWidth { get; private set; }
		public static int ClientHeight { get; private set; }

		Renderer renderer;
		Camera camera;
		InputManager input_manager;
		TileModel tile;
		TextModel text, fps, mouse_info;
		RectangleModel gui;

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if (float.Parse(GL.GetString(StringName.ShadingLanguageVersion)) < 1.3) {
				Close();
				Console.BackgroundColor = ConsoleColor.Red;
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine("Error: OpenGL Version");
				Console.WriteLine("> Minimum required OpenGL version: 3.0");
				Console.WriteLine($"> Detected OpenGL version: {GL.GetString(StringName.ShadingLanguageVersion)}");
				Console.ResetColor();
				Environment.Exit(1);
			}
			CursorVisible = true;

			ClientWidth = Width;
			ClientHeight = Height;

			renderer = new Renderer(new Color(20, 20, 20, 255));
			camera = new Camera();
			input_manager = new InputManager();

			tile = new TileModel();
			text = new TextModel("VoOoLoX", scale: .2f, color: Color.SteelBlue);
			fps = new TextModel("", scale: .2f, color: new Color(180, 20, 40, 255));
			mouse_info = new TextModel("", scale: .2f, color: Color.Cyan);
			gui = new RectangleModel(new Texture("Assets/Textures/gui.png"), new Rectangle(ClientWidth / 2 - 50, ClientHeight - 20 - 50, 100, 50));

		}

		protected override void OnKeyDown(KeyboardKeyEventArgs e) {
			base.OnKeyDown(e);
			input_manager.SetKey(e.Key, true);
		}

		protected override void OnKeyUp(KeyboardKeyEventArgs e) {
			base.OnKeyUp(e);
			input_manager.SetKey(e.Key, false);
		}

		protected override void OnMouseMove(MouseMoveEventArgs e) {
			base.OnMouseMove(e);
			input_manager.SetMousePos(new Vector2(e.X, e.Y));
		}

		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			ClientWidth = Width;
			ClientHeight = Height;
		}

		protected override void OnMouseWheel(MouseWheelEventArgs e) {
			base.OnMouseWheel(e);
			input_manager.SetScrollWeel(e.Delta);
		}

		protected override void OnMouseDown(MouseButtonEventArgs e) {
			base.OnMouseDown(e);
			input_manager.SetMouseButton(e.Button, true);
		}

		protected override void OnMouseUp(MouseButtonEventArgs e) {
			base.OnMouseUp(e);
			input_manager.SetMouseButton(e.Button, false);
		}

		protected override void OnUpdateFrame(FrameEventArgs e) {
			base.OnUpdateFrame(e);
			camera.Update(e.Time);
			tile.Update(e.Time);
			text.MoveText(ClientWidth - text.Width, 0);
			gui.MoveRect(new Vector2(ClientWidth / 2 - Utils.WorldUnitToScreen(1), ClientHeight - Utils.WorldUnitToScreen(1.2f)));
			mouse_info.SetText($"{InputManager.MousePos().X}:{InputManager.MousePos().Y}:{InputManager.IsButtonActive(MouseButton.Left)}:{InputManager.IsButtonActive(MouseButton.Right)}");
			mouse_info.MoveText(0, fps.Height);
		}

		protected override void OnRenderFrame(FrameEventArgs e) {
			base.OnRenderFrame(e);
			fps.SetText($"{1 / e.Time:.0}");

			renderer.Clear();

			renderer.Draw(camera, tile);
			renderer.Draw(camera, text, true);
			renderer.Draw(camera, gui, true);
			renderer.Draw(camera, fps, true);
			renderer.Draw(camera, mouse_info, true);


			SwapBuffers();
		}
	}
}
