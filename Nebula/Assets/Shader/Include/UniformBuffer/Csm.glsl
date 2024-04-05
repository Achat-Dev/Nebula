#replace CASCADE_COUNT

layout (std140, binding = 3) uniform ub_csm
{
    float u_cascadeDistances[CASCADE_COUNT];
    mat4 u_directionalLightViewProjections[CASCADE_COUNT];
};