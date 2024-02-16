using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal;

namespace Data.Stored
{
    [System.Serializable]
    public class PlayerData
    {
        #region fields & properties
        /// <summary>
        /// <see cref="{T0}"/> - modifier changed
        /// </summary>
        public UnityAction<Modifier> OnModifierRankIncreased;
        /// <summary>
        /// <see cref="{T0}"/> - modifier id
        /// </summary>
        public UnityAction<int> OnModifierOpened;
        public OpenedItems OpenedWeapons => openedWeapons;
        [SerializeField] private OpenedItems openedWeapons = new(new List<int>() { 0 });
        public OpenedItems OpenedItemsSkills => openedItemsSkills;
        [SerializeField] private OpenedItems openedItemsSkills = new();
        public PlayerStats Stats => stats;
        [SerializeField] private PlayerStats stats = new();
        public Wallet Wallet => wallet;
        [SerializeField] private Wallet wallet = new();
        public IReadOnlyList<Modifier> OpenedModifiers => openedModifiers.Items;
        [SerializeField] private OpenedItemsStored<Modifier> openedModifiers = new();
        #endregion fields & properties

        #region methods
        public bool TryOpenModifier(int id, out Modifier exists)
        {
            if (!openedModifiers.TryOpenItem(new(id, 0), x => x.Id == id, out exists)) return false;
            OnModifierOpened?.Invoke(id);
            return true;
        }
        public bool TryIncreaseModifierRank(int id)
        {
            if (!openedModifiers.IsOpened(x => x.Id == id, out Modifier modifier)) return false;
            return TryIncreaseModifierRank(modifier);
        }
        /// <summary>
        /// Use this if you know that 'exists' is really exists in <see cref="OpenedModifiers"/>
        /// </summary>
        /// <param name="exists"></param>
        /// <returns></returns>
        public bool TryIncreaseModifierRank(Modifier exists)
        {
            if (!exists.TryIncreaseRank()) return false;
            OnModifierRankIncreased?.Invoke(exists);
            return true;
        }
        #endregion methods
    }
}