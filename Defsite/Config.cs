using System;
using System.IO;
using System.Linq;

namespace Defsite {
	public class Config {
		string config_path;
		TomlTable table;

		public Config(string path) => Load(path);

		public TomlNode this[string variable_name] {
			get => Get(variable_name);
			set => Set(variable_name, value);
		}

		void Load(string file_path) {
			var path = File.Exists(file_path) ? file_path : string.Empty;
			if (path != string.Empty) {
				table = TOML.Parse(new StreamReader(path));
				table.IsInline = false;
				config_path = file_path;
			}
			else
				throw new Exception($"Invalid config file path: {path}");
		}

		public void Add(string key, TomlNode value) {
			if (!table.Keys.Contains(key))
				table.Add(key, value);
			else
				Log.Warn($"Config ({config_path}) already contains key: {key}");
		}

		public TomlNode Get(string key) {
			if (table.Keys.Contains(key))
				return table[key];
			Log.Warn($"Config ({config_path}) does not contain key: {key}");
			return null;
		}

		public void Set(string key, TomlNode value) {
			if (table.Keys.Contains(key))
				table[key] = value;
			else
				Add(key, value);
		}

		public void Remove(string key) {
			if (table.Keys.Contains(key))
				table.Delete(key);
			else
				Log.Warn($"Config ({config_path}) does not contain key: {key}");
		}

		public void Save(string section = "") {
			foreach (var node in table.Children) {
				node.Comment = null;
			}

			using (var writer = new StreamWriter(config_path)) {
				if (section != string.Empty)
					table.ToTomlString(writer, section);
				else
					table.ToTomlString(writer);
			}
		}
	}
}