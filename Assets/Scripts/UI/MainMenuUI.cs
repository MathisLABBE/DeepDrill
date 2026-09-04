using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    #region [Champs]

    [SerializeField]
    private TMP_Text _CoinsText;

    private const string _GameSceneName = "Game";

    #endregion


    #region [Menu]

    public void Jouer()
    {
        SceneManager.LoadScene(_GameSceneName);
    }

    public void Quitter()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #endregion


    #region [Méthodes Unity]

    private void Start()
    {
        Application.targetFrameRate = 120;

        AfficherPieces();
    }

    #endregion


    #region [Affichage]

    private void AfficherPieces()
    {
        _CoinsText.text = $"Pièces : {SaveManager.ObtenirPiecesSauvegardees()}";
    }

    #endregion
}