using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DebugGameManager : MonoBehaviourPunCallbacks
{

    [SerializeField] string debugRoomName;

    [SerializeField] int spawnStoneTime;

    private void Start()
    {
        PhotonNetwork.LocalPlayer.NickName = $"TestPlayer{Random.Range(100, 1000)}";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 8;
        options.IsVisible = false;
        TypedLobby typedLobby = new TypedLobby("DebugLobby", LobbyType.Default);

        PhotonNetwork.JoinOrCreateRoom(debugRoomName, options, typedLobby);
    }

    public override void OnJoinedRoom()
    {
        StartCoroutine(GameStartDelay());
    }

    IEnumerator GameStartDelay()
    {
        yield return new WaitForSeconds(1f);
        GameStart();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (newMasterClient.IsLocal)
        {
            spawnStoneRoutine = StartCoroutine(SpawnStoneRoutine());
        }
    }

    public void GameStart()
    {
        Vector2 pos = Random.insideUnitCircle * 30;
        PhotonNetwork.Instantiate("Player", new Vector3(pos.x, 0, pos.y), Quaternion.identity);
        if (PhotonNetwork.IsMasterClient)
        {
            spawnStoneRoutine = StartCoroutine(SpawnStoneRoutine());
        }
    }

    Coroutine spawnStoneRoutine;
    IEnumerator SpawnStoneRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnStoneTime);

            Vector2 direction = Random.insideUnitCircle.normalized;
            Vector3 position = new Vector3(direction.x, 0, direction.y) * 200f;

            Vector3 force = -position.normalized * 30f + new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
            Vector3 torque = Random.insideUnitSphere * Random.Range(1f, 3f);
            object[] instantiateData = { force, torque };
            if (Random.Range(0, 2) < 1)
            {
                PhotonNetwork.InstantiateRoomObject("LargeStone", position, Random.rotation, 0, instantiateData);
            }
            else
            {
                PhotonNetwork.InstantiateRoomObject("SmallStone", position, Random.rotation, 0, instantiateData);
            }
        }
    }
}
