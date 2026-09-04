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
    private TMP_Text _EnergyUpgradeText;

    [SerializeField]
    private Button _EnergyUpgradeButton;

    [SerializeField]
    private TMP_Text _DrillUpgradeText;

    [SerializeField]
    private Button _DrillUpgradeButton;

    [SerializeField]
    private TMP_Text _MagnetUpgradeText;

    [SerializeField]
    private Button _MagnetUpgradeButton;

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
        if (_IsDisplayed || GameManager.Instance == null || !GameManager.Instance.IsGameOver)
        {
            return;
        }

        AfficherGameOver();
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

        ActualiserAmeliorationEnergie();
        ActualiserAmeliorationForage();
        ActualiserAmeliorationAimant();
        ActualiserBoutonsAmeliorations();
        ActualiserBoutonReanimation();

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


    #region [Améliorations]

    public void AcheterEnergie()
    {
        if (UpgradeManager.Instance == null)
        {
            return;
        }

        UpgradeManager.Instance.AcheterEnergie();

        ActualiserAmeliorationEnergie();
        ActualiserPieces();
        ActualiserBoutonsAmeliorations();
    }

    private void ActualiserAmeliorationEnergie()
    {
        _EnergyUpgradeText.text =
            $"Énergie niveau {UpgradeManager.Instance.EnergyLevel}\nCoût : {UpgradeManager.Instance.EnergyCost} pièces";
    }

    private void ActualiserPieces()
    {
        _TotalCoinsText.text = $"Total pièces : {SaveManager.Instance.Coins}";
    }

    public void AcheterForage()
    {
        if (UpgradeManager.Instance == null)
        {
            return;
        }

        UpgradeManager.Instance.AcheterForage();

        ActualiserAmeliorationForage();
        ActualiserPieces();
        ActualiserBoutonsAmeliorations();
    }

    private void ActualiserAmeliorationForage()
    {
        _DrillUpgradeText.text =
            $"Forage niveau {UpgradeManager.Instance.DrillLevel}\nCoût : {UpgradeManager.Instance.DrillCost} pièces";
    }

    public void AcheterAimant()
    {
        if (UpgradeManager.Instance == null)
        {
            return;
        }

        UpgradeManager.Instance.AcheterAimant();

        ActualiserAmeliorationAimant();
        ActualiserPieces();
        ActualiserBoutonsAmeliorations();
    }

    private void ActualiserAmeliorationAimant()
    {
        _MagnetUpgradeText.text =
            $"Aimant niveau {UpgradeManager.Instance.MagnetLevel}\nCoût : {UpgradeManager.Instance.MagnetCost} pièces";
    }

    private void ActualiserBoutonsAmeliorations()
    {
        if (SaveManager.Instance == null || UpgradeManager.Instance == null)
        {
            return;
        }

        int lCoins = SaveManager.Instance.Coins;

        _EnergyUpgradeButton.interactable = lCoins >= UpgradeManager.Instance.EnergyCost;
        _DrillUpgradeButton.interactable = lCoins >= UpgradeManager.Instance.DrillCost;
        _MagnetUpgradeButton.interactable = lCoins >= UpgradeManager.Instance.MagnetCost;
    }

    #endregion


    #region [Récompense]

    public void RecupererRecompense()
    {
        RecupererRecompenseAvecMultiplicateur(1);
    }

    public void RecupererRecompenseDouble()
    {
        if (AdsManager.Instance == null)
        {
            return;
        }

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
        ActualiserBoutonsAmeliorations();

        _ClaimButton.interactable = false;
        _DoubleClaimButton.interactable = false;
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