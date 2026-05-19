using System;
using System.IO;
using System.Linq;
using WoadNameGen;
using WoadNameGen.Serialization;

string[] scottishPeople =
{
    "Aedan",
    "Alasdair",
    "Angus",
    "Callum",
    "Caelan",
    "Duncan",
    "Ewan",
    "Fergus",
    "Finlay",
    "Hamish",
    "Iain",
    "Lachlan",
    "Malcolm",
    "Murdo",
    "Ruaridh",
    "Seumas",
    "Torquil"
};

string[] scottishPlaces =
{
    "Aberdeen",
    "Inverness",
    "Dundee",
    "Stirling",
    "Dunfermline",
    "Kilmarnock",
    "Aviemore",
    "Oban",
    "Fortrose",
    "Glenrothes",
    "Pitlochry",
    "Dunblane",
    "Callander",
    "Mallaig",
    "Tobermory"
};

string[] scottishClans =
{
    "MacLeod",
    "MacDonald",
    "MacKenzie",
    "MacGregor",
    "Campbell",
    "Fraser",
    "Gordon",
    "Stewart",
    "Sinclair",
    "MacArthur",
    "MacNab",
    "Robertson"
};

string[] romanPeople =
{
    "Marcus",
    "Lucius",
    "Gaius",
    "Titus",
    "Quintus",
    "Cassius",
    "Aurelius",
    "Octavius",
    "Valerius",
    "Julius",
    "Flavius",
    "Severus"
};

string[] romanPlaces =
{
    "Roma",
    "Capua",
    "Ostia",
    "Ravenna",
    "Mediolanum",
    "Pompeii",
    "Neapolis",
    "Aquileia",
    "Lugdunum",
    "Massilia",
    "Ariminum"
};

NameCultureProfile scottishProfile = new NameCultureProfile("scottish")
    .AddCategory("people", scottishPeople)
    .AddCategory("places", scottishPlaces)
    .AddCategory("clans", scottishClans);

NameCultureProfile romanProfile = new NameCultureProfile("roman")
    .AddCategory("people", romanPeople)
    .AddCategory("places", romanPlaces);

NameModelLibrary library = NameModelLibrary.TrainMany(
    new[]
    {
        scottishProfile,
        romanProfile
    },
    order: 2);

NameGenerationOptions options = new NameGenerationOptions
{
    MinLength = 4,
    MaxLength = 12,
    AvoidTrainingDuplicates = true,
    MaxAttempts = 250,
    MaxConsecutiveIdenticalCharacters = 2
};

options.ForbiddenSubstrings.Add("xxx");
options.ForbiddenSubstrings.Add("qq");
options.ForbiddenCharacters.Add('$');
options.ForbiddenCharacters.Add('^');

Console.WriteLine("Cultures:");
foreach (string cultureKey in library.CultureKeys)
{
    Console.WriteLine($"- {cultureKey}");

    foreach (string categoryKey in library.GetCategoryKeys(cultureKey))
    {
        Console.WriteLine($"  - {categoryKey}");
    }
}

Console.WriteLine();
Console.WriteLine("Scottish people:");
foreach (string name in library.GenerateMany("scottish", "people", count: 10, seed: 100, options))
{
    Console.WriteLine(name);
}

Console.WriteLine();
Console.WriteLine("Scottish places:");
foreach (string name in library.GenerateMany("scottish", "places", count: 10, seed: 200, options))
{
    Console.WriteLine(name);
}

Console.WriteLine();
Console.WriteLine("Scottish clans:");
foreach (string name in library.GenerateMany("scottish", "clans", count: 10, seed: 300, options))
{
    Console.WriteLine(name);
}

Console.WriteLine();
Console.WriteLine("Roman people:");
foreach (string name in library.GenerateMany("roman", "people", count: 10, seed: 400, options))
{
    Console.WriteLine(name);
}

Console.WriteLine();
Console.WriteLine("Roman places:");
foreach (string name in library.GenerateMany("roman", "places", count: 10, seed: 500, options))
{
    Console.WriteLine(name);
}

Console.WriteLine();
Console.WriteLine("Single generated examples:");

string person = library.Generate("scottish", "people", seed: 12345, options);
string place = library.Generate("scottish", "places", seed: 12345, options);
string roman = library.Generate("roman", "people", seed: 12345, options);

Console.WriteLine($"Scottish person: {person}");
Console.WriteLine($"Scottish place: {place}");
Console.WriteLine($"Roman person: {roman}");

Console.WriteLine();
Console.WriteLine("Saving library to JSON...");

string outputPath = "names-library.json";

NameModelJsonSerializer.SaveLibrary(library, outputPath);

Console.WriteLine($"Saved to: {Path.GetFullPath(outputPath)}");

Console.WriteLine();
Console.WriteLine("Loading library from JSON...");

NameModelLibrary loadedLibrary = NameModelJsonSerializer.LoadLibrary(outputPath);

Console.WriteLine("Loaded library successfully.");

Console.WriteLine();
Console.WriteLine("Generated from loaded library:");

IReadOnlyList<string> loadedNames = loadedLibrary.GenerateMany(
    "scottish",
    "people",
    count: 10,
    seed: 999,
    options);

