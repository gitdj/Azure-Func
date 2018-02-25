public class Product
{
    public string ProductName { get; set; }
    public string ProductID { get; set; }
    public string Unit { get; set; }
}

public class RootObject
{
    public Product Product { get; set; }
}