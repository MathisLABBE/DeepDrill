using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    #region [Champs]

    private const string _EnergyLevelKey = "EnergyLevel";

    private int _EnergyLevel;

    private const string _DrillLevelKey = "DrillLevel";

    private int _DrillLevel;

    private const string _MagnetLevelKey = "MagnetLevel";

    private int _MagnetLevel;

    #endregion


    #region [Propriétés]

    public static UpgradeManager Instance { get; private set; }

    public int EnergyLevel => _EnergyLevel;

    public int EnergyCost => 10 + (_EnergyLevel * 10);

    public int DrillLevel => _DrillLevel;

    public int DrillCost => 15 + (_DrillLevel * 15);

    public int MagnetLevel => _MagnetLevel;

    public int MagnetCost => 20 + (_MagnetLevel * 20);

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

        Charger();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    #endregion


    #region [Sauvegarde]

    private void Charger()
    {
        _EnergyLevel = PlayerPrefs.GetInt(_EnergyLevelKey, 0);
        _DrillLevel = PlayerPrefs.GetInt(_DrillLevelKey, 0);
        _MagnetLevel = PlayerPrefs.GetInt(_MagnetLevelKey, 0);
    }

    private void Sauvegarder()
    {
        PlayerPrefs.SetInt(_EnergyLevelKey, _EnergyLevel);
        PlayerPrefs.SetInt(_DrillLevelKey, _DrillLevel);
        PlayerPrefs.SetInt(_MagnetLevelKey, _MagnetLevel);

        PlayerPrefs.Save();
    }

    #endregion


    #region [Énergie]

    public bool AcheterEnergie()
    {
        if (SaveManager.Instance == null)
        {
            return false;
        }

        if (!SaveManager.Instance.DepenserPieces(EnergyCost))
        {
            return false;
        }

        _EnergyLevel++;

        Sauvegarder();

        return true;
    }

    #endregion


    #region [Forage]

    public bool AcheterForage()
    {
        if (SaveManager.Instance == null)
        {
            return false;
        }

        if (!SaveManager.Instance.DepenserPieces(DrillCost))
        {
            return false;
        }

        _DrillLevel++;

        Sauvegarder();

        return true;
    }

    #endregion


    #region [Aimant]

    public bool AcheterAimant()
    {
        if (SaveManager.Instance == null)
        {
            return false;
        }

        if (!SaveManager.Instance.DepenserPieces(MagnetCost))
        {
            return false;
        }

        _MagnetLevel++;

        Sauvegarder();

        return true;
    }

    #endregion


    #region [Débogage]

    [ContextMenu("Réinitialiser les améliorations")]
    private void ReinitialiserAmeliorations()
    {
        _EnergyLevel = 0;
        _DrillLevel = 0;
        _MagnetLevel = 0;

        PlayerPrefs.DeleteKey(_EnergyLevelKey);
        PlayerPrefs.DeleteKey(_DrillLevelKey);
        PlayerPrefs.DeleteKey(_MagnetLevelKey);
        PlayerPrefs.Save();

        Debug.Log("Améliorations réinitialisées.");
    }

    #endregion
}