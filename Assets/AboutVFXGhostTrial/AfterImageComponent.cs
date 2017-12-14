using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


// 残影效果  
public class AfterImageComponent : MonoBehaviour
{
    class AfterImage
    {
        public Mesh mesh;
        public Material[] material;
        public Matrix4x4 matrix;
        public float showStartTime;
        public float duration;  // 残影镜像存在时间  
        public float alpha;
        public bool needRemove = false;
    }

    private float _duration; // 残影特效持续时间  
    private float _interval; // 间隔  
    private float _fadeTime; // 淡出时间  

    private List<AfterImage> _imageList = new List<AfterImage>();

    public Material _material;
    public Shader _shaderAfterImage;

    public bool isTurnOn = false;

    void Awake()
    {
        //_shaderAfterImage = Shader.Find("Shader/AfterImage");
    }

    private void Start()
    {
        Play(180, 0.1f, 3f);
    }

    public void PlayStart()
    {
        Play(180, 0.1f, 3f);
    }

    public void Play(float duration, float interval, float fadeout)
    {
        _duration = duration;
        _interval = interval;
        _fadeTime = fadeout;

        StartCoroutine(DoAddImage());
        isTurnOn = true;
    }

    public void Stop()
    {
        isTurnOn = false;
    }

    IEnumerator DoAddImage()
    {
        float startTime = Time.realtimeSinceStartup;
        while (isTurnOn)
        {
            CreateImage();

            if (Time.realtimeSinceStartup - startTime > _duration)
            {
                break;
            }

            yield return new WaitForSeconds(_interval);
        }
    }

    private void CreateImage()
    {
        SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();

        CombineInstance[] combineInstances = new CombineInstance[renderers.Length + meshRenderers.Length];

        Transform t = transform;
        Material[] mats = null;
        List<CombineInstance> Mesh_ALL = new List<CombineInstance>();
        for (int i = 0; i < renderers.Length; ++i)
        {
            var item = renderers[i];
            t = item.transform;
            mats = new Material[item.materials.Length];
            //mat.shader = _shaderAfterImage;
            for (int j = 0; j<mats.Length; j++)
            {
                mats[j] = new Material(_shaderAfterImage);
                mats[j].SetColor("_TintColor", _material.GetColor("_TintColor"));
                mats[j].SetTexture("_MainTex", _material.GetTexture("_MainTex"));
            }

            if (item.sharedMesh.subMeshCount > 1)
            {
                CombineInstance[] combineInstances_Sub = new CombineInstance[item.sharedMesh.subMeshCount];
               
                for (int k = 0; k < item.sharedMesh.subMeshCount; k++)
                {
                    Mesh Mesh_Sub = new Mesh();
                    item.BakeMesh(Mesh_Sub);
                    combineInstances_Sub[k] = new CombineInstance
                    {
                        mesh = Mesh_Sub,
                        subMeshIndex = k,
                    };
                    //Mesh_Sub.CombineMeshes(combineInstances_Sub,true,false);
                    Mesh_ALL.Add(combineInstances_Sub[k]);
                }
            }

            var mesh = new Mesh();
            item.BakeMesh(mesh);
            combineInstances[i] = new CombineInstance
            {
                mesh = mesh,
                subMeshIndex = 0,
            };
        }

        Mesh combinedMesh = new Mesh();
        //combinedMesh.CombineMeshes(combineInstances, true, false);
        combinedMesh.CombineMeshes(Mesh_ALL.ToArray(), true, false);
        
        _imageList.Add(new AfterImage
        {
            mesh = combinedMesh,
            material = mats,
            matrix = t.localToWorldMatrix,
            showStartTime = Time.realtimeSinceStartup,
            duration = _fadeTime,
        });
    }

    private void DrawMesh(Mesh trailMesh, Material trailMaterial)
    {
        Graphics.DrawMesh(trailMesh, Matrix4x4.identity, trailMaterial, gameObject.layer);
    }

    void Update()
    {
       
    }

    void LateUpdate()
    {
        bool hasRemove = false;
        foreach (var item in _imageList)
        {
            float time = Time.realtimeSinceStartup - item.showStartTime;

            if (time > item.duration)
            {
                item.needRemove = true;
                hasRemove = true;
                continue;
            }

            if (item.material[0].HasProperty("_Opacity"))
            {
                item.alpha = Mathf.Max(0, 1 - time / item.duration);

                foreach (var mat in item.material)
                {
                    
                    mat.SetFloat("_Opacity",   item.alpha);
                    Graphics.DrawMesh(item.mesh, item.matrix, mat, gameObject.layer);
                    

                }
                //Color color = Color.blue;
                //color.a = item.alpha;
                //item.material.SetColor("_TintColor", color);
  
            }

            //Graphics.DrawMesh(item.mesh, item.matrix, item.material[0], gameObject.layer);
        }

        if (hasRemove)
        {
            _imageList.RemoveAll(x => x.needRemove);
        }
    }
}