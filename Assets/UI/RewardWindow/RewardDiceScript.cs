using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI.Extensions.Tweens;

public class RewardDiceScript : MonoBehaviour
{
    [SerializeField] public DiceControl controlDice;
    public FaceWindow RewardFaceWindow;
    [SerializeField] List<Vector3> rotations = new List<Vector3> { new Vector3(25, -128, 27.5f), new Vector3(-25, -51, 27.5f), new Vector3(-25, 51, -27.5f), new Vector3(25, 128, -27.5f) };
    [SerializeField]
    List<Quaternion> Qrotationsss = new List<Quaternion>
    { new Quaternion(-0.116404966f,-0.874893844f,0.290684491f,0.369476914f),
    new Quaternion(-0.289657503f,-0.36182797f,0.118937284f,0.878084838f),
    new Quaternion(-0.289657503f,0.36182797f,-0.118937284f,0.878084838f),
    new Quaternion(-0.116404966f,0.874893844f,-0.290684491f,0.369476914f)
    };


    [SerializeField] int _currentSide = 1;
    Quaternion from;
    Quaternion to;
    public int CurrentSide
    {
        get { return _currentSide; }
        set
        {

            if (Time.timeSinceLevelLoad - rotationStarted <= rotationDuration) return;
            from = Qrotationsss[_currentSide];
            _currentSide = value;
            if (_currentSide == Qrotationsss.Count) { _currentSide = 0; }
            else if (_currentSide < 0) { _currentSide = Qrotationsss.Count - 1; }

            
            rotationStarted = Time.timeSinceLevelLoad;
            to = Qrotationsss[_currentSide];
        }
    }
    float rotationStarted = -2;
    public float rotationDuration;
    [SerializeField] GameObject buttons;
    private void Start()
    {
        RewardFaceWindow.Reference = controlDice.Faces[CurrentSide].Face.faceDefenition;
    }
    private void Update()
    {
        try
        {
            if (Time.timeSinceLevelLoad - rotationStarted <= rotationDuration)
            {
                transform.localRotation = Quaternion.Lerp(from, to, Time.timeSinceLevelLoad - rotationStarted / rotationDuration);
            }
            else if (Time.timeSinceLevelLoad - rotationStarted >= rotationDuration && !buttons.active)
            {
                buttons.SetActive(true);
                RewardFaceWindow.Reference = controlDice.Faces[CurrentSide].Face.faceDefenition;
            }
        }
        catch { }
    }
    public void ChangeSide(int change)
    {
        CurrentSide += change;
        buttons.SetActive(false);
    }
}
