using OpenTK;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Client {
	public partial class Window : GameWindow {
		public Window(int width, int height, GraphicsMode mode, string title, GameWindowFlags window_flags, DisplayDevice device, int major, int minor, GraphicsContextFlags context_flags)
			: base(width, height, mode, title, window_flags, device, major, minor, context_flags) {
		}

		public static int ClientWidth { get; private set; }
		public static int ClientHeight { get; private set; }
		public static Point ClientCenter { get; private set; }

		Renderer renderer;
		Camera camera;
		InputManager input_manager;
		TileModel tile;
		TextModel text, fps, mouse_info;

		Button button;

		protected override void OnLoad(EventArgs e) {
			Context.ErrorChecking = true;

			if (double.Parse(GL.GetString(StringName.ShadingLanguageVersion)) < 1.30) {
				Close();
				Console.BackgroundColor = ConsoleColor.Red;
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine("Error: OpenGL Version");
				Console.WriteLine("> Minimum required OpenGL version: 1.30");
				Console.WriteLine($"> Detected OpenGL version: {GL.GetString(StringName.ShadingLanguageVersion)}");
				Console.ResetColor();
				Environment.Exit(1);
			}

			if (!Context.IsCurrent) {
				Console.WriteLine("Invalid context");
				Environment.Exit(1);
			}

			AssetManager.Load(AssetType.Texture, "Pot", "Pot.vif");
			AssetManager.Load(AssetType.Font, "TinyFont", "Tiny.vif");

			AssetManager.Load(AssetType.Shader, "ObjectShader", "Object.shader");
			AssetManager.Load(AssetType.Shader, "TextureShader", "Texture.shader");
			AssetManager.Load(AssetType.Shader, "TextShader", "Text.shader");
			AssetManager.Load(AssetType.Shader, "ColorShader", "Color.shader");

			VSync = VSyncMode.Off;
			CursorVisible = true;

			ClientWidth = Width;
			ClientHeight = Height;
			ClientCenter = new Point(ClientWidth / 2, ClientHeight / 2);

			renderer = new Renderer(new Color(20, 20, 20, 255));
			camera = new Camera(60);
			input_manager = new InputManager();

			tile = new TileModel();

			text = new TextModel("VoOoLoX", scale: .2f, color: Color.SteelBlue);
			fps = new TextModel("", scale: .2f, color: Color.DarkViolet);
			mouse_info = new TextModel("", scale: .2f, color: Color.Cyan);

			button = new Button("Play", new Rectangle(ClientCenter.X - 40, ClientHeight - 60 - 40, 80, 40), Color.Tomato, Color.Wheat);
		}

		#region Inputs
		protected override void OnKeyDown(KeyboardKeyEventArgs e) => input_manager.Set(e.Key, true);

		protected override void OnKeyUp(KeyboardKeyEventArgs e) => input_manager.Set(e.Key, false);

		protected override void OnMouseMove(MouseMoveEventArgs e) => input_manager.Set(new Point(e.X, e.Y));

		protected override void OnMouseWheel(MouseWheelEventArgs e) => input_manager.Set(e.Delta);

		protected override void OnMouseDown(MouseButtonEventArgs e) => input_manager.Set(e.Button, true);

		protected override void OnMouseUp(MouseButtonEventArgs e) => input_manager.Set(e.Button, false);
		#endregion

		protected override void OnResize(EventArgs e) {
			ClientWidth = Width;
			ClientHeight = Height;
			ClientCenter = new Point(ClientWidth / 2, ClientHeight / 2);
			camera.UpdateProjectionMatrix();
			SwapBuffers();
		}

		protected override void OnUpdateFrame(FrameEventArgs e) {
			ProcessEvents();
			camera.Update(e.Time);
			tile.Update(e.Time);

			text.MoveText(ClientWidth - text.Width, 0);

			button.Move(ClientCenter.X - button.Width / 2, ClientHeight - (int)(button.Height * 1.2));

			mouse_info.Text = $"{InputManager.MousePos.X}:{InputManager.MousePos.Y}:{InputManager.IsActive(MouseButton.Left)}:{InputManager.IsActive(MouseButton.Right)}";
			mouse_info.MoveText(0, fps.Height);

			var mouse_p = InputManager.MousePos;
			button.Color = Color.Goldenrod;
			if ((mouse_p.X > button.X && mouse_p.X < button.X + button.Width) &&
				(mouse_p.Y > button.Y && mouse_p.Y < button.Y + button.Height) &&
				InputManager.IsActive(MouseButton.Left)) {
				button.Color = Color.DarkGoldenrod;
			}
		}

		protected override void OnRenderFrame(FrameEventArgs e) {
			if (WindowState != WindowState.Minimized) {
				fps.Text = $"{1 / e.Time:.0}";
				renderer.Clear();

				renderer.Draw(camera, tile);
				renderer.Draw(camera, text, true);
				renderer.Draw(camera, fps, true);
				renderer.Draw(camera, mouse_info, true);
				button.Draw(renderer, camera);

				SwapBuffers();
			}
		}
	}
}
