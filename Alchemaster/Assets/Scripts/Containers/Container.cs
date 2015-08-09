using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Container : MonoBehaviour {

    public Fluid contents;
    public int maxVolume = 1000; //Capacity of the container in mL
    public int minVolume = 0; //Shouldn't ever be anything other than 0, but can't hurt to be adjustable
    public int currentVolume;

    public Text volumeDisplay;

    public Connector[] connections;

    private MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
    }

    void Update()
    {
        UpdateVolumeDisplay();
        //UpdateColor();
    }

    private void UpdateVolumeDisplay()
    {
        if (volumeDisplay)
        {
            string formattedCurrent = string.Format("{0:F3}", (currentVolume / 1000f));
            string formattedMax = string.Format("{0:F3}", (maxVolume / 1000f));
            volumeDisplay.text = formattedCurrent + " Liters /\n" + formattedMax + " Liters";
        }
    }

    private void UpdateColor()
    {
        if (contents == null)
        {
            meshRenderer.material.SetColor("_Color", Color.white);
            Debug.Log("Setting " + gameObject.name + meshRenderer.material.color);
        }
        else
        {
            meshRenderer.material.SetColor("_Color", contents.color);
        }
    }

    //Called to insert fluid into the container. Adds fluid to the container and returns the volume of fluid input
    public virtual int InputFluid(int volume, Fluid fluid)
    {
        if (contents == null || contents.SameSolution(fluid))
        {
            if (currentVolume + volume > maxVolume)
            {
                currentVolume = maxVolume;
                return maxVolume - currentVolume;
            }
            currentVolume += volume;
            return volume;
        }
        else if (!contents.SameSolution(fluid))
        {
            Debug.Log("Error! " + contents.name + " is not the same fluid as " + fluid.name);
        }
        return 0;
    }

    //Attempts to remove fluid from the container through each output pipe. Removes fluid from the container and returns volume of fluid output
    public virtual int OutputFluid(int volume, Fluid fluid)
    {
        foreach (Connector connector in connections)
        {
            if (connector.output == true)
            {
                int outputVolume = Mathf.Min(volume, currentVolume);
                outputVolume = connector.OutputFluid(outputVolume, fluid);
                currentVolume -= outputVolume;
                return outputVolume;
            }
        }
        return 0;
    }
}
