using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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
		AudioContext audio_context;
		Shader color_shader;
		VertexArray gizmo_vao;
		IndexBuffer index_buffer;
		Scene main_scene;
		Process proc;

		public static new int Height { get; private set; }

		public static new int Width { get; private set; }

		public static new int X { get; private set; }

		public static new int Y { get; private set; }

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

			audio_context = new AudioContext();
			if (audio_context == null)
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

			SoundListener.Init();
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
			gizmo_vao = new VertexArray();

			color_shader = Assets.Get<Shader>("ColorShader");
			//var gizmo = new Vertex[]
			//{
			//	new Vertex
			//	{
			//		Position = new Vector3(-.5f, 0, 0),
			//		Color = new Vector4(1, 1, 1, 1)
			//	},
			//	new Vertex
			//	{
			//		Position = new Vector3(.5f, 0, 0),
			//		Color = new Vector4(1, 0, 0, 1)
			//	},

			//	new Vertex
			//	{
			//		Position = new Vector3(0, -.5f, 0),
			//		Color = new Vector4(1, 1, 1, 1)
			//	},
			//	new Vertex
			//	{
			//		Position = new Vector3(0, .5f, 0),
			//		Color = new Vector4(0, 1, 0, 1)
			//	},

			//	new Vertex
			//	{
			//		Position = new Vector3(0, 0, -.5f),
			//		Color = new Vector4(1, 1, 1, 1)
			//	},
			//	new Vertex
			//	{
			//		Position = new Vector3(0, 0, .5f),
			//		Color = new Vector4(0, 0, 1, 1)
			//	},
			//};

			var layout = new BufferLayout(new List<VertexAttribute>{
				new VertexAttribute(0, "Position", VertexAttributeType.Vector3),
				new VertexAttribute(1, "Color", VertexAttributeType.Vector4),
				new VertexAttribute(2, "TextureCoordinates", VertexAttributeType.Vector2),
				new VertexAttribute(3, "Normal", VertexAttributeType.Vector3)
			});

			//var vbo = new VertexBuffer(gizmo) {
			//	Layout = layout
			//};

			Vertex[] quads = new Vertex[40008];

			var ix = 0;

			var q = 50;

			index_buffer = new IndexBuffer(Enumerable.Range(0, q * q * 4).ToArray());

			for (var y = 1; y < q; y++) {
				for (var x = 1; x < q; x++) {
					//var quad = Primitives.CreateQuad(new Vector3(x, y, 0), (x + y) % 2 == 0 ? Color.Red : Color.Blue);

					var quad = Primitives.CreateQuad(new Vector3(x, y, 0), Color.Red.Blerp(Color.Blue, 1f / x, 1f / y));
					Array.Copy(quad, 0, quads, ix += quad.Length, quad.Length);
				}
			}

			var qvbo = new VertexBuffer(quads) {
				Layout = layout
			};

			//gizmo_vao.AddVertexBuffer(vbo);
			gizmo_vao.AddVertexBuffer(qvbo);
		}
		protected override void OnKeyDown(KeyboardKeyEventArgs e) => Input.Set(e.Key, true);

		protected override void OnKeyUp(KeyboardKeyEventArgs e) => Input.Set(e.Key, false);

		protected override void OnMouseDown(MouseButtonEventArgs e) => Input.Set(e.Button, true);

		protected override void OnMouseMove(MouseMoveEventArgs e) => Input.Set(new Point(e.X, e.Y));

		protected override void OnMouseUp(MouseButtonEventArgs e) => Input.Set(e.Button, false);

		protected override void OnMouseWheel(MouseWheelEventArgs e) => Input.Set(e.Delta);

		protected override void OnUnload(EventArgs e) => audio_context.Suspend();

		protected override void OnClosed(EventArgs e) => audio_context.Dispose();

		protected override void OnClosing(CancelEventArgs e) => DisplayDevice.Default.RestoreResolution();

		protected override void OnFocusedChanged(EventArgs e) {
			base.OnFocusedChanged(e);

			if (Defsite.Utils.IsDebug) {
				foreach (var shader in Assets.GetAll<Shader>(AssetType.Shader))
					shader.Reload();
				Log.Info("Shaders reloaded.");
			}

			if (Focused)
				WindowState = WindowState.Normal;
		}

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

			main_scene.Update((float)e.Time);

			if (Defsite.Utils.IsDebug) {
				proc = Process.GetCurrentProcess();
				var mb_ram_used = proc.PrivateMemorySize64 / 1024 / 1024;
				if (mb_ram_used > 500) {
					throw new Exception("High RAM usage. Exiting to prevent system lag. (Known memory leak)");
				}
			}
		}

		protected override void OnRenderFrame(FrameEventArgs e) {
			base.OnRenderFrame(e);
			if (WindowState == WindowState.Minimized) return;
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			main_scene.Render((float)e.Time);

			color_shader.Enable();

			color_shader.Set("projection", main_scene.Camera.ProjectionMatrix);

			color_shader.Set("view", main_scene.Camera.GetComponent<Transform>().Matrix);

			color_shader.Set("model", new Transform().Matrix);

			gizmo_vao.Enable();
			index_buffer.Enable();

			GL.DrawElements(PrimitiveType.Quads, index_buffer.Count, DrawElementsType.UnsignedInt, 0);

			SwapBuffers();
		}

		void UpdateClientRect() {
			Width = (this as NativeWindow).Width;
			Height = (this as NativeWindow).Height;
			X = (this as NativeWindow).X;
			Y = (this as NativeWindow).Y;
		}
	}
}