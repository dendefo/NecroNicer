using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventsScript : MonoBehaviour
{
    [SerializeField] UnitScript unit;
    public SpriteRenderer Head;
    private void Awake()
    {
        unit = transform.parent.GetComponent<UnitScript>();
    }
    public void AfterAttack()
    {
        unit.AfterAnimation();
    }

}
