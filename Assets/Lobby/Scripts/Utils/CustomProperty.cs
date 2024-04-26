using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;


public static class CustomProperty
{
    static PhotonHashTable property = new PhotonHashTable();

    public const string READY = "Ready";
    public const string LOAD = "Load";

    public static bool GetReady(this Player player)
    {
        PhotonHashTable customProperty = player.CustomProperties;
        bool ready;
        if (customProperty.TryGetValue(READY, out object value))
        {
            return (bool)value;
        }
        else
        {
            return false;
        }
    }

    public static void SetReady(this Player player, bool value)
    {
        PhotonHashTable customProperty = new PhotonHashTable();
        customProperty["Ready"] = value;
        player.SetCustomProperties(customProperty);
    }

    public static bool GetLoad(this Player player)
    {
        PhotonHashTable customProperty = player.CustomProperties;
        bool ready;
        if (customProperty.TryGetValue(LOAD, out object value))
        {
            return (bool)value;
        }
        else
        {
            return false;
        }
    }

    public static void SetLoad(this Player player, bool value)
    {
        PhotonHashTable customProperty = new PhotonHashTable();
        customProperty["Load"] = value;
        player.SetCustomProperties(customProperty);
    }

    public const string GAMESTART = "GameStart";

    public static bool GetGameStart(this Room room)
    {
        PhotonHashTable customProperty = room.CustomProperties;
        if (customProperty.TryGetValue(GAMESTART, out object value))
        {
            return (bool)value;
        }
        else
        {
            return false;
        }
    }


    public const string GameStarTime = "GameStartTime";
    public static double GetGameStartTIme(this Room room)
    {
        PhotonHashTable customProperty = room.CustomProperties;
        if (customProperty.TryGetValue(GAMESTART, out object value))
        {
            return (double)value;
        }
        else
        {
            return 0;
        }
    }

    public static void SetGameStartTime(this Room room, double value)
    {
        property.Clear();
        property[GAMESTARTTIME] = value;

    }
}
