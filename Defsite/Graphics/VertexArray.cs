using System;

using OpenTK.Graphics.OpenGL4;

namespace Defsite.Graphics;

public class VertexArray : IDisposable {
	public int ID { get; }

	public VertexArray() => ID = GL.GenVertexArray();

	public void AddVertexBuffer(VertexBuffer buffer) {
		Enable();
		buffer.Enable();

		foreach(var attribute in buffer.Layout.Attributes) {
			GL.EnableVertexAttribArray(attribute.ID);
			GL.VertexAttribPointer(attribute.ID, attribute.ComponentCount, attribute.GetVertexAttribPointerType(), attribute.Normalized, buffer.Layout.Stride, attribute.Offset);
		}

		GL.EnableVertexAttribArray(0);

		Disable();
	}

	public void Enable() => GL.BindVertexArray(ID);

	static void Disable() => GL.BindVertexArray(0);

	public void Dispose() {
		GL.DeleteVertexArray(ID);
		GC.SuppressFinalize(this);
	}
}
