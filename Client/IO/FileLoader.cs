using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Defsite;

namespace Client {
	public class Asset {

	}

	public class FileLoader {
		///
		/// Asyncronous file loading in separete thread blahblahblah
		///
		// static FileSystemWatcher watcher = new FileSystemWatcher();


		// public void Watch(string path, List<Action<object, FileSystemEventHandler>> actions) {
		// 	watcher.Path = path;
		// 	watcher.IncludeSubdirectories = true;
		// 	watcher.EnableRaisingEvents = true;

		// 	watcher.Changed += (o, e) => {
		// 		Log.Info(e.Name);
		// 		foreach (var a in actions)
		// 			a.DynamicInvoke(o, e);
		// 	};
		// }

		public FileLoader() {

		}

		// public char[] ReadChars(int count) {
		// 	var arr = new char[count];
		// 	for (int i = 0; i < count; i++)
		// 		arr[i] = (char)bytes[i];
		// 	return arr;
		// }

		public async Task<string> Read(string path) {
			var file = new FileInfo(path);
			if (!file.Exists) {
				Log.Error($"Can't find: {file.Name}");
				return null;
			}
			var reader = file.OpenText();
			return await reader.ReadToEndAsync();
		}

		public async Task<byte[]> ReadBinary(string path) {
			var file = new FileInfo(path);
			if (!file.Exists) {
				Log.Error($"Can't find: {file.Name}");
				return null;
			}
			var reader = file.OpenRead();
			var data = new byte[file.Length];
			await reader.ReadAsync(data, 0, (int)file.Length);
			return data;
		}
	}
}