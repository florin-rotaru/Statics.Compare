# What is Air.Compare?

Is a simple open source objects compare library

# Overview
Compares members of two objects and evaluates if they are equal or none equal based on conventions (public properties with the same names and same/derived/convertible types). Provides also few tweak options 

You can install it via [package manager console](https://docs.microsoft.com/en-us/nuget/consume-packages/install-use-packages-powershell)
```
PM> Install-Package Air.Compare
```

## Basic usage
```csharp
using static Air.Compare.Members;

var equals = CompareEquals(left, right);
```

### Default compare options
- StringComparison memberNameComparison = StringComparison.OrdinalIgnoreCase, 
- bool evaluateChildNodes = false, 
- bool ignoreDefaultLeftValues = false, 
- bool ignoreDefaultRightValues = false, 
- bool useConvert = false

Differences can be listed by using parameter memberDiffs:
```csharp
var equals = CompareEquals(
  leftEntries, 
  rightEntries, 
  out IEnumerable<MemberDiff> memberDiffs, 
  evaluateChildNodes: true);
```
