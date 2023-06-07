using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LogRow : MonoBehaviour
{
    [SerializeField] TMP_Text Basic;
    [SerializeField] TMP_Text Number;
    [SerializeField] LogText AbilityPref;
    [SerializeField] LogText EffectPref;
    public RectTransform ParentCanvas;
    /// <summary>
    /// Parses a text, enters references and outputs it as a Couple of TMP_Text Objects 
    /// example: "<> used [] dealing () damage to <> and causing him to become {}"
    /// </summary>
    /// <param name="text"> Text to output</param>
    /// <param name="UnitRefences"> Should have <> symnol in text </param>
    /// <param name="MaterialReferences"> Should have [] symbol int text </param>
    /// <param name="EffectReferences"> Should have {} symbol in text </param>
    /// <param name="NumberValues"> Should have () symbol in text </param>
    public void CreateAnyText(string text, UnitScript[] UnitRefences = null, Face[] MaterialReferences = null, EffectType[] EffectReferences = null, int[] NumberValues = null)
    {
        var str = text.Split(" ");
        int UnitCount = 0;
        int MaterialCount = 0;
        int EffectCount = 0;
        int ValueCount = 0;
        for (int i = 0; i < str.Length; i++)
        {
            switch (str[i])
            {
                case "<>":
                case "<>,":
                case "<>.":
                    try { UnitText(UnitRefences[UnitCount]); }
                    catch { BasicText("Unknown Unit"); }
                    UnitCount++;
                    break;
                case "[]":
                case "[],":
                case "[].":
                    try { AbilityText(MaterialReferences[MaterialCount]); }
                    catch { BasicText("Unknown ability"); }
                    MaterialCount++;
                    break;
                case "{}":
                    try { EffectText(EffectReferences[EffectCount]); }
                    catch { BasicText("Unknown Effect"); }
                    EffectCount++;
                    break;
                case "()":
                    try { NumberText(NumberValues[ValueCount]); }
                    catch { BasicText("Unknown value"); }
                    ValueCount++;
                    break;
                default:
                    BasicText(str[i].Trim());

                    break;
            }
        }
    }
    private void UnitText(UnitScript Unit)
    {
        LogText logText = Instantiate(BattleLogManager.Instance.UnitTextPrefab, transform).GetComponent<LogText>();
        logText.promp.ParentCanvas = ParentCanvas;
        logText.UnitReference = Unit;
    }
    private void BasicText(string text)
    {
        Instantiate(Basic, transform).text = text;
    }
    private void NumberText(int number)
    {

        Instantiate(Number, transform).text = number.ToString();
    }
    private void AbilityText(Face Ability)
    {
        LogText logText = Instantiate(AbilityPref, transform);
        logText.promp.ParentCanvas = ParentCanvas;
        logText.FaceReference = Ability;
    }
    private void EffectText(EffectType Effect)
    {
        LogText logText = Instantiate(EffectPref, transform);
        logText.promp.ParentCanvas = ParentCanvas;
        logText.EffectReference = GameManager.Instance.EffectLibraby.GiveEffect(Effect);
    }
}
