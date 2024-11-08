using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace Zombies
{
    public class Zombie : MonoBehaviour
    {
        public List<(Transform[], GameObject)> skins;
        private Rigidbody[] _ragdollRigidBodies;
        private Animator _animator;
        public GameObject bloodEffect;

        // private int limbIdx = 0;

        public List<Limb>  limbs;
        public bool Ragdolling { get; private set; }


        private void OnEnable()
        {

            _ragdollRigidBodies = GetComponentsInChildren<Rigidbody>();
            _animator = GetComponent<Animator>();           
            DisableRagdoll();
        }
        private void Awake()
        {
            skins = new List<(Transform[], GameObject)>();

            foreach (var limb in limbs)
            {
                var skinCopies = limb.MakeCopy(); // `skinCopies` is a List<(Transform[], GameObject)>

                // Add each tuple from the list returned by `MakeCopy` to `skins`
                skins.AddRange(skinCopies);

                limb.Initialize();
            }
        }


        void Start()
        {
            //_animator.Play("z_attack_left", 0, Random.Range(0f, 1f));
           
        }


        // Update is called once per frame
        void Update()
        {
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    EnableRagdoll();
            //}

            //if (Input.GetKeyDown(KeyCode.F))
            //{
            //    Debug.Log($"limbs {limbs.Count}");
            //    if (limbs.Count > limbIdx)
            //    {
            //        limbs[limbIdx].Detach();
            //        limbIdx++;
            //    }
            //}
        }

        public void DisableRagdoll()
        {
            foreach (var rg in _ragdollRigidBodies)
            {
                rg.isKinematic = true;
            }
            _animator.enabled = true;
            Ragdolling = false;
        }

        public void EnableRagdoll()
        {
            foreach (var rg in _ragdollRigidBodies)
            {
                rg.isKinematic = false;
            }
            _animator.enabled = false;
            Ragdolling = true;
        }

        public void TestSkinInfo(SkinnedMeshRenderer skin)
        {
            void Log(string s) => Debug.Log(s);

            Log("Playing with skin name: "+ skin.name);
            Log("rootbone: " + skin.rootBone);
            var s = "";
            foreach (var bone in skin.bones)
            {
                s += bone.name + ", ";
            }
            Log("Bones: "+s);
            Log($"skin Numbones: {skin.bones.Length}");


            var mesh = skin.sharedMesh;
            Log($"mesh Bindposes: {mesh.bindposes.Length}");
            var bonesPerVertex = mesh.GetBonesPerVertex();
            var boneWeights = mesh.GetAllBoneWeights();
            var boneWeightIndex = 0;
            var boneUsage = new int[mesh.bindposes.Length];
            var winfo = new List<WeightInfo>[mesh.vertexCount];
            for (var vertIndex = 0; vertIndex < mesh.vertexCount; vertIndex++) 
            {
                winfo[vertIndex] = new List<WeightInfo>();
                var vertNumBones = bonesPerVertex[vertIndex];
                for (var i = 0; i < vertNumBones; i++)
                {
                    var boneWeight = boneWeights[boneWeightIndex];
                    Log($"w: {boneWeight.weight} b: {boneWeight.boneIndex}");
                    var wi = new WeightInfo();
                    wi.weight = boneWeight.weight;
                    wi.originalBoneIndex = boneWeight.boneIndex;
                    wi.originalBone = skin.bones[boneWeight.boneIndex];
                    winfo[vertIndex].Add(wi);
                    boneUsage[boneWeight.boneIndex]++;                    
                    boneWeightIndex++;
                }
            }

            for (var i=0; i < boneUsage.Length; i++)
            {
                Log($"b {i} - {boneUsage[i]}");
            }
        }

    }

    public class WeightInfo
    {
        public float weight;
        public int originalBoneIndex;
        public int newIndex;
        public Transform originalBone;
        public Matrix4x4 originalBindPose; 
    }

    [System.Serializable]
    public class Limb
    {
        public static bool showLog = false;
        public SkinnedMeshRenderer skin;
        public Transform startBone;
        private List<Rigidbody> _rigidbodies = new();
        private List<Transform> _bones;

        private SkinnedMeshRenderer skinCopy;        
        private Transform startBoneCopy;
        private List<Rigidbody> _rigidbodiesCopy = new();
        private List<Transform> _bonesCopy;
    

        private GameObject CloneGO(GameObject go)
        {
            if (go == null) return null;
            var result = new GameObject("CC "+go.name);
            result.transform.position = go.transform.position;
            result.transform.rotation = go.transform.rotation;
            result.transform.localScale = go.transform.localScale;
            return result;
        }

        private void Log(string msg) { if (showLog) Debug.Log(msg); }

        public List<(Transform[], GameObject)> MakeCopy()
        {
            var resultList = new List<(Transform[], GameObject)>();
            if (startBone != null && skin != null)
            {
                var limbBones = startBone.GetComponentsInChildren<Transform>();
                var skinBones = skin.bones;
                var mesh = skin.sharedMesh;
                var bindPoses = mesh.bindposes;

                //foreach (var bone in skin.bones) Log(bone.name);
                var skinBonesSet = new HashSet<Transform>(skinBones);
                var definitiveBones = new List<Transform>();
                var defBindPoses = new List<Matrix4x4>();
                //intersecting sets, limbBones with skinBones but using a Foreach to ensure root is the first
                for (var i = 0; i < limbBones.Length; i++)
                {
                    var t = limbBones[i];
                    if (skinBonesSet.Contains(t))
                    {
                        definitiveBones.Add(t);
                        defBindPoses.Add(bindPoses[i]);
                    }
                }                                
                
                List<WeightInfo>[] winfo;
                GetWeightInfo(skinBones, mesh, definitiveBones,  out winfo);
                Log($"limbBones: {limbBones.Length} skin.bones: {skinBones.Length} defBones: {definitiveBones.Count}");
                CheckWeightSum(winfo);

                (var bonesCopy, var masterGoCopy) = CloneBones(definitiveBones, skin.gameObject);
                resultList.Add((bonesCopy, masterGoCopy));
                var meshCopy = BuildMesh(mesh);
                meshCopy.bindposes = BuildBindPoses(bonesCopy, masterGoCopy.transform);
                BuildWeightArrays(meshCopy, winfo);
                skinCopy = masterGoCopy.AddComponent<SkinnedMeshRenderer>();
                startBoneCopy = bonesCopy[0];
                skinCopy.bones = bonesCopy;
                skinCopy.sharedMesh = meshCopy;
                skinCopy.rootBone = bonesCopy[0];
                skinCopy.material = skin.material;

                //setting up LODs for limbs??:
                /*
                var lodGroup = masterGoCopy.AddComponent<LODGroup>();
                var lods = new LOD[1];
                lods[0] = new LOD(0.05f, new Renderer[] { skinCopy });                
                lodGroup.SetLODs(lods);
                lodGroup.RecalculateBounds();
                */

                return resultList;
            }
            else Debug.LogWarning("No limb startBone or mesh");
            return null;
        }


        private Matrix4x4[] BuildBindPoses(Transform[] bones, Transform masterTrans)
        {
            var bp = new Matrix4x4[bones.Length];
            for (int i = 0; i < bones.Length; i++)
            {
                bp[i] = bones[i].worldToLocalMatrix * masterTrans.localToWorldMatrix;
            }
            return bp;
        }

        private void BuildWeightArrays(Mesh mesh, List<WeightInfo>[] winfo)
        { 
            var bonesPerVertex = new byte[mesh.vertexCount];
            var weights = new List<BoneWeight1>();
            for (int i = 0; i < mesh.vertexCount; i++)
            {
                bonesPerVertex[i] = (byte) winfo[i].Count;
                foreach (var wi in winfo[i])
                {
                    var bw = new BoneWeight1();
                    bw.weight = wi.weight;
                    bw.boneIndex = wi.newIndex;
                    weights.Add(bw);
                }
            }
            // Create NativeArray versions of the two arrays
            var bonesPerVertexArray = new NativeArray<byte>(bonesPerVertex, Allocator.Temp);
            var weightsArray = new NativeArray<BoneWeight1>(weights.ToArray(), Allocator.Temp);
            mesh.SetBoneWeights(bonesPerVertexArray, weightsArray);
        }

        private Mesh BuildMesh(Mesh fromMesh)
        {
            var mesh = new Mesh();
            mesh.vertices = fromMesh.vertices;
            mesh.normals = fromMesh.normals;
            mesh.tangents = fromMesh.tangents;
            mesh.uv = fromMesh.uv;
            mesh.uv2 = fromMesh.uv2;
            mesh.triangles = fromMesh.triangles;
            return mesh;
        }

        private (Transform[], GameObject) CloneBones(List<Transform> bones, GameObject masterGo)
        {
            var copiedBones = new Transform[bones.Count];
            var masterGoCopy = CloneGO(masterGo);

            for (int i = 0; i < bones.Count; i++)
            {
                var go = CloneGO(bones[i].gameObject);
                copiedBones[i] = go.transform;
                var parent = bones[i].parent;
                var idxParent = bones.IndexOf(parent);
                if (parent != null)
                {
                    while (idxParent == -1)
                    {
                        if (parent.parent == null) break;
                        parent = parent.parent;
                        idxParent = bones.IndexOf(parent);
                    }
                }

                if (idxParent != -1)
                {
                    copiedBones[i].parent = copiedBones[idxParent];
                }
                else
                {
                    copiedBones[i].parent = masterGoCopy.transform;
                }
            }
            return (copiedBones, masterGoCopy);
            //relink parenting?
            
        }

        private void GetWeightInfo(Transform[] skinBones, Mesh mesh, List<Transform> definitiveBones,  out List<WeightInfo>[] winfo)
        {
            //Transfer wight info
            var definitiveBonesSet = new HashSet<Transform>(definitiveBones);
            var bonesPerVertex = mesh.GetBonesPerVertex();
            var boneWeights = mesh.GetAllBoneWeights();
            var boneWeightIndex = 0;
            int ignoredbonesWeights = 0;
            winfo = new List<WeightInfo>[mesh.vertexCount];
            for (var vertIndex = 0; vertIndex < mesh.vertexCount; vertIndex++)
            {
                winfo[vertIndex] = new List<WeightInfo>();
                var vertNumBones = bonesPerVertex[vertIndex];
                for (var i = 0; i < vertNumBones; i++)
                {
                    var boneWeight = boneWeights[boneWeightIndex];
                    var boneT = skinBones[boneWeight.boneIndex];
                    //will not discard bone?
                    if (definitiveBonesSet.Contains(boneT))
                    {
                        var wi = new WeightInfo();
                        wi.weight = boneWeight.weight;
                        wi.newIndex = definitiveBones.IndexOf(boneT);
                        if (wi.newIndex == -1) Debug.LogWarning("Bone index update not found");
                        winfo[vertIndex].Add(wi);
                    }
                    else
                    {
                        ignoredbonesWeights++;
                        //Log($"ignored b: {boneWeight.boneIndex} w: {boneWeight.weight}");
                    }
                    boneWeightIndex++;
                }
            }
            Log($"Ignored bones weights: {ignoredbonesWeights}");

        }

        private void CheckWeightSum(List<WeightInfo>[] winfo)
        {
            int numFixed = 0;
            int noWeights = 0;
            foreach (var wiList in winfo)
            {
                if (wiList.Count > 0)
                {
                    float sum = 0;
                    foreach (var wi in wiList)
                    {
                        sum += wi.weight;
                    }
                    //Log(sum.ToString());
                    if (sum < 1.0f)
                    {
                        float normalizer = 1.0f / sum;
                        foreach (var wi in wiList)
                        {
                            wi.weight *= normalizer;
                        }
                        numFixed++;
                    }
                    wiList.Sort((a, b) => b.weight.CompareTo(a.weight)); //sorting descending.
                }
                else
                {
                    //no weights? a vertex needs it
                    var wi = new WeightInfo();
                    wi.weight = 1.0f;
                    wi.newIndex = 0;
                    wiList.Add(wi);
                    noWeights++;
                }
                
            }
            Log($"normalized vert weights: {numFixed}");
            if (noWeights > 0) Log($"Orphan verts weighted to root: {noWeights}");
        }

        

        public bool Initialize()
        {
            if (startBone != null && startBoneCopy != null)
            {
                //Finding Rigidbodies from starting bone

                _bones = new(startBone.GetComponentsInChildren<Transform>());
                foreach (var b in _bones)
                {
                    var rg = b.GetComponent<Rigidbody>();
                    if (rg != null)
                    {
                        _rigidbodies.Add(rg);
                        Log(b.name);
                    }
                    
                }
                Log("-----");
                _bonesCopy = new(startBoneCopy.GetComponentsInChildren<Transform>());

                //Matching rigid bodies, and copying components                
                var notFirst = false;
                foreach (var rb in _rigidbodies)
                {
                    var bCopy = _bonesCopy.Find(x => x.name.Contains(rb.name));
                    Rigidbody rigidbodyCopy = null;
                    if (bCopy != null)
                    {
                        rigidbodyCopy = CloneRigidbody(rb, bCopy.gameObject);
                        CloneColliders(rb.gameObject, bCopy.gameObject);
                        if (notFirst)
                        {
                            var cj = rb.gameObject.GetComponent<CharacterJoint>();
                            if (cj != null) CloneCharacterJoint(cj, bCopy.gameObject);
                        }
                        notFirst = true;
                    }
                    _rigidbodiesCopy.Add(rigidbodyCopy);  //can be NULL: means not found
                }

                //fix connections
                FixConnectedBodies();
                skinCopy.gameObject.SetActive(false);
            }
            else return false;

            return true;
        }

        private Rigidbody CloneRigidbody(Rigidbody rb, GameObject dest)
        {
            var cc = dest.GetComponent<Rigidbody>();
            if (cc == null) cc = dest.AddComponent<Rigidbody>();
            cc.mass = rb.mass;
            cc.linearDamping = rb.linearDamping;
            cc.angularDamping = rb.angularDamping;
            cc.useGravity = rb.useGravity;
            cc.isKinematic = rb.isKinematic;
            cc.interpolation = rb.interpolation;
            cc.collisionDetectionMode = rb.collisionDetectionMode;
            cc.detectCollisions = rb.detectCollisions;
            return cc;
        }

        private void CloneColliders(GameObject src, GameObject dest)
        {
            // Remove all existing colliders from the destination
            foreach (var collider in dest.GetComponents<Collider>())
            {
                Object.Destroy(collider);
            }

            // Clone all BoxColliders
            foreach (var srcBoxCollider in src.GetComponents<BoxCollider>())
            {
                BoxCollider destBoxCollider = dest.AddComponent<BoxCollider>();
                destBoxCollider.center = srcBoxCollider.center;
                destBoxCollider.size = srcBoxCollider.size;
            }

            // Clone all SphereColliders
            foreach (var srcSphereCollider in src.GetComponents<SphereCollider>())
            {
                SphereCollider destSphereCollider = dest.AddComponent<SphereCollider>();
                destSphereCollider.center = srcSphereCollider.center;
                destSphereCollider.radius = srcSphereCollider.radius;
            }

            // Clone all CapsuleColliders
            foreach (var srcCapsuleCollider in src.GetComponents<CapsuleCollider>())
            {
                CapsuleCollider destCapsuleCollider = dest.AddComponent<CapsuleCollider>();
                destCapsuleCollider.center = srcCapsuleCollider.center;
                destCapsuleCollider.radius = srcCapsuleCollider.radius;
                destCapsuleCollider.height = srcCapsuleCollider.height;
                destCapsuleCollider.direction = srcCapsuleCollider.direction;
            }
        }

        private void CloneCharacterJoint(CharacterJoint src, GameObject dest)
        {
            if (src == null) return;

            CharacterJoint destJoint = dest.GetComponent<CharacterJoint>();
            if (destJoint == null) destJoint = dest.AddComponent<CharacterJoint>();

            destJoint.connectedBody = src.connectedBody;
            destJoint.anchor = src.anchor;
            destJoint.axis = src.axis;
            destJoint.autoConfigureConnectedAnchor = src.autoConfigureConnectedAnchor;
            destJoint.connectedAnchor = src.connectedAnchor;
            destJoint.swingAxis = src.swingAxis;

            destJoint.lowTwistLimit = src.lowTwistLimit;
            destJoint.highTwistLimit = src.highTwistLimit;
            destJoint.swing1Limit = src.swing1Limit;
            destJoint.swing2Limit = src.swing2Limit;
            destJoint.twistLimitSpring = src.twistLimitSpring;
            destJoint.swingLimitSpring = src.swingLimitSpring;

            destJoint.enableCollision = src.enableCollision;
            destJoint.enableProjection = src.enableProjection;
            destJoint.enablePreprocessing = src.enablePreprocessing;
        }

        private void FixConnectedBodies()
        {
            foreach (var b in _bonesCopy)
            {
                var cj = b.GetComponent<CharacterJoint>();
                if (cj != null)
                {
                    if (cj.connectedBody != null)
                    {
                        var replacement = _bonesCopy.Find(x => x.name.Contains(cj.connectedBody.name));
                        cj.connectedBody = replacement.gameObject.GetComponent<Rigidbody>();
                    }
                }
            }
        }

        public void Detach()
        {
           // if (skinCopy.gameObject.activeSelf) return;
            for (int i = 0; i < _rigidbodies.Count; i++)
            {
                var rb = _rigidbodies[i];
                var rbCopy = _rigidbodiesCopy[i];
                if (rbCopy != null)
                {
                    rbCopy.transform.position = rb.transform.position;
                    rbCopy.transform.rotation = rb.transform.rotation;
                    rbCopy.linearVelocity = rb.linearVelocity;
                    rbCopy.angularVelocity = rb.angularVelocity;
                    rb.gameObject.SetActive(false);
                    rbCopy.isKinematic = false;
                }
            }
            skin.gameObject.SetActive(false);
            skinCopy.gameObject.SetActive(true);
        }
        public void ReActiveSkin()
        {
            skin.gameObject.SetActive(true);
        }

    }
}