using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Rooms.Mechanics
{
    public class PlayerDamageProvider : MonoBehaviour
    {
        #region fields & properties
        [HelpBox("You need to invoke DoDamage method manually", HelpBoxMessageType.Info)]
        [SerializeField][Required] private Collider anyCollider;
        [SerializeField][Min(1)] private int damage;
        #endregion fields & properties

        #region methods
        [SerializedMethod]
        public void DoDamage() => DoDamage(damage);
        private void DoDamage(int amount)
        {
            InstancesProvider.Instance.PlayerDamageReceiver.TryReceiveDamage(amount, true, anyCollider, out _);
        }
        #endregion methods
    }
}