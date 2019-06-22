using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Client {
	public static class Renderer {
		public static void Init(Color clear_color) {
			GL.ClearColor(clear_color);
			GL.Enable(EnableCap.Blend);
			GL.Enable(EnableCap.Texture2D);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
		}

		public static void Draw(Model model, bool gui = false) {
			GL.Viewport(0, 0, Window.ClientWidth, Window.ClientHeight);
			model.Shader.Enable();
			model.VertexArray.Enable();
			model.IndexBuffer.Enable();
			model.Texture?.Enable();

			if (gui)
				model.Shader.Set("mvp", Camera.ProjectionMatrix * model.ModelMatrix);
			else
				model.Shader.Set("mvp", Camera.ProjectionMatrix * Camera.ViewMatrix * model.ModelMatrix);

			model.PreDraw();
			GL.DrawElements(PrimitiveType.Triangles, model.IndexBuffer.Count, DrawElementsType.UnsignedInt, 0);

			model.Texture?.Disable();
			model.VertexArray.Disable();
			model.IndexBuffer.Disable();
			model.Shader.Disable();
		}

		public static void Clear() {
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
		}
	}
}