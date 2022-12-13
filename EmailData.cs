namespace Email
{
  public class EmailOrder
  {
    public string Email { get; set; }
    public string RestaurantName { get; set; }
    public string Date { get; set; }
    public string Hour { get; set; }
    public string Mins { get; set; }    
    public string Address { get; set; }
    public string Phone { get; set; }
    public Order[] Orders { get; set; }

    public class Order {
      public string Name { get; set; }
      public string Amount { get; set; }
    }
  }
  public class EmailInvoice
  {
    public string Email { get; set; }
    public string RestaurantName { get; set; }
    public string Phone { get; set; }
    public Order[] Orders { get; set; }
    public int SumPrice { get; set; }

    public class Order {
      public string Name { get; set; }
      public int Amount { get; set; }
      public int Price { get; set; }
    }
  }

  public class EmailRegConfirmation{
    public string Email { get; set; }
    public string Code { get; set; }
  }
}