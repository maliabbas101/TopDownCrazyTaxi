using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Zombies {
    public class ZombiePunch : MonoBehaviour
    {
        [SerializeField] ZombieMovement zombie;
      PrometeoCarController car;
        private bool canDamage;

        private void Start()
        {
            car = GameObject.FindFirstObjectByType<PrometeoCarController>();
            canDamage = true;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.name == "Collider" && zombie.IsAttacking() && canDamage)
            {
                canDamage = false;
                car.DamageCar();
            }
    }

        private void OnTriggerExit(Collider other)
        {
            canDamage = true;
        }

    }
}