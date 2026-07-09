#!/bin/bash

git lfs install

dotnet dev-certs https --trust

sudo dotnet workload restore

sudo dotnet workload update

dotnet restore
