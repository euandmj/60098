#version 330 core

in vec3 vPosition;
out vec3 f_texcoord;

uniform mat4 projection;
uniform mat4 view;

void main()
{
    gl_Position = projection * view * vec4(vPosition, 1.0);
    f_texcoord = vPosition;
}  