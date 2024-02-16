using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;
using Universal.UI;
using Universal.UI.Audio;

namespace Animation
{
    public class ShatterExplosion : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private ShatterPrefab shatterPrefab;
        [SerializeField][Min(0)] private float explosionForce = 100f;
        [SerializeField][Min(0)] private float explosionRadius = 15f;
        [SerializeField][Min(0)] private float timeToDestroyParticles = 2f;
        [SerializeField] private bool useClip = false;
        [SerializeField][DrawIf(nameof(useClip), true)] private AudioClip explosionClip;
        private ShatterPrefab instantiated;
        #endregion fields & properties

        #region methods
        public void InstantiatePrefab(Transform originalObject)
        {
            instantiated = Instantiate(shatterPrefab, originalObject.position, originalObject.rotation);
        }
        /// <summary>
        /// Use <see cref="InstantiatePrefab(Transform)"/> before
        /// </summary>
        public void Explode(Vector3 atPosition)
        {
            if (instantiated == null)
            {
                Debug.LogError("You must instantiate prefab before explode");
                return;
            }

            foreach (var el in instantiated.Rigidbodies)
            {
                el.AddExplosionForce(explosionForce, atPosition, explosionRadius);
            }
            SingleGameInstance.Instance.StartCoroutine(DestroyParticles(instantiated));
            if (useClip && explosionClip != null)
                AudioManager.PlayClipAtPoint(explosionClip, Universal.UI.Audio.AudioType.Sound, instantiated.transform.position);
        }
        private IEnumerator DestroyParticles(ShatterPrefab instantiated)
        {
            yield return new WaitForSeconds(timeToDestroyParticles);
            if (instantiated == null) yield break;
            Destroy(instantiated.gameObject);
        }
        [Button(nameof(DoAtCurrentTransform))]
        private void DoAtCurrentTransform()
        {
            InstantiatePrefab(transform);
            transform.gameObject.SetActive(false);
            Explode(transform.position);
        }
        #endregion methods
    }
}