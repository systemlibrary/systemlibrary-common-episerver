using System.ComponentModel.DataAnnotations;

namespace SystemLibrary.Common.Episerver.Cms;

public interface IErrorPage
{
    IList<int> StatusCodes { get; set; }
}
