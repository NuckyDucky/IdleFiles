using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Projectile : MonoBehaviour
{
    private GameObject gO;
    public float timeToDestroy = 5.0f;

    private void Awake()
    {
        gO = GetComponent<GameObject>();
    }
    private void Update()
    {
        timeToDestroy -= Time.deltaTime;
        if (timeToDestroy <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
