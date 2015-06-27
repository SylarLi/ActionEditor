using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using System.Reflection;
using Core;
using UnityEngine;
using ProtoBuf;

namespace Local
{
    public enum StorageStatus
    {
        Idle,       // 闲置中
        Loading,    // 加载中
        Saving,     // 保存中
    }

    /// <summary>
    /// 本地存储实现(注意：有容量(条数)限制)
    /// 可存储字符串、Proto协议
    /// 使用方法：
    /// private void Handler()
    /// {
    ///     string value = localDataStorage.LoadString(key);
    ///     cs10001 proto = localDataStorage.LoadProto<cs10001>(key);
    ///     .....
    /// }
    /// 
    /// private void LoadStorage()
    /// {
    ///     if (localDataStorage.isLoaded)
    ///     {
    ///         Handler();
    ///     }
    ///     else
    ///     {
    ///         Action<IEvent> handler = (IEvent e) =>
    ///         {
    ///             localDataStorage.RemoveEventListener(LocalDataEvent.LOCAL_COMPLETE, handler);
    ///             Handler();
    ///         }
    ///         localDataStorage.AddEventListener(LocalDataEvent.LOCAL_COMPLETE, handler);
    ///     }
    /// }
    /// </summary>
    public class LocalDataStorage : MonoBehaviour, IEventDispatcher
    {
        /// <summary>
        /// 本地文件存储文件前缀
        /// </summary>
        private static string folderPath = Application.persistentDataPath + "/________";

        /// <summary>
        /// 本地存储文件默认路径
        /// </summary>
        private static string filePath = folderPath + "default";

        /// <summary>
        /// key, value分隔符
        /// </summary>
        private const string pairSeperator = ":";

        /// <summary>
        /// 存储最大容量(条数)(影响异步加载的速度)
        /// </summary>
        private const int maxCapacity = 300;

        /// <summary>
        /// 异步加载是否已经完成
        /// </summary>
        private bool _isLoaded = false;

        /// <summary>
        /// 本地存储当前状态
        /// </summary>
        private StorageStatus _status = StorageStatus.Idle;

        private StreamReader _reader;
        private StreamWriter _writer;

        private Dictionary<string, LocalData> _tempDatas = new Dictionary<string, LocalData>();
        private Dictionary<string, LocalData> _savedDatas = new Dictionary<string, LocalData>();
        private List<string> _savedBuffers = new List<string>();

        private float _flushTimeStamp = 0;
        private float _modifyTimeStamp = 0;

        private EventDispatcher _dispatcher = new EventDispatcher();

        /// <summary>
        /// 开启本地存储异步加载
        /// </summary>
        /// <param name="uniqueId">一个uniqueId代表一个独立的存储文件</param>
        public void TryLoadAsync(long uniqueId)
        {
            if (status == StorageStatus.Idle)
            {
#if UNITY_WEBPLAYER
                Debug.LogError("Local data storage is not supported on web player");
                return;
#endif
                filePath = folderPath + uniqueId;
                if (File.Exists(filePath))
                {
                    Debug.Log("Attemp to read file from: " + filePath);
                    try
                    {
                        _reader = new StreamReader(filePath);
                        _isLoaded = false;
                        status = StorageStatus.Loading;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Open file reader failed: " + e.Message + " : " + e.StackTrace);
                    }
                }
                else
                {
                    Debug.LogError("file not exist: " + filePath);
                }
            }
        }

