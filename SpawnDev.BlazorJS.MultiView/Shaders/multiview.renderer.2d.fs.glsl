// Multiview to many by Todd Tanner

#include<multiview.renderer.base.fs.glsl>

void main(void)
{
	gl_FragColor = texture2D(videoSampler, vUV);
}
