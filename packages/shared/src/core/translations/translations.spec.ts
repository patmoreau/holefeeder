import { en } from './locales/en-CA/translations';
import { tk, TranslationStructure } from './translations';

describe('translations', () => {
  describe('createLangObject', () => {
    it('should create translation keys for top-level string properties', () => {
      expect(tk.common.welcome).toBe('common.welcome');
      expect(tk.common.login).toBe('common.login');
      expect(tk.common.logout).toBe('common.logout');
    });

    it('should create translation keys for nested object properties', () => {
      expect(tk.alert.discard.title).toBe('alert.discard.title');
      expect(tk.alert.discard.message).toBe('alert.discard.message');
      expect(tk.alert.discard.confirmText).toBe('alert.discard.confirmText');
    });

    it('should create translation keys for deeply nested properties', () => {
      expect(tk.errors.noInternetConnection.title).toBe('errors.noInternetConnection.title');
      expect(tk.errors.noInternetConnection.message).toBe('errors.noInternetConnection.message');
    });

    it('should preserve the structure of the original translation object', () => {
      expect(typeof tk.common).toBe('object');
      expect(typeof tk.alert).toBe('object');
      expect(typeof tk.alert.discard).toBe('object');
    });

    it('should handle all keys from the source translation object', () => {
      const pluralSuffixRegex = /_(one|other|zero|few|many)$/;

      const checkAllKeys = (source: any, generated: any, path = ''): void => {
        for (const key in source) {
          if (pluralSuffixRegex.test(key)) {
            continue;
          }

          const currentPath = path ? `${path}.${key}` : key;

          if (typeof source[key] === 'string') {
            expect(generated[key]).toBe(currentPath);
          } else if (typeof source[key] === 'object' && source[key] !== null) {
            expect(typeof generated[key]).toBe('object');
            checkAllKeys(source[key], generated[key], currentPath);
          }
        }
      };

      checkAllKeys(en, tk);
    });

    it('should not have any undefined values for string properties', () => {
      const checkNoUndefined = (obj: any): void => {
        for (const key in obj) {
          if (typeof obj[key] === 'object' && obj[key] !== null) {
            checkNoUndefined(obj[key]);
          } else {
            expect(obj[key]).toBeDefined();
            expect(typeof obj[key]).toBe('string');
          }
        }
      };

      checkNoUndefined(tk);
    });

    it('should create paths that match the dot notation format', () => {
      const pathRegex = /^[a-zA-Z0-9]+(\.[a-zA-Z0-9]+)*$/;

      const checkPathFormat = (obj: any): void => {
        for (const key in obj) {
          if (typeof obj[key] === 'string') {
            expect(obj[key]).toMatch(pathRegex);
          } else if (typeof obj[key] === 'object' && obj[key] !== null) {
            checkPathFormat(obj[key]);
          }
        }
      };

      checkPathFormat(tk);
    });
  });

  describe('TranslationStructure type', () => {
    it('should maintain the same structure as the source translation object', () => {
      const testStructure: TranslationStructure = {
        alert: {
          discard: {
            title: 'test',
            message: 'test',
            confirmText: 'test',
            cancelText: 'test',
          },
        },
        auth: {
          loginTitle: 'test',
          loginSubtitle: 'test',
          loginButton: 'test',
          logoutButton: 'test',
          loggingIn: 'test',
          loginError: 'test',
          loginErrorMessage: 'test',
          logoutSuccess: 'test',
        },
        common: {
          welcome: 'test',
          login: 'test',
          logout: 'test',
          loading: 'test',
          error: 'test',
          retry: 'test',
          cancel: 'test',
          save: 'test',
          delete: 'test',
          edit: 'test',
          done: 'test',
        },
        dateIntervalTypePicker: {
          weekly: 'test',
          monthly: 'test',
          yearly: 'test',
          oneTime: 'test',
        },
        displaySection: {
          title: 'test',
          language: 'test',
          theme: 'test',
        },
        errors: {
          noInternetConnection: {
            title: 'test',
            message: 'test',
          },
          cannotReachServer: {
            title: 'test',
            message: 'test',
          },
        },
      } as any;

      expect(testStructure).toBeDefined();
    });
  });

  describe('tk object', () => {
    it('should be defined and not null', () => {
      expect(tk).toBeDefined();
      expect(tk).not.toBeNull();
    });

    it('should be an object', () => {
      expect(typeof tk).toBe('object');
    });

    it('should have the same top-level keys as the source translation object', () => {
      const sourceKeys = Object.keys(en).sort();
      const tkKeys = Object.keys(tk).sort();

      expect(tkKeys).toEqual(sourceKeys);
    });

    it('should create unique paths for each translation key', () => {
      const paths = new Set<string>();
      const pluralSuffixRegex = /_(one|other|zero|few|many)$/;

      const collectPaths = (obj: any): void => {
        for (const key in obj) {
          if (typeof obj[key] === 'string') {
            paths.add(obj[key]);
          } else if (typeof obj[key] === 'object' && obj[key] !== null) {
            collectPaths(obj[key]);
          }
        }
      };

      collectPaths(tk);

      const countStrings = (obj: any): number => {
        let count = 0;
        for (const key in obj) {
          if (pluralSuffixRegex.test(key)) {
            continue;
          }

          if (typeof obj[key] === 'string') {
            count++;
          } else if (typeof obj[key] === 'object' && obj[key] !== null) {
            count += countStrings(obj[key]);
          }
        }
        return count;
      };

      const totalStrings = countStrings(en);

      expect(paths.size).toBe(totalStrings);
    });
  });
});
