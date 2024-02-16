using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Data.Settings
{
    [System.Serializable]
    public class CharacterKeyCodeSettings
    {
        #region fields & properties
        public KeyCodeInfo MoveForward => moveForward;
        [SerializeField] private KeyCodeInfo moveForward = new(KeyCode.W, KeyCodeDescription.MoveForward);
        public KeyCodeInfo MoveBackward => moveBackward;
        [SerializeField] private KeyCodeInfo moveBackward = new(KeyCode.S, KeyCodeDescription.MoveBackward);
        public KeyCodeInfo MoveLeft => moveLeft;
        [SerializeField] private KeyCodeInfo moveLeft = new(KeyCode.A, KeyCodeDescription.MoveLeft);
        public KeyCodeInfo MoveRight => moveRight;
        [SerializeField] private KeyCodeInfo moveRight = new(KeyCode.D, KeyCodeDescription.MoveRight);
        public KeyCodeInfo Jump => jump;
        [SerializeField] private KeyCodeInfo jump = new(KeyCode.Space, KeyCodeDescription.Jump);
        public KeyCodeInfo Run => run;
        [SerializeField] private KeyCodeInfo run = new(KeyCode.LeftShift, KeyCodeDescription.Run);
        public KeyCodeInfo Aim => aim;
        [SerializeField] private KeyCodeInfo aim = new(KeyCode.Mouse1, KeyCodeDescription.Aim);
        public KeyCodeInfo Fire => fire;
        [SerializeField] private KeyCodeInfo fire = new(KeyCode.Mouse0, KeyCodeDescription.Fire);
        public KeyCodeInfo Crouch => crouch;
        [SerializeField] private KeyCodeInfo crouch = new(KeyCode.LeftControl, KeyCodeDescription.Crouch);
        public KeyCodeInfo Reload => reload;
        [SerializeField] private KeyCodeInfo reload = new(KeyCode.R, KeyCodeDescription.Reload);

        public KeyCodeInfo WeaponPrev => weaponPrev;
        [SerializeField] private KeyCodeInfo weaponPrev = new(KeyCode.Q, KeyCodeDescription.WeaponPrev);
        public KeyCodeInfo WeaponNext => weaponNext;
        [SerializeField] private KeyCodeInfo weaponNext = new(KeyCode.E, KeyCodeDescription.WeaponNext);
        public KeyCodeInfo Modifier => modifier;
        [SerializeField] private KeyCodeInfo modifier = new(KeyCode.F, KeyCodeDescription.Modifier);
        #endregion fields & properties

        #region methods
        /// <summary>
        /// Provides original classes.
        /// </summary>
        /// <returns></returns>
        public List<KeyCodeInfo> GetKeys()
        {
            List<KeyCodeInfo> list = new()
            {
                MoveForward,
                MoveBackward,
                MoveLeft,
                MoveRight,
                Jump,
                Run,
                Aim,
                Fire,
                Crouch,
                Reload,
                WeaponPrev,
                WeaponNext,
                Modifier
            };
            return list;
        }
        #endregion methods
    }
}