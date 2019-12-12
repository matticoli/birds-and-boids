using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Leap.Unity;

public class Title : MonoBehaviour
{
    const float period = 2.0f;

    private LeapServiceProvider lp;

    private void Start()
    {
        lp = GameObject.Find("Leap Motion Controller Variant").GetComponent<LeapServiceProvider>();
    }

    float fadeStartTime = 0f;

    // Update is called once per frame
    void Update()
    {
        if (Time.time > period)
        {
           // Destroy(gameObject);
        }
        if (lp.GetLeapController().Frame().Hands.Count == 0)
        {
            fadeStartTime = Time.time; 
        }
        Color colorOfObject = GetComponent<Text>().color;//Changed this
        float prop = ((Time.time - fadeStartTime) / period);
        colorOfObject.a = Mathf.Lerp(1, 0, prop);
        GetComponent<Text>().color = colorOfObject;//Changed this
    }

}
