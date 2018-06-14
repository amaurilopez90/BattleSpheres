using UnityEngine;
using System;

public class DataParser : MonoBehaviour {

    private static string KILLS_TAG = "[KILLS]";
    private static string DEATHS_TAG = "[DEATHS]";

	public static int DataToKills(string data)
    {
        return int.Parse(DataToValue(data, KILLS_TAG));
    }

    public static int DataToDeaths(string data)
    {
        return int.Parse(DataToValue(data, DEATHS_TAG));
    }

    private static string DataToValue(string data, string tag)
    {
        string[] dataPieces = data.Split('/');
        foreach (string piece in dataPieces)
        {
            if (piece.StartsWith(tag))
            {
                //Get just the number of kills
                return piece.Substring(tag.Length);
            }
        }

        Debug.LogError(tag + " not found in data.");
        return "";
    }

    public static string ValuesToData(int kills, int deaths)
    {
        return KILLS_TAG + kills + "/" + DEATHS_TAG + deaths;
    }
}
