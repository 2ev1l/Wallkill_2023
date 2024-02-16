using Data.Settings;
using Data.Stored;
using DebugStuff;
using EditorCustom.Attributes;
using Game.DataBase;
using Game.Player;
using Game.Rooms;
using Game.Rooms.Mechanics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;
using Universal.UI;

namespace Game.Tutorial
{
    public class TutorialObserver : MonoBehaviour
    {
        #region fields & properties
        private TutorialData Context => GameData.Data.TutorialData;
        [SerializeField] private Collider portalCollider;
        [SerializeField] private Collider tutorialRoomStartCollider;
        [SerializeField] private HelpMessages helpMessages;

        [Title("UI")]
        [SerializeField] private List<GameObject> staminaUI;
        [SerializeField] private List<GameObject> healthUI;
        [SerializeField] private List<GameObject> weaponsUI;
        [SerializeField] private List<GameObject> tasksUI;
        [SerializeField] private List<GameObject> hiddenUI;

        [Title("Animations")]
        [SerializeField] private GameObject arrowsToFirstRoom;
        [SerializeField] private GameObject hiddenObjectInFirstRoom;
        [SerializeField] private PickupableItem crouchStageHealth;
        [SerializeField] private PickupableWeapon weaponStageWeapon;
        [SerializeField] private List<GameObject> weaponStageTargets;

        [Title("Stages")]
        [SerializeField] private TutorialStageUI movingStage;
        [SerializeField] private TutorialStageUI handsAttackStage;
        [SerializeField] private TutorialStageUI cameraRotationStage;
        [SerializeField] private TutorialStageUI sprintJumpStage;
        [SerializeField] private TutorialStageUI crouchStage;
        [SerializeField] private TutorialStageUI weaponStageUI;
        [SerializeField] private TutorialStageUI infoStage;

        [Title("Rooms")]
        [SerializeField] private Room portalRoom;
        [SerializeField] private Room tutorialRoomStart;
        [SerializeField] private Room tutorialRoomSprintJump;
        [SerializeField] private Room tutorialRoomCrouch;
        [SerializeField] private Room tutorialRoomWeapon;
        [SerializeField] private Room tutorialRoomDamage;
        [SerializeField] private Room tutorialRoomInfo;
        #endregion fields & properties

        #region methods
        private void Start()
        {
            DisableAllRooms();
            if (Context.IsCompleted)
            {
                tutorialRoomStartCollider.enabled = false;
                return;
            }
            DisableAllUI();
            OnStageWakeUp();

#if UNITY_EDITOR
            StartEditor();
#endif //UNITY_EDITOR
        }
        private void OnEnable()
        {
            if (!Context.IsCompleted)
                StatsBehaviour.Stats.Health.OnValueReachedMinimum += OnPlayerDead;
        }
        private void OnDisable()
        {
            StatsBehaviour.Stats.Health.OnValueReachedMinimum -= OnPlayerDead;
            InstancesProvider.Instance.PlayerAttack.OnOutOfAmmo -= OnWeaponAmmoIsZero;
        }
        private void OnDestroy()
        {
            weaponStageWeapon.OnPickUp = null;
            crouchStageHealth.OnPickUp = null;
            tutorialRoomStart.OnStart = null;
            tutorialRoomSprintJump.OnStart = null;
            tutorialRoomCrouch.OnStart = null;
            tutorialRoomWeapon.OnStart = null;
            movingStage.OnStageCompleted = null;
            infoStage.OnStageCompleted = null;
        }

