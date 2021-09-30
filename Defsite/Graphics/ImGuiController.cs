using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Defsite.IO;
using Defsite.IO.DataFormats;

using ImGuiNET;

using ImGuizmoNET;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Defsite.Graphics;
public class ImGuiController {
	readonly List<char> pressed_chars = new();
	BufferLayout buffer_layout;

	Texture font_texture;
	IndexBuffer index_buffer;
	int index_buffer_size;
	bool run;

	readonly Vector2 scale_factor = Vector2.One;

	Shader shader;

	VertexArray vertex_array;
	VertexBuffer vertex_buffer;

	int vertex_buffer_size;
	int window_height;

	int window_width;

	public ImGuiController(int width, int height) {
		window_width = width;
		window_height = height;

		var context = ImGui.CreateContext();

		ImGui.SetCurrentContext(context);
		ImGuizmo.SetImGuiContext(context);
		//ImNodes.Initialize();
		//ImNodes.SetImGuiContext(context);
		//ImNodes.EditorContextSet(ImNodes.EditorContextCreate());

		var io = ImGui.GetIO();

		io.Fonts.AddFontDefault();
		io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset | ImGuiBackendFlags.HasMouseCursors;
		io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
		io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;

		//ImGuiPlatformIO
		//ImGui.GetPlatformIO().pla
		// io.ConfigDockingAlwaysTabBar = true;

		CreateDeviceResources();
		SetKeyMappings();

		SetPerFrameImGuiData(1f / 60f);

		ImGui.NewFrame();
		run = true;
	}

	public void WindowResized(int width, int height) {
		window_width = width;
		window_height = height;
	}

	public void CreateDeviceResources() {
		// GLUtils.CheckGLError();

		shader = Assets.Get<Shader>("ImGuiShader");

		buffer_layout = new BufferLayout(new List<VertexAttribute> {
				new(shader.GetAttributeLocation("v_position"), VertexAttributeType.Vector2),
				new(shader.GetAttributeLocation("v_texture_coordinates"), VertexAttributeType.Vector2),
				new(shader.GetAttributeLocation("v_color"), VertexAttributeType.Vector4b, true)
			});

		vertex_array = new VertexArray();

		vertex_buffer_size = 10000;

		index_buffer_size = 2000;

		vertex_buffer = new VertexBuffer() {
			Layout = buffer_layout,
			Size = vertex_buffer_size
		};


		index_buffer = new IndexBuffer() {
			Size = index_buffer_size
		};

		vertex_array.AddVertexBuffer(vertex_buffer);

		RecreateFontDeviceTexture();
	}

	public void RecreateFontDeviceTexture() {
		var io = ImGui.GetIO();

		io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out var width, out var height, out var bytes_per_pixel);

		var texture_file = new TextureData((ushort)width, (ushort)height, pixels, width * height * bytes_per_pixel);

		font_texture = new Texture(texture_file);

		io.Fonts.SetTexID((IntPtr)font_texture.ID);

