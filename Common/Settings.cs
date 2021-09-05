using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Common {
	public static class Settings<T> {
		public static async Task<T> LoadAsync(string file_path) {
			var path = File.Exists(file_path) ? file_path : string.Empty;

			if (path == string.Empty)
				return default;

			await using var file_stream = File.OpenRead(path);

			var options = new JsonSerializerOptions();
			options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
			return await JsonSerializer.DeserializeAsync<T>(file_stream, options);
		}
	}
}