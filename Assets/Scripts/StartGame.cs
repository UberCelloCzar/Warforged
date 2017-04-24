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

    public static IEnumerator waitForClickorLock()
    {
        yield return new WaitUntil(() => !OnClick.NoReturn.Equals(OnClick.cardReturn) || Game.p1.lockedIn);
        //Debug.Log("Got something");
        if (!OnClick.NoReturn.Equals(OnClick.cardReturn))
        {
            lib.setReturnObject(OnClick.cardReturn);
        }
        else
        {
            lib.setReturnObject(null);
        }
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

    public static IEnumerator resetLock()
    {
        OnClick.LockButton.enabled = true;
        OnClick.LockButton.isOn = false;
        OnClick.LockButtonImage.color = new UnityEngine.Color(0f,149/255f,255/255f);
        OnClick.LockButtonText.text = "Lock In";
        OnClick.OLockButtonImage.color = new UnityEngine.Color(240/255f, 5/255f, 5/255f);
        OnClick.OLockButtonText.text = "Playing...";
        yield return null;
    }

    public static IEnumerator resetIcons()
    {
        if (Game.p1.reinforce > 0)
        {
            GameObject.FindGameObjectWithTag("Reinforce_Icon").GetComponent<Image>().enabled = true;
            GameObject.FindGameObjectWithTag("Reinforce").GetComponent<Text>().enabled = true;
        }
        else
        {
            GameObject.FindGameObjectWithTag("Reinforce_Icon").GetComponent<Image>().enabled = false;
            GameObject.FindGameObjectWithTag("Reinforce").GetComponent<Text>().enabled = false;
        }
        if (Game.p2.reinforce > 0)
        {
            GameObject.FindGameObjectWithTag("OReinforce_Icon").GetComponent<Image>().enabled = true;
            GameObject.FindGameObjectWithTag("OReinforce").GetComponent<Text>().enabled = true;
        }
        else
        {
            GameObject.FindGameObjectWithTag("OReinforce_Icon").GetComponent<Image>().enabled = false;
            GameObject.FindGameObjectWithTag("OReinforce").GetComponent<Text>().enabled = false;
        }
        if (Game.p1.currEmpower > 0)
        {
            GameObject.FindGameObjectWithTag("Empower_Icon").GetComponent<Image>().enabled = true;
            GameObject.FindGameObjectWithTag("Empower").GetComponent<Text>().enabled = true;
        }
        else
        {
            GameObject.FindGameObjectWithTag("Empower_Icon").GetComponent<Image>().enabled = false;
            GameObject.FindGameObjectWithTag("Empower").GetComponent<Text>().enabled = false;
        }
        if (Game.p2.empower > 0)
        {
            GameObject.FindGameObjectWithTag("OEmpower_Icon").GetComponent<Image>().enabled = true;
            GameObject.FindGameObjectWithTag("OEmpower").GetComponent<Text>().enabled = true;
        }
        else
        {
            GameObject.FindGameObjectWithTag("OEmpower_Icon").GetComponent<Image>().enabled = false;
            GameObject.FindGameObjectWithTag("OEmpower").GetComponent<Text>().enabled = false;
        }
        yield return null;
    }

    public static IEnumerator resetDmgUI()
    {
        GameObject.FindGameObjectWithTag("Dmg_Icon").GetComponent<Image>().enabled = false;
        GameObject.FindGameObjectWithTag("ODmg_Icon").GetComponent<Image>().enabled = false;
        yield return null;
    }

    public static IEnumerator resetHealingUI()
    {
        GameObject.FindGameObjectWithTag("Healing_Icon").GetComponent<Image>().enabled = false;
        GameObject.FindGameObjectWithTag("OHealing_Icon").GetComponent<Image>().enabled = false;
        yield return null;
    }

    public static IEnumerator setDmgUI(bool isPlayer1)
    {
        if (isPlayer1)
        {
            GameObject.FindGameObjectWithTag("Dmg_Icon").GetComponent<Image>().enabled = true;
        }
        else
        {
            GameObject.FindGameObjectWithTag("ODmg_Icon").GetComponent<Image>().enabled = true;
        }
        yield return null;
    }
    public static IEnumerator setReflectPromptUI(bool isPlayer1)
    {
        if (isPlayer1)
        {
            GameObject.FindGameObjectWithTag("MadPromptsB").GetComponent<Text>().text = "You Reflect!";
        }
        else
        {
            GameObject.FindGameObjectWithTag("OPrompt").GetComponent<Text>().text = "Opponent Reflected!";
        }
        yield return null;
    }
    public static IEnumerator setNegatePromptUI(bool isPlayer1, int ngt)
    {
        if (isPlayer1)
        {
            GameObject.FindGameObjectWithTag("MadPromptsB").GetComponent<Text>().text = "You Negate " + ngt + " Damage!";
        }
        else
        {
            GameObject.FindGameObjectWithTag("OPrompt").GetComponent<Text>().text = "Opponent Negated " + ngt + " Damage!";
        }
        yield return null;
    }
    public static IEnumerator setSafeguardPromptUI(bool isPlayer1)
    {
        if (isPlayer1)
        {
            GameObject.FindGameObjectWithTag("MadPromptsB").GetComponent<Text>().text = "You Safeguarded!";
        }
        else
        {
            GameObject.FindGameObjectWithTag("OPrompt").GetComponent<Text>().text = "Opponent Safeguarded!";
        }
        yield return null;
    }
    public static IEnumerator setAbsorbPromptUI(bool isPlayer1)
    {
        if (isPlayer1)
        {
            GameObject.FindGameObjectWithTag("MadPromptsB").GetComponent<Text>().text = "You Absorbed Damage!";
        }
        else
        {
            GameObject.FindGameObjectWithTag("OPrompt").GetComponent<Text>().text = "Opponent Absorbed Damage!";
        }
        yield return null;
    }
    public static IEnumerator resetPrompts()
    {
        
        GameObject.FindGameObjectWithTag("MadPromptsB").GetComponent<Text>().text = "";
        
        
        GameObject.FindGameObjectWithTag("OPrompt").GetComponent<Text>().text = "";
        
        yield return null;
    }
    public static IEnumerator setHealingUI(bool isPlayer1)
    {
        if (isPlayer1)
        {
            GameObject.FindGameObjectWithTag("Healing_Icon").GetComponent<Image>().enabled = true;
        }
        else
        {
            GameObject.FindGameObjectWithTag("OHealing_Icon").GetComponent<Image>().enabled = true;
        }
        yield return null;
    }

    public static IEnumerator LockIn(bool isServer)
    {
        //if (isServer == PlayerController.controller.localPlayer.isServer)
        //{
        //    PlayerController.controller.localPlayer.readyFlag2 = true;
        //}
        //else
        //{
        //    PlayerController.controller.remotePlayer.readyFlag2 = true;
        //}
        //OnClick.OLockButtonImage.color = UnityEngine.Color.gray;
        //OnClick.OLockButtonText.text = "Locked In";
        //Debug.Log("Enemy Locked");
        PlayerController.controller.localPlayer.CmdImReady2(isServer);
        yield return null;
    }

    public static IEnumerator updateUI(Character ch, bool showCurrCard)
    {
        for (int i = 0; i < OnClick.Hand.Count; ++i)
        {
            if (ch.hand.Count <= i)
            {
                OnClick.Hand[i].name = "Hand"+i;
                OnClick.Hand[i].sprite = null;
                OnClick.Hand[i].color = new UnityEngine.Color(1.0f, 1.0f, 1.0f, 0.0f);
                OnClick.Hand[i].gameObject.SetActive(false);
            }
            else
            {
                OnClick.Hand[i].name = ch.hand[i].name;
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
                OnClick.Standby[i].name = "Standby"+i;
                OnClick.Standby[i].sprite = null;
                OnClick.Standby[i].color = new UnityEngine.Color(1.0f, 1.0f, 1.0f, 0.33f);
                OnClick.Standby[i].gameObject.SetActive(false);
            }
            else
            {
                OnClick.Standby[i].name = ch.standby[i].name;
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
                OnClick.Invocation[i].name = "Invocation"+i;
                OnClick.Invocation[i].sprite = null;
                OnClick.Invocation[i].color = new UnityEngine.Color(1.0f, 1.0f, 1.0f, 0.33f);
                OnClick.Invocation[i].gameObject.SetActive(false);
            }
            else if (!ch.invocation[i].active)
            {
                OnClick.Invocation[i].name = ch.invocation[i].name;
                OnClick.Invocation[i].sprite = OnClick.CardImages["Back"];
                OnClick.Invocation[i].color = new UnityEngine.Color(1, 1, 1);
                OnClick.Invocation[i].gameObject.SetActive(true);
                OnClick.cardDict["Invocation" + (i + 1)] = OnClick.NoReturn;
            }
            else
            {
                OnClick.Invocation[i].name = ch.invocation[i].name;
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
            OnClick.PlaySlot.name = ch.currCard.name;
            OnClick.PlaySlot.sprite = OnClick.CardImages[ch.currCard.name];
            OnClick.PlaySlot.color = new UnityEngine.Color(1, 1, 1);
            OnClick.PlaySlot.gameObject.SetActive(true);
            OnClick.cardDict["PlaySlot"] = ch.currCard;
        }
        else
        {
            OnClick.PlaySlot.name = ch.currCard.name;
            OnClick.PlaySlot.sprite = OnClick.CardImages["Back"]; // CHANGED
            OnClick.PlaySlot.color = new UnityEngine.Color(1, 1, 1);
            OnClick.PlaySlot.gameObject.SetActive(true);
            OnClick.cardDict["PlaySlot"] = ch.currCard;
        }
        OnClick.CharacterSlot.name = ch.name;
        OnClick.CharacterSlot.sprite = OnClick.CardImages[ch.name];
        OnClick.CharacterSlot.color = new UnityEngine.Color(1, 1, 1);
        OnClick.Health.text = ch.hp + "";
        OnClick.Empower.text = "" + ch.currEmpower + "";
        OnClick.Reinforce.text = "" + ch.currReinforce + "";
        OnClick.Phase.text = ch.displayPhase();
        switch (ch.seal)
        {
            case Warforged.Color.blue:
                OnClick.Seal.sprite = OnClick.SealSprites[0];
                OnClick.Seal.gameObject.SetActive(true);
                break;
            case Warforged.Color.red:
                OnClick.Seal.sprite = OnClick.SealSprites[1];
                OnClick.Seal.gameObject.SetActive(true);
                break;
            case Warforged.Color.green:
                OnClick.Seal.sprite = OnClick.SealSprites[2];
                OnClick.Seal.gameObject.SetActive(true);
                break;
            default:
                OnClick.Seal.sprite = null;
                OnClick.Seal.gameObject.SetActive(false);
                break;
        }
        yield return null;
    }

    public static IEnumerator endSlate(Character ch) // End game UI method
    {
        //Debug.Log("Game Over reached top");
        OnClick.GameOver.text = "Game Over.\n";
		if (ch.hp <= 0 && ch.opponent.hp <= 0)
		{
			OnClick.GameOver.text += "You tied";
		}
        else if (ch.endGame == 1)
        {
            OnClick.GameOver.text += "You win"; // Add the appropriate name to the win text
        }
		else if (ch.endGame == 2)
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
                OnClick.OHand[i].name = "OHand"+i;
                OnClick.OHand[i].sprite = null;
                OnClick.OHand[i].color = new UnityEngine.Color(1.0f, 1.0f, 1.0f, 0.0f);
                OnClick.OHand[i].gameObject.SetActive(false);
            }
            else if (!showHand)
            {
                OnClick.OHand[i].name = "OHand"+i;
                OnClick.OHand[i].sprite = OnClick.OCardImages["Back"];
                OnClick.OHand[i].color = new UnityEngine.Color(1, 1, 1, 1);
                OnClick.OHand[i].gameObject.SetActive(true);
                OnClick.cardDict["OHand" + i] = ch.hand[i];
            }
            else
            {
                OnClick.OHand[i].name = ch.hand[i].name;
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
                OnClick.OStandby[i].name = ch.standby[i].name;
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
                OnClick.OInvocation[i].name = "OInvocation" + i;
                OnClick.OInvocation[i].sprite = null;
                OnClick.OInvocation[i].color = new UnityEngine.Color(1.0f, 1.0f, 1.0f, 0.33f);
                OnClick.OInvocation[i].gameObject.SetActive(false);
            }
            else if (!ch.invocation[i].active)
            {
                OnClick.OInvocation[i].name = "OInvocation"+i;
                OnClick.OInvocation[i].sprite = OnClick.OCardImages["Back"];
                OnClick.OInvocation[i].color = new UnityEngine.Color(1, 1, 1);
                OnClick.OInvocation[i].gameObject.SetActive(true);
                OnClick.cardDict["OInvocation" + (i + 1)] = OnClick.NoReturn;
            }
            else
            {
                OnClick.OInvocation[i].name = ch.invocation[i].name;
                OnClick.OInvocation[i].sprite = OnClick.OCardImages[ch.invocation[i].name];
                OnClick.OInvocation[i].color = new UnityEngine.Color(1, 1, 1);
                OnClick.OInvocation[i].gameObject.SetActive(true);
                OnClick.cardDict["OInvocation" + (i + 1)] = ch.invocation[i];
            }
        }
        if (ch.currCard == null)
        {
            OnClick.OPlaySlot.name = "OPlaySlot";
            OnClick.OPlaySlot.sprite = null;
            OnClick.OPlaySlot.color = new UnityEngine.Color(1.0f, 1.0f, 1.0f, 0.33f);
            OnClick.OPlaySlot.gameObject.SetActive(false);
        }
        else if (showCurrCard)
        {
            OnClick.OPlaySlot.name = ch.currCard.name;
            OnClick.OPlaySlot.sprite = OnClick.OCardImages[ch.currCard.name];
            OnClick.OPlaySlot.color = new UnityEngine.Color(1, 1, 1);
            OnClick.OPlaySlot.gameObject.SetActive(true);
            OnClick.cardDict["OPlaySlot"] = ch.currCard;
        }
        else
        {
            OnClick.OPlaySlot.name = "OPlaySlot";
            OnClick.OPlaySlot.sprite = OnClick.OCardImages["Back"];
            OnClick.OPlaySlot.color = new UnityEngine.Color(1, 1, 1);
            OnClick.OPlaySlot.gameObject.SetActive(true);
            OnClick.cardDict["OPlaySlot"] = ch.currCard;
        }
        OnClick.OCharacterSlot.name = ch.name;
        OnClick.OCharacterSlot.sprite = OnClick.OCardImages[ch.name];
        OnClick.OCharacterSlot.color = new UnityEngine.Color(1, 1, 1);
        OnClick.OHealth.text = ch.hp +"";
        OnClick.OEmpower.text = "" + ch.currEmpower + "";
        OnClick.OReinforce.text = "" + ch.reinforce + "";
        switch (ch.seal)
        {
            case Warforged.Color.blue:
                OnClick.OSeal.sprite = OnClick.SealSprites[0];
                OnClick.OSeal.gameObject.SetActive(true);
                break;
            case Warforged.Color.red:
                OnClick.OSeal.sprite = OnClick.SealSprites[1];
                OnClick.OSeal.gameObject.SetActive(true);
                break;
            case Warforged.Color.green:
                OnClick.OSeal.sprite = OnClick.SealSprites[2];
                OnClick.OSeal.gameObject.SetActive(true);
                break;
            default:
                OnClick.OSeal.sprite = null;
                OnClick.OSeal.gameObject.SetActive(false);
                break;
        }
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

        CurrCardImages.Add("Back", Resources.Load("CardImages/Edros/Card Back (Edros)", typeof(Sprite)) as Sprite);

        if (!OnClick.CardZooms.ContainsKey("Celestial Surge"))
        {
            OnClick.CardZooms.Add("Celestial Surge", Resources.Load("CardZooms/Edros/Celestial Surge", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey("Purging Lightning"))
        {
            OnClick.CardZooms.Add("Purging Lightning", Resources.Load("CardZooms/Edros/Purging Lightning", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey("Crashing Sky"))
        {
            OnClick.CardZooms.Add("Crashing Sky", Resources.Load("CardZooms/Edros/Crashing Sky", typeof(Sprite)) as Sprite);
        }

        if (!OnClick.CardZooms.ContainsKey("Edros"))
        {
            OnClick.CardZooms.Add("Edros", Resources.Load<Sprite>("CardZooms/Edros/Edros"));
        }
        if (!OnClick.CardZooms.ContainsKey("Faith Unquestioned"))
        {
            OnClick.CardZooms.Add("Faith Unquestioned", Resources.Load("CardZooms/Edros/Faith Unquestioned", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey("Grace of Heaven"))
        {
            OnClick.CardZooms.Add("Grace of Heaven", Resources.Load("CardZooms/Edros/Grace of Heaven", typeof(Sprite)) as Sprite);
        }

        if (!OnClick.CardZooms.ContainsKey("Hand of Toren"))
        {
            OnClick.CardZooms.Add("Hand of Toren", Resources.Load("CardZooms/Edros/Hand of Toren", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey("Suppressing Bolt"))
        {
            OnClick.CardZooms.Add("Suppressing Bolt", Resources.Load("CardZooms/Edros/Suppressing Bolt", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey("Rolling Thunder"))
        {
            OnClick.CardZooms.Add("Rolling Thunder", Resources.Load("CardZooms/Edros/Rolling Thunder", typeof(Sprite)) as Sprite);
        }

        if (!OnClick.CardZooms.ContainsKey("Imminent Storm"))
        {
            OnClick.CardZooms.Add("Imminent Storm", Resources.Load("CardZooms/Edros/Imminent Storm", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey("Sky Blessed Shield"))
        {
            OnClick.CardZooms.Add("Sky Blessed Shield", Resources.Load("CardZooms/Edros/Sky Blessed Shield", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey("Toren's Favored"))
        {
            OnClick.CardZooms.Add("Toren's Favored", Resources.Load("CardZooms/Edros/Toren's Favored", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey("Wrath of Lightning"))
        {
            OnClick.CardZooms.Add("Wrath of Lightning", Resources.Load("CardZooms/Edros/Wrath of Lightning", typeof(Sprite)) as Sprite);
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

		if (!OnClick.CardZooms.ContainsKey("Ascendance"))
		{
			OnClick.CardZooms.Add("Ascendance", Resources.Load("CardZooms/Adrius/Ascendance", typeof(Sprite)) as Sprite);
		}
        if (!OnClick.CardZooms.ContainsKey("Divine Cataclysm"))
        {
            OnClick.CardZooms.Add("Divine Cataclysm", Resources.Load("CardZooms/Adrius/Divine Cataclysm", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey("Earth Piercer"))
        {
            OnClick.CardZooms.Add("Earth Piercer", Resources.Load("CardZooms/Adrius/Earth Piercer", typeof(Sprite)) as Sprite);
        }

        if (!OnClick.CardZooms.ContainsKey("Adrius (Ral'Taris Incarnate)"))
        {
            OnClick.CardZooms.Add("Adrius (Ral'Taris Incarnate)", Resources.Load<Sprite>("CardZooms/Adrius/Adrius (Ral'Taris Incarnate)"));
        }
        if (!OnClick.CardZooms.ContainsKey("Adrius (The Aspirer)"))
        {
            OnClick.CardZooms.Add("Adrius (The Aspirer)", Resources.Load<Sprite>("CardZooms/Adrius/Adrius (The Aspirer)"));
        }
        if (!OnClick.CardZooms.ContainsKey("Adrius (The Realm Bearer)"))
        {
            OnClick.CardZooms.Add("Adrius (The Realm Bearer)", Resources.Load<Sprite>("CardZooms/Adrius/Adrius (The Realm Bearer)"));
        }

        if (!OnClick.CardZooms.ContainsKey("Emerald Core"))
        {
            OnClick.CardZooms.Add("Emerald Core", Resources.Load("CardZooms/Adrius/Emerald Core", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey("Fist of Ruin"))
        {
            OnClick.CardZooms.Add("Fist of Ruin", Resources.Load("CardZooms/Adrius/Fist of Ruin", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey("Hero’s Resolution"))
        {
            OnClick.CardZooms.Add("Hero’s Resolution", Resources.Load("CardZooms/Adrius/Hero’s Resolution", typeof(Sprite)) as Sprite);
        }

        if (!OnClick.CardZooms.ContainsKey("Ruby Heart"))
        {
            OnClick.CardZooms.Add("Ruby Heart", Resources.Load("CardZooms/Adrius/Ruby Heart", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey("Sapphire Mantle"))
        {
            OnClick.CardZooms.Add("Sapphire Mantle", Resources.Load("CardZooms/Adrius/Sapphire Mantle", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey("Shattering Blow"))
        {
            OnClick.CardZooms.Add("Shattering Blow", Resources.Load("CardZooms/Adrius/Shattering Blow", typeof(Sprite)) as Sprite);
        }

        if (!OnClick.CardZooms.ContainsKey("Surging Hope"))
        {
            OnClick.CardZooms.Add("Surging Hope", Resources.Load("CardZooms/Adrius/Surging Hope", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey("Tremoring Impact"))
        {
            OnClick.CardZooms.Add("Tremoring Impact", Resources.Load("CardZooms/Adrius/Tremoring Impact", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey("Unyielding Faith"))
        {
            OnClick.CardZooms.Add("Unyielding Faith", Resources.Load("CardZooms/Adrius/Unyielding Faith", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey("Will Unbreakable"))
        {
            OnClick.CardZooms.Add("Will Unbreakable", Resources.Load("CardZooms/Adrius/Will Unbreakable", typeof(Sprite)) as Sprite);
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

        CurrCardImages.Add("Back", Resources.Load("CardImages/Tyras/Card Back (Tyras)", typeof(Sprite)) as Sprite);

        if (!OnClick.CardZooms.ContainsKey("A Brother's Virtue"))
        {
            OnClick.CardZooms.Add("A Brother's Virtue", Resources.Load("CardZooms/Tyras/A Brother's Virtue", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey("A Promise Unbroken"))
        {
            OnClick.CardZooms.Add("A Promise Unbroken", Resources.Load("CardZooms/Tyras/A Promise Unbroken", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey("A Soldier's Remorse"))
        {
            OnClick.CardZooms.Add("A Soldier's Remorse", Resources.Load("CardZooms/Tyras/A Soldier's Remorse", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey("An Oath Unforgotten"))
        {
            OnClick.CardZooms.Add("An Oath Unforgotten", Resources.Load<Sprite>("CardZooms/Tyras/An Oath Unforgotten"));
        }

        if (!OnClick.CardZooms.ContainsKey("Armor of Aldras"))
        {
            OnClick.CardZooms.Add("Armor of Aldras", Resources.Load("CardZooms/Tyras/Armor of Aldras", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey("Decrying Roar"))
        {
            OnClick.CardZooms.Add("Decrying Roar", Resources.Load("CardZooms/Tyras/Decrying Roar", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey("Grim Knight's Dread"))
        {
            OnClick.CardZooms.Add("Grim Knight's Dread", Resources.Load("CardZooms/Tyras/Grim Knight's Dread", typeof(Sprite)) as Sprite);
        }

        if (!OnClick.CardZooms.ContainsKey("In the King's Wake"))
        {
            OnClick.CardZooms.Add("In the King's Wake", Resources.Load("CardZooms/Tyras/In the King's Wake", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey("Onrai's Strike"))
        {
            OnClick.CardZooms.Add("Onrai's Strike", Resources.Load("CardZooms/Tyras/Onrai's Strike", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey("Onslaught of Tyras"))
        {
            OnClick.CardZooms.Add("Onslaught of Tyras", Resources.Load("CardZooms/Tyras/Onslaught of Tyras", typeof(Sprite)) as Sprite);
        }

        if (!OnClick.CardZooms.ContainsKey("Sundering Star"))
        {
            OnClick.CardZooms.Add("Sundering Star", Resources.Load("CardZooms/Tyras/Sundering Star", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey("Tyras"))
        {
            OnClick.CardZooms.Add("Tyras", Resources.Load("CardZooms/Tyras/Tyras", typeof(Sprite)) as Sprite);
        }
        if (!OnClick.CardZooms.ContainsKey("Warrior's Resolve"))
        {
            OnClick.CardZooms.Add("Warrior's Resolve", Resources.Load("CardZooms/Tyras/Warrior's Resolve", typeof(Sprite)) as Sprite);
        }
		yield return null;
	}

	public static IEnumerator setupAurian(Dictionary<string, Sprite> CurrCardImages)
    {
        CurrCardImages.Add("Aurian", Resources.Load("CardImages/Aurian/Aurian", typeof(Sprite)) as Sprite);

		CurrCardImages.Add("Absolute Focus", Resources.Load("CardImages/Aurian/Absolute Focus", typeof(Sprite)) as Sprite);
		CurrCardImages.Add("Body and Soul", Resources.Load("CardImages/Aurian/Body and Soul", typeof(Sprite)) as Sprite);
		CurrCardImages.Add("Clarity of Mind", Resources.Load<Sprite>("CardImages/Aurian/Clarity of Mind"));

		CurrCardImages.Add("Countering Strike", Resources.Load("CardImages/Aurian/Countering Strike", typeof(Sprite)) as Sprite);
		CurrCardImages.Add("Ethereal Strike", Resources.Load("CardImages/Aurian/Ethereal Strike", typeof(Sprite)) as Sprite);
		CurrCardImages.Add("Evading Step", Resources.Load("CardImages/Aurian/Evading Step", typeof(Sprite)) as Sprite);

		CurrCardImages.Add("Opening Blow", Resources.Load("CardImages/Aurian/Opening Blow", typeof(Sprite)) as Sprite);
		CurrCardImages.Add("Primed Attack", Resources.Load("CardImages/Aurian/Primed Attack", typeof(Sprite)) as Sprite);
		CurrCardImages.Add("Relentless Assault", Resources.Load("CardImages/Aurian/Relentless Assault", typeof(Sprite)) as Sprite);

		CurrCardImages.Add("Soul Strike", Resources.Load("CardImages/Aurian/Soul Strike", typeof(Sprite)) as Sprite);
		CurrCardImages.Add("Strength of Spirit", Resources.Load("CardImages/Aurian/Strength of Spirit", typeof(Sprite)) as Sprite);
		CurrCardImages.Add("Wrath of Era", Resources.Load("CardImages/Aurian/Wrath of Era", typeof(Sprite)) as Sprite);

        CurrCardImages.Add("Back", Resources.Load("CardImages/Aurian/Card Back (Aurian)", typeof(Sprite)) as Sprite);

        if (!OnClick.CardZooms.ContainsKey("Absolute Focus"))
		{
			OnClick.CardZooms.Add("Absolute Focus", Resources.Load("CardZooms/Aurian/Absolute Focus", typeof(Sprite)) as Sprite);
		}
		if (!OnClick.CardZooms.ContainsKey("Body and Soul"))
		{
			OnClick.CardZooms.Add("Body and Soul", Resources.Load("CardZooms/Aurian/Body and Soul", typeof(Sprite)) as Sprite);
		}
		if (!OnClick.CardZooms.ContainsKey("Clarity of Mind"))
		{
			OnClick.CardZooms.Add("Clarity of Mind", Resources.Load("CardZooms/Aurian/Clarity of Mind", typeof(Sprite)) as Sprite);
		}
		if (!OnClick.CardZooms.ContainsKey("Countering Strike"))
		{
			OnClick.CardZooms.Add("Countering Strike", Resources.Load<Sprite>("CardZooms/Aurian/Countering Strike"));
		}

		if (!OnClick.CardZooms.ContainsKey("Ethereal Strike"))
		{
			OnClick.CardZooms.Add("Ethereal Strike", Resources.Load("CardZooms/Aurian/Ethereal Strike", typeof(Sprite)) as Sprite);
		}
		if (!OnClick.CardZooms.ContainsKey("Evading Step"))
		{
			OnClick.CardZooms.Add("Evading Step", Resources.Load("CardZooms/Aurian/Evading Step", typeof(Sprite)) as Sprite);
		}
		if (!OnClick.CardZooms.ContainsKey("Opening Blow"))
		{
			OnClick.CardZooms.Add("Opening Blow", Resources.Load("CardZooms/Aurian/Opening Blow", typeof(Sprite)) as Sprite);
		}

		if (!OnClick.CardZooms.ContainsKey("Primed Attack"))
		{
			OnClick.CardZooms.Add("Primed Attack", Resources.Load("CardZooms/Aurian/Primed Attack", typeof(Sprite)) as Sprite);
		}
		if (!OnClick.CardZooms.ContainsKey("Relentless Assault"))
		{
			OnClick.CardZooms.Add("Relentless Assault", Resources.Load("CardZooms/Aurian/Relentless Assault", typeof(Sprite)) as Sprite);
		}
		if (!OnClick.CardZooms.ContainsKey("Soul Strike"))
		{
			OnClick.CardZooms.Add("Soul Strike", Resources.Load("CardZooms/Aurian/Soul Strike", typeof(Sprite)) as Sprite);
		}

		if (!OnClick.CardZooms.ContainsKey("Strength of Spirit"))
		{
			OnClick.CardZooms.Add("Strength of Spirit", Resources.Load("CardZooms/Aurian/Strength of Spirit", typeof(Sprite)) as Sprite);
		}
		if (!OnClick.CardZooms.ContainsKey("Aurian"))
		{
			OnClick.CardZooms.Add("Aurian", Resources.Load("CardZooms/Aurian/Aurian", typeof(Sprite)) as Sprite);
		}
		if (!OnClick.CardZooms.ContainsKey("Wrath of Era"))
		{
			OnClick.CardZooms.Add("Wrath of Era", Resources.Load("CardZooms/Aurian/Wrath of Era", typeof(Sprite)) as Sprite);
		}
		yield return null;
	}

    public static IEnumerator setupSeals(List<Sprite> seals)
    {
        seals.Add(Resources.Load("seal_symbol_blue", typeof(Sprite)) as Sprite);
        seals.Add(Resources.Load("seal_symbol_red", typeof(Sprite)) as Sprite);
        seals.Add(Resources.Load("seal_symbol_green", typeof(Sprite)) as Sprite);
        yield return null;
    }
}
