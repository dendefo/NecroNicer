using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class UnitWindowScript : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] UnitScript _reference;

    [Header("Objects")]
    [SerializeField] TMP_Text Name_Text;
    [SerializeField] Slider HealthBar;
    [SerializeField] TMP_Text HealthText;

    [SerializeField] List<EffectIconScript> EffectIcons;
    [SerializeField] Image UnitVisuals;

    [SerializeField] TMP_Text Initiative;
    [SerializeField] TMP_Text Damage;
    [SerializeField] TMP_Text M_Damage;
    [SerializeField] TMP_Text Defence;
    [SerializeField] TMP_Text M_Defence;
    [SerializeField] TMP_Text CritChance;
    [SerializeField] TMP_Text CritDamage;
    [SerializeField] UIFlippable Flippable;

    public UnitScript UnitReference
    {
        get { return _reference; }
        set
        {

            _reference = value;
            if (value == null) { gameObject.SetActive(false);return; }
            else gameObject.SetActive(true);
            Name_Text.text = value.EnemyScriptableObjects.Name;
            HealthBar.value = (float)value.UnitStats.Health / (float)value.EnemyScriptableObjects.BasicStats.Health;
            HealthText.text = $"{value.UnitStats.Health}/{value.EnemyScriptableObjects.BasicStats.Health}";

            for (int i = 0; i < EffectIcons.Count; i++)
            {
                if (value.activeEffects.effects.Count <= i)
                {
                    EffectIcons[i].effectReference = null;
                }
                else
                {
                    EffectIcons[i].effectReference = GameManager.Instance.EffectLibraby.GiveEffect(value.activeEffects[i].Type);
                }
            }

            try
            {
                UnitVisuals.sprite = value.EnemyScriptableObjects.Miniature;
                Flippable.horizontal = !value.IsEnemy;
            }
            catch { Debug.Log("Miniature problem"); }

            Initiative.text = value.UnitStats.Initiative.ToString();
            Damage.text = value.UnitStats.Attack.ToString();
            M_Damage.text = value.UnitStats.Magic.ToString();
            Defence.text = value.UnitStats.Defence.ToString();
            M_Defence.text = value.UnitStats.MagicDefence.ToString();
            CritChance.text = (value.UnitStats.CritChance * 100).ToString() + "%";
            CritDamage.text = (value.UnitStats.CritDamage * 100).ToString() + "%";

        }
    }

    
    public void Close()
    {
        UnitReference = null;
    }
}
