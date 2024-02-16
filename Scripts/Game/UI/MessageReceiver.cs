using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EditorCustom.Attributes;
using UnityEngine;
using UnityEngine.Events;
using Universal;
using Universal.UI.Layers;

namespace Game.UI
{
    public class MessageReceiver : MonoBehaviour
    {
        #region fields & properties
        /// <summary>
        /// Invokes only if message was shown by <see cref="MessageType"/>
        /// </summary>
        public UnityAction<MessageType> OnMessageShown;
        /// <summary>
        /// Invokes only if message was shown by <see cref="string"/>
        /// </summary>
        public UnityAction<string> OnMessageShownString;
        [SerializeField] private Message prefab;
        [Title("Settings")]
        [SerializeField] private Transform spawnCanvas;
        [SerializeField] private bool changePosition = true;
        [SerializeField] private Transform startPosition;
        [SerializeField][DrawIf(nameof(changePosition), true)] private Transform endPosition;
        [SerializeField][Min(0.1f)] private float liveTime = 1f;
        [SerializeField] private TimeDelay messageDelay;
        /// <summary>
        /// yeah, it's bad but simple
        /// </summary>
        private List<object> messageQueue = new();
        [SerializeField][ReadOnly] private Message currentMessage;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            messageDelay.OnDelayReady += CheckMessages;
            if (!changePosition)
                endPosition = startPosition;
        }
        private void OnDisable()
        {
            messageDelay.OnDelayReady -= CheckMessages;
        }
        private void CheckMessages()
        {
            if (messageQueue.Count == 0) return;
            var newInfo = messageQueue.First();
            messageQueue.Remove(newInfo);
            if (newInfo.GetType() == typeof(MessageType))
            {
                ReceiveMessage((MessageType)newInfo, false);
                return;
            }
            if (newInfo.GetType() == typeof(string))
            {
                ReceiveMessage((string)newInfo);
                return;
            }
        }
        public void ReceiveMessage(MessageType info, bool ignoreQueue = false)
        {
            if (!TryShowMessage(info, ignoreQueue)) return;
            OnMessageShown?.Invoke(info);
        }
        public void ReceiveMessage(string info, bool ignoreQueue = false)
        {
            if (!TryShowMessage(info, ignoreQueue)) return;
            OnMessageShownString?.Invoke((string)info);
        }
        private bool TryShowMessage(object info, bool ignoreQueue)
        {
            if (!messageDelay.CanActivate && !ignoreQueue)
            {
                if (currentMessage != null && currentMessage.Info.Equals(info)) return false;
                if (messageQueue.Exists(x => x.Equals(info))) return false;
                messageQueue.Add(info);
                return false;
            }
            messageDelay.Activate();
            currentMessage = InstantiateMessage();
            currentMessage.Init(info, liveTime, startPosition.localPosition, endPosition.localPosition);
            return true;
        }
        private Message InstantiateMessage()
        {
            Message message = Instantiate(prefab, spawnCanvas);
            message.transform.localPosition = startPosition.localPosition;
            return message;
        }
        #endregion methods
    }
}