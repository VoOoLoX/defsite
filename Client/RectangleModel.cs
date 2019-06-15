using OpenTK;

namespace Client {
	internal class RectangleModel : Model {
		static readonly Shader shader = AssetManager.Get<Shader>("ColorShader");

		Color rect_color = new Color(0, 0, 0, 255);
		float rect_scale_x = 1;
		float rect_scale_y = 1;
		Rectangle rect_screen;

		Vector2 rect_world_position = Vector2.Zero;
		readonly VertexBuffer<Vector2> vbo_pos = new VertexBuffer<Vector2>(Primitives.Quad.PositionData);

		public RectangleModel(Rectangle rect, Color color = default) {
			VA.Enable();

			rect_screen = rect;

			var pos = Shader.GetAttribute("position");
			VA.AddBuffer(vbo_pos, pos, 2);

			ResizeRect(rect_screen.Width, rect_screen.Height);
			MoveRect(rect_screen.X, rect_screen.Y);

			if (color != default)
				rect_color = color;

			SetColor(rect_color);

			VA.Disable();
		}

		public int Width {
			get => (int) ClientUtils.WorldUnitToScreen(rect_scale_x);
			set => ResizeRect(value, Height);
		}

		public int Height {
			get => (int) ClientUtils.WorldUnitToScreen(rect_scale_y);
			set => ResizeRect(Width, value);
		}

		public int X {
			get => rect_screen.X;
			set => MoveRect(value, rect_screen.Y);
		}

		public int Y {
			get => rect_screen.Y;
			set => MoveRect(rect_screen.X, value);
		}

		public Rectangle Rect {
			get => new Rectangle(rect_screen.X, rect_screen.Y, Width, Height);
			set {
				X = value.X;
				Y = value.Y;
				Width = value.Width;
				Height = value.Height;
			}
		}

		public Color Color {
			get => rect_color;
			set => SetColor(value);
		}

		public override Shader Shader => shader;

		public override VertextArray VA { get; } = new VertextArray();

		public override IndexBuffer IB { get; } = new IndexBuffer(Primitives.Quad.IndexBufferData);

		public void SetColor(Color color) {
			rect_color = color;
		}

		public void ResizeRect(int width, int height) {
			rect_screen.Width = width;
			rect_screen.Height = height;

			var sx = rect_screen.Width / ClientUtils.WorldUnitToScreen(rect_scale_x);
			var sy = rect_screen.Height / ClientUtils.WorldUnitToScreen(rect_scale_y);

			rect_scale_x *= sx;
			rect_scale_y *= sy;

			MoveRect(Window.ClientCenter.X, Window.ClientCenter.Y);
			Scale(sx, sy);
			MoveRect(rect_screen.X, rect_screen.Y);
		}

		public void MoveRect(int x, int y) {
			rect_screen.X = x;
			rect_screen.Y = y;

			var move_pos = ClientUtils.ScreenToWorld(x, y);

			Move(move_pos.X - rect_world_position.X, -move_pos.Y + rect_world_position.Y);
			rect_world_position = move_pos;
		}

		public override void PreDraw() {
			Shader.SetUniform("color", rect_color);
		}
	}
}