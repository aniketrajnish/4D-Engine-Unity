using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Mathematics.math;
using static Unity.Mathematics.MakraMathUtils;
using static RaymarchRenderer;

namespace Unity.Mathematics
{
    public class RaymarchCollision : MonoBehaviour
    {
        [SerializeField] float offset = 1.5f;
        [SerializeField] float maxMovement = 1f;
        [SerializeField] Transform[] bounds;
        Raymarcher raymarcher;

        private bool isDimesionChanging = false;    

        private void Awake()
        {
            EventCenter.RegisterEvent<OnDimensionChanging>(OnDimesionChanging);
        }

        private void OnDestroy()
        {
            EventCenter.UnRegisterEvent<OnDimensionChanging>(OnDimesionChanging);
        }

        private void Start()
        {
            raymarcher = Camera.main.GetComponent<Raymarcher>();
        }
        private void Update()
        {
            MoveToGround();
            CheckRaymarchDist(bounds);
        }
        public float GetShapeDistance(RaymarchRenderer rend, float3 p)
        {
            float wPos = rend.posW - raymarcher.wPos;
            float3 wRot = rend.rotW - raymarcher.wRot;
            
            float3 shapePos = rend.transform.position;
            float3 shapeRot = rend.transform.eulerAngles * Mathf.Deg2Rad;

            p -= shapePos;

            p.xz = mul(p.xz, float2x2(cos(shapeRot.y), sin(shapeRot.y), -sin(shapeRot.y), cos(shapeRot.y)));
            p.yz = mul(p.yz, float2x2(cos(shapeRot.x), -sin(shapeRot.x), sin(shapeRot.x), cos(shapeRot.x)));
            p.xy = mul(p.xy, float2x2(cos(shapeRot.z), -sin(shapeRot.z), sin(shapeRot.z), cos(shapeRot.z)));            

            vector12 dimensions = Helpers.GetDimensionVectors((int)rend.shape, rend.dimensions);

            switch (rend.shape)
            {
                case RaymarchRenderer.Shape.Shpere:
                    return DFs.sdSphere(p, dimensions.a, wPos, wRot);
                case RaymarchRenderer.Shape.Torus:
                    return DFs.sdTorus(p, new float2(dimensions.a, dimensions.b), wPos, wRot);
                case RaymarchRenderer.Shape.CappedTorus:
                    return DFs.sdCappedTorus(p, dimensions.a, dimensions.b, new float2(dimensions.c, dimensions.d), wPos, wRot);
                case RaymarchRenderer.Shape.Link:
                    return DFs.sdLink(p, dimensions.a, dimensions.b, dimensions.c, wPos, wRot);
                case RaymarchRenderer.Shape.Plane:
                    return DFs.sdPlane(p, new float3(dimensions.a, dimensions.b, dimensions.c), dimensions.d, wPos, wRot);
                case RaymarchRenderer.Shape.Cone:
                    return DFs.sdCone(p, new float2(dimensions.a, dimensions.b), dimensions.c, wPos, wRot);
                case RaymarchRenderer.Shape.InfCone:
                    return DFs.sdInfCone(p, new float2(dimensions.a, dimensions.b), wPos, wRot);
                case RaymarchRenderer.Shape.HexPrism:
                    return DFs.sdHexPrism(p, new float2(dimensions.a, dimensions.b), wPos, wRot);
                case RaymarchRenderer.Shape.TriPrism:
                    return DFs.sdTriPrism(p, new float2(dimensions.a, dimensions.b), wPos, wRot);
                case RaymarchRenderer.Shape.Capsule:
                    return DFs.sdCapsule(p, new float3(dimensions.a, dimensions.b, dimensions.c),
                        new float3(dimensions.d, dimensions.e, dimensions.f),dimensions.g,
                        wPos, wRot);
                case RaymarchRenderer.Shape.InfiniteCylinder:
                    return DFs.sdInfiniteCylinder(p, new float3(dimensions.a, dimensions.b, dimensions.c), wPos, wRot);
                case RaymarchRenderer.Shape.Box:
                    return DFs.sdBox(p, dimensions.a, wPos, wRot);
                case RaymarchRenderer.Shape.RoundBox:
                    return DFs.sdRoundBox(p, dimensions.a, dimensions.b, wPos, wRot);
                case RaymarchRenderer.Shape.RoundedCylinder:
                    return DFs.sdRoundedCylinder(p, dimensions.a, dimensions.b, dimensions.c, wPos, wRot);
                case RaymarchRenderer.Shape.CappedCone:
                    return DFs.sdCappedCone(p, dimensions.a, dimensions.b, dimensions.c, wPos, wRot);
                case RaymarchRenderer.Shape.BoxFrame:
                    return DFs.sdBoxFrame(p, new float3(dimensions.a, dimensions.b, dimensions.c), dimensions.d, wPos, wRot);
                case RaymarchRenderer.Shape.SolidAngle:
                    return DFs.sdSolidAngle(p, new float2(dimensions.a, dimensions.b), dimensions.c, wPos, wRot);
                case RaymarchRenderer.Shape.CutSphere:
                    return DFs.sdCutSphere(p, dimensions.a, dimensions.b, wPos, wRot);
                case RaymarchRenderer.Shape.CutHollowSphere:
                    return DFs.sdCutHollowSphere(p, dimensions.a, dimensions.b, dimensions.c, wPos, wRot);
                case RaymarchRenderer.Shape.DeathStar:
                    return DFs.sdDeathStar(p, dimensions.a, dimensions.b, dimensions.c, wPos, wRot);
                case RaymarchRenderer.Shape.RoundCone:
                    return DFs.sdRoundCone(p, dimensions.a, dimensions.b, dimensions.c, wPos, wRot);
                case RaymarchRenderer.Shape.Ellipsoid:
                    return DFs.sdEllipsoid(p, new float3(dimensions.a, dimensions.b, dimensions.c), wPos, wRot);
                case RaymarchRenderer.Shape.Rhombus:
                    return DFs.sdRhombus(p, dimensions.a, dimensions.b, dimensions.c, dimensions.d, wPos, wRot);
                case RaymarchRenderer.Shape.Octahedron:
                    return DFs.sdOctahedron(p, dimensions.a, wPos, wRot);
                case RaymarchRenderer.Shape.Pyramid:
                    return DFs.sdPyramid(p, dimensions.a, wPos, wRot);
                case RaymarchRenderer.Shape.Triangle:
                    return DFs.udTriangle(p,
                        new float3(dimensions.a, dimensions.b, dimensions.c),
                        new float3(dimensions.d, dimensions.e, dimensions.f),
                        new float3(dimensions.g, dimensions.h, dimensions.i),
                        wPos, wRot);
                case RaymarchRenderer.Shape.Quad:
                    return DFs.udQuad(p,
                        new float3(dimensions.a, dimensions.b, dimensions.c),
                        new float3(dimensions.d, dimensions.e, dimensions.f),
                        new float3(dimensions.g, dimensions.h, dimensions.i),
                        new float3(dimensions.j, dimensions.k, dimensions.l),
                        wPos, wRot);
                case RaymarchRenderer.Shape.Fractal:
                    return DFs.sdFractal(p,
                        dimensions.a, dimensions.b, dimensions.c,
                        wPos, wRot);
                case RaymarchRenderer.Shape.Tesseract:
                    return DFs.sdTesseract(p,
                        new float4(dimensions.a, dimensions.b, dimensions.c, dimensions.d),
                        wPos, wRot);
                case RaymarchRenderer.Shape.HyperSphere:
                    return DFs.sdHyperSphere(p,
                        dimensions.a,
                        wPos, wRot);
                case RaymarchRenderer.Shape.DuoCylinder:
                    return DFs.sdDuoCylinder(p,
                        new float2(dimensions.a, dimensions.b),
                        wPos, wRot);
                case RaymarchRenderer.Shape.VerticalCapsule:
                    return DFs.sdVerticalCapsule(p,
                        dimensions.a, dimensions.b,
                        wPos, wRot);
                case RaymarchRenderer.Shape.FiveCell:
                    return DFs.sdFiveCell(p,
                        new float4(dimensions.a, dimensions.b, dimensions.c, dimensions.d),
                        wPos, wRot);
                case RaymarchRenderer.Shape.SixteenCell:
                    return DFs.sdSixteenCell(p,
                        dimensions.a,
                        wPos, wRot);
            }

            return Camera.main.farClipPlane;
        }
        public RaymarchCollisionOutput DistanceField(Vector3 p)
        {
            float mod_x, mod_y, mod_z;

            if (raymarcher.loop.x != 0)
                mod_x = sdFMod(ref p.x, raymarcher.loop.x);
            if (raymarcher.loop.y != 0)
                mod_y = sdFMod(ref p.y, raymarcher.loop.y);
            if (raymarcher.loop.z != 0)
                mod_z = sdFMod(ref p.z, raymarcher.loop.z);

            float sigmaDist = Mathf.Infinity;
            RaymarchRenderer deltaCollider = null;

            foreach (var rend in raymarcher.renderers)
            {
                float deltaDist = GetShapeDistance(rend, p);
                switch (rend.operation)
                {
                    case Operation.Union:
                        //sigmaDist = sdUnion(sigmaDist, deltaDist);
                        if (deltaDist < sigmaDist)
                        {
                            sigmaDist = deltaDist;
                            deltaCollider = rend;
                        }
                        break;
                    case Operation.Intersect:
                        sigmaDist = sdIntersection(sigmaDist, deltaDist);
                        if (deltaDist < sigmaDist)
                        {
                            sigmaDist = deltaDist;
                            deltaCollider = rend;
                        }
                        break;
                    case Operation.Subtract:
                        sigmaDist = sdSubtraction(sigmaDist, deltaDist);
                        if (deltaDist < sigmaDist)
                        {
                            sigmaDist = deltaDist;
                            deltaCollider = rend;
                        }
                        break;
                }
            }
            return new RaymarchCollisionOutput(sigmaDist, deltaCollider);
        }
        void CheckRaymarchDist(Transform[] ro)
        {

            for (int i = 0; i < ro.Length; i++)
            {
                Vector3 p = ro[i].position;
                //check hit
                RaymarchCollisionOutput colliderOut = DistanceField(p);


                if (colliderOut.dist < 0) //hit
                {
                    //Debug.Log("Hit with: " + colliderOut.collider.name);
                    if (!isDimesionChanging)
                    {
                        //transform.Translate(ro[i].forward * colliderOut.dist * Time.fixedDeltaTime, Space.World);
                        EventCenter.PostEvent(new OnCollision4D(colliderOut.collider.gameObject));
                    }
                }
            }
        }

        //moves the player to the ground
        void MoveToGround()
        {
            Vector3 p = transform.position;
            //check hit

            RaymarchCollisionOutput colliderOut = DistanceField(p);

            float d = colliderOut.dist;

            d = Mathf.Min(d, maxMovement);
            //Debug.Log(d);

            if(!isDimesionChanging)
                transform.Translate(Vector3.down * d, Space.World);
        }

        private void OnDimesionChanging(OnDimensionChanging data)
        {
            isDimesionChanging = data.isChanging;
        }
    }
}
public class RaymarchCollisionOutput
{
    public float dist;
    public RaymarchRenderer collider;

    public RaymarchCollisionOutput(float _dist, RaymarchRenderer _collider)
    {
        this.dist = _dist;
        this.collider = _collider;
    }
}

