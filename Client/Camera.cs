using OpenTK;
using OpenTK.Input;

namespace Client {
	public class Camera {
		const float zoom_constant = 1f / 15f;

		Vector2 direction_vector = Vector2.Zero;
		float zoom = 1;

		public Camera(float zoom = 50) {
			ZoomFactor = zoom;
			ViewMatrix = Matrix4.LookAt(
				new Vector3(0, 0, 1),
				new Vector3(0, 0, 0),
				new Vector3(0, 1, 0)
			);
			UpdateProjectionMatrix();
		}

		public Matrix4 ViewMatrix { get; private set; }
		public Matrix4 ProjectionMatrix { get; private set; }
		public static float ZoomFactor { get; private set; }

		public void Move(Vector2 move_vector) {
			ViewMatrix *= Matrix4.CreateTranslation(move_vector.X, move_vector.Y, 0);
		}

		public void UpdateProjectionMatrix() {
			ProjectionMatrix = Matrix4.CreateOrthographic(Window.ClientWidth, Window.ClientHeight, 1.0f, -1.0f);
			Zoom(ZoomFactor);
		}

		public void Zoom(float zoom_factor) {
			ProjectionMatrix *= Matrix4.CreateScale(zoom_factor, zoom_factor, 0);
		}

		public void Update(double delta_time) {
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