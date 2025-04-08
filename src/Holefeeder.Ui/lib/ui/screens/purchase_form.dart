import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:holefeeder/core/view_models/base_form_state.dart';
import 'package:holefeeder/core/view_models/screens/purchase_view_model.dart';
import 'package:holefeeder/ui/shared/account_picker.dart';
import 'package:holefeeder/ui/shared/amount_field.dart';
import 'package:holefeeder/ui/shared/category_picker.dart';
import 'package:holefeeder/ui/shared/date_picker_field.dart';
import 'package:holefeeder/ui/shared/error_banner.dart';
import 'package:holefeeder/ui/shared/platform_tag_selector.dart';
import 'package:holefeeder/ui/shared/platform_text_field.dart';
import 'package:universal_platform/universal_platform.dart';

import '../../core/enums/date_interval_type_enum.dart';

class PurchaseForm extends StatefulWidget {
  final PurchaseViewModel model;
  final GlobalKey<FormState> formKey;

  const PurchaseForm({super.key, required this.model, required this.formKey});

  @override
  State<PurchaseForm> createState() => _PurchaseFormState();
}

class _PurchaseFormState extends State<PurchaseForm> {
  // final _formKey = GlobalKey<FormState>();

  @override
  Widget build(BuildContext context) {
    return Form(
      key: widget.formKey,
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
              margin: const EdgeInsets.all(16.0),
              clipBehavior: Clip.none,
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
    // Add cashflow switch
    UniversalPlatform.isApple
        ? CupertinoFormRow(
          prefix: const Text('Cashflow'),
          child: CupertinoSwitch(
            value: widget.model.formState.isCashflow,
            onChanged: widget.model.updateIsCashflow,
          ),
        )
        : SwitchListTile(
          title: const Text('Cashflow'),
          value: widget.model.formState.isCashflow,
          onChanged: widget.model.updateIsCashflow,
        ),
    // Show additional fields when isCashflow is true
    if (widget.model.formState.isCashflow) ...[
      DatePickerField(
        // labelText: 'Effective Date',
        selectedDate: widget.model.formState.effectiveDate,
        onDateChanged: widget.model.updateEffectiveDate,
      ),
      UniversalPlatform.isApple
          ? CupertinoFormRow(
            prefix: const Text('Interval Type'),
            child: CupertinoSlidingSegmentedControl<DateIntervalType>(
              groupValue: widget.model.formState.intervalType,
              onValueChanged: widget.model.updateIntervalType,
              children: const {
                DateIntervalType.daily: Text('Daily'),
                DateIntervalType.weekly: Text('Weekly'),
                DateIntervalType.monthly: Text('Monthly'),
                DateIntervalType.yearly: Text('Yearly'),
                DateIntervalType.oneTime: Text('One Time'),
              },
            ),
          )
          : Padding(
            padding: const EdgeInsets.symmetric(vertical: 8.0),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Padding(
                  padding: EdgeInsets.only(left: 4.0, bottom: 8.0),
                  child: Text('Interval Type'),
                ),
                SegmentedButton<DateIntervalType>(
                  segments: const [
                    ButtonSegment(
                      value: DateIntervalType.daily,
                      label: Text('Daily'),
                    ),
                    ButtonSegment(
                      value: DateIntervalType.weekly,
                      label: Text('Weekly'),
                    ),
                    ButtonSegment(
                      value: DateIntervalType.monthly,
                      label: Text('Monthly'),
                    ),
                    ButtonSegment(
                      value: DateIntervalType.yearly,
                      label: Text('Yearly'),
                    ),
                    ButtonSegment(
                      value: DateIntervalType.oneTime,
                      label: Text('One Time'),
                    ),
                  ],
                  selected: {widget.model.formState.intervalType},
                  onSelectionChanged: (Set<DateIntervalType> selected) {
                    if (selected.isNotEmpty) {
                      widget.model.updateIntervalType(selected.first);
                    }
                  },
                  multiSelectionEnabled: false,
                  showSelectedIcon: false,
                ),
              ],
            ),
          ),
      PlatformTextField(
        labelText: 'Frequency',
        initialValue: widget.model.formState.frequency.toString(),
        // keyboardType: TextInputType.number,
        onChanged:
            (value) => widget.model.updateFrequency(int.tryParse(value) ?? 1),
      ),
      PlatformTextField(
        labelText: 'Recurrence',
        initialValue: widget.model.formState.recurrence.toString(),
        // keyboardType: TextInputType.number,
        onChanged:
            (value) => widget.model.updateRecurrence(int.tryParse(value) ?? 0),
      ),
    ],
  ];
}
