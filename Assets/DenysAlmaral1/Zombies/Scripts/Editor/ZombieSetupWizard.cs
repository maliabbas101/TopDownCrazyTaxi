// Creates a simple wizard that lets you create a Light GameObject
// or if the user clicks in "Apply", it will set the color of the currently
// object selected to red
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Zombies
{

    public class ZombieSetupWizard : ScriptableWizard
    {
        public bool overrideWeight = true;
        public float TotalWeight = 60f;

        public bool LimbsAutoSetup = true;

        public bool reassignMaterial = true;
        public Material material;

        [MenuItem("Tools/DA/Zombie Setup")]
        static void CreateWizard()
        {
            ScriptableWizard.DisplayWizard<ZombieSetupWizard>("Zombie Setup", "Exit", "Apply");
            //If you don't want to use the secondary button simply leave it out:
            //ScriptableWizard.DisplayWizard<BipedRagdollWizard>("BipedRagdollWizard ", "Create");
        }

        void OnWizardCreate()
        {
            if (Selection.activeTransform != null)
            {
                var myChar = Selection.activeTransform;
                Debug.Log(myChar.name);
                var RigidBodies = myChar.GetComponentsInChildren<Rigidbody>();
                float totalMass = 0;
                foreach (var rb in RigidBodies)
                {
                    //Debug.Log(rb.name);
                    totalMass += rb.mass;
                }
                Debug.Log($"Total Mass: {totalMass} kg");
                helpString = "Current hierarchy total mass: " + totalMass.ToString();
            }
        }

        void OnWizardUpdate()
        {
            // Debug.Log("Somethign changed. -Wizard guy");
        }

        // When the user presses the "Apply" button OnWizardOtherButton is called.
        void OnWizardOtherButton()
        {
            if (Selection.activeTransform != null)
            {
                var myChar = Selection.activeTransform;
                if (overrideWeight)
                {                    
                    Debug.Log(myChar.name);
                    var RigidBodies = myChar.GetComponentsInChildren<Rigidbody>();
                    float totalMass = 0;
                    foreach (var rb in RigidBodies)
                    {
                        totalMass += rb.mass;

                    }
                    Debug.Log("Current total mass: " + totalMass);
                    float factor = TotalWeight / totalMass;
                    foreach (var rb in RigidBodies)
                    {
                        rb.mass *= factor;
                    }
                }

                if (LimbsAutoSetup)
                {
                    var allObjects = myChar.GetComponentsInChildren<Transform>();                    
                    Dictionary<string, Transform> objectsDict = allObjects.ToDictionary(go => go.name);
                    var limbs = new List<Limb>();

                    void addLimb(string skin, string startBone)
                    {
                        if (objectsDict.TryGetValue(skin, out var skinObj))
                        {
                            var newLimb = new Limb();
                            newLimb.skin = skinObj.GetComponent<SkinnedMeshRenderer>();
                            newLimb.startBone = objectsDict[startBone];
                            limbs.Add(newLimb);
                            Debug.Log("Added limb: "+skin+" - "+startBone);
                        }
                    }
                    addLimb("arm_L", "bip L UpperArm");
                    addLimb("arm_R", "bip R UpperArm");
                    addLimb("head", "bip Head");
                    addLimb("leg_L", "bip L Calf");
                    addLimb("leg_R", "bip R Calf");

                    var zombie = myChar.GetComponent<Zombie>();
                    if (zombie != null)
                    {
                        zombie.limbs = limbs;
                    }
                    else Debug.LogWarning("Zombie script component no found!");                   
                }

                if (reassignMaterial)
                {
                    var allSkins = myChar.GetComponentsInChildren<SkinnedMeshRenderer>();
                    var allMesh = myChar.GetComponentsInChildren<MeshRenderer>();
                    foreach (var skin in allSkins)
                    {
                        skin.material = material;
                    }
                    foreach (var mesh in allMesh)
                    {
                        mesh.material = material;
                    }

                }

            }
        }
    }
}