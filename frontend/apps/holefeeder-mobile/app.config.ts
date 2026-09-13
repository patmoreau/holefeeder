import fs from 'node:fs';
import path from 'node:path';
import { ConfigContext } from '@expo/config';
import { ExpoConfig } from '@expo/config-types';
import { withInfoPlist, withXcodeProject, type ConfigPlugin } from 'expo/config-plugins';

// Workaround for https://github.com/expo/expo/issues/46204
// expo-dev-client@56 adds inputPaths to the "Strip Local Network Keys" build phase
// but no outputPaths, creating a cycle in Xcode's dependency graph that causes
// the build to fail. Clearing inputPaths removes the offending dependency edge.
const withDevLauncherBuildPhaseFix: ConfigPlugin = (config) =>
  withXcodeProject(config, (modConfig) => {
    const project = modConfig.modResults;
    const buildPhases = project.hash.project.objects['PBXShellScriptBuildPhase'] ?? {};
    for (const key of Object.keys(buildPhases)) {
      if (key.endsWith('_comment')) continue;
      const phase = buildPhases[key];
      if (phase?.name === '"[Expo Dev Launcher] Strip Local Network Keys for Release"') {
        phase.inputPaths = [];
        phase.inputFileListPaths = [];
      }
    }
    return modConfig;
  });

// Workaround for https://github.com/expo/expo/issues/46663
// The iOS 27 SDK refuses to launch an app that has not adopted the UIScene
// lifecycle, and Expo SDK 57 still generates a pre-scene AppDelegate with no
// UIApplicationSceneManifest. Remove this once Expo ships its own migration —
// expo-modules-core leaves scene handling unimplemented today
// (ExpoAppDelegateSubscriberManager.swift, "TODO: - Configuring and Discarding Scenes").
const SCENE_DELEGATE_FILENAME = 'SceneDelegate.swift';

const sceneDelegateSource = `import UIKit

// The AppDelegate builds the window and starts React Native in
// didFinishLaunchingWithOptions, which runs before a scene connects, so adopt
// that window rather than creating a second one. URL and activity callbacks are
// delivered here under the scene lifecycle, so hand them back to the app
// delegate chain that Expo's subscribers — Auth0, expo-linking — hook into.
@objc(SceneDelegate)
class SceneDelegate: UIResponder, UIWindowSceneDelegate {
  var window: UIWindow?

  func scene(
    _ scene: UIScene,
    willConnectTo session: UISceneSession,
    options connectionOptions: UIScene.ConnectionOptions
  ) {
    guard let windowScene = scene as? UIWindowScene else { return }

    let appDelegate = UIApplication.shared.delegate

    if let existingWindow = (appDelegate?.window ?? nil) {
      existingWindow.windowScene = windowScene
      window = existingWindow
      existingWindow.makeKeyAndVisible()
    }

    if let url = connectionOptions.urlContexts.first?.url {
      _ = appDelegate?.application?(UIApplication.shared, open: url, options: [:])
    }

    for userActivity in connectionOptions.userActivities {
      _ = appDelegate?.application?(UIApplication.shared, continue: userActivity) { _ in }
    }
  }

  func scene(_ scene: UIScene, openURLContexts URLContexts: Set<UIOpenURLContext>) {
    guard let url = URLContexts.first?.url else { return }
    _ = UIApplication.shared.delegate?.application?(UIApplication.shared, open: url, options: [:])
  }

  func scene(_ scene: UIScene, continue userActivity: NSUserActivity) {
    _ = UIApplication.shared.delegate?.application?(UIApplication.shared, continue: userActivity) { _ in }
  }
}
`;

const withSceneManifest: ConfigPlugin = (config) =>
  withInfoPlist(config, (modConfig) => {
    modConfig.modResults.UIApplicationSceneManifest = {
      UIApplicationSupportsMultipleScenes: false,
      UISceneConfigurations: {
        UIWindowSceneSessionRoleApplication: [
          {
            UISceneConfigurationName: 'Default Configuration',
            UISceneDelegateClassName: '$(PRODUCT_MODULE_NAME).SceneDelegate',
          },
        ],
      },
    };
    return modConfig;
  });

const withSceneDelegateSource: ConfigPlugin = (config) =>
  withXcodeProject(config, (modConfig) => {
    const { projectName, platformProjectRoot } = modConfig.modRequest;
    if (!projectName) {
      throw new Error('withUIScene: could not resolve the iOS project name');
    }

    // Written here rather than in a separate mod so the file always exists
    // before it is registered in the same pass.
    fs.writeFileSync(path.join(platformProjectRoot, projectName, SCENE_DELEGATE_FILENAME), sceneDelegateSource);

    const project = modConfig.modResults;
    const relativePath = `${projectName}/${SCENE_DELEGATE_FILENAME}`;
    if (!project.hasFile(relativePath)) {
      project.addSourceFile(relativePath, { target: project.getFirstTarget().uuid }, project.findPBXGroupKey({ name: projectName }));
    }

    return modConfig;
  });

const withUIScene: ConfigPlugin = (config) => withSceneManifest(withSceneDelegateSource(config));

const environment = process.env.APP_ENV || 'development';

// override: true is required. A Release build makes the Expo CLI set
// NODE_ENV=production, so it auto-loads .env.production before this runs, and
// dotenv leaves already-set variables alone — without the override, APP_ENV is
// ignored and a development Release build silently points at production.
// quiet: true silences dotenv v17's "injected env" banner. That banner goes to
// stdout, and `expo config --json` / expo-doctor parse this config's stdout as
// JSON, so any extra line makes them fail.
// eslint-disable-next-line @typescript-eslint/no-require-imports
require('dotenv').config({ quiet: true });
// eslint-disable-next-line @typescript-eslint/no-require-imports
require('dotenv').config({
  path: `.env.${environment}`,
  override: true,
  quiet: true,
});

const IS_DEV = process.env.APP_ENV === 'development';
const iosIconFile = IS_DEV ? 'safe_dev.icon' : 'safe.icon';
console.error(`Running in ${environment} mode`);

export default ({ config }: ConfigContext): ExpoConfig => {
  const appConfig: ExpoConfig = {
    ...config,
    name: 'Holefeeder',
    slug: 'holefeeder',
    owner: 'Drifter Apps Inc.',
    version: '1.0.0',
    orientation: 'portrait',
    icon: './assets/images/safe.png',
    scheme: 'holefeeder',
    userInterfaceStyle: 'automatic',
    ios: {
      supportsTablet: true,
      icon: `./assets/${iosIconFile}`,
      bundleIdentifier: 'com.drifterapps.holefeeder',
      deploymentTarget: '16.4',
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
      'expo-status-bar',
      'expo-build-properties',
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
      // Swaps Auth0 for a link-driven stand-in, so it must never reach a
      // production build. Dropped here rather than trusted to the env files.
      EXPO_PUBLIC_E2E: environment === 'production' ? undefined : process.env.EXPO_PUBLIC_E2E,
    },
    experiments: {
      typedRoutes: true,
      reactCompiler: true,
    },
  };
  return withUIScene(withDevLauncherBuildPhaseFix(appConfig));
};
