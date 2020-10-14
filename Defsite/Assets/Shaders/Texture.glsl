#type vertex
#version 330

layout(location = 0) in vec3 v_position;
layout(location = 1) in vec4 v_color;
layout(location = 2) in vec2 v_texture_coordinates;

uniform mat4 u_projection;
uniform mat4 u_view;
uniform mat4 u_model;

out vec2 f_texture_coordinates;
out vec4 f_color;

void main() {
	gl_Position = u_projection * u_view * u_model * vec4(v_position, 1.0);
	f_texture_coordinates = v_texture_coordinates;
	f_color = v_color;
}

#type pixel
#version 330

uniform sampler2D u_texture_sampler;

in vec2 f_texture_coordinates;
in vec4 f_color;

void main() {
	gl_FragColor = texture(u_texture_sampler, f_texture_coordinates) * f_color;
}