using System;
using System.Collections.Generic;
using UnityEngine;

namespace Warforged
{
    public class Edros : Character
    {
        private bool bolster2;
        private bool bonusEmp;
        public Edros() : base()
        {
            name = "Edros";
            title = "Envoy of Toren";
            bolster2 = false;
            bonusEmp = false;
        }
        

        /// Deal damage to another character
        public override int dealDamage()
        {
            // Will probably need more logic in the future
            int tempdamage = damage - opponent.negate;
            if (opponent.reflect) {
                takeDamage(tempdamage);
                ((Edros)this).bonusEmp = false;
                return 0;
            }
            else if (opponent.absorb) {
                opponent.heal += tempdamage;
                ((Edros)this).bonusEmp = false;
                return 0;
            }
            else
            {
                opponent.takeDamage(tempdamage);
                int opponentDamage = opponent.damage - negate;
                if(tempdamage > 0 ||(opponentDamage > 0 && reflect))
                {
                    bolster();
                    if (((Edros)this).bonusEmp)
                    {
                        empower += 1;
                        //Debug.Log("Added Edros Emp 1");
                    }
                }

                if (((Edros)this).bolster2 && (tempdamage > 0 || (opponentDamage > 0 && reflect)))
                {
                    bolster();
                    ((Edros)this).bolster2 = false;
                }
                ((Edros)this).bonusEmp = false;
                return tempdamage;
            }
        }

        // Offense
        public class PurgingLightning : Card
        {
            public PurgingLightning() : base()
            {
                name = "Purging Lightning";
                effect = "Deal 2 damage.\nBloodlust: Deal 1 additional damage";
                color = Color.red;
            }

            public override void activate()
            {
                user.addDamage(2);
                if (user.bloodlust)
                {
                    user.damage += 1;
                }
            }
        }

        public class HandofToren : Card
        {
            public HandofToren() : base()
            {
                name = "Hand of Toren";
                effect = "Deal 1 damage.\nAlign (B, R, R): Deal 3 additional damage.";
                color = Color.red;
            }

            public override void activate()
            {
                user.addDamage(1);
                if (user.hasAlign("BRR"))
                {
                    user.damage += 3;
                }
            }
        }

        public class RollingThunder : Card
        {
            public RollingThunder() : base()
            {
                name = "Rolling Thunder";
                effect = "Chain (R): Deal 2 damage.\nStrike: Bolster";
                color = Color.red;
            }

            public override void activate()
            {
                if (user.hasChain("R"))
                {
                    user.addDamage(2);
                }
                // Strike, and thus bolster, are deided after both card effects take place
                ((Edros)user).bolster2 = true;
            }
        }

        public class PillarofLightning : Card
        {
            public PillarofLightning() : base()
            {
                name = "Suppressing Bolt";
                effect = "Deal 2 damage.\nCounter(G): Seal (B)";
                color = Color.red;
            }

            public override void activate()
            {
                user.addDamage(2);
                if (user.opponent.currCard.color == Color.green)
                {
                    user.sealCard(Color.blue);
                }
            }
        }

        public class CelestialSurge : Card
        {
            public CelestialSurge() : base()
            {
                name = "Celestial Surge";
                effect = "Deal 2 damage.\nStrike: Empower (1).";
                color = Color.red;
            }

            public override void activate()
            {
                user.addDamage(2);
                ((Edros)user).bonusEmp = true;
                Debug.Log("Edros may or may not get empower");
            }
        }

        // Defense TODO
        public class SkyBlessedShield : Card
        {
            public SkyBlessedShield() : base()
            {
                name = "Sky Blessed Shield";
                effect = "Gain 2 health.\nEndure (3): Counter (R): Reflect.";
                color = Color.blue;
            }

            public override void activate()
            {
                user.heal += 2;
                if (user.hp <= 3 && user.opponent.currCard.color == Color.red)
                {
                    user.reflect = true;
                }
            }
        }

        public class TorensFavored : Card
        {
            private bool strove = false;
            public TorensFavored() : base()
            {
                name = "Toren's Favored";
                effect = "Strive(1): Negate 3 damage.";
                color = Color.blue;
            }

