using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region [Champs]

    private bool _IsGameOver;
    private bool _IsRewardClaimed;
    private bool _HasRevived;
    private float _InvulnerableUntil;

    [SerializeField]
    private float _ReviveInvulnerabilityDuration = 0.75f;

    [SerializeField]
    private AudioSource _AudioSource;

    [SerializeField]
    private AudioClip _GameOverSound;

    [SerializeField]
    private bool _VibrationsEnabled = true;

    [SerializeField]
    [Min(1)]
    private int _CoinsPerDiamond = 1;

    #endregion


    #region [Propriétés]

    public static GameManager Instance { get; private set; }

    public bool IsGameOver => _IsGameOver;

    public bool IsRewardClaimed => _IsRewardClaimed;

    public bool HasRevived => _HasRevived;

    public bool CanRevive => _IsGameOver && !_HasRevived && !_IsRewardClaimed;

    public bool IsInvulnerable => Time.time < _InvulnerableUntil;

    public int CoinsPerDiamond => Mathf.Max(1, _CoinsPerDiamond);

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

        Application.targetFrameRate = 120;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    #endregion


    #region [Partie]

    public void TerminerPartie()
    {
        if (_IsGameOver || IsInvulnerable)
        {
            return;
        }

        _IsGameOver = true;

        JouerSonGameOver();
        JouerVibrationGameOver();

        Debug.Log("Game Over");
    }

    public bool Reanimer()
    {
        if (!CanRevive || RunManager.Instance == null)
        {
            return false;
        }

        RunManager.Instance.RestaurerEnergieApresReanimation();

        _HasRevived = true;
        _IsGameOver = false;
        _InvulnerableUntil = Time.time + _ReviveInvulnerabilityDuration;

        Debug.Log("Réanimation réussie.");

        return true;
    }

    public int RecupererRecompense(int pMultiplicateur)
    {
        if (!_IsGameOver || _IsRewardClaimed)
        {
            return 0;
        }

        if (RunManager.Instance == null || SaveManager.Instance == null)
        {
            return 0;
        }

        int lMultiplicateur = Mathf.Max(1, pMultiplicateur);
        int lPiecesGagnees = RunManager.Instance.Ore * CoinsPerDiamond * lMultiplicateur;

        SaveManager.Instance.AjouterPieces(lPiecesGagnees);

        _IsRewardClaimed = true;

        return lPiecesGagnees;
    }

    #endregion


    #region [Effets]

    private void JouerSonGameOver()
    {
        if (_AudioSource == null || _GameOverSound == null)
        {
            return;
        }

        _AudioSource.PlayOneShot(_GameOverSound);
    }

    private void JouerVibrationGameOver()
    {
        if (!_VibrationsEnabled)
        {
            return;
        }

        Handheld.Vibrate();
    }

    #endregion
}