        private void OnPlayerDead()
        {
            if (Context.IsCompleted) return;
            int deathCount = GameData.Data.StatisticData.DeathCount;
            SavingUtils.ResetTotalProgress();
            GameData.Data.StatisticData.DeathCount = deathCount;
        }
        private void OnStageWakeUp()
        {
            portalCollider.enabled = false;
            Context.CurrentStage = TutorialStage.WakeUp;
            Context.CurrentStage.TryGetMessage(out string message);
            message = message.Replace("[ID]", GameData.Data.StatisticData.PlayerId);
            helpMessages.TryShowMessage(message);
            helpMessages.OnMessageHidden += OnStageMoving;
        }
        private void OnStageMoving()
        {
            helpMessages.OnMessageHidden -= OnStageMoving;
            Context.CurrentStage = TutorialStage.Moving;
            Context.CurrentStage.TryGetMessage(out string message);
            string replaceText = "";
            replaceText += $"\n{LanguageLoader.GetTextByKeyCode(SettingsData.Data.KeyCodeSettings.CharacterSettings.MoveForward.Key)}/";
            replaceText += $"{LanguageLoader.GetTextByKeyCode(SettingsData.Data.KeyCodeSettings.CharacterSettings.MoveBackward.Key)}/";
            replaceText += $"{LanguageLoader.GetTextByKeyCode(SettingsData.Data.KeyCodeSettings.CharacterSettings.MoveLeft.Key)}/";
            replaceText += $"{LanguageLoader.GetTextByKeyCode(SettingsData.Data.KeyCodeSettings.CharacterSettings.MoveRight.Key)}";
            message = message.Replace("[WASD]", $"{replaceText}");
            helpMessages.TryShowMessage(message);
            helpMessages.OnMessageHidden += OnStageMovingInvokeUI;
        }
        private void OnStageMovingInvokeUI()
        {
            helpMessages.OnMessageHidden -= OnStageMovingInvokeUI;
            SetStageRewards(TutorialStage.Moving);
            movingStage.ActivateStage();
            movingStage.OnStageCompleted += OnStageHandsAttack;
        }
        private void OnStageHandsAttack()
        {
            movingStage.OnStageCompleted -= OnStageHandsAttack;
            movingStage.DisableStage();
            Context.CurrentStage = TutorialStage.HandsAttack;
            Context.CurrentStage.TryGetMessage(out string message);
            helpMessages.TryShowMessage(message);
            arrowsToFirstRoom.SetActive(true);
            helpMessages.OnMessageHidden += OnStageHandsAttackInvokeUI1;
        }
        private void OnStageHandsAttackInvokeUI1()
        {
            helpMessages.OnMessageHidden -= OnStageHandsAttackInvokeUI1;
            string message = LanguageLoader.GetTextByType(TextType.Game, 12);

            message = message.Replace("[AIM]", $"[{LanguageLoader.GetTextByKeyCode(SettingsData.Data.KeyCodeSettings.CharacterSettings.Aim.Key)}]");
            message = message.Replace("[FIRE]", $"[{LanguageLoader.GetTextByKeyCode(SettingsData.Data.KeyCodeSettings.CharacterSettings.Fire.Key)}]");
            helpMessages.TryShowMessage(message);
            staminaUI.ForEach(x => x.SetActive(true));
            helpMessages.OnMessageHidden += OnStageHandsAttackInvokeUI2;
        }
        private void OnStageHandsAttackInvokeUI2()
        {
            helpMessages.OnMessageHidden -= OnStageHandsAttackInvokeUI2;
            handsAttackStage.ActivateStage();
            SetStageRewards(TutorialStage.HandsAttack);
            tutorialRoomStart.OnStart += OnStageCameraRotation;
        }
        private void OnStageCameraRotation()
        {
            tutorialRoomStart.OnStart -= OnStageCameraRotation;
            handsAttackStage.DisableStage();
            arrowsToFirstRoom.SetActive(false);
            Invoke(nameof(DisablePortalRoom), 1f);
            Context.CurrentStage = TutorialStage.CameraRotation;
            hiddenObjectInFirstRoom.SetActive(false);
            Context.CurrentStage.TryGetMessage(out string message);
            message = message.Replace("[ROT]", $"[{LanguageLoader.GetTextByKeyCode(SettingsData.Data.KeyCodeSettings.CameraRotation.Key)}]");
            message = message.Replace("[CROP]", $"[{LanguageLoader.GetTextByKeyCode(SettingsData.Data.KeyCodeSettings.CameraCrop.Key)}] (+MMB)");
            helpMessages.TryShowMessage(message);
            helpMessages.OnMessageHidden += OnStageCameraRotationInvokeUI;
        }
        private void OnStageCameraRotationInvokeUI()
        {
            helpMessages.OnMessageHidden -= OnStageCameraRotationInvokeUI;
            cameraRotationStage.ActivateStage();
            SetStageRewards(TutorialStage.CameraRotation);
            hiddenObjectInFirstRoom.SetActive(true);
            tutorialRoomSprintJump.OnStart += OnStageSprintJump;
        }
        private void OnBeforeStageSprintJump()
        {
            InstancesProvider.Instance.PlayerAttack.TryEndAiming();
            string message = LanguageLoader.GetTextByType(TextType.Game, 14);
            message = message.Replace("[AIM]", $"({LanguageLoader.GetTextByKeyCode(SettingsData.Data.KeyCodeSettings.CharacterSettings.Aim.Key)})");
            helpMessages.TryShowMessage(message);
            helpMessages.OnMessageHidden += OnStageSprintJump;
        }
        private void OnStageSprintJump()
        {
            tutorialRoomSprintJump.OnStart -= OnStageSprintJump;
            helpMessages.OnMessageHidden -= OnStageSprintJump;
            cameraRotationStage.DisableStage();
            DisablePlayerInput();
            if (InstancesProvider.Instance.PlayerAttack.IsAiming)
            {
                OnBeforeStageSprintJump();
                return;
            }
            Invoke(nameof(DisableFirstRoom), 1f);
            Context.CurrentStage = TutorialStage.SprintJump;
            healthUI.ForEach(x => x.SetActive(true));
            Context.CurrentStage.TryGetMessage(out string message);
            message = message.Replace("[JUMP]", $"[{LanguageLoader.GetTextByKeyCode(SettingsData.Data.KeyCodeSettings.CharacterSettings.Jump.Key)}]");
            message = message.Replace("[RUN]", $"[{LanguageLoader.GetTextByKeyCode(SettingsData.Data.KeyCodeSettings.CharacterSettings.Run.Key)}]");
            helpMessages.TryShowMessage(message);
            helpMessages.OnMessageHidden += OnStageSprintJumpInvokeUI;
        }
        private void OnStageSprintJumpInvokeUI()
        {
            sprintJumpStage.ActivateStage();
            helpMessages.OnMessageHidden -= OnStageSprintJumpInvokeUI;
            EnablePlayerInput();
            SetStageRewards(TutorialStage.SprintJump);
            tutorialRoomCrouch.OnStart += OnStageCrouch;
        }
        private void OnStageCrouch()
        {
            DisablePlayerInput();
            tutorialRoomCrouch.OnStart -= OnStageCrouch;
            sprintJumpStage.DisableStage();
            Invoke(nameof(DisableSprintJumpRoom), 1f);
            Context.CurrentStage = TutorialStage.Crouch;
            Context.CurrentStage.TryGetMessage(out string message);
            message = message.Replace("[CROUCH]", $"[{LanguageLoader.GetTextByKeyCode(SettingsData.Data.KeyCodeSettings.CharacterSettings.Crouch.Key)}]");
            helpMessages.TryShowMessage(message);
            helpMessages.OnMessageHidden += OnStageCrouchInvokeUI;
        }
        private void OnStageCrouchInvokeUI()
        {
            EnablePlayerInput();
            helpMessages.OnMessageHidden -= OnStageCrouchInvokeUI;
            crouchStage.ActivateStage();
            SetStageRewards(TutorialStage.Crouch);
            crouchStageHealth.OnPickUp += OnStageCrouchPickupItem;
            tutorialRoomWeapon.OnStart += OnStageWeapon;
        }
        private void OnStageCrouchPickupItem()
        {
            DisablePlayerInput();
            crouchStageHealth.OnPickUp -= OnStageCrouchPickupItem;
            string message = LanguageLoader.GetTextByType(TextType.Game, 17);
            message = message.Replace("[KEY]", $"[{LanguageLoader.GetTextByKeyCode(SettingsData.Data.KeyCodeSettings.Inventory1.Key)}]");
            helpMessages.TryShowMessage(message);
            helpMessages.OnMessageHidden += OnStageCrouchPickupItemInvokeUI;
        }
        private void OnStageCrouchPickupItemInvokeUI()
        {
            EnablePlayerInput();
            helpMessages.OnMessageHidden -= OnStageCrouchPickupItemInvokeUI;
        }

