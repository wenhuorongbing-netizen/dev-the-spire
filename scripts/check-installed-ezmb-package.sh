#!/usr/bin/env sh
set -eu

mod_dir="${1:-${STS2_PATH:-}/mods/EZMicroBalance}"
handoff_path="${2:-docs/private-beta-verification-handoff.md}"

if [ -z "$mod_dir" ] || [ ! -d "$mod_dir" ]; then
    fallback="$HOME/Library/Application Support/Steam/steamapps/common/Slay the Spire 2/mods/EZMicroBalance"
    if [ -d "$fallback" ]; then
        mod_dir="$fallback"
    else
        echo "Could not locate EZMicroBalance install directory." >&2
        echo "Usage: scripts/check-installed-ezmb-package.sh <mod-dir> [handoff-path]" >&2
        exit 1
    fi
fi

if [ ! -f "$handoff_path" ]; then
    echo "Handoff file not found: $handoff_path" >&2
    exit 1
fi

hash_file() {
    file="$1"
    if command -v shasum >/dev/null 2>&1; then
        shasum -a 256 "$file" | awk '{ print toupper($1) }'
    elif command -v sha256sum >/dev/null 2>&1; then
        sha256sum "$file" | awk '{ print toupper($1) }'
    else
        echo "Neither shasum nor sha256sum is available." >&2
        exit 1
    fi
}

expected_hash() {
    label="$1"
    sed -n "s/^- ${label} SHA256: \`\([A-Fa-f0-9][A-Fa-f0-9]*\)\`.*$/\1/p" "$handoff_path" |
        head -n 1 |
        tr '[:lower:]' '[:upper:]'
}

check_file() {
    file_name="$1"
    label="$2"
    expected="$(expected_hash "$label")"
    file_path="$mod_dir/$file_name"

    if [ -z "$expected" ]; then
        echo "$file_name | expected:<missing> | FAIL"
        return 1
    fi

    if [ ! -f "$file_path" ]; then
        echo "$file_name | MISSING | expected:$expected | FAIL"
        return 1
    fi

    actual="$(hash_file "$file_path")"
    if [ "$actual" = "$expected" ]; then
        echo "$file_name | expected:$expected | actual:$actual | PASS"
        return 0
    fi

    echo "$file_name | expected:$expected | actual:$actual | FAIL"
    return 1
}

echo "Checking installed EZMicroBalance artifacts at: $mod_dir"
echo "Using expected hashes from: $handoff_path"

status=0
check_file "EZMicroBalance.dll" "DLL" || status=1
check_file "EZMicroBalance.json" "Manifest" || status=1
check_file "EZMicroBalance.pck" "PCK" || status=1
check_file "README_INSTALL.txt" "README_INSTALL" || status=1

if [ "$status" -eq 0 ]; then
    echo "PASS: installed EZMicroBalance artifacts match handoff hashes."
else
    echo "FAIL: one or more installed artifact hashes do not match handoff." >&2
fi

exit "$status"
