using System;

namespace SystemLibrary.Common.Episerver.Extensions;

/// <summary>
/// Contains application builder options that you can override if you want to
/// </summary>
public class UseCommonEpiserverAppOptions : Web.Extensions.CommonWebApplicationBuilderOptions
{
    public bool UseExceptionHandler = true;
    public bool MapEndpoints = true;
    public string CmsLoginPath = null;
    public Func<string, string> DefaultComponentPathPredicate = null;
}