# Generation Options

`NameGenerationOptions` controls how generated names are filtered and formatted.

---

## Example

```csharp
NameGenerationOptions options = new NameGenerationOptions
{
    MinLength = 4,
    MaxLength = 12,
    AvoidTrainingDuplicates = true,
    MaxAttempts = 1000,
    RequiredPrefix = "Mac",
    UseGuidedPrefix = true,
    MaxConsecutiveIdenticalCharacters = 2
};

options.ForbiddenSubstrings.Add("xxx");
options.ForbiddenCharacters.Add('$');
```

---

## Length

```csharp
MinLength = 4;
MaxLength = 12;
```

Generated names outside this range are rejected.

---

## Attempts

```csharp
MaxAttempts = 1000;
```

Controls how many attempts the generator makes before giving up.

Strict filters may need higher attempts.

---

## Duplicate Avoidance

```csharp
AvoidTrainingDuplicates = true;
```

Rejects names that exactly match the training data.

---

## Formatting

```csharp
CapitaliseFirstLetter = true;
LowercaseRest = true;
```

Controls basic casing.

---

## Required Prefix

```csharp
RequiredPrefix = "Mac";
UseGuidedPrefix = true;
```

When guided prefix is enabled, the generator starts from the prefix.

---

## Required Suffix

```csharp
RequiredSuffix = "us";
UseGuidedSuffix = true;
```

When guided suffix is enabled, the generator appends and validates the suffix.

---

## Forbidden Substrings

```csharp
options.ForbiddenSubstrings.Add("xxx");
options.ForbiddenSubstrings.Add("qq");
```

Rejects any generated name containing those substrings.

---

## Forbidden Characters

```csharp
options.ForbiddenCharacters.Add('$');
options.ForbiddenCharacters.Add('^');
```

Rejects generated names containing those characters.

---

## Allowed Characters

```csharp
foreach (char c in "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ")
{
    options.AllowedCharacters.Add(c);
}
```

If `AllowedCharacters` is empty, all characters are allowed.

If populated, every generated character must be in the allowed set.

---

## Maximum Consecutive Identical Characters

```csharp
MaxConsecutiveIdenticalCharacters = 2;
```

Allows:

```text
ll
```

Rejects:

```text
lll
```

---

## Custom Validator

```csharp
CustomValidator = name => !name.EndsWith("son");
```

Use this for project-specific rules.