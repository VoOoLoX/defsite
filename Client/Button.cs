using OpenTK;

namespace Client {
	public class Button {

		RectangleModel rect_model, shadow_rect_model;
		TextModel text_model;

		public Button(string text, Rectangle rect, Color button_color, Color text_color) {
			rect_model = new RectangleModel(rect, button_color);
			shadow_rect_model = new RectangleModel(new Rectangle(rect.X, rect.Y, rect.Width, (int)(rect.Height * .1)), button_color.Lerp(Color.Black, .4f));
			text_model = new TextModel(text, scale: .2f, color: Color.Wheat);
			Move(rect.X, rect.Y);
		}

		public void Move(int x, int y) {
			rect_model.MoveRect(x, y);
			shadow_rect_model.MoveRect(x, y + rect_model.Height);
			text_model.MoveText(rect_model.Rect.X + rect_model.Width / 2 - text_model.Width / 2, rect_model.Rect.Y + rect_model.Height / 2 - text_model.Height / 2);
		}

		public void Resize(int width, int height) {
			var old = rect_model.Rect;
			rect_model.ResizeRect(width, height);
			shadow_rect_model.ResizeRect(width, (int)(rect_model.Height * .1));
			Move(old.X, old.Y);
		}

		public void SetColor(Color color) {
			rect_model.SetColor(color);
			shadow_rect_model.SetColor(color.Lerp(Color.Black, .4f));
		}

		public int Width { get => rect_model.Width; set => Resize(value, Height); }

		public int Height { get => rect_model.Height + (int)(rect_model.Height * .1); set => Resize(Width, value); }

		public int X { get => rect_model.X; set => Move(value, rect_model.Y); }

		public int Y { get => rect_model.Y; set => Move(rect_model.X, value); }

		public Color Color { get => rect_model.Color; set => SetColor(value); }

		public void Draw(Renderer renderer, Camera camera) {
			renderer.Draw(camera, rect_model, true);
			renderer.Draw(camera, shadow_rect_model, true);
			renderer.Draw(camera, text_model, true);
		}
	}
}