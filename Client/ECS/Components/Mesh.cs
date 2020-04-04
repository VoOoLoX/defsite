using OpenTK;

namespace Client {

	public class Mesh : Component {
		public IndexBuffer IndexBuffer;

		public Color Color { get; set; } = Color.Transparent;

		public Shader Shader { get; } = Assets.Get<Shader>("MeshShader");

		public Texture Texture { get; }

		public VertexArray VertexArray { get; } = new VertexArray();

		public Mesh(Texture texture, Vector3[] data, Vector2[] uv_data, Vector3[] normals_data, uint[] ib_data) {
			Texture = texture;
			var vbo_pos = new VertexBuffer<Vector3>(data);
			var vbo_uv = new VertexBuffer<Vector2>(uv_data);
			var vbo_normals = new VertexBuffer<Vector3>(normals_data);
			VertexArray.AddVertexBuffer(vbo_pos, Shader["position"]);
			VertexArray.AddVertexBuffer(vbo_uv, Shader["uv"]);
			VertexArray.AddVertexBuffer(vbo_normals, Shader["normal"]);
			IndexBuffer = new IndexBuffer(ib_data);
		}
	}
}