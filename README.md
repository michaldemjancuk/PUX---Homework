# 1. [PUX---Homework](https://github.com/michaldemjancuk/PUX---Homework/tree/main)

## 1.1 Základní popis projektu

Jedná se o C# REST API představující zjednodušenou implementaci verzovacího systému obdobného Gitu (implementuje pouze jeho nejzákladnější funkcionalitu).

### Projekt sleduje

- Nové soubory (`0`)
- Upravené soubory (`1`)
- Smazané soubory (`2`)
- Nezměněné soubory (`3`)

---

## 1.2 Základní popis funkcionality

Po zavolání endpointu `GET /Home/Index` s nepovinným parametrem (výchozí hodnota: `C:\Temp\`) API vrátí JSON obsahující všechny změny, ke kterým došlo v daném adresáři od posledního zavolání endpointu.

### Vzorová JSON odpověď

<details>
<summary>Zobrazit JSON odpověď</summary>

```json
{
  "itemsCount": 7,
  "items": [
    {
      "path": "C:\\Temp\\ProjectTest\\changing.txt",
      "fileStatus": 3,
      "version": 5
    },
    {
      "path": "C:\\Temp\\ProjectTest\\empty.txt",
      "fileStatus": 3,
      "version": 1
    },
    {
      "path": "C:\\Temp\\ProjectTest\\1\\1.txt",
      "fileStatus": 3,
      "version": 1
    },
    {
      "path": "C:\\Temp\\ProjectTest\\1\\1\\11.txt",
      "fileStatus": 3,
      "version": 1
    },
    {
      "path": "C:\\Temp\\ProjectTest\\1",
      "fileStatus": 3,
      "version": 1
    },
    {
      "path": "C:\\Temp\\ProjectTest\\2",
      "fileStatus": 3,
      "version": 1
    },
    {
      "path": "C:\\Temp\\ProjectTest\\1\\1",
      "fileStatus": 3,
      "version": 1
    }
  ]
}
```

</details>

---

## 1.3 Úložiště dat

### Aplikační konfigurace

Aplikace ukládá data pomocí JSON souboru, jehož cesta je konfigurovatelná prostřednictvím `appsettings.json`, konkrétně položkou `FileComparer:SnapshotFilePath`.

Výchozí konfigurace:

```json
{
  "FileComparer": {
    "SnapshotFilePath": "data.json"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Aplikační data

Aplikace ukládá stav souborového systému z poslední kontroly.

Ukázka uložených dat:

```json
{
  "Items": [
    {
      "FilePath": "C:\\Temp\\ProjectTest\\changing.txt",
      "Md5Hash": "c06db68e819be6ec3d26c6038d8e8d1f",
      "Version": 5,
      "IsFile": true
    },
    {
      "FilePath": "C:\\Temp\\ProjectTest\\empty.txt",
      "Md5Hash": "d41d8cd98f00b204e9800998ecf8427e",
      "Version": 1,
      "IsFile": true
    },
    {
      "FilePath": "C:\\Temp\\ProjectTest\\1\\1.txt",
      "Md5Hash": "c4ca4238a0b923820dcc509a6f75849b",
      "Version": 1,
      "IsFile": true
    },
    {
      "FilePath": "C:\\Temp\\ProjectTest\\1\\1\\11.txt",
      "Md5Hash": "6512bd43d9caa6e02c990b0a82652dca",
      "Version": 1,
      "IsFile": true
    },
    {
      "FilePath": "C:\\Temp\\ProjectTest\\1",
      "Md5Hash": "",
      "Version": 1,
      "IsFile": false
    },
    {
      "FilePath": "C:\\Temp\\ProjectTest\\2",
      "Md5Hash": "",
      "Version": 1,
      "IsFile": false
    },
    {
      "FilePath": "C:\\Temp\\ProjectTest\\1\\1",
      "Md5Hash": "",
      "Version": 1,
      "IsFile": false
    }
  ]
}
```

---

## 1.4 Restrikce aplikace

- Aplikace pracuje s jedním hlavním adresářem definovaným v `appsettings.json`.
- Změna hlavního adresáře není podporována. Pokud uživatel změní sledovaný adresář bez smazání uloženého snapshotu, mohou být soubory z původního adresáře vyhodnoceny jako smazané.
- Aplikace nepodporuje sledování více adresářů současně.
- Aplikace neobsahuje autentizaci ani autorizaci.
- API vrací změny pouze ve strojově čitelném JSON formátu.
- Hodnota `fileStatus` představuje číselnou hodnotu výčtového typu `FileStatusEnum`:

```csharp
namespace PuxHomework.Enums.FileData;

public enum FileStatusEnum
{
    New = 0,
    Updated = 1,
    Deleted = 2,
    Unchanged = 3,
    Unknown = 4
}
```
