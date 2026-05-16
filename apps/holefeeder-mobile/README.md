# Welcome to your Expo app 👋

This is an [Expo](https://expo.dev) project created with [`create-expo-app`](https://www.npmjs.com/package/create-expo-app).

## Get started

1. Install dependencies

   ```bash
   npm install
   ```

2. Start the app

   ```bash
   npx expo start
   ```

In the output, you'll find options to open the app in a

- [development build](https://docs.expo.dev/develop/development-builds/introduction/)
- [Android emulator](https://docs.expo.dev/workflow/android-studio-emulator/)
- [iOS simulator](https://docs.expo.dev/workflow/ios-simulator/)
- [Expo Go](https://expo.dev/go), a limited sandbox for trying out app development with Expo

You can start developing by editing the files inside the **app** directory. This project uses [file-based routing](https://docs.expo.dev/router/introduction).

## iOS Simulator — Local HTTPS Setup

The app connects to local services (`powersync.localtest.me`, `holefeeder.localtest.me`) running behind a Traefik
reverse proxy with **mkcert** TLS certificates. Two one-time setup steps are required for the iOS Simulator to reach
these services.

### 1. Override DNS for `localtest.me` subdomains

`*.localtest.me` resolves publicly to `127.0.0.1` (loopback). From the iOS Simulator, `127.0.0.1` points to the
simulator itself — not your Mac. Override the entries in `/etc/hosts` to point to your Mac's LAN IP:

```bash
sudo bash -c 'echo "127.0.0.1 powersync.localtest.me holefeeder.localtest.me" >> /etc/hosts'
sudo dscacheutil -flushcache && sudo killall -HUP mDNSResponder
```

> **Note:** If your Mac's LAN IP changes (DHCP), update the `/etc/hosts` entry accordingly. Consider setting a static IP
> on your router.

### 2. Trust the mkcert CA in the iOS Simulator

The Traefik proxy uses mkcert certificates. The macOS keychain trusts the mkcert CA automatically, but the iOS Simulator
has its own isolated trust store and must be configured separately.

Find the booted simulator's UDID and install the root CA:

```bash
# Find the booted simulator UDID
xcrun simctl list devices | grep Booted

# Install the mkcert root CA (replace <UDID> with the value from above)
xcrun simctl keychain <UDID> add-root-cert "$(mkcert -CAROOT)/rootCA.pem"
```

> **Note:** Repeat step 2 for any new simulator device you create, or after erasing an existing one.

---

## Get a fresh project

When you're ready, run:

```bash
npm run reset-project
```

This command will move the starter code to the **app-example** directory and create a blank **app** directory where you can start developing.

## Learn more

To learn more about developing your project with Expo, look at the following resources:

- [Expo documentation](https://docs.expo.dev/): Learn fundamentals, or go into advanced topics with our [guides](https://docs.expo.dev/guides).
- [Learn Expo tutorial](https://docs.expo.dev/tutorial/introduction/): Follow a step-by-step tutorial where you'll create a project that runs on Android, iOS, and the web.

## Join the community

Join our community of developers creating universal apps.

- [Expo on GitHub](https://github.com/expo/expo): View our open source platform and contribute.
- [Discord community](https://chat.expo.dev): Chat with Expo users and ask questions.
