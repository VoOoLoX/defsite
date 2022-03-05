using System;
using Defsite.Graphics.VertexTypes;
using OpenTK.Graphics.OpenGL4;

namespace Defsite.Graphics.Buffers;

public class VertexBuffer : IDisposable {
	public int ID { get; }

	int size = 0;
	public int Size {
		get => size;
		set {
			size = value;
			Resize(value);
		}
	}

	public BufferLayout Layout { get; set; }

	public VertexBuffer() => ID = GL.GenBuffer();

	public void SetData<T>(T[] data) where T : IVertex {
		Bind();

		switch(data) {
			case ColoredVertex[] colored_vertices:
				GL.BufferData(BufferTarget.ArrayBuffer, data.Length * colored_vertices[0].SizeInBytes, colored_vertices, BufferUsageHint.DynamicDraw);
				break;

			case TexturedVertex[] textured_vertices:
				GL.BufferData(BufferTarget.ArrayBuffer, data.Length * textured_vertices[0].SizeInBytes, textured_vertices, BufferUsageHint.DynamicDraw);
				break;

			default:
				break;
		}

		Unbind();
	}

	public void SetData<T>(T[] data, int count) where T : IVertex {
		Bind();

		switch(data) {
			case ColoredVertex[] colored_vertices:
				GL.BufferData(BufferTarget.ArrayBuffer, count * colored_vertices[0].SizeInBytes, colored_vertices, BufferUsageHint.DynamicDraw);
				break;

			case TexturedVertex[] textured_vertices:
				GL.BufferData(BufferTarget.ArrayBuffer, count * textured_vertices[0].SizeInBytes, textured_vertices, BufferUsageHint.DynamicDraw);
				break;

			default:
				break;
		}

		Unbind();
	}

	public void UpdateData<T>(T[] data, int offset = 0) where T : IVertex {
		Bind();

		var data_size = data.Length * data[0].SizeInBytes;
		if(data_size < size) {
			Resize(data_size);
		}

		switch(data) {
			case ColoredVertex[] colored_vertices:
				GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)offset, data.Length * colored_vertices[0].SizeInBytes, colored_vertices);
				break;

			case TexturedVertex[] textured_vertices:
				GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)offset, data.Length * textured_vertices[0].SizeInBytes, textured_vertices);
				break;

			default:
				break;
		}

		Unbind();
	}

	public void UpdateData(int size, IntPtr data, int offset = 0) {
		Bind();

		GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)offset, size, data);

		Unbind();
	}

	void Resize(int size_in_bytes) {
		Bind();

		GL.BufferData(BufferTarget.ArrayBuffer, size_in_bytes, IntPtr.Zero, BufferUsageHint.DynamicDraw);

		Unbind();
	}

	public void Bind() => GL.BindBuffer(BufferTarget.ArrayBuffer, ID);

	static void Unbind() => GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

	public void Dispose() {
		GL.DeleteBuffer(ID);
		GC.SuppressFinalize(this);
	}
}
