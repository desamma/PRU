using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStat", menuName = "StatTree/Stat")]
public class StatSO : ScriptableObject
{
    public string statName;
    public int statNumber;
    public Sprite statIcon; // If we need
}
