using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zombies
{

    public class DemoZombieGroup : MonoBehaviour
    {
        List<Zombie> _zombies;
        private string[] _aniStates = { "z_slowwalk",  "z_idle",
                                        "z_idle2", "z_idle_alert", "z_walk"};

        // Start is called before the first frame update
        private void Awake()
        {
            _zombies = new List<Zombie>(GetComponentsInChildren<Zombie>());
        }

        void Start()
        {
            foreach (Zombie zombie in _zombies)
            {
                var ani = zombie.GetComponent<Animator>();
                ani.Play(_aniStates[Random.Range(0, _aniStates.Length)], 0, Random.Range(0f, 1f));
            }

            StartCoroutine(UpdateInterval());
        }

        IEnumerator UpdateInterval()
        {
            while (true)
            {
                //detaching a random limp
                if (_zombies.Count == 0)
                {
                    StopCoroutine(UpdateInterval());
                    break;
                }
                var zidx = Random.Range(0, _zombies.Count);
                var zombie = _zombies[zidx];
                if (zombie.limbs.Count > 0)
                {
                    var i = Random.Range(0, zombie.limbs.Count);
                    var limb = zombie.limbs[i];
                    limb.Detach();
                    zombie.limbs.RemoveAt(i);
                    var part = limb.skin.name;

                    //enabling ragdoll if...
                    if (part == "leg_L" || part == "leg_R" || part == "head")
                    {
                        if (!zombie.Ragdolling) zombie.EnableRagdoll();
                        _zombies.RemoveAt(zidx);
                    }
                }
                yield return new WaitForSeconds(1.5f);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}