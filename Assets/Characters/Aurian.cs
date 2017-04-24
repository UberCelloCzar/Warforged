using System;
using System.Collections.Generic;
using UnityEngine;

namespace Warforged
{
    public class Aurian : Character
    {

        private bool waitingForStrike;
        private bool waitingForGuard;

        public Aurian() : base()
        {
            name = "Aurian";
            title = "the Hand of Era";
            waitingForStrike = false;
			waitingForGuard = false;
        }

        public override void takeDamage(int dmg)
        {
            base.takeDamage(dmg);
			if (isPlayer1 && negate > opponent.pierce && opponent.damage > 0 && waitingForGuard && standby.Count > 0)
            {
                Card cardToTake;
                while (true)
                {
                    Game.library.setPromptText("Choose a Standby card to send to your hand.");
                    cardToTake = Game.library.waitForClick();
                    if (standby.Contains(cardToTake))
                    {
                        Game.library.setPromptText("");
                        break;
                    }
                }
                takeStandby(cardToTake);
			}
			waitingForGuard = false;
        }


        public override int dealDamage()
        {
			int damageDealt = base.dealDamage();
            if (damageDealt > 0) // This deals damage and checks that it is >0
            {
                if (waitingForStrike && standby.Count > 0 && hasStandbyColor(Color.red))
                {
                    Card cardToTake;
                    while (true)
                    {
                        Game.library.setPromptText("Choose a Standby Offense card to send to your hand.");
                        cardToTake = Game.library.waitForClick();
                        if (standby.Contains(cardToTake))
                        {
                            // This is nested as a safety check
                            if (cardToTake.color == Color.red)
                            {
                                Game.library.setPromptText("");
                                break;
                            }
                        }
                    }
                    takeStandby(cardToTake);
                }
			}
			waitingForStrike = false;
			return damageDealt;
        }

		public override bool hasChain(string chain)
		{
			bool returnValue = base.hasChain(chain);
			if (returnValue)
			{
				bolster();
			}
			return returnValue;
		}

        public class OpeningBlow : Card
        {
            public OpeningBlow() : base()
            {
                name = "Opening Blow";
                effect = "Deal 2 damage.\nStrike: Send a Standby Offense card to your hand.";
                color = Color.red;
            }

            public override void activate()
            {
                user.addDamage(2);
                ((Aurian)user).waitingForStrike = true;
            }
        }

        public class RelentlessAssault : Card
        {
            public RelentlessAssault() : base()
            {
                name = "Relentless Assault";
                effect = "Deal 1 damage.\nChain (R, R): Bloodlust: Deal 3 additional damage.";
                color = Color.red;
            }

            public override void activate()
            {
                user.addDamage(1);
                if (user.hasChain("RR") && user.bloodlust)
                {
                    user.addDamage(3);
                }
            }
        }

        public class SoulStrike : Card
        {
            public SoulStrike() : base()
            {
                name = "Soul Strike";
                effect = "Deal 2 damage.\nChain (G, B): Lifesteal.";
                color = Color.red;
            }

            public override void activate()
            {
                user.addDamage(2);
                if (user.hasChain("GB"))
                {
                    user.lifesteal = true;
                }
            }
        }

        public class PrimedAttack : Card
        {
            public PrimedAttack() : base()
            {
                name = "Primed Attack";
                effect = "Deal 1 damage.\nChain (G): Empower (2).";
                color = Color.red;
            }

            public override void activate()
            {
                user.addDamage(1);
                if (user.hasChain("G")) {
                    user.empower += 2;
                }
            }
        }

// Intent
        public class ClarityofMind : Card
        {
            Card cardToTake = null;

            public ClarityofMind() : base()
            {
                name = "Clarity of Mind";
                effect = "Send your Leftmost or Rightmost card to your hand.\nChain (R, R): Bolster.";
                color = Color.green;
            }

            public override void activate()
            {
                user.takeStandby(cardToTake);
                if (user.hasChain("RR"))
                {
                    user.bolster();
                }
            }

            public override void declare()
            {
                if (user.standby.Count > 0)
                {
                    while (true)
                    {
                        Game.library.setPromptText("Choose your leftmost OR rightmost standby card to send to your hand.");
                        cardToTake = Game.library.waitForClick();
                        if (cardToTake.Equals(user.standby[0]) || cardToTake.Equals(user.standby[user.standby.Count-1]))
                        {
                            Game.library.setPromptText("");
                            break;
                        }
                    }
                }
            }
        }

// Defense
        public class CounteringStrike : Card
        {
            public CounteringStrike() : base()
            {
                name = "Countering Strike";
                effect = "Negate 2 damage.\nChain (R, R): Instead, Reflect.";
                color = Color.blue;
            }

            public override void activate()
            {
                if (user.hasChain("RR"))
                {
                    user.reflect = true;
                }
                else
                {
                    user.addNegate(2);
                }
            }
        }

        public class BodyandSoul : Card
        {
            public BodyandSoul() : base()
            {
                name = "Body and Soul";
                effect = "Negate 2 damage.\nRally: Gain 1 health.\nChain (G): Empower (1).";
                color = Color.blue;
            }

            public override void activate()
            {
                user.addNegate(2);
                if (user.hasRally())
                {
                    user.heal += 1;
                }
                if (user.hasChain("G"))
                {
                    user.empower += 1;
                }
            }
        }

        public class EvadingStep : Card
        {
            public EvadingStep() : base()
            {
                name = "Evading Step";
                effect = "Negate 2 damage.\nGuard: Send a Standby card to your hand.";
                color = Color.blue;
            }

            public override void activate()
            {
                user.addNegate(2);
				((Aurian)user).waitingForGuard = true;
            }
        }

        public class EtherealStrike : Card
        {
            public EtherealStrike() : base()
            {
                name = "Ethereal Strike";
                effect = "Deal 2 damage.\nPierce (2).";
				color = Color.red;
				setAwakening();
				active = false;
            }

            public override void activate()
            {
                user.addDamage(2);
                user.pierce += 2;
            }
        }

        public class WrathofEra : Card
        {
            public WrathofEra() : base()
            {
                name = "Wrath of Era";
                effect = "Chain (G, R, R): Deal 2 damage for every Standby Offense card.";
				color = Color.red;
				setAwakening();
				active = false;
            }

            public override void activate()
            {
                if (user.hasChain("GRR"))
                {
                    foreach (Card card in user.standby)
                    {
                        if (card.color == Color.red)
                        {
                            user.addDamage(2);
                        }
                    }
                }
            }
        }

        public class AbsoluteFocus : Card
        {
            public AbsoluteFocus() : base()
            {
                name = "Absolute Focus";
                effect = "Chain (G, B): Empower (3).";
				color = Color.green;
				setAwakening();
				active = false;
            }

            public override void activate()
            {
                if (user.hasChain("GB"))
                {
                    user.empower += 3;
                }
            }
        }

        public class StrengthofSpirit : Card
        {
            public StrengthofSpirit() : base()
            {
                name = "Strength of Spirit";
                effect = "Chain (B, R): Gain 2 health. Absorb.";
				color = Color.blue;
				setAwakening();
				active = false;
            }

            public override void activate()
            {
                if (user.hasChain("BR"))
                {
                    user.heal += 2;
                    user.absorb = true;
                }
            }
        }
    }
}
