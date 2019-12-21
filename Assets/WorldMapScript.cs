using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapScript : MonoBehaviour
{
    [SerializeField] private int moveToNode = 1;
    [SerializeField] private float delay = 4f;
    [SerializeField] private float resetDelay = 0.9f;
    [SerializeField] private Vector3 defaultCamPosition;

    private float timer = 0f;

    private GameObject cam;

    void Start()
    {
        cam = GameObject.Find("FollowCam");

        cam.transform.position = gameObject.transform.GetChild(2).GetChild(moveToNode - 1).transform.position;

        timer = delay;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if(timer <= 0f)
        {
            if(timer < -resetDelay)
            {
                cam.transform.position = defaultCamPosition;
                GetComponent<AutoDisableAfterTime>().SetToZero();
            }

            return;
        }

        cam.transform.position = Vector3.Lerp(
            gameObject.transform.GetChild(2).GetChild(moveToNode - 1).transform.position,
            gameObject.transform.GetChild(2).GetChild(moveToNode).transform.position, 1f - (timer / delay));

        Vector3 pos = cam.transform.position;
        pos.z = -10;
        cam.transform.position = pos;
    }
}
