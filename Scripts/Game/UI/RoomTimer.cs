using EditorCustom.Attributes;
using Game.Rooms;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Universal;

namespace Game.UI
{
    public class RoomTimer : MonoBehaviour
    {
        #region fields & properties
        public UnityEvent OnTimerStartEvent;
        public UnityAction OnTimerStart;

        public UnityEvent OnTimerEndEvent;
        public UnityAction OnTimerEnd;

        public UnityEvent OnTimerStopEvent;
        public UnityAction OnTimerStop;

        [SerializeField] private Room room;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private bool invokeAtRoomStart = true;
        [SerializeField] private bool useRoomOptimalTime = true;
        [SerializeField] private bool stopAtRoomComplete = false;
        [SerializeField] private bool resetUIOnEnd = false;
        public float MaxTime => useRoomOptimalTime ? room.OptimalTimeForReward : maxTime;
        [SerializeField][DrawIf(nameof(useRoomOptimalTime), false)] private float maxTime;

        [Title("Read Only")]
        [SerializeField][ReadOnly] private float storedTime = 0f;
        [SerializeField][ReadOnly] private float startTime = -Mathf.Infinity;
        [SerializeField][ReadOnly] private bool isStarted = false;
        [SerializeField][ReadOnly] private bool isStopped = false;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            if (invokeAtRoomStart)
                room.OnStart += RestartTimer;
            if (stopAtRoomComplete)
                room.OnCompleted += StopTimer;

            UpdateUI();
        }
        private void OnDisable()
        {
            room.OnStart -= RestartTimer;
        }
        public void StopTimer()
        {
            if (!isStarted) return;
            isStarted = false;
            isStopped = true;
            OnTimerStop?.Invoke();
            OnTimerStopEvent.Invoke();
        }
        [SerializedMethod]
        public void TryRestartTimer()
        {
            if (isStarted) return;
            if (isStopped) return;
            RestartTimer();
        }

        [Button(nameof(RestartTimer))]
        public void RestartTimer()
        {
            startTime = Time.time;
            storedTime = 0;
            isStarted = true;
            UpdateUI();
            OnTimerStart?.Invoke();
            OnTimerStartEvent?.Invoke();
        }
        public void EndTimer()
        {
            if (!isStarted) return;
            storedTime = MaxTime;
            isStarted = false;
            isStopped = false;
            UpdateUI();
            OnTimerEnd?.Invoke();
            OnTimerEndEvent?.Invoke();
        }
        private void IncreaseTimer()
        {
            storedTime += Time.deltaTime;
            UpdateUI();
            if (storedTime >= MaxTime)
                EndTimer();
        }
        private void UpdateUI()
        {
            if ((MaxTime - storedTime).Approximately(0, 0.02f) && resetUIOnEnd)
            {
                text.text = $"{(MaxTime):F1} s.";
                return;
            }
            text.text = $"{(MaxTime - storedTime):F1} s.";
        }
        private void Update()
        {
            if (!isStarted) return;
            IncreaseTimer();
        }
        #endregion methods
    }
}