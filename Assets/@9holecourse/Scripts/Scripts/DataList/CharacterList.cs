
using Blueberry.Core.AddressableAssets;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "CharacterList", menuName = "Scriptable Objects/CharacterList", order = 4)]
public class CharacterList : ScriptableObject
{
    /*0 = Blueberry
      1 = Honeysue
      2 = Huckelberry
      3 = Gooseberry
      4 = CEO
      5 = Maxly
      6 = Boxnard
      7 = Ms Barker
      8 = Mr Farmer
      9 = Mrs Farmer*/

    public static CharacterList Instance { get; private set; }
    public static async Task Initialize()
    {
        //Instance = await Addressables.LoadAssetAsync<CharacterList>("CharacterList").Task;
        Instance = await AddressablesManager.LoadAsset<CharacterList>("CharacterList");
    }

    [SerializeField] private Character[] characters;

    public Character GetCharacter(int index) => characters[index];

    public Character[] GetAllCharacters() => characters;
}
