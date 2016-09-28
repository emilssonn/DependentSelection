using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using EPiServer.Cms.Shell.UI.ObjectEditing;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Modules;

namespace DependentSelection
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Attribute" />
    /// <seealso cref="System.Web.Mvc.IMetadataAware" />
    public abstract class DependentSelectAttribute : Attribute, IMetadataAware
    {
        private readonly Injected<IServiceLocator> _serviceLocator;
        private readonly Injected<ModuleTable> _moduleTable;
        private readonly string _clientEditingClass;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependentSelectAttribute"/> class.
        /// </summary>
        /// <param name="dependentSelectionFactoryType">Type of the dependent selection factory.</param>
        /// <param name="clientEditingClass">The client editing class.</param>
        /// <exception cref="System.ArgumentException">
        /// dependentSelectionFactoryType needs to implement IDependentSelectionFactory;dependentSelectionFactoryType
        /// or
        /// clientEditingClass cannot be null or white space;clientEditingClass
        /// </exception>
        public DependentSelectAttribute(Type dependentSelectionFactoryType, string clientEditingClass)
        {
            if (dependentSelectionFactoryType.IsAssignableFrom(typeof(IDependentSelectionFactory)))
            {
                throw new ArgumentException("dependentSelectionFactoryType needs to implement IDependentSelectionFactory", "dependentSelectionFactoryType");
            }

            if (string.IsNullOrWhiteSpace(clientEditingClass))
            {
                throw new ArgumentException("clientEditingClass cannot be null or white space", "clientEditingClass");
            }

            DependentSelectionFactoryType = dependentSelectionFactoryType;
            _clientEditingClass = clientEditingClass;
        }

        /// <summary>
        /// Gets the type of the dependent selection factory.
        /// </summary>
        /// <value>
        /// The type of the dependent selection factory.
        /// </value>
        public Type DependentSelectionFactoryType { get; private set; }

        /// <summary>
        /// Gets or sets the properties this property is dependent on.
        /// </summary>
        /// <value>
        /// The properties this property is dependent on.
        /// </value>
        public string[] DependentOn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this property should be read-only when the list of <see cref="EPiServer.Shell.ObjectEditing.ISelectItem"/> is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if property should be read-only when the list of <see cref="EPiServer.Shell.ObjectEditing.ISelectItem"/> is empty; otherwise, <c>false</c>.
        /// </value>
        public bool ReadOnlyOnEmpty { get; set; }

        /// <summary>
        /// When implemented in a class, provides metadata to the model metadata creation process.
        /// </summary>
        /// <param name="metadata">The model metadata.</param>
        public void OnMetadataCreated(ModelMetadata metadata)
        {
            var contentDataMetadata = metadata as ContentDataMetadata;
            if (contentDataMetadata == null || contentDataMetadata.OwnerContent == null)
            {
                return;
            }

            var iContent = contentDataMetadata.OwnerContent as IContent;
            if (iContent == null)
            {
                return;
            }

            contentDataMetadata.ClientEditingClass = _clientEditingClass;
            contentDataMetadata.EditorConfiguration[Constants.ReadOnlyOnEmpty] = ReadOnlyOnEmpty;

            var urlFormat = _moduleTable.Service.ResolvePath("DependentSelection", "stores/dependentselection/{0}/");
            contentDataMetadata.EditorConfiguration[Constants.StoreUrl] = string.Format(CultureInfo.InvariantCulture, urlFormat, DependentSelectionFactoryType.FullName);

            // This property is dependent on the values of these properties.
            IDictionary<string, object> values = new Dictionary<string, object>();

            if (DependentOn != null)
            {
                foreach (var keyValuePair in contentDataMetadata.OwnerContent.Property.Where(p => DependentOn.Contains(p.Name) && p.Name != metadata.PropertyName).Select(p => new KeyValuePair<string, object>(char.ToLowerInvariant(p.Name[0]) + p.Name.Substring(1), p.Value)))
                {
                    values.Add(keyValuePair);
                }
            }

            contentDataMetadata.EditorConfiguration[Constants.DependentOn] = values;

            var dependentSelectionFactory = _serviceLocator.Service.GetInstance(DependentSelectionFactoryType) as IDependentSelectionFactory;
            var selections = dependentSelectionFactory.GetSelections(contentDataMetadata.OwnerContent);
            contentDataMetadata.EditorConfiguration[Constants.Selections] = selections;

            if (ReadOnlyOnEmpty && !selections.Any())
            {
                contentDataMetadata.IsReadOnly = true;
            }
        }
    }
}
