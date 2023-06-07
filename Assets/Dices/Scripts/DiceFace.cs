using UnityEngine;

/// <summary>
/// Script that lays on each face of dice
/// </summary>
public class DiceFace : MonoBehaviour
{
    /// <summary>
    /// Describes a face and changes it visual representation
    /// </summary>
    public InstanceFace Face
    {
        get
        {
            return _face;
        }
        set
        {
            value.Uses = Mathf.Min(value.faceDefenition.Stats.MaxUses, value.Uses);
            _face = value;
            _meshRenderer.material = value.faceDefenition.material;
        }
    } 

    [SerializeField] private InstanceFace _face;

    [SerializeField] MeshRenderer _meshRenderer;

    void Start()
    {
        if (_face != null) { _meshRenderer.material = Face.faceDefenition.material; }
    }
}