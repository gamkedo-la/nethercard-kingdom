using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    private float projectileSpeed = 1.0f;
    private GameObject projectile = null;
    public Unit unitFiredFrom = null;

    public Vector3 direction;

    void Awake()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(direction * projectileSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Enemy" || other.gameObject.tag == "ProjectileShredder")
        {
            Destroy(this.gameObject);
            Debug.Log(other.gameObject.name);
        }
        
    }
}
