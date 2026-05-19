# WoadStoat.MarkovNames

[![.NET Build and Test](https://github.com/woadstoat-create/WoadStoat.MarkovNames/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/woadstoat-create/WoadStoat.MarkovNames/actions/workflows/dotnet.yml)

WoadStoat.MarkovNames is an engine-agnostic C# library for procedural name generation using Markov chains.

It can generate:

- character names
- place names
- clan names
- faction names
- planet names
- settlement names
- ship names
- company names
- fantasy or sci-fi culture names

The library is designed for use in games, tools, content pipelines, and procedural generation systems.

It works with:

- plain C#/.NET
- MonoGame
- Unity
- Godot C#
- custom engines
- server-side tools

---

## Features

- Character-based Markov name generation
- Token-based Markov name generation
- Custom culture profiles
- Name categories such as `people`, `places`, `clans`, `planets`
- Seeded deterministic generation
- JSON culture profile loading
- JSON loading from files or strings
- Guided prefixes and suffixes
- Generation filters and custom validation
- Engine-agnostic design
- Unit-tested core behaviour

---

## Quick Start

```csharp
using WoadStoat.MarkovNames;

string[] names =
{
    "Aedan",
    "Alasdair",
    "Caelan",
    "Duncan",
    "Ewan",
    "Fergus",
    "Malcolm",
    "Ruaridh"
};

MarkovNameTrainer trainer = new MarkovNameTrainer(order: 2);
MarkovNameModel model = trainer.Train(names);

MarkovNameGenerator generator = new MarkovNameGenerator(model, seed: 12345);

string name = generator.Generate();

Console.WriteLine(name);
```

---

## Generating Multiple Names

```csharp
NameGenerationOptions options = new NameGenerationOptions
{
    MinLength = 4,
    MaxLength = 12,
    AvoidTrainingDuplicates = true,
    MaxAttempts = 500
};

IReadOnlyList<string> generatedNames = generator.GenerateMany(
    count: 20,
    options);

foreach (string generatedName in generatedNames)
{
    Console.WriteLine(generatedName);
}
```

---

## Deterministic Generation

WoadStoat.MarkovNames supports seeded generation.

This means the same training data and same seed will produce the same sequence of names.

```csharp
MarkovNameGenerator generatorA = new MarkovNameGenerator(model, seed: 123);
MarkovNameGenerator generatorB = new MarkovNameGenerator(model, seed: 123);
```

This is useful for procedural world generation.

For example:

```csharp
int worldSeed = 12345;

string townName = library.Generate(
    "gaelic",
    "places",
    seed: worldSeed + 100);
```

---

## Culture Profiles

A culture profile groups related name categories together.

```csharp
NameCultureProfile profile = new NameCultureProfile("roman")
    .AddCategory("people", new[]
    {
        "Marcus",
        "Lucius",
        "Gaius",
        "Aurelius",
        "Cassius"
    })
    .AddCategory("places", new[]
    {
        "Roma",
        "Capua",
        "Ostia",
        "Ravenna"
    });

NameModelLibrary library = NameModelLibrary.Train(profile, order: 2);

string person = library.Generate("roman", "people", seed: 100);
string place = library.Generate("roman", "places", seed: 200);
```

---

## Categories

Categories are developer-defined.

Common examples:

```text
people
places
clans
families
settlements
planets
ships
factions
companies
rivers
mountains
```

A culture can contain as many categories as needed.

---

## Token-Based Generation

Character-based generation works well for many simple names.

Token-based generation is better when a culture has meaningful letter clusters.

Examples:

```text
Mac
Mc
ch
dh
ae
eo
kh
zh
ll
th
```

Example:

```csharp
INameTokenizer tokenizer = new GreedyNameTokenizer(new[]
{
    "mac",
    "dh",
    "ch",
    "gh",
    "ae",
    "eo",
    "ai"
});

TokenMarkovNameTrainer trainer = new TokenMarkovNameTrainer(
    order: 2,
    tokenizer: tokenizer);

TokenMarkovNameModel model = trainer.Train(new[]
{
    "MacLeod",
    "MacDonald",
    "MacKenzie",
    "Aedan",
    "Eoghan",
    "Donnchadh"
});

TokenMarkovNameGenerator generator = new TokenMarkovNameGenerator(
    model,
    seed: 12345);

string name = generator.Generate();
```

---

## Guided Prefixes

Guided prefixes let the generator begin from a required prefix instead of repeatedly generating and rejecting names.

```csharp
NameGenerationOptions options = new NameGenerationOptions
{
    MinLength = 5,
    MaxLength = 16,
    RequiredPrefix = "Mac",
    UseGuidedPrefix = true,
    AvoidTrainingDuplicates = true,
    MaxAttempts = 1000
};

string clanName = generator.Generate(options);
```

Useful for:

```text
Mac-style clans
Inver-style settlements
House names
corporate prefixes
alien caste prefixes
planet catalogue prefixes
```

---

## Guided Suffixes

Guided suffixes append and validate a required suffix.

```csharp
NameGenerationOptions options = new NameGenerationOptions
{
    MinLength = 5,
    MaxLength = 14,
    RequiredSuffix = "us",
    UseGuidedSuffix = true,
    AvoidTrainingDuplicates = true,
    MaxAttempts = 1000
};
```

This is useful for Roman-style names, family names, faction naming conventions, and settlement naming rules.

---

## Generation Filters

`NameGenerationOptions` can reject generated names that do not fit your rules.

```csharp
NameGenerationOptions options = new NameGenerationOptions
{
    MinLength = 4,
    MaxLength = 12,
    AvoidTrainingDuplicates = true,
    MaxAttempts = 1000,
    MaxConsecutiveIdenticalCharacters = 2,
    CustomValidator = name => !name.EndsWith("son")
};

options.ForbiddenSubstrings.Add("xxx");
options.ForbiddenSubstrings.Add("qq");
options.ForbiddenCharacters.Add('$');
options.ForbiddenCharacters.Add('^');
```

Available options include:

```text
MinLength
MaxLength
MaxAttempts
AvoidTrainingDuplicates
CapitaliseFirstLetter
LowercaseRest
RequiredPrefix
RequiredSuffix
UseGuidedPrefix
UseGuidedSuffix
ForbiddenSubstrings
ForbiddenCharacters
AllowedCharacters
MaxConsecutiveIdenticalCharacters
CustomValidator
```

---

## JSON Culture Profiles

Culture profiles can be stored in JSON.

Example:

```json
{
  "cultureKey": "gaelic",
  "order": 2,
  "useTokens": true,
  "tokens": [
    "mac",
    "mc",
    "mh",
    "dh",
    "ch",
    "gh",
    "ae",
    "ai",
    "eo",
    "io",
    "ua",
    "nn",
    "ll"
  ],
  "categories": {
    "people": [
      "Aedan",
      "Caelan",
      "Eoghan",
      "Ruairidh",
      "Fionnlagh",
      "Domhnall",
      "Eachann",
      "Coinneach",
      "Murchadh",
      "Donnchadh"
    ],
    "clans": [
      "MacLeod",
      "MacDonald",
      "MacKenzie",
      "MacGregor",
      "MacArthur",
      "MacNab"
    ],
    "places": [
      "Inverness",
      "Dunblane",
      "Aviemore",
      "Callander",
      "Mallaig",
      "Tobermory"
    ]
  }
}
```

Load from a file:

```csharp
TokenNameModelLibrary library =
    NameCultureProfileJsonLoader.TrainTokenLibraryFromProfileFile(
        "Data/gaelic.profile.json");
```

Load from a JSON string:

```csharp
string json = File.ReadAllText("Data/gaelic.profile.json");

TokenNameModelLibrary library =
    NameCultureProfileJsonLoader.TrainTokenLibraryFromProfileJson(json);
```

---

## Multi-Culture JSON Profile Sets

You can also define multiple cultures in one file:

```json
{
  "profiles": [
    {
      "cultureKey": "gaelic",
      "order": 2,
      "useTokens": true,
      "tokens": ["mac", "dh", "ch", "ae", "eo"],
      "categories": {
        "people": ["Aedan", "Caelan", "Eoghan"],
        "clans": ["MacLeod", "MacDonald", "MacGregor"]
      }
    },
    {
      "cultureKey": "roman",
      "order": 2,
      "useTokens": true,
      "tokens": ["us", "ius", "ae", "qu"],
      "categories": {
        "people": ["Marcus", "Lucius", "Gaius", "Aurelius"]
      }
    }
  ]
}
```

Load it with:

```csharp
TokenNameModelLibrary library =
    NameCultureProfileJsonLoader.TrainTokenLibraryFromProfileSetFile(
        "Data/cultures.profile-set.json");
```

---

## Saving and Loading Trained Token Libraries

Raw culture profile JSON is useful for editing training data.

Trained model JSON is useful when you want to train once and load quickly later.

```csharp
using WoadStoat.MarkovNames;
using WoadStoat.MarkovNames.Serialization;

TokenNameModelLibrary library =
    NameCultureProfileJsonLoader.TrainTokenLibraryFromProfileJson(profileJson);

string trainedJson =
    TokenNameModelJsonSerializer.LibraryToJson(library);

TokenNameModelLibrary loaded =
    TokenNameModelJsonSerializer.LibraryFromJson(trainedJson);

string name = loaded.Generate(
    "gaelic",
    "clans",
    seed: 12345);
```

## Unity Usage

WoadStoat.MarkovNames does not depend on Unity.

You can load a JSON profile through a `TextAsset`.

```csharp
using UnityEngine;
using WoadStoat.MarkovNames;

public sealed class NameGeneratorExample : MonoBehaviour
{
    [SerializeField] private TextAsset profileJson;

    private TokenNameModelLibrary library;

    private void Awake()
    {
        library = NameCultureProfileJsonLoader
            .TrainTokenLibraryFromProfileJson(profileJson.text);
    }

    private void Start()
    {
        string name = library.Generate(
            "gaelic",
            "people",
            seed: 12345);

        Debug.Log(name);
    }
}
```

---

## MonoGame Usage

WoadStoat.MarkovNames does not depend on MonoGame.

You can use it during startup, world generation, or content loading.

```csharp
using WoadStoat.MarkovNames;

public sealed class WorldNameService
{
    private readonly TokenNameModelLibrary _library;

    public WorldNameService(string profileJson)
    {
        _library = NameCultureProfileJsonLoader
            .TrainTokenLibraryFromProfileJson(profileJson);
    }

    public string GenerateTownName(int worldSeed, int townIndex)
    {
        return _library.Generate(
            "gaelic",
            "places",
            seed: worldSeed + townIndex,
            new NameGenerationOptions
            {
                MinLength = 4,
                MaxLength = 14,
                AvoidTrainingDuplicates = true
            });
    }
}
```

---

## Installing a Local Package

After packing the project locally, install it into another project with:

```bash
dotnet add package WoadStoat.WoadStoat.MarkovNames \
  --version 0.1.0 \
  --source /path/to/WoadStoat.MarkovNames/artifacts/packages
```

Example:

```bash
dotnet add package WoadStoat.WoadStoat.MarkovNames \
  --version 0.1.0 \
  --source /workspaces/WoadStoat.MarkovNames/WoadStoat.MarkovNames/artifacts/packages
```

---

## Running the Console Example

```bash
dotnet run --project examples/WoadStoat.MarkovNames.ConsoleExample
```

---

## Running Tests

```bash
dotnet test
```

---

## Packing Locally

Build and test:

```bash
dotnet clean
dotnet build -c Release
dotnet test -c Release
```

Create the package:

```bash
mkdir -p artifacts/packages
dotnet pack src/WoadStoat.MarkovNames/WoadStoat.MarkovNames.csproj -c Release -o artifacts/packages
```

The package will be created under:

```text
artifacts/packages/
```

Expected files:

```text
WoadStoat.WoadStoat.MarkovNames.0.1.0.nupkg
WoadStoat.WoadStoat.MarkovNames.0.1.0.snupkg
```

The `.nupkg` file is the actual NuGet package.

The `.snupkg` file is the symbols package, useful for debugging.

---

## Testing the Package in a Fresh Project

Create a test project:

```bash
mkdir WoadStoat.MarkovNamesPackageTest
cd WoadStoat.MarkovNamesPackageTest
dotnet new console
```

Install the local package:

```bash
dotnet add package WoadStoat.WoadStoat.MarkovNames \
  --version 0.1.0 \
  --source /path/to/WoadStoat.MarkovNames/artifacts/packages
```

Example test program:

```csharp
using WoadStoat.MarkovNames;

string[] samples =
{
    "Aedan",
    "Alasdair",
    "Caelan",
    "Duncan",
    "Ewan",
    "Fergus",
    "Malcolm",
    "Ruaridh"
};

MarkovNameModel model = new MarkovNameTrainer(order: 2).Train(samples);

MarkovNameGenerator generator = new MarkovNameGenerator(model, seed: 12345);

NameGenerationOptions options = new NameGenerationOptions
{
    MinLength = 4,
    MaxLength = 12,
    AvoidTrainingDuplicates = true,
    MaxAttempts = 1000
};

for (int i = 0; i < 10; i++)
{
    Console.WriteLine(generator.Generate(options));
}
```

Run:

```bash
dotnet run
```

---

## Project Structure

```text
WoadStoat.MarkovNames/
├── src/
│   └── WoadStoat.MarkovNames/
│       ├── MarkovNameTrainer.cs
│       ├── MarkovNameModel.cs
│       ├── MarkovNameGenerator.cs
│       ├── TokenMarkovNameTrainer.cs
│       ├── TokenMarkovNameModel.cs
│       ├── TokenMarkovNameGenerator.cs
│       ├── NameCultureProfile.cs
│       ├── NameModelLibrary.cs
│       ├── TokenNameModelLibrary.cs
│       └── NameCultureProfileJsonLoader.cs
│
├── examples/
│   └── WoadStoat.MarkovNames.ConsoleExample/
│
├── tests/
│   └── WoadStoat.MarkovNames.Tests/
│
├── docs/
│   ├── json-profile-format.md
│   ├── tokenisation.md
│   └── generation-options.md
│
└── samples/
```

---

## Recommended Markov Orders

```text
Order 1: loose, chaotic, good for alien or strange names
Order 2: good general default
Order 3: closer to training data
Order 4+: may overfit, especially with small datasets
```

Suggested defaults:

```text
human names: 2 or 3
place names: 2 or 3
alien names: 1 or 2
faction names: 2
short datasets: 1 or 2
large datasets: 2 or 3
```

---

## Notes on Training Data

For best results:

- keep categories focused
- avoid mixing people, places, and factions in one category
- use at least 20–50 examples per category where possible
- use token-based generation for languages with important letter clusters
- use order 2 as the default starting point
- use deterministic seeds for repeatable procedural worlds

Poor category:

```text
Duncan
Inverness
MacLeod
Ben Nevis
The Red Company
```

Better categories:

```text
people: Duncan, Ewan, Malcolm
places: Inverness, Dunblane, Mallaig
clans: MacLeod, MacDonald, MacGregor
factions: Red Company, Iron League
```

---

## Documentation

Additional documentation:

- [JSON Profile Format](WoadStoat.MarkovNames/docs/json-profile-format.md)
- [Tokenisation](WoadStoat.MarkovNames/docs/tokenisation.md)
- [Generation Options](WoadStoat.MarkovNames/docs/generation-options.md)

---

## Current Status

WoadStoat.MarkovNames currently supports:

- character-based Markov generation
- token-based Markov generation
- culture/category model libraries
- JSON profile files
- JSON profile strings
- guided prefixes and suffixes
- generation filters
- deterministic seeded generation
- unit tests
- local NuGet package generation

Future improvements may include:

- GitHub Actions CI
- token model serialisation
- improved suffix generation
- larger sample culture packs
- Unity and MonoGame sample projects