using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if GOOGLE_MOBILE_ADS && !UNITY_EDITOR
using GoogleMobileAds.Api;
using GoogleMobileAds.Api.Mediation;
#endif

/// <summary>
/// 광고 매니저
/// </summary>
public class VideoAdsManager : SingletonBehaviour<VideoAdsManager>
{
    public Action OnVideoAdsFinished { get; set; } = null;

    //안드로이드 테스트 키 : ca-app-pub-3940256099942544~3347511713
    //IOS 테스트 키 : ca-app-pub-3940256099942544~1458002511
    
#if UNITY_ANDROID
#if RELEASE_MODE            //실제 광고키
    public static string adUnitId               = "ca-app-pub-3940256099942544/5224354917";
#else                       //테스트용 광고키
    public static string adUnitId               = "ca-app-pub-3940256099942544/5224354917";
#endif
#elif UNITY_IOS
#if RELEASE_MODE            //실제 광고키
    public static string adUnitId               = "ca-app-pub-3940256099942544/1712485313";
#else                       //테스트용 광고키
    public static string adUnitId               = "ca-app-pub-3940256099942544/1712485313";
#endif
#else
    public static string adUnitId               = "unused";
#endif

#if GOOGLE_MOBILE_ADS && !UNITY_EDITOR
     private RewardedAd  rewardedAd = null;
#endif

    /// <summary>
    /// 광고 초기화 함수
    /// </summary>
    public void Initialize()
    {
#if GOOGLE_MOBILE_ADS && !UNITY_EDITOR
        MobileAds.Initialize(initStatus =>
        {
            LoadAd();
        });
#else
        Debug.LogWarning("Google Mobile Ads SDK not present; rewarded ads disabled.");
#endif
    }
    /// <summary>
    /// 광고 로드 함수
    /// </summary>
     public void LoadAd()
     {
#if GOOGLE_MOBILE_ADS && !UNITY_EDITOR
         // Clean up the old ad before loading a new one.
         if (rewardedAd != null)
         {
             rewardedAd.Destroy();
             rewardedAd = null;
         }

         Debug.Log("Loading the rewarded ad.");

         // create our request used to load the ad.
         var adRequest = new AdRequest();

         // send the request to load the ad.
         RewardedAd.Load(adUnitId, adRequest,
             (RewardedAd ad, LoadAdError error) =>
             {
                 // if error is not null, the load request failed.
                 if (error != null || ad == null)
                 {
                     Debug.LogError("Rewarded ad failed to load an ad " +
                                    "with error : " + error);
                     return;
                 }

                 Debug.Log("Rewarded ad loaded with response : "
                           + ad.GetResponseInfo());

                 rewardedAd = ad;
                 
                 RegisterEventHandlers(ad);
                 RegisterReloadHandler(ad);
             });
#else
         Debug.LogWarning("LoadAd called but Google Mobile Ads SDK is missing.");
#endif
     }

    /// <summary>
    /// 광고 보여주는 함수
    /// </summary>
     public void ShowAd()
     {
#if GOOGLE_MOBILE_ADS && !UNITY_EDITOR

         const string rewardMsg =
             "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

         if (rewardedAd != null && rewardedAd.CanShowAd())
         {
             rewardedAd.Show((Reward reward) =>
             {
                 // TODO: Reward the user.
                 Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
                 
                 if (OnVideoAdsFinished != null)
                     OnVideoAdsFinished();
             });
         }
#else
         // In the editor or when the Mobile Ads SDK is not installed, immediately
         // invoke the completion callback to keep game flow moving.
         if (OnVideoAdsFinished != null)
             OnVideoAdsFinished();
#endif
     }

    
#if GOOGLE_MOBILE_ADS && !UNITY_EDITOR
    /// <summary>
    /// 광고 이벤트 연결 함수
    /// </summary>
    /// <param name="ad">리워드 광고</param>
     private void RegisterEventHandlers(RewardedAd ad)
     {
         // Raised when the ad is estimated to have earned money.
         ad.OnAdPaid += (AdValue adValue) =>
         {
             Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                 adValue.Value,
                 adValue.CurrencyCode));
         };
         // Raised when an impression is recorded for an ad.
         ad.OnAdImpressionRecorded += () =>
         {
             Debug.Log("Rewarded ad recorded an impression.");
         };
         // Raised when a click is recorded for an ad.
         ad.OnAdClicked += () =>
         {
             Debug.Log("Rewarded ad was clicked.");
         };
         // Raised when an ad opened full screen content.
         ad.OnAdFullScreenContentOpened += () =>
         {
             Debug.Log("Rewarded ad full screen content opened.");
         };
         // Raised when the ad closed full screen content.
         ad.OnAdFullScreenContentClosed += () =>
         {
             Debug.Log("Rewarded ad full screen content closed.");
         };
         // Raised when the ad failed to open full screen content.
         ad.OnAdFullScreenContentFailed += (AdError error) =>
         {
             Debug.LogError("Rewarded ad failed to open full screen content " +
                            "with error : " + error);
         };
     }

    /// <summary>
    /// 광고 리로드 이벤트 연결 함수
    /// </summary>
    /// <param name="ad">리워드 함수</param>
     private void RegisterReloadHandler(RewardedAd ad)
     {
         // Raised when the ad closed full screen content.
         ad.OnAdFullScreenContentClosed += () =>
         {
             Debug.Log("Rewarded Ad full screen content closed.");

             // Reload the ad so that we can show another as soon as possible.
             LoadAd();
         };
         // Raised when the ad failed to open full screen content.
         ad.OnAdFullScreenContentFailed += (AdError error) =>
         {
             Debug.LogError("Rewarded ad failed to open full screen content " +
                            "with error : " + error);

             // Reload the ad so that we can show another as soon as possible.
             LoadAd();
         };
     }
#endif
}
