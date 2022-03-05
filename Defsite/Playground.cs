using System;
using System.Drawing;
using System.IO;
using System.Threading;

using Defsite.Core;
using Defsite.Graphics;
using Defsite.Graphics.Buffers;
using Defsite.Graphics.Cameras;
using Defsite.Graphics.Renderers;
using Defsite.IO;
using Defsite.IO.DataFormats;
using Defsite.Utils;
using ImGuiNET;

using ImGuizmoNET;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Defsite;

public class Playground : Scene {
	//ALContext audio_context;

	FirstPersonCamera? camera, camera2, active_camera;

	OrthographicCamera? orthographic_camera;

	FrameBuffer? frame_buffer;

	ImGuiRenderer? imgui_controller;

	Texture? ground_texture;

	int quads_count = 20;
	float axis_lenght = 10;
	float crosshair_lenght = 20;
	bool show_grid = false;
	bool use_framebuffer = false;

	Matrix4 test_cube = Matrix4.Identity;
	Matrix4 delta_test_cube = Matrix4.Identity;
	Matrix4 camera_rot = Matrix4.Identity;
	Vector3 snap = Vector3.Zero;

	TransformOperation transform_operation = TransformOperation.Translate;
	TransformMode transform_mode = TransformMode.World;

	//IPoint[] points = new IPoint[10 * 10];

	//IPoint[] tris;

	public override Color ClearColor => Color.Black;

	public override void Start() {
		GLUtils.InitDebugCallback();

		Renderer2D.Init();

		//SoundListener.Init();

		imgui_controller = new ImGuiRenderer(Window.ClientSize.X, Window.ClientSize.Y);

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

		camera = new FirstPersonCamera(new Vector3(0, 0, 3), Window.ClientSize.X, Window.ClientSize.Y);
		camera2 = new FirstPersonCamera(new Vector3(-3, 2, -3), Window.ClientSize.X, Window.ClientSize.Y);
		active_camera = camera;

		orthographic_camera = new OrthographicCamera(0, Window.ClientSize.X, 0, Window.ClientSize.Y);

		ground_texture = Assets.Get<Texture>("Ground");

		frame_buffer = new FrameBuffer(Window.ClientSize.X, Window.ClientSize.Y);

		//var random = new Random();

		//for(var y = 0; y < 10; y++) {
		//	for(var x = 0; x < 10; x++) {
		//		points[x + y * 10] = new Point(random.Next(-10, 10), random.Next(-10, 10));
		//	}
		//}

		//var de = new Delaunator(points);

		//var idx = 0;
		//var cnt = 0;

		//de.ForEachVoronoiCellBasedOnCentroids((cell) => {
		//	foreach(var p in cell.Points) {
		//		cnt++;
		//	}
		//});

		//tris = new IPoint[cnt];

		//de.ForEachVoronoiCellBasedOnCentroids((cell) => {
		//	foreach(var p in cell.Points) {
		//		tris[idx++] = p;
		//	}
		//});

		Window.Resize += (resize_event) => {
			GL.Viewport(0, 0, Window.ClientSize.X, Window.ClientSize.Y);

			imgui_controller.WindowResized(Window.ClientSize.X, Window.ClientSize.Y);

			frame_buffer.Resize(Window.ClientSize.X, Window.ClientSize.Y);

			camera.UpdateAspectRatio(Window.ClientSize.X, Window.ClientSize.Y);
			camera2.UpdateAspectRatio(Window.ClientSize.X, Window.ClientSize.Y);

			orthographic_camera.UpdateProjection(0, Window.ClientSize.X, 0, Window.ClientSize.Y);
		};

		Window.TextInput += (text_input_event) => imgui_controller.PressChar((char)text_input_event.Unicode);

		Window.Closed += () => GLUtils.Dispose();
	}

