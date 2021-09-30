using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;

using Common;

using Defsite.Audio;
using Defsite.Core;
using Defsite.Graphics;
using Defsite.IO;
using Defsite.Utils;

using ImGuiNET;

using ImGuizmoNET;

using OpenTK.Audio.OpenAL;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Defsite;
public class Playground : GameWindow {
	ALContext audio_context;

	FirstPersonCamera camera, camera2, active_camera;

	FrameBuffer frame_buffer;

	Shader color_shader, texture_shader;

	BufferLayout color_shader_layout;

	IndexBuffer gizmo_index_buffer, quads_index_buffer;

	VertexArray gizmo_vao, quads_vao, viewport_vao;

	VertexBuffer quads_vbo;

	ImGuiController imgui_controller;

	Process process;

	public Playground(GameWindowSettings game_window_settings, NativeWindowSettings native_window_settings) : base(game_window_settings, native_window_settings) {
	}

	public static double ElapsedTime { get; private set; }

	public static int GameHeight { get; private set; }

	public static int GameWidth { get; private set; }

	public static int GameX { get; private set; }

	public static int GameY { get; private set; }

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
		texture_shader = Assets.Get<Shader>("TextureShader");

		color_shader_layout = new BufferLayout(new List<VertexAttribute> {
				new(color_shader.GetAttributeLocation("v_position"), VertexAttributeType.Vector3),
				new(color_shader.GetAttributeLocation("v_color"), VertexAttributeType.Vector4)
			});

		#region Gizmo

		gizmo_vao = new VertexArray();
		const float gizmo_size = 10f;

		var gizmo_vertices = new ColoredVertex[] {
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

		var gizmo_vbo = new VertexBuffer() {
			Layout = color_shader_layout
		};
		gizmo_vbo.SetData(gizmo_vertices);
		//gizmo_vbo.Resize(gizmo_vertices);

		gizmo_vao.AddVertexBuffer(gizmo_vbo);

		#endregion

		#region Quads

		quads_vao = new VertexArray();

		var ix = 0;
		// var quads_count = 20;
		var grid_count = quads_count * quads_count;

		var quads_vertices = new ColoredVertex[grid_count * 4];

		var indices = new int[grid_count * 6];
		var ind_ix = 0;

		for(var ind = 0; ind < indices.Length; ind += 6) {
			indices[ind + 0] = ind_ix;
			indices[ind + 1] = ind_ix + 1;
			indices[ind + 2] = ind_ix + 2;

			indices[ind + 3] = ind_ix + 2;
			indices[ind + 4] = ind_ix + 3;
			indices[ind + 5] = ind_ix;
			ind_ix += 4;
		}

		quads_index_buffer = new IndexBuffer(indices);

		for(var y = -quads_count / 2; y < quads_count / 2; y++) {
			for(var x = -quads_count / 2; x < quads_count / 2; x++) {
				var quad = Primitives.CreateQuad(new Vector3(x, 0, y), (x + y) % 2 == 0 ? Color.Maroon : Color.Goldenrod);
				Array.Copy(quad, 0, quads_vertices, ix, quad.Length);
				ix += quad.Length;
			}
		}

		quads_vbo = new VertexBuffer() {
			Layout = color_shader_layout
		};

		quads_vbo.SetData(quads_vertices);

		quads_vao.AddVertexBuffer(quads_vbo);

		#endregion

		#region Camera

		camera_vao = new VertexArray();

		camera_index_buffer = new IndexBuffer(new[] { 0, 1, 2, 2, 3, 0 });

		camera_vbo = new VertexBuffer() {
			Layout = color_shader_layout
		};

		camera_vao.AddVertexBuffer(camera_vbo);

		#endregion

		var dock_space = ImGui.GetID("DockSpace");

		// Allow to rebuild/reset nodes
		//var node = ImGui.DockBuilderGetNode(dock_space);

		//ImGui.DockBuilderRemoveNode(dock_space);

		ImGui.DockBuilderAddNode(dock_space);
		ImGui.DockBuilderSetNodeSize(dock_space, new Vector2(500, 500));

		// Split the dock into half top/bottom and then split
		// the resulting top node into 25% left and 75% right
		ImGui.DockBuilderSplitNode(dock_space, ImGuiDir.Up, 0.5f, out var top_id, out var bottom_id);
		ImGui.DockBuilderSplitNode(top_id, ImGuiDir.Right, 0.75f, out var left_id, out var right_id);

		// Dock specific windows by their name
		ImGui.DockBuilderDockWindow("Left", left_id);
		ImGui.DockBuilderDockWindow("Right", right_id);
		ImGui.DockBuilderDockWindow("Bottom", bottom_id);



		ImGui.DockBuilderFinish(dock_space);

		frame_buffer = new FrameBuffer(new Texture(GameWidth, GameHeight));
	}

