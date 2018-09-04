using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client {
	class TextModel : Model {
		VertextArray va = new VertextArray();
		VertexBuffer<Vector2> vbo_pos;
		VertexBuffer<Vector2> vbo_uv;
		IndexBuffer ib = default;

		static Shader shader = AssetManager.Get<Shader>("TextShader");
		static Texture texture = AssetManager.Get<Texture>("TinyFont");

		static readonly string chars = " ABCDEFGHIJKLMNOPRSTQUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_-+=[]{}<>,.;:'\"/?\\|";
		static readonly float uv_char_width = 1f / chars.Length;

		Vector2 TextWorldPosition = default;
		Vector2 TextPosition = default;
		string TextValue = default;
		float TextScale = default;

		Color Color = new Color(0, 0, 0, 255);

		public TextModel(string text, Vector2 position = default, float scale = 1, Color color = default) {
			VA.Enable();

			TextValue = text;
			TextPosition = position;
			TextScale = scale;

			vbo_pos = new VertexBuffer<Vector2>(GenerateCharPositions(text));
			vbo_uv = new VertexBuffer<Vector2>(GenerateCharUVs(text));
			ib = new IndexBuffer(GenerateIndexBuffers(text.Length));

			var pos = Shader.GetAttribute("position");
			VA.AddBuffer(vbo_pos, pos, 2, 0);

			var uv = Shader.GetAttribute("uv_coords");
			VA.AddBuffer(vbo_uv, uv, 2, 0);

			Scale(TextScale);
			MoveText(TextPosition);

			if (color != default)
				Color = color;

			SetColor(Color);

			VA.Disable();
		}

		public int Width => (int)Utils.TextWidth(TextValue, TextScale);
		public int Height => (int)Utils.TextHeight(TextScale);

		public string Text { get => TextValue; set => SetText(value); }

		public void SetColor(Color color) => Color = color;

		public void SetText(string text) {
			vbo_pos.Update(GenerateCharPositions(text));
			vbo_uv.Update(GenerateCharUVs(text));
			ib = new IndexBuffer(GenerateIndexBuffers(text.Length));
		}

		public void MoveText(int x, int y) => MoveText(new Vector2(x, y));

		public void MoveText(Vector2 position) {
			TextPosition = position;
			var move_pos = Utils.ScreenToWorld(position.X, position.Y);
			Move(new Vector2(move_pos.X - TextWorldPosition.X, -move_pos.Y + TextWorldPosition.Y));
			TextWorldPosition = move_pos;
		}

		public void ScaleText(float scale) {
			var pos = TextPosition;
			MoveText(Window.ClientCenter.X - Width / 2, Window.ClientCenter.Y - Height / 2);
			Scale(scale);
			MoveText(pos);
		}

		Vector2[] GenerateCharPositions(string text) {
			var poslist = new List<Vector2[]>();
			var offset = 0;
			foreach (var ch in text) {
				var pos = new Vector2[] {
					new Vector2(offset, -1f),
					new Vector2(offset + 1, -1f),
					new Vector2(offset + 1,  0),
					new Vector2(offset,  0),
				};
				poslist.Add(pos);
				offset++;
			}
			return poslist.SelectMany(i => i).ToArray();
		}

		Vector2[] GenerateCharUVs(string text) {
			var uvlist = new List<Vector2[]>();
			foreach (var ch in text) {
				var offset = chars.IndexOf(ch);
				var uv = new Vector2[] {
					new Vector2(offset * uv_char_width, 0f),
					new Vector2(offset * uv_char_width + uv_char_width, 0f),
					new Vector2(offset * uv_char_width + uv_char_width, 1f),
					new Vector2(offset * uv_char_width, 1f),
				};
				uvlist.Add(uv);
			}
			return uvlist.SelectMany(x => x).ToArray();
		}


		uint[] GenerateIndexBuffers(int text_length) {
			var ibs = new List<uint[]>();
			for (uint i = 0; i < text_length; i++) {
				var offset = (i * 4);
				ibs.Add(new uint[] {
					offset, offset + 1, offset + 2,
					offset + 2, offset + 3, offset
				});
			}
			return ibs.SelectMany(x => x).ToArray();
		}

		public override void PreDraw() => Shader.SetUniform("text_color", Color);

		public override Shader Shader => shader;

		public override VertextArray VA => va;

		public override IndexBuffer IB => ib;

		public override Texture Texture => texture;
	}
}