	public override void Update(FrameEventArgs frame_event) {
		if(Window.WindowState == WindowState.Minimized || !Window.IsFocused) {
			Thread.Sleep(1000 / 10);
		}

		Input.OnKeyPress(Keys.Tab, () => active_camera = active_camera == camera ? camera2 : camera);

		var speed = 10f;
		if(Window.CursorGrabbed) {
			if(Input.KeyDown(Keys.LeftControl)) {
				speed = 2.5f;
			}

			if(Input.KeyDown(Keys.W)) {
				active_camera.Position += active_camera.Forward * speed * (float)frame_event.Time;
			}

			if(Input.KeyDown(Keys.S)) {
				active_camera.Position -= active_camera.Forward * speed * (float)frame_event.Time;
			}

			if(Input.KeyDown(Keys.D)) {
				active_camera.Position += active_camera.Right * speed * (float)frame_event.Time;
			}

			if(Input.KeyDown(Keys.A)) {
				active_camera.Position -= active_camera.Right * speed * (float)frame_event.Time;
			}

			if(Input.KeyDown(Keys.Space)) {
				active_camera.Position += active_camera.Up * speed * (float)frame_event.Time;
			}

			if(Input.KeyDown(Keys.LeftShift)) {
				active_camera.Position -= active_camera.Up * speed * (float)frame_event.Time;
			}
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

		if(Input.KeyDown(Keys.LeftAlt)) {
			snap = transform_operation == TransformOperation.Rotate ? new Vector3(5) : Vector3.One;
		}

		Input.OnKeyPress(Keys.GraveAccent, () => {
			Window.CursorGrabbed = !Window.CursorGrabbed;
			active_camera.FirstUpdate = true;
		});

		if(!Window.CursorGrabbed) {
			Window.CursorVisible = true;
		}

		//Input.OnKeyPress(Keys.T, () => Assets.Get<Sound>("Fireplace").Play());

		//SoundListener.Position = active_camera.Position;
		//SoundListener.Orientation = (active_camera.Forward, active_camera.Up);

		ImGui.GetIO().ConfigFlags &= ~(ImGuiConfigFlags.NoMouse | ImGuiConfigFlags.NavEnableKeyboard);
		if(Window.CursorGrabbed) {
			ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.NoMouse | ImGuiConfigFlags.NavEnableKeyboard;
			active_camera.Update();
		}

		//if(active_camera.Position.Y < 30 && jump) {
		//	active_camera.Position += new Vector3(0, 50, 0) * (float)frame_event.Time;
		//} else {
		//	jump = false;
		//	active_camera.Position -= new Vector3(0, 15, 0) * (float)frame_event.Time;
		//}

		//if(active_camera.Position.Y < 1) {
		//	active_camera.Position = new Vector3(active_camera.Position.X, 1, active_camera.Position.Z);
		//}

		camera_rot = Matrix4.CreateFromQuaternion(active_camera == camera ? camera2.ViewMatrix.ExtractRotation() : camera.ViewMatrix.ExtractRotation());

		imgui_controller.Update((float)frame_event.Time);
	}

	public override void Render(FrameEventArgs frame_event) {
		if(Window.WindowState == WindowState.Minimized) {
			return;
		}

		if(use_framebuffer) {
			frame_buffer.Bind();
		}

		GL.ClearColor(ClearColor);
		GL.Enable(EnableCap.CullFace);
		GL.CullFace(CullFaceMode.Back);
		GL.FrontFace(FrontFaceDirection.Ccw);
		GL.DepthFunc(DepthFunction.Lequal);
		GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
		GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		var view_matrix = active_camera.ViewMatrix;
		var projection_matrix = active_camera.ProjectionMatrix;
		var identity = Matrix4.Identity;

		Renderer2D.Begin(active_camera);
		for(var z = -quads_count / 2; z < quads_count / 2; z++) {
			for(var x = -quads_count / 2; x < quads_count / 2; x++) {
				//ground_texture.Enable();
				Renderer2D.DrawTile(new Vector3(x, 0, z), centered: true, transform: Matrix4.CreateTranslation(0, -MathF.Pow(x * x + z * z, 0.4f), 0), texture: ground_texture);
				//ground_texture.Disable();
			}
		}

		//foreach(var point in points) {
		//	Renderer2D.DrawTile(new Vector3((float)point.X, 0, (float)point.Y), true, .2f, Color.White);
		//}

		//foreach(var p in tris) {
		//	Renderer2D.DrawTile(new Vector3((float)p.X, 0, (float)p.Y), true, .1f, Color.Red);
		//}

		Renderer2D.LineWidth = 5f;
		Renderer2D.DrawLine(new Vector3(-axis_lenght / 2, 0, 0), new Vector3(axis_lenght / 2, 0, 0), Color.White, Color.Red);
		Renderer2D.DrawLine(new Vector3(0, -axis_lenght / 2, 0), new Vector3(0, axis_lenght / 2, 0), Color.White, Color.Green);
		Renderer2D.DrawLine(new Vector3(0, 0, -axis_lenght / 2), new Vector3(0, 0, axis_lenght / 2), Color.White, Color.Blue);

		var transform = Matrix4.CreateTranslation(active_camera == camera ? -camera2.Position : -camera.Position) * camera_rot.Inverted() * Matrix4.CreateTranslation(active_camera == camera ? camera2.Position : camera.Position);
		Renderer2D.DrawQuad(active_camera == camera ? camera2.Position : camera.Position, color: Color.FromArgb(0, 155, 114), size: new Vector2(.5f), centered: true, transform: transform);
		Renderer2D.DrawQuad(active_camera == camera ? camera2.Position + (0, 0, .3f) : camera.Position + (0, 0, .3f), color: Color.FromArgb(0, 124, 114), size: new Vector2(.1f), centered: true, transform: transform);

		Renderer2D.End();

		Renderer2D.Begin(orthographic_camera);
		Renderer2D.LineWidth = 1f;
		Renderer2D.DrawLine(new Vector3(Window.ClientSize.X / 2 - crosshair_lenght / 2, Window.ClientSize.Y / 2, 0), new Vector3(Window.ClientSize.X / 2 + crosshair_lenght / 2, Window.ClientSize.Y / 2, 0), Color.Aqua);
		Renderer2D.DrawLine(new Vector3(Window.ClientSize.X / 2, Window.ClientSize.Y / 2 - crosshair_lenght / 2, 0), new Vector3(Window.ClientSize.X / 2, Window.ClientSize.Y / 2 + crosshair_lenght / 2, 0), Color.Aqua);
		Renderer2D.End();

		{
			if(show_grid) {
				ImGuizmo.DrawGrid(ref view_matrix, ref projection_matrix, ref identity, 200f);
			}

			ImGuizmo.DrawCubes(ref view_matrix, ref projection_matrix, ref test_cube, 1);
			ImGuizmo.Manipulate(ref view_matrix, ref projection_matrix, transform_operation, transform_mode, ref test_cube, ref delta_test_cube, ref snap);
		}

		frame_buffer.Unbind();

		if(use_framebuffer) {
			GL.ClearColor(ClearColor);
			GL.Clear(ClearBufferMask.ColorBufferBit);

			GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, frame_buffer.ID);
			GL.FramebufferTexture2D(FramebufferTarget.ReadFramebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, frame_buffer.ColorTexture.ID, 0);

			Input.OnKeyPress(Keys.F12, () => {
				var pixels = new byte[Window.ClientSize.X * Window.ClientSize.Y * 3];
				GL.ReadPixels(0, 0, Window.ClientSize.X, Window.ClientSize.Y, PixelFormat.Rgb, PixelType.UnsignedByte, pixels);
				var tex = new TextureData(Window.ClientSize.X, Window.ClientSize.Y, pixels, 3);
				tex.Save(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), $"{Guid.NewGuid()}.vif"));
			});

