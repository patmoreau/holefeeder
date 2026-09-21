#!/usr/bin/env bash
#
# Generates the per-assistant command files from docs/contributing/tasks/**/*.md.
#
#   scripts/sync-agent-files.sh           regenerate the files in place
#   scripts/sync-agent-files.sh --check   fail if any generated file is stale
#
# Each source file carries YAML front matter with `name` and `description`; the
# body below it is copied verbatim into every target. Edit the source, never a
# generated file.
#
# Sources live one directory deep, and that directory is the category:
# docs/contributing/tasks/backend/deploy-staging.md becomes
# .claude/commands/backend/deploy-staging.md (Claude namespaces commands by
# folder) and .github/prompts/deploy-staging.prompt.md (Copilot prompts are flat).
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
SRC_DIR="$ROOT/docs/contributing/tasks"

CHECK=0
if [[ "${1:-}" == "--check" ]]; then
    CHECK=1
elif [[ $# -gt 0 ]]; then
    echo "usage: $(basename "$0") [--check]" >&2
    exit 2
fi

OUT_ROOT="$ROOT"
if [[ $CHECK -eq 1 ]]; then
    OUT_ROOT="$(mktemp -d)"
    trap 'rm -rf "$OUT_ROOT"' EXIT
fi

front_matter_value() {
    awk -v key="$2" '
        NR == 1 && $0 == "---" { fm = 1; next }
        fm == 1 && $0 == "---" { exit }
        fm == 1 {
            idx = index($0, ":")
            if (idx == 0) { next }
            k = substr($0, 1, idx - 1)
            v = substr($0, idx + 1)
            gsub(/^[ \t]+|[ \t]+$/, "", k)
            gsub(/^[ \t]+|[ \t]+$/, "", v)
            if (k == key) { print v; exit }
        }
    ' "$1"
}

body() {
    awk '
        NR == 1 && $0 == "---" { fm = 1; next }
        fm == 1 && $0 == "---" { fm = 2; next }
        fm == 2 {
            if (!started && $0 ~ /^[[:space:]]*$/) { next }
            started = 1
            print
        }
    ' "$1"
}

write_target() {
    # write_target <path> <front-matter-block> <source-relative-path> <source-file>
    local target="$1" front="$2" origin="$3" source="$4"
    mkdir -p "$(dirname "$target")"
    {
        if [[ -n "$front" ]]; then
            printf '%s\n\n' "$front"
        fi
        printf '<!-- Generated from %s by scripts/sync-agent-files.sh. Do not edit. -->\n\n' "$origin"
        body "$source"
    } >"$target"
}

generated=()

shopt -s nullglob
sources=("$SRC_DIR"/*/*.md)
stray=("$SRC_DIR"/*.md)
shopt -u nullglob

if [[ ${#stray[@]} -gt 0 ]]; then
    echo "task sources must sit in a category directory, not directly in ${SRC_DIR#"$ROOT"/}:" >&2
    printf '  %s\n' "${stray[@]#"$ROOT"/}" >&2
    exit 1
fi

if [[ ${#sources[@]} -eq 0 ]]; then
    echo "no task sources found in ${SRC_DIR#"$ROOT"/}" >&2
    exit 1
fi

for source in "${sources[@]}"; do
    category="$(basename "$(dirname "$source")")"
    origin="docs/contributing/tasks/$category/$(basename "$source")"
    name="$(front_matter_value "$source" name)"
    description="$(front_matter_value "$source" description)"

    if [[ -z "$name" || -z "$description" ]]; then
        echo "$origin: front matter must set both 'name' and 'description'" >&2
        exit 1
    fi
    if [[ "$name" != "$(basename "$source" .md)" ]]; then
        echo "$origin: front matter name '$name' does not match the file name" >&2
        exit 1
    fi

    # Claude Code slash command: /<category>:<name>
    write_target "$OUT_ROOT/.claude/commands/$category/$name.md" \
        "---
description: $description
---" "$origin" "$source"
    generated+=(".claude/commands/$category/$name.md")

    # GitHub Copilot prompt file: /<name>
    write_target "$OUT_ROOT/.github/prompts/$name.prompt.md" \
        "---
mode: agent
description: $description
---" "$origin" "$source"
    generated+=(".github/prompts/$name.prompt.md")
done

if [[ $CHECK -eq 0 ]]; then
    printf 'wrote %d files from %d task sources\n' "${#generated[@]}" "${#sources[@]}"
    exit 0
fi

stale=()
for file in "${generated[@]}"; do
    if ! cmp -s "$OUT_ROOT/$file" "$ROOT/$file"; then
        stale+=("$file")
    fi
done

# Flag generated files whose source was deleted or renamed.
shopt -s nullglob
for existing in "$ROOT"/.claude/commands/*/*.md "$ROOT"/.github/prompts/*.prompt.md; do
    relative="${existing#"$ROOT"/}"
    found=0
    for file in "${generated[@]}"; do
        if [[ "$file" == "$relative" ]]; then
            found=1
            break
        fi
    done
    if [[ $found -eq 0 ]]; then
        stale+=("$relative (orphaned: no matching task source)")
    fi
done
shopt -u nullglob

if [[ ${#stale[@]} -gt 0 ]]; then
    echo "::error::Generated assistant command files are out of date. Run scripts/sync-agent-files.sh and commit the result."
    printf '  %s\n' "${stale[@]}"
    exit 1
fi

printf 'all %d generated files are up to date\n' "${#generated[@]}"
