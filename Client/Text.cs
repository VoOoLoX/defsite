using System.Collections.Generic;
using System.Linq;
using OpenTK;

namespace Client {
	public class Text : Entity {
		Font font;

		public Text(string text) {
			font = AssetManager.Get<Font>("MinimalFont");
			AddComponent(new Transform());
			AddComponent(new Sprite(font.Texture, GeneratePositions(text), GenerateUVs(text), GenerateIndexBuffer(text.Length)));
		}

		public Text(string text, Font font) {
			this.font = font;
			AddComponent(new Transform());
			AddComponent(new Sprite(font.Texture, GeneratePositions(text), GenerateUVs(text), GenerateIndexBuffer(text.Length)));
		}

//		VertexBuffer<Vector2> GeneratePositions(string text) {
//			var pos_list = new List<Vector2[]>();
//			var char_spacing = .25f;
//			for (uint offset = 0; offset < text.Length; offset++) {
//				var pos = new[] {
//					new Vector2(char_spacing + offset, -1f),
//					new Vector2(offset + 1, -1f),
//					new Vector2(offset + 1, 0),
//					new Vector2(char_spacing + offset, 0)
//				};
//				
//				pos_list.Add(pos);
//			}
//
//			var arr = pos_list.SelectMany(x => x).ToArray();
//			return new VertexBuffer<Vector2>(arr); 
//		}

		//FIx this so every char is proper size and add char spaceing but better solution is to do something with UVs and keep char vertex size all same
		VertexBuffer<Vector2> GeneratePositions(string text) {
			var pos_list = new List<Vector2[]>();
			var char_spacing = .5f;
//			for (uint offset = 0; offset < text.Length; offset++) {
			var x_offset = 0;
			foreach (var ch in text.ToLower()) {
				if (ch == ' ') continue;

				var rectangle = font.Layout[ch];

				var pos = new[] {
					new Vector2(char_spacing + x_offset, 0),
					new Vector2(x_offset + rectangle.Width, 0),
					new Vector2(x_offset + rectangle.Width, rectangle.Height),
					new Vector2(char_spacing + x_offset, rectangle.Height)
				};

				pos_list.Add(pos);
				x_offset += rectangle.Width;
			}

			var arr = pos_list.SelectMany(x => x).ToArray();
			return new VertexBuffer<Vector2>(arr);
		}

		VertexBuffer<Vector2> GenerateUVs(string text) {
			var uv_list = new List<Vector2[]>();

			var pixel_width = 1f / font.Texture.Width;

			var pixel_height = 1f / font.Texture.Height;

//			var char_width = 1f / (font.Layout.Length + 2);

//			Log.Info(pixel_width);

			foreach (var ch in text.ToLower()) {
				if (ch == ' ') continue;

				var rectangle = font.Layout[ch];
//				Log.Error(ch);
//				Log.Indent();
//				Log.Error(rectangle.X);
//				Log.Error(rectangle.Y);
//				Log.Error(rectangle.Width);
//				Log.Error(rectangle.Height);
//				Log.Unindent();

				var uv = new[] {
					new Vector2(rectangle.X * pixel_width, rectangle.Y * pixel_height),
					new Vector2(rectangle.X * pixel_width + rectangle.Width * pixel_width, rectangle.Y * pixel_height),

					new Vector2(rectangle.X * pixel_width + rectangle.Width * pixel_width, rectangle.Y * pixel_height + (rectangle.Height + 1) * pixel_height),
					new Vector2(rectangle.X * pixel_width, rectangle.Y * pixel_height + (rectangle.Height + 1) * pixel_height),
				};
				uv_list.Add(uv);
			}

			var arr = uv_list.SelectMany(x => x).ToArray();
			return new VertexBuffer<Vector2>(arr);
		}

		IndexBuffer GenerateIndexBuffer(int text_length) {
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
	}
}