using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Container : MonoBehaviour {

    public Fluid contents;
    public int maxVolume = 10000; //Capacity of the container in mL
    public int minVolume = 0; //Shouldn't ever be anything other than 0, but can't hurt to be adjustable
    public int currentVolume;

    public int outputVolume;

    public Text volumeDisplay;

    public Connector[] connections;

    private MeshRenderer meshRenderer;

    public virtual void Awake()
    {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        connections = transform.GetComponentsInChildren<Connector>();
    }

    public virtual void Update()
    {
        UpdateVolumeDisplay(volumeDisplay, currentVolume, maxVolume, contents.name);
        OutputFluid(outputVolume, contents);
        //UpdateColor();
    }

    public virtual void UpdateVolumeDisplay(Text display, int current, int max, string name)
    {
        if (display)
        {
            string formattedCurrent = string.Format("{0:F3}", (current / 1000f));
            string formattedMax = string.Format("{0:F3}", (max / 1000f));
            display.text = formattedCurrent + " L /\n" + formattedMax + " L " + (current == 0 ? "" : name);
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
    public virtual int InputFluid(int volume, Fluid fluid, int index = 0)
    {
        if (currentVolume == 0 || contents == null)
        {
            contents = fluid;
        }
        if (contents == null || contents.SameSolution(fluid))
        {
            if (currentVolume + volume > maxVolume)
            {
                int insertedVolume = maxVolume - currentVolume;
                currentVolume = maxVolume;
                return insertedVolume;
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
                int maxOutput = Mathf.Min(volume, currentVolume);
                int realOutput = connector.OutputFluid(maxOutput, fluid);
                currentVolume -= realOutput;
                return realOutput;
            }
        }
        return 0;
    }
}
