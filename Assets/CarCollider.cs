using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Zombies {
    public class CarCollider : MonoBehaviour
    {
        enum ColliderType { hit, grip };
        [SerializeField] PrometeoCarController car;

        [SerializeField] ColliderType colliderType;
        [SerializeField] float carColliderActiveSpeed;
        BoxCollider collider;
        private bool _zombieStick;

        private void Start()
        {
            collider = GetComponent<BoxCollider>();
        }
        private void Update()
        {
            if (Mathf.Abs(car.carSpeed) <= carColliderActiveSpeed &collider.enabled)
            {
                collider.enabled=false;
            }
            else if(Mathf.Abs(car.carSpeed) > carColliderActiveSpeed & !collider.enabled)
            {
                collider.enabled = true;
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent<ZombieMovement>(out var zombie))
            {
                if (colliderType == ColliderType.hit)
                {
                    zombie.Die();
                }
                else if (colliderType == ColliderType.grip)
                {
                    if (_zombieStick) return;
                    _zombieStick = true;
                    zombie.StickWithCar(transform);
                    
                }
            }

        }
    }
}