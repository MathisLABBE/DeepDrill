using System;
using Unity.Services.LevelPlay;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    #region [Champs]

    [SerializeField]
    private string _AppKey;

    [SerializeField]
    private string _RewardedAdUnitId;

    private LevelPlayRewardedAd _RewardedAd;
    private bool _IsRewardedReady;

    private Action _OnRewarded;

    [SerializeField]
    private string _ReviveAdUnitId;

    private LevelPlayRewardedAd _ReviveAd;

    private bool _IsReviveReady;

    private Action _OnReviveRewarded;

    [SerializeField]
    private string _InterstitialAdUnitId;

    private LevelPlayInterstitialAd _InterstitialAd;
    private bool _IsInterstitialReady;

    [SerializeField]
    private int _RunsPerInterstitial = 3;

    private int _RunsSinceInterstitial;
    private Action _OnInterstitialClosed;

    #endregion


    #region [Propriétés]

    public static AdsManager Instance { get; private set; }

    public bool IsRewardedReady => _IsRewardedReady;

    public bool IsReviveReady => _IsReviveReady;

    public bool IsInterstitialReady => _IsInterstitialReady;

    #endregion


    #region [Méthodes Unity]

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitialiserLevelPlay();
    }

    private void OnDestroy()
    {
        if (Instance != this)
        {
            return;
        }

        LevelPlay.OnInitSuccess -= InitialisationReussie;
        LevelPlay.OnInitFailed -= InitialisationEchouee;

        if (_RewardedAd != null)
        {
            _RewardedAd.OnAdLoaded -= PubRecompenseeChargee;
            _RewardedAd.OnAdLoadFailed -= ChargementPubRecompenseeEchoue;
            _RewardedAd.OnAdDisplayed -= PubRecompenseeAffichee;
            _RewardedAd.OnAdDisplayFailed -= AffichagePubRecompenseeEchoue;
            _RewardedAd.OnAdRewarded -= PubRecompenseeAccordee;
            _RewardedAd.OnAdClosed -= PubRecompenseeFermee;
        }

        if (_ReviveAd != null)
        {
            _ReviveAd.OnAdLoaded -= PubReanimationChargee;
            _ReviveAd.OnAdLoadFailed -= ChargementPubReanimationEchoue;
            _ReviveAd.OnAdDisplayed -= PubReanimationAffichee;
            _ReviveAd.OnAdDisplayFailed -= AffichagePubReanimationEchoue;
            _ReviveAd.OnAdRewarded -= PubReanimationAccordee;
            _ReviveAd.OnAdClosed -= PubReanimationFermee;
        }

        if (_InterstitialAd != null)
        {
            _InterstitialAd.OnAdLoaded -= PubInterstitialChargee;
            _InterstitialAd.OnAdLoadFailed -= ChargementPubInterstitialEchoue;
            _InterstitialAd.OnAdDisplayed -= PubInterstitialAffichee;
            _InterstitialAd.OnAdDisplayFailed -= AffichagePubInterstitialEchoue;
            _InterstitialAd.OnAdClosed -= PubInterstitialFermee;
        }

        Instance = null;
    }

    #endregion


    #region [Initialisation]

    private void InitialiserLevelPlay()
    {
        if (string.IsNullOrWhiteSpace(_AppKey))
        {
            Debug.LogError("LevelPlay : App Key manquante.");
            return;
        }

        LevelPlay.OnInitSuccess += InitialisationReussie;
        LevelPlay.OnInitFailed += InitialisationEchouee;

        LevelPlay.Init(_AppKey);

        Debug.Log("LevelPlay : initialisation...");
    }

    private void InitialisationReussie(LevelPlayConfiguration pConfiguration)
    {
        Debug.Log("LevelPlay : initialisation réussie.");

        CreerPubRecompensee();
        CreerPubReanimation();
        CreerPubInterstitial();
    }

    private void InitialisationEchouee(LevelPlayInitError pErreur)
    {
        Debug.LogError($"LevelPlay : échec de l'initialisation : {pErreur}");
    }

    #endregion


    #region [Pub récompensée]

    private void CreerPubRecompensee()
    {
        if (string.IsNullOrWhiteSpace(_RewardedAdUnitId))
        {
            Debug.LogError("LevelPlay : Rewarded Ad Unit ID manquant.");
            return;
        }

        _RewardedAd = new LevelPlayRewardedAd(_RewardedAdUnitId);

        _RewardedAd.OnAdLoaded += PubRecompenseeChargee;
        _RewardedAd.OnAdLoadFailed += ChargementPubRecompenseeEchoue;
        _RewardedAd.OnAdDisplayed += PubRecompenseeAffichee;
        _RewardedAd.OnAdDisplayFailed += AffichagePubRecompenseeEchoue;
        _RewardedAd.OnAdRewarded += PubRecompenseeAccordee;
        _RewardedAd.OnAdClosed += PubRecompenseeFermee;

        _RewardedAd.LoadAd();

        Debug.Log("LevelPlay : chargement de la pub récompensée...");
    }

    private void PubRecompenseeChargee(LevelPlayAdInfo pAdInfo)
    {
        _IsRewardedReady = true;

        Debug.Log("LevelPlay : pub récompensée chargée.");
    }

    private void ChargementPubRecompenseeEchoue(LevelPlayAdError pErreur)
    {
        _IsRewardedReady = false;

        Debug.LogError($"LevelPlay : échec du chargement de la pub récompensée : {pErreur}");
    }

    public void AfficherPubRecompensee(Action pOnRewarded)
    {
        if (_RewardedAd == null || !_RewardedAd.IsAdReady())
        {
            Debug.LogWarning("LevelPlay : aucune pub récompensée disponible.");
            return;
        }

        _OnRewarded = pOnRewarded;

        _RewardedAd.ShowAd();
    }

    private void PubRecompenseeAffichee(LevelPlayAdInfo pAdInfo)
    {
        _IsRewardedReady = false;

        Debug.Log("LevelPlay : pub récompensée affichée.");
    }

    private void AffichagePubRecompenseeEchoue(LevelPlayAdInfo pAdInfo, LevelPlayAdError pErreur)
    {
        _IsRewardedReady = false;
        _OnRewarded = null;

        Debug.LogError($"LevelPlay : échec de l'affichage : {pErreur}");

        _RewardedAd.LoadAd();
    }

    private void PubRecompenseeAccordee(LevelPlayAdInfo pAdInfo, LevelPlayReward pReward)
    {
        Debug.Log("LevelPlay : récompense accordée.");

        Action lOnRewarded = _OnRewarded;
        _OnRewarded = null;

        lOnRewarded?.Invoke();
    }

    private void PubRecompenseeFermee(LevelPlayAdInfo pAdInfo)
    {
        _IsRewardedReady = false;

        Debug.Log("LevelPlay : pub récompensée fermée.");

        _RewardedAd.LoadAd();
    }

    private void CreerPubReanimation()
    {
        if (string.IsNullOrWhiteSpace(_ReviveAdUnitId))
        {
            Debug.LogError("LevelPlay : Revive Ad Unit ID manquant.");
            return;
        }

        _ReviveAd = new LevelPlayRewardedAd(_ReviveAdUnitId);

        _ReviveAd.OnAdLoaded += PubReanimationChargee;
        _ReviveAd.OnAdLoadFailed += ChargementPubReanimationEchoue;
        _ReviveAd.OnAdDisplayed += PubReanimationAffichee;
        _ReviveAd.OnAdDisplayFailed += AffichagePubReanimationEchoue;
        _ReviveAd.OnAdRewarded += PubReanimationAccordee;
        _ReviveAd.OnAdClosed += PubReanimationFermee;

        _ReviveAd.LoadAd();

        Debug.Log("LevelPlay : chargement de la pub de réanimation...");
    }

    private void PubReanimationChargee(LevelPlayAdInfo pAdInfo)
    {
        _IsReviveReady = true;

        Debug.Log("LevelPlay : pub de réanimation chargée.");
    }

    private void ChargementPubReanimationEchoue(LevelPlayAdError pErreur)
    {
        _IsReviveReady = false;

        Debug.LogError($"LevelPlay : échec du chargement de la pub de réanimation : {pErreur}");
    }

    private void PubReanimationAffichee(LevelPlayAdInfo pAdInfo)
    {
        _IsReviveReady = false;

        Debug.Log("LevelPlay : pub de réanimation affichée.");
    }

    private void AffichagePubReanimationEchoue(LevelPlayAdInfo pAdInfo, LevelPlayAdError pErreur)
    {
        _IsReviveReady = false;
        _OnReviveRewarded = null;

        Debug.LogError($"LevelPlay : échec de l'affichage de la pub de réanimation : {pErreur}");

        _ReviveAd.LoadAd();
    }

    private void PubReanimationAccordee(LevelPlayAdInfo pAdInfo, LevelPlayReward pReward)
    {
        Debug.Log("LevelPlay : réanimation accordée.");

        Action lOnRewarded = _OnReviveRewarded;
        _OnReviveRewarded = null;

        lOnRewarded?.Invoke();
    }

    private void PubReanimationFermee(LevelPlayAdInfo pAdInfo)
    {
        _IsReviveReady = false;

        Debug.Log("LevelPlay : pub de réanimation fermée.");

        _ReviveAd.LoadAd();
    }

    public void AfficherPubReanimation(Action pOnRewarded)
    {
        if (_ReviveAd == null || !_ReviveAd.IsAdReady())
        {
            Debug.LogWarning("LevelPlay : aucune pub de réanimation disponible.");
            return;
        }

        _OnReviveRewarded = pOnRewarded;

        _ReviveAd.ShowAd();
    }

    #endregion


    #region [Pub interstitielle]

    private void CreerPubInterstitial()
    {
        if (string.IsNullOrWhiteSpace(_InterstitialAdUnitId))
        {
            Debug.LogError("LevelPlay : Interstitial Ad Unit ID manquant.");
            return;
        }

        _InterstitialAd = new LevelPlayInterstitialAd(_InterstitialAdUnitId);

        _InterstitialAd.OnAdLoaded += PubInterstitialChargee;
        _InterstitialAd.OnAdLoadFailed += ChargementPubInterstitialEchoue;
        _InterstitialAd.OnAdDisplayed += PubInterstitialAffichee;
        _InterstitialAd.OnAdDisplayFailed += AffichagePubInterstitialEchoue;
        _InterstitialAd.OnAdClosed += PubInterstitialFermee;

        _InterstitialAd.LoadAd();

        Debug.Log("LevelPlay : chargement de la pub interstitielle...");
    }

    private void PubInterstitialChargee(LevelPlayAdInfo pAdInfo)
    {
        _IsInterstitialReady = true;

        Debug.Log("LevelPlay : pub interstitielle chargée.");
    }

    private void ChargementPubInterstitialEchoue(LevelPlayAdError pErreur)
    {
        _IsInterstitialReady = false;

        Debug.LogError($"LevelPlay : échec du chargement de la pub interstitielle : {pErreur}");
    }

    private void PubInterstitialAffichee(LevelPlayAdInfo pAdInfo)
    {
        _IsInterstitialReady = false;

        Debug.Log("LevelPlay : pub interstitielle affichée.");
    }

    private void AffichagePubInterstitialEchoue(LevelPlayAdInfo pAdInfo, LevelPlayAdError pErreur)
    {
        _IsInterstitialReady = false;

        Debug.LogError($"LevelPlay : échec de l'affichage de la pub interstitielle : {pErreur}");

        Action lOnComplete = _OnInterstitialClosed;
        _OnInterstitialClosed = null;

        lOnComplete?.Invoke();

        _InterstitialAd.LoadAd();
    }

    private void PubInterstitialFermee(LevelPlayAdInfo pAdInfo)
    {
        _IsInterstitialReady = false;

        Debug.Log("LevelPlay : pub interstitielle fermée.");

        Action lOnComplete = _OnInterstitialClosed;
        _OnInterstitialClosed = null;

        lOnComplete?.Invoke();

        _InterstitialAd.LoadAd();
    }

    public void TerminerRunEtAfficherInterstitialSiNecessaire(Action pOnComplete, bool pAutoriserInterstitial = true)
    {
        _RunsSinceInterstitial++;

        if (!pAutoriserInterstitial)
        {
            pOnComplete?.Invoke();
            return;
        }

        if (_RunsSinceInterstitial < _RunsPerInterstitial)
        {
            pOnComplete?.Invoke();
            return;
        }

        if (_InterstitialAd == null || !_InterstitialAd.IsAdReady())
        {
            pOnComplete?.Invoke();
            return;
        }

        _RunsSinceInterstitial = 0;
        _OnInterstitialClosed = pOnComplete;

        _InterstitialAd.ShowAd();
    }

    #endregion
}