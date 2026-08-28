import { DateOnly, today } from '@holefeeder/shared/core';
import { AccountTypes } from '@/accounts/core/account-type';
import { EditAccountFormData } from '@/accounts/presentation/core/edit-account-form-data';

describe('EditAccountFormData', () => {
  describe('forNewAccount', () => {
    it('should leave the id null, which is what the shared save reads as a creation', () => {
      const formData = EditAccountFormData.forNewAccount();

      expect(formData.id).toBeNull();
    });

    it('should start from an empty checking account opened today', () => {
      const formData = EditAccountFormData.forNewAccount();

      expect(formData).toMatchObject({
        name: '',
        type: AccountTypes.checking,
        openBalance: 0,
        openDate: DateOnly.valid(today()),
        description: '',
      });
    });

    it('should start active and not favourite', () => {
      const formData = EditAccountFormData.forNewAccount();

      expect(formData.favorite).toBe(false);
      expect(formData.inactive).toBe(false);
    });

    it('should apply the overrides it is given', () => {
      const formData = EditAccountFormData.forNewAccount({ favorite: true });

      expect(formData.favorite).toBe(true);
    });
  });
});
