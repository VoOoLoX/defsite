using OpenTK;
using OpenTK.Input;

namespace Client {
	public class TileModel : Model {
		static readonly Shader shader = AssetManager.Get<Shader>("ObjectShader");
		static readonly Texture texture = AssetManager.Get<Texture>("Ghost");

		Vector3 direction_vector;
		Vector3 position;

		public TileModel() {
			VA.Enable();

			var vbo_pos = new VertexBuffer<Vector2>(Primitives.QuadCentered.PositionData);
			var vbo_uv = new VertexBuffer<Vector2>(Primitives.QuadCentered.UVData);

			var pos = Shader.GetAttribute("position");
			var uv = Shader.GetAttribute("uv_coords");

			VA.AddBuffer(vbo_pos, pos, 2);
			VA.AddBuffer(vbo_uv, uv, 2);

			VA.Disable();
		}

		public override Shader Shader => shader;

		public override VertextArray VA { get; } = new VertextArray();

		public override IndexBuffer IB { get; } = new IndexBuffer(Primitives.QuadCentered.IndexBufferData);

		public override Texture Texture => texture;

		public override void Update(double delta_time) {
			direction_vector = Vector3.Zero;

			if (Input.IsActive(Key.D))
				direction_vector.X = 1;

			if (Input.IsActive(Key.A))
				direction_vector.X = -1;

			if (Input.IsActive(Key.W))
				direction_vector.Y = 1;

			if (Input.IsActive(Key.S))
				direction_vector.Y = -1;

			if (Input.IsActive(Key.Tilde))
				direction_vector = Vector3.Zero - position;

			direction_vector.NormalizeFast();
			direction_vector *= (float) delta_time * 2;

			Move(direction_vector);

			position += direction_vector;
		}

		public override void PreDraw() {
			Shader.SetUniform("glow_size", .3f);
			Shader.SetUniform("glow_color", Color.Black);
		}
	}
}