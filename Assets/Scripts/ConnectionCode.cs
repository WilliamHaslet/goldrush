using System;
using UnityEngine;

public class ConnectionCode : MonoBehaviour
{

    public static string Encode(string ipAddress)
    {

        string encodedAddress = string.Empty;

        try
        {

            string[] splitAddress = ipAddress.Split('.');

            for (int i = 0; i < 4; i++)
            {

                string hexValue = int.Parse(splitAddress[i]).ToString("X2");

                encodedAddress += hexValue;

            }

        }
        catch (Exception e)
        {

            Debug.LogError(e);

            encodedAddress = "Encode Failed";

        }

        return encodedAddress;

    }

    public static string Decode(string connectionCode)
    {

        string ipAddress = string.Empty;

        try
        {

            if (connectionCode.Length == 8)
            {

                for (int i = 0; i < connectionCode.Length; i += 2)
                {

                    int ipPart = int.Parse(connectionCode.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);

                    ipAddress += ipPart;

                    if (i < connectionCode.Length - 2)
                    {

                        ipAddress += ".";

                    }

                }

            }
            else
            {

                ipAddress = "Decode Failed";

            }

        }
        catch (Exception e)
        {

            Debug.LogError(e);

            ipAddress = "Decode Failed";

        }

        return ipAddress;

    }

}
