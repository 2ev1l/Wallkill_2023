using Data.Stored;
using System.Collections;
using System.Collections.Generic;
using EditorCustom.Attributes;
using UnityEngine;
using Universal.UI;
using DebugStuff;
using Universal;

namespace Game.UI
{
    public class StatBar : MonoBehaviour
    {
        #region fields & properties
        public Stat Stat
        {
            get => stat;
            set
            {
                UnSubscribe();
                stat = value;
                Subscribe();
            }
        }
        private Stat stat;
        public ProgressBar ProgressBar => progressBar;
        [Required][SerializeField] private ProgressBar progressBar;
        private bool isSubscribed = false;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            Subscribe();
        }
        private void OnDisable()
        {
            UnSubscribe();
        }
        private void Start()
        {
            SetRange();
            SetValue();
        }
        public void Subscribe()
        {
            if (isSubscribed || stat == null) return;
            Stat.OnValueChanged += SetValue;
            Stat.OnRangeChanged += SetRange;
            SetRange();
            SetValue();
            isSubscribed = true;
        }
        public void UnSubscribe()
        {
            if (!isSubscribed || stat == null) return;
            Stat.OnValueChanged -= SetValue;
            Stat.OnRangeChanged -= SetRange;
            isSubscribed = false;
        }
        private void SetRange()
        {
            progressBar.MinMaxValues = Stat.GetRange();
        }
        private void SetValue(int _) => SetValue();
        private void SetValue()
        {
            progressBar.Value = Stat.Value;
        }
        #endregion methods

#if UNITY_EDITOR
        [Title("Tests")]
        [SerializeField][DontDraw] private bool ___testBool;
        [Button(nameof(TestDecreaseStat))]
        private void TestDecreaseStat()
        {
            if (!DebugCommands.IsApplicationPlaying()) return;
            Stat.TryDecreaseValue(CustomMath.Multiply(Stat.Value, 10), out _);
        }
#endif //UNITY_EDITOR
    }
}