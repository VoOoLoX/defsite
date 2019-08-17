using System;
using Defsite;
using OpenTK;
using OpenTK.Graphics;

namespace Client {
	internal static class Program {
		static void Main(string[] args) {
			GameWindow window;

			var device = DisplayDevice.GetDisplay(DisplayIndex.Primary);

			if (Defsite.Utils.IsDebug)
				try {
					window = new Window(
						width: device.Width / 2,
						height: device.Height / 2,
						title: "Defsite - Debug",
						mode: GraphicsMode.Default,
						window_flags: GameWindowFlags.Default,
						device: device,
						major: 1, minor: 0,
						context_flags: GraphicsContextFlags.ForwardCompatible | GraphicsContextFlags.Debug);
					window.Run();
				} catch (Exception e) {
					Log.Panic(e);
				}
			else
				try {
					window = new Window(
						width: device.Width,
						height: device.Height,
						title: "Defsite",
						mode: GraphicsMode.Default,
						window_flags: GameWindowFlags.Fullscreen,
						device: device,
						major: 1, minor: 0,
						context_flags: GraphicsContextFlags.ForwardCompatible);
					window.Run();
				} catch (Exception e) {
					Log.Panic(e);
				}
		}
	}
}