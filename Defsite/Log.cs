using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace Defsite {
	public static class Log {

		static short indent_level = 0;
		const short tab_size = 4;

		public static void Indent() {
			indent_level++;
		}

		public static void Unindent() {
			if (indent_level >= 1)
				indent_level--;
		}

		public static void Info(object text, [CallerFilePath] string file = "", [CallerLineNumber] int line_number = 0) {
			var file_info = new FileInfo(file);
			Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] <{file_info.Name.Split('.')[0]}:{line_number}> {new string(' ', indent_level * tab_size) + text}");
		}

		public static void Warn(object text, [CallerFilePath] string file = "", [CallerLineNumber] int line_number = 0) {
			var file_info = new FileInfo(file);
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] <{file_info.Name.Split('.')[0]}:{line_number}> {new string(' ', indent_level * tab_size) + text}");
			Console.ResetColor();
		}

		public static void Error(object text, [CallerFilePath] string file = "", [CallerLineNumber] int line_number = 0) {
			var file_info = new FileInfo(file);
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] <{file_info.Name.Split('.')[0]}:{line_number}> {new string(' ', indent_level * tab_size) + text}");
			Console.ResetColor();
		}

		public static void Panic(object text, [CallerFilePath] string file = "", [CallerLineNumber] int line_number = 0) {
			var file_info = new FileInfo(file);
			Console.BackgroundColor = ConsoleColor.Red;
			Console.ForegroundColor = ConsoleColor.Black;
			Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] <{file_info.Name.Split('.')[0]}:{line_number}> {new string(' ', indent_level * tab_size) + text}");
			Console.ResetColor();
			Environment.Exit(1);
		}

		// public enum NotificationType {
		// 	Info,
		// 	Error
		// }

		// public static void Notify(string title, string text, int duration, NotificationType type) {

		// 	//TODO: Fix this shit
		// 	string cmd = "";
		// 	string icon_exe = "";
		// 	if (Utils.IsWindows)
		// 		cmd = $@"Add-Type -AssemblyName System.Windows.Forms;$balloon = New-Object System.Windows.Forms.NotifyIcon;$balloon.Icon = [System.Drawing.Icon]::ExtractAssociatedIcon(""{icon_exe}"");$balloon.BalloonTipIcon = [System.Windows.Forms.ToolTipIcon]""{type.ToString()}"";$balloon.BalloonTipText = ""{text}"";$balloon.BalloonTipTitle = ""{title}"";$balloon.Visible = $true;$balloon.ShowBalloonTip({duration});";
		// 	else
		// 		cmd = @"notify-send ""Title"" ""Test""";

		// 	// var psh = PowerShell.Ceate()

		// 	var ps_info = new ProcessStartInfo() {
		// 		FileName = "notify-send",
		// 		Arguments = $@"""{title}"" ""{text}"" -t {duration}"
		// 	};
		// 	Process ps = new Process() {
		// 		StartInfo = ps_info
		// 	};
		// 	ps.Start();
		// }
	}
}