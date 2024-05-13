using System.ComponentModel.DataAnnotations;

namespace SystemLibrary.Common.Episerver.Cms;

/// <summary>
/// Implement your own 'error page' look and feel for responses larger than 399, and for responses not equal to 503
/// </summary>
public interface IErrorPage
{
    IList<int> StatusCodes { get; set; }
}
