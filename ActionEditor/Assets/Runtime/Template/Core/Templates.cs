using System.Collections.Generic;
using System.IO;
using System;

public class Templates<T> : Dictionary<string, T>, ITemplates where T : ITemplate, new()
{
    public void Parse(BinaryReader reader, int length)
    {
        for (int i = 0; i < length; i++)
        {
            T t = new T();
            t.Parse(reader);
            Add(t.key, t);
        }
    }

    public T GetTemplate(string key)
    {
        T value = default(T);
        if (ContainsKey(key))
        {
            value = this[key];
        }
        return value;
    }
}
