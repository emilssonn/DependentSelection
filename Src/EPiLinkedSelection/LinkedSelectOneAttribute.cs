using System;
using System.Web.Mvc;
using EPiServer.Shell.ObjectEditing;

namespace EPiLinkedSelection
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class LinkedSelectOneAttribute : Attribute, IMetadataAware
    {
        public void OnMetadataCreated(ModelMetadata metadata)
        {
            ExtendedMetadata extendedMetadata = metadata as ExtendedMetadata;
            if (extendedMetadata == null)
                return;

            extendedMetadata.ClientEditingClass = "epi-linked-selection/LinkedSelectionEditor";
        }
    }
}
