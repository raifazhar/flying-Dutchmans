inline float2 voronoi_noise_randomVector(float2 UV, float offset)
{
    float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
    UV = frac(sin(mul(UV, m)) * 46839.32);
    return float2(sin(UV.y * +offset) * 0.5 + 0.5, cos(UV.x * offset) * 0.5 + 0.5);
}
 
void Voronoi_float(float2 UV, float AngleOffset, float CellDensity, out float2 Out, out float Cells)
{
    float2 g = floor(UV * CellDensity);
    float2 f = frac(UV * CellDensity);
    float3 res = float3(8.0, 8.0, 8.0);
 
    for (int y = -1; y <= 1; y++)
    {
        for (int x = -1; x <= 1; x++)
        {
            float2 lattice = float2(x, y);
            float2 offset = voronoi_noise_randomVector(g + lattice, AngleOffset);
            float2 v = lattice + offset - f;
            float d = dot(v, v);
             
            if (d < res.x)
            {
                res.y = res.x;
                res.x = d;
                res.z = offset.x;
            }
            else if (d < res.y)
            {
                res.y = d;
            }
        }
    }
 
    Out = float2(sqrt(res.x), sqrt(res.y));
    Cells = res.z;
}