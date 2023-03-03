using UnityEngine;
using Blueberry.Core.SceneManagement;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameManager : SingletonBehaviour<GameManager>
{
    public GameModes GameMode
    {
        get;
        set;
    }
    public int ActiveCourseIndex
    {
        get;
        private set;
    }

    public void Proceed()
    {
        if (ActiveCourseIndex + 1 < CourseList.Instance.GetCourseListSize())
        {
            if (GameMode == GameModes.SinglePlayer)
                SceneHandler.Load("Main");
            else
                PhotonNetwork.LoadLevel("Main");

            ActiveCourseIndex++;
            CourseManager.Instance.SetCourse();
        }
        else
        {
            GameStateManager.Instance.SetState(GameSummaryState.Instance.name);
        }
    }

    public void ResetActiveCourse() => ActiveCourseIndex = 0;
    public enum GameModes
    { 
        None,
        SinglePlayer,
        CustomRoom,
        MultiPlayer
    }
}
