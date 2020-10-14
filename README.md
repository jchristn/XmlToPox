# XmlToPox

[![][nuget-img]][nuget]

[nuget]:     https://www.nuget.org/packages/XmlToPox/1.0.0
[nuget-img]: https://badge.fury.io/nu/Object.svg

Convert XML to Plain Old XML and Query using XPath

## Help or feedback

First things first - do you need help or have feedback?  Contact me at joel dot christner at gmail or file an issue here!

## New in v1.0.2

- Retarget to .NET Core 2.0 and .NET Framework 4.5.2

## Simple example
```csharp
using XmlToPox;

string pox = XmlTools.Convert("Some ridiculous XML document goes here");
Console.WriteLine(pox);
```

## Version history

Notes from previous versions (starting with v1.0.0) will be moved here.

v1.0.x
- Initial release
