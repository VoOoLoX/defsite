using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Audio.OpenAL;
using OpenTK.Windowing.Common;
using System.Drawing;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ImGuiNET;
using System.Runtime.InteropServices;

namespace Defsite {
	public class Window : GameWindow {
		ALContext audio_context;

		MonitorInfo monitor_info;

		FirstPersonCamera camera;

		Shader color_shader;
		BufferLayout color_shader_layout;

		VertexArray gizmo_vao, quads_vao;
		IndexBuffer gizmo_index_buffer, quads_index_buffer;

		ImGuiController imgui_controller;

		Process process;

		public static double ElapsedTime { get; private set; }

		public static int Height { get; private set; }

		public static int Width { get; private set; }

		public static int X { get; private set; }

		public static int Y { get; private set; }

		public Window(
			int width,
			int height,
			string title,
			ContextFlags context_flags
		) : base(
			new GameWindowSettings
			{
				IsMultiThreaded = false
			},
			new NativeWindowSettings
			{
				Title = title,
				Size = new Vector2i(width, height),
				Flags = context_flags,
				APIVersion = new Version(4, 5)
			}) {
			if (Monitors.TryGetMonitorInfo(0, out var info))
				monitor_info = info;

			VSync = VSyncMode.Adaptive;
			//CursorVisible = false;
			//CursorGrabbed = true;

			Log.Info("Info:");
			Log.Indent();
			Log.Info($"OS: {Environment.OSVersion}");
			Log.Info($"Framework: {Environment.Version}");
			Log.Info($"CWD: {Environment.CurrentDirectory}");
			Log.Info($"Processor cores: {Environment.ProcessorCount}");
			Log.Info($"64 bit processor: {Environment.Is64BitProcess}");
			Log.Info($"64 bit OS: {Environment.Is64BitOperatingSystem}");
			Log.Unindent();

			Log.Info("Display Info:");
			Log.Indent();
			Log.Info($"Display: {monitor_info.ClientArea}");
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

			try {
				var devices = ALC.GetStringList(GetEnumerationStringList.DeviceSpecifier);
				var devices_list = devices.ToList();
				Log.Info($"Devices: {string.Join(", ", devices_list)}");

				var device_name = ALC.GetString(ALDevice.Null, AlcGetString.DefaultDeviceSpecifier);

				foreach (var d in devices_list.Where(d => d.Contains("OpenAL Soft")))
					device_name = d;

				var device = ALC.OpenDevice(device_name);
				audio_context = ALC.CreateContext(device, (int[]) null);
				ALC.MakeContextCurrent(audio_context);
			}
			catch {
				Log.Panic("Could not load 'openal32.dll'. Try installing OpenAL.");
			}

			Log.Info("OpenAL Info:");
			Log.Indent();
			Log.Info(AL.Get(ALGetString.Vendor));
			Log.Info(AL.Get(ALGetString.Renderer));
			Log.Info(AL.Get(ALGetString.Version));
			Log.Unindent();
		}

		static void DebugCallback(DebugSource source,
			DebugType type,
			int id,
			DebugSeverity severity,
			int length,
			IntPtr message,
			IntPtr user_param) {
			var message_string = Marshal.PtrToStringAnsi(message, length);

			if (type == DebugType.DebugTypeError)
				Log.Error($"{severity} {type} | {message_string}");
			else
				Log.Info($"{severity} {type} | {message_string}");
		}

		static DebugProc debug_proc_callback = DebugCallback;
		static GCHandle debug_proc_callback_handle;


