using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    #region [Champs]

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
    private TMP_Text _DrillSizeUpgradeText;

    [SerializeField]
    private Button _DrillSizeUpgradeButton;

    #endregion


    #region [Méthodes Unity]

    private void Start()
    {
        ActualiserUI();
    }

    #endregion


    #region [Navigation]

    public void Jouer()
    {
        SceneManager.LoadScene("Game");
    }

    #endregion


    #region [Améliorations]

    public void AcheterEnergie()
    {
        if (UpgradeManager.Instance == null)
        {
            return;
        }

        if (!UpgradeManager.Instance.AcheterEnergie())
        {
            return;
        }

        ActualiserUI();
    }

    public void AcheterForage()
    {
        if (UpgradeManager.Instance == null)
        {
            return;
        }

        if (!UpgradeManager.Instance.AcheterForage())
        {
            return;
        }

        ActualiserUI();
    }

    public void AcheterAimant()
    {
        if (UpgradeManager.Instance == null)
        {
            return;
        }

        if (!UpgradeManager.Instance.AcheterAimant())
        {
            return;
        }

        ActualiserUI();
    }

    public void AcheterTailleForeuse()
    {
        if (UpgradeManager.Instance == null)
        {
            return;
        }

        if (!UpgradeManager.Instance.AcheterTailleForeuse())
        {
            return;
        }

        ActualiserUI();
    }

    #endregion


    #region [Affichage]

    private void ActualiserUI()
    {
        ActualiserPieces();
        ActualiserTextesAmeliorations();
        ActualiserBoutonsAmeliorations();
    }

    private void ActualiserPieces()
    {
        if (_TotalCoinsText == null || SaveManager.Instance == null)
        {
            return;
        }

        _TotalCoinsText.text = $"Pièces : {SaveManager.Instance.Coins}";
    }

    private void ActualiserTextesAmeliorations()
    {
        if (UpgradeManager.Instance == null)
        {
            return;
        }

        if (_EnergyUpgradeText != null)
        {
            _EnergyUpgradeText.text =
                $"Énergie niv. {UpgradeManager.Instance.EnergyLevel}\nCoût : {UpgradeManager.Instance.EnergyCost}";
        }

        if (_DrillUpgradeText != null)
        {
            _DrillUpgradeText.text =
                $"Forage niv. {UpgradeManager.Instance.DrillLevel}\nCoût : {UpgradeManager.Instance.DrillCost}";
        }

        if (_MagnetUpgradeText != null)
        {
            _MagnetUpgradeText.text =
                $"Aimant niv. {UpgradeManager.Instance.MagnetLevel}\nCoût : {UpgradeManager.Instance.MagnetCost}";
        }

        if (_DrillSizeUpgradeText != null)
        {
            _DrillSizeUpgradeText.text =
                $"Taille niv. {UpgradeManager.Instance.DrillSizeLevel}\nCoût : {UpgradeManager.Instance.DrillSizeCost}";
        }
    }

    private void ActualiserBoutonsAmeliorations()
    {
        if (SaveManager.Instance == null || UpgradeManager.Instance == null)
        {
            return;
        }

        int lCoins = SaveManager.Instance.Coins;

        if (_EnergyUpgradeButton != null)
        {
            _EnergyUpgradeButton.interactable = lCoins >= UpgradeManager.Instance.EnergyCost;
        }

        if (_DrillUpgradeButton != null)
        {
            _DrillUpgradeButton.interactable = lCoins >= UpgradeManager.Instance.DrillCost;
        }

        if (_MagnetUpgradeButton != null)
        {
            _MagnetUpgradeButton.interactable = lCoins >= UpgradeManager.Instance.MagnetCost;
        }

        if (_DrillSizeUpgradeButton != null)
        {
            _DrillSizeUpgradeButton.interactable = lCoins >= UpgradeManager.Instance.DrillSizeCost;
        }
    }

    #endregion
}