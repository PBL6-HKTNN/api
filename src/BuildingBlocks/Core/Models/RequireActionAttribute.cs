namespace Codemy.BuildingBlocks.Core.Models
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RequireActionAttribute : Attribute
    {
        public string ActionCode { get; }

        public RequireActionAttribute(string actionCode)
        {
            ActionCode = actionCode;
        }
    }

}
