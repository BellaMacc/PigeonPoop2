using System.Collections.Generic;
using UnityEngine;

public class trafficControl : MonoBehaviour
{
    public bool trafficDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("switchDirection", 10f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void switchDirection()
    {
        trafficDirection = !trafficDirection;
        Invoke("switchDirection", 10f);
    }
}
