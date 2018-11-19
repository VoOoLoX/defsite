using System;
using OpenTK;
using OpenTK.Input;

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

		public event Action<Panel> OnLeftClick;

		public event Action<Panel> OnRightClick;

		public event Action<Panel> OnHover;

		public event Action<Panel> OnUpdate;

		bool LeftButtonDown;

		bool RightButtonDown;

		Point OldPosition, MousePosition = new Point();

		public void Draw(Renderer renderer, Camera camera) {
			renderer.Draw(camera, rect_model, true);
		}

		public void Update() {
			MousePosition = InputManager.MousePos;

			if (OnUpdate != null) {
				var on_update_actions = OnUpdate.GetInvocationList();
				if (on_update_actions.Length > 0)
					foreach (var action in on_update_actions)
						action.DynamicInvoke(this);
			}
			if ((MousePosition.X > this.X && MousePosition.X < this.X + this.Width) &&
				(MousePosition.Y > this.Y && MousePosition.Y < this.Y + this.Height)) {
				if (OnHover != null) {
					var on_hover_actions = OnHover.GetInvocationList();
					if (on_hover_actions.Length > 0)
						foreach (var action in on_hover_actions)
							action.DynamicInvoke(this);
				}
				if (InputManager.IsActive(MouseButton.Left) && !LeftButtonDown) {
					LeftButtonDown = true;
					if (OnLeftClick != null) {
						OldPosition = MousePosition;
						var on_click_actions = OnLeftClick.GetInvocationList();
						if (on_click_actions.Length > 0)
							foreach (var action in on_click_actions)
								action.DynamicInvoke(this);

						System.Console.WriteLine($"{OldPosition}-{MousePosition}");
					}
				}

				if (InputManager.IsActive(MouseButton.Right) && !RightButtonDown) {
					RightButtonDown = true;
					if (OnRightClick != null) {
						OldPosition = MousePosition;
						var on_click_actions = OnRightClick.GetInvocationList();
						if (on_click_actions.Length > 0)
							foreach (var action in on_click_actions)
								action.DynamicInvoke(this);


					}
				}
			}
			System.Console.WriteLine($"{OldPosition}-{MousePosition}");
			if (!InputManager.IsActive(MouseButton.Left))
				LeftButtonDown = false;

			if (!InputManager.IsActive(MouseButton.Right))
				RightButtonDown = false;
		}
	}
}