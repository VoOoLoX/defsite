using System;
using OpenTK;
using OpenTK.Graphics;

namespace Client {
	class Program {
		static void Main(string[] args) {
			var win = new Window(800, 600, GraphicsMode.Default, "Defsite", GameWindowFlags.Default, DisplayDevice.Default, 3, 0, GraphicsContextFlags.ForwardCompatible);
			win.Run(200, 60);
		}
	}
}
