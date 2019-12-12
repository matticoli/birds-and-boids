using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDFader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float f = gameObject.GetComponent<CanvasRenderer>().GetAlpha();
        gameObject.GetComponent<CanvasRenderer>().SetAlpha(f - 0.025f);

    }
}
