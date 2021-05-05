using System.Collections;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class GoogleAdManager : MonoBehaviour
{
    private GameManager manager;

    private BannerView banner;
    private InterstitialAd interstitial;
    private RewardedAd reward;

    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponent<GameManager>();

        MobileAds.Initialize(initStatus => { });

        //RequestBanner();
        RequestInterstitial();
        RequestReward();

        //StartCoroutine(ShowInterstitial());
        //StartCoroutine(ShowReward());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RequestBanner()
    {
        string testID = "ca-app-pub-3940256099942544/6300978111";

        banner = new BannerView(testID, AdSize.Banner, AdPosition.Top);

        AdRequest request = new AdRequest.Builder().Build();
        banner.LoadAd(request);
        //banner.Hide();
    }

    private void RequestInterstitial()
    {
        string testID = "ca-app-pub-3940256099942544/1033173712";

        interstitial = new InterstitialAd(testID);

        AdRequest request = new AdRequest.Builder().Build();
        interstitial.LoadAd(request);

        interstitial.OnAdClosed += OnInterstialClosed;
    }

    private void RequestReward()
    {
        string testID = "ca-app-pub-3940256099942544/5224354917";

        reward = new RewardedAd(testID);

        AdRequest request = new AdRequest.Builder().Build();
        reward.LoadAd(request);
        reward.OnUserEarnedReward += OnRewardWatced;
    }

    public IEnumerator ShowInterstitial()
    {
        while (!interstitial.IsLoaded())
            yield return null;

        interstitial.Show();
    }

    public IEnumerator ShowReward()
    {
        while (!reward.IsLoaded())
            yield return null;

        reward.Show();
    }

    private void OnInterstialClosed(object sender, EventArgs args)
    {
        manager.ShowPanel("Main");
        manager.UpdateTries(3);
        RequestInterstitial();
    }

    private void OnRewardWatced(object sender, EventArgs args)
    {
        manager.UpdateTries(0);
        RequestReward();
    }
}
