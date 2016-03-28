using System;
using System.Globalization;
using System.Web.Mvc;
using EPiServer.Cms.Shell.UI.ObjectEditing;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Modules;

namespace EPiLinkedSelection
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class LinkedSelectOneAttribute : Attribute, IMetadataAware
    {
        private readonly Injected<IServiceLocator> _serviceLocator;
        private readonly Injected<ModuleTable> _moduleTable;
        private Type LinkedSelectionFactoryType { get; set; }

        public LinkedSelectOneAttribute(Type linkedSelectionFactoryType)
        {
            if (linkedSelectionFactoryType.IsAssignableFrom(typeof(ILinkedSelectionFactory)))
                throw new ArgumentException("linkedSelectionFactoryType needs to implement ILinkedSelectionFactory", "linkedSelectionFactoryType");
            LinkedSelectionFactoryType = linkedSelectionFactoryType;
        }

        public string[] DependsOn { get; set; }

        public string[] ReadOnlyWhen { get; set; }

        public void OnMetadataCreated(ModelMetadata metadata)
        {
            var contentDataMetadata = metadata as ContentDataMetadata;
            if (contentDataMetadata == null || contentDataMetadata.OwnerContent == null)
                return;

            contentDataMetadata.ClientEditingClass = "epi-linked-selection/LinkedSelectionEditor";
            contentDataMetadata.EditorConfiguration[Constants.ReadOnlyWhen] = ReadOnlyWhen;

            //var format = _moduleTable.Service.ResolvePath("EPiLinkedSelection", "stores/linkedselection/{0}/");
            var format = "/modules/app/stores/linkedselection/{0}/";
            contentDataMetadata.EditorConfiguration[Constants.StoreUrl] = string.Format(CultureInfo.InvariantCulture, format, LinkedSelectionFactoryType.FullName);

            contentDataMetadata.EditorConfiguration[Constants.DependsOn] = DependsOn;

            var linkedSelectionFactory = _serviceLocator.Service.GetInstance(LinkedSelectionFactoryType) as ILinkedSelectionFactory;

            contentDataMetadata.EditorConfiguration[Constants.Selections] = linkedSelectionFactory.GetSelections(contentDataMetadata.OwnerContent);
        }
    }
}
