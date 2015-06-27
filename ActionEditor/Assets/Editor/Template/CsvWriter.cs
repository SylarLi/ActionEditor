using Ideafixxxer.CsvParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary>
/// 配置表序列化工具类
/// </summary>
public class CsvWriter
{
    private const int MinLine = 5;

    private const int LineField = 0;
    private const int LineType = 1;
    private const int LineComment = 2;
    private const int LineKey = 3;
    private const int LineFlag = 4;

    private static readonly Encoding Encode = Encoding.UTF8;

    private const string TypeString = "string";
    private const string TypeInt = "int";

    private const int KeyValue = 1;
    private const int FlagValue = 1;

    private const string Template = "Template";

    private const string FormatUsing = "using System.IO;";
    private const string FormatClass = "public class {class_name} : ITemplate";
    private const string FormatField = "public {field_type} {field_name};";
    private const string FormatKeyProperty = "public string key";
    private const string FormatMethodParse = "public void Parse(BinaryReader reader)";
    private const string FormatMethodParseLine = "{field_name} = reader.{reader_method}();";
    private const string FormatTab = "\t";
    private const string FormatTemplatesClass = "public class {class_name}s : Templates<{class_name}>";

    /// <summary>
    /// 判断字段是否需要导出
    /// </summary>
    /// <param name="flag"></param>
    /// <returns></returns>
    public static bool FieldShouldBeExport(string flag)
    {   
        bool result = true;
        if (!string.IsNullOrEmpty(flag))
        {
            int value = 0;
            result = int.TryParse(flag, out value);
            if (result)
            {
                result = value != FlagValue;
            }
        }
        return result;
    }

    /// <summary>
    /// 判断字段是否为主键
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool FieldIsKey(string key)
    {
        bool result = false;
        if (!string.IsNullOrEmpty(key))
        {
            int value = 0;
            result = int.TryParse(key, out value);
            if (result)
            {
                result = value == KeyValue;
            }
        }
        return result;
    }

    /// <summary>
    /// 下划线-->骆驼
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string UnderlineToCamelStyle(string name)
    {
        string[] pieces = name.Split(new char[] { '_' });
        pieces = Array.ConvertAll<string, string>(pieces, (string piece) => piece[0].ToString().ToUpper() + piece.Substring(1, piece.Length - 1));
        return string.Join("", pieces);
    }

