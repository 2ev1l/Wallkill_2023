using EditorCustom.Attributes;
using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Universal;

namespace Game.Player.Bullets
{
    public class HitImpactPool : SingleSceneInstance<HitImpactPool>
    {
        #region fields & properties
        private Dictionary<HitImpact, ObjectPool<DestroyablePoolableObject>> items = new();
        #endregion fields & properties

        #region methods
        public void GetImpactData(PhysicMaterial referenceMaterial, out DestroyablePoolableObject instantiatedEffect, out AudioClip audioClip, out Texture2D decal, out Texture2D decalNormal)
        {
            HitImpactSO dbKeySO = DB.Instance.HitImpacts.Find(x => x.Data.ReferenceMaterial == referenceMaterial);
            HitImpact dbKey = dbKeySO != null ? dbKeySO.Data : DB.Instance.HitImpacts.Data.First().Data;

            if (!items.TryGetValue(dbKey, out ObjectPool<DestroyablePoolableObject> objectPool))
            {
                objectPool = new(dbKey.VisualEffectPrefab);
                objectPool.HideObjectsInHierarchy = true;
                items.TryAdd(dbKey, objectPool);
            }

            instantiatedEffect = objectPool.GetObject();
            dbKey.TryGetRandomClip(out audioClip);
            decal = dbKey.DecalTexture;
            decalNormal = dbKey.DecalNormalTexture;
#if UNITY_EDITOR
            DebugDictionary();
#endif //UNITY_EDITOR
        }

#if UNITY_EDITOR
        [Title("Debug")]
        [SerializeField] private List<HitImpact> TKeys;
        [SerializeField] private List<ObjectPool<DestroyablePoolableObject>> TValues;
        private void DebugDictionary()
        {
            TKeys = new();
            TValues = new();
            foreach (var el in items)
            {
                TKeys.Add(el.Key);
                TValues.Add(el.Value);
            }
        }
#endif //UNITY_EDITOR
        #endregion methods
    }
}