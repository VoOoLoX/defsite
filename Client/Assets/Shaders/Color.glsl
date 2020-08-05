#type vertex
#version 330

layout(location = 0) in vec3 v_position;
layout(location = 1) in vec4 v_color;
layout(location = 2) in vec2 v_texture_coordinates;
layout(location = 3) in vec3 v_normal;

uniform mat4 u_projection;
uniform mat4 u_view;
uniform mat4 u_model;

out vec4 f_color;

void main() {
	gl_Position = u_projection * u_view * u_model * vec4(v_position, 1.0);
	f_color = v_color;
}

#type pixel
#version 330

in vec4 f_color;

void main() {
	gl_FragColor = f_color;
}