        private void FixedUpdate()
        {
            if (_reader != null)
            {
                string line = null;
                try
                {
                    if (_reader.Peek() >= 0)
                    {
                        line = _reader.ReadLine();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Read file line failed: " + e.Message + " : " + e.StackTrace);
                }
                if (!string.IsNullOrEmpty(line))
                {
                    string[] splittedData = line.Split(new string[] { pairSeperator }, StringSplitOptions.RemoveEmptyEntries);
                    if (splittedData.Length == 2)
                    {
                        string key = splittedData[0];
                        string value = splittedData[1];
                        try
                        {
                            LocalData data = Deserialize(value);
                            data.serial = value;
                            _savedDatas[data.key] = data;
                        }
                        catch (Exception e)
                        {
                            Debug.LogError("Deserialize failed: " + key + " because of " + e.Message + " : " + e.StackTrace);
                        }
                    }
                }
                else
                {
                    try
                    {
                        _reader.Close();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Close stream reader failed: " + e.Message + " : " + e.StackTrace);
                    }
                    finally
                    {
                        _reader = null;
                        _isLoaded = true;
                        status = StorageStatus.Idle;
                        Debug.Log("Local data storage load complete");
                    }
                }
            }
            else if (_writer != null)
            {
                int bufferCount = _savedBuffers.Count;
                if (bufferCount > 0)
                {
                    try
                    {
                        _writer.WriteLine(_savedBuffers[bufferCount - 1]);
                        _savedBuffers.RemoveAt(bufferCount - 1);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("File write line failed: " + e.Message + " : " + e.StackTrace);
                    }
                }
                else
                {
                    try
                    {
                        _writer.Flush();
                        _writer.Close();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Close stream writer failed: " + e.Message + " : " + e.StackTrace);
                    }
                    finally
                    {
                        _writer = null;
                        status = StorageStatus.Idle;
                        Debug.Log("Local data storage save compelte");
                    }
                }
            }
        }

        private void OnApplicationQuit()
        {
            Flush();
        }

        /// <summary>
        /// 同步保存本地数据(阻塞)
        /// </summary>
        private void Flush()
        {
#if UNITY_WEBPLAYER
                Debug.LogError("Local data storage is not supported on web player");
                return;
#endif
            if (status == StorageStatus.Loading)
            {
                Debug.LogWarning("File Loading, flush operation will be expired");
            }
            else if (status == StorageStatus.Saving)
            {
                if (_writer != null)
                {
                    for (int i = _savedBuffers.Count - 1; i >= 0; i--)
                    {
                        try
                        {
                            _writer.WriteLine(_savedBuffers[i]);
                            _savedBuffers.RemoveAt(i);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError("File write line failed: " + e.Message + " : " + e.StackTrace);
                        }
                    }
                    try
                    {
                        _writer.Flush();
                        _writer.Close();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Flush or close stream failed: " + e.Message + " : " + e.StackTrace);
                    }
                    finally
                    {
                        _writer = null;
                        status = StorageStatus.Idle;
                        Debug.Log("Local data storage save complete");
                    }
                }
            }
            else if (_flushTimeStamp < _modifyTimeStamp)
            {
                FlushTemp();
                Debug.Log("Attemp to sava file: " + filePath);
                try
                {
                    FileStream fileStream = File.Open(filePath, FileMode.OpenOrCreate);
                    _writer = new StreamWriter(fileStream);
                    _writer.Write(string.Join(Environment.NewLine, _savedBuffers.ToArray()));
                    _writer.Flush();
                    _writer.Close();
                }
                catch (Exception e)
                {
                    Debug.LogError("Save failed: " + e.Message + " : " + e.StackTrace);
                }
                finally
                {
                    _writer = null;
                    Debug.Log("Local data storage save complete");
                }
                _flushTimeStamp = Time.realtimeSinceStartup;
            }
        }

        /// <summary>
        /// 异步保存本地数据(非阻塞)
        /// </summary>
        public void FlushAsync()
        {
#if UNITY_WEBPLAYER
                Debug.LogError("Local data storage is not supported on web player");
                return;
#endif
            if (status == StorageStatus.Idle && _flushTimeStamp < _modifyTimeStamp)
            {
                FlushTemp();
                Debug.Log("Attemp to save file: " + filePath);
                try
                {
                    _writer = new StreamWriter(File.Open(filePath, FileMode.OpenOrCreate));
                    status = StorageStatus.Saving;
                }
                catch (Exception e)
                {
                    Debug.LogError("Open file writer failed: " + e.Message + " : " + e.StackTrace);
                }
                _flushTimeStamp = Time.realtimeSinceStartup;
            }
        }

        /// <summary>
        /// 填充临时数据到持久化数据字典中，并按时间晚早筛选出最多maxCapacity条数据到buffer中，等待IO写入
        /// </summary>
        private void FlushTemp()
        {
            List<string> tempKeys = new List<string>(_tempDatas.Keys);
            foreach (string key in tempKeys)
            {
                LocalData data = _tempDatas[key];
                _savedDatas[key] = data;
                _tempDatas.Remove(key);
            }
            _savedBuffers.Clear();
            List<LocalData> datas = new List<LocalData>(_savedDatas.Values);
            datas.Sort(SortByDateTimeFunc);
            for (int i = 0, count = Math.Min(datas.Count, maxCapacity); i < count; i++)
            {
                _savedBuffers.Add(datas[i].key + pairSeperator + datas[i].serial);
            }
        }

        private int SortByDateTimeFunc(LocalData data1, LocalData data2)
        {
            TimeSpan dispan = data1.timeStamp.Subtract(data2.timeStamp);
            if (dispan < TimeSpan.Zero)
            {
                return 1;
            }
            else if (dispan > TimeSpan.Zero)
            {
                return -1;
            }
            return 0;
        }

        private LocalData Load(string key)
        {
            if (_tempDatas.ContainsKey(key))
            {
                return _tempDatas[key];
            }
            else if (_savedDatas.ContainsKey(key))
            {
                return _savedDatas[key];
            }
            return null;
        }

        public string LoadString(string key)
        {
            LocalString data = Load(key) as LocalString;
            return data == null ? null : data.value;
        }

        public void SaveString(string key, string value)
        {
            LocalString data = new LocalString();
            data.key = key;
            data.value = value;
            data.MarkTimeStamp();
            try
            {
                data.serial = SerializeString(data);
                _tempDatas[data.key] = data;
                _modifyTimeStamp = Time.realtimeSinceStartup;
            }
            catch (Exception e)
            {
                Debug.LogError("Serialize failed: " + e.Message + " : " + e.StackTrace);
            }
        }

        public T LoadProto<T>(string key) where T : IExtensible
        {
            LocalProto<T> data = Load(key) as LocalProto<T>;
            return data == null ? default(T) : data.value;
        }

        public void SaveProto<T>(string key, T value) where T : IExtensible
        {
            LocalProto<T> data = new LocalProto<T>();
            data.key = key;
            data.value = value;
            data.MarkTimeStamp();
            try
            {
                data.serial = SerializeProto<T>(data);
                _tempDatas[data.key] = data;
                _modifyTimeStamp = Time.realtimeSinceStartup;
            }
            catch (Exception e)
            {
                Debug.LogError("Serialize failed: " + e.Message + " : " + e.StackTrace);
            }
        }

        /// <summary>
        /// 格式：
        /// 字节长度    内容
        /// 存储类型    LocalDataType
        /// 序列化内容  bytes
        /// 最后编码为Base64
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string SerializeString(LocalString data)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Serializer.Serialize<LocalString>(stream, data);
                byte[] contentBytes = stream.ToArray();
                stream.SetLength(0);
                stream.WriteByte(data.type);
                stream.Write(contentBytes, 0, contentBytes.Length);
                stream.Flush();
                byte[] fullBytes = stream.ToArray();
                string serial = Convert.ToBase64String(fullBytes);
                return serial;
            }
        }

