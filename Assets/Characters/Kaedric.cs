using System;
using System.Collections.Generic;

namespace Warforged
{
    public class Kaedric : Character
    {
        public Kaedric() : base()
        {
            name = "Kaedric";
            title = "Heir of Chaos";
        }

        private class ShiftingChaos : Card
        {
            private bool leftSuspend, rightSuspend;

            public ShiftingChaos() : base()
            {
                name = "Shifting Chaos";
                effect = "You may Suspend your Leftmost and/or Rightmost Standby cards. Deal 1 damage for each card suspended.";
                color = Color.red;
                leftSuspend = false;
                rightSuspend = false;
            }

            public override void activate()
            {
                if (leftSuspend)
                {
                    user.addDamage(1);
                }
                if (rightSuspend)
                {
                    user.addDamage(1);
                }
                leftSuspend = false;
                rightSuspend = false;
            }

            public override void declare()
            {
                // TODO
                while (true)
                {
                    Card card = Game.library.waitForClickOrCancel("You may select your leftmost Standby card to Suspend it. TEMPORARY");
                    if (card == null)
                    {
                        break;
                    }
                    if (card == user.standby[0])
                    {
                        leftSuspend = true;
                        break;
                    }
                }
                while (true)
                {
                    Card card = Game.library.waitForClickOrCancel("You may select your rightmost Standby card to Suspend it. TEMPORARY");
                    if (card == null)
                    {
                        break;
                    }
                    if (card == user.standby[user.standby.Count - 1])
                    {
                        rightSuspend = true;
                        break;
                    }
                }
            }
        }

        private class StrikingChaos : Card
        {
            public StrikingChaos() : base()
            {
                name = "Striking Chaos";
                effect = "Deal 2 damage.\nAlign (G, B, G): Deal 3 additional damage.";
                color = Color.red;
            }

            public override void activate()
            {
                user.addDamage(2);
                if (user.hasAlign("GBG"))
                {
                    user.addDamage(3);
                }
            }
        }

        private class GraspofChaos : Card
        {
            private Card suspendCard, returnCard;

            public GraspofChaos() : base()
            {
                name = "Grasp of Chaos";
                effect = "Deal 1 damage for each of your Suspended cards.\nAlign (R, B): Suspend 1 of your cards and then return a Suspended card to your hand.";
                color = Color.red;
                suspendCard = null;
                returnCard = null;
            }

            public override void activate()
            {
                //TODO
                user.addDamage(user.suspended.Count);
                if (user.hasAlign("RB"))
                {
                    user.suspend(suspendCard);
                    user.unSuspend(returnCard);
                    suspendCard = null;
                    returnCard = null;
                }
            }

            public override void declare()
            {
                if (user.hasAlign("RB"))
                {
                    Game.library.setPromptText("Select a card to suspend.");
                    while (true)
                    {
                        suspendCard = Game.library.waitForClick();
                        if (user.hand.Contains(suspendCard) || user.standby.Contains(suspendCard))
                        {
                            break;
                        }
                    }
                    Game.library.setPromptText("Select a suspended card to return. Can be the card being suspended this turn.");
                    while (true)
                    {
                        returnCard = Game.library.waitForClick();
                        if (user.suspended.Contains(returnCard) || returnCard == suspendCard)
                        {
                            break;
                        }
                    }
                    Game.library.setPromptText("");
                }
            }
        }

// Intent
        private class EncroachingDarkness : Card
        {
            public EncroachingDarkness() : base()
            {
                name = "Encroaching Darkness";
                effect = "Return the leftmost Standby card to your hand.\nAlign (B, R): Return all Suspended cards to your hand.";
                color = Color.green;
            }

            public override void activate()
            {
                user.takeStandby(user.standby[0]);
                if (user.hasAlign("BR"))
                {
                    foreach (Card card in user.suspended)
                    {
                        user.unSuspend(card);
                    }
                }
            }
        }

        private class ScheminginShadow : Card
        {
            private List<Card> handCards, standbyCards;
            public ScheminginShadow() : base()
            {
                name = "Scheming in Shadow";
                effect = "Swap 2 cards in your hand with two Standby cards.\nResidual: Rearrange up to 3 of your Standby cards in any order.";
                color = Color.green;
                handCards = new List<Card>(2);
                standbyCards = new List<Card>(2);
            }

            public override void activate()
            {
                user.swap(handCards[0], standbyCards[0]);
                handCards.Clear();
                standbyCards.Clear();
            }

