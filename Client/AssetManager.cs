using System;
using System.Collections.Generic;
using System.IO;

namespace Client {
	public enum AssetType {
		Texture,
		Shader,
		Font,
		Audio,
		Map
	}

	public static class AssetManager {
		const string AssetsRoot = "Assets";
		const string TexturesPath = "Textures";
		const string ShadersPath = "Shaders";
		const string FontsPath = "Fonts";

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
				}
			}

		}

		public static void Add<T>(string name, T asset) {
			if (!assets.ContainsKey(name))
				assets.Add(name, asset);
		}

		public static T Get<T>(string name) {
			if (assets.ContainsKey(name))
				return (T)Convert.ChangeType(assets[name], typeof(T));
			else
				return default(T);
		}
	}
}