using System;
using System.Collections.Generic;
using System.IO;
using Defsite;

namespace Client {

	public enum AssetType {
		Texture,
		Shader,
		Font,
		Sound,
		Map
	}

	public static class Assets {
		static Dictionary<string, (AssetType type, object obj)> assets = new Dictionary<string, (AssetType, object)>();
		static Config settings = new Config("Assets/Settings.toml");

		static string assets_root = settings["assets_root"] ?? "Assets";
		static string fonts_path = settings["assets_fonts"] ?? "Fonts";
		static string shaders_path = settings["assets_shaders"] ?? "Shaders";
		static string sounds_path = settings["assets_sounds"] ?? "Sounds";
		static string textures_path = settings["assets_textures"] ?? "Textures";

		public static T Get<T>(string name) {
			if (assets.ContainsKey(name))
				return (T)Convert.ChangeType(assets[name].obj, typeof(T));
			return default;
		}

		public static List<T> GetAll<T>(AssetType type) {
			var result = new List<T>();
			foreach (var asset in assets) {
				if (asset.Value.type == type)
					result.Add((T)Convert.ChangeType(asset.Value.obj, typeof(T)));
			}

			return result;
		}

		public static void Init(string directory_path) {
			if (!Directory.Exists(directory_path)) {
				Log.Warning($"Can't find directory: {directory_path}");
				return;
			}

			assets_root = directory_path;
		}

		public static void Load(AssetType type, string name, string asset_name) {
			if (assets.ContainsKey(name)) return;
			switch (type) {
				case AssetType.Texture:
					assets.Add(name, (type: AssetType.Texture, obj: new Texture(Path.Join(assets_root, textures_path, asset_name))));
					break;

				case AssetType.Shader:
					assets.Add(name, (type: AssetType.Shader, obj: new Shader(Path.Join(assets_root, shaders_path, asset_name))));
					break;

				case AssetType.Font:
					assets.Add(name, (type: AssetType.Font, obj: new FontFile(Path.Join(assets_root, fonts_path, asset_name))));
					break;

				case AssetType.Sound:
					assets.Add(name, (type: AssetType.Sound, obj: new SoundSource(Path.Join(assets_root, sounds_path, asset_name))));
					break;
			}
		}

		public static void LoadAssets(string file_path) {
			var new_path = Path.Join(assets_root, file_path);
			var path = File.Exists(new_path) ? new_path : string.Empty;
			if (path == string.Empty) Log.Error($"Invalid assets config file path: {path}");

			var cfg = new Config(path);
			if (cfg["texture"] != null) {
				foreach (var child in cfg["texture"].Children) {
					Load(AssetType.Texture, child["name"], child["file"]);
				}
			}

			if (cfg["shader"] != null) {
				foreach (var child in cfg["shader"].Children) {
					Load(AssetType.Shader, child["name"], child["file"]);
				}
			}

			if (cfg["font"] != null) {
				foreach (var child in cfg["font"].Children) {
					Load(AssetType.Font, child["name"], child["file"]);
				}
			}

			if (cfg["sound"] != null) {
				foreach (var child in cfg["sound"].Children) {
					Load(AssetType.Sound, child["name"], child["file"]);
				}
			}
		}
	}
}