# Holiday Data

Dateview uses bundled offline holiday files under `assets/holidays/cn/{year}.json`.

## Policy

- Use official yearly public holiday notices as the source of truth.
- Do not infer adjusted workdays from formulas.
- Keep source metadata in every yearly JSON file.
- Validate ISO dates, duplicate dates, known day types, jurisdiction, and year consistency.
- Treat missing future-year data as a visible product state instead of invented data.

## MVP Coverage

- 2025: planned bundled data.
- 2026: planned bundled data.
- 2027: placeholder only after official data exists.

## Day Types

`dayOff`

- A public holiday or adjusted rest day.
- Visual priority is higher than a normal weekday.
- Later UI should show a non-color-only badge such as `休`.

`adjustedWorkday`

- A weekend day that is officially adjusted to a workday.
- Visual priority is higher than the normal weekend marker.
- Later UI should show a non-color-only badge such as `班`.

`festivalOnly`

- Reserved for informational festival labels that do not change work/rest status.
- It must not be treated as a day off or adjusted workday.
- MVP parser may accept it, but MVP data should use it only when the source explicitly supports the meaning.

## Schema

```json
{
  "schemaVersion": 1,
  "jurisdiction": "CN",
  "year": 2026,
  "source": {
    "title": "Official notice title",
    "publishedDate": "yyyy-MM-dd",
    "url": "https://example.invalid/official-notice"
  },
  "days": [
    { "date": "2026-01-01", "type": "dayOff", "name": "Yuan Dan" },
    { "date": "2026-01-04", "type": "adjustedWorkday", "name": "Yuan Dan adjusted workday" }
  ]
}
```

## Field Rules

- `schemaVersion`: required integer; MVP supports only `1`.
- `jurisdiction`: required string; MVP supports only `CN`.
- `year`: required integer; must match the file name and every `days[].date` year.
- `source.title`: required non-empty string.
- `source.publishedDate`: required ISO date string, `yyyy-MM-dd`.
- `source.url`: optional non-empty string when present. Use an official source URL when available.
- `days`: required array. Empty is allowed only when a yearly official source is intentionally represented with no known entries.
- `days[].date`: required ISO date string, `yyyy-MM-dd`.
- `days[].type`: required string; must be `dayOff`, `adjustedWorkday`, or `festivalOnly`.
- `days[].name`: required non-empty string.

## Validation Rules

- Reject invalid dates instead of silently skipping them.
- Reject duplicate `days[].date` entries.
- Reject unknown day types.
- Reject unsupported jurisdictions.
- Reject year mismatches between file name, `year`, and `days[].date`.
- Do not fetch network data during app startup.
- Do not generate future adjusted workdays without official data.
