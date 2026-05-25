#!/usr/bin/env sh
set -eu

mod_dir="${1:-${STS2_PATH:-}/mods/EZMicroBalance}"
handoff_path="${2:-docs/private-beta-verification-handoff.md}"

if [ -z "$mod_dir" ] || [ ! -d "$mod_dir" ]; then
    fallback="$HOME/Library/Application Support/Steam/steamapps/common/Slay the Spire 2/mods/EZMicroBalance"
    if [ -d "$fallback" ]; then
        mod_dir="$fallback"
    else
        echo "Could not locate the Spire Plus compatibility install directory (EZMicroBalance)." >&2
        echo "Usage: scripts/check-installed-spire-plus-package.sh <mod-dir> [handoff-path]" >&2
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

check_pck_contains() {
    label="$1"
    fragment="$2"
    if grep -aF -- "$fragment" "$pck_path" >/dev/null 2>&1; then
        echo "PCK content $label | PASS"
    else
        echo "PCK content $label | MISSING | FAIL"
        status=1
    fi
}

check_pck_absent() {
    fragment="$1"
    if grep -aF -- "$fragment" "$pck_path" >/dev/null 2>&1; then
        echo "PCK forbidden stale fragment found: $fragment | FAIL"
        status=1
    fi
}

check_sere_talon_imported_textures() {
    imported_count="$(
        grep -ao 'sere_talon_spire_plus\.png-[A-Fa-f0-9][A-Fa-f0-9]*\.ctex' "$pck_path" 2>/dev/null |
            sort -u |
            wc -l |
            tr -d '[:space:]'
    )"

    if [ "${imported_count:-0}" -ge 2 ]; then
        echo "PCK content Sere Talon imported small/big textures | PASS"
    else
        echo "PCK content Sere Talon imported small/big textures | found:${imported_count:-0} | FAIL"
        status=1
    fi
}

echo "Checking installed Spire Plus compatibility artifacts at: $mod_dir"
echo "Using expected hashes from: $handoff_path"

status=0
check_file "EZMicroBalance.dll" "DLL" || status=1
check_file "EZMicroBalance.json" "Manifest" || status=1
check_file "EZMicroBalance.pck" "PCK" || status=1
check_file "README_INSTALL.txt" "README_INSTALL" || status=1

pck_path="$mod_dir/EZMicroBalance.pck"
if [ -f "$pck_path" ]; then
    check_pck_contains "Sere Talon EN title" "\"SERE_TALON.title\": \"Vakuu's Sere Talon\""
    check_pck_contains "Sere Talon EN effect" "\"SERE_TALON.description\": \"On pickup, choose [blue]1[/blue] of [blue]4[/blue] Curses. Add it, [blue]2[/blue] Wish, and [blue]1[/blue] Wish+ to your deck.\""
    check_pck_contains "Sere Talon ZHS title" "\"SERE_TALON.title\": \"瓦库原初之爪\""
    check_pck_contains "Sere Talon ZHS effect" "\"SERE_TALON.description\": \"拾取时，从[blue]4[/blue]张诅咒中选择[blue]1[/blue]张。将它、[blue]2[/blue]张[gold]许愿[/gold]和[blue]1[/blue]张[gold]许愿+[/gold]加入你的牌组。\""
    check_pck_contains "Sere Talon owned icon" "sere_talon_spire_plus.png"
    check_pck_contains "Sere Talon small import" "EZMicroBalance/images/relics/sere_talon_spire_plus.png.import"
    check_pck_contains "Sere Talon big import" "EZMicroBalance/images/relics/big/sere_talon_spire_plus.png.import"
    check_pck_contains "Tanx Claws EN title" "\"CLAWS.title\": \"Tanx Claws\""
    check_pck_contains "Tanx Claws EN effect" "\"CLAWS.description\": \"On pickup, transform up to [blue]{Cards}[/blue] cards into upgraded Maul.\""
    check_pck_contains "Tanx Claws ZHS title" "\"CLAWS.title\": \"坦克斯利爪\""
    check_pck_contains "Tanx Claws ZHS effect" "\"CLAWS.description\": \"拾取时，将至多[blue]{Cards}[/blue]张牌变化为撕咬+。\""
    check_sere_talon_imported_textures

    check_pck_absent "\"SERE_TALON.description\": \"claws.png\""
    check_pck_absent "\"SERE_TALON.title\": \"利爪\""
    check_pck_absent "\"CLAWS.title\": \"利爪\""
    check_pck_absent "\"SERE_TALON.description\": \"拾取时，将至多[blue]{Cards}[/blue]张牌变化为撕咬。"
    check_pck_absent "\"CLAWS.description\": \"拾取时，将[blue]2[/blue]张随机诅咒"
    check_pck_absent "Sere Talon\", \"CLAWS.description\""
    check_pck_absent "Vakuu's Sere Talon\", \"CLAWS.description\""
    check_pck_absent "瓦库原初之爪\", \"CLAWS.description\""
    check_pck_absent "\"CLAWS.description\": \"Choose 1 of 4 Curses"
else
    echo "PCK content Sere Talon / Tanx Claws split | PCK missing | FAIL"
    status=1
fi

if [ "$status" -eq 0 ]; then
    echo "PASS: installed Spire Plus compatibility artifacts, Sere Talon imported textures, and Sere Talon / Tanx Claws PCK content match handoff."
else
    echo "FAIL: one or more installed Spire Plus compatibility artifact hashes, Sere Talon imported texture checks, or Sere Talon / Tanx Claws PCK checks did not match handoff." >&2
fi

exit "$status"
