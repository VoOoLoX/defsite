using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Defsite {

	public class UIRenderer : ISystem {
		OrthographicCamera camera;

		Shader sprite_shader = Assets.Get<Shader>("SpriteShader");

		public UIRenderer() {
			camera = new OrthographicCamera();
			GL.ClearColor(Color.FromArgb(20, 20, 20, 255));
			GL.Enable(EnableCap.Blend);
			GL.Enable(EnableCap.Texture2D);
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
		}

		public void Render(IEnumerable<EntityOLD> entities) {
			camera.UpdateProjection();

			sprite_shader.Enable();

			sprite_shader.Set("projection", camera.ProjectionMatrix);

			sprite_shader.Set("view", Matrix4.Identity);

			foreach (var entity in entities) {
				if (!entity.HasComponent<SpriteComponent>() || !entity.HasComponent<Transform>()) continue;

				var sprite = entity.GetComponent<SpriteComponent>();

				sprite.VertexArray.Enable();
				sprite.IndexBuffer.Enable();
				sprite.Texture.Enable();

				sprite_shader.Set("model", entity.GetComponent<Transform>().Matrix);

				sprite_shader.Set("override_color", sprite.Color != Color.Transparent);

				sprite_shader.Set("color", sprite.Color);

				sprite_shader.Set("billboard", sprite.Billboard);

				sprite_shader.Set("glow", sprite.GlowColor != Color.Black || sprite.GlowIterations != 10 || Math.Abs(sprite.GlowSize - 0.5f) > 0.05 || Math.Abs(sprite.GlowIntensity - 1.0f) > 0.05 || sprite.Glow);

				sprite_shader.Set("glow_iterations", sprite.GlowIterations);

				sprite_shader.Set("glow_color", sprite.GlowColor);

				sprite_shader.Set("glow_size", sprite.GlowSize);

				sprite_shader.Set("glow_intensity", sprite.GlowIntensity);

				GL.DrawElements(PrimitiveType.Quads, sprite.IndexBuffer.Count, DrawElementsType.UnsignedInt, 0);
			}
		}
	}
}