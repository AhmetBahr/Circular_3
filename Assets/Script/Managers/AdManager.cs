using System;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance { get; private set; }

#if UNITY_ANDROID
    [SerializeField] private string rewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917"; // TEST
#elif UNITY_IOS
    [SerializeField] private string rewardedAdUnitId = "ca-app-pub-3940256099942544/1712485313"; // TEST
#else
    [SerializeField] private string rewardedAdUnitId = "unused";
#endif

    [Header("Pool")]
    [SerializeField] private int preloadCount = 5;

    private readonly Queue<RewardedAd> _loaded = new Queue<RewardedAd>();
    private int _loadingInFlight = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // GMA init
        MobileAds.Initialize(_ =>
        {
            // init tamam → havuzu doldur
            TopUpPool();
        });
    }

    /// <summary> Havuzu hedef sayıda dolu tutar. </summary>
    private void TopUpPool()
    {
        while (_loaded.Count + _loadingInFlight < preloadCount)
        {
            _loadingInFlight++;

            // YENİ API: Build() YOK → direkt new AdRequest()
            AdRequest request = new AdRequest();

            RewardedAd.Load(rewardedAdUnitId, request, (RewardedAd ad, LoadAdError loadError) =>
            {
                _loadingInFlight--;

                if (loadError != null)
                {
                    Debug.LogWarning("[AdManager] Rewarded load failed: " + loadError);
                    // istersen küçük bir gecikmeyle tekrar dene
                    // Invoke(nameof(TopUpPool), 1.0f);
                    return;
                }

                if (ad == null)
                {
                    Debug.LogWarning("[AdManager] Rewarded load returned null ad");
                    return;
                }

                // fullscreen eventleri → kapanınca yerine yenisini yükle
                ad.OnAdFullScreenContentClosed += () =>
                {
                    TopUpPool();
                };
                ad.OnAdFullScreenContentFailed += (AdError err) =>
                {
                    Debug.LogWarning("[AdManager] Show failed: " + err);
                    TopUpPool();
                };

                _loaded.Enqueue(ad);

                // Hala hedefe ulaşmadıysak devam yükle
                TopUpPool();
            });
        }
    }

    /// <summary>
    /// Hazır bir ödüllü reklamı göster. Ödül kazanılırsa onReward çağrılır.
    /// </summary>
    public void ShowRewarded(Action onReward, Action onUnavailable = null)
    {
        if (_loaded.Count == 0)
        {
            Debug.Log("[AdManager] No rewarded available. Loading…");
            TopUpPool();
            onUnavailable?.Invoke();
            return;
        }

        var ad = _loaded.Dequeue();

        // YENİ API: ödül callback’i Show içine verilir
        ad.Show((Reward reward) =>
        {
            try { onReward?.Invoke(); }
            catch (Exception e) { Debug.LogException(e); }
        });

        // Kapanınca/başarısızlıkta yerine yenisini TopUpPool ile event’lerde yüklüyoruz.
    }
}
