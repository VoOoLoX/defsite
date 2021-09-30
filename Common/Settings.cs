using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Common;

public static class Settings<T> {
	public static async Task<T> LoadAsync(string file_path) {
		if(!File.Exists(file_path)) {
			return default;
		}

		await using var file_stream = File.OpenRead(file_path);

		var options = new JsonSerializerOptions() {
			ReadCommentHandling = JsonCommentHandling.Skip
		};
		options.Converters.Add(new JsonStringEnumConverter());
		options.Converters.Add(new VectorTupleConverter());

		return await JsonSerializer.DeserializeAsync<T>(file_stream, options);
	}

	public static async Task SaveAsync(string file_path, T settings) {
		if(!File.Exists(file_path)) {
			await using var file_stream = File.Create(file_path);

			var options = new JsonSerializerOptions() {
				WriteIndented = true
			};
			options.Converters.Add(new JsonStringEnumConverter());
			options.Converters.Add(new VectorTupleConverter());

			await JsonSerializer.SerializeAsync(file_stream, settings, options: options);
		}
	}
}