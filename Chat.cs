//The class does not contain a full implementation of components, just an example
//Example of implementing a multiplayer chat on the library Steamworks

using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Chat : SingletonBehaviour<Chat>
{
    public ScrollRect ChatScroll;
    public GameObject ChatContent;
    public TMP_InputField TextInput;
    public GameObject MessagePrefab;
    public float AutoScrollStart = 0.05f;

    private protected List<GameObject> ChatCells = new List<GameObject>();

    private protected void Start()
    {
        ChatScroll.verticalNormalizedPosition = 0;
    }

    public void ReceiveNewMessageFromAnotherPlayer(string message)
    {
        DisplayNewMessage(message);
    }

    public void EnterMessage(string message)
    {
        if (message.Length == 0)
            return;
            
        TextInput.text = "";

        SteamLobby.Instance.Lobby.SendChatString(message);
    }

    public void DisplayNewMessage(string newMessage)
    {
        GameObject newMessageView =
            PoolManager.Instantiate(MessagePrefab, MessagePrefab.transform.position, MessagePrefab.transform.rotation);

        newMessageView.transform.SetParent(ChatContent.transform);
        newMessageView.transform.position = MessagePrefab.transform.position;
        newMessageView.transform.localScale = MessagePrefab.transform.localScale;
        newMessageView.GetComponent<TextMeshProUGUI>().text = newMessage;

        CheckOvercrowdingOfChat();
        ChatCells.Add(newMessageView);

        AutoScroll();
    }

    private protected void CheckOvercrowdingOfChat()
    {
        if (ChatCells.Count >= 10)
        {
            PoolManager.Destroy(ChatCells[0]);
            ChatCells.Remove(ChatCells[0]);
        }
    }

    private protected void AutoScroll()
    {
        var savedScrollPos = ChatScroll.verticalNormalizedPosition;
        Canvas.ForceUpdateCanvases();

        if (AutoScrollStart > ChatScroll.verticalNormalizedPosition)
        {
            ChatScroll.verticalNormalizedPosition = 0;
        }
        else
        {
            ChatScroll.verticalNormalizedPosition = savedScrollPos;
        }
    }
}
