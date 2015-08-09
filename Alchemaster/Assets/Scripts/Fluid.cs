using UnityEngine;
using System.Collections;

[System.Serializable]
public class Fluid {

    public string name;

    public Solute[] solutes;

    public Color color;

    public Fluid()
    {
        name = "Undefined Fluid";
        solutes = new Solute[0];
        color = Color.white;
    }

    public Fluid(Solute[] newSolutes, string newName = "Unnamed Compound")
    {
        name = newName;
        solutes = new Solute[newSolutes.Length];
        solutes = newSolutes;
        Color newColor = Color.clear;
        foreach (Solute solute in solutes)
        {
            newColor += solute.color;
        }
        newColor = newColor / solutes.Length;
        color = newColor;
    }

    //Incomplete function, should check list of solutes in each fluid to see if they are the same despite order
    public bool SameSolution(Fluid otherFluid)
    {
        if (otherFluid.name == this.name)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
