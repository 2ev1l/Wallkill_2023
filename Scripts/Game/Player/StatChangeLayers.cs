using Data.Stored;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.UI;
using Universal.UI.Layers;

namespace Game.Player
{
    [System.Serializable]
    public class StatChangeLayers : DefaultLayers<StatChangeLayer, Stat>
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        public StatChangeLayer GetLayerByStat(Stat stat) => Layers.Find(x => x.Value == stat);

        public void InitLayer(int layerId, Stat layerValue) => GetLayer(layerId).Value = layerValue;
        #endregion methods
    }
}