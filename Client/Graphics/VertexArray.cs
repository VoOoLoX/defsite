using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace Client {
	public class VertexArray {
		public VertexArray() {
			GL.CreateVertexArrays(1, out int buffer);
			ID = buffer;
		}

		public int ID { get; }
		List<IVertexBuffer> vertex_buffers = new List<IVertexBuffer>();

		public void AddVertexBuffer<T>(VertexBuffer<T> buffer, int id, int stride = 0, int offset = 0) {
			Enable();
			buffer.Enable();
			
			vertex_buffers.Add(buffer);
			
			EnableAttributeArray(id);
			if (stride != 0)
				GL.VertexAttribPointer(id, buffer.Dimensions(), VertexAttribPointerType.Float, false, stride * sizeof(float), offset);
			else
				GL.VertexAttribPointer(id, buffer.Dimensions(), VertexAttribPointerType.Float, false, buffer.Dimensions() * sizeof(float), offset);
			
			DisableAttributeArray();
			buffer.Disable();
			Disable();
		}

		void EnableAttributeArray(int id = 0) {
			GL.EnableVertexAttribArray(id);
		}

		void DisableAttributeArray() {
			GL.EnableVertexAttribArray(0);
		}

		public void Enable() {
			GL.BindVertexArray(ID);
		}

		public void Disable() {
			GL.BindVertexArray(0);
		}

		~VertexArray() {
			// GL.DeleteVertexArray(ID);
		}
	}
}