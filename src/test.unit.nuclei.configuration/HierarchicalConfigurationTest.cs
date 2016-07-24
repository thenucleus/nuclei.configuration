﻿//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Moq;
using NUnit.Framework;

namespace Nuclei.Configuration
{
    [TestFixture]
    public sealed class HierarchicalConfigurationTest
    {
        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Nuclei.Configuration.HierarchicalConfiguration",
            Justification = "Testing that the constructor throws.")]
        public void CreateWithNullCollection()
        {
            Assert.Throws<ArgumentNullException>(() => new HierarchicalConfiguration(null));
        }

        [Test]
        public void HasValueWithKnownKey()
        {
            var key = new ConfigurationKey("a", typeof(string));
            var sub = new Mock<IConfiguration>();
            {
                sub.Setup(s => s.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(true)
                    .Verifiable();
            }

            var configuration = new HierarchicalConfiguration(new IConfiguration[] { sub.Object });
            Assert.IsTrue(configuration.HasValueFor(key));

            sub.Verify(s => s.HasValueFor(It.IsAny<ConfigurationKey>()), Times.Once());
        }

        [Test]
        public void HasValueWithMultipleConfigurationsAndKnownKey()
        {
            var key = new ConfigurationKey("a", typeof(string));
            var sub1 = new Mock<IConfiguration>();
            {
                sub1.Setup(s => s.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false)
                    .Verifiable();
            }

            var sub2 = new Mock<IConfiguration>();
            {
                sub2.Setup(s => s.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(true)
                    .Verifiable();
            }

            var configuration = new HierarchicalConfiguration(
                new IConfiguration[]
                {
                    sub1.Object,
                    sub2.Object
                });

            Assert.IsTrue(configuration.HasValueFor(key));
            sub1.Verify(s => s.HasValueFor(It.IsAny<ConfigurationKey>()), Times.Once());
            sub2.Verify(s => s.HasValueFor(It.IsAny<ConfigurationKey>()), Times.Once());
        }

        [Test]
        public void HasValueWithMultipleConfigurationsAndUnknownKey()
        {
            var key = new ConfigurationKey("a", typeof(string));
            var sub1 = new Mock<IConfiguration>();
            {
                sub1.Setup(s => s.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false)
                    .Verifiable();
            }

            var sub2 = new Mock<IConfiguration>();
            {
                sub2.Setup(s => s.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false)
                    .Verifiable();
            }

            var configuration = new HierarchicalConfiguration(
                new IConfiguration[]
                {
                    sub1.Object,
                    sub2.Object
                });

            Assert.IsFalse(configuration.HasValueFor(key));
            sub1.Verify(s => s.HasValueFor(It.IsAny<ConfigurationKey>()), Times.Once());
            sub2.Verify(s => s.HasValueFor(It.IsAny<ConfigurationKey>()), Times.Once());
        }

        [Test]
        public void HasValueWithNullKey()
        {
            var sub = new Mock<IConfiguration>();
            {
                sub.Setup(s => s.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(true)
                    .Verifiable();
            }

            var configuration = new HierarchicalConfiguration(new IConfiguration[] { sub.Object });
            Assert.IsFalse(configuration.HasValueFor(null));

            sub.Verify(s => s.HasValueFor(It.IsAny<ConfigurationKey>()), Times.Never());
        }

        [Test]
        public void HasValueWithUnknownKey()
        {
            var key = new ConfigurationKey("a", typeof(string));
            var sub = new Mock<IConfiguration>();
            {
                sub.Setup(s => s.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false)
                    .Verifiable();
            }

            var configuration = new HierarchicalConfiguration(new IConfiguration[] { sub.Object });
            Assert.IsFalse(configuration.HasValueFor(key));

            sub.Verify(s => s.HasValueFor(It.IsAny<ConfigurationKey>()), Times.Once());
        }

        [Test]
        public void ValueWithKnownKey()
        {
            var value = "a";
            var key = new ConfigurationKey("b", typeof(string));
            var sub = new Mock<IConfiguration>();
            {
                sub.Setup(s => s.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(true)
                    .Verifiable();
                sub.Setup(s => s.Value<string>(It.IsAny<ConfigurationKey>()))
                    .Returns(value)
                    .Verifiable();
            }

            var configuration = new HierarchicalConfiguration(new IConfiguration[] { sub.Object });
            Assert.AreSame(value, configuration.Value<string>(key));

            sub.Verify(s => s.HasValueFor(It.IsAny<ConfigurationKey>()), Times.Once());
            sub.Verify(s => s.Value<string>(It.IsAny<ConfigurationKey>()), Times.Once());
        }

        [Test]
        public void ValueWithMultipleConfigurationsAndKnownKey()
        {
            var value = "a";
            var key = new ConfigurationKey("b", typeof(string));
            var sub1 = new Mock<IConfiguration>();
            {
                sub1.Setup(s => s.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false)
                    .Verifiable();
                sub1.Setup(s => s.Value<string>(It.IsAny<ConfigurationKey>()))
                    .Verifiable();
            }

            var sub2 = new Mock<IConfiguration>();
            {
                sub2.Setup(s => s.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(true)
                    .Verifiable();
                sub2.Setup(s => s.Value<string>(It.IsAny<ConfigurationKey>()))
                    .Returns(value)
                    .Verifiable();
            }

            var configuration = new HierarchicalConfiguration(
                new IConfiguration[]
                {
                    sub1.Object,
                    sub2.Object
                });

            Assert.AreSame(value, configuration.Value<string>(key));

            sub1.Verify(s => s.HasValueFor(It.IsAny<ConfigurationKey>()), Times.Once());
            sub1.Verify(s => s.Value<string>(It.IsAny<ConfigurationKey>()), Times.Never());

            sub2.Verify(s => s.HasValueFor(It.IsAny<ConfigurationKey>()), Times.Once());
            sub2.Verify(s => s.Value<string>(It.IsAny<ConfigurationKey>()), Times.Once());
        }

        [Test]
        public void ValueWithMultipleConfigurationsAndUnknownKey()
        {
            var key = new ConfigurationKey("b", typeof(string));
            var sub1 = new Mock<IConfiguration>();
            {
                sub1.Setup(s => s.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false)
                    .Verifiable();
                sub1.Setup(s => s.Value<string>(It.IsAny<ConfigurationKey>()))
                    .Verifiable();
            }

            var sub2 = new Mock<IConfiguration>();
            {
                sub2.Setup(s => s.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false)
                    .Verifiable();
                sub2.Setup(s => s.Value<string>(It.IsAny<ConfigurationKey>()))
                    .Verifiable();
            }

            var configuration = new HierarchicalConfiguration(
                new IConfiguration[]
                {
                    sub1.Object,
                    sub2.Object
                });

            Assert.Throws<ArgumentException>(() => configuration.Value<string>(key));

            sub1.Verify(s => s.HasValueFor(It.IsAny<ConfigurationKey>()), Times.Once());
            sub1.Verify(s => s.Value<string>(It.IsAny<ConfigurationKey>()), Times.Never());

            sub2.Verify(s => s.HasValueFor(It.IsAny<ConfigurationKey>()), Times.Once());
            sub2.Verify(s => s.Value<string>(It.IsAny<ConfigurationKey>()), Times.Never());
        }

        [Test]
        public void ValueWithNullKey()
        {
            var sub = new Mock<IConfiguration>();
            {
                sub.Setup(s => s.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false)
                    .Verifiable();
                sub.Setup(s => s.Value<string>(It.IsAny<ConfigurationKey>()))
                    .Verifiable();
            }

            var configuration = new HierarchicalConfiguration(new IConfiguration[] { sub.Object });
            Assert.Throws<ArgumentNullException>(() => configuration.Value<string>(null));

            sub.Verify(s => s.HasValueFor(It.IsAny<ConfigurationKey>()), Times.Never());
            sub.Verify(s => s.Value<string>(It.IsAny<ConfigurationKey>()), Times.Never());
        }

        [Test]
        public void ValueWithUnknownKey()
        {
            var key = new ConfigurationKey("b", typeof(string));
            var sub = new Mock<IConfiguration>();
            {
                sub.Setup(s => s.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false)
                    .Verifiable();
                sub.Setup(s => s.Value<string>(It.IsAny<ConfigurationKey>()))
                    .Verifiable();
            }

            var configuration = new HierarchicalConfiguration(new IConfiguration[] { sub.Object });
            Assert.Throws<ArgumentException>(() => configuration.Value<string>(key));

            sub.Verify(s => s.HasValueFor(It.IsAny<ConfigurationKey>()), Times.Once());
            sub.Verify(s => s.Value<string>(It.IsAny<ConfigurationKey>()), Times.Never());
        }
    }
}
