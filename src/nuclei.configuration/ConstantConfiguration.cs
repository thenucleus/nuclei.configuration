//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Nuclei.Configuration
{
    /// <summary>
    /// Defines a <see cref="IConfiguration"/> object stores a set of pre-configured constant values.
    /// </summary>
    public sealed class ConstantConfiguration : ConfigurationBase
    {
        private readonly Dictionary<ConfigurationKeyBase, object> _values
            = new Dictionary<ConfigurationKeyBase, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantConfiguration"/> class.
        /// </summary>
        /// <param name="values">The collection of configuration values.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="values"/> is <see langword="null" />.
        /// </exception>
        public ConstantConfiguration(IDictionary<ConfigurationKeyBase, object> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            foreach (var pair in values)
            {
                _values.Add(pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// Returns a collection containing a mapping of all the known keys to the connected values.
        /// </summary>
        /// <returns>The collection containing the mapping of all the known keys to the connected values.</returns>
        protected override IReadOnlyDictionary<ConfigurationKeyBase, object> KeyToValueMap()
        {
            return new ReadOnlyDictionary<ConfigurationKeyBase, object>(_values);
        }
    }
}
