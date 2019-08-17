using System.Collections.Generic;
using System.Linq;
using OpenTK;

namespace Client {
	public class Text : Entity {
		FontFile font;
		string text_value;

		public Text(string text) {
			text_value = text;
			font = Assets.Get<FontFile>("Monogram");
			AddComponent(new Transform());
			AddComponent(new Sprite(font.Texture, GeneratePositions(text), GenerateUVs(text), GenerateIndexBuffer(text.Length)));
		}

		public Text(string text, FontFile font) {
			this.font = font;
			AddComponent(new Transform());
			AddComponent(new Sprite(font.Texture, GeneratePositions(text), GenerateUVs(text), GenerateIndexBuffer(text.Length)));
		}

		public string Value {
			get => text_value;
			set => SetText(value);
		}

		public override string ToString() => text_value;

		VertexBuffer<Vector2> GeneratePositions(string text) {
			var pos_list = new List<Vector2[]>();
			var char_spacing = 1f;
			var x_position = 0;
			var line = 1;
			foreach (var ch in text) {
				if (ch == ' ') {
					x_position += 5;
					continue;
				}

				if (ch == '\n') {
					line++;
					x_position = 0;
					continue;
				}

				var char_info = font.Layout[ch];

				var pos = new[] {
					new Vector2(char_spacing + x_position + char_info.XOffset, char_info.YOffset * line + line),
					new Vector2(x_position + char_info.XOffset + char_info.Width, char_info.YOffset * line + line),
					new Vector2(x_position + char_info.XOffset + char_info.Width, (char_info.YOffset * line) + line + char_info.Height),
					new Vector2(char_spacing + char_info.XOffset + x_position, (char_info.YOffset * line) + line + char_info.Height)
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

		void SetText(string text) {
			RemoveComponent(GetComponent<Sprite>());
			AddComponent(new Sprite(font.Texture, GeneratePositions(text), GenerateUVs(text), GenerateIndexBuffer(text.Length)));
		}
	}
}