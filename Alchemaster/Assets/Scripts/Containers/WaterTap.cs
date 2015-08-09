using UnityEngine;
using System.Collections;

public class WaterTap : Container {

    void Start()
    {
        contents = new Fluid();
    }

    void Update()
    {
        Refill(5);
        OutputFluid(1, contents);
    }

    private void Refill(int volume)
    {
        currentVolume = Mathf.Clamp(currentVolume + 5, minVolume, maxVolume);
    }
}
