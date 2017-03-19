using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MessageBoard : MonoBehaviour {

    public Text text;
    public InputField inputField;

    public ArrayList messages = new ArrayList();
    private const int MAX_MESSAGES = 10;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && inputField.text != "")
        {
            MatchController controller = GameObject.Find("Match Controller").GetComponent<MatchController>();

            controller.localPlayer.CmdSendMessage(controller.localPlayer.playerName + ": " + inputField.text);

            inputField.text = "";
        }
    }
    

    public void updateMessageBox(string msg)
    {
        // Adds message to chatlog for players once connected..

        // todo: Fix formmating so that the chat messages look better
        // todo: Add time message was sent


        messages.Add(msg);
            
        if (messages.Count > MAX_MESSAGES)
        {
            messages.Remove(messages[0]);
        }
        text.text = "";
        for(int x = 0; x < messages.Count; x++)
        {
            text.text += messages[x];
            if(x != 9)
                text.text += "\n\n";
        }
    }
}
