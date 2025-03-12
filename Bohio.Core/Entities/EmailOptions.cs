namespace Bohio.Core.Entities;

public class EmailOptions()
{
  public required string From { get; set; }
  public required string To { get; set; }
  public required string HtmlBody { get; set; }
  public required string TextBody { get; set; }
  public required string Subject { get; set; }
}