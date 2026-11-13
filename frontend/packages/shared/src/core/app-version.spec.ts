import { AppVersion } from './app-version';

describe('AppVersion', () => {
  describe('label', () => {
    it('joins the version and the short commit', () => {
      expect(AppVersion.label({ version: '2.0.1161', commit: '20b01381c0ffee1234567890abcdef1234567890' })).toBe('2.0.1161 · 20b01381');
    });

    it('keeps an already short commit', () => {
      expect(AppVersion.label({ version: '2.0.1161', commit: '20b01381' })).toBe('2.0.1161 · 20b01381');
    });

    it('shows the version alone without a commit', () => {
      expect(AppVersion.label({ version: '2.0.1161', commit: undefined })).toBe('2.0.1161');
    });

    it('shows dev without a version', () => {
      expect(AppVersion.label({ version: undefined, commit: '20b01381' })).toBe('dev · 20b01381');
    });

    it('treats blank values as missing', () => {
      expect(AppVersion.label({ version: ' ', build: ' ', commit: '' })).toBe('dev');
    });

    it('adds the build number after the version', () => {
      expect(AppVersion.label({ version: '2026.10.20', build: '2', commit: '650039e9' })).toBe('2026.10.20 (2) · 650039e9');
    });

    it('shows the version and build without a commit', () => {
      expect(AppVersion.label({ version: '2026.10.20', build: '2', commit: undefined })).toBe('2026.10.20 (2)');
    });
  });
});