        /// <summary>
        /// 格式：
        /// 字节长度                        内容
        /// 存储类型                        LocalDataType
        /// Proto类型名字符串长度           int
        /// Proto类型名字符串               string
        /// 序列化内容                      bytes
        /// 最后编码为Base64
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        private string SerializeProto<T>(LocalProto<T> data) where T : IExtensible
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Serializer.Serialize<LocalProto<T>>(stream, data);
                byte[] contentBytes = stream.ToArray();
                stream.SetLength(0);
                stream.WriteByte(data.type);
                byte[] pnbytes = Encoding.ASCII.GetBytes(data.protoName);
                byte[] pnbyteslen = BitConverter.GetBytes(pnbytes.Length);
                stream.Write(pnbyteslen, 0, pnbyteslen.Length);
                stream.Write(pnbytes, 0, pnbytes.Length);
                stream.Write(contentBytes, 0, contentBytes.Length);
                stream.Flush();
                byte[] fullBytes = stream.ToArray();
                string serial = Convert.ToBase64String(fullBytes);
                return serial;
            }
        }

        private LocalData Deserialize(string serial)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                byte[] bytes = Convert.FromBase64String(serial);
                byte type = bytes[0];
                LocalData data = null;
                switch (type)
                {
                    case LocalDataType.STRING:
                        {
                            stream.Write(bytes, 1, bytes.Length - 1);
                            stream.Flush();
                            stream.Position = 0;
                            data = Serializer.Deserialize<LocalString>(stream);
                            break;
                        }
                    case LocalDataType.PROTO:
                        {
                            int index = 1;
                            byte[] pnbyteslen = new byte[sizeof(int)];
                            Array.ConstrainedCopy(bytes, 1, pnbyteslen, 0, pnbyteslen.Length);
                            index += pnbyteslen.Length;
                            byte[] pnbytes = new byte[BitConverter.ToInt32(pnbyteslen, 0)];
                            Array.ConstrainedCopy(bytes, index, pnbytes, 0, pnbytes.Length);
                            index += pnbytes.Length;
                            string protoName = Encoding.ASCII.GetString(pnbytes);
                            Type dataType = typeof(LocalProto<>);
                            Type protoType = Type.GetType(protoName);
                            dataType = dataType.MakeGenericType(protoType);
                            stream.Write(bytes, index, bytes.Length - index);
                            stream.Flush();
                            stream.Position = 0;
                            data = ProtoBuf.Meta.RuntimeTypeModel.Default.Deserialize(stream, null, dataType) as LocalData;
                            break;
                        }
                    default:
                        {
                            throw new NotSupportedException();
                        }
                }
                return data;
            }
        }

        public StorageStatus status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    LocalDataEvent evt = null;
                    if (_status == StorageStatus.Loading)
                    {
                        evt = new LocalDataEvent(LocalDataEvent.LOAD_COMPLETE);
                    }
                    else if (_status == StorageStatus.Saving)
                    {
                        evt = new LocalDataEvent(LocalDataEvent.SAVE_COMPLETE);
                    }
                    _status = value;
                    if (evt != null)
                    {
                        DispatchEvent(evt);
                    }
                }
            }
        }

        public bool isLoaded
        {
            get { return _isLoaded; }
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
}
