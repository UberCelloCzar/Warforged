using System;
using System.Collections.Generic;
using UnityEngine;

namespace Warforged
{
    public class Tyras : Character
    {
        private bool aldrasGuard;
        public Tyras() : base()
        {
            name = "Tyras";
            title = "Hero of a Lost Age";
            aldrasGuard = false;
        }

        // dawn()

        // playCard()

		// declarePhase()

        public override void damagePhase()
        {
			// Activate effects of cards first, then the UI updates, then this stuff happens
            if (isPlayer1 && (currCard.color - opponent.currCard.color == -1
                || currCard.color - opponent.currCard.color == 2))
            {
                // An Oath Unforgotten
                // TODO I think there's a more efficient way to do this
                foreach (Card card in invocation)
                {
                    if (card is AnOathUnforgotten && card.active
                        && standby.Count > 0)
                    {
                        Game.library.setPromptText("Choose a card to take from your standby.");
                        while (true)
                        {
                            var card1 = Game.library.waitForClick();
                            if (standby.Contains(card1))
                            {
                                takeStandby(card1);
                                break;
                            }
                        }
                    }
                }
                Game.library.setPromptText("");
                bolster(); // This may cause problems; idk how things should be ordered
			}
			// Damage applies after this; I don't think that should be an issue
			base.damagePhase();
        }

        public override int dealDamage()
        {
            int damage = base.dealDamage();
            // A Promise Unbroken
            foreach (Card card in invocation)
            {
                if (card is APromiseUnbroken && card.active)
                {
                    if (damage > 0)
                    {
                        empower += 1;
                    }
                    // Guard effect
                    if (negate > opponent.pierce && opponent.damage > 0 && hp <= 4)
                    {
						healSelf(1);
                    }
                }
            }
            if (negate > opponent.pierce && opponent.damage > 0 && aldrasGuard)
            {
                // May take a standby offense
                if (isPlayer1 && hasStandbyColor(Color.red))
                {
                    while (true)
                    {
						Card cardToTake = Game.library.waitForClickOrCancel("You may choose a standby Offense card to send to your hand.");
                        if (cardToTake == null)
                        {
                            break;
                        }
                        else if (standby.Contains(cardToTake) && cardToTake.color == Color.red)
                        {
                            takeStandby(cardToTake);
                            break;
                        }
                    }
                }
                aldrasGuard = false;
            }
			return damage;
        }

        // dusk()

        public class OnraisStrike : Card
        {
            public OnraisStrike() : base()
            {
                name = "Onrai's Strike";
                effect = "Deal 2 damage.\nCounter (G): Deal 2 additional damage.";
                color = Color.red;
            }

            public override void activate()
            {
                user.addDamage(2);
                if (user.opponent.currCard.color == Color.green)
                {
                    user.damage += 2;
                }
            }
        }

        public class OnslaughtofTyras : Card
        {
            public OnslaughtofTyras() : base()
            {
                name = "Onslaught of Tyras";
                effect = "Deal 2 damage.\nBloodlust: Negate 2 damage this turn.";
                color = Color.red;
            }

            public override void activate()
            {
                user.addDamage(2);
                if (user.bloodlust)
                {
                    user.negate += 2; // Don't use reinforce
                }
            }
        }

        public class ASoldiersRemorse : Card
        {
            Card standbyCard = null;
            public ASoldiersRemorse() : base()
            {
                name = "A Soldier's Remorse";
                effect = "Deal 2 damage.\nChain (R): You may send a Standby Defense card to your hand.";
                color = Color.red;
            }

            public override void activate()
            {
                user.addDamage(2);
                standbyCard = null;
            }

            public override void declare()
            {
                if (user.hasChain("R") && user.hasStandbyColor(Color.blue))
                {
                    while (true)
                    {
                        // Ideally this should only happen once
                        // But if the user chooses an invalid card, try again
						standbyCard = Game.library.waitForClickOrCancel("Choose a blue standby card to send to your hand.");
                        if (standbyCard != null) // Safety check
                        {
                            if (standbyCard.color == Color.blue && user.standby.Contains(standbyCard))
                            {
                                Game.library.setPromptText("");
                                break;
                            }
                        }
                        else
                        {
                            Game.library.setPromptText("");
                            break;
                        }

                    }
                }
            }
        }

