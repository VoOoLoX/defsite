using OpenTK;

namespace Client {
	public class Sprite : IComponent {
		readonly VertexBuffer<Vector2> vbo_pos = new VertexBuffer<Vector2>(Primitives.QuadCentered.PositionData);
		readonly VertexBuffer<Vector2> vbo_uv = new VertexBuffer<Vector2>(Primitives.QuadCentered.UVData);

		public Sprite(Texture texture) {
			Texture = texture;
			VertexArray.AddVertexBuffer(vbo_pos, Shader["position"]);
			VertexArray.AddVertexBuffer(vbo_uv, Shader["uv_coords"]);
		}
		
		public Shader Shader { get; } = AssetManager.Get<Shader>("SpriteShader");
		public Texture Texture { get; }
		public VertexArray VertexArray { get; } = new VertexArray();
		public IndexBuffer IndexBuffer { get; } = new IndexBuffer(Primitives.QuadCentered.IndexBufferData);

		public Color Color { get; set; } = Color.Transparent;
		public bool Glow { get; set;} = false;
		
		public int GlowIterations = 10;
		public Color GlowColor = Color.Black;
		public float GlowSize = 0.5f;
		public float GlowIntensity = 1.0f;
	}
}