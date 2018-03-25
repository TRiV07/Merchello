namespace Merchello.Web.Models.VirtualContent
{
    using Merchello.Web.Models.ContentEditing;
    using System;

    using Umbraco.Core.Models;

    /// <summary>
    /// The virtual content event args.
    /// </summary>
    public class VirtualContentEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualContentEventArgs"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public VirtualContentEventArgs(IPublishedContent parent)
        {
            Parent = parent;
        }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        public IPublishedContent Parent { get; set; }

        /// <summary>
        /// Gets or sets function to get parrent by content
        /// </summary>
        public Func<ProductDisplay, IPublishedContent> ParentGetter { get; set; }
    }
}