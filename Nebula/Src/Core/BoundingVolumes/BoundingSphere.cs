namespace Nebula;

internal struct BoundingSphere
{
    public readonly Vector3 Center;
    public readonly float Radius;

    public BoundingSphere(Frustum frustum)
    {
        Center = frustum.GetCenter();
        Vector3[] corners = frustum.GetCorners();

        float maxSqrtDistance = float.NegativeInfinity;

        for (int i = 0; i < corners.Length; i++)
        {
            for (int j = 0; j < corners.Length; j++)
            {
                float sqrtDistance = Vector3.SqrDistance(corners[i], corners[j]);
                if (sqrtDistance > maxSqrtDistance)
                {
                    maxSqrtDistance = sqrtDistance;
                }
            }
        }

        Radius = MathF.Sqrt(maxSqrtDistance);
    }
}
