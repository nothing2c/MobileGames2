using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAdManager : MonoBehaviour, IUnityAdsListener
{
    private GameManager manager;

    const string gameId = "4045765";
    const string bannerId = "Banner_Android";
    const string interstitialId = "Interstitial_Android";
    const string rewardId = "Rewarded_Android";
    bool testMode = true;

    void Start()
    {
        manager = GetComponent<GameManager>();

        Advertisement.AddListener(this);
        Advertisement.Initialize(gameId, testMode);

        //StartCoroutine(ShowBanner());
        //StartCoroutine(ShowInterstitial());
        //StartCoroutine(ShowReward());
    }

    public IEnumerator ShowBanner()
    {
        while (!Advertisement.IsReady(bannerId))
            yield return null;

        Advertisement.Banner.SetPosition(BannerPosition.TOP_CENTER);
        Advertisement.Banner.Show(bannerId);
    }

    public IEnumerator ShowInterstitial()
    {
        while (!Advertisement.IsReady(interstitialId))
            yield return null;

        Advertisement.Show(interstitialId);
    }

    public IEnumerator ShowReward()
    {
        while (!Advertisement.IsReady(rewardId))
            yield return null;

        Advertisement.Show(rewardId);
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        Debug.Log(placementId + ", " + showResult);

        switch(placementId)
        {
            case interstitialId:
                manager.ShowPanel("Main");
                manager.UpdateTries(3);
                break;

            case rewardId:
                Debug.Log(placementId + ", " + showResult);
                manager.UpdateTries(0);
                break;

            default:
                break;
        }
    }

    public void OnUnityAdsDidError(string message)
    {
    }


    public void OnUnityAdsDidStart(string placementId)
    {
    }

    public void OnUnityAdsReady(string placementId)
    {
    }

}
