using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public string notes;
    public string picture;
    public int totalScore;

    public PlayerData(string playerName, string notes, string picture)
    {
        this.playerName = playerName;
        this.notes = notes;
        this.picture = picture;
        totalScore = 0;
    }
}
