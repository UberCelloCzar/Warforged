using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

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
	[XmlInclude(typeof(Aurian))]
    public abstract class Character
	{
		// Overall information
		public string name{get; set;}
		public string title{get; protected set;}
		public int hp{get; protected set; }
        public bool isPlayer1;
        // Information about the current turn
        // Card color that cannot be played this turn; set one turn in advance and reset after a card is played
        public Color seal;
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
        public bool lifesteal{get; set;}
		public bool reflect{get; set;}
		public bool absorb{get; set;}
		public bool undying{get; set;}
		// Information about previous turn
		public bool stalwart{get; protected set;}
		public bool bloodlust{get; protected set;}
		protected int overheal;
        public int turn = 0;
        public bool lockedIn = false;
		// Information about cards
		public Card currCard{get; protected set;}
		public List<Card> standby{get; protected set;}
		public List<Card> hand{get; protected set;}
		public List<Card> invocation{get; protected set;}
        public List<Card> suspended{get; protected set;}
        public List<Card> recentSuspended{get; protected set;}
        // Keeps track of the last four cards played (for aligns and such)
        // The most recent card played is at the end of the list
		public List<Card> prevCards{get; protected set;}
        // Represents the opposing character
        // Should be fine this way since the game is 1v1
        [XmlIgnore]
        public Character opponent{get; protected set;}
		private List<Card> stroveCards;
        public int endGame {get; set;} // True will trigger end game slate and indicates winning player (no need for new scene, just an overlay)
        // Stores the current phase information to track what phase the game is currently on
        public enum Phase
        {
            Dawn,
            Selection,
            WaitingSelection,
            Declare,
            WaitingDeclare,
            Damage,
            Dusk,
            Leave
        }
        public Phase phase {get; set;}

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
            lifesteal = false;

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
            prevCards = new List<Card>(5);
            endGame = 0;
            phase = Phase.Dawn;
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
        // TODO change this so that negate is calculated here
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
            //Debug.Log("I'm opponent, taking damage: " + hp);
            //Debug.Log(name + " is at " + hp);
            if (hp <= 0) // When the player has died, set endgame to be resolved at turn end (See Game.cs)
            {
                //Debug.Log("I lose");
                hp = 0;
                //Debug.Log("Endgame triggered");
                endGame = 2;
                opponent.endGame = 1;
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
            currReinforce = reinforce;
            reinforce = 0;
            currEmpower = empower;
            empower = 0;
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

            if (navySeal != Color.black && seal == Color.black) // Takes the 2nd turn seal and applies it to this turn then gets rid of the 2nd turn seal (Super Seal)
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
            lockedIn = false;
            if (hand.Count == 0)
			{
				currCard = null;
                seal = Color.black; // Reset any seal from last turn
                return false;
			}
			while (true)
            {
                Card card = Game.library.waitForClickorLock();
                if (lockedIn)
                {
                    //Debug.Log("Leaving loop");
                    Game.library.LockIn(OnClick.controller.localPlayer.isServer);
                    return true;
                }
                //Card card = hand[0];
                //Debug.Log("got " + card.name);
                if (card == null) // This might happen if somehow an edge case slips through the lock in
                {
                    //Debug.Log("Card was null");
                    continue;
                }
                if (!hand.Contains(card))
				{
                    //Debug.Log("Card not in hand");
					continue;
				}
				if (card.color == seal)
				{
                    //Debug.Log("Sealed: " + seal);
					continue;
				}
                if (currCard == null)
                {
                    //Debug.Log("Added first card");
                    currCard = card;
                    hand.Remove(card);
                }
                else
                {
                    //Debug.Log("Swapping cards");
                    hand.Add(currCard);
                    currCard = card;
                    hand.Remove(card);
                }
                seal = Color.black; // Reset any seal from last turn
                Game.library.updateUI(Game.p1, false); // Display the new card that's down
            }
		}

		/// After both players have played their cards, activate this method.
		/// It activates the effects of the cards (i.e. buffing the characters)
		/// then calculates damages and healing.
		public virtual void declarePhase()
		{
			// Declarations should happen BEFORE activateCard(), since activateCard()reads current information and declarations should happen before card calculations.
			currCard.declare();
            Game.library.setPromptText("");
            activateCard();
        }

		/// Calculate all damage and healing occuring this turn.
		public virtual void damagePhase()
        {
           if(reflect == true) // tell you're reflecting
            {
                Game.library.setReflectPromptUI(isPlayer1);
            }
            if(negate > 0 && negate < 100) // tell you're negating
            {
                Game.library.setNegatePromptUI(isPlayer1, negate);
            }
            if(negate> 100) // tell you're safeguarding
            {
                Game.library.setSafeguardPromptUI(isPlayer1);
            }
            if(absorb == true) // tell you're absorbing
            {
                Game.library.setAbsorbPromptUI(isPlayer1);
            }
            //Debug.Log(name + " Added " + currEmpower + " to dmg");
            healSelf(heal);
            if (heal > 0)
            {
                Game.library.setHealingUI(isPlayer1);
            }
            
            dealDamage();
			
            // Keeps track of the last four cards played
			prevCards.Add(currCard);
            if (prevCards.Count > 4)
            {
                // Remove the first card in the list if it's getting too long
                prevCards.RemoveAt(0);
            }
            
            //Debug.Log(name + " Empower reset, curr: " + currEmpower);
			lifesteal = false;
        }

		/// Calculates if the character dealt or negated damage this turn
		/// Probably will be overwritten a ton
		/// e.g. If a card effect happens at dusk, then make a new field in the child class
		/// to represent if that effect is happening, then make it happen
		public virtual void dusk()
        {
            Game.library.resetPrompts();
            rotate();
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
		public virtual void healSelf(int toHeal)
		{
			hp += toHeal;
		}

		public virtual void healSelf()
		{
			healSelf(this.heal);
		}

		/// Deal damage to another character
		/// This happens AFTER the cards are played and effects activated.
		/// The character has the stats, now they strike.
        /// Returns the amount of damage dealt.
		public virtual int dealDamage()
		{
			// Will probably need more logic in the future
            // TODO change this so that negate is calculated by takeDamage
            //      have it take pierce as a parameter or something
            int tempnegate = opponent.negate - pierce;
            if (tempnegate < 0)
            {
                tempnegate = 0;
            }
			int tempdamage = damage - tempnegate;
			if (opponent.reflect)
			{
				takeDamage(tempdamage);
                if (tempdamage > 0)
                {
                    //Debug.Log("Tyras is player 1 " + isPlayer1 + " and is turning on opponents dmg");
                    Game.library.setDmgUI(isPlayer1); // Turn on your own dmg icon
                }
                return 0;
			}
			else if (opponent.absorb)
			{
				opponent.healSelf(tempdamage);
                return 0;
			}
			else
			{
				opponent.takeDamage(tempdamage);
                if (tempdamage > 0)
                {
                    //Debug.Log("Tyras is player 1 " + isPlayer1 + " and is turning on opponents dmg.");
                    Game.library.setDmgUI(!isPlayer1); // Turn on opponent's dmg icon
                }
                if (lifesteal)
                {
                    //Debug.Log("Stealing is wrong");
                    healSelf(tempdamage);
                    Game.library.setHealingUI(isPlayer1);
                }
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

        /// Tells if the user has a chain active.
        /// chain is formatted without any spaces, just letters
        /// e.g. "GB"
        public virtual bool hasChain(string chain)
        {
            string currChain = "";
			// Safety check; make sure we're far enough into the game to have chains
			if (prevCards.Count >= chain.Length)
			{
				// Loop through the history to see what chain we DO have
				for (int i = 0; i < prevCards.Count; i++)
				{
					switch (prevCards[i].color)
					{
					case (Color.red):
						currChain += "R";
						break;
					case (Color.green):
						currChain += "G";
						break;
					case (Color.blue):
						currChain += "B";
						break;
					}
				}
				return currChain.EndsWith(chain);
			}
			else
			{
				return false;
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
                    Debug.Log("Align not long enough");
					return false;
				}
				bool found = false;
				for (int i = 0; i < standby.Count-1; i++)
				{
					if (standby[i].color == color)
					{
                        Debug.Log("Found card at position " + i);
                        if (i+1 > standby.Count-1) // If the align would continue, but there are no more cards in the standby row with which to continue it
                        {
                            return false;
                        }
						found = hasAlign(align.Substring(1), i+1);
					}
					if (found)
					{
                        Debug.Log("Align is good");
						return true;
					}
				}
				return false;
			}
			// Stop recursion if the align breaks
			if (standby[index].color != color)
			{
                Debug.Log("Align Broken");
				return false;
			}
			// Also stop if we're at the last character
			if (standby[index].color == color && align.Length == 1)
			{
                //Debug.Log("Align is good");
				return true;
			}

            // If we're good on the current character, go to the next one, unless there's not one, then return false
            if (index + 1 > standby.Count - 1) // If the align would continue, but there are no more cards in the standby row with which to continue it
            {
                return false;
            }
            else
            {
                return (hasAlign(align.Substring(1), index + 1));
            }
		}

        /// Tells if the character has a certain color in his/her standby row
        public bool hasStandbyColor(Color color)
        {
            foreach (Card card in standby)
            {
                if (card.color == color)
                {
                    return true;
                }
            }
            return false;
        }



		/// Seals a certain card type for the opponent next turn
		public void sealCard(Color color)
		{
            opponent.seal = color;
            //Debug.Log("Sealing Opponent: " + opponent.seal);
        }

        /// Seals a certain card type for the opponent's next two turns
        public void superSeal(Color color) // Super Seal doesn't work unless we add it to the network stuff
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

        // Method to switch to the next phase.
        public void nextPhase()
        {
            if (phase.Equals(Phase.Selection))
            {
                phase = Phase.WaitingSelection;
            }
            else if (phase.Equals(Phase.WaitingSelection))
            {
                phase = Phase.Declare;
            }
            else if (phase.Equals(Phase.Declare))
            {
                phase = Phase.WaitingDeclare;
            }
            else if (phase.Equals(Phase.WaitingDeclare))
            {
                phase = Phase.Damage;
            }
            else if (phase.Equals(Phase.Damage))
            {
                phase = Phase.Dusk;
            }
            else if (phase.Equals(Phase.Dusk))
            {
                phase = Phase.Dawn;
            }
            else if (phase.Equals(Phase.Dawn))
            {
                phase = Phase.Selection;
            }
            else
            {
                phase = Phase.Leave;
            }
        }

        // Phase display UI method
        public string displayPhase()
        {
            if (phase.Equals(Phase.Selection))
            {
                return phase + " Phase: Please select a card to play.";
            }
            else if (phase.Equals(Phase.WaitingSelection))
            {
                return "Waiting Phase: Waiting for other player to select a card.";
            }
            else if (phase.Equals(Phase.Declare))
            {
                return phase + " Phase: Please choose card effects.";
            }
            else if(phase.Equals(Phase.WaitingDeclare))
            {
                return "Waiting: Phase: Waiting for other player to resolve card effects.";
            }
            else if (phase.Equals(Phase.Damage))
            {
                return phase + " Phase: Dealing damage.";
            }
            else if (phase.Equals(Phase.Dusk))
            {
                return phase + " Phase: Activating dusk effects.";
            }
            else if (phase.Equals(Phase.Dawn))
            {
                return phase + " Phase: Activating dawn effects.";
            }
            else
            {
                return "Game Message: Your opponent has left the game.";
            }
        }

        /// Tells whether or not this character can use Rally effects
        /// i.e. If this character lower health than their opponent?
        public bool hasRally()
        {
            return hp < opponent.hp;
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
        [XmlInclude(typeof(Edros.ImminentStorm))]
        [XmlInclude(typeof(Edros.SkyBlessedShield))]
        [XmlInclude(typeof(Edros.TorensFavored))]
        [XmlInclude(typeof(Edros.WrathofLightning))]

		[XmlInclude(typeof(Aurian.AbsoluteFocus))]
		[XmlInclude(typeof(Aurian.BodyandSoul))]
		[XmlInclude(typeof(Aurian.ClarityofMind))]
		[XmlInclude(typeof(Aurian.CounteringStrike))]
		[XmlInclude(typeof(Aurian.EtherealStrike))]
		[XmlInclude(typeof(Aurian.EvadingStep))]
		[XmlInclude(typeof(Aurian.OpeningBlow))]
		[XmlInclude(typeof(Aurian.PrimedAttack))]
		[XmlInclude(typeof(Aurian.RelentlessAssault))]
		[XmlInclude(typeof(Aurian.SoulStrike))]
		[XmlInclude(typeof(Aurian.StrengthofSpirit))]
		[XmlInclude(typeof(Aurian.WrathofEra))]
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

            public bool Equals(Card other) // Method for card comparison
            {
                return this.name == other.name;
            }
        }
	}
}

