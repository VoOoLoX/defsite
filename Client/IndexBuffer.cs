using System;
using OpenTK.Graphics.OpenGL;

namespace Client {
	public class IndexBuffer {
		public int Count { get; private set; }
		public int ID { get; private set; }

		public IndexBuffer(uint[] data) {
			ID = GL.GenBuffer();
			Enable();
			Count = data.Length;
			GL.BufferData(BufferTarget.ElementArrayBuffer, data.Length * sizeof(uint), data, BufferUsageHint.StaticDraw);
			Disable();
		}

		public void Enable() => GL.BindBuffer(BufferTarget.ElementArrayBuffer, ID);
		public void Disable() => GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
	}
}
