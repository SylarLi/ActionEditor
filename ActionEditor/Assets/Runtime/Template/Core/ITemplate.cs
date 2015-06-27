using System.IO;

public interface ITemplate
{
    string key { get; }

    void Parse(BinaryReader reader);
}
