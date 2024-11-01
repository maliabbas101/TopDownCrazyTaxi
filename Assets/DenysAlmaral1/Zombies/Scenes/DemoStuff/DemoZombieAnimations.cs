using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zombies
{

    public class DemoZombieAnimations : MonoBehaviour
    {
        List<Zombie> _zombies;
        private string[] _aniStates = { "z_slowwalk",  "z_idle", "z_attack_left", "z_attack_right",
                                        "z_idle2", "z_idle_alert", "z_walk", "z_running", "z_limp", "z_walk_agressive"};

        // Start is called before the first frame update
        private void Awake()
        {
            _zombies = new List<Zombie>(GetComponentsInChildren<Zombie>());
        }

        void Start()
        {
            for (int i = 0; i < _zombies.Count; i++)
            {
                var zombie = _zombies[i];
                var ani = zombie.GetComponent<Animator>();
                ani.Play(_aniStates[i]);
            }

        }


        // Update is called once per frame
        void Update()
        {

        }
    }
}
