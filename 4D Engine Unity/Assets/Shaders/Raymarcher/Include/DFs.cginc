#include "MakraMathUtils.cginc"

			//sphere
float sdSphere(float3 p, float r, float wPos, float3 wRot)
{
    float4 p4 = rotWposW(p, wPos, wRot);
    return length(p4) - r;
}

			//torus
float sdTorus(float3 p, float2 s, float wPos, float3 wRot)
{
    float4 p4 = rotWposW(p, wPos, wRot);
    float3 q = float3(length(p4.xz) - s.x, p4.y, p4.w); // Use p4.w to affect the distance calculation
    return length(q) - s.y; // The distance calculation now includes the 4th dimension
}

			//capped torus
float sdCappedTorus(float3 p, float ro, float ri, float2 t, float wPos, float3 wRot)
{
    float4 p4 = rotWposW(p, wPos, wRot);
    p4.x = abs(p4.x);
    float x = (t.y * p4.x > t.x * p4.y) ? dot(p4.xy, t) : length(p4.xy);
    return sqrt(dot(p4, p4) + ro * ro - 2 * ro * x) - ri;
}

			//link
float sdLink(float3 p, float s, float ro, float ri, float wPos, float3 wRot)
{
    float4 p4 = rotWposW(p, wPos, wRot);
    // Adjust q to consider the separation in 4D by including the w component.
    float4 q = float4(p4.x, max(abs(p4.y) - s, 0), p4.z, p4.w);
    // Calculate the distance including the w component, which affects the final distance.
    float2 q_xyw = float2(length(q.xy) - ro, length(float2(q.z, q.w)));
    return length(q_xyw) - ri;
}


			//cone
float sdCone(float3 p, float2 c, float h, float wPos, float3 wRot)
{
    float4 p4 = rotWposW(p, wPos, wRot);
    p4 -= float4(0, h / 2, 0, wPos);
    float2 q = h * float2(c.x / c.y, -1.0);
    float2 w = float2(length(p4.xz), p4.y);
    float2 a = w - q * clamp(dot(w, q) / dot(q, q), 0.0, 1.0);
    float2 b = w - q * float2(clamp(w.x / q.x, 0.0, 1.0), 1.0);
    float k = sign(q.y);
    float d = min(dot(a, a), dot(b, b));
    float s = max(k * (w.x * q.y - w.y * q.x), k * (w.y - q.y));
    return sqrt(d) * sign(s);
}

			//infinite cone
float sdInfCone(float3 p, float2 c, float wPos, float3 wRot)
{
    float4 p4 = rotWposW(p, wPos, wRot);
    float2 q = float2(length(p4.xz), -p4.y);
    float d = length(q - c * max(dot(q, c), 0.0));
    return d * ((q.x * c.y - q.y * c.x < 0.0) ? -1.0 : 1.0);
}

			//plane
float sdPlane(float3 p, float3 n, float h, float wPos, float3 wRot)
{
    float4 p4 = rotWposW(p, wPos, wRot);
    float4 n4 = float4(n, 0); // Extend normal to 4D, w component is 0
    return dot(p4, n4) + h;
}

			//hexagonal prism
float sdHexPrism(float3 p, float2 h, float wPos, float3 wRot)
{
    float4 p4 = rotWposW(p, wPos, wRot);
    const float3 k = float3(-0.8660254, 0.5, 0.57735);
    p4.xyz = abs(p4.xyz);
    p4.xy -= 2.0 * min(dot(k.xy, p4.xy), 0.0) * k.xy;
    float2 d = float2(
        length(p4.xy - float2(clamp(p4.x, -k.z * h.x, k.z * h.x), h.x)) * sign(p4.y - h.x),
        p4.z - h.y);
    return min(max(d.x, d.y), 0.0) + length(max(d, 0.0));
}

			//triangular prism
float sdTriPrism(float3 p, float2 h, float wPos, float3 wRot)
{
    float4 p4 = rotWposW(p, wPos, wRot);
    float4 q = abs(p4);
    return max(q.z - h.y, max(q.x * 0.866025 + p4.y * 0.5, -p4.y) - h.x * 0.5);
}

			//capsule
float sdCapsule(float3 p, float3 a, float3 b, float r, float wPos, float3 wRot)
{
    float4 p4 = rotWposW(p, wPos, wRot);
    float4 a4 = float4(a, wPos);
    float4 b4 = float4(b, wPos);
    float4 pa = p4 - a4, ba = b4 - a4;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h) - r;
}

			//infinite cylinder
float sdInfiniteCylinder(float3 p, float3 c, float wPos, float3 wRot)
{
    float4 p4 = rotWposW(p, wPos, wRot);
    float4 c4 = float4(c, wPos);
    return length(p4.xz - c4.xy) - c4.z;
}



			//box
