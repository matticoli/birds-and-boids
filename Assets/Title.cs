using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Title : MonoBehaviour
{
    const float period = 4.0f;
  

    // Update is called once per frame
    void Update()
    {
        if (Time.time > period)
        {
            Destroy(gameObject);
        }

        Color colorOfObject = GetComponent<Text>().color;//Changed this
        float prop = (Time.time / period);
        colorOfObject.a = Mathf.Lerp(1, 0, prop);
        GetComponent<Text>().color = colorOfObject;//Changed this
    }

}
