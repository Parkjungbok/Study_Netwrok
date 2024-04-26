using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public class RoomPanel : MonoBehaviour
{
    [SerializeField] RectTransform playerContent;
    [SerializeField] PlayerEntry playerEntryPrefab;
    [SerializeField] Button startButton;

    private List<PlayerEntry> playerList;
    private void Awake()
    {
        playerList = new List<PlayerEntry>();
    }

    private void OnEnable()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            PlayerEntry playerEntry = Instantiate(playerEntryPrefab, playerContent);
            playerEntry.SetPlayer(player);
            playerList.Add(playerEntry);
        }
        startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        PhotonNetwork.LocalPlayer.SetReady(false);
        PhotonNetwork.LocalPlayer.SetLoad(false);
        AllPlayerReadyCheck();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void OnDisable()
    {
        for (int i = 0; i < playerContent.childCount; i++)
        {
            Destroy(playerContent.GetChild(i).gameObject);
        }
        playerList.Clear();
        PhotonNetwork.AutomaticallySyncScene = false;
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel("GameSccene");
    }

    public void PlayerEnterRoom(Player newplayer)
    {
        PlayerEntry playerEntry = Instantiate(playerEntryPrefab, playerContent);
        playerEntry.SetPlayer(newplayer);
        playerList.Add(playerEntry);
    }

    public void PlayerLeftRoom(Player otherPlayer)
    {
        PlayerEntry playerEntry = null;
        foreach (PlayerEntry entry in playerList)
        {
            if (entry.Player.ActorNumber == otherPlayer.ActorNumber)
            {
                playerEntry = entry;
            }
        }
        playerList.Remove(playerEntry);
        Destroy(playerEntry.gameObject);

        AllPlayerReadyCheck();
    }

    public void MasterClientSwitched(Player newMasterClientt)
    {
        startButton.gameObject.SetActive(newMasterClientt.IsLocal);
        AllPlayerReadyCheck();
    }

    public void PlayerproperiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        PlayerEntry playerEntry = null;
        foreach(PlayerEntry entry in playerList)
        {
            if (entry.Player.ActorNumber == targetPlayer.ActorNumber)
            {
                playerEntry = entry;
            }
        }       
        playerEntry.ChangeCustomProperty(changedProps);

        AllPlayerReadyCheck();
    }


    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void AllPlayerReadyCheck()
    {
        if (PhotonNetwork.IsMasterClient == false) return;

        int readyCount = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.GetReady())
            {
                readyCount++;
            }
        }

        startButton.interactable = readyCount == PhotonNetwork.PlayerList.Length;
    }
}
