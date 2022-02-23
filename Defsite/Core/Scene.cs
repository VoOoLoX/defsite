using System.Drawing;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Defsite.Core;

public abstract class Scene {
	public GameWindow Window { get; set; }

	public abstract Color ClearColor { get; }

	public abstract void Start();

	public abstract void Update(FrameEventArgs frame_event);

	public abstract void Render(FrameEventArgs frame_event);
}
