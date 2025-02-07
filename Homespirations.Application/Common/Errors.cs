using Homespirations.Core.Results;

namespace Application.Common.Errors;

public static class Errors
{
    public static class HomeSpace
    {
        public static readonly Error NotFound = new("HomeSpace.NotFound", "The requested home space was not found.");
        public static readonly Error AlreadyExists = new("HomeSpace.AlreadyExists", "A home space with the given details already exists.");
        public static readonly Error InvalidData = new("HomeSpace.InvalidData", "The provided home space data is invalid.");
    }

}