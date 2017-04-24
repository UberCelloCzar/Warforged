using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using Warforged;

public class CharacterTest {
    // A suite of tests to make sure that general character methods work properly
    // For the sake of testing, Tyras is used as the instantiated character

	Character player;
	Character opponent;
    // Run before every test
	[SetUp]
    public void Setup()
    {
        player = new Tyras();
        opponent = new Tyras();
        player.setOpponent(opponent);
        opponent.setOpponent(player);
    }

    /// Make sure that bolsters properly activate inherents and awakenings
    /// Makes sure that an inherent is left there and set to active,
    /// and that an awakening is sent to the hand and set to active.
    [Test]
    public void BolsterTest()
    {
        // Arrange
        player.invocation.Add(new Tyras.AnOathUnforgotten());
        player.invocation.Add(new Tyras.SunderingStar());

        // Act
        player.bolster();
        player.bolster();

        // Assert
        Assert.AreEqual(1, player.hand.Count);
        Assert.AreEqual(1, player.invocation.Count);
        Assert.AreEqual(true, player.hand[0].active);
        Assert.AreEqual(true, player.invocation[0].active);
    }

    [Test]
    public void TakeDamageTest()
    {
        // Arrange
        // Act
        player.takeDamage(3);

        // Assert
        Assert.AreEqual(7, player.hp);
    }

    [Test]
    public void UndyingTest()
    {
        // Arrange
        player.undying = true;

        // Act
        player.takeDamage(11);

        // Assert
        Assert.AreEqual(1, player.hp);
    }

    [Test]
    public void OverhealTest()
    {
        // Arrange
		player.heal += 3;
		//player.healSelf();
		player.dawn(); // Sets the overheal to 3
		player.heal += 1;
		//player.healSelf();
		// End result: player has 14 hp with an overheal of 3
		// They should lose 3 hp next dawn, then 1 more the dawn after

        // Act
		player.dawn();
		int dawn1 = player.hp;
		player.dawn();
		int dawn2 = player.hp;

		// Assert
		Assert.AreEqual(11, dawn1);
		Assert.AreEqual(10, dawn2);
    }

    [Test]
    public void PierceTest()
    {
        // Arrange
        player.damage = 2;
        player.pierce = 2;
        opponent.negate = 3;

        // Act
        int damage = player.dealDamage();

        // Assert
        Assert.AreEqual(1, damage);
        Assert.AreEqual(9, opponent.hp);
    }

	[Test]
	public void ChainTest()
	{
		// Arrange
        player.prevCards.Add(new Tyras.OnraisStrike());     // R
        player.prevCards.Add(new Tyras.WarriorsResolve());  // G
        player.prevCards.Add(new Tyras.ArmorofAldras());    // B
        player.prevCards.Add(new Tyras.ASoldiersRemorse()); // R
        // The total chain should be RGBR, since the first card played is at the beginning

        // Act
        bool chain1 = player.hasChain("R");
        bool chain2 = player.hasChain("BR");
        bool chain3 = player.hasChain("GBR");
        bool chain4 = player.hasChain("RGBR");

        // Assert
		Assert.AreEqual(true, chain1);
		Assert.AreEqual(true, chain2);
		Assert.AreEqual(true, chain3);
		Assert.AreEqual(true, chain4);
	}

    [Test]
    public void AlignTest()
    {
        // Arrange
        player.standby.Add(new Tyras.OnraisStrike());     // R
        player.standby.Add(new Tyras.WarriorsResolve());  // G
        player.standby.Add(new Tyras.ArmorofAldras());    // B
        player.standby.Add(new Tyras.ASoldiersRemorse()); // R
        // The total align should be RGBR, going from left to right

        // Act
        bool align1 = player.hasAlign("RG");
        bool align2 = player.hasAlign("GB");
        bool align3 = player.hasAlign("BR");
        bool align4 = player.hasAlign("RGB");
        bool align5 = player.hasAlign("GBR");
        bool align6 = player.hasAlign("RGBR");

        // Assert
        Assert.AreEqual(true, align1);
        Assert.AreEqual(true, align2);
        Assert.AreEqual(true, align3);
        Assert.AreEqual(true, align4);
        Assert.AreEqual(true, align5);
        Assert.AreEqual(true, align6);
    }

}
