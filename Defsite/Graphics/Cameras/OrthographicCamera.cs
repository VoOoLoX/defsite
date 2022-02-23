using OpenTK.Mathematics;

namespace Defsite.Graphics.Cameras;

public class OrthographicCamera : ICamera {

	public Matrix4 ProjectionMatrix => Matrix4.CreateOrthographicOffCenter(Left, Right, Bottom, Top, -1000, 1000f);

	public Matrix4 ViewMatrix => Matrix4.CreateTranslation(Position) * Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(MathHelper.DegreesToRadians(RotationX), MathHelper.DegreesToRadians(RotationY), MathHelper.DegreesToRadians(RotationZ)));

	public Vector3 Position { get; set; } = Vector3.Zero;

	public float RotationX { get; set; } = 0;

	public float RotationY { get; set; } = 0;

	public float RotationZ { get; set; } = 0;

	public float Left { get; set; }

	public float Right { get; set; }

	public float Bottom { get; set; }

	public float Top { get; set; }

	public OrthographicCamera(float left, float right, float bottom, float top) => UpdateProjection(left, right, bottom, top);

	public void UpdateProjection(float left, float right, float bottom, float top) {
		Left = left;
		Right = right;
		Bottom = bottom;
		Top = top;
	}
}
