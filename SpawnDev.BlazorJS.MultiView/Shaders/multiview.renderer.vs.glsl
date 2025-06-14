// https://stackoverflow.com/questions/24104939/rendering-a-fullscreen-quad-using-webgl/24112706#24112706
precision mediump float;

// xy = vertex position in normalized device coordinates ([-1,+1] range).
attribute vec2 vertexPosition;

varying vec2 vUV;

const vec2 scale = vec2(0.5, 0.5);

void main()
{
    vUV  = vertexPosition * scale + scale; // scale vertex attribute to [0,1] range
    gl_Position = vec4(vertexPosition.x, -vertexPosition.y, 0.0, 1.0);
}