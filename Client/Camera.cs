using OpenTK;

namespace Client {
	public class Camera : Entity {
		//Make these configurable!!!
		float fov = 60;
		float z_far = 100f;
		float z_near = 0.1f;

		public Camera(Vector3 position, Vector3 rotation, float fov = 60, float z_near = 0.1f, float z_far = 100f) {
			Transform = new Transform {
				Position = position,
				Rotation = rotation
			};

			AddComponent(Transform);
			Transform.SetMatrix(
				Matrix4.LookAt(
					position,
					new Vector3(0, 0, 0),
					new Vector3(0, 1, 0)
				)
			);
			UpdatePerspectiveProjection();
		}

		public Camera() {
			ProjectionMatrix = Matrix4.CreateOrthographic(1, 1, -1, 1);
		}

		public Transform Transform { get; set; }
		public Matrix4 ProjectionMatrix { get; private set; }

		public void UpdatePerspectiveProjection() {
			ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fov), (float) Window.ClientWidth / Window.ClientHeight, z_near, z_far);
		}

		public void Move(Vector3 position) {
		}
	}
}