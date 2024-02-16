using Game.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Universal.UI.Triggers
{
    /// <summary>
    /// EXPERIMENTAL! VERY BUGGY! DO NOT USE
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class ControllerCollisionCatcher : MonoBehaviour
    {
        #region fields & properties
        public UnityAction<ControllerColliderHit> OnControllerColliderHitted;
        public UnityAction<Collider> OnColliderStay;
        public UnityAction<Collider> OnColliderExit;
        public UnityAction<Collider> OnColliderEnter;
        [SerializeField] private Moving moving;

        [SerializeField] private TimeLiveableList<Collider> timeLiveableHits = new(0.1f);
        #endregion fields & properties

        #region methods

        private void OnEnable()
        {
            timeLiveableHits.OnLiveableDead += OnColliderDead;
            timeLiveableHits.OnLiveableSpawn += OnColliderAlive;
        }
        private void OnDisable()
        {
            timeLiveableHits.OnLiveableDead -= OnColliderDead;
            timeLiveableHits.OnLiveableSpawn -= OnColliderAlive;
        }
        private void Update()
        {
            timeLiveableHits.DecreaseListTime(Time.deltaTime);
        }
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            timeLiveableHits.StackObject(hit.collider, Time.deltaTime);
        }
        private void OnColliderDead(TimeLiveable<Collider> tl)
        {
            print($"exit at {tl.Object.name}");
        }
        private void OnColliderAlive(TimeLiveable<Collider> tl)
        {
            print($"enter at {tl.Object.name}");
        }
        #endregion methods
    }
}