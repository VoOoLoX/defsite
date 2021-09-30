using System;
using System.IO;
using System.Threading.Tasks;

using Common;

using Defsite.Core;

using NLog;

namespace Defsite;

public class Program {
	static async Task Main(string[] args) {
		//TODO(@VoOoLoX): allow settings as command line args

		ConfigureLogger();

		var settings_path = Path.Combine(Environment.CurrentDirectory, "Settings.json");

		var settings = await Settings<ApplicationSettings>.LoadAsync(settings_path);

		//await Settings<ApplicationSettings>.SaveAsync(Path.Combine(Environment.CurrentDirectory, "test.json"), settings);

		var application = new Application(settings, new MainScene());

		application.Run();
	}

	static void ConfigureLogger() {
		var config = new NLog.Config.LoggingConfiguration();

		var layout = "${level:uppercase=true:padding=-5} [${longdate}] ${message}";

		var log_file = new NLog.Targets.FileTarget("log") {
			FileName = $"logs/{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.log",
			Layout = layout,
			CreateDirs = true
		};

		var log_console = new NLog.Targets.ColoredConsoleTarget("log") {
			Layout = layout
		};

		config.AddRule(LogLevel.Info, LogLevel.Fatal, log_console);
		config.AddRule(LogLevel.Trace, LogLevel.Fatal, log_file);

		LogManager.Configuration = config;
	}
}
