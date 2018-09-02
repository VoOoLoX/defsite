using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client {
	class RectangleModel : Model {
		VertextArray va = new VertextArray();
		VertexBuffer<Vector2> vbo_pos = new VertexBuffer<Vector2>(PositionData);
		VertexBuffer<Vector2> vbo_uv = new VertexBuffer<Vector2>(UVData);
		IndexBuffer ib = new IndexBuffer(IndexBufferData);

		static Shader shader = AssetManager.Get<Shader>("ColorShader");

		Vector2 RectWorldPosition = default;
		Rectangle RectScreen = default;
		float RectScaleX = 1;
		float RectScaleY = 1;

		Color Color = new Color(0, 0, 0, 255);

		public RectangleModel(Rectangle rect, Color color = default) {
			VA.Enable();

			RectScreen = rect;

			var pos = Shader.GetAttribute("position");
			VA.AddBuffer(vbo_pos, pos, 2, 0);

			ResizeRect(RectScreen.Width, RectScreen.Height);
			MoveRect(RectScreen.X, RectScreen.Y);

			if (color != default)
				Color = color;

			SetColor(Color);

			VA.Disable();
		}

		public int Width { get => (int)Utils.WorldUnitToScreen(RectScaleX); set => ResizeRect(value, Height); }
		public int Height { get => (int)Utils.WorldUnitToScreen(RectScaleY); set => ResizeRect(Width, value); }

		public void ResizeRect(int width, int height) {
			RectScreen.Width = width;
			RectScreen.Height = height;

			var sx = RectScreen.Width / Utils.WorldUnitToScreen(RectScaleX);
			var sy = RectScreen.Height / Utils.WorldUnitToScreen(RectScaleY);

			RectScaleX *= sx;
			RectScaleY *= sy;
			var pos = new Vector2(RectScreen.X, RectScreen.Y);

			MoveRect(Window.ClientWidth / 2, Window.ClientHeight / 2);
			Scale(sx, sy);
			MoveRect((int)pos.X, (int)pos.Y);
		}

		public void MoveRect(int x, int y) {
			RectScreen.X = x;
			RectScreen.Y = y;
			var move_pos = Utils.ScreenToWorld(x, y);
			Move(new Vector2(move_pos.X - RectWorldPosition.X, -move_pos.Y + RectWorldPosition.Y));
			RectWorldPosition = move_pos;
		}

		public void SetColor(Color color) => Color = color;


		static Vector2[] PositionData =
			new Vector2[] {
					new Vector2(0, 0),
					new Vector2(1, 0),
					new Vector2(1, -1),
					new Vector2(0, -1),
				};

		static Vector2[] UVData =
			new Vector2[] {
					new Vector2(0, 0),
					new Vector2(1, 0),
					new Vector2(1, 1),
					new Vector2(0, 1),
				};

		static uint[] IndexBufferData =
			new uint[] {
				0,1,2,
				2,3,0
			};

		public override void PreDraw() => Shader.SetUniform("color", Color);

		public Rectangle Rect => new Rectangle(RectScreen.X, RectScreen.Y, Width, Height);

		public override Shader Shader => shader;

		public override VertextArray VA => va;

		public override IndexBuffer IB => ib;
	}
}
