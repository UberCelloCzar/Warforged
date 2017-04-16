using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using Warforged;

public class AurianTest {

	public static Character opponent = new Aurian();

	[Test]
	public void ChainTest()
	{
		// Arrange
		Character aurian = new Aurian();
		aurian.standby.Add(new Aurian.OpeningBlow());
		aurian.standby.Add(new Aurian.RelentlessAssault());
		aurian.standby.Add(new Aurian.ClarityofMind());
		// Set up the chain
		aurian.prevCards.Add(aurian.standby[2]); // G
		aurian.prevCards.Add(aurian.standby[1]); // R
		aurian.prevCards.Add(aurian.standby[0]); // R

		Assert.AreEqual(Warforged.Color.green, aurian.prevCards[0].color);
		Assert.AreEqual(Warforged.Color.red, aurian.prevCards[1].color);
		Assert.AreEqual(Warforged.Color.red, aurian.prevCards[2].color);
		Assert.AreEqual(true, aurian.hasChain("GRR"));
	}

	[Test]
	public void WrathofEraTest()
	{
		// Arrange
		Character aurian = new Aurian();
		aurian.hand.Add(new Aurian.BodyandSoul());
		aurian.hand.Add(new Aurian.EvadingStep());
		aurian.hand.Add(new Aurian.SoulStrike());
		aurian.hand.Add(new Aurian.PrimedAttack());
		foreach (Character.Card c in aurian.hand)
		{
			c.init(aurian);
		}
		aurian.standby.Add(new Aurian.OpeningBlow());
		aurian.standby.Add(new Aurian.RelentlessAssault());
		aurian.standby.Add(new Aurian.ClarityofMind());
		aurian.standby.Add(new Aurian.CounteringStrike());
		foreach (Character.Card c in aurian.standby)
		{
			c.init(aurian);
		}
		aurian.invocation.Add(new Aurian.EtherealStrike());
		aurian.invocation.Add(new Aurian.WrathofEra());
		aurian.invocation.Add(new Aurian.AbsoluteFocus());
		aurian.invocation.Add(new Aurian.StrengthofSpirit());
		foreach (Character.Card c in aurian.invocation)
		{
			c.init(aurian);
		}
		aurian.setOpponent(opponent);
		// Set up the chain
		aurian.prevCards.Add(aurian.standby[2]); // G
		aurian.prevCards.Add(aurian.standby[1]); // R
		aurian.prevCards.Add(aurian.standby[0]); // R

		// Act
		// Activate the card effect
		aurian.invocation[1].activate();

		// Assert
		// The object has a new name
		Assert.AreEqual(4, aurian.damage);
	}
}