            public override void declare()
            {
                Game.library.setPromptText("Select the first pair of cards to swap.");
                while (true)
                {
                    Character.Card card1 = Game.library.waitForClick();
                    Game.library.highlight(card1, 255, 255, 0);
                    Character.Card card2 = Game.library.waitForClick();
                    Game.library.clearAllHighlighting();
                    if(user.hand.Contains(card1) && user.standby.Contains(card2))
                    {
                        handCards[0] = card1;
                        standbyCards[0] = card2;
                        break;
                    }
                    else if(user.hand.Contains(card2) && user.standby.Contains(card1))
                    {
                        handCards[0] = card2;
                        standbyCards[0] = card1;
                        break;
                    }
                }
                Game.library.setPromptText("Select the second pair of cards to swap.");
                while (true)
                {
                    Character.Card card1 = Game.library.waitForClick();
                    Game.library.highlight(card1, 255, 255, 0);
                    Character.Card card2 = Game.library.waitForClick();
                    Game.library.clearAllHighlighting();
                    if(user.hand.Contains(card1) && user.standby.Contains(card2))
                    {
                        handCards[1] = card1;
                        standbyCards[1] = card2;
                        break;
                    }
                    else if(user.hand.Contains(card2) && user.standby.Contains(card1))
                    {
                        handCards[1] = card2;
                        standbyCards[1] = card1;
                        break;
                    }
                }
                Game.library.setPromptText("");
            }

            public override void residual()
            {
                // TODO this isn't quite right, can rearrange up to 4
                List<Card> newOrder = new List<Card>(4);
                Game.library.setPromptText("Select your standby cards in the order you would like to arrange them (left to right).");
                for (int i = 0; i < user.standby.Count; i++)
                {
                    while (true)
                    {
                        Card card = Game.library.waitForClick();
                        if (user.standby.Contains(card) && !newOrder.Contains(card))
                        {
                            newOrder.Add(card);
                            break;
                        }
                    }
                }
                user.standby.Clear();
                foreach (Card card in newOrder)
                {
                    user.standby.Add(card);
                }
            }
        }

        private class UmbralOrder : Card
        {
            private List<Card> suspendedCards, otherCards;
            public UmbralOrder() : base()
            {
                name = "Umbral Order";
                effect = "Swap up to 2 Suspended cards with 2 cards. (Does not trigger Residual)\nResidual: Empower (2).";
                color = Color.green;
                suspendedCards = new List<Card>(2);
                otherCards = new List<Card>(2);
            }

            public override void activate()
            {
                user.swap(suspendedCards[0], otherCards[0]);
                user.swap(suspendedCards[1], otherCards[1]);
                suspendedCards.Clear();
                otherCards.Clear();
            }

            public override void declare()
            {
                while (true)
                {
                    Character.Card card1 = Game.library.waitForClickOrCancel("Select the first pair of cards to swap.");
                    if (card1 == null)
                    {
                        break;
                    }
                    Game.library.highlight(card1, 255, 255, 0);
                    Character.Card card2 = Game.library.waitForClick();
                    Game.library.clearAllHighlighting();
                    if(user.suspended.Contains(card1) && (user.standby.Contains(card2) || user.hand.Contains(card2)))
                    {
                        suspendedCards[0] = card1;
                        otherCards[0] = card2;
                        break;
                    }
                    else if(user.suspended.Contains(card2) && (user.standby.Contains(card1) || user.hand.Contains(card1)))
                    {
                        suspendedCards[0] = card2;
                        otherCards[0] = card1;
                        break;
                    }
                }
                while (true)
                {
                    Character.Card card1 = Game.library.waitForClickOrCancel("Select the second pair of cards to swap.");
                    if (card1 == null)
                    {
                        break;
                    }
                    Game.library.highlight(card1, 255, 255, 0);
                    Character.Card card2 = Game.library.waitForClick();
                    Game.library.clearAllHighlighting();
                    if(user.suspended.Contains(card1) && (user.standby.Contains(card2) || user.hand.Contains(card2)))
                    {
                        suspendedCards[1] = card1;
                        otherCards[1] = card2;
                        break;
                    }
                    else if(user.suspended.Contains(card2) && (user.standby.Contains(card1) || user.hand.Contains(card1)))
                    {
                        suspendedCards[1] = card2;
                        otherCards[1] = card1;
                        break;
                    }
                }
                Game.library.setPromptText("");
            }
        }

// Defense
        private class DarkEmbrace : Card
        {
            public DarkEmbrace() : base()
            {
                name = "Dark Embrace";
                effect = "Negate 2 damage.\nAlign (R, G): Instead, Absorb.";
                color = Color.blue;
            }

            public override void activate()
            {
                if (user.hasAlign("RG"))
                {
                    user.absorb = true;
                }
                else
                {
                    user.addNegate(2);
                }
            }
        }

