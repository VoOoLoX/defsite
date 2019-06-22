$vertex
#version 130

in vec2 position;

uniform mat4 mvp;

void main() {
	gl_Position = mvp * vec4(position, 0.0, 1.0);
}

$fragment
#version 130

uniform vec4 color;

void main() {
	gl_FragColor = color;
}