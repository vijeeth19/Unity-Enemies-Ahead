using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class ItemDetails : ScriptableObject
{
    public string name, power;
    public int cost, numberOfItems, id, level;
    public Sprite image;
    public enum ItemType{Shoot, Build};
    public ItemType type;
}
