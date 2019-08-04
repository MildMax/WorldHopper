using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderLayerListener : MonoBehaviour
{
    int currentLayer;

    private void Awake()
    {
        LayerAssignmentScript[] lA = Object.FindObjectsOfType<LayerAssignmentScript>();

        SetRenderLayers(lA);
    }

    private void SetRenderLayers(LayerAssignmentScript[] l)
    {

        currentLayer = l.Length;

        //Debug.Log(currentLayer);

        for (int j = 0; j != 9; ++j)
        {
            List<LayerAssignmentScript> lList = new List<LayerAssignmentScript>();

            for (int i = 0; i != l.Length; ++i)
            {
                if (l[i].layerImportance == j)
                {
                    lList.Add(l[i]);
                }
            }

            if (lList.Count != 0)
            {
                BufferLayer(j);
                if (j == 0 || j == 2 || j == 6 || j == 7)
                { SetSingle(lList); /*Debug.Log("Setting single on layer " + j);*/ }
                else
                { SetMultiple(lList); /*Debug.Log("Setting multiple on layer " + j);*/ }
                BufferLayer(j);
            }
        }

        //Debug.Log(currentLayer);
    }

    private void SetSingle(List<LayerAssignmentScript> l)
    {
        for (int i = 0; i != l.Count; ++i)
        {
            SpriteRenderer[] rends = l[i].gameObject.GetComponentsInChildren<SpriteRenderer>();
            for (int j = 0; j != rends.Length; ++j) { rends[j].sortingOrder = currentLayer; /*Debug.Log(rends[j].gameObject.name);*/ }
            
        }
        --currentLayer;
    }

    private void SetMultiple(List<LayerAssignmentScript> l)
    {
        for(int i = 0; i != 4; ++i)
        {
            for(int j = 0; j != l.Count; ++j)
            {
                if(l[j].worldNum == i)
                {
                    SpriteRenderer[] rends = l[j].gameObject.GetComponentsInChildren<SpriteRenderer>();
                    for (int h = rends.Length - 1; h >= 0; --h) { rends[h].sortingOrder = currentLayer; /*Debug.Log(rends[h].gameObject.name);*/ --currentLayer;}
                }
            }

            
        }
    }

    private void BufferLayer(int i) { if (i == 2 || i == 6) --currentLayer;  }

}
