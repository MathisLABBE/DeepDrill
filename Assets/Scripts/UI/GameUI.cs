using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    #region [Champs]

    [SerializeField]
    private TMP_Text _DepthText;

    [SerializeField]
    private TMP_Text _OreText;

    [SerializeField]
    private Image _EnergyBarFill;

    #endregion


    #region [Méthodes Unity]

    private void Update()
    {
        if (RunManager.Instance == null)
        {
            return;
        }

        AfficherProfondeur();
        AfficherMinerai();
        AfficherEnergie();
    }

    #endregion


    #region [Affichage]

    private void AfficherProfondeur()
    {
        _DepthText.text = $"{RunManager.Instance.Depth} m";
    }

    private void AfficherMinerai()
    {
        _OreText.text = $"Diamant : {RunManager.Instance.Ore}";
    }

    private void AfficherEnergie()
    {
        if (RunManager.Instance.MaxEnergy <= 0f)
        {
            _EnergyBarFill.fillAmount = 0f;
            return;
        }

        _EnergyBarFill.fillAmount = RunManager.Instance.Energy / RunManager.Instance.MaxEnergy;
    }

    #endregion
}