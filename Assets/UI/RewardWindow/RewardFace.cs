using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardFace : MonoBehaviour
{
    [SerializeField] private InstanceFace _Face;

    public InstanceFace Face
    {
        get { return _Face; }
        set { value.Uses = Mathf.Min(value.faceDefenition.Stats.MaxUses, value.Uses); _Face = value; renderer.material = value.faceDefenition.material; }
    }

    public MeshRenderer renderer;

    // Use this for initialization
    void Start()
    {
        renderer.material = Face.faceDefenition.material;
    }
}
