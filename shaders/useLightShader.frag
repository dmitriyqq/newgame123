#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D texture_diffuse1;
uniform sampler2D texture_specular1;
uniform sampler2D texture_normal1;
uniform sampler2D texture_height1;

void main()
{    
    FragColor = texture(texture_diffuse1, TexCoords);
    // vec4 FragColor2 = texture(texture_specular1, TexCoords);
    // vec4 FragColor3 = texture(texture_normal1, TexCoords);
    // vec4 FragColor4 = texture(texture_height1, TexCoords);
    // FragColor = FragColor1 / 4.0;//  + FragColor4 / 4.0;
    // FragColor.a = 1.0;
}