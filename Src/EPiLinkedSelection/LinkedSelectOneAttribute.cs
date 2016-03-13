using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EPiServer.Cms.Shell.UI.ObjectEditing;
using EPiServer.Shell.ObjectEditing;

namespace EPiLinkedSelection
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class LinkedSelectOneAttribute : Attribute, IMetadataAware
    {
        public string DependsOn { get; set; }

        public string ReadOnlyWhen { get; set; }

        public string IncludeValueFrom { get; set; }

        public void OnMetadataCreated(ModelMetadata metadata)
        {
            var contentDataMetadata = metadata as ContentDataMetadata;
            if (contentDataMetadata == null || contentDataMetadata.OwnerContent == null)
                return;

            contentDataMetadata.ClientEditingClass = "epi-linked-selection/LinkedSelectionEditor";

            contentDataMetadata.EditorConfiguration[Constants.Namespace] = contentDataMetadata.ContainerType.FullName;

            contentDataMetadata.EditorConfiguration[Constants.DependsOn] = DependsOn;

            contentDataMetadata.EditorConfiguration[Constants.ReadOnlyWhen] = ReadOnlyWhen;

            contentDataMetadata.EditorConfiguration[Constants.Selections] = new List<SelectItem> { new SelectItem() { Text = "", Value = "" } };

            contentDataMetadata.EditorConfiguration[Constants.IncludeValueFrom] = IncludeValueFrom;

            var u = new Dictionary<string, object>();
            var t = contentDataMetadata.OwnerContent.Property.FirstOrDefault(p => p.Name == IncludeValueFrom);
            u.Add(t.Name, t.Value);
            contentDataMetadata.EditorConfiguration[Constants.Values] = u;
            
        }
    }
}
