using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Club", menuName = "Scriptable Objects/Club", order = 1)]
public class Club : ScriptableObject
{
    public enum Type
    {
        Driver,
        Chiper,
        Irons,
        Puter
    }

    [Serializable]
    public struct Force
    {
        public float upward;
        public float forward;
    }

    public Type type;
    public Force force;
    public GameObject prefab;
    public Sprite clubIcon;
    public Sprite selectedClubIcon;
}
