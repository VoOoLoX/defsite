using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Defsite.Core;
using Defsite.Graphics.Buffers;
using Defsite.IO;
using Defsite.IO.DataFormats;

using ImGuiNET;

using ImGuizmoNET;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Defsite.Graphics.Renderers;

public class ImGuiRenderer {
	Shader shader;
	Texture font_texture;

	VertexArray vertex_array;
	VertexBuffer vertex_buffer;
	IndexBuffer index_buffer;
	BufferLayout buffer_layout;

	int vertex_buffer_size;
	int index_buffer_size;

	int client_width;
	int client_height;

	bool run;

	readonly List<char> pressed_characters = new();
	readonly ImGuiIOPtr io;

	public ImGuiRenderer(int width, int height) {
		client_width = width;
		client_height = height;

		var context = ImGui.CreateContext();

		ImGui.SetCurrentContext(context);
		ImGuizmo.SetImGuiContext(context);

		io = ImGui.GetIO();

		io.Fonts.AddFontDefault();
		io.BackendFlags |= ImGuiBackendFlags.HasMouseCursors;
		io.ConfigFlags |= ImGuiConfigFlags.DockingEnable | ImGuiConfigFlags.NavEnableKeyboard;

		CreateResources();
		SetKeyMappings();

		ImGui.NewFrame();

		run = true;
	}

	public void Render() {
		if(run) {
			run = false;
			ImGui.Render();
			RenderDrawData(ImGui.GetDrawData());
		}
	}

	public void Update(float delta_time) {
		io.DisplaySize.X = client_width;
		io.DisplaySize.Y = client_height;
		io.DeltaTime = delta_time;

		UpdateInputs();

		ImGui.NewFrame();
		ImGuizmo.BeginFrame();
		ImGuizmo.SetRect(0, 0, io.DisplaySize.X, io.DisplaySize.Y);

		run = true;
	}

	public void PressChar(char char_key) => pressed_characters.Add(char_key);

	public void WindowResized(int width, int height) {
		client_width = width;
		client_height = height;
	}

