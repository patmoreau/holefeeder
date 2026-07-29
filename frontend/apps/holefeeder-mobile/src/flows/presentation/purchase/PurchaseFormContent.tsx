import { Account } from '@/accounts/core/account';
import { Category } from '@/flows/core/categories/category';
import { Tag } from '@/flows/core/flows/tag';
import { BasicSection } from '@/flows/presentation/purchase/BasicSection';
import { CashflowSection } from '@/flows/presentation/purchase/CashflowSection';
import { PurchaseType } from '@/flows/presentation/purchase/core/purchase-form-data';
import { usePurchaseForm } from '@/flows/presentation/purchase/core/use-purchase-form';
import { PurchaseTransferSection } from '@/flows/presentation/purchase/PurchaseTransferSection';
import { TransferSection } from '@/flows/presentation/purchase/TransferSection';
import { amountToneFor } from '@/flows/presentation/shared/core/amount-tone';
import { AmountField } from '@/shared/presentation/components/fields/AmountField';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppFieldGroup } from '@/shared/presentation/components/native/AppFieldGroup';
import { AppNative } from '@/shared/presentation/components/native/AppNative';

type PurchaseFormProps = {
  accounts: Account[];
  categories: Category[];
  tags: Tag[];
};

export const PurchaseFormContent = ({ accounts, categories, tags }: PurchaseFormProps) => {
  const { formData, updateFormField } = usePurchaseForm();

  return (
    <AppNative style={{ flex: 1 }}>
      <AppColumn>
        <PurchaseTransferSection
          selectedPurchaseType={formData.purchaseType}
          onSelectPurchaseType={(type) => updateFormField('purchaseType', type)}
        />
        <AmountField
          autoFocus
          amount={formData.amount}
          onAmountChange={(amount) => updateFormField('amount', amount)}
          tone={amountToneFor(formData.purchaseType)}
        />
        <AppFieldGroup>
          {formData.purchaseType !== PurchaseType.transfer && (
            <>
              <BasicSection accounts={accounts} categories={categories} tags={tags} />
              <CashflowSection />
            </>
          )}
          {formData.purchaseType === PurchaseType.transfer && <TransferSection accounts={accounts} />}
        </AppFieldGroup>
      </AppColumn>
    </AppNative>
  );
};
