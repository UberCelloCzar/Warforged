using UnityEngine;
using System.Threading;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Warforged;

public class OnClick : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public static MatchController controller = null;// Match Controller reference
    public static List<Button> allButtons;
    public static object buttonReturn = NoReturn;
    public static object cardReturn = NoReturn;
    public static Text Prompt = null;
    public static Text Health = null;
    public static Text OHealth = null;
    public static Text Empower = null;
    public static Text Reinforce = null;
    public static Text OEmpower = null;
    public static Text OReinforce = null;
	public static Dictionary<string, object> cardDict = null;
	public static Dictionary<string, Sprite> CardImages = new Dictionary<string, Sprite>();
	public static Dictionary<string, Sprite> CardZooms = new Dictionary<string, Sprite>();
    static List<string> cardTags = new List<string>() {"Invocation1","Invocation2","Invocation3","Invocation4",
            "Hand1", "Hand2", "Hand3" , "Hand4" , "Hand5" ,"Hand6" ,"Hand7" ,"Hand8" ,"Hand9" ,"Hand0",
            "Standby1","Standby2","Standby3","Standby4",
            "Suspend1","Suspend2","Suspend3","Suspend4","Suspend5","Suspend6","Suspend7","Suspend8","Suspend9","Suspend0",
            "Link1_1","Link1_2","Link1_3","Link1_4","Link1_5","Link1_6","Link1_7","Link1_8",
            "Link2_1","Link2_2","Link2_3","Link2_4","Link2_5","Link2_6","Link2_7","Link2_8",
            "Link3_1","Link3_2","Link3_3","Link3_4","Link3_5","Link3_6","Link3_7","Link3_8",
            "Link4_1","Link4_2","Link4_3","Link4_4","Link4_5","Link4_6","Link4_7","Link4_8",
            "CharacterSlot","PlaySlot"};


    public static Dictionary<string, Sprite> OCardImages = new Dictionary<string, Sprite>();
    static List<string> OcardTags = new List<string>() {"OInvocation1","OInvocation2","OInvocation3","OInvocation4",
            "OHand1", "OHand2", "OHand3" , "OHand4" , "OHand5" ,"OHand6" ,"OHand7" ,"OHand8" ,"OHand9" ,"OHand0",
            "OStandby1","OStandby2","OStandby3","OStandby4",
            "OSuspend1","OSuspend2","OSuspend3","OSuspend4","OSuspend5","OSuspend6","OSuspend7","OSuspend8","OSuspend9","OSuspend0",
            "OLink1_1","OLink1_2","OLink1_3","OLink1_4","OLink1_5","OLink1_6","OLink1_7","OLink1_8",
            "OLink2_1","OLink2_2","OLink2_3","OLink2_4","OLink2_5","OLink2_6","OLink2_7","OLink2_8",
            "OLink3_1","OLink3_2","OLink3_3","OLink3_4","OLink3_5","OLink3_6","OLink3_7","OLink3_8",
            "OLink4_1","OLink4_2","OLink4_3","OLink4_4","OLink4_5","OLink4_6","OLink4_7","OLink4_8",
            "OCharacterSlot","OPlaySlot"};
    static List<string> buttonTags = new List<string>() { "Choice1", "Choice2", "Choice3", "Choice4", "Choice5", "Choice6" };
    public static List<Sprite> SealSprites = new List<Sprite>();
    static Dictionary<string, object> buttonDict = null;
    private Image im;
    private Button button;
    public static Image RightZoom = null;
    public static Image LeftZoom = null;

    public static Image CharacterSlot = null;
    public static Image PlaySlot = null;
    public static List<Image> Standby = new List<Image>();
    public static List<Image> Hand = new List<Image>();
    public static List<Image> Invocation = new List<Image>();
    public static List<Image> Suspend = new List<Image>();
    public static Image Seal = null;
    public static Toggle LockButton;
    public static Image LockButtonImage;
    public static Text LockButtonText;

    public static Image OCharacterSlot = null;
    public static Image OPlaySlot = null;
    public static List<Image> OStandby = new List<Image>();
    public static List<Image> OHand = new List<Image>();
    public static List<Image> OInvocation = new List<Image>();
    public static List<Image> OSuspend = new List<Image>();
    public static Image OSeal = null;
    public static object NoReturn = new object();
    public static Text GameOver = null;
    public static Text Phase = null;
    public static Image OLockButtonImage;
    public static Text OLockButtonText;

    // Use this for initialization
    void Start ()
    {
        if (controller == null)
        {
            controller = GameObject.Find("Match Controller").GetComponent<MatchController>(); // Sets match controller reference.
        }
        //Thread t = new Thread(()=>new Game());
        im = gameObject.GetComponent<Image>();
        button = gameObject.GetComponent<Button>();
        if (cardDict == null)
        {
            cardDict = new Dictionary<string, object>();
            foreach(string t in cardTags)
            {
                cardDict.Add(t, NoReturn);
            }
            foreach (string t in OcardTags)
            {
                cardDict.Add(t, NoReturn);
            }

        }
        if(buttonDict == null)
        {
            buttonDict = new Dictionary<string, object>();
            allButtons = new List<Button>(FindObjectsOfType<Button>());
            allButtons.Sort((x,y) => x.tag.CompareTo(y.tag));
            foreach (string t in buttonTags)
            {
                buttonDict.Add(t, NoReturn);
            }
            for (int i = 0; i < 6; ++i)
            {
                allButtons[i].gameObject.SetActive(false);
                allButtons[i].GetComponentInChildren<Text>().text = i.ToString();
            }
        }
        if(Prompt == null)
        {
            Prompt = gameObject.GetComponent<Text>();
            if(Prompt != null &&!Prompt.tag.Equals("Prompt"))
            {
                Prompt = null;
            }
        }
        if (OHealth == null)
        {
            OHealth = gameObject.GetComponent<Text>();
            if (OHealth != null && !OHealth.tag.Equals("OHealth"))
            {
                OHealth = null;
            }
        }

        if (Health == null)
        {
            Health = gameObject.GetComponent<Text>();
            if (Health != null && !Health.tag.Equals("Health"))
            {
                Health = null;
            }
        }

        if (GameOver == null)
        {
            GameOver = gameObject.GetComponent<Text>();
            if (GameOver != null && !GameOver.tag.Equals("GameOver"))
            {
                GameOver = null;
            }
        }

        if (Phase == null)
        {
            Phase = gameObject.GetComponent<Text>();
            if (Phase != null && !Phase.tag.Equals("Phase"))
            {
                Phase = null;
            }
        }

        if (Reinforce == null)
        {
            Reinforce = gameObject.GetComponent<Text>();
            if (Reinforce != null && !Reinforce.tag.Equals("Reinforce"))
            {
                Reinforce = null;
            }
        }

        if (OReinforce == null)
        {
            OReinforce = gameObject.GetComponent<Text>();
            if (OReinforce != null && !OReinforce.tag.Equals("OReinforce"))
            {
                OReinforce = null;
            }
        }

        if (Empower == null)
        {
            Empower = gameObject.GetComponent<Text>();
            if (Empower != null && !Empower.tag.Equals("Empower"))
            {
                Empower = null;
            }
        }

        if (OEmpower == null)
        {
            OEmpower = gameObject.GetComponent<Text>();
            if (OEmpower != null && !OEmpower.tag.Equals("OEmpower"))
            {
                OEmpower = null;
            }
        }

        if (LockButton == null)
        {
            LockButton = gameObject.GetComponent<Toggle>();
            if (LockButton != null && !LockButton.tag.Equals("LockButton"))
            {
                LockButton = null;
            }
            else
            {
                LockButtonText = gameObject.GetComponentInChildren<Text>();
                LockButtonImage = gameObject.GetComponentInChildren<Image>();
            }
        }

        if (OLockButtonImage == null)
        {
            if (gameObject.tag == "OLockButton")
            {
                OLockButtonImage = gameObject.GetComponentInChildren<Image>();
                OLockButtonText = gameObject.GetComponentInChildren<Text>();
            }
        }

        if (im == null)
        {
            return;
        }

        if (im.tag.Equals("OSeal"))
        {
            OSeal = im;
            OnClick.OSeal.gameObject.SetActive(false);
        }
        else if (im.tag.Equals("Seal"))
        {
            Seal = im;
            OnClick.Seal.gameObject.SetActive(false);
        }
        else if (im.tag.StartsWith("OHand"))
        {
            OHand.Add(im);
            OHand.Sort((x, y) => x.tag.CompareTo(y.tag));
        }
        else if (im.tag.StartsWith("OInvocation"))
        {
            OInvocation.Add(im);
            OInvocation.Sort((x, y) => x.tag.CompareTo(y.tag));
        }
        else if (im.tag.StartsWith("OSuspend"))
        {
            OSuspend.Add(im);
            OSuspend.Sort((x, y) => x.tag.CompareTo(y.tag));
        }
        else if (im.tag.Contains("OStandby"))
        {
            OStandby.Add(im);
            OStandby.Sort((x, y) => x.tag.CompareTo(y.tag));
        }
        else if (im.tag.Equals("OPlaySlot"))
        {
            OPlaySlot = im;
        }
        else if (im.tag.Equals("OCharacterSlot"))
        {
            OCharacterSlot = im;
        }
        else if(im.tag.StartsWith("Hand"))
        {
            Hand.Add(im);
            Hand.Sort((x,y) => x.tag.CompareTo(y.tag));
        }
        else if (im.tag.StartsWith("Invocation"))
        {
            Invocation.Add(im);
            Invocation.Sort((x, y) => x.tag.CompareTo(y.tag));
        }
        else if (im.tag.StartsWith("Suspend"))
        {
            Suspend.Add(im);
            Suspend.Sort((x, y) => x.tag.CompareTo(y.tag));
        }
        else if(im.tag.Contains("Standby"))
        {
            Standby.Add(im);
            Standby.Sort((x, y) => x.tag.CompareTo(y.tag));
        }
        else if (im.tag.Equals("PlaySlot"))
        {
            PlaySlot = im;
        }
        else if (im.tag.Equals("CharacterSlot"))
        {
            CharacterSlot = im;
        }
        else if(im.tag.Equals("LeftZoom"))
        {
            LeftZoom = im;
        }
        
    }
	
	// Update is called once per frame
	void Update ()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (cardTags.Contains(eventData.pointerCurrentRaycast.gameObject.tag))
        {
            cardReturn = cardDict[eventData.pointerCurrentRaycast.gameObject.tag];
        }
        else if (im != null && buttonTags.Contains(im.tag))
        {
            //Do button clicky stuff
            buttonReturn = buttonDict[button.tag];
            for (int i = 0; i < 6; ++i)
            {
                allButtons[i].gameObject.SetActive(false);
                allButtons[i].GetComponentInChildren<Text>().text = "";
            }
        }
        else if (LockButton.enabled && gameObject.tag == "LockButton")
        {
            if (Game.p1.currCard != null)
            {
                Prompt.text = "";
                Game.p1.lockedIn = true;
                LockButton.enabled = false;
                LockButtonImage.color = new UnityEngine.Color(3/255f, 69/255f, 116/255f);
                LockButtonText.text = "Locked In";
            }
            else
            {
                Prompt.text = "You can't lock in without playing a card!";
            }
        }
    }

    public static void setButtonOptions(string promptText, List<string> names, List<object> returns)
    {
        Prompt.text = promptText;
        for (int i = 0; i < Math.Min(Math.Min(names.Count, 6), returns.Count); ++i)
        {
            allButtons[i].gameObject.SetActive(true);
            allButtons[i].GetComponentInChildren<Text>().text = names[i];
            buttonDict[allButtons[i].tag] = returns[i];
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (cardTags.Contains(eventData.pointerCurrentRaycast.gameObject.tag) || OcardTags.Contains(eventData.pointerCurrentRaycast.gameObject.tag))
        {
            var img = eventData.pointerEnter.GetComponent<Image>();
            if (img != null && LeftZoom != null && img.sprite != null)
            {
                if (img.tag.StartsWith("O") && (img.name.StartsWith("OInv") || img.name.StartsWith("OHand") || img.name.StartsWith("OPlay")))
                {
                    return;
                }
                else
                {
                    LeftZoom.sprite = CardZooms[img.name];
                    LeftZoom.color = new UnityEngine.Color(1, 1, 1, 1);
                }
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (cardTags.Contains(eventData.pointerEnter.gameObject.tag) || OcardTags.Contains(eventData.pointerEnter.gameObject.tag))
        {
            var img = eventData.pointerEnter.GetComponent<Image>();
            if (img != null && LeftZoom != null)
            {
                LeftZoom.color = new UnityEngine.Color(1, 1, 1, 0);
            }
        }
    }
}
