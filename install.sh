#!/bin/bash

# Activate conda
eval "$(conda shell.bash hook)"
# source /opt/miniconda3/etc/profile.d/conda.sh

# Create and activate mlagents environment
conda create --name mlagents python=3.10.12 -y
conda activate mlagents

# Install requirements
pip install -r requirements.txt

# Check Python version and conda list for torch
python --version
conda list torch

# Clone ml-agents repository
cd Packages
git clone --branch release_21 https://github.com/Unity-Technologies/ml-agents.git
cd ml-agents

# Install ml-agents-envs and ml-agents
pip3 install -e ./ml-agents-envs
pip3 install -e ./ml-agents
cd ../
rm -rf ml-agents
cd ../

# Display conda env
conda list

# Run check_pt.py
python3 check_pt.py