	protected override void OnFocusedChanged(FocusedChangedEventArgs e) => base.OnFocusedChanged(e);//if (Platform.IsDebug) {//	foreach (var shader in Assets.GetAll<Shader>(AssetType.Shader))//		shader.Reload();//	Log.Info("Shaders reloaded.");//}// if (IsFocused)// 	WindowState = WindowState.Normal;

	protected override void OnUnload() => base.OnUnload();

	protected override void OnMove(WindowPositionEventArgs e) => base.OnMove(e);

	protected override void OnRefresh() => base.OnRefresh();

	protected override void OnClosing(CancelEventArgs e) => base.OnClosing(e);

	protected override void OnClosed() => base.OnClosed();

	protected override void OnMouseLeave() => base.OnMouseLeave();

	protected override void OnMouseEnter() => base.OnMouseEnter();

	protected override void OnMinimized(MinimizedEventArgs e) {
		base.OnMinimized(e);
		GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);

		imgui_controller.WindowResized(ClientSize.X, ClientSize.Y);

		UpdateClientRect();
	}

	protected override void OnMaximized(MaximizedEventArgs e) {
		base.OnMaximized(e);
		GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);

		imgui_controller.WindowResized(ClientSize.X, ClientSize.Y);

		UpdateClientRect();
	}

	protected override void OnFileDrop(FileDropEventArgs e) => base.OnFileDrop(e);

	protected override void OnResize(ResizeEventArgs e) {
		base.OnResize(e);

		UpdateClientRect();

		GL.Viewport(0, 0, GameWidth, GameHeight);

		imgui_controller.WindowResized(GameWidth, GameHeight);

		frame_buffer = new FrameBuffer(new Texture(GameWidth, GameHeight));
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
		UpdateClientRect();
		ElapsedTime += e.Time;

		if(WindowState == WindowState.Minimized || !IsFocused) {
			Thread.Sleep(100);
		}

		if(Platform.IsDebug) {
			process = Process.GetCurrentProcess();
			var mb_ram_used = process.PrivateMemorySize64 / 1024 / 1024;

			if(mb_ram_used > 500) {
				throw new Exception("High RAM usage. Exiting to prevent system lag. (Known memory leak)");
			}
		}

		Input.KeyUp(Keys.Tab, () => tab_down = false);

		Input.KeyDown(Keys.Tab, () => {
			if(!tab_down) {
				active_camera = active_camera == camera ? camera2 : camera;
			}

			tab_down = true;
		});

		var speed = 10f;
		if(CursorGrabbed) {
			if(Input.IsActive(Keys.W)) {
				active_camera.Position += active_camera.Forward * speed * (float)e.Time;
			}

			if(Input.IsActive(Keys.S)) {
				active_camera.Position -= active_camera.Forward * speed * (float)e.Time;
			}

			if(Input.IsActive(Keys.Space) && jump == false) {
				jump = true;
			}
			// active_camera.Position += active_camera.Up * speed * 5 * (float) e.Time;

			// if (Input.IsActive(Keys.LeftShift))
			// 	active_camera.Position -= active_camera.Up * speed * (float) e.Time;

			if(Input.IsActive(Keys.D)) {
				active_camera.Position += active_camera.Right * speed * (float)e.Time;
			}

			if(Input.IsActive(Keys.A)) {
				active_camera.Position -= active_camera.Right * speed * (float)e.Time;
			}
		}

		Input.KeyUp(Keys.GraveAccent, () => down = false);

		Input.KeyDown(Keys.GraveAccent, () => {
			if(!down) {
				CursorGrabbed = !CursorGrabbed;
			}

			down = true;
		});

		if(!CursorGrabbed) {
			CursorVisible = true;
			CursorGrabbed = false;
		}

		if(Input.IsActive(Keys.T)) {
			Assets.Get<Sound>("Fireplace").Play();
		}

		SoundListener.Position = active_camera.Position;
		SoundListener.Orientation = (active_camera.Forward, active_camera.Up);

		if(CursorGrabbed) {
			active_camera.Update();
		}

		if(active_camera.Position.Y < 5 && jump) {
			active_camera.Position += new Vector3(0, 50, 0) * (float)e.Time;
		} else {
			jump = false;
		}

		active_camera.Position -= new Vector3(0, 9.81f, 0) * (float)e.Time;
		if(active_camera.Position.Y < 1) {
			active_camera.Position = new Vector3(active_camera.Position.X, 1, active_camera.Position.Z);
		}

		var rotation = active_camera.GetViewMatrix().ExtractRotation();

		camera_vbo.SetData(Primitives.CreateQuad(camera2.Position, Color.Magenta));
	}

	Matrix4 zzz = Matrix4.Identity;
	bool show_grid = true;

	protected override void OnRenderFrame(FrameEventArgs e) {
		base.OnRenderFrame(e);

		if(WindowState == WindowState.Minimized) {
			return;
		}

		imgui_controller.Update(this, (float)e.Time);

		ImGuizmo.BeginFrame();
		ImGuizmo.SetRect(0, 0, GameWidth, GameHeight);

		GL.ClearColor(new Color4(255, 255, 255, 255));
		GL.Clear(ClearBufferMask.ColorBufferBit);

		frame_buffer.Enable();
		GL.ClearColor(new Color4(20, 20, 20, 255));
		GL.Enable(EnableCap.DepthTest);
		GL.DepthFunc(DepthFunction.Lequal);
		GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		var view_matrix = active_camera.GetViewMatrix();
		var projection_matrix = active_camera.GetProjectionMatrix();
		var identity = Matrix4.Identity;

		color_shader.Enable();
		color_shader.Set("u_projection", projection_matrix);
		color_shader.Set("u_view", view_matrix);
		color_shader.Set("u_model", identity);

		quads_vao.Enable();
		quads_index_buffer.Enable();
		GL.DrawElements(PrimitiveType.Triangles, quads_index_buffer.Count, DrawElementsType.UnsignedInt, 0);

		camera_vao.Enable();
		camera_index_buffer.Enable();
		GL.DrawElements(PrimitiveType.Triangles, camera_index_buffer.Count, DrawElementsType.UnsignedInt, 0);

		gizmo_vao.Enable();
		gizmo_index_buffer.Enable();
		GL.LineWidth(10f);
		GL.DrawElements(PrimitiveType.Lines, gizmo_index_buffer.Count, DrawElementsType.UnsignedInt, 0);
		if(show_grid) {
			ImGuizmo.DrawGrid(ref view_matrix.Row0.X, ref projection_matrix.Row0.X, ref identity.Row0.X, 200f);
		}
		ImGuizmo.DrawCubes(ref view_matrix.Row0.X, ref projection_matrix.Row0.X, ref zzz.Row0.X, 1);
		ImGuizmo.Manipulate(ref view_matrix.Row0.X, ref projection_matrix.Row0.X, TransformOperation.Translate, TransformMode.World, ref zzz.Row0.X);

		GL.BindVertexArray(0);

		color_shader.Disable();
		frame_buffer.Disable();

		GL.ClearColor(new Color4(255, 255, 255, 255));
		GL.Disable(EnableCap.DepthTest);
		GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, frame_buffer.ID);
		GL.FramebufferTexture2D(FramebufferTarget.ReadFramebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, frame_buffer.Texture.ID, 0);
		GL.BlitFramebuffer(0, 0, GameWidth, GameHeight, 0, 0, GameWidth, GameHeight, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear);

		#region ImGUI

		ImGui.Begin("Sidebar");
		ImGui.SliderInt("Quads", ref quads_count, 2, 100);

		if(ImGui.IsItemEdited()) {
			var ix = 0;
			var grid_count = quads_count * quads_count;

			var quads_vertices = new ColoredVertex[grid_count * 4];

			var indices = new int[grid_count * 6];
			var ind_ix = 0;

			for(var ind = 0; ind < indices.Length; ind += 6) {
				indices[ind + 0] = ind_ix;
				indices[ind + 1] = ind_ix + 1;
				indices[ind + 2] = ind_ix + 2;

				indices[ind + 3] = ind_ix + 2;
				indices[ind + 4] = ind_ix + 3;
				indices[ind + 5] = ind_ix;
				ind_ix += 4;
			}

			quads_index_buffer.SetData(indices);

			for(var y = -quads_count / 2; y < quads_count / 2; y++) {
				for(var x = -quads_count / 2; x < quads_count / 2; x++) {
					var quad = Primitives.CreateQuad(new Vector3(x, 0, y), (x + y) % 2 == 0 ? Color.Maroon : Color.Goldenrod);
					Array.Copy(quad, 0, quads_vertices, ix, quad.Length);
					ix += quad.Length;
				}
			}

			quads_vbo.SetData(quads_vertices);
		}

		ImGui.Separator();
		ImGui.SliderFloat("X", ref zzz.Row3.X, -10, 10);

		ImGui.Separator();
		ImGui.Checkbox("Grid", ref show_grid);
		ImGui.End();


		ImGui.SetNextWindowPos(Vector2.Zero);
		ImGui.SetNextWindowBgAlpha(0.35f);
		ImGui.Begin("Overlay", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoBackground);
		ImGui.SetWindowSize(new Vector2(200, 100));
		ImGui.Text($"FPS: {1 / e.Time:00} | FT: {e.Time * 1000:N}");
		ImGui.Separator();
		ImGui.Text($"{active_camera.Position}");
		ImGui.Separator();
		ImGui.Text($"{GameX}, {GameY}");
		ImGui.Separator();
		if(ImGui.IsMousePosValid()) {
			ImGui.Text($"Mouse Position: ({Input.MousePos.X}, {Input.MousePos.Y})");
		}

		ImGui.End();

		ImGui.End();

		//ImGui.SetNextWindowSize(new Vector2(300, 300));
		//ImGui.Begin("NodeEditor");

		//ImNodes.BeginNodeEditor();

		//ImNodes.BeginNode(1);
		//ImGui.Dummy(new Vector2(20, 20));
		//ImNodes.EndNode();

		//ImNodes.EndNodeEditor();

		//ImGui.End();

		var flags = ImGuiWindowFlags.NoTitleBar
			| ImGuiWindowFlags.NoCollapse
			| ImGuiWindowFlags.NoResize
			| ImGuiWindowFlags.NoMove
			| ImGuiWindowFlags.NoBringToFrontOnFocus
			| ImGuiWindowFlags.NoNavFocus;

		ImGui.Begin("Docking window", flags);
		var dummy_bool = true;

		ImGui.SetNextWindowPos(new Vector2(450, 50), ImGuiCond.Appearing);
		ImGui.SetNextWindowSize(new Vector2(150, 100), ImGuiCond.Appearing);
		if(ImGui.Begin("Left", ref dummy_bool, flags)) {
			ImGui.Text("Foo");
		}

		ImGui.End();

		ImGui.SetNextWindowPos(new Vector2(450, 200), ImGuiCond.Appearing);
		ImGui.SetNextWindowSize(new Vector2(150, 100), ImGuiCond.Appearing);
		if(ImGui.Begin("Right", ref dummy_bool, flags)) {
			ImGui.Text("Bar");
		}

		ImGui.End();

		ImGui.SetNextWindowPos(new Vector2(450, 350), ImGuiCond.Appearing);
		ImGui.SetNextWindowSize(new Vector2(150, 100), ImGuiCond.Appearing);
		if(ImGui.Begin("Bottom", ref dummy_bool, flags)) {
			ImGui.Text("Baz");
		}

		ImGui.End();

		ImGui.End();

		//ImGui.ShowDemoWindow();
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

	protected override void OnMouseMove(MouseMoveEventArgs e) => Input.Set(e.Position);

	protected override void OnMouseUp(MouseButtonEventArgs e) => Input.Set(e.Button, false);

	protected override void OnMouseWheel(MouseWheelEventArgs e) {
		Input.Set(e.OffsetY);
		imgui_controller.MouseScroll(e.Offset);
	}

	protected override void OnTextInput(TextInputEventArgs e) => imgui_controller.PressChar((char)e.Unicode);

	//protected override void OnUnload() => audio_context.Suspend();

	//protected override void OnClosed() => audio_context.Dispose();

	//protected override void OnClosing(CancelEventArgs e) => DisplayDevice.Default.RestoreResolution();

	protected override void Dispose(bool disposing) {
		base.Dispose(disposing);
		GLUtils.Dispose();
	}

	#endregion
}