            public override void activate()
            {
                if (((TorensFavored)this).strove)
                {
                    user.addNegate(3);
                }
            }

            public override void declare()
            {
                // Prompt for input
                // null should be accepted as input; that means they choose not to strive
                // store input in striveCard
                Card striveCard = Game.library.waitForClickOrCancel("Strive 1 card");
                if (user.strive(striveCard))
                {
                    ((TorensFavored)this).strove = true;
                }
            }
        }

        // Utility
        public class FaithUnquestioned : Card
        {
            private bool strove = false;
            Card offenseCard = null;
            Card standbyCard = null;
            Card defenseCard = null;
            public FaithUnquestioned() : base()
            {
                name = "Faith Unquestioned";
                effect = "Swap 1 Offense card in your hand with 1 Standby card.\nStrive(1): Send a Standby Defense card to your hand.";
                color = Color.green;
            }

            public override void activate()
            {
                user.swap(offenseCard, standbyCard);
                if (((FaithUnquestioned)this).strove)
                {
                    user.takeStandby(defenseCard);
                }
                offenseCard = null;
                standbyCard = null;
                defenseCard = null;
            }

            public override void declare()
            {
                // GUI should do checking for offense cards, or right here
                // TODO ASK FOR CARDS TO SWAP
                if (canSwap())
                {
                    Game.library.setPromptText("Swap an offense card from your hand with a standbycard");
                    while (true)
                    {
                        Character.Card card1 = Game.library.waitForClick();
                        Game.library.highlight(card1, 255, 255, 0);
                        Character.Card card2 = Game.library.waitForClick();
                        Game.library.clearAllHighlighting();
                        if (user.hand.Contains(card1) && user.standby.Contains(card2) && card1.color == Color.red)
                        {
                            offenseCard = card1;
                            standbyCard = card2;
                            break;
                        }
                        else if (user.hand.Contains(card2) && user.standby.Contains(card1) && card2.color == Color.red)
                        {
                            offenseCard = card2;
                            standbyCard = card1;
                            break;
                        }
                    }
                }
                Game.library.highlight(offenseCard,255,255,0);
                Game.library.highlight(standbyCard, 255, 255, 0);
                // ASK IF WANT TO STRIVE
                // ASK WHAT TO STRIVE
                /*
                int blueCardsInStandby = 0;
                foreach (Character.Card card in user.standby)
                {
                    if (card.color == Color.blue)
                    {
                        blueCardsInStandby += 1;
                    }
                }*/
                ((FaithUnquestioned)this).strove = false;
                bool canStrive = false;
                foreach (Character.Card c in user.standby)
                {
                    if (c.color == Color.blue && c != standbyCard)
                    {
                        hasBlueStandby = true;
                        break;
                    }
                }
                if (hasBlueStandby)
                {
                    while (true /*&& (blueCardsInStandby == 2 || standbyCard.color != Color.blue)*/)
                    {
                        Character.Card card = Game.library.waitForClickOrCancel("Choose an inherent to strive");
                        if(card == null)
                        {
                            ((FaithUnquestioned)this).strove = false;
                            break;
                        }
                        else if (user.strive(card))
                        {
                            ((FaithUnquestioned)this).strove = true;
                            break;
                        }
                    }
                }
                // ASK WHAT TO TAKE
                while (((FaithUnquestioned)this).strove && hasBlueStandby)
                {
                    Game.library.setPromptText("Choose a blue standby card to send to your hand");
                    Character.Card card = Game.library.waitForClick();
                    if(card.color == Color.blue && user.standby.Contains(card) && card != standbyCard)
                    {
                        ((FaithUnquestioned)this).defenseCard = card;
                        break;
                    }
                }
                Game.library.setPromptText("");
                Game.library.clearAllHighlighting();
            }

