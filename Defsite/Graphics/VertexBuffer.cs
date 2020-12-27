using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Defsite {

	public class VertexBuffer {
		public int ID { get; }
		public BufferLayout Layout { get; set; }

		public VertexBuffer(int size) {
			ID = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, ID);
			SetData(size);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
		}

		public VertexBuffer(Vertex[] data) {
			ID = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, ID);
			SetData(data);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
		}

		public void Disable() => GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

		public void Enable() => GL.BindBuffer(BufferTarget.ArrayBuffer, ID);

		public void SetSubData(int size, IntPtr data, int offset = 0) {
			GL.BindBuffer(BufferTarget.ArrayBuffer, ID);

			GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)offset, size, data);
			//GL.BufferData(BufferTarget.ArrayBuffer, size, data, BufferUsageHint.DynamicDraw);

			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
		}

		public void SetData(int size) {
			GL.BindBuffer(BufferTarget.ArrayBuffer, ID);

			GL.BufferData(BufferTarget.ArrayBuffer, size, IntPtr.Zero, BufferUsageHint.DynamicDraw);

			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
		}

		public void SetData(Vertex[] data) {
			GL.BindBuffer(BufferTarget.ArrayBuffer, ID);

			GL.BufferData(BufferTarget.ArrayBuffer, data.Length * Vertex.SizeInBytes, data, BufferUsageHint.DynamicDraw);

			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
		}
	}

	public class VertexBuffer<T> {
		public int ID { get; }

		public VertexBuffer(T[] data) {
			ID = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, ID);
			SetData(data);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
		}

		public int Dimensions => (this) switch
		{
			VertexBuffer<Vector2> _ => 2,
			VertexBuffer<Vector3> _ => 3,
			VertexBuffer<Vector4> _ => 4,
			_ => 0,
		};

		public void Disable() => GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

		public void Dispose() => GL.DeleteBuffer(ID);

		public void Enable() => GL.BindBuffer(BufferTarget.ArrayBuffer, ID);

		public void SetData(T[] data) {
			GL.BindBuffer(BufferTarget.ArrayBuffer, ID);

			switch (data) {
				case Vector2[] vec2:
					GL.BufferData(BufferTarget.ArrayBuffer, data.Length * Vector2.SizeInBytes, vec2, BufferUsageHint.DynamicDraw);
					break;

				case Vector3[] vec3:
					GL.BufferData(BufferTarget.ArrayBuffer, data.Length * Vector3.SizeInBytes, vec3, BufferUsageHint.DynamicDraw);
					break;

				case Vector4[] vec4:
					GL.BufferData(BufferTarget.ArrayBuffer, data.Length * Vector4.SizeInBytes, vec4, BufferUsageHint.DynamicDraw);
					break;
			}

			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
		}
	}
}