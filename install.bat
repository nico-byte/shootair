@echo off

:: Activate conda environment
call conda.bat activate base

:: Create and activate mlagents environment
call conda.bat create --name mlagents python=3.10.12 -y
call conda.bat activate mlagents

:: Install requirements
call pip install -r requirements.txt

:: Check Python version and conda list for torch
call python --version
call conda.bat list torch

:: Clone ml-agents repository
cd Packages
git clone --branch release_21 https://github.com/Unity-Technologies/ml-agents.git
cd ml-agents

:: Install ml-agents-envs and ml-agents
call python -m pip install ./ml-agents-envs
call python -m pip install ./ml-agents
cd ../../

:: Display conda env
call conda.bat list

:: Run check_pt.py
call python check_pt.py