            private bool canSwap()
            {
                if (user.hand.Count == 0 || user.standby.Count == 0)
                {
                    return false;
                }
                foreach(Character.Card card in user.hand)
                {
                    if(card.color == Color.red)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        // Invocation TODO all of these
        public class WrathofLightning : Card
        {
            public WrathofLightning() : base()
            {
                name = "Wrath of Lightning";
                effect = "Depart: Deal damage equal to the amount of Standby Offense cards you have.";
                color = Color.black;
                active = false;
            }

            public override void depart()
            {
                int offenseCards = 0;
                foreach(Character.Card card in user.standby)
                {
                    if(card.color == Color.red)
                    {
                        offenseCards += 1;
                    }
                }
                user.damage += offenseCards; // No empower here
            }
        }
        //TODO: We might be changing the name of this card.
        public class ScornofThunder : Card
        {
            public ScornofThunder() : base()
            {
                name = "Imminent Storm";
                effect = "Depart: Return up to 2 Standby cards to your hand.";
                color = Color.black;
                active = false;
            }

            public override void depart()
            {
            Character.Card card1 = null;
            Character.Card card2 = null;
            while (true)
                {
                    if (user.standby.Count > 0)
                    {
                        card1 = Game.library.waitForClickOrCancel("Select up to 2 standby cards to send to your hand");
                        if(card1 != null && !user.standby.Contains(card1))
                        {
                            continue;
                        }
                        if(card1 == null)
                        {
                            break;
                        }
                        Game.library.highlight(card1, 0, 0, 255);
                        while (true)
                        {
                            if (user.standby.Count > 1)
                            {
                                card2 = Game.library.waitForClickOrCancel("Select up to 1 more standby card to send to your hand");
                                if (card2 != null && !user.standby.Contains(card2))
                                {
                                    continue;
                                }
                                else
                                {
                                    goto EndLoop;
                                }
                            }
                            else
                            {
                                goto EndLoop;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            EndLoop:
                Game.library.clearAllHighlighting();
                if (card1 == null)
                {
                    return;
                }
                else if(card2 == null)
                {
                    user.takeStandby(card1);
                }
                else
                {
                    user.takeStandby(card1);
                    user.takeStandby(card2);
                }

            }
        }

        public class GraceofHeaven : Card
        {
            public GraceofHeaven() : base()
            {
                name = "Grace of Heaven";
                effect = "Depart: Gain 2 health for every non-Offense Standby card.";
                color = Color.black;
                active = false;
            }

            public override void depart()
            {
                int hpToGain = 0;
                foreach(Card c in user.standby)
                {
                    if(c.color != Color.red)
                    {
                        hpToGain += 2;
                    }
                }
                user.heal += hpToGain;
            }
        }

        public class CrashingSky : Card
        {
            short strove = 0;
            public CrashingSky() : base()
            {
                name = "Crashing Sky";
                effect = "Choose 1:\nStrive (X)\nOR\nStrive (3): Deal 3 damage.";
                color = Color.red;
                setAwakening();
                active = false;
            }
            
            public override void activate()
            {
                if(strove == 3)
                {
                    user.addDamage(3);
                }
                strove = 0;
            }

            public override void declare()
            {
                // First declare
                while (true)
                {
                    Card card1 = Game.library.waitForClickOrCancel("Choose an inherent to strive.");
                    if (card1 == null)
                    {
                        break;
                    }
                    else if (user.strive(card1))
                    {
                        ((CrashingSky)this).strove += 1;
                        break;
                    }
                }

                // Second declare
                while (true)
                {
                    Card card2 = Game.library.waitForClickOrCancel("Choose an additional inherent to strive.");
                    if (card2 == null)
                    {
                        break;
                    }
                    else if (user.strive(card2))
                    {
                        ((CrashingSky)this).strove += 1;
                        break;
                    }
                }

                // Third declare
                while (true)
                {
                    Card card2 = Game.library.waitForClickOrCancel("Choose an additional inherent to strive.");
                    if (card2 == null)
                    {
                        break;
                    }
                    else if (user.strive(card2))
                    {
                        ((CrashingSky)this).strove += 1;
                        break;
                    }
                }
            }
        }
    }
}
