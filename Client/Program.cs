using System;
using OpenTK;
using OpenTK.Graphics;
using Defsite;

namespace Client {
	class Program {

		static void Main(string[] args) {
			Window win = default;

			DisplayDevice device = DisplayDevice.Default;

			if (Utils.IsDebug)
				win = new Window(device.Width / 2, device.Height / 2, GraphicsMode.Default, "Defsite - Debug", GameWindowFlags.Default, device, 2, 1, GraphicsContextFlags.ForwardCompatible | GraphicsContextFlags.Debug);
			else
				win = new Window(device.Width, device.Height, GraphicsMode.Default, "Defsite", GameWindowFlags.Fullscreen, device, 2, 1, GraphicsContextFlags.ForwardCompatible);

			//win.Run(200, 60);
			win.Run();
		}
	}
}
