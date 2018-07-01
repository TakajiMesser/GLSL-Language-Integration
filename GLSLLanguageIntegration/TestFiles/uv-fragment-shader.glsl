#version 330 core

// Interpolated values from the vertex shaders
in vec2 UV;

// Ouput data
out vec3 color;

// Values that stay constant for the whole mesh.
uniform sampler2D myTextureSampler;

void main(){

    // Output color = color of the texture at the specified UV
    color = texture( myTextureSampler, UV ).rgb; // Test comment

    int a = 4;
    float b = 4.4f; /*var
    moretests
    testy tests
    int a = asdf;
    int c = 5;*/
}