using System.Collections;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    #region Variables  

    [Header("Zombie Config")]
    [SerializeField] private float RotationSpeed = 10f;
    [SerializeField] private float ChasingRange=4;
    [SerializeField] private float ZombieWalkingSpeed=10;
    [SerializeField] private float ZombieRestTime=1F;
    [SerializeField] private float DieDelayTime;
    [SerializeField] private float ZombieRuningSpeed;
    [SerializeField] private float AttackingRange;

    [Header("Object Refs")]
    [SerializeField] private PrometeoCarController Player;
    [SerializeField] private GameObject BloodEffect;
    [SerializeField] private Rigidbody ZombieRigidBody;
    [SerializeField] private CapsuleCollider ZombieCapsuleCollider;

    private Vector3 _targetPosition;
    private Vector3 dir;
    private bool _isMoving;
    private bool _isChasing;
    private bool _isAlive;

    #endregion

    #region Private Functions
    private void OnEnable()
    {
        BloodEffect.gameObject.SetActive(false);

        ZombieCapsuleCollider.isTrigger =false;
        ZombieRigidBody.isKinematic = false;
        ZombieRigidBody.constraints = RigidbodyConstraints.FreezeRotation;

        _isAlive = true;
        _isMoving = false;
        _isChasing = false;
    }

    private void Start()
    {
        Player= GameObject.FindAnyObjectByType<PrometeoCarController>();
    }

    private void Update()
    {
        if (_isAlive)
        {
            if (Vector3.Distance(Player.transform.position, transform.position) > ChasingRange)
            {

                PetrolRandomly();
            }

            else if (Vector3.Distance(Player.transform.position, transform.position) > AttackingRange)
            {
                ChasePlayer();
            }

            else
            {

                 
            }

        }
    }

    private void ChasePlayer()
    {
        var playerTranform = Player.transform;

        _isMoving = true;
        _isChasing = true;

        var dir = (playerTranform.position - transform.position).normalized;
        RotateTowardsDir(dir);
        transform.position = Vector3.MoveTowards(transform.position, playerTranform.position, Time.deltaTime * ZombieRuningSpeed);

    }

    private void PetrolRandomly()
    {
        _isChasing = false;
        if (!_isMoving)
        {

            _targetPosition = GetRandomDeltaPosition();
            dir = (_targetPosition - transform.position).normalized;
            RotateTowardsDir(dir);
            _isMoving = true;
        }

        if (_isMoving)
        {
            RotateTowardsDir(dir);
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, Time.deltaTime*ZombieWalkingSpeed);
   
        }
        if (Vector3.Distance(transform.position, _targetPosition) == 0)
        {

            StartCoroutine(ZombieResting());


        }
    }

    private Vector3 GetRandomDeltaPosition()
    {
        float maxDeltaInPos = 7;

        float deltaInX = UnityEngine.Random.Range(-maxDeltaInPos, maxDeltaInPos);
        float deltaInZ = UnityEngine.Random.Range(-maxDeltaInPos, maxDeltaInPos);

        Vector3 DeltaPosition = new Vector3(deltaInX+transform.position.x, transform.position.y, deltaInZ+transform.position.z);

        return DeltaPosition;
    }

    private void RotateTowardsDir(Vector3 Dir)
    {
        var targetRotation = Quaternion.LookRotation(Dir);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * RotationSpeed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == Player.gameObject)
        {
            _isAlive = false;
            StartCoroutine(DieDelay());
        }
    }

    private IEnumerator DieDelay()
    {
        BloodEffect.SetActive(true);
        yield return new WaitForSeconds(DieDelayTime*0.25f);
        ZombieCapsuleCollider.isTrigger = true;
        ZombieRigidBody.isKinematic = true;
        yield return new WaitForSeconds(DieDelayTime*0.75f);
        gameObject.SetActive(false);
        _isMoving = false;

    }
    private IEnumerator ZombieResting()
    {
        yield return new WaitForSeconds(ZombieRestTime);
        _isMoving = false
            ;
    }
    #endregion

    #region Public Functions
    public bool IsMoving()
    {
        return _isMoving;
    }
    public bool IsChasing()
    {
        return _isChasing;
    }
    public bool IsAlive()
    {
        return _isAlive;
    }
    #endregion
}

