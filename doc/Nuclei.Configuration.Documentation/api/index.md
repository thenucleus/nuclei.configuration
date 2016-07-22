
# Nuclei.Configuration


## Configuration keys

To define a configuration key create an instance of the `ConfigurationKey` class with the type of the value that should be retrieved and the 'path' to the value in the
configuration store.

[!code-csharp[ConfigurationKey](..\..\Nuclei.Configuration.Samples\ApplicationConfigurationSample.cs?range=19)]

## ApplicationConfiguration

The `ApplicationConfiguration` class gets configuration data from the application configuration file. To store information in the configuration file create
a configuration section that will hold the configuration values and insert the desired values. Each value is marked by a key and stored so that the
.NET xml serializer can read the values.

[!code-xml[Configuration sample](..\..\Nuclei.Configuration.Samples\app.config)]

Create the `ApplicationConfiguration` instance and provide the name of the configuration section.

[!code-csharp[ApplicationConfiguration.HasValueFor](..\..\Nuclei.Configuration.Samples\ApplicationConfigurationSample.cs?range=19-22)]

[!code-csharp[ApplicationConfiguration.Value](..\..\Nuclei.Configuration.Samples\ApplicationConfigurationSample.cs?range=32-35)]


## HierarchicalConfiguration

The `HierarchicalConfiguration` class does not store any configuration values itself. Instead it stores a collection of `IConfiguration` instances and retrieves the
configuration data from the items in that collection. If multiple configurations have a configuration for a given configurations key then the first configuration
to provide the configuration value is used.

[!code-csharp[HierarchicalConfiguration.HasValueFor](..\..\Nuclei.Configuration.Samples\HierarchicalConfigurationSample.cs?range=19-26)]

[!code-csharp[HierarchicalConfiguration.Value](..\..\Nuclei.Configuration.Samples\HierarchicalConfigurationSample.cs?range=36-43)]


## NullConfiguration

The `NullConfiguration` class does not store any configuration values. Return values are the default value for the type of data that is supposed to be stored.

[!code-csharp[NullConfiguration.HasValueFor](..\..\Nuclei.Configuration.Samples\NullConfigurationSample.cs?range=18-21)]

[!code-csharp[NullConfiguration.Value](..\..\Nuclei.Configuration.Samples\NullConfigurationSample.cs?range=29-32)]

