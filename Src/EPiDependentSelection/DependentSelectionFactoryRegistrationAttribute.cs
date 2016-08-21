using System;
using EPiServer.ServiceLocation;

namespace EPiDependentSelection
{
    /// <summary>
    /// Register a <see cref="IDependentSelectionFactory" /> instance to the IOC container.
    /// </summary>
    /// <seealso cref="EPiServer.ServiceLocation.ServicePlugInAttributeBase" />
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class DependentSelectionFactoryRegistrationAttribute : ServicePlugInAttributeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependentSelectionFactoryRegistrationAttribute" /> class.
        /// </summary>
        public DependentSelectionFactoryRegistrationAttribute()
            : base(typeof(IDependentSelectionFactory))
        {

        }
    }
}
