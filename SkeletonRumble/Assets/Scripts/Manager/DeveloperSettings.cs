using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class DeveloperSettings : MonoBehaviour
{
    [TextArea(3, 10)]
    public string Info;
    [Space]

    [Header("SlowMotion")]
    public float slowmotionValue;
    private bool slowMotion;

    [Header("LimitFPS")]
    public int[] maxFPSRate;
    private int currentFPSRateIndex;

    [Header("DrawSpheres")]
    private bool drawSpheres;
    public Sphere sphere;
    public Transform[] objectsToFollow;
    public float spawnRate;
    private float spawnTimer;
    public float lifeTime;
    [HideInInspector] public Queue<Sphere> spheres = new Queue<Sphere>();

    [Header("BeABox")]
    public GameObject[] playerGraphics;
    public GameObject boxGraphic;
    private bool player;

    private void Start()
    {
        this.currentFPSRateIndex = -1;
    }

    private void Update()
    {
        if (this.drawSpheres)
        {
            this.DrawSpheres();
        }
    }

    public void SlowMotion(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        if (this.slowMotion)
        {
            Time.timeScale = 1;
            this.slowMotion = false;
            print("SlowMo Off");
        }
        else
        {
            Time.timeScale = this.slowmotionValue;
            this.slowMotion = true;
            print("SlowMo On");
        }
    }

    public void ReloadCurrentScene(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SwitchFPSRate(InputAction.CallbackContext context)
    {
        if (this.maxFPSRate.Length <= 0 || !context.performed)
        {
            return;
        }

        this.currentFPSRateIndex++;

        if (this.currentFPSRateIndex > this.maxFPSRate.Length - 1)
        {
            this.currentFPSRateIndex = 0;
        }

        Application.targetFrameRate = this.maxFPSRate[this.currentFPSRateIndex];
    }

    public void BeABox(InputAction.CallbackContext context)
    {
        if (!context.performed || this.boxGraphic == null || this.playerGraphics.Length <= 0)
        {
            return;
        }

        foreach (GameObject item in this.playerGraphics)
        {
            item.SetActive(this.player);
        }

        this.player = !this.player;

        this.boxGraphic.SetActive(this.player);
    }

    private void DrawSpheres()
    {
        if (this.objectsToFollow.Length <= 0)
        {
            return;
        }

        this.spawnTimer -= Time.deltaTime;

        if (this.spawnTimer <= 0)
        {
            foreach (Transform objectToFollow in this.objectsToFollow)
            {
                Sphere sphere = this.spheres.Count > 0 ? this.spheres.Dequeue() : Instantiate(this.sphere);

                sphere.lifeTime = this.lifeTime;
                sphere.transform.position = objectToFollow.position;
                sphere.gameObject.SetActive(true);
                sphere.sphereRenderer.material.color = sphere.normalColor;
            }

            this.spawnTimer = this.spawnRate;
        }  
    }

    public void Spheres(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        this.drawSpheres = !this.drawSpheres;
    }
}
