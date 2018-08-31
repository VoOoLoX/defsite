using System;
using OpenTK;
using OpenTK.Graphics;

namespace Client {
	class Program {

		[STAThread]
		static void Main(string[] args) {
			Window win = default;
			if (Utils.IsDebug)
				win = new Window(800, 600, GraphicsMode.Default, "Defsite - Debug", GameWindowFlags.Default, DisplayDevice.Default, 3, 0, GraphicsContextFlags.ForwardCompatible | GraphicsContextFlags.Debug);
			else
				win = new Window(800, 600, GraphicsMode.Default, "Defsite", GameWindowFlags.Default, DisplayDevice.Default, 3, 0, GraphicsContextFlags.ForwardCompatible);

			win.Run(200, 60);
		}
	}
}
