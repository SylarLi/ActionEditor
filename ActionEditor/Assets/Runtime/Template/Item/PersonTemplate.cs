using System.IO;

public class PersonTemplate : ITemplate
{
    public int id;

    public string name;

    public int sex;

    public int age;

    public string pp1;

    public int pp2;

    public int pp3;

    public int pp4;

    public string key
    {
        get
        {
            return "" + id;
        }
    }

    public void Parse(BinaryReader reader)
    {
        id = reader.ReadInt32();
        name = reader.ReadString();
        sex = reader.ReadInt32();
        age = reader.ReadInt32();
        pp1 = reader.ReadString();
        pp2 = reader.ReadInt32();
        pp3 = reader.ReadInt32();
        pp4 = reader.ReadInt32();
    }
}
