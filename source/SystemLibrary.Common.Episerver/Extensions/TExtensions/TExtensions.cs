﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Web;

using EPiServer;
using EPiServer.Core;
using EPiServer.Shell;
using EPiServer.SpecializedProperties;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Primitives;
using Microsoft.Identity.Client.Extensions.Msal;

using React;

using SystemLibrary.Common.Episerver.Cms.Properties;
using SystemLibrary.Common.Net;
using SystemLibrary.Common.Net.Extensions;
using SystemLibrary.Common.Web;

using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SystemLibrary.Common.Episerver.Extensions;

public static partial class TExtensions
{
    internal const string SysLibComponentLevel = "___" + nameof(SysLibComponentLevel);
    internal const string SysLibComponentArgs = "___" + nameof(SysLibComponentArgs);
    internal const string SysLibComponentKeys = "___" + nameof(SysLibComponentKeys);

    public static StringBuilder ReactServerSideRender<T>(this T model, object additionalProps = null, string tagName = "div", bool camelCaseProps = false, string cssClass = null, string id = null, string componentFullName = null, bool renderClientOnly = false, bool renderServerOnly = false) where T : class
    {
        Validate(model, additionalProps, tagName, renderClientOnly, renderServerOnly);

        var props = ModelToProps(model, additionalProps, camelCaseProps);

        var jsonProps = PropsToJsonProps(props, camelCaseProps);

        var ssrId = GetSSRID(id, model, props, jsonProps);

        var renderServerSide = !renderClientOnly || renderServerOnly;
        var renderClientSide = renderClientOnly || !renderServerOnly;

        componentFullName = GetComponentFullName(model, componentFullName);

        var root = GetRootElementStart(componentFullName, id, cssClass, tagName, renderClientSide, ssrId);

        try
        {
            var serverSideComponent = CreateServerSideComponent(props, jsonProps, componentFullName, tagName, cssClass, renderServerSide, renderClientSide);

            RenderServerSideComponent(root, serverSideComponent, ssrId, tagName, renderClientSide);
        }
        catch(Exception ex)
        {
            Log.Error(ex);

            root.Append("<div>Error: " + ex.Message + "</div>");
        }
        finally
        {
            try
            {
                ReactEnvironment.GetCurrentOrThrow.ReturnEngineToPool();
            }
            catch (Exception ex)
            {
                Log.Error("React returning engine too pool failed, continue silently..." + ex.Message);
            }
        }

        AppendRootElementEnd(root, tagName);

        var keys = GetHashSet(renderClientSide);

        AppendHiddenInput(renderClientSide, ssrId, componentFullName, jsonProps, keys, root);

        return root;
    }
}
