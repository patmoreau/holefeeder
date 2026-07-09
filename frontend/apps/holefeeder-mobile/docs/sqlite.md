# List your simulators to find the booted one

xcrun simctl list devices | grep Booted

# Find the PowerSync DB file (replace with your app bundle ID)

find ~/Library/Developer/CoreSimulator/Devices/<device-uuid> -name "holefeeder.db" 2>/dev/null