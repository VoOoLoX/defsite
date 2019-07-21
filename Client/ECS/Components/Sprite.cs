using OpenTK;

namespace Client {
	public class Sprite : IComponent {
		public Color GlowColor = Color.Black;
		public float GlowIntensity = 1.0f;

		public int GlowIterations = 10;
		public float GlowSize = 0.5f;

		public Sprite(Texture texture) {
			Texture = texture;

			var vbo_pos = new VertexBuffer<Vector2>(Primitives.QuadCentered.PositionData);
			var vbo_uv = new VertexBuffer<Vector2>(Primitives.QuadCentered.UVData);

			VertexArray = new VertexArray();
			VertexArray.AddVertexBuffer(vbo_pos, Shader["position"]);
			VertexArray.AddVertexBuffer(vbo_uv, Shader["uv_coords"]);

			IndexBuffer = new IndexBuffer(Primitives.QuadCentered.IndexBufferData);
		}

		public Sprite(Texture texture, VertexBuffer<Vector2> pos, VertexBuffer<Vector2> uv, IndexBuffer index_buffer) {
			Texture = texture;

			VertexArray = new VertexArray();
			VertexArray.AddVertexBuffer(pos, Shader["position"]);
			VertexArray.AddVertexBuffer(uv, Shader["uv_coords"]);

			IndexBuffer = index_buffer;
		}

		public Shader Shader { get; } = AssetManager.Get<Shader>("SpriteShader");
		public Texture Texture { get; }
		public VertexArray VertexArray { get; }
		public IndexBuffer IndexBuffer { get; }

		public Color Color { get; set; } = Color.Transparent;
		public bool Glow { get; set; } = false;
	}
}