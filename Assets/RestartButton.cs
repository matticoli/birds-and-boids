using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class RestartButton : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(RestartGame);
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

}
