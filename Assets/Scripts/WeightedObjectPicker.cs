using UnityEngine;

public class WeightedObjectPicker
{
    private float totalSpawnChance = 200;
    private GameObject[] platformArray;
    private float[] platformSpawnChanceArray;

    ///<summary>
    /// oddsPerEntry should be of same length as platform array
    ///</summary>
    public WeightedObjectPicker(GameObject[] platformArray, float[] oddsPerEntry)
    {
        this.platformArray = platformArray;
        this.platformSpawnChanceArray = new float[this.platformArray.Length + 1];
        this.platformSpawnChanceArray[0] = 0;

        for (int i = 0; i < this.platformArray.Length; i++)
        {
            this.totalSpawnChance += oddsPerEntry[i];
            this.platformSpawnChanceArray[i + 1] = this.totalSpawnChance;
        }
    }

    public GameObject GetRandomEntry()
    {
        float rand = Random.Range(0, totalSpawnChance);
        for (int i = 0; i < this.platformArray.Length; i++)
        {
            if (this.platformSpawnChanceArray[i] < rand && rand < this.platformSpawnChanceArray[i + 1])
                return this.platformArray[i];
        }
        return this.platformArray[0];
    }
}
