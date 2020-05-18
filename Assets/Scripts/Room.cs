using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Room : MonoBehaviour
{

    public int rows;
    public int columns;

    public void SetSize(int r, int c) {
        rows = r;
        columns = c;
    }
}
