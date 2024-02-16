using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Universal.UI.Layers
{
    [System.Serializable]
    public class DefaultLayers<Layer, LayerValue> where Layer : DefaultLayer<LayerValue>, new()
    {
        #region fields & properties
        public OpenedItemsStored<Layer> Layers => layers;
        [SerializeField] private OpenedItemsStored<Layer> layers = new();
        #endregion fields & properties

        #region methods
        public Layer GetLayer(int layerId)
        {
            TryAddLayer(layerId);
            return FindLayer(layerId);
        }
        protected Layer FindLayer(int layerId) => layers.Find(x => x.Id == layerId);
        protected bool TryAddLayer(int layerId)
        {
            Layer newLayer = new()
            {
                Id = layerId
            };
            return layers.TryOpenItem(newLayer, x => x.Id == layerId, out Layer exists);
        }
        #endregion methods
    }
}