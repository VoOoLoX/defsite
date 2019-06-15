using System;
using System.ComponentModel;
using System.Threading;
using Defsite;
using OpenTK;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Client {
	public class Window : GameWindow {
		readonly AudioContext AudioContext;

		Button button;
		Camera camera;
		Panel panel, panel2;

		Renderer renderer;
		SoundBuffer sb;

		WAVFile sound;
		SoundSource ss;

		CubeModel test;
		TextModel text, fps, mouse_info;
		bool text_glow;
		TileModel tile;

		public Window(int width, int height, GraphicsMode mode, string title, GameWindowFlags window_flags, DisplayDevice device, int major, int minor, GraphicsContextFlags context_flags)
			: base(width, height, mode, title, window_flags, device, major, minor, context_flags) {
			Context.ErrorChecking = true;

			Log.Info("OpenGL Info:");
			Log.Indent();
			Log.Info(GL.GetString(StringName.Vendor));
			Log.Info(GL.GetString(StringName.Renderer));
			Log.Info(GL.GetString(StringName.Version));
			Log.Info(GL.GetString(StringName.ShadingLanguageVersion));
			Log.Unindent();

			Log.Info("Display Info:");
			Log.Indent();
			Log.Info($"Display: {device.Bounds}");
			Log.Info($"Refresh rate: {device.RefreshRate}");
			Log.Info($"IsPrimary: {device.IsPrimary}");
			Log.Unindent();

			if (double.Parse(GL.GetString(StringName.ShadingLanguageVersion)) < 1.30) {
				Close();
				Log.Error("Minimum required GLSL version: 1.30");
				Log.Indent();
				Log.Error($"Detected OpenGL version: {GL.GetString(StringName.Version)}");
				Log.Error($"Detected GLSL version: {GL.GetString(StringName.ShadingLanguageVersion)}");
				Log.Unindent();
				Log.Panic("Exiting");
			}

			if (Context == null)
				Log.Panic("Invalid context");

			VSync = VSyncMode.Off;
			CursorVisible = true;

			ClientWidth = Width;
			ClientHeight = Height;
			ClientCenter = new Point(ClientWidth / 2, ClientHeight / 2);

			AudioContext = new AudioContext();
		}

		public static int ClientWidth { get; private set; }
		public static int ClientHeight { get; private set; }
		public static Point ClientCenter { get; private set; }

		protected override void OnLoad(EventArgs e) {
			AssetManager.Load(AssetType.Texture, "Ghost", "Ghost.vif");

			//Add credits for both fonts & fix scaling of scientifica
			AssetManager.Load(AssetType.Font, "TinyFont", "Tiny.vif");
			AssetManager.Load(AssetType.Font, "ScientificaFont", "Scientifica.vif");

			AssetManager.Load(AssetType.Shader, "ObjectShader", "Object.shdr");
			AssetManager.Load(AssetType.Shader, "TextureShader", "Texture.shdr");
			AssetManager.Load(AssetType.Shader, "TextShader", "Text.shdr");
			AssetManager.Load(AssetType.Shader, "ColorShader", "Color.shdr");

			AssetManager.Load(AssetType.Sound, "Chching", "Coins.wav");

			renderer = new Renderer(new Color(20, 20, 20, 255));
			camera = new Camera(60);
			camera.UpdateProjectionMatrix();

			tile = new TileModel();

			test = new CubeModel(Color.Red);

			var f = new FileLoader();

			var settings = new Config("Assets/Settings.cfg");

			AL.Listener(ALListener3f.Position, 0, 0, 0);
			AL.Listener(ALListener3f.Velocity, 0, 0, 0);

			sound = AssetManager.Get<WAVFile>("Chching");
			sb = new SoundBuffer(sound.Format, sound.Data, sound.SampleRate);
			ss = new SoundSource();

			text = new TextModel(settings["client"].GetString("player_name"), scale: .16f, color: Color.SteelBlue);

			fps = new TextModel("", scale: .2f, color: Color.DarkViolet);
			mouse_info = new TextModel("", scale: .2f, color: Color.Cyan);

			panel = new Panel(new Rectangle(100, 100, 200, 200), Color.SeaGreen);
			panel2 = new Panel(new Rectangle(100, 100, 200, 20), Color.Gray);
			button = new Button("X", new Rectangle(panel.X + panel.Width - 40, panel.Y, 40, 19), Color.Tomato, Color.Wheat);

			button.OnClick += b => { b.Color = Color.Tomato; };

			button.OnHover += b => { b.Color = Color.IndianRed; };

			button.OnUpdate += b => { button.Color = Color.Goldenrod; };

			panel.OnLeftClick += p => {
				text_glow = !text_glow;
				mouse_info.Glow = text_glow;
				ss.Play(sb);
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

		protected override void OnKeyDown(KeyboardKeyEventArgs e) {
			Input.Set(e.Key, true);
		}

		protected override void OnKeyUp(KeyboardKeyEventArgs e) {
			Input.Set(e.Key, false);
		}

		protected override void OnMouseMove(MouseMoveEventArgs e) {
			Input.Set(new Point(e.X, e.Y));
		}

		protected override void OnMouseWheel(MouseWheelEventArgs e) {
			Input.Set(e.Delta);
		}

		protected override void OnMouseDown(MouseButtonEventArgs e) {
			Input.Set(e.Button, true);
		}

		protected override void OnMouseUp(MouseButtonEventArgs e) {
			Input.Set(e.Button, false);
		}

		protected override void OnClosing(CancelEventArgs e) {
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
			if (WindowState == WindowState.Minimized || !Focused) Thread.Sleep(200);

			if (Input.IsActive(Key.Escape))
				Close();

			camera.Update(e.Time);
			tile.Update(e.Time);
			test.Update(e.Time);

			Update();
		}

		void Update() {
			text.ScaleText(.2f);
			text.MoveText(ClientWidth - text.Width, 0);

			panel.X = panel.X;
			panel.Y = panel.Y;

			panel2.X = panel2.X;
			panel2.Y = panel2.Y;
			button.X = panel.X + panel.Width - button.Width;
			button.Y = panel.Y;

			// mouse_info.Text = $"{Input.MousePos.X}:{Input.MousePos.Y}:{Input.IsActive(MouseButton.Left)}:{Input.IsActive(MouseButton.Right)}";
			mouse_info.MoveText(0, fps.Height);

			button.Update();
			panel.Update();
			panel2.Update();
		}

		protected override void OnRenderFrame(FrameEventArgs e) {
			if (WindowState == WindowState.Minimized || !Focused) return;

			// fps.Text = $"{1 / e.Time:.0}";
			renderer.Clear();

			renderer.Draw(camera, tile);
			renderer.Draw(camera, test);
			renderer.Draw(camera, text, true);
			renderer.Draw(camera, fps, true);
			renderer.Draw(camera, mouse_info, true);
			panel.Draw(renderer, camera);
			panel2.Draw(renderer, camera);
			button.Draw(renderer, camera);

			SwapBuffers();
		}

		protected override void OnUnload(EventArgs e) {
			AudioContext.Suspend();
		}

		protected override void OnClosed(EventArgs e) {
			AudioContext.Dispose();
		}
	}
}