        private void OnStageWeapon()
        {
            DisablePlayerInput();
            tutorialRoomWeapon.OnStart -= OnStageWeapon;
            crouchStageHealth.OnPickUp -= OnStageCrouchPickupItem;
            SetStageRewards(TutorialStage.Weapon);
            crouchStage.DisableStage();
            Invoke(nameof(DisableCrouchRoom), 1f);
            Context.CurrentStage = TutorialStage.Weapon;
            Context.CurrentStage.TryGetMessage(out string message);

            helpMessages.TryShowMessage(message);
            helpMessages.OnMessageHidden += OnStageWeaponInvokeUI;
            weaponStageWeapon.OnPickUp += OnWeaponPickUp;
        }
        private void OnStageWeaponInvokeUI()
        {
            EnablePlayerInput();
            helpMessages.OnMessageHidden -= OnStageWeaponInvokeUI;

            tutorialRoomDamage.OnStart += OnStageDamage;
        }
        private void OnWeaponPickUp()
        {
            DisablePlayerInput();
            weaponStageWeapon.OnPickUp -= OnWeaponPickUp;
            weaponStageTargets.ForEach(x => x.SetActive(true));
            weaponsUI.ForEach(x => x.SetActive(true));
            string message = LanguageLoader.GetTextByType(TextType.Game, 20);
            message = message.Replace("[AIM]", $"[{LanguageLoader.GetTextByKeyCode(SettingsData.Data.KeyCodeSettings.CharacterSettings.Aim.Key)}]");
            message = message.Replace("[PW]", $"[{LanguageLoader.GetTextByKeyCode(SettingsData.Data.KeyCodeSettings.CharacterSettings.WeaponPrev.Key)}]");
            message = message.Replace("[NW]", $"[{LanguageLoader.GetTextByKeyCode(SettingsData.Data.KeyCodeSettings.CharacterSettings.WeaponNext.Key)}]");
            message = message.Replace("[RELOAD]", $"[{LanguageLoader.GetTextByKeyCode(SettingsData.Data.KeyCodeSettings.CharacterSettings.Reload.Key)}]");
            helpMessages.TryShowMessage(message);
            helpMessages.OnMessageHidden += OnWeaponAmmoIsZero;
        }
        private void OnWeaponAmmoIsZero()
        {
            helpMessages.OnMessageHidden -= OnWeaponAmmoIsZero;
            string message = LanguageLoader.GetTextByType(TextType.Game, 19);
            message = message.Replace("[X]", $"{LanguageLoader.GetTextByType(TextType.MainMenu, 23)}");
            helpMessages.TryShowMessage(message);
            helpMessages.OnMessageHidden += OnWeaponAmmoIsZeroInvokeUI;
        }
        private void OnWeaponAmmoIsZeroInvokeUI()
        {
            helpMessages.OnMessageHidden -= OnWeaponAmmoIsZeroInvokeUI;
            weaponStageUI.ActivateStage();
            EnablePlayerInput();
        }

