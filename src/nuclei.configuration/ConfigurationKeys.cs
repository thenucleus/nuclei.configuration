//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Nuclei.Configuration
{
    /// <summary>
    /// Defines the <see cref="ConfigurationKey{T}"/> instances used for the configuration of the configuration system.
    /// </summary>
    public static class ConfigurationKeys
    {
        /// <summary>
        /// The configuration key that is used to retrieve the address of the local <a href="http://consul.io">consul</a> agent.
        /// </summary>
        [SuppressMessage(
            "Microsoft.Security",
            "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "ConfigurationKey objects are immutable.")]
        public static readonly ConfigurationKey<string> ConsulAddress
            = new ConfigurationKey<string>("ConsulAddress");

        /// <summary>
        /// The configuration key that is used to retrieve the prefix that is used to store configuration key-value pairs in
        /// <a href="http://consul.io">consul</a>.
        /// </summary>
        [SuppressMessage(
            "Microsoft.Security",
            "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "ConfigurationKey objects are immutable.")]
        public static readonly ConfigurationKey<string> ConsulConfigurationPrefix
            = new ConfigurationKey<string>("ConsulConfigurationPrefix");

        /// <summary>
        /// The configuration key that is used to retrieve the datacenter of the local <a href="http://consul.io">consul</a> agent.
        /// </summary>
        [SuppressMessage(
            "Microsoft.Security",
            "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "ConfigurationKey objects are immutable.")]
        public static readonly ConfigurationKey<string> ConsulDatacenter
            = new ConfigurationKey<string>("ConsulDatacenter");

        /// <summary>
        /// The configuration key that is used to retrieve the toke that should be used to connect to
        /// the local <a href="http://consul.io">consul</a> agent.
        /// </summary>
        [SuppressMessage(
            "Microsoft.Security",
            "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "ConfigurationKey objects are immutable.")]
        public static readonly ConfigurationKey<string> ConsulToken
            = new ConfigurationKey<string>("ConsulToken");

        /// <summary>
        /// The configuration key that is used to retrieve the time out time that should be used when connecting to
        /// the local <a href="http://consul.io">consul</a> agent.
        /// </summary>
        [SuppressMessage(
            "Microsoft.Security",
            "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "ConfigurationKey objects are immutable.")]
        public static readonly ConfigurationKey<long> ConsulWaitTimeInMilliseconds
            = new ConfigurationKey<long>("ConsulWaitTimeInMilliseconds");

        /// <summary>
        /// Returns a collection containing all the configuration keys for the application.
        /// </summary>
        /// <returns>A collection containing all the configuration keys for the application.</returns>
        public static IEnumerable<ConfigurationKeyBase> ToCollection()
        {
            return new List<ConfigurationKeyBase>
                {
                    ConsulAddress,
                    ConsulConfigurationPrefix,
                    ConsulDatacenter,
                    ConsulToken,
                    ConsulWaitTimeInMilliseconds,
                };
        }
    }
}
