import { Logger, Result } from '@holefeeder/shared/core';
import { Directory, File } from 'expo-file-system';
import { widgetsDirectory } from 'expo-widgets';
import { Image } from 'react-native';
const widgetTile = require('../../../assets/images/widget-tile.png');

export const WidgetLogoErrors = {
  installFailed: 'install-widget-logo-failed',
};

const logger = Logger.create('installWidgetLogo');

export const installWidgetLogo = async (): Promise<Result<string>> => {
  try {
    const destination = new File(widgetsDirectory, 'widget-tile.png');
    const source = Image.resolveAssetSource(widgetTile)?.uri;
    if (!source) return Result.failure([WidgetLogoErrors.installFailed]);
    new Directory(widgetsDirectory).create({ idempotent: true, intermediates: true });
    if (destination.exists) destination.delete();
    if (source.startsWith('http')) {
      await File.downloadFileAsync(source, destination);
    } else {
      new File(source).copy(destination);
    }
    return Result.success(destination.uri);
  } catch (error) {
    if (error instanceof Error) {
      logger.error(`${WidgetLogoErrors.installFailed}: `, error.message);
    }
    return Result.failure([WidgetLogoErrors.installFailed]);
  }
};
