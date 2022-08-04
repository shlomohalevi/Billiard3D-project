using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "DifficultyGameData")]
public class DifficultyGameData : ScriptableObject
{
    public float daviatinAmountFromXaxisOfOriginalDir;
    public void SetGameDiffculty(float daviationAmountOfAi)
    {
        daviatinAmountFromXaxisOfOriginalDir = daviationAmountOfAi;
    }
}
