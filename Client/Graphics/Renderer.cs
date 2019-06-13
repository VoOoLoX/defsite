using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

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
			model.VA.Enable();
			model.Texture?.Enable();
			model.IB.Enable();

			if (gui)
				model.Shader.SetUniform("mvp", camera.ProjectionMatrix * model.ModelMatrix);
			else
				model.Shader.SetUniform("mvp", camera.ProjectionMatrix * camera.ViewMatrix * model.ModelMatrix);

			model.PreDraw();
			GL.DrawElements(PrimitiveType.Triangles, model.IB.Count, DrawElementsType.UnsignedInt, 0);

			model.IB.Disable();
			model.Texture?.Disable();
			model.VA.Disable();
			model.Shader.Disable();
		}

		public void Clear() => GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
	}
}
