using System;
using System.Collections.Generic;
using System.Drawing;
using Defsite.Graphics.Cameras;
using Defsite.IO;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Defsite.Graphics;

public static unsafe class Renderer2D {
	const int max_quads = 10000;
	const int max_verticies = max_quads * 4;
	const int max_indices = max_quads * 6;

	static int quad_vertices_count = 0;
	static int quad_indices_count = 0;

	static int line_vertices_count = 0;

	static VertexArray line_vertex_array, quad_vertex_array;
	static VertexBuffer line_vertex_buffer, quad_vertex_buffer;
	static IndexBuffer quad_index_buffer;
	static BufferLayout line_buffer_layout, quad_buffer_layout;

	static Shader color_shader, texture_shader;

	static TexturedVertex[] quad_vertices;
	static ColoredVertex[] line_vertices;

	static ICamera camera_data;

	static float line_width = 1.0f;
	public static float LineWidth {
		get => line_width;
		set {
			line_width = value;
			GL.LineWidth(value);
		}
	}

	public static void Init() {
		//Quads
		{
			quad_vertices = new TexturedVertex[max_verticies];

			quad_vertex_array = new VertexArray();

			texture_shader = Assets.Get<Shader>("TextureShader");

			quad_buffer_layout = new BufferLayout(new List<VertexAttribute> {
				new(texture_shader["v_position"], VertexAttributeType.Vector4),
				new(texture_shader["v_color"], VertexAttributeType.Vector4),
				new(texture_shader["v_texture_coordinates"], VertexAttributeType.Vector2)
			});

			quad_vertex_buffer = new VertexBuffer() {
				Layout = quad_buffer_layout,
				Size = max_verticies
			};

			quad_index_buffer = new IndexBuffer();

			var indices = new int[max_indices];
			var offset = 0;

			for(var i = 0; i < max_indices; i += 6) {
				indices[i + 0] = offset + 0;
				indices[i + 1] = offset + 1;
				indices[i + 2] = offset + 2;

				indices[i + 3] = offset + 2;
				indices[i + 4] = offset + 3;
				indices[i + 5] = offset + 0;
				offset += 4;
			}

			quad_index_buffer.SetData(indices);

			quad_vertex_array.AddVertexBuffer(quad_vertex_buffer);
		}

		//Lines
		{
			line_vertices = new ColoredVertex[max_verticies];

			line_vertex_array = new VertexArray();

			color_shader = Assets.Get<Shader>("ColorShader");

			line_buffer_layout = new BufferLayout(new List<VertexAttribute> {
				new(color_shader["v_position"], VertexAttributeType.Vector4),
				new(color_shader["v_color"], VertexAttributeType.Vector4)
			});

			line_vertex_buffer = new VertexBuffer() {
				Layout = line_buffer_layout,
				Size = max_verticies
			};

			line_vertex_array.AddVertexBuffer(line_vertex_buffer);
		}
	}

	public static void Begin(ICamera camera) {
		camera_data = camera;
		StartBatch();
	}

	static void StartBatch() {
		quad_indices_count = 0;
		quad_vertices_count = 0;

		line_vertices_count = 0;

		Array.Clear(quad_vertices);
		Array.Clear(line_vertices);

		LineWidth = 1;
	}

	public static void DrawQuad(Vector3 position, Vector2 size = default, Color color = default, bool centered = true, Matrix4 transform = default) {
		var data = Primitives.CreateTexturedQuad(position, size, color, centered, transform);

		if(quad_vertices_count >= max_verticies) {
			Flush();
			StartBatch();
		}

		for(var i = 0; i < data.Length; i++) {
			quad_vertices[quad_vertices_count++] = data[i];
		}

		quad_indices_count += 6;
	}

	public static void DrawTile(Vector3 position, Vector2 size = default, Color color = default, bool centered = true, Matrix4 transform = default) {
		var data = Primitives.CreateTexturedTile(position, size, color, centered, transform);

		if(quad_vertices_count >= max_verticies) {
			Flush();
			StartBatch();
		}

		for(var i = 0; i < data.Length; i++) {
			quad_vertices[quad_vertices_count++] = data[i];
		}

		quad_indices_count += 6;
	}

	public static void DrawLine(Vector3 start_position, Vector3 end_position, Color color = default) {
		var data = Primitives.CreateLine(start_position, end_position, color);

		if(line_vertices_count >= max_verticies) {
			Flush();
			StartBatch();
		}

		for(var i = 0; i < data.Length; i++) {
			line_vertices[line_vertices_count++] = data[i];
		}
	}

	public static void DrawLine(Vector3 start_position, Vector3 end_position, Color start_color = default, Color end_color = default) {
		var data = Primitives.CreateLine(start_position, end_position, start_color, end_color);

		if(line_vertices_count >= max_verticies) {
			Flush();
			StartBatch();
		}

		for(var i = 0; i < data.Length; i++) {
			line_vertices[line_vertices_count++] = data[i];
		}
	}

	static void Flush() {
		if(quad_vertices_count > 0) {
			quad_vertex_array.Enable();

			quad_vertex_buffer.SetData(quad_vertices, quad_vertices_count);

			quad_index_buffer.Enable();

			//TODO(VoOoLoX): set texture slots for multiple textures
			Texture.Default.Enable();

			texture_shader.Enable();
			texture_shader.Set("u_projection", camera_data.ProjectionMatrix);
			texture_shader.Set("u_view", camera_data.ViewMatrix);
			texture_shader.Set("u_model", Matrix4.Identity);

			GL.DrawElements(PrimitiveType.Triangles, quad_indices_count, DrawElementsType.UnsignedInt, 0);
		}

		if(line_vertices_count > 0) {
			line_vertex_array.Enable();

			line_vertex_buffer.SetData(line_vertices, line_vertices_count);

			color_shader.Enable();
			color_shader.Set("u_projection", camera_data.ProjectionMatrix);
			color_shader.Set("u_view", camera_data.ViewMatrix);
			color_shader.Set("u_model", Matrix4.Identity);

			GL.DrawArrays(PrimitiveType.Lines, 0, line_vertices_count);
		}
	}

	public static void End() => Flush();
}
