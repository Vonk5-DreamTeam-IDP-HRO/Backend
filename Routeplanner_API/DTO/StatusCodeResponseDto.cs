using Routeplanner_API.Enums;

namespace Routeplanner_API.DTO
{
    /// <summary>
    /// Generic DTO to encapsulate a status code response, message, and optional data.
    /// </summary>
    /// <typeparam name="T">The type of the data payload.</typeparam>
    public class StatusCodeResponseDto<T>
    {
        /// <summary>
        /// Gets or sets the status code response.
        /// </summary>
        public StatusCodeResponse StatusCodeResponse { get; set; }

        /// <summary>
        /// Gets or sets an optional message describing the response.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Gets or sets the optional data payload.
        /// </summary>
        public T? Data { get; set; }
    }
}
