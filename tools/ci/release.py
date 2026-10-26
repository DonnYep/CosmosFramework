#!/usr/bin/env python3
"""Package / release consistency checks for CosmosFramework.

Usage:
    python3 tools/ci/release.py verify [--tag v2.0.1]
    python3 tools/ci/release.py notes --tag v2.0.1 > RELEASE_NOTES.md

Only the Python standard library is used, so it runs on a bare runner.
Exit code 0 = all checks passed, 1 = at least one error.
"""

import argparse
import json
import os
import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
PKG_DIR = ROOT / "Assets" / "CosmosFramework"
PKG_JSON = PKG_DIR / "package.json"
README = ROOT / "README.md"
README_EN = ROOT / "README_EN.md"
CHANGELOG = ROOT / "CHANGELOG.md"

REPO = "https://github.com/DonnYep/CosmosFramework"
UPM_SUFFIX = "?path=/Assets/CosmosFramework"
SEMVER = re.compile(r"^\d+\.\d+\.\d+(?:-[0-9A-Za-z.-]+)?$")
GIT_URL = re.compile(r"https://github\.com/DonnYep/CosmosFramework\.git[^\s\"'<>),]*")
SECTION = re.compile(r"^##\s+\[(?P<version>[^\]]+)\]")
LINK_DEF = re.compile(r"^\[[^\]]+\]:\s")

errors = []
warnings = []


def fail(message):
    errors.append(message)


def warn(message):
    warnings.append(message)


def read(path):
    return path.read_text(encoding="utf-8")


def normalize_tag(tag):
    tag = tag.strip()
    return tag[1:] if tag[:1] in ("v", "V") else tag


def changelog_section(version):
    """Return the body of `## [version] ...` from CHANGELOG.md, or None."""
    lines = read(CHANGELOG).splitlines()
    start = None
    for index, line in enumerate(lines):
        match = SECTION.match(line)
        if match and match.group("version") == version:
            start = index
            break
    if start is None:
        return None
    body = []
    for line in lines[start + 1:]:
        if SECTION.match(line):
            break
        if not LINK_DEF.match(line):
            body.append(line)
    return "\n".join(body).strip()


def check_manifest():
    if not PKG_JSON.is_file():
        fail("missing Assets/CosmosFramework/package.json")
        return None
    try:
        manifest = json.loads(read(PKG_JSON))
    except json.JSONDecodeError as exc:
        fail("package.json is not valid JSON: %s" % exc)
        return None

    for key in ("name", "displayName", "version", "unity"):
        if not manifest.get(key):
            fail("package.json: missing required field '%s'" % key)

    version = str(manifest.get("version", ""))
    if not SEMVER.match(version):
        fail("package.json: version '%s' is not SemVer (expected X.Y.Z)" % version)

    for key in ("licensesUrl", "changelogUrl", "documentationUrl"):
        url = manifest.get(key)
        if not url:
            warn("package.json: '%s' is not set" % key)
            continue
        if not url.startswith(REPO + "/blob/main/"):
            fail("package.json: '%s' should point at %s/blob/main/... (got %s)" % (key, REPO, url))
            continue
        target = url[len(REPO + "/blob/main/"):]
        if not (ROOT / target).is_file():
            fail("package.json: '%s' points at a missing file: %s" % (key, target))

    return manifest


def check_readme_urls(path):
    text = read(path)
    urls = GIT_URL.findall(text)
    if not urls:
        warn("%s: no UPM git URL found" % path.name)
        return []
    for url in urls:
        if UPM_SUFFIX not in url:
            fail("%s: UPM url must use '?path=/Assets/CosmosFramework' (got %s)" % (path.name, url))
        if "#" not in url:
            fail("%s: UPM url must pin a ref after '#' (got %s)" % (path.name, url))
    return urls


def check_layout():
    for name in ("Runtime", "Editor"):
        if not (PKG_DIR / name).is_dir():
            fail("package layout: missing %s/" % name)
    if not list(PKG_DIR.rglob("*.asmdef")):
        fail("package layout: no .asmdef found under Assets/CosmosFramework")


def verify(tag):
    manifest = check_manifest()
    if not CHANGELOG.is_file():
        fail("missing CHANGELOG.md")
        changelog_ok = False
    else:
        changelog_ok = True

    urls = check_readme_urls(README)
    urls += check_readme_urls(README_EN)
    check_layout()

    version = str((manifest or {}).get("version", ""))
    if changelog_ok and version and SEMVER.match(version):
        if changelog_section(version) is None:
            fail("CHANGELOG.md: no '## [%s]' section for the current package version" % version)

    if tag:
        expected = normalize_tag(tag)
        if version != expected:
            fail("tag %s does not match package.json version %s" % (tag, version))
        if changelog_ok and changelog_section(expected) is None:
            fail("CHANGELOG.md: missing '## [%s]' section for tag %s" % (expected, tag))
        pinned = [u for u in urls if u.endswith("#" + tag) or u.endswith("#" + expected)]
        if version and not pinned:
            warn("README does not pin the released tag yet; consider updating '#...' to '%s'" % (tag if tag.startswith("v") else "v" + tag))

    report()


def report():
    for message in warnings:
        print("[warn] %s" % message)
        if os.environ.get("GITHUB_ACTIONS") == "true":
            print("::warning::%s" % message)

    for message in errors:
        print("[error] %s" % message)
        if os.environ.get("GITHUB_ACTIONS") == "true":
            print("::error::%s" % message)

    if errors:
        print("\nFAILED: %d error(s), %d warning(s)" % (len(errors), len(warnings)))
        sys.exit(1)
    print("OK: package, README and CHANGELOG are consistent (%d warning(s))" % len(warnings))


def notes(tag, output=None):
    if not tag:
        print("notes requires --tag", file=sys.stderr)
        sys.exit(2)
    version = normalize_tag(tag)
    if not CHANGELOG.is_file():
        print("missing CHANGELOG.md", file=sys.stderr)
        sys.exit(2)
    body = changelog_section(version)
    if body is None:
        print("CHANGELOG.md has no section for %s" % version, file=sys.stderr)
        sys.exit(2)
    text = "\n".join([
        "<!-- Release notes generated from CHANGELOG.md by tools/ci/release.py -->",
        "",
        body,
        "",
        "**Full changelog:** %s/blob/main/CHANGELOG.md" % REPO,
        "",
    ])
    if output:
        Path(output).write_text(text, encoding="utf-8")
        print("wrote %s" % output)
    else:
        sys.stdout.write(text)


def main():
    for stream in (sys.stdout, sys.stderr):
        try:
            stream.reconfigure(encoding="utf-8")
        except (AttributeError, ValueError):
            pass

    parser = argparse.ArgumentParser(description="CosmosFramework release checks")
    sub = parser.add_subparsers(dest="command", required=True)

    verify_parser = sub.add_parser("verify", help="check package.json, README and CHANGELOG")
    verify_parser.add_argument("--tag", default="", help="release tag to validate against, e.g. v2.0.1")

    notes_parser = sub.add_parser("notes", help="print GitHub Release notes from CHANGELOG.md")
    notes_parser.add_argument("--tag", required=True)
    notes_parser.add_argument("--output", default="", help="write notes to this file instead of stdout")

    args = parser.parse_args()
    if args.command == "verify":
        verify(args.tag)
    else:
        notes(args.tag, args.output or None)


if __name__ == "__main__":
    main()
