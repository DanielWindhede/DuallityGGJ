using UnityEngine;

public class WeightedObjectPicker
{
    private float totalSpawnChance = 0;
    private GameObject[] entires;
    private float[] weightedOdds;

    ///<summary>
    /// oddsPerEntry should be of same length as platform array
    ///</summary>
    public WeightedObjectPicker(GameObject[] entries, float[] oddsPerEntry)
    {
        this.entires = entries;
        this.weightedOdds = new float[this.entires.Length + 1];
        this.weightedOdds[0] = 0;

        for (int i = 0; i < this.entires.Length; i++)
        {
            this.totalSpawnChance += oddsPerEntry[i];
            this.weightedOdds[i + 1] = this.totalSpawnChance;
        }
    }

    public GameObject GetRandomEntry()
    {
        float rand = Random.Range(0, totalSpawnChance);
        for (int i = 0; i < this.entires.Length; i++)
        {
            if (this.weightedOdds[i] < rand && rand < this.weightedOdds[i + 1])
                return this.entires[i];
        }
        return this.entires[0];
    }
}
