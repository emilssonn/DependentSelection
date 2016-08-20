using System;

namespace EPiLinkedSelection
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="EPiLinkedSelection.LinkedSelectAttribute" />
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class LinkedSelectManyAttribute : LinkedSelectAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedSelectManyAttribute"/> class.
        /// </summary>
        /// <param name="linkedSelectionFactoryType">Type of the linked selection factory.</param>
        public LinkedSelectManyAttribute(Type linkedSelectionFactoryType) 
            : base(linkedSelectionFactoryType, "epi-linked-selection/LinkedCheckBoxListEditor")
        {
        }
    }
}
