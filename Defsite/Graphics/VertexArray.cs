using OpenTK.Graphics.OpenGL;

namespace Defsite {

	public class VertexArray {
		public int ID { get; }
		public IndexBuffer IndexBuffer { get; private set; }

		public VertexArray() => ID = GL.GenVertexArray();

		public void AddVertexBuffer(VertexBuffer buffer) {
			GL.BindVertexArray(ID);
			buffer.Enable();

			foreach (var attribute in buffer.Layout.Attributes) {
				GL.EnableVertexAttribArray(attribute.ID);
				GL.VertexAttribPointer(attribute.ID, attribute.ComponentCount, attribute.GetVertexAttribPointerType(), attribute.Normalized, buffer.Layout.Stride, attribute.Offset);
			}

			GL.EnableVertexAttribArray(0);

			buffer.Disable();
			GL.BindVertexArray(0);
		}

		public void AddVertexBuffer<T>(VertexBuffer<T> buffer, int id, int stride = 0, int offset = 0) {
			GL.BindVertexArray(ID);
			buffer.Enable();

			GL.EnableVertexAttribArray(id);

			if (stride != 0)
				GL.VertexAttribPointer(id, buffer.Dimensions, VertexAttribPointerType.Float, false, stride * sizeof(float), offset);
			else
				GL.VertexAttribPointer(id, buffer.Dimensions, VertexAttribPointerType.Float, false, buffer.Dimensions * sizeof(float), offset);

			GL.EnableVertexAttribArray(0);

			buffer.Disable();
			GL.BindVertexArray(0);
		}

		public void Disable() => GL.BindVertexArray(0);

		public void Enable() => GL.BindVertexArray(ID);

		public void SetIndexBuffer(IndexBuffer buffer) => IndexBuffer = buffer;
	}
}