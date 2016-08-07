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
        private sealed class MessageIdEqualityContractVerifier : EqualityContractVerifier<ConfigurationKeyBase>
        {
            private readonly ConfigurationKeyBase _first
                = new ConfigurationKey<string>("a");

            private readonly ConfigurationKeyBase _second
                 = new ConfigurationKey<int>("b");

            protected override ConfigurationKeyBase Copy(ConfigurationKeyBase original)
            {
                if (original == _first)
                {
                    return new ConfigurationKey<string>(original.Name);
                }
                else
                {
                    return new ConfigurationKey<int>(original.Name);
                }
            }

            protected override ConfigurationKeyBase FirstInstance
            {
                get
                {
                    return _first;
                }
            }

            protected override ConfigurationKeyBase SecondInstance
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
            private readonly IEnumerable<ConfigurationKeyBase> _distinctInstances
                = new List<ConfigurationKeyBase>
                     {
                        new ConfigurationKey<string>("a"),
                        new ConfigurationKey<int>("b"),
                        new ConfigurationKey<double>("c"),
                        new ConfigurationKey<float>("d"),
                        new ConfigurationKey<string>("e"),
                        new ConfigurationKey<Version>("f"),
                        new ConfigurationKey<object>("g"),
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
            var key = new ConfigurationKey<string>(name);

            Assert.AreEqual(name, key.Name);
            Assert.AreEqual(typeof(string), key.TranslateTo);
        }
    }
}
