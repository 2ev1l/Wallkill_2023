using Data.Settings;
using Game.Player.Bullets;
using Game.Rooms;
using Overlay;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Universal;
using Universal.UI;

namespace Game.Player
{
    public class Input : MonoBehaviour
    {
        #region fields & properties
        public UnityAction OnInputStop;
        public UnityAction OnInputStart;

        [SerializeField] private Moving moving;
        [SerializeField] private Jumping jumping;
        [SerializeField] private Crouching crouching;
        [SerializeField] private Attack attack;
        [SerializeField] private StatsBehaviour statsBehaviour;
        [SerializeField] private KeyCodeStateMachine overlayStateMachine;

        public bool IsStopped => isStopped;
        [SerializeField] private bool isStopped = false;
        public IReadOnlyList<KeyCodeInfo> ActionKeys
        {
            get
            {
                actionKeys ??= GetPlayerInputKeys();
                return actionKeys;
            }
        }
        private static List<KeyCodeInfo> actionKeys;

        private bool isPlayerDead = false;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            InputController.OnKeyHold += CheckHoldKey;
            InputController.OnKeyDown += CheckDownKey;

            moving.OnControlledMoveStart += StopInput;
            moving.OnControlledMoveStop += StartInput;

            ScreenFade.OnBlackScreenFadeDown += StartInput;
            ScreenFade.OnBlackScreenFadeUp += StopInput;

            StatsBehaviour.Stats.Health.OnValueReachedMinimum += StopInput;

            overlayStateMachine.Context.OnStateChanged += CheckOverlayState;

            actionKeys = GetPlayerInputKeys(); //While game it's not possible to change key settings, so just collect them.
        }
        private void OnDisable()
        {
            InputController.OnKeyHold -= CheckHoldKey;
            InputController.OnKeyDown -= CheckDownKey;

            moving.OnControlledMoveStart -= StopInput;
            moving.OnControlledMoveStop -= StartInput;

            ScreenFade.OnBlackScreenFadeDown -= StartInput;
            ScreenFade.OnBlackScreenFadeUp -= StopInput;

            StatsBehaviour.Stats.Health.OnValueReachedMinimum -= StopInput;

            overlayStateMachine.Context.OnStateChanged -= CheckOverlayState;
        }
        private void CheckOverlayState(StateChange state)
        {
            if (state != overlayStateMachine.Context.DefaultState)
            {
                attack.TryEndAiming();
            }
        }
        public void StopInputFor(float seconds)
        {
            Timer timer = new()
            {
                OnChangeEnd = delegate { StartInput(); }
            };
            timer.TryStartTimer(seconds);
            StopInput();
        }
        public void StopInput()
        {
            if (isStopped) return;
            isStopped = true;
            OnInputStop?.Invoke();
        }
        public void StartInput()
        {
            if (!isStopped) return;

            isStopped = false;
            OnInputStart?.Invoke();
        }
        private static List<KeyCodeInfo> GetPlayerInputKeys() => SettingsData.Data.KeyCodeSettings.CharacterSettings.GetKeys();
        private bool TryFindKey(KeyCode key, out KeyCodeInfo pressedKey)
        {
            pressedKey = null;
            if (IsStopped) return false;
            pressedKey = actionKeys.Find(x => x.Key == key);
            if (pressedKey == null) return false;

            return true;
        }
        public bool IsInputStopped()
        {
            if (isStopped) return true;
            if (moving.IsMoveControlled) return true;
            if (overlayStateMachine != null && overlayStateMachine.Context.CurrentState != overlayStateMachine.Context.DefaultState) return true;
            if (!gameObject.activeSelf) return true;
            if (statsBehaviour.IsDead) return true;
            if (InstancesProvider.Instance.CameraController.IsDevCameraEnabled) return true;
            return false;
        }
        private void CheckDownKey(KeyCode key)
        {
            if (IsInputStopped()) return;
            if (!TryFindKey(key, out KeyCodeInfo pressedKey)) return;
            CheckDownKey(pressedKey.Description);
        }
        private void CheckHoldKey(KeyCode key)
        {
            if (IsInputStopped()) return;
            if (!TryFindKey(key, out KeyCodeInfo pressedKey)) return;
            CheckHoldKey(pressedKey.Description);
        }
        private void CheckHoldKey(KeyCodeDescription description)
        {
            switch (description)
            {
                case KeyCodeDescription.MoveForward: moving.AddInputMove(Vector2.right); break;
                case KeyCodeDescription.MoveBackward: moving.AddInputMove(-Vector2.right); break;
                case KeyCodeDescription.MoveRight: moving.AddInputMove(Vector2.up); break;
                case KeyCodeDescription.MoveLeft: moving.AddInputMove(-Vector2.up); break;
                case KeyCodeDescription.Run:
                    if (!attack.IsAiming)
                        moving.DoAccelerate = true;
                    break;
                case KeyCodeDescription.Fire: attack.TryShoot(); break;
                default: break;
            }
        }
        private void CheckDownKey(KeyCodeDescription description)
        {
            switch (description)
            {
                case KeyCodeDescription.Jump: jumping.TryJump(); break;
                case KeyCodeDescription.Aim:
                    if (attack.IsAiming) attack.TryEndAiming();
                    else attack.TryStartAiming();
                    break;
                case KeyCodeDescription.Crouch:
                    if (crouching.IsCrouching) crouching.StopCrouching();
                    else crouching.StartCrouching();
                    break;
                case KeyCodeDescription.Reload: attack.TryReload(); break;
                case KeyCodeDescription.WeaponNext: attack.ChangeWeaponFromStack(1); break;
                case KeyCodeDescription.WeaponPrev: attack.ChangeWeaponFromStack(-1); break;
                case KeyCodeDescription.Modifier: ModifierBehaviour.OnModifierActivate?.Invoke(); break;
                default: break;
            }
        }
        #endregion methods
    }
}