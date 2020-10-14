using System;
using System.Collections.Generic;
using System.IO;
using Common;

namespace Defsite {

	public class Asset {
		public AssetType Type { get; set; }
		public string Name { get; set; }
		public string Path { get; set; }

		public override string ToString() => $"[{Type}] {Name}@{Path}";
	}

	public enum AssetType {
		Texture,
		Shader,
		Font,
		Sound,
		Map
	}

	public class AssetsSettings {
		public List<Asset> Assets { get; set; }
	}

	public static class Assets {
		static Dictionary<string, (AssetType type, object obj)> assets = new Dictionary<string, (AssetType, object)>();

		static AssetsSettings settings = default;

		static string assets_root = "Assets";

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

		public static void Load(AssetType type, string name, string asset_name) {
			if (assets.ContainsKey(name)) return;
			switch (type) {
				case AssetType.Texture:
					assets.Add(name, (type: AssetType.Texture, obj: new Texture(Path.Join(assets_root, asset_name))));
					break;

				case AssetType.Shader:
					assets.Add(name, (type: AssetType.Shader, obj: new Shader(Path.Join(assets_root, asset_name))));
					break;

				case AssetType.Font:
					assets.Add(name, (type: AssetType.Font, obj: new FontFile(Path.Join(assets_root, asset_name))));
					break;

				case AssetType.Sound:
					assets.Add(name, (type: AssetType.Sound, obj: new Sound(Path.Join(assets_root, asset_name))));
					break;
			}
		}

		public static void LoadAssets(string file_path) {
			if (!File.Exists(file_path)) {
				Log.Error($"Invalid assets list file path: {file_path}");
				return;
			}

			assets_root = new FileInfo(file_path).DirectoryName;
			settings = Settings<AssetsSettings>.LoadAsync(file_path).Result;

			foreach (var asset in settings.Assets)
				Load(asset.Type, asset.Name, asset.Path);
		}
	}
}