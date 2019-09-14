#type vertex
#version 130

in vec3 position;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

void main() {
	gl_Position = projection * view * model * vec4(position, 1.0);
}

#type pixel
#version 130

uniform vec4 color;

void main() {
	gl_FragColor = color;
}