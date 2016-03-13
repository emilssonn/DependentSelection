using System;
using System.Collections.Generic;
using System.Web.Mvc;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Services.Rest;

namespace EPiLinkedSelection
{
    [RestStore("linkedselection")]
    public class LinkedSelectionStore : RestControllerBase
    {
        private readonly Injected<IServiceLocator> _serviceLocator;

        [HttpGet]
        public RestResult Get(string id, Dictionary<string, object> values)
        {
            var type = Type.GetType(id);
            var linkedSelectionFactory = _serviceLocator.Service.GetInstance(type) as ILinkedSelectionFactory;

            return Rest(linkedSelectionFactory.GetSelections(values));
        }
    }
}
