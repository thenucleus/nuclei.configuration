//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using Moq;
using NUnit.Framework;

namespace Nuclei.Configuration
{
    [TestFixture]
    public sealed class ConsulConfigurationTest
    {
        private const string ConsulConfigurationPrefix = "";
        private const string ConsulDatacenter = "dc1";
        private const string ConsulIPAddress = "127.0.0.1";
        private const int ConsulPort = 7890;

        private static Process _consulApplication;
        private static IConfiguration _subConfiguration;

        private static string CreateConsulConfigurationFile(
            string executingDirectory,
            string consulAddress,
            int consulPort,
            string consulDatacenter)
        {
            var consulConfigFile = Path.Combine(executingDirectory, "nuclei.configuration_consulconfig.json");
            using (var writer = new StreamWriter(consulConfigFile, false))
            {
                var configurationText = @"{{
    ""log_level"": ""TRACE"",
    ""bind_addr"": ""{0}"",
    ""ports"": {{
        ""http"": {1}
      }},
    ""server"": true,
    ""bootstrap"": true,
    ""acl_datacenter"": ""{2}""
}}";
                var configuration = string.Format(
                    CultureInfo.InvariantCulture,
                    configurationText,
                    consulAddress,
                    consulPort,
                    consulDatacenter);
                writer.Write(configuration);
            }

