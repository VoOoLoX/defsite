#type vertex
#version 450

in vec2 v_position;
in vec2 v_texture_coordinates;
in vec4 v_color;

out vec2 f_texture_coordinates;
out vec4 f_color;

uniform mat4 u_projection;

void main() {
    gl_Position = u_projection * vec4(v_position, 0, 1);
    f_texture_coordinates = v_texture_coordinates;
    f_color = v_color;
}

#type pixel
#version 450

in vec2 f_texture_coordinates;
in vec4 f_color;

out vec4 o_color;

uniform sampler2D u_font_texture;

void main() {
    o_color = f_color * texture(u_font_texture, f_texture_coordinates);
}

