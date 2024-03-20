using System.Collections;
using System.Collections.Generic;

public static class Utility
{

    public static T[] SuffleArray<T>(T[] array, int seed)
    {
        System.Random prnd = new System.Random(seed);

        for (int i = 0; i < array.Length; i++)
        {
            int randomIndex = prnd.Next(i, array.Length);
            T tempItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempItem;
        }

        return array;
    }
}
