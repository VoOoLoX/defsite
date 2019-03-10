using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Defsite;

namespace Client {
	public class FileLoader {
		///
		/// Asyncronous file loading in separete thread blahblahblah
		///


		static FileSystemWatcher watcher = new FileSystemWatcher();
		public void Watch(string path, List<Action<object, FileSystemEventHandler>> actions) {
			watcher.Path = path;
			watcher.IncludeSubdirectories = true;
			watcher.EnableRaisingEvents = true;

			watcher.Changed += (o, e) => {
				Log.Info(e.Name);
				foreach (var a in actions)
					a.DynamicInvoke(o, e);
			};
		}
	}
}