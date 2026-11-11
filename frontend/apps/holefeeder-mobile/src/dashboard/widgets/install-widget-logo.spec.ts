import { File } from 'expo-file-system';
import { Image } from 'react-native';
import { installWidgetLogo, WidgetLogoErrors } from '@/dashboard/widgets/install-widget-logo';

const files: { uri: string; exists: boolean; copy: jest.Mock; delete: jest.Mock }[] = [];

jest.mock('expo-file-system', () => {
  const File = jest.fn().mockImplementation((...uris: string[]) => {
    const file = { uri: uris.join('/'), exists: false, copy: jest.fn(), delete: jest.fn() };
    files.push(file);
    return file;
  });
  Object.assign(File, { downloadFileAsync: jest.fn() });
  const Directory = jest.fn().mockImplementation(() => ({ create: jest.fn() }));
  return { File: File, Directory: Directory };
});

const downloadFileAsync = (File as unknown as { downloadFileAsync: jest.Mock }).downloadFileAsync;
const resolveAssetSource = jest.spyOn(Image, 'resolveAssetSource');

describe('installWidgetLogo', () => {
  beforeEach(() => {
    files.length = 0;
    jest.clearAllMocks();
  });

  it('should copy the bundled logo into the widgets directory', async () => {
    resolveAssetSource.mockReturnValue({ uri: 'file:///app/assets/widget-tile.png', width: 180, height: 180, scale: 1 });

    const result = await installWidgetLogo();

    expect(result).toBeSuccessWithValue('file:///group/widgets/widget-tile.png');
    const [destination, source] = files;
    expect(source.uri).toBe('file:///app/assets/widget-tile.png');
    expect(source.copy).toHaveBeenCalledWith(destination);
  });

  it('should download the logo when the bundler serves it', async () => {
    resolveAssetSource.mockReturnValue({ uri: 'http://localhost:8081/assets/widget-tile.png', width: 180, height: 180, scale: 1 });

    await installWidgetLogo();

    expect(downloadFileAsync).toHaveBeenCalledWith('http://localhost:8081/assets/widget-tile.png', files[0]);
  });

  it('should replace a logo installed earlier', async () => {
    resolveAssetSource.mockReturnValue({ uri: 'file:///app/assets/widget-tile.png', width: 180, height: 180, scale: 1 });
    (File as unknown as jest.Mock).mockImplementationOnce((...uris: string[]) => {
      const file = { uri: uris.join('/'), exists: true, copy: jest.fn(), delete: jest.fn() };
      files.push(file);
      return file;
    });

    await installWidgetLogo();

    expect(files[0].delete).toHaveBeenCalled();
  });

  it('should fail when the logo cannot be installed', async () => {
    resolveAssetSource.mockImplementation(() => {
      throw new Error('missing asset');
    });

    const result = await installWidgetLogo();

    expect(result).toBeFailureWithErrors([WidgetLogoErrors.installFailed]);
  });
});
