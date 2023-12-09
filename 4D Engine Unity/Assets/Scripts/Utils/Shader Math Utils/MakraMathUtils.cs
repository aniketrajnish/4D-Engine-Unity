using UnityEngine;
using static Unity.Mathematics.math;

namespace Unity.Mathematics
{
    public static class MakraMathUtils
    {
        public static Vector2 Abs(Vector2 v)
        {
            return new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));
        }
        public static Vector3 Abs(Vector3 v)
        {
            return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
        }
        public static Vector4 Abs(Vector4 v)
        {
            return new Vector4(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z), Mathf.Abs(v.w));
        }
        public static Vector2 Max(Vector2 vec1, Vector2 vec2)
        {
            return new Vector2(Mathf.Max(vec1.x, vec2.x), Mathf.Max(vec1.y, vec2.y));
        }
        public static Vector2 Min(Vector2 vec1, Vector2 vec2)
        {
            return new Vector2(Mathf.Max(vec1.x, vec2.x), Mathf.Max(vec1.y, vec2.y));
        }
        public static Vector3 Max(Vector3 vec1, Vector3 vec2)
        {
            return new Vector3(Mathf.Max(vec1.x, vec2.x), Mathf.Max(vec1.y, vec2.y), Mathf.Max(vec1.z, vec2.z));
        }
        public static Vector3 Min(Vector3 vec1, Vector3 vec2)
        {
            return new Vector3(Mathf.Min(vec1.x, vec2.x), Mathf.Min(vec1.y, vec2.y), Mathf.Min(vec1.z, vec2.z));
        }
        public static Vector4 Max(Vector4 vec1, Vector4 vec2)
        {
            return new Vector4(Mathf.Max(vec1.x, vec2.x), Mathf.Max(vec1.y, vec2.y), Mathf.Max(vec1.z, vec2.z), Mathf.Max(vec1.w, vec2.w));
        }
        public static Vector4 Min(Vector4 vec1, Vector4 vec2)
        {
            return new Vector4(Mathf.Min(vec1.x, vec2.x), Mathf.Min(vec1.y, vec2.y), Mathf.Min(vec1.z, vec2.z), Mathf.Min(vec1.w, vec2.w));
        }
        public static float dot2(float3 f)
        {
            return dot(f, f);
        }
        public static float dot2(float2 f)
        {
            return dot(f, f);
        }
        public static float ndot(float2 a, float2 b)
        {
            return a.x * b.x - a.y * b.y;
        }
        public static float sdUnion(float d1, float d2)
        {
            return min(d1, d2);
        }
        public static float sdIntersection(float d1, float d2)
        {
            return max(d1, d2);
        }
        public static float sdSubtraction(float d1, float d2)
        {
            return max(-d1, d2);
        }
        public static float sdFMod(ref float p, float s)
        {
            float h = s * 0.5f;
            float c = floor((p + h) / s);
            p = fmod(p + h, s) - h;
            p = fmod(-p + h, s) - h;
            return c;
        }
        public static float4 rotWposW(float3 p, float wPos, float3 wRot)
        {
            float4 p4 = new float4(p, wPos);

            wRot *= math.radians(1f); 

            p4.xz = mul(p4.xz, new float2x2(cos(wRot.y), sin(wRot.y), -sin(wRot.y), cos(wRot.y)));
            p4.yz = mul(p4.yz, new float2x2(cos(wRot.x), -sin(wRot.x), sin(wRot.x), cos(wRot.x)));
            p4.xy = mul(p4.xy, new float2x2(cos(wRot.z), -sin(wRot.z), sin(wRot.z), cos(wRot.z)));
            p4.xw = mul(p4.xw, new float2x2(cos(wRot.x), sin(wRot.x), -sin(wRot.x), cos(wRot.x)));
            p4.zw = mul(p4.zw, new float2x2(cos(wRot.z), -sin(wRot.z), sin(wRot.z), cos(wRot.z)));
            p4.yw = mul(p4.yw, new float2x2(cos(wRot.y), -sin(wRot.y), sin(wRot.y), cos(wRot.y)));

            return p4;
        }        
    }
}
