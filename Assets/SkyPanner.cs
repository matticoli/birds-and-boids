using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyPanner : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(new Vector3(0f, 0.05f));
    }
}
