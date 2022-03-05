using System;

using OpenTK.Graphics.OpenGL4;

namespace Defsite.Graphics.Buffers;

public class IndexBuffer : IDisposable {
	public int ID { get; }

	int size = 0;
	public int Size {
		get => size;
		set {
			size = value;
			Resize(value);
		}
	}

	public int Count { get; private set; }

	public IndexBuffer() => ID = GL.GenBuffer();

	public IndexBuffer(uint[] data) {
		ID = GL.GenBuffer();
		Bind();
		SetData(data);
		Unbind();
	}

	public IndexBuffer(int[] data) {
		ID = GL.GenBuffer();
		Bind();
		SetData(data);
		Unbind();
	}

	public void SetData(uint[] data) {
		Bind();

		Count = data.Length;
		GL.BufferData(BufferTarget.ElementArrayBuffer, data.Length * sizeof(uint), data, BufferUsageHint.DynamicDraw);

		Unbind();
	}

	public void SetData(int[] data) {
		Bind();

		Count = data.Length;
		GL.BufferData(BufferTarget.ElementArrayBuffer, data.Length * sizeof(int), data, BufferUsageHint.DynamicDraw);

		Unbind();
	}

	public void UpdateData(int size, IntPtr data, int offset = 0) {
		Bind();

		GL.BufferSubData(BufferTarget.ElementArrayBuffer, (IntPtr)offset, size, data);

		Unbind();
	}

	void Resize(int size) {
		Bind();

		GL.BufferData(BufferTarget.ElementArrayBuffer, size, IntPtr.Zero, BufferUsageHint.DynamicDraw);

		Unbind();
	}

	public void Bind() => GL.BindBuffer(BufferTarget.ElementArrayBuffer, ID);

	static void Unbind() => GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

	public void Dispose() {
		GL.DeleteBuffer(ID);
		GC.SuppressFinalize(this);
	}
}
