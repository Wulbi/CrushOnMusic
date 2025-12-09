using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoAdTestScripts : MonoBehaviour
{
    public void InitVideoAds()
    {
        VideoAdsManager.Instance.Initialize();   
    }

    public void ShowVideoAds()
    {
        VideoAdsManager.Instance.ShowAd();   
    }
}
