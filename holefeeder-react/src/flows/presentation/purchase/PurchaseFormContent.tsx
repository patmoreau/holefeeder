import React, { useEffect, useRef } from 'react';
import { Account } from '@/accounts/core/account';
import { Category } from '@/flows/core/categories/category';
import { Tag } from '@/flows/core/flows/tag';
import { BasicSection } from '@/flows/presentation/purchase/BasicSection';
import { CashflowSection } from '@/flows/presentation/purchase/CashflowSection';
import { PurchaseType } from '@/flows/presentation/purchase/core/purchase-form-data';
import { usePurchaseForm } from '@/flows/presentation/purchase/core/use-purchase-form';
import { PurchaseTransferSection } from '@/flows/presentation/purchase/PurchaseTransferSection';
import { TransferSection } from '@/flows/presentation/purchase/TransferSection';
import { AmountField, AmountFieldRef } from '@/flows/presentation/shared/components/AmountField';
import { AppForm } from '@/shared/presentation/AppForm';

type PurchaseFormProps = {
  accounts: Account[];
  categories: Category[];
  tags: Tag[];
};

export const PurchaseFormContent = ({ accounts, categories, tags }: PurchaseFormProps) => {
  const { formData, updateFormField } = usePurchaseForm();
  const amountFieldRef = useRef<AmountFieldRef>(null);

  useEffect(() => {
    // Auto-focus the amount field when the component mounts
    amountFieldRef.current?.focus();
  }, []);

  return (
    <AppForm>
      <PurchaseTransferSection
        selectedPurchaseType={formData.purchaseType}
        onSelectPurchaseType={(type) => updateFormField('purchaseType', type)}
      />
      <AmountField ref={amountFieldRef} amount={formData.amount} onAmountChange={(amount) => updateFormField('amount', amount)} />
      {formData.purchaseType !== PurchaseType.transfer && (
        <>
          <BasicSection accounts={accounts} categories={categories} tags={tags} />
          <CashflowSection />
        </>
      )}
      {formData.purchaseType === PurchaseType.transfer && <TransferSection accounts={accounts} />}
    </AppForm>
  );
};
