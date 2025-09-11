# Region Support Implementation Guide

## Overview

This document outlines how to extend the Revit Batch Processor to support Autodesk Construction Cloud regions by adding a region column to task files. Currently, the system assumes the US region by default and falls back to EMEA on failure.

## Current Implementation Analysis

### Task File Reading Location

The task file reading happens in **`revit_file_list.py`** in the following key functions:

- **`GetRevitFileListData(rows)`** - Core function that processes rows from task files
- **File reading functions:**
  - `FromTextFile()` - reads .txt files
  - `FromCSVFile()` - reads .csv files  
  - `FromExcelFile()` - reads .xlsx/.xls files

The `RevitFilePathData` class stores each row:
```python
class RevitFilePathData:
    def __init__(self, revitFilePath, associatedData):
        self.RevitFilePath = revitFilePath.Trim()
        self.AssociatedData = [value.Trim() for value in associatedData]  # Additional columns stored here
```

### Autodesk Cloud Detection

Cloud information is determined in the **`RevitCloudModelInfo`** class in `revit_file_list.py`:

```python
class RevitCloudModelInfo:
    def __init__(self, cloudModelDescriptor):
        # Parses space-separated format: "<Revit version> <Project Guid> <Model Guid>"
        parts = self.GetCloudModelDescriptorParts(cloudModelDescriptor)
        numberOfParts = len(parts)
        if numberOfParts > 1:
            # Parse the format and extract GUIDs
```

The system identifies cloud models by parsing the format: `<Revit version> <Project Guid> <Model Guid>`

### Current Region Hardcoding

The region is currently hardcoded in **`revit_file_util.py`** in the `ToCloudPath2021()` function:

```python
def ToCloudPath2021(cloudProjectId, cloudModelId):
    cloudProjectGuid = ToGuid(cloudProjectId)
    cloudModelGuid = ToGuid(cloudModelId)
    try:
        cloudPath = ModelPathUtils.ConvertCloudGUIDsToCloudPath(ModelPathUtils.CloudRegionUS, cloudProjectGuid, cloudModelGuid)
    except:
        cloudPath = ModelPathUtils.ConvertCloudGUIDsToCloudPath(ModelPathUtils.CloudRegionEMEA, cloudProjectGuid, cloudModelGuid)
    return cloudPath
```

## Implementation Strategy for Region Support

To add region support with a new column in the task file, modify these key areas:

### 1. Create Cloud Region Utilities Module (`cloud_region_util.py`)

**Create new module for centralized region handling:**
```python
import clr
clr.AddReference("RevitAPI")
from Autodesk.Revit.DB import ModelPathUtils

# Constants for hardcoded region strings (no official API support)
AUSTRALIA_REGION_STRING = "AUS"

# Mapping of user-friendly region codes to their descriptions
REGION_DESCRIPTIONS = {
    'US': 'United States East Region',
    'EU': 'Europe, Middle East, Africa', 
    'APAC': 'Australia',
}

# Mapping of user-friendly region codes to actual Revit API constants
# Note: Revit API currently only supports US and EMEA regions
REGION_API_MAPPING = {
    'US': ModelPathUtils.CloudRegionUS,      # Direct mapping
    'EU': ModelPathUtils.CloudRegionEMEA,    # Europe, Middle East, Africa -> EMEA
    'APAC': AUSTRALIA_REGION_STRING,         # Australia -> "AUS" (hardcoded string)
}

# Default region when none is specified
DEFAULT_REGION = 'US'
```

### 2. Extend Cloud Model Parsing (`revit_file_list.py`)

**Modify `RevitCloudModelInfo` class to use the utilities module:**
```python
import cloud_region_util

class RevitCloudModelInfo:
    def __init__(self, cloudModelDescriptor):
        self.cloudModelDescriptor = cloudModelDescriptor
        self.projectGuid = None
        self.modelGuid = None
        self.revitVersionText = None
        self.region = None  # Add region property
        self.isValid = False
        
        parts = self.GetCloudModelDescriptorParts(cloudModelDescriptor)
        numberOfParts = len(parts)
        
        if numberOfParts > 1:
            revitVersionPart = str.Empty
            otherParts = parts
            
            if numberOfParts > 2:
                revitVersionPart = parts[0]
                otherParts = parts[1:]
            else:
                otherParts = parts
                
            self.projectGuid = self.SafeParseGuidText(otherParts[0])
            self.modelGuid = self.SafeParseGuidText(otherParts[1])
            
            # Handle optional region parameter using utilities
            if len(otherParts) > 2:
                regionPart = otherParts[2].strip()
                if cloud_region_util.ValidateRegionCode(regionPart):
                    self.region = cloud_region_util.NormalizeRegionCode(regionPart)
            
            if RevitVersion.IsSupportedRevitVersionNumber(revitVersionPart):
                self.revitVersionText = revitVersionPart
                
            self.isValid = (
                self.projectGuid is not None
                and
                self.modelGuid is not None
            )
    
    def GetRegion(self):
        return self.region
    
    def GetRegionOrDefault(self, defaultRegion=None):
        if self.region is not None:
            return self.region
        if defaultRegion is not None:
            return defaultRegion
        return cloud_region_util.DEFAULT_REGION
    
    def GetRegionDescription(self):
        return cloud_region_util.GetRegionDescription(self.region)
    
    def GetRevitApiRegion(self):
        return cloud_region_util.GetRevitApiRegion(self.region)
```

