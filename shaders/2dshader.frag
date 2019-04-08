#version 330 core
in vec2 uv;

out vec4 FragColor;
uniform sampler2D ourTexture;

void main() {
    // FragColor = texture(ourTexture, uv);
    FragColor = vec4(1.0, 0.0,0.0,1.0);
}