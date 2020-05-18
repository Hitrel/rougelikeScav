using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {
    [Serializable]
    [HideInInspector] public class RoomType {
        public int rows { get; }
        public int columns { get; }

        public RoomType(int rows, int cols) {
            this.rows = rows;
            this.columns = cols;
        }
        public (int, int) GetRoom() { return (rows, columns); }
    }

    public enum Direction { N, S, W, E, NW, NE, SW, SE};

    class Direct {
        public static Direction GetDirection(Vector3 p1, Vector3 p2) {
            Vector3 temp = p1 - p2;
            if (temp.x < 0 && temp.y < 0) {
                return Direction.NE;
            } else if (temp.x < 0 && temp.y == 0) {
                return Direction.E;
            } else if (temp.x < 0 && temp.y > 0) {
                return Direction.SE;
            } else if (temp.x == 0 && temp.y < 0) {
                return Direction.N;
            } else if (temp.x == 0 && temp.y > 0) {
                return Direction.S;
            } else if (temp.x > 0 && temp.y < 0) {
                return Direction.NW;
            } else if (temp.x > 0 && temp.y == 0) {
                return Direction.W;
            } else if (temp.x > 0 && temp.y > 0) {
                return Direction.SW;
            }

            throw new Exception("Error Direction {}" + temp.ToString());
        }
    }

    [HideInInspector] public GameObject startRoom;

    [HideInInspector] public GameObject exitRoom;

    public List<GameObject> rooms;

    [HideInInspector] public RoomManager roomManager;

    public GameObject roomTemplate;

    private int positionShift;

    // Path Generation
    [HideInInspector] public class Path {
        public int left_index { get; set; }
        public int right_index { get; set; }
        public float distance { get; set; }

        public Path(int l, int r, float d) {
            left_index = l;
            right_index = r;
            distance = d;
        }
    }

    private List<Path> paths;

    private List<Path> tree;


    private void Init(int level) {
        rooms = new List<GameObject>();

        startRoom = GameObject.Find("StartRoom");
        exitRoom = GameObject.Find("ExitRoom");
        roomManager = GetComponent<RoomManager>();

        exitRoom.SetActive(false);

        positionShift = (int)Mathf.Log(level + 1, 2) * 10;

        positionShift = positionShift > 40 ? 40 : positionShift;

        paths = new List<Path>();

        tree = new List<Path>();

    }

    public void SetupScene(int level) {
        Init(level);

        rooms.Add(startRoom);

        int roomGenerateTimes = (int)Mathf.Log(level + 1, 2) * 5;

        for (int i = 0; i < roomGenerateTimes; i++) {
            Vector3 randomPosition = RandomPosition();
            RoomType randomRoomType = RandomType();

            GameObject newRoom = Instantiate(roomTemplate, randomPosition, Quaternion.identity) as GameObject;
            newRoom.GetComponent<Room>().SetSize(randomRoomType.rows, randomRoomType.columns);

            foreach (GameObject room in rooms) {
                if (newRoom == null) {
                    break;
                }

                if (IsOverlap(newRoom, room)) {
                    Destroy(newRoom);
                    newRoom = null;
                }
            }

            if (newRoom != null) {
                roomManager.SetupScene(level, newRoom);
                rooms.Add(newRoom);
            }
        }

        while (true) {
        start:
            Vector3 randomPosition = RandomPosition();

            exitRoom.transform.position = randomPosition;

            foreach (GameObject room in rooms) {
                if (IsOverlap(exitRoom, room)) {
                    goto start;
                }
            }

            exitRoom.SetActive(true);
            rooms.Add(exitRoom);
            break;
        }

        PathFindMGT();

    }

    private bool IsOverlap(GameObject room1, GameObject room2) {

        RoomType getSize(Room room) {
            return new RoomType(room.rows + 1, room.columns + 1);
        }

        Vector3 room1Position = room1.transform.position;
        RoomType room1Size = getSize(room1.GetComponent<Room>());

        Vector3 room2Position = room2.transform.position;
        RoomType room2Size = getSize(room2.GetComponent<Room>());

        if (room1Position.x + room1Size.columns >= room2Position.x &&
            room2Position.x + room2Size.columns >= room1Position.x &&
            room1Position.y + room1Size.rows >= room2Position.y &&
            room2Position.y + room2Size.rows >= room1Position.y) {

            return true;
        } else {
            return false;
        }
    }

    private Vector3 RandomPosition() {
        int randomX = (int)Random.Range(startRoom.transform.position.x - positionShift, startRoom.transform.position.x + positionShift);
        int randomY = (int)Random.Range(startRoom.transform.position.y - positionShift, startRoom.transform.position.y + positionShift);

        return new Vector3(randomX, randomY, startRoom.transform.position.z);
    }

    private RoomType RandomType() {
        int randomRows = Random.Range(3, 10);
        int randomCols = Random.Range(3, 10);
        return new RoomType(randomRows, randomCols);
    }

    private void PathFindMGT() {

        for (int i = 0; i < rooms.Count; i++) {
            for (int j = 0; j < rooms.Count; j++) {
                if (j != i) {
                    float temp = Vector3.Distance(rooms[i].transform.position, rooms[j].transform.position);

                    paths.Add(new Path(i, j, temp));
                }
            }
        }

        List<int> traverse = new List<int>();
        traverse.Add(0);
        List<int> untraved = new List<int>(rooms.Count - 1);

        for (int i = 0; i < rooms.Count - 1; i++) {
            untraved.Add(i + 1);
        }

        while (untraved.Count > 0) {

            float minDistance = float.MaxValue;
            int left = 0;
            int right = 0;

            foreach (int i in traverse) {
                foreach (Path p in paths) {
                    if (p.left_index == i && !traverse.Contains(p.right_index)) {
                        if (p.distance < minDistance) {
                            minDistance = p.distance;
                            right = p.right_index;
                            left = i;
                        }
                    }
                }
            }
            if (!traverse.Contains(right)) {
                traverse.Add(right);
                untraved.Remove(right);
                tree.Add(new Path(left, right, minDistance));
            }

        }

        foreach (Path path in tree) {
            PathGeneration(path);
        }
    }

    private void PathGeneration(Path path) {
        
        GameObject leftRoom = rooms[path.left_index];
        GameObject rightRoom = rooms[path.right_index];
        
        
    }
}
