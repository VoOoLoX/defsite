using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenTK;

namespace Client {
	public class Font {
		Dictionary<char, Rectangle> char_layout;
		Texture image_file;

		public Font(string file) {
//			Log.Error(file);
			var path = File.Exists(file) ? file : string.Empty;
			if (path == string.Empty) return;

			var lines = File.ReadAllLines(path);
//			Log.Info(lines);

			image_file = new Texture(Path.Join("Assets/Fonts/", lines[0]));

			char_layout = new Dictionary<char, Rectangle>();

			foreach (var line in lines.Skip(1)) {
				var data = line.Split(" ");
				char_layout.Add(data[0][0], new Rectangle(int.Parse(data[1]), int.Parse(data[2]), int.Parse(data[3]), int.Parse(data[4])));
			}
		}

		public Texture Texture => image_file;
		public Dictionary<char, Rectangle> Layout => char_layout;
	}
}