using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;

namespace Warforged
{
	public enum Color
	{
		red,
		green,
		blue,
		black
	}
    [XmlInclude(typeof(Tyras))]
    [XmlInclude(typeof(Edros))]
    [XmlInclude(typeof(Adrius))]
    public abstract class Character
	{
		// Overall information
		public string name{get; set;}
		public string title{get; protected set;}
		public int hp{get; protected set;}
		// Information about the current turn
        // Card color that cannot be played this turn; set one turn in advance and reset after a card is played
		protected Color seal;
        // Card color that cannot be played next turn; set two turn in advance and reset on dawn on the second turn
        // This is transitioned into regular seal on the second turn
        // i.e. Turn 0 sets seal and navySeal
        //      Turn 1 has seal in place, navySeal waiting
        //      Turn 2 dawn sees that seal is used up, sets equal to navySeal and clears navySeal
        protected Color navySeal;
		public int negate{get; set;}
		public int damage{get; set;}
		public int pierce{get; set;}
		public int heal{get; set;}
		public int empower{get; set;} // Stores how much empower for next turn
        public int currEmpower{get; set;} // How much attacks this turn are empowered by
		public int reinforce{get; set;} // Same as above
		public int currReinforce{get; set;}
		public bool reflect{get; set;}
		public bool absorb{get; set;}
		public bool undying{get; set;}
		// Information about previous turn
		public bool stalwart{get; protected set;}
		public bool bloodlust{get; protected set;}
		protected int overheal;
        public int turn = 0;
		// Information about cards
		public Card currCard{get; protected set;}
		public List<Card> standby{get; protected set;}
		public List<Card> hand{get; protected set;}
		public List<Card> invocation{get; protected set;}
        public List<Card> suspended{get; protected set;}
        public List<Card> recentSuspended{get; protected set;}
		public Card prevCard{get; protected set;}
        // Represents the opposing character
        // Should be fine this way since the game is 1v1
        [XmlIgnore]
        public Character opponent{get; protected set;}
		private List<Card> stroveCards;

		/// Set the opponent character.
		/// There doesn't need to be a way to un-set this.
		public void setOpponent(Character opponent)
		{
			this.opponent = opponent;
		}

		/// Constructor; takes no parameters and defaults all values
		public Character()
		{
			hp = 10;
			negate = 0;
			damage = 0;
            pierce = 0;
			heal = 0;
			empower = 0;
            currEmpower = 0;
			reinforce = 0;
            currReinforce = 0;
			reflect = false;
			absorb = false;
			undying = false;

			stalwart = false;
			bloodlust = false;
			overheal = 0;
			seal = Color.black;
            navySeal = Color.black;

			standby = new List<Card>();
			hand = new List<Card>();
			invocation = new List<Card>();
            suspended = new List<Card>();
            recentSuspended = new List<Card>();
			stroveCards = new List<Card>();
		}

		/// Bolster the first card that is bolster-able.
		/// Places the card in hand if it's an awakening.
		public void bolster()
		{
			for (int i = 0; i < invocation.Count; i++)
			{
				if (!invocation[i].active && !stroveCards.Contains(invocation[i]))
				{
					invocation[i].active = true;
					if (invocation[i].color != Color.black)
					{
						hand.Add(invocation[i]);
						invocation.Remove(invocation[i]);
					}
					break;
				}
			}
		}
		// Bolster effects will be taken care of in each individual function that would cause the bolster (e.g. Edros bolsters in the dealDamage method)

		/// Take damage and double check that damage numbers are within the allowed range
		/// Behaves just about exactly as expected
		public virtual void takeDamage(int dmg)
		{
			if (dmg < 0)
			{
				dmg = 0;
			}
			hp -= dmg;
			if (undying && hp < 1)
			{
				hp = 1;
			}
		}

		/// Add damage from a red card.
		/// Also adds empower and resets empower to 0.
		public void addDamage(int dmg)
		{
			damage += dmg + currEmpower;
			currEmpower = 0;
		}

		/// Add negation effects from a blue card.
		/// Also adds reinforce and resets reinforce to 0.
		public void addNegate(int ngt)
		{
			negate += ngt + currReinforce;
			currReinforce = 0;
		}

		/// These next five methods should ALWAYS be called in order.

