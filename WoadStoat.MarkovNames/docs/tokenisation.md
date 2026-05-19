# Tokenisation

WoadStoat.MarkovNames supports both character-based and token-based generation.

---

## Character-Based Generation

Character-based generation treats each character as a unit.

Example:

```text
MacLeod
```

becomes:

```text
M, a, c, L, e, o, d
```

This is simple and works well for many datasets.

---

## Token-Based Generation

Token-based generation can treat letter clusters as units.

With tokens:

```text
mac
eo
```

the name:

```text
MacLeod
```

becomes:

```text
mac, l, eo, d
```

This helps preserve language or culture-specific patterns.

---

## Greedy Tokeniser

`GreedyNameTokenizer` uses the longest matching token first.

Example:

```csharp
INameTokenizer tokenizer = new GreedyNameTokenizer(new[]
{
    "mac",
    "ae",
    "eo",
    "ch"
});
```

When tokenising:

```text
MacLeod
```

it produces:

```text
mac, l, eo, d
```

---

## Good Token Candidates

Gaelic-inspired:

```text
mac
mc
mh
dh
ch
gh
ae
ai
eo
io
ua
```

Welsh-inspired:

```text
ll
dd
ff
rh
ch
th
wy
ae
```

Roman-inspired:

```text
us
ius
ian
ae
au
qu
um
```

Harsh sci-fi / alien:

```text
kh
zh
xq
vr
sk
th
aa
uun
```

---

## When to Use Tokens

Use tokenisation when:

- important letter clusters are being broken apart
- generated names feel too noisy
- you are modelling a specific culture or conlang
- you want alien/fantasy phonetic consistency

Use character generation when:

- names are simple
- datasets are small
- you want more chaotic variation
- you do not need custom phonetic structure