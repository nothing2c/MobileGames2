using UnityEngine;

public class GPGSLeaderboards : MonoBehaviour
{
    public void OpenLeaderboard()
    {
        Social.ShowLeaderboardUI();
    }

    public void UpdateLeaderboardScore()
    {
        Debug.Log(PlayerPrefs.GetInt("ScoreToUpdate"));
        if (PlayerPrefs.GetInt("ScoreToUpdate", 0) == 0)
            return;

        Social.ReportScore(PlayerPrefs.GetInt("ScoreToUpdate", 1), GPGSIds.leaderboard_guess_combo, (bool success) =>
        {
            if (success)
            {
                PlayerPrefs.SetInt("ScoreToUpdate", 0);
            }
        });
    }
}
