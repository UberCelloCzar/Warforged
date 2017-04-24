using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Warforged
{
	public abstract class WindowLibrary
	{
		protected object returnObject = null;

		//WindowLibrary provides shorthand utility which allows for the model to interact with the UI.



		//Tell the UI that a player is going to be Edros. You only need to do this once per game.
		//@param isP2: If this value is 2, the UI is notified that P2 is Edros, otherwise P1 will be Edros
		//This function must be called twice, once for each player.
		public abstract void setupEdros(int player);
		public abstract void setupTyras(int player);
		public abstract void setupAdrius(int player);
		public abstract void setupAurian(int player);
        public abstract void setupSeals();

        //Updates the UI for Player 1 based on the character object passed in.
        //@param showCurrCard: If this is true the played card will be shown on the UI, otherwise the back of the card is used.
        public abstract void updateUI(Character ch, bool showCurrCard);
        public abstract void resetLock();
        public abstract void resetDmgUI();
        public abstract void resetIcons();
        public abstract void setDmgUI(bool isPlayer1);
        public abstract void setSafeguardPromptUI(bool isPlayer1);
        public abstract void setReflectPromptUI(bool isPlayer1);
        public abstract void setNegatePromptUI(bool isPlayer1, int ngt);
        public abstract void setAbsorbPromptUI(bool isPlayer1);
        public abstract void resetPrompts();
        public abstract void resetHealingUI();
        public abstract void setHealingUI(bool isPlayer1);
        public abstract void LockIn(bool isServer);
        public abstract void endSlate(Character ch); // End game UI method

        //Updates the Network, so that the opponent recieves our changes
        public abstract void updateNetwork(Character ch1, Character ch2, bool UpdateVars);
        //Waits for both players to have cards out on the field
        public abstract void waitOnNetwork(ref Character ch1, ref Character ch2);
        //Updates the UI for Player 2 based on the character object passed in.
        //@param showCurrCard: If this is true the played card will be shown on the UI, otherwise the back of the card is used.
        //@param showHand: If this is true the Opponent's hand will be shown to the user, otherwise the hand will show the back of the cards.
        public abstract void updateOpponentUI(Character ch, bool showCurrCard, bool showHand);

		public abstract void setPromptText(string text);

		//Provide a yes/no prompt to the user with the given text.
		public abstract bool yesnoPrompt(string text);

		//Provide a multitude of options to the user
		//@param buttonTexts: 
		public abstract object multiPrompt(string text, List<string> buttonTexts, List<object> returnTypes);

		public abstract Character.Card waitForClick();
        public abstract Character.Card waitForClickorLock();

        public abstract Character.Card waitForClickOrCancel(string text);

		//These highlighting functions highlight a given card with a given color.
		//They may also clear highlighting from a card.
		public abstract void highlight(Character.Card card, byte r, byte g, byte b);
		public abstract void clearHighlight(Character.Card card);
		public abstract void clearAllHighlighting();

		//Used in the UI to set a returning value to this Library.
		//You probably should not use this unless you are coding the UI.
		public void setReturnObject(object o)
		{
			returnObject = o;
		}
	}
}