foreach (string name in loadedNames)
{
    Console.WriteLine(name);
}

Console.WriteLine();
Console.WriteLine("Determinism check:");

IReadOnlyList<string> beforeSave = library.GenerateMany(
    "roman",
    "people",
    count: 10,
    seed: 123456,
    options);

IReadOnlyList<string> afterLoad = loadedLibrary.GenerateMany(
    "roman",
    "people",
    count: 10,
    seed: 123456,
    options);

bool same = beforeSave.SequenceEqual(afterLoad);

Console.WriteLine($"Same before/after load: {same}");

Console.WriteLine();
Console.WriteLine("Filtered Scottish place names:");

NameGenerationOptions placeOptions = new NameGenerationOptions
{
    MinLength = 5,
    MaxLength = 14,
    AvoidTrainingDuplicates = true,
    MaxAttempts = 500,
    MaxConsecutiveIdenticalCharacters = 2
};

placeOptions.ForbiddenSubstrings.Add("ii");
placeOptions.ForbiddenSubstrings.Add("uu");

IReadOnlyList<string> filteredPlaces = library.GenerateMany(
    "scottish",
    "places",
    count: 10,
    seed: 777,
    placeOptions);

foreach (string p in filteredPlaces)
{
    Console.WriteLine(p);
}

Console.WriteLine();
Console.WriteLine("Roman names that do not end with 'us':");

NameGenerationOptions romanOptions = new NameGenerationOptions
{
    MinLength = 4,
    MaxLength = 12,
    AvoidTrainingDuplicates = true,
    MaxAttempts = 1000,
    CustomValidator = name => !name.EndsWith("us", StringComparison.OrdinalIgnoreCase)
};

IReadOnlyList<string> filteredRomanNames = library.GenerateMany(
    "roman",
    "people",
    count: 10,
    seed: 888,
    romanOptions);

foreach (string name in filteredRomanNames)
{
    Console.WriteLine(name);
}

Console.WriteLine();
Console.WriteLine("Names starting with 'Ma':");

NameGenerationOptions prefixOptions = new NameGenerationOptions
{
    MinLength = 4,
    MaxLength = 12,
    AvoidTrainingDuplicates = false,
    MaxAttempts = 2000,
    RequiredPrefix = "Ma"
};

IReadOnlyList<string> prefixedNames = library.GenerateMany(
    "scottish",
    "people",
    count: 5,
    seed: 999,
    prefixOptions);

foreach (string name in prefixedNames)
{
    Console.WriteLine(name);
}

Console.WriteLine();
Console.WriteLine("Scottish names using restricted character set:");

NameGenerationOptions restrictedAlphabetOptions = new NameGenerationOptions
{
    MinLength = 4,
    MaxLength = 12,
    AvoidTrainingDuplicates = true,
    MaxAttempts = 1000
};

foreach (char c in "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ")
{
    restrictedAlphabetOptions.AllowedCharacters.Add(c);
}

IReadOnlyList<string> restrictedNames = library.GenerateMany(
    "scottish",
    "people",
    count: 10,
    seed: 1234,
    restrictedAlphabetOptions);

foreach (string name in restrictedNames)
{
    Console.WriteLine(name);
}

Console.WriteLine();
Console.WriteLine("Token-based Gaelic-style names:");

string[] gaelicInspiredNames =
{
    "MacLeod",
    "MacDonald",
    "MacKenzie",
    "MacGregor",
    "MacArthur",
    "MacNab",
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
};

INameTokenizer gaelicTokenizer = new GreedyNameTokenizer(new[]
{
    "mac",
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
});

TokenMarkovNameTrainer tokenTrainer = new TokenMarkovNameTrainer(
    order: 2,
    tokenizer: gaelicTokenizer);

TokenMarkovNameModel tokenModel = tokenTrainer.Train(gaelicInspiredNames);

TokenMarkovNameGenerator tokenGenerator = new TokenMarkovNameGenerator(
    tokenModel,
    seed: 54321);

NameGenerationOptions tokenOptions = new NameGenerationOptions
{
    MinLength = 4,
    MaxLength = 14,
    AvoidTrainingDuplicates = true,
    MaxAttempts = 1000,
    MaxConsecutiveIdenticalCharacters = 2
};

IReadOnlyList<string> tokenNames = tokenGenerator.GenerateMany(
    count: 20,
    options: tokenOptions);

foreach (string name in tokenNames)
{
    Console.WriteLine(name);
}

Console.WriteLine();
Console.WriteLine("Character-based comparison:");

MarkovNameTrainer characterTrainer = new MarkovNameTrainer(order: 2);
MarkovNameModel characterModel = characterTrainer.Train(gaelicInspiredNames);
MarkovNameGenerator characterGenerator = new MarkovNameGenerator(characterModel, seed: 54321);

IReadOnlyList<string> characterNames = characterGenerator.GenerateMany(
    count: 20,
    options: tokenOptions);

foreach (string name in characterNames)
{
    Console.WriteLine(name);
}

Console.WriteLine();
Console.WriteLine("Guided prefix example: Scottish clan names beginning with Mac");

