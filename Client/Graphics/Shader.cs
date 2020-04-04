using System;
using System.Collections.Generic;
using Defsite;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Client {
	public class Shader {
		Dictionary<string, int> attribute_location_cache = new Dictionary<string, int>();
		Dictionary<string, int> uniform_location_cache = new Dictionary<string, int>();

		ShaderFile shader_file;
		List<int> shaders = new List<int>();
		public int ID { get; private set; }

		public Shader(string file_path) {
			ID = GL.CreateProgram();
			GL.UseProgram(ID);

			shader_file = new ShaderFile(file_path);
			Create(shader_file);

			GL.UseProgram(0);
		}
		public void Disable() => GL.UseProgram(0);

		public void Enable() => GL.UseProgram(ID);

		public int GetAttributeLocation(string attribute) {
			if (attribute_location_cache.ContainsKey(attribute))
				return attribute_location_cache[attribute];

			var location = GL.GetAttribLocation(ID, attribute);
			attribute_location_cache.Add(attribute, location);

			return location;
		}

		public int GetUniformLocation(string uniform) {
			if (uniform_location_cache.ContainsKey(uniform))
				return uniform_location_cache[uniform];

			var location = GL.GetUniformLocation(ID, uniform);
			uniform_location_cache.Add(uniform, location);

			return location;
		}

		public void Reload() {
			shader_file.Reload();

			foreach (var shader_id in shaders) {
				GL.DetachShader(ID, shader_id);
				GL.DeleteShader(shader_id);
			}

			GL.UseProgram(0);
			GL.DeleteProgram(ID);

			ID = GL.CreateProgram();
			GL.UseProgram(ID);

			Create(shader_file);

			GL.UseProgram(0);
		}

		public void Set<T>(string uniform, T data) {
			GL.UseProgram(ID);

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

		void Create(ShaderFile file) => Create(file.Shaders);

		void Create(Dictionary<ShaderType, string> shader_list) {
			foreach (var (type, source) in shader_list) {
				var shader_id = GL.CreateShader(type);
				GL.ShaderSource(shader_id, source);
				GL.CompileShader(shader_id);

				var shader_info = GL.GetShaderInfoLog(shader_id);
				if (!string.IsNullOrEmpty(shader_info)) {
					Log.Panic($"[{type.ToString()}] Shader compile error: {shader_info}");
				}

				shaders.Add(shader_id);

				GL.AttachShader(ID, shader_id);
			}

			GL.LinkProgram(ID);
			var program_info = GL.GetProgramInfoLog(ID);
			if (!string.IsNullOrEmpty(program_info)) {
				Log.Panic($"Shader program error: {program_info}");
			}
		}

		public int this[string attr] => GetAttributeLocation(attr);
	}
}