			GL.BlitFramebuffer(0, 0, Window.ClientSize.X, Window.ClientSize.Y, 0, 0, Window.ClientSize.X, Window.ClientSize.Y, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);
		}

		#region ImGUI

		ImGui.Begin("Sidebar");
		{
			ImGui.SliderInt("Quads", ref quads_count, 3, 100);

			ImGui.Separator();

			ImGui.SliderFloat("Axis", ref axis_lenght, 0, 50);

			ImGui.Separator();

			ImGui.SliderFloat("Crosshair", ref crosshair_lenght, 0, 50);

			ImGui.Separator();

			ImGui.InputFloat4("Cube", ref test_cube.Row3);
			ImGui.Separator();

			ImGui.Checkbox("Grid", ref show_grid);
			ImGui.Checkbox("Framebuffer", ref use_framebuffer);
			ImGui.End();
		}

		ImGui.End();

		ImGui.SetNextWindowPos(Vector2.Zero);
		ImGui.SetNextWindowBgAlpha(0.35f);
		ImGui.Begin("Overlay", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoInputs);
		{
			ImGui.SetWindowSize(new Vector2(250, 240));

			ImGui.Text($"FPS: {1 / frame_event.Time:00} | FT: {frame_event.Time * 1000:N}");
			ImGui.Separator();

			ImGui.Text($"Camera: {active_camera.Position.X:F2} {active_camera.Position.Y:F2} {active_camera.Position.Z:F2}");
			ImGui.Separator();

			if(ImGui.IsMousePosValid()) {
				ImGui.Text($"Mouse Position: ({Input.MousePosition.X}, {Input.MousePosition.Y})");
			}

			ImGui.Separator();
			ImGui.Text($"Controls:");
			ImGui.Text($"WASD - move");
			ImGui.Text($"Space - up");
			ImGui.Text($"Left Shift - down");
			ImGui.Text($"Tab - toggle camera");
			ImGui.Text($"~ - toggle focus");
			ImGui.Text($"1 - translate");
			ImGui.Text($"2 - rotate");
			ImGui.Text($"3 - scale");
			ImGui.Text($"Left Alt - snapping");
		}

		ImGui.End();

		//ImGui.ShowDemoWindow();

		imgui_controller.Render();

		#endregion
	}
}
