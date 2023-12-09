using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq.Expressions;

[ExecuteInEditMode]
public class Raymarcher : SceneViewFilter
{
    List<ComputeBuffer> disposable = new List<ComputeBuffer>();
    [HideInInspector] public List<RaymarchRenderer> renderers;
    public Properties shapeProperties;
    ComputeBuffer shapeBuffer;
    Material raymarchMaterial;
    private Camera _cam;
    [Header("General Settings")]
    [SerializeField] Shader shader;
    [SerializeField] Light sun;
    [SerializeField] public Vector3 loop;
    [Header("4D Settings")]
    [SerializeField] public float wPos;
    [SerializeField] public Vector3 wRot;
    [Header("Light Settings")]
    [SerializeField] bool isLit;
    [SerializeField] bool isShadowHard = true;
    [SerializeField] bool isAO;
    [SerializeField] Color lightCol;
    [SerializeField] public float lightIntensity, shadowIntensity, shadowMin, shadowMax, shadowSmooth, AOStep, AOIntensity;
    [SerializeField] int AOIteration;
    [Header("Render Settings")]
    [SerializeField] float maxSteps = 225;
    [SerializeField] float maxDist = 1000;
    [SerializeField] float surfDist = .01f;
    public Material _raymarchMaterial
    {
        get
        {
            if (!raymarchMaterial && shader)
            {
                raymarchMaterial = new Material(shader);
                raymarchMaterial.hideFlags = HideFlags.HideAndDontSave;
            }

            return raymarchMaterial;
        }
    }   
    public Camera _camera
    {
        get
        {
            if (!_cam)
            {
                _cam = GetComponent<Camera>();
            }
            return _cam;
        }
    }    
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!raymarchMaterial)
        {
            Graphics.Blit(source, destination);
        }

        RaymarchRender();
        
        RenderTexture.active = destination;
        _raymarchMaterial.SetTexture("_MainTex", source);

        GL.PushMatrix();
        GL.LoadOrtho();
        _raymarchMaterial.SetPass(0);
        GL.Begin(GL.QUADS);

        //BL
        GL.MultiTexCoord2(0, 0.0f, 0.0f);
        GL.Vertex3(0.0f, 0.0f, 3.0f);

        //BR
        GL.MultiTexCoord2(0, 1.0f, 0.0f);
        GL.Vertex3(1.0f, 0.0f, 2.0f);

        //TR
        GL.MultiTexCoord2(0, 1.0f, 1.0f);
        GL.Vertex3(1.0f, 1.0f, 1.0f);

        //TL
        GL.MultiTexCoord2(0, 0.0f, 1.0f);
        GL.Vertex3(0.0f, 1.0f, 0.0f);

        GL.End();
        GL.PopMatrix();        

        foreach (var buffer in disposable)        
            buffer.Dispose();
        
    }   
    void RaymarchRender()
    {
        renderers = new List<RaymarchRenderer>(FindObjectsOfType<RaymarchRenderer>());

        if (renderers.Count != 0)
        {
            Properties[] properties = new Properties[renderers.Count];

            for (int i = 0; i < renderers.Count; i++)
            {
                var s = renderers[i];

                s.transform.localScale = Vector3.one;
                Vector3 color = new Vector3(s.color.r, s.color.g, s.color.b);

                Properties p = new Properties()
                {
                    pos = s.transform.position,
                    posW = s.posW - wPos,
                    rot = s.transform.eulerAngles * Mathf.Deg2Rad,
                    rotW = (s.rotW - wRot) * Mathf.Deg2Rad,
                    col = color,
                    blendFactor = s.blendFactor * 100,
                    shapeIndex = (int)s.shape,
                    opIndex = (int)s.operation,
                    dimensions = Helpers.GetDimensionVectors((int)s.shape, s.dimensions)
                };
                properties[i] = p;

                //_raymarchMaterial.SetFloat("_BlendFactor", s.blendFactor * 10);

                if (renderers[i] == GetComponent<RaymarchRenderer>())
                    _raymarchMaterial.SetInt("_Rank", i);
            }

            shapeBuffer = new ComputeBuffer(renderers.Count, 112);
            shapeBuffer.SetData(properties);
            
            _raymarchMaterial.SetInt("_Count", renderers.Count);
            _raymarchMaterial.SetBuffer("shapes", shapeBuffer);
            _raymarchMaterial.SetFloat("_WPos", wPos);
            _raymarchMaterial.SetVector("_WRot", wRot);
            _raymarchMaterial.SetMatrix("_CamFrustrum", CamFrustrum(_camera));
            _raymarchMaterial.SetMatrix("_CamToWorld", _camera.cameraToWorldMatrix);
            _raymarchMaterial.SetVector("_Loop", loop);
            _raymarchMaterial.SetVector("_LightDir", sun ? sun.transform.forward : Vector3.down);
            _raymarchMaterial.SetFloat("max_steps", maxSteps);
            _raymarchMaterial.SetFloat("max_dist", maxDist);
            _raymarchMaterial.SetFloat("surf_dist", surfDist);
            _raymarchMaterial.SetColor("_LightCol", lightCol);
            _raymarchMaterial.SetFloat("_LightIntensity", lightIntensity);
            _raymarchMaterial.SetFloat("_ShadowIntensity", shadowIntensity);
            _raymarchMaterial.SetFloat("_ShadowMin", shadowMin);
            _raymarchMaterial.SetFloat("_ShadowMax", shadowMax);
            _raymarchMaterial.SetFloat("_ShadowSmooth", shadowSmooth);
            _raymarchMaterial.SetFloat("_AOStep", AOStep);
            _raymarchMaterial.SetFloat("_AOIntensity", AOIntensity);
            _raymarchMaterial.SetInt("_AOIteration", AOIteration);

            if (isLit)
                _raymarchMaterial.SetInt("_isLit", 1);
            else
                _raymarchMaterial.SetInt("_isLit", 0);            

            if (isShadowHard)
                _raymarchMaterial.SetInt("_isShadowHard", 1);
            else
                _raymarchMaterial.SetInt("_isShadowHard", 0);

            if (isAO)
                _raymarchMaterial.SetInt("_isAO", 1);
            else
                _raymarchMaterial.SetInt("_isAO", 0);

            /* SetShaderBoolean(() => isLit);
             SetShaderBoolean(() => isWOverride);
             SetShaderBoolean(() => isShadowHard);*/

            disposable.Add(shapeBuffer);
        }
    }
    void SetShaderBoolean(Expression<Func<bool>> expr)
    {
        var body = (MemberExpression)expr.Body;
        string variableName = body.Member.Name;
        string propertyName = "_" + variableName;

        bool flag = expr.Compile().Invoke();
        _raymarchMaterial.SetInt(propertyName, flag ? 1 : 0);
    }

    private Matrix4x4 CamFrustrum(Camera cam)
    {
        Matrix4x4 frustrum = Matrix4x4.identity;
        float fov = Mathf.Tan((cam.fieldOfView * .5f) * Mathf.Deg2Rad);

        Vector3 goUp = Vector3.up * fov;
        Vector3 goRight = Vector3.right * fov * cam.aspect;

        Vector3 TL = (-Vector3.forward - goRight + goUp);
        Vector3 TR = (-Vector3.forward + goRight + goUp);
        Vector3 BL = (-Vector3.forward - goRight - goUp);
        Vector3 BR = (-Vector3.forward + goRight - goUp);

        frustrum.SetRow(0, TL);
        frustrum.SetRow(1, TR);
        frustrum.SetRow(2, BR);
        frustrum.SetRow(3, BL);

        return frustrum;
    }
}
public struct Properties
{
    public Vector3 pos;
    public float posW;
    public Vector3 rot;
    public Vector3 rotW;
    public Vector3 col;
    public float blendFactor;
    public int shapeIndex;
    public int opIndex;
    public vector12 dimensions;
}
public struct vector12
{
    public float a;
    public float b;
    public float c;
    public float d;
    public float e;
    public float f;
    public float g;
    public float h;
    public float i;
    public float j;
    public float k;
    public float l;

    public vector12(float _a, float _b, float _c, float _d, float _e, float _f, float _g, float _h, float _i, float _j, float _k, float _l)
    {
        this.a = _a;
        this.b = _b;
        this.c = _c;
        this.d = _d;
        this.e = _e;
        this.f = _f;
        this.g = _g;
        this.h = _h;
        this.i = _i;
        this.j = _j;
        this.k = _k;
        this.l = _l;
    }
}