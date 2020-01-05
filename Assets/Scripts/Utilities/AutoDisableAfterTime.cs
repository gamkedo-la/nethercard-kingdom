using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoDisableAfterTime : MonoBehaviour
{
    [SerializeField] private float delay = 1f;
    [SerializeField] private string sceneToLoad = "Main";
    [SerializeField] private string sceneToUnLoad = "World Map";
    [SerializeField] private GameObject[] objectsToActivateOnDisable = null;
    [SerializeField] private GameObject[] objectsToDectivateOnDisable = null;

    private float _delay = -999f;

    public void Deactivate(GameObject deactivate)
    {
        deactivate.SetActive(false);
    }

    public void IterateThroughObjectsList()
    {
        if(objectsToActivateOnDisable != null)
            foreach(var o in objectsToActivateOnDisable)
                o.SetActive(true);

        if(objectsToDectivateOnDisable != null)
            foreach(var o in objectsToDectivateOnDisable)
                o.SetActive(false);
    }

    public void SetToZero()
    {
        delay = 0f;
    }

    public void LoadLevel( )
    {
        SceneManager.LoadScene( sceneToLoad, LoadSceneMode.Single );
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

            if(objectsToDectivateOnDisable != null)
                foreach(var o in objectsToDectivateOnDisable)
                    o.SetActive(false);

            gameObject.SetActive(false);
        }
        else
        {
            delay -= Time.deltaTime;
        }
    }
}
