using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

using Common;

using ImGuiNET;
using ImGuizmoNET;

using OpenTK.Audio.OpenAL;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

using Vector2 = System.Numerics.Vector2;

namespace Defsite {
	public class Game : GameWindow {
		ALContext audio_context;

		FirstPersonCamera camera, camera2, active_camera;

		FrameBuffer frame_buffer;

		Shader color_shader;

		BufferLayout color_shader_layout;

		IndexBuffer gizmo_index_buffer, quads_index_buffer;

		VertexArray gizmo_vao, quads_vao;

		VertexBuffer quads_vbo;

		ImGuiController imgui_controller;

		readonly MonitorInfo monitor_info;

		Process process;

		public Game(GameWindowSettings game_window_settings, NativeWindowSettings native_window_settings) : base(game_window_settings, native_window_settings) {
			var monitor_handle = Monitors.GetMonitorFromWindow(this);
			Monitors.TryGetMonitorInfo(monitor_handle, out monitor_info);

			LogInfo();
		}

		void LogInfo() {
			Log.Info("Info:");
			Log.Indent();
			Log.Info($"OS: {Environment.OSVersion}");
			Log.Info($".NET: {Environment.Version}");
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
			} catch {
				Log.Panic("Could not load 'openal32.dll'. Try installing OpenAL.");
			}

			Log.Info("OpenAL Info:");
			Log.Indent();
			Log.Info(AL.Get(ALGetString.Vendor));
			Log.Info(AL.Get(ALGetString.Renderer));
			Log.Info(AL.Get(ALGetString.Version));
			Log.Unindent();
		}

		public static double ElapsedTime { get; private set; }

		public static int GameHeight { get; private set; }

		public static int GameWidth { get; private set; }

		public static int GameX { get; private set; }

		public static int GameY { get; private set; }

		public virtual void Start() { }

		protected override void OnLoad() {
			base.OnLoad();

			#if DEBUG
			GLUtils.InitDebugCallback();
			#endif

			UpdateClientRect();

			Assets.LoadAssets("Assets/Assets.json");

			// SoundListener.Init();

			imgui_controller = new ImGuiController(ClientSize.X, ClientSize.Y);
			

			//TODO move settings to a json file and load on controller initialisation
			var style = ImGui.GetStyle();
			style.ChildBorderSize = 0;
			style.FrameBorderSize = 0;
			style.PopupBorderSize = 0;
			style.TabBorderSize = 0;
			style.WindowBorderSize = 0;

			style.WindowTitleAlign = new Vector2(0.5f);

			style.ScrollbarSize = 12;
			style.GrabMinSize = 5;

			style.ChildRounding = 0;
			style.FrameRounding = 0;
			style.GrabRounding = 0;
			style.PopupRounding = 0;
			style.ScrollbarRounding = 0;
			style.TabRounding = 0;
			style.WindowRounding = 0;
			style.FramePadding = new Vector2(5);
			style.WindowPadding = new Vector2(0);


			camera = new FirstPersonCamera(new Vector3(0, 0, 3));
			active_camera = camera;
			camera2 = new FirstPersonCamera(new Vector3(-3, 2, -3));

			color_shader = Assets.Get<Shader>("ColorShader");

			color_shader_layout = new BufferLayout(new List<VertexAttribute> {
				new(color_shader.GetAttributeLocation("v_position"), VertexAttributeType.Vector3),
				new(color_shader.GetAttributeLocation("v_color"), VertexAttributeType.Vector4)
			});

			#region Gizmo

			gizmo_vao = new VertexArray();
			const float gizmo_size = 10f;

			var gizmo_vertices = new Vertex[] {
				new() {
					Position = new Vector3(-gizmo_size, 0, 0),
					Color = new Vector4(1, 1, 1, 1)
				},
				new() {
					Position = new Vector3(gizmo_size, 0, 0),
					Color = new Vector4(1, 0, 0, 1)
				},

				new() {
					Position = new Vector3(0, -gizmo_size, 0),
					Color = new Vector4(1, 1, 1, 1)
				},
				new() {
					Position = new Vector3(0, gizmo_size, 0),
					Color = new Vector4(0, 1, 0, 1)
				},

				new() {
					Position = new Vector3(0, 0, -gizmo_size),
					Color = new Vector4(1, 1, 1, 1)
				},
				new() {
					Position = new Vector3(0, 0, gizmo_size),
					Color = new Vector4(0, 0, 1, 1)
				}
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
			// var quads_count = 20;
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
					var quad = Primitives.CreateQuad(new Vector3(x, 0, y), (x + y) % 2 == 0 ? Color.Maroon : Color.Goldenrod);
					Array.Copy(quad, 0, quads_vertices, ix, quad.Length);
					ix += quad.Length;
				}
			}
			
			quads_vbo = new VertexBuffer(quads_vertices) {
				Layout = color_shader_layout
			};
			
