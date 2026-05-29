import { Account } from '@/accounts/core/account';
import { Category } from '@/flows/core/categories/category';
import { Tag } from '@/flows/core/flows/tag';
import { BasicSection } from '@/flows/presentation/purchase/BasicSection';
import { CashflowSection } from '@/flows/presentation/purchase/CashflowSection';
import { PurchaseType } from '@/flows/presentation/purchase/core/purchase-form-data';
import { usePurchaseForm } from '@/flows/presentation/purchase/core/use-purchase-form';
import { PurchaseTransferSection } from '@/flows/presentation/purchase/PurchaseTransferSection';
import { TransferSection } from '@/flows/presentation/purchase/TransferSection';
import { AmountField } from '@/flows/presentation/shared/components/AmountField';
import { AppColumn } from '@/shared/presentation/components/app/AppColumn';
import { AppFieldGroup } from '@/shared/presentation/components/app/AppFieldGroup';
import { AppHost } from '@/shared/presentation/components/app/AppHost';

type PurchaseFormProps = {
  accounts: Account[];
  categories: Category[];
  tags: Tag[];
};

export const PurchaseFormContent = ({ accounts, categories, tags }: PurchaseFormProps) => {
  const { formData, updateFormField } = usePurchaseForm();

  return (
    <AppHost style={{ flex: 1 }}>
      <AppColumn spacing={8}>
        <PurchaseTransferSection
          selectedPurchaseType={formData.purchaseType}
          onSelectPurchaseType={(type) => updateFormField('purchaseType', type)}
        />
        <AmountField
          autoFocus
          amount={formData.amount}
          onAmountChange={(amount) => updateFormField('amount', amount)}
          purchaseType={formData.purchaseType}
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
    </AppHost>
  );
};
