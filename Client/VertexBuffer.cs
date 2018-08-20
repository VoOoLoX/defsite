using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace Client {
	public class VertexBuffer<T> {
		public int ID { get; private set; }

		public VertexBuffer(T[] data) {
			ID = GL.GenBuffer();
			Enable();
			if (typeof(T) == typeof(Vector2))
				GL.BufferData(BufferTarget.ArrayBuffer, data.Length * Vector2.SizeInBytes, (Vector2[])Convert.ChangeType(data, typeof(Vector2[])), BufferUsageHint.DynamicDraw);
			if (typeof(T) == typeof(Vector3))
				GL.BufferData(BufferTarget.ArrayBuffer, data.Length * Vector3.SizeInBytes, (Vector3[])Convert.ChangeType(data, typeof(Vector3[])), BufferUsageHint.DynamicDraw);
			if (typeof(T) == typeof(Vector4))
				GL.BufferData(BufferTarget.ArrayBuffer, data.Length * Vector4.SizeInBytes, (Vector4[])Convert.ChangeType(data, typeof(Vector3[])), BufferUsageHint.DynamicDraw);
			Disable();
		}

		public void Enable() => GL.BindBuffer(BufferTarget.ArrayBuffer, ID);
		public void Disable() => GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

		public void Update(T[] data) {
			Enable();
			GL.InvalidateBufferData(ID);
			if (typeof(T) == typeof(Vector2))
				GL.BufferData(BufferTarget.ArrayBuffer, data.Length * Vector2.SizeInBytes, (Vector2[])Convert.ChangeType(data, typeof(Vector2[])), BufferUsageHint.DynamicDraw);
			if (typeof(T) == typeof(Vector3))
				GL.BufferData(BufferTarget.ArrayBuffer, data.Length * Vector3.SizeInBytes, (Vector3[])Convert.ChangeType(data, typeof(Vector3[])), BufferUsageHint.DynamicDraw);
			if (typeof(T) == typeof(Vector4))
				GL.BufferData(BufferTarget.ArrayBuffer, data.Length * Vector4.SizeInBytes, (Vector4[])Convert.ChangeType(data, typeof(Vector3[])), BufferUsageHint.DynamicDraw);
			Disable();
		}
	}
}