using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class DrillController : MonoBehaviour
{
    #region [Champs]

    [SerializeField]
    private float _HorizontalSpeed = 8f;

    [SerializeField]
    private float _DescentSpeed = 2f;

    [SerializeField]
    private float _DescentSpeedPerLevel = 0.3f;

    private float _CurrentDescentSpeed;

    [SerializeField]
    private float _MagnetRadiusPerLevel = 0.75f;

    [SerializeField]
    private float _MagnetCheckInterval = 0.1f;

    private float _MagnetRadius;
    private float _MagnetTimer;

    [SerializeField]
    private float _MinX = -2.5f;

    [SerializeField]
    private float _MaxX = 2.5f;

    private Rigidbody2D _Rb;
    private float _TargetX;

    [SerializeField]
    private ParticleSystem _DrillParticles;

    [SerializeField]
    private AudioSource _AudioSource;

    [SerializeField]
    private AudioClip _DrillSound;

    [SerializeField]
    private AudioClip _DiamondSound;

    [SerializeField]
    private bool _VibrationsEnabled = true;

    #endregion


    #region [Méthodes Unity]

    private void Awake()
    {
        _Rb = GetComponent<Rigidbody2D>();
        _TargetX = _Rb.position.x;
    }

    private void Start()
    {
        int lDrillLevel = 0;
        int lMagnetLevel = 0;

        if (UpgradeManager.Instance != null)
        {
            lDrillLevel = UpgradeManager.Instance.DrillLevel;
            lMagnetLevel = UpgradeManager.Instance.MagnetLevel;
        }

        _CurrentDescentSpeed = _DescentSpeed + (lDrillLevel * _DescentSpeedPerLevel);
        _MagnetRadius = lMagnetLevel * _MagnetRadiusPerLevel;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            return;
        }

        LireSouris();
        LireClavier();
        LireTactile();
        VerifierAimant();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            return;
        }

        Vector2 lNouvellePosition = _Rb.position;

        lNouvellePosition.y -= _CurrentDescentSpeed * Time.fixedDeltaTime;
        lNouvellePosition.x = Mathf.MoveTowards(lNouvellePosition.x, _TargetX, _HorizontalSpeed * Time.fixedDeltaTime);

        _Rb.MovePosition(lNouvellePosition);
    }

    #endregion


    #region [Contrôles]

    private void LireSouris()
    {
        if (Mouse.current == null)
        {
            return;
        }

        if (!Mouse.current.leftButton.isPressed)
        {
            return;
        }

        Vector2 lPositionSouris = Mouse.current.position.ReadValue();
        Vector3 lPositionMonde = Camera.main.ScreenToWorldPoint(lPositionSouris);

        _TargetX = Mathf.Clamp(lPositionMonde.x, _MinX, _MaxX);
    }

    private void LireClavier()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        float lDirection = 0f;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
        {
            lDirection = -1f;
        }

        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            lDirection = 1f;
        }

        _TargetX += lDirection * _HorizontalSpeed * Time.deltaTime;
        _TargetX = Mathf.Clamp(_TargetX, _MinX, _MaxX);
    }

    private void LireTactile()
    {
        if (Touchscreen.current == null)
        {
            return;
        }

        if (!Touchscreen.current.primaryTouch.press.isPressed)
        {
            return;
        }

        Vector2 lPositionTouch = Touchscreen.current.primaryTouch.position.ReadValue();
        Vector3 lPositionMonde = Camera.main.ScreenToWorldPoint(lPositionTouch);

        _TargetX = Mathf.Clamp(lPositionMonde.x, _MinX, _MaxX);
    }

    #endregion


    #region [Aimant]

    private void VerifierAimant()
    {
        if (_MagnetRadius <= 0f)
        {
            return;
        }

        _MagnetTimer += Time.deltaTime;

        if (_MagnetTimer < _MagnetCheckInterval)
        {
            return;
        }

        _MagnetTimer = 0f;

        Collider2D[] lColliders = Physics2D.OverlapCircleAll(transform.position, _MagnetRadius);

        foreach (Collider2D lCollider in lColliders)
        {
            Block lBloc = lCollider.GetComponent<Block>();

            if (lBloc == null)
            {
                continue;
            }

            lBloc.CollecterParAimant(this);
        }
    }

    #endregion


    #region [Effets]

    public void JouerEffetForage()
    {
        if (_DrillParticles != null)
        {
            _DrillParticles.Play();
        }

        if (_AudioSource != null && _DrillSound != null)
        {
            _AudioSource.PlayOneShot(_DrillSound);
        }
    }

    public void JouerSonDiamant()
    {
        if (_AudioSource == null || _DiamondSound == null)
        {
            return;
        }

        _AudioSource.PlayOneShot(_DiamondSound);
    }

    public void JouerVibrationDiamant()
    {
        if (!_VibrationsEnabled)
        {
            return;
        }

        Handheld.Vibrate();
    }

    #endregion
}