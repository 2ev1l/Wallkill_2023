using DebugStuff;
using EditorCustom.Attributes;
using Game.DataBase;
using Game.UI;
using Overlay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Universal;
using Universal.UI;

namespace Game.Rooms
{
    public class EnemyUI : MonoBehaviour
    {
        #region fields & properties
        public Enemy Enemy => enemy;
        [HelpBox("You need to manually invoke EnableUI & DisableUI methods", HelpBoxMessageType.Info)]
        [SerializeField] private Enemy enemy;
        public LanguageInfo EnemyName => enemyName;
        [SerializeField] private LanguageInfo enemyName;
        [SerializeField] private Volume volume;
        [SerializeField][Min(0)] private float volumeWeightChangeTime = 1f;
        [SerializeField][Min(0)] private float volumeBlurChangeTime = 5f;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            enemy.Health.OnValueChanged += ChangeVolume;
            enemy.Health.OnValueReachedMinimum += BlurVolume;
        }
        private void OnDisable()
        {
            enemy.Health.OnValueChanged -= ChangeVolume;
            enemy.Health.OnValueReachedMinimum -= BlurVolume;
        }

        [Button(nameof(EnableUI))]
        [SerializedMethod]
        public void EnableUI()
        {
            EnemyStaticUI.Instance.EnableUI(this);
        }
        [Button(nameof(DisableUI))]
        [SerializedMethod]
        public void DisableUI()
        {
            EnemyStaticUI.Instance.DisableUI();
        }
        private void ChangeVolume(int _) => ChangeVolume();
        private void ChangeVolume()
        {
            Vector2 healthRange = enemy.Health.GetRange();
            float finalWeight = Mathf.InverseLerp(healthRange.y, healthRange.x, enemy.Health.Value);
            ValueTimeChanger vtc = new(volume.weight, finalWeight, volumeWeightChangeTime, ValueTimeChanger.DefaultCurve, x => volume.weight = x, delegate { volume.weight = finalWeight; });
        }
        private void BlurVolume()
        {
            if (!volume.profile.TryGet(out Bloom bloom)) return;
            float finalIntensity = 200;
            float finalThreshold = 0;
            ValueTimeChanger vtcI = new(bloom.intensity.value, finalIntensity, volumeBlurChangeTime, x => bloom.intensity.value = x);
            ValueTimeChanger vtcT = new(bloom.threshold.value, finalThreshold, volumeBlurChangeTime, x => bloom.threshold.value = x);
        }
        #endregion methods

#if UNITY_EDITOR
        private void Awake()
        {
            TestFields();
        }
        private void TestFields()
        {
            _ = enemyName.Text;
        }

#endif //UNITY_EDITOR

    }
}