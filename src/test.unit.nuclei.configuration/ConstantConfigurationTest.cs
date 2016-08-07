//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Nuclei.Configuration
{
    [TestFixture]
    public sealed class ConstantConfigurationTest
    {
        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Nuclei.Configuration.ConstantConfiguration",
            Justification = "Testing that the constructor throws.")]
        public void CreateWithNullCollection()
        {
            Assert.Throws<ArgumentNullException>(() => new ConstantConfiguration(null));
        }

        [Test]
        public void HasValueWithKnownKey()
        {
            var key = new ConfigurationKey<string>("a");

            var configuration = new ConstantConfiguration(
                new Dictionary<ConfigurationKeyBase, object>
                {
                    [key] = 10
                });
            Assert.IsTrue(configuration.HasValueFor(key));
        }

        [Test]
        public void HasValueWithNullKey()
        {
            var configuration = new ConstantConfiguration(new Dictionary<ConfigurationKeyBase, object>());
            Assert.IsFalse(configuration.HasValueFor(null));
        }

        [Test]
        public void HasValueWithUnknownKey()
        {
            var key = new ConfigurationKey<string>("a");

            var configuration = new ConstantConfiguration(new Dictionary<ConfigurationKeyBase, object>());
            Assert.IsFalse(configuration.HasValueFor(key));
        }

        [Test]
        public void ValueWithKnownKey()
        {
            var value = "a";
            var key = new ConfigurationKey<string>("b");
            var configuration = new ConstantConfiguration(
                new Dictionary<ConfigurationKeyBase, object>
                {
                    [key] = value
                });
            Assert.AreSame(value, configuration.Value(key));
        }

        [Test]
        public void ValueWithNullKey()
        {
            var configuration = new ConstantConfiguration(new Dictionary<ConfigurationKeyBase, object>());
            Assert.Throws<ArgumentNullException>(() => configuration.Value<int>(null));
        }

        [Test]
        public void ValueWithUnknownKey()
        {
            var key = new ConfigurationKey<int>("b");
            var configuration = new ConstantConfiguration(new Dictionary<ConfigurationKeyBase, object>());
            Assert.Throws<ArgumentException>(() => configuration.Value(key));
        }
    }
}
