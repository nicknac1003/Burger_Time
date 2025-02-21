using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PopularityChart
{
    [SerializeField] private List<Popularity> popularityChart;
}

[System.Serializable]
public class Popularity
{
    [SerializeField] private int hour;
    [SerializeField] private float popularity;
}