            return consulConfigFile;
        }

        private static Process StartConsulAndWaitForInitializationCompleted(
            string executingDirectory,
            string configurationFilePath)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(executingDirectory, "consul.exe"),
                Arguments = string.Format(
                        CultureInfo.InvariantCulture,
                        "agent -dev -config-file {0} -ui consul_0.6.4_web_ui",
                        configurationFilePath),
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
            };

            var process = new Process
            {
                StartInfo = startInfo,
            };

            bool isRunning = false;
            DataReceivedEventHandler handler = (o, e) =>
            {
                if ((e.Data != null) && e.Data.Contains("cluster leadership acquired"))
                {
                    isRunning = true;
                }
            };
            process.OutputDataReceived += handler;

            process.Start();
            process.BeginOutputReadLine();

            var killTime = DateTimeOffset.Now + TimeSpan.FromSeconds(15);
            while (!isRunning && (DateTimeOffset.Now < killTime))
            {
                Thread.Sleep(100);
            }

            process.OutputDataReceived -= handler;

            return process;
        }

        private static void SetConfigurationKeyValue(
            string consulUrl,
            int consulPort,
            string consulPrefix,
            string key,
            string data)
        {
            var url = string.Format(CultureInfo.InvariantCulture, "http://{0}:{1}/v1/kv/{2}{3}", consulUrl, consulPort, consulPrefix, key);
            var request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "PUT";
            request.ContentType = "application/json";
            request.Accept = "application/json";

            byte[] arrData = Encoding.UTF8.GetBytes(data);
            request.ContentLength = arrData.Length;

            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(arrData, 0, arrData.Length);
            }

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.OK)
                {
                    throw new ArgumentException("Failed to set key value.");
                }
            }
        }

        private static void SetConfigurationKeyValue(
            string consulUrl,
            int consulPort,
            string consulPrefix,
            string key,
            object data)
        {
            var url = string.Format(CultureInfo.InvariantCulture, "http://{0}:{1}/v1/kv/{2}{3}", consulUrl, consulPort, consulPrefix, key);
            var request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "PUT";
            request.ContentType = "application/json";
            request.Accept = "application/json";

            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            string jsonData = jsSerializer.Serialize(data);

            byte[] arrData = Encoding.UTF8.GetBytes(jsonData);
            request.ContentLength = arrData.Length;

            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(arrData, 0, arrData.Length);
            }

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.OK)
                {
                    throw new ArgumentException("Failed to set key value.");
                }
            }
        }

        private static void SetConfigurationKeyValues(
            string consulUrl,
            int consulPort,
            string consulPrefix)
        {
            SetConfigurationKeyValue(consulUrl, consulPort, consulPrefix, "bool", true);
            SetConfigurationKeyValue(consulUrl, consulPort, consulPrefix, "char", 'a');
            SetConfigurationKeyValue(consulUrl, consulPort, consulPrefix, "double", 10.0d);
            SetConfigurationKeyValue(consulUrl, consulPort, consulPrefix, "float", 10.0f);
            SetConfigurationKeyValue(consulUrl, consulPort, consulPrefix, "int", 10);
            SetConfigurationKeyValue(consulUrl, consulPort, consulPrefix, "long", 10L);
            SetConfigurationKeyValue(consulUrl, consulPort, consulPrefix, "short", (short)10);
            SetConfigurationKeyValue(consulUrl, consulPort, consulPrefix, "string", (object)"string");
            SetConfigurationKeyValue(consulUrl, consulPort, consulPrefix, "uint", 10U);
            SetConfigurationKeyValue(consulUrl, consulPort, consulPrefix, "ulong", 10UL);
            SetConfigurationKeyValue(consulUrl, consulPort, consulPrefix, "ushort", (ushort)10);

            SetConfigurationKeyValue(consulUrl, consulPort, consulPrefix, "flat_string", "string");
        }

        private static IConfiguration StoreConfiguration(
            string consulUrl,
            int consulPort,
            string consulDatacenter,
            string consulPrefix)
        {
            return new ConstantConfiguration(
                new Dictionary<ConfigurationKeyBase, object>
                {
                    [ConfigurationKeys.ConsulAddress] = string.Format(CultureInfo.InvariantCulture, "http://{0}:{1}", consulUrl, consulPort),
                    [ConfigurationKeys.ConsulDatacenter] = consulDatacenter,
                    [ConfigurationKeys.ConsulConfigurationPrefix] = consulPrefix,
                });
        }

        [TestFixtureTearDown]
        public void AfterTests()
        {
            if (_consulApplication != null)
            {
                _consulApplication.Kill();
                _consulApplication.Dispose();
            }
        }

        [TestFixtureSetUp]
        public void BeforeTests()
        {
            _subConfiguration = StoreConfiguration(
                ConsulIPAddress,
                ConsulPort,
                ConsulDatacenter,
                ConsulConfigurationPrefix);

            var executingDirectory = Assembly.GetExecutingAssembly().LocalDirectoryPath();
            var consulConfigFile = CreateConsulConfigurationFile(
                executingDirectory,
                ConsulIPAddress,
                ConsulPort,
                ConsulDatacenter);

            _consulApplication = StartConsulAndWaitForInitializationCompleted(executingDirectory, consulConfigFile);
            SetConfigurationKeyValues(
                ConsulIPAddress,
                ConsulPort,
                ConsulConfigurationPrefix);
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Nuclei.Configuration.ConsulConfiguration",
            Justification = "Testing that the constructor throws.")]
        public void CreateWithNullConfiguration()
        {
            Assert.Throws<ArgumentNullException>(() => new ConsulConfiguration(new List<ConfigurationKeyBase>(), null));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Nuclei.Configuration.ConsulConfiguration",
            Justification = "Testing that the constructor throws.")]
        public void CreateWithNullConfigurationKeys()
        {
            Assert.Throws<ArgumentNullException>(() => new ConsulConfiguration(null, new Mock<IConfiguration>().Object));
        }

        [Test]
        public void HasValueWithKnownKey()
        {
            var key = new ConfigurationKey<string>("string");

            var configuration = new ConsulConfiguration(
                new List<ConfigurationKeyBase>
                {
                    key
                },
                _subConfiguration);
            Assert.IsTrue(configuration.HasValueFor(key));
        }

        [Test]
        public void HasValueWithNullKey()
        {
            var key = new ConfigurationKey<string>("string");

            var configuration = new ConsulConfiguration(
                new List<ConfigurationKeyBase>
                {
                    key
                },
                _subConfiguration);
            Assert.IsFalse(configuration.HasValueFor(null));
        }

        [Test]
        public void HasValueWithUnknownKey()
        {
            var key = new ConfigurationKey<string>("unknown");

            var configuration = new ConsulConfiguration(
                new List<ConfigurationKeyBase>
                {
                    key
                },
                _subConfiguration);
            Assert.IsFalse(configuration.HasValueFor(key));
        }

        [Test]
        public void ValueAsBooleanWithKnownKey()
        {
            var value = true;

            var key = new ConfigurationKey<bool>("bool");
            var configuration = new ConsulConfiguration(
                new List<ConfigurationKeyBase>
                {
                    key
                },
                _subConfiguration);
            Assert.AreEqual(value, configuration.Value(key));
        }

        [Test]
        public void ValueAsCharacterWithKnownKey()
        {
            var value = 'a';

            var key = new ConfigurationKey<char>("char");
            var configuration = new ConsulConfiguration(
                new List<ConfigurationKeyBase>
                {
                    key
                },
                _subConfiguration);
            Assert.AreEqual(value, configuration.Value(key));
        }

        [Test]
        public void ValueAsDoubleWithKnownKey()
        {
            var value = 10.0d;

            var key = new ConfigurationKey<double>("double");
            var configuration = new ConsulConfiguration(
                new List<ConfigurationKeyBase>
                {
                    key
                },
                _subConfiguration);
            Assert.AreEqual(value, configuration.Value(key));
        }

        [Test]
        public void ValueAsFloatWithKnownKey()
        {
            var value = 10.0f;

            var key = new ConfigurationKey<float>("float");
            var configuration = new ConsulConfiguration(
                new List<ConfigurationKeyBase>
                {
                    key
                },
                _subConfiguration);
            Assert.AreEqual(value, configuration.Value(key));
        }

        [Test]
        public void ValueAsIntWithKnownKey()
        {
            var value = 10;

            var key = new ConfigurationKey<int>("int");
            var configuration = new ConsulConfiguration(
                new List<ConfigurationKeyBase>
                {
                    key
                },
                _subConfiguration);
            Assert.AreEqual(value, configuration.Value(key));
        }

        [Test]
        public void ValueAsLongWithKnownKey()
        {
            var value = 10L;

            var key = new ConfigurationKey<long>("long");
            var configuration = new ConsulConfiguration(
                new List<ConfigurationKeyBase>
                {
                    key
                },
                _subConfiguration);
            Assert.AreEqual(value, configuration.Value(key));
        }

        [Test]
        public void ValueAsNonQuotedStringWithKnownKey()
        {
            var value = "string";

            var key = new ConfigurationKey<string>("flat_string");
            var configuration = new ConsulConfiguration(
                new List<ConfigurationKeyBase>
                {
                    key
                },
                _subConfiguration);
            Assert.AreEqual(value, configuration.Value(key));
        }

        [Test]
        public void ValueAsShortWithKnownKey()
        {
            short value = 10;

            var key = new ConfigurationKey<short>("short");
            var configuration = new ConsulConfiguration(
                new List<ConfigurationKeyBase>
                {
                    key
                },
                _subConfiguration);
            Assert.AreEqual(value, configuration.Value(key));
        }

        [Test]
        public void ValueAsStringWithKnownKey()
        {
            var value = "string";

            var key = new ConfigurationKey<string>("string");
            var configuration = new ConsulConfiguration(
                new List<ConfigurationKeyBase>
                {
                    key
                },
                _subConfiguration);
            Assert.AreEqual(value, configuration.Value(key));
        }

        [Test]
        public void ValueAsUnsignedIntWithKnownKey()
        {
            var value = 10U;

            var key = new ConfigurationKey<uint>("uint");
            var configuration = new ConsulConfiguration(
                new List<ConfigurationKeyBase>
                {
                    key
                },
                _subConfiguration);
            Assert.AreEqual(value, configuration.Value(key));
        }

        [Test]
        public void ValueAsUnsignedLongWithKnownKey()
        {
            var value = 10UL;

            var key = new ConfigurationKey<ulong>("ulong");
            var configuration = new ConsulConfiguration(
                new List<ConfigurationKeyBase>
                {
                    key
                },
                _subConfiguration);
            Assert.AreEqual(value, configuration.Value(key));
        }

        [Test]
        public void ValueAsUnsignedShortWithKnownKey()
        {
            ushort value = 10;

            var key = new ConfigurationKey<ushort>("ushort");
            var configuration = new ConsulConfiguration(
                new List<ConfigurationKeyBase>
                {
                    key
                },
                _subConfiguration);
            Assert.AreEqual(value, configuration.Value(key));
        }

        [Test]
        public void ValueWithNullKey()
        {
            var key = new ConfigurationKey<string>("a");

            var configuration = new ConsulConfiguration(
                new List<ConfigurationKeyBase>
                {
                    key
                },
                _subConfiguration);
            Assert.Throws<ArgumentNullException>(() => configuration.Value<string>(null));
        }

        [Test]
        public void ValueWithUnknownKey()
        {
            var key = new ConfigurationKey<string>("unknown");

            var configuration = new ConsulConfiguration(
                new List<ConfigurationKeyBase>
                {
                    key
                },
                _subConfiguration);
            Assert.Throws<ArgumentException>(() => configuration.Value(key));
        }
    }
}
