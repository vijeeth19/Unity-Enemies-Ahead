using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Array
{
    private float[] arr;
    private int length;
    private int capacity;
    private int column;
    private WallEventArgs wall;

    public Array() : this(9, 0) {}

    public Array(int capacity, int column){
        this.capacity = capacity;
        arr = new float[capacity];

        for(int i = 0; i < capacity; i++){
            arr[i] = -100f;
        }

        length = 0;
        this.column = column;
        wall = new WallEventArgs(column, 0f, false);
    }

    private void SortArray(float[] arr){

        for(int i = 0; i < length - 1; i++){
            for(int j = 0; j < length - i - 1; j++){
                if (arr[j] < arr[j+1]) {  
                    float temp = arr[j]; 
                    arr[j] = arr[j+1]; 
                    arr[j+1] = temp;
                }
            }
        }
    }

    public float Peek(int index){
        try{
            return arr[index];
        }catch(IndexOutOfRangeException e){
            Debug.Log("Error index "+index);
        }
        return -1;
    }

    public void Add(float el){
        arr[length++] = el;
        SortArray(arr);
        WallEventArgs wall = new WallEventArgs(column, el, true);
        OnWallUpdated(wall);
    }

    public int IndexOf(float el){
        for(int i = 0; i < length; i++){
            if(arr[i] == el)
                return i;
        }
        return -1;
    }

    public void Remove(float el){
        int indexOfEl = IndexOf(el);
        
            
        if(indexOfEl != -1){
            
                
            for(int i = indexOfEl; i < length - 1; i++){
                try{
                    arr[i] = arr[i+1];
                }catch(IndexOutOfRangeException e){
                    Debug.Log("Error index "+i);
                }
                
            }
            arr[length-1] = -100f;
            length--;
            Debug.Log("Removed element");
        }
        
        
        
        WallEventArgs wall = new WallEventArgs(column, el, false);
        OnWallUpdated(wall);
    }

    public delegate void WallUpdatedEventHandler(object source, WallEventArgs args);
    public event WallUpdatedEventHandler WallUpdated;

    protected virtual void OnWallUpdated(WallEventArgs wall){
        if(WallUpdated != null)
            WallUpdated(this, wall);
    }

    public int getLength(){
        return length;
    }


}
