#!/bin/bash

# Script to rename a .NET solution and its projects
# Usage: ./rename-project.sh OldName NewName
# Example: ./rename-project.sh SpringBoard MyAwesomeProject

set -e

# Function to check if a file is binary
is_binary() {
    if file "$1" | grep -q "text"; then
        return 1  # Not binary (text file)
    else
        return 0  # Binary file
    fi
}

# Function to check if a file is ignored by git
is_git_ignored() {
    if git check-ignore -q "$1" 2>/dev/null; then
        return 0  # File is ignored
    else
        return 1  # File is not ignored
    fi
}

# Function to safely update text in a file
safe_replace() {
    local file=$1
    local pattern=$2
    
    # Skip binary files
    if is_binary "$file"; then
        echo "  Skipping binary file: $file"
        return
    fi
    
    # Skip git-ignored files
    if is_git_ignored "$file"; then
        echo "  Skipping git-ignored file: $file"
        return
    fi
    
    sed -i '' "$pattern" "$file"
}

# Check if both arguments are provided
if [ $# -ne 2 ]; then
    echo "Usage: $0 OldName NewName"
    echo "Example: $0 SpringBoard MyAwesomeProject"
    exit 1
fi

OLD_NAME=$1
NEW_NAME=$2

# Validate input - ensure they're not the same
if [ "$OLD_NAME" = "$NEW_NAME" ]; then
    echo "Error: Old name and new name are the same."
    exit 1
fi

# Check if the solution file exists with the old name
if [ -f "${OLD_NAME}.sln" ]; then
    SOLUTION_FILE="${OLD_NAME}.sln"
    echo "Found solution file: $SOLUTION_FILE"
else
    # Try to find any solution file
    SOLUTION_FILE=$(find . -maxdepth 1 -name "*.sln" | head -n 1)
    
    if [ -z "$SOLUTION_FILE" ]; then
        echo "Error: No .sln file found in the current directory."
        exit 1
    fi
    
    echo "Found solution file: $SOLUTION_FILE"
    echo "Warning: The solution file name doesn't match the old name provided."
    echo "Press Enter to continue or Ctrl+C to abort..."
    read
fi

echo "Renaming project from '$OLD_NAME' to '$NEW_NAME'"
echo "This will modify files and directories. Please make sure you have a backup."
echo "Press Enter to continue or Ctrl+C to abort..."
read

# 1. Rename directories
echo "Renaming directories..."
for dir in ${OLD_NAME}.*; do
    if [ -d "$dir" ]; then
        # Skip git-ignored directories
        if is_git_ignored "$dir"; then
            echo "  Skipping git-ignored directory: $dir"
            continue
        fi
        
        NEW_DIR="${dir/$OLD_NAME/$NEW_NAME}"
        echo "  $dir -> $NEW_DIR"
        mv "$dir" "$NEW_DIR"
    fi
done

# 2. Rename solution file
echo "Renaming solution file..."
SOLUTION_NAME=$(basename "$SOLUTION_FILE")
NEW_SOLUTION_FILE="${SOLUTION_FILE/$SOLUTION_NAME/${NEW_NAME}.sln}"

if [ "$SOLUTION_FILE" != "$NEW_SOLUTION_FILE" ]; then
    echo "  $SOLUTION_FILE -> $NEW_SOLUTION_FILE"
    mv "$SOLUTION_FILE" "$NEW_SOLUTION_FILE"
    SOLUTION_FILE="$NEW_SOLUTION_FILE"
fi

# 3. Update solution file content
echo "Updating solution file content..."
safe_replace "$SOLUTION_FILE" "s/${OLD_NAME}\./${NEW_NAME}./g"

# 4. Update project files (.csproj)
echo "Updating project files..."
find . -name "*.csproj" -type f | while read -r file; do
    # Skip git-ignored files for renaming
    if is_git_ignored "$file"; then
        echo "  Skipping git-ignored file: $file"
        continue
    fi
    
    echo "  Updating $file"
    # Update project references inside the file first
    safe_replace "$file" "s/${OLD_NAME}\./${NEW_NAME}./g"
    
    # Rename the project file itself if needed
    NEW_FILE="${file/$OLD_NAME/$NEW_NAME}"
    if [ "$file" != "$NEW_FILE" ] && [ -f "$file" ]; then
        echo "  Renaming $file -> $NEW_FILE"
        # Create directory if it doesn't exist
        mkdir -p "$(dirname "$NEW_FILE")"
        mv "$file" "$NEW_FILE"
    fi
done

# 5. Update namespace references and fully qualified type names in C# files
echo "Updating references in C# files..."
find . -name "*.cs" -type f | while read -r file; do
    # Skip git-ignored files
    if is_git_ignored "$file"; then
        echo "  Skipping git-ignored file: $file"
        continue
    fi
    
    echo "  Updating $file"
    # Skip binary files
    if is_binary "$file"; then
        echo "  Skipping binary file: $file"
        continue
    fi
    
    # Update namespace declarations
    safe_replace "$file" "s/namespace ${OLD_NAME}\./namespace ${NEW_NAME}./g"
    
    # Update using statements
    safe_replace "$file" "s/using ${OLD_NAME}\./using ${NEW_NAME}./g"
    
    # Update fully qualified type names (e.g., typeof(OldName.Namespace.Type))
    safe_replace "$file" "s/typeof(${OLD_NAME}\./typeof(${NEW_NAME}./g"
    safe_replace "$file" "s/${OLD_NAME}\./${NEW_NAME}./g"
    
    # Update string literals containing the old name (case-sensitive)
    safe_replace "$file" "s/${OLD_NAME}/${NEW_NAME}/g"
done

# 6. Update AssemblyInfo.cs files if they exist
find . -name "AssemblyInfo.cs" -type f | while read -r file; do
    echo "  Updating assembly info in $file"
    safe_replace "$file" "s/${OLD_NAME}/${NEW_NAME}/g"
done

# 7. Update any JSON files (launchSettings.json, appsettings.json, etc.)
echo "Updating JSON files..."
find . -name "*.json" -type f | while read -r file; do
    # Skip git-ignored files
    if is_git_ignored "$file"; then
        echo "  Skipping git-ignored file: $file"
        continue
    fi
    
    echo "  Updating $file"
    safe_replace "$file" "s/${OLD_NAME}/${NEW_NAME}/g"
done

# 8. Update any XML files (*.config, etc.)
echo "Updating XML files..."
find . -name "*.config" -type f -o -name "*.xml" -type f | while read -r file; do
    # Skip git-ignored files
    if is_git_ignored "$file"; then
        echo "  Skipping git-ignored file: $file"
        continue
    fi
    
    echo "  Updating $file"
    safe_replace "$file" "s/${OLD_NAME}/${NEW_NAME}/g"
done

# 9. Update email addresses and URLs containing the old name (lowercase)
echo "Updating email addresses and URLs..."
OLD_NAME_LOWER=$(echo "$OLD_NAME" | tr '[:upper:]' '[:lower:]')
NEW_NAME_LOWER=$(echo "$NEW_NAME" | tr '[:upper:]' '[:lower:]')



# Find files with lowercase references and update them if they're text files
find . -type f -not -path "*/\.*" | xargs grep -l "$OLD_NAME_LOWER" 2>/dev/null | while read -r file; do
    # Skip git-ignored files
    if is_git_ignored "$file"; then
        echo "  Skipping git-ignored file: $file"
        continue
    fi
    
    # Skip binary files
    if is_binary "$file"; then
        echo "  Skipping binary file: $file"
        continue
    fi
    
    echo "  Updating lowercase references in $file"
    safe_replace "$file" "s/${OLD_NAME_LOWER}/${NEW_NAME_LOWER}/g"
done

echo "Rename complete! Solution and projects have been renamed from '$OLD_NAME' to '$NEW_NAME'."
echo ""
echo "IMPORTANT: You should rebuild your solution to ensure all references are updated correctly."
echo "Run: dotnet build"
echo ""
echo "You may need to reload your IDE to see all changes."
echo ""
echo "Note: While this script handles most renaming tasks, you might need to manually check:"
echo "  - Custom build scripts"
echo "  - Docker files"
echo "  - CI/CD configuration files"
echo "  - Any other files with custom references to the old name"
