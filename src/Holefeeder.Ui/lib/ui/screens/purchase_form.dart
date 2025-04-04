import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:holefeeder/core/view_models/base_form_state.dart';
import 'package:holefeeder/core/view_models/screens/purchase_view_model.dart';
import 'package:holefeeder/ui/services/notification_service.dart';
import 'package:holefeeder/ui/shared/account_picker.dart';
import 'package:holefeeder/ui/shared/amount_field.dart';
import 'package:holefeeder/ui/shared/category_picker.dart';
import 'package:holefeeder/ui/shared/date_picker_field.dart';
import 'package:holefeeder/ui/shared/error_banner.dart';
import 'package:holefeeder/ui/shared/platform_button_widget.dart';
import 'package:holefeeder/ui/shared/platform_tag_selector.dart';
import 'package:holefeeder/ui/shared/platform_text_field.dart';

class PurchaseForm extends StatefulWidget {
  final PurchaseViewModel model;

  const PurchaseForm({super.key, required this.model});

  @override
  State<PurchaseForm> createState() => _PurchaseFormState();
}

class _PurchaseFormState extends State<PurchaseForm> {
  final _formKey = GlobalKey<FormState>();

  @override
  Widget build(BuildContext context) {
    return Form(
      key: _formKey,
      child: ListView(
        padding: const EdgeInsets.all(16.0),
        children: [
          if (widget.model.formState.state == ViewFormState.loading)
            const Center(child: CircularProgressIndicator())
          else if (widget.model.formState.state == ViewFormState.error)
            ErrorBanner(
              message: widget.model.formState.errorMessage ?? 'Unknown error',
            )
          else
            CupertinoFormSection(
              margin: const EdgeInsets.all(16),
              children: [..._buildFormFields()],
            ),
        ],
      ),
    );
  }

  List<Widget> _buildFormFields() => [
    AmountField(
      initialValue: widget.model.formState.amount,
      onChanged: widget.model.updateAmount,
    ),
    DatePickerField(
      selectedDate: widget.model.formState.date,
      onDateChanged: widget.model.updateDate,
    ),
    AccountPicker(
      accounts: widget.model.accounts,
      selectedAccount: widget.model.formState.selectedAccount,
      onChanged: widget.model.setSelectedAccount,
    ),
    CategoryPicker(
      categories: widget.model.categories,
      selectedCategory: widget.model.formState.selectedCategory,
      onChanged: widget.model.setSelectedCategory,
    ),
    PlatformTextField(
      labelText: 'Note',
      initialValue: widget.model.formState.note,
      onChanged: widget.model.updateNote,
    ),
    PlatformTagSelector(
      allTags: widget.model.tags,
      selectedTags: widget.model.formState.tags,
      onTagsChanged: widget.model.updateTags,
    ),
    const SizedBox(height: 24),
    PlatformButton(
      label: 'Purchase',
      onPressed: _handleSubmit,
      isLoading: widget.model.formState.state == ViewFormState.loading,
    ),
  ];

  Future<void> _handleSubmit() async {
    if (!_formKey.currentState!.validate()) {
      return;
    }

    try {
      await widget.model.makePurchase();
      if (!mounted) return;

      await NotificationService.show(
        context: context,
        message: 'Purchase successful',
      );

      if (!mounted) return;
      if (context.canPop()) {
        context.pop();
      } else {
        context.go('/');
      }
    } catch (error) {
      if (!mounted) return;
      await NotificationService.show(
        context: context,
        message: 'Error: $error',
        isError: true,
      );
    }
  }
}
