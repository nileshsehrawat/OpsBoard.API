namespace OpsBoard.API.Models {}

public class Organisation{
  public int Id{ get; set;}
  public required string Name{ get; set;}
  public List<Resource> Resources{ get; set;} = new();
}
