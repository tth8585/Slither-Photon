using ExitGames.Client.Photon;
using System.IO;
using UnityEngine;

[System.Serializable]
public class DataCustomSerialization
{
    void Register()
    {
        PhotonPeer.RegisterType(typeof(DataCustomSerialization), (byte)'V', Serialize, Desserialize);
    }

    public int Id { get; set; }
    public string Name { get; set; }

    public byte[] Serialize(object customType)
    {
        using (MemoryStream m = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(m))
            {
                writer.Write(Id);
                writer.Write(Name);
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
