#!/usr/bin/env bash
set -euo pipefail

PACKAGE_VERSION="0.1.0"
PACKAGE_ID="WoadStoat.MarkovNames"
PACKAGE_SOURCE="$(pwd)/artifacts/packages"
TEST_DIR="/tmp/woadstoat-markovnames-package-test"

dotnet clean
dotnet build WoadStoat.MarkovNames.slnx -c Release
dotnet test WoadStoat.MarkovNames.slnx -c Release

mkdir -p artifacts/packages

dotnet pack WoadStoat.MarkovNames/src/WoadStoat.MarkovNames/WoadStoat.MarkovNames.csproj \
  -c Release \
  -o artifacts/packages

rm -rf "$TEST_DIR"
mkdir -p "$TEST_DIR"
cd "$TEST_DIR"

dotnet new console

dotnet add package "$PACKAGE_ID" \
  --version "$PACKAGE_VERSION" \
  --source "$PACKAGE_SOURCE"

cat > Program.cs <<'CS'
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

Console.WriteLine(generator.Generate(options));
CS

dotnet run

echo "Local package test completed successfully."