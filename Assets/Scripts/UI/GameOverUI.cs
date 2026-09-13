using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    #region [Champs]

    [SerializeField]
    private GameObject _GameOverPanel;

    [SerializeField]
    private TMP_Text _DepthText;

    [SerializeField]
    private TMP_Text _OreText;

    [SerializeField]
    private TMP_Text _CoinsEarnedText;

    [SerializeField]
    private TMP_Text _TotalCoinsText;

    [SerializeField]
    private Button _ClaimButton;

    [SerializeField]
    private Button _DoubleClaimButton;

    [SerializeField]
    private Button _ReviveButton;

    private bool _IsDisplayed;

    #endregion


    #region [Méthodes Unity]

    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsGameOver)
        {
            return;
        }

        if (!_IsDisplayed)
        {
            AfficherGameOver();
            return;
        }

        ActualiserBoutonReanimation();
        ActualiserBoutonRecompenseDouble();
    }

    #endregion


    #region [Game Over]

    private void AfficherGameOver()
    {
        _IsDisplayed = true;

        _DepthText.text = $"Profondeur : {RunManager.Instance.Depth} m";
        _OreText.text = $"Diamant : {RunManager.Instance.Ore}";
        _CoinsEarnedText.text = $"Pièces à récupérer : {RunManager.Instance.Ore}";
        _TotalCoinsText.text = $"Total pièces : {SaveManager.Instance.Coins}";

        ActualiserBoutonReanimation();
        ActualiserBoutonRecompenseDouble();

        _GameOverPanel.SetActive(true);
    }

    public void Rejouer()
    {
        if (AdsManager.Instance == null)
        {
            ChargerPartie();
            return;
        }

        AdsManager.Instance.TerminerRunEtAfficherInterstitialSiNecessaire(ChargerPartie);
    }

    private void ChargerPartie()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RetourMenu()
    {
        if (AdsManager.Instance == null)
        {
            ChargerMenu();
            return;
        }

        AdsManager.Instance.TerminerRunEtAfficherInterstitialSiNecessaire(ChargerMenu);
    }

    private void ChargerMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    #endregion


    #region [Récompense]

    public void RecupererRecompense()
    {
        RecupererRecompenseAvecMultiplicateur(1);
    }

    public void RecupererRecompenseDouble()
    {
        if (AdsManager.Instance == null || GameManager.Instance == null)
        {
            return;
        }

        if (!AdsManager.Instance.IsRewardedReady || GameManager.Instance.IsRewardClaimed)
        {
            return;
        }

        _DoubleClaimButton.interactable = false;

        AdsManager.Instance.AfficherPubRecompensee(AccorderRecompenseDouble);
    }

    private void AccorderRecompenseDouble()
    {
        RecupererRecompenseAvecMultiplicateur(2);
    }

    private void RecupererRecompenseAvecMultiplicateur(int pMultiplicateur)
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        int lPiecesGagnees = GameManager.Instance.RecupererRecompense(pMultiplicateur);

        if (lPiecesGagnees <= 0)
        {
            return;
        }

        _CoinsEarnedText.text = $"Pièces gagnées : {lPiecesGagnees}";

        ActualiserPieces();

        _ClaimButton.interactable = false;
        _DoubleClaimButton.interactable = false;
    }

    private void ActualiserBoutonRecompenseDouble()
    {
        if (_DoubleClaimButton == null || GameManager.Instance == null)
        {
            return;
        }

        bool lPubDisponible = AdsManager.Instance != null && AdsManager.Instance.IsRewardedReady;

        _DoubleClaimButton.interactable = !GameManager.Instance.IsRewardClaimed && lPubDisponible;
    }

    private void ActualiserPieces()
    {
        _TotalCoinsText.text = $"Total pièces : {SaveManager.Instance.Coins}";
    }

    #endregion


    #region [Réanimation]

    public void ReanimerAvecPub()
    {
        if (GameManager.Instance == null || AdsManager.Instance == null)
        {
            return;
        }

        if (!GameManager.Instance.CanRevive || !AdsManager.Instance.IsReviveReady)
        {
            return;
        }

        _ReviveButton.interactable = false;

        AdsManager.Instance.AfficherPubReanimation(AccorderReanimation);
    }

    private void AccorderReanimation()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        if (!GameManager.Instance.Reanimer())
        {
            return;
        }

        _GameOverPanel.SetActive(false);
        _IsDisplayed = false;
    }

    private void ActualiserBoutonReanimation()
    {
        if (_ReviveButton == null || GameManager.Instance == null)
        {
            return;
        }

        bool lPubDisponible = AdsManager.Instance != null && AdsManager.Instance.IsReviveReady;

        _ReviveButton.interactable = GameManager.Instance.CanRevive && lPubDisponible;
    }

    #endregion
}