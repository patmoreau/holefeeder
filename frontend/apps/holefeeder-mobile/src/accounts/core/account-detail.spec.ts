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

  describe('owned balances', () => {
    const withBalances = (type: AccountType, balance: number, projectedBalance: number = balance): AccountDetail =>
      AccountDetail.valid({
        ...anAccountDetail(type, 0),
        balance: Variation.valid(balance),
        projectedBalance: Variation.valid(projectedBalance),
      });

    it('should count a checking account in credit as owned', () => {
      expect(AccountDetail.ownedBalance(withBalances(AccountTypes.checking, 500))).toBe(500);
    });

    it('should count an overdrawn checking account as owed', () => {
      expect(AccountDetail.ownedBalance(withBalances(AccountTypes.checking, -508.11))).toBe(-508.11);
    });

    it('should count a credit card balance as owed', () => {
      expect(AccountDetail.ownedBalance(withBalances(AccountTypes.creditCard, 250))).toBe(-250);
    });

    it('should count a credit card in credit as owned', () => {
      expect(AccountDetail.ownedBalance(withBalances(AccountTypes.creditCard, -20))).toBe(20);
    });

    it('should take the projected balance from the projection, not the current balance', () => {
      expect(AccountDetail.ownedProjectedBalance(withBalances(AccountTypes.checking, -508.11, 461.89))).toBe(461.89);
    });

    it('should count a credit card projected to owe as owed', () => {
      expect(AccountDetail.ownedProjectedBalance(withBalances(AccountTypes.creditCard, 0, 250))).toBe(-250);
    });
  });

  describe('balanceCaption', () => {
    const withBalance = (type: AccountType, balance: number): AccountDetail =>
      AccountDetail.valid({ ...anAccountDetail(type, 0), balance: Variation.valid(balance) });

    it('should name a positive checking balance the available balance', () => {
      expect(AccountDetail.balanceCaption(withBalance(AccountTypes.checking, 500))).toBe('availableBalance');
    });

    it('should name an overdrawn checking balance the amount due', () => {
      expect(AccountDetail.balanceCaption(withBalance(AccountTypes.checking, -508.11))).toBe('amountDue');
    });

    it('should name a positive credit card balance the amount due', () => {
      expect(AccountDetail.balanceCaption(withBalance(AccountTypes.creditCard, 250))).toBe('amountDue');
    });

    it('should name a negative credit card balance the credit balance', () => {
      expect(AccountDetail.balanceCaption(withBalance(AccountTypes.creditCard, -20))).toBe('creditBalance');
    });

    it('should name a positive loan balance the amount due', () => {
      expect(AccountDetail.balanceCaption(withBalance(AccountTypes.loan, 12000))).toBe('amountDue');
    });

    it('should follow the normal state of an asset at zero', () => {
      expect(AccountDetail.balanceCaption(withBalance(AccountTypes.checking, 0))).toBe('availableBalance');
    });

    it('should follow the normal state of a liability at zero', () => {
      expect(AccountDetail.balanceCaption(withBalance(AccountTypes.creditCard, 0))).toBe('amountDue');
    });

    it('should treat a balance that rounds to zero as zero', () => {
      expect(AccountDetail.balanceCaption(withBalance(AccountTypes.creditCard, -0.001))).toBe('amountDue');
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
