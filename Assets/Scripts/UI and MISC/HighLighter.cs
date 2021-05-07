using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HighLighter : MonoBehaviour
{
    Material normalMat;
    Material highlightMat;
    MeshRenderer mesh;
    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        Color32 highlight32 = new Color32(10, 10, 10, 0);
        Color highlight = highlight32 + new Color(0,0,0, mesh.material.color.a);
        mesh.material.SetColor("_EmissionColor", highlight);
    }

    public void HighLight() { mesh.material.EnableKeyword("_EMISSION"); }
    public void UnHighLight() { mesh.material.DisableKeyword("_EMISSION"); }

    private void OnMouseEnter()
    {
        if(!EventSystem.current.IsPointerOverGameObject()) HighLight();
    }

    private void OnMouseExit()
    {
        UnHighLight();
    }
}