        private class CaressingChaos : Card
        {
            public CaressingChaos() : base()
            {
                name = "Caressing Chaos";
                effect = "Negate 2 damage.\nBloodlust: Negate 4 additional damage.\nResidual: Gain 1 health for each of your Suspended cards.";
                color = Color.blue;
            }

            public override void activate()
            {
                user.addNegate(2);
                if (user.bloodlust)
                {
                    user.addNegate(4);
                }
            }

            public override void residual()
            {
                user.heal += user.suspended.Count - user.recentSuspended.Count;
            }
        }

        private class CalloftheEnd : Card
        {
            public CalloftheEnd() : base()
            {
                name = "Call of the End";
                effect = "Dusk: You may sacrifice 1 health to Suspend 1 of your Standby cards.\nEndure (5): You may return a Suspended card to your hand.";
                color = Color.black;
                active = false;
            }
        }

        public override void dusk()
        {
            base.dusk();
            // Call of the End effect
            foreach (Card inherent in invocation)
            {
                if (inherent.name == "Call of the End" && inherent.active)
                {
                    if (hp > 1)
                    {
                        while (true)
                        {
                            Card card = Game.library.waitForClickOrCancel("You may sacrifice 1 health to Suspend 1 of your Standby cards.");
                            if (card == null)
                            {
                                break;
                            }
                            if (standby.Contains(card))
                            {
                                hp -= 1;
                                suspend(card);
                                break;
                            }
                        }
                    }
                    if (hp <= 5)
                    {
                        while (true)
                        {
                            Card card = Game.library.waitForClickOrCancel("You may return a Suspended card to your hand.");
                            if (card == null)
                            {
                                break;
                            }
                            if (standby.Contains(card))
                            {
                                unSuspend(card);
                                break;
                            }
                        }
                    }
                    Game.library.setPromptText("");
                }
            }
        }

        private class WitnessoftheEnd : Card
        {
            public WitnessoftheEnd() : base()
            {
                name = "Witness of the End";
                effect = "Dawn: You may swap 1 of your Suspended cards with 1 of your Standby cards. (Does not trigger Residual)";
                color = Color.black;
                active = false;
            }
        }

        public override void dawn()
        {
            base.dawn();
            // Witness of the End effect
            foreach (Card card in invocation)
            {
                if (card.name == "Witness of the End" && card.active)
                {
                    while (true)
                    {
                        Character.Card card1 = Game.library.waitForClickOrCancel("You may swap 1 of your Suspended cards with 1 of your Standby cards. (Does not trigger Residual)");
                        if (card1 == null)
                        {
                            break;
                        }
                        Game.library.highlight(card1, 255, 255, 0);
                        Character.Card card2 = Game.library.waitForClick();
                        Game.library.clearAllHighlighting();
                        if(suspended.Contains(card1) && standby.Contains(card2))
                        {
                            swap(card1, card2);
                            break;
                        }
                        else if(suspended.Contains(card2) && standby.Contains(card1))
                        {
                            swap(card1, card2);
                            break;
                        }
                    }
                    Game.library.setPromptText("");
                }
            }
        }

        private class HarbingersDecree : Card
        {
            Color declared;

            public HarbingersDecree() : base()
            {
                name = "Harbinger’s Decree";
                effect = "Effect: Align (B, G, R, G): Declare a card type. Your opponent cannot play that card type for the next 2 turns.";
                color = Color.green;
                declared = Color.black;
                active = false;
                setAwakening();
            }

            public override void activate()
            {
                if (user.hasAlign("BGRG"))
                {
                    user.superSeal(declared);
                    declared = Color.black;
                }
            }

            public override void declare()
            {
                if (user.hasAlign("BGRG"))
                {
                    //TODO
                    while (true)
                    {
                        Card card = Game.library.waitForClickOrCancel("Select a card of the type you would like to seal. TEMPORARY");
                        if (card.color != Color.black)
                        {
                            declared = card.color;
                            break;
                        }
                    }
                    Game.library.setPromptText("");
                }
            }
        }

        private class BloodofKorek : Card
        {
            public BloodofKorek() : base()
            {
                name = "Blood of Korek";
                effect = "Effect: Align (R, B, R, G): Deal 2 damage. Deal additional damage equal to the difference between you and your opponent’s health totals.";
                color = Color.red;
                active = false;
                setAwakening();
            }

            public override void activate()
            {
                if (user.hasAlign("RBRG"))
                {
                    user.addDamage(2 + Math.Abs(user.hp - user.opponent.hp));
                }
            }
        }
    }
}
