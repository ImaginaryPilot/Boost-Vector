using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementvector = new Vector3(10f, 10f, 10f);
    [SerializeField] float period = 2f;

    float MovementFactor;
    Vector3 startPos;
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (period <= Mathf.Epsilon) { return;  }
        float cycles = Time.time / period;

        const float tau = Mathf.PI * 2;
        float rawSinWave = Mathf.Sin(cycles * tau);

        MovementFactor = rawSinWave / 2f + 0.5f;
        Vector3 offset = movementvector * MovementFactor;
        transform.position = startPos + offset;
    }
}
