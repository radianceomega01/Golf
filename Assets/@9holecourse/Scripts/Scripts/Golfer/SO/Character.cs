using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Scriptable Objects/Character", order = 2)]
public class Character : ScriptableObject
{
    public int avatarId;
    public Sprite avatarIcon;
    public new string name;
    public string description;
    public Stats stats;
    public Golfer gameplayPrefab;
    public StatValues statValues;

    [Serializable]
    public struct Stats
    {
        public int luck;
        public int power;
        public int accuracy;
        public int drives;
        public int putts;
        public int winds;
    }

    [Serializable]
    public struct StatValues
    {
        public float luck;
        public float inaccuracy;
        public float drives;
        public float inaccPutts;
        public float inaccWinds;
    }
}
