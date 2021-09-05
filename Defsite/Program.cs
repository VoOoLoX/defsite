using System;
using System.IO;
using System.Threading.Tasks;

using Common;

using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;

namespace Defsite {
	public class Program {
		static async Task Main(string[] args) {
			//TODO allow window settings as command line args
			var settings_path = Path.Combine(Environment.CurrentDirectory, "Settings.json");
			
			var settings = await Settings<WindowSettings>.LoadAsync(settings_path);

			var native_window_settings = new NativeWindowSettings() {
				Title = settings.Title,
				Size = new Vector2i(settings.Width, settings.Height),
				IsFullscreen = settings.Fullscreen,
				APIVersion = new Version(settings.GLVersion),
				Flags = ContextFlags.Debug,
				AutoLoadBindings = true,
				// NumberOfSamples = 8
			};
			
			try {
				var window = new Game(GameWindowSettings.Default, native_window_settings);
				window.Run();
			} catch (Exception e) {
				Log.Panic(e);
			}
		}
	}
}