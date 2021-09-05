namespace Defsite {
	public class WindowSettings {
		public string Title { get; set; } = "Defsite";
		public bool Fullscreen { get; set; } = false;
		public int Width { get; set; } = 1280;
		public int Height { get; set; } = 720;
		public bool VSync { get; set; } = false;
		public string GLVersion { get; set; } = "3.3";
	}
}