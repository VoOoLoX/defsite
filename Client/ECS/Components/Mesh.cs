using OpenTK;

namespace Client {
	public class Mesh : IComponent {
		public IndexBuffer IndexBuffer;

		public Mesh(Texture texture, Vector3[] data, Vector2[] uv_data, uint[] ib_data) {
			Texture = texture;
			var vbo_pos = new VertexBuffer<Vector3>(data);
			var vbo_uv = new VertexBuffer<Vector2>(uv_data);
			VertexArray.AddVertexBuffer(vbo_pos, Shader["position"]);
			VertexArray.AddVertexBuffer(vbo_uv, Shader["uv_coords"]);
			IndexBuffer = new IndexBuffer(ib_data);
		}

		public Shader Shader { get; } = AssetManager.Get<Shader>("MeshShader");
		public Texture Texture { get; }
		public VertexArray VertexArray { get; } = new VertexArray();

		public Color Color { get; set; } = Color.Transparent;
	}
}