	void CreateResources() {
		shader = Assets.Get<Shader>("ImGuiShader");

		buffer_layout = new BufferLayout(new List<VertexAttribute> {
			new(shader["v_position"], VertexAttributeType.Vector2),
			new(shader["v_texture_coordinates"], VertexAttributeType.Vector2),
			new(shader["v_color"], VertexAttributeType.Vector4b, true)
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

		RecreateFontTexture();
	}

	void RecreateFontTexture() {
		io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out var width, out var height, out var bytes_per_pixel);

		var texture_data = new TextureData(width, height, pixels, width * height * bytes_per_pixel, (byte)bytes_per_pixel);

		font_texture = new Texture(texture_data);

		io.Fonts.SetTexID((IntPtr)font_texture.ID);

		io.Fonts.ClearTexData();
	}

	void UpdateInputs() {
		var mouse_state = Input.MouseState;
		var keyboard_state = Input.KeyboardState;

		io.MouseDown[0] = mouse_state[MouseButton.Left];
		io.MouseDown[1] = mouse_state[MouseButton.Right];
		io.MouseDown[2] = mouse_state[MouseButton.Middle];
		io.MouseDown[3] = mouse_state[MouseButton.Button4];
		io.MouseDown[4] = mouse_state[MouseButton.Button5];

		io.MousePos.X = mouse_state.X;
		io.MousePos.Y = mouse_state.Y;

		io.MouseWheel = mouse_state.ScrollDelta.Y;
		io.MouseWheelH = mouse_state.ScrollDelta.X;

		foreach(Keys key in Enum.GetValues(typeof(Keys))) {
			if(key == Keys.Unknown) {
				continue;
			}

			io.KeysDown[(int)key] = keyboard_state.IsKeyDown(key);
		}

		foreach(var char_key in pressed_characters) {
			io.AddInputCharacter(char_key);
		}

		pressed_characters.Clear();

		io.KeyCtrl = keyboard_state.IsKeyDown(Keys.LeftControl) || keyboard_state.IsKeyDown(Keys.RightControl);
		io.KeyAlt = keyboard_state.IsKeyDown(Keys.LeftAlt) || keyboard_state.IsKeyDown(Keys.RightAlt);
		io.KeyShift = keyboard_state.IsKeyDown(Keys.LeftShift) || keyboard_state.IsKeyDown(Keys.RightShift);
		io.KeySuper = keyboard_state.IsKeyDown(Keys.LeftSuper) || keyboard_state.IsKeyDown(Keys.RightSuper);
	}

	void SetKeyMappings() {
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
		io.KeyMap[(int)ImGuiKey.KeyPadEnter] = (int)Keys.KeyPadEnter;
		io.KeyMap[(int)ImGuiKey.Escape] = (int)Keys.Escape;
		io.KeyMap[(int)ImGuiKey.Space] = (int)Keys.Space;
		io.KeyMap[(int)ImGuiKey.A] = (int)Keys.A;
		io.KeyMap[(int)ImGuiKey.C] = (int)Keys.C;
		io.KeyMap[(int)ImGuiKey.V] = (int)Keys.V;
		io.KeyMap[(int)ImGuiKey.X] = (int)Keys.X;
		io.KeyMap[(int)ImGuiKey.Y] = (int)Keys.Y;
		io.KeyMap[(int)ImGuiKey.Z] = (int)Keys.Z;
	}

	void RenderDrawData(ImDrawDataPtr draw_data) {
		if(draw_data.CmdListsCount == 0) {
			return;
		}

		var projection_matrtix = Matrix4.CreateOrthographicOffCenter(0.0f, io.DisplaySize.X, io.DisplaySize.Y, 0.0f, -1.0f, 1.0f);

		shader.Bind();
		shader.Set("u_projection", projection_matrtix);
		shader.Set("u_font_texture", 0); // 0 = Texture0 (texture slot)

		vertex_array.Bind();

		draw_data.ScaleClipRects(io.DisplayFramebufferScale);

		GL.Enable(EnableCap.Blend);
		GL.Enable(EnableCap.ScissorTest);
		GL.BlendEquation(BlendEquationMode.FuncAdd);
		GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
		GL.Disable(EnableCap.CullFace);
		GL.Disable(EnableCap.DepthTest);

		var vtx_offset = 0;
		var idx_offset = 0;

		for(var n = 0; n < draw_data.CmdListsCount; n++) {
			var cmd_list = draw_data.CmdListsRange[n];

			var vertex_size = cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>();

			if(vertex_size > vertex_buffer_size) {
				var new_size = (int)Math.Max(vertex_buffer_size * 1.5f, vertex_size);
				vertex_buffer.Size = new_size;
				vertex_buffer_size = new_size;
			}

			var index_size = cmd_list.IdxBuffer.Size * sizeof(ushort);

			if(index_size > index_buffer_size) {
				var new_size = (int)Math.Max(index_buffer_size * 1.5f, index_size);
				index_buffer.Size = new_size;
				index_buffer_size = new_size;
			}

			vertex_buffer.UpdateData(cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>(), cmd_list.VtxBuffer.Data);

			index_buffer.UpdateData(cmd_list.IdxBuffer.Size * sizeof(ushort), cmd_list.IdxBuffer.Data);

			for(var cmd_i = 0; cmd_i < cmd_list.CmdBuffer.Size; cmd_i++) {
				var p_cmd = cmd_list.CmdBuffer[cmd_i];

				if(p_cmd.UserCallback != IntPtr.Zero) {
					throw new NotImplementedException();
				}

				GL.ActiveTexture(TextureUnit.Texture0);
				GL.BindTexture(TextureTarget.Texture2D, (int)p_cmd.TextureId);

				var clip = p_cmd.ClipRect;
				GL.Scissor((int)clip.X, client_height - (int)clip.W, (int)(clip.Z - clip.X), (int)(clip.W - clip.Y));

				index_buffer.Bind();

				if(io.BackendFlags.HasFlag(ImGuiBackendFlags.RendererHasVtxOffset)) {
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
	}
}
