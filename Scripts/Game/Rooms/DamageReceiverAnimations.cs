using System;
using System.Collections;
using System.Collections.Generic;
using EditorCustom.Attributes;
using Materials;
using UnityEngine;
using UnityEngine.Events;
using Universal;
using Universal.UI.Layers;

namespace Game.Rooms
{
    public class DamageReceiverAnimations : MonoBehaviour
    {
        #region fields & properties
        public UnityEvent OnHealthReachedMinimumEvent;
        [SerializeField] private DamageReceiver damageReceiver;

        [Title("Materials")]
        [SerializeField] private bool changeMaterialsOnDamaged = true;
        [DrawIf(nameof(changeMaterialsOnDamaged), true, DisablingType.ReadOnly)][SerializeField] private List<Renderer> renderers;
        [DrawIf(nameof(changeMaterialsOnDamaged), true)][SerializeField][Min(0)] private int materialIndex = 0;
        [DrawIf(nameof(changeMaterialsOnDamaged), true)]
        [DrawIf(nameof(changeEmissionInsteadOfMaterial), false)]
        [SerializeField] private Material defaultMaterial;
        [DrawIf(nameof(changeMaterialsOnDamaged), true)]
        [DrawIf(nameof(changeEmissionInsteadOfMaterial), false)]
        [SerializeField] private Material damagedMaterial;

        [DrawIf(nameof(changeEmissionInsteadOfMaterial), true)]
        [DrawIf(nameof(changeMaterialsOnDamaged), true)]
        [SerializeField] private string emissionShaderName = "_Emission";
        [DrawIf(nameof(changeEmissionInsteadOfMaterial), true)]
        [DrawIf(nameof(changeMaterialsOnDamaged), true)]
        [ColorUsage(true, true)][SerializeField] private Color defaultEmissionColor;
        [DrawIf(nameof(changeEmissionInsteadOfMaterial), true)]
        [DrawIf(nameof(changeMaterialsOnDamaged), true)]
        [ColorUsage(true, true)][SerializeField] private Color damagedEmissionColor;
        [DrawIf(nameof(changeEmissionInsteadOfMaterial), true)]
        [DrawIf(nameof(changeMaterialsOnDamaged), true)]
        [SerializeField] private ColorLayer emissionColorLayer;

        [DrawIf(nameof(changeMaterialsOnDamaged), true)][SerializeField] private bool changeEmissionInsteadOfMaterial = false;

        [Title("Force")]
        [SerializeField] private bool useForceOnDamaged = true;
        [DrawIf(nameof(useForceOnDamaged), true)][SerializeField] private float damageForce = 1f;
        [DrawIf(nameof(useForceOnDamaged), true)][SerializeField] private bool useDirectionUp = false;
        [DrawIf(nameof(useForceOnDamaged), true)][SerializeField] private VectorLayer positionLayer;

        [DrawIf(nameof(useForceOnDamaged), true)]
        [DrawIf(nameof(useCharacterControllerToForce), false, DisablingType.ReadOnly)]
        [DrawIf(nameof(useColliderToForce), false, DisablingType.ReadOnly)]
        [SerializeField] private bool useRigidbodyToForce = true;
        [DrawIf(nameof(useForceOnDamaged), true)]
        [DrawIf(nameof(useRigidbodyToForce), true)]
        [Required][SerializeField] private Rigidbody forceRigidbody;

        [DrawIf(nameof(useForceOnDamaged), true)]
        [DrawIf(nameof(useRigidbodyToForce), false, DisablingType.ReadOnly)]
        [DrawIf(nameof(useColliderToForce), false, DisablingType.ReadOnly)]
        [SerializeField] private bool useCharacterControllerToForce = false;
        [DrawIf(nameof(useForceOnDamaged), true)]
        [DrawIf(nameof(useCharacterControllerToForce), true)]
        [Required][SerializeField] private CharacterController forceCharacterController;

        [DrawIf(nameof(useForceOnDamaged), true)]
        [DrawIf(nameof(useCharacterControllerToForce), false, DisablingType.ReadOnly)]
        [DrawIf(nameof(useRigidbodyToForce), false, DisablingType.ReadOnly)]
        [SerializeField] private bool useColliderToForce = false;
        [DrawIf(nameof(useForceOnDamaged), true)]
        [DrawIf(nameof(useColliderToForce), true)]
        [Required][SerializeField] private Collider forceCollider;
        [DrawIf(nameof(useForceOnDamaged), true)]
        [DrawIf(nameof(useColliderToForce), true)]
        [SerializeField][Min(0)] private float maxPositionDelta = 0;
        private Vector3 storedDefaultPosition = Vector3.zero;

