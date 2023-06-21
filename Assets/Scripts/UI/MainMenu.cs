using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void GoToBattleScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    public void Exit() => Application.Quit();

}


