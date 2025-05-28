using Routeplanner_API.Enums;

namespace Routeplanner_API.DTO
{
    public class StatusCodeResponseDto<T>
    {
        public StatusCodeResponse StatusCodeResponse;

        public string? Message;

        public T? Data; 
    }
}
