using OpenTK.Graphics.OpenGL;

namespace Client {
	public class VertextArray {
		public VertextArray() {
			GL.CreateVertexArrays(1, out int buffer);
			ID = buffer;
		}

		public int ID { get; }

		public void AddBuffer<T>(VertexBuffer<T> buffer, int id, int stride, int offset = 0) {
			Enable();
			buffer.Enable();
			EnableAttribArray(id);
			GL.VertexAttribPointer(id, buffer.Dimensions(), VertexAttribPointerType.Float, false, stride * sizeof(float), offset);
			DisableAttribArray();
			buffer.Disable();
			Disable();
		}

		public void EnableAttribArray(int id = 0) {
			GL.EnableVertexAttribArray(id);
		}

		public void DisableAttribArray() {
			GL.EnableVertexAttribArray(0);
		}

		public void Enable() {
			GL.BindVertexArray(ID);
		}

		public void Disable() {
			GL.BindVertexArray(0);
		}

		~VertextArray() {
			// GL.DeleteVertexArray(ID);
		}
	}
}