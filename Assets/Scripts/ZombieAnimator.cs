using UnityEngine;
using UnityEngine.Serialization;
using Zombies;

public class ZombieAnimator : MonoBehaviour
{
    private static readonly int IsChasing = Animator.StringToHash("isChasing");
    private static readonly int IsAttacking = Animator.StringToHash("isAttacking");
    private static readonly int IsHanging = Animator.StringToHash("isHanging");
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    Animator _zombieAnimator;
    [FormerlySerializedAs("Zombie_L")] [SerializeField] private ZombieMovement zombieL;

    #region Mono
    private void OnEnable()
    {
        transform.localPosition = Vector3.zero;
    }

    #endregion

    #region Private Functions
    private void Start()
    {
        _zombieAnimator = this.GetComponent<Animator>();
    }

    private void Update()
    {
        _zombieAnimator.SetBool(IsWalking, zombieL.IsMoving());


        _zombieAnimator.SetBool(IsChasing, zombieL.IsChasing());
        _zombieAnimator.SetBool(IsAttacking, zombieL.IsAttacking());
        _zombieAnimator.SetBool(IsHanging, zombieL.IsHanging());
    }

    #endregion
}