//The class does not contain a full implementation of components, just an example
//GuiController implementation

using Steamworks;
using Steamworks.Data;
using Steamworks.Extensions.Lobby;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GuiController : GuiControllerExtension
{

    [Header("Main Menu")]
    public GameObject MainMenu;

    [Header("Creating Game")]
    public GameObject CreatingGame;
    public GameObject RunGame;

    [Header("Players slots")]
    public PlayerInfo[] Players;

    [Header("Search And Connect")]
    public GameObject SearchAndConnect;
    public Toggle SortCheckmark;

    #region MainMenu
    
    public void Exit()
    {
        Application.Quit();
    }

    public void ShowWindowStartScreen()
    {
        ShowWindow(Windows.StartScreen);
    }

    public void ShowWindowJoin()
    {
        ShowWindow(Windows.SearchAndConnect);
    }
    
    #endregion MainMenu

    public void ShowWindow(Windows windows)
    {
        LoadHandlerBehaviour.Instance.ActivateFadeOutEffect();

        switch (windows)
        {
            case Windows.StartScreen:
                ActivateCameraEffect();
                CreatingGame.SetActive(false);
                SearchAndConnect.SetActive(false);
                MainMenu.SetActive(true);
                break;
            case Windows.Lobby:
                ActivateCameraEffect();
                CreatingGame.SetActive(true);
                SearchAndConnect.SetActive(false);
                MainMenu.SetActive(false);
                break;
            case Windows.SearchAndConnect:
                ActivateCameraEffect();
                CreatingGame.SetActive(false);
                SearchAndConnect.SetActive(true);
                MainMenu.SetActive(false);
                break;
        }
    }

    public void RefreshSearchList()
    {
        var descendingSortList = SteamLobby.Instance.FoundLobbies.OrderByDescending(room => room.MemberCount);
        CellSpawn(descendingSortList);
    }

    public async void SortByName(bool value)
    {
        if (!value)
        {
            RefreshSearchList();
            return;
        }

        var LobbyList = await SteamMatchmaking.LobbyList
                                   .RequestAsync();

        IEnumerable<Lobby> sortedList = LobbyList.OrderByDescending(room => room.GetNameLobby());

        if (sortedList.Count() <= 0)
            return;

        CellSpawn(sortedList);
    }

    public void EnterRoomAsClient(SteamId steamId)
    {
        SteamLobby.Instance.LoginToLobby(steamId);
    }

    public void ClearSlotsPlayers()
    {
        foreach (var slot in Players)
        {
            slot.ClearInfo();
        }
    }

    public void GrantRightsOfRoomCreator()
    {
        if (SteamLobby.Instance.Lobby.Owner.IsMe)
        {
            RunGame.SetActive(true);
            PlayersLimit.enabled = true;
            PrivateGame.enabled = true;
        }
        else
        {
            RunGame.SetActive(false);
            PlayersLimit.enabled = false;
            PrivateGame.enabled = false;
        }
    }
}
