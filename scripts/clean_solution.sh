#!/bin/bash
find . -type d \( -name bin -o -name obj \) -print0 | xargs -0 -I '{}' sh -c 'rm -rf "{}"'
