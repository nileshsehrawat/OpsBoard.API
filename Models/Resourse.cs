namespace OpsBoard.API.Models {}

public class Resource{
  public int Id{get; set;}
  public required string Name{ get; set;}
  public required string Type{ get; set;}
  public int OrganizationId{ get; set;}

}

