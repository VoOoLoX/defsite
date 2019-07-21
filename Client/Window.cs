using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using Defsite;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Client {
	public class Window : GameWindow {
//		Config settings = new Config("Assets/Settings.cfg");

		Camera camera;

		Entity cube = new Entity();
		List<Entity> entities = new List<Entity>();

		Entity player = new Entity();
		Entity player2 = new Entity();
		RenderSystem render_system;

		Text t;
//		 AudioContext AudioContext;
//
//		Button button;
//		Panel panel, panel2;
//
//		SoundBuffer sb;
//
//		WAVFile sound;
//		SoundSource ss;
//
//		CubeModel test;
//		TextModel fps, mouse_info;
//		bool text_glow;
//		TileModel tile;
//		Panel rtest;

		public Window(int width, int height, GraphicsMode mode, string title, GameWindowFlags window_flags, DisplayDevice device, int major, int minor, GraphicsContextFlags context_flags)
			: base(width, height, mode, title, window_flags, device, major, minor, context_flags) {
			Context.ErrorChecking = true;

			Log.Info("Info:");
			Log.Indent();
			Log.Info($"OS: {Environment.OSVersion}");
			Log.Info($"Framework: {Environment.Version}");
			Log.Info($"CWD: {Environment.CurrentDirectory}");
			Log.Info($"Processor cores: {Environment.ProcessorCount}");
			Log.Info($"64 bit processor: {Environment.Is64BitProcess}");
			Log.Info($"64 bit OS: {Environment.Is64BitOperatingSystem}");
			Log.Unindent();

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

			VSync = VSyncMode.Adaptive;
			CursorVisible = true;

			ClientWidth = Width;
			ClientHeight = Height;
			ClientCenter = new Point(ClientWidth / 2, ClientHeight / 2);

//			AudioContext = new AudioContext();
		}

//		public Window(DisplayDevice device, string title, int width, int height, GameWindowFlags window_flags)

		public static int ClientWidth { get; private set; }
		public static int ClientHeight { get; private set; }
		public static Point ClientCenter { get; private set; }

		protected override void OnLoad(EventArgs e) {
			AssetManager.Load(AssetType.Texture, "Ghost", "Ghost.vif");

			//Add credits for both fonts & fix scaling of scientifica
//			AssetManager.Load(AssetType.Font, "ScientificaFont", "Scientifica.vif");

//			AssetManager.Load(AssetType.Font, "TinyFont", "Tiny.font");
//			AssetManager.Load(AssetType.Font, "RotorcapFont", "Rotorcap.font");
			AssetManager.Load(AssetType.Font, "MinimalFont", "Minimal.font");

			AssetManager.Load(AssetType.Shader, "SpriteShader", "Sprite.glsl");
			AssetManager.Load(AssetType.Shader, "MeshShader", "Mesh.glsl");
//			AssetManager.Load(AssetType.Shader, "TextureShader", "Texture.glsl");
//			AssetManager.Load(AssetType.Shader, "TextShader", "Text.glsl");
			AssetManager.Load(AssetType.Shader, "ColorShader", "Color.glsl");

			AssetManager.Load(AssetType.Sound, "Chching", "Coins.wav");

//			var asd = new Texture("Assets/Fonts/Rotorcap.vif");

//			CameraOld.Init();
			camera = new Camera(new Vector3(0, 0, 5), Vector3.Zero);
			render_system = new RenderSystem(camera, Color.White);

//			CameraOld.UpdateProjectionMatrix();

//			tile = new TileModel();
//
//			test = new CubeModel(Color.Red);
//
//			var f = new FileLoader();
//
//			
//			AL.Listener(ALListener3f.Position, 0, 0, 0);
//			AL.Listener(ALListener3f.Velocity, 0, 0, 0);
//
//			sound = AssetManager.Get<WAVFile>("Chching");
//			sb = new SoundBuffer(sound.Format, sound.Data, sound.SampleRate);
//			ss = new SoundSource();
//
//			text = new TextModel(settings["client"].GetString("player_name"), scale: .16f, color: Color.Black);
//
//			fps = new TextModel("", scale: .2f, color: Color.Black);
//			mouse_info = new TextModel("", scale: .2f, color: Color.Black);
//
//			panel = new Panel(new Rectangle(100, 100, 200, 200), new Color(10,10,10,150));
//			panel2 = new Panel(new Rectangle(100, 100, 200, 20), new Color(0,0,0,150));
//			button = new Button("X", new Rectangle(panel.X + panel.Width - 40, panel.Y, 40, 19), Color.Goldenrod, Color.Wheat);
//			
//			rtest = new Panel(new Rectangle(100, 100, 100, 100), Color.Aqua);

//			button.OnClick += b => {
//				b.Color = Color.Tomato;
//				ss.Play(sb);
//				rtest.RotateRect((float)-Math.PI / 256,0, 0);
//			};

//			button.OnHover += b => { 
//				b.Color = Color.IndianRed;
//				rtest.RotateRect(0,(float)Math.PI / 512, 0);
//			};

//			button.OnUpdate += b => { button.Color = Color.Goldenrod;  };
//
//			panel.OnLeftClick += p => {
//				text_glow = !text_glow;
//				mouse_info.Glow = text_glow;
//				
//			};
//
//			panel2.OnDrag += (p, delta) => {
//				p.X += delta.X;
//				p.Y += delta.Y;
//				panel.X = p.X;
//				panel.Y = p.Y;
//				button.X = panel.X + panel.Width - button.Width;
//				button.Y = panel.Y;
//			};


//			Log.Info(testvar);

			player.AddComponent(new Transform());
			player.AddComponent(new Sprite(AssetManager.Get<Texture>("Ghost")));
			player.GetComponent<Sprite>().GlowColor = Color.RoyalBlue;
//			
//			player2.AddComponent(new Transform());
//			player2.AddComponent(new Sprite(AssetManager.Get<Texture>("Ghost")));
//			player2.GetComponent<Sprite>().GlowColor = Color.Red;
//			player2.GetComponent<Transform>().Position = new Vector3(1,0,0);
//			
//			player2.GetComponent<Transform>().Scale = new Vector3(-0.5f);

//			cube.AddComponent(new Transform());
//			cube.AddComponent(new Mesh(
//				AssetManager.Get<Texture>("Ghost"),
//				Primitives.CubeCentered.PositionData,
//				Primitives.CubeCentered.UVData,
//				Primitives.CubeCentered.IndexBufferData)
//			);
//			cube.GetComponent<Mesh>().Color = Color.Red;

			entities.Add(player);
//			entities.Add(player2);

//			entities.Add(cube);

			t = new Text("FPS: 60");
			t.GetComponent<Sprite>().Color = Color.Red;

			entities.Add(t);
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
			GL.Viewport(0, 0, ClientWidth, ClientHeight);
			SwapBuffers();
		}

		protected override void OnUpdateFrame(FrameEventArgs e) {
			base.OnUpdateFrame(e);
			if (WindowState == WindowState.Minimized || !Focused) Thread.Sleep(100);

			if (Input.IsActive(Key.Escape))
				Close();

//			CameraOld.Update(e.Time);
//			tile.Update(e.Time);
//			test.Update(e.Time);
//			player.GetComponent<Transform>().GetMatrix *= Matrix4.CreateRotationY((float)e.Time);
//			player2.GetComponent<Transform>().GetMatrix *= Matrix4.CreateRotationY(-(float)e.Time);
			Update(e.Time);
		}

		void Update(double delta_time) {
//			text.ScaleText(.2f);
//			text.MoveText(ClientWidth - text.Width, 0);
//
//			rtest.X = rtest.X;
//			rtest.Y = rtest.Y;
//
//			panel.X = panel.X;
//			panel.Y = panel.Y;
//
//			panel2.X = panel2.X;
//			panel2.Y = panel2.Y;
//			button.X = panel.X + panel.Width - button.Width;
//			button.Y = panel.Y;
//
//			mouse_info.Text = $"{Input.MousePos.X}:{Input.MousePos.Y}:{Input.IsActive(MouseButton.Left)}:{Input.IsActive(MouseButton.Right)}";
//			mouse_info.MoveText(0, fps.Height);
//
//			button.Update();
//			panel.Update();
//			panel2.Update();

//			text.Text = settings["client"].GetString("player_name");


			camera.UpdatePerspectiveProjection();

			var direction_vector = Vector3.Zero;
			if (Input.IsActive(Key.Right))
				direction_vector.X = 1;

			if (Input.IsActive(Key.Left))
				direction_vector.X = -1;

			if (Input.IsActive(Key.Up))
				direction_vector.Y = 1;

			if (Input.IsActive(Key.Down))
				direction_vector.Y = -1;

			if (Input.IsActive(Key.E))
				direction_vector.Z = 1;

			if (Input.IsActive(Key.Q))
				direction_vector.Z = -1;

			direction_vector.X = Joystick.GetState(0).IsConnected ? -Joystick.GetState(0).GetAxis(JoystickAxis.Axis0) : direction_vector.X;
			direction_vector.Y = Joystick.GetState(0).IsConnected ? Joystick.GetState(0).GetAxis(JoystickAxis.Axis1) : direction_vector.Y;

			if (direction_vector != Vector3.Zero) {
				direction_vector.NormalizeFast();
				camera.Transform.MoveBy(direction_vector * 10 * (float) delta_time);
			}
		}

		protected override void OnRenderFrame(FrameEventArgs e) {
			base.OnRenderFrame(e);
			if (WindowState == WindowState.Minimized) return;
//			fps.Text = $"{1 / e.Time:00}";
			render_system.Clear();

//			rtest.Draw(Camera);
//			Renderer.Draw(tile);
//			Renderer.Draw(test);
//			Renderer.Draw(text, true);
//			Renderer.Draw(fps, true);
//			Renderer.Draw(mouse_info, true);
//			panel.Draw();
//			panel2.Draw();
//			button.Draw();
			render_system.Render(entities);

			SwapBuffers();
		}

		protected override void OnUnload(EventArgs e) {
//			AudioContext.Suspend();
		}

		protected override void OnClosed(EventArgs e) {
//			AudioContext.Dispose();
		}
	}
}