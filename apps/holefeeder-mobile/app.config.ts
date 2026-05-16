import { ConfigContext } from '@expo/config';
import { ExpoConfig } from '@expo/config-types';
import 'dotenv/config';

const environment = process.env.APP_ENV || 'development';

// eslint-disable-next-line @typescript-eslint/no-require-imports
require('dotenv').config({
  path: `.env.${environment}`,
});

const IS_DEV = process.env.APP_ENV === 'development';
const iosIconFile = IS_DEV ? 'safe_dev.icon' : 'safe.icon';
console.log(`Running in ${environment} mode`);

export default ({ config }: ConfigContext): ExpoConfig => ({
  ...config,
  name: 'Holefeeder',
  slug: 'holefeeder',
  version: '1.0.0',
  orientation: 'portrait',
  icon: './assets/images/safe.png',
  scheme: 'holefeeder',
  userInterfaceStyle: 'automatic',
  ios: {
    supportsTablet: true,
    icon: `./assets/${iosIconFile}`,
    bundleIdentifier: 'com.drifterapps.holefeeder',
    infoPlist: {
      ...config.ios?.infoPlist,
      NSAppTransportSecurity: {
        NSAllowsArbitraryLoads: true, // Allows all insecure connections in dev
        NSExceptionDomains: {
          'localtest.me': {
            NSIncludesSubdomains: true,
            NSTemporaryExceptionAllowsInsecureHTTPLoads: true,
          },
          'moreaulab.ca': {
            NSIncludesSubdomains: true,
            NSTemporaryExceptionAllowsInsecureHTTPLoads: true,
          },
        },
      },
    },
  },
  android: {
    adaptiveIcon: {
      foregroundImage: './assets/images/adaptive-icon.png',
      backgroundColor: '#ffffff',
    },
    predictiveBackGestureEnabled: false,
    package: 'com.drifterapps.holefeeder',
  },
  web: {
    output: 'static',
    favicon: './assets/images/safe.png',
  },
  plugins: [
    [
      'expo-image',
      {
        disableLibdav1d: true,
      },
    ],
    'expo-font',
    'expo-localization',
    'expo-web-browser',
    [
      'expo-file-system',
      {
        supportsOpeningDocumentsInPlace: true,
        enableFileSharing: true,
      },
    ],
    [
      'expo-sharing',
      {
        ios: {
          enabled: true,
          activationRule: {
            supportsImageWithMaxCount: 5,
          },
        },
      },
    ],
    'expo-quick-actions',
    [
      'expo-router',
      {
        root: './src/app',
      },
    ],
    [
      'expo-splash-screen',
      {
        image: './assets/images/safe.png',
        resizeMode: 'contain',
        backgroundColor: '#ffffff',
      },
    ],
  ],
  extra: {
    EXPO_PUBLIC_AUTH0_DOMAIN: process.env.EXPO_PUBLIC_AUTH0_DOMAIN,
    EXPO_PUBLIC_AUTH0_CLIENT_ID: process.env.EXPO_PUBLIC_AUTH0_CLIENT_ID,
    EXPO_PUBLIC_AUTH0_AUDIENCE: process.env.EXPO_PUBLIC_AUTH0_AUDIENCE,
    EXPO_PUBLIC_AUTH0_SCOPE: process.env.EXPO_PUBLIC_AUTH0_SCOPE,
    EXPO_PUBLIC_AUTH0_IOS_REDIRECT_URI: process.env.EXPO_PUBLIC_AUTH0_IOS_REDIRECT_URI,
    EXPO_PUBLIC_AUTH0_IOS_LOGOUT_REDIRECT_URI: process.env.EXPO_PUBLIC_AUTH0_IOS_LOGOUT_REDIRECT_URI,
    EXPO_PUBLIC_AUTH0_ANDROID_REDIRECT_URI: process.env.EXPO_PUBLIC_AUTH0_ANDROID_REDIRECT_URI,
    EXPO_PUBLIC_AUTH0_ANDROID_LOGOUT_REDIRECT_URI: process.env.EXPO_PUBLIC_AUTH0_ANDROID_LOGOUT_REDIRECT_URI,
    EXPO_PUBLIC_AUTH0_WEB_REDIRECT_URI: process.env.EXPO_PUBLIC_AUTH0_WEB_REDIRECT_URI,
    EXPO_PUBLIC_AUTH0_WEB_LOGOUT_REDIRECT_URI: process.env.EXPO_PUBLIC_AUTH0_WEB_LOGOUT_REDIRECT_URI,
    EXPO_PUBLIC_API_BASE_URL: process.env.EXPO_PUBLIC_API_BASE_URL,
    EXPO_PUBLIC_API_TIMEOUT: process.env.EXPO_PUBLIC_API_TIMEOUT,
    EXPO_PUBLIC_API_LOG_REQUEST: process.env.EXPO_PUBLIC_API_LOG_REQUEST,
    EXPO_PUBLIC_SIMULATE_NETWORK_DELAY: process.env.EXPO_PUBLIC_SIMULATE_NETWORK_DELAY,
    EXPO_PUBLIC_CACHE_REQUESTS: process.env.EXPO_PUBLIC_CACHE_REQUESTS,
    EXPO_PUBLIC_POWERSYNC_URL: process.env.EXPO_PUBLIC_POWERSYNC_URL,
    EXPO_PUBLIC_FORCE_LOGS: process.env.EXPO_PUBLIC_FORCE_LOGS,
  },
  experiments: {
    typedRoutes: true,
    reactCompiler: true,
  },
});
