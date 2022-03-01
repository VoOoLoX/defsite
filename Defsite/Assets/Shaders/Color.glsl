#type vertex
#version 450

in vec4 v_position;
in vec4 v_color;

out vec4 f_color;

uniform mat4 u_projection;
uniform mat4 u_view;
uniform mat4 u_model;

void main() {
	f_color = v_color;
	gl_Position = u_projection * u_view * u_model * v_position;
}

#type pixel
#version 450

in vec4 f_color;

out vec4 o_color;

void main() {
	o_color = f_color;
}