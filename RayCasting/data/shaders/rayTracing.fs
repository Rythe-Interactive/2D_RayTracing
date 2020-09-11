#ifdef GL_ES
precision mediump float;
precision mediump int;
#endif

uniform sampler2D texture;

varying vec4 vertColor;
varying vec4 vertTexCoord;
in vec3 fragPosition;

uniform vec2 rayHitPosition;
uniform vec4 rayHitColor;

void main() 
{
    vec2 toLight = rayHitPosition-fragPosition.xy;
    float distToLight = max(length(toLight), 0);
    vec4 distToLightVec = vec4(distToLight, distToLight, distToLight, 1);
    vec4 textureColor = texture2D(texture, vertTexCoord.st) * vertColor;
    gl_FragColor = (textureColor * rayHitColor / distToLightVec) * 20;
}