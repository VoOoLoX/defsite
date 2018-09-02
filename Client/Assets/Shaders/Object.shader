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

uniform int sprite_size;
uniform vec4 outline_color;

uniform sampler2D texture_sampler;

out vec4 Output;

void main() {
	vec4 col = texture(texture_sampler, UV);

	float offset = 1.0 / sprite_size;

	if (col.a > 0.1)
		Output = col;
	else {
		float a =
			texture(texture_sampler, vec2(UV.x + offset, UV.y + offset)).a +
			texture(texture_sampler, vec2(UV.x - offset, UV.y - offset)).a +
			texture(texture_sampler, vec2(UV.x + offset, UV.y - offset)).a +
			texture(texture_sampler, vec2(UV.x - offset, UV.y + offset)).a +

			texture(texture_sampler, vec2(UV.x + offset, UV.y)).a +
			texture(texture_sampler, vec2(UV.x, UV.y - offset)).a +
			texture(texture_sampler, vec2(UV.x - offset, UV.y)).a +
			texture(texture_sampler, vec2(UV.x, UV.y + offset)).a;

		if (col.a < 1.0 && a > 0.0)
			Output = outline_color;
		else
			Output = col;
	}
}