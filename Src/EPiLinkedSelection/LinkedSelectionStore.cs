using System;
using System.Collections.Generic;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Services.Rest;

namespace EPiLinkedSelection
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="EPiServer.Shell.Services.Rest.RestControllerBase" />
    [RestStore("linkedselection")]
    public class LinkedSelectionStore : RestControllerBase
    {
        private readonly Injected<IServiceLocator> _serviceLocator;

        /// <summary>
        /// Gets a list of selection items for a specific <see cref="ILinkedSelectionFactory"/>.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        [HttpPost]
        public RestResult Post(string id, Dictionary<string, object> values)
        {
            var type = Type.GetType(id);
            var linkedSelectionFactory = _serviceLocator.Service.GetInstance(type) as ILinkedSelectionFactory;

            return Rest(linkedSelectionFactory.GetSelections(values));
        }

        /// <summary>
        /// Gets a list of selection items for a specific <see cref="ILinkedSelectionFactory"/>.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="complexReference">The complex reference.</param>
        /// <returns></returns>
        [HttpGet]
        public RestResult Get(string id, string complexReference)
        {
            var type = Type.GetType(id);
            var linkedSelectionFactory = _serviceLocator.Service.GetInstance(type) as ILinkedSelectionFactory;

            var contentLoader = _serviceLocator.Service.GetInstance<IContentLoader>();
            IContentData contentData = contentLoader.Get<IContentData>(new ContentReference(complexReference));

            return Rest(linkedSelectionFactory.GetSelectionsByContentData(contentData));
        }
    }
}
