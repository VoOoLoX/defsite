using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Common;
using OpenTK.Graphics.OpenGL;

namespace Defsite {

	public class ShaderFile {

		public static ShaderFile Default = new(new MemoryStream(Encoding.ASCII.GetBytes(@"
			#type vertex
			#version 130

			in vec3 position;

			uniform mat4 projection;
			uniform mat4 view;
			uniform mat4 model;

			void main() {
				gl_Position = projection * view * model * vec4(position, 1.0);
			}

			#type pixel
			#version 130

			uniform vec4 color;

			void main() {
				gl_FragColor = color;
			}"
		)));

		bool from_file;

		bool from_stream;

		FileInfo shader_file_info;

		Stream shader_stream;

		public Dictionary<ShaderType, string> Shaders { get; private set; }

		public ShaderFile(string path) {
			Load(path);
			from_file = true;
			from_stream = false;
		}

		public ShaderFile(Stream stream) {
			Load(stream);
			from_file = false;
			from_stream = true;
		}
		public void Reload() {
			if (from_file)
				Reload(shader_file_info.FullName);
			else if (from_stream)
				Reload(shader_stream);
			else
				Log.Panic("Can't reload shader.");
		}

		void Load(string file_path) {
			var file_info = File.Exists(file_path) ? new FileInfo(file_path) : null;
			if (file_info == null)
				Log.Panic($"Invalid shader file path: {file_path}");

			shader_file_info = file_info;

			Load(file_info?.OpenRead());
		}

		void Load(Stream data_stream) {
			shader_stream = data_stream;
			Shaders = new Dictionary<ShaderType, string>();

			using var reader = new StreamReader(data_stream);
			var data = reader.ReadToEnd();

			var shaders = data.Trim().Split("#type", StringSplitOptions.RemoveEmptyEntries);

			foreach (var shader in shaders) {
				if (shader.Length < 1) Log.Panic("Invalid shader");

				var source_lines = shader.Split(new[] { "\r\n", "\r", "\n", "\t" }, StringSplitOptions.RemoveEmptyEntries);
				var type = source_lines[0].Trim();

				var shader_type = (ShaderType)0;

				switch (type.ToLower()) {
					case "vertex":
						shader_type = ShaderType.VertexShader;
						break;

					case "fragment":
					case "pixel":
						shader_type = ShaderType.FragmentShader;
						break;

					case "geometry":
						shader_type = ShaderType.GeometryShader;
						break;

					case "compute":
						shader_type = ShaderType.ComputeShader;
						break;

					default:
						Log.Panic($"Invalid shader type: {type}");
						break;
				}

				var shader_source = string.Join("\n", source_lines.Skip(1));

				Shaders.Add(shader_type, shader_source);
			}
		}
		void Reload(string path) => Load(path);

		void Reload(Stream stream) => Load(stream);
	}
}