    /// <summary>
    /// CSV文件转换为二维数组
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string[][] FileToRaw(string path)
    {
        string[][] raw = null;
        FileInfo fileInfo = new FileInfo(path);
        StreamReader fileReader = null;
        try
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                fileReader = new StreamReader(fileStream, Encode);
                raw = new CsvParser().Parse(fileReader);
            }
        }
        catch (Exception e)
        {
            throw new Exception("表" + fileInfo.Name + "导出错误: " + e.Message);
        }

        if (raw.Length < MinLine)
        {
            throw new Exception("表" + fileInfo.Name + "导出错误: 行数少于" + MinLine);
        }
        for (int i = 1, max = raw.Length; i < max; i++)
        {
            if (raw[i].Length != raw[LineField].Length)
            {
                throw new Exception("表" + fileInfo.Name + "导出错误: " + "第" + (i + 1) + "行列数错误, 应为" + raw[0].Length + "列实为" + raw[i].Length + "列");
            }
        }
        string[] lineField = raw[LineField];
        string[] lineType = raw[LineType];
        string[] lineComment = raw[LineComment];
        string[] lineKey = raw[LineKey];
        string[] lineFlag = raw[LineFlag];
        bool keyExist = false;
        bool exportContentExist = false;
        for (int i = 0, max = lineField.Length; i < max; i++)
        {
            if (string.IsNullOrEmpty(lineField[i]))
            {
                throw new Exception("表" + fileInfo.Name + "导出错误: " + "第" + (i + 1) + "列字段名为空");
            }
            if (string.IsNullOrEmpty(lineType[i]))
            {
                throw new Exception("表" + fileInfo.Name + "导出错误: " + "第" + (i + 1) + "列字段类型为空");
            }
            if (!keyExist && FieldIsKey(lineKey[i]))
            {
                keyExist = true;
            }
            if (!exportContentExist && FieldShouldBeExport(lineFlag[i]))
            {
                exportContentExist = true;
            }
        }
        if (!keyExist)
        {
            throw new Exception("表" + fileInfo.Name + "导出错误: " + "未设置主键");
        }
        if (!exportContentExist)
        {
            throw new Exception("表" + fileInfo.Name + "导出错误: " + "没有需要导出的字段");
        }

        return raw;
    }

    /// <summary>
    /// 导出序列化字节表
    /// byte格式:
    /// 表名 string
    /// 行数 int
    /// 数据数据数据...
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static byte[] FileToBytes(string path)
    {
        byte[] bytes = null;
        FileInfo fileInfo = new FileInfo(path);
        string fileName = fileInfo.Name.Replace(fileInfo.Extension, "");
        string[][] raw = FileToRaw(path);
        string[] lineField = raw[LineField];
        string[] lineType = raw[LineType];
        string[] lineFlag = raw[LineFlag];
        int currentRow = MinLine, currentColumn = 0;
        try
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryWriter writer = new BinaryWriter(stream, Encode);
                writer.Write(fileName);
                writer.Write(raw.Length - MinLine);
                for (int rowLen = raw.Length; currentRow < rowLen; currentRow++)
                {
                    currentColumn = 0;
                    for (int columnLen = raw[currentRow].Length; currentColumn < columnLen; currentColumn++)
                    {
                        if (FieldShouldBeExport(lineFlag[currentColumn]))
                        {
                            switch (lineType[currentColumn])
                            {
                                case TypeInt:
                                    {
                                        writer.Write(int.Parse(raw[currentRow][currentColumn]));
                                        break;
                                    }
                                default:
                                    {
                                        writer.Write(raw[currentRow][currentColumn]);
                                        break;
                                    }
                            }
                        }
                    }
                }
                bytes = new byte[stream.Length];
                stream.Position = 0;
                stream.Flush();
                stream.Read(bytes, 0, bytes.Length);
            }
        }
        catch (Exception e)
        {
            throw new Exception("表" + fileInfo.Name + "导出错误: " + currentRow + "行" + currentColumn + "列" + e.Message + "\n" + e.StackTrace);
        }
        return bytes;
    }

    /// <summary>
    /// 遍历文件夹子级合并导出序列化字节表
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static byte[] FolderToBytes(string path)
    {
        byte[] bytes = null;
        using (MemoryStream stream = new MemoryStream())
        {
            string[] filePaths = Directory.GetFiles(path, "*.csv");
            foreach (string filePath in filePaths)
            {
                byte[] fileBytes = FileToBytes(filePath);
                stream.Write(fileBytes, 0, fileBytes.Length);
            }
            bytes = new byte[stream.Length];
            stream.Position = 0;
            stream.Flush();
            stream.Read(bytes, 0, bytes.Length);
        }
        return bytes;
    }

    /// <summary>
    /// 配置表转换为C# Templates类
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string FileToTemplates(string path)
    {
        FileInfo fileInfo = new FileInfo(path);
        string fileName = fileInfo.Name.Replace(fileInfo.Extension, "");
        string className = UnderlineToCamelStyle(fileName) + "Template";
        StringBuilder classBuilder = new StringBuilder();
        classBuilder.AppendLine(FormatTemplatesClass.Replace("{class_name}", className));
        classBuilder.AppendLine("{");
        classBuilder.AppendLine("}");
        return classBuilder.ToString();
    }

    /// <summary>
    /// 配置表转换为C# Template类
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string FileToTemplate(string path)
    {
        FileInfo fileInfo = new FileInfo(path);
        string fileName = fileInfo.Name.Replace(fileInfo.Extension, "");
        string className = UnderlineToCamelStyle(fileName) + Template;
        string[][] raw = FileToRaw(path);
        string[] lineField = raw[LineField];
        string[] lineType = raw[LineType];
        string[] lineKey = raw[LineKey];
        string[] lineFlag = raw[LineFlag];
        StringBuilder classBuilder = new StringBuilder();
        classBuilder.AppendLine(FormatUsing);
        classBuilder.AppendLine();
        classBuilder.AppendLine(FormatClass.Replace("{class_name}", className));
        classBuilder.AppendLine("{");
        for (int i = 0, columnLen = lineField.Length; i < columnLen; i++)
        {
            if (FieldShouldBeExport(lineFlag[i]))
            {
                classBuilder.AppendLine(FormatTab + FormatField.Replace("{field_type}", lineType[i]).Replace("{field_name}", lineField[i]));
                classBuilder.AppendLine();
            }
        }
        classBuilder.AppendLine(FormatTab + FormatKeyProperty);
        classBuilder.AppendLine(FormatTab + "{");
        classBuilder.AppendLine(FormatTab + FormatTab + "get");
        classBuilder.AppendLine(FormatTab + FormatTab + "{");
        classBuilder.Append(FormatTab + FormatTab + FormatTab + "return \"\" + ");
        List<string> keys = new List<string>();
        for (int i = 0, columnLen = lineField.Length; i < columnLen; i++)
        {
            if (FieldIsKey(lineKey[i]))
            {
                keys.Add(lineField[i]);
            }
        }
        classBuilder.Append(string.Join(" + ", keys.ToArray()));
        classBuilder.AppendLine(";");
        classBuilder.AppendLine(FormatTab + FormatTab + "}");
        classBuilder.AppendLine(FormatTab + "}");
        classBuilder.AppendLine();
        classBuilder.AppendLine(FormatTab + FormatMethodParse);
        classBuilder.AppendLine(FormatTab + "{");
        for (int i = 0, columnLen = lineField.Length; i < columnLen; i++)
        {
            if (FieldShouldBeExport(lineFlag[i]))
            {
                string methodLine = FormatMethodParseLine.Replace("{field_name}", lineField[i]);
                switch (lineType[i])
                {
                    case TypeInt:
                        {
                            methodLine = methodLine.Replace("{reader_method}", "ReadInt32");
                            break;
                        }
                    default:
                        {
                            methodLine = methodLine.Replace("{reader_method}", "ReadString");
                            break;
                        }
                }
                classBuilder.AppendLine(FormatTab + FormatTab + methodLine);
            }
        }
        classBuilder.AppendLine(FormatTab + "}");
        classBuilder.AppendLine("}");
        return classBuilder.ToString();
    }
}