/**
 * Description: Manages Units on the field.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitsManager : MonoBehaviour
{
	public static UnitsManager Instance { get; private set; }

	public List<Unit> EnemyUnits { get; private set; } = new List<Unit>( );
	public List<Unit> PlayerUnits { get; private set; } = new List<Unit>( );

	private void Awake( )
	{
		if ( Instance != null && Instance != this )
			Destroy( this );
		else
			Instance = this;
	}

	private void OnDestroy( ) { if ( this == Instance ) { Instance = null; } }

	public void AddUnit( Unit unit, ConflicSide side )
	{
		if ( side == ConflicSide.Player )
			PlayerUnits.Add( unit );
		else
			EnemyUnits.Add( unit );
	}

	public void RemoveUnit( Unit unit, ConflicSide side )
	{
		if ( side == ConflicSide.Player )
			PlayerUnits.Remove( unit );
		else
			EnemyUnits.Remove( unit );
	}

	public Unit FindOponent( ConflicSide mySide, Vector2 myPosition, float distance )
	{
		Unit oponent = null;

		List<Unit> oponents = mySide == ConflicSide.Player ? EnemyUnits : PlayerUnits;

		foreach ( var op in oponents )
		{
			if ( Vector2.Distance( op.Center, myPosition ) <= distance )
			{
				oponent = op;
				return oponent;
			}
		}

		return oponent;
	}

	public Unit[] FindUnitsInRange( Vector2 myPosition, float distance, ConflicSide mySide )
	{
		List<Unit> units = mySide == ConflicSide.Player ? EnemyUnits : PlayerUnits;
		Unit[] unitsInRange = units.Select( u => u ).Where( u => Vector2.Distance( u.Center, myPosition ) <= distance ).ToArray( );

		return unitsInRange;
	}

	public Unit[] FindUnitsInRange( Vector2 myPosition, float distance )
	{
		IEnumerable<Unit> units = EnemyUnits.Concat( PlayerUnits );
		Unit[] unitsInRange = units.Select( u => u ).Where( u => Vector2.Distance( u.Center, myPosition ) <= distance ).ToArray( );

		return unitsInRange;
	}

	public void StopUnits( )
	{
		foreach ( var unit in EnemyUnits )
			unit.Stop( );

		foreach ( var unit in PlayerUnits )
			unit.Stop( );
	}
}
