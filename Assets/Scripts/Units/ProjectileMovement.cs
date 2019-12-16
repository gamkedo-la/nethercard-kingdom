using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    private float projectileSpeed = 1.0f;
    public Vector2 direction;
    Unit currentOpponent;
    HP enemyHP;

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
        Debug.Log("projectile collided with " + other.gameObject.name);
        if(other.gameObject.tag == "Enemy" || other.gameObject.tag == "ProjectileShredder")
        {
            if(other.gameObject.tag == "Enemy")
            {
                currentOpponent = other.GetComponent<Unit>();
                enemyHP = other.GetComponent<HP>();
                enemyHP.DoDamage(20f, currentOpponent.Center);
            }
            Destroy(this.gameObject);
            Debug.Log(other.gameObject.name);
        }
        
    }
}
