using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Defsite {
	public class ConfigOld {
		ConfigScope root = new ConfigScope("");

		public ConfigOld(string filepath) {
			Parse(filepath);
		}

		public ConfigScope this[string scope] => GetScope(scope);

		void Parse(string file) {
			var path = File.Exists(file) ? file : string.Empty;
			if (path == string.Empty) return;

			var lines = new StreamReader(file).ReadToEnd();
			var current_scope = root;
			foreach (var line in lines.Split('\n')) {
				var no_comment = line.Split(new[] {'#'}, 1)[0];
				var trimed = line.Trim();

				if (trimed.StartsWith("[") && trimed.EndsWith("]")) {
					var obj = trimed.Replace("[", "").Replace("]", "");

					current_scope = GetScope(obj, true);
					continue;
				}

				if (!trimed.Contains('=')) continue;
				var s = trimed.Split(new[] {'='}, 2);
				var key = s[0].Trim();
				var value = s[1].Trim();

				current_scope.AddProperty(key, value);
			}
		}

		ConfigScope GetScope(string scope_path, bool insert = false) {
			var sub_objs = scope_path.Split(':');
			var obj_idx = 0;
			var scope = root;

			foreach (var _ in sub_objs) {
				if (insert) scope.AddObject(sub_objs[obj_idx], new ConfigScope(sub_objs[obj_idx]));
				scope = scope.GetObject(sub_objs[obj_idx]);
				obj_idx++;
			}

			return scope;
		}

		public ConfigScope GetScope(string path) {
			return GetScope(path, false);
		}

		(ConfigScope scope, string key) ParsePath(string path) {
			var scope_path = path.Split('.')[0];
			var key = path.Split('.')[1];
			return (GetScope(scope_path), key);
		}

		public bool GetBool(string path) {
			var (scope, key) = ParsePath(path);
			return scope.GetBool(key);
		}

		public int GetInt(string path) {
			var (scope, key) = ParsePath(path);
			return scope.GetInt(key);
		}

		public decimal GetDecimal(string path) {
			var (scope, key) = ParsePath(path);
			return scope.GetDecimal(key);
		}

		public string GetString(string path) {
			var (scope, key) = ParsePath(path);
			return scope.GetString(key);
		}

		public class ConfigScope {
			string name;
			Dictionary<string, ConfigScope> objects = new Dictionary<string, ConfigScope>();
			Dictionary<string, string> properties = new Dictionary<string, string>();

			public ConfigScope(string name) {
				this.name = name;
			}

			public string Name => name;
			public IDictionary<string, string> Properties => properties;
			public IEnumerable<ConfigScope> Objects => objects.Values.ToList();

			public void AddProperty(string key, string value) {
				if (!properties.ContainsKey(key))
					properties[key] = value;
			}

			public void AddObject(string key, ConfigScope value) {
				if (!objects.ContainsKey(key))
					objects[key] = value;
			}

			string GetProperty(string key) {
				return properties.ContainsKey(key) ? properties[key] : string.Empty;
			}

			public ConfigScope GetObject(string key) {
				return objects.ContainsKey(key) ? objects[key] : this;
			}

			public bool GetBool(string key) {
				return bool.TryParse(GetProperty(key), out var value) && value;
			}

			public int GetInt(string key) {
				return int.TryParse(GetProperty(key), out var value) ? value : 0;
			}

			public uint GetUInt(string key) {
				return uint.TryParse(GetProperty(key), out var value) ? value : 0;
			}

			public decimal GetDecimal(string key) {
				return decimal.TryParse(GetProperty(key), out var value) ? value : decimal.Zero;
			}

			public string GetString(string key) {
				if (GetProperty(key).Contains('\'') || GetProperty(key).Contains('"') || GetProperty(key).Contains('`'))
					return GetProperty(key).Replace("\"", "").Replace("'", "").Replace("`", "");
				return GetProperty(key);
			}
		}
	}
}