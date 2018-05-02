namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.Serialization;

    /// <summary>
    /// The Carrier.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class Carrier : CustomerBase, ICarrier
    {
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        #region fields

        /// <summary>
        /// The first name.
        /// </summary>
        private string _firstName;

        /// <summary>
        /// The last name.
        /// </summary>
        private string _lastName;

        /// <summary>
        /// The email.
        /// </summary>
        private string _email;

        /// <summary>
        /// The login name.
        /// </summary>
        private string _loginName;

        private bool _isDisabled;

        private bool _isOnDuty;

        private int _activeOrders;

        /// <summary>
        /// The examine id.
        /// </summary>
        private int _examineId = 1;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Carrier"/> class.
        /// </summary>
        /// <param name="loginName">
        /// The login Name associated with the membership provider users
        /// </param>
        internal Carrier(string loginName, int storeId) : base(false, storeId)
        {
            Ensure.ParameterNotNullOrEmpty(loginName, "loginName");

            _loginName = loginName;
        }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        [IgnoreDataMember]
        public string FullName
        {
            get { return string.Format("{0} {1}", _firstName, _lastName).Trim(); }
        }

        /// <inheritdoc/>
        [DataMember]
        public string FirstName
        {
            get
            {
                return _firstName;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _firstName, _ps.Value.FirstNameSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string LastName
        {
            get
            {
                return _lastName;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _lastName, _ps.Value.LastNameSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string Email
        {
            get
            {
                return _email;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _email, _ps.Value.EmailSelector);                    
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string LoginName
        {
            get
            {
                return _loginName;
            }

            internal set
            {
                SetPropertyValueAndDetectChanges(value, ref _loginName, _ps.Value.LoginNameSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool IsDisabled
        {
            get
            {
                return _isDisabled;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _isDisabled, _ps.Value.IsDisabledSelector);
            }
        }

        [DataMember]
        public bool IsOnDuty
        {
            get
            {
                return _isOnDuty;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _isOnDuty, _ps.Value.IsOnDutySelector);
            }
        }


        [DataMember]
        public int ActiveOrders
        {
            get
            {
                return _activeOrders;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _activeOrders, _ps.Value.ActiveOrdersSelector);
            }
        }

        /// <summary>
        /// Gets or sets the examine id.
        /// </summary>
        [IgnoreDataMember]
        internal int ExamineId
        {
            get { return _examineId; }
            set { _examineId = value; }
        }

        /// <summary>
        /// Property selectors.
        /// </summary>
        private class PropertySelectors
        {
            /// <summary>
            /// The login name selector.
            /// </summary>
            public readonly PropertyInfo LoginNameSelector = ExpressionHelper.GetPropertyInfo<Carrier, string>(x => x.LoginName);

            /// <summary>
            /// The first name selector.
            /// </summary>
            public readonly PropertyInfo FirstNameSelector = ExpressionHelper.GetPropertyInfo<Carrier, string>(x => x.FirstName);

            /// <summary>
            /// The last name selector.
            /// </summary>
            public readonly PropertyInfo LastNameSelector = ExpressionHelper.GetPropertyInfo<Carrier, string>(x => x.LastName);

            /// <summary>
            /// The email selector.
            /// </summary>
            public readonly PropertyInfo EmailSelector = ExpressionHelper.GetPropertyInfo<Carrier, string>(x => x.Email);

            public readonly PropertyInfo IsDisabledSelector = ExpressionHelper.GetPropertyInfo<Carrier, bool>(x => x.IsDisabled);

            public readonly PropertyInfo IsOnDutySelector = ExpressionHelper.GetPropertyInfo<Carrier, bool>(x => x.IsOnDuty);

            public readonly PropertyInfo ActiveOrdersSelector = ExpressionHelper.GetPropertyInfo<Carrier, int>(x => x.ActiveOrders);

        }
    }
}