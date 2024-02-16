using Game.DataBase;
using EditorCustom.Attributes;
using UnityEngine;
using Universal.UI;
using Universal.UI.Layers;
using Universal.UI.Audio;
using Game.UI;
using Animation;
using Data.Stored;
using UnityEngine.UIElements;
using Data.Settings;
using Universal;

namespace Game.Player
{
    public class Animations : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private CharacterController characterController;
        [SerializeField] private CharacterPhysics characterPhysics;
        [SerializeField] private Jumping jumping;
        [SerializeField] private Moving moving;
        [SerializeField] private Attack attack;
        [SerializeField] private Crouching crouching;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform aimingPosition;
        [SerializeField] private MessageReceiver smallMessageReceiver;
        [SerializeField] private ShatterExplosion shatterExplosionEffect;
        [SerializeField] private Inventory inventory;

        public int LayerWalk => layerWalk;
        [Title("Animator Properties")]
        [SerializeField][Min(0)] private int layerWalk = 1;
        public int LayerCrouching => layerCrouching;
        [SerializeField][Min(0)] private int layerCrouching = 2;
        public int LayerRun => layerRun;
        [SerializeField][Min(0)] private int layerRun = 3;
        public int LayerJump => layerJump;
        [SerializeField][Min(0)] private int layerJump = 4;
        public int LayerAttack => layerAttack;
        [SerializeField][Min(0)] private int layerAttack = 6;

        [Title("Settings")]
        [SerializeField][Range(0f, 1f)] private float rotationTime = 0.1f;
        [SerializeField] private AudioClip groundedClip;
        private bool isRotatingWithCamera = false;
        private bool isIdleAnimationRandomized = false;
        private bool isSubscribedAtWeapon = false;

        private SmoothLayers<AnimatorSmoothLayer> animatorLayers = new();
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            moving.AfterPossibleMove += UpdateAnimatorMove;

            moving.OnControlledMoveStart += DisableRotateWithCamera;
            moving.OnControlledMoveStop += EnableRotateWithCamera;

            jumping.OnFalled += PlayClipOnGrounded;

            crouching.OnCrouchingStart += SetAnimatorAtCrouchingStart;
            crouching.OnCrouchingStop += SetAnimatorAtCrouchingStop;

            attack.OnAimingStart += StartAnimationAtAiming;
            attack.OnAimingEnd += StopAnimationAtAiming;

            attack.OnWeaponChanged += SubscribeAtWeaponChange;
            attack.OnWeaponChanged += StartAnimationAtAiming;
            attack.OnBeforeWeaponChanged += UnSubscribeBeforeWeaponChange;

            attack.OnShoot += PlayClipAtShoot;
            attack.OnReloadStart += PlayClipAtReload;

            inventory.OnItemUsed += PlayClipOnItemUsed;

            StatsBehaviour.Stats.Stamina.OnFailedToDecrease += OnStaminaDecreaseFailed;
            StatsBehaviour.Stats.Health.OnValueReachedMinimum += OnDead;

