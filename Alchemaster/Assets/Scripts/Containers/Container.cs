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

    void Awake()
    {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        connections = transform.GetComponentsInChildren<Connector>();
    }

    void Update()
    {
        UpdateVolumeDisplay();
        OutputFluid(outputVolume, contents);
        //UpdateColor();
    }

    private void UpdateVolumeDisplay()
    {
        if (volumeDisplay)
        {
            string formattedCurrent = string.Format("{0:F3}", (currentVolume / 1000f));
            string formattedMax = string.Format("{0:F3}", (maxVolume / 1000f));
            volumeDisplay.text = formattedCurrent + " L /\n" + formattedMax + " L " + (currentVolume == 0 ? "" : contents.name);
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
