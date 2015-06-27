using System.IO;

public interface ITemplates
{
    void Parse(BinaryReader reader, int length);
}