            EnableRotateWithCamera();
            StartIdleAnimation();
            SubscribeAtWeaponChange(attack.CurrentWeapon);
        }
        private void OnDisable()
        {
            moving.AfterPossibleMove -= UpdateAnimatorMove;

            moving.OnControlledMoveStart -= DisableRotateWithCamera;
            moving.OnControlledMoveStop -= EnableRotateWithCamera;

            jumping.OnFalled -= PlayClipOnGrounded;

            crouching.OnCrouchingStart -= SetAnimatorAtCrouchingStart;
            crouching.OnCrouchingStop -= SetAnimatorAtCrouchingStop;

            attack.OnAimingStart -= StartAnimationAtAiming;
            attack.OnAimingEnd -= StopAnimationAtAiming;

            attack.OnWeaponChanged -= SubscribeAtWeaponChange;
            attack.OnWeaponChanged -= StartAnimationAtAiming;
            attack.OnBeforeWeaponChanged -= UnSubscribeBeforeWeaponChange;

            attack.OnShoot -= PlayClipAtShoot;
            attack.OnReloadStart -= PlayClipAtReload;

            inventory.OnItemUsed -= PlayClipOnItemUsed;

            StatsBehaviour.Stats.Stamina.OnFailedToDecrease -= OnStaminaDecreaseFailed;
            StatsBehaviour.Stats.Health.OnValueReachedMinimum -= OnDead;

            DisableRotateWithCamera();
            CancelInvoke(nameof(RandomizeIdleAnimation));
        }
        private void PlayClipOnItemUsed(int itemId)
        {
            Item item = DB.Instance.Items.GetObjectById(itemId).Data;
            if (item.TryGetUseSound(out AudioClip sound))
                AudioManager.PlayClipAtPoint(sound, Universal.UI.Audio.AudioType.Sound, characterController.transform.position + Vector3.up);
        }
        private void PlayClipOnGrounded()
        {
            AudioManager.PlayClipAtPoint(groundedClip, Universal.UI.Audio.AudioType.Sound, characterController.transform.position);
        }
        private void PlayClipAtShoot()
        {
            if (attack.CurrentWeapon.TryGetShootClip(out AudioClip shootClip))
            {
                AudioManager.PlayClipAtPoint(shootClip, Universal.UI.Audio.AudioType.Sound, attack.CurrentWeapon.Model.Instantiated.transform.position);
            }
        }
        private void PlayClipAtReload()
        {
            if (attack.CurrentWeapon.TryGetReloadClip(out AudioClip reloadClip))
            {
                AudioManager.PlayClipAtPoint(reloadClip, Universal.UI.Audio.AudioType.Sound, attack.CurrentWeapon.Model.Instantiated.transform.position);
            }
        }

        private void OnDead()
        {
            characterController.gameObject.SetActive(false);
            shatterExplosionEffect.InstantiatePrefab(characterController.transform);
            shatterExplosionEffect.Explode(characterController.transform.position);
        }
        private void OnStaminaDecreaseFailed()
        {
            smallMessageReceiver.ReceiveMessage(MessageType.StaminaIsOver);
        }
        private void StartIdleAnimation()
        {
            if (isIdleAnimationRandomized) return;
            InvokeRepeating(nameof(RandomizeIdleAnimation), 1f, 5f);
            isIdleAnimationRandomized = true;
        }
        private void ResetIdleAnimation()
        {
            if (!isIdleAnimationRandomized) return;
            CancelInvoke(nameof(RandomizeIdleAnimation));
            isIdleAnimationRandomized = false;
            animator.SetInteger("RandomIdle", 0);
        }

        private void SetAnimatorAtCrouchingStart()
        {
            ChangeLayerValue(layerCrouching, 0.75f, 0.4f);
        }
        private void SetAnimatorAtCrouchingStop()
        {
            ChangeLayerValue(layerCrouching, 0, 0.4f);
        }

        private void SubscribeAtWeaponChange(Weapon newWeapon)
        {
            if (isSubscribedAtWeapon) return;
            newWeapon.ShootDelay.OnActivated += StartAnimationAtShooting;
            newWeapon.ShootDelay.OnDelayReady += StopAnimationAtShooting;
            attack.OnAimingEnd += StopAnimationAtShooting;
            isSubscribedAtWeapon = true;
        }
        private void UnSubscribeBeforeWeaponChange(Weapon oldWeapon)
        {
            oldWeapon.ShootDelay.OnActivated -= StartAnimationAtShooting;
            oldWeapon.ShootDelay.OnDelayReady -= StopAnimationAtShooting;
            attack.OnAimingEnd -= StopAnimationAtShooting;
            isSubscribedAtWeapon = false;
            StopAnimationAtShooting();
        }

        private void StartAnimationAtAiming(Weapon newWeapon) => StartAnimationAtAiming();
        private void StartAnimationAtAiming()
        {
            DisableRotateWithCamera();
            ChangeLayerValue(layerAttack, 1, 0.5f);
            SetAnimatorAimingBool();
            SetAnimatorAttackType();
            ResetIdleAnimation();
        }
        private void StopAnimationAtAiming()
        {
            EnableRotateWithCamera();
            ChangeLayerValue(layerAttack, 0, 0.5f);
            SetAnimatorAimingBool();
            SetAnimatorAttackType();
            StartIdleAnimation();
        }
        private void StartAnimationAtShooting()
        {
            if (animator != null)
                SetAnimatorShootingBool(true);
        }
        private void StopAnimationAtShooting()
        {
            if (animator != null)
                SetAnimatorShootingBool(false);
        }
        private void SetAnimatorShootingBool(bool isShooting) => animator.SetBool("IsShooting", isShooting);
        private void SetAnimatorAimingBool() => animator.SetBool("IsAiming", attack.IsAiming);
        private void SetAnimatorAttackType() => animator.SetInteger("AttackType", (int)attack.CurrentWeapon.Type);

        [SerializedMethod]
        public void ChangePlayerForawrdAxis(Transform newTransformAxis)
        {
            bool isRotatedWithCamera = isRotatingWithCamera;
            DisableRotateWithCamera();
            characterController.transform.forward = newTransformAxis.forward;
            if (isRotatedWithCamera)
                EnableRotateWithCamera();
        }
        [SerializedMethod]
        public void DisableRotateWithCamera()
        {
            if (!isRotatingWithCamera) return;
            moving.OnMoved -= RotateWithCamera;
            isRotatingWithCamera = false;
        }
        [SerializedMethod]
        public void EnableRotateWithCamera()
        {
            if (moving.IsMoveControlled || attack.IsAiming || isRotatingWithCamera) return;
            moving.OnMoved += RotateWithCamera;
            isRotatingWithCamera = true;
        }

        [SerializedMethod]
        public void ChangeLayerValueUp(int layerId) => ChangeLayerValue(layerId, 1, 1);
        [SerializedMethod]
        public void ChangeLayerValueDown(int layerId) => ChangeLayerValue(layerId, 0, 1);
        public void ChangeLayerValue(int layerId, float weight, float seconds)
        {
            animatorLayers.GetLayer(layerId).ChangeLayerWeight(animator, layerId, weight, seconds);
        }

        private void LateUpdate()
        {
            UpdateAnimatorJump();
            if (attack.IsAiming)
                RotateAtAimingPoint();
        }
        private void UpdateAnimatorJump()
        {
            float finalJumpWeight = jumping.ClampedAltitude;
            float lerpedJumpWieght = LerpAnimatorLayer(layerJump, finalJumpWeight, Time.deltaTime * 10);
            animator.SetLayerWeight(layerJump, lerpedJumpWieght);
        }
        private void UpdateAnimatorMove(bool isMoved)
        {
            animator.SetFloat("Speed", characterPhysics.CurrentSpeed);
            if (attack.IsAiming && moving.IsAlwaysMovingWithCamera)
            {
                moving.CalculateMoveVector(moving.LastInput, characterController.transform, out _, out Vector3 moveDirectionPlayer);
                moving.CalculateMoveVector(moving.LastInput, moving.CameraTransform, out _, out Vector3 moveDirectionCamera);
                Matrix4x4 coordinateSystem = Matrix4x4.TRS(Vector3.zero, characterController.transform.rotation, Vector3.one);
                Vector3 localDirection = moveDirectionPlayer;
                Vector3 transformedDirection = coordinateSystem.MultiplyPoint(moveDirectionCamera);
                float eulerPlayerAngle = Mathf.Abs(180 - characterController.transform.eulerAngles.y);
                if (eulerPlayerAngle.Approximately(90, 45))
                {
                    transformedDirection.z *= -1;
                    transformedDirection.x *= -1;
                }

                animator.SetFloat("MoveX", transformedDirection.z);
                animator.SetFloat("MoveY", transformedDirection.x);
            }
            else
            {
                animator.SetFloat("MoveX", moving.LastInput.x);
                animator.SetFloat("MoveY", moving.LastInput.y);
            }


            float finalMoveWeight = isMoved ? 1f : 0f;
            finalMoveWeight *= attack.IsAiming ? 0.7f : 1f;
            float lerpedMoveWeight = LerpAnimatorLayer(layerWalk, finalMoveWeight, Time.deltaTime * 10);
            animator.SetLayerWeight(layerWalk, lerpedMoveWeight);

            float finalRunWeight = moving.DoAccelerate ? 1 : 0;
            finalRunWeight *= isMoved ? 1 : 0;
            finalRunWeight *= moving.IsMoveControlled ? 0 : 1;

            float lerpedRunWeight = LerpAnimatorLayer(layerRun, finalRunWeight, Time.deltaTime * 5);
            animator.SetLayerWeight(layerRun, lerpedRunWeight);

            float finalAttackWeight = animator.GetLayerWeight(layerAttack);
            if (moving.DoAccelerate && isMoved)
            {
                float attackAccelerateWeight = attack.CurrentWeapon.Type == WeaponType.Hand ? 0.5f : 0.2f;
                if (finalAttackWeight > attackAccelerateWeight)
                    finalAttackWeight = attackAccelerateWeight;
            }
            else
            {
                finalAttackWeight = attack.IsAiming ? 1 : 0;
            }
            float lerpedAttackWeight = LerpAnimatorLayer(layerAttack, finalAttackWeight, Time.deltaTime * 5);
            animator.SetLayerWeight(layerAttack, lerpedAttackWeight);
        }

        private void RandomizeIdleAnimation(bool _) => RandomizeIdleAnimation();
        private void RandomizeIdleAnimation()
        {
            int rnd = Random.Range(0, 100);
            animator.SetInteger("RandomIdle", rnd);
        }

        private float LerpAnimatorLayer(int layer, float b, float t) => Mathf.Lerp(animator.GetLayerWeight(layer), b, t);
        private void RotateWithCamera()
        {
            Vector3 cameraForward = Camera.main.transform.forward;
            CustomAnimation.RotateToDirectionForward(characterController.transform, cameraForward, rotationTime);
        }

        private void RotateAtAimingPoint()
        {
            Vector2 flatCharacterPosition = new(characterController.transform.position.x, characterController.transform.position.z);
            Vector2 flatAimingPointPosition = new(aimingPosition.position.x, aimingPosition.position.z);
            float minDistanceToAimingPosition = Vector2.Distance(flatCharacterPosition, flatAimingPointPosition);
            float distanceLimit = attack.CurrentWeapon.Type == WeaponType.Hand ? 0.3f : 0.5f;
            if (minDistanceToAimingPosition < distanceLimit) return;
            CustomAnimation.RotateToDirectionForward(characterController.transform, aimingPosition.position - characterController.transform.position, 0.2f);
        }
        #endregion methods
    }
}