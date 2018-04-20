namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <inheritdoc/>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class Store : Entity, IStore
    {
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        #region Fields

        private int _storeId;

        #endregion

        public Store(int storeId)
        {
            _storeId = storeId;
        }

        /// <inheritdoc/>
        [DataMember]
        public int StoreId
        {
            get
            {
                return _storeId;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _storeId, _ps.Value.StoreIdSelector);
            }
        }

        /// <summary>
        /// The property selectors.
        /// </summary>
        private class PropertySelectors
        {
            public readonly PropertyInfo StoreIdSelector = ExpressionHelper.GetPropertyInfo<Store, int>(x => x.StoreId);
        }
    }
}