        public class WarriorsResolve : Card
        {
            public Card card1 = null;
            public Card card2 = null;
            public WarriorsResolve() : base()
            {
                name = "Warrior's Resolve";
                effect = "Send a Standby card to your hand.\nCounter (B): Send an additional Standby card to your hand.";
                color = Color.green;
            }

            public override void activate()
            {
                user.takeStandby(card1);
                card1 = null;
                user.takeStandby(card2);
                card2 = null;
            }

            public override void declare()
            {
                if (user.standby.Count > 0)
                {
                    while (true)
                    {
                        Game.library.setPromptText("Choose a standby card to send to your hand.");
                        card1 = Game.library.waitForClick();
                        if (user.standby.Contains(card1))
                        {
                            break;
                        }

                    }
                }
                Game.library.setPromptText("");
                if (user.opponent.currCard.color == Color.blue && user.standby.Count > 1)
                {
                    while (true)
                    {
                        Game.library.setPromptText("Choose another standby card to send to your hand.");
                        card2 = Game.library.waitForClick();
                        if (user.standby.Contains(card2) && card2 != card1)
                        {
                            break;
                        }
                    }

                    Game.library.setPromptText("");
                }
            }
        }

        public class GrimKnightsDread : Card
        {
            Card stdbyCard1 = null;
            Card stdbyCard2 = null;
            Card handCard1 = null;
            Card handCard2 = null;
            public GrimKnightsDread() : base()
            {
                name = "Grim Knight's Dread";
                effect = "Swap 2 cards in your hand with 2 Standby cards.\nEndure (5): Put all Standby Offense cards in your hand.";
                color = Color.green;
            }

            public override void activate()
            {
                user.swap(handCard1, stdbyCard1);
                handCard1 = null;
                stdbyCard1 = null;
                user.swap(handCard2, stdbyCard2);
                handCard2 = null;
                stdbyCard2 = null;
                if (user.hp <= 5) // TODO does this come before or after
                {
                    List<Card> tmp = new List<Card>();
                    foreach (Card card in user.standby)
                    {
                        tmp.Add(card);
                    }
                    foreach(Card t in tmp)
                    {
                        if (t.color == Color.red)
                        {
                            user.takeStandby(t);
                        }
                    }
                }
            }

            public override void declare()
            {
                // First swap
                if (user.standby.Count > 0 && user.hand.Count > 0)
                {
                    Game.library.setPromptText("Pick the first pair of cards to swap.");
                    while (true)
                    {
                        Character.Card card1 = Game.library.waitForClick();
                        Game.library.highlight(card1, 255, 255, 0);
                        Character.Card card2 = Game.library.waitForClick();
                        Game.library.clearAllHighlighting();
                        if (user.hand.Contains(card1) && user.standby.Contains(card2))
                        {
                            handCard1 = card1;
                            stdbyCard1 = card2;
                            break;
                        }
                        else if (user.hand.Contains(card2) && user.standby.Contains(card1))
                        {
                            handCard1 = card2;
                            stdbyCard1 = card1;
                            break;
                        }
                    }
                }
                // Second swap
                if (user.standby.Count > 1 && user.hand.Count > 1)
                {
                    Game.library.setPromptText("Pick the second pair of cards to swap.");
                    while (true)
                    {
                        Character.Card card1 = Game.library.waitForClick();
                        Game.library.highlight(card1, 255, 255, 0);
                        Character.Card card2 = Game.library.waitForClick();
                        Game.library.clearAllHighlighting();
                        if (user.hand.Contains(card1) && user.standby.Contains(card2) && handCard1 != card1 && handCard1 != card2 && stdbyCard1 != card1 && stdbyCard1 != card2)
                        {
                            handCard2 = card1;
                            stdbyCard2 = card2;
                            break;
                        }
                        else if (user.hand.Contains(card2) && user.standby.Contains(card1) && handCard1 != card1 && handCard1 != card2 && stdbyCard1 != card1 && stdbyCard1 != card2)
                        {
                            handCard2 = card2;
                            stdbyCard2 = card1;
                            break;
                        }
                    }
                }
            }
        }

