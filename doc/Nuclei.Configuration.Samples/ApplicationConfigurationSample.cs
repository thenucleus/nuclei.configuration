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
    public sealed class ApplicationConfigurationSample
    {
        [Test]
        public void HasValue()
        {
            var key = new ConfigurationKey<int>("Key1");

            var configuration = new ApplicationConfiguration(new[] { key }, "samples");
            var hasValue = configuration.HasValueFor(key);

            Assert.IsTrue(hasValue);
        }

        [Test]
        public void ValueFor()
        {
            try
            {
                var key = new ConfigurationKey<int>("Key1");
                var configuration = new ApplicationConfiguration(new[] { key }, "samples");

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
