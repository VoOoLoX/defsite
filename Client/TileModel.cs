using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client {
	public class TileModel : Model {
		Vector2[] VertexData =
			new Vector2[] {
				new Vector2(-1f, -1f),
				new Vector2( 1f, -1f),
				new Vector2( 1f,  1f),
				new Vector2(-1f,  1f),
			};

		Vector2[] UVData =
			new Vector2[] {
				new Vector2(0f, 0f),
				new Vector2(1f, 0f),
				new Vector2(1f, 1f),
				new Vector2(0f, 1f),
			};

		uint[] IndexBufferData =
			new uint[] {
				0, 1, 2,
				2, 3, 0
			};

		Shader shader;
		VertextArray va;
		IndexBuffer ib;
		Texture texture;


		public TileModel() {
			shader = new Shader("Assets/Shaders/Object.shader");
			va = new VertextArray();
			ib = new IndexBuffer(IndexBufferData);
			texture = new Texture("Assets/Textures/pot.png");

			VA.Enable();

			var vbo_pos = new VertexBuffer<Vector2>(VertexData);
			var vbo_uv = new VertexBuffer<Vector2>(UVData);

			var pos = Shader.GetAttribute("position");
			var uv = Shader.GetAttribute("uv_coords");

			VA.AddBuffer(vbo_pos, pos, 2, 0);
			VA.AddBuffer(vbo_uv, uv, 2, 0);

			VA.Disable();
		}
		public override Shader Shader => shader;

		public override VertextArray VA => va;

		public override IndexBuffer IB => ib;

		public override Texture Texture => texture;

		Vector3 direction_vector = new Vector3();

		Vector3 position = new Vector3();

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

			//Feature(@vooolox): make option to choose input device

			// if (GamePad.GetState(0).IsConnected) {
			// 	GamePad.SetVibration(0, 1, 0);
			// 	direction_vector = new Vector3(MathF.Round(GamePad.GetState(0).ThumbSticks.Left.X, 2), -MathF.Round(GamePad.GetState(0).ThumbSticks.Left.Y, 2), 0);
			// }



			direction_vector.NormalizeFast();
			direction_vector *= (float)delta_time * 2;
			Move(direction_vector);

			position += direction_vector;

			var r = new Random();

			Shader.SetUniform("sprite_size", 64);
			Shader.SetUniform("outline_color", new Vector4(1, 1, 0, 1.0f));
		}
	}
}
