using Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public enum TemplateParseState
{
    Idle,           // 闲置
    Parsing,        // 解析中
    Success,        // 解析成功
    Failure,        // 解析失败
}

public class TemplateParser : MonoBehaviour, IEventDispatcher
{
    private static readonly Encoding Encode = Encoding.UTF8;

    private BinaryReader reader;

    private TemplateConfig templateConfig;

    private TemplateParseState mState = TemplateParseState.Idle;

    private string mFailureReason = "";

    private int frameLimit = int.MaxValue;

    private int frameCount = int.MaxValue;

    private int currCount = 0;

    private ITemplates currTemplates = default(ITemplates);

    private EventDispatcher _dispatcher = new EventDispatcher();

    /// <summary>
    /// 表解析状态
    /// </summary>
    public TemplateParseState state
    {
        get
        {
            return mState;
        }
        set
        {
            if (mState != value)
            {
                mState = value;
                DispatchEvent(new TemplateParseEvent(TemplateParseEvent.STATE_CHANGE));
            }
        }
    }

    /// <summary>
    /// 如果state == TemplateParseState.Failure时的失败原因
    /// </summary>
    public string failureReason
    {
        get
        {
            return mFailureReason;
        }
    }

    /// <summary>
    /// 同步解析表结构
    /// </summary>
    /// <param name="pTemplateConfig"></param>
    /// <param name="pSource"></param>
    public void ParseSync(TemplateConfig pTemplateConfig, Stream pSource)
    {
        state = TemplateParseState.Parsing;
        templateConfig = pTemplateConfig;
        try
        {
            reader = new BinaryReader(pSource, Encode);
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                string templateName = reader.ReadString();
                int length = reader.ReadInt32();
                ITemplates templates = templateConfig.GetTemplates(templateName);
                templates.Parse(reader, length);
            }
            state = TemplateParseState.Success;
        }
        catch (Exception e)
        {
            mFailureReason = String.Format("{0}\n{1}", e.Message, e.StackTrace);
            state = TemplateParseState.Failure;
        }
    }

    /// <summary>
    /// 异步解析表结构
    /// </summary>
    /// <param name="pTemplateConfig"></param>
    /// <param name="pSource"></param>
    /// <param name="pFrameLimit">每物理帧处理的最大配置条数</param>
    public void ParseAsync(TemplateConfig pTemplateConfig, Stream pSource, int pFrameLimit = 2000)
    {
        state = TemplateParseState.Parsing;
        templateConfig = pTemplateConfig;
        reader = new BinaryReader(pSource, Encode);
        frameLimit = pFrameLimit;
    }

    private void FixedUpdate()
    {
        if (state == TemplateParseState.Parsing)
        {
            try
            {
                frameCount = frameLimit;
                while (frameCount != 0)
                {
                    if (currCount == 0)
                    {
                        if (reader.BaseStream.Position < reader.BaseStream.Length)
                        {
                            string templateName = reader.ReadString();
                            currCount = reader.ReadInt32();
                            currTemplates = templateConfig.GetTemplates(templateName);
                        }
                        else
                        {
                            state = TemplateParseState.Success;
                            break;
                        }
                    }
                    int parseCount = Math.Min(frameCount, currCount);
                    currTemplates.Parse(reader, parseCount);
                    frameCount -= parseCount;
                    currCount -= parseCount;
                }
            }
            catch (Exception e)
            {
                mFailureReason = String.Format("{0}\n{1}", e.Message, e.StackTrace);
                state = TemplateParseState.Failure;
            }
        }
    }

    public void DispatchEvent(IEvent e)
    {
        _dispatcher.DispatchEvent(e);
    }

    public void AddEventListener(string type, Action<IEvent> handler)
    {
        _dispatcher.AddEventListener(type, handler);
    }

    public void RemoveEventListener(string type, Action<IEvent> handler)
    {
        _dispatcher.RemoveEventListener(type, handler);
    }

    public void RemoveEventListeners(string type)
    {
        _dispatcher.RemoveEventListeners(type);
    }

    public void RemoveAllEventListeners()
    {
        _dispatcher.RemoveAllEventListeners();
    }

    public void Dispose()
    {
        RemoveAllEventListeners();
    }
}
