namespace APIoxxo;

public class Item
{
    public string id { get; set; }
    public string name { get; set; }
    public int price { get; set; }
    public string prefabPath { get; set; }
    public bool isPurchased { get; set; }
    
    public Item(string id, string name, int price, string prefabPath)
    {
        this.id = id;
        this.name = name;
        this.price = price;
        this.prefabPath = prefabPath;
        this.isPurchased = false;
    }
}