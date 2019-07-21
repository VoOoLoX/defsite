using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Client {
	public interface IVertexBuffer {
	}

	public class VertexBuffer<T> : IVertexBuffer {
		public VertexBuffer(T[] data) {
			ID = GL.GenBuffer();
			Enable();
			SetData(data);
			Disable();
		}

		public int ID { get; }

		public void Enable() {
			GL.BindBuffer(BufferTarget.ArrayBuffer, ID);
		}

		public void Disable() {
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
		}

		public void SetData(T[] data) {
			Enable();
			GL.InvalidateBufferData(ID);
			switch (data) {
				case Vector2[] v2:
					GL.BufferData(BufferTarget.ArrayBuffer, data.Length * Vector2.SizeInBytes, v2, BufferUsageHint.StaticDraw);
					break;
				case Vector3[] v3:
					GL.BufferData(BufferTarget.ArrayBuffer, data.Length * Vector3.SizeInBytes, v3, BufferUsageHint.StaticDraw);
					break;
				case Vector4[] v4:
					GL.BufferData(BufferTarget.ArrayBuffer, data.Length * Vector4.SizeInBytes, v4, BufferUsageHint.StaticDraw);
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

		public void Dispose() {
			GL.DeleteBuffer(ID);
		}
	}
}