float sdBox(float3 p, float s, float wPos, float3 wRot)
{
    float4 p4 = rotWposW(p, wPos, wRot);
    float4 q = abs(p4) - float4(s, s, s, wPos);
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}
			//round box 
float sdRoundBox(float3 p, float s, float t, float wPos, float3 wRot)
{
    float4 p4 = rotWposW(p, wPos, wRot);
    float4 q = abs(p4) - float4(s, s, s, wPos);
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0) - t;
}


			//rounded cylinder
float sdRoundedCylinder(float3 p, float ra, float rb, float h, float wPos, float3 wRot)
{
    float4 p4 = rotWposW(p, wPos, wRot);
    float2 d = float2(length(p4.xz) - 2.0 * ra + rb, abs(p4.y) - h);
    return min(max(d.x, d.y), 0.0) + length(max(d, 0.0)) - rb;
}
			//capped cone
float sdCappedCone(float3 p, float h, float r1, float r2, float wPos, float3 wRot)
{
    float4 p4 = rotWposW(p, wPos, wRot);
    p4 -= float4(0, h / 2, 0, wPos);
    float2 q = float2(length(p4.xz), p4.y);
    float2 k1 = float2(r2, h);
    float2 k2 = float2(r2 - r1, 2.0 * h);
    float2 ca = float2(q.x - min(q.x, (q.y < 0.0) ? r1 : r2), abs(q.y) - h);
    float2 cb = q - k1 + k2 * clamp(dot(k1 - q, k2) / dot2(k2), 0.0, 1.0);
    float s = (cb.x < 0.0 && ca.y < 0.0) ? -1.0 : 1.0;
    return s * sqrt(min(dot2(ca), dot2(cb)));
}


			//box frame
float sdBoxFrame(float3 p, float3 s, float t, float wPos, float3 wRot)
{
    float4 p4 = rotWposW(p, wPos, wRot);
    p4 = abs(p4) - float4(s, wPos);
    float4 q = abs(p4 + float4(t, t, t, wPos)) - float4(t, t, t, wPos);
    return min(min(
        length(max(float4(p4.x, q.y, q.z, p4.w), 0.0)) + min(max(p4.x, max(q.y, q.z)), 0.0),
        length(max(float4(q.x, p4.y, q.z, p4.w), 0.0)) + min(max(q.x, max(p4.y, q.z)), 0.0)),
        length(max(float4(q.x, q.y, p4.z, p4.w), 0.0)) + min(max(q.x, max(q.y, p4.z)), 0.0));
}

			//solid angle
float sdSolidAngle(float3 p, float2 c, float ra, float wPos, float3 wRot)
{
    float4 p4 = rotWposW(p, wPos, wRot);
    float2 q = float2(length(p4.xz), p4.y);
    float l = length(q) - ra;
    float m = length(q - c * clamp(dot(q, c), 0.0, ra));
    return max(l, m * sign(c.y * q.x - c.x * q.y));
}

			//cut sphere
float sdCutSphere(float3 p, float r, float h, float wPos, float3 wRot)
{
    float4 p4 = rotWposW(p, wPos, wRot);
    float w = sqrt(r * r - h * h);
    float2 q = float2(length(p4.xz), p4.y);
    float s = max((h - r) * q.x * q.x + w * w * (h + r - 2.0 * q.y), h * q.x - w * q.y);
    return (s < 0.0) ? length(q) - r : ((q.x < w) ? h - q.y : length(q - float2(w, h)));
}

			//cut hollow sphere
float sdCutHollowSphere(float3 p, float r, float h, float t, float wPos, float3 wRot)
{
    float4 p4 = rotWposW(p, wPos, wRot);
    float w = sqrt(r * r - h * h);
    float2 q = float2(length(p4.xz), p4.y);
    return ((h * q.x < w * q.y) ? length(q - float2(w, h)) : abs(length(q) - r)) - t;
}

			//death star
float sdDeathStar(float3 p, float ra, float rb, float d, float wPos, float3 wRot)
{
    float4 p4 = rotWposW(p, wPos, wRot);
    float a = (ra * ra - rb * rb + d * d) / (2.0 * d);
    float b = sqrt(max(ra * ra - a * a, 0.0));
    float2 p2 = float2(p4.x, length(p4.yz));
    if (p2.x * b - p2.y * a > d * max(b - p2.y, 0.0))
        return length(p2 - float2(a, b));
    else
        return max((length(p2) - ra), -(length(p2 - float2(d, 0)) - rb));
}


			//round cone
