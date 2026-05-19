# Publishing WoadStoat.MarkovNames

This document describes the manual publishing process for WoadStoat.MarkovNames.

## Pre-release Checks

Run from the repository root:

```bash
dotnet clean
dotnet format WoadStoat.MarkovNames.slnx
dotnet build WoadStoat.MarkovNames.slnx -c Release
dotnet test WoadStoat.MarkovNames.slnx -c Release
```

Pack:

```bash
mkdir -p artifacts/packages
dotnet pack WoadStoat.MarkovNames/src/WoadStoat.MarkovNames/WoadStoat.MarkovNames.csproj -c Release -o artifacts/packages
```

Inspect package contents:

```bash
unzip -l artifacts/packages/WoadStoat.MarkovNames.0.1.0.nupkg
```

Expected contents include:

```text
lib/netstandard2.1/WoadStoat.MarkovNames.dll
lib/netstandard2.1/WoadStoat.MarkovNames.xml
README.md
LICENSE
RELEASE_NOTES.md
```

## Test Local Install

Create a temporary project outside the repository:

```bash
mkdir WoadStoat.MarkovNames.PackageTest
cd WoadStoat.MarkovNames.PackageTest
dotnet new console
```

Install from local source:

```bash
dotnet add package WoadStoat.MarkovNames \
  --version 0.1.0 \
  --source /path/to/WoadStoat.MarkovNames/artifacts/packages
```

Run a basic generation test.

## Publishing to NuGet.org

Create an API key in your NuGet.org account.

Then publish manually:

```bash
dotnet nuget push artifacts/packages/WoadStoat.MarkovNames.0.1.0.nupkg \
  --api-key YOUR_NUGET_API_KEY \
  --source https://api.nuget.org/v3/index.json
```

Never commit API keys.

## Tagging the Release

After publishing:

```bash
git tag v0.1.0
git push origin v0.1.0
```