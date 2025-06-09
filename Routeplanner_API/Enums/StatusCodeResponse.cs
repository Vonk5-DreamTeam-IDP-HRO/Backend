namespace Routeplanner_API.Enums
{
    /// <summary>
    /// Represents standard HTTP status codes used for API responses.
    /// </summary>
    public enum StatusCodeResponse
    {
        /// <summary>
        /// Indicates a successful request (HTTP 200).
        /// </summary>
        Success = 200,

        /// <summary>
        /// Indicates that a resource was successfully created (HTTP 201).
        /// </summary>
        Created = 201,

        /// <summary>
        /// Indicates a successful request with no content to return (HTTP 204).
        /// </summary>
        NoContent = 204,

        /// <summary>
        /// Indicates a bad request due to invalid data or parameters (HTTP 400).
        /// </summary>
        BadRequest = 400,

        /// <summary>
        /// Indicates an unauthorized request (HTTP 401).
        /// </summary>
        Unauthorized = 401,

        /// <summary>
        /// Indicates that the requested resource was not found (HTTP 404).
        /// </summary>
        NotFound = 404,

        /// <summary>
        /// Indicates a server-side error (HTTP 500).
        /// </summary>
        InternalServerError = 500,
    }
}
