using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class RestartButton : MonoBehaviour
{
    public void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(RestartGame);
    }
    public static void RestartGame()
    {
        SceneManager.LoadSceneAsync(0, LoadSceneMode.Single); 
    }

    

}
