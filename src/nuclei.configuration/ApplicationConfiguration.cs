//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Nuclei.Configuration.Properties;

namespace Nuclei.Configuration
{
    /// <summary>
    /// Defines a <see cref="IConfiguration"/> object that gets its value from the application configuration file.
    /// </summary>
    public sealed class ApplicationConfiguration : ConfigurationBase
    {
        private readonly Dictionary<ConfigurationKeyBase, object> _values
            = new Dictionary<ConfigurationKeyBase, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationConfiguration"/> class.
        /// </summary>
        /// <param name="knownKeys">The collection containing all the known configuration keys for the application.</param>
        /// <param name="sectionPath">The path of the section in the configuration file.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="knownKeys"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="sectionPath"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="sectionPath"/> is an empty string.
        /// </exception>
        public ApplicationConfiguration(IEnumerable<ConfigurationKeyBase> knownKeys, string sectionPath)
        {
            if (knownKeys == null)
            {
                throw new ArgumentNullException("knownKeys");
            }

            if (sectionPath == null)
            {
                throw new ArgumentNullException("sectionPath");
            }

            if (string.IsNullOrWhiteSpace(sectionPath))
            {
                throw new ArgumentException(
                    Resources.Exceptions_Messages_ParameterShouldNotBeAnEmptyString,
                    "sectionPath");
            }

            var sections = ConfigurationManager.GetSection(sectionPath) as IEnumerable<XmlNode>;
            foreach (var section in sections)
            {
                var key = knownKeys.FirstOrDefault(k => string.Equals(k.Name, section.Name, StringComparison.Ordinal));
                if (key == null)
                {
                    continue;
                }

                try
                {
                    var node = section.FirstChild;
                    while (!(node is XmlElement) && (node != null))
                    {
                        node = node.NextSibling;
                    }

                    if (node != null)
                    {
                        var serializer = new XmlSerializer(key.TranslateTo);
                        var reader = new XmlNodeReader(node);
                        object data = serializer.Deserialize(reader);

                        if (data != null)
                        {
                            _values.Add(key, data);
                        }
                    }
                }
                catch (InvalidOperationException)
                {
                    // Ignore it. We just won't get this section
                }
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
