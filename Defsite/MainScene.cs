using System.Collections.Generic;
using System.Drawing;
using Defsite.Core;
using Defsite.Graphics;
using Defsite.Graphics.Cameras;
using Defsite.IO;
using NLog;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Defsite;
class MainScene : Scene {

	OrthographicCamera camera;
	VertexArray vao;
	VertexBuffer vbo;
	IndexBuffer ibo;
	Shader shader;

	static readonly Logger log = LogManager.GetCurrentClassLogger();

	public override Color ClearColor => Color.DimGray;

	public override void Start() {
		camera = new OrthographicCamera(0, Window.ClientSize.X, Window.ClientSize.Y, 0);
		vao = new VertexArray();

		shader = Assets.Get<Shader>("ColorShader");

		var layout = new BufferLayout(new List<VertexAttribute> {
			new(shader.GetAttributeLocation("v_position"), VertexAttributeType.Vector3),
			new(shader.GetAttributeLocation("v_color"), VertexAttributeType.Vector4)
		});

		var pos = Vector3.Zero;
		var quad = Primitives.CreateQuadCentered(pos, Color.Red, 2);

		ibo = new IndexBuffer(new int[] {
			0, 1, 2, 2, 3, 0
		});

		vbo = new VertexBuffer() {
			Layout = layout
		};

		vbo.SetData(quad);

		vao.AddVertexBuffer(vbo);
	}

	public override void Update(FrameEventArgs frame_event) {
		camera.UpdateProjection(0, Window.ClientSize.X, Window.ClientSize.Y, 0);
		camera.Position = new Vector3(1, 5, 1);
		//camera.RotationZ = 0;
		var pv = Matrix4.Invert(Matrix4.LookAt(camera.Position, Vector3.Zero, new Vector3(0, 1, 0)) * camera.ProjectionMatrix);
		var nx = Utils.Utils.Map(Input.MousePosition.X, 0, Window.ClientSize.X, -1f, 1f);
		var ny = Utils.Utils.Map(Input.MousePosition.Y, 0, Window.ClientSize.Y, 1f, -1f);
		var nz = Utils.Utils.Map(0, -1000, 1000, -1, 1);
		var c = new Vector4(nx, ny, nz, 1.0f);
		var q = c * pv;
		q.W = 1.0f / q.W;
		q.X *= q.W;
		q.Y *= q.W;
		q.Z *= q.W;

		var pos = new Vector3(q.X, q.Y, q.Z);
		//log.Info(pos);
		var quad = Primitives.CreateTileCentered(pos, Color.Cyan, 200);
		vbo.UpdateData(quad);
	}

	public override void Render(FrameEventArgs frame_event) {
		shader.Enable();
		shader.Set("u_projection", camera.ProjectionMatrix);
		shader.Set("u_view", Matrix4.LookAt(camera.Position, Vector3.Zero, new Vector3(0, 1, 0)));
		//shader.Set("u_view", Matrix4.Identity);
		shader.Set("u_model", Matrix4.Identity);

		vao.Enable();
		ibo.Enable();
		GL.DrawElements(PrimitiveType.Triangles, ibo.Count, DrawElementsType.UnsignedInt, 0);
	}
}