		protected override void OnLoad() {
			base.OnLoad();

			debug_proc_callback_handle = GCHandle.Alloc(debug_proc_callback);

			GL.DebugMessageCallback(debug_proc_callback, IntPtr.Zero);
			GL.Enable(EnableCap.DebugOutput);
			GL.Enable(EnableCap.DebugOutputSynchronous);


			UpdateClientRect();
			Assets.LoadAssets("Assets/Assets.json");
			SoundListener.Init();
			imgui_controller = new ImGuiController(ClientSize.X, ClientSize.Y);

			camera = new FirstPersonCamera(new Vector3(0, 0, 3));

			color_shader = Assets.Get<Shader>("ColorShader");

			color_shader_layout = new BufferLayout(new List<VertexAttribute>
			{
				new(color_shader.GetAttributeLocation("v_position"), VertexAttributeType.Vector3),
				new(color_shader.GetAttributeLocation("v_color"), VertexAttributeType.Vector4)
			});

			#region Gizmo

			gizmo_vao = new VertexArray();
			const float gizmo_size = 10f;

			var gizmo_vertices = new Vertex[]
			{
				new()
				{
					Position = new Vector3(-gizmo_size, 0, 0),
					Color = new Vector4(1, 1, 1, 1)
				},
				new()
				{
					Position = new Vector3(gizmo_size, 0, 0),
					Color = new Vector4(1, 0, 0, 1)
				},

				new()
				{
					Position = new Vector3(0, -gizmo_size, 0),
					Color = new Vector4(1, 1, 1, 1)
				},
				new()
				{
					Position = new Vector3(0, gizmo_size, 0),
					Color = new Vector4(0, 1, 0, 1)
				},

				new()
				{
					Position = new Vector3(0, 0, -gizmo_size),
					Color = new Vector4(1, 1, 1, 1)
				},
				new()
				{
					Position = new Vector3(0, 0, gizmo_size),
					Color = new Vector4(0, 0, 1, 1)
				}
			};

			gizmo_index_buffer = new IndexBuffer(Enumerable.Range(0, 6).ToArray());

			var gizmo_vbo = new VertexBuffer(gizmo_vertices)
			{
				Layout = color_shader_layout
			};

			gizmo_vao.AddVertexBuffer(gizmo_vbo);

			#endregion


			#region Quads

			quads_vao = new VertexArray();

			var ix = 0;
			var quads_count = 20;
			var grid_count = quads_count * quads_count;

			var quads_vertices = new Vertex[grid_count * 4];

			var indices = new int[grid_count * 6];
			var ind_ix = 0;

			for (var ind = 0; ind < indices.Length; ind += 6) {
				indices[ind + 0] = ind_ix;
				indices[ind + 1] = ind_ix + 1;
				indices[ind + 2] = ind_ix + 2;

				indices[ind + 3] = ind_ix + 2;
				indices[ind + 4] = ind_ix + 3;
				indices[ind + 5] = ind_ix;
				ind_ix += 4;
			}

			quads_index_buffer = new IndexBuffer(indices);

			for (var y = -quads_count / 2; y < quads_count / 2; y++) {
				for (var x = -quads_count / 2; x < quads_count / 2; x++) {
					var quad = Primitives.CreateQuad(new Vector3(x, y, 0), (x + y) % 2 == 0 ? Color.Maroon : Color.Goldenrod);
					Array.Copy(quad, 0, quads_vertices, ix, quad.Length);
					ix += quad.Length;
				}
			}

			var quads_vbo = new VertexBuffer(quads_vertices)
			{
				Layout = color_shader_layout
			};

			quads_vao.AddVertexBuffer(quads_vbo);

			#endregion
		}

		#region Input Events

		protected override void OnKeyDown(KeyboardKeyEventArgs e) => Input.Set(e.Key, true);

		protected override void OnKeyUp(KeyboardKeyEventArgs e) => Input.Set(e.Key, false);

		protected override void OnMouseDown(MouseButtonEventArgs e) => Input.Set(e.Button, true);

		protected override void OnMouseMove(MouseMoveEventArgs e) => Input.Set(new Point((int) e.X, (int) e.Y));

		protected override void OnMouseUp(MouseButtonEventArgs e) => Input.Set(e.Button, false);

		protected override void OnMouseWheel(MouseWheelEventArgs e) {
			Input.Set(e.OffsetY);
			imgui_controller.MouseScroll(e.Offset);
		}

		protected override void OnTextInput(TextInputEventArgs e) => imgui_controller.PressChar((char) e.Unicode);


