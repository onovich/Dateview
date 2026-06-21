# Holiday Data

Dateview uses bundled offline holiday files under `assets/holidays/cn/{year}.json`.

## Policy

- Use official yearly public holiday notices as the source of truth.
- Do not infer adjusted workdays from formulas.
- Keep source metadata in each JSON file.
- Validate ISO dates, duplicate dates, known day types, jurisdiction, and year consistency.
- Treat missing future-year data as a visible product state instead of invented data.

## MVP Coverage

- 2025: planned bundled data.
- 2026: planned bundled data.
- 2027: placeholder only after official data exists.

## Schema Sketch

```json
{
  "schemaVersion": 1,
  "jurisdiction": "CN",
  "year": 2026,
  "source": {
    "title": "Official notice title",
    "publishedDate": "yyyy-MM-dd"
  },
  "days": [
    { "date": "2026-01-01", "type": "dayOff", "name": "元旦" },
    { "date": "2026-01-04", "type": "adjustedWorkday", "name": "元旦调休上班" }
  ]
}
```
