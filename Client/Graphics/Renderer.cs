using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Client {
	public class Renderer {
		public Renderer(Color clear_color = default) {
			GL.ClearColor(clear_color);
			GL.Enable(EnableCap.Blend);
			GL.Enable(EnableCap.Texture2D);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
		}

		public void Draw(Camera camera, Model model, bool gui = false) {
			GL.Viewport(0, 0, Window.ClientWidth, Window.ClientHeight);
			model.Shader.Enable();
			model.VertexArray.Enable();
			model.IndexBuffer.Enable();
			model.Texture?.Enable();

			if (gui)
				model.Shader.Set("mvp", camera.ProjectionMatrix * model.ModelMatrix);
			else
				model.Shader.Set("mvp", camera.ProjectionMatrix * camera.ViewMatrix * model.ModelMatrix);

			model.PreDraw();
			GL.DrawElements(PrimitiveType.Triangles, model.IndexBuffer.Count, DrawElementsType.UnsignedInt, 0);

			model.Texture?.Disable();
			model.VertexArray.Disable();
			model.IndexBuffer.Disable();
			model.Shader.Disable();
		}

		public void Clear() {
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
		}
	}
}