		//protected override void OnUnload() => audio_context.Suspend();

		//protected override void OnClosed() => audio_context.Dispose();

		//protected override void OnClosing(CancelEventArgs e) => DisplayDevice.Default.RestoreResolution();

		#endregion

		protected override void OnFocusedChanged(FocusedChangedEventArgs e) {
			base.OnFocusedChanged(e);

			//if (Platform.IsDebug) {
			//	foreach (var shader in Assets.GetAll<Shader>(AssetType.Shader))
			//		shader.Reload();
			//	Log.Info("Shaders reloaded.");
			//}

			if (IsFocused)
				WindowState = WindowState.Normal;
		}

		protected override void OnResize(ResizeEventArgs e) {
			base.OnResize(e);

			GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);

			imgui_controller.WindowResized(ClientSize.X, ClientSize.Y);

			UpdateClientRect();
		}

		protected override void OnUpdateFrame(FrameEventArgs e) {
			base.OnUpdateFrame(e);
			ElapsedTime += e.Time;

			if (WindowState == WindowState.Minimized || !IsFocused) Thread.Sleep(300);

			if (Input.IsActive(Keys.Escape))
				Close();

			if (Platform.IsDebug) {
				process = Process.GetCurrentProcess();
				var mb_ram_used = process.PrivateMemorySize64 / 1024 / 1024;

				if (mb_ram_used > 500) {
					throw new Exception("High RAM usage. Exiting to prevent system lag. (Known memory leak)");
				}
			}

			var speed = 10f;

			if (Input.IsActive(Keys.W))
				camera.Position += camera.Forward * speed * (float) e.Time;

			if (Input.IsActive(Keys.S))
				camera.Position -= camera.Forward * speed * (float) e.Time;

			if (Input.IsActive(Keys.Space))
				camera.Position += camera.Up * speed * (float) e.Time;

			if (Input.IsActive(Keys.LeftShift))
				camera.Position -= camera.Up * speed * (float) e.Time;

			if (Input.IsActive(Keys.D))
				camera.Position += camera.Right * speed * (float) e.Time;

			if (Input.IsActive(Keys.A))
				camera.Position -= camera.Right * speed * (float) e.Time;

			if (Input.IsActive(Keys.GraveAccent))
				CursorGrabbed = !CursorGrabbed;

			if (!CursorGrabbed)
				CursorVisible = true;

			if (Input.IsActive(Keys.T))
				Assets.Get<Sound>("Fireplace").Play();

			SoundListener.Position = camera.Position;
			SoundListener.Orientation = (camera.Forward, camera.Up);

			if (CursorGrabbed)
				camera.Update();
		}

		protected override void OnRenderFrame(FrameEventArgs e) {
			base.OnRenderFrame(e);

			if (WindowState == WindowState.Minimized) return;

			imgui_controller.Update(this, (float) e.Time);

			GL.ClearColor(new Color4(20, 20, 20, 255));
			GL.Enable(EnableCap.DepthTest);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
			GL.DepthFunc(DepthFunction.Lequal);

			color_shader.Enable();

			color_shader.Set("u_projection", camera.GetProjectionMatrix());
			color_shader.Set("u_view", camera.GetViewMatrix());
			color_shader.Set("u_model", new Transform().Matrix);

			quads_vao.Enable();
			quads_index_buffer.Enable();
			GL.DrawElements(PrimitiveType.Triangles, quads_index_buffer.Count, DrawElementsType.UnsignedInt, 0);

			gizmo_vao.Enable();
			gizmo_index_buffer.Enable();
			GL.DrawElements(PrimitiveType.Lines, gizmo_index_buffer.Count, DrawElementsType.UnsignedInt, 0);

			GL.Disable(EnableCap.DepthTest);

			ImGui.ShowDemoWindow();
			imgui_controller.Render();


			SwapBuffers();
		}

		void UpdateClientRect() {
			Width = ClientSize.X;
			Height = ClientSize.Y;
			X = ClientRectangle.Min.X;
			Y = ClientRectangle.Min.Y;
		}
	}
}