float sdRoundCone(float3 p, float r1, float r2, float h, float wPos, float3 wRot)
{
    float4 p4 = rotWposW(p, wPos, wRot);
    float b = (r1 - r2) / h;
    float a = sqrt(1.0 - b * b);
    float2 q = float2(length(p4.xz), p4.y);
    float k = dot(q, float2(-b, a));
    if (k < 0.0)
        return length(q) - r1;
    if (k > a * h)
        return length(q - float2(0.0, h)) - r2;
    return dot(q, float2(a, b)) - r1;
}

			//ellipsoid
float sdEllipsoid(float3 p, float3 r, float wPos, float3 wRot)
{
    float4 p4 = rotWposW(p, wPos, wRot);
    float k0 = length(p4 / float4(r, 1));
    float k1 = length(p4 / (float4(r, 1) * float4(r, 1)));
    return k0 * (k0 - 1.0) / k1;
}

			//rhombus
float sdRhombus(float3 p, float la, float lb, float h, float ra, float wPos, float3 wRot)
{
    float4 p4 = rotWposW(p, wPos, wRot);
    float4 p4_swapped = float4(p4.x, p4.z, -p4.y, p4.w); // Swap and negate to match original orientation
    p4_swapped.xyz = abs(p4_swapped.xyz);
    float2 b = float2(la, lb);
    float f = clamp((ndot(b, b - 2.0 * p4_swapped.xz)) / dot(b, b), -1.0, 1.0);
    float2 q = float2(length(p4_swapped.xz - 0.5 * b * float2(1.0 - f, 1.0 + f)) * sign(p4_swapped.x * b.y + p4_swapped.z * b.x - b.x * b.y) - ra, p4_swapped.y - h);
    return min(max(q.x, q.y), 0.0) + length(max(q, 0.0));
}


			//octahedron
float sdOctahedron(float3 p, float s, float wPos, float3 wRot)
{
    float4 p4 = rotWposW(p, wPos, wRot);
    p4 = abs(p4);
    float m = p4.x + p4.y + p4.z - s;
    float4 q;
    if (3.0 * p4.x < m)
        q = p4.xyzx;
    else if (3.0 * p4.y < m)
        q = p4.yzxy;
    else if (3.0 * p4.z < m)
        q = p4.zxyz;
    else
        return m * 0.57735027;

    float k = clamp(0.5 * (q.z - q.y + s), 0.0, s);
    return length(float3(q.x, q.y - s + k, q.z - k));
}
			//pyramid
float sdPyramid(float3 p, float h, float wPos, float3 wRot)
{
    float4 p4 = rotWposW(p, wPos, wRot);
    p4 += float4(0, h / 2, 0, wPos);
    float m2 = h * h + 0.25;

    p4.xz = abs(p4.xz);
    p4.xz = (p4.z > p4.x) ? p4.zx : p4.xz;
    p4.xz -= 0.5;

    float4 q = float4(p4.z, h * p4.y - 0.5 * p4.x, h * p4.x + 0.5 * p4.y, p4.w);

    float s = max(-q.x, 0.0);
    float t = clamp((q.y - 0.5 * p4.z) / (m2 + 0.25), 0.0, 1.0);

    float a = m2 * (q.x + s) * (q.x + s) + q.y * q.y;
    float b = m2 * (q.x + 0.5 * t) * (q.x + 0.5 * t) + (q.y - m2 * t) * (q.y - m2 * t);

    float d2 = min(q.y, -q.x * m2 - q.y * 0.5) > 0.0 ? 0.0 : min(a, b);

    return sqrt((d2 + q.z * q.z) / m2) * sign(max(q.z, -p4.y));
}

			//triangle
float udTriangle(float3 p, float3 a, float3 b, float3 c, float wPos, float3 wRot)
{
    float4 p4 = rotWposW(p, wPos, wRot);
    float4 a4 = rotWposW(a, wPos, wRot);
    float4 b4 = rotWposW(b, wPos, wRot);
    float4 c4 = rotWposW(c, wPos, wRot);
    float3 ba = b4.xyz - a4.xyz;
    float3 pa = p4.xyz - a4.xyz;
    float3 cb = c4.xyz - b4.xyz;
    float3 pb = p4.xyz - b4.xyz;
    float3 ac = a4.xyz - c4.xyz;
    float3 pc = p4.xyz - c4.xyz;
    float3 nor = cross(ba, ac);

    return sqrt(
        (sign(dot(cross(ba, nor), pa)) +
         sign(dot(cross(cb, nor), pb)) +
         sign(dot(cross(ac, nor), pc)) < 2.0)
            ? min(min(
                      dot2(ba * clamp(dot(ba, pa) / dot2(ba), 0.0, 1.0) - pa),
                      dot2(cb * clamp(dot(cb, pb) / dot2(cb), 0.0, 1.0) - pb)),
                  dot2(ac * clamp(dot(ac, pc) / dot2(ac), 0.0, 1.0) - pc))
            : dot(nor, pa) * dot(nor, pa) / dot2(nor));
}

			//quad
