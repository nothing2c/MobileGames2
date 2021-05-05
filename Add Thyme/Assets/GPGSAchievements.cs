using GooglePlayGames;
using UnityEngine;

public class GPGSAchievements : MonoBehaviour
{
    public void OpenAchievementPanel()
    {
        Social.ShowAchievementsUI();
    }

    public void UpdateIncremental()
    {
        PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_wildcard, 1, null);
    }

    public void UnlockRegular(string id)
    {
        Social.ReportProgress(id, 100f, null);
    }
}
