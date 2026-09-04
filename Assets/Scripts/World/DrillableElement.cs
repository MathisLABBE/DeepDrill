using UnityEngine;

public class DrillableElement : MonoBehaviour
{
    #region [Champs]

    [SerializeField]
    private float _Resistance = 1f;

    private float _CurrentDrilling;

    #endregion


    #region [Forage]

    public void Forer(float pDrillPower, float pDeltaTime)
    {
        _CurrentDrilling += pDrillPower * pDeltaTime;

        if (_CurrentDrilling >= _Resistance)
        {
            Destroy(gameObject);
        }
    }

    #endregion
}