        [Title("Shaders")]
        [SerializeField] private bool useShadersAnimation = false;
        [SerializeField][DrawIf(nameof(useShadersAnimation), true)] private DisappearMaterial disappearMaterial;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            damageReceiver.OnInvisibleStateEnd += ReturnAnimationsToDefault;
            damageReceiver.OnDamageReceived += SetDamagedAnimations;
            if (forceCollider != null) storedDefaultPosition = forceCollider.transform.position;
        }
        private void OnDisable()
        {
            damageReceiver.OnInvisibleStateEnd -= ReturnAnimationsToDefault;
            damageReceiver.OnDamageReceived -= SetDamagedAnimations;
        }
        private void SetShadersAnimation(DamageReceiver.CallbackInfo callbackInfo)
        {
            if (disappearMaterial != null)
                SetDisappearMaterialAnimation();
        }
        private void SetDisappearMaterialAnimation()
        {
            Vector2Int range = damageReceiver.Health.GetRange();
            float lerp = Mathf.InverseLerp(range.x, range.y, damageReceiver.Health.Value);
            disappearMaterial.ChangeAmount(lerp, 1f);
        }
        private void ReturnAnimationsToDefault()
        {
            if (!changeMaterialsOnDamaged) return;
            if (!changeEmissionInsteadOfMaterial)
                SetDefaultMaterial();
        }
        private void SetDamagedAnimations(DamageReceiver.CallbackInfo callbackInfo)
        {
            if (changeMaterialsOnDamaged)
            {
                if (changeEmissionInsteadOfMaterial)
                    SetDamagedEmission();
                else
                    SetDamagedMaterial();
            }

            if (useForceOnDamaged)
                UseForceOnDamaged(callbackInfo);

            if (useShadersAnimation)
                SetShadersAnimation(callbackInfo);

            if (damageReceiver.Health.IsReachedMinimum())
                OnHealthReachedMinimumEvent?.Invoke();
        }
        private void UseForceOnDamaged(DamageReceiver.CallbackInfo callbackInfo)
        {
            Transform collidedOwnPosition = callbackInfo.ColliderOwn.transform;

            if (useRigidbodyToForce)
                collidedOwnPosition = forceRigidbody.transform;
            if (useColliderToForce)
            {
                if (maxPositionDelta >= 0.001f && !storedDefaultPosition.Approximately(forceCollider.transform.position, maxPositionDelta)) return;
                collidedOwnPosition = forceCollider.transform;
            }
            if (useCharacterControllerToForce)
                collidedOwnPosition = forceCharacterController.transform;

            Vector3 direction = collidedOwnPosition.position - callbackInfo.ColliderInvoked.transform.position;
            if (!useDirectionUp)
                direction.y = 0;
            direction.Normalize();
            Vector2Int healthRange = callbackInfo.StatChanged.GetRange();
            float damageReceivedScale = ((float)callbackInfo.DamageReceived / (float)(healthRange.y - healthRange.x));
            float moveDistance = this.damageForce * Mathf.Clamp(damageReceivedScale, 0.1f, 1f);
            Vector3 motion = direction * moveDistance;
            float moveTime = Mathf.Clamp(damageReceiver.InvincibleDelay / 2, 0.1f, 1f);
            if (useCharacterControllerToForce)
            {
                positionLayer.ChangeVector(Vector3.zero, motion * (1f / moveTime), moveTime, x => forceCharacterController.Move(x * Time.deltaTime), null, null);
            }
            if (useRigidbodyToForce)
                forceRigidbody.AddForce(motion, ForceMode.Force);
            if (useColliderToForce)
            {
                positionLayer.ChangeVector(Vector3.zero, motion * (1f / moveTime), moveTime, x => forceCollider.transform.position += x * Time.deltaTime, null, null);
            }
        }
        private void SetDefaultMaterial()
        {
            renderers.ForEach(x => x.materials[materialIndex] = defaultMaterial);
        }
        private void SetDefaultEmission()
        {
            float time = Mathf.Max(damageReceiver.InvincibleDelay * 0.75f, 0.001f);
            emissionColorLayer.ChangeColor(GetRendererEmissionColor(), defaultEmissionColor, time, x => SetRendererEmissionColor(x), delegate { SetRendererEmissionColor(defaultEmissionColor); }, null);
        }

        private void SetDamagedMaterial()
        {
            renderers.ForEach(x => x.materials[materialIndex] = damagedMaterial);
        }
        private void SetDamagedEmission()
        {
            float time = Mathf.Max(damageReceiver.InvincibleDelay * 0.25f, 0.001f);
            emissionColorLayer.ChangeColor(GetRendererEmissionColor(), damagedEmissionColor, time, x => SetRendererEmissionColor(x), delegate { SetDefaultEmission(); }, null);
        }
        private void SetRendererEmissionColor(Color color) => renderers.ForEach(x => x.materials[materialIndex].SetColor(emissionShaderName, color));
        private Color GetRendererEmissionColor() => renderers[0].materials[materialIndex].GetColor(emissionShaderName);
        #endregion methods
    }
}