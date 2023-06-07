using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowMove : MonoBehaviour
{

    [SerializeField] AnimationCurve Curve;
    public Vector3 StartPos;
    public Vector3 SecondPoint;
    public Vector3 ThirdPoint;
    public Vector3 EndPos;

    public List<Vector3> PointsOnCurve;


    // Start is called before the first frame update
    void Start()
    {
        StartPos = transform.GetChild(0).localPosition;

        SecondPoint = new(1, Curve.keys[0].outTangent);
        SecondPoint = Vector3.Lerp(new(0, 0), SecondPoint, Curve.keys[0].outWeight);

        ThirdPoint = new(1, Curve.keys[1].inTangent);
        ThirdPoint = Vector3.LerpUnclamped(new(0, 0), ThirdPoint, Curve.keys[1].inWeight);
        ThirdPoint = new(1 - ThirdPoint.x, -(ThirdPoint.y - 1));

        PointsOnCurve = new();
        for (int i = 1; i < transform.childCount; i++)
        {
            float t = i / (float)transform.childCount;
            Vector3 M1 = Vector3.Lerp(new(0, 0), SecondPoint, t);
            Vector3 M2 = Vector3.Lerp(SecondPoint, ThirdPoint, t);
            Vector3 M3 = Vector3.Lerp(ThirdPoint, new(1, 1), t);

            Vector3 MM1 = Vector3.Lerp(M1, M2, t);
            Vector3 MM2 = Vector3.Lerp(M2, M3, t);

            PointsOnCurve.Add(Vector3.Lerp(MM1, MM2, t));
        }
    }

    // Update is called once per frame
    void Update()
    {

        var pos = GameManager.Instance.camera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        EndPos = new Vector3(pos.x, pos.y) * 3;


        for (int i = 1; i < transform.childCount; i++)
        {
            var toMove = transform.GetChild(i);
            var toRotate = transform.GetChild(i - 1);

            toMove.localPosition = Vector3.Scale(PointsOnCurve[i - 1], EndPos);

            Rotate(toRotate, toMove.position); //Not needed in position calculation

        }

        Rotate(transform.GetChild(transform.childCount - 1), transform.GetChild(transform.childCount - 2).position);
        transform.GetChild(transform.childCount - 1).transform.localEulerAngles = new(0, 0, transform.GetChild(transform.childCount - 1).transform.localEulerAngles.z + 180);


    }
    static void Rotate(Transform toRotate, Vector3 toMove)
    {
        var norm = Vector3.Normalize(toRotate.position - toMove);
        var Acos = Mathf.Acos(norm.y);
        var z = Acos / Mathf.PI * (toRotate.position.x > toMove.x ? -180 : 180);

        toRotate.localEulerAngles = new Vector3(0, 0, z + 180);
    }
}
