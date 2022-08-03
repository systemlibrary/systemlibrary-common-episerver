using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

using EPiServer;
using EPiServer.Core;
using EPiServer.Validation;

using SystemLibrary.Common.Episerver.Extensions;

namespace SystemLibrary.Common.Episerver;

/// <summary>
/// Validator class supports Error, Warning and Information messages displayed upon when editors in the CMS clicks 'Publish'
/// 
/// - Error messages prevents the content from being published
/// - Info and Warning messages does not prevent the content from being published
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
///         if(!IsValid(instance.Title)) 
///         {
///             Error(nameof(instance.Title));
///             Info(nameof(instance.Title));
///             Warning(nameof(instance.Title));
///             
///             //Pass a custom message?
///             //Note: the 'Title', property name, will be prefixed with the message automatically, if message does not contain it already
///             Error(nameof(instance.Title), "must be between 1-255 chars long");
///         }
///     }
/// }
/// </code>
/// </example>
public abstract class Validator<T> : IValidate<T> where T : IContent
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
    ///         if(!IsValid(instance.Title)) 
    ///         {
    ///             Error(nameof(instance.Title));
    ///             Info(nameof(instance.Title));
    ///             Warning(nameof(instance.Title));
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
    /// The internal Validate method that the CMS invokes, we never call it
    /// </summary>
    public IEnumerable<ValidationError> Validate(T instance)
    {
        Validations.Clear();
        try
        {
            if (instance != null)
                OnValidate(instance);
        }
        catch (Exception ex)
        {
            Add(null, "Exception occured, let your developers know: " + ex.Message + ". The publishing continues as normal...", ValidationErrorSeverity.Warning);
        }
        return Validations;
    }

    /// <summary>
    /// Display a 'red error message' during 'OnPublish' button clicked
    /// - This prevents the content from being published
    /// 
    /// Do not pass a message, to get either 'must be set' or 'should be set' depending on the severity level
    /// </summary>
    /// <example>
    /// <code>
    /// public override void OnValidate(StartPage instance) 
    /// {
    ///     Error(nameof(instance.Title));
    ///     
    ///     //Custom message: if message does not contain 'Title' (property name), it will be prefixed automatically with it
    ///     Error(nameof(instance.Title), "must be between 1-255 chars long");
    /// }
    /// </code>
    /// </example>
    public void Error(string propertyName, string message = null)
    {
        Add(propertyName, message, ValidationErrorSeverity.Error);
    }

    /// <summary>
    /// Display a 'orange warning message' during 'OnPublish' button clicked
    /// - This does not prevent the content from being published
    ///
    /// Do not pass a message, to get either 'must be set' or 'should be set' depending on the severity level
    /// </summary>
    ///   /// <example>
    /// <code>
    /// public override void OnValidate(StartPage instance) 
    /// {
    ///     Warning(nameof(instance.Title));
    ///     
    ///     //Custom message: if message does not contain 'Title' (property name), it will be prefixed automatically with it
    ///     Warning(nameof(instance.Title), "must be between 1-255 chars long");
    /// }
    /// </code>
    /// </example>
    public void Warning(string propertyName, string message = null)
    {
        Add(propertyName, message, ValidationErrorSeverity.Warning);
    }

    /// <summary>
    /// Display a 'info message' during 'OnPublish' button clicked
    /// - This does not prevent the content from being published
    /// 
    /// Do not pass a message, to get either 'must be set' or 'should be set' depending on the severity level
    /// </summary>
    /// <example>
    /// <code>
    /// public override void OnValidate(StartPage instance) 
    /// {
    ///     Info(nameof(instance.Title));
    ///     
    ///     //Custom message: if message does not contain 'Title' (property name), it will be prefixed automatically with it
    ///     Info(nameof(instance.Title), "must be between 1-255 chars long");
    /// }
    /// </code>
    /// </example>
    public void Info(string propertyName, string message = null)
    {
        Add(propertyName, message, ValidationErrorSeverity.Info);
    }

    /// <summary>
    /// Returns true if the value has some content, else false
    /// 
    /// A simple check that they are not null and contains at least some value, for properties of type:
    /// string, int, bool, ContentReference, ContentArea, XhtmlString, Uri and Url
    /// </summary>
    public bool IsValid(object value)
    {
        if (value == null || value == "" || value + "" == "0" || value == " ")
        {
            return false;
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
        else if (value is bool b)
        {
            return b;
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
        return true;
    }

    void Add(string propertyName, string message, ValidationErrorSeverity severity)
    {
        string propertyDisplayName = GetPropertyDisplayName(propertyName);

        if (message.IsNot())
        {
            if (severity == ValidationErrorSeverity.Error)
                message = "must be set";
            else if (severity == ValidationErrorSeverity.Info)
                message = "can be set";
            else
                message = "should be set";
        }
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

        if (!message.Contains(propertyName) && !message.Contains(propertyDisplayName))
        {
            message = propertyDisplayName + ": " + message;
        }

        Validations.Add(new ValidationError
        {
            ErrorMessage = message,
            PropertyName = propertyDisplayName,
            Severity = severity,
            ValidationType = ValidationErrorType.AttributeMatched
        });
    }

    static string GetPropertyDisplayName(string propertyName)
    {
        if (propertyName.IsNot()) return propertyName;

        var property = typeof(T).GetProperty(propertyName);

        var display = property.GetCustomAttribute<DisplayAttribute>();

        if (display?.Name != null)
            return display.Name;

        return property.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? propertyName;
    }
}
