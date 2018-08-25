using System;
using OpenTK;
using OpenTK.Graphics;

namespace Client {
	class Program {
		static void Main(string[] args) {
#if DEBUG
			var win = new Window(800, 600, GraphicsMode.Default, "Defsite", GameWindowFlags.Default, DisplayDevice.Default, 3, 0, GraphicsContextFlags.Debug);
#else
			var win = new Window(800, 600, GraphicsMode.Default, "Defsite", GameWindowFlags.Default, DisplayDevice.Default, 3, 0, GraphicsContextFlags.ForwardCompatible);
#endif
			win.Run(200, 60);
		}
	}
}
