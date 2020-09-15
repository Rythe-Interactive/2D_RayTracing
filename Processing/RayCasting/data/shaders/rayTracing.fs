#ifdef GL_ES
precision mediump float;
precision mediump int;
#endif

uniform sampler2D texture;

varying vec4 vertColor;
varying vec4 vertTexCoord;
in vec3 fragPosition;

uniform vec2 rayHitPosition[100];
uniform vec4 rayHitColor[100];
uniform int rayCount;

void main() 
{
    vec4 color;
    for(int i = 0; i < rayCount; ++i)
    {
        vec2 toLight = rayHitPosition[i]-fragPosition.xy;
        float distToLight = max(length(toLight), 0) * 0.1;
        vec4 distToLightVec = vec4(distToLight, distToLight, distToLight, 1);
        color += rayHitColor[i] / distToLightVec;
    }
    vec4 textureColor = texture2D(texture, vertTexCoord.st) * vertColor;
    gl_FragColor = textureColor * color;
}