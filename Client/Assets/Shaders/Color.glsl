#type vertex
#version 330

layout(location = 0) in vec3 position;
layout(location = 1) in vec4 color;
layout(location = 2) in vec2 texture_coordinates;
layout(location = 3) in vec3 normal;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

out vec4 COLOR;

void main() {
	gl_Position = projection * view * model * vec4(position, 1.0);
	COLOR = color;
}

#type pixel
#version 330

in vec4 COLOR;

void main() {
	gl_FragColor = COLOR;
}