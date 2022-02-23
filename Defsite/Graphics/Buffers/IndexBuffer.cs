using System;

using OpenTK.Graphics.OpenGL4;

namespace Defsite.Graphics;

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
		Enable();
		SetData(data);
		Disable();
	}

	public IndexBuffer(int[] data) {
		ID = GL.GenBuffer();
		Enable();
		SetData(data);
		Disable();
	}

	public void SetData(uint[] data) {
		Enable();

		Count = data.Length;
		GL.BufferData(BufferTarget.ElementArrayBuffer, data.Length * sizeof(uint), data, BufferUsageHint.DynamicDraw);

		Disable();
	}

	public void SetData(int[] data) {
		Enable();

		Count = data.Length;
		GL.BufferData(BufferTarget.ElementArrayBuffer, data.Length * sizeof(int), data, BufferUsageHint.DynamicDraw);

		Disable();
	}

	public void UpdateData(int size, IntPtr data, int offset = 0) {
		Enable();

		GL.BufferSubData(BufferTarget.ElementArrayBuffer, (IntPtr)offset, size, data);

		Disable();
	}

	void Resize(int size) {
		Enable();

		GL.BufferData(BufferTarget.ElementArrayBuffer, size, IntPtr.Zero, BufferUsageHint.DynamicDraw);

		Disable();
	}

	public void Enable() => GL.BindBuffer(BufferTarget.ElementArrayBuffer, ID);

	static void Disable() => GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

	public void Dispose() {
		GL.DeleteBuffer(ID);
		GC.SuppressFinalize(this);
	}
}
