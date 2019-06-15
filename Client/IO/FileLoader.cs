using System.IO;
using System.Threading.Tasks;
using Defsite;

namespace Client {
	public class Asset {
	}

	public class FileLoader {
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
			await reader.ReadAsync(data, 0, (int) file.Length);
			return data;
		}
	}
}