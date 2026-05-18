using WoadNameGen;

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
    AvoidTrainingDuplicates = true
};

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