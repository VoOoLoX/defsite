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
		Panel panel, panel2;
		bool text_glow = false;
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
			//Add credits for both fonts & fix scaling of scientifica
			AssetManager.Load(AssetType.Font, "ScientificaFont", "Scientifica.vif");

			AssetManager.Load(AssetType.Shader, "ObjectShader", "Object.shdr");
			AssetManager.Load(AssetType.Shader, "TextureShader", "Texture.shdr");
			AssetManager.Load(AssetType.Shader, "TextShader", "Text.shdr");
			AssetManager.Load(AssetType.Shader, "ColorShader", "Color.shdr");

			VSync = VSyncMode.Off;
			CursorVisible = true;

			ClientWidth = Width;
			ClientHeight = Height;
			ClientCenter = new Point(ClientWidth / 2, ClientHeight / 2);

			renderer = new Renderer(new Color(20, 20, 20, 255));
			camera = new Camera(60);
			input_manager = new InputManager();

			tile = new TileModel();

			text = new TextModel("VoOoLoX", scale: .16f, color: Color.SteelBlue);

			fps = new TextModel("", scale: .2f, color: Color.DarkViolet);
			mouse_info = new TextModel("", scale: .2f, color: Color.Cyan);

			panel = new Panel(new Rectangle(100, 100, 200, 200), Color.SeaGreen);
			panel2 = new Panel(new Rectangle(100, 100, 200, 20), Color.Gray);
			button = new Button("X", new Rectangle(panel.X + panel.Width - 40, panel.Y, 40, 19), Color.Tomato, Color.Wheat);

			button.OnClick += (b) => {
				b.Color = Color.Tomato;
			};

			button.OnHover += (b) => {
				b.Color = Color.IndianRed;
			};

			button.OnUpdate += (b) => {
				button.Color = Color.Goldenrod;
			};

			panel.OnLeftClick += (p) => {
				text_glow = !text_glow;
				mouse_info.Glow = text_glow;
			};

			panel2.OnDrag += (p, delta) => {
				p.X += delta.X;
				p.Y += delta.Y;
				panel.X = p.X;
				panel.Y = p.Y;
				button.X = panel.X + panel.Width - button.Width;
				button.Y = panel.Y;
			};

		}

		#region Inputs
		protected override void OnKeyDown(KeyboardKeyEventArgs e) => input_manager.Set(e.Key, true);

		protected override void OnKeyUp(KeyboardKeyEventArgs e) => input_manager.Set(e.Key, false);

		protected override void OnMouseMove(MouseMoveEventArgs e) => input_manager.Set(new Point(e.X, e.Y));

		protected override void OnMouseWheel(MouseWheelEventArgs e) => input_manager.Set(e.Delta);

		protected override void OnMouseDown(MouseButtonEventArgs e) => input_manager.Set(e.Button, true);

		protected override void OnMouseUp(MouseButtonEventArgs e) => input_manager.Set(e.Button, false);
		#endregion

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
			DisplayDevice.Default.RestoreResolution();
		}

		protected override void OnResize(EventArgs e) {
			ClientWidth = Width;
			ClientHeight = Height;
			ClientCenter = new Point(ClientWidth / 2, ClientHeight / 2);
			camera.UpdateProjectionMatrix();
			SwapBuffers();
		}

		protected override void OnUpdateFrame(FrameEventArgs e) {
			ProcessEvents();

			if (InputManager.IsActive(Key.Escape))
				Close();

			camera.Update(e.Time);
			tile.Update(e.Time);

			if (WindowState != WindowState.Minimized)
				Update();
		}

		void Update() {
			text.ScaleText(.2f);
			text.MoveText(ClientWidth - text.Width, 0);

			panel.Move(panel.X, panel.Y);
			panel2.Move(panel2.X, panel2.Y);
			button.X = panel.X + panel.Width - button.Width;
			button.Y = panel.Y;

			mouse_info.Text = $"{InputManager.MousePos.X}:{InputManager.MousePos.Y}:{InputManager.IsActive(MouseButton.Left)}:{InputManager.IsActive(MouseButton.Right)}";
			mouse_info.MoveText(0, fps.Height);

			button.Update();
			panel.Update();
			panel2.Update();

		}

		protected override void OnRenderFrame(FrameEventArgs e) {
			if (WindowState != WindowState.Minimized) {
				fps.Text = $"{1 / e.Time:.0}";
				renderer.Clear();

				renderer.Draw(camera, tile);
				renderer.Draw(camera, text, true);
				renderer.Draw(camera, fps, true);
				renderer.Draw(camera, mouse_info, true);
				panel.Draw(renderer, camera);
				panel2.Draw(renderer, camera);
				button.Draw(renderer, camera);

				SwapBuffers();
			}
		}
	}
}
