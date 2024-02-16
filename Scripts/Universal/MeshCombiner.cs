using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NUnit.Framework;

#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

namespace Universal
{
    internal class MeshCombiner : MonoBehaviour
    {
#if UNITY_EDITOR
        #region fields & properties
        [SerializeField] private string filePath = "Assets/Models/";
        private Transform MeshesRoot
        {
            get => useThisAsRoot ? transform : meshesRoot;
        }
        [SerializeField][DrawIf(nameof(useThisAsRoot), false)] private Transform meshesRoot;
        [SerializeField] private bool useThisAsRoot = true;
        [Tooltip("Use this if you combine meshes from the different objects")]
        [SerializeField] private bool saveObjectsWorldPosition = false;
        [SerializeField] private bool mergeSubMeshes = false;
        [SerializeField] private List<MeshFilter> meshes;
        [SerializeField][ReadOnly] Mesh generatedMesh;
        #endregion fields & properties

        #region methods
        [Button(nameof(GetAllMeshesInChild))]
        private void GetAllMeshesInChild()
        {
            meshes = MeshesRoot.GetComponentsInChildren<MeshFilter>().ToList();
        }
        [Button(nameof(ReadPath))]
        private void ReadPath()
        {
            string newPath = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", string.IsNullOrEmpty(filePath) ? "Assets/" : filePath, MeshesRoot.name + " Combined", "asset");
            if (string.IsNullOrEmpty(newPath)) return;
            filePath = FileUtil.GetProjectRelativePath(newPath);
        }

        private void Combine()
        {
            Vector3 lastMeshRootPosition = Vector3.zero;
            Vector3 lastMeshRootScale = Vector3.zero;
            Quaternion lastMeshRootRotation = Quaternion.identity;
            if (!saveObjectsWorldPosition)
                SaveAndResetRoot(out lastMeshRootPosition, out lastMeshRootScale, out lastMeshRootRotation);

            int count = meshes.Count;
            CombineInstance[] instances = new CombineInstance[count];
            for (int i = 0; i < count; ++i)
            {
                MeshFilter currentFilter = meshes[i];
                instances[i].mesh = currentFilter.sharedMesh;
                instances[i].transform = currentFilter.transform.localToWorldMatrix;
            }

            generatedMesh = new Mesh();
            generatedMesh.CombineMeshes(instances, mergeSubMeshes);

            if (!saveObjectsWorldPosition)
                RevertRootChanges(lastMeshRootPosition, lastMeshRootScale, lastMeshRootRotation);
        }
        private void SaveAndResetRoot(out Vector3 lastMeshRootPosition, out Vector3 lastMeshRootScale, out Quaternion lastMeshRootRotation)
        {
            lastMeshRootPosition = MeshesRoot.position;
            lastMeshRootScale = MeshesRoot.localScale;
            lastMeshRootRotation = MeshesRoot.rotation;

            MeshesRoot.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            MeshesRoot.eulerAngles = Vector3.zero;
            MeshesRoot.localScale = Vector3.one;
        }
        private void RevertRootChanges(Vector3 lastMeshRootPosition, Vector3 lastMeshRootScale, Quaternion lastMeshRootRotation)
        {
            MeshesRoot.SetPositionAndRotation(lastMeshRootPosition, lastMeshRootRotation);
            MeshesRoot.localScale = lastMeshRootScale;
        }
        [Button(nameof(CombineAndSave))]
        private void CombineAndSave()
        {
            Combine();
            SaveMesh(generatedMesh, true, true);
        }
        [Button(nameof(InstantiateMesh))]
        private void InstantiateMesh()
        {
            if (savedMesh == null)
            {
                Debug.LogError("You must save mesh before");
                return;
            }

            GameObject newObject = new(MeshesRoot.name + " Combined");
            newObject.transform.SetParent(MeshesRoot.parent);
            newObject.transform.SetPositionAndRotation(MeshesRoot.position, MeshesRoot.rotation);
            newObject.transform.transform.localScale = MeshesRoot.localScale;

            MeshFilter filter = newObject.AddComponent<MeshFilter>();
            filter.sharedMesh = savedMesh;

            if (!meshes.First().TryGetComponent(out MeshRenderer render)) return;
            if (render.sharedMaterial == null) return;
            MeshRenderer newRender = newObject.AddComponent<MeshRenderer>();
            newRender.sharedMaterial = render.sharedMaterial;
        }
        private void SaveMesh(Mesh mesh, bool makeNewInstance, bool optimizeMesh)
        {
            if (string.IsNullOrEmpty(filePath)) return;

            Mesh meshToSave = (makeNewInstance) ? Object.Instantiate(mesh) as Mesh : mesh;

            if (optimizeMesh)
                MeshUtility.Optimize(meshToSave);

            AssetDatabase.CreateAsset(meshToSave, filePath);
            AssetDatabase.SaveAssets();

            savedMesh = meshToSave;
        }
        [SerializeField][ReadOnly] private Mesh savedMesh;
        #endregion methods
#endif //UNITY_EDITOR
    }
}