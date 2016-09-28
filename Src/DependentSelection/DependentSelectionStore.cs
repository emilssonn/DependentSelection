using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Services.Rest;

namespace DependentSelection
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="EPiServer.Shell.Services.Rest.RestControllerBase" />
    [RestStore("dependentselection")]
    internal class DependentSelectionStore : RestControllerBase
    {
        private readonly Injected<IContentLoader> _contentLoader;
        private readonly IEnumerable<IDependentSelectionFactory> _dependentSelectionFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependentSelectionStore" /> class.
        /// </summary>
        /// <param name="dependentSelectionFactories">The dependent selection factories.</param>
        public DependentSelectionStore(IEnumerable<IDependentSelectionFactory> dependentSelectionFactories)
        {
            _dependentSelectionFactories = dependentSelectionFactories;
        }

        /// <summary>
        /// Gets a list of selection items for a specific <see cref="IDependentSelectionFactory" />.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="complexReference">The complex reference.</param>
        /// <returns></returns>
        [HttpGet]
        public RestResultBase Get(string id, string complexReference)
        {
            IDependentSelectionFactory dependentSelectionFactory = _dependentSelectionFactories != null ? _dependentSelectionFactories.FirstOrDefault(x => string.Equals(x.GetType().FullName, id)) : null;
            if (dependentSelectionFactory == null)
            {
                return new RestStatusCodeResult(404, "No matching dependent selection factory was found");
            }

            IContentData contentData = _contentLoader.Service.Get<IContentData>(new ContentReference(complexReference));

            return Rest(dependentSelectionFactory.GetSelections(contentData));
        }
    }
}
