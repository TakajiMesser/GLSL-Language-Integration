#version 440

uniform vec3 ambientColor;
uniform vec3 diffuseColor;
uniform vec3 specularColor;
uniform float specularExponent;

uniform sampler2D diffuseMap;
uniform sampler2D normalMap;
uniform sampler2D specularMap;
uniform sampler2D parallaxMap;

uniform int useDiffuseMap;
uniform int useNormalMap;
uniform int useSpecularMap;
uniform int useParallaxMap;

uniform vec3 cameraPosition;

in vec3 fPosition;
in vec4 fClipPosition;
in vec4 fPreviousClipPosition;
in vec3 fNormal;
in vec3 fTangent;
in vec4 fColor;
in vec2 fUV;

vec2 calculateParallaxMap(vec2 texCoords)
{
    vec2 displacedTexCoords = texCoords - p;
    if (displacedTexCoords.x > 1.0 || displacedTexCoords.x < 0.0 || displacedTexCoords.y > 1.0 || displacedTexCoords.y < 0.0)
    {
        discard;
    }

    if (displacedTexCoords.x > 1.0 || displacedTexCoords.x < 0.0 || displacedTexCoords.y > 1.0 || displacedTexCoords.y < 0.0)
    {
        discard;

        if (displacedTexCoords.x > 1.0 || displacedTexCoords.x < 0.0 || displacedTexCoords.y > 1.0 || displacedTexCoords.y < 0.0)
        {
            discard;
        }
    }

    return displacedTexCoords;
}