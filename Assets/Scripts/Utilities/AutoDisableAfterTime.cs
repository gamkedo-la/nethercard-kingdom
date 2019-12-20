using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDisableAfterTime : MonoBehaviour
{
    [SerializeField] private float delay = 1f;

    private float _delay = -999f;

    void Start()
    {
        if(_delay < -1f) _delay = delay;
    }

    void Update()
    {
        if(delay <= 0f) { delay = _delay; gameObject.SetActive(false); }
        else delay -= Time.deltaTime;
    }
}
