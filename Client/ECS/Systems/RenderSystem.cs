using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Client {
	public class RenderSystem : ISystem {
		public RenderSystem() {
			
		}

		public void Render(List<Entity> entities) {
			foreach (var entity in entities) {
				if (entity.ContainsComponent<Transform>() && entity.ContainsComponent<Sprite>()) {
					var transform = entity.GetComponent<Transform>();
					var sprite = entity.GetComponent<Sprite>();
					
					GL.Viewport(0, 0, Window.ClientWidth, Window.ClientHeight);
					
					sprite.Shader.Enable();
					sprite.VertexArray.Enable();
					sprite.IndexBuffer.Enable();
					sprite.Texture.Enable();

					sprite.Shader.Set("mvp", transform.ModelMatrix * Camera.ViewMatrix * Camera.ProjectionMatrix);

					sprite.Shader.Set("override_color", sprite.Color != Color.Transparent);
					
					sprite.Shader.Set("color", sprite.Color);
					
					sprite.Shader.Set("glow", (sprite.GlowColor != Color.Black || sprite.GlowIterations != 10 || Math.Abs(sprite.GlowSize - 0.5f) > 0.05 || Math.Abs(sprite.GlowIntensity - 1.0f) > 0.05) || sprite.Glow);
					
					sprite.Shader.Set("glow_iterations", sprite.GlowIterations);
					
					sprite.Shader.Set("glow_color", sprite.GlowColor);
					
					sprite.Shader.Set("glow_size", sprite.GlowSize);
					
					sprite.Shader.Set("glow_intensity", sprite.GlowIntensity);

					GL.DrawElements(PrimitiveType.Triangles, sprite.IndexBuffer.Count, DrawElementsType.UnsignedInt, 0);

					sprite.Texture.Disable();
					sprite.VertexArray.Disable();
					sprite.IndexBuffer.Disable();
					sprite.Shader.Disable();
				}
			}
		}
	}
}