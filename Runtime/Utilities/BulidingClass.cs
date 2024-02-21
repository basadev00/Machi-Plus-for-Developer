using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadBuilding
{
    public string BuildingID;
    public double latitude;
    public double longitude;
    public int surface_num;
    public int surfacecolor_r;
    public int surfacecolor_g;
    public int surfacecolor_b;
    public int surfacecolor_a;
    public int surface_width;
    public int surface_height;
    public int Vertex0_X;
    public int Vertex0_Y;
    public int Vertex1_X;
    public int Vertex1_Y;
    public int Vertex2_X;
    public int Vertex2_Y;
    public int Vertex3_X;
    public int Vertex3_Y;
}

public class LoadBuildingData
{
    [JsonProperty("BuildingData")]
    public Dictionary<string, LoadBuilding> loadBuildingData;
}



