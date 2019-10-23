/**
 * Description: Game enums.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

public enum ConflicSide
{
	Player,
	Enemy,
	All
}

public enum CardType
{
	Unit,
	DirectOffensiveSpell,
	DirectDefensiveSpell,
	AoeSpell,
	None
}

public enum CardSelectionMode
{
	InHand,
	InCollection,
	InDeck,
	InUpgrade
}