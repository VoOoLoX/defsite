using OpenTK.Mathematics;

namespace Defsite {

	public class OrthographicCamera {

		public Matrix4 ProjectionMatrix { get; private set; }
		public OrthographicCamera() => UpdateProjection();
		public void UpdateProjection() => ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(0, Game.GameWidth, Game.GameHeight, 0, 0, 1000);
	}
}