using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EPiServer.Core;
using EPiServer.Filters;

namespace SystemLibrary.Common.Episerver;

internal class IgnoreTemplateFilter : FilterTemplate
{
    public override bool ShouldFilter(IContent content)
    {
        return false;
    }
}