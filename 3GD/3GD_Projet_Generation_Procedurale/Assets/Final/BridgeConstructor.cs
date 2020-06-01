using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using World;

public class BridgeConstructor : MonoBehaviour {

    public GameObject bridgePart;

    //Dictionary that hold all bridges and their respected islands
    public Dictionary<KeyValuePair<Island,Island>, Bridge> bridges;

	// Use this for initialization
	void Start ()
    {
        bridges = new Dictionary<KeyValuePair<Island, Island>, Bridge>();
	}
	
	public void GenerateBridge(Island begin, Island end)
    {
        if (begin != end)
        {
            if (!(bridges.ContainsKey(new KeyValuePair<Island, Island>(begin, end)) || bridges.ContainsKey(new KeyValuePair<Island, Island>(end, begin))))
            {
                Bridge bridge = new Bridge(begin, end, bridgePart, gameObject);
                bridge.GenerateBridge();
                bridges.Add(new KeyValuePair<Island, Island>(begin, end), bridge);
                Debug.Log("New bridge added");
            }
        }
    }
}
