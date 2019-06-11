using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace Client {
	public class VertexBuffer<T> {
		public int ID { get; private set; }

		public VertexBuffer(T[] data) {
			GL.CreateBuffers(1, out int buffer);
			ID = buffer;
			Enable();
			Update(data);
			Disable();
		}

		public void Enable() => GL.BindBuffer(BufferTarget.ArrayBuffer, ID);
		public void Disable() => GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

		public void Update(T[] data) {
			Enable();
			GL.InvalidateBufferData(ID);
			switch (data) {
				case Vector2[] v2:
					GL.BufferData(BufferTarget.ArrayBuffer, data.Length * Vector2.SizeInBytes, v2, BufferUsageHint.StaticDraw);
					break;
				case Vector3[] v3:
					GL.BufferData(BufferTarget.ArrayBuffer, data.Length * Vector2.SizeInBytes, v3, BufferUsageHint.StaticDraw);
					break;
				case Vector4[] v4:
					GL.BufferData(BufferTarget.ArrayBuffer, data.Length * Vector2.SizeInBytes, v4, BufferUsageHint.StaticDraw);
					break;
			}
			Disable();
		}

		public int Dimensions() {
			switch (this) {
				case VertexBuffer<Vector2> _:
					return 2;
				case VertexBuffer<Vector3> _:
					return 3;
				case VertexBuffer<Vector4> _:
					return 4;
				default: return 0;
			}
		}

		~VertexBuffer() {
			GL.DeleteBuffer(ID);
		}
	}
}