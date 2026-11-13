# Patch notes — DepotDownloaderMod 3.4.0.3 (DDM-GUI fork)

- Improved large-manifest validation by indexing prior-manifest files and chunks rather than repeatedly searching them.
- Replaced byte-at-a-time checksum reads with pooled, buffered reads.
- Added automatic validation concurrency selection on Windows: one worker for drives reporting a rotational seek penalty, and the configured download concurrency for SSD/NVMe or unknown drives.
- Added `-max-validators <n>` to override the automatic validation worker count (`0` means automatic).
- Added optional cumulative progress telemetry for DDM-GUI via `DEPOTDROP_TELEMETRY=1`.
- Published a self-contained Windows x64 executable; no separate .NET runtime installation is required for this executable.

## Notes

- Validation worker selection is based on the storage type reported by Windows. If that information is unavailable, the program falls back to `-max-downloads` workers.
- Telemetry validation bytes count logical checksum reads, which may be satisfied from the OS cache; they do not represent guaranteed physical disk I/O.
