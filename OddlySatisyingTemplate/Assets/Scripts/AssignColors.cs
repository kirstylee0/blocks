using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignColors : MonoBehaviour
{
    [SerializeField] List<Material> _materials = new List<Material>();
    public MeshRenderer MeshRend => _meshRenderer;
    [SerializeField] MeshRenderer _meshRenderer;
    
    public List<GameObject> indicatorsList = new List<GameObject>();
    private GameObject activeIndicatorObject;


    void Start()
    {
        CycleIndicator(0);
    }

    void Update()
    {
        
    }

    public void RecolorMesh(int rotation)
    {
        _meshRenderer.material = _materials[rotation];
    }

    public void CycleIndicator(int id)
    {
        foreach (GameObject indicatorOption in indicatorsList)
        {
            indicatorOption.SetActive(false); 
        }

        indicatorsList[id].SetActive(true);

    }
}
