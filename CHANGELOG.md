# Change Log

## Current Version

v1.1.0

- Remove strong-name signing; assemblies are now unsigned (breaking for consumers bound to the previous strong name / public key token 0f25fbe302795bd9)
- Update test dependencies: Touchstone.* 0.1.12 -> 0.2.1, NUnit 4.6.1 -> 5.0.0, Microsoft.NET.Test.Sdk 18.9.0 -> 18.10.1, coverlet.collector 10.0.1 -> 10.1.0, NUnit.Analyzers 4.14.0 -> 4.15.0, NUnit3TestAdapter 6.2.0 -> 6.3.0, xunit.analyzers 2.0.0 -> 2.1.0

## Previous Versions

v1.0.4

- Update test dependencies
- Improve empty element cleanup for valid XML names and nested empty elements

v1.0.x

- Retarget to .NET Core 2.0 and .NET Framework 4.5.2
- Initial release
