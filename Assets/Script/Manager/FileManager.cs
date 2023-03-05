using System.Xml;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileManager
{
    private static FileManager _instance;
    public static FileManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new FileManager();
            return _instance; 
        }
    }

    public void SaveMapData(MapData mapData)
    {
        Debug.Log("Save Map Data");
        string path = "Assets/Resources/MapData/";
        string fileName = mapData.MapName + ".xml";
        
        XmlDocument xmlFile = new XmlDocument();
        xmlFile.AppendChild(xmlFile.CreateXmlDeclaration("1.0", "utf-8", "yes"));

        XmlNode rootNode = xmlFile.CreateNode(XmlNodeType.Element, "MapInfo", string.Empty);
        xmlFile.AppendChild(rootNode);

        XmlElement mapSize = xmlFile.CreateElement("MapSize");
        mapSize.InnerText = mapData.SizeX + " " + mapData.SizeY + " " + mapData.SizeZ;
        rootNode.AppendChild(mapSize);

        for (int i = 0; i < mapData.HexDatas.Count; i++)
        {
            XmlElement hexData = xmlFile.CreateElement("HexData");
            rootNode.AppendChild(hexData);

            XmlElement index = xmlFile.CreateElement("Index");
            index.InnerText = mapData.HexDatas[i].HexInfoIndex.ToString();
            hexData.AppendChild(index);

            XmlElement mapPos = xmlFile.CreateElement("MapPos");
            mapPos.InnerText = mapData.HexDatas[i].MapPos.ToString();
            hexData.AppendChild(mapPos);

            XmlElement possibleType = xmlFile.CreateElement("PossibleType");
            possibleType.InnerText = ((int)mapData.HexDatas[i].PossibleType).ToString();
            hexData.AppendChild(possibleType);

            XmlElement areaType = xmlFile.CreateElement("AreaType");
            areaType.InnerText = ((int)mapData.HexDatas[i].AreaType).ToString();
            hexData.AppendChild(areaType);
        }
        
        xmlFile.Save(path + fileName);
    }

    public MapData LoadMapData(string mapName)
    {
        Debug.Log("Load Map Data : " + mapName);
        //string path = "Assets/Resources/MapData/";
        //string fileName = path + mapName + ".xml";
        //string fileName = mapName + ".xml";

        TextAsset txtAsset = (TextAsset)Resources.Load("MapData/" + mapName);

        XmlDocument xmlFile = new XmlDocument();
        //xmlFile.Load(fileName);
        xmlFile.LoadXml(txtAsset.text);

        XmlNode mapSize = xmlFile.SelectSingleNode("MapInfo/MapSize");
        string mapSizeString = mapSize.InnerText;
        string[] sizes = mapSizeString.Split(' ');
        MapData mapDataInfo = new MapData();
        mapDataInfo.SizeX = int.Parse(sizes[0]);
        mapDataInfo.SizeY = int.Parse(sizes[1]);
        mapDataInfo.SizeZ = int.Parse(sizes[2]); 
        mapDataInfo.HexDatas = new List<MapData.HexData>();
        XmlNodeList infos = xmlFile.SelectNodes("MapInfo/HexData");
        foreach (XmlNode node in infos)
        {
            string index = node["Index"].InnerText;
            string mapPosString = node["MapPos"].InnerText;
            string[] mapPos = mapPosString.Split(' ');
            string possibleType = node["PossibleType"].InnerText;
            string areaType = node["AreaType"].InnerText;

            MapData.HexData hexData = new MapData.HexData();
            hexData.HexInfoIndex = int.Parse(index);
            hexData.MapPos = new HexPoint(int.Parse(mapPos[0]), int.Parse(mapPos[1]), int.Parse(mapPos[2]));
            hexData.PossibleType = (HexTile.PossibleType)(int.Parse(possibleType));
            hexData.AreaType = (HexTile.SpawnAreaType)(int.Parse(areaType));
            mapDataInfo.HexDatas.Add(hexData);
        }

        mapDataInfo.MapName = mapName;
        return mapDataInfo;
    }

    public void SaveHexData()
    {
        Debug.Log("Save Map Data");
        string fileName = "Assets/Resources/MapData/HexData.xml";
        //
        XmlDocument xmlFile = new XmlDocument();
        xmlFile.AppendChild(xmlFile.CreateXmlDeclaration("1.0", "utf-8", "yes"));

        XmlNode rootNode = xmlFile.CreateNode(XmlNodeType.Element, "HexInfo", string.Empty);
        xmlFile.AppendChild(rootNode);

        XmlNode mainNode = xmlFile.CreateNode(XmlNodeType.Element, "Infos", string.Empty);
        rootNode.AppendChild(mainNode);

        XmlElement Index = xmlFile.CreateElement("Index");
        Index.InnerText = "0";
        mainNode.AppendChild(Index);

        XmlElement name = xmlFile.CreateElement("Name");
        name.InnerText = "AllPossible";
        mainNode.AppendChild(name);

        XmlElement objName = xmlFile.CreateElement("ObjName");
        objName.InnerText = "NormalHex";
        mainNode.AppendChild(objName);

        xmlFile.Save(fileName);
    }

    public List<TRHex> LoadHexTable()
    {
        List<TRHex> datas = new List<TRHex>();
        Debug.Log("Load Hex Items");
        //string path = "Assets/Resources/Tables/";
        //string fileName = path + "HexData.xml";
        string fileName = "HexData";

        TextAsset txtAsset = (TextAsset)Resources.Load("Tables/" + fileName);

        XmlDocument xmlFile = new XmlDocument();
        //xmlFile.Load(fileName);
        xmlFile.LoadXml(txtAsset.text);

        XmlNodeList infos = xmlFile.SelectNodes("HexInfo/Infos");
        foreach(XmlNode node in infos)
        {
            string index = node["Index"].InnerText;
            string name = node["Name"].InnerText;
            string objName = node["ObjName"].InnerText;

            TRHex newInfo = new TRHex();
            newInfo.Index = int.Parse(index);
            newInfo.Name = name;
            newInfo.ObjName = objName;
            datas.Add(newInfo);
        }

        return datas;
    }

    public void SaveMapTable()
    {
        Debug.Log("Save Map Data");
        string fileName = "Assets/Resources/Tables/MapTable.xml";
        //
        XmlDocument xmlFile = new XmlDocument();
        xmlFile.AppendChild(xmlFile.CreateXmlDeclaration("1.0", "utf-8", "yes"));

        XmlNode rootNode = xmlFile.CreateNode(XmlNodeType.Element, "MapList", string.Empty);
        xmlFile.AppendChild(rootNode);


        for (int i = 1; i < 4; i++)
        {
            XmlNode mainNode = xmlFile.CreateNode(XmlNodeType.Element, "MapData", string.Empty);
            rootNode.AppendChild(mainNode);

            XmlElement Index = xmlFile.CreateElement("Index");
            Index.InnerText = i.ToString();
            mainNode.AppendChild(Index);

            XmlElement name = xmlFile.CreateElement("MapName");
            name.InnerText = "Test" + i;
            mainNode.AppendChild(name);
        }

        xmlFile.Save(fileName);
    }

    public List<TRMap> LoadMapTable()
    {
        Debug.Log("Load Map Items");
        //string path = "Assets/Resources/Tables/";
        //string fileName = path + "MapTable.xml";
        string fileName = "MapTable";

        TextAsset txtAsset = (TextAsset)Resources.Load("Tables/" + fileName);

        XmlDocument xmlFile = new XmlDocument();
        //xmlFile.Load(fileName);
        xmlFile.LoadXml(txtAsset.text);

        List<TRMap> mapItems = new List<TRMap>();
        XmlNodeList infos = xmlFile.SelectNodes("MapList/MapData");
        foreach (XmlNode node in infos)
        {
            string index = node["Index"].InnerText;
            string name = node["MapName"].InnerText;

            TRMap mapItem = new TRMap();
            mapItem.Index = int.Parse(index);
            mapItem.MapName = name;
            mapItems.Add(mapItem);
        }

        return mapItems;
    }

    public void SaveDungeonData(DungeonData data)
    {
        Debug.Log("Save Dungeon Data");
        string path = "Assets/Resources/DungeonData/";
        string fileName = data.DungeonName + ".xml";

        XmlDocument xmlFile = new XmlDocument();
        xmlFile.AppendChild(xmlFile.CreateXmlDeclaration("1.0", "utf-8", "yes"));

        XmlNode rootNode = xmlFile.CreateNode(XmlNodeType.Element, "DungeonInfo", string.Empty);
        xmlFile.AppendChild(rootNode);

        for(int i = 0; i < data.Infos.Count; i++)
        {
            XmlElement info = xmlFile.CreateElement("Info");
            rootNode.AppendChild(info);

            XmlElement dungeonType = xmlFile.CreateElement("DungeonType");
            dungeonType.InnerText = ((int)data.Infos[i].dungeonType).ToString();
            info.AppendChild(dungeonType);

            XmlElement squPoint = xmlFile.CreateElement("SquPoint");
            squPoint.InnerText = data.Infos[i].Point.ToString();
            info.AppendChild(squPoint);

            foreach (KeyValuePair<DirectionType, SquPoint> kv in data.Infos[i].Path)
            {
                XmlElement pathInfo = xmlFile.CreateElement("PathInfo");
                info.AppendChild(pathInfo);

                XmlElement directionType = xmlFile.CreateElement("DirectionType");
                directionType.InnerText = ((int)kv.Key).ToString();
                pathInfo.AppendChild(directionType);

                XmlElement endTilePoint = xmlFile.CreateElement("EndTilePoint");
                endTilePoint.InnerText = kv.Value.ToString();
                pathInfo.AppendChild(endTilePoint);
            }
        }
        xmlFile.Save(path + fileName);
    }

    public DungeonData LoadDungeonData(string dungeonName)
    {
        Debug.Log("Load Map Data : " + dungeonName);

        TextAsset txtAsset = (TextAsset)Resources.Load("DungeonData/" + dungeonName);

        XmlDocument xmlFile = new XmlDocument();
        xmlFile.LoadXml(txtAsset.text);

        DungeonData data = new DungeonData();
        data.Infos = new List<DungeonData.Info>();
        data.DungeonName = dungeonName;

        XmlNodeList infos = xmlFile.SelectNodes("DungeonInfo/Info");
        foreach(XmlNode info in infos)
        {
            DungeonData.Info newInfo = new DungeonData.Info();
            XmlNode squPointNode = xmlFile.SelectSingleNode("DungeonInfo/Info/SquPoint");
            string dungeonTypeStr = info["DungeonType"].InnerText;
            newInfo.dungeonType = (DungeonTypes)int.Parse(dungeonTypeStr);

            string squPointString = info["SquPoint"].InnerText;
            string[] suqPoints = squPointString.Split(' ');

            newInfo.Point = new SquPoint(int.Parse(suqPoints[0]), int.Parse(suqPoints[1]));
            newInfo.Path = new Dictionary<DirectionType, SquPoint>();
            
            XmlNodeList paths = info.SelectNodes("PathInfo");
            foreach (XmlNode node in paths)
            {
                string dirString = node["DirectionType"].InnerText;
                DirectionType dirType = (DirectionType)(int.Parse(dirString));

                string endPointString = node["EndTilePoint"].InnerText;
                string[] endTilePoints = endPointString.Split(' ');
                newInfo.Path.Add(dirType, new SquPoint(int.Parse(endTilePoints[0]), int.Parse(endTilePoints[1])));
                data.Infos.Add(newInfo);
            }
            data.Infos.Add(newInfo);
        }
        return data;
    }
}
