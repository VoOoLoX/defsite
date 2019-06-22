using System;
using OpenTK;
using OpenTK.Input;

namespace Client {
	public static class Camera {
		const float zoom_constant = 1f / 15f;

		static Vector2 direction_vector = Vector2.Zero;
		static float zoom = 1;

		public static void Init(float zoom = 50) {
			ZoomFactor = 1;
			UpdateProjectionMatrix();
			ViewMatrix = Matrix4.LookAt(
				new Vector3(0,0,1f),
				new Vector3(0,0,0),
				new Vector3(0,1,0));
		}

		public static Matrix4 ViewMatrix { get; private set; }
		public static Matrix4 ProjectionMatrix { get; private set; }
		public static float ZoomFactor { get; private set; }

		public static void Move(Vector2 move_vector) {
			ViewMatrix *= Matrix4.CreateTranslation(move_vector.X, move_vector.Y, 0);
		}

		public static void UpdateProjectionMatrix() {
//			ProjectionMatrix = Matrix4.CreateOrthographic(1, 1, 1.0f, -1.0f);
			ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 3, (float) Window.ClientWidth / Window.ClientHeight, 0.1f, 10f);
			Zoom(ZoomFactor);
		}

		public static void Zoom(float zoom_factor) {
			ProjectionMatrix *= Matrix4.CreateScale(zoom_factor, zoom_factor, 0);
		}

		public static void Update(double delta_time) {
			direction_vector = Vector2.Zero;
			if (Input.IsActive(Key.Right))
				direction_vector.X = 1;

			if (Input.IsActive(Key.Left))
				direction_vector.X = -1;

			if (Input.IsActive(Key.Up))
				direction_vector.Y = 1;

			if (Input.IsActive(Key.Down))
				direction_vector.Y = -1;

			var scroll_value = Input.ScrollWheel;

			if (scroll_value > 0) {
				ZoomFactor *= 1 + zoom_constant;
				zoom = 1 + zoom_constant;
			}
			else if (scroll_value < 0) {
				ZoomFactor *= 1 - zoom_constant;
				zoom = 1 - zoom_constant;
			}

			if (direction_vector != Vector2.Zero) {
				direction_vector.NormalizeFast();
				Move(direction_vector * (float) delta_time);
			}

			if (zoom != 0)
				Zoom(zoom);

			zoom = 0;
		}
	}
}