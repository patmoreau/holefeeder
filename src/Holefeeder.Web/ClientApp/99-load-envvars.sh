#!/usr/bin/env sh
set -eu

jq -n env | grep -E '\{|\}|NGX_' | sed ':begin;$!N;s/,\n}/\n}/g;tbegin;P;D' > /usr/share/nginx/html/assets/config/_ngx-rtconfig.json

exec "$@"
