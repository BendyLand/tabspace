# tabspace

A command-line tool for converting between tabs and spaces in source files. Written in F# targeting .NET 8.

## Features

- Convert tabs to spaces
- Convert spaces to tabs
- Strip trailing whitespace
- Auto-detect indentation style and convert to the opposite

## Building

**Run directly with .NET:**
```sh
dotnet run -- <file> [flags]
```

**Build a native self-contained binary (Linux x64):**
```sh
dotnet publish -r linux-x64 -p:PublishSingleFile=true -p:PublishTrimmed=true
mv bin/Release/net8.0/linux-x64/publish/tabspace .
```

After building, the `tabspace` binary can be placed anywhere on your `PATH`.

## Usage

```
tabspace <file> [flags]
```

The file is modified in-place.

### Flags

| Flag | Description |
|------|-------------|
| `-tab` | Convert spaces to tabs |
| `-space` | Convert tabs to spaces |
| `-strip` | Strip trailing whitespace only |
| `-N` | Spaces per tab (e.g. `-2`, `-4`). Default: `4` |

Flag matching is case-insensitive and substring-based — `-tabs`, `-totabs`, `-TAB` all work.

### Auto-detection

If no conversion flag is given, `tabspace` inspects the file and picks the conversion automatically:

- If most indented lines start with tabs → converts to spaces
- If most indented lines start with spaces → converts to tabs
- If counts are equal → converts to tabs

### Examples

```sh
# Auto-detect and convert
tabspace main.c

# Convert tabs to 2-space indentation
tabspace main.c -space -2

# Convert spaces to tabs (4 spaces = 1 tab)
tabspace main.c -tab -4

# Strip trailing whitespace only
tabspace main.c -strip
```

## How it works

Conversion operates line-by-line and only touches leading whitespace — the content of each line is preserved. Blank lines are left unchanged.

- **Tabs → spaces:** counts leading tabs, replaces them with `N` spaces each
- **Spaces → tabs:** counts leading spaces, divides by `N`, replaces with that many tabs
- **Strip:** trims trailing spaces and tabs from every non-blank line

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download) (to build)
- Linux x64 (for the prebuilt native binary target; other targets can be specified via `-r`)