		/// Resets all fields that relay information about the current turn.
		/// Also removes overheal from further back than last turn
		public virtual void dawn()
		{
			stroveCards = new List<Card>();
            recentSuspended.Clear();
			negate = 0;
			damage = 0;
            currEmpower = empower;
            empower = 0;
            currReinforce = reinforce;
            reinforce = 0;
            pierce = 0;
			heal = 0;
			reflect = false;
			absorb = false;
			if (hp > 10)
			{
				hp -= Math.Min(hp-10, overheal);
			}
			overheal = 0;
			if (hp > 10)
			{
				overheal = hp - 10;
			}
            if (navySeal != Color.black && seal == Color.black)
            {
                seal = navySeal;
                navySeal = Color.black;
            }
		}

		/// Play a card from your hand
		/// If your hand does not contain the card, the method returns false
		/// Returns true otherwise
		public bool playCard()
		{
			if(hand.Count == 0)
			{
				currCard = null;
				return false;
			}
			while (true)
			{
                Card card = Game.library.waitForClick();
                //Card card = hand[0];

                if (!hand.Contains(card))
				{
					continue;
				}
				if (card.color == seal)
				{
					continue;
				}
				currCard = card;
				hand.Remove(card);
				return true;
			}
		}

		/// After both players have played their cards, activate this method.
		/// It activates the effects of the cards (i.e. buffing the characters)
		/// then calculates damages and healing.
		public virtual void declarePhase()
		{
			seal = Color.black; // Reset any seal from last turn
			// Declarations should happen BEFORE activateCard(), since activateCard()reads current information and declarations should happen before card calculations.
			currCard.declare();
			activateCard();
		}

		/// Calculate all damage and healing occuring this turn.
		public virtual void damagePhase()
		{
			dealDamage();
			healSelf();
			prevCard = currCard;
			rotate();
		}

		/// Calculates if the character dealt or negated damage this turn
		/// Probably will be overwritten a ton
		/// e.g. If a card effect happens at dusk, then make a new field in the child class
		/// to represent if that effect is happening, then make it happen
		public virtual void dusk()
		{
			// TODO will need changing based on stuff
			// Can't think of any examples, but I know they exist.
			bloodlust = (damage > opponent.negate) ? true : false;
			stalwart = (opponent.damage > 0 && negate > 0) ? true : false;
		}

		/// Strive an active inherent.
		/// The parameter can be null. It will fail the second check.
		/// returns true if something was strived
		/// returns false otherwise
		public virtual bool strive(Card card)
		{
			if (invocation.Count == 0)
			{
				return false;
			}
			if (!invocation.Contains(card))
			{
				return false;
			}
			if (!card.active)
			{
				return false;
			}
			card.active = false;
			card.depart();
			stroveCards.Add(card);
			return true;
		}

        /// Suspend a card.
        /// Returns true if the card was successfully suspended.
        /// Only allows suspensions from hand or standby row.
        /// Card can be null, will return false.
        public virtual bool suspend(Card card)
        {
            if (suspended.Contains(card))
            {
                return false;
            }
            if (card == null)
            {
                return false;
            }
            if (hand.Contains(card))
            {
                suspended.Add(card);
                hand.Remove(card);
                card.residual();
                return true;
            }
            if (standby.Contains(card))
            {
                suspended.Add(card);
                standby.Remove(card);
                card.residual();
                return true;
            }
            return false;
        }

        /// Take a card back from the suspended area.
        /// Returns true if the card was successfully taken back.
        /// Argument can be null
        public virtual bool unSuspend(Card card)
        {
            if (!suspended.Contains(card))
            {
                return false;
            }
            hand.Add(card);
            suspended.Remove(card);
            card.recall();
            return true;
        }

		/// Definitely doesn't need to be its own function
		public virtual void healSelf()
		{
			hp += heal;
		}

		/// Deal damage to another character
		/// This happens AFTER the cards are played and effects activated.
		/// The character has the stats, now they strike.
        /// Returns the amount of damage dealt.
		public virtual int dealDamage()
		{
			// Will probably need more logic in the future
            int tempnegate = opponent.negate - pierce;
            if (tempnegate < 0)
            {
                tempnegate = 0;
            }
			int tempdamage = damage - tempnegate;
			if (opponent.reflect)
			{
				takeDamage(tempdamage);
                return 0;
			}
			else if (opponent.absorb)
			{
				opponent.heal += tempdamage;
                return 0;
			}
			else
			{
				opponent.takeDamage(tempdamage);
                return tempdamage;
			}
		}

