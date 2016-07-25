//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Nuclei.Configuration.Properties;

namespace Nuclei.Configuration
{
    /// <summary>
    /// Defines an <see cref="IConfiguration"/> that gets configuration values from one or more <see cref="IConfiguration"/> objects.
    /// </summary>
    public sealed class HierarchicalConfiguration : IConfiguration
    {
        private readonly IConfiguration[] _configurations;

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchicalConfiguration"/> class.
        /// </summary>
        /// <param name="configurations">
        /// The collection containing the configuration objects that should be searched when the user
        /// requests a configuration value.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="configurations"/> is <see langword="null" />.
        /// </exception>
        public HierarchicalConfiguration(IEnumerable<IConfiguration> configurations)
        {
            if (configurations == null)
            {
                throw new ArgumentNullException("configurations");
            }

            _configurations = configurations.ToArray();
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
        public bool HasValueFor(ConfigurationKeyBase key)
        {
            if (key == null)
            {
                return false;
            }

            foreach (var configuration in _configurations)
            {
                if (configuration.HasValueFor(key))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the value for the given configuration key.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="key">The configuration key.</param>
        /// <returns>
        /// The desired value.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="key"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="key"/> does not match any of the registered configuration keys.
        /// </exception>
        public T Value<T>(ConfigurationKeyBase key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            foreach (var configuration in _configurations)
            {
                if (configuration.HasValueFor(key))
                {
                    return configuration.Value<T>(key);
                }
            }

            throw new ArgumentException(
                Resources.Exceptions_Messages_UnknownConfigurationKey,
                "key");
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
