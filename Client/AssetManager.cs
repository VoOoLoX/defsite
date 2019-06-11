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
		static Config settings = new Config("Assets/Settings.cfg");
		static string AssetsRoot = settings["client"].GetString("assets_root") ?? "Assets";
		static string TexturesPath = settings["client"].GetString("assets_textures") ?? "Textures";
		static string ShadersPath = settings["client"].GetString("assets_shaders") ?? "Shadres";
		static string FontsPath = settings["client"].GetString("assets_fonts") ?? "Fonts";
		static string SoundsPath = settings["client"].GetString("assets_sounds") ?? "Sounds";

		static Dictionary<string, object> assets = new Dictionary<string, object>();

		public static void Load(AssetType type, string name, string asset_name) {
			if (!assets.ContainsKey(name)) {
				switch (type) {
					case AssetType.Texture:
						assets.Add(name, new Texture(Path.Join(AssetsRoot, TexturesPath, asset_name)));
						break;
					case AssetType.Shader:
						assets.Add(name, new Shader(Path.Join(AssetsRoot, ShadersPath, asset_name)));
						break;
					case AssetType.Font:
						assets.Add(name, new Texture(Path.Join(AssetsRoot, FontsPath, asset_name)));
						break;
					case AssetType.Sound:
						assets.Add(name, new WAVFile(Path.Join(AssetsRoot, SoundsPath, asset_name)));
						break;
				}
			}

		}

		public static T Get<T>(string name) {
			if (assets.ContainsKey(name))
				return (T)Convert.ChangeType(assets[name], typeof(T));
			else
				return default(T);
		}
	}
}