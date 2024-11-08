using System;
using Unity.VisualScripting;
using UnityEngine;
using Zombies;

public class CarCollider : MonoBehaviour
{
    private enum ColliderType { Hit = 0, Grip = 1
    };
    [SerializeField] private PrometeoCarController car;

    [SerializeField] private ColliderType colliderType;
    [SerializeField] private float carColliderActiveSpeed;
    private BoxCollider _collider;
    public bool _zombieStick;
    private ZombieMovement zombieAttachedToCollider;
    private bool colliderHittingFloor;

    private void Start()
    {
        _collider = GetComponent<BoxCollider>();
    }
    private void Update()
    {
        if (colliderType == ColliderType.Hit)
        {
            if (Mathf.Abs(car.carSpeed) <= carColliderActiveSpeed & _collider.enabled)
            {
                _collider.enabled = false;
            }
            else if (Mathf.Abs(car.carSpeed) > carColliderActiveSpeed & !_collider.enabled)
            {
                _collider.enabled = true;
            }
        }
        else
        {
            if (car.carSpeed <= 10)
            {
                _collider.enabled = false;
            }
            if (Mathf.Abs(car.carSpeed) <= carColliderActiveSpeed & !_collider.enabled &car.carSpeed>10)
            {
                _collider.enabled = true;
            }
            else if (Mathf.Abs(car.carSpeed) > carColliderActiveSpeed & _collider.enabled)
            {
                _collider.enabled = false;
            }
        }

        if(car.transform.rotation.eulerAngles.z>=90 && car.transform.rotation.eulerAngles.z<=270 && zombieAttachedToCollider.IsAlive())
        {
            zombieAttachedToCollider.Die();
            car.DestroyCar();
        }
        


    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Floor")
        {
             colliderHittingFloor = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Floor")
        {
            colliderHittingFloor = true;
        }
        if (!other.gameObject.TryGetComponent<ZombieMovement>(out var zombie)) return;
        zombieAttachedToCollider = zombie;
        if(!zombie.IsAlive())return;
        switch (colliderType)
        {
            case ColliderType.Hit:
                zombie.Die();
                break;
            case ColliderType.Grip when _zombieStick:
                return;
            case ColliderType.Grip :
                _zombieStick = true;
                zombie.StickWithCar(transform,this);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    

}