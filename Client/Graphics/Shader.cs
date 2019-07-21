using System;
using System.IO;
using System.Linq;
using Defsite;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Client {
	public class Shader {
		public Shader(FileInfo file) :
			this(File.Exists(file.FullName) ? file.FullName : "") {
		}

		public Shader(string file) {
			ID = GL.CreateProgram();
			Enable();
			Parse(File.Exists(file) ? File.ReadAllText(file) : "");
			Disable();
		}

		public int ID { get; }

		public int this[string attr] => GetAttributeLocation(attr);

		public void Parse(string source) {
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
				var t = default(ShaderType);

				switch (type.ToLower().Trim()) {
					case "vertex":
						t = ShaderType.VertexShader;
						break;
					case "fragment":
						t = ShaderType.FragmentShader;
						break;
				}

				var id = GL.CreateShader(t);
				GL.ShaderSource(id, src.Trim());
				GL.CompileShader(id);

				var shader_info = GL.GetShaderInfoLog(id);
				if (!string.IsNullOrEmpty(shader_info)) {
					Log.Error($"Shader compile error: {shader_info}");
					Environment.Exit(1);
				}

				GL.AttachShader(ID, id);
			}

			GL.LinkProgram(ID);
			var program_info = GL.GetProgramInfoLog(ID);
			if (!string.IsNullOrEmpty(program_info)) {
				Log.Error($"Shader program error: {program_info}");
				Environment.Exit(1);
			}
		}

		public void Enable() {
			GL.UseProgram(ID);
		}

		public void Disable() {
			GL.UseProgram(0);
		}

		public int GetAttributeLocation(string attr) {
			return GL.GetAttribLocation(ID, attr);
		}

		public int Get(string unif) {
			return GL.GetUniformLocation(ID, unif);
		}

		public void Set<T>(string unif, T data) {
			Enable();
			switch (data) {
				case Matrix2 m2:
					GL.UniformMatrix2(Get(unif), false, ref m2);
					break;
				case Matrix3 m3:
					GL.UniformMatrix3(Get(unif), false, ref m3);
					break;
				case Matrix4 m4:
					GL.UniformMatrix4(Get(unif), false, ref m4);
					break;
				case bool b:
					GL.Uniform1(Get(unif), b ? 1 : 0);
					break;
				case int i:
					GL.Uniform1(Get(unif), i);
					break;
				case float f:
					GL.Uniform1(Get(unif), f);
					break;
				case double d:
					GL.Uniform1(Get(unif), d);
					break;
				case Vector2 v2:
					GL.Uniform2(Get(unif), v2);
					break;
				case Vector3 v3:
					GL.Uniform3(Get(unif), v3);
					break;
				case Vector4 v4:
					GL.Uniform4(Get(unif), v4);
					break;
				case Color v4:
					GL.Uniform4(Get(unif), v4);
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