using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Server {
	public class Config {
		public class ConfigScope {
			string name = string.Empty;
			Dictionary<string, string> properties = new Dictionary<string, string>();
			Dictionary<string, ConfigScope> objects = new Dictionary<string, ConfigScope>();

			public string Name => name;
			public IReadOnlyDictionary<string, string> Properties => properties;
			public IReadOnlyList<ConfigScope> Objects => objects.Values.ToList();

			public ConfigScope(string name) {
				this.name = name;
			}

			public void AddPropery(string key, string value) {
				if (!properties.ContainsKey(key))
					properties[key] = value;
			}

			public void AddObject(string key, ConfigScope value) {
				if (!objects.ContainsKey(key))
					objects[key] = value;
			}

			string GetPropery(string key) => properties.ContainsKey(key) ? properties[key] : string.Empty;

			public ConfigScope GetObject(string key) => objects.ContainsKey(key) ? objects[key] : this;

			public bool GetBool(string key) {
				if (bool.TryParse(GetPropery(key), out bool value))
					return value;
				return false;
			}

			public int GetInt(string key) {
				if (int.TryParse(GetPropery(key), out int value))
					return value;
				return 0;
			}

			public uint GetUInt(string key) {
				if (uint.TryParse(GetPropery(key), out uint value))
					return value;
				return 0;
			}

			public decimal GetDecimal(string key) {
				if (decimal.TryParse(GetPropery(key), out decimal value))
					return value;
				return decimal.Zero;
			}

			public string GetString(string key) {
				if (GetPropery(key).Contains('\'') || GetPropery(key).Contains('"') || GetPropery(key).Contains('`'))
					return GetPropery(key).Replace("\"", "").Replace("'", "").Replace("`", "");
				return GetPropery(key);
			}
		}

		ConfigScope root = new ConfigScope("");

		public Config(string filepath) => Parse(filepath);

		void Parse(string filepath) {
			var lines = new StreamReader(filepath).ReadToEnd();
			var current_scope = root;
			foreach (var line in lines.Split('\n')) {
				var no_comment = line.Split('#')[0];
				var trimed = no_comment.Trim();

				if (trimed.Contains('[') && trimed.Contains(']')) {
					var obj = trimed.Replace("[", "").Replace("]", "");

					current_scope = GetScope(obj, true);
					continue;
				}

				if (trimed.Contains('=')) {
					var key = trimed.Split('=')[0].Trim();
					var value = trimed.Split('=')[1].Trim();

					current_scope.AddPropery(key, value);
				}
			}
		}

		ConfigScope GetScope(string scope_path, bool insert = false) {
			var sub_objs = scope_path.Split(':');
			var obj_idx = 0;
			var scope = root;

			foreach (var o in sub_objs) {
				if (insert) scope.AddObject(sub_objs[obj_idx], new ConfigScope(sub_objs[obj_idx]));
				scope = scope.GetObject(sub_objs[obj_idx]);
				obj_idx++;
			}

			return scope;
		}

		public ConfigScope GetScope(string path) => GetScope(path, false);

		(ConfigScope scope, string key) ParsePath(string path) {
			var scope_path = path.Split('.')[0];
			var key = path.Split('.')[1];
			return (GetScope(scope_path), key);
		}

		public bool GetBool(string path) {
			var x = ParsePath(path);
			return x.scope.GetBool(x.key);
		}

		public int GetInt(string path) {
			var x = ParsePath(path);
			return x.scope.GetInt(x.key);
		}

		public decimal GetDecimal(string path) {
			var x = ParsePath(path);
			return x.scope.GetDecimal(x.key);
		}

		public string GetString(string path) {
			var x = ParsePath(path);
			return x.scope.GetString(x.key);
		}
	}
}
