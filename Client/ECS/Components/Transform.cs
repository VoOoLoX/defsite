using OpenTK;

namespace Client {
	public class Transform : IComponent {
		
		public Matrix4 ModelMatrix { get;  set; } = Matrix4.Identity;

		public Transform() {
		}
		
		public Vector3 Position;
		public Quaternion Rotation;
		public Vector3 Scale;
	}
}