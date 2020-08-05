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

		FirstPersonCamera camera;

		Shader color_shader;
		BufferLayout color_shader_layout;

		VertexArray gizmo_vao, quads_vao, camera_vao;
		IndexBuffer gizmo_index_buffer, quads_index_buffer, camera_index_buffer;

		VertexBuffer camera_vbo;

		bool move = true;
		int old_x, old_y;
		float delta_x, delta_y;
		float zoom = 1;


		Process proc;

		public static new double ElapsedTime { get; private set; }

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
			CursorVisible = false;

			UpdateClientRect();

			SoundListener.Init();
		}



		protected override void OnLoad(EventArgs e) {
			Assets.LoadAssets("Assets.toml");

			camera = new FirstPersonCamera(new Vector3(0, 0, 3));

			color_shader = Assets.Get<Shader>("ColorShader");
			color_shader_layout = new BufferLayout(new List<VertexAttribute>{
				new VertexAttribute(color_shader.GetAttributeLocation("v_position"), VertexAttributeType.Vector3),
				new VertexAttribute(color_shader.GetAttributeLocation("v_color"), VertexAttributeType.Vector4),
				new VertexAttribute(color_shader.GetAttributeLocation("v_texture_coordinates"), VertexAttributeType.Vector2),
				new VertexAttribute(color_shader.GetAttributeLocation("v_normal"), VertexAttributeType.Vector3)
			});

			#region Gizmo
			gizmo_vao = new VertexArray();
			var gizmo_size = 10f;
			var gizmo_vertices = new Vertex[]
			{
				new Vertex
				{
					Position = new Vector3(-gizmo_size, 0, 0),
					Color = new Vector4(1, 1, 1, 1)
				},
				new Vertex
				{
					Position = new Vector3(gizmo_size, 0, 0),
					Color = new Vector4(1, 0, 0, 1)
				},

				new Vertex
				{
					Position = new Vector3(0, -gizmo_size, 0),
					Color = new Vector4(1, 1, 1, 1)
				},
				new Vertex
				{
					Position = new Vector3(0, gizmo_size, 0),
					Color = new Vector4(0, 1, 0, 1)
				},

				new Vertex
				{
					Position = new Vector3(0, 0, -gizmo_size),
					Color = new Vector4(1, 1, 1, 1)
				},
				new Vertex
				{
					Position = new Vector3(0, 0, gizmo_size),
					Color = new Vector4(0, 0, 1, 1)
				},
			};

			gizmo_index_buffer = new IndexBuffer(Enumerable.Range(0, 6).ToArray());

			var gizmo_vbo = new VertexBuffer(gizmo_vertices) {
				Layout = color_shader_layout
			};

			gizmo_vao.AddVertexBuffer(gizmo_vbo);
			#endregion


			#region Quads
			quads_vao = new VertexArray();

			var ix = 0;
			var quads_count = 100;
			var max_verticies = quads_count * quads_count * 4;

			Vertex[] quads_vertices = new Vertex[max_verticies];

			quads_index_buffer = new IndexBuffer(Enumerable.Range(0, max_verticies).ToArray());

			for (var y = -quads_count / 2; y < quads_count / 2; y++) {
				for (var x = -quads_count / 2; x < quads_count / 2; x++) {
					var quad = Primitives.CreateQuad(new Vector3(x, y, 0), (x + y) % 2 == 0 ? Color.Maroon : Color.Goldenrod);
					Array.Copy(quad, 0, quads_vertices, ix, quad.Length);
					ix += quad.Length;
				}
			}

			var quads_vbo = new VertexBuffer(quads_vertices) {
				Layout = color_shader_layout
			};

			quads_vao.AddVertexBuffer(quads_vbo);
			#endregion


			camera_vao = new VertexArray();

			camera_index_buffer = new IndexBuffer(Enumerable.Range(0, 2).ToArray());

			var camera_vertices = new Vertex[]
			{
				new Vertex
				{
					Position = new Vector3(0, 0, 0),
					Color = new Vector4(1, 1, 1, 1)
				},
				new Vertex
				{
					Position = new Vector3(1, 1, 1),
					Color = new Vector4(1, 0, 1, 1)
				}
			};

			camera_vbo = new VertexBuffer(camera_vertices) {
				Layout = color_shader_layout
			};

			camera_vao.AddVertexBuffer(camera_vbo);
		}

		#region Input Events
		protected override void OnKeyDown(KeyboardKeyEventArgs e) => Input.Set(e.Key, true);

		protected override void OnKeyUp(KeyboardKeyEventArgs e) => Input.Set(e.Key, false);

		protected override void OnMouseDown(MouseButtonEventArgs e) => Input.Set(e.Button, true);

		protected override void OnMouseMove(MouseMoveEventArgs e) {
			Input.Set(new Point(e.X, e.Y));

			if (Focused)
				Mouse.SetPosition(X + Width / 2f, Y + Height / 2f);
		}

		protected override void OnMouseUp(MouseButtonEventArgs e) => Input.Set(e.Button, false);

		protected override void OnMouseWheel(MouseWheelEventArgs e) => Input.Set(e.Delta);

		protected override void OnUnload(EventArgs e) => audio_context.Suspend();

		protected override void OnClosed(EventArgs e) => audio_context.Dispose();

		protected override void OnClosing(CancelEventArgs e) => DisplayDevice.Default.RestoreResolution();

		#endregion

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


		float speed = 10f;
		protected override void OnUpdateFrame(FrameEventArgs e) {
			base.OnUpdateFrame(e);
			ElapsedTime += e.Time;

			if (WindowState == WindowState.Minimized || !Focused) Thread.Sleep(100);

			if (Input.IsActive(Key.Escape))
				Close();

			if (Defsite.Utils.IsDebug) {
				proc = Process.GetCurrentProcess();
				var mb_ram_used = proc.PrivateMemorySize64 / 1024 / 1024;
				if (mb_ram_used > 500) {
					throw new Exception("High RAM usage. Exiting to prevent system lag. (Known memory leak)");
				}
			}

			if (Input.IsActive(Key.W))
				camera.Position += camera.Front * speed * (float)e.Time;
			if (Input.IsActive(Key.S))
				camera.Position -= camera.Front * speed * (float)e.Time;

			if (Input.IsActive(Key.Space))
				camera.Position += camera.Up * speed * (float)e.Time;
			if (Input.IsActive(Key.ShiftLeft))
				camera.Position -= camera.Up * speed * (float)e.Time;

			if (Input.IsActive(Key.D))
				camera.Position += camera.Right * speed * (float)e.Time;
			if (Input.IsActive(Key.A))
				camera.Position -= camera.Right * speed * (float)e.Time;


			var mouse = Mouse.GetState();

			if (move) {
				old_x = mouse.X;
				old_y = mouse.Y;
				move = false;
			} else {
				delta_x += (mouse.X - old_x);
				delta_y += (mouse.Y - old_y);

				old_x = mouse.X;
				old_y = mouse.Y;

				camera.Yaw = delta_x * 0.2f;
				camera.Pitch = -delta_y * 0.2f;
			}

			var camera_vertices = new Vertex[]
			{
				new Vertex
				{
					Position = Vector3.Zero,
					Color = new Vector4(1, 1, 1, 1)
				},
				new Vertex
				{
					Position = camera.Front * 50,
					Color = new Vector4(1, 0, 1, 1)
				}
			};

			camera_vao.Enable();
			camera_vbo.SetData(camera_vertices);
		}

		protected override void OnRenderFrame(FrameEventArgs e) {
			base.OnRenderFrame(e);
			if (WindowState == WindowState.Minimized) return;
			GL.ClearColor(0, 0, 0, 1);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			color_shader.Enable();

			color_shader.Set("u_projection", camera.GetProjectionMatrix());
			color_shader.Set("u_view", camera.GetViewMatrix());
			color_shader.Set("u_model", new Transform().Matrix);

			quads_vao.Enable();
			quads_index_buffer.Enable();
			GL.DrawElements(PrimitiveType.Quads, quads_index_buffer.Count, DrawElementsType.UnsignedInt, 0);

			color_shader.Set("u_model", new Transform().Matrix);

			gizmo_vao.Enable();
			gizmo_index_buffer.Enable();
			GL.LineWidth(3);
			GL.DrawElements(PrimitiveType.Lines, gizmo_index_buffer.Count, DrawElementsType.UnsignedInt, 0);

			camera_vao.Enable();
			camera_index_buffer.Enable();
			GL.LineWidth(5);
			GL.DrawElements(PrimitiveType.Lines, camera_index_buffer.Count, DrawElementsType.UnsignedInt, 0);

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