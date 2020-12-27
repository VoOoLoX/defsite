using System;
using OpenTK.Graphics.OpenGL4;

namespace Defsite {

	public class IndexBuffer {

		public int Count { get; private set; }

		public int ID { get; }

		public IndexBuffer(int size) {
			ID = GL.GenBuffer();
			Enable();
			SetData(size);
			Disable();
		}

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
		public void Disable() => GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

		public void Dispose() => GL.DeleteBuffer(ID);

		public void Enable() => GL.BindBuffer(BufferTarget.ElementArrayBuffer, ID);

		public void SetSubData(int size, IntPtr data, int offset = 0) {
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, ID);

			GL.BufferSubData(BufferTarget.ElementArrayBuffer, (IntPtr)offset, size, data);
			//GL.BufferData(BufferTarget.ElementArrayBuffer, size, data, BufferUsageHint.DynamicDraw);

			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
		}

		public void SetData(int size) {
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, ID);

			GL.BufferData(BufferTarget.ElementArrayBuffer, size, IntPtr.Zero, BufferUsageHint.DynamicDraw);

			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
		}

		public void SetData(uint[] data) {
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, ID);

			Count = data.Length;
			GL.BufferData(BufferTarget.ElementArrayBuffer, data.Length * sizeof(uint), data, BufferUsageHint.DynamicDraw);

			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
		}

		public void SetData(int[] data) {
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, ID);

			Count = data.Length;
			GL.BufferData(BufferTarget.ElementArrayBuffer, data.Length * sizeof(int), data, BufferUsageHint.DynamicDraw);

			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
		}
	}
}