using Data.Stored;
using DebugStuff;
using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Rooms
{
    public class Enemy : MonoBehaviour
    {
        #region fields & properties
        public Stat Health => health;
        [SerializeField] private Stat health;
        [SerializeField] private DamageReceiver damageReceiver;
        #endregion fields & properties

        #region methods
        private void Awake()
        {
            damageReceiver.Init(health);
        }
        #endregion methods

#if UNITY_EDITOR
        [Title("Tests")]
        [SerializeField][DontDraw] private bool ___testBool;
        [SerializeField] private DamageProvider testDamageProvider;
        [Button(nameof(TestSimulateDamage))]
        private void TestSimulateDamage()
        {
            if (!DebugCommands.IsApplicationPlaying()) return;
            damageReceiver.SimulateCollisionDamage(testDamageProvider);
        }
#endif //UNITY_EDITOR

    }
}