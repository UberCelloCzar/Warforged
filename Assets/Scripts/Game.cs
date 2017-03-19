using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

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
                p1.standby.Add(new Edros.SkyBlessedShield());
                p1.standby.Add(new Edros.RollingThunder());
                p1.standby.Add(new Edros.PurgingLightning());
                p1.standby.Add(new Edros.FaithUnquestioned());
                foreach (Character.Card c in p1.standby)
                {
                    c.init(p1);
                }
                p1.invocation.Add(new Edros.WrathofLightning());
                p1.invocation.Add(new Edros.GraceofHeaven());
                p1.invocation.Add(new Edros.ScornofThunder());
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
                p2.standby.Add(new Edros.SkyBlessedShield());
                p2.standby.Add(new Edros.RollingThunder());
                p2.standby.Add(new Edros.PurgingLightning());
                p2.standby.Add(new Edros.FaithUnquestioned());
                foreach (Character.Card c in p2.standby)
                {
                    c.init(p1);
                }
                p2.invocation.Add(new Edros.WrathofLightning());
                p2.invocation.Add(new Edros.GraceofHeaven());
                p2.invocation.Add(new Edros.ScornofThunder());
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
                p2.setOpponent(p2);
            }
            p2.setOpponent(p2);
        }
		public void takeTurn()
        {
            try
            {
                //library.setPromptText("before sleep0");
                library.updateUI(p1, true);
                ++p1.turn;
                library.updateNetowrk(p1);
                library.waitOnNetwork(ref p1, ref p2);
                //library.updateNetowrk(p1);
                library.updateOpponentUI(p2, true, false);
                //library.setPromptText("before sleep");

                p1.playCard();
                //p2.playCard();

                library.updateUI(p1, false);
                ++p1.turn;
                library.updateNetowrk(p1);
                library.waitOnNetwork(ref p1, ref p2);
                library.updateOpponentUI(p2, false, false);
                //library.setPromptText("before sleep1");

                p1.declarePhase();
                //p2.declarePhase();


                library.updateUI(p1, true);
                ++p1.turn;
                library.updateNetowrk(p1);
                library.waitOnNetwork(ref p1, ref p2);
                //library.updateNetowrk(p1);
                library.updateOpponentUI(p2, true, false);
                //library.setPromptText("before sleep2");
                p1.damagePhase();
                p2.damagePhase();
                //library.setPromptText("before sleep3");
                Thread.Sleep(2500);
                //library.setPromptText("after sleep");
                library.updateUI(p1, true);
                ++p1.turn;
                library.updateNetowrk(p1);
                library.waitOnNetwork(ref p1, ref p2);
                //library.updateNetowrk(p1);
                library.updateOpponentUI(p2, true, false);
                //library.setPromptText("after sleep1");

                p1.dusk();


                library.updateUI(p1, true);
                ++p1.turn;
                library.updateNetowrk(p1);
                library.waitOnNetwork(ref p1, ref p2);
                library.updateOpponentUI(p2, true, false);
                //library.setPromptText("after sleep2");

                p1.dawn();
            } catch(Exception e)
            {
                library.setPromptText("Exception: "+e);
            }

			// Heal
			// If anyone dies, do it at the end
		}

		//@param: This will return a string which comes from the UI
		//The UI tells the model which character has been selected
		public static void Main()
		{

            //We need to determin our opponent before this point.
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

            while (true)
            {
                game.takeTurn();
			}
		}

	}
}

