using OpenTK.Windowing.Common;

namespace Defsite.Core;

public abstract class Scene {
	public IWindowProperties WindowProperties;

	public abstract void Start();

	public abstract void Update(FrameEventArgs frame_event);

	public abstract void Render(FrameEventArgs frame_event);
}
