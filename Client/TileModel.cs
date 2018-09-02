using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client {
	public class TileModel : Model {
		VertextArray va = new VertextArray();
		IndexBuffer ib = new IndexBuffer(IndexBufferData);

		static Shader shader = AssetManager.Get<Shader>("ObjectShader");
		static Texture texture = AssetManager.Get<Texture>("Pot");

		Vector3 direction_vector = new Vector3();
		Vector3 position = new Vector3();

		public TileModel() {
			VA.Enable();

			var vbo_pos = new VertexBuffer<Vector2>(PositionData);
			var vbo_uv = new VertexBuffer<Vector2>(UVData);

			var pos = Shader.GetAttribute("position");
			var uv = Shader.GetAttribute("uv_coords");

			VA.AddBuffer(vbo_pos, pos, 2, 0);
			VA.AddBuffer(vbo_uv, uv, 2, 0);

			VA.Disable();
		}

		public override void Update(double delta_time) {
			direction_vector = Vector3.Zero;

			if (InputManager.IsKeyActive(Key.D))
				direction_vector.X = 1;

			if (InputManager.IsKeyActive(Key.A))
				direction_vector.X = -1;

			if (InputManager.IsKeyActive(Key.W))
				direction_vector.Y = 1;

			if (InputManager.IsKeyActive(Key.S))
				direction_vector.Y = -1;

			if (InputManager.IsKeyActive(Key.Tilde))
				direction_vector = Vector3.Zero - position;

			direction_vector.NormalizeFast();
			direction_vector *= (float)delta_time * 2;

			Move(direction_vector);

			position += direction_vector;
		}

		public override void PreDraw() {
			Shader.SetUniform("sprite_size", 64);
			Shader.SetUniform("outline_color", new Vector4(1, 1, 0, 1.0f));
		}

		static Vector2[] PositionData =
			new Vector2[] {
				new Vector2(-1, -1),
				new Vector2( 1, -1),
				new Vector2( 1,  1),
				new Vector2(-1,  1),
			};

		static Vector2[] UVData =
			new Vector2[] {
				new Vector2(0, 0),
				new Vector2(1, 0),
				new Vector2(1, 1),
				new Vector2(0, 1),
			};

		static uint[] IndexBufferData =
			new uint[] {
				0, 1, 2,
				2, 3, 0
			};

		public override Shader Shader => shader;

		public override VertextArray VA => va;

		public override IndexBuffer IB => ib;

		public override Texture Texture => texture;
	}
}
