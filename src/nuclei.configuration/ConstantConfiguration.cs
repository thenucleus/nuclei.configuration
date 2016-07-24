//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Nuclei.Configuration
{
    /// <summary>
    /// Defines a <see cref="IConfiguration"/> object stores a set of pre-configured constant values.
    /// </summary>
    public sealed class ConstantConfiguration : IConfiguration
    {
        private readonly Dictionary<ConfigurationKey, object> _values
            = new Dictionary<ConfigurationKey, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantConfiguration"/> class.
        /// </summary>
        /// <param name="values">The collection of configuration values.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="values"/> is <see langword="null" />.
        /// </exception>
        public ConstantConfiguration(IDictionary<ConfigurationKey, object> values)
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
        /// Returns a value indicating if there is a value for the given key or not.
        /// </summary>
        /// <param name="key">The configuration key.</param>
        /// <returns>
        /// <see langword="true" /> if there is a value for the given key; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool HasValueFor(ConfigurationKey key)
        {
            return (key != null) && _values.ContainsKey(key);
        }

        /// <summary>
        /// Returns the value for the given configuration key.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="key">The configuration key.</param>
        /// <returns>
        /// The desired value.
        /// </returns>
        public T Value<T>(ConfigurationKey key)
        {
            if (HasValueFor(key))
            {
                var obj = _values[key];
                return (T)obj;
            }

            return default(T);
        }
    }
}
