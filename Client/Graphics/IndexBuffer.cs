using OpenTK.Graphics.OpenGL;

namespace Client {
	public class IndexBuffer {
		public IndexBuffer(uint[] data) {
			ID = GL.GenBuffer();
			Enable();
			Count = data.Length;
			GL.BufferData(BufferTarget.ElementArrayBuffer, data.Length * sizeof(uint), data, BufferUsageHint.StaticDraw);
			Disable();
		}

		public int Count { get; }
		public int ID { get; }

		public void Enable() {
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, ID);
		}

		public void Disable() {
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
		}

		~IndexBuffer() {
			// GL.DeleteBuffer(ID);
		}
	}
}