//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Configuration
{
    /// <summary>
    /// Defines the interface for objects that serve as keys for the <see cref="IConfiguration"/> collection.
    /// </summary>
    /// <typeparam name="T">The type of the object to which the data in the current configuration section should be translated.</typeparam>
    public sealed class ConfigurationKey<T> : ConfigurationKeyBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationKey{T}"/> class.
        /// </summary>
        /// <param name="name">The name of the configuration section that the current key links to.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="name"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="name"/> is an empty string.
        /// </exception>
        public ConfigurationKey(string name)
            : base(name, typeof(T))
        {
        }
    }
}
