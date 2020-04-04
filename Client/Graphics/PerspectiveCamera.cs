using OpenTK;

namespace Client {

	public class PerspectiveCamera : Entity {
		float fov;
		float z_far;
		float z_near;

		public Matrix4 ProjectionMatrix { get; private set; }

		public PerspectiveCamera(Vector3 position, Quaternion rotation, float fov = 50, float z_near = 0.01f, float z_far = 100f) {
			var transform = new Transform {
				Position = position,
				Rotation = rotation
			};

			this.fov = fov;
			this.z_near = z_near;
			this.z_far = z_far;

			AddComponent(transform);
			GetComponent<Transform>().SetMatrix(
				Matrix4.LookAt(
					position,
					new Vector3(0, 0, -1),
					new Vector3(0, 1, 0)
				)
			);
			UpdateProjection();
		}
		public void UpdateProjection() => ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fov), (float)Window.Width / Window.Height, z_near, z_far);
	}
}