# Changelog

All notable changes to RomForge are documented here. This project follows [Semantic Versioning](https://semver.org).

## [Unreleased]

## [0.1.0] — 2026-06-23

Initial release.

### Added

- Import and manage OfflineList-format DAT files (ZIP-wrapped or raw XML)
- Scan ROM folders and match against DAT entries by CRC32
- Visual match status per game: Verified, Missing, Incorrectly Named, Wrong Archive Type, Untrimmed
- Sortable columns (release number, title, publisher, status) and per-status filter checkboxes
- Rename ROMs to DAT-expected filenames
- Re-archive ROMs between ZIP and 7z formats
- Trim ROMs (GBA/NDS padding removal)
- Auto-update DAT files from their configured update URL
- Scan cache keyed by folder path — only re-hashes files that have changed
- Status persistence via SQLite — game list survives restarts without re-scanning
- Multi-DAT support — open and switch between multiple DAT files in one session
- Progress dialog with cancellation for all long-running operations
- macOS, Windows, and Linux support via Avalonia UI
