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

		Texture RectTexture = default;
		Vector2 RectWorldPosition = default;
		Rectangle RectPosition = default;
		float RectScale = default;

		Color Color = new Color(0, 0, 0, 255);

		public RectangleModel(Texture texture = default, Rectangle rect = default, float scale = 1, Color color = default) {
			VA.Enable();

			RectTexture = texture;
			RectPosition = rect;
			RectScale = scale;

			var pos = Shader.GetAttribute("position");
			VA.AddBuffer(vbo_pos, pos, 2, 0);

			// var uv = Shader.GetAttribute("uv_coords");
			// VA.AddBuffer(vbo_uv, uv, 2, 0);

			var scale_x = rect.Width / Utils.WorldUnitToScreen(scale);
			var scale_y = rect.Height / Utils.WorldUnitToScreen(scale);

			Scale(scale_x, scale_y);
			MoveRect(RectPosition);

			if (color != default)
				Color = color;

			SetColor(Color);

			VA.Disable();
		}

		public void MoveRect(Rectangle rect) {
			RectPosition = rect;
			var move_pos = Utils.ScreenToWorld(rect.X, rect.Y);
			Move(new Vector2(move_pos.X - RectWorldPosition.X, -move_pos.Y + RectWorldPosition.Y));
			RectWorldPosition = move_pos;
		}

		public void MoveRect(Vector2 position) {
			RectPosition.X = (int)position.X;
			RectPosition.Y = (int)position.Y;
			var move_pos = Utils.ScreenToWorld(position.X, position.Y);
			Move(new Vector2(move_pos.X - RectWorldPosition.X, -move_pos.Y + RectWorldPosition.Y));
			RectWorldPosition = move_pos;
		}

		public void SetColor(Color color) {
			Color = color;
		}
		public override void PreDraw() {
			// Shader.SetUniform("sprite_size", 64);
			Shader.SetUniform("color", Color);
		}

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

		public Rectangle Position => RectPosition;

		public override Shader Shader => shader;

		public override VertextArray VA => va;

		public override IndexBuffer IB => ib;

		public override Texture Texture => RectTexture;
	}
}
