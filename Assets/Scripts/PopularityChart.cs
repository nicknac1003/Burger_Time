using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PopularityChart
{
    [SerializeField] private List<Popularity> popularityChart;

    public float GetPopularity(int hour)
    {
        return popularityChart.Find(popularity => popularity.hour == hour).popularity;
    }
}

[System.Serializable]
public class Popularity
{
    [SerializeField] public int hour;
    [SerializeField] public float popularity;
}