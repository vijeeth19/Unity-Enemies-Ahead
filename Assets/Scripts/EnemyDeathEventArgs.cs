using System.Collections;
using System.Collections.Generic;
using System;

public class EnemyDeathEventArgs : EventArgs
{
    public String enemyType;
    public EnemyDeathEventArgs(String enemyType){
        this.enemyType = enemyType;
    }
}
