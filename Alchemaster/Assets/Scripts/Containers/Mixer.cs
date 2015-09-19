using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Mixer : Container {

    public Fluid aContents;
    public Fluid bContents;

    public int inputAVolume;
    public int inputBVolume;

    public int maxAVolume;
    public int maxBVolume;

    public Text inputAText;
    public Text inputBText;

    public override void Awake()
    {
        base.Awake();
        maxVolume = (int)Mathf.Clamp(maxVolume - (maxAVolume + maxBVolume), 0f, maxVolume);
    }

    public override void Update()
    {
        base.Update();
        UpdateVolumeDisplay(inputAText, inputAVolume, maxAVolume, aContents.name);
        UpdateVolumeDisplay(inputBText, inputBVolume, maxBVolume, bContents.name);
        Mix();
    }

    private void Mix()
    {
        Fluid product = CombineFluids(aContents, bContents);
        if (product.SameSolution(contents))
        {
            if (inputAVolume > 0 && inputBVolume > 0 && currentVolume <= maxVolume - 2)
            {
                inputAVolume--;
                inputBVolume--;
                currentVolume += 2;
            }
        }
    }

    private Fluid CombineFluids(Fluid a, Fluid b)
    {
        Solute[] aSolutes = a.solutes;
        Solute[] bSolutes = b.solutes;
        Solute[] productSolutes = new Solute[aSolutes.Length + bSolutes.Length];
        aSolutes.CopyTo(productSolutes, 0);
        bSolutes.CopyTo(productSolutes, productSolutes.Length);
        Fluid product = new Fluid(productSolutes, a.name + b.name);
        return product;
    }

    public override int InputFluid(int volume, Fluid fluid, int index = 0)
    {
        if (index == 0)
        {
            return InputFluidA(volume, fluid);
        }
        else if (index == 1)
        {
            return InputFluidB(volume, fluid);
        }
        else return base.InputFluid(volume, fluid);
    }

    public int InputFluidA(int volume, Fluid fluid)
    {
        if (inputAVolume == 0 || aContents == null)
        {
            aContents = fluid;
        }
        if (aContents == null || aContents.SameSolution(fluid))
        {
            if (inputAVolume + volume > maxAVolume)
            {
                int insertedVolume = maxAVolume - inputAVolume;
                inputAVolume = maxAVolume;
                return insertedVolume;
            }
            inputAVolume += volume;
            return volume;
        }
        else if (!aContents.SameSolution(fluid))
        {
            Debug.Log("Error! " + aContents.name + " is not the same fluid as " + fluid.name);
        }
        return 0;
    }

    public int InputFluidB(int volume, Fluid fluid)
    {
        if (inputBVolume == 0 || bContents == null)
        {
            bContents = fluid;
        }
        if (bContents == null || bContents.SameSolution(fluid))
        {
            if (inputBVolume + volume > maxBVolume)
            {
                int insertedVolume = maxBVolume - inputBVolume;
                inputBVolume = maxBVolume;
                return insertedVolume;
            }
            inputBVolume += volume;
            return volume;
        }
        else if (!bContents.SameSolution(fluid))
        {
            Debug.Log("Error! " + bContents.name + " is not the same fluid as " + fluid.name);
        }
        return 0;
    }
}
