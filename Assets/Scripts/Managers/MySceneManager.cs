using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

abstract public class MySceneManager : MonoBehaviour
{
    static public MySceneManager Instance;
    [SerializeField] List<Image> images;
    public virtual void Awake()
    {
        Instance = this;
        try { GameManager.Instance.SceneManager = this; }
        catch
        {
            Debug.Log("Battle Manager without GameManager");
        }
        foreach (Image image in images)
        {
            image.alphaHitTestMinimumThreshold = 0.5f;
        }
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
