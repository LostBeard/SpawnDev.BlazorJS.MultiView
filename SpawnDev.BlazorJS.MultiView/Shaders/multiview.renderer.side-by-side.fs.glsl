// Multiview to many by Todd Tanner

#include<multiview.renderer.base.fs.glsl>

void main(void)
{
    //vec4 uiColor = uiColor(vUV);
    vec4 ret = pseudoSBS(vUV);
    //ret.rgb = mix(ret.rgb, uiColor.rgb, uiColor.a);
	//ret.a = 1.0;
    gl_FragColor = ret;
}
