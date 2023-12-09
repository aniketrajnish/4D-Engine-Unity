float ndot(float2 a, float2 b)
{
    return a.x * b.x - a.y * b.y;
}

float dot2(float3 f)
{
    return dot(f, f);
}
			
float dot2(float2 f)
{
    return dot(f, f);
}

float sdUnion(float d1, float d2)
{
    return min(d1, d2);
}

float sdIntersection(float d1, float d2)
{
    return max(d1, d2);
}

float sdSubtraction(float d1, float d2)
{
    return max(-d1, d2);
}

float sdFMod(inout float p, float s)
{
    float h = s * .5f;
    float c = floor((p + h) / s);
    p = fmod(p + h, s) - h;
    p = fmod(-p + h, s) - h;
    return c;
}

float4 rotWposW(float3 p, float wPos, float3 wRot)
{
    float4 p4 = float4(p, wPos);
				//4d rot matrix
    p4.xz = mul(p4.xz, float2x2(cos(wRot.y), sin(wRot.y), -sin(wRot.y), cos(wRot.y)));
    p4.yz = mul(p4.yz, float2x2(cos(wRot.x), -sin(wRot.x), sin(wRot.x), cos(wRot.x)));
    p4.xy = mul(p4.xy, float2x2(cos(wRot.z), -sin(wRot.z), sin(wRot.z), cos(wRot.z)));
    p4.xw = mul(p4.xw, float2x2(cos(wRot.x), sin(wRot.x), -sin(wRot.x), cos(wRot.x)));
    p4.zw = mul(p4.zw, float2x2(cos(wRot.z), -sin(wRot.z), sin(wRot.z), cos(wRot.z)));
    p4.yw = mul(p4.yw, float2x2(cos(wRot.y), -sin(wRot.y), sin(wRot.y), cos(wRot.y)));
    return p4;
}

float3 normalizeF3(float3 f)
{
    return (f * 0.5) + 0.5;
}