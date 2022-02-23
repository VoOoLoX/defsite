using System;

using OpenTK.Graphics.OpenGL4;

namespace Defsite.Graphics;

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
		Enable();

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

		Disable();
	}

	public void SetData<T>(T[] data, int count) where T : IVertex {
		Enable();

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

		Disable();
	}

	public void UpdateData<T>(T[] data, int offset = 0) where T : IVertex {
		Enable();

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

		Disable();
	}

	public void UpdateData(int size, IntPtr data, int offset = 0) {
		Enable();

		GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)offset, size, data);

		Disable();
	}

	void Resize(int size_in_bytes) {
		Enable();

		GL.BufferData(BufferTarget.ArrayBuffer, size_in_bytes, IntPtr.Zero, BufferUsageHint.DynamicDraw);

		Disable();
	}

	public void Enable() => GL.BindBuffer(BufferTarget.ArrayBuffer, ID);

	static void Disable() => GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

	public void Dispose() {
		GL.DeleteBuffer(ID);
		GC.SuppressFinalize(this);
	}
}
