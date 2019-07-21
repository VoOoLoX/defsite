using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Client {
	public class RenderSystem : ISystem {
		Camera camera;

		public RenderSystem(Camera cam, Color clear_color = default) {
			camera = cam;
			GL.ClearColor(clear_color);
			GL.Enable(EnableCap.Blend);
			GL.Enable(EnableCap.Texture2D);
//			GL.Enable(EnableCap.DepthTest);
			//Fix depth test thing 
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
		}

		public void Render(List<Entity> entities) {
			var vp = camera.Transform.GetMatrix() * camera.ProjectionMatrix;

			foreach (var entity in entities) {
				// Shouldn't look for components each frame. Find better solution (also there is not much components to look for but again find better solution)
				if (entity.ContainsComponent<Transform>() && entity.ContainsComponent<Sprite>()) {
					var transform = entity.GetComponent<Transform>();
					var sprite = entity.GetComponent<Sprite>();

					sprite.Shader.Enable();
					sprite.VertexArray.Enable();
					sprite.IndexBuffer.Enable();
					sprite.Texture.Enable();

					sprite.Shader.Set("view_projection", vp);

					sprite.Shader.Set("model", transform.GetMatrix());

					sprite.Shader.Set("override_color", sprite.Color != Color.Transparent);

					sprite.Shader.Set("color", sprite.Color);

					sprite.Shader.Set("glow", sprite.GlowColor != Color.Black || sprite.GlowIterations != 10 || Math.Abs(sprite.GlowSize - 0.5f) > 0.05 || Math.Abs(sprite.GlowIntensity - 1.0f) > 0.05 || sprite.Glow);

					sprite.Shader.Set("glow_iterations", sprite.GlowIterations);

					sprite.Shader.Set("glow_color", sprite.GlowColor);

					sprite.Shader.Set("glow_size", sprite.GlowSize);

					sprite.Shader.Set("glow_intensity", sprite.GlowIntensity);

					GL.DrawElements(PrimitiveType.Quads, sprite.IndexBuffer.Count, DrawElementsType.UnsignedInt, 0);
				}

				// Mesh thing is bad and should be replaced
				if (entity.ContainsComponent<Transform>() && entity.ContainsComponent<Mesh>()) {
					var transform = entity.GetComponent<Transform>();
					var mesh = entity.GetComponent<Mesh>();

					mesh.Shader.Enable();
					mesh.VertexArray.Enable();
					mesh.IndexBuffer.Enable();
					mesh.Texture.Enable();

					mesh.Shader.Set("view_projection", vp);

					mesh.Shader.Set("model", transform.GetMatrix());

					mesh.Shader.Set("override_color", mesh.Color != Color.Transparent);

					mesh.Shader.Set("color", mesh.Color);

//					mesh.Shader.Set("glow", (mesh.GlowColor != Color.Black || mesh.GlowIterations != 10 || Math.Abs(mesh.GlowSize - 0.5f) > 0.05 || Math.Abs(mesh.GlowIntensity - 1.0f) > 0.05) || mesh.Glow);
//					
//					mesh.Shader.Set("glow_iterations", mesh.GlowIterations);
//					
//					mesh.Shader.Set("glow_color", mesh.GlowColor);
//					
//					mesh.Shader.Set("glow_size", mesh.GlowSize);
//					
//					mesh.Shader.Set("glow_intensity", mesh.GlowIntensity);

					GL.DrawElements(PrimitiveType.Quads, mesh.IndexBuffer.Count, DrawElementsType.UnsignedInt, 0);
				}
			}
		}

		public void Clear() {
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
		}
	}
}