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

	public static class AssetManager {
		static Config settings = new Config("Assets/Settings.toml");
		static string assets_root = settings["assets_root"] ?? "Assets";
		static string textures_path = settings["assets_textures"] ?? "Textures";
		static string shaders_path = settings["assets_shaders"] ?? "Shaders";
		static string fonts_path = settings["assets_fonts"] ?? "Fonts";
		static string sounds_path = settings["assets_sounds"] ?? "Sounds";

		static Dictionary<string, object> assets = new Dictionary<string, object>();

		public static void Load(AssetType type, string name, string asset_name) {
			if (assets.ContainsKey(name)) return;
			switch (type) {
				case AssetType.Texture:
					assets.Add(name, new Texture(Path.Join(assets_root, textures_path, asset_name)));
					break;
				case AssetType.Shader:
					assets.Add(name, new Shader(Path.Join(assets_root, shaders_path, asset_name)));
					break;
				case AssetType.Font:
					assets.Add(name, new Font(Path.Join(assets_root, fonts_path, asset_name)));
					break;
				case AssetType.Sound:
					assets.Add(name, new Sound(Path.Join(assets_root, sounds_path, asset_name)));
					break;
			}
		}

		public static T Get<T>(string name) {
			if (assets.ContainsKey(name))
				return (T) Convert.ChangeType(assets[name], typeof(T));
			return default;
		}
	}
}