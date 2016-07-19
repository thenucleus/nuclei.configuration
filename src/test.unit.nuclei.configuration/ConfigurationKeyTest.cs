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
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Nuclei.Configuration
{
    [TestFixture]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class ConfigurationKeyTest : EqualityContractVerifierTest
    {
        private sealed class MessageIdEqualityContractVerifier : EqualityContractVerifier<ConfigurationKey>
        {
            private readonly ConfigurationKey _first
                = new ConfigurationKey("a", typeof(string));

            private readonly ConfigurationKey _second
                 = new ConfigurationKey("b", typeof(int));

            protected override ConfigurationKey Copy(ConfigurationKey original)
            {
                return new ConfigurationKey(original.Name, original.TranslateTo);
            }

            protected override ConfigurationKey FirstInstance
            {
                get
                {
                    return _first;
                }
            }

            protected override ConfigurationKey SecondInstance
            {
                get
                {
                    return _second;
                }
            }

            protected override bool HasOperatorOverloads
            {
                get
                {
                    return true;
                }
            }
        }

        private sealed class MessageIdHashCodeContractVerfier : HashCodeContractVerifier
        {
            private readonly IEnumerable<ConfigurationKey> _distinctInstances
                = new List<ConfigurationKey>
                     {
                        new ConfigurationKey("a", typeof(string)),
                        new ConfigurationKey("b", typeof(int)),
                        new ConfigurationKey("c", typeof(double)),
                        new ConfigurationKey("d", typeof(float)),
                        new ConfigurationKey("e", typeof(string)),
                        new ConfigurationKey("f", typeof(Version)),
                        new ConfigurationKey("g", typeof(object)),
                     };

            protected override IEnumerable<int> GetHashCodes()
            {
                return _distinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly MessageIdHashCodeContractVerfier _hashCodeVerifier = new MessageIdHashCodeContractVerfier();

        private readonly MessageIdEqualityContractVerifier _equalityVerifier = new MessageIdEqualityContractVerifier();

        protected override HashCodeContractVerifier HashContract
        {
            get
            {
                return _hashCodeVerifier;
            }
        }

        protected override IEqualityContractVerifier EqualityContract
        {
            get
            {
                return _equalityVerifier;
            }
        }

        [Test]
        public void Create()
        {
            var name = "a";
            var type = typeof(string);
            var key = new ConfigurationKey(name, type);

            Assert.AreEqual(name, key.Name);
            Assert.AreEqual(type, key.TranslateTo);
        }
    }
}
