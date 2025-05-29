using Routeplanner_API.Enums;

namespace Routeplanner_API.DTO
{
    public class StatusCodeResponseDto<T>
    {
        public StatusCodeResponse StatusCodeResponse { get; set; }

        public string? Message { get; set; }

        public T? Data { get; set; }
    }
}
