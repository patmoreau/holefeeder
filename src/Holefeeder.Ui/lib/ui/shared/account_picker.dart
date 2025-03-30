import 'package:flutter/widgets.dart';
import 'package:holefeeder/core/models/account.dart';
import 'package:holefeeder/ui/shared/platform_picker_widget.dart';

class AccountPicker extends StatelessWidget {
  final List<Account> accounts;
  final Account? selectedAccount;
  final ValueChanged<Account?> onChanged;

  const AccountPicker({
    super.key,
    required this.accounts,
    required this.selectedAccount,
    required this.onChanged,
  });

  @override
  Widget build(BuildContext context) {
    return PlatformPicker<Account>(
      label: 'Account',
      value: selectedAccount,
      items: accounts,
      displayStringFor: (account) => account.name,
      onChanged: onChanged,
      placeholder: 'Select account',
    );
  }
}
