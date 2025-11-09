#!/bin/bash

# Define variables
PROJECT_DIR="$(pwd)"
CONFIG="Release"
FRAMEWORK="net9.0-ios"
RID="ios-arm64"
BUILD_DIR="$PROJECT_DIR/bin/$CONFIG/$FRAMEWORK/$RID"
PAYLOAD_DIR="$BUILD_DIR/Payload"
IPA_NAME="AppUnsigned.ipa"

echo "--- Starting Unsigned MAUI Build ---"

# Step 1: Run dotnet build without signing or archiving
# We use 'build' instead of 'publish' to get the .app bundle ready for manual packaging
dotnet build -f "$FRAMEWORK" -c "$CONFIG" -p:RuntimeIdentifier="$RID" /p:EnableCodeSigning=false

if [ $? -ne 0 ]; then
    echo "ERROR: dotnet build failed. Aborting script."
    exit 1
fi

echo "--- Build successful. Packaging IPA ---"

# Check if the .app bundle was created
APP_BUNDLE=$(find "$BUILD_DIR" -maxdepth 1 -name "*.app" | head -n 1)

if [ -z "$APP_BUNDLE" ]; then
    echo "ERROR: Could not find the .app bundle in $BUILD_DIR. Aborting script."
    exit 1
fi

echo "Found App Bundle: $APP_BUNDLE"

# Step 2: Create the Payload directory
mkdir -p "$PAYLOAD_DIR"

# Step 3: Move the .app bundle into the Payload directory
mv "$APP_BUNDLE" "$PAYLOAD_DIR/"

# Step 4: Zip the Payload directory to create the .ipa file
cd "$BUILD_DIR"
zip -r "$IPA_NAME" Payload/

# Move the final IPA to a cleaner location (optional, e.g., project root)
# mv "$IPA_NAME" "$PROJECT_DIR/$IPA_NAME"

# Clean up the Payload directory after zipping
rm -rf Payload/

echo "--- Successfully created $IPA_NAME in $BUILD_DIR ---"
