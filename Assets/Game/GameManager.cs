using ExitGames.Client.Photon.Encryption;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text infogText;
    [SerializeField] float countDown;

    private void Start()
    {
        PhotonNetwork.LocalPlayer.SetLoad(true);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashtable changedProps)
    {
        if (changedProps.ContainsKey(CustomProperty.LOAD))
        {
            if (PlyerLoadCount() == PhotonNetwork.PlayerList.Length)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.CurrentRoom.GetGameStart(true);
                    PhotonNetwork.CurrentRoom.SetGameStartTime(PhotonNetwork.Time);
                }
            }
            else
            {
                infogText.text = $"Wait {PlyerLoadCount()} / {PhotonNetwork.PlayerList.Length}";
            }
        }
    }

    public override void OnRoomPropertiesUpdate(PhotonHashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(CustomProperty.GAMESTART))
        {
            StartCoroutine(StartTimer());
        }
    }

    IEnumerator StartTimer()
    {
        double LoadTimne = PhotonNetwork.CurrentRoom.GetGameStartTIme();
        while (PhotonNetwork.Time - LoadTimne < CountDownTime)
        {
            int remainTime = (int)(CountdownTime - (PhotonNetwork.Time - LoadTIme));
            infogText.text = (remainTime + 1).ToString();
            yield return null;
        }

        float currentTime = 0;
        while (currentTime < countDown)
        {
            currentTime += Time.deltaTime;
            infogText.text = currentTime.ToString();
            yield return null;
        }

        infogText.text = "Game Start!";
        yield return new WaitForSeconds(3f);

        infogText.text = "";
    }

    private int PlyerLoadCount()
    {
        int loadCount = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.GetLoad())
            {
                loadCount++;
            }
        }
        return loadCount;


    }
}
