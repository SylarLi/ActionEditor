using Kolibri;
using System;
using UnityEngine;
using UnityEditor;
using System.IO;

public class Exporter : EditorWindow
{
    [MenuItem("Window/Template Exporter")]
    private static void Init()
    {
        Exporter exporter = (Exporter)EditorWindow.GetWindow(typeof(Exporter));
    }

    [SerializeField]
    private string configFolder;

    [SerializeField]
    private string configTo;

    [SerializeField]
    private string chartFile;

    [SerializeField]
    private string templateFile;

    [SerializeField]
    private string templatesFile;

    [SerializeField]
    private TemplateParseState asyncState;

    [NonSerialized]
    private Vector2 scrollPos;

    private void OnEnable()
    {
        hideFlags = HideFlags.HideAndDontSave;
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        EditorGUILayout.LabelField("配置导出");
        configFolder = EditorGUILayout.TextField("文件夹路径", configFolder);
        configTo = EditorGUILayout.TextField("导出至", configTo);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("导出", GUILayout.Width(120)))
        {
            ExportConfig();
        }
        GUI.enabled = asyncState != TemplateParseState.Parsing;
        if (GUILayout.Button("同步解析测试", GUILayout.Width(120)))
        {
            ParseSyncTest();
        }
        if (GUILayout.Button("异步解析测试", GUILayout.Width(120)))
        {
            ParseAsyncTest();
        }
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Template生成");
        chartFile = EditorGUILayout.TextField("表路径", chartFile);
        if (GUILayout.Button("生成", GUILayout.Width(120)))
        {
            ParseTemplate();
        }
        EditorGUILayout.LabelField("Template");
        templateFile = EditorGUILayout.TextArea(templateFile, GUILayout.Height(400));
        EditorGUILayout.LabelField("Templates");
        templatesFile = EditorGUILayout.TextArea(templatesFile, GUILayout.Height(100));

        EditorGUILayout.EndScrollView();
    }

    private void ExportConfig()
    {
        if (!Directory.Exists(configFolder))
        {
            Debug.LogError("文件夹不存在 : " + configFolder);
        }
        else
        {
            byte[] bytes = CsvWriter.FolderToBytes(configFolder);
            FileStream stream = File.Open(configTo, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            stream.Position = 0;
            stream.SetLength(0);
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
            stream.Close();
            Debug.Log("导出成功 : " + configTo);
        }
    }

    private void ParseSyncTest()
    {
        //FileStream stream = File.Open(chartFile, FileMode.Append, FileAccess.Write);
        //BinaryWriter writer = new BinaryWriter(stream, System.Text.Encoding.UTF8);
        //for (int i = 0; i < 100000; i++)
        //{
        //    writer.Write(i + ",sylar,1,1,案说法是否打算的发生地方按时发生的发生的发生的发生的发生的发生的发生的发生的法师法师的s,1,1,1\n");
        //}
        //writer.Flush();
        //writer.Close();
        FileStream stream = File.Open(configTo, FileMode.Open, FileAccess.Read);
        TemplateConfig templateConfig = new TemplateConfig();
        GameObject templateProxy = new GameObject();
        templateProxy.hideFlags = HideFlags.HideInHierarchy;
        TemplateParser templateParser = templateProxy.AddComponent<TemplateParser>();
        DateTime now1 = DateTime.Now;
        templateParser.ParseSync(templateConfig, stream);
        DateTime now2 = DateTime.Now;
        TimeSpan span = now2.Subtract(now1);
        switch (templateParser.state)
        {
            case TemplateParseState.Success:
                {
                    Debug.Log("同步解析成功: " + span.TotalSeconds + "秒");
                    break;
                }
            case TemplateParseState.Failure:
                {
                    Debug.LogError("同步解析失败: " + templateParser.failureReason);
                    break;
                }
            default:
                {
                    Debug.LogError("同步解析失败: 状态异常");
                    break;
                }
        }
        stream.Close();
        GameObject.DestroyImmediate(templateProxy);
    }

    private void ParseAsyncTest()
    {
        if (!Application.isPlaying)
        {
            Debug.Log("请先启动游戏");
        }
        else
        {
            FileStream stream = File.Open(configTo, FileMode.Open, FileAccess.Read);
            TemplateConfig templateConfig = new TemplateConfig();
            GameObject templateProxy = new GameObject();
            templateProxy.hideFlags = HideFlags.HideInHierarchy;
            TemplateParser templateParser = templateProxy.AddComponent<TemplateParser>();
            DateTime now1 = DateTime.Now;
            templateParser.AddEventListener(TemplateParseEvent.STATE_CHANGE, (Core.IEvent e) =>
            {
                asyncState = templateParser.state;
                switch (asyncState)
                {
                    case TemplateParseState.Parsing:
                        {
                            Debug.Log("异步解析中...");
                            break;
                        }
                    case TemplateParseState.Success:
                        {
                            DateTime now2 = DateTime.Now;
                            TimeSpan span = now2.Subtract(now1);
                            Debug.Log("异步解析成功: " + span.TotalSeconds + "秒");
                            stream.Close();
                            GameObject.Destroy(templateProxy);
                            break;
                        }
                    case TemplateParseState.Failure:
                        {
                            Debug.LogError("异步解析失败：" + templateParser.failureReason);
                            stream.Close();
                            GameObject.Destroy(templateProxy);
                            break;
                        }
                    default:
                        {
                            Debug.LogError("异步解析失败: 状态异常");
                            break;
                        }
                }
                Repaint();
            });
            templateParser.ParseAsync(templateConfig, stream);
        }
    }

    private void ParseTemplate()
    {
        if (!File.Exists(chartFile))
        {
            Debug.Log("配置表不存在 : " + chartFile);
        }
        else
        {
            templateFile = CsvWriter.FileToTemplate(chartFile);
            templatesFile = CsvWriter.FileToTemplates(chartFile);
            Debug.Log("生成成功");
        }
    }
}

