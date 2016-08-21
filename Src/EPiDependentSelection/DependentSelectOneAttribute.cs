using System;

namespace EPiDependentSelection
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="EPiDependentSelection.DependentSelectAttribute" />
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DependentSelectOneAttribute : DependentSelectAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependentSelectOneAttribute" /> class.
        /// </summary>
        /// <param name="dependentSelectionFactoryType">Type of the dependent selection factory.</param>
        public DependentSelectOneAttribute(Type dependentSelectionFactoryType) 
            : base(dependentSelectionFactoryType, "epi-dependent-selection/DependentSelectionEditor")
        {
        }
    }
}
