using System;
using OpenTK;
using OpenTK.Input;

namespace Client {
	public class Panel {
		bool LeftButtonDown, RightButtonDown;

		Point LeftMousePos, RightMousePos, MousePos;
		readonly RectangleModel rect_model;

		public Panel(Rectangle rect, Color color) {
			rect_model = new RectangleModel(rect, color);
			Move(rect.X, rect.Y);
		}

		public int Width {
			get => rect_model.Width;
			set => Resize(value, Height);
		}

		public int Height {
			get => rect_model.Height;
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
		}

		void Resize(int width, int height) {
			var old = rect_model.Rect;
			rect_model.ResizeRect(width, height);
			Move(old.X -= width, old.Y -= height);
		}

		void SetColor(Color color) {
			rect_model.SetColor(color);
		}

		public event Action<Panel> OnLeftClick, OnRightClick, OnHover, OnUpdate;

		public event Action<Panel, Point> OnDrag;

		public void Draw() {
			Renderer.Draw(rect_model, true);
		}

		public void Update() {
			MousePos = Input.MousePos;

			if (OnUpdate != null) {
				var on_update_actions = OnUpdate.GetInvocationList();
				if (on_update_actions.Length > 0)
					foreach (var action in on_update_actions)
						action.DynamicInvoke(this);
			}

			if (MousePos.X > X && MousePos.X < X + Width && MousePos.Y > Y && MousePos.Y < Y + Height) {
				if (OnHover != null) {
					var on_hover_actions = OnHover.GetInvocationList();
					if (on_hover_actions.Length > 0)
						foreach (var action in on_hover_actions)
							action.DynamicInvoke(this);
				}

				if (Input.IsActive(MouseButton.Left) && !LeftButtonDown) {
					LeftButtonDown = true;
					LeftMousePos = MousePos;

					if (OnLeftClick != null) {
						var on_click_actions = OnLeftClick.GetInvocationList();
						if (on_click_actions.Length > 0)
							foreach (var action in on_click_actions)
								action.DynamicInvoke(this);
					}
				}

				if (Input.IsActive(MouseButton.Right) && !RightButtonDown) {
					RightButtonDown = true;
					RightMousePos = MousePos;

					if (OnRightClick != null) {
						var on_click_actions = OnRightClick.GetInvocationList();
						if (on_click_actions.Length > 0)
							foreach (var action in on_click_actions)
								action.DynamicInvoke(this);
					}
				}
			}

			if (LeftButtonDown)
				if (OnDrag != null) {
					var on_drag_actions = OnDrag.GetInvocationList();
					if (on_drag_actions.Length > 0)
						foreach (var action in on_drag_actions)
							action.DynamicInvoke(this, new Point(MousePos.X - LeftMousePos.X, MousePos.Y - LeftMousePos.Y));
					LeftMousePos = MousePos;
				}

			if (!Input.IsActive(MouseButton.Left)) LeftButtonDown = false;

			if (!Input.IsActive(MouseButton.Right)) RightButtonDown = false;
		}
	}
}