		/// Pretty simple; activates the current card
		/// Definitely doesn't need to be its own function
		public void activateCard()
		{
			currCard.activate();
		}

		/// Rotate the cards
		/// Active goes to left end of standby
		/// Right end of standby goes to hand
		public void rotate()
		{
            if (currCard == null)
            {
                return;
            }
			if (currCard.isAwakening)
			{
				currCard.active = false;
				invocation.Add(currCard);
				currCard = null;
				return;
			}
			standby.Insert(0, currCard);
			currCard = null;
			if (standby.Count > 4) // Rotate if there are more than 4 cards
			{
				hand.Add(standby[standby.Count-1]);
				standby.Remove(hand[hand.Count-1]);
			}
		}

		/// Tells whether or not a player has an align on their standby
		/// align is formatted without any spaces, just letters
		/// e.g. "RBB"
		/// align MUST be at least 2 characters
		/// index should never be specified outside this function.
		public bool hasAlign(string align, int index=0)
		{
			Color color;
			if (align[0] == 'B')
			{
				color = Color.blue;
			}
			else if (align[0] == 'R')
			{
				color = Color.red;
			}
			else if (align[0] == 'G')
			{
				color = Color.green;
			}
			else // Should never happen
			{
				color = Color.black;
			}

			if (index == 0)
			{
				// Safety check
				if (align.Length < 2)
				{
					return false;
				}
				bool found = false;
				for (int i = 0; i < standby.Count-1; i++)
				{
					if (standby[i].color == color)
					{
						found = hasAlign(align.Substring(1), i+1);
					}
					if (found)
					{
						return true;
					}
				}
				return false;
			}
			// Stop recursion if the align breaks
			if (standby[index].color != color)
			{
				return false;
			}
			// Also stop if we're at the last character
			if (standby[index].color == color && align.Length == 1)
			{
				return true;
			}
			// If we're good on the current character, go to the next one
			return (hasAlign(align.Substring(1), index+1));
		}


		/// Seals a certain card type for the opponent next turn
		public void sealColor(Color color)
		{
			opponent.seal = color;
		}

        /// Seals a certain card type for the opponent's next two turns
        public void superSeal(Color color)
        {
            opponent.seal = color;
            opponent.navySeal = color;
        }

		/// Swap two cards.
        /// These cards can be anywhere on the field.
		/// If either of the cards are not found, nothing happens.
		/// Either card can be null.
        /// This does NOT trigger effects based on moving cards.
        /// e.g. Residuals or recalls will not occur.
		public void swap(Card card1, Card card2)
		{
			if (hand.Contains(card1) && standby.Contains(card2))
			{
                standby.Insert(standby.IndexOf(card2), card1);
                hand.Add(card2);
                standby.Remove(card2);
                hand.Remove(card1);
			}
            else if (standby.Contains(card1) && hand.Contains(card2))
            {
                standby.Insert(standby.IndexOf(card1), card2);
                hand.Add(card1);
                standby.Remove(card1);
                hand.Remove(card2);
            }
            else if (hand.Contains(card1) && suspended.Contains(card2))
            {
                hand.Add(card2);
                suspended.Add(card1);
                hand.Remove(card1);
                suspended.Remove((card2));
            }
            else if (suspended.Contains(card1) && hand.Contains(card2))
            {
                hand.Add(card1);
                suspended.Add(card2);
                hand.Remove(card2);
                suspended.Remove((card1));
            }
            else if (standby.Contains(card1) && suspended.Contains(card2))
            {
                standby.Insert(standby.IndexOf(card1), card2);
                suspended.Add(card1);
                standby.Remove(card1);
                suspended.Remove(card2);
            }
            else if (suspended.Contains(card1) && standby.Contains(card2))
			{
                standby.Insert(standby.IndexOf(card2), card1);
                suspended.Add(card2);
                standby.Remove(card2);
                suspended.Remove(card1);
			}
		}

		/// Send a standby card to your hand
		/// card can be null, because then the check fails
		public void takeStandby(Card card)
		{
			if (!standby.Contains(card))
			{
				return;
			}
			hand.Add(card);
			standby.Remove(card);
		}


