# Shootair

## Overview

This repository contains the source code for the ML Agent Shootair. The project is developed using Python, C# and Unity, and it requires Python and Conda for setup.

## Prerequisites

Before you begin, make sure you have the following installed on your machine:

- [Python + Conda](https://www.anaconda.com/download)

## Installation Process

Follow the steps below to set up the project on your local machine:

1. **Install Unity Hub:**
   - Download and install Unity Hub from the official [Unity website](https://unity3d.com/get-unity/download).

2. **Clone the Project:**
   - Clone this repository to your local machine using Git:
     ```bash
     git clone https://github.com/nico-byte/shotair.git
     ```

3. **Run Installation Script:** <br>
   - The installation script will automatically clone the Unity [ML Agents](https://github.com/Unity-Technologies/ml-agents) repo into the Packages folder inside the project and setup the python environment named mlagents via conda. Additionally it will check if PyTorch is working as expected.
   - For Windows users:
     - Open Anaconda Prompt.
     - Navigate to the project directory using the `cd` command.
     - Execute the install script:
       ```bash
       install.bat
       ```
   - For Linux users:
     - Open a terminal.
     - Navigate to the project directory using the `cd` command.
     - Make the install script executable (if needed):
       ```bash
       chmod +x install.sh
       ```
     - Execute the install script:
       ```bash
       ./install.sh
       ```

5. **Open Project in Unity Hub:**
   - Open Unity Hub.
   - Click on the "Add" button to add the project folder.
   - Select the project folder you cloned in step 2.

6. **Install Unity Editor:**
   - When prompted, Unity Hub will detect the required Unity Editor version.
   - Click on "Install" to download and install the specified Unity Editor version.

## Usage

After completing the installation process, you can now open and work on the project using Unity Hub. Make sure to check for any additional project-specific instructions in the project documentation.

Feel free to reach out if you encounter any issues or have questions!
