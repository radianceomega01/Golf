using Blueberry.Core.AddressableAssets;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "ClubList", menuName = "Scriptable Objects/ClubList", order = 5)]
public class ClubList : ScriptableObject
{

    /*0 = Driver
      1 = Chiper
      2 = Iron
      3 = Putter*/

    [SerializeField] private Club[] clubs;

    public static ClubList Instance { get; private set; }

    public static async Task Initialize()
    {
        Instance = await AddressablesManager.LoadAsset<ClubList>("ClubList");
    }

    public Club GetClub(int index) => clubs[index];

    public Club[] GetAllClubs() => clubs;
}
