using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class DrillController : MonoBehaviour
{
    #region [Champs]

    [SerializeField]
    private float _MovementForce = 30f;

    [SerializeField]
    private float _MaxSpeed = 5f;

    [SerializeField]
    private float _MovementDamping = 4f;

    [SerializeField]
    private float _TargetStopDistance = 0.15f;

    [SerializeField]
    private float _RotationSpeed = 360f;

    [SerializeField]
    [Range(0.1f, 1f)]
    private float _MinTurnSpeedMultiplier = 0.35f;

    [SerializeField]
    private float _RotationOffset = 90f;

    private Rigidbody2D _Rb;
    private Vector2 _TargetDirection;
    private bool _HasTargetDirection;

    [SerializeField]
    private float _BaseDrillPower = 1f;

    [SerializeField]
    private float _DrillPowerPerLevel = 0.25f;

    private float _DrillPower;

    [SerializeField]
    private float _MagnetRadiusPerLevel = 0.75f;

    [SerializeField]
    private float _MagnetCheckInterval = 0.1f;

    private float _MagnetRadius;
    private float _MagnetTimer;

    [SerializeField]
    private float _SizePerLevel = 0.1f;

    private Vector3 _BaseScale;

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
        _Rb.linearDamping = _MovementDamping;

        _BaseScale = transform.localScale;
    }

    private void Start()
    {
        int lMagnetLevel = 0;
        int lDrillLevel = 0;
        int lDrillSizeLevel = 0;

        if (UpgradeManager.Instance != null)
        {
            lMagnetLevel = UpgradeManager.Instance.MagnetLevel;
            lDrillLevel = UpgradeManager.Instance.DrillLevel;
            lDrillSizeLevel = UpgradeManager.Instance.DrillSizeLevel;
        }

        _MagnetRadius = lMagnetLevel * _MagnetRadiusPerLevel;
        _DrillPower = _BaseDrillPower + (lDrillLevel * _DrillPowerPerLevel);

        float lSizeMultiplier = 1f + (lDrillSizeLevel * _SizePerLevel);

        transform.localScale = _BaseScale * lSizeMultiplier;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            return;
        }

        _HasTargetDirection = false;

        LireSouris();
        LireClavier();
        LireTactile();
        VerifierAimant();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            _Rb.linearVelocity = Vector2.zero;
            return;
        }

        float lRotation = _Rb.rotation;

        if (_HasTargetDirection)
        {
            float lAngleCible = Mathf.Atan2(_TargetDirection.y, _TargetDirection.x) * Mathf.Rad2Deg + _RotationOffset;

            lRotation = Mathf.MoveTowardsAngle(
                lRotation,
                lAngleCible,
                _RotationSpeed * Time.fixedDeltaTime);

            _Rb.MoveRotation(lRotation);
        }

        float lForwardAngle = (lRotation - _RotationOffset) * Mathf.Deg2Rad;

        Vector2 lForwardDirection = new Vector2(
            Mathf.Cos(lForwardAngle),
            Mathf.Sin(lForwardAngle));

        float lMovementMultiplier = 1f;

        if (_HasTargetDirection)
        {
            float lTargetAngle = Mathf.Atan2(_TargetDirection.y, _TargetDirection.x) * Mathf.Rad2Deg + _RotationOffset;
            float lAngleDifference = Mathf.Abs(Mathf.DeltaAngle(lRotation, lTargetAngle));

            float lTurnRatio = Mathf.Clamp01(lAngleDifference / 90f);

            lMovementMultiplier = Mathf.Lerp(1f, _MinTurnSpeedMultiplier, lTurnRatio);
        }

        _Rb.AddForce(lForwardDirection * _MovementForce * lMovementMultiplier, ForceMode2D.Force);

        if (_Rb.linearVelocity.sqrMagnitude > _MaxSpeed * _MaxSpeed)
        {
            _Rb.linearVelocity = _Rb.linearVelocity.normalized * _MaxSpeed;
        }
    }

    #endregion


    #region [Contrôles]

    private void LireSouris()
    {
        if (Mouse.current == null || !Mouse.current.leftButton.isPressed)
        {
            return;
        }

        Vector2 lPositionSouris = Mouse.current.position.ReadValue();

        DefinirDirectionVersPositionEcran(lPositionSouris);
    }

    private void LireClavier()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        Vector2 lDirection = Vector2.zero;

        if (Keyboard.current.aKey.isPressed ||
            Keyboard.current.qKey.isPressed ||
            Keyboard.current.leftArrowKey.isPressed)
        {
            lDirection.x -= 1f;
        }

        if (Keyboard.current.dKey.isPressed ||
            Keyboard.current.rightArrowKey.isPressed)
        {
            lDirection.x += 1f;
        }

        if (Keyboard.current.wKey.isPressed ||
            Keyboard.current.zKey.isPressed ||
            Keyboard.current.upArrowKey.isPressed)
        {
            lDirection.y += 1f;
        }

        if (Keyboard.current.sKey.isPressed ||
            Keyboard.current.downArrowKey.isPressed)
        {
            lDirection.y -= 1f;
        }

        if (lDirection == Vector2.zero)
        {
            return;
        }

        _TargetDirection = lDirection.normalized;
        _HasTargetDirection = true;
    }

    private void LireTactile()
    {
        if (Touchscreen.current == null ||
            !Touchscreen.current.primaryTouch.press.isPressed)
        {
            return;
        }

        Vector2 lPositionTouch = Touchscreen.current.primaryTouch.position.ReadValue();

        DefinirDirectionVersPositionEcran(lPositionTouch);
    }

    private void DefinirDirectionVersPositionEcran(Vector2 pPositionEcran)
    {
        Camera lCamera = Camera.main;

        if (lCamera == null)
        {
            return;
        }

        Vector3 lPositionMonde = lCamera.ScreenToWorldPoint(pPositionEcran);
        Vector2 lDirection = (Vector2)lPositionMonde - _Rb.position;

        if (lDirection.magnitude <= _TargetStopDistance)
        {
            return;
        }

        _TargetDirection = lDirection.normalized;
        _HasTargetDirection = true;
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
            DrillableElement lElement = lCollider.GetComponent<DrillableElement>();

            if (lElement == null)
            {
                continue;
            }

            lElement.CollecterParAimant(this);
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


    #region [Forage]

    public void ForerElement(DrillableElement pElement, float pDeltaTime)
    {
        if (pElement == null)
        {
            return;
        }

        pElement.Forer(this, _DrillPower, pDeltaTime);
    }

    #endregion
}