namespace SystemLibrary.Common.Episerver;

/// <summary>
/// Enables the ContentData to only be created once, then it will be hidden from the "New Content Dialogs" in the CMS.
/// If you delete the content it will be visible again
/// </summary>
public interface ISingleton
{
}
