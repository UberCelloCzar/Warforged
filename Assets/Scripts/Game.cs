using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Warforged
{
	public class Game
	{
		public static Character p1;
		public static Character p2;
		public static WindowLibrary library = null;
        public Game ()
        {
        }
        public static void setup(Character character, Character opponent)
        {
            p1 = character;
            if (p1 is Edros)
            {

                p1.hand.Add(new Edros.HandofToren());
                p1.hand.Add(new Edros.CelestialSurge());
                p1.hand.Add(new Edros.PillarofLightning());
                p1.hand.Add(new Edros.TorensFavored());
                foreach (Character.Card c in p1.hand)
                {
                    c.init(p1);
                }
				p1.standby.Add(new Edros.RollingThunder());
				p1.standby.Add(new Edros.SkyBlessedShield());
                p1.standby.Add(new Edros.PurgingLightning());
                p1.standby.Add(new Edros.FaithUnquestioned());
                foreach (Character.Card c in p1.standby)
                {
                    c.init(p1);
                }
                p1.invocation.Add(new Edros.WrathofLightning());
                p1.invocation.Add(new Edros.GraceofHeaven());
                p1.invocation.Add(new Edros.ImminentStorm());
                p1.invocation.Add(new Edros.CrashingSky());
                foreach (Character.Card c in p1.invocation)
                {
                    c.init(p1);
                }
            }
            else if (p1 is Tyras)
            {
                p1.hand.Add(new Tyras.OnraisStrike());
                p1.hand.Add(new Tyras.OnslaughtofTyras());
                p1.hand.Add(new Tyras.WarriorsResolve());
                p1.hand.Add(new Tyras.DecryingRoar());
                foreach (Character.Card c in p1.hand)
                {
                    c.init(p1);
                }
                p1.standby.Add(new Tyras.ArmorofAldras());
                p1.standby.Add(new Tyras.ABrothersVirtue());
                p1.standby.Add(new Tyras.ASoldiersRemorse());
                p1.standby.Add(new Tyras.GrimKnightsDread());
                foreach (Character.Card c in p1.standby)
                {
                    c.init(p1);
                }
                p1.invocation.Add(new Tyras.APromiseUnbroken());
                p1.invocation.Add(new Tyras.AnOathUnforgotten());
                p1.invocation.Add(new Tyras.IntheKingsWake());
                p1.invocation.Add(new Tyras.SunderingStar());
                foreach (Character.Card c in p1.invocation)
                {
                    c.init(p1);
                }
            }

            else if (p1 is Adrius)
            {
                p1.hand.Add(new Adrius.SurgingHope());
                p1.hand.Add(new Adrius.WillUnbreakable());
                p1.hand.Add(new Adrius.FistofRuin());
                p1.hand.Add(new Adrius.TremoringImpact());
                foreach (Character.Card c in p1.hand)
                {
                    c.init(p1);
                }
                p1.standby.Add(new Adrius.EarthPiercer());
                p1.standby.Add(new Adrius.ShatteringBlow());
                p1.standby.Add(new Adrius.HerosResolution());
                p1.standby.Add(new Adrius.UnyieldingFaith());
                foreach (Character.Card c in p1.standby)
                {
                    c.init(p1);
                }
                p1.invocation.Add(new Adrius.SapphireMantle());
                p1.invocation.Add(new Adrius.RubyHeart());
                p1.invocation.Add(new Adrius.EmeraldCore());
                p1.invocation.Add(new Adrius.Ascendance());
                foreach (Character.Card c in p1.invocation)
                {
                    c.init(p1);
                }
            }

			else if (p1 is Aurian)
			{
				p1.hand.Add(new Aurian.ClarityofMind());
				p1.hand.Add(new Aurian.OpeningBlow());
				p1.hand.Add(new Aurian.PrimedAttack());
				p1.hand.Add(new Aurian.EvadingStep());
				foreach (Character.Card c in p1.hand)
				{
					c.init(p1);
				}
				p1.standby.Add(new Aurian.CounteringStrike());
				p1.standby.Add(new Aurian.RelentlessAssault());
				p1.standby.Add(new Aurian.SoulStrike());
				p1.standby.Add(new Aurian.BodyandSoul());
				foreach (Character.Card c in p1.standby)
				{
					c.init(p1);
				}
				p1.invocation.Add(new Aurian.AbsoluteFocus());
				p1.invocation.Add(new Aurian.EtherealStrike());
				p1.invocation.Add(new Aurian.WrathofEra());
				p1.invocation.Add(new Aurian.StrengthofSpirit());
				foreach (Character.Card c in p1.invocation)
				{
					c.init(p1);
				}
			}
                Thread.Sleep(30);
            p2 = opponent;

            if (p2 is Edros)
            {

                p2.hand.Add(new Edros.HandofToren());
                p2.hand.Add(new Edros.CelestialSurge());
                p2.hand.Add(new Edros.PillarofLightning());
                p2.hand.Add(new Edros.TorensFavored());
                foreach (Character.Card c in p2.hand)
                {
                    c.init(p1);
                }
				p2.standby.Add(new Edros.RollingThunder());
				p2.standby.Add(new Edros.SkyBlessedShield());
                p2.standby.Add(new Edros.PurgingLightning());
                p2.standby.Add(new Edros.FaithUnquestioned());
                foreach (Character.Card c in p2.standby)
                {
                    c.init(p1);
                }
                p2.invocation.Add(new Edros.WrathofLightning());
                p2.invocation.Add(new Edros.GraceofHeaven());
                p2.invocation.Add(new Edros.ImminentStorm());
                p2.invocation.Add(new Edros.CrashingSky());
                foreach (Character.Card c in p2.invocation)
                {
                    c.init(p2);
                }
            }
            else if (p2 is Tyras)
            {
                p2.hand.Add(new Tyras.OnraisStrike());
                p2.hand.Add(new Tyras.OnslaughtofTyras());
                p2.hand.Add(new Tyras.WarriorsResolve());
                p2.hand.Add(new Tyras.DecryingRoar());
                foreach (Character.Card c in p2.hand)
                {
                    c.init(p2);
                }
                p2.standby.Add(new Tyras.ArmorofAldras());
                p2.standby.Add(new Tyras.ABrothersVirtue());
                p2.standby.Add(new Tyras.ASoldiersRemorse());
                p2.standby.Add(new Tyras.GrimKnightsDread());
                foreach (Character.Card c in p2.standby)
                {
                    c.init(p2);
                }
                p2.invocation.Add(new Tyras.APromiseUnbroken());
                p2.invocation.Add(new Tyras.AnOathUnforgotten());
                p2.invocation.Add(new Tyras.IntheKingsWake());
                p2.invocation.Add(new Tyras.SunderingStar());
                foreach (Character.Card c in p2.invocation)
                {
                    c.init(p2);
                }
            }
            if (p2 is Adrius)
            {
                p2.hand.Add(new Adrius.SurgingHope());
                p2.hand.Add(new Adrius.WillUnbreakable());
                p2.hand.Add(new Adrius.FistofRuin());
                p2.hand.Add(new Adrius.TremoringImpact());
                foreach (Character.Card c in p2.hand)
                {
                    c.init(p2);
                }
                p2.standby.Add(new Adrius.EarthPiercer());
                p2.standby.Add(new Adrius.ShatteringBlow());
                p2.standby.Add(new Adrius.HerosResolution());
                p2.standby.Add(new Adrius.UnyieldingFaith());
                foreach (Character.Card c in p2.standby)
                {
                    c.init(p2);
                }
                p2.invocation.Add(new Adrius.SapphireMantle());
                p2.invocation.Add(new Adrius.RubyHeart());
                p2.invocation.Add(new Adrius.EmeraldCore());
                p2.invocation.Add(new Adrius.Ascendance());
                foreach (Character.Card c in p2.invocation)
                {
                    c.init(p2);
                }
			}
			else if (p2 is Aurian)
			{
				p2.hand.Add(new Aurian.ClarityofMind());
				p2.hand.Add(new Aurian.OpeningBlow());
				p2.hand.Add(new Aurian.PrimedAttack());
				p2.hand.Add(new Aurian.EvadingStep());
				foreach (Character.Card c in p2.hand)
				{
					c.init(p2);
				}
				p2.standby.Add(new Aurian.CounteringStrike());
				p2.standby.Add(new Aurian.RelentlessAssault());
				p2.standby.Add(new Aurian.SoulStrike());
				p2.standby.Add(new Aurian.BodyandSoul());
				foreach (Character.Card c in p2.standby)
				{
					c.init(p2);
				}
				p2.invocation.Add(new Aurian.AbsoluteFocus());
				p2.invocation.Add(new Aurian.EtherealStrike());
				p2.invocation.Add(new Aurian.WrathofEra());
				p2.invocation.Add(new Aurian.StrengthofSpirit());
				foreach (Character.Card c in p2.invocation)
				{
					c.init(p2);
				}
			}
			p1.setOpponent(p2);
			p2.setOpponent(p1);
            p1.isPlayer1 = true;
            p2.isPlayer1 = false;
        }
		public void takeTurn()
        {
            //Debug.Log("taketurn");
            try
            {
                // Start Selection Phase
                OnClick.controller.localPlayer.readyFlag2 = false;
                OnClick.controller.remotePlayer.readyFlag2 = false;
                //Debug.Log("First updates");
                p1.nextPhase();
                //library.setPromptText("before sleep0");
                library.updateUI(p1, true);
                ++p1.turn;
                //Debug.Log("Attempting to update network");
                library.updateNetwork(p1, p2, false);
                //Debug.Log("Waiting on network");
                library.waitOnNetwork(ref p1, ref p2);
                //library.updateNetowrk(p1);
                library.updateOpponentUI(p2, true, false);
                //library.setPromptText("before sleep");
                //Debug.Log("Got to card phase");
                library.resetLock();
                p1.playCard();

                // Start Waiting Selection
                if (OnClick.controller.remotePlayer.readyFlag2 == true)
                {
                    p1.phase = Character.Phase.Declare;
                }
                else
                {
                    p1.nextPhase();
                }
                library.updateUI(p1, false);
                while (OnClick.controller.remotePlayer.readyFlag2 != true)
                {
                    //Debug.Log("Waiting for enemy lock in");
                }
                //Debug.Log("Both Players Locked in");

                // Start Declare Phase
                library.updateUI(p1, false);
                ++p1.turn;
                library.updateNetwork(p1, p2, false);
                library.waitOnNetwork(ref p1, ref p2);
                library.updateOpponentUI(p2, false, false);
                //library.setPromptText("before sleep1");
                if(!p1.phase.Equals(Character.Phase.Declare))
                {
                    p1.nextPhase();
                }
                library.updateUI(p1, false);

                p1.declarePhase();
                //Debug.Log("My seal: " + p1.seal);
                //Debug.Log("His seal: " + p2.seal);

                p1.nextPhase();
                p2.nextPhase();
                library.updateUI(p1, true);
                ++p1.turn;
                library.updateNetwork(p1, p2, true);
                library.waitOnNetwork(ref p1, ref p2);
                //library.updateNetowrk(p1);
                library.updateOpponentUI(p2, true, false);
                //library.setPromptText("before sleep2");
                //Debug.Log("My seal updated: " + p1.seal);
                //Debug.Log("His seal updated: " + p2.seal);

                // Start Damage Phase
                p1.nextPhase();
                p2.nextPhase();
                library.updateUI(p1, true);

                p1.damagePhase();
                p2.damagePhase();

                if (p1.endGame != 0) // If the endgame is triggered, show it
                {
                    Debug.Log("Game Over escalated");
                    library.endSlate(p1);
                }
                //library.setPromptText("before sleep3");
                Thread.Sleep(2500);
                //library.setPromptText("after sleep");

                // Start Dusk Phase
                p1.nextPhase();
                library.updateUI(p1, true);
                ++p1.turn;
                library.updateNetwork(p1, p2, false);
                library.waitOnNetwork(ref p1, ref p2);
                //library.updateNetowrk(p1);
                library.updateOpponentUI(p2, true, false);
                //library.setPromptText("after sleep1");

                library.resetDmgUI();
                library.resetHealingUI();
                p1.dusk();
                Thread.Sleep(1500);

                // Start Dawn Phase
                p1.nextPhase();
                library.updateUI(p1, true);
                ++p1.turn;
                library.updateNetwork(p1, p2, false);
                library.waitOnNetwork(ref p1, ref p2);
                library.updateOpponentUI(p2, true, false);
                //library.setPromptText("after sleep2");

                p1.dawn();
                library.resetIcons();
                Thread.Sleep(1500);
            } catch(Exception e)
            {
                Debug.Log("Exception: " + e);
                library.setPromptText("Exception: " + e);
                library.setPromptText("Exception: "+e);
            }

			// Heal
			// If anyone dies, do it at the end
		}

		//@param: This will return a string which comes from the UI
		//The UI tells the model which character has been selected
		public static void Main()
		{

            //We need to determine our opponent before this point.
            Game game = new Game();
            library = new UnityLibrary();

            if (p1 is Edros)
            {
                library.setupEdros(1);
            }
            else if (p1 is Tyras)
            {
                library.setupTyras(1);
			}
			else if (p1 is Adrius)
			{
				library.setupAdrius(1);
			}
			else if (p1 is Aurian)
			{
				library.setupAurian(1);
			}

            Thread.Sleep(30);

            if (p2 is Edros)
            {
                library.setupEdros(2);
            }
            else if (p2 is Tyras)
            {
                library.setupTyras(2);
			}
			else if (p2 is Adrius)
			{
				library.setupAdrius(2);
			}
			else if (p2 is Aurian)
			{
				library.setupAurian(2);
			}

            library.setupSeals();

            while (true)
            {
                game.takeTurn();
			}
		}

	}
}