        private void OnStageDamage()
        {
            DisablePlayerInput();
            Invoke(nameof(DisableWeaponRoom), 1f);
            tutorialRoomDamage.OnStart -= OnStageDamage;
            weaponStageUI.DisableStage();
            Context.CurrentStage = TutorialStage.Damage;
            Context.CurrentStage.TryGetMessage(out string message);

            helpMessages.TryShowMessage(message);
            helpMessages.OnMessageHidden += OnStageDamageInvokeUI;
        }
        private void OnStageDamageInvokeUI()
        {
            EnablePlayerInput();
            helpMessages.OnMessageHidden -= OnStageDamageInvokeUI;
            tutorialRoomInfo.OnStart += OnStageInfo;
        }
        private void OnStageInfo()
        {
            tutorialRoomInfo.OnStart -= OnStageInfo;
            DisablePlayerInput();
            Invoke(nameof(DisableDamageRoom), 1f);
            Context.CurrentStage = TutorialStage.Info;
            Context.CurrentStage.TryGetMessage(out string message);
            message = message.Replace("[MEM]", $"[{LanguageLoader.GetTextByKeyCode(SettingsData.Data.KeyCodeSettings.Memory.Key)}]");
            message = message.Replace("[TAS]", $"[{LanguageLoader.GetTextByKeyCode(SettingsData.Data.KeyCodeSettings.Tasks.Key)}]");
            helpMessages.TryShowMessage(message);
            helpMessages.OnMessageHidden += OnStageInfoInvokeUI;
        }
        private void OnStageInfoInvokeUI()
        {
            EnablePlayerInput();
            SetStageRewards(Context.CurrentStage);
            infoStage.ActivateStage();
            tasksUI.ForEach(x => x.SetActive(true));
            helpMessages.OnMessageHidden -= OnStageInfoInvokeUI;
            GameData.Data.TasksData.TryStartTask(0);
            infoStage.OnStageCompleted += AfterStageInfo;
        }
        [SerializedMethod]
        public void AfterStageInfo()
        {
            if (!infoStage.IsActivated()) return;
            infoStage.DisableStage();
            DisablePlayerInput();
            infoStage.OnStageCompleted -= AfterStageInfo;
            string message = LanguageLoader.GetTextByType(TextType.Game, 23);
            helpMessages.TryShowMessage(message);
            helpMessages.OnMessageHidden += AfterStageInfoInvokeUI;
        }
        private void AfterStageInfoInvokeUI()
        {
            helpMessages.OnMessageHidden -= AfterStageInfoInvokeUI;
            Invoke(nameof(CompleteAndReloadScene), 1f);
        }
        private void CompleteAndReloadScene()
        {
            Context.TryComplete();
            SceneReloader.Instance.ReloadScene();
        }
        private void SetStageRewards(TutorialStage stage)
        {
            switch (stage)
            {
                case TutorialStage.WakeUp: break;
                case TutorialStage.Moving:
                    TryOpenKey(KeyCodeDescription.MoveForward);
                    TryOpenKey(KeyCodeDescription.MoveBackward);
                    TryOpenKey(KeyCodeDescription.MoveLeft);
                    TryOpenKey(KeyCodeDescription.MoveRight);
                    break;
                case TutorialStage.HandsAttack:
                    TryOpenKey(KeyCodeDescription.Aim);
                    TryOpenKey(KeyCodeDescription.Fire);
                    break;
                case TutorialStage.CameraRotation:
                    TryOpenKey(KeyCodeDescription.CameraRotation);
                    TryOpenKey(KeyCodeDescription.CameraCrop);
                    break;
                case TutorialStage.SprintJump:
                    TryOpenKey(KeyCodeDescription.Jump);
                    TryOpenKey(KeyCodeDescription.Run);
                    break;
                case TutorialStage.Crouch:
                    TryOpenKey(KeyCodeDescription.Crouch);
                    TryOpenKey(KeyCodeDescription.Inventory1);
                    TryOpenKey(KeyCodeDescription.Inventory2);
                    TryOpenKey(KeyCodeDescription.Inventory3);
                    TryOpenKey(KeyCodeDescription.Inventory4);
                    TryOpenKey(KeyCodeDescription.Inventory5);
                    TryOpenKey(KeyCodeDescription.Inventory6);
                    TryOpenKey(KeyCodeDescription.Inventory7);
                    TryOpenKey(KeyCodeDescription.Inventory8);
                    TryOpenKey(KeyCodeDescription.Inventory9);
                    break;
                case TutorialStage.Weapon:
                    TryOpenKey(KeyCodeDescription.WeaponNext);
                    TryOpenKey(KeyCodeDescription.WeaponPrev);
                    TryOpenKey(KeyCodeDescription.Reload);
                    break;
                case TutorialStage.Info:
                    TryOpenKey(KeyCodeDescription.Memory);
                    TryOpenKey(KeyCodeDescription.Tasks);
                    TryOpenKey(KeyCodeDescription.Modifier);
                    break;
                default: break;
            }
        }
        private void DisablePlayerInput() => InstancesProvider.Instance.PlayerInput.StopInput();
        private void EnablePlayerInput() => InstancesProvider.Instance.PlayerInput.StartInput();
        private void DisablePortalRoom() => portalRoom.gameObject.SetActive(false);
        private void DisableFirstRoom() => tutorialRoomStart.gameObject.SetActive(false);
        private void DisableSprintJumpRoom() => tutorialRoomSprintJump.gameObject.SetActive(false);
        private void DisableCrouchRoom() => tutorialRoomCrouch.gameObject.SetActive(false);
        private void DisableWeaponRoom() => tutorialRoomWeapon.gameObject.SetActive(false);
        private void DisableDamageRoom() => tutorialRoomDamage.gameObject.SetActive(false);
        private void DisableInfoRoom() => tutorialRoomInfo.gameObject.SetActive(false);
        private void DisableAllRooms()
        {
            DisableFirstRoom();
            DisableSprintJumpRoom();
            DisableCrouchRoom();
            DisableWeaponRoom();
            DisableDamageRoom();
            DisableInfoRoom();
        }
        private void DisableAllUI()
        {
            staminaUI.ForEach(x => x.SetActive(false));
            healthUI.ForEach(x => x.SetActive(false));
            weaponsUI.ForEach(x => x.SetActive(false));
            tasksUI.ForEach(x => x.SetActive(false));
            hiddenUI.ForEach(x => x.SetActive(false));
            weaponStageTargets.ForEach(x => x.SetActive(false));
        }
        private void TryOpenKey(KeyCodeDescription description) => GameData.Data.KeyCodesData.TryOpenKey(description);
        #endregion methods

#if UNITY_EDITOR
        [Title("Tests")]
        [SerializeField] private bool testAtStart = true;
        [SerializeField] private TutorialStage stageToTest = TutorialStage.Moving;
        private void StartEditor()
        {
            if (!testAtStart) return;
            TestStage();
        }
        [Button(nameof(TestStage))]
        private void TestStage()
        {
            if (!DebugCommands.IsApplicationPlaying()) return;

            helpMessages.OnMessageHidden = null;
            helpMessages.OnMessageShown = null;
            DisableAllRooms();
            helpMessages.TryHideMessageInstantly();
            for (int i = 0; i < (int)stageToTest; ++i)
                SetStageRewards((TutorialStage)i);
            if ((int)stageToTest >= (int)TutorialStage.CameraRotation) DisablePortalRoom();
            if ((int)stageToTest >= (int)TutorialStage.CameraRotation) staminaUI.ForEach(x => x.SetActive(true));
            if ((int)stageToTest >= (int)TutorialStage.SprintJump) healthUI.ForEach(x => x.SetActive(true));
            if ((int)stageToTest >= (int)TutorialStage.Damage)
            {
                GameData.Data.PlayerData.OpenedWeapons.TryOpenItem(1);
                weaponsUI.ForEach(x => x.SetActive(true));
            }

            switch (stageToTest)
            {
                case TutorialStage.WakeUp:
                    OnStageWakeUp();
                    break;
                case TutorialStage.Moving:
                    OnStageMoving();
                    break;
                case TutorialStage.HandsAttack:
                    OnStageHandsAttack();
                    break;
                case TutorialStage.CameraRotation:
                    tutorialRoomStart.gameObject.SetActive(true);
                    InstancesProvider.Instance.PlayerMoving.TeleportToUnsafe(tutorialRoomStart.SafePlayerOffset.position);
                    OnStageCameraRotation();
                    break;
                case TutorialStage.SprintJump:
                    tutorialRoomSprintJump.gameObject.SetActive(true);
                    InstancesProvider.Instance.PlayerMoving.TeleportToUnsafe(tutorialRoomSprintJump.SafePlayerOffset.position);
                    OnStageSprintJump();
                    break;
                case TutorialStage.Crouch:
                    tutorialRoomCrouch.gameObject.SetActive(true);
                    InstancesProvider.Instance.PlayerMoving.TeleportToUnsafe(tutorialRoomCrouch.SafePlayerOffset.position);
                    OnStageCrouch();
                    break;
                case TutorialStage.Weapon:
                    tutorialRoomWeapon.gameObject.SetActive(true);
                    InstancesProvider.Instance.PlayerMoving.TeleportToUnsafe(tutorialRoomWeapon.SafePlayerOffset.position);
                    OnStageWeapon();
                    break;
                case TutorialStage.Damage:
                    tutorialRoomDamage.gameObject.SetActive(true);
                    InstancesProvider.Instance.PlayerMoving.TeleportToUnsafe(tutorialRoomDamage.SafePlayerOffset.position);
                    OnStageDamage();
                    break;
                case TutorialStage.Info:
                    tutorialRoomInfo.gameObject.SetActive(true);
                    InstancesProvider.Instance.PlayerMoving.TeleportToUnsafe(tutorialRoomInfo.SafePlayerOffset.position);
                    OnStageInfo();
                    break;
            }
        }
#endif
    }
}