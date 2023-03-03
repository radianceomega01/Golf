using Blueberry.Core.API.Game;
using Blueberry.Core.SceneManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class BootStrap : MonoBehaviour
{
    async void Start()
    {
        await CharacterList.Initialize();
        await ClubList.Initialize();
        await CourseList.Initialize();
        await UpdateSkillsAndCoins();
        SceneHandler.Load("Title");
    }

    async Task UpdateSkillsAndCoins()
    {
        JObject response =  await UserGameSkillGetter.GetGameSkill();
        if ((bool)response["status"] == false || (string)response["record"] == null)
        {
            PurchaseHandler.Coins = 0;
            return;
        }

        GameData gameData = JsonConvert.DeserializeObject<GameData>((string)response["record"]["settingJSON"]);

        int count = CharacterList.Instance.GetAllCharacters().Length;
        for (int i = 0; i < count; i++)
        {
            Character character = CharacterList.Instance.GetCharacter(i);

            StatsData statsData = gameData.GetStatsData(i);
            StatsValueData statsValueData = gameData.GetStatsValueData(i);
            if (statsData == null)
            {
                PurchaseHandler.Coins = 0;
                return;
            }

            character.stats.luck = statsData.GetLuck();
            character.stats.power = statsData.GetPower();
            character.stats.accuracy = statsData.GetAccuracy();
            character.stats.drives = statsData.GetDrives();
            character.stats.putts = statsData.GetPutts();
            character.stats.winds = statsData.GetWinds();

            character.statValues.luck = statsValueData.GetLuck();
            character.statValues.inaccuracy = statsValueData.GetInaccuracy();
            character.statValues.drives = statsValueData.GetDrives();
            character.statValues.inaccPutts = statsValueData.GetInaccPutts();
            character.statValues.inaccWinds = statsValueData.GetInaccWinds();
        }
        PurchaseHandler.Coins = gameData.coins;
    }
}
