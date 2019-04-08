#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec4 vPos;
layout (location = 2) in vec4 color;

uniform mat4 view;
uniform mat4 projection;

uniform vec3 cameraUp;
uniform vec3 cameraRight;

out vec4 clr;

void main(){
    float bbSize = vPos.w;

    vec3 vertexPosition_worldspace =
        vPos.xyz
        + cameraUp * aPos.x * bbSize
        + cameraRight * aPos.y * bbSize;

    gl_Position = projection * view * vec4(vertexPosition_worldspace, 1.0);
    clr = color;
}