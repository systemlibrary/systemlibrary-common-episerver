﻿using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

using EPiServer;
using EPiServer.Core;
using EPiServer.SpecializedProperties;
using EPiServer.Validation;

using SystemLibrary.Common.Episerver.Extensions;

namespace SystemLibrary.Common.Episerver;

/// <summary>
/// Validator class supports Error, Warning and Information messages displayed upon when editors in the CMS clicks 'Publish'
/// <para>- Error messages prevents the content from being published</para>
/// <para>- Info and Warning messages does not prevent the content from being published</para>
/// </summary>
/// <example>
/// <code>
/// public class StartPage : PageData 
/// {
///     public virtual XhtmlString Title { get; set; }
/// }
/// 
/// public class StartPageValidator : Validator&lt;StartPage&gt;
/// {
///     public override void OnValidate(StartPage instance) 
///     {
///         if(!IsValid(instance.Title)) //Inherited method
///         {
///             Error(nameof(instance.Title)); //Inherited method, has built-in message as: 'Property: must be set'
///             Info(nameof(instance.Title)); //Inherited method, has built-in message as: 'Property: should be set'
///             Warning(nameof(instance.Title)); //Inherited method, has built-in message as: 'Property: can be set'
///             
///             //Pass a custom message?
///             //Note: the 'Title', property name, will be prefixed with the message automatically, if message does not contain it already
///             Error(nameof(instance.Title), "must be between 1-255 chars long");
///         }
///     }
/// }
/// </code>
/// </example>
public abstract class Validator<T> : IValidate<T> where T : IContentData
{
    List<ValidationError> Validations = new List<ValidationError>();

    /// <summary>
    /// Override the OnValidate method and add validation logic and validation messages
    /// </summary>
    /// <example>
    /// <code>
    /// public class StartPage : PageData 
    /// {
    ///     public virtual XhtmlString Title { get; set; }
    /// }
    /// 
    /// public class StartPageValidator : Validator&lt;StartPage&gt;
    /// {
    ///     public override void OnValidate(StartPage instance) 
    ///     {
    ///         if(!IsValid(instance.Title))  //Inherited method
    ///         {
    ///             Error(nameof(instance.Title)); //Inherited method
    ///             Info(nameof(instance.Title)); //Inherited method
    ///             Warning(nameof(instance.Title)); //Inherited method
    ///             
    ///             //Pass a custom message?
    ///             //Note: the 'Title', property name, will be prefixed with the message automatically, if message does not contain it already
    ///             Error(nameof(instance.Title), "must be between 1-255 chars long");
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    public abstract void OnValidate(T content);

    /// <summary>
    /// Internal method: CMS invokes this
    /// </summary>
    public IEnumerable<ValidationError> Validate(T instance)
    {
        Validations ??= new List<ValidationError>();

        Validations.Clear();
        try
        {
            if (instance != null)
                OnValidate(instance);
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            Add(null, "Exception occured, let your developers know: " + ex.Message + ". Publishing flow continues as normal...", ValidationErrorSeverity.Warning);
        }
        return Validations;
    }

    /// <summary>
    /// Display 'red error message' on publishing content
    /// <para>- This prevents the content from being published</para>
    /// Default 'message' is set to "must be set"
    /// </summary>
    /// <remarks>
    /// Message is prefixed with property name (or its display name if it has the display attribute), unless message contains the prefix value already
    /// </remarks>
    /// <example>
    /// <code>
    /// public override void OnValidate(StartPage instance) 
    /// {
    ///     Error(nameof(instance.Title));
    ///     
    ///     //Message is prefixed with property name (or its display name if it has the display attribute), unless message contains the prefix value already
    ///     Error(nameof(instance.Title), "must be between 1-255 chars long");
    /// }
    /// </code>
    /// </example>
    public void Error(string propertyName, string message = "must be set")
    {
        Add(propertyName, message, ValidationErrorSeverity.Error);
    }

    /// <summary>
    /// Display 'orange warning message' on publishing content
    /// <para>- This does not prevent the content from being published</para>
    /// Default 'message' is set to "should be set"
    /// </summary>
    /// <remarks>
    /// Message is prefixed with property name (or its display name if it has the display attribute), unless message contains the prefix value already
    /// </remarks>
    /// <example>
    /// <code>
    /// public override void OnValidate(StartPage instance) 
    /// {
    ///     Warning(nameof(instance.Title));
    ///     
    ///     //Message is prefixed with property name (or its display name if it has the display attribute), unless message contains the prefix value already
    ///     Warning(nameof(instance.Title), "must be between 1-255 chars long");
    /// }
    /// </code>
    /// </example>
    public void Warning(string propertyName, string message = "should be set")
    {
        Add(propertyName, message, ValidationErrorSeverity.Warning);
    }

