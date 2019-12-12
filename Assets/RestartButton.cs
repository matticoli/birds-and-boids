using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class RestartButton : MonoBehaviour
{
    public static void RestartGame()
    {
        SceneManager.LoadScene(0); 
    }

}
