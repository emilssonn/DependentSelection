using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using EPiServer.Cms.Shell.UI.ObjectEditing;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Modules;

namespace EPiLinkedSelection
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Attribute" />
    /// <seealso cref="System.Web.Mvc.IMetadataAware" />
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class LinkedSelectOneAttribute : Attribute, IMetadataAware
    {
        private readonly Injected<IServiceLocator> _serviceLocator;
        private readonly Injected<ModuleTable> _moduleTable;
        private Type LinkedSelectionFactoryType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedSelectOneAttribute"/> class.
        /// </summary>
        /// <param name="linkedSelectionFactoryType">Type of the linked selection factory.</param>
        /// <exception cref="System.ArgumentException">linkedSelectionFactoryType needs to implement ILinkedSelectionFactory;linkedSelectionFactoryType</exception>
        public LinkedSelectOneAttribute(Type linkedSelectionFactoryType)
        {
            if (linkedSelectionFactoryType.IsAssignableFrom(typeof(ILinkedSelectionFactory)))
            {
                throw new ArgumentException("linkedSelectionFactoryType needs to implement ILinkedSelectionFactory", "linkedSelectionFactoryType");
            }
                
            LinkedSelectionFactoryType = linkedSelectionFactoryType;
        }

        /// <summary>
        /// Gets or sets the properties to depend on.
        /// </summary>
        /// <value>
        /// The properties to depend on.
        /// </value>
        public string[] DependsOn { get; set; }

        /// <summary>
        /// Gets or sets the properties that decides if this property should be read only.
        /// </summary>
        /// <value>
        /// The properties that decides if this property should be read only.
        /// </value>
        public string[] ReadOnlyWhen { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use content data.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use content data]; otherwise, <c>false</c>.
        /// </value>
        public bool UseContentData { get; set; }

        /// <summary>
        /// When implemented in a class, provides metadata to the model metadata creation process.
        /// </summary>
        /// <param name="metadata">The model metadata.</param>
        public void OnMetadataCreated(ModelMetadata metadata)
        {
            var contentDataMetadata = metadata as ContentDataMetadata;
            if (contentDataMetadata == null || contentDataMetadata.OwnerContent == null)
                return;

            contentDataMetadata.ClientEditingClass = "epi-linked-selection/LinkedSelectionEditor";
            contentDataMetadata.EditorConfiguration[Constants.UseContentData] = UseContentData;

            var iContent = contentDataMetadata.OwnerContent as IContent;
            if (iContent != null)
            {
                var contentDataStoreUrl = _moduleTable.Service.ResolvePath("cms", "stores/contentdata/{0}");
                contentDataMetadata.EditorConfiguration[Constants.ContentDataStoreUrl] = string.Format(CultureInfo.InvariantCulture, contentDataStoreUrl, iContent.ContentLink.ToString()).ToLower();
            }

            var format1 = _moduleTable.Service.ResolvePath("epilinkedselection", "stores/linkedselection/{0}/");
            var format = "/modules/app/stores/linkedselection/{0}/";
            contentDataMetadata.EditorConfiguration[Constants.StoreUrl] = string.Format(CultureInfo.InvariantCulture, format, LinkedSelectionFactoryType.FullName);

            // This property is read only when any of these properties are null or empty.
            if (ReadOnlyWhen != null)
            {
                contentDataMetadata.EditorConfiguration[Constants.ReadOnlyWhen] = ReadOnlyWhen.Select(x => char.ToLowerInvariant(x[0]) + x.Substring(1));
                contentDataMetadata.IsReadOnly = contentDataMetadata.OwnerContent.Property.Any(IsReadOnly);
            }
            else
            {
                contentDataMetadata.EditorConfiguration[Constants.ReadOnlyWhen] = new string[0];
            }

            // This property depends on the values of these properties.
            IDictionary<string, object> values = new Dictionary<string, object>();

            if (DependsOn != null)
            {
                foreach (var keyValuePair in contentDataMetadata.OwnerContent.Property.Where(p => DependsOn.Contains(p.Name)).Select(p => new KeyValuePair<string, object>(char.ToLowerInvariant(p.Name[0]) + p.Name.Substring(1), p.Value)))
                {
                    values.Add(keyValuePair);
                }
            }

            contentDataMetadata.EditorConfiguration[Constants.DependsOn] = values;

            var linkedSelectionFactory = _serviceLocator.Service.GetInstance(LinkedSelectionFactoryType) as ILinkedSelectionFactory;

            contentDataMetadata.EditorConfiguration[Constants.Selections] = UseContentData ?
                linkedSelectionFactory.GetSelectionsByContentData(contentDataMetadata.OwnerContent) :
                linkedSelectionFactory.GetSelections(values as Dictionary<string, object>);
        }

        /// <summary>
        /// Determines whether this property should be read only.
        /// This property is readonly if the passed property is one of the properties specified in <see cref="LinkedSelectOneAttribute.ReadOnlyWhen"/> and is null or white space.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        private bool IsReadOnly(PropertyData property)
        {
            var readOnly = false;

            if (!ReadOnlyWhen.Contains(property.Name))
            {
                return readOnly;
            }

            if (property.Type == PropertyDataType.String || property.Type == PropertyDataType.LongString)
            {
                readOnly = string.IsNullOrWhiteSpace(property.Value as string);
            }
            else
            {
                readOnly = property.Value == null;
            }
            return readOnly;
        }
    }
}
