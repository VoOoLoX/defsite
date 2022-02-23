using OpenTK.Mathematics;

namespace Defsite.Graphics.Cameras;

public interface ICamera {
	public Matrix4 ViewMatrix { get; }
	public Matrix4 ProjectionMatrix { get; }
}
