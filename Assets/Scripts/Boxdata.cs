using UnityEngine;

[CreateAssetMenu(fileName = "Newbox", menuName = "Inventory/Boxes")]
public class SulyDobozData : ScriptableObject
{
    public string boxname;
    public float weight;
    public Sprite boxpicture; 
    public GameObject boxprefab;
}