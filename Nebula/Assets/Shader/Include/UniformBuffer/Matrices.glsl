layout (std140, binding = 0) uniform ub_matrices
{
	mat4 u_viewProjection;
	mat4 u_lightSpaceViewProjection;
};