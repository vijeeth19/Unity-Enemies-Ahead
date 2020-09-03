using System.Collections;
using System.Collections.Generic;
using System;

public class WallEventArgs : EventArgs
{
    public int column;
    public float z;
    public bool wallAdded; //if true - added, if false - removed

    public WallEventArgs(int column, float z, bool wallAdded){
        this.column = column;
        this.z = z;
        this.wallAdded = wallAdded;
    }
    
}
