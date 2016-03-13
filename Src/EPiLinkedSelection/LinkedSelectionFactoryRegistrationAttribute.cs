using System;
using EPiServer.ServiceLocation;

namespace EPiLinkedSelection
{
    /// <summary>
    /// Register a <see cref="ILinkedSelectionFactory"/> instance to IOC container.
    /// </summary>
    /// <seealso cref="ServicePlugInAttributeBase" />
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class LinkedSelectionFactoryRegistrationAttribute : ServicePlugInAttributeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedSelectionFactoryRegistrationAttribute"/> class.
        /// </summary>
        public LinkedSelectionFactoryRegistrationAttribute()
            : base(typeof(ILinkedSelectionFactory))
        {

        }
    }
}
