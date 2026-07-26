using System.Text.Json.Serialization;

namespace SibersTest.Domain.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProjectTaskStatus
    {
        ToDo,
        Progress,
        Done
    }
}