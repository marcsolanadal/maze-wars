using UnityEngine;
using System.Collections.Generic;

public class ZoneMeshes
{
    //private string zoneDirectoryPath = "./Assets/Resources/Zones/";
    private System.Random pseudoRNG;

    // Mesh lists for ground and chests.
    List<Mesh> groundList = new List<Mesh>();
    List<Mesh> chestList = new List<Mesh>();

    // Mesh lists for different wall types.
    List<Mesh> isolateWallList = new List<Mesh>();
    List<Mesh> normalWallList = new List<Mesh>();
    List<Mesh> jointWallList = new List<Mesh>();
    List<Mesh> invisibleWallList = new List<Mesh>();
    List<Mesh> outterWallList = new List<Mesh>();

    // Mesh lists for corner types.
    List<Mesh> normalCornerList = new List<Mesh>();
    List<Mesh> endCornerList = new List<Mesh>();

    // Used to convert to List<Mesh>.
    Mesh[] meshList;

    // Load all the Mesh objects into the corresponding lists. 
    public ZoneMeshes(string zoneType, string seed)
    {
        // Getting random seed.
        pseudoRNG = new System.Random(seed.GetHashCode());

        // Throw exception if the zone doesn't exist.

        string localPath = "Artists/Zones/" + zoneType + "/Maze/";

        // Loading all ground meshes.
        meshList = Resources.LoadAll<Mesh>(localPath + "Ground");
        foreach (Mesh mesh in meshList)
        {
            groundList.Add(mesh);
        }

        // Loading all chest meshes.
        meshList = Resources.LoadAll<Mesh>(localPath + "Chests");
        foreach (Mesh mesh in meshList)
        {
            chestList.Add(mesh);
        }

        // Loading all isolated/column wall meshes.
        meshList = Resources.LoadAll<Mesh>(localPath + "Walls/Isolate");
        foreach (Mesh mesh in meshList)
        {
            isolateWallList.Add(mesh);
        }

        // Loading all normal wall meshes.
        meshList = Resources.LoadAll<Mesh>(localPath + "Walls/Normal");
        foreach (Mesh mesh in meshList)
        {
            normalWallList.Add(mesh);
        }

        // Loading all joint wall meshes.
        meshList = Resources.LoadAll<Mesh>(localPath + "Walls/Joint");
        foreach (Mesh mesh in meshList)
        {
            jointWallList.Add(mesh);
        }

        // Loading all invisible wall meshes.
        meshList = Resources.LoadAll<Mesh>(localPath + "Walls/invisible");
        foreach (Mesh mesh in meshList)
        {
            invisibleWallList.Add(mesh);
        }

        // Loading all outter wall meshes.
        meshList = Resources.LoadAll<Mesh>(localPath + "Walls/Outter");
        foreach (Mesh mesh in meshList)
        {
            outterWallList.Add(mesh);
        }

        // Loading all normal corners meshes.
        meshList = Resources.LoadAll<Mesh>(localPath + "Walls/Corner/Normal");
        foreach (Mesh mesh in meshList)
        {
            normalCornerList.Add(mesh);
        }

        // Loading all end corners meshes.
        meshList = Resources.LoadAll<Mesh>(localPath + "Walls/Corner/End");
        foreach (Mesh mesh in meshList)
        {
            endCornerList.Add(mesh);
        }

    }

    // Getters for all the types of meshes. Returning random mesh or null in case 
    public Mesh ground
    {
        get
        {
            if (groundList.Count != 0)
            {
                int index = pseudoRNG.Next(groundList.Count);
                return groundList[index];
            }
            else
            {
                return null;
            }
        }
    }
    public Mesh chest
    {
        get
        {
            if (chestList.Count != 0)
            {
                int index = pseudoRNG.Next(chestList.Count);
                return chestList[index];
            }
            else
            {
                return null;
            }
        }
    }
    public Mesh isolateWall
    {
        get
        {
            if (isolateWallList.Count != 0)
            {
                int index = pseudoRNG.Next(isolateWallList.Count);
                return isolateWallList[index];
            }
            else
            {
                return null;
            }
        }
    }
    public Mesh normalWall
    {
        get
        {
            if (normalWallList.Count != 0)
            {
                int index = pseudoRNG.Next(normalWallList.Count);
                return normalWallList[index];
            }
            else
            {
                return null;
            }
        }
    }
    public Mesh jointWall
    {
        get
        {
            if (jointWallList.Count != 0)
            {
                int index = pseudoRNG.Next(jointWallList.Count);
                return jointWallList[index];
            }
            else
            {
                return null;
            }
        }
    }
    public Mesh invisibleWall
    {
        get
        {
            if (invisibleWallList.Count != 0)
            {
                int index = pseudoRNG.Next(invisibleWallList.Count);
                return invisibleWallList[index];
            }
            else
            {
                return null;
            }
        }
    }
    public Mesh outterWall
    {
        get
        {
            if (outterWallList.Count != 0)
            {
                int index = pseudoRNG.Next(outterWallList.Count);
                return outterWallList[index];
            }
            else
            {
                return null;
            }
        }
    }
    public Mesh normalCorner
    {
        get
        {
            if (normalCornerList.Count != 0)
            {
                int index = pseudoRNG.Next(normalCornerList.Count);
                return normalCornerList[index];
            }
            else
            {
                return null;
            }
        }
    }
    public Mesh endCorner
    {
        get
        {
            if (endCornerList.Count != 0)
            {
                int index = pseudoRNG.Next(endCornerList.Count);
                return endCornerList[index];
            }
            else
            {
                return null;
            }
        }
    }

}
