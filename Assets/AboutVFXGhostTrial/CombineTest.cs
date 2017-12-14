using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(SkinnedMeshRenderer))]
public class CombineTest : MonoBehaviour
{

    public GameObject[] Group;
    void Start()
    {
        SkinnedMeshRenderer[] meshes = GetComponentsInChildren<SkinnedMeshRenderer>();
        
        CombineInstance[] combine = new CombineInstance[2];
        int i = 0;
        while (i < combine.Length)
        {
            combine[i] = new CombineInstance
            {
                mesh = meshes[0].sharedMesh,
                subMeshIndex = i,
            };
            
            combine[i].transform = meshes[0].transform.localToWorldMatrix;
            meshes[0].gameObject.active = false;
            i++;
        }
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine,true,false);
        transform.gameObject.active = true;
    }
}