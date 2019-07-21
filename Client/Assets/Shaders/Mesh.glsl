$vertex
#version 130

in vec3 position;

in vec2 uv_coords;

uniform mat4 view_projection;

uniform mat4 model;

out vec2 UV;

void main() {
	gl_Position = view_projection * model * vec4(position, 1.0);

	UV = uv_coords;
}

$fragment
//Credits to kiwipxl (https://github.com/kiwipxl/GLSL-shaders)
#version 130
in vec2 UV;

uniform bool override_color = false;
uniform vec4 color = vec4(0, 0, 0, 0);
uniform bool glow = false;
uniform int glow_iterations = 10;
uniform vec4 glow_color = vec4(0, 0, 0, 1);
uniform float glow_size = .5;
uniform float glow_intensity = 1.0;

uniform sampler2D texture_sampler;

out vec4 Output;

void main() {
	Output = texture(texture_sampler, UV).rgba;
	
	if (Output.a > 0 && override_color)
		Output = mix(texture(texture_sampler, UV), color, color.a);

	if (Output.a <= .5 && glow) {
		ivec2 size = textureSize(texture_sampler, 0);

		float uv_x = UV.x * size.x;
		float uv_y = UV.y * size.y;

		float sum = 0.0;
		
		int glow_iterations = 9;
		int half_iterations = glow_iterations / 2;
		
		for (int n = 0; n < glow_iterations; ++n) {
			uv_y = (UV.y * size.y) + (glow_size * float(n - half_iterations));
			float h_sum = 0.0;
			
			for (int k = half_iterations; k >= 1; --k)
			    h_sum += texelFetch(texture_sampler, ivec2(uv_x - (k * glow_size), uv_y), 0).a;
			
			h_sum += texelFetch(texture_sampler, ivec2(uv_x, uv_y), 0).a;
			
			for (int k = 1; k <= half_iterations; ++k)
			    h_sum += texelFetch(texture_sampler, ivec2(uv_x + (k * glow_size), uv_y), 0).a;
			
			sum += h_sum / glow_iterations;
		}

		Output = vec4(glow_color.rgb, (sum / glow_iterations) * glow_intensity);
	}
}