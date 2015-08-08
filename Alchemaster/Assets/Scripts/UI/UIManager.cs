using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {

    public Animator spawnMenu;

    public void SpawnMenuToggle()
    {
        spawnMenu.enabled = true;

        bool isHidden = spawnMenu.GetBool("isHidden");
        spawnMenu.SetBool("isHidden", !isHidden);
    }
}
