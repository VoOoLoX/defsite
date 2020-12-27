using System;
using Common;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Defsite {

	internal static class Program {

		static void Main(string[] args) {
			GameWindow window;
			Monitors.TryGetMonitorInfo(0, out var monitor_info);

			if (Platform.IsDebug)
				try {
					window = new Window(
						monitor_info.HorizontalResolution / 2,
						monitor_info.VerticalResolution / 2,
						"Defsite - Debug",
						ContextFlags.ForwardCompatible | ContextFlags.Debug);
					window.Run();
				} catch (Exception e) {
					Log.Panic(e);
				}
			else
				try {
					window = new Window(
						monitor_info.HorizontalResolution,
						monitor_info.VerticalResolution,
						"Defsite",
						ContextFlags.ForwardCompatible);
					window.Run();
				} catch (Exception e) {
					Log.Panic(e);
				}
		}
	}
}