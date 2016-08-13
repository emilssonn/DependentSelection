﻿using System;
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
        /// Gets the type of the linked selection factory.
        /// </summary>
        /// <value>
        /// The type of the linked selection factory.
        /// </value>
        public Type LinkedSelectionFactoryType { get; private set; }

        /// <summary>
        /// Gets or sets the properties to be linked to.
        /// </summary>
        /// <value>
        /// The properties to be linked to.
        /// </value>
        public string[] LinkedTo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this property should be read only when the list of <see cref="EPiServer.Shell.ObjectEditing.ISelectItem"/> is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if property should be read only; otherwise, <c>false</c>.
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

            contentDataMetadata.ClientEditingClass = "epi-linked-selection/LinkedSelectionEditor";
            contentDataMetadata.EditorConfiguration[Constants.ReadOnlyOnEmpty] = ReadOnlyOnEmpty;

            //var format = "/modules/app/stores/linkedselection/{0}/";// _moduleTable.Service.ResolvePath("epilinkedselection", "stores/linkedselection/{0}/");
            //contentDataMetadata.EditorConfiguration[Constants.StoreUrl] = string.Format(CultureInfo.InvariantCulture, format, LinkedSelectionFactoryType.FullName);
            var urlFormat = "/modules/app/stores/linkedselection/{0}/";//var contentDataStoreUrl = _moduleTable.Service.ResolvePath("cms", "stores/contentdata/{0}");
            contentDataMetadata.EditorConfiguration[Constants.StoreUrl] = string.Format(CultureInfo.InvariantCulture, urlFormat, LinkedSelectionFactoryType.FullName);
            
            // This property is linked to the values of these properties.
            IDictionary<string, object> values = new Dictionary<string, object>();

            if (LinkedTo != null)
            {
                foreach (var keyValuePair in contentDataMetadata.OwnerContent.Property.Where(p => LinkedTo.Contains(p.Name) && p.Name != metadata.PropertyName).Select(p => new KeyValuePair<string, object>(char.ToLowerInvariant(p.Name[0]) + p.Name.Substring(1), p.Value)))
                {
                    values.Add(keyValuePair);
                }
            }

            contentDataMetadata.EditorConfiguration[Constants.LinkedTo] = values;

            var linkedSelectionFactory = _serviceLocator.Service.GetInstance(LinkedSelectionFactoryType) as ILinkedSelectionFactory;
            var selections = linkedSelectionFactory.GetSelections(contentDataMetadata.OwnerContent);
            contentDataMetadata.EditorConfiguration[Constants.Selections] = selections;

            if (ReadOnlyOnEmpty && !selections.Any())
            {
                contentDataMetadata.IsReadOnly = true;
            }
        }
    }
}
