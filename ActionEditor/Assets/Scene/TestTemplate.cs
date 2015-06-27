using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class TestTemplate : MonoBehaviour
{
    private TemplateParseState asyncState;

    private string randomText = "";

    private int frameLength = 2000;

    private void Update()
    {
        randomText = UnityEngine.Random.Range(0, int.MaxValue).ToString();
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(100, 100, 200, 40), "Sync"))
        {
            ParseSyncTest();
        }
        if (GUI.Button(new Rect(100, 150, 200, 40), "ASync"))
        {
            ParseAsyncTest();
        }
        GUI.Label(new Rect(100, 200, 200, 40), randomText);
    }

    private void ParseSyncTest()
    {
#if UNITY_ANDROID
        string configTo = "jar:file://" + Application.dataPath + "!/assets" + "/data.txt";
        WWW www = new WWW(configTo);
        while (!www.isDone);
        MemoryStream stream = new MemoryStream(www.bytes);
        stream.Position = 0;
        stream.Flush();
#else
        string configTo = Application.dataPath + "/StreamingAssets/data";
        FileStream stream = File.Open(configTo, FileMode.Open, FileAccess.Read);
#endif
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
#if UNITY_ANDROID
            string configTo = "jar:file://" + Application.dataPath + "!/assets" + "/data.txt";
            WWW www = new WWW(configTo);
            while (!www.isDone) ;
            MemoryStream stream = new MemoryStream(www.bytes);
            stream.Position = 0;
            stream.Flush();
#else
            string configTo = Application.dataPath + "/StreamingAssets/data";
            FileStream stream = File.Open(configTo, FileMode.Open, FileAccess.Read);
#endif
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
            });
            templateParser.ParseAsync(templateConfig, stream, frameLength);
        }
    }
}
