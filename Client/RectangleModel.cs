using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client {
	class RectangleModel : Model {
		VertextArray va = new VertextArray();
		VertexBuffer<Vector2> vbo_pos = new VertexBuffer<Vector2>(Primitives.Quad.PositionData);
		VertexBuffer<Vector2> vbo_uv = new VertexBuffer<Vector2>(Primitives.Quad.UVData);
		IndexBuffer ib = new IndexBuffer(Primitives.Quad.IndexBufferData);

		static Shader shader = AssetManager.Get<Shader>("ColorShader");

		Vector2 RectWorldPosition = Vector2.Zero;
		Rectangle RectScreen = Rectangle.Zero;
		float RectScaleX = 1;
		float RectScaleY = 1;

		Color RectColor = new Color(0, 0, 0, 255);

		public RectangleModel(Rectangle rect, Color color = default) {
			VA.Enable();

			RectScreen = rect;

			var pos = Shader.GetAttribute("position");
			VA.AddBuffer(vbo_pos, pos, 2, 0);

			ResizeRect(RectScreen.Width, RectScreen.Height);
			MoveRect(RectScreen.X, RectScreen.Y);

			if (color != default)
				RectColor = color;

			SetColor(RectColor);

			VA.Disable();
		}

		public void SetColor(Color color) => RectColor = color;

		public void ResizeRect(int width, int height) {
			RectScreen.Width = width;
			RectScreen.Height = height;

			var sx = RectScreen.Width / Utils.WorldUnitToScreen(RectScaleX);
			var sy = RectScreen.Height / Utils.WorldUnitToScreen(RectScaleY);

			RectScaleX *= sx;
			RectScaleY *= sy;

			MoveRect(Window.ClientCenter.X, Window.ClientCenter.Y);
			Scale(sx, sy);
			MoveRect(RectScreen.X, RectScreen.Y);
		}

		public void MoveRect(int x, int y) {
			RectScreen.X = x;
			RectScreen.Y = y;

			var move_pos = Utils.ScreenToWorld(x, y);

			Move(move_pos.X - RectWorldPosition.X, -move_pos.Y + RectWorldPosition.Y);
			RectWorldPosition = move_pos;
		}

		public override void PreDraw() => Shader.SetUniform("color", RectColor);

		public int Width { get => (int)Utils.WorldUnitToScreen(RectScaleX); set => ResizeRect(value, Height); }

		public int Height { get => (int)Utils.WorldUnitToScreen(RectScaleY); set => ResizeRect(Width, value); }

		public int X { get => RectScreen.X; set => MoveRect(value, RectScreen.Y); }

		public int Y { get => RectScreen.Y; set => MoveRect(RectScreen.X, value); }

		public Rectangle Rect { get => new Rectangle(RectScreen.X, RectScreen.Y, Width, Height); set { X = value.X; Y = value.Y; Width = value.Width; Height = value.Height; } }

		public Color Color { get => RectColor; set => SetColor(value); }

		public override Shader Shader => shader;

		public override VertextArray VA => va;

		public override IndexBuffer IB => ib;
	}
}
