using UnityEngine;

public class RunManager : MonoBehaviour
{
    #region [Champs]

    [SerializeField]
    private Transform _Drill;

    [SerializeField]
    private float _BaseEnergy = 20f;

    [SerializeField]
    private float _EnergyPerLevel = 5f;

    [SerializeField]
    private float _EnergyDrainPerSecond = 1f;

    [SerializeField]
    private float _ReviveEnergyRatio = 0.5f;

    [SerializeField]
    private int _DiamondValue = 1;

    [SerializeField]
    private int _GoldValue = 2;

    private float _StartY;
    private float _Depth;
    private int _Diamond;
    private int _Gold;

    private float _Energy;
    private float _MaxEnergy;

    #endregion


    #region [Propriétés]

    public static RunManager Instance { get; private set; }

    public int Depth => Mathf.FloorToInt(_Depth);

    public int Diamond => _Diamond;

    public int Gold => _Gold;

    public int Ore => _Diamond;

    public int RunValue => (_Diamond * _DiamondValue) + (_Gold * _GoldValue);

    public float Energy => _Energy;

    public float MaxEnergy => _MaxEnergy;

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
    }

    private void Start()
    {
        _StartY = _Drill.position.y;

        InitialiserEnergie();
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            return;
        }

        CalculerProfondeur();
        ConsommerEnergie();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    #endregion


    #region [Profondeur]

    private void CalculerProfondeur()
    {
        _Depth = Mathf.Max(0f, _StartY - _Drill.position.y);
    }

    #endregion


    #region [Minerai]

    public void AjouterMinerai(int pQuantite)
    {
        _Diamond += pQuantite;

        Debug.Log($"Diamant  : {_Diamond}");
    }

    public void AjouterMinerai(MineralType pMineralType, int pQuantite)
    {
        switch (pMineralType)
        {
            case MineralType.Gold:
                _Gold += pQuantite;
                Debug.Log($"Or : {_Gold}");
                break;

            default:
                _Diamond += pQuantite;
                Debug.Log($"Diamant : {_Diamond}");
                break;
        }
    }

    #endregion


    #region [Énergie]

    private void InitialiserEnergie()
    {
        int lEnergyLevel = 0;

        if (UpgradeManager.Instance != null)
        {
            lEnergyLevel = UpgradeManager.Instance.EnergyLevel;
        }

        _MaxEnergy = _BaseEnergy + (lEnergyLevel * _EnergyPerLevel);
        _Energy = _MaxEnergy;
    }

    private void ConsommerEnergie()
    {
        _Energy -= _EnergyDrainPerSecond * Time.deltaTime;
        _Energy = Mathf.Max(0f, _Energy);

        if (_Energy <= 0f)
        {
            GameManager.Instance.TerminerPartie();
        }
    }

    public void RestaurerEnergieApresReanimation()
    {
        float lRatio = Mathf.Clamp01(_ReviveEnergyRatio);

        _Energy = _MaxEnergy * lRatio;

        Debug.Log($"Énergie restaurée : {_Energy}/{_MaxEnergy}");
    }

    #endregion
}