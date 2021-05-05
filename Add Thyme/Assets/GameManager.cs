using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Canvas gui;
    public GameObject MainOptionsPanel;
    public GameObject ChoicePanel;
    public GameObject GPGSPanel;
    public Text ResultText;

    public Text TriesText;
    public Text ChoiceText;

    private string choice;
    private List<GameObject> Panels;

    private Coin coin;

    private GoogleAdManager googleAds;
    private UnityAdManager unityAds;
    private int tries;
    private bool unlimitedTriesActive;

    private GPGSAchievements achievements;
    private GPGSLeaderboards leaderboard;
    private Score combo;
    // Start is called before the first frame update
    private void Awake()
    {
        googleAds = GetComponent<GoogleAdManager>();
        unityAds = GetComponent<UnityAdManager>();

        achievements = GetComponent<GPGSAchievements>();
        leaderboard = GetComponent<GPGSLeaderboards>();

        combo = GetComponent<Score>();
    }
    void Start()
    {
        coin = FindCoin();

        ResultText.text = "";

        ChoiceText.text = "";

        Panels = new List<GameObject>();
        Panels.Add(MainOptionsPanel);
        Panels.Add(ChoicePanel);
        Panels.Add(GPGSPanel);
        

        int adSource = DetermineAdSource();

        if (adSource == 0)
        {
            googleAds.RequestBanner();
        }
        else
        {
            StartCoroutine(unityAds.ShowBanner());
        }

        UpdateTries(3);
        unlimitedTriesActive = false;
    }

    private Coin FindCoin()
    {
        return FindObjectOfType<Coin>();
    }

    public void ShowPanel(string menu)
    {
        GameObject currentPanel;

        switch(menu)
        {
            case "Main":
                currentPanel = MainOptionsPanel;
                break;

            case "Choice":
                currentPanel = ChoicePanel;
                break;

            case "GPGS":
                currentPanel = GPGSPanel;
                break;

            default:
                currentPanel = MainOptionsPanel;
                break;
        }

        foreach(GameObject panel in Panels)
        {
            if (panel == currentPanel)
                panel.SetActive(true);
            else
                panel.SetActive(false);
        }
    }

    private void DeactivatePanels()
    {
        foreach (GameObject panel in Panels)
        {
            panel.SetActive(false);
        }
    }

    public void ShowResult(string result)
    {
        if (result != choice)
        {
            achievements.UnlockRegular(GPGSIds.achievement_unlucky);
            if(combo.GetScore() > 0)
            {
                leaderboard.UpdateLeaderboardScore();
                combo.ResetScore();
            }
            else
            {
                combo.ResetScore();
            }
            ResultText.text = result + "\nWrong";
        }

        else
        {
            achievements.UnlockRegular(GPGSIds.achievement_good_start);
            achievements.UpdateIncremental();
            combo.IncrementScore();
            ResultText.text = result + "\nRight";
        }

        if (!unlimitedTriesActive)
        {
            UpdateTries(-1);
        }

        if (tries <= 0 && !unlimitedTriesActive)
        {
            Debug.Log("out of tries");
            int adSource = DetermineAdSource();

            if(adSource == 0)
            {
                StartCoroutine(googleAds.ShowInterstitial());
            }
            else
            {
                StartCoroutine(unityAds.ShowInterstitial());
            }
        }
        else
            ShowPanel("Main");
    }

    public void OnChoiceSelected(string option)
    {
        choice = option;
        ChoiceText.text = "Your Choice: " + option;
        DeactivatePanels();
        coin.Flip();
    }

    public void OnRewardButtonClick()
    {
        if(unlimitedTriesActive)
        {
            Debug.Log("already active");
            return;
        }

        int adSource = DetermineAdSource();

        if (adSource == 0)
        {
            StartCoroutine(googleAds.ShowReward());
        }
        else
        {
            StartCoroutine(unityAds.ShowReward());
        }
    }

    public void OnAchievementsClick()
    {
        achievements.OpenAchievementPanel();
        ShowPanel("Main");
    }

    public void OnLeaderboardClick()
    {
        leaderboard.OpenLeaderboard();
        ShowPanel("Main");
    }

    // 0 for unity, 1 for google
    private int DetermineAdSource()
    {
        return Random.Range(0, 2);
    }

    public void UpdateTries(int amount)
    {
        if(amount == 0)
        {
            unlimitedTriesActive = true;
            tries = 0;
            StartCoroutine(UnlimitedTriesTimer());
        }
        else
        {
            tries += amount;
            TriesText.text = "Number of tries left: " + tries.ToString();
        }
    }

    private IEnumerator UnlimitedTriesTimer()
    {
        float duration = 120f;
        float timeRemaining = duration;

        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;

            float minutes = Mathf.FloorToInt(timeRemaining / 60);
            float seconds = Mathf.FloorToInt(timeRemaining % 60);

            if (timeRemaining > 0)
                TriesText.text = "Unlimited Tries: " + string.Format("{0:00}:{1:00}", minutes, seconds);

            yield return null;
        }

        while (coin.isFlipping)
            yield return null;

        unlimitedTriesActive = false;
        UpdateTries(3);
    }
}
