using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Client {
	public class SpriteRenderer : ISystem {
		PerspectiveCamera Camera;

		Shader SpriteShader = Assets.Get<Shader>("SpriteShader");

		public SpriteRenderer(PerspectiveCamera camera) {
			Camera = camera;
			GL.ClearColor(new Color(20, 20, 20, 255));
			GL.Enable(EnableCap.Blend);
			GL.Enable(EnableCap.Texture2D);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
		}

		public void Render(IEnumerable<Entity> entities) {
			Camera.UpdateProjection();

			SpriteShader.Enable();

			SpriteShader.Set("projection", Camera.ProjectionMatrix);

			SpriteShader.Set("view", Camera.GetComponent<Transform>().Matrix);

			foreach (var entity in entities) {
				if (!entity.HasComponent<Sprite>() || !entity.HasComponent<Transform>()) continue;

				var sprite = entity.GetComponent<Sprite>();

				sprite.VertexArray.Enable();
				sprite.IndexBuffer.Enable();
				sprite.Texture.Enable();

				SpriteShader.Set("model", entity.GetComponent<Transform>().Matrix);

				SpriteShader.Set("override_color", sprite.Color != Color.Transparent);

				SpriteShader.Set("color", sprite.Color);

				SpriteShader.Set("billboard", sprite.Billboard);

				SpriteShader.Set("glow", sprite.GlowColor != Color.Black || sprite.GlowIterations != 10 || Math.Abs(sprite.GlowSize - 0.5f) > 0.05 || Math.Abs(sprite.GlowIntensity - 1.0f) > 0.05 || sprite.Glow);

				SpriteShader.Set("glow_iterations", sprite.GlowIterations);

				SpriteShader.Set("glow_color", sprite.GlowColor);

				SpriteShader.Set("glow_size", sprite.GlowSize);

				SpriteShader.Set("glow_intensity", sprite.GlowIntensity);

				GL.DrawElements(PrimitiveType.Quads, sprite.IndexBuffer.Count, DrawElementsType.UnsignedInt, 0);
			}
		}
	}
}