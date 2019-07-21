using System;
using Defsite;
using OpenTK;
using OpenTK.Graphics;

namespace Client {
	internal class Program {
		static void Main(string[] args) {
			GameWindow window;

			var device = DisplayDevice.GetDisplay(DisplayIndex.Primary);

			if (Utils.IsDebug)
				try {
					window = new Window(device.Width / 2, device.Height / 2, GraphicsMode.Default, "Defsite - Debug", GameWindowFlags.Default, device, 1, 0, GraphicsContextFlags.ForwardCompatible | GraphicsContextFlags.Debug);
					window.Run(0.0, 60);
				}
				catch (Exception e) {
					Log.Panic(e);
				}
			else
				try {
					window = new Window(device.Width, device.Height, GraphicsMode.Default, "Defsite", GameWindowFlags.Fullscreen, device, 1, 0, GraphicsContextFlags.ForwardCompatible);
					window.Run(0.0, 60);
				}
				catch (Exception e) {
					Log.Panic(e);
				}
		}
	}
}