using UnityEngine;
using UnityEngine.Events;
using EditorCustom.Attributes;

namespace Game.Player
{
    public class WeaponModel : MonoBehaviour
    {
        #region fields & properties
        public UnityAction OnInitialized;
        public UnityAction OnDisable;
        public UnityAction OnEnable;
        public GameObject Root => gameObject;
        public Transform ModelTransform => modelTransform;
        [SerializeField] private Transform modelTransform;

        public Vector3 BulletPosition => bulletTransform.position;
        public Transform BulletTransform => bulletTransform;
        [SerializeField] private Transform bulletTransform;
        public Transform LeftHand => leftHandTransform;
        [SerializeField] private Transform leftHandTransform;
        public Transform RightHand => rightHandTransform;
        [SerializeField] private Transform rightHandTransform;
        public Attack Context => context;
        [SerializeField][ReadOnly] private Attack context;
        public bool IsInitialized => isInitialized;
        [SerializeField][ReadOnly] private bool isInitialized = false;
        #endregion fields & properties

        #region methods
        public void Init(Attack context)
        {
            this.context = context;
            isInitialized = true;
            Root.transform.forward = Context.CharacterController.transform.forward;
            OnInitialized?.Invoke();
        }
        public void ChangeModelState(bool isActive)
        {
            if (isActive)
                EnableModel();
            else
                DisableModel();
        }
        public void EnableModel()
        {
            OnEnable?.Invoke();
            Root.SetActive(true);
        }
        public void DisableModel()
        {
            OnDisable?.Invoke();
            Root.SetActive(false);
        }
        #endregion methods
    }
}