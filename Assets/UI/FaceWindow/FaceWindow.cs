using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI.Extensions;

public class FaceWindow : MonoBehaviour
{
    public bool closable = true;
    [Header("Reference")]
    [SerializeField] Face _reference;

    [SerializeField] LogRow Prefab;
    public RectTransform rectTransform;
    public float DescriptionSize;
    public float EffectsSize;

    [Header("Text Objects")]


    [SerializeField] TMP_Text Tier;
    [SerializeField] TMP_Text Name;
    [SerializeField] GameObject Description;
    [SerializeField] TMP_Text Target;
    [SerializeField] TMP_Text Damage;
    [SerializeField] GameObject EffectTextLine;
    [SerializeField] TMP_Text UpgradeCost;
    [SerializeField] TMP_Text MaxUses;
    [SerializeField] TMP_Text DustRecieved;
    [Header("Panels")]
    [SerializeField] GameObject TierAndName;
    [SerializeField] GameObject TitleLine;

    [SerializeField] GameObject DescriptionPanel;
    [SerializeField] GameObject DescriptionLine;

    [SerializeField] GameObject TargetPanel;
    [SerializeField] GameObject TargeLine;

    [SerializeField] GameObject EffectPanel;
    [SerializeField] GameObject EffectLine;

    [SerializeField] GameObject NumbersPanel;
    [SerializeField] GameObject NumbersLine;

    [SerializeField] GameObject EffectFather;

    public Face Reference
    {
        get { return _reference; }
        set
        {
            _reference = value;
            if (_reference != null) { gameObject.SetActive(true); }
            else {gameObject.SetActive(false); return; }
            Tier.text = value.Stats.Tier.ToString();
            Name.text = value.Stats.Name;

            var DescLine = Instantiate(Prefab, DescriptionPanel.transform);
            DescLine.CreateAnyText(value.Stats.Description, EffectReferences: value.Stats.EffectToReferenceInDescription);
            if (Description!=null) { DestroyImmediate(Description); }
            Description = DescLine.gameObject;
            DescLine.GetComponent<FlowLayoutGroup>().childAlignment = TextAnchor.UpperCenter;
            DescLine.GetComponent<FlowLayoutGroup>().SpacingX = 5;
            for (int i = 0; i < DescLine.transform.childCount; i++)
            {
                LogText a;
                if (!DescLine.transform.GetChild(i).TryGetComponent(out a)) ;
                {
                    DescLine.transform.GetChild(i).GetComponent<TMP_Text>().color = Color.white;
                }
                DescLine.transform.GetChild(i).GetComponent<TMP_Text>().fontSize = DescriptionSize;
            }

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

            if (value.Stats.Bonuses == "") { EffectPanel.SetActive(false); EffectLine.SetActive(false); }
            else { EffectPanel.SetActive(true); EffectLine.SetActive(true); }

            UpgradeCost.text = value.Stats.DustCost.ToString();
            MaxUses.text = value.Stats.MaxUses.ToString();
            DustRecieved.text = value.Stats.DustOnDestroy.ToString();
            if (value.Stats.Description == "") { DescriptionPanel.SetActive(false); DescriptionLine.SetActive(false); }
            else { DescriptionPanel.SetActive(true); DescriptionLine.SetActive(true); }

            if (value.Stats.Value == 0) Damage.gameObject.SetActive(false);
            else Damage.gameObject.SetActive(true);

        }
    }
    [ContextMenu("UpdateVisuals")]
    public void UUpdate()
    {
        Reference = _reference;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)&&closable) Destroy(gameObject);
    }

}
