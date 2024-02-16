using Game.UI;
using System.Collections;
using System.Collections.Generic;
using EditorCustom.Attributes;
using UnityEngine;
using Universal;
using Game.DataBase;
using System.Linq;
using Universal.UI;
using Universal.UI.Audio;

namespace Game.Player.Bullets
{
    public class BulletAnimations : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private Bullet bullet;

        [Title("Crosshair")]
        [SerializeField] private bool enableCrosshairTrigger = true;

        [Title("Trail")]
        [SerializeField] private bool enableTrailSettings = true;
        [DrawIf(nameof(enableTrailSettings), true)][SerializeField] private TrailRenderer trailRenderer;

        [Title("Decals")]
        [SerializeField] private bool enableDecalOnHit = true;
        [DrawIf(nameof(enableDecalOnHit), true)][SerializeField] private bool enableEffectsOnHit = true;
        [DrawIf(nameof(enableDecalOnHit), true)][SerializeField] private ObjectPool<DestroyablePoolableObject> hitDecalPool;
        [DrawIf(nameof(enableDecalOnHit), true)][SerializeField] private Transform decalForward;
        [DrawIf(nameof(enableDecalOnHit), true)][SerializeField] private TimeDelay hitDecalDelay;
        [DrawIf(nameof(enableDecalOnHit), true)][SerializeField] private TimeDelay hitSoundDelay;
        [DrawIf(nameof(useDecalOnDamage), false, DisablingType.ReadOnly)]
        [DrawIf(nameof(enableDecalOnHit), true)][SerializeField] private bool useDecalAlways = true;
        [DrawIf(nameof(useDecalAlways), false, DisablingType.ReadOnly)]
        [DrawIf(nameof(enableDecalOnHit), true)][SerializeField] private bool useDecalOnDamage = false;

        private bool isSubscribed = false;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            bullet.OnSubscribedAtActions += Subscribe;
            bullet.OnUnsubscribedAtActions += UnSubscribe;
        }
        private void OnDisable()
        {
            bullet.OnSubscribedAtActions -= Subscribe;
            bullet.OnUnsubscribedAtActions -= UnSubscribe;
            if (enableTrailSettings)
                ClearTrail();
        }
        private void Subscribe()
        {
            if (isSubscribed) return;
            if (enableDecalOnHit)
            {
                if (useDecalAlways)
                    bullet.OnCollisionEntered += SpawnDecal;
            }
            isSubscribed = true;
        }
        private void UnSubscribe()
        {
            if (!isSubscribed) return;
            bullet.OnCollisionEntered -= SpawnDecal;

            hitDecalDelay.ResetDelay();
            hitSoundDelay.ResetDelay();

            isSubscribed = false;
        }
        public void OnDamageProvide(int amount)
        {
            if (enableCrosshairTrigger)
                TriggerCrosshair();

            if (enableDecalOnHit)
            {
                if (useDecalOnDamage)
                    SpawnDecal();
            }
        }
        public void ClearTrail() => trailRenderer.Clear();
        private void SpawnDecal() => SpawnDecal(bullet.LastHitCollision);
        private void SpawnDecal(Collision hitCollision)
        {
            if (!TryFindDecalPosition(out Vector3 spawnPosition, out Vector3 normal)) return;
            if (bullet.Rigidbody.velocity.magnitude < bullet.MinVelocityToDamage) return;
            DecalProjectorPoolable decal = (DecalProjectorPoolable)hitDecalPool.GetObject();
            decal.transform.position = spawnPosition;
            decal.transform.forward = normal;
            if (enableEffectsOnHit)
                SpawnDecalEffects(hitCollision.collider.sharedMaterial, spawnPosition, decal);
        }
        private void SpawnDecalEffects(PhysicMaterial hitMaterial, Vector3 spawnPosition, DecalProjectorPoolable decal)
        {
            HitImpactPool.Instance.GetImpactData(hitMaterial, out DestroyablePoolableObject effect, out AudioClip clip, out Texture2D decalTexture, out Texture2D decalNormalTexture);
            effect.transform.position = spawnPosition;
            effect.transform.forward = -bullet.LastCollisionHitDirection;
            decal.DecalProjector.material.SetTexture("Base_Map", decalTexture);
            decal.DecalProjector.material.SetTexture("Normal_Map", decalNormalTexture);
            if (clip != null && hitSoundDelay.CanActivate)
            {
                AudioManager.PlayClipAtPoint(clip, Universal.UI.Audio.AudioType.Sound, spawnPosition);
                hitSoundDelay.Activate();
            }
        }
        private bool TryFindDecalPosition(out Vector3 position, out Vector3 normal)
        {
            position = Vector3.zero;
            normal = Vector3.zero;
            //do not modify the origin for properly normal
            Vector3 origin = bullet.transform.position;
            Vector3 direction = bullet.LastCollisionHitDirection;
            if (Physics.Raycast(origin, direction, out RaycastHit hit, 3f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                position = hit.point;
                normal = hit.normal;
                return true;
            }
            if (Physics.Raycast(origin - direction * 0.1f, direction, out hit, 3f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                position = hit.point;
                normal = hit.normal;
                Debug.DrawRay(origin - direction * 0.1f, direction, Color.red);
                return true;
            }
            return false;
        }
        private void TriggerCrosshair()
        {
            Crosshair.Instance.BurstColorToRed();
        }

        #endregion methods
    }
}