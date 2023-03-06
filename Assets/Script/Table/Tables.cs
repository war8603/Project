using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class TableDoc
{
    public Dictionary<TableType, TableInfo> infoDict;
}

public class TableInfo
{
    public long size;
}

public class Tables
{
    private static Tables _instance;
    public static Tables Instance => _instance;

    Dictionary<Type, TableType> _tableTypeDic = new Dictionary<Type, TableType>();
    Dictionary<TableType, List<TRFoundation>> _tableDatas = new Dictionary<TableType, List<TRFoundation>>();

    public void InitTabeDatas()
    {
        _instance = this;
        _tableDatas.Clear();
        LoadTableData();
    }

    private void LoadTableData()
    {
        Debug.Log("LoadTableData");
#if LOAD_LOCAL_TABLE
        var xmlRoot = Func.GetProjectPath() + "/Assets/AssetBundles/Tables";
        _tableDatas = new Dictionary<TableType, List<TRFoundation>>();
        _tableTypeDic = new Dictionary<Type, TableType>();
        for (int i = 0; i < (int)TableType.Length; ++i)
        {
            var type = (TableType)i;
            var recordList = new List<TRFoundation>();
            string path = xmlRoot + string.Format("/TR{0}.xml", type);
            Tables.ReadTable(path, recordList, type, type.ToString());
            if (recordList.Count > 0)
                _tableTypeDic[recordList[0].GetType()] = type;
            _tableDatas.Add(type, recordList);
        }
#else

#endif
    }

    public static bool ReadTable(string xmlFileName, List<TRFoundation> records, TableType type, string tableName)
    {
        XmlReaderSettings settings = XmlHelper.GetDefaultReadSetting();
        using (XmlReader reader = XmlReader.Create(xmlFileName, settings))
        {
            using (XmlReader reader2 = XmlReader.Create(xmlFileName, settings))
            {
                try
                {
                    if (reader == null)
                    {
                        Debug.LogError("[Failed] read table: " + xmlFileName);
                        return false;
                    }

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(reader2);

                    string recordName = "TR" + type.ToString();
                    int elementCount = xmlDoc.SelectNodes("Root/" + recordName).Count;
                    if (elementCount == 0)
                    {
                        Debug.LogError("[Failed]Table record's count is 0: tableRecordName:" + recordName);
                        return false;
                    }

                    int lineCount = 0;
                    TRFoundation.ReadStartRoot(reader);
                    while (lineCount < elementCount)
                    {
                        TRFoundation newRecord = CreateRecord(type);
                        if (newRecord == null)
                        {
                            reader.Close();
                            return false;
                        }
                        if (!newRecord.ReadRawData(reader))
                        {
                            Debug.LogError($"[Failed] {tableName} table wrong line: {lineCount + 1}, index: {newRecord.Index}");
                            reader.Close();
                            return false;
                        }
                        records.Add(newRecord);
                        lineCount++;
                    }

                    TRFoundation.ReadEndRoot(reader);
                    reader.Close();
                }
                catch (XmlException e)
                {
                    Debug.LogError($"[Failed] invalid Table:, path: {xmlFileName}\n{e.Message}");
                    return false;
                }
            }
        }
        return true;
    }

    private static TRFoundation CreateRecord(TableType type)
    {
        TRFoundation result = null;
        switch (type)
        {
            case TableType.Player:
                return new TRPlayer();
            case TableType.Map:
                return new TRMap();
        }
        return result;
    }

    public List<T> GetTable<T>() where T : TRFoundation
    {
        TableType type = _tableTypeDic[typeof(T)];
        List<T> tables = new List<T>();
        for(int i = 0; i < _tableDatas[type].Count; i++)
        {
            tables.Add((T)_tableDatas[type][i]);
        }
        return tables;
    }

    public T GetRecord<T>(System.Func<T, bool> selector) where T : TRFoundation
    {
        TableType type = _tableTypeDic[typeof(T)];
        return GetRecord<T>(type, selector);
    }

    T GetRecord<T>(TableType type, System.Func<T, bool> selector) where T : TRFoundation
    {
        var recordList = _tableDatas[type];
        for (int i = 0; i < recordList.Count; ++i)
        {
            T tr = (T)recordList[i];
            if (selector(tr))
                return tr;
        }
        return default(T);
    }

    public List<T> GetRecords<T>(System.Func<T, bool> selector) where T : TRFoundation
    {
        TableType type = _tableTypeDic[typeof(T)];
        return GetRecords<T>(type, selector);
    }

    List<T> GetRecords<T>(TableType type, System.Func<T, bool> selector) where T : TRFoundation
    {
        var recordList = _tableDatas[type];
        List<T> records = new List<T>();
        for (int i = 0; i < recordList.Count; ++i)
        {
            T tr = (T)recordList[i];
            if (selector(tr))
                records.Add(tr);
        }
        return records;
    }
}
