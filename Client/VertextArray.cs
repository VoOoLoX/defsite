using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Client {
	public class VertextArray {
		public int ID { get; private set; }

		public VertextArray() {
			ID = GL.GenVertexArray();
		}

		public void AddBuffer<T>(VertexBuffer<T> buffer, int id, int stride, int offset) {
			Enable();
			buffer.Enable();
			EnableBuffer(id);
			if (typeof(T) == typeof(Vector2))
				GL.VertexAttribPointer(id, 2, VertexAttribPointerType.Float, false, stride * sizeof(float), offset);
			if (typeof(T) == typeof(Vector3))
				GL.VertexAttribPointer(id, 3, VertexAttribPointerType.Float, false, stride * sizeof(float), offset);
			if (typeof(T) == typeof(Vector4))
				GL.VertexAttribPointer(id, 4, VertexAttribPointerType.Float, false, stride * sizeof(float), offset);
			DisableBuffer();
			buffer.Disable();
			Disable();
		}

		public void EnableBuffer(int id = 0) => GL.EnableVertexAttribArray(id);
		public void DisableBuffer() => GL.EnableVertexAttribArray(0);

		public void Enable() => GL.BindVertexArray(ID);
		public void Disable() => GL.BindVertexArray(0);
	}
}