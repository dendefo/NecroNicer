using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;

public class EffectIconScript : MonoBehaviour
{
    [SerializeField] Effect _effectReference;
    [SerializeField] TooltipUIpromp tooltip;
    public Effect effectReference
    {
        get { return _effectReference; }
        set
        {
            if (value == null) 
            { 
                IconText.enabled = false;
                return;
            }
            else
            {
                IconText.enabled = true;
            }
            _effectReference = value;
            IconText.text = value.ToString();
            tooltip.header = value.Name;
            tooltip.content = value.Description;

        }
    }

    public TMP_Text IconText;

    private void OnValidate()
    {
        effectReference = _effectReference;
    }
    
}
