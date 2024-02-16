using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.UI.Triggers;

namespace Game.Rooms.Mechanics
{
    public class BackToSafeZone : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private bool useCollision = true;
        [SerializeField] private bool overrideSafeZone = false;
        private Transform SafeZone => overrideSafeZone ? safeZone : room.SafePlayerOffset;
        [SerializeField][DrawIf(nameof(overrideSafeZone), false)][Required] private Room room;
        [SerializeField][DrawIf(nameof(overrideSafeZone), true)][Required] private Transform safeZone;
        [SerializeField][DrawIf(nameof(useCollision), true)] private CollisionCatcher collisionCatcher;
        [SerializeField][DrawIf(nameof(useCollision), false)] private TriggerCatcher triggerCatcher;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            if (useCollision)
            {
                collisionCatcher.OnEnterTagSimple += ReturnToSafeZone;
            }
            else
            {
                triggerCatcher.OnEnterTagSimple += ReturnToSafeZone;
            }
        }
        private void OnDisable()
        {
            if (collisionCatcher != null)
                collisionCatcher.OnEnterTagSimple -= ReturnToSafeZone;
            if (triggerCatcher != null)
                triggerCatcher.OnEnterTagSimple -= ReturnToSafeZone;
        }
        public void ReturnToSafeZone()
        {
            InstancesProvider.Instance.PlayerMoving.TeleportToUnsafe(SafeZone.position);
        }
        #endregion methods
    }
}