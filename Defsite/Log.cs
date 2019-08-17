using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Defsite {
	public static class Log {
		const short tab_size = 4;
		static short indent_level = 0;

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

		public static void Warning(object text, [CallerFilePath] string file = "", [CallerLineNumber] int line_number = 0) {
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
	}
}