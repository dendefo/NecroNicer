using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

abstract public class MySceneManager : MonoBehaviour
{
    static public MySceneManager Instance;
    public virtual void Awake()
    {
        Instance = this;
        GameManager.Instance.SceneManager = this;
    }

    public virtual void OpenInventory()
    {
        if (InventoryItemHandler.drag != null) return;
        GameManager.Instance.OpenInventory();
    }

    public virtual void RestartTheGame()
    {
        Destroy(GameManager.Instance.gameObject);
        SceneManager.LoadScene(0);
    }
}
