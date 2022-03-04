#type vertex
#version 450

in vec4 v_position;
in vec4 v_color;
in vec2 v_texture_coordinates;
in float v_texture_index;

out vec2 f_texture_coordinates;
out vec4 f_color;

out flat float f_texture_index;

uniform mat4 u_projection;
uniform mat4 u_view;
uniform mat4 u_model;

void main() {
	gl_Position = u_projection * u_view * u_model * v_position;
	f_texture_coordinates = v_texture_coordinates;
	f_color = v_color;
	f_texture_index = v_texture_index;
}

#type pixel
#version 450

in vec2 f_texture_coordinates;
in vec4 f_color;
in float f_texture_index;

out vec4 o_color;

uniform sampler2D u_textures[16];

void main() {
	vec4 texture_color = f_color;
	
	switch(int(f_texture_index))
	{
		case  0: texture_color *= texture(u_textures[ 0], f_texture_coordinates); break;
		case  1: texture_color *= texture(u_textures[ 1], f_texture_coordinates); break;
		case  2: texture_color *= texture(u_textures[ 2], f_texture_coordinates); break;
		case  3: texture_color *= texture(u_textures[ 3], f_texture_coordinates); break;
		case  4: texture_color *= texture(u_textures[ 4], f_texture_coordinates); break;
		case  5: texture_color *= texture(u_textures[ 5], f_texture_coordinates); break;
		case  6: texture_color *= texture(u_textures[ 6], f_texture_coordinates); break;
		case  7: texture_color *= texture(u_textures[ 7], f_texture_coordinates); break;
		case  8: texture_color *= texture(u_textures[ 8], f_texture_coordinates); break;
		case  9: texture_color *= texture(u_textures[ 9], f_texture_coordinates); break;
		case 10: texture_color *= texture(u_textures[10], f_texture_coordinates); break;
		case 11: texture_color *= texture(u_textures[11], f_texture_coordinates); break;
		case 12: texture_color *= texture(u_textures[12], f_texture_coordinates); break;
		case 13: texture_color *= texture(u_textures[13], f_texture_coordinates); break;
		case 14: texture_color *= texture(u_textures[14], f_texture_coordinates); break;
		case 15: texture_color *= texture(u_textures[15], f_texture_coordinates); break;
	}

	o_color = texture_color;
}