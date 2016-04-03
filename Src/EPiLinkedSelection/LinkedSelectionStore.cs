using System.Collections.Generic;
using System.Linq;
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
    internal class LinkedSelectionStore : RestControllerBase
    {
        private readonly Injected<IServiceLocator> _serviceLocator;
        private readonly IEnumerable<ILinkedSelectionFactory> _linkedSelectionFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedSelectionStore" /> class.
        /// </summary>
        /// <param name="linkedSelectionFactories">The linked selection factories.</param>
        public LinkedSelectionStore(IEnumerable<ILinkedSelectionFactory> linkedSelectionFactories)
        {
            _linkedSelectionFactories = linkedSelectionFactories;
        }

        /// <summary>
        /// Gets a list of selection items for a specific <see cref="ILinkedSelectionFactory" />.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        [HttpPost]
        public RestResultBase Post(string id, Dictionary<string, object> values)
        {
            ILinkedSelectionFactory linkedSelectionFactory = _linkedSelectionFactories != null ? _linkedSelectionFactories.FirstOrDefault(x => string.Equals(x.GetType().FullName, id)) : null;
            if (linkedSelectionFactory == null)
            {
                return new RestStatusCodeResult(404, "No matching linked selection factory was found");
            }

            return Rest(linkedSelectionFactory.GetSelections(values));
        }

        /// <summary>
        /// Gets a list of selection items for a specific <see cref="ILinkedSelectionFactory" />.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="complexReference">The complex reference.</param>
        /// <returns></returns>
        [HttpGet]
        public RestResultBase Get(string id, string complexReference)
        {
            ILinkedSelectionFactory linkedSelectionFactory = _linkedSelectionFactories != null ? _linkedSelectionFactories.FirstOrDefault(x => string.Equals(x.GetType().FullName, id)) : null;
            if (linkedSelectionFactory == null)
            {
                return new RestStatusCodeResult(404, "No matching linked selection factory was found");
            }

            var contentLoader = _serviceLocator.Service.GetInstance<IContentLoader>();
            IContentData contentData = contentLoader.Get<IContentData>(new ContentReference(complexReference));

            return Rest(linkedSelectionFactory.GetSelectionsByContentData(contentData));
        }
    }
}
