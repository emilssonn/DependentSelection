using System;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Services.Rest;

namespace EPiLinkedSelection
{
    [RestStore("linkedselection")]
    public class LinkedSelectionStore : RestControllerBase
    {
        private readonly Injected<IServiceLocator> _serviceLocator;
        private readonly Injected<IContentLoader> _contentLoader;

        [HttpGet]
        public RestResult Get(string id, int contentID, int versionID)
        {
            var type = Type.GetType(id);
            var linkedSelectionFactory = _serviceLocator.Service.GetInstance(type) as ILinkedSelectionFactory;

            var contentData = _contentLoader.Service.Get<IContentData>(new ContentReference(contentID, versionID));

            return Rest(linkedSelectionFactory.GetSelections(contentData));
        }
    }
}
