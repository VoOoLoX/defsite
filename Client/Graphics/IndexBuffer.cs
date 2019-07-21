using OpenTK.Graphics.OpenGL;

namespace Client {
	public class IndexBuffer {
		public IndexBuffer(uint[] data) {
			ID = GL.GenBuffer();
			Enable();
			SetData(data);
			Disable();
		}

		public int Count { get; private set; }
		public int ID { get; }

		public void Enable() {
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, ID);
		}

		public void Disable() {
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
		}

		public void SetData(uint[] data) {
			Enable();
			Count = data.Length;
			GL.InvalidateBufferData(ID);
			GL.BufferData(BufferTarget.ElementArrayBuffer, data.Length * sizeof(uint), data, BufferUsageHint.StaticDraw);
			Disable();
		}

		public void Dispose() {
			GL.DeleteBuffer(ID);
		}
	}
}