using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

        public string DependsOn { get; set; }

        public string ReadOnlyWhen { get; set; }

        public string IncludeValueFrom { get; set; }

        public Type LinkedSelectionFactoryType { get; set; }

        public void OnMetadataCreated(ModelMetadata metadata)
        {
            var contentDataMetadata = metadata as ContentDataMetadata;
            if (contentDataMetadata == null || contentDataMetadata.OwnerContent == null)
                return;

            contentDataMetadata.ClientEditingClass = "epi-linked-selection/LinkedSelectionEditor";

            contentDataMetadata.EditorConfiguration[Constants.Namespace] = contentDataMetadata.ContainerType.FullName;

            contentDataMetadata.EditorConfiguration[Constants.DependsOn] = DependsOn;

            contentDataMetadata.EditorConfiguration[Constants.ReadOnlyWhen] = ReadOnlyWhen;

            contentDataMetadata.EditorConfiguration[Constants.IncludeValueFrom] = IncludeValueFrom;

            //var format = _moduleTable.Service.ResolvePath("app", "stores/linkedselection/{0}/");
            var format = "/modules/app/stores/linkedselection/{0}/";
            contentDataMetadata.EditorConfiguration[Constants.StoreUrl] = string.Format(CultureInfo.InvariantCulture, format, LinkedSelectionFactoryType.FullName);

            IDictionary<string, object> values = new Dictionary<string, object>();
            foreach(var keyValuePair in contentDataMetadata.OwnerContent.Property.Where(p => p.Name == IncludeValueFrom || p.Name == contentDataMetadata.PropertyName).Select(p => new KeyValuePair<string, object>(p.Name, p.Value)))
            {
                values.Add(keyValuePair);
            }
            contentDataMetadata.EditorConfiguration[Constants.Values] = values;

            var linkedSelectionFactory = _serviceLocator.Service.GetInstance(LinkedSelectionFactoryType) as ILinkedSelectionFactory;

            contentDataMetadata.EditorConfiguration[Constants.Selections] = linkedSelectionFactory.GetSelections(values as Dictionary<string, object>);
        }
    }
}
