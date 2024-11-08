using UnityEngine;
using Zombies;

public class ZombiePunch : MonoBehaviour
{
    [SerializeField] private ZombieMovement zombie;
    private PrometeoCarController _car;
    private bool _canDamage;

    private void Start()
    {
        _car = GameObject.FindFirstObjectByType<PrometeoCarController>();
        _canDamage = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.name != "Collider" || !zombie.IsAttacking() || !_canDamage) return;
        _canDamage = false;
        _car.DamageCar();
    }
    
    private void OnTriggerExit(Collider other)
    {
        _canDamage = true;
    }
}