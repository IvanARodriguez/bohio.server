namespace Bohio.Core.Interfaces
{
  public interface ITokenService
  {
    Task<string?> GetUserIdFromTokenAsync(string accessToken);
  }
}
