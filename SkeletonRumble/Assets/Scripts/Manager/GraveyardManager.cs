using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraveyardManager : MonoBehaviour
{
    public GameObject[] tombs;
    private GameObject[] randomTombs;
    private int tombCounter;

    private void Start()
    {
        System.Random rnd = new System.Random();
        this.randomTombs = this.tombs.OrderBy(x => rnd.Next()).ToArray();
        this.tombCounter = 0;

        foreach (GameObject item in this.tombs)
        {
            item.SetActive(false);
        }
    }

    public void SpawnTomb()
    {
        if (this.tombCounter > this.randomTombs.Length - 1)
        {
            return;
        }

        this.randomTombs[this.tombCounter].SetActive(true);
        this.tombCounter++;
    }
}
