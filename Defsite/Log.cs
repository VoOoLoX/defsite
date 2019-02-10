using System;
using System.Diagnostics;

namespace Defsite {
	public static class Log {
		static string format = $"[{DateTime.Now.ToString("HH:mm:ss")}] ";

		public static void Info(string info) => Console.WriteLine(format + info);

		public static void Warn(string warn) {
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(format + warn);
			Console.ResetColor();
		}

		public static void Error(string warn) {
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(format + warn);
			Console.ResetColor();
		}

		public enum NotificationType {
			Info,
			Error
		}

		public static void Notify(string title, string text, int duration, NotificationType type) {

			//TODO: Fix this shit
			string cmd = "";
			string icon_exe = "";
			if (Utils.IsWindows)
				cmd = $@"Add-Type -AssemblyName System.Windows.Forms;$balloon = New-Object System.Windows.Forms.NotifyIcon;$balloon.Icon = [System.Drawing.Icon]::ExtractAssociatedIcon(""{icon_exe}"");$balloon.BalloonTipIcon = [System.Windows.Forms.ToolTipIcon]""{type.ToString()}"";$balloon.BalloonTipText = ""{text}"";$balloon.BalloonTipTitle = ""{title}"";$balloon.Visible = $true;$balloon.ShowBalloonTip({duration});";
			else
				cmd = @"notify-send ""Title"" ""Test""";

			// var psh = PowerShell.Ceate()

			var ps_info = new ProcessStartInfo() {
				FileName = "notify-send",
				Arguments = $@"""{title}"" ""{text}"" -t {duration}"
			};
			Process ps = new Process() {
				StartInfo = ps_info
			};
			ps.Start();
		}
	}
}