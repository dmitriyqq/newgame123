#version 330 core
out vec4 FragColor;
in vec4 clr;
uniform float time;
void main()
{
    FragColor = vec4(clr);
}