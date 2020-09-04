using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FoodDataCustom : MonoBehaviourPunCallbacks
{

    List<FoodInfo> listFood;
    
    private void Start()
    {
        //PhotonPeer.RegisterType(typeof(FoodDataCustom), (byte)'V', Serialize, Desserialize);
    }
    public byte[] Serialize(object customType) 
    {
        using (MemoryStream m = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(m))
            {
                //writer.Write(listFood);
               // writer.Write(Name);
            }
            return m.ToArray();
        }
    }
    public static DataCustomSerialization Desserialize(byte[] data)
    {
        DataCustomSerialization result = new DataCustomSerialization();
        using (MemoryStream m = new MemoryStream(data))
        {
            using (BinaryReader reader = new BinaryReader(m))
            {
                result.Id = reader.ReadInt32();
                result.Name = reader.ReadString();
            }
        }
        return result;
    }
}

public class FoodInfo
{
    float power { get; set; }
    Vector3 positionSpawn { get; set; }
    int indexFood { get; set; }
    public FoodInfo(float p, Vector3 v,int i)
    {
        power = p;
        positionSpawn = v;
        indexFood = i;
    }
}
