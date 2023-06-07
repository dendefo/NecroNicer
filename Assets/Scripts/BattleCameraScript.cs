using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCameraScript : MonoBehaviour
{

    public Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.camera = camera;
        GameManager.Instance.InventoryCanvas.worldCamera = camera;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
