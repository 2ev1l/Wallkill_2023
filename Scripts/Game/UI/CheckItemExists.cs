using Data.Stored;
using EditorCustom.Attributes;
using Game.Animations;
using Game.DataBase;
using Game.Rooms;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Universal;

namespace Game.UI
{
    public class CheckItemExists : MonoBehaviour
    {
        #region fields & properties
        public UnityEvent OnItemExistEvent;
        public UnityEvent OnItemNotExistEvent;

        [SerializeField] private ExistItemType itemType;
        [SerializeField][Min(0)] private int referenceId = 0;
        [SerializeField] private bool checkOnEnable = true;

        [SerializeField][DrawIf(nameof(itemType), ExistItemType.Modifier)] private bool anyModifierRank = true;
        [SerializeField][DrawIf(nameof(itemType), ExistItemType.Modifier)][DrawIf(nameof(anyModifierRank), false)][Min(0)] private int modifierRank = 0;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            if (checkOnEnable)
                Check();
        }
        private void Check()
        {
            if (Exists())
                OnItemExistEvent?.Invoke();
            else
                OnItemNotExistEvent?.Invoke();
        }
        public bool Exists() => itemType switch
        {
            ExistItemType.Item => InstancesProvider.Instance.PlayerInventory.ContainItem(x => x.Id == referenceId),
            ExistItemType.Weapon => GameData.Data.PlayerData.OpenedWeapons.IsOpened(referenceId),
            ExistItemType.Modifier => IsModifierExists(),
            _ => throw new System.NotImplementedException($"{itemType}"),
        };
        private bool IsModifierExists()
        {
            if (!GameData.Data.PlayerData.OpenedModifiers.Exists(x => x.Id == referenceId, out Modifier modifier)) return false;
            if (anyModifierRank) return true;
            if (modifierRank == modifier.Rank) return true;

            return false;
        }

        #endregion methods

    }
}