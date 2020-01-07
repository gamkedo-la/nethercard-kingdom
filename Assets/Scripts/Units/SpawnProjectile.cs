using UnityEngine;
using UnityEngine.Assertions;

public class SpawnProjectile : Attack
{
    [SerializeField]
    private GameObject projectilePrefab = null;

    private float timeToNextAttack = 0;
    private Unit currentOpponent = null;

    override protected void Start()
    {
        base.Start();

        //Assert.IsNotNull(unit, $"Please assign <b>{nameof(unit)}</b> field on <b>{GetType().Name}</b> script on <b>{name}</b> object");
    }

    void Update()
    {
        TryToAttack();
    }

    public void OnNewOponent(Unit newOponent) => currentOpponent = newOponent;

    private void TryToAttack()
    {
        if (Frozen)
            return;

        timeToNextAttack -= Time.deltaTime;

        // Needs to be in attack range of an oponent and have no attack cool-down
        if (!currentOpponent || timeToNextAttack > 0)
            return;

        GameObject go = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
		ProjectileMovement projectileMovement = go.GetComponent<ProjectileMovement>( );

		Vector3 direction = (currentOpponent.Center - transform.position).normalized;
		projectileMovement.direction = direction;
		projectileMovement.damage = atackDamage;

        if ( attackSound )
            attackSound.Play( );

        timeToNextAttack = atackDelay;

		if ( CheatAndDebug.Instance.ShowDebugInfo )
		{
			Debug.DrawLine( transform.position, currentOpponent.Center, Color.red, 0.2f );
			Debug.DrawLine( transform.position, transform.position + direction, Color.green, 0.5f );
		}
    }
}
