using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private Vector3 startPositionRelativeToTextBox;

    public float appearTimer;

    [HideInInspector] public bool waveing = false;
    [HideInInspector] public bool waveFromLeftToRight;
    [HideInInspector] public float waveHeight;
    [HideInInspector] public float waveSpeed;
    [HideInInspector] public float waveOffset;


    [HideInInspector] public bool shaking = false;
    [HideInInspector] public float shakingStrength;

    private void OnEnable()
    {
        startPositionRelativeToTextBox = this.transform.localPosition;
    }

    private void Update()
    {
        if (waveing) { Wave(); }
        if (shaking) { Shake(); }
    }

    private void Wave()
    {
        if (waveFromLeftToRight)
        {
            //this.transform.Translate(new Vector3(0, 1 * Mathf.Sin((-Time.time * waveSpeed) + waveOffset) * waveHeight * Time.deltaTime, 0));
            //this.transform.Translate(Vector3.up * Mathf.Sin((-Time.time * waveSpeed)) * waveHeight * Time.deltaTime);
            //this.transform.localPosition += Vector3.up * Mathf.PingPong(Time.deltaTime, waveHeight) * waveSpeed;
            this.transform.localPosition += Vector3.up * Mathf.Sin(-Time.time * waveSpeed + waveOffset) * waveHeight * Time.deltaTime;
            //this.transform.Translate(Vector3.up * Mathf.Sin((-Time.time * waveSpeed) + waveOffset) * waveHeight * Time.deltaTime);
            //this.transform.position = new Vector3(relativePositionToTextBox.x, relativePositionToTextBox.y + Mathf.Sin((-Time.time * waveSpeed) + waveOffset) * waveHeight, relativePositionToTextBox.z);
            //this.transform.localPosition += new Vector3( 0, Mathf.Sin((-Time.time * waveSpeed) + waveOffset) * waveHeight * Time.deltaTime);
                        
        }
        else
        {
            this.transform.position = new Vector3(startPositionRelativeToTextBox.x, startPositionRelativeToTextBox.y + Mathf.Sin((Time.time * waveSpeed) + waveOffset) * waveHeight, startPositionRelativeToTextBox.z);
        }
    }

    private void Shake()
    {
        //this.transform.position = relativePositionToTextBox + new Vector3(Random.Range(-shakingStrength, shakingStrength), Random.Range(-shakingStrength, shakingStrength), 0);
        this.transform.localPosition = new Vector3(startPositionRelativeToTextBox.x + Random.Range(-shakingStrength, shakingStrength), startPositionRelativeToTextBox.y + Random.Range(-shakingStrength, shakingStrength), 0);
    }
}
