//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using NUnit.Framework;

namespace Nuclei.Configuration.Samples
{
    [TestFixture]
    public sealed class XmlConfigurationSample
    {
        [Test]
        public void HasValue()
        {
            var key = new ConfigurationKey("Key1", typeof(int));

            var configuration = new XmlConfiguration(new[] { key }, "samples");
            var hasValue = configuration.HasValueFor(key);

            Assert.IsFalse(hasValue);
        }

        [Test]
        public void ValueFor()
        {
            try
            {
                var key = new ConfigurationKey("Key1", typeof(int));
                var configuration = new XmlConfiguration(new[] { key }, "samples");

                var value = configuration.Value<int>(key);
                Assert.AreEqual(0, value);
            }
            catch (ArgumentException e)
            {
                Assert.IsInstanceOf<ArgumentException>(e);
            }
        }
    }
}
