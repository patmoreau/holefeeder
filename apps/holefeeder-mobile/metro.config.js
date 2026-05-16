// Learn more https://docs.expo.io/guides/customizing-metro
const path = require('path');
const { getDefaultConfig } = require('expo/metro-config');

/** @type {import('expo/metro-config').MetroConfig} */
const config = getDefaultConfig(__dirname);

// Monorepo: preserve auto-detected watch folders (monorepo root node_modules
// and workspace packages) then add packages/core for hot-reload.
config.watchFolders = [...(config.watchFolders ?? []), path.resolve(__dirname, '../../packages/core')];

/** @type {import('metro-config').GetTransformOptions} */
const getTransformOptions = async (_entryPoints, _options, _getDependenciesOf) => ({
  transform: {
    inlineRequires: {
      blockList: {
        [require.resolve('@powersync/react-native')]: true,
      },
    },
  },
});

config.transformer.getTransformOptions = getTransformOptions;

module.exports = config;
