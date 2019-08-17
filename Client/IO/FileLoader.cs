using System.IO;
using System.Threading.Tasks;
using Defsite;

namespace Client {
	public class FileLoader {
		public FileLoader(string file_path) {
			var path = File.Exists(file_path) ? file_path : string.Empty;
			if (path == string.Empty) Log.Error($"Invalid file path: {file_path}");
			Path = path;
		}

		string Path { get; set; }
		public long Length { get; private set; }

		public async Task<byte[]> ReadBinary() {
			var file = new FileInfo(Path);
			if (!file.Exists) {
				Log.Error($"Can't find: {file.Name}");
				return null;
			}

			var reader = file.OpenRead();
			var data = new byte[file.Length];
			Length = file.Length;
			await reader.ReadAsync(data, 0, (int) file.Length);
			return data;
		}
	}
}