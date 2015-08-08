using UnityEngine;
using System.Collections;

public class InventoryManager : MonoBehaviour {

    public InputController inputController;

    //public GameObject barrelPrefab;
    //public GameObject bottlePrefab;
    //public GameObject mixerPrefab;

    public void Spawn(GameObject prefab)
    {
        inputController.holdingObject = true;
        inputController.heldObject = Instantiate(prefab) as GameObject;
    }
}
