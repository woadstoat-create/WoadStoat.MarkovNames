---
name: Release checklist
about: Checklist for publishing a WoadStoat.MarkovNames release
title: "Release vX.Y.Z"
labels: release
---

## Version

- [ ] Version updated in `.csproj`
- [ ] Release notes updated
- [ ] README checked
- [ ] Docs checked

## Build

- [ ] `dotnet format WoadStoat.MarkovNames.slnx`
- [ ] `dotnet build WoadStoat.MarkovNames.slnx -c Release`
- [ ] `dotnet test WoadStoat.MarkovNames.slnx -c Release`
- [ ] `dotnet pack ...`

## Package

- [ ] `.nupkg` created
- [ ] `.snupkg` created
- [ ] Package contents inspected
- [ ] Local package install tested

## Publish

- [ ] API key available locally
- [ ] Published manually
- [ ] Git tag created
- [ ] GitHub release created