		io.Fonts.ClearTexData();
	}

	public void Render() {
		if(run) {
			run = false;
			ImGui.Render();
			RenderImDrawData(ImGui.GetDrawData());
		}
	}

	public void Update(GameWindow wnd, float delta_time) {
		if(run) {
			ImGui.Render();
		}

		SetPerFrameImGuiData(delta_time);
		UpdateImGuiInput(wnd);

		run = true;
		ImGui.NewFrame();
	}

	void SetPerFrameImGuiData(float delta_time) {
		var io = ImGui.GetIO();
		io.DisplaySize = new Vector2(
			window_width / scale_factor.X,
			window_height / scale_factor.Y);
		io.DisplayFramebufferScale = scale_factor;
		io.DeltaTime = delta_time; // DeltaTime is in seconds.
	}

	void UpdateImGuiInput(GameWindow window) {
		var io = ImGui.GetIO();

		var mouse_state = window.MouseState;
		var keyboard_state = window.KeyboardState;

		io.MouseDown[0] = mouse_state[MouseButton.Left];
		io.MouseDown[1] = mouse_state[MouseButton.Right];
		io.MouseDown[2] = mouse_state[MouseButton.Middle];

		var screen_point = new Vector2i((int)mouse_state.X, (int)mouse_state.Y);
		var point = screen_point;
		io.MousePos = new Vector2(point.X, point.Y);
		//TODO FIX Memory leak???
		foreach(Keys key in Enum.GetValues(typeof(Keys))) {
			if(key == Keys.Unknown) {
				continue;
			}

			io.KeysDown[(int)key] = keyboard_state.IsKeyDown(key);
		}

		foreach(var c in pressed_chars) {
			io.AddInputCharacter(c);
		}

		pressed_chars.Clear();

		io.KeyCtrl = keyboard_state.IsKeyDown(Keys.LeftControl) || keyboard_state.IsKeyDown(Keys.RightControl);
		io.KeyAlt = keyboard_state.IsKeyDown(Keys.LeftAlt) || keyboard_state.IsKeyDown(Keys.RightAlt);
		io.KeyShift = keyboard_state.IsKeyDown(Keys.LeftShift) || keyboard_state.IsKeyDown(Keys.RightShift);
		io.KeySuper = keyboard_state.IsKeyDown(Keys.LeftSuper) || keyboard_state.IsKeyDown(Keys.RightSuper);
	}

	internal void PressChar(char key_char) => pressed_chars.Add(key_char);

	internal void MouseScroll(OpenTK.Mathematics.Vector2 offset) {
		var io = ImGui.GetIO();

		io.MouseWheel = offset.Y;
		io.MouseWheelH = offset.X;
	}

	static void SetKeyMappings() {
		var io = ImGui.GetIO();
		io.KeyMap[(int)ImGuiKey.Tab] = (int)Keys.Tab;
		io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Keys.Left;
		io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Keys.Right;
		io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Keys.Up;
		io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Keys.Down;
		io.KeyMap[(int)ImGuiKey.PageUp] = (int)Keys.PageUp;
		io.KeyMap[(int)ImGuiKey.PageDown] = (int)Keys.PageDown;
		io.KeyMap[(int)ImGuiKey.Home] = (int)Keys.Home;
		io.KeyMap[(int)ImGuiKey.End] = (int)Keys.End;
		io.KeyMap[(int)ImGuiKey.Delete] = (int)Keys.Delete;
		io.KeyMap[(int)ImGuiKey.Backspace] = (int)Keys.Backspace;
		io.KeyMap[(int)ImGuiKey.Enter] = (int)Keys.Enter;
		io.KeyMap[(int)ImGuiKey.Escape] = (int)Keys.Escape;
		io.KeyMap[(int)ImGuiKey.A] = (int)Keys.A;
		io.KeyMap[(int)ImGuiKey.C] = (int)Keys.C;
		io.KeyMap[(int)ImGuiKey.V] = (int)Keys.V;
		io.KeyMap[(int)ImGuiKey.X] = (int)Keys.X;
		io.KeyMap[(int)ImGuiKey.Y] = (int)Keys.Y;
		io.KeyMap[(int)ImGuiKey.Z] = (int)Keys.Z;
	}

	void RenderImDrawData(ImDrawDataPtr draw_data) {
		if(draw_data.CmdListsCount == 0) {
			return;
		}

		for(var i = 0; i < draw_data.CmdListsCount; i++) {
			var cmd_list = draw_data.CmdListsRange[i];

			var vertex_size = cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>();
			if(vertex_size > vertex_buffer_size) {
				var new_size = (int)Math.Max(vertex_buffer_size * 1.5f, vertex_size);
				vertex_buffer.Size = new_size;
				vertex_buffer_size = new_size;

				// Log.Info($"Resized dear imgui vertex buffer to new size {vertex_buffer_size}");
			}

			var index_size = cmd_list.IdxBuffer.Size * sizeof(ushort);
			if(index_size > index_buffer_size) {
				var new_size = (int)Math.Max(index_buffer_size * 1.5f, index_size);
				index_buffer.Size = new_size;
				index_buffer_size = new_size;

				// Log.Info($"Resized dear imgui index buffer to new size {index_buffer_size}");
			}
		}

		// Setup orthographic projection matrix into our constant buffer
		var io = ImGui.GetIO();
		var mvp = Matrix4.CreateOrthographicOffCenter(
			0.0f,
			io.DisplaySize.X,
			io.DisplaySize.Y,
			0.0f,
			-1.0f,
			1.0f);

		shader.Enable();
		shader.Set("u_projection", mvp);
		shader.Set("u_font_texture", 0); // 0 = Texture0 (texture slot)

		vertex_array.Enable();

		draw_data.ScaleClipRects(io.DisplayFramebufferScale);

		GL.Enable(EnableCap.Blend);
		GL.Enable(EnableCap.ScissorTest);
		GL.BlendEquation(BlendEquationMode.FuncAdd);
		GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
		GL.Disable(EnableCap.CullFace);
		GL.Disable(EnableCap.DepthTest);

		for(var n = 0; n < draw_data.CmdListsCount; n++) {
			var cmd_list = draw_data.CmdListsRange[n];

			vertex_buffer.UpdateData(cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>(), cmd_list.VtxBuffer.Data);

			index_buffer.UpdateData(cmd_list.IdxBuffer.Size * sizeof(ushort), cmd_list.IdxBuffer.Data);

			var vtx_offset = 0;
			var idx_offset = 0;

			index_buffer.Enable();

			for(var cmd_i = 0; cmd_i < cmd_list.CmdBuffer.Size; cmd_i++) {
				var p_cmd = cmd_list.CmdBuffer[cmd_i];

				if(p_cmd.UserCallback != IntPtr.Zero) {
					throw new NotImplementedException();
				}

				GL.ActiveTexture(TextureUnit.Texture0);
				GL.BindTexture(TextureTarget.Texture2D, (int)p_cmd.TextureId);

				var clip = p_cmd.ClipRect;
				GL.Scissor((int)clip.X, window_height - (int)clip.W, (int)(clip.Z - clip.X), (int)(clip.W - clip.Y));

				if((io.BackendFlags & ImGuiBackendFlags.RendererHasVtxOffset) != 0) {
					GL.DrawElementsBaseVertex(PrimitiveType.Triangles, (int)p_cmd.ElemCount, DrawElementsType.UnsignedShort, (IntPtr)(idx_offset * sizeof(ushort)), vtx_offset);
				} else {
					GL.DrawElements(BeginMode.Triangles, (int)p_cmd.ElemCount, DrawElementsType.UnsignedShort, (int)p_cmd.IdxOffset * sizeof(ushort));
				}

				idx_offset += (int)p_cmd.ElemCount;
			}

			vtx_offset += cmd_list.VtxBuffer.Size;
		}

		GL.ActiveTexture(TextureUnit.Texture0);
		GL.BindTexture(TextureTarget.Texture2D, 0);

		GL.Disable(EnableCap.Blend);
		GL.Disable(EnableCap.ScissorTest);

		//vertex_array.Disable();
	}
}
