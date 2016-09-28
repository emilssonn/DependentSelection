using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.Shell.ObjectEditing;

namespace DependentSelection
{
    /// <summary>
    /// Creates a list of <see cref="ISelectItem" /> for a specific property.
    /// </summary>
    public interface IDependentSelectionFactory
    {
        /// <summary>
        /// Creates a list of <see cref="ISelectItem" /> for a specific property.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>
        /// A list of <see cref="ISelectItem" /> for a specific property.
        /// </returns>
        IEnumerable<ISelectItem> GetSelections(IContentData contentData);
    }
}
