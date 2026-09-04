using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    #region [Champs]

    [SerializeField]
    private Transform _Target;

    [SerializeField]
    private float _VerticalOffset = -2f;

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

    #endregion
}