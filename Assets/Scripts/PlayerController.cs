using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform _targetZone;

    [Header("Stats")]
    public PlayerStats Statistics;

    [Header("Components")]
    [SerializeField] private Animator _animator;
    [SerializeField] private CharacterController _chController;


    private bool _forward;
    [SerializeField] private float _currentSpeed;
    [SerializeField] private float _speed;
    private Quaternion _targetRotation;
    private float _velocityDown;
    private Vector2 _inputMovement;
    public Vector3 _direction;
    private WeaponModel _weapon;

    private static PlayerController _instance;

    public static PlayerController Instance { get { return _instance; } }

    public AudioSource hitSound;
    public AudioSource miss;

    public bool IsCanMoving{ private get; set; }
    [field:SerializeField]
    public bool IsCanAttack { private get; set; }
    private bool criticalDamage;

    private float actMeter;

    private Animator _cameraAnimator;

    public GameObject DeadScreen;
    private GameStats stat;

    private float _attackDelayCount = 0;

    private void Awake()
    {
        _instance = this;
        _weapon = new SwordModel();
        Statistics.SetDefValue();

        _cameraAnimator = Camera.main.GetComponent<Animator>();
    }

    private void Start()
    {
        _forward = true;
        IsCanAttack = true;
        stat = GameObject.Find("Game Manager").GetComponent<GameStats>();
        _direction = Vector3.zero;

        //IF SOMEONE FUCKING TAKES vSYNC OUT OF HERE -BITCH I'LL RIP OFF YOUR FUCKING HANDS !!!!!
        /* DO NOT TOUCH */ QualitySettings.vSyncCount = 1; /* DO NOT TOUCH */
        //DO NOT TOUCH !!

        actMeter = transform.position.z;
        _targetRotation = transform.rotation;
    }

    private void Update()
    {
        if(IsCanMoving == false)
            return;

        ApplyGravity();
        ApplyMovement();
        ApplyRotation();
        MeterCouner();

        if(Statistics.Health > 20)
        {
            _cameraAnimator.SetTrigger("Health ok");
        }
        if(Statistics.Health > 10 && Statistics.Health <= 20)
        {
            _cameraAnimator.SetTrigger("Health 20");
        }
        if(Statistics.Health > 5 && Statistics.Health <= 10)
        {
            _cameraAnimator.SetTrigger("Health 10");
        }
        if(Statistics.Health >= 0 && Statistics.Health <= 5)
        {
            _cameraAnimator.SetTrigger("Health 5");
        }

        if (Statistics.Health <= 0)
            GameOver();


        if(!IsCanAttack)
        {
            float expect = Statistics.AttackDelay;
                _attackDelayCount += Time.deltaTime;
                UIController.Instance.UpdateACDUI(_attackDelayCount);

            if(_attackDelayCount >= expect)
                IsCanAttack = true;
        }
    }
    private void MeterCouner()
    {
        if (transform.position.z > actMeter + 2 || transform.position.z < actMeter - 2)
        {
            actMeter = transform.position.z;
            stat.allDistance++;
        }
    }

    private void GameOver()
    {
        IsCanMoving = false;
        DeadScreen.SetActive(true);
        stat.SetStat();
        IsCanMoving = false;
        _direction = Vector3.zero;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_targetZone.position, _targetZone.position + _targetZone.position * 0);
        Gizmos.DrawWireSphere(_targetZone.position + _targetZone.position * 0, 1);
    }

    public void ChangeMovement(Vector2 movement)
    {
        if (!IsCanMoving) return;

        _inputMovement = movement;
    }

    public WeaponModel GetWeapon()
    {
        return _weapon;
    }

    private void ApplyGravity()
    {
        if (!IsCanMoving) return;

        if (_chController.isGrounded && _velocityDown < 0.0f)
        {
            _velocityDown = -1.0f;
        }
        else
        {
            _velocityDown += Physics.gravity.y * Statistics.Speed * Time.deltaTime;
        }

        _direction.y = _velocityDown;
    }

    private void ApplyMovement()
    {
        if (!IsCanMoving) return;

        if (_inputMovement.x != 0)
        {
            _currentSpeed = Mathf.Lerp(_currentSpeed, 1f, Time.deltaTime * 4f);
        }
        else
        {
            _currentSpeed = Mathf.Lerp(_currentSpeed, 0, Time.deltaTime * 4f);
        }

        _direction.z = _inputMovement.x;

        _chController.Move(_direction * _speed * Time.deltaTime);

        _animator.SetFloat("speed", _currentSpeed);

        if (_inputMovement.x > 0)
        {
            if (!_forward)
            {
                _targetRotation = Quaternion.Euler(0, 0, 0);
                _forward = true;
            }
        }
        else if (_inputMovement.x < 0)
        {
            if (_forward)
            {
                _targetRotation = Quaternion.Euler(0, 180, 0);
                _forward = false;
            }
        }
    }
    private void ApplyRotation()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * 7f);
    }
    public void ToAttack()
    {
        if (!IsCanMoving) return;
        if (!IsCanAttack) return;
        IsCanAttack = false;
        _attackDelayCount = 0;

        stat.allHits++;
        PlayAttackAnimation();
    }
    public void GiveDamage()
    {
        RaycastHit[] hist;
        hist = Physics.SphereCastAll(_targetZone.position, 1f, _targetZone.position, 0);

        foreach (RaycastHit hit in hist)
        {
            var enemy = hit.collider.GetComponent<EnemyController>();
            if (enemy)
            {
                stat.allNoMissedHits++;
                if (criticalDamage)
                { 
                    stat.allCritycalHits++;
                    enemy.ClaimDamage(_weapon.Damage * 2 + Statistics.AdditionalDamage);
                }
                else
                    enemy.ClaimDamage(_weapon.Damage + Statistics.AdditionalDamage);
                    

                hitSound.Play();
                break;
            }
            else
            {
                miss.Play();
            }
        }
    }

    public void ClaimDamage(int damage)
    {
        Statistics.Health -= damage;
        UIController.Instance.UpdateHPUI(Statistics.Health);
    }

    public void ClaimMoney(int reward)
    {
        Statistics.Money += (int)(reward * Statistics.MoneyMultipler);
        UIController.Instance.UpdateCoinText(Statistics.Money);
    }

    private void PlayAttackAnimation()
    {
        if (_inputMovement.x == 0)
        {
            criticalDamage = false;
            _animator.SetBool("atack_1", true);
            _animator.SetBool("atack_2", false);
        }
        else if (_inputMovement.x != 0)
        {
            criticalDamage = true;
            _animator.SetBool("atack_1", false);
            _animator.SetBool("atack_2", true);
        }
    }
}
