using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using Warforged;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using UnityEngine.Networking;

public class StartGame : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		OnClick.buttonReturn = OnClick.NoReturn;
		OnClick.cardReturn = OnClick.NoReturn;
        StartCoroutine(StartModel(characterPick));
        
    }

	// Update is called once per frame
	void Update () {
	}
    public static bool clientReady = false;

    //void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    //{

    //}
    public static bool networkUpdated = false;
    public static Character characterPick = null;
	public delegate IEnumerator ModelSignal();
	public static ModelSignal signal = null;
	public static UnityLibrary lib = null;
	private static int threadID = 0;

    private static Thread t = null;
    public static IEnumerator StartModel(Character character)
    {
        t = new Thread(() => Game.Main());
        XmlSerializer xml = new XmlSerializer(typeof(Character));
        StringWriter sw = new StringWriter();
        //var xwrite = XmlWriter.Create(sw);
        xml.Serialize(sw, characterPick);
        if (PlayerController.controller.localPlayer.isServer)
        {
            PlayerController.controller.localPlayer.CmdInit(sw.GetStringBuilder().ToString(), PlayerController.controller.localPlayer.isServer);
        }
        else
        {
            PlayerController.controller.localPlayer.CmdInit(sw.GetStringBuilder().ToString(), PlayerController.controller.localPlayer.isServer);
        }
        yield return new WaitUntil(() => Game.p2 != null);
        Game.setup(characterPick,Game.p2);
        t.Start();
        yield return new WaitUntil(() => Game.library != null && ((UnityLibrary)Game.library).barrier != null);


        lib = (UnityLibrary)Game.library;
        threadID = lib.barrier.AddThread();
		while (true)
		{
			yield return new WaitUntil(() => signal != null);
			yield return signal();
            signal = null;
            lib.barrier.SignalAndWait(threadID);
		}
	}

	public static IEnumerator yesnoPrompt(string text)
	{
		OnClick.setButtonOptions(text,new List<string>() {"Yes","No" },new List<object>() {true,false });
		yield return new WaitUntil(() => !OnClick.NoReturn.Equals(OnClick.buttonReturn));
		OnClick.Prompt.text = "";
		lib.setReturnObject(OnClick.buttonReturn);
		OnClick.buttonReturn = OnClick.NoReturn;
		yield return null;
	}
	public static IEnumerator multiPrompt(string text, List<string> buttonTexts, List<object> returns)
	{
		OnClick.Prompt.text = text;
		OnClick.setButtonOptions(text, buttonTexts, returns);
		yield return new WaitUntil(() => !OnClick.NoReturn.Equals(OnClick.buttonReturn));
		lib.setReturnObject(OnClick.buttonReturn);
		OnClick.buttonReturn = OnClick.NoReturn;
		yield return null;
	}

	public static IEnumerator setPromptText(string text)
	{
		OnClick.Prompt.text = text;
		yield return null;
	}

	public static IEnumerator waitForClick()
	{

		yield return new WaitUntil(() => !OnClick.NoReturn.Equals(OnClick.cardReturn));
		lib.setReturnObject(OnClick.cardReturn);
		OnClick.cardReturn = OnClick.NoReturn;
		yield return null;
	}

	public static IEnumerator waitForClickOrCancel(string text)
	{
		OnClick.setButtonOptions(text, new List<string>() { "Cancel" }, new List<object>() { null });
		yield return new WaitUntil(() => !OnClick.NoReturn.Equals(OnClick.cardReturn) || !OnClick.NoReturn.Equals(OnClick.buttonReturn));
		for (int i = 0; i < 6; ++i)
		{
			OnClick.allButtons[i].gameObject.SetActive(false);
			OnClick.allButtons[i].GetComponentInChildren<Text>().text = "";
		}
		if (!OnClick.NoReturn.Equals(OnClick.buttonReturn))
		{
			lib.setReturnObject(OnClick.buttonReturn);
		}
		else
		{
			lib.setReturnObject(OnClick.cardReturn);
		}
		OnClick.cardReturn = OnClick.NoReturn;
		OnClick.buttonReturn = OnClick.NoReturn;
		OnClick.Prompt.text = "";
		yield return null;
    }
    public static IEnumerator waitOnNetwork(Character ch1, Character ch2)
    {
        yield return new WaitUntil(() => { /*Debug.Log("Turn 1: "+ Game.p1.turn+" Turn 2: "+ Game.p2.turn);*/ return networkUpdated; });
        networkUpdated = false;
    }
    public static IEnumerator updateNetwork(Character ch1, Character ch2, bool UpdateVars)
    {
        XmlSerializer xml = new XmlSerializer(typeof(Character));
        StringWriter sw = new StringWriter();
        //var xwrite = XmlWriter.Create(sw);
        xml.Serialize(sw, ch1);
        //if (PlayerController.controller.localPlayer.isServer)
        //{
            PlayerController.controller.localPlayer.CmdSetCharacter(sw.GetStringBuilder().ToString(), PlayerController.controller.localPlayer.isServer, 1, UpdateVars);
        //}
        //else
        //{
        //    PlayerController.controller.localPlayer.CmdSetCharacter(sw.GetStringBuilder().ToString(), PlayerController.controller.localPlayer.isServer);
        //}
        xml = new XmlSerializer(typeof(Character));
        sw.Close();
        sw = new StringWriter();
        xml.Serialize(sw, ch2);
        //if (PlayerController.controller.localPlayer.isServer)
        //{
               PlayerController.controller.localPlayer.CmdSetCharacter(sw.GetStringBuilder().ToString(), PlayerController.controller.localPlayer.isServer, 2, UpdateVars);
        //}
        //else
        //{
        // 
        yield return null;
    }

    public static IEnumerator updateUI(Character ch, bool showCurrCard)
    {
        for (int i = 0; i < OnClick.Hand.Count; ++i)
        {
            if (ch.hand.Count <= i)
            {
                OnClick.Hand[i].sprite = null;
                OnClick.Hand[i].color = new UnityEngine.Color(1.0f, 1.0f, 1.0f, 0.0f);
                OnClick.Hand[i].gameObject.SetActive(false);
            }
            else
            {

                OnClick.Hand[i].sprite = OnClick.CardImages[ch.hand[i].name];
                OnClick.Hand[i].color = new UnityEngine.Color(1, 1, 1);
                OnClick.Hand[i].gameObject.SetActive(true);
                OnClick.cardDict["Hand" + i] = ch.hand[i];
            }
        }
        /*
        for (int i = 0; i < OnClick.Suspend.Count; ++i)
        {
            if (ch.Suspend.Count <= i)
            {
                OnClick.Suspend[i].sprite = null;
            }
            else
            {
                OnClick.Suspend[i].sprite = OnClick.CardImages[ch.name];
            }
        }*/
        for (int i = 0; i < OnClick.Standby.Count; ++i)
        {
            if (ch.standby.Count <= i)
            {
                OnClick.Standby[i].sprite = null;
                OnClick.Standby[i].color = new UnityEngine.Color(1.0f, 1.0f, 1.0f, 0.33f);
                OnClick.Standby[i].gameObject.SetActive(false);
            }
            else
            {
                OnClick.Standby[i].sprite = OnClick.CardImages[ch.standby[i].name];
				if (ch.standby [i].active)
				{
					OnClick.Standby [i].color = new UnityEngine.Color (1, 1, 1);
				}
				else
				{
					OnClick.Standby [i].color = new UnityEngine.Color (0, 0, 0);
				}
                OnClick.Standby[i].gameObject.SetActive(true);
                OnClick.cardDict["Standby" + (i + 1)] = ch.standby[i];
            }
        }

        for (int i = 0; i < OnClick.Invocation.Count; ++i)
        {
            if (ch.invocation.Count <= i)
            {
                OnClick.Invocation[i].sprite = null;
                OnClick.Invocation[i].color = new UnityEngine.Color(1.0f, 1.0f, 1.0f, 0.33f);
                OnClick.Invocation[i].gameObject.SetActive(false);
            }
            else if (!ch.invocation[i].active)
			{
				OnClick.Invocation[i].sprite = OnClick.CardImages[ch.invocation[i].name];
                OnClick.Invocation[i].color = new UnityEngine.Color(0, 0, 0);
                OnClick.Invocation[i].gameObject.SetActive(true);
                OnClick.cardDict["Invocation" + (i + 1)] = OnClick.NoReturn; ;
            }
            else
            {
                OnClick.Invocation[i].sprite = OnClick.CardImages[ch.invocation[i].name];
                OnClick.Invocation[i].color = new UnityEngine.Color(1, 1, 1);
                OnClick.Invocation[i].gameObject.SetActive(true);
                OnClick.cardDict["Invocation" + (i + 1)] = ch.invocation[i];
            }
        }
        if (ch.currCard == null)
        {
            OnClick.PlaySlot.sprite = null;
            OnClick.PlaySlot.color = new UnityEngine.Color(1.0f, 1.0f, 1.0f, 0.33f);
            OnClick.PlaySlot.gameObject.SetActive(false);
        }
        else if (showCurrCard)
        {
            OnClick.PlaySlot.sprite = OnClick.CardImages[ch.currCard.name];
            OnClick.PlaySlot.color = new UnityEngine.Color(1, 1, 1);
            OnClick.PlaySlot.gameObject.SetActive(true);
            OnClick.cardDict["PlaySlot"] = ch.currCard;
        }
        else
        {
			OnClick.PlaySlot.sprite = OnClick.CardImages[ch.currCard.name]; // CHANGED
            OnClick.PlaySlot.color = new UnityEngine.Color(0, 0, 0);
            OnClick.PlaySlot.gameObject.SetActive(true);
            OnClick.cardDict["PlaySlot"] = ch.currCard;
        }
        OnClick.CharacterSlot.sprite = OnClick.CardImages[ch.name];
        OnClick.CharacterSlot.color = new UnityEngine.Color(1, 1, 1);
        OnClick.Health.text = ch.hp + "HP";
        OnClick.Empower.text = "Empower(" + ch.currEmpower + ")";
        OnClick.Reinforce.text = "Reinforce(" + ch.reinforce + ")";
        OnClick.Phase.text = ch.displayPhase();
        yield return null;
    }

    public static IEnumerator endSlate(Character ch) // End game UI method
    {
        //Debug.Log("Game Over reached top");
        OnClick.GameOver.text = "Game Over.\n";
        if (ch.endGame == 1)
        {
            OnClick.GameOver.text += "You win"; // Add the appropriate name to the win text
        }
        else
        {
            OnClick.GameOver.text += "Your opponent wins";
        }
        OnClick.GameOver.text += "!\nClick anywhere to quit.";
        OnClick.GameOver.rectTransform.anchoredPosition3D = new Vector3(0, 0, -.1f); // Move the message on screen
        OnClick.GameOver.rectTransform.pivot = new Vector2(.5f, .5f);
        while (true) // Nice
        {
            if (Input.GetMouseButtonDown(0))
            {
                Application.Quit();
            }
            yield return null;
        }
    }

    public static IEnumerator updateOpponentUI(Character ch, bool showCurrCard, bool showHand)
    {
        for (int i = 0; i < OnClick.OHand.Count; ++i)
        {
            if (ch.hand.Count <= i)
            {
                OnClick.OHand[i].sprite = null;
                OnClick.OHand[i].color = new UnityEngine.Color(1.0f, 1.0f, 1.0f, 0.0f);
                OnClick.OHand[i].gameObject.SetActive(false);
            }
            else if (!showHand)
            {
                OnClick.OHand[i].sprite = null;
                OnClick.OHand[i].color = new UnityEngine.Color(0, 0, 0, 1);
                OnClick.OHand[i].gameObject.SetActive(true);
                OnClick.cardDict["OHand" + i] = ch.hand[i];
            }
            else
            {

                OnClick.OHand[i].sprite = OnClick.OCardImages[ch.hand[i].name];
                OnClick.OHand[i].color = new UnityEngine.Color(1, 1, 1, 1);
                OnClick.OHand[i].gameObject.SetActive(true);
                OnClick.cardDict["OHand" + i] = ch.hand[i];
            }
        }
        /*
        for (int i = 0; i < OnClick.Suspend.Count; ++i)
        {
            if (ch.Suspend.Count <= i)
            {
                OnClick.Suspend[i].sprite = null;
            }
            else
            {
                OnClick.Suspend[i].sprite = OnClick.CardImages[ch.name];
            }
        }*/
        for (int i = 0; i < OnClick.OStandby.Count; ++i)
        {
            if (ch.standby.Count <= i)
            {
                OnClick.OStandby[i].sprite = null;
                OnClick.OStandby[i].color = new UnityEngine.Color(1.0f, 1.0f, 1.0f, 0.33f);
                OnClick.OStandby[i].gameObject.SetActive(false);
            }
            else
            {
                OnClick.OStandby[i].sprite = OnClick.OCardImages[ch.standby[i].name];
                OnClick.OStandby[i].color = new UnityEngine.Color(1, 1, 1);
                OnClick.OStandby[i].gameObject.SetActive(true);
                OnClick.cardDict["OStandby" + (i + 1)] = ch.standby[i];
            }
        }
        for (int i = 0; i < OnClick.OInvocation.Count; ++i)
        {
            if (ch.invocation.Count <= i)
            {
                OnClick.OInvocation[i].sprite = null;
                OnClick.OInvocation[i].color = new UnityEngine.Color(1.0f, 1.0f, 1.0f, 0.33f);
                OnClick.OInvocation[i].gameObject.SetActive(false);
            }
            else if (!ch.invocation[i].active)
            {
                OnClick.OInvocation[i].sprite = null;
                OnClick.OInvocation[i].color = new UnityEngine.Color(0, 0, 0);
                OnClick.OInvocation[i].gameObject.SetActive(true);
                OnClick.cardDict["OInvocation" + (i + 1)] = OnClick.NoReturn;
            }
            else
            {
                OnClick.OInvocation[i].sprite = OnClick.OCardImages[ch.invocation[i].name];
                OnClick.OInvocation[i].color = new UnityEngine.Color(1, 1, 1);
                OnClick.OInvocation[i].gameObject.SetActive(true);
                OnClick.cardDict["OInvocation" + (i + 1)] = ch.invocation[i];
            }
        }
        if (ch.currCard == null)
        {
            OnClick.OPlaySlot.sprite = null;
            OnClick.OPlaySlot.color = new UnityEngine.Color(1.0f, 1.0f, 1.0f, 0.33f);
            OnClick.OPlaySlot.gameObject.SetActive(false);
        }
        else if (showCurrCard)
        {
            OnClick.OPlaySlot.sprite = OnClick.OCardImages[ch.currCard.name];
            OnClick.OPlaySlot.color = new UnityEngine.Color(1, 1, 1);
            OnClick.OPlaySlot.gameObject.SetActive(true);
            OnClick.cardDict["OPlaySlot"] = ch.currCard;
        }
        else
        {
            OnClick.OPlaySlot.sprite = null;
            OnClick.OPlaySlot.color = new UnityEngine.Color(0, 0, 0);
            OnClick.OPlaySlot.gameObject.SetActive(true);
            OnClick.cardDict["OPlaySlot"] = ch.currCard;
        }
        OnClick.OCharacterSlot.sprite = OnClick.OCardImages[ch.name];
        OnClick.OCharacterSlot.color = new UnityEngine.Color(1, 1, 1);
        OnClick.OHealth.text = ch.hp + "HP";
        OnClick.OEmpower.text = "Empower(" + ch.empower + ")";
        OnClick.OReinforce.text = "Reinforce(" + ch.reinforce + ")";
        yield return null;
    }


    public static IEnumerator setupEdros(Dictionary<string, Sprite> CurrCardImages)
	{

		CurrCardImages.Add("Celestial Surge", Resources.Load("CardImages/Edros/Celestial Surge", typeof(Sprite)) as Sprite);
		CurrCardImages.Add("Purging Lightning", Resources.Load("CardImages/Edros/Purging Lightning", typeof(Sprite)) as Sprite);
		CurrCardImages.Add("Crashing Sky", Resources.Load("CardImages/Edros/Crashing Sky", typeof(Sprite)) as Sprite);
		CurrCardImages.Add("Edros", Resources.Load<Sprite>("CardImages/Edros/Edros"));

		CurrCardImages.Add("Faith Unquestioned", Resources.Load("CardImages/Edros/Faith Unquestioned", typeof(Sprite)) as Sprite);
		CurrCardImages.Add("Grace of Heaven", Resources.Load("CardImages/Edros/Grace of Heaven", typeof(Sprite)) as Sprite);
		CurrCardImages.Add("Hand of Toren", Resources.Load("CardImages/Edros/Hand of Toren", typeof(Sprite)) as Sprite);

		CurrCardImages.Add("Suppressing Bolt", Resources.Load("CardImages/Edros/Suppressing Bolt", typeof(Sprite)) as Sprite);
		CurrCardImages.Add("Rolling Thunder", Resources.Load("CardImages/Edros/Rolling Thunder", typeof(Sprite)) as Sprite);
		CurrCardImages.Add("Imminent Storm", Resources.Load("CardImages/Edros/Imminent Storm", typeof(Sprite)) as Sprite);

		CurrCardImages.Add("Sky Blessed Shield", Resources.Load("CardImages/Edros/Sky Blessed Shield", typeof(Sprite)) as Sprite);
		CurrCardImages.Add("Toren's Favored", Resources.Load("CardImages/Edros/Toren's Favored", typeof(Sprite)) as Sprite);
		CurrCardImages.Add("Wrath of Lightning", Resources.Load("CardImages/Edros/Wrath of Lightning", typeof(Sprite)) as Sprite);

        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Celestial Surge"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Celestial Surge"], Resources.Load("CardZooms/Edros/Celestial Surge", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Purging Lightning"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Purging Lightning"], Resources.Load("CardZooms/Edros/Purging Lightning", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Crashing Sky"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Crashing Sky"], Resources.Load("CardZooms/Edros/Crashing Sky", typeof(Sprite)) as Sprite);
        }

        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Edros"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Edros"], Resources.Load<Sprite>("CardZooms/Edros/Edros"));
        }
        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Faith Unquestioned"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Faith Unquestioned"], Resources.Load("CardZooms/Edros/Faith Unquestioned", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Grace of Heaven"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Grace of Heaven"], Resources.Load("CardZooms/Edros/Grace of Heaven", typeof(Sprite)) as Sprite);
        }

        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Hand of Toren"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Hand of Toren"], Resources.Load("CardZooms/Edros/Hand of Toren", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Suppressing Bolt"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Suppressing Bolt"], Resources.Load("CardZooms/Edros/Suppressing Bolt", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Rolling Thunder"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Rolling Thunder"], Resources.Load("CardZooms/Edros/Rolling Thunder", typeof(Sprite)) as Sprite);
        }

        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Imminent Storm"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Imminent Storm"], Resources.Load("CardZooms/Edros/Imminent Storm", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Sky Blessed Shield"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Sky Blessed Shield"], Resources.Load("CardZooms/Edros/Sky Blessed Shield", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Toren's Favored"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Toren's Favored"], Resources.Load("CardZooms/Edros/Toren's Favored", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Wrath of Lightning"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Wrath of Lightning"], Resources.Load("CardZooms/Edros/Wrath of Lightning", typeof(Sprite)) as Sprite);
        }
		yield return null;
	}

    public static IEnumerator setupAdrius(Dictionary<string, Sprite> CurrCardImages)
    {

        CurrCardImages.Add("Ascendance", Resources.Load("CardImages/Adrius/Ascendance", typeof(Sprite)) as Sprite);
        CurrCardImages.Add("Divine Cataclysm", Resources.Load("CardImages/Adrius/Divine Cataclysm", typeof(Sprite)) as Sprite);
        CurrCardImages.Add("Earth Piercer", Resources.Load("CardImages/Adrius/Earth Piercer", typeof(Sprite)) as Sprite);

        CurrCardImages.Add("Adrius (Ral'Taris Incarnate)", Resources.Load<Sprite>("CardImages/Adrius/Adrius (Ral'Taris Incarnate)"));
        CurrCardImages.Add("Adrius (The Aspirer)", Resources.Load<Sprite>("CardImages/Adrius/Adrius (The Aspirer)"));
        CurrCardImages.Add("Adrius (The Realm Bearer)", Resources.Load<Sprite>("CardImages/Adrius/Adrius (The Realm Bearer)"));

        CurrCardImages.Add("Emerald Core", Resources.Load("CardImages/Adrius/Emerald Core", typeof(Sprite)) as Sprite);
        CurrCardImages.Add("Fist of Ruin", Resources.Load("CardImages/Adrius/Fist of Ruin", typeof(Sprite)) as Sprite);
        CurrCardImages.Add("Hero’s Resolution", Resources.Load("CardImages/Adrius/Hero’s Resolution", typeof(Sprite)) as Sprite);

        CurrCardImages.Add("Ruby Heart", Resources.Load("CardImages/Adrius/Ruby Heart", typeof(Sprite)) as Sprite);
        CurrCardImages.Add("Sapphire Mantle", Resources.Load("CardImages/Adrius/Sapphire Mantle", typeof(Sprite)) as Sprite);
        CurrCardImages.Add("Shattering Blow", Resources.Load("CardImages/Adrius/Shattering Blow", typeof(Sprite)) as Sprite);

        CurrCardImages.Add("Surging Hope", Resources.Load("CardImages/Adrius/Surging Hope", typeof(Sprite)) as Sprite);
        CurrCardImages.Add("Tremoring Impact", Resources.Load("CardImages/Adrius/Tremoring Impact", typeof(Sprite)) as Sprite);
        CurrCardImages.Add("Unyielding Faith", Resources.Load("CardImages/Adrius/Unyielding Faith", typeof(Sprite)) as Sprite);
        CurrCardImages.Add("Will Unbreakable", Resources.Load("CardImages/Adrius/Will Unbreakable", typeof(Sprite)) as Sprite);

		if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Ascendance"]))
		{
			OnClick.CardZooms.Add(CurrCardImages["Ascendance"], Resources.Load("CardZooms/Adrius/Ascendance", typeof(Sprite)) as Sprite);
		}
        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Divine Cataclysm"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Divine Cataclysm"], Resources.Load("CardZooms/Adrius/Divine Cataclysm", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Earth Piercer"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Earth Piercer"], Resources.Load("CardZooms/Adrius/Earth Piercer", typeof(Sprite)) as Sprite);
        }

        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Adrius (Ral'Taris Incarnate)"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Adrius (Ral'Taris Incarnate)"], Resources.Load<Sprite>("CardZooms/Adrius/Adrius (Ral'Taris Incarnate)"));
        }
        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Adrius (The Aspirer)"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Adrius (The Aspirer)"], Resources.Load<Sprite>("CardZooms/Adrius/Adrius (The Aspirer)"));
        }
        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Adrius (The Realm Bearer)"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Adrius (The Realm Bearer)"], Resources.Load<Sprite>("CardZooms/Adrius/Adrius (The Realm Bearer)"));
        }

        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Emerald Core"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Emerald Core"], Resources.Load("CardZooms/Adrius/Emerald Core", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Fist of Ruin"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Fist of Ruin"], Resources.Load("CardZooms/Adrius/Fist of Ruin", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Hero’s Resolution"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Hero’s Resolution"], Resources.Load("CardZooms/Adrius/Hero’s Resolution", typeof(Sprite)) as Sprite);
        }

        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Ruby Heart"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Ruby Heart"], Resources.Load("CardZooms/Adrius/Ruby Heart", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Sapphire Mantle"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Sapphire Mantle"], Resources.Load("CardZooms/Adrius/Sapphire Mantle", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Shattering Blow"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Shattering Blow"], Resources.Load("CardZooms/Adrius/Shattering Blow", typeof(Sprite)) as Sprite);
        }

        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Surging Hope"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Surging Hope"], Resources.Load("CardZooms/Adrius/Surging Hope", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Tremoring Impact"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Tremoring Impact"], Resources.Load("CardZooms/Adrius/Tremoring Impact", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Unyielding Faith"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Unyielding Faith"], Resources.Load("CardZooms/Adrius/Unyielding Faith", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Will Unbreakable"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Will Unbreakable"], Resources.Load("CardZooms/Adrius/Will Unbreakable", typeof(Sprite)) as Sprite);
        }
        yield return null;
    }

    public static IEnumerator setupTyras(Dictionary<string, Sprite> CurrCardImages)
	{

		CurrCardImages.Add("A Brother's Virtue", Resources.Load("CardImages/Tyras/A Brother's Virtue", typeof(Sprite)) as Sprite);
		CurrCardImages.Add("A Promise Unbroken", Resources.Load("CardImages/Tyras/A Promise Unbroken", typeof(Sprite)) as Sprite);
		CurrCardImages.Add("A Soldier's Remorse", Resources.Load("CardImages/Tyras/A Soldier's Remorse", typeof(Sprite)) as Sprite);
		CurrCardImages.Add("An Oath Unforgotten", Resources.Load<Sprite>("CardImages/Tyras/An Oath Unforgotten"));

		CurrCardImages.Add("Armor of Aldras", Resources.Load("CardImages/Tyras/Armor of Aldras", typeof(Sprite)) as Sprite);
		CurrCardImages.Add("Decrying Roar", Resources.Load("CardImages/Tyras/Decrying Roar", typeof(Sprite)) as Sprite);
		CurrCardImages.Add("Grim Knight's Dread", Resources.Load("CardImages/Tyras/Grim Knight's Dread", typeof(Sprite)) as Sprite);

		CurrCardImages.Add("In the King's Wake", Resources.Load("CardImages/Tyras/In the King's Wake", typeof(Sprite)) as Sprite);
		CurrCardImages.Add("Onrai's Strike", Resources.Load("CardImages/Tyras/Onrai's Strike", typeof(Sprite)) as Sprite);
		CurrCardImages.Add("Onslaught of Tyras", Resources.Load("CardImages/Tyras/Onslaught of Tyras", typeof(Sprite)) as Sprite);

		CurrCardImages.Add("Sundering Star", Resources.Load("CardImages/Tyras/Sundering Star", typeof(Sprite)) as Sprite);
		CurrCardImages.Add("Tyras", Resources.Load("CardImages/Tyras/Tyras", typeof(Sprite)) as Sprite);
		CurrCardImages.Add("Warrior's Resolve", Resources.Load("CardImages/Tyras/Warrior's Resolve", typeof(Sprite)) as Sprite);

        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["A Brother's Virtue"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["A Brother's Virtue"], Resources.Load("CardZooms/Tyras/A Brother's Virtue", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["A Promise Unbroken"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["A Promise Unbroken"], Resources.Load("CardZooms/Tyras/A Promise Unbroken", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["A Soldier's Remorse"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["A Soldier's Remorse"], Resources.Load("CardZooms/Tyras/A Soldier's Remorse", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["An Oath Unforgotten"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["An Oath Unforgotten"], Resources.Load<Sprite>("CardZooms/Tyras/An Oath Unforgotten"));
        }

        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Armor of Aldras"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Armor of Aldras"], Resources.Load("CardZooms/Tyras/Armor of Aldras", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Decrying Roar"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Decrying Roar"], Resources.Load("CardZooms/Tyras/Decrying Roar", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Grim Knight's Dread"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Grim Knight's Dread"], Resources.Load("CardZooms/Tyras/Grim Knight's Dread", typeof(Sprite)) as Sprite);
        }

        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["In the King's Wake"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["In the King's Wake"], Resources.Load("CardZooms/Tyras/In the King's Wake", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Onrai's Strike"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Onrai's Strike"], Resources.Load("CardZooms/Tyras/Onrai's Strike", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Onslaught of Tyras"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Onslaught of Tyras"], Resources.Load("CardZooms/Tyras/Onslaught of Tyras", typeof(Sprite)) as Sprite);
        }

        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Sundering Star"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Sundering Star"], Resources.Load("CardZooms/Tyras/Sundering Star", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Tyras"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Tyras"], Resources.Load("CardZooms/Tyras/Tyras", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey(CurrCardImages["Warrior's Resolve"]))
        {
            OnClick.CardZooms.Add(CurrCardImages["Warrior's Resolve"], Resources.Load("CardZooms/Tyras/Warrior's Resolve", typeof(Sprite)) as Sprite);
        }
		yield return null;
	}
}
