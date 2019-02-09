#!/bin/bash
set -e

find . -type f -name '*' ! -path './.git/*' ! -name '*.dll' ! -path '*/obj/*' ! -path '*/bin/*' -exec sed -i -e 's/\r\+$//' {} \;
