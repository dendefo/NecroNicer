using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNuber : MonoBehaviour
{
    public TMPro.TMP_Text m_TextMeshPro;

    public void DeleteAfterAnimation()
    {
        Destroy(gameObject);
    }
}
