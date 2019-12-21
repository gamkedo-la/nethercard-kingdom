using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDisableAfterTime : MonoBehaviour
{
    [SerializeField] private float delay = 1f;
    [SerializeField] private GameObject[] objectsToActivateOnDisable = null;

    private float _delay = -999f;

    public void SetToZero()
    {
        delay = 0f;
    }

    void Start()
    {
        if(_delay < -1f) _delay = delay;
    }

    void Update()
    {
        if(delay <= 0f)
        {
            delay = _delay;

            if(objectsToActivateOnDisable != null)
                foreach(var o in objectsToActivateOnDisable)
                    o.SetActive(true);

            gameObject.SetActive(false);
        }
        else
        {
            delay -= Time.deltaTime;
        }
    }
}
