using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;

public class MediumFaceTooltip : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] Face _reference;

    [SerializeField] LogRow Prefab;
    public RectTransform rectTransform;
    public float DescriptionSize;
    public float EffectsSize;

    [Header("Text Objects")]


    [SerializeField] TMP_Text Tier;
    [SerializeField] TMP_Text Name;

    [SerializeField] TMP_Text Target;
    [SerializeField] TMP_Text Damage;

    [SerializeField] GameObject EffectTextLine;

    [SerializeField] TMP_Text UsesLeft;

    [Header("Panels")]

    [SerializeField] GameObject TierAndName;
    [SerializeField] GameObject TitleLine;

    [SerializeField] GameObject TargetPanel;
    [SerializeField] GameObject TargeLine;

    [SerializeField] GameObject EffectPanel;

    [SerializeField] GameObject NumbersPanel;
    [SerializeField] GameObject NumbersLine;

    [SerializeField] GameObject EffectFather;
    public Face Reference
    {
        get { return _reference; }
        set
        {
            _reference = value;
            Tier.text = value.Stats.Tier.ToString();
            Name.text = value.Stats.Name;

            Target.text = "Target: " + value.Stats.Target.ToString();
            if (value.Offensive)
            {
                Damage.text = value.Stats.Value.ToString() + " " + value.Stats.DamageType.ToString() + " Damage";
                Damage.color = Color.red;
            }
            else if (value.Defensive)
            {
                Damage.text = value.Stats.Value.ToString() + " " + value.Stats.DamageType.ToString() + " Defence";
                Damage.color = Color.blue;
            }
            else if (!value.Offensive && !value.Defensive && value.Stats.Value != 0)
            {
                Damage.text = value.Stats.Value.ToString() + " " + " Healing";
                Damage.color = Color.green;
            }

            if (EffectTextLine != null) { DestroyImmediate(EffectTextLine); }

            var Line = Instantiate(Prefab, EffectFather.transform);
            Line.CreateAnyText(value.Stats.Bonuses, EffectReferences: value.Stats.EffectsToReferenceInBonuses);
            EffectTextLine = Line.gameObject;
            Line.GetComponent<FlowLayoutGroup>().childAlignment = TextAnchor.UpperCenter;
            Line.GetComponent<FlowLayoutGroup>().SpacingX = 5;

            for (int i = 0; i < Line.transform.childCount; i++)
            {
                LogText a;
                if (!Line.transform.GetChild(i).TryGetComponent(out a))
                {
                    Line.transform.GetChild(i).GetComponent<TMP_Text>().color = Color.white;
                }
                Line.transform.GetChild(i).GetComponent<TMP_Text>().fontSize = EffectsSize;
            }

            if (value.Stats.Bonuses == "") { EffectPanel.SetActive(false);}
            else { EffectPanel.SetActive(true); }

            if (value.Stats.Value == 0) Damage.gameObject.SetActive(false);
            else Damage.gameObject.SetActive(true);

        }
    }
    [ContextMenu("UpdateVisuals")]
    public void UUpdate()
    {
        Reference = _reference;
    }

}
