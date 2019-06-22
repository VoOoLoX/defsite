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
		static readonly Config settings = new Config("Assets/Settings.cfg");
		static readonly string AssetsRoot = settings["client"].GetString("assets_root") ?? "Assets";
		static readonly string TexturesPath = settings["client"].GetString("assets_textures") ?? "Textures";
		static readonly string ShadersPath = settings["client"].GetString("assets_shaders") ?? "Shadres";
		static readonly string FontsPath = settings["client"].GetString("assets_fonts") ?? "Fonts";
		static readonly string SoundsPath = settings["client"].GetString("assets_sounds") ?? "Sounds";

		static readonly Dictionary<string, object> Assets = new Dictionary<string, object>();

		public static void Load(AssetType type, string name, string asset_name) {
			if (Assets.ContainsKey(name)) return;
			switch (type) {
				case AssetType.Texture:
					Assets.Add(name, new Texture(Path.Join(AssetsRoot, TexturesPath, asset_name)));
					break;
				case AssetType.Shader:
					Assets.Add(name, new Shader(Path.Join(AssetsRoot, ShadersPath, asset_name)));
					break;
				case AssetType.Font:
					Assets.Add(name, new Texture(Path.Join(AssetsRoot, FontsPath, asset_name)));
					break;
				case AssetType.Sound:
					Assets.Add(name, new WAVFile(Path.Join(AssetsRoot, SoundsPath, asset_name)));
					break;
			}
		}

		public static T Get<T>(string name) {
			if (Assets.ContainsKey(name))
				return (T) Convert.ChangeType(Assets[name], typeof(T));
			return default;
		}
	}
}