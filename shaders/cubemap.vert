#version 330 core
layout (location = 0) in vec3 aPos;

out vec3 TexCoords;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

void main()
{
    TexCoords = aPos;
    mat4 no_translation = view;
    no_translation[3] = vec4(0, 0, 0, 1);
    gl_Position = projection * no_translation * model * vec4(aPos, 1.0);
}