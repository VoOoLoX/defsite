$vertex
#version 130

in vec2 position;

in vec2 uv_coords;

uniform mat4 mvp;

out vec2 UV;

void main() {
	gl_Position = mvp * vec4(position, 0.0, 1.0);

	UV = uv_coords;
}

$fragment
#version 130

in vec2 UV;

uniform vec4 text_color;

uniform sampler2D texture_sampler;

out vec4 Output;

void main() {
	vec4 pixel = texture(texture_sampler, UV).rgba;
	if (pixel.a > 0)
		pixel = text_color;
	Output = pixel;
}