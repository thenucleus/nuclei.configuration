﻿//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using Consul;

namespace Nuclei.Configuration
{
    /// <summary>
    /// Defines a <see cref="IConfiguration"/> object that gets configuration values from a <see href="http://consul.io"/> key-value store.
    /// </summary>
    public sealed class ConsulConfiguration : ConfigurationBase
    {
        private static readonly IDictionary<Type, Func<byte[], object>> _typeToConverterMap
            = new Dictionary<Type, Func<byte[], object>>
            {
                [typeof(bool)] = b => BitConverter.ToBoolean(b, 0),
                [typeof(string)] = b => BitConverter.ToString(b, 0),
                [typeof(char)] = b => BitConverter.ToString(b, 0),
                [typeof(float)] = b => BitConverter.ToSingle(b, 0),
                [typeof(double)] = b => BitConverter.ToString(b, 0),
                [typeof(short)] = b => BitConverter.ToInt16(b, 0),
                [typeof(int)] = b => BitConverter.ToInt32(b, 0),
                [typeof(long)] = b => BitConverter.ToInt64(b, 0),
                [typeof(ushort)] = b => BitConverter.ToUInt16(b, 0),
                [typeof(uint)] = b => BitConverter.ToUInt32(b, 0),
                [typeof(ulong)] = b => BitConverter.ToUInt64(b, 0),
            };

        private readonly Dictionary<ConfigurationKeyBase, object> _values
            = new Dictionary<ConfigurationKeyBase, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsulConfiguration"/> class.
        /// </summary>
        /// <param name="knownKeys">The collection containing all the known configuration keys for the application.</param>
        /// <param name="configuration">
        /// The configuration object that contains the configuration values that allow connection to the <a href="http://consul.io">consul</a>
        /// agent.
        /// </param>
        /// <remarks>
        /// <para>
        ///     If the <paramref name="configuration"/> does not have any consul configuration information then the
        ///     <a href="http://consul.io">consul</a> agent on the localhost at the standard port will be contacted.
        /// </para>
        /// <list type = "table" >
        ///     <listheader>
        ///         <term>Key</term>
        ///         <term>Value</term>
        ///     </listheader>
        ///     <item>
        ///         <term>ConsulAddress</term>
        ///         <term>The URL of the consul agent.</term>
        ///     </item>
        ///     <item>
        ///         <term>ConsulConfigurationPrefix</term>
        ///         <term>The prefix for all the configuration options which should be retrieved, e.g. abc to retrieve all configuration options starting with abc.</term>
        ///     </item>
        ///     <item>
        ///         <term>ConsulDatacenter</term>
        ///         <term>The name of the consul datacenter for which the configuration values should be returned.</term>
        ///     </item>
        ///     <item>
        ///         <term>ConsulToken</term>
        ///         <term>The security token that should be used to connect to consul.</term>
        ///     </item>
        ///     <item>
        ///         <term>ConsulWaitTimeInMilliSeconds</term>
        ///         <term>The time-out in milliseconds.</term>
        ///     </item>
        /// </list>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="knownKeys"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="configuration"/> is <see langword="null" />.
        /// </exception>
        public ConsulConfiguration(IEnumerable<ConfigurationKeyBase> knownKeys, IConfiguration configuration)
        {
            if (knownKeys == null)
            {
                throw new ArgumentNullException("knownKeys");
            }

            if (configuration == null)
            {
                throw new ArgumentNullException("configurations");
            }

            var consulConfig = new ConsulClientConfiguration();
            if (configuration.HasValueFor(ConfigurationKeys.ConsulAddress))
            {
                consulConfig.Address = new Uri(configuration.Value(ConfigurationKeys.ConsulAddress));
            }

            if (configuration.HasValueFor(ConfigurationKeys.ConsulDatacenter))
            {
                consulConfig.Datacenter = configuration.Value(ConfigurationKeys.ConsulDatacenter);
            }

            if (configuration.HasValueFor(ConfigurationKeys.ConsulToken))
            {
                consulConfig.Token = configuration.Value(ConfigurationKeys.ConsulToken);
            }

            if (configuration.HasValueFor(ConfigurationKeys.ConsulWaitTimeInMilliSeconds))
            {
                consulConfig.WaitTime = TimeSpan.FromMilliseconds(configuration.Value(ConfigurationKeys.ConsulWaitTimeInMilliSeconds));
            }

            var configPrefix = configuration.HasValueFor(ConfigurationKeys.ConsulConfigurationPrefix)
                ? configuration.Value(ConfigurationKeys.ConsulConfigurationPrefix)
                : string.Empty;

            using (var client = new ConsulClient(consulConfig))
            {
                var task = client.KV.List(configPrefix);
                task.ContinueWith(
                    t =>
                    {
                        if (t.IsFaulted)
                        {
                            return;
                        }

                        if (t.Result.StatusCode != HttpStatusCode.OK)
                        {
                            return;
                        }

                        foreach (var pair in t.Result.Response)
                        {
                            var key = knownKeys.FirstOrDefault(k => string.Equals(k.Name, pair.Key, StringComparison.Ordinal));
                            if (key == null)
                            {
                                continue;
                            }

                            if (!_values.ContainsKey(key))
                            {
                                if (_typeToConverterMap.ContainsKey(key.TranslateTo))
                                {
                                    object value = _typeToConverterMap[key.TranslateTo](pair.Value);
                                    _values.Add(key, value);
                                }
                            }
                        }
                    });
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
