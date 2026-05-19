# JSON Profile Format

WoadNameGen can train name libraries from JSON culture profiles.

A profile describes:

- the culture key
- the Markov order
- whether tokenisation should be used
- optional tokens
- name categories
- training samples for each category

---

## Single Culture Profile

```json
{
  "cultureKey": "gaelic",
  "order": 2,
  "useTokens": true,
  "tokens": ["mac", "dh", "ch", "ae", "eo"],
  "categories": {
    "people": ["Aedan", "Caelan", "Eoghan"],
    "clans": ["MacLeod", "MacDonald", "MacGregor"]
  }
}
```

---

## Fields

### `cultureKey`

A unique key for the culture.

Example:

```json
"cultureKey": "gaelic"
```

Use lowercase kebab-case or simple lowercase names where possible.

Examples:

```text
gaelic
roman
norse
fantasy-elven
alien-harsh
corporate
```

---

### `order`

The Markov order.

```json
"order": 2
```

Recommended values:

```text
1 = loose and chaotic
2 = balanced default
3 = closer to source material
4+ = likely to overfit small datasets
```

---

### `useTokens`

Whether the profile should use token-based generation.

```json
"useTokens": true
```

If false, the character-based generation path is used.

---

### `tokens`

A list of custom tokens.

```json
"tokens": ["mac", "dh", "ch", "ae", "eo"]
```

Tokens are useful for clusters that should be treated as one unit.

Examples:

```text
mac
mc
ch
dh
gh
ae
eo
kh
zh
ll
th
qu
```

---

### `categories`

A dictionary of category keys to training samples.

```json
"categories": {
  "people": ["Aedan", "Caelan", "Eoghan"],
  "places": ["Inverness", "Dunblane", "Mallaig"]
}
```

Common category keys:

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

---

## Multi-Culture Profile Set

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

All profiles in the same profile set currently need to use the same Markov order.

---

## Loading from File

```csharp
TokenNameModelLibrary library =
    NameCultureProfileJsonLoader.TrainTokenLibraryFromProfileFile(
        "Data/gaelic.profile.json");
```

---

## Loading from String

```csharp
TokenNameModelLibrary library =
    NameCultureProfileJsonLoader.TrainTokenLibraryFromProfileJson(json);
```