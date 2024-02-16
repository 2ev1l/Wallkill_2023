using Data.Interfaces;
using Data.Stored;
using EditorCustom.Attributes;
using Game.Player;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.DataBase
{
    [System.Serializable]
    public class Task : ICloneable<Task>
    {
        #region fields & properties
        public int Id => id;
        [SerializeField][Min(0)] private int id = 0;
        public WorldType World => world;
        [SerializeField] private WorldType world;

        public bool UseDescription => useDescription;
        [Title("UI")][SerializeField] private bool useDescription = false;
        public bool UsePreview => usePreview;
        [SerializeField] private bool usePreview = false;

        public LanguageInfo Name => name;
        [SerializeField] private LanguageInfo name = new(0, Universal.UI.TextType.Tasks);
        public LanguageInfo Description => description;
        [SerializeField][DrawIf(nameof(useDescription), true)] private LanguageInfo description = new(0, Universal.UI.TextType.Tasks);
        public Sprite PreviewSprite => previewSprite;
        [SerializeField][DrawIf(nameof(usePreview), true)] private Sprite previewSprite;

        public bool DoReward => doReward;
        [Title("Reward")][SerializeField] private bool doReward = false;
        public IReadOnlyList<Reward> Rewards => rewards;
        [SerializeField] private List<Reward> rewards = new();
        #endregion fields & properties

        #region methods

        public Task Clone() => new()
        {
            id = Id,
            world = World,
            name = Name,
            description = Description,
            usePreview = UsePreview,
            useDescription = UseDescription,
            previewSprite = PreviewSprite,
            doReward = DoReward,
            rewards = Rewards.ToList()
        };
        #endregion methods
    }
}