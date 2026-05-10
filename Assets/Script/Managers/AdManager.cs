using System;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public enum RewardPlacement { Revive, Coins }

public class AdManager : MonoBehaviour
{
    public static AdManager Instance { get; private set; }
    public int coinsAmount = 20;
    // ——— Ad Unit IDs ———
#if UNITY_ANDROID
    [Header("Android Rewarded IDs (PROD)")]
    [SerializeField] private string reviveAdUnitId = "ca-app-pub-5932543810636514/3005322929";
    [SerializeField] private string coinsAdUnitId  = "ca-app-pub-5932543810636514/3643461648";
    [SerializeField] private string interstitialAdUnitId = "ca-app-pub-5932543810636514/6349516030"; 
#else
    [SerializeField] private string reviveAdUnitId = "unused";
    [SerializeField] private string coinsAdUnitId  = "unused";
    [SerializeField] private string interstitialAdUnitId = "unused";
#endif

    [Header("Pool")]
    [SerializeField] private int preloadCountPerPlacement = 3;
    [SerializeField] private bool initializeAdsOnStart = true;
    private bool _adsInitialized;
    
    [Header("Interstitial Settings")]
    [SerializeField, Range(1, 10)] private int interstitialFrequency = 3; // her kaç restartta bir
    private InterstitialAd _interstitialAd;
    private Action _onInterstitialClosed;

    private readonly Dictionary<RewardPlacement, Queue<RewardedAd>> _pools = new();
    private readonly Dictionary<RewardPlacement, int> _loadingInFlight = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _pools[RewardPlacement.Revive] = new Queue<RewardedAd>();
        _pools[RewardPlacement.Coins]  = new Queue<RewardedAd>();
        _loadingInFlight[RewardPlacement.Revive] = 0;
        _loadingInFlight[RewardPlacement.Coins]  = 0;

    }
    
    private void Start()
    {
        if (initializeAdsOnStart)
            StartCoroutine(InitializeAdsRoutine());
    }

    private System.Collections.IEnumerator InitializeAdsRoutine()
    {
        yield return null;

        try
        {
            MobileAds.Initialize(_ =>
            {
                _adsInitialized = true;

                TopUpPool(RewardPlacement.Revive);
                TopUpPool(RewardPlacement.Coins);
                LoadInterstitial();
            });
        }
        catch (Exception e)
        {
            _adsInitialized = false;
            Debug.LogException(e);
        }
    }

    private void LoadInterstitial()
    {
        if (!_adsInitialized) return;

        var request = new AdRequest();
        InterstitialAd.Load(interstitialAdUnitId, request, (InterstitialAd ad, LoadAdError err) =>
        {
            if (err != null || ad == null)
            {
                Debug.LogWarning("[AdManager] Interstitial load failed: " + err);
                return;
            }

            _interstitialAd = ad;

            _interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                // önce kapandığını haber ver, sonra yeniden yükle
                _onInterstitialClosed?.Invoke();
                _onInterstitialClosed = null;
                LoadInterstitial();
            };

            _interstitialAd.OnAdFullScreenContentFailed += (AdError e) =>
            {
                Debug.LogWarning("[AdManager] Interstitial show failed: " + e);
                // başarısız da olsa oyuna dönmek için callback'i tetikle
                _onInterstitialClosed?.Invoke();
                _onInterstitialClosed = null;
                LoadInterstitial();
            };
        });
    }

    public void TryShowInterstitial(Action onClosed = null)
    {
        if (!_adsInitialized)
        {
            onClosed?.Invoke();
            return;
        }

        if (_interstitialAd != null)
        {
            _onInterstitialClosed = onClosed; // kapanınca haber ver
            _interstitialAd.Show();
            _interstitialAd = null; // bir kez kullanılıyor
        }
        else
        {
            // reklam yoksa oyunu bekletme
            onClosed?.Invoke();
            LoadInterstitial();
        }
    }

    public void TryShowInterstitial()
    {
        if (!_adsInitialized) return;

        if (_interstitialAd != null)
        {
            _interstitialAd.Show();
            _interstitialAd = null; // bir kez kullanılır
            LoadInterstitial();
        }
    }
    public int GetInterstitialFrequency() => interstitialFrequency;

    private string GetUnitId(RewardPlacement p) =>
        p == RewardPlacement.Revive ? reviveAdUnitId : coinsAdUnitId;

    private void TopUpPool(RewardPlacement placement)
    {
        if (!_adsInitialized) return;

        var q = _pools[placement];
        while (q.Count + _loadingInFlight[placement] < preloadCountPerPlacement)
        {
            _loadingInFlight[placement]++;

            // Yeni API: new AdRequest()
            var request = new AdRequest();

            RewardedAd.Load(GetUnitId(placement), request, (RewardedAd ad, LoadAdError loadError) =>
            {
                _loadingInFlight[placement]--;

                if (loadError != null || ad == null)
                {
                    Debug.LogWarning($"[AdManager] Load failed ({placement}): {loadError}");
                    return;
                }

                ad.OnAdFullScreenContentClosed += () => TopUpPool(placement);
                ad.OnAdFullScreenContentFailed += (AdError err) =>
                {
                    Debug.LogWarning($"[AdManager] Show failed ({placement}): {err}");
                    TopUpPool(placement);
                };

                q.Enqueue(ad);
                // Hedefe ulaşmadıysak doldurmaya devam et
                TopUpPool(placement);
            });
        }
    }

    public bool HasRewarded(RewardPlacement placement) =>
        _pools.TryGetValue(placement, out var q) && q.Count > 0;
    
    
    public void ShowRewarded(RewardPlacement placement, Action onReward, Action onUnavailable = null)
    {
        if (!_adsInitialized)
        {
            onUnavailable?.Invoke();
            return;
        }

        var q = _pools[placement];
        if (q.Count == 0)
        {
            Debug.Log($"[AdManager] No rewarded available for {placement}. Loading…");
            TopUpPool(placement);
            onUnavailable?.Invoke();
            return;
        }

        var ad = q.Dequeue();
        ad.Show((Reward reward) =>
        {
            try { onReward?.Invoke(); }
            catch (Exception e) { Debug.LogException(e); }
        });
        // Kapanınca TopUpPool zaten event’lerde çağrılıyor.
    }
}