### 3. Modify Cloud Path Functions (`revit_file_util.py`)

**Update `ToCloudPath2021()` function to use utilities module:**
```python
import cloud_region_util

def ToCloudPath2021(cloudProjectId, cloudModelId, region=None):
    """
    Convert cloud project and model GUIDs to a cloud path with region support.
    Uses cloud_region_util for region handling.
    """
    cloudProjectGuid = ToGuid(cloudProjectId)
    cloudModelGuid = ToGuid(cloudModelId)
    
    # Normalize and validate the region using the utility module
    normalizedRegion = cloud_region_util.NormalizeRegionCode(region)
    
    # Get the appropriate Revit API region constant or hardcoded string
    cloudRegion = cloud_region_util.GetRevitApiRegion(normalizedRegion)
    
    try:
        cloudPath = ModelPathUtils.ConvertCloudGUIDsToCloudPath(cloudRegion, cloudProjectGuid, cloudModelGuid)
        return cloudPath
    except Exception as e:
        # Fallback logic - use EMEA region
        cloudPath = ModelPathUtils.ConvertCloudGUIDsToCloudPath(ModelPathUtils.CloudRegionEMEA, cloudProjectGuid, cloudModelGuid)
        return cloudPath
```

## Task File Format

### Current Format (Cloud Models)
```
<Revit version> <Project Guid> <Model Guid>
```

**Example:**
```
2020 75b6464c-ba0f-4529-b049-0de9e473c2d6 0d54b8cc-3837-4df2-8c8e-0a94f4828868
2021 c0dc2fda-fd34-42fe-8bb7-bd9f43841dbf d9f011d6-d52c-4c9f-9d7b-eb8388bd3ed0
```

### New Format (With Region Support)
```
<Revit version> <Project Guid> <Model Guid> <Region>
```

**Example:**
```
2020 75b6464c-ba0f-4529-b049-0de9e473c2d6 0d54b8cc-3837-4df2-8c8e-0a94f4828868 EU
2021 c0dc2fda-fd34-42fe-8bb7-bd9f43841dbf d9f011d6-d52c-4c9f-9d7b-eb8388bd3ed0 US
2022 another-project-guid another-model-guid APAC
```

### Supported Region Codes
- **US** - United States East Region (default)
- **EU** - Europe, Middle East, Africa
- **APAC** - Australia

**Note:** Only US and EU have direct Revit API support. APAC uses a hardcoded "AUS" string.

### Backwards Compatibility
- Files without region column default to US region (United States East Region)
- Existing task files continue to work without modification
- Empty or invalid region codes fall back to US region

## Implementation Steps

1. **Create `cloud_region_util.py` module** with centralized region handling
2. **Modify `RevitCloudModelInfo` class** to parse and store region information using the utilities
3. **Update `ToCloudPath2021()` function** to accept region parameter and use utilities module
4. **Modify calling functions** to pass region information through the call chain
5. **Test with all supported formats** (with and without region column)
6. **Update documentation** to reflect new task file format options

## Key Implementation Changes Made

### Revit API Limitations Discovered
- **Reality Check:** Revit API only supports `CloudRegionUS` and `CloudRegionEMEA`
- **Solution:** Created mapping system where unsupported regions use closest available API region or hardcoded strings
- **Australia Special Case:** Uses hardcoded "AUS" string since no API support exists

### Simplified Design Decisions
- **Centralized Logic:** All region handling moved to `cloud_region_util.py` module
- **No Logging:** Removed complex logging to keep functions clean and simple
- **Simple Fallback:** Falls back directly to EMEA region instead of complex retry logic
- **Focused Scope:** Limited to 3 regions (US, EU, APAC) instead of original 8

### Module Separation Benefits
- **Reusable:** Region utilities can be used by any component
- **Maintainable:** Easy to update region constants (especially Australia string)
- **Consistent:** Same validation and normalization everywhere
- **Future-Proof:** Easy to add new regions when Revit API expands

## Testing Considerations

- Test with all region combinations (US, EU, APAC)
- Verify backwards compatibility with existing task files
- Test fallback behavior when specified region fails
- Validate with actual Autodesk Construction Cloud projects in different regions
- Test Excel, CSV, and text file formats with new column
- Verify region code case-insensitivity (US, us, Us should all work)
- Test with invalid region codes to ensure proper fallback behavior

## Files Modified

1. **Added:** `cloud_region_util.py` - New utilities module for region handling
2. **Modified:** `revit_file_list.py` - Updated `RevitCloudModelInfo` class
3. **Modified:** `revit_file_util.py` - Updated `ToCloudPath2021` function

## Notes

- The `AssociatedData` infrastructure already exists to handle additional columns
- Region parsing is case-insensitive but stored in uppercase for consistency
- Implementation maintains backwards compatibility by making the region column optional
- Australia region uses hardcoded "AUS" string that can be easily changed via `AUSTRALIA_REGION_STRING` constant