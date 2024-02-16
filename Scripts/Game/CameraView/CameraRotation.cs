using Data.Settings;
using UnityEngine;
using Universal.UI;
using Universal;
using EditorCustom.Attributes;

namespace Game.CameraView
{
    public class CameraRotation : MonoBehaviour
    {
        #region fields & properties
        private static readonly string animatorRotationState = "Rotation-360";
        [SerializeField] private Animator rotationAnimator;
        [SerializeField] private float baseSensitivity = 1f;

        [Header("Read Only")]
        [SerializeField][ReadOnly] private float sensitivityScale = 1f;
        [SerializeField][ReadOnly] private float animatorNormalizedTime;
        [SerializeField][ReadOnly] private float savedAnimatorNormalizedTime = 0f;
        #endregion fields & properties

        #region methods
        private void Start()
        {
            PlayAnimatorAtTime(savedAnimatorNormalizedTime);
        }
        private void OnEnable()
        {
            SettingsData.Data.OnGraphicsChanged += ChangeSensitivity;
            InputController.OnKeyHold += CheckKey;
            ChangeSensitivity(SettingsData.Data.GraphicsSettings);
        }
        private void OnDisable()
        {
            SettingsData.Data.OnGraphicsChanged -= ChangeSensitivity;
            InputController.OnKeyHold -= CheckKey;
        }
        private void ChangeSensitivity(GraphicsSettings gs)
        {
            sensitivityScale = gs.CameraSensitvity / 100f;
        }
        private void CheckKey(KeyCode key)
        {
            if (key != SettingsData.Data.KeyCodeSettings.CameraRotation.Key) return;
            Rotate();
        }
        public Vector2 GetRotationAxis()
        {
            Vector2 dir = CursorSettings.MouseDirection;
            return baseSensitivity * sensitivityScale * (dir / 1000f);
        }
        private void Rotate()
        {
            float move = GetRotationAxis().x;
            float normalizedTime = CustomAnimation.GetNormalizedAnimatorTime(rotationAnimator, 0) % 1;
            float time = move + normalizedTime;
            if (normalizedTime < 0.01f && move < 0)
                time = 0.9999f;
            PlayAnimatorAtTime(time);
            animatorNormalizedTime = rotationAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            savedAnimatorNormalizedTime = animatorNormalizedTime;
        }
        private void PlayAnimatorAtTime(float time) => rotationAnimator.Play(animatorRotationState, 0, time);
        #endregion methods
    }
}