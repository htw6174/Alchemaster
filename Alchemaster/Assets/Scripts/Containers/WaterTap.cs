using UnityEngine;
using System.Collections;

public class WaterTap : Container {

    void Start()
    {
        contents = new Fluid();
    }

    void Update()
    {
        Refill(outputVolume * 2);
        OutputFluid(outputVolume, contents);
    }

    private void Refill(int volume)
    {
        currentVolume = Mathf.Clamp(currentVolume + volume, minVolume, maxVolume);
    }
}
