using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Warforged
{
	public class UnityLibrary : WindowLibrary
	{
		public Barrier barrier = new Barrier(2);
		private int threadID = 0;

		public UnityLibrary():base()
		{
			threadID = barrier.AddThread();
		}


		public override void clearAllHighlighting()
		{
			//throw new NotImplementedException();
		}

		public override void clearHighlight(Character.Card card)
		{
			//throw new NotImplementedException();
		}

		public override void highlight(Character.Card card, byte r, byte g, byte b)
		{
			//throw new NotImplementedException();
		}

		public override object multiPrompt(string text, List<string> buttonTexts, List<object> returnTypes)
		{
			OnClick.buttonReturn = OnClick.NoReturn;
			StartGame.signal = () => { return StartGame.multiPrompt(text,buttonTexts,returnTypes); };
			barrier.SignalAndWait(threadID);
			setPromptText("");
			return returnObject;

        }

		public override void setPromptText(string text)
		{
			OnClick.buttonReturn = OnClick.NoReturn;
			StartGame.signal = () => { return StartGame.setPromptText(text); };
			barrier.SignalAndWait(threadID);
		}

		public override void setupEdros(int player)
		{
			if(player == 2)
			{
				StartGame.signal = () => { return StartGame.setupEdros(OnClick.OCardImages); };
				barrier.SignalAndWait(threadID);
			}
			else
			{
				StartGame.signal = () => { return StartGame.setupEdros(OnClick.CardImages); };
				barrier.SignalAndWait(threadID);
			}
		}

		public override void setupTyras(int player)
		{
			if (player == 2)
			{
				StartGame.signal = () => { return StartGame.setupTyras(OnClick.OCardImages); };
				barrier.SignalAndWait(threadID);
			}
			else
			{
				StartGame.signal = () => { return StartGame.setupTyras(OnClick.CardImages); };
				barrier.SignalAndWait(threadID);
			}
		}

		public override void setupAdrius(int player)
		{
			if (player == 2)
			{
				StartGame.signal = () => { return StartGame.setupAdrius(OnClick.OCardImages); };
				barrier.SignalAndWait(threadID);
			}
			else
			{
				StartGame.signal = () => { return StartGame.setupAdrius(OnClick.CardImages); };
				barrier.SignalAndWait(threadID);
			}
		}

		public override void setupAurian(int player)
		{
			if (player == 2)
			{
				StartGame.signal = () => { return StartGame.setupAurian(OnClick.OCardImages); };
				barrier.SignalAndWait(threadID);
			}
			else
			{
				StartGame.signal = () => { return StartGame.setupAurian(OnClick.CardImages); };
				barrier.SignalAndWait(threadID);
			}
		}

        public override void setupSeals()
        {
            StartGame.signal = () => { return StartGame.setupSeals(OnClick.SealSprites); };
            barrier.SignalAndWait(threadID);
        }

        public override void updateNetwork(Character ch1, Character ch2, bool UpdateVars)
        {
            StartGame.signal = () => { return StartGame.updateNetwork(ch1, ch2, UpdateVars); };
            barrier.SignalAndWait(threadID);
        }

        public override void updateOpponentUI(Character ch, bool showCurrCard, bool showHand)
		{
			StartGame.signal = () => { return StartGame.updateOpponentUI(ch, showCurrCard,showHand); };
			barrier.SignalAndWait(threadID);
		}

		public override void updateUI(Character ch, bool showCurrCard)
		{
			StartGame.signal = () => { return StartGame.updateUI(ch,showCurrCard); };
			barrier.SignalAndWait(threadID);
		}

        public override void resetLock()
        {
            StartGame.signal = () => { return StartGame.resetLock(); };
            barrier.SignalAndWait(threadID);
        }

        public override void resetIcons()
        {
            StartGame.signal = () => { return StartGame.resetIcons(); };
            barrier.SignalAndWait(threadID);
        }

        public override void resetDmgUI()
        {
            StartGame.signal = () => { return StartGame.resetDmgUI(); };
            barrier.SignalAndWait(threadID);
        }
        public override void resetHealingUI()
        {
            StartGame.signal = () => { return StartGame.resetHealingUI(); };
            barrier.SignalAndWait(threadID);
        }

        public override void setDmgUI(bool isPlayer1)
        {
            StartGame.signal = () => { return StartGame.setDmgUI(isPlayer1); };
            barrier.SignalAndWait(threadID);
        }
        public override void setSafeguardPromptUI(bool isPlayer1)
        {
            StartGame.signal = () => { return StartGame.setSafeguardPromptUI(isPlayer1); };
            barrier.SignalAndWait(threadID);
        }
        public override void setReflectPromptUI(bool isPlayer1)
        {
            StartGame.signal = () => { return StartGame.setReflectPromptUI(isPlayer1); };
            barrier.SignalAndWait(threadID);
        }
        public override void setNegatePromptUI(bool isPlayer1, int ngt)
        {
            StartGame.signal = () => { return StartGame.setNegatePromptUI(isPlayer1,ngt); };
            barrier.SignalAndWait(threadID);
        }
        public override void setAbsorbPromptUI(bool isPlayer1)
        {
            StartGame.signal = () => { return StartGame.setAbsorbPromptUI(isPlayer1); };
            barrier.SignalAndWait(threadID);
        }
        public override void resetPrompts()
        {
            StartGame.signal = () => { return StartGame.resetPrompts(); };
            barrier.SignalAndWait(threadID);
        }
        public override void setHealingUI(bool isPlayer1)
        {
            StartGame.signal = () => { return StartGame.setHealingUI(isPlayer1); };
            barrier.SignalAndWait(threadID);
        }

        public override void LockIn(bool isServer)
        {
            StartGame.signal = () => { return StartGame.LockIn(isServer); };
            barrier.SignalAndWait(threadID);
        }

        public override void endSlate(Character ch) // End game UI method
        {
            StartGame.signal = () => { return StartGame.endSlate(ch); };
            barrier.SignalAndWait(threadID);
        }

        public override Character.Card waitForClick()
		{
			OnClick.cardReturn = OnClick.NoReturn;
			StartGame.signal = () => { return StartGame.waitForClick(); };
			barrier.SignalAndWait(threadID);
			return (Character.Card)returnObject;
		}

        public override Character.Card waitForClickorLock()
        {
            OnClick.cardReturn = OnClick.NoReturn;
            StartGame.signal = () => { return StartGame.waitForClickorLock(); };
            barrier.SignalAndWait(threadID);
            if (returnObject != null)
            {
                return (Character.Card)returnObject;
            }
            else
            {
                return null;
            }
        }

        public override Character.Card waitForClickOrCancel(string text)
		{
			OnClick.buttonReturn = OnClick.NoReturn;
			OnClick.cardReturn = OnClick.NoReturn;
			StartGame.signal = () => { return StartGame.waitForClickOrCancel(text); };
			barrier.SignalAndWait(threadID);
			setPromptText("");
			return (Character.Card)returnObject;
		}

		public override bool yesnoPrompt(string text)
		{
			OnClick.buttonReturn = OnClick.NoReturn;
			StartGame.signal = () => { return StartGame.yesnoPrompt(text); };
			barrier.SignalAndWait(threadID);
			setPromptText("");
			return (bool)returnObject;
		}

        public override void waitOnNetwork(ref Character ch1, ref Character ch2)
        {
            var p1 = ch1;
            var p2 = ch2;
            StartGame.signal = () =>{ return StartGame.waitOnNetwork(p1,p2); };
            barrier.SignalAndWait(threadID);
        }
    }
}