float udQuad(float3 p, float3 a, float3 b, float3 c, float3 d, float wPos, float3 wRot)
{
    float4 p4 = rotWposW(p, wPos, wRot);
    float4 a4 = rotWposW(a, wPos, wRot);
    float4 b4 = rotWposW(b, wPos, wRot);
    float4 c4 = rotWposW(c, wPos, wRot);
    float4 d4 = rotWposW(d, wPos, wRot);
    float3 ba = b4.xyz - a4.xyz;
    float3 pa = p4.xyz - a4.xyz;
    float3 cb = c4.xyz - b4.xyz;
    float3 pb = p4.xyz - b4.xyz;
    float3 dc = d4.xyz - c4.xyz;
    float3 pc = p4.xyz - c4.xyz;
    float3 ad = a4.xyz - d4.xyz;
    float3 pd = p4.xyz - d4.xyz;
    float3 nor = cross(ba, ad);

    return sqrt(
        (sign(dot(cross(ba, nor), pa)) +
         sign(dot(cross(cb, nor), pb)) +
         sign(dot(cross(dc, nor), pc)) +
         sign(dot(cross(ad, nor), pd)) < 3.0)
            ? min(min(min(
                          dot2(ba * clamp(dot(ba, pa) / dot2(ba), 0.0, 1.0) - pa),
                          dot2(cb * clamp(dot(cb, pb) / dot2(cb), 0.0, 1.0) - pb)),
                      dot2(dc * clamp(dot(dc, pc) / dot2(dc), 0.0, 1.0) - pc)),
                  dot2(ad * clamp(dot(ad, pd) / dot2(ad), 0.0, 1.0) - pd))
            : dot(nor, pa) * dot(nor, pa) / dot2(nor));
}

			//fractal
float sdFractal(float3 z, float i, float s, float o, float wPos, float3 wRot)
{
    float4 z4 = rotWposW(z, wPos, wRot);
    int n = 0;
    while (n < i)
    {
        if (z4.x + z4.y < 0)
            z4.xy = -z4.yx;
        if (z4.x + z4.z < 0)
            z4.xz = -z4.zx;
        if (z4.y + z4.z < 0)
            z4.zy = -z4.yz;
        z4.xyz = z4.xyz * s - o * (s - 1.0);
        n++;
    }
    return (length(z4.xyz)) * pow(s, -float(n));
}
			//tesseract
float sdTesseract(float3 p, float4 s, float wPos, float3 wRot)
{
    float4 p4 = rotWposW(p, wPos, wRot);
    float4 d = abs(p4) - s;
    return min(max(d.x, max(d.y, max(d.z, d.w))), 0.0) + length(max(d, 0.0));
}

			//hypersphere
float sdHyperSphere(float3 p, float s, float wPos, float3 wRot)
{
    float4 p4 = rotWposW(p, wPos, wRot);
    return length(p4) - s;
}

			//duoCylinder
float sdDuoCylinder(float3 p, float2 r1r2, float wPos, float3 wRot)
{
    float4 p4 = rotWposW(p, wPos, wRot);
    float2 d = abs(float2(length(p4.xz), length(p4.yw))) - r1r2;
    return min(max(d.x, d.y), 0.) + length(max(d, 0.));
}
			//verticalCapsule
float sdVerticalCapsule(float3 p, float h, float r, float wPos, float3 wRot)
{
    float4 p4 = rotWposW(p, wPos, wRot);
    p4.y -= clamp(p4.y, 0.0, h);
    return length(p4) - r;
}

float sdFiveCell(float3 p, float4 a, float wPos, float3 wRot)
{
    float4 p4 = rotWposW(p, wPos, wRot);
    return (max(max(max(abs(p4.x + p4.y + (p4.w / a.w)) - p4.z, abs(p4.x - p4.y + (p4.w / a.w)) + p4.z), abs(p4.x - p4.y - (p4.w / a.w)) + p4.z), abs(p4.x + p4.y - (p4.w / a.w)) - p4.z) - a.x) / sqrt(3.);				
}

float sdSixteenCell(float3 p, float s, float wPos, float3 wRot)
{
    float4 p4 = rotWposW(p, wPos, wRot);
    p4 = abs(p4);
    return (p4.x + p4.y + p4.z + p4.w - s) * 0.57735027f;
}


			

			

			


			