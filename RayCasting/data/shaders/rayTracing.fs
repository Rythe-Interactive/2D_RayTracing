#ifdef GL_ES
precision mediump float;
precision mediump int;
#endif

uniform sampler2D texture;

varying vec4 vertColor;
varying vec4 vertTexCoord;

uniform vec2 rayHitPosition;
uniform vec4 rayHitColor;

void main() 
{
    if(texture2D(texture, vertTexCoord.st).a == 0) gl_FragColor = vec4(0,0,0,0);
    //gl_FragColor = texture2D(texture, vertTexCoord.st) * vertColor;
    else
    {
        vec2 toLight = (rayHitPosition/500)-vertTexCoord.xy;
        float distToLight = max(length(toLight), 0);
        vec4 textureColor = texture2D(texture, vertTexCoord.st) * vertColor;
        gl_FragColor = textureColor * rayHitColor / (distToLight+1);
    }
}