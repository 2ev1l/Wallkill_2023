using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;

namespace Game.DataBase
{
    [System.Serializable]
    public class HitImpact
    {
        #region fields & properties
        public PhysicMaterial ReferenceMaterial => referenceMaterial;
        [SerializeField] private PhysicMaterial referenceMaterial;
        public DestroyablePoolableObject VisualEffectPrefab => visualEffect;
        [SerializeField] private DestroyablePoolableObject visualEffect;
        [SerializeField] private List<AudioClip> audioClips;

        public Texture2D DecalTexture => decalTexture;
        [SerializeField] private Texture2D decalTexture;
        public Texture2D DecalNormalTexture => decalNormalTexture;
        [SerializeField] private Texture2D decalNormalTexture;
        #endregion fields & properties

        #region methods
        public bool TryGetRandomClip(out AudioClip clipToPlay)
        {
            clipToPlay = null;
            if (audioClips.Count == 0) return false;
            int clipId = Random.Range(0, audioClips.Count);
            clipToPlay = audioClips[clipId];
            return true;
        }
        #endregion methods
    }
}