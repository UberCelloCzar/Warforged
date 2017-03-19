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
	public static IEnumerator multiPrompt(string text, List<string> buttonTexts,List<object> returns)
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
        yield return new WaitUntil(() => { Debug.Log("Turn 1: "+ Game.p1.turn+" Turn 2: "+ Game.p2.turn); return networkUpdated; });
        networkUpdated = false;
    }
    public static IEnumerator updateNetwork(Character ch)
    {
        XmlSerializer xml = new XmlSerializer(typeof(Character));
        StringWriter sw = new StringWriter();
        //var xwrite = XmlWriter.Create(sw);
        xml.Serialize(sw, ch);
        if (PlayerController.controller.localPlayer.isServer)
        {
            PlayerController.controller.localPlayer.CmdSetCharacter(sw.GetStringBuilder().ToString(), PlayerController.controller.localPlayer.isServer);
        }
        else
        {
            PlayerController.controller.localPlayer.CmdSetCharacter(sw.GetStringBuilder().ToString(), PlayerController.controller.localPlayer.isServer);
        }
        /*if (!PlayerController.controller.localPlayer.isServer)
        {
            //yield return new WaitUntil(() => PlayerController.controller.remotePlayer.remoteSequence + 1 >= PlayerController.controller.remotePlayer.localSequence);
            Debug.Log("ClientReady: "+ PlayerController.playerController.getClientReady()+" ServerReady: "+ PlayerController.playerController.getServerReady());
            PlayerController.controller.localPlayer.CmdClientReady(PlayerController.playerController.getClientReady() + 1);
            yield return new WaitUntil(() => {
                var rdy = PlayerController.playerController.getServerReady() > PlayerController.controller.localPlayer.prevServerReady+3;
                if(rdy)
                {
                    PlayerController.controller.localPlayer.CmdClientReady(PlayerController.playerController.getClientReady() + 6);
                    PlayerController.controller.localPlayer.prevServerReady = PlayerController.playerController.getServerReady();
                    return true;
                }
                else
                {
                    PlayerController.controller.localPlayer.CmdClientReady(PlayerController.playerController.getClientReady()+1);
                    Debug.Log("ClientReady: " + PlayerController.playerController.getClientReady() + " ServerReady: " + PlayerController.playerController.getServerReady());
                }
                return false;
            });
            //yield return new WaitUntil(() => PlayerController.controller.remotePlayer.readyFlag == false);
        }
        else
        {
            //yield return new WaitUntil(() => PlayerController.controller.remotePlayer.remoteSequence + 1 >= PlayerController.controller.remotePlayer.localSequence);
            yield return new WaitUntil(() => {
                var rdy = PlayerController.playerController.getClientReady() > PlayerController.controller.localPlayer.prevClientReady+5;
                if (rdy)
                {
                    PlayerController.controller.localPlayer.prevClientReady = PlayerController.playerController.getClientReady();
                    PlayerController.playerController.updateServerReady(PlayerController.playerController.getServerReady()+4);
                    Debug.Log("ClientReady: " + PlayerController.playerController.getClientReady() + " ServerReady: " + PlayerController.playerController.getServerReady());
                    return true;
                }
                else
                {
                    PlayerController.playerController.updateServerReady(PlayerController.playerController.getServerReady() + 1);
                    Debug.Log("ClientReady: " + PlayerController.playerController.getClientReady() + " ServerReady: " + PlayerController.playerController.getServerReady());
                }
                return false;
            });
            //yield return new WaitUntil(() => PlayerController.controller.remotePlayer.readyFlag == false);
        }*/
        //PlayerController.controller.localPlayer.CmdImReady(PlayerController.controller.localPlayer.isServer);
        ////if (!PlayerController.controller.remotePlayer.readyFlag)
        //{
        //    yield return new WaitUntil(() => {
        //        PlayerController.controller.localPlayer.CmdImReady(PlayerController.controller.localPlayer.isServer);
        //        PlayerController.controller.localPlayer.CmdImReady1(PlayerController.controller.localPlayer.isServer);
        //        return PlayerController.controller.remotePlayer.readyFlag;
        //    });
        //    /*yield return new WaitUntil(() => {
        //        PlayerController.controller.localPlayer.CmdImReady(PlayerController.controller.localPlayer.isServer);
        //        PlayerController.controller.localPlayer.CmdImReady1(PlayerController.controller.localPlayer.isServer);
        //        return PlayerController.controller.localPlayer.readyFlag;
        //    });*/
        //}
        //PlayerController.controller.remotePlayer.readyFlag = false;
        //PlayerController.controller.localPlayer.readyFlag = false;
        //PlayerController.controller.localPlayer.CmdImReady1(PlayerController.controller.localPlayer.isServer);
        ////if (!PlayerController.controller.remotePlayer.readyFlag)
        //{
        //    yield return new WaitUntil(() => {
        //        PlayerController.controller.localPlayer.CmdImReady1(PlayerController.controller.localPlayer.isServer);
        //        PlayerController.controller.localPlayer.CmdImReady2(PlayerController.controller.localPlayer.isServer);
        //        return PlayerController.controller.remotePlayer.readyFlag1;
        //    });
        //    /*yield return new WaitUntil(() => {
        //        PlayerController.controller.localPlayer.CmdImReady1(PlayerController.controller.localPlayer.isServer);
        //        PlayerController.controller.localPlayer.CmdImReady2(PlayerController.controller.localPlayer.isServer);
        //        return PlayerController.controller.localPlayer.readyFlag1;
        //    });*/
        //}
        //PlayerController.controller.remotePlayer.readyFlag1 = false;
        //PlayerController.controller.localPlayer.readyFlag1 = false;
        //PlayerController.controller.localPlayer.CmdImReady2(PlayerController.controller.localPlayer.isServer);
        ////if (!PlayerController.controller.remotePlayer.readyFlag)
        //{
        //    yield return new WaitUntil(() => {
        //        PlayerController.controller.localPlayer.CmdImReady2(PlayerController.controller.localPlayer.isServer);
        //        PlayerController.controller.localPlayer.CmdImReady(PlayerController.controller.localPlayer.isServer);
        //        return PlayerController.controller.remotePlayer.readyFlag2;
        //    });
        //    /*yield return new WaitUntil(() => {
        //        PlayerController.controller.localPlayer.CmdImReady2(PlayerController.controller.localPlayer.isServer);
        //        PlayerController.controller.localPlayer.CmdImReady(PlayerController.controller.localPlayer.isServer);
        //        return PlayerController.controller.localPlayer.readyFlag2;
        //    });*/
        //}

        //PlayerController.controller.remotePlayer.readyFlag2 = false;
        //PlayerController.controller.localPlayer.readyFlag2 = false;
        //PlayerController.controller.localPlayer.CmdUnReady(PlayerController.controller.localPlayer.isServer);
        //yield return new WaitUntil(() => PlayerController.controller.remotePlayer.readyFlag == false);
        //yield return new WaitUntil(() => PlayerController.controller.localPlayer.readyFlag == false);
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
                OnClick.Standby[i].color = new UnityEngine.Color(1, 1, 1);
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
                OnClick.Invocation[i].sprite = null;
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
            OnClick.PlaySlot.sprite = null;
            OnClick.PlaySlot.color = new UnityEngine.Color(0, 0, 0);
            OnClick.PlaySlot.gameObject.SetActive(true);
            OnClick.cardDict["PlaySlot"] = ch.currCard;
        }
        OnClick.CharacterSlot.sprite = OnClick.CardImages[ch.name];
        OnClick.CharacterSlot.color = new UnityEngine.Color(1, 1, 1);
        OnClick.Health.text = ch.hp + "HP";
        OnClick.Empower.text = "EMP(" + ch.empower + ")";
        OnClick.Reinforce.text = "REI(" + ch.reinforce + ")";
        yield return null;
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
        OnClick.OEmpower.text = "EMP(" + ch.empower + ")";
        OnClick.OReinforce.text = "REI(" + ch.reinforce + ")";
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

		CurrCardImages.Add("Pillar of Lightning", Resources.Load("CardImages/Edros/Pillar of Lightning", typeof(Sprite)) as Sprite);
		CurrCardImages.Add("Rolling Thunder", Resources.Load("CardImages/Edros/Rolling Thunder", typeof(Sprite)) as Sprite);
		CurrCardImages.Add("Scorn of Thunder", Resources.Load("CardImages/Edros/Scorn of Thunder", typeof(Sprite)) as Sprite);

		CurrCardImages.Add("Sky Blessed Shield", Resources.Load("CardImages/Edros/Sky Blessed Shield", typeof(Sprite)) as Sprite);
		CurrCardImages.Add("Toren's Favored", Resources.Load("CardImages/Edros/Toren's Favored", typeof(Sprite)) as Sprite);
		CurrCardImages.Add("Wrath of Lightning", Resources.Load("CardImages/Edros/Wrath of Lightning", typeof(Sprite)) as Sprite);
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
		yield return null;
	}
}
