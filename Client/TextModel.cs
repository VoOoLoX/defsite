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

		Vector2 TextWorldPosition = Vector2.Zero;
		Vector2 TextPosition = Vector2.Zero;
		string TextValue = string.Empty;
		float TextScale = 1;
		bool TextGlow = false;

		Color TextColor = new Color(0, 0, 0, 255);

		public TextModel(string text, Vector2 position = default, float scale = 1, Color color = default, bool glow = false) {
			VA.Enable();

			TextValue = text;
			TextPosition = position;
			TextScale = scale;
			TextGlow = glow;

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
				TextColor = color;

			SetColor(TextColor);

			VA.Disable();
		}

		public void SetColor(Color color) => TextColor = color;

		public void SetText(string text) {
			vbo_pos.Update(GenerateCharPositions(text));
			vbo_uv.Update(GenerateCharUVs(text));
			ib = new IndexBuffer(GenerateIndexBuffers(text.Length));
		}

		public void MoveText(int x, int y) => MoveText(new Vector2(x, y));

		public void MoveText(Vector2 position) {
			TextPosition = position;
			var move_pos = ClientUtils.ScreenToWorld(position.X, position.Y);
			Move(new Vector2(move_pos.X - TextWorldPosition.X, -move_pos.Y + TextWorldPosition.Y));
			TextWorldPosition = move_pos;
		}

		public void ScaleText(float scale) {
			var pos = TextPosition;
			MoveText(Window.ClientCenter.X - Width / 2, Window.ClientCenter.Y - Height / 2);
			Scale(1.0f / TextScale);
			Scale(scale);
			TextScale = scale;
			MoveText(pos);
		}

		//fix this
		public void Size(int pixels) {
			ScaleText((float)pixels / Camera.ZoomFactor);
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

		public override void PreDraw() {
			Shader.SetUniform("text_color", TextColor);
			Shader.SetUniform("glow", TextGlow);
		}

		public bool Glow { get => TextGlow; set => TextGlow = value; }

		public int Width => (int)ClientUtils.TextWidth(TextValue, TextScale);

		public int Height => (int)ClientUtils.TextHeight(TextScale);

		public string Text { get => TextValue; set => SetText(value); }

		public Color Color { get => TextColor; set => SetColor(value); }

		public override Shader Shader => shader;

		public override VertextArray VA => va;

		public override IndexBuffer IB => ib;

		public override Texture Texture => texture;
	}
}