    /// <summary>
    /// Display 'blue info message' on publishing content
    /// <para>- This does not prevent the content from being published</para>
    /// Default 'message' is set to "can be set"
    /// </summary>
    /// <remarks>
    /// Message is prefixed with property name (or its display name if it has the display attribute), unless message contains the prefix value already
    /// </remarks>
    /// <example>
    /// <code>
    /// public override void OnValidate(StartPage instance) 
    /// {
    ///     Info(nameof(instance.Title));
    ///     
    ///     //Message is prefixed with property name (or its display name if it has the display attribute), unless message contains the prefix value already
    ///     Info(nameof(instance.Title), "must be between 1-255 chars long");
    /// }
    /// </code>
    /// </example>
    public void Info(string propertyName, string message = "can be set")
    {
        Add(propertyName, message, ValidationErrorSeverity.Info);
    }

    /// <summary>
    /// Returns true if the value is something, else false
    /// <para>A quick and simple validation to see that the 'value' is filled out with something, for properties of type:</para>
    /// string, int, ContentReference, LinkItem, LinkItemCollection, ContentArea, XhtmlString, IList, DateTime, Uri, ContentLink, Url and Enums
    /// </summary>
    /// <remarks>
    /// Simple check to validate that input in string, xhtmlstring or similar is something more than a blank
    /// <para>Simple check to validate that links at least have a href, does not force a title nor a name</para>
    /// Simple check that content area has at least one item
    /// <para>Simple check that an IList has at least one item</para>
    /// </remarks>
    public bool IsValid(object value)
    {
        if (value == null || value == "" || value + "" == "0" || value == " " || (value is DateTime dt && dt == DateTime.MinValue))
        {
            return false;
        }
        else if (value is LinkItem li)
        {
            return li != null && li.Href != null;
        }
        else if (value is LinkItemCollection lic)
        {
            return lic?.Count > 0;
        }
        else if (value is ContentReference contentReference)
        {
            return contentReference.Is();
        }
        else if (value is PageReference pageReference)
        {
            return pageReference.Is();
        }
        else if (value is ContentArea contentArea)
        {
            return contentArea.Is();
        }
        else if (value is Uri uri)
        {
            return uri != null && uri.OriginalString.Is();
        }
        else if (value is Url url)
        {
            return url != null && url.OriginalString.Is();
        }
        else if (value is XhtmlString xhtmlString)
        {
            return xhtmlString.Is();
        }
        else if (value is IList ilist)
        {
            return ilist.Count > 0;
        }

        try
        {
            var i = int.Parse(value.ToString());
            return i != 0;
        }
        catch
        {
        }
        return true;
    }

    void Add(string propertyName, string message, ValidationErrorSeverity severity)
    {
        try
        {
            var propertyDisplayName = GetPropertyDisplayName(propertyName);

            for (int i = 0; i < Validations.Count; i++)
            {
                if (Validations[i].ErrorMessage == message)
                {
                    if ((int)Validations[i].Severity >= (int)severity)
                        return;

                    Validations.RemoveAt(i);
                    break;
                }
            }

            if (propertyDisplayName != null && !message.Contains(propertyDisplayName))
                message = propertyDisplayName + ": " + message;

            Validations.Add(new ValidationError
            {
                ErrorMessage = message,
                PropertyName = propertyDisplayName ?? "",
                Severity = severity,
                ValidationType = ValidationErrorType.AttributeMatched
            });
        }
        catch (Exception ex)
        {
            Validations.Add(new ValidationError
            {
                ErrorMessage = "Validation failed: " + ex.Message,
                PropertyName = null,
                Severity = ValidationErrorSeverity.Error,
                ValidationType = ValidationErrorType.Unspecified
            });
        }
    }

    static string GetPropertyDisplayName(string propertyName)
    {
        try
        {
            if (propertyName.IsNot()) return null;

            var property = typeof(T).GetProperty(propertyName);

            if (property == null) return propertyName;

            var display = property?.GetCustomAttribute<DisplayAttribute>();

            if (display?.Name != null)
                return display.Name;

            return property?.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? propertyName;
        }
        catch
        {
            return null;
        }
    }
}

