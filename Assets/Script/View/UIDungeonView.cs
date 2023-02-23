using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DirectionType
{
    Left,
    LeftUp,
    Up,
    RightUp,
    Right,
    RightDown,
    Down,
    LeftDown,
}

public class UIDungeonView : MonoBehaviour
{
    private static readonly string LayerName2D = "UI";
    private static readonly string LayerNameSpine = "Spine";
    private static readonly string LayerName3D = "3D";
    private static readonly string BackImageName = "contents_kamibase_bg";
    private const int WidthX = 2;
    private const int HeightY = 2;
    private float _sizeX = 10;
    private float _sizeY = 5f;

    [SerializeField]
    Image _backImage;

    [SerializeField]
    Transform _playerRoot;

    [SerializeField]
    float _playerMoveSpeed = 5f;

    DungeonController _controller;

    public GameObject Player => _player.gameObject;
    DungeonPlayer _player;

    public void Update()
    {
        _player?.OnUpdate();
    }

    public void SetController(DungeonController controller)
    {
        _controller = controller;
    }

    public void CreateDungeon(DungeonData data, GameObject objRoot)
    {
        _backImage.sprite = ResourcesManager.Instance.Load<Sprite>("Sprites/", BackImageName);
        // 룸 정보를 가져온다.
        // 룸정보는 squpoint로 저장되어 있으므로 squpoint를 생성해야한다.
        // 최대 squpoint x값과 y값을 먼저 산출해야한다.
        int widthX = int.MinValue;
        int heightY = int.MinValue;
        for(int i = 0; i < data.Infos.Count; i++)
        {
            widthX = widthX < data.Infos[i].Point.x ? data.Infos[i].Point.x : widthX;
            heightY = heightY < data.Infos[i].Point.y ? data.Infos[i].Point.y : heightY;
        }

        for (int i = 0; i <= widthX; i++)
        {
            for(int j = 0; j <= heightY; j++)
            {
                DungeonData.Info info = data.Infos.Find(x => x.Point == new SquPoint(i, j));
                if (info != null)
                {
                    // 룸 오브젝트 생성
                    GameObject obj = GameObject.Instantiate(ResourcesManager.Instance.Load<GameObject>("Prefabs/UI/", "DungeonItem"), objRoot.transform);
                    obj.name = "RoomItem";

                    obj.transform.localScale = Vector3.one;
                    obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
                    Tools.SetLayer(obj.transform, LayerName3D);

                    DungeonItem item = obj.GetComponent<DungeonItem>();
                    _sizeX = item.RectSize.x / 3;
                    _sizeY = item.RectSize.y / 3;

                    item.transform.position = GetWorldPos(i, j);

                    List<Tuple<DirectionType, GameObject>> pathItems = new List<Tuple<DirectionType, GameObject>>();
                    foreach (KeyValuePair<DirectionType, SquPoint> kv in info.Path)
                    {
                        GameObject pathItem = new GameObject();
                        pathItem.transform.SetParent(item.transform);
                        pathItem.transform.position = GetWorldPos(new SquPoint(i, j) + DungeonController.Dir[kv.Key]);
                        pathItems.Add(new Tuple<DirectionType, GameObject>(kv.Key, pathItem));
                    }

                    DungeonItemInfo itemData = new DungeonItemInfo();
                    itemData.Point = new SquPoint(i, j);
                    itemData.Paths = info.Path;
                    itemData.Item = item;
                    itemData.PathItems = pathItems;
                    itemData.DungeonType = info.dungeonType;
                    itemData.IsUsed = info.dungeonType == DungeonTypes.Start ? true : false;

                    _controller.AddDungeonData(itemData);
                }
            }
        }
    }

    public Vector3 GetWorldPos(SquPoint point)
    {
        return GetWorldPos(point.x, point.y);
    }

    Vector3 GetWorldPos(int x, int y)
    {
        float xPos = _sizeX * (x + 1) - _sizeX / 2;
        xPos = xPos - ((float)WidthX / (float)2 * _sizeX);

        float yPos = _sizeY * (y + 1) - _sizeY / 2;
        yPos = yPos - ((float)HeightY / (float)2 * _sizeY);
        return new Vector3(xPos, yPos, 0);
    }

    public void CreatePlayer(GameObject spineRoot)
    {
        // todo : dummy 
        TRPlayer item = DataManager.Instance.TRPlayers.Find(x => x.Index == 1);
        _player = PlayerManager.Instance.CreateDungeonPlayer(spineRoot.transform, LayerNameSpine, item.KeyName, item.SpineDataAsset);
    }

    public void MovePlayer(Vector3 dest, bool isTeleport)
    {
        if (isTeleport == true)
        {
            _player.Position = dest;
            _controller.OnEndMovePlayer();
        }
        else
        {
            Vector3 curPos = _player.Position;
            float distance = Vector3.Distance(curPos, dest);
            if (distance > 0.01f)
            {
                _player.Position += (dest - curPos).normalized * Time.deltaTime * _playerMoveSpeed;
            }
            else
            {
                //_dungeonItemRoot.transform.position = dest;
                //_controller.OnEndMovingRoomRoot();
                _player.Position = dest;
                _controller.OnEndMovePlayer();
            }
        }
    }
}
