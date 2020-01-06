using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    public float projectileSpeed = 3.0f;
    public Vector2 direction;
    public float damage = 1;
    public ProjecttileType projectileType = ProjecttileType.Normal;
    public float siegeDamageMultiplier = 3;

    void Update()
    {
		Vector3 moveVector = direction * projectileSpeed * Time.deltaTime;
		//transform.Translate( moveVector, Space.World );
		transform.position += moveVector;

		if ( CheatAndDebug.Instance.ShowDebugInfo )
			Debug.DrawLine( transform.position, transform.position + (Vector3) direction, Color.blue );

		float angle = Mathf.Atan2( moveVector.y, moveVector.x ) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler( 0f, 0f, angle );
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Enemy" || other.gameObject.tag == "ProjectileShredder")
        {
            if(other.gameObject.tag == "Enemy")
            {
				HP enemyHP = other.GetComponent<HP>();
				Unit unit = other.GetComponent<Unit>();
                float damageToInflict = damage;
                if (other.gameObject.name == "Enemy Wall" && projectileType == ProjecttileType.Siege)
                {
                    damageToInflict *= siegeDamageMultiplier;
                }
                enemyHP.DoDamage(damageToInflict, unit ? unit.Center : transform.position);
            }
            Destroy(gameObject);
        }
    }
}
