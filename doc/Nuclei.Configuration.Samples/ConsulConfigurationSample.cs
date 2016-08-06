//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Nuclei.Configuration.Samples
{
    [TestFixture]
    public sealed class ConsulConfigurationSample
    {
        [Test]
        public void HasValue()
        {
            var key = new ConfigurationKey<int>("Key1");

            var connectionConfiguration = new ConstantConfiguration(
                new Dictionary<ConfigurationKeyBase, object>
                    {
                        [ConfigurationKeys.ConsulAddress] = "http://localhost:8500",
                        [ConfigurationKeys.ConsulDatacenter] = "dc1",
                        [ConfigurationKeys.ConsulConfigurationPrefix] = "all-my-keys-start-with-this-value",
                    });

            var configuration = new ConsulConfiguration(
                new[] { key },
                connectionConfiguration);
            var hasValue = configuration.HasValueFor(key);

            Assert.IsFalse(hasValue);
        }

        [Test]
        public void ValueFor()
        {
            try
            {
                var key = new ConfigurationKey<int>("Key1");

                var connectionConfiguration = new ConstantConfiguration(
                new Dictionary<ConfigurationKeyBase, object>
                    {
                        [ConfigurationKeys.ConsulAddress] = "http://localhost:8500",
                        [ConfigurationKeys.ConsulDatacenter] = "dc1",
                        [ConfigurationKeys.ConsulConfigurationPrefix] = "all-my-keys-start-with-this-value",
                    });

                var configuration = new ConsulConfiguration(
                    new[] { key },
                    connectionConfiguration);

                var value = configuration.Value(key);
                Assert.AreEqual(10, value);
            }
            catch (ArgumentException e)
            {
                Assert.IsInstanceOf<ArgumentException>(e);
            }
        }
    }
}
