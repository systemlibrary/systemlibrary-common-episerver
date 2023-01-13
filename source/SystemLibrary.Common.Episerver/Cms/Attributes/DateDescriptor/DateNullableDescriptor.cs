using System;
using System.Collections.Generic;

using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace SystemLibrary.Common.Episerver.Cms.Attributes;

//[EditorDescriptorRegistration(TargetType = typeof(DateTime?), UIHint = "Date")]
//public class DateNullableDescriptor : EditorDescriptor
//{
//    public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
//    {
//        base.ModifyMetadata(metadata, attributes);
//        this.LayoutClass = "systemlibrary-common-episerver-date-descriptor";
//        ClientEditingClass = "dijit/form/DateTextBox";
//    }
//}