        /* Nested class representing a generic card */
        [XmlInclude(typeof(Adrius.Ascendance))]
        [XmlInclude(typeof(Adrius.DivineCataclysm))]
        [XmlInclude(typeof(Adrius.EarthPiercer))]
        [XmlInclude(typeof(Adrius.EmeraldCore))]
        [XmlInclude(typeof(Adrius.FistofRuin))]
        [XmlInclude(typeof(Adrius.HerosResolution))]
        [XmlInclude(typeof(Adrius.RubyHeart))]
        [XmlInclude(typeof(Adrius.SapphireMantle))]
        [XmlInclude(typeof(Adrius.ShatteringBlow))]
        [XmlInclude(typeof(Adrius.SurgingHope))]
        [XmlInclude(typeof(Adrius.TremoringImpact))]
        [XmlInclude(typeof(Adrius.UnyieldingFaith))]
        [XmlInclude(typeof(Adrius.WillUnbreakable))]

        [XmlInclude(typeof(Tyras.ABrothersVirtue))]
        [XmlInclude(typeof(Tyras.AnOathUnforgotten))]
        [XmlInclude(typeof(Tyras.APromiseUnbroken))]
        [XmlInclude(typeof(Tyras.ArmorofAldras))]
        [XmlInclude(typeof(Tyras.ASoldiersRemorse))]
        [XmlInclude(typeof(Tyras.DecryingRoar))]
        [XmlInclude(typeof(Tyras.GrimKnightsDread))]
        [XmlInclude(typeof(Tyras.IntheKingsWake))]
        [XmlInclude(typeof(Tyras.OnraisStrike))]
        [XmlInclude(typeof(Tyras.OnslaughtofTyras))]
        [XmlInclude(typeof(Tyras.SunderingStar))]
        [XmlInclude(typeof(Tyras.WarriorsResolve))]

        [XmlInclude(typeof(Edros.CelestialSurge))]
        [XmlInclude(typeof(Edros.CrashingSky))]
        [XmlInclude(typeof(Edros.FaithUnquestioned))]
        [XmlInclude(typeof(Edros.GraceofHeaven))]
        [XmlInclude(typeof(Edros.HandofToren))]
        [XmlInclude(typeof(Edros.PillarofLightning))]
        [XmlInclude(typeof(Edros.PurgingLightning))]
        [XmlInclude(typeof(Edros.RollingThunder))]
        [XmlInclude(typeof(Edros.ScornofThunder))]
        [XmlInclude(typeof(Edros.SkyBlessedShield))]
        [XmlInclude(typeof(Edros.TorensFavored))]
        [XmlInclude(typeof(Edros.WrathofLightning))]
        public abstract class Card
		{
			public string name{get; protected set;}
			public string effect{get; protected set;}
			public Color color{get; protected set;}
			public bool active = true;
			public bool isAwakening{get; private set;}
            // Nothing other than cards need this reference,
            // since cards are only called through the reference in the first place
            [XmlIgnore]
            protected Character user;

			//Used in the UI. This is the image assosiated with the card.

			//A variable used to easily get the current directory of card images


			protected Card()
			{
				//This was moved to here so the model becomes compatible with an older version of
				//C# Which is supported by Unity.
				isAwakening = false;
			}

			/// Flags a card as an awakening card.
			/// You'll notice that this is a one-time thing.
			/// That's because there's no reason to ever change this.
			protected void setAwakening()
			{
				isAwakening = true;
			}

			/// Both players should have locked in cards when this happens
			public virtual void activate() { }

			/// Happens when an Inherent is deactivated
			public virtual void depart() { }

			/// Happens when a card is suspended
			public virtual void residual() { }

			/// Covers anything that the card needs to do before effects happen
			/// i.e. Declaration effects (including swaps, strive)
			/// This happens AFTER both cards are set
			/// I think this might be redudant, since it happens
			/// immediately before activate(), with nothing in between
			public virtual void declare() { }

			/// Happens when a card is removed from suspention
			public virtual void recall() { }
            /// <summary>
            /// This is inplace of our constructor, so card objects can be serialized
            /// </summary>
            /// <param name="user"></param>
            public void init(Character user)
            {
                this.user = user;
            }
		}
	}
}

