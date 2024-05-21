import os
import re


def update_readme():
    """
    Function to update the README.md file with the latest version number
    """
    # Get the current version number
    root_dir = os.getenv("GITHUB_WORKSPACE")
    tag_val = os.getenv("TAG_VALUE")
    tag_without_v = tag_val[1:] if tag_val.lower().startswith("v") else tag_val
    version = os.getenv("VERSION_NUM")

    os.chdir(root_dir)

    # Read the README.md file
    with open("README.md", "r") as file:
        readme = file.read()

    # Replace the old version number with the new version number
    readme = re.sub(r"\d+\.\d+\.\d+-beta", tag_without_v, readme)
    readme = re.sub(r"\d+\.\d+\.\d+", version, readme)

    # Write the updated README.md file
    with open("README.md", "w") as file:
        file.write(readme)

    print(f"Updated README.md with version {version}")


if __name__ == "__main__":
    update_readme()
