$vertex
#version 130

in vec3 position;

in vec2 uv;

in vec3 normal;

uniform mat4 view_projection;
uniform mat4 model;

out vec2 UV;

out vec3 Normal;

void main() {
	gl_Position = view_projection * model * vec4(position, 1.0);

	UV = uv;
	
	Normal = normal;
}

$fragment
#version 130

in vec2 UV;

in vec3 Normal;

in vec3 LightVector;

uniform bool override_color = false;
uniform vec4 color = vec4(0, 0, 0, 1);
uniform vec3 light_position = vec3(0, 0, 0);
uniform vec3 light_color = vec3(1, 1, 1);
uniform sampler2D texture_sampler;

out vec4 Output;

void main() {
    vec3 norm = normalize(Normal);
    
    float diff = max(dot(norm, light_position), 0.0);
    vec4 diffuse = vec4(diff * light_color, 1.0);

	Output = texture(texture_sampler, UV) * diffuse;
	
	if (Output.a > 0 && override_color)
		Output = mix(texture(texture_sampler, UV), color, color.a) * diffuse;
}