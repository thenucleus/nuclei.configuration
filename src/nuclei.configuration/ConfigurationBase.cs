//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Nuclei.Configuration.Properties;

namespace Nuclei.Configuration
{
    /// <summary>
    /// Defines the base implementation for a <see cref="IConfiguration"/> instance.
    /// </summary>
    public abstract class ConfigurationBase : IConfiguration
    {
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
        public bool HasValueFor(ConfigurationKeyBase key)
        {
            return (key != null) && KeyToValueMap().ContainsKey(key);
        }

        /// <summary>
        /// Returns a collection containing a mapping of all the known keys to the connected values.
        /// </summary>
        /// <returns>The collection containing the mapping of all the known keys to the connected values.</returns>
        protected abstract IReadOnlyDictionary<ConfigurationKeyBase, object> KeyToValueMap();

        /// <summary>
        /// Returns the value for the given configuration key.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="key">The configuration key.</param>
        /// <returns>
        /// The desired value.
        /// </returns>
        public T Value<T>(ConfigurationKeyBase key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            var values = KeyToValueMap();
            if (!values.ContainsKey(key))
            {
                throw new ArgumentException(
                    Resources.Exceptions_Messages_UnknownConfigurationKey,
                    "key");
            }

            var obj = values[key];
            return (T)obj;
        }

        /// <summary>
        /// Returns the value for the given configuration key.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="key">The configuration key.</param>
        /// <returns>
        /// The desired value.
        /// </returns>
        public T Value<T>(ConfigurationKey<T> key)
        {
            return Value<T>((ConfigurationKeyBase)key);
        }
    }
}
