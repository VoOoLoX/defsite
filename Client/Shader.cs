using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Client {
	public class Shader {
		public List<(int, ShaderType, string)> Shaders { get; private set; }
		public int ID { get; private set; }

		public Shader(FileInfo file) :
			this(File.Exists(file.FullName) ? file.FullName : "") {
		}

		public Shader(string file) {
			Shaders = new List<(int, ShaderType, string)>();
			ID = GL.CreateProgram();
			Enable();
			Parse(File.Exists(file) ? File.ReadAllText(file) : "");
			Disable();
		}

		public void Parse(string source) {
			var shaders = new List<string>();

			if (!source.Contains("#type ")) {
				Console.WriteLine("Please specify type of the shader by adding line '#type {vertex | fragment}' on top of shader source.");
				return;
			}

			shaders = source.Split("#type ").ToList();

			foreach (var shader in shaders) {
				if (string.IsNullOrEmpty(shader))
					continue;
				var shdr = shader.Split(Environment.NewLine, 2);
				var type = shdr[0];
				var src = shdr[1];
				var t = default(ShaderType);

				//Console.WriteLine(type);
				//Console.WriteLine("###");
				//Console.WriteLine(src);
				//Console.WriteLine("###############################");

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

				GL.GetShader(id, ShaderParameter.CompileStatus, out int error_code);

				if ((ErrorCode)error_code != ErrorCode.NoError) {
					var info = GL.GetShaderInfoLog(id);
					Console.WriteLine(info);
				}

				Shaders.Add((id, t, src.Trim()));

				GL.AttachShader(ID, id);
			}

			GL.LinkProgram(ID);
		}

		public void Enable() => GL.UseProgram(ID);

		public void Disable() => GL.UseProgram(0);

		public int GetAttribute(string attr) => GL.GetAttribLocation(ID, attr);

		public int GetUniform(string unif) => GL.GetUniformLocation(ID, unif);
				
		public void SetUniform<T>(string unif, T data) {
			Enable();
			switch(data) {
				case Matrix2 m2:
					GL.UniformMatrix2(GetUniform(unif), false, ref m2);
					break;
				case Matrix3 m3:
					GL.UniformMatrix3(GetUniform(unif), false, ref m3);
					break;
				case Matrix4 m4:
					GL.UniformMatrix4(GetUniform(unif), false, ref m4);
					break;
				case int i:
					GL.Uniform1(GetUniform(unif), i);
					break;
				case float f:
					GL.Uniform1(GetUniform(unif), f);
					break;
				case double d:
					GL.Uniform1(GetUniform(unif), d);
					break;
				case Vector2 v2:
					GL.Uniform2(GetUniform(unif), v2);
					break;
				case Vector3 v3:
					GL.Uniform3(GetUniform(unif), v3);
					break;
				case Vector4 v4:
					GL.Uniform4(GetUniform(unif), v4);
					break;
				case Color v4:
					GL.Uniform4(GetUniform(unif), v4);
					break;
				default:
					throw (new InvalidCastException());
			}
		}
	}
}
