using UnityEngine;

public class SaveManager : MonoBehaviour
{
    #region [Champs]

    private const string _CoinsKey = "Coins";

    private int _Coins;

    #endregion


    #region [Propriétés]

    public static SaveManager Instance { get; private set; }

    public int Coins => _Coins;

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
        _Coins = PlayerPrefs.GetInt(_CoinsKey, 0);
    }

    private void Sauvegarder()
    {
        PlayerPrefs.SetInt(_CoinsKey, _Coins);
        PlayerPrefs.Save();
    }

    #endregion


    #region [Pièces]

    public void AjouterPieces(int pQuantite)
    {
        _Coins += pQuantite;

        Sauvegarder();

        Debug.Log($"Pièces totales : {_Coins}");
    }

    public bool DepenserPieces(int pQuantite)
    {
        if (_Coins < pQuantite)
        {
            return false;
        }

        _Coins -= pQuantite;

        Sauvegarder();

        return true;
    }

    public static int ObtenirPiecesSauvegardees()
    {
        return PlayerPrefs.GetInt(_CoinsKey, 0);
    }

    #endregion


    #region [Débogage]

    [ContextMenu("Ajouter 100 pièces")]
    private void Ajouter100Pieces()
    {
        _Coins = PlayerPrefs.GetInt(_CoinsKey, 0);

        AjouterPieces(100);
    }

    [ContextMenu("Ajouter 1 000 pièces")]
    private void Ajouter1000Pieces()
    {
        _Coins = PlayerPrefs.GetInt(_CoinsKey, 0);

        AjouterPieces(1000);
    }

    [ContextMenu("Mettre 100 000 pièces")]
    private void Mettre100000Pieces()
    {
        _Coins = 100000;

        Sauvegarder();

        Debug.Log($"Pièces totales : {_Coins}");
    }

    [ContextMenu("Réinitialiser les pièces")]
    private void ReinitialiserPieces()
    {
        _Coins = 0;

        Sauvegarder();

        Debug.Log("Pièces réinitialisées.");
    }

    #endregion
}