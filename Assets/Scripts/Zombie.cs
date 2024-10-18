using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    public float moveSpeed = 0.001f;
    public float rotationSpeed = 100f;
    [SerializeField] PrometeoCarController player;
    private Vector3 directionToHead;
    private float angleToRotae;

    void Start()
    {
        //StartCoroutine(Wander());
    }
    private void Update()
    {
        directionToHead = (player.transform.position - transform.position).normalized;
        if (Vector3.Distance(transform.position, player.transform.position)> 5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime * 2);
            angleToRotae = Mathf.Atan2(directionToHead.y, directionToHead.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0,angleToRotae,0));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime);
        }
        

    }
    IEnumerator Wander()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1f,1.1f));
            float randomRotation = Random.Range(-rotationSpeed, rotationSpeed);
            transform.Rotate(Vector3.up, randomRotation);
        }
    }
}
    