using OpenTK;

namespace Client {
	public class OrthographicCamera {
		public OrthographicCamera() => UpdateProjection();

		public Matrix4 ProjectionMatrix { get; private set; }

		public void UpdateProjection() => ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(0, Window.Width, Window.Height, 0, 0, 1);
	}
}