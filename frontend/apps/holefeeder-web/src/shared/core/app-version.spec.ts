import { describe, expect, it } from 'vitest';
import { AppVersion } from './app-version';

describe('AppVersion.label', () => {
  it('ShouldJoinVersionAndShortCommit', () => {
    expect(AppVersion.label({ version: '2.0.1161', commit: '20b01381c0ffee1234567890abcdef1234567890' })).toBe('2.0.1161 · 20b01381');
  });

  it('ShouldKeepAlreadyShortCommit', () => {
    expect(AppVersion.label({ version: '2.0.1161', commit: '20b01381' })).toBe('2.0.1161 · 20b01381');
  });

  it('ShouldShowVersionAloneWithoutCommit', () => {
    expect(AppVersion.label({ version: '2.0.1161', commit: undefined })).toBe('2.0.1161');
  });

  it('ShouldShowDevWithoutVersion', () => {
    expect(AppVersion.label({ version: undefined, commit: '20b01381' })).toBe('dev · 20b01381');
  });

  it('ShouldTreatBlankValuesAsMissing', () => {
    expect(AppVersion.label({ version: ' ', commit: '' })).toBe('dev');
  });
});
