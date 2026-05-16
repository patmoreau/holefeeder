# Maestro E2E Test Suite

This folder contains Maestro end-to-end test flows for the Holefeeder React Native application. Maestro is a simple,
powerful mobile UI testing framework that allows you to automate UI flows without complicated setup or boilerplate code.

## 📁 Folder Structure

```text
.maestro/
├── README.md                          # This documentation
├── .env.template                      # Environment variables template
├── config.yaml                        # Maestro configuration file
├── auth/                              # Authentication test flows
│   └── login.yaml                     # Login flow test
├── common/                            # Reusable test components
│   ├── auth/                          # Common authentication flows
│   │   ├── login.yaml                 # Shared login flow
│   │   └── logout.yaml                # Shared logout flow
│   └── ios/
│       └── dark-box-return.yaml      # iOS-specific keyboard return action
├── navigation/                        # Navigation test flows
│   └── bottom-navigation.yaml         # Bottom navigation tests
└── test-output/                       # Test execution results (auto-generated)
    ├── 2025-12-03_225949/            # Test run artifacts
    └── 2025-12-03_230151/            # Test run artifacts
```

## 🧪 Test Files Description

### Configuration

#### `config.yaml`

**Purpose**: Maestro configuration file that defines test flow organization

- Specifies flow directories (`auth/*`, `navigation/*`)
- Sets test output directory to `.maestro/test-output`
- Centralizes test execution configuration

### Authentication Flows

#### `auth/login.yaml`

**Purpose**: Direct authentication test flow

- Standalone login test
- Can be run independently or as part of larger test suites

#### `common/auth/login.yaml`

**Purpose**: Reusable login flow component

- Shared authentication logic
- Used by other test flows
- Includes Auth0 integration steps

#### `common/auth/logout.yaml`

**Purpose**: Reusable logout flow component

- Shared logout logic
- Handles Auth0 sign-out process
- Validates successful logout

### Navigation Flows

#### `navigation/bottom-navigation.yaml`

**Purpose**: Tests bottom navigation functionality

- Validates navigation between main app sections
- Tests tab switching behavior
- Ensures proper screen transitions

### Utility Components

#### `common/ios/dark-box-return.yaml`

**Purpose**: Reusable iOS-specific utility flow

- Handles keyboard "Return" button interaction
- Taps at coordinates (87%, 89%) which corresponds to the return key location
- Used during text input operations across different flows

## 🔧 Setup and Configuration

### 1. Environment Variables

#### Using .zshenv (Global environment variables)

Add the variables to your `~/.zshenv` file for system-wide availability:

```bash
# Add to ~/.zshenv
echo 'export MAESTRO_APP_ID="your.app.bundle.id"' >> ~/.zshenv
echo 'export MAESTRO_USER_EMAIL="test@example.com"' >> ~/.zshenv
echo 'export MAESTRO_USER_PASSWORD="your_test_password"' >> ~/.zshenv
source ~/.zshenv
```

**Note**: Replace the placeholder values with your actual app bundle ID and test credentials.

## 🚀 Running Tests

### Quick Start

Run tests using the configured flows:

```bash
# Navigate to project root
cd /path/to/holefeeder-react

# Run E2E tests using npm script (recommended)
pnpm test:e2e:ios

# Or run all flows directly with Maestro
maestro test .maestro/

# Run specific flow categories
maestro test .maestro/auth/
maestro test .maestro/navigation/
```

### Using npm Scripts

The project includes a convenient npm script for running E2E tests:

```bash
# Run iOS E2E tests with regression tag filtering
pnpm test:e2e:ios
```

This script runs: `maestro test --include-tags=regression .maestro/`

### Run Individual Tests

```bash
# Run authentication flows
maestro test .maestro/auth/login.yaml

# Run navigation tests
maestro test .maestro/navigation/bottom-navigation.yaml

# Run all flows from a specific directory
maestro test .maestro/auth/
maestro test .maestro/navigation/
```

### Run All Tests

```bash
# Run all test files in the folder
maestro test .maestro/

# Run only regression tagged tests
maestro test --tags regression .maestro/
```

### Run Tests with Options

```bash
# Run with debug output
maestro test --debug-output=./output .maestro/regression.yaml

# Run and include device logs
maestro test --include-logs .maestro/regression.yaml
```

## 📋 Test Scenarios Covered

- ✅ **User Authentication**: Multiple login flow variations
- ✅ **Session Management**: Proper logout functionality
- ✅ **Navigation Testing**: Bottom navigation and screen transitions
- ✅ **Component Reusability**: Shared flows for common actions
- ✅ **Platform-Specific Actions**: iOS keyboard handling

## 🔍 Debugging Tests

### View Test Execution

Maestro automatically captures test results in the `.maestro/test-output/` folder with timestamped directories for each
test run.

### Common Issues

1. **App not found**: Verify `MAESTRO_APP_ID` matches your app's bundle identifier
2. **Element not found**: Check if UI elements have changed or if timing issues exist
3. **Authentication failures**: Verify test credentials in `.env` file

### Debug Mode

Run tests with debug flag to see detailed execution logs:

```bash
maestro test --debug .maestro/regression.yaml
```

## 📝 Notes

- Tests are designed for iOS but can be adapted for Android
- Coordinate-based taps in `dark-box-return.yaml` may need adjustment for different screen sizes
- The regression test uses clean app state (`clearState: true, clearKeychain: true`) to ensure consistent test
  conditions
- Auth0 authentication flow is handled with web view interactions

## 🔄 Continuous Integration

These tests can be integrated into CI/CD pipelines:

```yaml
# Example GitHub Actions step
- name: Run Maestro Tests
  run: |
    maestro test .maestro/regression.yaml
```

For CI environments, consider using headless testing and capturing test artifacts.
