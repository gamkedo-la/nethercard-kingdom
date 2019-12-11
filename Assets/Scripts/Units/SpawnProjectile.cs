
using UnityEngine;
using UnityEngine.Assertions;

public class SpawnProjectile : Attack
{

    //[SerializeField]
    //private float attackRate = 1.0f;
    [SerializeField]
    private GameObject projectilePrefab = null;
    [SerializeField]
    private Transform attackOrigin = null;

    private float timeToNextAttack = 0;
    [SerializeField]
    private Unit unit = null;
    private Unit currentOpponent = null;

    override protected void Start()
    {
        base.Start();

        Assert.IsNotNull(unit, $"Please assign <b>{nameof(unit)}</b> field on <b>{GetType().Name}</b> script on <b>{name}</b> object");
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
        {
            return;
        }

        AimProjectile();
        Instantiate(projectilePrefab, attackOrigin);
        timeToNextAttack = atackDelay;
    }

    private void AimProjectile()
    {

        ProjectileMovement projectileMovement = projectilePrefab.GetComponent<ProjectileMovement>();
        projectileMovement.direction = currentOpponent.Center - unit.Center;

        if (CheatAndDebug.Instance.ShowDebugInfo)
            Debug.DrawLine(unit.Center, currentOpponent.Center, Color.red, 0.2f);
    }
}
