// 2D+Z multi-view by Todd Tanner (LostBeard)
// Improvements welcomed
// This is a prtial fragment shader that can be included to allow easy access to psuedo-views 
// from various angles that will be generated using the depth and rgb data

precision highp float;

// uniforms required for PostProcess
varying vec2 vUV;
// overlay texture
uniform sampler2D textureSampler;
// custom uniforms
// frame image
uniform sampler2D videoSampler;
// frame depthmap
// expected to be 1 byte per pixel representing the rgb pixel's depth in videoSampler with the same xy position
// the depth value will be in the alpha channel
uniform sampler2D depthSampler;
uniform bool views_index_invert_x; // false 0x is left, true 0x is right
uniform float rC0[4];
uniform int rI0[1];

const vec4 colorBlack = vec4(0.0, 0.0, 0.0, 1.0);

float getDepth(vec2 uv){
	return texture2D(depthSampler, uv).a;
}

vec4 viewColor2DZ(float view_index, vec2 view_uv) {
	vec4 o = vec4(0.0, 0.0, 0.0, 0.5);
	if (views_index_invert_x) {
		view_index *= -1.0;
	}	
	float sep_max_x = rC0[0] * abs(view_index);
	float pixel_width = rC0[1];
	float pixel_width_half = rC0[2];
	float offset_f = rC0[3];
	if (view_index == 0.0 || sep_max_x < pixel_width) {
		o = texture2D(videoSampler, view_uv);
	}
	else {
		//vec4 d = viewColorTiled(1.0, vUV);
		// compute r using l and d
		float pDepth;
		vec2 uv = view_uv;
		vec2 uvNext = view_uv;
		float cur_depth = -2.0;
		float dest_x = 0.0;
		float diff_x = 0.0;
		float cur_coord_x = 0.0;
		float lowestDepthDiff = 1.0;
		float lowestDepth = 1.0;
		float lowestDepthX = 0.0;
		float shiftMode = view_index > 0.0 ? -1.0 : 1.0;	// ++-+
		float pixel_width_signed = pixel_width * shiftMode;
		float sep_max_x_signed = sep_max_x * shiftMode;
		float offset_f_signed = offset_f * sep_max_x_signed;
		float start_x = uv.x - sep_max_x_signed + offset_f_signed;
		start_x = start_x - mod(start_x, pixel_width);
		//uvNext.x = start_x;
		//uvNext.x = uv.x - sep_max_x; // + (pixel_width * 2.0);
		//uvNext.x += pixel_width * shiftMode * 2.0;
		for (int n = 0; n < 100; n++)
		{
			if (n >= rI0[0]) break;
			uvNext.x = start_x + (pixel_width_signed * float(n));
			pDepth = getDepth(uvNext.xy);
			//pDepth = clamp(pDepth, 0.0, 1.0);
			dest_x = uvNext.x + (pDepth * sep_max_x_signed) - offset_f_signed;
			diff_x = abs(uv.x - dest_x);
			if (diff_x <= pixel_width && cur_depth <= pDepth)
			{
				cur_depth = pDepth;
				cur_coord_x = uvNext.x;
				o.a = 1.0;
			}
			if (pDepth <= lowestDepth && diff_x <= lowestDepthDiff + pixel_width) {
				lowestDepthDiff = diff_x;
				lowestDepth = pDepth;
				lowestDepthX = uvNext.x;
			}
			//uvNext.x += pixel_width; //
			//uvNext.x = (uv.x - sep_max_x) + (pixel_width * n);
			//if (uvNext.x >= uv.x + sep_max_x) test += 1.0;
		}
		if (o.a == 1.0) {
			//o = texture2D(mapToShift, vec2(cur_coord_x - (pixel_width * shiftMode), uvNext.y));
			o = texture2D(videoSampler, vec2(cur_coord_x, uvNext.y));
			//frag_out.c1.r = cur_depth;
		}
		else
		{
			// fill
			o = texture2D(videoSampler, vec2(lowestDepthX, uvNext.y));
			//frag_out.c1.r = lowestDepth; // vec4(lowestDepth, lowestDepth, lowestDepth, 1.0);
		}
	}
	return o;
}

vec4 viewColor(float view_index, vec2 view_uv) {
	return viewColor2DZ(view_index, view_uv);
}

vec4 pseudoSBS(vec2 view_uv) {
	if (view_uv.x > 0.5) {
		return viewColor2DZ(1.0, vec2((view_uv.x - 0.5) * 2.0, view_uv.y));
	}
	else 
	{
		return viewColor2DZ(0.0, vec2(view_uv.x * 2.0, view_uv.y));
	}
}

vec4 pseudo2DZ(vec2 view_uv) {
	if (view_uv.x > 0.5) {
		float z = getDepth(vec2((view_uv.x - 0.5) * 2.0, view_uv.y));
		return vec4(z, z, z, 1.0);
	}
	else 
	{
		return viewColor2DZ(0.0, vec2(view_uv.x * 2.0, view_uv.y));
	}
}

vec4 uiColor(vec2 uv) {
	return texture2D(textureSampler, uv);
}


