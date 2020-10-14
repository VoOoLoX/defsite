using OpenTK;

namespace Defsite {

	public class SpriteComponent : Component {
		public Color GlowColor = Color.Black;
		public float GlowIntensity = 1.0f;
		public int GlowIterations = 10;
		public float GlowSize = 0.5f;

		public bool Billboard { get; set; } = false;

		public Color Color { get; set; } = Color.Transparent;

		public bool Glow { get; set; } = false;

		public IndexBuffer IndexBuffer { get; }

		public Texture Texture { get; }

		public VertexArray VertexArray { get; }

		public SpriteComponent() {
		}

		public SpriteComponent(Texture texture) {
			Texture = texture;

			var vbo_pos = new VertexBuffer<Vector2>(Primitives.QuadCentered.PositionData);
			var vbo_uv = new VertexBuffer<Vector2>(Primitives.QuadCentered.UVData);

			VertexArray = new VertexArray();
			VertexArray.AddVertexBuffer(vbo_pos, Assets.Get<Shader>("SpriteShader")["position"]);
			VertexArray.AddVertexBuffer(vbo_uv, Assets.Get<Shader>("SpriteShader")["uv"]);

			IndexBuffer = new IndexBuffer(Primitives.QuadCentered.IndexBufferData);
		}

		public SpriteComponent(Texture texture, VertexBuffer<Vector2> pos, VertexBuffer<Vector2> uv, IndexBuffer index_buffer) {
			Texture = texture;

			VertexArray = new VertexArray();
			VertexArray.AddVertexBuffer(pos, Assets.Get<Shader>("SpriteShader")["position"]);
			VertexArray.AddVertexBuffer(uv, Assets.Get<Shader>("SpriteShader")["uv"]);

			IndexBuffer = index_buffer;
		}
	}
}