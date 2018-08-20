using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client {
	class RectangleModel : Model {
		VertextArray va = new VertextArray();
		VertexBuffer<Vector2> vbo_pos;
		VertexBuffer<Vector2> vbo_uv;
		IndexBuffer ib = default;
		Shader shader = new Shader("Assets/Shaders/Object.shader");

		Texture RectTexture = default;
		Vector2 RectWorldPosition = default;
		Vector2 RectPosition = default;
		float RectScale = default;

		Color Color = new Color(0, 0, 0, 255);

		public RectangleModel(Texture texture = default, Rectangle rect = default, float scale = 1, Color color = default) {
			VA.Enable();

			RectTexture = texture;
			RectPosition = new Vector2(rect.X, rect.Y);
			RectScale = scale;

			vbo_pos = new VertexBuffer<Vector2>(GeneratePositions());
			vbo_uv = new VertexBuffer<Vector2>(GenerateUVs());
			ib = new IndexBuffer(GenerateIndexBuffers());

			var pos = Shader.GetAttribute("position");
			VA.AddBuffer(vbo_pos, pos, 2, 0);

			var uv = Shader.GetAttribute("uv_coords");
			VA.AddBuffer(vbo_uv, uv, 2, 0);

			//250px width - 50px unit object = 200px
			var scale_x = rect.Width / Utils.WorldUnitToScreen(scale);
			var scale_y = rect.Height / Utils.WorldUnitToScreen(scale);
			Scale(scale_x, scale_y);
			MoveRect(RectPosition);

			if (color != default)
				Color = color;

			//Shader.SetUniform("text_color", Color);
			Shader.SetUniform("sprite_size", 64);
			Shader.SetUniform("outline_color", new Vector4(1, 0, 0, 1.0f));

			VA.Disable();
		}

		// public float Width => Utils.TextWidth(Text, RectScale);
		// public float Height => Utils.TextHeight(RectScale);

		// public void SetColor(Color color) {
		// 	//Shader.SetUniform("text_color", color);
		// }

		// public void SetText(string text) {
		// 	vbo_pos.Update(GenerateCharPositions(text));
		// 	vbo_uv.Update(GenerateCharUVs(text));
		// 	ib = new IndexBuffer(GenerateIndexBuffers(text.Length));
		// }

		public void MoveRect(Vector2 position) {
			RectPosition = position;
			var move_pos = Utils.ScreenToWorld(position.X, position.Y);
			Move(new Vector2(move_pos.X - RectWorldPosition.X, -move_pos.Y + RectWorldPosition.Y));
			RectWorldPosition = move_pos;
		}


		Vector2[] GeneratePositions() {
			var pos = new Vector2[] {
					new Vector2(0, 0),
					new Vector2(1, 0),
					new Vector2(1, -1),
					new Vector2(0, -1),
				};
			return pos;
		}
		Vector2[] GenerateUVs() {
			var uv = new Vector2[] {
					new Vector2(0, 0),
					new Vector2(1, 0),
					new Vector2(1, 1),
					new Vector2(0, 1),
				};
			return uv;
		}
		uint[] GenerateIndexBuffers() {
			var ibs = new uint[] {
				0,1,2,
				2,3,0
			};
			return ibs;
		}
		public override Shader Shader => shader;
		public override VertextArray VA => va;
		public override IndexBuffer IB => ib;
		public override Texture Texture => RectTexture;
	}
}
