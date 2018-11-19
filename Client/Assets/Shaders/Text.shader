$vertex
#version 130

in vec2 position;

in vec2 uv_coords;

uniform mat4 mvp;

out vec2 UV;

void main() {
	gl_Position = mvp * vec4(position, 0.0, 1.0);

	UV = uv_coords;
}

$fragment
#version 130
//Credits to kiwipxl (https://github.com/kiwipxl/GLSL-shaders)
in vec2 UV;

uniform bool glow = false;
uniform vec4 text_color;
uniform vec4 glow_color = vec4(1,0,0,1);
uniform float glow_size = .5;
uniform float glow_intensity = .35;

uniform sampler2D texture_sampler;

out vec4 Output;

void main() {
	Output = texture(texture_sampler, UV).rgba;
	if (Output.a > 0)
		Output = text_color;

	if (Output.a <= .5 && glow) {
        ivec2 size = textureSize(texture_sampler, 0);

        float uv_x = UV.x * size.x;
        float uv_y = UV.y * size.y;

        float sum = 0.0;
        for (int n = 0; n < 9; ++n) {
            uv_y = (UV.y * size.y) + (glow_size * float(n - 4.5));
            float h_sum = 0.0;
            h_sum += texelFetch(texture_sampler, ivec2(uv_x - (4.0 * glow_size), uv_y), 0).a;
            h_sum += texelFetch(texture_sampler, ivec2(uv_x - (3.0 * glow_size), uv_y), 0).a;
            h_sum += texelFetch(texture_sampler, ivec2(uv_x - (2.0 * glow_size), uv_y), 0).a;
            h_sum += texelFetch(texture_sampler, ivec2(uv_x - glow_size, uv_y), 0).a;
            h_sum += texelFetch(texture_sampler, ivec2(uv_x, uv_y), 0).a;
            h_sum += texelFetch(texture_sampler, ivec2(uv_x + glow_size, uv_y), 0).a;
            h_sum += texelFetch(texture_sampler, ivec2(uv_x + (2.0 * glow_size), uv_y), 0).a;
            h_sum += texelFetch(texture_sampler, ivec2(uv_x + (3.0 * glow_size), uv_y), 0).a;
            h_sum += texelFetch(texture_sampler, ivec2(uv_x + (4.0 * glow_size), uv_y), 0).a;
            sum += h_sum / 9.0;
        }

        Output = vec4(glow_color.rgb, (sum / 9.0) * glow_intensity);
    }
}