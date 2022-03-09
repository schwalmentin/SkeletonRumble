using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Sphere : MonoBehaviour
{
    private DeveloperSettings developerSettings;

    public Color normalColor;
    public Color queuedColor;
    [HideInInspector] public float lifeTime;
    [HideInInspector] public Renderer sphereRenderer;

    private void Awake()
    {
        this.developerSettings = FindObjectOfType<DeveloperSettings>();
        this.sphereRenderer = this.GetComponent<Renderer>();
    }

    private void Update()
    {
        this.lifeTime -= Time.deltaTime;

        this.sphereRenderer.material.color = Color.Lerp(this.queuedColor, this.normalColor, this.lifeTime);       

        if (this.lifeTime <= 0)
        {
            this.developerSettings.spheres.Enqueue(this);
            this.gameObject.SetActive(false);
        }
    }  
}
