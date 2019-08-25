using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_RandomizeItems : MonoBehaviour
{
    public GameObject[] objects;
    public Vector2[] locations;

    string[] sequences = { "012", "021", "102", "120", "201", "210" };

    private void Awake()
    {
        RandomizeObjects();
    }

    private void RandomizeObjects()
    {
        int[] arr = ArraySplitter.SplitString(sequences[Random.Range(0, sequences.Length - 1)]);
        int[] arr1 = ArraySplitter.SplitString(sequences[Random.Range(0, sequences.Length - 1)]);

        for (int i = 0; i != arr.Length; ++i)
        {
            GameObject g = Instantiate(objects[arr[i]], locations[arr1[i]], Quaternion.identity);
            g.transform.SetParent(GameObject.FindGameObjectWithTag("GameController").transform);
        }
    }
}

static public class ArraySplitter
{
    static public int[] SplitString(string h)
    {
        int[] l = new int[h.Length];

        for(int i = 0; i != l.Length; ++i)
        {
            Debug.Log((int)char.GetNumericValue(h[i]));
            l[i] = (int)char.GetNumericValue(h[i]);
        }

        return l;
    }
}