			quads_vao.AddVertexBuffer(quads_vbo);

			#endregion

			#region Camera

			camera_vao = new VertexArray();

			camera_index_buffer = new IndexBuffer(new []{0,1,2,2,3,0});

			camera_vbo = new VertexBuffer(Primitives.CreateQuad(camera2.Position, Color.Magenta)) {
				Layout = color_shader_layout
			};

			camera_vao.AddVertexBuffer(camera_vbo);

			#endregion
			
			frame_buffer = new FrameBuffer(new Texture(new TextureFile((ushort) GameWidth, (ushort) GameHeight)));
		}

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

		bool jump = false;
		bool down = false;
		bool tab_down = false;
		int quads_count = 20;
		
		VertexArray camera_vao;
		IndexBuffer camera_index_buffer;
		VertexBuffer camera_vbo;

		protected override void OnUpdateFrame(FrameEventArgs e) {
			base.OnUpdateFrame(e);
			ElapsedTime += e.Time;

			if (WindowState == WindowState.Minimized || !IsFocused) Thread.Sleep(100);

			if (Platform.IsDebug) {
				process = Process.GetCurrentProcess();
				var mb_ram_used = process.PrivateMemorySize64 / 1024 / 1024;

				if (mb_ram_used > 500) throw new Exception("High RAM usage. Exiting to prevent system lag. (Known memory leak)");
			}

			Input.KeyUp(Keys.Tab, () => {
				tab_down = false;
			});
			
			Input.KeyDown(Keys.Tab, () => {
				if (!tab_down)
					if (active_camera == camera)
						active_camera = camera2;
					else
						active_camera = camera;
				tab_down = true;
			});
			
			var speed = 10f;
			if (CursorGrabbed) {
				if (Input.IsActive(Keys.W))
					active_camera.Position += active_camera.Forward * speed * (float) e.Time;

				if (Input.IsActive(Keys.S))
					active_camera.Position -= active_camera.Forward * speed * (float) e.Time;

				if (Input.IsActive(Keys.Space) && jump == false)
					jump = true;
				// active_camera.Position += active_camera.Up * speed * 5 * (float) e.Time;

				// if (Input.IsActive(Keys.LeftShift))
				// 	active_camera.Position -= active_camera.Up * speed * (float) e.Time;

				if (Input.IsActive(Keys.D))
					active_camera.Position += active_camera.Right * speed * (float) e.Time;

				if (Input.IsActive(Keys.A))
					active_camera.Position -= active_camera.Right * speed * (float) e.Time;
			}

			

			Input.KeyUp(Keys.GraveAccent, () => {
				down = false;
			});
			
			Input.KeyDown(Keys.GraveAccent, () => {
				if (!down)
					CursorGrabbed = !CursorGrabbed;
				down = true;
			});

			if (!CursorGrabbed) {
				CursorVisible = true;
				CursorGrabbed = false;
			}

			if (Input.IsActive(Keys.T))
				Assets.Get<Sound>("Fireplace").Play();

			SoundListener.Position = active_camera.Position;
			SoundListener.Orientation = (active_camera.Forward, active_camera.Up);

			if (CursorGrabbed) {
				active_camera.Update();
				// unsafe {
				// 	GLFW.SetCursorPos(WindowPtr, 0, 0);	
				// }

			}

			if (active_camera.Position.Y < 5 && jump)
				active_camera.Position += new Vector3(0, 50, 0) * (float) e.Time;
			else
				jump = false;

			active_camera.Position -= new Vector3(0, 9.81f, 0) * (float) e.Time;
			if (active_camera.Position.Y < 1)
				active_camera.Position = new Vector3(active_camera.Position.X, 1, active_camera.Position.Z);
			
			camera_vbo.SetData(Primitives.CreateQuad(camera2.Position, Color.Magenta));
		}

		Matrix4 zzz = Matrix4.Identity;
		protected override void OnRenderFrame(FrameEventArgs e) {
			base.OnRenderFrame(e);

			if (WindowState == WindowState.Minimized) return;

			imgui_controller.Update(this, (float) e.Time);
			
			ImGuizmo.BeginFrame();
			
			
			GL.ClearColor(new Color4(255, 255, 255, 255));
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			GL.Enable(EnableCap.DepthTest);
			GL.DepthFunc(DepthFunction.Lequal);
			
			frame_buffer.Enable();
			GL.ClearColor(new Color4(20, 20, 20, 255));
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			
			color_shader.Enable();
			color_shader.Set("u_projection", active_camera.GetProjectionMatrix());
			color_shader.Set("u_view", active_camera.GetViewMatrix());
			color_shader.Set("u_model", new Transform().Matrix);
			
			quads_vao.Enable();
			quads_index_buffer.Enable();
			GL.DrawElements(PrimitiveType.Triangles, quads_index_buffer.Count, DrawElementsType.UnsignedInt, 0);

			camera_vao.Enable();
			camera_index_buffer.Enable();
			GL.DrawElements(PrimitiveType.Triangles, camera_index_buffer.Count, DrawElementsType.UnsignedInt, 0);
			
			gizmo_vao.Enable();
			gizmo_index_buffer.Enable();
			GL.LineWidth(3f);
			GL.DrawElements(PrimitiveType.Lines, gizmo_index_buffer.Count, DrawElementsType.UnsignedInt, 0);

			GL.BindVertexArray(0);
			
			color_shader.Disable();
			frame_buffer.Disable();

			#region ImGUI
			
			var window_flags = ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoDocking;
			var viewport = ImGui.GetMainViewport();
			ImGui.SetNextWindowPos(viewport.Pos);
			ImGui.SetNextWindowSize(viewport.Size);
			ImGui.SetNextWindowViewport(viewport.ID);
			ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
			ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);

			window_flags |= ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove;
			window_flags |= ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus;
			window_flags |= ImGuiWindowFlags.NoBackground;

			ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
			ImGui.Begin("DockSpace", window_flags);
			ImGui.PopStyleVar();
			ImGui.PopStyleVar(2);
			if (ImGui.GetIO().ConfigFlags.HasFlag(ImGuiConfigFlags.DockingEnable)) {
				var docking_space = ImGui.GetID("DockingSpace");
				ImGui.DockSpace(docking_space, Vector2.Zero, ImGuiDockNodeFlags.PassthruCentralNode);
				
				if (ImGui.BeginMainMenuBar()) {
					if (ImGui.BeginMenu("File")) {
						if (ImGui.MenuItem("New")) { }

						ImGui.EndMenu();
					}

					ImGui.EndMainMenuBar();
				}


				if (ImGui.Begin("Sidebar")) {
					ImGui.Text($"Sidebar");
					
					ImGui.Separator();
					ImGui.SliderInt("Quads", ref quads_count, 2, 100);
					
					
					if (ImGui.IsItemEdited()) {
						var ix = 0;
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
			
						quads_index_buffer.SetData(indices);
			
						for (var y = -quads_count / 2; y < quads_count / 2; y++) {
							for (var x = -quads_count / 2; x < quads_count / 2; x++) {
								var quad = Primitives.CreateQuad(new Vector3(x, 0, y), (x + y) % 2 == 0 ? Color.Maroon : Color.Goldenrod);
								Array.Copy(quad, 0, quads_vertices, ix, quad.Length);
								ix += quad.Length;
							}
						}
						quads_vbo.SetData(quads_vertices);
					}
					ImGui.Separator();
					ImGui.SliderFloat("X", ref zzz.Row3.X, -10, 10);
					ImGui.End();
				}


				if (ImGui.Begin("Viewport")) {
					ImGui.Image((IntPtr) frame_buffer.Texture.ID, ImGui.GetContentRegionAvail(), new Vector2(0, 1), new Vector2(1, 0));

					var pos = ImGui.GetWindowPos();
					ImGuizmo.SetRect(pos.X, pos.Y + 23, ImGui.GetWindowContentRegionMax().X, ImGui.GetWindowContentRegionMax().Y - 23);
					var view_matrix = active_camera.GetViewMatrix();
					var projection_matrix = active_camera.GetProjectionMatrix();
					var identity = Matrix4.Identity;
					
					ImGuizmo.DrawGrid(ref view_matrix.Row0.X, ref projection_matrix.Row0.X, ref identity.Row0.X, 200f);
					ImGuizmo.DrawCubes(ref view_matrix.Row0.X, ref projection_matrix.Row0.X, ref zzz.Row0.X, 1);
					ImGuizmo.Manipulate(ref view_matrix.Row0.X, ref projection_matrix.Row0.X, OPERATION.SCALE, MODE.WORLD, ref zzz.Row0.X);
					
					
					ImGui.SetNextWindowPos(new Vector2(pos.X, pos.Y + 24), ImGuiCond.Always);
					if (ImGui.Begin("Overlay", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoBackground)) {
						ImGui.SetWindowSize(new Vector2(200, 100));
						
						ImGui.Text($"FPS: {1 / e.Time:00} | FT: {e.Time * 1000:N}");
						ImGui.Separator();
						ImGui.Text($"{active_camera.Position}");
						ImGui.Separator();
						if (ImGui.IsMousePosValid())
							ImGui.Text($"Mouse Position: ({Input.MousePos.X}, {Input.MousePos.Y})");
						ImGui.End();
					}
					ImGui.End();
				}

				ImGui.ShowDemoWindow();
			}
			ImGui.End();
			
			imgui_controller.Render();
			#endregion

			SwapBuffers();
		}

		void UpdateClientRect() {
			GameWidth = ClientSize.X;
			GameHeight = ClientSize.Y;
			GameX = ClientRectangle.Min.X;
			GameY = ClientRectangle.Min.Y;
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

		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			GLUtils.Dispose();
		}

		#endregion
	}
}