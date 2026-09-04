using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    #region [Champs]

    [SerializeField]
    private Transform _Target;

    [SerializeField]
    private float _VerticalOffset = -2f;

    [SerializeField]
    private float _WorldWidth = 6f;

    #endregion


    #region [Méthodes Unity]

    private void LateUpdate()
    {
        if (_Target == null)
        {
            return;
        }

        Vector3 lNouvellePosition = transform.position;
        lNouvellePosition.y = _Target.position.y + _VerticalOffset;

        transform.position = lNouvellePosition;
    }

    private void Start()
    {
        AdapterCameraALaLargeur();
    }

    #endregion


    #region [Affichage]

    private void AdapterCameraALaLargeur()
    {
        Camera lCamera = GetComponent<Camera>();

        if (lCamera == null || !lCamera.orthographic)
        {
            return;
        }

        float lHalfWidth = _WorldWidth / 2f;

        lCamera.orthographicSize = lHalfWidth / lCamera.aspect;
    }

    #endregion
}