using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomManager : MonoBehaviour
{
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;


        public Count (int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public int columns = 0;
    public int rows = 0;

    public Count wallCount;
    public Count foodCount; 

    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;

    private Transform boardHolder;

    private List <Vector3> gridPostions = new List <Vector3> ();

    void InitializeList (Transform parent)
    {
        gridPostions.Clear ();

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                gridPostions.Add(new Vector3(x, y, 0f) + parent.position);
            }
        }
    }

    // Sets up the outer walls and floor (background) of the game board.
    void BoardSetup (GameObject room)
    {
        // Instantiate Board and set boardHolder to its transform.
        boardHolder = room.transform;

        // Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
        for (int x = -1; x < columns + 1; x++)
        {
            // Loop along y axis, start from -1 to place floor or outerwall tiles.
            for (int y = -1; y < rows + 1; y++)
            {
                // Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];

                if (x == -1 || x == columns || y == -1 || y == rows)
                {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                }
                // Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
                GameObject instance = Instantiate(toInstantiate, (new Vector3(x, y, 0f) + boardHolder.position), Quaternion.identity) as GameObject;
                // Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering herarchy.
                instance.transform.SetParent(boardHolder);

            }
        }
    }

    // RandomPosition returns a random position from our list gridPositions.
    Vector3 RandomPosition ()
    {
        // Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
        int randomIndex = Random.Range(0, gridPostions.Count);

        if (randomIndex >= gridPostions.Count) {
            throw new ArgumentOutOfRangeException("No position");
        }

        // Declare an variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
        Vector3 randomPosition = gridPostions[randomIndex];

        // Remove the entry at randomIndex from the list so that is can't be re-used.
        gridPostions.RemoveAt(randomIndex);

        // Return the randomly selected Vector3 position.
        return randomPosition;

    }
    
    // LayoutObjectAtRandom accepts an array of GameObjects to choose from along with a minimum and maximum range for the number of objects to create.
    void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum, GameObject room)
    {
        // Choose a random number of objects to instantiate within the minimum and maximum limits.
        int objectCount = Random.Range(minimum, maximum);

        // Instantiate objects until the randomly chosen limit objectCount is reached.
        for (int i = 0; i < objectCount; i++)
        {

            // Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPositions.
            Vector3 randomPosition = new Vector3(0, 0, 0);
            try {
                randomPosition = RandomPosition();
            } catch (ArgumentOutOfRangeException e) {
                break;
            }
            // Choose a random tile from tileArray and assign it to tileChoice.
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];


            // Instantiate tileChoice at the position returned by RandomPosition with no change in rotation.
            tileChoice = Instantiate(tileChoice, randomPosition, Quaternion.identity) as GameObject;
            
            // Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering herarchy.
            tileChoice.transform.SetParent(room.transform);


        }
    }

    void SetSize(GameObject room) {
        Room roomController = room.GetComponent<Room>();
        rows = roomController.rows;
        columns = roomController.columns;

        foodCount = new Count(0, (rows - 1) * (columns - 1)  / 3);
        wallCount = new Count(0, (rows - 1) * (columns - 1) * 5 / 8);
    }


    // SetupScene initializes our leve and calls the previous functions to lay out the game board.
    public void SetupScene (int level, GameObject room)
    {
        SetSize(room);

        
        // Creates the outer walls and floor.
        BoardSetup(room);

        // Reset our list of gridpositions.
        InitializeList(room.transform);

        // Instantiate a random number of wall tiles based on minimum and maximum, at randomized position.
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum, room);

        // Instantiate a random number of bood tiles based on minimum and maximum, at randomized position.
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum, room);

        // Determine number of enemies based on current level number, based on a logarithmic progression.
        int enemyCount = (int)Mathf.Log10(level+8);

        enemyCount = enemyCount > rows * columns / 3 ? 2 : enemyCount;
        // Instantiate enemyCount random type of enemies at randomized positions. 
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount, room);

    }
}
