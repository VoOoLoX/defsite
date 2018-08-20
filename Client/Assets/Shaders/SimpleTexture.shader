﻿#type vertex
#version 130

in vec2 position;

in vec2 uv_coords;

uniform mat4 mvp;

out vec2 UV;

void main() {
    gl_Position = mvp * vec4(position, 0.0, 1.0);

    UV = uv_coords;
}

#type fragment
#version 130

in vec2 UV;

uniform sampler2D texture_sampler;

out vec4 Output;

void main() {
    Output = texture(texture_sampler, UV).rgba;
}