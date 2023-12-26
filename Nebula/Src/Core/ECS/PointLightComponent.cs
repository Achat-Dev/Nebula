﻿using Nebula.Rendering;

namespace Nebula;

public class PointLightComponent : StartableComponent
{
    private Colour m_colour = Colour.White;
    private float m_ambientStrength = 0.2f;
    private float m_diffuseStrength = 0.5f;
    private float m_specularStrength = 1f;

    private float m_linearFalloff = 0.01f;
    private float m_quadraticFalloff = 0.1f;

    public override void OnCreate()
    {
        Lighting.AddPointLight(this);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Lighting.RemovePointLight(this);
    }

    public Vector3 GetAmbient()
    {
        return (Vector3)m_colour * m_ambientStrength;
    }

    public Vector3 GetDiffuse()
    {
        return (Vector3)m_colour * m_diffuseStrength;
    }

    public Vector3 GetSpecular()
    {
        return (Vector3)m_colour * m_specularStrength;
    }

    public float GetLinearFalloff()
    {
        return m_linearFalloff;
    }

    public float GetQuadraticFalloff()
    {
        return m_quadraticFalloff;
    }

    public Colour GetColour()
    {
        return m_colour;
    }

    public void SetColour(Colour colour)
    {
        m_colour = colour;
    }
}
