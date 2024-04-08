using System.Dynamic;
using System.Reflection;

using React;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class TExtensions
{
    static FieldInfo __props;
    static FieldInfo _props
    {
        get
        {
            if (__props == null)
                __props = typeof(ReactComponent).GetField("_props", BindingFlags.Instance | BindingFlags.NonPublic);

            return __props;
        }
    }
    static FieldInfo __serializedProps;
    static FieldInfo _serializedProps
    {
        get
        {
            if (__serializedProps == null)
                __serializedProps = typeof(ReactComponent).GetField("_serializedProps", BindingFlags.Instance | BindingFlags.NonPublic);

            return __serializedProps;
        }
    }

    static IReactComponent CreateServerSideComponent(ExpandoObject props, string jsonProps, string componentFullName, string tagName, string cssClass, bool renderServerSide, bool renderClientSide)
    {
        if (!renderServerSide) return null;

        // NOTE: Sending dummy data as CreateComponent immediately serializes the props using Newtonsoft
        // NOTE: System.Text.Json uses less memory, and is faster
        // NOTE: For client side, we also need json serialized, adding it to the attribute 'data-rcssr-props'
        // NOTE: Newtonsoft's default escaping is not sufficient, and EscapeHtml generates 10% more text than System.Text.Json

        var dummy = new Dictionary<string, object>();

        IReactComponent component = ReactEnvironment.GetCurrentOrThrow.CreateComponent(componentFullName, dummy, null, false, true, true);
        component.ContainerClass = renderClientSide ? null : cssClass;
        component.ContainerTag = renderClientSide ? null : tagName;

        _props.SetValue(component, props);
        _serializedProps.SetValue(component, jsonProps);

        return component;

        //content.Append(component.RenderHtml(false, true));

        //if (tagName.IsNot() && renderClientSide)
        //{
        //    var space = content?.IndexOf(" ") ?? 0;
        //    if (space > 0 && space < 18)
        //    {
        //        content.Insert(space, " data-rcssr-id=\"" + key + "\"");

        //        if (debug)
        //            Log.Debug("Inserted data-rcssr-id=" + key + " at " + space);
        //    }
        //}
    }
}