        public class DecryingRoar : Card
        {
            public DecryingRoar() : base()
            {
                name = "Decrying Roar";
                effect = "Negate 2 damage.\nCounter (R): Seal (B).";
                color = Color.blue;
            }

            public override void activate()
            {
                user.addNegate(2);
                if (user.opponent.currCard.color == Color.red)
                {
                    user.sealCard(Color.blue);
                }
            }
        }

        public class ArmorofAldras : Card
        {
            public ArmorofAldras() : base()
            {
                name = "Armor of Aldras";
                effect = "Negate 2 damage.\nGuard: You may send a standby Offense card to you hand.\nAlign (G, R): Safeguard.";
                color = Color.blue;
            }

            public override void activate()
            {
                user.addNegate(2);
                ((Tyras)user).aldrasGuard = true;
                if (user.hasAlign("GR"))
                {
                    user.negate = 255;
                }
            }
        }

        public class ABrothersVirtue : Card
        {
            public ABrothersVirtue() : base()
            {
                name = "A Brother's Virtue";
                effect = "Negate 2 damage.\nStalwart: Reflect.";
                color = Color.blue;
            }

            public override void activate()
            {
                user.addNegate(2);
                if (user.stalwart)
                {
                    user.reflect = true;
                }
            }
        }

        public class AnOathUnforgotten : Card
        {
            public AnOathUnforgotten() : base()
            {
                name = "An Oath Unforgotten";
                effect = "Whenever you play a counter type of your opponent's card, send a Standby card to your hand.";
                color = Color.black;
                active = false;
            }

            // Effects are covered in declarePhase()
        }

        public class APromiseUnbroken : Card
        {
            public APromiseUnbroken() : base()
            {
                name = "A Promise Unbroken";
                effect = "Strike: Empower (1)\nGuard: Endure (4): Gain 1 health";
                color = Color.black;
                active = false;
            }
            // Effects are covered in dawn()
        }

        public class SunderingStar : Card
        {
            public SunderingStar() : base()
            {
                name = "Sundering Star";
                effect = "Strive (2): Deal 2 damage for each of your standby Offense cards.\nCounter (G): Deal 2 additional damage.";
                color = Color.red;
                setAwakening();
                active = false;
            }

            public override void activate()
            {
                // Make sure we have two active inherents
                bool one = false;
                bool two = false;
                foreach (Card inherent in user.invocation)
                {
                    if (inherent.active)
                    {
                        if (one)
                        {
                            two = true;
                            break;
                        }
                        one = true;
                    }
                }
                if (!two)
                {
                    return;
                }
                // Strive cards NOT A REUSABLE ALGORITHM
                foreach (Card inherent in user.invocation)
                {
                    user.strive(inherent);
                }
                // Check standby offense cards
                user.addDamage(0); // Just add empower
                foreach (Card card in user.standby)
                {
                    if (card.color == Color.red)
                    {
                        user.damage += 2;
                    }
                }
                // Counter (G)
                if (user.opponent.currCard.color == Color.green)
                {
                    user.damage += 2;
                }
            }
        }

        public class IntheKingsWake : Card
        {
            short strove = 0;
            public IntheKingsWake() : base()
            {
                name = "In the King's Wake";
                effect = "Strive (X): Gain 2 health for every Inherent Card you deactivated.\nCounter (R): Safeguard.";
                color = Color.blue;
                setAwakening();
                active = false;
            }

            public override void activate()
            {
                user.heal += 2 * strove;
                strove = 0;
                if (user.opponent.currCard.color == Color.red)
                {
                    user.negate = 255;
                }
            }

            public override void declare()
            {
                // First declare
                while (true)
                {
                    Card card1 = Game.library.waitForClickOrCancel("Choose an inherent to strive or cancel.");
                    if (card1 == null)
                    {
                        return;
                    }
                    else if (user.strive(card1))
                    {
                        ((IntheKingsWake)this).strove += 1;
                        break;
                    }
                }

                // Second declare
                while (true)
                {
                    Card card2 = Game.library.waitForClickOrCancel("Choose an additional inherent to strive or cancel.");
                    if (card2 == null)
                    {
                        return;
                    }
                    else if (user.strive(card2))
                    {
                        ((IntheKingsWake)this).strove += 1;
                        break;
                    }
                }
            }
        }
    }
}
