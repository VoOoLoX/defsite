#type vertex
#version 450

in vec3 v_position;
in vec4 v_color;
in vec2 v_texture_coordinates;

out vec2 f_texture_coordinates;
out vec4 f_color;

uniform mat4 u_projection;
uniform mat4 u_view;
uniform mat4 u_model;

void main() {
	gl_Position = u_projection * u_view * u_model * vec4(v_position, 1.0);
	f_texture_coordinates = v_texture_coordinates;
	f_color = v_color;
}

#type pixel
#version 450

in vec2 f_texture_coordinates;
in vec4 f_color;

out vec4 o_color;

uniform sampler2D u_texture_sampler;

void main() {
	o_color = f_color * texture(u_texture_sampler, f_texture_coordinates);
}