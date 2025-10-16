#!/usr/bin/env sh
set -e

export HUSKY=1
if [ -n "$1" ]; then
  export HUSKY_GIT_PARAMS="$*"
fi
