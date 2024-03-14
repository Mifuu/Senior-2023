using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawn : MonoBehaviour
{
    public GameObject gameObject;

    public float period = 0.5f;
    float timer = 0;
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > period)
        {
            timer = 0;
            Instantiate(gameObject, transform.position, Quaternion.identity);
        }

        if (Input.GetKeyDown(KeyCode.O))
            Instantiate(gameObject, transform.position, Quaternion.identity);
    }
}
