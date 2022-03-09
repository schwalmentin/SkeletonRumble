using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class postprocessingScenechange : MonoBehaviour
{
    public Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {

        }
    }
}
