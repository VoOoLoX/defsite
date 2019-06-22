using System.Collections.Generic;
using System.Linq;
using OpenTK;

namespace Client {
	internal class TextModel : Model {
		const string chars = " ABCDEFGHIJKLMNOPRSTQUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_-+=[]{}<>,.;:'\"/?\\|";

		static readonly Shader shader = AssetManager.Get<Shader>("SpriteShader");
		static readonly Texture texture = AssetManager.Get<Texture>("TinyFont");
		static readonly float uv_char_width = 1f / chars.Length;
		IndexBuffer ib;

		Color text_color = Color.Black;
		Vector2 text_position;
		float text_scale = 1;
		readonly string text_value;

		Vector2 text_world_position = Vector2.Zero;
		readonly VertexBuffer<Vector2> vbo_pos;
		readonly VertexBuffer<Vector2> vbo_uv;

		public TextModel(string text, Vector2 position = default, float scale = 1, Color color = default, bool glow = false) {
			text_value = text;
			text_position = position;
			text_scale = scale;
			Glow = glow;

			vbo_pos = new VertexBuffer<Vector2>(GenerateCharPositions(text));
			vbo_uv = new VertexBuffer<Vector2>(GenerateCharUVs(text));
			ib = new IndexBuffer(GenerateIndexBuffers(text.Length));

			var pos = Shader["position"];
			var uv = Shader["uv_coords"];

			VertexArray.AddVertexBuffer(vbo_pos, pos);
			VertexArray.AddVertexBuffer(vbo_uv, uv);
			
			Scale(text_scale);
			MoveText(text_position);

			if (color != default)
				text_color = color;

			SetColor(text_color);
		}

		public bool Glow { get; set; }

		public int Width => (int) ClientUtils.TextWidth(text_value, text_scale);

		public int Height => (int) ClientUtils.TextHeight(text_scale);

		public string Text {
			get => text_value;
			set => SetText(value);
		}

		public Color Color {
			get => text_color;
			set => SetColor(value);
		}

		public override Shader Shader => shader;

		public override VertexArray VertexArray { get; } = new VertexArray();

		public override IndexBuffer IndexBuffer => ib;

		public override Texture Texture => texture;

		void SetColor(Color color) {
			text_color = color;
		}

		void SetText(string text) {
			vbo_pos.SetData(GenerateCharPositions(text));
			vbo_uv.SetData(GenerateCharUVs(text));
			ib.SetData(GenerateIndexBuffers(text.Length));
		}

		public void MoveText(int x, int y) {
			MoveText(new Vector2(x, y));
		}

		public void MoveText(Vector2 position) {
			text_position = position;
			var move_pos = ClientUtils.ScreenToWorld(position.X, position.Y);
			Move(new Vector2(move_pos.X - text_world_position.X, -move_pos.Y + text_world_position.Y));
			text_world_position = move_pos;
		}

		public void ScaleText(float scale) {
			var pos = text_position;
			MoveText(Window.ClientCenter.X - Width / 2, Window.ClientCenter.Y - Height / 2);
			Scale(1.0f / text_scale);
			Scale(scale);
			text_scale = scale;
			MoveText(pos);
		}

		//fix this
		// public void Size(int pixels) {
		// 	ScaleText((float)pixels / Camera.ZoomFactor);
		// }

		static Vector2[] GenerateCharPositions(string text) {
			var poslist = new List<Vector2[]>();
			var offset = 0;
			foreach (var ch in text) {
				var pos = new[] {
					new Vector2(offset, -1f),
					new Vector2(offset + 1, -1f),
					new Vector2(offset + 1, 0),
					new Vector2(offset, 0)
				};
				poslist.Add(pos);
				offset++;
			}

			return poslist.SelectMany(i => i).ToArray();
		}

		static Vector2[] GenerateCharUVs(string text) {
			var uvlist = new List<Vector2[]>();
			foreach (var ch in text) {
				var offset = chars.IndexOf(ch);
				var uv = new[] {
					new Vector2(offset * uv_char_width, 0f),
					new Vector2(offset * uv_char_width + uv_char_width, 0f),
					new Vector2(offset * uv_char_width + uv_char_width, 1f),
					new Vector2(offset * uv_char_width, 1f)
				};
				uvlist.Add(uv);
			}

			return uvlist.SelectMany(x => x).ToArray();
		}

		static uint[] GenerateIndexBuffers(int text_length) {
			var ibs = new List<uint[]>();
			for (uint i = 0; i < text_length; i++) {
				var offset = i * 4;
				ibs.Add(new[] {
					offset, offset + 1, offset + 2,
					offset + 2, offset + 3, offset
				});
			}

			return ibs.SelectMany(x => x).ToArray();
		}

		public override void PreDraw() {
			Shader.Set("text_color", text_color);
			Shader.Set("glow", Glow);
		}
	}
}