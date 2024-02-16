using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.UI;

namespace Game.Shop
{
    public class MerchantAnimations : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private Animator animator;

        [Title("Layers")]
        [Header("Idle")]
        [SerializeField][Min(0)] private int layerIdle = 0;
        
        [Header("Sitting")]
        [SerializeField][Min(0)] private int layerSitting = 1;
        [SerializeField] private string sittingStartName = "Sitting-Start";
        [SerializeField] private string sittingEndName = "Sitting-End";
        [SerializeField] private string sittingLoopName = "Sitting-Loop";
        [SerializeField] private bool isSitting = false;

        #endregion fields & properties

        #region methods
        private void Start()
        {
            if (isSitting)
            {
                isSitting = false;
                StartSittingLoop();
            }
        }

        public void StartSitting()
        {
            if (isSitting) return;
            animator.Play(sittingStartName, layerSitting);
            isSitting = true;
        }
        public void StartSittingLoop()
        {
            if (isSitting) return;
            animator.Play(sittingLoopName, layerSitting);
            isSitting = true;
        }
        public void EndSitting()
        {
            if (!isSitting) return;
            animator.Play(sittingEndName, layerSitting);
            isSitting = false;
        }
        #endregion methods
    }
}