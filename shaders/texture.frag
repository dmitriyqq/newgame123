#version 330 core
out vec4 FragColor;

in vec2 TexCoords;
in vec3 Normal;
in vec3 FragPos;

uniform sampler2D ourTexture;

void main()
{    
     FragColor = vec4(1.0f, 1.0f, 1.0f, 1.0f);
    // FragColor = texture(ourTexture, TexCoords);
    // FragColor = texture(texture_diffuse1, TexCoords);
}