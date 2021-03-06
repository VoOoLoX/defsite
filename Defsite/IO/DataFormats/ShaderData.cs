using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using NLog;

using OpenTK.Graphics.OpenGL4;

namespace Defsite.IO.DataFormats;

public class ShaderData {

	public static ShaderData Default = new(new MemoryStream(Encoding.ASCII.GetBytes(@"
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

	FileInfo shader_file_info;

	Stream shader_stream;

	readonly bool from_file;
	readonly bool from_stream;

	static readonly Logger log = LogManager.GetCurrentClassLogger();

	public Dictionary<ShaderType, string> Shaders { get; private set; }

	public ShaderData(string path) {
		Load(path);
		from_file = true;
		from_stream = false;
	}

	public ShaderData(Stream stream) {
		Load(stream);
		from_file = false;
		from_stream = true;
	}
	public void Reload() {
		if(from_file) {
			Reload(shader_file_info.FullName);
		} else if(from_stream) {
			Reload(shader_stream);
		} else {
			log.Error("Can't reload shader.");
		}
	}

	void Load(string file_path) {
		var file_info = File.Exists(file_path) ? new FileInfo(file_path) : null;
		if(file_info == null) {
			log.Error($"Invalid shader file path: {file_path}");
			return;
		}

		shader_file_info = file_info;

		Load(file_info?.OpenRead());
	}

	void Load(Stream data_stream) {
		shader_stream = data_stream;
		Shaders = new Dictionary<ShaderType, string>();

		using var reader = new StreamReader(data_stream);
		var data = reader.ReadToEnd();

		var shaders = data.Trim().Split("#type", StringSplitOptions.RemoveEmptyEntries);

		foreach(var shader in shaders) {
			if(shader.Length < 1) {
				log.Error("Invalid shader");
				continue;
			}

			var source_lines = shader.Split(new[] { "\r\n", "\r", "\n", "\t" }, StringSplitOptions.RemoveEmptyEntries);
			var type = source_lines[0].Trim();

			var shader_type = (ShaderType)0;

			switch(type.ToLower()) {
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
					log.Error($"Invalid shader type: {type}");
					break;
			}

			var shader_source = string.Join("\n", source_lines.Skip(1));

			Shaders.Add(shader_type, shader_source);
		}
	}
	void Reload(string path) => Load(path);

	void Reload(Stream stream) => Load(stream);
}
