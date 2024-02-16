using Data.Interfaces;
using EditorCustom.Attributes;
using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Universal.UI;
using Universal.UI.Layers;

namespace Game.UI
{
    public class Message : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private List<SpriteRenderer> renderers;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private ValueTimeChanger alphaChanger;
        [SerializeField] private bool changePosition = true;
        [SerializeField] private bool changeAlphaTwice = false;
        [SerializeField][DrawIf(nameof(changePosition), true)] private VectorLayer positionLayer;
        public object Info => info;
        private object info;
        private bool isInitialized = false;
        #endregion fields & properties

        #region methods
        public void Init(object info, float liveTime, Vector3 startLocalPosition, Vector3 endLocalPosition)
        {
            if (isInitialized) return;
            isInitialized = true;
            this.info = info;
            if (info.GetType() == typeof(MessageType))
            {
                text.text = ((MessageType)info).GetMessage();
            }
            if (info.GetType() == typeof(string))
            {
                text.text = (string)info;
            }
            CheckAlpha(liveTime);
            if (changePosition)
                positionLayer.ChangeVector(startLocalPosition, endLocalPosition, liveTime, x => gameObject.transform.localPosition = x, onEnd: null, breakCondition: delegate { return gameObject == null; });
        }
        private void CheckAlpha(float liveTime)
        {
            if (!changeAlphaTwice)
            {
                CreateSelfDestroyableAlphaChanger(liveTime);
                return;
            }
            alphaChanger = new(text.alpha, 1, liveTime / 2, alphaChanger.Curve, ChangeAlpha, delegate { CreateSelfDestroyableAlphaChanger(liveTime / 2); });
        }
        private void CreateSelfDestroyableAlphaChanger(float liveTime)
        {
            alphaChanger = new(text.alpha, 0, liveTime, alphaChanger.Curve, ChangeAlpha,
                delegate
                {
                    if (this != null)
                        Destroy(gameObject);
                });
        }
        private void ChangeAlpha(float alpha)
        {
            text.alpha = alpha;
            foreach (var el in renderers)
            {
                if (el == null) continue;
                Color col = el.color;
                col.a = alpha;
                el.color = col;
            }
        }
        #endregion methods
    }
}