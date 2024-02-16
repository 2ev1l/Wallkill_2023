using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using EditorCustom.Attributes;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Universal.UI;
using Universal.UI.Layers;

namespace Game.Player
{
    public class Rigging : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Animator animator;
        [SerializeField] private Animations animations;
        [SerializeField] private Crouching crouching;
        [SerializeField] private Attack attack;
        [SerializeField] private Moving moving;

        [Title("Rigs")]
        [SerializeField] private Rig attackLayer;
        [SerializeField] private Rig weaponTargetLayer;
        [SerializeField] private Rig aimingTargetLayer;

        [Title("Bones")]
        [SerializeField] private Transform weaponOffset;
        [SerializeField] private Transform aimingPosition;
        [SerializeField] private Transform aimingDirection;
        [SerializeField] private Transform weaponTargetTransform;

        [SerializeField] private Transform leftHandWeaponTarget;
        [SerializeField] private Transform rightHandWeaponTarget;

        [Title("Settings")]
        [SerializeField] private Vector3 weaponTargetOffsetAtCrouching = -Vector3.up;

        private Vector3 startWeaponTargetLocalPosition;
        private SmoothLayer smoothWeaponTargetPosition = new();
        private Vector3 lastWorldAimingPosition;
        #endregion fields & properties

        #region methods
        private void Start()
        {
            startWeaponTargetLocalPosition = weaponTargetTransform.localPosition;
        }
        private void OnEnable()
        {
            crouching.OnCrouchingStart += SetWeaponTargetPositionOnCrouching;
            crouching.OnCrouchingStop += ResetWeaponTargetPosition;
        }
        private void OnDisable()
        {
            crouching.OnCrouchingStart -= SetWeaponTargetPositionOnCrouching;
            crouching.OnCrouchingStop -= ResetWeaponTargetPosition;
            attack.OnWeaponChanged -= SetWeaponTargetOffset;
        }
        private void SetWeaponTargetOffset() => SetWeaponTargetOffset(attack.CurrentWeapon);
        private void SetWeaponTargetOffset(Weapon newWeapon)
        {
            WeaponModel instantiated = newWeapon.Model.Instantiated;
            if (instantiated == null) return;
            rightHandWeaponTarget.transform.SetPositionAndRotation((instantiated.RightHand.position), (instantiated.RightHand.rotation));
            leftHandWeaponTarget.transform.SetPositionAndRotation((instantiated.LeftHand.position), (instantiated.LeftHand.rotation));
        }

        private void SetWeaponTargetPositionOnCrouching()
        {
            Vector3 finalPosition = startWeaponTargetLocalPosition + weaponTargetOffsetAtCrouching;
            SetWeaponTargetPosition(finalPosition, 0.7f);
        }
        private void ResetWeaponTargetPosition()
        {
            SetWeaponTargetPosition(startWeaponTargetLocalPosition, 0.7f);
        }
        private void SetWeaponTargetPosition(Vector3 newPosition, float seconds)
        {
            Vector3 startPosition = weaponTargetTransform.localPosition;
            smoothWeaponTargetPosition.ChangeLayerWeight(0, 1, seconds, x => weaponTargetTransform.localPosition = Vector3.Lerp(startPosition, newPosition, (1f / seconds) * x), delegate { weaponTargetTransform.localPosition = newPosition; });
        }
        private void SetRigWeights()
        {
            SetAttackWeights();
        }
        private void SetAttackWeights()
        {
            float attackLayerWeight = animator.GetLayerWeight(animations.LayerAttack);
            attackLayer.weight = attackLayerWeight;

            float finalAimingWeight = attack.IsAiming ? 1f : 0f;
            aimingTargetLayer.weight = finalAimingWeight;

            float weaponTargetWeight = finalAimingWeight * (attack.CurrentWeapon.Type == WeaponType.Hand ? 0f : 1f);
            weaponTargetLayer.weight = weaponTargetWeight;
        }
        private void SetConstraintsAtAiming()
        {
            Vector2 flatCharacterPosition = new(characterController.transform.position.x, characterController.transform.position.z);
            Vector2 flatAimingPointPosition = new(aimingPosition.position.x, aimingPosition.position.z);
            float minDistanceToAimingPosition = Vector2.Distance(flatCharacterPosition, flatAimingPointPosition);
            WeaponModel instantiatedModel = attack.CurrentWeapon.Model.Instantiated;

            RotateWeapon(instantiatedModel, minDistanceToAimingPosition);

            Ray aimingRay = attack.AimingRay;
            Vector2 flatPointPosition = new(CursorSettings.LastRayHit3D.point.x, CursorSettings.LastRayHit3D.point.z);
            float distanceToWeapon = Vector2.Distance(flatPointPosition, new Vector2(aimingRay.origin.x, aimingRay.origin.z));
            float distanceToPlayer = Vector2.Distance(flatPointPosition, flatCharacterPosition);
            float minDistanceToPlayer = Mathf.Min(distanceToWeapon, distanceToPlayer);
            ClampAimingConstraint(instantiatedModel, minDistanceToPlayer);
        }

        private void ClampAimingConstraint(WeaponModel instantiatedModel, float minDistanceToPlayer)
        {
            float distanceLimit = 0.2f;

            //set world position
            if (minDistanceToPlayer > distanceLimit)
                lastWorldAimingPosition = CursorSettings.LastRayHit3D.point;
            aimingPosition.position = lastWorldAimingPosition;

            if (minDistanceToPlayer < distanceLimit || instantiatedModel == null) return;
            //set const direction position
            aimingDirection.position = instantiatedModel.BulletTransform.position + instantiatedModel.BulletTransform.forward;
        }
        private void RotateWeapon(WeaponModel instantiatedModel, float minDistanceToAimingPosition)
        {
            Ray aimingRay = attack.AimingRay;
            float distanceLimit = 1.1f;

            if (minDistanceToAimingPosition < distanceLimit || instantiatedModel == null) return;
            CustomAnimation.RotateToDirection(instantiatedModel.transform, aimingRay.direction, 0.2f);
        }
        private void Update()
        {
            if (attack.IsAiming)
                SetConstraintsAtAiming();

            SetWeaponTargetOffset();

            SetRigWeights();
        }
        #endregion methods
    }
}