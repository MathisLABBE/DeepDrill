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
    private float _RotationOffset = 90f;

    private Rigidbody2D _Rb;
    private Vector2 _MovementDirection;
    private bool _HasMovementInput;

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

        _MovementDirection = Vector2.zero;
        _HasMovementInput = false;

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

        if (_HasMovementInput)
        {
            _Rb.AddForce(_MovementDirection * _MovementForce, ForceMode2D.Force);

            float lAngleCible = Mathf.Atan2(_MovementDirection.y, _MovementDirection.x) * Mathf.Rad2Deg + _RotationOffset;
            float lNouvelAngle = Mathf.MoveTowardsAngle(_Rb.rotation, lAngleCible, _RotationSpeed * Time.fixedDeltaTime);

            _Rb.MoveRotation(lNouvelAngle);
        }

        if (_Rb.linearVelocity.sqrMagnitude > _MaxSpeed * _MaxSpeed)
        {
            _Rb.linearVelocity = _Rb.linearVelocity.normalized * _MaxSpeed;
        }
    }

    private void OnCollisionStay2D(Collision2D pCollision)
    {
        DrillableElement lElement = pCollision.gameObject.GetComponent<DrillableElement>();

        if (lElement == null)
        {
            return;
        }

        lElement.Forer(this, _DrillPower, Time.fixedDeltaTime);
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

        _MovementDirection = lDirection.normalized;
        _HasMovementInput = true;
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

        _MovementDirection = lDirection.normalized;
        _HasMovementInput = true;
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

            if (lElement != null)
            {
                lElement.CollecterParAimant(this);
                continue;
            }

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