using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Zombies {
    public class ZombieMovement : MonoBehaviour
    {
        #region Variables  

        [Header("Zombie Config")]
        [SerializeField] private float RotationSpeed = 10f;
        [SerializeField] private float ChasingRange = 4;
        [SerializeField] private float ZombieWalkingSpeed = 10;
        [SerializeField] private float ZombieRestTime = 1F;
        [SerializeField] private float DieDelayTime;
        [SerializeField] private float ZombieRuningSpeed;
        [SerializeField] private float AttackingRange;
        [SerializeField] private GameObject zombieModelHangPoint;
        [Header("Object Refs")]
        [SerializeField] private PrometeoCarController Car;
        [SerializeField] private GameObject BloodEffect;
        [SerializeField] private Rigidbody ZombieRigidBody;
        [SerializeField] private CapsuleCollider ZombieCapsuleCollider;
        [SerializeField] private GameObject zombieModel;
        [SerializeField] private AudioSource zombieHitAudio;
        private List<AudioSource>  zombieScreamingSound;
        private List<Limb> limbs;
        private Vector3 _targetPosition;
        private Vector3 dir;
        private bool _isMoving;
        private bool _isChasing;
        private bool _isAttacking;
        private bool _isAlive;
        private Zombie zombie;
        int limbsCount;
        [SerializeField] private float impactForce;
        private bool isJumped;
        private bool _isResting;
        private bool _isHanging;

        [SerializeField] float PullDelayTime = 2;
        #endregion


        private void Awake()
        {
            zombieHitAudio = GameObject.Find("ZombieHitSound").GetComponent<AudioSource>();
            zombieScreamingSound = new List<AudioSource>();
            zombieScreamingSound.Add(GameObject.Find("ZombieScreamingSound1").GetComponent<AudioSource>());
            zombieScreamingSound.Add(GameObject.Find("ZombieScreamingSound2").GetComponent<AudioSource>());
    
        }

        IEnumerator Screaming()
        {
            var randomScream = Random.Range(0, 2);
            yield return new WaitForSeconds(Random.Range(5, 20));
            if (!zombieScreamingSound[randomScream].isPlaying  & _isChasing)
            {
                zombieScreamingSound[randomScream].Play();
            }
            StartCoroutine(Screaming());
        }


        #region Private Functions
        private void OnEnable()
        {
            StartCoroutine(Screaming());
            limbsCount = 5;
            zombieModel = transform.GetChild(Random.Range(0, 20)).gameObject;
            zombieModel.SetActive(true);
            zombie = zombieModel.GetComponent<Zombie>();
          

            ZombieCapsuleCollider.isTrigger = false;
            ZombieRigidBody.isKinematic = false;
            ZombieRigidBody.constraints = RigidbodyConstraints.FreezeRotation;

            _isAlive = true;
            _isMoving = false;
            _isChasing = false;
            limbs = new List<Limb>();
            limbs = zombie.limbs;
            StartCoroutine(ReActiveZombie());
           
        }

        private void Start()
        {
            Car = GameObject.FindAnyObjectByType<PrometeoCarController>();


        }

        private void Update()
        {
            if (_isAlive)
            {
                if (Vector3.Distance(Car.transform.position, transform.position) > ChasingRange*3)
                {

                    ChaseCar();
                }

                else if (Vector3.Distance(Car.transform.position, transform.position) > ChasingRange)
                {

                    PetrolRandomly();
                }

                else if (Vector3.Distance(Car.transform.position, transform.position) > AttackingRange)
                {
                    ChaseCar();
                }

                else
                {

                    TackAction();
                }

            }
        }

        private void TackAction()
        {

            if (!_isHanging)
            {

                dir = (Car.transform.position - transform.position).normalized;
                RotateTowardsDir(dir);
                _isAttacking = true;
            }
            else
            {
                AdjustModelTransformWhileHanging();

            }


        }

        private void AdjustModelTransformWhileHanging()
        {
            zombieModel.transform.localPosition = Vector3.zero;


            dir = (Car.transform.position - transform.position).normalized;

            float sideCheck = Vector3.Dot(transform.right, dir);

            if (sideCheck > 0)
            {
                // On the right side
                zombieModel.transform.localRotation = Quaternion.Euler(0, 90, 0);
            }
            else
            {
                // On the left side
                zombieModel.transform.localRotation = Quaternion.Euler(0, -90, 0);
            }
        }


        private void ChaseCar()
        {
            var playerTranform = Car.transform;

            _isMoving = true;
            _isChasing = true;
            _isAttacking = false;

            var dir = (playerTranform.position - transform.position).normalized;
            RotateTowardsDir(dir);
            transform.position = Vector3.MoveTowards(transform.position, playerTranform.position, Time.deltaTime * ZombieRuningSpeed);

        }

        private void PetrolRandomly()
        {
            _isAttacking = false;
            if (_isChasing )
            {
                _isChasing = false;
                _isMoving = false;
            }
            if (!_isMoving & !_isResting)
            {

                _targetPosition = GetRandomDeltaPosition();
                dir = (_targetPosition - transform.position).normalized;
                RotateTowardsDir(dir);
                _isMoving = true;
            }

            if (_isMoving)
            {
                RotateTowardsDir(dir);
                transform.position = Vector3.MoveTowards(transform.position, _targetPosition, Time.deltaTime * ZombieWalkingSpeed);

            }
            if (Vector3.Distance(transform.position, _targetPosition) < 0.5f)
            {

                StartCoroutine(ZombieResting());


            }
        }

        private Vector3 GetRandomDeltaPosition()
        {
            float maxDeltaInPos = 15;
            float excludedMin = -3;
            float excludedMax = 3;

            float deltaInX = GetRandomOutsideRange(-maxDeltaInPos, maxDeltaInPos, excludedMin, excludedMax);
            float deltaInZ = GetRandomOutsideRange(-maxDeltaInPos, maxDeltaInPos, excludedMin, excludedMax);


            Vector3 DeltaPosition = new Vector3(deltaInX + transform.position.x, transform.position.y, deltaInZ + transform.position.z);

            return DeltaPosition;
        }
        float GetRandomOutsideRange(float min, float max, float excludeMin, float excludeMax)
        {
            float value;
            do
            {
                value = UnityEngine.Random.Range(min, max);
            }
            while (value > excludeMin && value < excludeMax);
            return value;
        }

        private void RotateTowardsDir(Vector3 Dir)
        {
            var targetRotation = Quaternion.LookRotation(Dir);
            if (Quaternion.Angle(transform.rotation, targetRotation) > 10)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * RotationSpeed);
            }
            //reset rotation
            zombieModel.transform.localRotation = Quaternion.identity;

            //reset model position to dont disturb logic position
            zombieModel.transform.localPosition = Vector3.zero;
        }

        public void Die()
        {
            _isAlive = false;
            zombieHitAudio.Play();
            StartCoroutine(UpdateInterval());
            ZombieRigidBody.constraints = RigidbodyConstraints.None;
            Vector3 hitDirection = Car.transform.position - ZombieRigidBody.position;
            hitDirection = -hitDirection.normalized;  // Get the hit direction
            ZombieRigidBody.isKinematic = false;
            ZombieRigidBody.AddForce(hitDirection * impactForce, ForceMode.Impulse);
            StartCoroutine(DieDelay());
        }

        private IEnumerator DieDelay()
        {
            zombie.bloodEffect.SetActive(true);
           yield return new WaitForSeconds(DieDelayTime * 0.25f);

            yield return new WaitForSeconds(DieDelayTime * 0.75f);
            gameObject.SetActive(false);
            _isMoving = false;

        }
        private IEnumerator ZombieResting()
        {
            _isResting = true;
            _isMoving = false;
            _isChasing = false;
            yield return new WaitForSeconds(ZombieRestTime);
            _isResting = false;



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




        IEnumerator UpdateInterval()
        {
            while (limbsCount > -1)
            {
                limbsCount--;
                var _zombie = zombie;

                if (_zombie.limbs.Count > 0)
                {
                    var i = Random.Range(0, _zombie.limbs.Count);
                    var limb = _zombie.limbs[i];

                    limb.Detach();
                //   zombie.limbs.RemoveAt(i);
                    var part = limb.skin.name;

                    //enabling ragdoll if...
                    if (part == "leg_L" || part == "leg_R" || part == "head")
                    {
                        if (!_zombie.Ragdolling) _zombie.EnableRagdoll();
                    }
                }
                yield return new WaitForSeconds(0f);
            }
        }
        IEnumerator ReActiveZombie()
        {
            foreach (var g in limbs)
            {
                g.ReActiveSkin();
            }
                yield return new WaitForSeconds(0f);
            
        }

        private void OnDisable()
        {
            zombieModel.SetActive(false);
            zombie.bloodEffect.SetActive(false);
 
            foreach (var skin in zombie.skins)
            {
                if (skin.Item2 != null)
                {
                    skin.Item2.SetActive(false);
                }
            }
        }
        public bool IsAttacking()
        {
            return _isAttacking;
        }
        public bool IsHanging()
        {
            return _isHanging;
        }
        public void StickWithCar(Transform stickColliderTransform)
        {
            //
            ZombieRigidBody.isKinematic = true;

            transform.SetParent(Car.transform);

            //adjusting zombie transform to align with collider
            transform.position = new Vector3(stickColliderTransform.position.x, stickColliderTransform.position.y, stickColliderTransform.position.z);
            transform.rotation = stickColliderTransform.rotation;

            ZombieCapsuleCollider.isTrigger = true;

            _isHanging = true;
            _isChasing = false;
            _isMoving = false;
            _isAttacking = false;

            StartCoroutine(PullCarForFlip());
        }

       IEnumerator PullCarForFlip()
        {
            yield return new WaitForSeconds(PullDelayTime);

        }

        private void OnCollisionStay(Collision collision)
        {

            if (collision.gameObject == Car.gameObject)
            {
                TackAction();
            }
        }

        #endregion
    }

}