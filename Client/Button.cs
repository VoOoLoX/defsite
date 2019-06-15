using System;
using OpenTK;
using OpenTK.Input;

namespace Client {
	public class Button {
		static Color shadow_color;
		readonly RectangleModel rect_model;
		readonly RectangleModel shadow_rect_model;
		readonly TextModel text_model;

		public Button(string text, Rectangle rect, Color button_color, Color text_color) {
			shadow_color = button_color.Lerp(Color.Black, .4f);
			rect_model = new RectangleModel(rect, button_color);
			shadow_rect_model = new RectangleModel(new Rectangle(rect.X, rect.Y, rect.Width, (int) (rect.Height * .1)), shadow_color);
			text_model = new TextModel(text, scale: .2f, color: text_color);
			Move(rect.X, rect.Y);
		}

		public int Width {
			get => rect_model.Width;
			set => Resize(value, Height);
		}

		public int Height {
			get => rect_model.Height + (int) (rect_model.Height * .1);
			set => Resize(Width, value);
		}

		public int X {
			get => rect_model.X;
			set => Move(value, rect_model.Y);
		}

		public int Y {
			get => rect_model.Y;
			set => Move(rect_model.X, value);
		}

		public Color Color {
			get => rect_model.Color;
			set => SetColor(value);
		}

		void Move(int x, int y) {
			rect_model.MoveRect(x, y);
			shadow_rect_model.MoveRect(x, y + rect_model.Height);
			text_model.MoveText(rect_model.Rect.X + rect_model.Width / 2 - text_model.Width / 2, rect_model.Rect.Y + rect_model.Height / 2 - text_model.Height / 2);
		}

		void Resize(int width, int height) {
			var old = rect_model.Rect;
			rect_model.ResizeRect(width, height);
			shadow_rect_model.ResizeRect(width, (int) (rect_model.Height * .1));
			Move(old.X, old.Y);
		}

		void SetColor(Color color) {
			rect_model.SetColor(color);
			shadow_rect_model.SetColor(shadow_color);
		}

		public event Action<Button> OnClick;

		public event Action<Button> OnHover;

		public event Action<Button> OnUpdate;

		public void Draw(Renderer renderer, Camera camera) {
			renderer.Draw(camera, rect_model, true);
			renderer.Draw(camera, shadow_rect_model, true);
			renderer.Draw(camera, text_model, true);
		}

		public void Update() {
			var mouse_p = Input.MousePos;

			if (OnUpdate != null) {
				var on_update_actions = OnUpdate.GetInvocationList();
				if (on_update_actions.Length > 0)
					foreach (var action in on_update_actions)
						action.DynamicInvoke(this);
			}

			if (mouse_p.X > X && mouse_p.X < X + Width && mouse_p.Y > Y && mouse_p.Y < Y + Height) {
				if (OnHover != null) {
					var on_hover_actions = OnHover.GetInvocationList();
					if (on_hover_actions.Length > 0)
						foreach (var action in on_hover_actions)
							action.DynamicInvoke(this);
				}

				if (Input.IsActive(MouseButton.Left))
					if (OnClick != null) {
						var on_click_actions = OnClick.GetInvocationList();
						if (on_click_actions.Length > 0)
							foreach (var action in on_click_actions)
								action.DynamicInvoke(this);
					}
			}
		}
	}
}