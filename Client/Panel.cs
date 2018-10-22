using OpenTK;

namespace Client {
	public class Panel {
		RectangleModel rect_model;

		public Panel(Rectangle rect, Color color) {
			rect_model = new RectangleModel(rect, color);
			Move(rect.X, rect.Y);
		}

		public void Move(int x, int y) {
			rect_model.MoveRect(x, y);
		}

		public void Resize(int width, int height) {
			var old = rect_model.Rect;
			rect_model.ResizeRect(width, height);
			Move(old.X, old.Y);
		}

		public void SetColor(Color color) {
			rect_model.SetColor(color);
		}

		public int Width { get => rect_model.Width; set => Resize(value, Height); }

		public int Height { get => rect_model.Height; set => Resize(Width, value); }

		public int X { get => rect_model.X; set => Move(value, rect_model.Y); }

		public int Y { get => rect_model.Y; set => Move(rect_model.X, value); }

		public Color Color { get => rect_model.Color; set => SetColor(value); }

		public void Draw(Renderer renderer, Camera camera) {
			renderer.Draw(camera, rect_model, true);
		}
	}
}