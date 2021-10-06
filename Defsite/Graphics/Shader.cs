using System.Collections.Generic;
using System.Drawing;

using Defsite.IO.DataFormats;

using NLog;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Defsite.Graphics;
public class Shader {
	public int ID { get; private set; }

	readonly Dictionary<string, int> attribute_location_cache = new();
	readonly Dictionary<string, int> uniform_location_cache = new();
	readonly ShaderData shader_data;
	readonly List<int> shaders = new();

	static readonly Logger log = LogManager.GetCurrentClassLogger();

	public Shader(string file_path) {
		ID = GL.CreateProgram();

		shader_data = new ShaderData(file_path);
		Create(shader_data);
	}
	public void Disable() => GL.UseProgram(0);

	public void Enable() => GL.UseProgram(ID);

	public int GetAttributeLocation(string attribute) {
		if(attribute_location_cache.ContainsKey(attribute)) {
			return attribute_location_cache[attribute];
		}

		var location = GL.GetAttribLocation(ID, attribute);
		attribute_location_cache.Add(attribute, location);

		return location;
	}

	public int GetUniformLocation(string uniform) {
		if(uniform_location_cache.ContainsKey(uniform)) {
			return uniform_location_cache[uniform];
		}

		var location = GL.GetUniformLocation(ID, uniform);
		uniform_location_cache.Add(uniform, location);

		return location;
	}

	public void Reload() {
		shader_data.Reload();

		foreach(var shader_id in shaders) {
			GL.DetachShader(ID, shader_id);
			GL.DeleteShader(shader_id);
		}

		GL.UseProgram(0);
		GL.DeleteProgram(ID);

		ID = GL.CreateProgram();
		GL.UseProgram(ID);

		Create(shader_data);

		GL.UseProgram(0);
	}

	public void Set<T>(string uniform, T data) {
		GL.UseProgram(ID);
		var location = GetUniformLocation(uniform);
		switch(data) {
			case Matrix2 m2:
				GL.UniformMatrix2(location, false, ref m2);
				break;
			case Matrix3 m3:
				GL.UniformMatrix3(location, false, ref m3);
				break;
			case Matrix4 m4:
				GL.UniformMatrix4(location, false, ref m4);
				break;
			case bool b:
				GL.Uniform1(location, b ? 1 : 0);
				break;
			case int i:
				GL.Uniform1(location, i);
				break;
			case float f:
				GL.Uniform1(location, f);
				break;
			case double d:
				GL.Uniform1(location, d);
				break;
			case Vector2 v2:
				GL.Uniform2(location, v2);
				break;
			case Vector3 v3:
				GL.Uniform3(location, v3);
				break;
			case Vector4 v4:
				GL.Uniform4(location, v4);
				break;
			case Color v4:
				GL.Uniform4(location, v4);
				break;
			default:
				log.Error("Invalid uniform data type");
				break;
		}
	}

	void Create(ShaderData data) => Create(data.Shaders);

	void Create(Dictionary<ShaderType, string> shader_list) {
		foreach(var (type, source) in shader_list) {
			var shader_id = GL.CreateShader(type);
			GL.ShaderSource(shader_id, source);
			GL.CompileShader(shader_id);

			var shader_info = GL.GetShaderInfoLog(shader_id);
			if(!string.IsNullOrEmpty(shader_info)) {
				log.Error($"[{type}] Shader compile error: {shader_info}");
			}

			shaders.Add(shader_id);

			GL.AttachShader(ID, shader_id);
		}

		GL.LinkProgram(ID);
		var program_info = GL.GetProgramInfoLog(ID);
		if(!string.IsNullOrEmpty(program_info)) {
			log.Error($"Shader program error: {program_info}");
		}
	}

	public int this[string attr] => GetAttributeLocation(attr);
}
