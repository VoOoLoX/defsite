using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

using Defsite.Audio;
using Defsite.Core;
using Defsite.Graphics;
using Defsite.IO;
using Defsite.Utils;

using ImGuiNET;

using ImGuizmoNET;

using NLog;

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

	BufferLayout color_shader_layout, texture_shader_layout;

	IndexBuffer axis_ibo, quads_ibo, camera_ibo, crosshair_ibo;

	VertexArray axis_vao, quads_vao, camera_vao, crosshair_vao;

	VertexBuffer axis_vbo, quads_vbo, camera_vbo, crosshair_vbo;

	ImGuiRenderer imgui_controller;

	InputController input_controller;

	Texture ground_texture;

	bool jump = false;

	int quads_count = 20;
	float axis_lenght = 10;
	float crosshair_lenght = 20;
	bool show_grid = false;

	Matrix4 test_cube = Matrix4.Identity;
	Matrix4 delta_test_cube = Matrix4.Identity;
	Vector3 snap = Vector3.Zero;

	TransformOperation transform_operation = TransformOperation.Translate;
	TransformMode transform_mode = TransformMode.World;

	Matrix4 camera_rot = Matrix4.Identity;

	static readonly Logger log = LogManager.GetCurrentClassLogger();

	public Playground(GameWindowSettings game_window_settings, NativeWindowSettings native_window_settings) : base(game_window_settings, native_window_settings) {
	}

	public static int GameHeight { get; private set; }

	public static int GameWidth { get; private set; }

	public static int GameX { get; private set; }

	public static int GameY { get; private set; }

	protected override void OnLoad() {
		base.OnLoad();

		try {
			var devices = ALC.GetStringList(GetEnumerationStringList.DeviceSpecifier);
			var devices_list = devices.ToList();
			log.Info($"Devices: {string.Join(", ", devices_list)}");

			var device_name = ALC.GetString(ALDevice.Null, AlcGetString.DefaultDeviceSpecifier);

			foreach(var d in devices_list.Where(d => d.Contains("OpenAL Soft"))) {
				device_name = d;
			}

			var device = ALC.OpenDevice(device_name);
			audio_context = ALC.CreateContext(device, (int[])null);
			ALC.MakeContextCurrent(audio_context);
		} catch {
			log.Fatal("Could not load 'openal32.dll'. Try installing OpenAL.");
		}

#if DEBUG
		GLUtils.InitDebugCallback();
#endif

		UpdateClientRect();

		Assets.LoadAssets("Assets/Assets.json");

		SoundListener.Init();

		input_controller = new InputController();

		imgui_controller = new ImGuiRenderer(ClientSize.X, ClientSize.Y);

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
		style.WindowPadding = new Vector2(5);

		camera = new FirstPersonCamera(new Vector3(0, 0, 3));
		camera2 = new FirstPersonCamera(new Vector3(-3, 2, -3));
		active_camera = camera;

		color_shader = Assets.Get<Shader>("ColorShader");
		texture_shader = Assets.Get<Shader>("TextureShader");

		ground_texture = Assets.Get<Texture>("Ground");

		color_shader_layout = new BufferLayout(new List<VertexAttribute> {
			new(color_shader["v_position"], VertexAttributeType.Vector3),
			new(color_shader["v_color"], VertexAttributeType.Vector4)
		});

		texture_shader_layout = new BufferLayout(new List<VertexAttribute> {
			new(texture_shader["v_position"], VertexAttributeType.Vector3),
			new(texture_shader["v_color"], VertexAttributeType.Vector4),
			new(texture_shader["v_texture_coordinates"], VertexAttributeType.Vector2)
		});

		#region Axis

		axis_vao = new VertexArray();
		axis_ibo = new IndexBuffer();
		axis_vbo = new VertexBuffer() {
			Layout = color_shader_layout
		};

		GenerateAxis();

		axis_vao.AddVertexBuffer(axis_vbo);

		#endregion

		#region Quads

		quads_vao = new VertexArray();
		quads_ibo = new IndexBuffer();
		quads_vbo = new VertexBuffer() {
			Layout = texture_shader_layout
		};

		GenerateTexturedTiles();

		quads_vao.AddVertexBuffer(quads_vbo);

		#endregion

		#region Camera

		camera_vao = new VertexArray();

		camera_ibo = new IndexBuffer(new[] { 0, 1, 2, 2, 3, 0, 4, 5, 6, 6, 7, 4 });

		camera_vbo = new VertexBuffer() {
			Layout = color_shader_layout
		};

		camera_vao.AddVertexBuffer(camera_vbo);

		#endregion

		#region Crosshair
		crosshair_vao = new VertexArray();
		crosshair_ibo = new IndexBuffer();
		crosshair_vbo = new VertexBuffer() {
			Layout = color_shader_layout
		};

		GenerateCrosshair();

		crosshair_vao.AddVertexBuffer(crosshair_vbo);
		#endregion

		frame_buffer = new FrameBuffer(new Texture(GameWidth, GameHeight));
	}

	protected override void OnUpdateFrame(FrameEventArgs e) {
		base.OnUpdateFrame(e);
		input_controller.SetState(MouseState);
		input_controller.SetState(KeyboardState);
		input_controller.SetState(JoystickStates);

		UpdateClientRect();

		if(WindowState == WindowState.Minimized || !IsFocused) {
			Thread.Sleep(1000 / 10);
		}

		Input.OnKeyPress(Keys.Tab, () => active_camera = active_camera == camera ? camera2 : camera);

		var speed = 10f;
		if(CursorGrabbed) {
			if(Input.KeyDown(Keys.LeftShift)) {
				speed = 2.5f;
			}

			if(Input.KeyDown(Keys.W)) {
				active_camera.Position += active_camera.Forward * speed * (float)e.Time;
			}

			if(Input.KeyDown(Keys.S)) {
				active_camera.Position -= active_camera.Forward * speed * (float)e.Time;
			}

			if(Input.KeyDown(Keys.D)) {
				active_camera.Position += active_camera.Right * speed * (float)e.Time;
			}

			if(Input.KeyDown(Keys.A)) {
				active_camera.Position -= active_camera.Right * speed * (float)e.Time;
			}



			Input.OnKeyPress(Keys.Space, () => jump = true);
		}

		if(Input.KeyDown(Keys.D1)) {
			transform_operation = TransformOperation.Translate;
			transform_mode = TransformMode.World;
		}

		if(Input.KeyDown(Keys.D2)) {
			transform_operation = TransformOperation.Rotate;
			transform_mode = TransformMode.World;
		}

		if(Input.KeyDown(Keys.D3)) {
			transform_operation = TransformOperation.Scale;
			transform_mode = TransformMode.Local;
		}

		snap = Vector3.Zero;

		if(Input.KeyDown(Keys.LeftShift)) {
			snap = transform_operation == TransformOperation.Rotate ? new Vector3(5) : Vector3.One;
		}

		Input.OnKeyPress(Keys.GraveAccent, () => CursorGrabbed = !CursorGrabbed);

		if(!CursorGrabbed) {
			CursorVisible = true;
		}

		Input.OnKeyPress(Keys.T, () => Assets.Get<Sound>("Fireplace").Play());

		SoundListener.Position = active_camera.Position;
		SoundListener.Orientation = (active_camera.Forward, active_camera.Up);

		if(CursorGrabbed) {
			active_camera.Update();
		}

		if(active_camera.Position.Y < 10 && jump) {
			active_camera.Position += new Vector3(0, 50, 0) * (float)e.Time;
		} else {
			jump = false;
			active_camera.Position -= new Vector3(0, 15, 0) * (float)e.Time;
		}

		if(active_camera.Position.Y < 1) {
			active_camera.Position = new Vector3(active_camera.Position.X, 1, active_camera.Position.Z);
		}

		if(active_camera == camera2) {
			var rotation = active_camera.GetViewMatrix().ExtractRotation();
			camera_rot = Matrix4.CreateFromQuaternion(rotation);
		}

		camera_vbo.SetData(Primitives.CreateQuadCentered(camera2.Position, Color.FromArgb(0, 155, 114), .5f)
			.Concat(Primitives.CreateQuadCentered(camera2.Position + (0, 0, .3f), Color.FromArgb(0, 124, 114), .2f))
			.ToArray()
		);

		imgui_controller.Update((float)e.Time);
	}

	protected override void OnRenderFrame(FrameEventArgs e) {
		base.OnRenderFrame(e);

		if(WindowState == WindowState.Minimized) {
			return;
		}

		GL.ClearColor(new Color4(255, 255, 255, 255));
		GL.Clear(ClearBufferMask.ColorBufferBit);

		frame_buffer.Enable();
		GL.ClearDepth(1000.0);
		GL.ClearColor(new Color4(20, 20, 20, 255));
		GL.Enable(EnableCap.DepthTest);
		//GL.Enable(EnableCap.Blend);
		GL.DepthFunc(DepthFunction.Lequal);
		//GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
		GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		var view_matrix = active_camera.GetViewMatrix();
		var projection_matrix = active_camera.GetProjectionMatrix();
		var identity = Matrix4.Identity;

		texture_shader.Enable();
		texture_shader.Set("u_projection", projection_matrix);
		texture_shader.Set("u_view", view_matrix);
		texture_shader.Set("u_model", identity);

		{
			quads_vao.Enable();
			quads_ibo.Enable();
			ground_texture.Enable();
			GL.DrawElements(PrimitiveType.Triangles, quads_ibo.Count, DrawElementsType.UnsignedInt, 0);
		}

		texture_shader.Disable();

		color_shader.Enable();
		color_shader.Set("u_projection", projection_matrix);
		color_shader.Set("u_view", view_matrix);
		color_shader.Set("u_model", identity);

		{
			axis_vao.Enable();
			axis_ibo.Enable();
			GL.LineWidth(10f);
			GL.DrawElements(PrimitiveType.Lines, axis_ibo.Count, DrawElementsType.UnsignedInt, 0);
		}

		{
			color_shader.Set("u_model", Matrix4.CreateTranslation(-camera2.Position) * camera_rot.Inverted() * Matrix4.CreateTranslation(camera2.Position));

			camera_vao.Enable();
			camera_ibo.Enable();
			GL.DrawElements(PrimitiveType.Triangles, camera_ibo.Count, DrawElementsType.UnsignedInt, 0);

			color_shader.Set("u_model", identity);
		}

		{
			color_shader.Set("u_projection", Matrix4.CreateOrthographic(GameWidth, GameHeight, -1000, 1000f));
			color_shader.Set("u_view", identity);

			crosshair_vao.Enable();
			crosshair_ibo.Enable();
			GL.LineWidth(1f);
			GL.DrawElements(PrimitiveType.Lines, crosshair_ibo.Count, DrawElementsType.UnsignedInt, 0);

			//color_shader.Set("u_projection", projection_matrix);
			color_shader.Set("u_view", view_matrix);
		}

		{
			if(show_grid) {
				ImGuizmo.DrawGrid(ref view_matrix, ref projection_matrix, ref identity, 200f);
			}
			ImGuizmo.DrawCubes(ref view_matrix, ref projection_matrix, ref test_cube, 1);
			ImGuizmo.Manipulate(ref view_matrix, ref projection_matrix, transform_operation, transform_mode, ref test_cube, ref delta_test_cube, ref snap);
		}

		color_shader.Disable();
		frame_buffer.Disable();

		GL.BindVertexArray(0);

		{
			GL.ClearColor(new Color4(255, 255, 255, 255));
			//GL.Disable(EnableCap.DepthTest);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, frame_buffer.ID);
			GL.FramebufferTexture2D(FramebufferTarget.ReadFramebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, frame_buffer.Texture.ID, 0);
			GL.BlitFramebuffer(0, 0, GameWidth, GameHeight, 0, 0, GameWidth, GameHeight, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear);
		}

		#region ImGUI

		ImGui.Begin("Sidebar");
		{
			if(ImGui.SliderInt("Quads", ref quads_count, 2, 500)) {
				GenerateTexturedTiles();
			}

			ImGui.Separator();

			if(ImGui.SliderFloat("Axis", ref axis_lenght, 0, 50)) {
				GenerateAxis();
			}

			ImGui.Separator();

			if(ImGui.SliderFloat("Crosshair", ref crosshair_lenght, 0, 50)) {
				GenerateCrosshair();
			}

			ImGui.Separator();

			ImGui.InputFloat4("Cube", ref test_cube.Row3);
			ImGui.Separator();

			ImGui.Checkbox("Grid", ref show_grid);
			ImGui.End();
		}

		ImGui.End();

		ImGui.SetNextWindowPos(Vector2.Zero);
		ImGui.SetNextWindowBgAlpha(0.35f);
		ImGui.Begin("Overlay", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoInputs);
		{
			ImGui.SetWindowSize(new Vector2(250, 225));

			ImGui.Text($"FPS: {1 / e.Time:00} | FT: {e.Time * 1000:N}");
			ImGui.Separator();

			ImGui.Text($"Camera: {active_camera.Position.X:F2} {active_camera.Position.Y:F2} {active_camera.Position.Z:F2}");
			ImGui.Separator();

			if(ImGui.IsMousePosValid()) {
				ImGui.Text($"Mouse Position: ({Input.MousePosition.X}, {Input.MousePosition.Y})");
			}
			ImGui.Separator();
			ImGui.Text($"Controls:");
			ImGui.Text($"WASD - move");
			ImGui.Text($"Space - jump");
			ImGui.Text($"LShift - sneak");
			ImGui.Text($"Tab - toggle camera");
			ImGui.Text($"~ - toggle focus");
			ImGui.Text($"1 - translate");
			ImGui.Text($"2 - rotate");
			ImGui.Text($"3 - scale");

		}

		ImGui.End();

		ImGui.ShowDemoWindow();

		imgui_controller.Render();

		#endregion

		SwapBuffers();
	}

	void GenerateCrosshair() {
		var crosshair_vertices = new ColoredVertex[] {
			new() {
				Position = new Vector3(-crosshair_lenght / 2, 0, -.1f),
				Color = new Vector4(1, 1, 1, 1)
			},
			new() {
				Position = new Vector3(crosshair_lenght / 2, 0, -.1f),
				Color = new Vector4(1, 1, 1, 1)
			},

			new() {
				Position = new Vector3(0, -crosshair_lenght / 2, -.1f),
				Color = new Vector4(1, 1, 1, 1)
			},
			new() {
				Position = new Vector3(0, crosshair_lenght / 2, -.1f),
				Color = new Vector4(1, 1, 1, 1)
			}
		};

		crosshair_ibo.SetData(Enumerable.Range(0, 4).ToArray());

		crosshair_vbo.SetData(crosshair_vertices);
	}

	void GenerateAxis() {
		var axis_vertices = new ColoredVertex[] {
			new() {
				Position = new Vector3(-axis_lenght / 2, 0, 0),
				Color = new Vector4(1, 1, 1, 1)
			},
			new() {
				Position = new Vector3(axis_lenght / 2, 0, 0),
				Color = new Vector4(1, 0, 0, 1)
			},

			new() {
				Position = new Vector3(0, -axis_lenght / 2, 0),
				Color = new Vector4(1, 1, 1, 1)
			},
			new() {
				Position = new Vector3(0, axis_lenght / 2, 0),
				Color = new Vector4(0, 1, 0, 1)
			},

			new() {
				Position = new Vector3(0, 0, -axis_lenght / 2),
				Color = new Vector4(1, 1, 1, 1)
			},
			new() {
				Position = new Vector3(0, 0, axis_lenght / 2),
				Color = new Vector4(0, 0, 1, 1)
			}
		};

		axis_ibo.SetData(Enumerable.Range(0, 6).ToArray());

		axis_vbo.SetData(axis_vertices);
	}

	void GenerateCheckerTiles() {
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

		quads_ibo.SetData(indices);

		for(var y = -quads_count / 2; y < quads_count / 2; y++) {
			for(var x = -quads_count / 2; x < quads_count / 2; x++) {
				var quad = Primitives.CreateTile(new Vector3(x, 0, y), (x + y) % 2 == 0 ? Color.FromArgb(42, 45, 52) : Color.FromArgb(0, 157, 220));
				Array.Copy(quad, 0, quads_vertices, ix, quad.Length);
				ix += quad.Length;
			}
		}

		quads_vbo.SetData(quads_vertices);
	}

	void GenerateTexturedTiles() {
		var ix = 0;
		var grid_count = quads_count * quads_count;

		var quads_vertices = new TexturedVertex[grid_count * 4];

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

		quads_ibo.SetData(indices);

		for(var y = -quads_count / 2; y < quads_count / 2; y++) {
			for(var x = -quads_count / 2; x < quads_count / 2; x++) {
				var quad = Primitives.CreateTexturedTile(new Vector3(x, 0, y), Color.White);
				Array.Copy(quad, 0, quads_vertices, ix, quad.Length);
				ix += quad.Length;
			}
		}

		quads_vbo.SetData(quads_vertices);
	}

	void UpdateClientRect() {
		GameWidth = ClientSize.X;
		GameHeight = ClientSize.Y;
		GameX = ClientRectangle.Min.X;
		GameY = ClientRectangle.Min.Y;
	}

	protected override void OnTextInput(TextInputEventArgs e) => imgui_controller.PressChar((char)e.Unicode);

	protected override void OnResize(ResizeEventArgs e) {
		base.OnResize(e);

		UpdateClientRect();

		GL.Viewport(0, 0, GameWidth, GameHeight);

		imgui_controller.WindowResized(GameWidth, GameHeight);

		frame_buffer.Texture.Resize(GameWidth, GameHeight);
	}

	protected override void Dispose(bool disposing) {
		base.Dispose(disposing);
		GLUtils.Dispose();
	}
}