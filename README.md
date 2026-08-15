# XmlToPox

[![][nuget-img]][nuget]

[nuget]:     https://www.nuget.org/packages/XmlToPox/1.0.0
[nuget-img]: https://badge.fury.io/nu/Object.svg

Convert XML to Plain Old XML and Query using XPath

## Help or feedback

First things first - do you need help or have feedback?  File an issue here!

## New in v1.0.3

- Retarget and bugfixes

## Simple example
```csharp
using XmlToPox;

string pox = XmlTools.Convert("Some ridiculous XML document goes here");
Console.WriteLine(pox);
```

## Testing

Tests are built on [Touchstone](https://www.nuget.org/packages/Touchstone.Core).  Every case is
defined once in `src/Test.Shared` (the central source of truth) and executed unchanged through three
hosts:

```
# Touchstone CLI runner (optionally emit JSON results)
dotnet run --project src/Test.Automated -f net10.0 -- --results test-results/cli-results.json

# xUnit adapter
dotnet test src/Test.Xunit

# NUnit adapter
dotnet test src/Test.Nunit
```

The suites cover both public methods (`XmlTools.Convert` and `XmlTools.QueryXml`) exhaustively in
the positive and negative directions.

## Version history

Notes from previous versions (starting with v1.0.0) will be moved here.

v1.0.x
- Initial release
