using System.Collections.Generic;
using System.Linq;
using OpenTK;

namespace Client {

	public class Text : Entity {
		FontFile font;
		int line_spacing = 1;
		string text_value;

		public string Value {
			get => text_value;
			set => SetText(value);
		}

		public Text(string text, int spacing = 10) {
			text_value = text;
			line_spacing = spacing;
			font = Assets.Get<FontFile>("Monogram");
			AddComponent(new Transform());
			AddComponent(new Sprite(font.Texture, GeneratePositions(text), GenerateUVs(text), GenerateIndexBuffer(text.Length)));
		}

		public Text(string text, FontFile font, int spacing = 10) {
			this.font = font;
			line_spacing = spacing;
			AddComponent(new Transform());
			AddComponent(new Sprite(font.Texture, GeneratePositions(text), GenerateUVs(text), GenerateIndexBuffer(text.Length)));
		}
		public override string ToString() => text_value;

		static IndexBuffer GenerateIndexBuffer(int text_length) {
			var ibs = new List<uint[]>();
			for (uint i = 0; i < text_length; i++) {
				var offset = i * 4;
				ibs.Add(new[] {
					offset, offset + 1, offset + 2, offset + 3
				});
			}

			var arr = ibs.SelectMany(x => x).ToArray();
			return new IndexBuffer(arr);
		}

		VertexBuffer<Vector2> GeneratePositions(string text) {
			var pos_list = new List<Vector2[]>();
			var char_spacing = 1f;
			var x_position = 0;
			var line = 0;
			foreach (var ch in text) {
				if (ch == ' ') {
					x_position += 5;
					continue;
				}

				if (ch == '\n') {
					line += line_spacing;
					x_position = 0;
					continue;
				}

				var char_info = font.Layout[ch];

				var baseline = line;

				var char_bottom = baseline + char_info.YOffset;
				var char_left = char_spacing + x_position + char_info.XOffset;
				var char_top = char_bottom + char_info.Height;
				var char_right = x_position + char_info.XOffset + char_info.Width;

				var pos = new[] {
					new Vector2(char_left, char_bottom),
					new Vector2(char_right, char_bottom),
					new Vector2(char_right, char_top),
					new Vector2(char_left, char_top)
				};

				pos_list.Add(pos);
				x_position += char_info.Width;
			}

			var arr = pos_list.SelectMany(x => x).ToArray();
			return new VertexBuffer<Vector2>(arr);
		}

		VertexBuffer<Vector2> GenerateUVs(string text) {
			var uv_list = new List<Vector2[]>();

			var pixel_width = 1f / font.Texture.Width;

			var pixel_height = 1f / font.Texture.Height;

			foreach (var ch in text) {
				if (ch == ' ' || ch == '\n') continue;

				var char_info = font.Layout[ch];

				var uv = new[] {
					new Vector2(char_info.X * pixel_width, char_info.Y * pixel_height), //Top left
					new Vector2(char_info.X * pixel_width + char_info.Width * pixel_width, char_info.Y * pixel_height), //Top right
					new Vector2(char_info.X * pixel_width + char_info.Width * pixel_width, char_info.Y * pixel_height + char_info.Height * pixel_height), // Bottom right
					new Vector2(char_info.X * pixel_width, char_info.Y * pixel_height + char_info.Height * pixel_height), //Bottom left
				};
				uv_list.Add(uv);
			}

			var arr = uv_list.SelectMany(x => x).ToArray();
			return new VertexBuffer<Vector2>(arr);
		}
		void SetText(string text) {
			//FIX MEMORY LEAK

			RemoveComponent(GetComponent<Sprite>());
			AddComponent(new Sprite(font.Texture, GeneratePositions(text), GenerateUVs(text), GenerateIndexBuffer(text.Length)));
		}
	}
}