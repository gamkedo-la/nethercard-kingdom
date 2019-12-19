using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    private float projectileSpeed = 1.0f;
    public Vector2 direction;
    public float damage = 1;
    Unit currentOpponent;
    HP enemyHP;

    void Awake()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(direction * projectileSpeed * Time.deltaTime);

		float rot = Mathf.Atan2( direction.y, direction.x ) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler( 0f, 0f, rot );
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("projectile collided with " + other.gameObject.name);
        if(other.gameObject.tag == "Enemy" || other.gameObject.tag == "ProjectileShredder")
        {
            if(other.gameObject.tag == "Enemy")
            {
                currentOpponent = other.GetComponent<Unit>();
                enemyHP = other.GetComponent<HP>();
                enemyHP.DoDamage(damage, currentOpponent.Center);
            }
            Destroy(gameObject);
            //Debug.Log(other.gameObject.name);
        }
    }
}