NameGenerationOptions macOptions = new NameGenerationOptions
{
    MinLength = 5,
    MaxLength = 14,
    MaxAttempts = 500,
    AvoidTrainingDuplicates = true,
    RequiredPrefix = "Mac",
    UseGuidedPrefix = true,
    MaxConsecutiveIdenticalCharacters = 2
};

IReadOnlyList<string> macNames = library.GenerateMany(
    "scottish",
    "clans",
    count: 10,
    seed: 4242,
    macOptions);

foreach (string name in macNames)
{
    Console.WriteLine(name);
}

Console.WriteLine();
Console.WriteLine("Guided suffix example: Roman-style names ending in us");

NameGenerationOptions romanSuffixOptions = new NameGenerationOptions
{
    MinLength = 5,
    MaxLength = 14,
    MaxAttempts = 500,
    AvoidTrainingDuplicates = true,
    RequiredSuffix = "us",
    UseGuidedSuffix = true,
    MaxConsecutiveIdenticalCharacters = 2
};

IReadOnlyList<string> romanUsNames = library.GenerateMany(
    "roman",
    "people",
    count: 10,
    seed: 5151,
    romanSuffixOptions);

foreach (string name in romanUsNames)
{
    Console.WriteLine(name);
}

Console.WriteLine();
Console.WriteLine("Token-based guided prefix example:");

NameGenerationOptions tokenPrefixOptions = new NameGenerationOptions
{
    MinLength = 5,
    MaxLength = 14,
    MaxAttempts = 1000,
    AvoidTrainingDuplicates = true,
    RequiredPrefix = "Mac",
    UseGuidedPrefix = true,
    MaxConsecutiveIdenticalCharacters = 2
};

IReadOnlyList<string> tokenMacNames = tokenGenerator.GenerateMany(
    count: 10,
    options: tokenPrefixOptions);

foreach (string name in tokenMacNames)
{
    Console.WriteLine(name);
}

Console.WriteLine();
Console.WriteLine("Data-file culture profile example:");

string dataDirectory = Path.Combine(AppContext.BaseDirectory, "Data");
string gaelicProfilePath = Path.Combine(dataDirectory, "gaelic.profile.json");
string romanProfilePath = Path.Combine(dataDirectory, "roman.profile.json");

TokenNameModelLibrary gaelicTokenLibrary =
    NameCultureProfileJsonLoader.TrainTokenLibraryFromProfileFile(gaelicProfilePath);

TokenNameModelLibrary romanTokenLibrary =
    NameCultureProfileJsonLoader.TrainTokenLibraryFromProfileFile(romanProfilePath);

NameGenerationOptions dataFileOptions = new NameGenerationOptions
{
    MinLength = 4,
    MaxLength = 14,
    MaxAttempts = 1000,
    AvoidTrainingDuplicates = true,
    MaxConsecutiveIdenticalCharacters = 2
};

Console.WriteLine();
Console.WriteLine("Gaelic people from JSON:");

foreach (string name in gaelicTokenLibrary.GenerateMany(
             "gaelic",
             "people",
             count: 10,
             seed: 1001,
             dataFileOptions))
{
    Console.WriteLine(name);
}

Console.WriteLine();
Console.WriteLine("Gaelic clans from JSON, guided prefix Mac:");

NameGenerationOptions macDataFileOptions = new NameGenerationOptions
{
    MinLength = 5,
    MaxLength = 16,
    MaxAttempts = 1000,
    AvoidTrainingDuplicates = true,
    RequiredPrefix = "Mac",
    UseGuidedPrefix = true,
    MaxConsecutiveIdenticalCharacters = 2
};

foreach (string name in gaelicTokenLibrary.GenerateMany(
             "gaelic",
             "clans",
             count: 10,
             seed: 1002,
             macDataFileOptions))
{
    Console.WriteLine(name);
}

Console.WriteLine();
Console.WriteLine("Roman people from JSON, guided suffix us:");

NameGenerationOptions romanDataFileOptions = new NameGenerationOptions
{
    MinLength = 5,
    MaxLength = 14,
    MaxAttempts = 1000,
    AvoidTrainingDuplicates = true,
    RequiredSuffix = "us",
    UseGuidedSuffix = true,
    MaxConsecutiveIdenticalCharacters = 2
};

foreach (string name in romanTokenLibrary.GenerateMany(
             "roman",
             "people",
             count: 10,
             seed: 1003,
             romanDataFileOptions))
{
    Console.WriteLine(name);
}

Console.WriteLine();
Console.WriteLine("Multi-culture JSON profile set:");

string profileSetPath = Path.Combine(dataDirectory, "cultures.profile-set.json");

TokenNameModelLibrary profileSetLibrary =
    NameCultureProfileJsonLoader.TrainTokenLibraryFromProfileSetFile(profileSetPath);

foreach (string cultureKey in profileSetLibrary.CultureKeys)
{
    Console.WriteLine($"Culture: {cultureKey}");

    foreach (string categoryKey in profileSetLibrary.GetCategoryKeys(cultureKey))
    {
        Console.WriteLine($"  Category: {categoryKey}");
    }
}