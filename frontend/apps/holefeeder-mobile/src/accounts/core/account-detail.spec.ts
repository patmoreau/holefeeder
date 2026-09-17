import { DateOnly, Id, Variation } from '@holefeeder/shared/core';
import { AccountDetail } from '@/accounts/core/account-detail';
import { AccountType, AccountTypes } from '@/accounts/core/account-type';

const anAccountDetail = (type: AccountType, upcomingVariation: number): AccountDetail =>
  AccountDetail.valid({
    id: Id.valid('00000000-0000-0000-0000-000000000001'),
    name: 'Account',
    type,
    balance: Variation.valid(500),
    lastTransactionDate: DateOnly.valid('2026-09-01'),
    projectedBalance: Variation.valid(500),
    upcomingVariation: Variation.valid(upcomingVariation),
  });

describe('AccountDetail', () => {
  describe('upcomingChange', () => {
    it('should add upcoming gains to a checking balance', () => {
      expect(AccountDetail.upcomingChange(anAccountDetail(AccountTypes.checking, 100))).toBe(100);
    });

    it('should subtract upcoming expenses from a checking balance', () => {
      expect(AccountDetail.upcomingChange(anAccountDetail(AccountTypes.checking, -250))).toBe(-250);
    });

    it('should raise a credit card balance by its upcoming expenses', () => {
      expect(AccountDetail.upcomingChange(anAccountDetail(AccountTypes.creditCard, -250))).toBe(250);
    });
  });

  describe('isUpcomingFavourable', () => {
    it('should be favourable when upcoming gains outweigh expenses', () => {
      expect(AccountDetail.isUpcomingFavourable(anAccountDetail(AccountTypes.checking, 100))).toBe(true);
    });

    it('should be unfavourable when upcoming expenses outweigh gains', () => {
      expect(AccountDetail.isUpcomingFavourable(anAccountDetail(AccountTypes.checking, -250))).toBe(false);
    });

    it('should be unfavourable when upcoming expenses raise a credit card balance', () => {
      expect(AccountDetail.isUpcomingFavourable(anAccountDetail(AccountTypes.creditCard, -250))).toBe(false);
    });

    it('should be favourable when nothing is upcoming', () => {
      expect(AccountDetail.isUpcomingFavourable(anAccountDetail(AccountTypes.checking, 0))).toBe(true);
    });
  });
});
