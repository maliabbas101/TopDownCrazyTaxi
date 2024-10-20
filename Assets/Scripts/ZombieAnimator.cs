using UnityEngine;

public class ZombieAnimator : MonoBehaviour
{
    Animator _zombieAnimator;
    [SerializeField] private Zombie Zombie_L;

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
        _zombieAnimator.SetBool("isWalking", Zombie_L.IsMoving());
        _zombieAnimator.SetBool("isChasing", Zombie_L.IsChasing());
        _zombieAnimator.SetBool("isDieing", !Zombie_L.IsAlive());

    }

    #endregion
}
