/**
 * Description: Enums used in the game.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

public enum ConflicSide
{
	Player,
	Enemy,
	All
}

[System.Flags]
public enum CardType
{
	Undefined = 1,
	Unit = 2,
	DirectOffensiveSpell = 4,
	DirectDefensiveSpell = 8,
	AoeSpell = 16,
	EnemyUnit = 32
}

public enum CardSelectionMode
{
	InHand,
	InCollection,
	InDeck,
	InUpgrade
}

public enum CardLevel
{
	Level1,
	Level2,
	Level3
}

public enum CardClass
{
	Other,
	Melee,
	Ranged,
	Support
}

public enum ProjecttileType
{
    Normal,
    Siege
}
