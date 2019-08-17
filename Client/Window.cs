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
		AudioContext AudioContext;

		Scene main_scene;

		public Window(
			int width,
			int height,
			string title,
			GraphicsMode mode,
			GameWindowFlags window_flags,
			DisplayDevice device,
			int major, int minor,
			GraphicsContextFlags context_flags
		) : base(width, height, mode, title, window_flags, device, major, minor, context_flags) {
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

			if (Context == null)
				Log.Panic("Invalid graphics context");

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

			AudioContext = new AudioContext();
			if (AudioContext == null)
				Log.Panic("Invalid audio context");

			Log.Info("OpenAL Info:");
			Log.Indent();
			Log.Info(AL.Get(ALGetString.Vendor));
			Log.Info(AL.Get(ALGetString.Renderer));
			Log.Info(AL.Get(ALGetString.Version));
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

			VSync = VSyncMode.Adaptive;
			CursorVisible = true;

			UpdateClientRect();

			var window_info_handle = WindowInfo.Handle;

//			Debug.Assert(false, "Test");
			SoundListener.Init();
		}

		public new static int X { get; private set; }
		public new static int Y { get; private set; }
		public new static int Width { get; private set; }
		public new static int Height { get; private set; }

		void UpdateClientRect() {
			Width = (this as NativeWindow).Width;
			Height = (this as NativeWindow).Height;
			X = (this as NativeWindow).X;
			Y = (this as NativeWindow).Y;
		}

		protected override void OnLoad(EventArgs e) {
			Assets.LoadAssets("Assets.toml");

			main_scene = new MainScene();

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
		}

		protected override void OnKeyDown(KeyboardKeyEventArgs e) => Input.Set(e.Key, true);

		protected override void OnKeyUp(KeyboardKeyEventArgs e) => Input.Set(e.Key, false);

		protected override void OnMouseMove(MouseMoveEventArgs e) => Input.Set(new Point(e.X, e.Y));

		protected override void OnMouseWheel(MouseWheelEventArgs e) => Input.Set(e.Delta);

		protected override void OnMouseDown(MouseButtonEventArgs e) => Input.Set(e.Button, true);

		protected override void OnMouseUp(MouseButtonEventArgs e) => Input.Set(e.Button, false);

		protected override void OnClosing(CancelEventArgs e) => DisplayDevice.Default.RestoreResolution();

		protected override void OnResize(EventArgs e) {
			UpdateClientRect();
			GL.Viewport(0, 0, Width, Height);
			SwapBuffers();
		}

		protected override void OnUpdateFrame(FrameEventArgs e) {
			base.OnUpdateFrame(e);
			if (WindowState == WindowState.Minimized || !Focused) Thread.Sleep(100);

			if (Input.IsActive(Key.Escape))
				Close();

			Update((float) e.Time);
		}

		protected override void OnFocusedChanged(EventArgs e) {
			base.OnFocusedChanged(e);
			if (Focused)
				WindowState = WindowState.Normal;
		}

		void Update(float delta_time) {
//			if (Input.IsActive(MouseButton.Left) && !down) {
//				down = true;
//				mouse_old = Input.MousePos;
//			}
//
//			if (down) {
//				var dx = Input.MousePos.X - mouse_old.X;
//				var dy = Input.MousePos.Y - mouse_old.Y;
//				
//				cube.GetComponent<Transform>().RotateBy(dy * delta_time,dx * delta_time,0);
//			}
//
//			if (!Input.IsActive(MouseButton.Left)) down = false;
//
//			light_cube.GetComponent<Transform>().Matrix *= Matrix4.CreateRotationY((MathF.PI / 4.0f) * delta_time);
//			cube.GetComponent<Mesh>().Shader.Set("light_position", light_cube.GetComponent<Transform>().Position);

			main_scene.Update(delta_time);
		}

		protected override void OnRenderFrame(FrameEventArgs e) {
			base.OnRenderFrame(e);
			if (WindowState == WindowState.Minimized) return;

			main_scene.Render((float) e.Time);

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