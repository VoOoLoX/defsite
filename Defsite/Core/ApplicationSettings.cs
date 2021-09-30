using OpenTK.Windowing.Common;

namespace Defsite.Core;

public class ApplicationSettings {
	public string Title { get; set; } = "Defsite";

	public (int Width, int Height) Size { get; set; } = (1280, 720);

	public (int X, int Y) Position { get; set; } = (-1, -1);

	public bool VSync { get; set; } = false;

	public string GLVersion { get; set; } = "3.3";

	public bool IsMultiThreaded { get; set; } = false;

	public double UpdateFrequency { get; set; } = 0;

	public double RenderFrequency { get; set; } = 0;

	public int NumberOfSamples { get; set; } = 0;

	public string Icon { get; set; } = "";

	public WindowBorder WindowBorder { get; set; } = WindowBorder.Resizable;

	public WindowState WindowState { get; set; } = WindowState.Normal;

	public ContextFlags Flags { get; set; } = ContextFlags.Default | ContextFlags.Debug | ContextFlags.ForwardCompatible;

	public bool StartFocused { get; set; } = true;

	public bool StartVisible { get; set; } = true;

}
