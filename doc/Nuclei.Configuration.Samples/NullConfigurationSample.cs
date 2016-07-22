//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using NUnit.Framework;

namespace Nuclei.Configuration.Samples
{
    [TestFixture]
    public sealed class NullConfigurationSample
    {
        [Test]
        public void HasValue()
        {
            var key = new ConfigurationKey("my_property", typeof(int));

            var configuration = new NullConfiguration();
            var hasValue = configuration.HasValueFor(key);

            Assert.IsFalse(hasValue);
        }

        [Test]
        public void ValueFor()
        {
            var key = new ConfigurationKey("my_property", typeof(int));

            var configuration = new NullConfiguration();
            var value = configuration.Value<int>(key);

            Assert.AreEqual(default(int), value);
        }
    }
}
