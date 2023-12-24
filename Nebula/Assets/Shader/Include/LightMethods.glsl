vec3 CalculateDirectionalLight(DirectionalLight light, vec3 normal, vec3 viewDirection)
{
	vec3 lightDirection = normalize(-light.direction);

	float diffuseCoefficient = max(dot(normal, lightDirection), 0.0);

	vec3 reflectDirection = reflect(-lightDirection, normal);
	float specularCoefficient = pow(max(dot(viewDirection, reflectDirection), 0.0), u_material.shininess);

	vec3 ambient = light.ambient * u_material.ambient;
	vec3 diffuse = light.diffuse * diffuseCoefficient * u_material.diffuse;
	vec3 specular = light.specular * specularCoefficient * u_material.specular;

	return (ambient + diffuse + specular);
};

vec3 CalculatePointLight(PointLight light, vec3 normal, vec3 vertexPosition, vec3 viewDirection)
{
	vec3 lightDirection = normalize(light.position - vertexPosition);

	float diffuseCoefficient = max(dot(normal, lightDirection), 0.0);

	vec3 reflectDirection = reflect(-lightDirection, normal);
	float specularCoefficient = pow(max(dot(viewDirection, reflectDirection), 0.0), u_material.shininess);

	float lightDistance = length(light.position - vertexPosition);
	float attenuation = 1.0 / (1.0 + light.linearFalloff * lightDistance + light.quadraticFalloff * (lightDistance * lightDistance));

	vec3 ambient = light.ambient * u_material.ambient;
	vec3 diffuse = light.diffuse * diffuseCoefficient * u_material.diffuse;
	vec3 specular = light.specular * specularCoefficient * u_material.specular;

	ambient *= attenuation;
	diffuse *= attenuation;
	specular *= attenuation;

	return (ambient + diffuse + specular);
};