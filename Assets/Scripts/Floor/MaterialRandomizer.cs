using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialRandomizer : MonoBehaviour
{
    // Start is called before the first frame update
    public UnitPlacable script;
    [SerializeField] MeshRenderer rend;
    [SerializeField] Collider collider;
    [SerializeField] Material[] materials;
    [SerializeField] Material[] TriangleMaterials;
    [SerializeField] Vector3[] deviation;
    void Start()
    {
        if (transform.position.y > -0.5)
        {
            rend.material = TriangleMaterials[Random.Range(0, TriangleMaterials.Length)];
            Destroy(script);
            Destroy(collider);
            return;
        }
        int a = Random.Range(0, materials.Length);
        rend.material = materials[a];
        transform.Translate(deviation[a], Space.World);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
