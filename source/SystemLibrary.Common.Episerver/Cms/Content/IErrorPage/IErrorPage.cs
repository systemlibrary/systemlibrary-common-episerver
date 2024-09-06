namespace SystemLibrary.Common.Episerver;

/// <summary>
/// Implement your own 'error page' look and feel for responses larger than 399, and for responses not equal to 503
/// </summary>
/// <remarks>
/// IErrorPages are cached for 600 seconds, so a new IErrorPage take up to 600 seconds before being served as a response
/// </remarks>
public interface IErrorPage
{
    IList<int> StatusCodes { get; set; }
}
