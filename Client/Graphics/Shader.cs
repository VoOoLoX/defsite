using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Defsite;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Client {
	public class Shader {
		Dictionary<string, int> AttributeLocationCache = new Dictionary<string, int>();
		Dictionary<string, int> UniformLocationCache = new Dictionary<string, int>();

		public Shader(FileInfo file) :
			this(File.Exists(file.FullName) ? file.FullName : "") { }

		public Shader(string file_path) {
			ID = GL.CreateProgram();
			Enable();
			Parse(file_path);
			Disable();
		}

		public int ID { get; }

		public int this[string attr] => GetAttributeLocation(attr);

		public void Parse(string file_path) {
			var path = File.Exists(file_path) ? file_path : string.Empty;
			if (path == string.Empty) Log.Error($"Invalid shader file path: {path}");

			var source = File.ReadAllText(path);

			if (!source.Contains('$')) {
				Log.Error("Please specify type of the shader by adding line '${vertex | fragment}' on top of shader source.");
				return;
			}

			var shaders = source.Split('$').ToList();

			foreach (var shader in shaders) {
				if (string.IsNullOrEmpty(shader)) continue;
				shader.Replace('\r', '\n').Replace("\n\n", "\n");
				var shdr = shader.Split('\n', 2);
				var type = shdr[0];
				var src = shdr[1];
				var shader_type = default(ShaderType);

				switch (type.ToLower().Trim()) {
					case "vertex":
						shader_type = ShaderType.VertexShader;
						break;
					case "fragment":
						shader_type = ShaderType.FragmentShader;
						break;
					case "geometry":
						shader_type = ShaderType.GeometryShader;
						break;
				}

				var id = GL.CreateShader(shader_type);
				GL.ShaderSource(id, src.Trim());
				GL.CompileShader(id);

				var shader_info = GL.GetShaderInfoLog(id);
				if (!string.IsNullOrEmpty(shader_info)) {
					Log.Panic($"[{shader_type.ToString()}] Shader compile error: {shader_info}");
				}

				GL.AttachShader(ID, id);
			}

			GL.LinkProgram(ID);
			var program_info = GL.GetProgramInfoLog(ID);
			if (!string.IsNullOrEmpty(program_info)) {
				Log.Panic($"Shader program error: {program_info}");
			}
		}

		public void Enable() => GL.UseProgram(ID);

		public void Disable() => GL.UseProgram(0);

		public int GetAttributeLocation(string attrute) {
			if (AttributeLocationCache.ContainsKey(attrute))
				return AttributeLocationCache[attrute];

			var location = GL.GetAttribLocation(ID, attrute);
			AttributeLocationCache.Add(attrute, location);

			return location;
		}

		public int GetUniformLocation(string uniform) {
			if (UniformLocationCache.ContainsKey(uniform))
				return UniformLocationCache[uniform];

			var location = GL.GetUniformLocation(ID, uniform);
			UniformLocationCache.Add(uniform, location);

			return location;
		}

		public void Set<T>(string uniform, T data) {
			Enable();
			switch (data) {
				case Matrix2 m2:
					GL.UniformMatrix2(GetUniformLocation(uniform), false, ref m2);
					break;
				case Matrix3 m3:
					GL.UniformMatrix3(GetUniformLocation(uniform), false, ref m3);
					break;
				case Matrix4 m4:
					GL.UniformMatrix4(GetUniformLocation(uniform), false, ref m4);
					break;
				case bool b:
					GL.Uniform1(GetUniformLocation(uniform), b ? 1 : 0);
					break;
				case int i:
					GL.Uniform1(GetUniformLocation(uniform), i);
					break;
				case float f:
					GL.Uniform1(GetUniformLocation(uniform), f);
					break;
				case double d:
					GL.Uniform1(GetUniformLocation(uniform), d);
					break;
				case Vector2 v2:
					GL.Uniform2(GetUniformLocation(uniform), v2);
					break;
				case Vector3 v3:
					GL.Uniform3(GetUniformLocation(uniform), v3);
					break;
				case Vector4 v4:
					GL.Uniform4(GetUniformLocation(uniform), v4);
					break;
				case Color v4:
					GL.Uniform4(GetUniformLocation(uniform), v4);
					break;
				default:
					throw new InvalidCastException();
			}
		}

		~Shader() {
			GL.DeleteProgram(ID);
		}
	}
}